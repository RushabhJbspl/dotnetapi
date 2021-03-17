using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Transaction
{
    public class SiteTokenConversionService: ISiteTokenConversion
    {
        string ControllerName = "SiteTokenConversionService";
        private readonly ICommonRepository<TradePairStastics> _tradePairStastics;
        private readonly ICommonRepository<SiteTokenMaster> _SiteTokenMaster;
        private readonly ITrnMasterConfiguration _trnMasterConfiguration;
        private readonly ICommonRepository<SiteTokenConversion> _SiteTokenConversion;
        private readonly IWalletSPRepositories _IWalletSPRepositories;

        private readonly ICommonRepository<TradePairStasticsMargin> _tradePairStasticsMargin;
        private readonly ICommonRepository<SiteTokenMasterMargin> _SiteTokenMasterMargin;
        private readonly ICommonRepository<SiteTokenConversionMargin> _SiteTokenConversionMargin;
        private readonly ISettlementRepositoryMarginV1<BizResponse> _SettlementRepo;
        private readonly ICommonRepository<MarginTradingAllowToUser> _MarginTradingAllowToUser;


        public SiteTokenConversionService(ICommonRepository<TradePairStastics> tradePairStastics, ICommonRepository<SiteTokenMaster> SiteTokenMaster,
            ITrnMasterConfiguration trnMasterConfiguration, ICommonRepository<SiteTokenConversion> SiteTokenConversion, 
            IWalletSPRepositories IWalletSPRepositories, ICommonRepository<SiteTokenMasterMargin> SiteTokenMasterMargin, ICommonRepository<SiteTokenConversionMargin> SiteTokenConversionMargin,
            ICommonRepository<TradePairStasticsMargin> tradePairStasticsMargin, ISettlementRepositoryMarginV1<BizResponse> SettlementRepo,
            ICommonRepository<MarginTradingAllowToUser> MarginTradingAllowToUser)
        {
            _tradePairStastics = tradePairStastics;
            _SiteTokenMaster = SiteTokenMaster;
            _trnMasterConfiguration = trnMasterConfiguration;
            _SiteTokenConversion = SiteTokenConversion;
            _IWalletSPRepositories = IWalletSPRepositories;
            //Rita 16-4-19 for margin Trading
            _SiteTokenMasterMargin = SiteTokenMasterMargin;
            _SiteTokenConversionMargin = SiteTokenConversionMargin;
            _tradePairStasticsMargin = tradePairStasticsMargin;
            _SettlementRepo = SettlementRepo;
            _MarginTradingAllowToUser = MarginTradingAllowToUser;
        }

        public async Task<SiteTokenConversionResponse> SiteTokenConversionAsync(SiteTokenConversionRequest Request,long UserID,string accesstoken)
        {
            SiteTokenConversionResponse Response = new SiteTokenConversionResponse();
            try
            {
                Response.response = new SiteTokenConversionInfo()
                {
                    TrnID = Guid.NewGuid()
                };
                if (Request.SourceCurrencyQty == null || Request.SourceCurrencyQty == 0)
                {
                    Response.ReturnMsg = "Invalid Input Qty";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidInputQty;
                    return Response;
                }

                SiteTokenMaster SiteTokenMasterObj = await _SiteTokenMaster.GetSingleAsync(e => e.Id == Request.SiteTokenMasterID);
                if(SiteTokenMasterObj==null)
                {
                    Response.ReturnMsg = "Invalid Site Token Selected";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.Invalid_Site_Token;
                    return Response;
                }
                if (Request.SourceCurrencyID == SiteTokenMasterObj.CurrencyID)//Can not convert to same currency
                {
                    Response.ReturnMsg = "Invalid Site Token Selected";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.Invalid_Site_Token;
                    return Response;
                }

                ServiceMaster SourceCurrencyObj = _trnMasterConfiguration.GetServices().Where(item => item.Id == Request.SourceCurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (SourceCurrencyObj == null)
                {
                    Response.ReturnMsg = "Invalid Source Currency";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidSourceCurrency;
                    return Response;
                }

                decimal TokenRate = 0;

                switch (SiteTokenMasterObj.RateType)
                {
                    case (short)enSiteTokenRateType.API:
                        TokenRate = 0;//skip as now not used
                        break;
                    case (short)enSiteTokenRateType.Market:
                        TradePairStastics pairStasticsSiteTokenObj = await _tradePairStastics.GetSingleAsync(pair => pair.PairId == SiteTokenMasterObj.PairID);
                        TokenRate = pairStasticsSiteTokenObj.LTP;                       
                        break;
                    case (short)enSiteTokenRateType.UserSpecific:
                        TokenRate = SiteTokenMasterObj.Rate;
                        break;
                    default:
                        TokenRate = 0;
                        break;
                }

                if (TokenRate == 0)
                {
                    Response.ReturnMsg = "Invalid Token Rate";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidTokenRate;
                    return Response;
                }

                decimal QtyToBaseCurrency = 0;
                decimal SourceToBasePrice = 0;
                if (Request.SourceCurrencyID== SiteTokenMasterObj.BaseCurrencyID)//if Selected Currency is Base currency then direct convert to Site Token
                {
                    SourceToBasePrice = TokenRate;
                    QtyToBaseCurrency = Request.SourceCurrencyQty;//Direct take it as Base Curr Qty
                }
                else
                {
                    //find site token's base currecny and user's selested wallet - PAIR and ptice , for conversion to base currenct
                    TradePairMaster _TradePairObj = _trnMasterConfiguration.GetTradePairMaster().Where(item => item.BaseCurrencyId == SiteTokenMasterObj.BaseCurrencyID &&
                                                    item.SecondaryCurrencyId == Request.SourceCurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();

                    if (_TradePairObj == null)
                    {
                        Response.ReturnMsg = "No Pair found for Given User's Currency with " + SiteTokenMasterObj.BaseCurrencySMSCode + " Market";
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.NoPairforUserCurrency;
                        return Response;
                    }

                    TradePairStastics pairStasticsUserCurrencyObj = await _tradePairStastics.GetSingleAsync(pair => pair.PairId == _TradePairObj.Id);
                    if (pairStasticsUserCurrencyObj == null)
                    {
                        Response.ReturnMsg = "No LTP found for Given User's Currency relate to " + SiteTokenMasterObj.BaseCurrencySMSCode + " Market";
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ErrorCode = enErrorCode.NoLTPfound;
                        return Response;
                    }
                    SourceToBasePrice = pairStasticsUserCurrencyObj.LTP;
                    QtyToBaseCurrency = Helpers.DoRoundForTrading(Request.SourceCurrencyQty * pairStasticsUserCurrencyObj.LTP, 18);
                }                
                
               

                decimal TargerCurrencyQty = Helpers.DoRoundForTrading(QtyToBaseCurrency / TokenRate, 18);
                //===========================Site Token Validation===================================
                //CHANGE Qty Parameter after confirmation , also change to TargerCurrencyQty for daily , monthly
                if (TargerCurrencyQty < SiteTokenMasterObj.MinLimit && SiteTokenMasterObj.MinLimit != 0)
                {
                    Response.ReturnMsg = "Minimum Qty of Currency " + SiteTokenMasterObj.CurrencySMSCode + " Should be: " + SiteTokenMasterObj.MinLimit;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.MinimumQtyRequired;
                    return Response;
                }
                if (TargerCurrencyQty > SiteTokenMasterObj.MaxLimit && SiteTokenMasterObj.MaxLimit != 0)
                {
                    Response.ReturnMsg = "Maximum Qty of Currency " + SiteTokenMasterObj.CurrencySMSCode + " Should be: " + SiteTokenMasterObj.MaxLimit;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.MaximumQtyRequired;
                    return Response;
                }

                IEnumerable<SiteTokenConversion> AllSiteTokenConversaionList = _SiteTokenConversion.GetAll().Where(e => e.Status == Convert.ToInt16(ServiceStatus.Active) && e.TargerCurrencyID == SiteTokenMasterObj.CurrencyID);//Rita 10-4-19 apply only on Target currency

                IEnumerable<SiteTokenConversion> SiteTokenConversaionListDaily = AllSiteTokenConversaionList.Where(a => a.CreatedDate.Day == Helpers.UTC_To_IST().Day && a.CreatedDate.Month == Helpers.UTC_To_IST().Month && a.CreatedDate.Year == Helpers.UTC_To_IST().Year);
                decimal DailyQty = Helpers.DoRoundForTrading(Enumerable.Sum(SiteTokenConversaionListDaily, a => a.TargerCurrencyQty), 18);//Change column here
                if ((DailyQty + TargerCurrencyQty) >= SiteTokenMasterObj.DailyLimit && SiteTokenMasterObj.DailyLimit != 0)
                {
                    Response.ReturnMsg = "Daily Conversion Qty " + SiteTokenMasterObj.DailyLimit + " of Currency " + SiteTokenMasterObj.CurrencySMSCode + " is Reached";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.DailyQtyReached;
                    return Response;
                }

                DateTime SevenDaysBeforeToday = Helpers.UTC_To_IST().AddDays(-7);
                IEnumerable<SiteTokenConversion> SiteTokenConversaionListWeekly = AllSiteTokenConversaionList.Where(a => a.CreatedDate >= SevenDaysBeforeToday);
                decimal WeeklyQty = Helpers.DoRoundForTrading(Enumerable.Sum(SiteTokenConversaionListWeekly, a => a.TargerCurrencyQty), 18);//Change column here
                if (WeeklyQty + TargerCurrencyQty >= SiteTokenMasterObj.WeeklyLimit && SiteTokenMasterObj.WeeklyLimit != 0)
                {
                    Response.ReturnMsg = "Weekly Conversion Qty " + SiteTokenMasterObj.WeeklyLimit + " of Currency " + SiteTokenMasterObj.CurrencySMSCode + " is Reached";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.WeeklyQtyReached;
                    return Response;
                }

                IEnumerable<SiteTokenConversion> SiteTokenConversaionListMonthly = AllSiteTokenConversaionList.Where(a => a.CreatedDate.Month == Helpers.UTC_To_IST().Month && a.CreatedDate.Year == Helpers.UTC_To_IST().Year);
                decimal MonthlyQty = Helpers.DoRoundForTrading(Enumerable.Sum(SiteTokenConversaionListMonthly, a => a.TargerCurrencyQty), 18);//Change column here
                if (MonthlyQty + TargerCurrencyQty >= SiteTokenMasterObj.MonthlyLimit && SiteTokenMasterObj.MonthlyLimit != 0)
                {
                    Response.ReturnMsg = "Monthly Conversion Qty " + SiteTokenMasterObj.MonthlyLimit + " of Currency " + SiteTokenMasterObj.CurrencySMSCode + " is Reached";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.MonthlyQtyReached;
                    return Response;
                }

                //===========================END===================================

                string TimeStamp = Helpers.GetTimeStamp();

                SiteTokenConversion SiteTokenConversion = await SiteTokenConversionEntry(Response.response.TrnID,UserID,Request.SourceCurrencyID, SourceCurrencyObj.SMSCode, 
                    SiteTokenMasterObj.CurrencyID, SiteTokenMasterObj.CurrencySMSCode,Request.SourceCurrencyQty, TargerCurrencyQty,
                    SourceToBasePrice, TokenRate, Request.SiteTokenMasterID, QtyToBaseCurrency, TimeStamp);

                //(string CrCurr,string DrCurr,decimal CrAmt,decimal DrAmt,long UserID,long TrnNo,int channelType , string timeStamp,
                //int serviceType,int walletTrnType, int IsUseDefaultWallet=1)
                BizResponseClass WalletReponse = _IWalletSPRepositories.CallSP_ConvertFundWalletOperation(SiteTokenMasterObj.CurrencySMSCode, SourceCurrencyObj.SMSCode,
                    TargerCurrencyQty,Request.SourceCurrencyQty,UserID, SiteTokenConversion.Id, Request.TrnMode, TimeStamp,
                    Convert.ToInt32(enServiceType.SiteTokenConversation),Convert.ToInt32(enWalletTrnType.ConvertFund),1, Response.response.TrnID.ToString());//2019-7-15 added guid

                if (WalletReponse.ReturnCode != enResponseCode.Success)
                {
                    Response.ReturnMsg = WalletReponse.ReturnMsg;
                    Response.ReturnCode = WalletReponse.ReturnCode;
                    Response.ErrorCode = WalletReponse.ErrorCode;

                    SiteTokenConversion.Status = 9;
                    SiteTokenConversion.StatusMsg = Response.ReturnMsg;
                    _SiteTokenConversion.Update(SiteTokenConversion);

                    return Response;
                }

                SiteTokenConversion.Status = 1;
                SiteTokenConversion.StatusMsg = "Success";
                _SiteTokenConversion.Update(SiteTokenConversion);

                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Success";
                Response.ErrorCode = enErrorCode.Success;

            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog("SiteTokenConversionAsync Internal Error:##UserID " + UserID, ControllerName, ex);                
                Response.ReturnMsg = EnResponseMessage.CommFailMsgInternal;
                Response.ReturnCode = enResponseCode.InternalError;
                Response.ErrorCode = enErrorCode.TransactionProcessInternalError;
            }
            return Response;
        }
        //Rita 16-4-19,Margin trading-used for convert Qty Base to Second currency
        public async Task<SiteTokenConversionResponse> SiteTokenConversionAsyncMargin(SiteTokenConversionRequest Request, long UserID, string accesstoken)
        {
            SiteTokenConversionResponse Response = new SiteTokenConversionResponse();
            try
            {
                Response.response = new SiteTokenConversionInfo()
                {
                    TrnID = Guid.NewGuid()
                };
                //Rita 16-4-19 Convert only From --BASE--
                MarketMargin MarketMarginSourceObj = _trnMasterConfiguration.GetMarketMargin().Where(item => item.ServiceID == Request.SourceCurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (MarketMarginSourceObj == null)
                {
                    Response.ReturnMsg = "Source Currency Shoul be from Base Market";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.Site_Token_SourceCurrencyShoulBeFromBaseMarket;
                    return Response;
                }                

                if (Request.SourceCurrencyQty == null || Request.SourceCurrencyQty == 0)
                {
                    Response.ReturnMsg = "Invalid Input Qty";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidInputQty;
                    return Response;
                }

                SiteTokenMasterMargin SiteTokenMasterObj = await _SiteTokenMasterMargin.GetSingleAsync(e => e.Id == Request.SiteTokenMasterID);
                if (SiteTokenMasterObj == null)
                {
                    Response.ReturnMsg = "Invalid Site Token Selected";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.Invalid_Site_Token;
                    return Response;
                }
                if (Request.SourceCurrencyID == SiteTokenMasterObj.CurrencyID)//Can not convert to same currency
                {
                    Response.ReturnMsg = "Invalid Site Token Selected";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.Invalid_Site_Token;
                    return Response;
                }
                //Rita 16-4-19 Convert to --SECOND-- Currency
                MarketMargin MarketMarginTargetObj = _trnMasterConfiguration.GetMarketMargin().Where(item => item.ServiceID == SiteTokenMasterObj.CurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (MarketMarginTargetObj != null)
                {
                    Response.ReturnMsg = "Target Currency Shoul Not be from Base Market";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.Site_Token_TargetCurrencyShoulNotBeFromBaseMarket;
                    return Response;
                }

                ServiceMasterMargin SourceCurrencyObj = _trnMasterConfiguration.GetServicesMargin().Where(item => item.Id == Request.SourceCurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (SourceCurrencyObj == null)
                {
                    Response.ReturnMsg = "Invalid Source Currency";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidSourceCurrency;
                    return Response;
                }
                //Rita 26-4-19 check whether margin trading allow or not
                MarginTradingAllowToUser MarginAllowRecord = _MarginTradingAllowToUser.GetSingle(e => e.UserId == UserID && e.Status == 1);
                if (MarginAllowRecord == null)//Not allowed
                {
                    Response.ReturnMsg = "Margin Trading not allowed";                  
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.Margin_TradingNotAllowed;
                    return Response;
                }

                decimal TokenRate = 0;

                switch (SiteTokenMasterObj.RateType)
                {
                    case (short)enSiteTokenRateType.API:
                        TokenRate = 0;//skip as now not used
                        break;
                    case (short)enSiteTokenRateType.Market:
                        TradePairStasticsMargin pairStasticsSiteTokenObj = await _tradePairStasticsMargin.GetSingleAsync(pair => pair.PairId == SiteTokenMasterObj.PairID);
                        TokenRate = pairStasticsSiteTokenObj.LTP;
                        break;
                    case (short)enSiteTokenRateType.UserSpecific:
                        TokenRate = SiteTokenMasterObj.Rate;
                        break;
                    default:
                        TokenRate = 0;
                        break;
                }

                if (TokenRate == 0)
                {
                    Response.ReturnMsg = "Invalid Token Rate";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidTokenRate;
                    return Response;
                }

                decimal QtyToBaseCurrency = 0;
                decimal SourceToBasePrice = 0;
                if (Request.SourceCurrencyID == SiteTokenMasterObj.BaseCurrencyID)//if Selected Currency is Base currency then direct convert to Site Token
                {
                    SourceToBasePrice = TokenRate;
                    QtyToBaseCurrency = Request.SourceCurrencyQty;//Direct take it as Base Curr Qty
                }
                else
                {
                    //Rita 17-4-19 selected currency always should be Base currency , so remove middle conversion , as PairID required in Opern Position
                    Response.ReturnMsg = "Invalid Source Currency";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InvalidSourceCurrency;
                    return Response;
                    //find site token's base currecny and user's selested wallet - PAIR and ptice , for conversion to base currenct
                    //TradePairMasterMargin _TradePairObj = _trnMasterConfiguration.GetTradePairMasterMargin().Where(item => item.BaseCurrencyId == SiteTokenMasterObj.BaseCurrencyID &&
                    //                                item.SecondaryCurrencyId == Request.SourceCurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();

                    //if (_TradePairObj == null)
                    //{
                    //    Response.ReturnMsg = "No Pair found for Given User's Currency with " + SiteTokenMasterObj.BaseCurrencySMSCode + " Market";
                    //    Response.ReturnCode = enResponseCode.Fail;
                    //    Response.ErrorCode = enErrorCode.NoPairforUserCurrency;
                    //    return Response;
                    //}

                    //TradePairStasticsMargin pairStasticsUserCurrencyObj = await _tradePairStasticsMargin.GetSingleAsync(pair => pair.PairId == _TradePairObj.Id);
                    //if (pairStasticsUserCurrencyObj == null)
                    //{
                    //    Response.ReturnMsg = "No LTP found for Given User's Currency relate to " + SiteTokenMasterObj.BaseCurrencySMSCode + " Market";
                    //    Response.ReturnCode = enResponseCode.Fail;
                    //    Response.ErrorCode = enErrorCode.NoLTPfound;
                    //    return Response;
                    //}
                    //SourceToBasePrice = pairStasticsUserCurrencyObj.LTP;
                    //QtyToBaseCurrency = Helpers.DoRoundForTrading(Request.SourceCurrencyQty * pairStasticsUserCurrencyObj.LTP, 18);
                }



                decimal TargerCurrencyQty = Helpers.DoRoundForTrading(QtyToBaseCurrency / TokenRate, 18);
                //===========================Site Token Validation===================================
                //CHANGE Qty Parameter after confirmation , also change to TargerCurrencyQty for daily , monthly
                if (TargerCurrencyQty < SiteTokenMasterObj.MinLimit && SiteTokenMasterObj.MinLimit != 0)
                {
                    Response.ReturnMsg = "Minimum Qty of Currency " + SiteTokenMasterObj.CurrencySMSCode + " Should be: " + SiteTokenMasterObj.MinLimit;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.MinimumQtyRequired;
                    return Response;
                }
                if (TargerCurrencyQty > SiteTokenMasterObj.MaxLimit && SiteTokenMasterObj.MaxLimit != 0)
                {
                    Response.ReturnMsg = "Maximum Qty of Currency " + SiteTokenMasterObj.CurrencySMSCode + " Should be: " + SiteTokenMasterObj.MaxLimit;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.MaximumQtyRequired;
                    return Response;
                }

                IEnumerable<SiteTokenConversionMargin> AllSiteTokenConversaionList = _SiteTokenConversionMargin.GetAll().Where(e => e.Status == Convert.ToInt16(ServiceStatus.Active) && e.TargerCurrencyID == SiteTokenMasterObj.CurrencyID);//Rita 10-4-19 apply only on Target currency

                IEnumerable<SiteTokenConversionMargin> SiteTokenConversaionListDaily = AllSiteTokenConversaionList.Where(a => a.CreatedDate.Day == Helpers.UTC_To_IST().Day && a.CreatedDate.Month == Helpers.UTC_To_IST().Month && a.CreatedDate.Year == Helpers.UTC_To_IST().Year);
                decimal DailyQty = Helpers.DoRoundForTrading(Enumerable.Sum(SiteTokenConversaionListDaily, a => a.TargerCurrencyQty), 18);//Change column here
                if ((DailyQty + TargerCurrencyQty) >= SiteTokenMasterObj.DailyLimit && SiteTokenMasterObj.DailyLimit != 0)
                {
                    Response.ReturnMsg = "Daily Conversion Qty " + SiteTokenMasterObj.DailyLimit + " of Currency " + SiteTokenMasterObj.CurrencySMSCode + " is Reached";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.DailyQtyReached;
                    return Response;
                }

                DateTime SevenDaysBeforeToday = Helpers.UTC_To_IST().AddDays(-7);
                IEnumerable<SiteTokenConversionMargin> SiteTokenConversaionListWeekly = AllSiteTokenConversaionList.Where(a => a.CreatedDate >= SevenDaysBeforeToday);
                decimal WeeklyQty = Helpers.DoRoundForTrading(Enumerable.Sum(SiteTokenConversaionListWeekly, a => a.TargerCurrencyQty), 18);//Change column here
                if (WeeklyQty + TargerCurrencyQty >= SiteTokenMasterObj.WeeklyLimit && SiteTokenMasterObj.WeeklyLimit != 0)
                {
                    Response.ReturnMsg = "Weekly Conversion Qty " + SiteTokenMasterObj.WeeklyLimit + " of Currency " + SiteTokenMasterObj.CurrencySMSCode + " is Reached";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.WeeklyQtyReached;
                    return Response;
                }

                IEnumerable<SiteTokenConversionMargin> SiteTokenConversaionListMonthly = AllSiteTokenConversaionList.Where(a => a.CreatedDate.Month == Helpers.UTC_To_IST().Month && a.CreatedDate.Year == Helpers.UTC_To_IST().Year);
                decimal MonthlyQty = Helpers.DoRoundForTrading(Enumerable.Sum(SiteTokenConversaionListMonthly, a => a.TargerCurrencyQty), 18);//Change column here
                if (MonthlyQty + TargerCurrencyQty >= SiteTokenMasterObj.MonthlyLimit && SiteTokenMasterObj.MonthlyLimit != 0)
                {
                    Response.ReturnMsg = "Monthly Conversion Qty " + SiteTokenMasterObj.MonthlyLimit + " of Currency " + SiteTokenMasterObj.CurrencySMSCode + " is Reached";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.MonthlyQtyReached;
                    return Response;
                }

                //===========================END===================================

                string TimeStamp = Helpers.GetTimeStamp();

                SiteTokenConversionMargin SiteTokenConversion = await SiteTokenConversionEntryMargin(Response.response.TrnID, UserID, Request.SourceCurrencyID, SourceCurrencyObj.SMSCode,
                    SiteTokenMasterObj.CurrencyID, SiteTokenMasterObj.CurrencySMSCode, Request.SourceCurrencyQty, TargerCurrencyQty,SourceToBasePrice, TokenRate, Request.SiteTokenMasterID, QtyToBaseCurrency, TimeStamp);

                string CreditAccoutID = "";
                string DebitAccoutID = "";

                BizResponseClass WalletReponse = _IWalletSPRepositories.CallSP_MarginConvertFundWalletOperation(SiteTokenMasterObj.CurrencySMSCode, SourceCurrencyObj.SMSCode,
                    TargerCurrencyQty, Request.SourceCurrencyQty, UserID, SiteTokenConversion.Id, Request.TrnMode, TimeStamp,Convert.ToInt32(enServiceType.SiteTokenConversation), Convert.ToInt32(enWalletTrnType.ConvertFund), SiteTokenMasterObj.PairID, TokenRate,ref CreditAccoutID,ref DebitAccoutID,1,Response.response.TrnID.ToString());//2019-7-15 added guid

                if (WalletReponse.ReturnCode != enResponseCode.Success)
                {
                    Response.ReturnMsg = WalletReponse.ReturnMsg;
                    Response.ReturnCode = WalletReponse.ReturnCode;
                    Response.ErrorCode = WalletReponse.ErrorCode;

                    SiteTokenConversion.Status = 9;
                    SiteTokenConversion.StatusMsg = Response.ReturnMsg;
                    _SiteTokenConversionMargin.Update(SiteTokenConversion);

                    return Response;
                }

                //Rita 24-04-19 make stop & limit order for member
                try
                {
                    string PairName = _trnMasterConfiguration.GetTradePairMasterMargin().Where(e => e.Id == SiteTokenMasterObj.PairID).FirstOrDefault().PairName;
                   var ret = await _SettlementRepo.SystemCreatedOrderAlsoFromSiteToken(SiteTokenConversion.Id, SiteTokenConversion.Id, SiteTokenMasterObj.PairID, PairName, UserID, SiteTokenMasterObj.BaseCurrencySMSCode, CreditAccoutID,DebitAccoutID, TokenRate, "", "");//reverse Account ID for Sell ATCC to buy BTC
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SiteTokenConversionAsyncMargin", ControllerName, " SystemCreated Result:" + ret + "##ConversionID:" + SiteTokenConversion.Id, Helpers.UTC_To_IST()));
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog("SiteTokenConversionAsyncMargin SystemCreatedOrder Internal Error:##UserID " + UserID + " ConversionID:" + SiteTokenConversion.Id, ControllerName, ex);
                }
               
                SiteTokenConversion.Status = 1;
                SiteTokenConversion.StatusMsg = "Success";
                _SiteTokenConversionMargin.Update(SiteTokenConversion);

                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Success";
                Response.ErrorCode = enErrorCode.Success;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("SiteTokenConversionAsyncMargin Internal Error:##UserID " + UserID, ControllerName, ex);
                Response.ReturnMsg = EnResponseMessage.CommFailMsgInternal;
                Response.ReturnCode = enResponseCode.InternalError;
                Response.ErrorCode = enErrorCode.TransactionProcessInternalError;
            }
            return Response;
        }

        public async Task<SiteTokenConversion> SiteTokenConversionEntry(Guid GUID, long UserID, long SourceCurrencyID,string SourceCurrency, long TargerCurrencyID,string TargerCurrency,decimal SourceCurrencyQty,decimal TargerCurrencyQty,decimal SourceToBasePrice,decimal TokenPrice,long SiteTokenMasterID,decimal SourceToBaseQty,string TimeStamp)
        {
            try
            {
                SiteTokenConversion SiteTokenConversion = new SiteTokenConversion()
                {
                    CreatedDate = Helpers.UTC_To_IST(),//rita 29-1-19 added for log
                    GUID = GUID,
                    UserID = UserID,
                    SourceCurrencyID = SourceCurrencyID,
                    SourceCurrency = SourceCurrency,
                    TargerCurrencyID = TargerCurrencyID,
                    TargerCurrency = TargerCurrency,
                    SourceCurrencyQty = SourceCurrencyQty,
                    TargerCurrencyQty = TargerCurrencyQty,
                    SourceToBasePrice = SourceToBasePrice,
                    TokenPrice = TokenPrice,
                    SiteTokenMasterID = SiteTokenMasterID,
                    SourceToBaseQty = SourceToBaseQty,
                    Status=0,//for imitialization
                    TimeStamp = TimeStamp,
                };
                SiteTokenConversion = _SiteTokenConversion.Add(SiteTokenConversion);
                return SiteTokenConversion;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("SiteTokenConversionEntry:##UserID " + UserID + " SourceCurrencyID:"+ SourceCurrencyID + " To TargerCurrencyID:"+ TargerCurrencyID, ControllerName, ex);
                throw ex;
            }
        }
        //Rita 16-4-19,Margin trading-used for convert Qty Base to Second currency
        public async Task<SiteTokenConversionMargin> SiteTokenConversionEntryMargin(Guid GUID, long UserID, long SourceCurrencyID, string SourceCurrency, long TargerCurrencyID, string TargerCurrency, decimal SourceCurrencyQty, decimal TargerCurrencyQty, decimal SourceToBasePrice, decimal TokenPrice, long SiteTokenMasterID, decimal SourceToBaseQty, string TimeStamp)
        {
            try
            {
                SiteTokenConversionMargin SiteTokenConversionMargin = new SiteTokenConversionMargin()
                {
                    CreatedDate = Helpers.UTC_To_IST(),//rita 29-1-19 added for log
                    GUID = GUID,
                    UserID = UserID,
                    SourceCurrencyID = SourceCurrencyID,
                    SourceCurrency = SourceCurrency,
                    TargerCurrencyID = TargerCurrencyID,
                    TargerCurrency = TargerCurrency,
                    SourceCurrencyQty = SourceCurrencyQty,
                    TargerCurrencyQty = TargerCurrencyQty,
                    SourceToBasePrice = SourceToBasePrice,
                    TokenPrice = TokenPrice,
                    SiteTokenMasterID = SiteTokenMasterID,
                    SourceToBaseQty = SourceToBaseQty,
                    Status = 0,//for imitialization
                    TimeStamp = TimeStamp,
                };
                SiteTokenConversionMargin = _SiteTokenConversionMargin.Add(SiteTokenConversionMargin);
                return SiteTokenConversionMargin;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("SiteTokenConversionEntryMargin:##UserID " + UserID + " SourceCurrencyID:" + SourceCurrencyID + " To TargerCurrencyID:" + TargerCurrencyID, ControllerName, ex);
                throw ex;
            }
        }

        public async Task<SiteTokenCalculationResponse> SiteTokenCalculation(SiteTokenCalculationRequest Request, long UserID, string accesstoken)
        {
            SiteTokenCalculationResponse Response = new SiteTokenCalculationResponse();
            try
            {
                Response.response = new SiteTokenCalculationInfo();
                //{
                //    TrnID = Guid.NewGuid()
                //};
                SiteTokenMaster SiteTokenMasterObj = await _SiteTokenMaster.GetSingleAsync(e => e.Id == Request.SiteTokenMasterID);
                if (SiteTokenMasterObj == null)
                {
                    Response.ReturnMsg = "Invalid Site Token Selected";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.TransactionProcessInternalError;
                    return Response;
                }
                ServiceMaster SourceCurrencyObj = _trnMasterConfiguration.GetServices().Where(item => item.Id == Request.SourceCurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (SourceCurrencyObj == null)
                {
                    Response.ReturnMsg = "Invalid Source Currency";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.TransactionProcessInternalError;
                    return Response;
                }
                decimal TokenRate = 0;

                switch (SiteTokenMasterObj.RateType)
                {
                    case (short)enSiteTokenRateType.API:
                        TokenRate = 0;//skip as now not used
                        break;
                    case (short)enSiteTokenRateType.Market:
                        TradePairStastics pairStasticsSiteTokenObj = await _tradePairStastics.GetSingleAsync(pair => pair.PairId == SiteTokenMasterObj.PairID);
                        TokenRate = pairStasticsSiteTokenObj.LTP;
                        break;
                    case (short)enSiteTokenRateType.UserSpecific:
                        TokenRate = SiteTokenMasterObj.Rate;
                        break;
                    default:
                        TokenRate = 0;
                        break;
                }

                if (TokenRate == 0)
                {
                    Response.ReturnMsg = "Invalid Token Rate";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.TransactionProcessInternalError;
                    return Response;
                }

                if (Request.ISSiteTokenToCurrency == 0)//Regular
                {
                    decimal QtyToBaseCurrency = 0;
                    if (Request.SourceCurrencyID == SiteTokenMasterObj.BaseCurrencyID)//if Selected Currency is Base currency then direct convert to Site Token
                    {
                        QtyToBaseCurrency = Request.Qty;//Direct take it as Base Curr Qty
                    }
                    else
                    {
                        //find site token's base currecny and user's selested wallet - PAIR and ptice , for conversion to base currenct
                        TradePairMaster _TradePairObj = _trnMasterConfiguration.GetTradePairMaster().Where(item => item.BaseCurrencyId == SiteTokenMasterObj.BaseCurrencyID &&
                                                        item.SecondaryCurrencyId == Request.SourceCurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();

                        if (_TradePairObj == null)
                        {
                            Response.ReturnMsg = "No Pair found for Given User's Currency with " + SiteTokenMasterObj.BaseCurrencySMSCode + " Market";
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.NoPairforUserCurrency;
                            return Response;
                        }

                        TradePairStastics pairStasticsUserCurrencyObj = await _tradePairStastics.GetSingleAsync(pair => pair.PairId == _TradePairObj.Id);
                        if (pairStasticsUserCurrencyObj == null)
                        {
                            Response.ReturnMsg = "No LTP found for Given User's Currency relate to " + SiteTokenMasterObj.BaseCurrencySMSCode + " Market";
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.NoLTPfound;
                            return Response;
                        }
                        QtyToBaseCurrency = Helpers.DoRoundForTrading(Request.Qty * pairStasticsUserCurrencyObj.LTP, 18);
                    }

                    decimal TargerCurrencyQty = Helpers.DoRoundForTrading(QtyToBaseCurrency / TokenRate, 18);
                    Response.response.ResultQty = TargerCurrencyQty;
                    Response.response.ResultCurrecy = SiteTokenMasterObj.CurrencySMSCode;
                }
                else if(Request.ISSiteTokenToCurrency == 1)//Reverse Calculation
                {
                    decimal QtyToBaseCurrency = 0;                   
                    decimal TargerCurrencyQty = 0;                   
                    //Site  token to Base Currency, ATCC_BTC - LTP = 0.5
                    //ATCC      BTC
                    //1         0.5
                    //0.4       ==> (0.4*0.5=0.2) BTC 
                    QtyToBaseCurrency = Helpers.DoRoundForTrading(Request.Qty * TokenRate, 18);//Direct take it as Base Curr Qty

                    if (Request.SourceCurrencyID == SiteTokenMasterObj.BaseCurrencyID)//if Selected Currency is Base currency then direct convert to Base currency ,as Token price entry
                    {                       
                        TargerCurrencyQty = QtyToBaseCurrency;
                    }
                    else
                    {
                        //find site token's base currecny and user's selested wallet - PAIR and ptice , for conversion to base currenct
                        TradePairMaster _TradePairObj = _trnMasterConfiguration.GetTradePairMaster().Where(item => item.BaseCurrencyId == SiteTokenMasterObj.BaseCurrencyID &&
                                                        item.SecondaryCurrencyId == Request.SourceCurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();

                        if (_TradePairObj == null)
                        {
                            Response.ReturnMsg = "No Pair found for Given User's Currency with " + SiteTokenMasterObj.BaseCurrencySMSCode + " Market";
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.NoPairforUserCurrency;
                            return Response;
                        }

                        TradePairStastics pairStasticsUserCurrencyObj = await _tradePairStastics.GetSingleAsync(pair => pair.PairId == _TradePairObj.Id);
                        if (pairStasticsUserCurrencyObj == null)
                        {
                            Response.ReturnMsg = "No LTP found for Given User's Currency relate to " + SiteTokenMasterObj.BaseCurrencySMSCode + " Market";
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.NoLTPfound;
                            return Response;
                        }
                        //Site-Base Currency to Currency, ETH_BTC - LTP = 0.1
                        //BTC      ETH
                        //0.1       1
                        //0.2       ==> (0.2/0.1=2) ETH 
                        TargerCurrencyQty = Helpers.DoRoundForTrading(QtyToBaseCurrency / pairStasticsUserCurrencyObj.LTP, 18);
                    }
                    
                    Response.response.ResultQty = TargerCurrencyQty;
                    Response.response.ResultCurrecy = SourceCurrencyObj.SMSCode;
                }

                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Success";
                Response.ErrorCode = enErrorCode.Success;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("SiteTokenCalculation_Internal_Error:##UserID " + UserID, ControllerName, ex);
                Response.ReturnMsg = EnResponseMessage.CommFailMsgInternal;
                Response.ReturnCode = enResponseCode.InternalError;
                Response.ErrorCode = enErrorCode.TransactionProcessInternalError;
            }
            return Response;
        }
        //Rita 16-4-19,Margin trading-used for convert Qty Base to Second currency
        public async Task<SiteTokenCalculationResponse> SiteTokenCalculationMargin(SiteTokenCalculationRequest Request, long UserID, string accesstoken)
        {
            SiteTokenCalculationResponse Response = new SiteTokenCalculationResponse();
            try
            {
                Response.response = new SiteTokenCalculationInfo();
                //{
                //    TrnID = Guid.NewGuid()
                //};
                SiteTokenMasterMargin SiteTokenMasterObj = await _SiteTokenMasterMargin.GetSingleAsync(e => e.Id == Request.SiteTokenMasterID);
                if (SiteTokenMasterObj == null)
                {
                    Response.ReturnMsg = "Invalid Site Token Selected";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.TransactionProcessInternalError;
                    return Response;
                }
                ServiceMasterMargin SourceCurrencyObj = _trnMasterConfiguration.GetServicesMargin().Where(item => item.Id == Request.SourceCurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (SourceCurrencyObj == null)
                {
                    Response.ReturnMsg = "Invalid Source Currency";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.TransactionProcessInternalError;
                    return Response;
                }
                decimal TokenRate = 0;

                switch (SiteTokenMasterObj.RateType)
                {
                    case (short)enSiteTokenRateType.API:
                        TokenRate = 0;//skip as now not used
                        break;
                    case (short)enSiteTokenRateType.Market:
                        TradePairStasticsMargin pairStasticsSiteTokenObj = await _tradePairStasticsMargin.GetSingleAsync(pair => pair.PairId == SiteTokenMasterObj.PairID);
                        TokenRate = pairStasticsSiteTokenObj.LTP;
                        break;
                    case (short)enSiteTokenRateType.UserSpecific:
                        TokenRate = SiteTokenMasterObj.Rate;
                        break;
                    default:
                        TokenRate = 0;
                        break;
                }

                if (TokenRate == 0)
                {
                    Response.ReturnMsg = "Invalid Token Rate";
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.TransactionProcessInternalError;
                    return Response;
                }

                if (Request.ISSiteTokenToCurrency == 0)//Regular
                {
                    decimal QtyToBaseCurrency = 0;
                    if (Request.SourceCurrencyID == SiteTokenMasterObj.BaseCurrencyID)//if Selected Currency is Base currency then direct convert to Site Token
                    {
                        QtyToBaseCurrency = Request.Qty;//Direct take it as Base Curr Qty
                    }
                    else
                    {
                        //find site token's base currecny and user's selested wallet - PAIR and ptice , for conversion to base currenct
                        TradePairMasterMargin _TradePairObj = _trnMasterConfiguration.GetTradePairMasterMargin().Where(item => item.BaseCurrencyId == SiteTokenMasterObj.BaseCurrencyID &&
                                                        item.SecondaryCurrencyId == Request.SourceCurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();

                        if (_TradePairObj == null)
                        {
                            Response.ReturnMsg = "No Pair found for Given User's Currency with " + SiteTokenMasterObj.BaseCurrencySMSCode + " Market";
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.NoPairforUserCurrency;
                            return Response;
                        }

                        TradePairStasticsMargin pairStasticsUserCurrencyObj = await _tradePairStasticsMargin.GetSingleAsync(pair => pair.PairId == _TradePairObj.Id);
                        if (pairStasticsUserCurrencyObj == null)
                        {
                            Response.ReturnMsg = "No LTP found for Given User's Currency relate to " + SiteTokenMasterObj.BaseCurrencySMSCode + " Market";
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.NoLTPfound;
                            return Response;
                        }
                        QtyToBaseCurrency = Helpers.DoRoundForTrading(Request.Qty * pairStasticsUserCurrencyObj.LTP, 18);
                    }

                    decimal TargerCurrencyQty = Helpers.DoRoundForTrading(QtyToBaseCurrency / TokenRate, 18);
                    Response.response.ResultQty = TargerCurrencyQty;
                    Response.response.ResultCurrecy = SiteTokenMasterObj.CurrencySMSCode;
                }
                else if (Request.ISSiteTokenToCurrency == 1)//Reverse Calculation
                {
                    decimal QtyToBaseCurrency = 0;
                    decimal TargerCurrencyQty = 0;
                    //Site  token to Base Currency, ATCC_BTC - LTP = 0.5
                    //ATCC      BTC
                    //1         0.5
                    //0.4       ==> (0.4*0.5=0.2) BTC 
                    QtyToBaseCurrency = Helpers.DoRoundForTrading(Request.Qty * TokenRate, 18);//Direct take it as Base Curr Qty

                    if (Request.SourceCurrencyID == SiteTokenMasterObj.BaseCurrencyID)//if Selected Currency is Base currency then direct convert to Base currency ,as Token price entry
                    {
                        TargerCurrencyQty = QtyToBaseCurrency;
                    }
                    else
                    {
                        //find site token's base currecny and user's selested wallet - PAIR and ptice , for conversion to base currenct
                        TradePairMasterMargin _TradePairObj = _trnMasterConfiguration.GetTradePairMasterMargin().Where(item => item.BaseCurrencyId == SiteTokenMasterObj.BaseCurrencyID &&
                                                        item.SecondaryCurrencyId == Request.SourceCurrencyID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();

                        if (_TradePairObj == null)
                        {
                            Response.ReturnMsg = "No Pair found for Given User's Currency with " + SiteTokenMasterObj.BaseCurrencySMSCode + " Market";
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.NoPairforUserCurrency;
                            return Response;
                        }

                        TradePairStasticsMargin pairStasticsUserCurrencyObj = await _tradePairStasticsMargin.GetSingleAsync(pair => pair.PairId == _TradePairObj.Id);
                        if (pairStasticsUserCurrencyObj == null)
                        {
                            Response.ReturnMsg = "No LTP found for Given User's Currency relate to " + SiteTokenMasterObj.BaseCurrencySMSCode + " Market";
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ErrorCode = enErrorCode.NoLTPfound;
                            return Response;
                        }
                        //Site-Base Currency to Currency, ETH_BTC - LTP = 0.1
                        //BTC      ETH
                        //0.1       1
                        //0.2       ==> (0.2/0.1=2) ETH 
                        TargerCurrencyQty = Helpers.DoRoundForTrading(QtyToBaseCurrency / pairStasticsUserCurrencyObj.LTP, 18);
                    }

                    Response.response.ResultQty = TargerCurrencyQty;
                    Response.response.ResultCurrecy = SourceCurrencyObj.SMSCode;
                }

                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Success";
                Response.ErrorCode = enErrorCode.Success;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("SiteTokenCalculationMargin Internal Error:##UserID " + UserID, ControllerName, ex);
                Response.ReturnMsg = EnResponseMessage.CommFailMsgInternal;
                Response.ReturnCode = enResponseCode.InternalError;
                Response.ErrorCode = enErrorCode.TransactionProcessInternalError;
            }
            return Response;
        }
    }
}
