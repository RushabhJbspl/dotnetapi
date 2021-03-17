using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Interfaces.User;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.Arbitrage;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using Worldex.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Worldex.Core.ViewModels.Wallet;

namespace Worldex.Infrastructure.Services.Transaction
{
    public class FrontTrnService : IFrontTrnService
    {
        #region constructor
        private readonly IFrontTrnRepository _frontTrnRepository;
        private readonly ICommonRepository<TradePairMaster> _tradeMasterRepository;
        private readonly ICommonRepository<TradePairDetail> _tradeDetailRepository;
        private readonly ICommonRepository<ServiceMaster> _serviceMasterRepository;
        private readonly ILogger<FrontTrnService> _logger;
        private readonly ICommonRepository<TradeTransactionQueue> _tradeTransactionQueueRepository;
        private readonly ICommonRepository<SettledTradeTransactionQueue> _settelTradeTranQueue;
        private readonly ICommonRepository<TradePairStastics> _tradePairStastics;
        private readonly ICommonRepository<Market> _marketRepository;
        private readonly ICommonRepository<MarketMargin> _marketRepositoryMargin;
        private readonly ICommonRepository<FavouritePair> _favouritePairRepository;
        private readonly IBasePage _basePage;
        private readonly ICommonRepository<TradeGraphDetail> _graphDetailRepository;
        private readonly ISignalRService _signalRService;
        private readonly ICommonRepository<SettledTradeTransactionQueue> _settleTradeTransactionQueue;
        private readonly IBackOfficeTrnRepository _backOfficeTrnRepository;
        private readonly IWalletService _walletService;
        private readonly IWalletRepository _walletRepository;
        private readonly ICommonRepository<TransactionQueue> _transactionQueue;
        private readonly IUserService _userService;
        private readonly IProfileConfigurationService _profileConfigurationService;
        //Rita 20-2-19 for Margin Trading
        private readonly ICommonRepository<TradeGraphDetailMargin> _graphDetailRepositoryMargin;
        private readonly ICommonRepository<SettledTradeTransactionQueueMargin> _settleTradeTransactionQueueMargin;
        private readonly ICommonRepository<TradePairStasticsMargin> _tradePairStasticsMargin;
        private readonly ICommonRepository<TradePairDetailMargin> _tradeDetailRepositoryMargin;
        private readonly ITrnMasterConfiguration _trnMasterConfiguration;//master configuration Take from cache
        private readonly ICommonRepository<FavouritePairMargin> _favouritePairRepositoryMargin;
        private readonly ICommonRepository<TradingConfiguration> _tradingConfigurationRepository;
        private readonly ICommonRepository<TradePoolQueueV1> _TradePoolQueueV1Repository;
        private readonly ICommonRepository<TradePoolQueueMarginV1> _TradePoolQueueMarginV1Repository;
        //komal 6-6-2019 Arbitrage Trading
        private readonly ICommonRepository<SettledTradeTransactionQueueArbitrage> _settleTradeTransactionQueueArbitrage;
        private readonly ICommonRepository<TradeGraphDetailArbitrage> _graphDetailRepositoryArbitrage;
        private readonly ICommonRepository<TradePairStasticsArbitrage> _tradePairStasticsArbitrage;
        private readonly ICommonRepository<TradePairDetailArbitrage> _tradeDetailRepositoryArbitrage;
        private readonly ICommonRepository<ArbitrageTradingAllowToUser> _arbitrageTradingAllowToUserRepository;

        string ControllerName = "FrontTrnService";

        public FrontTrnService(IFrontTrnRepository frontTrnRepository,
            ICommonRepository<TradePairMaster> tradeMasterRepository,
            ICommonRepository<TradePairDetail> tradeDetailRepository,
            ILogger<FrontTrnService> logger, IWalletRepository walletRepository,
            ICommonRepository<ServiceMaster> serviceMasterRepository,
            ICommonRepository<TradeTransactionQueue> tradeTransactionQueueRepository,
            ICommonRepository<SettledTradeTransactionQueue> settelTradeTranQueue,
            ICommonRepository<TradePairStastics> tradePairStastics,
            ICommonRepository<Market> marketRepository,
            ICommonRepository<FavouritePair> favouritePairRepository,
            IBasePage basePage,
            ICommonRepository<TradeGraphDetail> graphDetailRepository,
            ISignalRService signalRService,
            ICommonRepository<SettledTradeTransactionQueue> settleTradeTransactionQueue,
            IBackOfficeTrnRepository backOfficeTrnRepository,
            IWalletService walletService, ICommonRepository<TransactionQueue> transactionQueue,
            IUserService userService, IProfileConfigurationService profileConfigurationService,
            ICommonRepository<SettledTradeTransactionQueueMargin> settleTradeTransactionQueueMargin,
            ICommonRepository<TradeGraphDetailMargin> graphDetailRepositoryMargin,
            ICommonRepository<TradePairStasticsMargin> tradePairStasticsMargin,
            ICommonRepository<TradePairDetailMargin> tradeDetailRepositoryMargin, ITrnMasterConfiguration trnMasterConfiguration,
            ICommonRepository<MarketMargin> marketRepositoryMargin, ICommonRepository<FavouritePairMargin> favouritePairRepositoryMargin,
            ICommonRepository<TradingConfiguration> TradingConfigurationRepository,
            ICommonRepository<SettledTradeTransactionQueueArbitrage> settleTradeTransactionQueueArbitrage,
            ICommonRepository<TradeGraphDetailArbitrage> graphDetailRepositoryArbitrage,
            ICommonRepository<TradePairStasticsArbitrage> tradePairStasticsArbitrage,
            ICommonRepository<TradePairDetailArbitrage> tradeDetailRepositoryArbitrage,
            ICommonRepository<ArbitrageTradingAllowToUser> arbitrageTradingAllowToUserRepository, ICommonRepository<TradePoolQueueV1> TradePoolQueueV1Repository,
            ICommonRepository<TradePoolQueueMarginV1> TradePoolQueueMarginV1Repository)

        {
            _frontTrnRepository = frontTrnRepository;
            _tradeMasterRepository = tradeMasterRepository;
            _tradeDetailRepository = tradeDetailRepository;
            _logger = logger;
            _walletRepository = walletRepository;
            _serviceMasterRepository = serviceMasterRepository;
            _tradeTransactionQueueRepository = tradeTransactionQueueRepository;
            _settelTradeTranQueue = settelTradeTranQueue;
            _tradePairStastics = tradePairStastics;
            _marketRepository = marketRepository;
            _favouritePairRepository = favouritePairRepository;
            _basePage = basePage;
            _graphDetailRepository = graphDetailRepository;
            _signalRService = signalRService;
            _settleTradeTransactionQueue = settleTradeTransactionQueue;
            _backOfficeTrnRepository = backOfficeTrnRepository;
            _walletService = walletService;
            _transactionQueue = transactionQueue;
            _userService = userService;
            _profileConfigurationService = profileConfigurationService;
            //Rita 20-2-19 for Margin Trading
            _settleTradeTransactionQueueMargin = settleTradeTransactionQueueMargin;
            _graphDetailRepositoryMargin = graphDetailRepositoryMargin;
            _tradePairStasticsMargin = tradePairStasticsMargin;
            _tradeDetailRepositoryMargin = tradeDetailRepositoryMargin;
            _trnMasterConfiguration = trnMasterConfiguration;
            _marketRepositoryMargin = marketRepositoryMargin;
            _favouritePairRepositoryMargin = favouritePairRepositoryMargin;
            _tradingConfigurationRepository = TradingConfigurationRepository;
            _settleTradeTransactionQueueArbitrage = settleTradeTransactionQueueArbitrage;
            _graphDetailRepositoryArbitrage = graphDetailRepositoryArbitrage;
            _tradePairStasticsArbitrage = tradePairStasticsArbitrage;
            _tradeDetailRepositoryArbitrage = tradeDetailRepositoryArbitrage;
            _arbitrageTradingAllowToUserRepository = arbitrageTradingAllowToUserRepository;
            _TradePoolQueueV1Repository = TradePoolQueueV1Repository;
            _TradePoolQueueMarginV1Repository = TradePoolQueueMarginV1Repository;
        }
        #endregion

        #region History Methods

        public CopiedLeaderOrdersResponse GetCopiedLeaderOrders(long MemberID, string FromDate = "", string TodDate = "", long PairId = 999, short trnType = 999, string FollowTradeType = "", long FollowingTo = 0, int PageSize = 0, int PageNo = 0)
        {
            List<CopiedLeaderOrdersInfo> leaderOrdersInfos = new List<CopiedLeaderOrdersInfo>();
            CopiedLeaderOrdersResponse _Res = new CopiedLeaderOrdersResponse();
            int skip;
            int size;
            try
            {
                if (PageSize == 0)
                    size = Helpers.PageSize;
                else
                    size = PageSize;

                skip = size * (PageNo);
                var list = _frontTrnRepository.GetCopiedLeaderOrders(MemberID, FromDate, TodDate, PairId, trnType, FollowTradeType, FollowingTo);
                if (list.Count == 0)
                {
                    _Res.Response = leaderOrdersInfos;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                _Res.TotalCount = list.Count();
                foreach (var model in list.Skip(skip).Take(size))
                {
                    leaderOrdersInfos.Add(new CopiedLeaderOrdersInfo
                    {
                        Amount = model.Amount,
                        ChargeRs = 0,// model.ChargeRs,
                        DateTime = model.DateTime,
                        PairName = model.PairName,
                        Price = model.Price,
                        Status = model.Status,
                        StatusText = model.StatusText,
                        TrnNo = model.TrnNo,
                        Type = model.Type,
                        Total = model.Type == "BUY" ? ((model.Price * model.Amount) - model.ChargeRs) : ((model.Price * model.Amount)),
                        IsCancel = model.IsCancelled,
                        OrderType = Enum.GetName(typeof(enTransactionMarketType), model.ordertype),
                        SettledDate = model.SettledDate,
                        SettledQty = model.SettledQty,
                        FollowTradeType = model.FollowTradeType,
                        FollowingTo = model.FollowingTo
                    });
                }
                _Res.Response = leaderOrdersInfos;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region Optimize method

        public List<GetOrderHistoryInfo> GetOrderHistory(long PairId, short IsMargin = 0)
        {
            try
            {
                if (IsMargin == 1)
                    return _frontTrnRepository.GetOrderHistoryMargin(PairId);
                else
                    return _frontTrnRepository.GetOrderHistory(PairId);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetOrderHistoryInfoArbitrageV1> GetOrderHistoryArbitrageV1(long PairId)
        {
            try
            {
                return _frontTrnRepository.GetOrderHistoryArbitrage(PairId);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetTradeHistoryInfoV1> GetTradeHistoryV1(long MemberID, string sCondition, string FromDate, string TodDate, int page, int IsAll, short IsMargin = 0)
        {
            try
            {
                List<GetTradeHistoryInfoV1> list;
                if (IsMargin == 1)//Rita 22-2-19 for Margin Trading Data bit
                    list = _frontTrnRepository.GetTradeHistoryMarginV1(MemberID, sCondition, FromDate, TodDate, page, IsAll);
                else
                    list = _frontTrnRepository.GetTradeHistoryV1(MemberID, sCondition, FromDate, TodDate, page, IsAll);

                if (list.Count>0)
                {
                    if (page > 0)
                    {
                        int skip = Helpers.PageSize * (page - 1);
                        list = list.Skip(skip).Take(Helpers.PageSize).ToList();
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<GetTradeHistoryInfoArbitrageV1> GetTradeHistoryArbitrageV1(long MemberID, string sCondition, string FromDate, string TodDate, int page, int IsAll)
        {
            try
            {
                List<GetTradeHistoryInfoArbitrageV1>  list = _frontTrnRepository.GetTradeHistoryArbitrageV1(MemberID, sCondition, FromDate, TodDate, page, IsAll);

                if (list.Count > 0)
                {
                    if (page > 0)
                    {
                        int skip = Helpers.PageSize * (page - 1);
                        list = list.Skip(skip).Take(Helpers.PageSize).ToList();
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<RecentOrderInfoV1> GetRecentOrderV1(long PairId, long MemberID, short IsMargin = 0)
        {
            try
            {
                if (IsMargin == 1)
                    return  _frontTrnRepository.GetRecentOrderMarginV1(PairId, MemberID);
                else
                    return _frontTrnRepository.GetRecentOrderV1(PairId, MemberID);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<RecentOrderInfoArbitrageV1> GetRecentOrderArbitrageV1(long PairId, long MemberID)
        {
            try
            {
                return _frontTrnRepository.GetRecentOrderArbitrageV1(PairId, MemberID);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ActiveOrderInfoV1> GetActiveOrderV1(long MemberID, string FromDate, string TodDate, long PairId, int Page, short trnType, short IsMargin = 0)//Rita 22-2-19 for Margin Trading Data bit
        {
            try
            {
                List<ActiveOrderInfoV1> ActiveOrderList;
                if (IsMargin == 1)
                    ActiveOrderList = _frontTrnRepository.GetActiveOrderMarginV1(MemberID, FromDate, TodDate, PairId, trnType);
                else
                    ActiveOrderList = _frontTrnRepository.GetActiveOrderV1(MemberID, FromDate, TodDate, PairId, trnType);

                if (ActiveOrderList.Count > 0)
                {
                    if (Page > 0)
                    {
                        int skip = Helpers.PageSize * (Page - 1);
                        ActiveOrderList = ActiveOrderList.Skip(skip).Take(Helpers.PageSize).ToList();
                    }
                }
                return ActiveOrderList;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ActiveOrderInfoArbitrageV1> GetActiveOrderArbitrageV1(long MemberID, string FromDate, string TodDate, long PairId, int Page, short trnType)
        {
            try
            {
                List<ActiveOrderInfoArbitrageV1> ActiveOrderList = _frontTrnRepository.GetActiveOrderArbitrageV1(MemberID, FromDate, TodDate, PairId, trnType);
                if (ActiveOrderList.Count > 0)
                {
                    if (Page > 0)
                    {
                        int skip = Helpers.PageSize * (Page - 1);
                        ActiveOrderList = ActiveOrderList.Skip(skip).Take(Helpers.PageSize).ToList();
                    }
                }
                return ActiveOrderList;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public CopiedLeaderOrdersResponseV1 GetCopiedLeaderOrdersV1(long MemberID, string FromDate = "", string TodDate = "", long PairId = 999, short trnType = 999, string FollowTradeType = "", long FollowingTo = 0, int PageSize = 0, int PageNo = 0)
        {
            List<CopiedLeaderOrdersInfoV1> leaderOrdersInfos = new List<CopiedLeaderOrdersInfoV1>();
            CopiedLeaderOrdersResponseV1 _Res = new CopiedLeaderOrdersResponseV1();
            int skip;
            int size;
            try
            {
                if (PageSize == 0)
                    size = Helpers.PageSize;
                else
                    size = PageSize;

                skip = size * (PageNo);
                leaderOrdersInfos = _frontTrnRepository.GetCopiedLeaderOrdersV1(MemberID, FromDate, TodDate, PairId, trnType, FollowTradeType, FollowingTo);
                if (leaderOrdersInfos.Count == 0)
                {
                    _Res.Response = leaderOrdersInfos;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                _Res.TotalCount = leaderOrdersInfos.Count();
                _Res.Response = leaderOrdersInfos.Skip(skip).Take(size).ToList();
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region Insert Trading Data methods

        public async Task GetPairAdditionalVal(long PairId, decimal CurrentRate, long TrnNo, decimal Quantity, DateTime TranDate, string UserID = "")
        {
            try
            {
                //Task.Run(() => HelperForLog.WriteLogIntoFile("#GetPairAdditionalVal# #ParameterValue1# " + " #TrnNo# :" + TrnNo + " #CurrentRate# : " + CurrentRate + " #Quantity# : " + Quantity, "FrontService", "Object Data : "));
                decimal Volume24 = 0, ChangePer = 0, ChangeValue = 0;
                decimal FirstRateOftheDay = 0; //todayopen
                decimal LastRate = 0;//todayclose;
                //komal 31-11-2019 change settle queue to tradepool queue
                LastRate = _TradePoolQueueV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-1) && e.Status == 1).OrderByDescending(x => x.Id).Select(e => e.TakerPrice).FirstOrDefault();
                FirstRateOftheDay = _TradePoolQueueV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-1) && e.Status == 1).OrderBy(x => x.Id).Select(e => e.TakerPrice).FirstOrDefault();

                if (LastRate > 0 && FirstRateOftheDay > 0)
                {
                    ChangePer = ((LastRate * 100) / FirstRateOftheDay) - 100;
                    ChangeValue = LastRate - FirstRateOftheDay;
                }
                else if (LastRate > 0 && FirstRateOftheDay == 0)
                {
                    ChangePer = 100;
                    ChangeValue = LastRate;
                }

                var SettledData = _settleTradeTransactionQueue.FindBy(x => x.PairID == PairId && x.Status == 1 && x.SettledDate >= _basePage.UTC_To_IST().AddDays(-1)).ToList();//TrnDate Rita 11-4-19 only settle txn , not created txn
                decimal tradeqty = 0, sum = 0; var tradedata1 = SettledData;
                if (tradedata1 != null && tradedata1.Count() > 0)
                {
                    foreach (var trade in tradedata1)//Rita 11-4-19 taken settledQty instead of total Qty
                    {
                        if (trade.TrnType == 4)
                        {
                            tradeqty = trade.SettledSellQty;
                        }
                        else if (trade.TrnType == 5)
                        {
                            tradeqty = trade.SettledBuyQty;
                        }
                        else
                        {
                            tradeqty = 0;
                        }
                        sum += tradeqty;
                    }
                    Volume24 = sum;
                }
                ////Task.Run(() => HelperForLog.WriteLogIntoFile("#GetPairAdditionalVal# #ParameterValue1# " + " #TrnNo# :" + TrnNo + " #CurrentRate# : " + CurrentRate + " #Quantity# : " + Quantity, "FrontService", "Object Data : "));
                //decimal Volume24 = 0, ChangePer = 0, High24Hr = 0, Low24Hr = 0, WeekHigh = 0, WeekLow = 0, Week52High = 0, Week52Low = 0, ChangeValue = 0;
                //short UpDownBit = 1; //komal 13-11-2018 set defau
                //decimal tradeprice = 0; //todayopen, todayclose;
                //decimal LastRate = 0;

                ////Uday 22-12-2018 Get All Record Of Last One Day Of Particular Pair
                //var SettledData = _settleTradeTransactionQueue.FindBy(x => x.PairID == PairId && x.Status == 1 && x.SettledDate >= _basePage.UTC_To_IST().AddDays(-1)).ToList();//TrnDate Rita 11-4-19 only settle txn , not created txn
                //var tradeRateData = SettledData.OrderByDescending(x => x.Id).FirstOrDefault();
                //if (tradeRateData != null)
                //{
                //    if (tradeRateData.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
                //    {
                //        LastRate = tradeRateData.BidPrice;
                //    }
                //    else if (tradeRateData.TrnType == Convert.ToInt16(enTrnType.Sell_Trade))
                //    {
                //        LastRate = tradeRateData.AskPrice;
                //    }
                //}
                //else
                //    LastRate = 0;

                //var tradedata = SettledData.OrderBy(x => x.Id).FirstOrDefault();
                //if (tradedata != null)
                //{
                //    //Task.Run(()=>HelperForLog.WriteLogIntoFile("#GetPairAdditionalVal# #CHANGEPER# #Count# : 1 " + " #TrnNo# :" + TrnNo + " #BidPrice# : " + tradedata.BidPrice + " #AskPrice# : " + tradedata.AskPrice, "FrontService", "Object Data : "));
                //    if (tradedata.TrnType == 4)
                //        tradeprice = tradedata.BidPrice;
                //    else if (tradedata.TrnType == 5)
                //        tradeprice = tradedata.AskPrice;
                //    if (LastRate > 0 && tradeprice > 0)
                //    {
                //        ChangePer = ((LastRate * 100) / tradeprice) - 100;
                //        ChangeValue = LastRate - tradeprice;
                //    }
                //    else if (LastRate > 0 && tradeprice == 0)
                //    {
                //        ChangePer = 100;
                //        ChangeValue = LastRate;
                //    }
                //    else
                //    {
                //        ChangePer = 0;
                //        ChangeValue = 0;
                //    }
                //}
                //else
                //{
                //    ChangePer = 0;
                //    ChangeValue = 0;
                //}

                //tradeprice = 0;
                //decimal tradeqty = 0, sum = 0;var tradedata1 = SettledData;
                //if (tradedata1 != null && tradedata1.Count() > 0)
                //{
                //    foreach (var trade in tradedata1)//Rita 11-4-19 taken settledQty instead of total Qty
                //    {
                //        if (trade.TrnType == 4)
                //        {
                //            tradeqty = trade.SettledSellQty;
                //        }
                //        else if (trade.TrnType == 5)
                //        {
                //            tradeqty = trade.SettledBuyQty;
                //        }
                //        else
                //        {
                //            tradeqty = 0;
                //        }
                //        sum += tradeqty;
                //    }
                //    Volume24 = sum;
                //}
                //else
                //    Volume24 = 0;

                //Insert In GraphDetail Only BidPrice
                var DataDate = TranDate;
                var tradegraph = new TradeGraphDetail()
                {
                    PairId = PairId,
                    TranNo = TrnNo,
                    DataDate = DataDate,
                    ChangePer = ChangePer,
                    Volume = Volume24,
                    BidPrice = CurrentRate,
                    LTP = CurrentRate,
                    Quantity = Quantity,
                    CreatedBy = 1,
                    CreatedDate = _basePage.UTC_To_IST()
                };

                try
                {
                    tradegraph = _graphDetailRepository.Add(tradegraph);
                }
                catch (Exception ex)
                {
                    //Uday 08-01-2019 add Trnno in errorlog. check which trnno has been duplicate
                    HelperForLog.WriteLogIntoFile("#GetPairAdditionalVal# " + " #TradeGraphDetail# #DuplicateTrnNo# : " + TrnNo, "FrontService", "Duplicate TrnNo in TradeGraphDetail");
                    HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " #TrnNo# : " + TrnNo + "  ", this.GetType().Name, ex);
                }
                finally
                {
                    ////Calculate High Low Data For 24Hr
                    //var tardeTrabDetail = SettledData.OrderByDescending(x => x.Id).ToList();
                    //High24Hr = LastRate;
                    //Low24Hr = LastRate;
                    //if (tardeTrabDetail.Count > 0)
                    //{
                    //    foreach (SettledTradeTransactionQueue type in tardeTrabDetail)
                    //    {
                    //        decimal price = 0;
                    //        if (type.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
                    //            price = type.BidPrice;
                    //        else if (type.TrnType == Convert.ToInt16(enTrnType.Sell_Trade))
                    //            price = type.AskPrice;

                    //        if (price > High24Hr)
                    //            High24Hr = price;
                    //        if (price < Low24Hr)
                    //            Low24Hr = price;
                    //    }
                    //}

                    ////Calculate High Low Data For Week
                    //var WeekData = _frontTrnRepository.GetHighLowValue(PairId, -7);
                    //if (WeekData != null)
                    //{
                    //    WeekHigh = WeekData.HighPrice;
                    //    WeekLow = WeekData.LowPrice;
                    //}
                    ////Calculate High Low Data For 52Week
                    //var Week52Data = _frontTrnRepository.GetHighLowValue(PairId, -365);
                    //if (Week52Data != null)
                    //{
                    //    Week52High = Week52Data.HighPrice;
                    //    Week52Low = Week52Data.LowPrice;
                    //}
                    //var pairData = _tradePairStastics.GetSingle(x => x.PairId == PairId);
                    //HelperForLog.WriteLogIntoFile("#GetLastPairData# #PairId : " + PairId + " #TrnNo# : " + TrnNo + " UpDownBit : " + pairData.UpDownBit + " LTP : " + pairData.LTP + " Last TrnDate : " + pairData.TranDate, "Object Data : ");
                    //if (CurrentRate > pairData.High24Hr) //komal 13-11-2018 Change code sequence cos got 0 every time
                    //    UpDownBit = 1;
                    //else if (CurrentRate < pairData.Low24Hr)
                    //    UpDownBit = 0;
                    //else
                    //{
                    //    if (CurrentRate < pairData.LTP)
                    //        UpDownBit = 0;
                    //    else if (CurrentRate > pairData.LTP)
                    //        UpDownBit = 1;
                    //    else if (CurrentRate == pairData.LTP)//komal 13-11-2018 if no change then set as it is
                    //        UpDownBit = pairData.UpDownBit;
                    //}

                    //Calculate High Low Data For 24Hr
                    decimal High24Hr = 0, Low24Hr = 0, WeekHigh = 0, WeekLow = 0, Week52High = 0, Week52Low = 0;
                    short UpDownBit = 1; //komal 13-11-2018 set defaut

                    High24Hr = CurrentRate; //if not found then set current rate
                    Low24Hr = CurrentRate;
                    High24Hr = _TradePoolQueueV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-1) && e.Status == 1).Max(e => e.TakerPrice);
                    Low24Hr = _TradePoolQueueV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-1) && e.Status == 1).Min(e => e.TakerPrice);
                    //Calculate High Low Data For Week
                    WeekHigh = _TradePoolQueueV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-7) && e.Status == 1).Max(e => e.TakerPrice);
                    WeekLow = _TradePoolQueueV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-7) && e.Status == 1).Min(e => e.TakerPrice);
                    //Calculate High Low Data For 52Week
                    Week52High = _TradePoolQueueV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-365) && e.Status == 1).Max(e => e.TakerPrice);
                    Week52Low = _TradePoolQueueV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-365) && e.Status == 1).Min(e => e.TakerPrice);

                    if (CurrentRate > High24Hr) //komal 13-11-2018 Change code sequence cos got 0 every time
                        UpDownBit = 1;
                    else if (CurrentRate < Low24Hr)
                        UpDownBit = 0;

                    var pairData = _tradePairStastics.GetSingle(x => x.PairId == PairId);
                    Task.Run(() => HelperForLog.WriteLogIntoFile("#UpdatePairLTPStart# " + " UpDownBit : " + UpDownBit + " TrnNo : " + TrnNo + " CurrentRate : " + pairData.CurrentRate, "Object Data : "));
                    pairData.ChangePer24 = ChangePer;
                    pairData.ChangeVol24 = Volume24;
                    pairData.High24Hr = High24Hr;
                    pairData.Low24Hr = Low24Hr;
                    pairData.LTP = CurrentRate;
                    pairData.CurrentRate = CurrentRate;
                    pairData.HighWeek = WeekHigh;
                    pairData.LowWeek = WeekLow;
                    pairData.High52Week = Week52High;
                    pairData.Low52Week = Week52Low;
                    pairData.TranDate = TranDate;
                    pairData.UpDownBit = UpDownBit;
                    pairData.ChangeValue = ChangeValue;
                    _tradePairStastics.Update(pairData);
                    Task.Run(() => HelperForLog.WriteLogIntoFile("#UpdatePairLTEND# " + " UpDownBit : " + UpDownBit + " TrnNo : " + TrnNo + " CurrentRate : " + pairData.CurrentRate, "Object Data : "));
                    //komal 16-11-2018 Set Volume Data avoid DB call

                    //komal 16-07-2019 add local LTP change to CryptoWatcher
                    LTPcls LTPobj = new LTPcls();
                    LTPobj.LpType= (short)enAppType.COINTTRADINGLocal;
                    LTPobj.Pair= _tradeMasterRepository.GetById(PairId).PairName;
                    LTPobj.Price = CurrentRate;
                    var Res = _frontTrnRepository.UpdateLTPData(LTPobj);
                    if (Res == null)
                        _frontTrnRepository.InsertLTPData(LTPobj);


                    VolumeDataRespose VolumeData = new VolumeDataRespose();
                    VolumeData.PairId = PairId;
                    VolumeData.PairName = _tradeMasterRepository.GetById(PairId).PairName;
                    VolumeData.Currentrate = pairData.CurrentRate;
                    VolumeData.ChangePer = pairData.ChangePer24;
                    VolumeData.Volume24 = pairData.ChangeVol24;
                    VolumeData.High24Hr = pairData.High24Hr;
                    VolumeData.Low24Hr = pairData.Low24Hr;
                    VolumeData.HighWeek = pairData.HighWeek;
                    VolumeData.LowWeek = pairData.LowWeek;
                    VolumeData.High52Week = pairData.High52Week;
                    VolumeData.Low52Week = pairData.Low52Week;
                    VolumeData.UpDownBit = pairData.UpDownBit;

                    //komal 16-11-2018 Set MArket Data avoid DB call
                    MarketCapData MarketData = new MarketCapData();
                    MarketData.Change24 = pairData.High24Hr - pairData.Low24Hr;
                    MarketData.ChangePer = pairData.ChangePer24;
                    MarketData.High24 = pairData.High24Hr;
                    MarketData.Low24 = pairData.Low24Hr;
                    MarketData.LastPrice = pairData.LTP;
                    MarketData.Volume24 = pairData.ChangeVol24;

                    Task.Run(() => HelperForLog.WriteLogIntoFile("#VolumeDataToSocket# #PairId# : " + PairId + " #TrnNo# : " + TrnNo, "FrontService", "Object Data : "));
                    Task.Run(() => HelperForLog.WriteLogIntoFile("#MarketDataToSocket# #PairId# : " + PairId + " #TrnNo# : " + TrnNo, "FrontService", "Object Data : "));
                    Task.Run(() => _signalRService.OnVolumeChange(VolumeData, MarketData, UserID));

                    //Uday 25-12-2018  SignalR Call For Market Ticker
                    var PairDetailMarketTicker = _tradeDetailRepository.GetSingle(x => x.PairId == PairId);
                    if (PairDetailMarketTicker != null)
                    {
                        if (PairDetailMarketTicker.IsMarketTicker == 1)
                        {
                            List<VolumeDataRespose> MarketTickerData = new List<VolumeDataRespose>();
                            MarketTickerData.Add(VolumeData);
                            Task.Run(() => HelperForLog.WriteLogIntoFile("#MarketTickerSocket# #PairId# : " + PairId + " #TrnNo# : " + TrnNo, "FrontService", "Object Data : "));
                            Task.Run(() => _signalRService.MarketTicker(MarketTickerData, UserID));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 20-2-19 for margin trading
        public async Task GetPairAdditionalValMargin(long PairId, decimal CurrentRate, long TrnNo, decimal Quantity, DateTime TranDate, string UserID = "")
        {
            string MethodName = "GetPairAdditionalValMargin";
            try
            {
                decimal Volume24 = 0, ChangePer = 0, ChangeValue = 0;
                decimal FirstRateOftheDay = 0; //todayopen
                decimal LastRate = 0;//todayclose;
                //komal 31-11-2019 change settle queue to tradepool queue
                LastRate = _TradePoolQueueMarginV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-1) && e.Status == 1).OrderByDescending(x => x.Id).Select(e => e.TakerPrice).FirstOrDefault();
                FirstRateOftheDay = _TradePoolQueueMarginV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-1) && e.Status == 1).OrderBy(x => x.Id).Select(e => e.TakerPrice).FirstOrDefault();

                if (LastRate > 0 && FirstRateOftheDay > 0)
                {
                    ChangePer = ((LastRate * 100) / FirstRateOftheDay) - 100;
                    ChangeValue = LastRate - FirstRateOftheDay;
                }
                else if (LastRate > 0 && FirstRateOftheDay == 0)
                {
                    ChangePer = 100;
                    ChangeValue = LastRate;
                }
                var SettledData = _settleTradeTransactionQueueMargin.FindBy(x => x.PairID == PairId && x.Status == 1 && x.SettledDate >= _basePage.UTC_To_IST().AddDays(-1)).ToList();//TrnDate Rita 11-4-19 only settle txn , not created txn
                decimal tradeqty = 0, sum = 0; var tradedata1 = SettledData;
                if (tradedata1 != null && tradedata1.Count() > 0)
                {
                    foreach (var trade in tradedata1)//Rita 11-4-19 taken settledQty instead of total Qty
                    {
                        if (trade.TrnType == 4)
                        {
                            tradeqty = trade.SettledSellQty;
                        }
                        else if (trade.TrnType == 5)
                        {
                            tradeqty = trade.SettledBuyQty;
                        }
                        else
                        {
                            tradeqty = 0;
                        }
                        sum += tradeqty;
                    }
                    Volume24 = sum;
                }
                //Task.Run(() => HelperForLog.WriteLogIntoFile(MethodName + " #ParameterValue1# " + " #TrnNo# :" + TrnNo + " #CurrentRate# : " + CurrentRate + " #Quantity# : " + Quantity, "FrontService", "Object Data : "));
                ////Calucalte ChangePer
                //decimal Volume24 = 0, ChangePer = 0, High24Hr = 0, Low24Hr = 0, WeekHigh = 0, WeekLow = 0, Week52High = 0, Week52Low = 0, ChangeValue = 0;
                //short UpDownBit = 1; //komal 13-11-2018 set defau
                //decimal tradeprice = 0; //todayopen, todayclose;
                //decimal LastRate = 0;
                ////Uday 22-12-2018 Get All Record Of Last One Day Of Particular Pair
                //var SettledData = _settleTradeTransactionQueueMargin.FindBy(x => x.PairID == PairId && x.Status == 1 && x.SettledDate >= _basePage.UTC_To_IST().AddDays(-1)).ToList();//TrnDate Rita 11-4-19 only settle txn , not created txn
                //var tradeRateData = SettledData.OrderByDescending(x => x.Id).FirstOrDefault();
                //if (tradeRateData != null)
                //{
                //    if (tradeRateData.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
                //    {
                //        LastRate = tradeRateData.BidPrice;
                //    }
                //    else if (tradeRateData.TrnType == Convert.ToInt16(enTrnType.Sell_Trade))
                //    {
                //        LastRate = tradeRateData.AskPrice;
                //    }
                //}
                //else
                //{
                //    LastRate = 0;
                //}

                //var tradedata = SettledData.OrderBy(x => x.Id).FirstOrDefault();
                //if (tradedata != null)
                //{
                //    if (tradedata.TrnType == 4)
                //    {
                //        tradeprice = tradedata.BidPrice;
                //    }
                //    else if (tradedata.TrnType == 5)
                //    {
                //        tradeprice = tradedata.AskPrice;
                //    }
                //    if (LastRate > 0 && tradeprice > 0)
                //    {
                //        ChangePer = ((LastRate * 100) / tradeprice) - 100;
                //        //Calculate ChangeValue
                //        ChangeValue = LastRate - tradeprice;
                //    }
                //    else if (LastRate > 0 && tradeprice == 0)
                //    {
                //        ChangePer = 100;
                //        ChangeValue = LastRate;
                //    }
                //    else
                //    {
                //        ChangePer = 0;
                //        ChangeValue = 0;
                //    }
                //}
                //else
                //{
                //    ChangePer = 0;
                //    ChangeValue = 0;
                //}

                ////Calculate Volume24
                //tradeprice = 0;
                //decimal tradeqty = 0, sum = 0;
                //var tradedata1 = SettledData;
                //if (tradedata1 != null && tradedata1.Count() > 0)
                //{
                //    foreach (var trade in tradedata1)//Rita 11-4-19 taken settledQty instead of total Qty
                //    {
                //        if (trade.TrnType == 4)
                //            tradeqty = trade.SettledSellQty;
                //        else if (trade.TrnType == 5)
                //            tradeqty = trade.SettledBuyQty;
                //        else
                //            tradeqty = 0;
                //        sum += tradeqty;
                //    }
                //    Volume24 = sum;
                //}
                //else
                //{
                //    Volume24 = 0;
                //}

                //Insert In GraphDetail Only BidPrice
                var DataDate = TranDate;
                var tradegraph = new TradeGraphDetailMargin()
                {
                    PairId = PairId,
                    TranNo = TrnNo,
                    DataDate = DataDate,
                    ChangePer = ChangePer,
                    Volume = Volume24,
                    BidPrice = CurrentRate,
                    LTP = CurrentRate,
                    Quantity = Quantity,
                    CreatedBy = 1,
                    CreatedDate = _basePage.UTC_To_IST()
                };

                try
                {
                    tradegraph = _graphDetailRepositoryMargin.Add(tradegraph);
                }
                catch (Exception ex)
                {
                    //Uday 08-01-2019 add Trnno in errorlog. check which trnno has been duplicate
                    HelperForLog.WriteLogIntoFile(MethodName + " #TradeGraphDetail# #DuplicateTrnNo# : " + TrnNo, "FrontService", "Duplicate TrnNo in TradeGraphDetail");
                    HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " #TrnNo# : " + TrnNo + "  ", this.GetType().Name, ex);
                }
                finally
                {
                    ////Calculate High Low Data For 24Hr
                    //var tardeTrabDetail = SettledData.OrderByDescending(x => x.Id).ToList();
                    //High24Hr = LastRate;
                    //Low24Hr = LastRate;
                    //if (tardeTrabDetail.Count > 0)
                    //{
                    //    foreach (SettledTradeTransactionQueueMargin type in tardeTrabDetail)
                    //    {
                    //        decimal price = 0;
                    //        if (type.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
                    //            price = type.BidPrice;
                    //        else if (type.TrnType == Convert.ToInt16(enTrnType.Sell_Trade))
                    //            price = type.AskPrice;

                    //        if (price > High24Hr)
                    //            High24Hr = price;
                    //        if (price < Low24Hr)
                    //            Low24Hr = price;
                    //    }
                    //}

                    ////Calculate High Low Data For Week
                    //var WeekData = _frontTrnRepository.GetHighLowValueMargin(PairId, -7);
                    //if (WeekData != null)
                    //{
                    //    WeekHigh = WeekData.HighPrice;
                    //    WeekLow = WeekData.LowPrice;
                    //}

                    ////Calculate High Low Data For 52Week
                    //var Week52Data = _frontTrnRepository.GetHighLowValueMargin(PairId, -365);
                    //if (Week52Data != null)
                    //{
                    //    Week52High = Week52Data.HighPrice;
                    //    Week52Low = Week52Data.LowPrice;
                    //}
                    //var pairData = _tradePairStasticsMargin.GetSingle(x => x.PairId == PairId);
                    //HelperForLog.WriteLogIntoFile(MethodName + " #GetLastPairData# #PairId : " + PairId + " #TrnNo# : " + TrnNo + " UpDownBit : " + pairData.UpDownBit + " LTP : " + pairData.LTP + " Last TrnDate : " + pairData.TranDate, "Object Data : ");

                    //if (CurrentRate > pairData.High24Hr) //komal 13-11-2018 Change code sequence cos got 0 every time
                    //    UpDownBit = 1;
                    //else if (CurrentRate < pairData.Low24Hr)
                    //    UpDownBit = 0;
                    //else
                    //{
                    //    if (CurrentRate < pairData.LTP)
                    //    {
                    //        UpDownBit = 0;
                    //    }
                    //    else if (CurrentRate > pairData.LTP)
                    //    {
                    //        UpDownBit = 1;
                    //    }
                    //    else if (CurrentRate == pairData.LTP)//komal 13-11-2018 if no change then set as it is
                    //    {
                    //        UpDownBit = pairData.UpDownBit;
                    //    }
                    //}
                    //Calculate High Low Data For 24Hr
                    decimal High24Hr = 0, Low24Hr = 0, WeekHigh = 0, WeekLow = 0, Week52High = 0, Week52Low = 0;
                    short UpDownBit = 1; //komal 13-11-2018 set defaut

                    High24Hr = CurrentRate; //if not found then set current rate
                    Low24Hr = CurrentRate;
                    High24Hr = _TradePoolQueueMarginV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-1) && e.Status == 1).Max(e => e.TakerPrice);
                    Low24Hr = _TradePoolQueueMarginV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-1) && e.Status == 1).Min(e => e.TakerPrice);
                    //Calculate High Low Data For Week
                    WeekHigh = _TradePoolQueueMarginV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-7) && e.Status == 1).Max(e => e.TakerPrice);
                    WeekLow = _TradePoolQueueMarginV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-7) && e.Status == 1).Min(e => e.TakerPrice);
                    //Calculate High Low Data For 52Week
                    Week52High = _TradePoolQueueMarginV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-365) && e.Status == 1).Max(e => e.TakerPrice);
                    Week52Low = _TradePoolQueueMarginV1Repository.FindBy(e => e.PairID == PairId && e.CreatedDate >= _basePage.UTC_To_IST().AddDays(-365) && e.Status == 1).Min(e => e.TakerPrice);

                    if (CurrentRate > High24Hr) //komal 13-11-2018 Change code sequence cos got 0 every time
                        UpDownBit = 1;
                    else if (CurrentRate < Low24Hr)
                        UpDownBit = 0;
                    var pairData = _tradePairStasticsMargin.GetSingle(x => x.PairId == PairId);
                    Task.Run(() => HelperForLog.WriteLogIntoFile("#UpdatePairLTPStart# " + " UpDownBit : " + UpDownBit + " TrnNo : " + TrnNo + " CurrentRate : " + pairData.CurrentRate + " High24Hr : " + High24Hr + " Low24Hr : " + Low24Hr, "Object Data : "));
                    pairData.ChangePer24 = ChangePer;
                    pairData.ChangeVol24 = Volume24;
                    pairData.High24Hr = High24Hr;
                    pairData.Low24Hr = Low24Hr;
                    pairData.LTP = CurrentRate;
                    pairData.CurrentRate = CurrentRate;
                    pairData.HighWeek = WeekHigh;
                    pairData.LowWeek = WeekLow;
                    pairData.High52Week = Week52High;
                    pairData.Low52Week = Week52Low;
                    pairData.TranDate = TranDate;
                    pairData.UpDownBit = UpDownBit;
                    pairData.ChangeValue = ChangeValue;
                    _tradePairStasticsMargin.Update(pairData);
                    Task.Run(() => HelperForLog.WriteLogIntoFile(MethodName + " #UpdatePairLTEND# " + " UpDownBit : " + UpDownBit + " TrnNo : " + TrnNo + " CurrentRate : " + pairData.CurrentRate, "Object Data : "));
                    //komal 16-11-2018 Set Volume Data avoid DB call
                    VolumeDataRespose VolumeData = new VolumeDataRespose();
                    VolumeData.PairId = PairId;
                    //VolumeData.PairName = _tradeMasterRepository.GetById(PairId).PairName;
                    VolumeData.PairName = _trnMasterConfiguration.GetTradePairMasterMargin().Where(e => e.Id == PairId).FirstOrDefault().PairName;
                    VolumeData.Currentrate = pairData.CurrentRate;
                    VolumeData.ChangePer = pairData.ChangePer24;
                    VolumeData.Volume24 = pairData.ChangeVol24;
                    VolumeData.High24Hr = pairData.High24Hr;
                    VolumeData.Low24Hr = pairData.Low24Hr;
                    VolumeData.HighWeek = pairData.HighWeek;
                    VolumeData.LowWeek = pairData.LowWeek;
                    VolumeData.High52Week = pairData.High52Week;
                    VolumeData.Low52Week = pairData.Low52Week;
                    VolumeData.UpDownBit = pairData.UpDownBit;

                    //komal 16-11-2018 Set MArket Data avoid DB call
                    MarketCapData MarketData = new MarketCapData();
                    MarketData.Change24 = pairData.High24Hr - pairData.Low24Hr;
                    MarketData.ChangePer = pairData.ChangePer24;
                    MarketData.High24 = pairData.High24Hr;
                    MarketData.Low24 = pairData.Low24Hr;
                    MarketData.LastPrice = pairData.LTP;
                    MarketData.Volume24 = pairData.ChangeVol24;
                    Task.Run(() => _signalRService.OnVolumeChangeMargin(VolumeData, MarketData, UserID));

                    //Uday 25-12-2018  SignalR Call For Market Ticker
                    var PairDetailMarketTicker = _tradeDetailRepositoryMargin.GetSingle(x => x.PairId == PairId);
                    if (PairDetailMarketTicker != null)
                    {
                        if (PairDetailMarketTicker.IsMarketTicker == 1)
                        {
                            List<VolumeDataRespose> MarketTickerData = new List<VolumeDataRespose>();
                            MarketTickerData.Add(VolumeData);Task.Run(() => HelperForLog.WriteLogIntoFile(MethodName + " #MarketTickerSocket# #PairId# : " + PairId + " #TrnNo# : " + TrnNo, "FrontService", "Object Data : "));
                            Task.Run(() => _signalRService.MarketTicker(MarketTickerData, UserID, "", IsMargin: 1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetPairAdditionalValMargin ##TrnNo:" + TrnNo, "FrontTrnService", ex);
                //throw ex;
            }
        }

        public async Task GetPairAdditionalValArbitrage(long PairId, decimal CurrentRate, long TrnNo, decimal Quantity, DateTime TranDate, string UserID = "")
        {
            string MethodName = "GetPairAdditionalValArbitrage";
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFile(MethodName + " #ParameterValue1# " + " #TrnNo# :" + TrnNo + " #CurrentRate# : " + CurrentRate + " #Quantity# : " + Quantity, "FrontService", "Object Data : "));
                //Calucalte ChangePer
                decimal Volume24 = 0, ChangePer = 0, High24Hr = 0, Low24Hr = 0, WeekHigh = 0, WeekLow = 0, Week52High = 0, Week52Low = 0, ChangeValue = 0;
                short UpDownBit = 1; //komal 13-11-2018 set default
                decimal tradeprice = 0; //todayopen, todayclose;
                decimal LastRate = 0;

                //Uday 22-12-2018 Get All Record Of Last One Day Of Particular Pair
                var SettledData = _settleTradeTransactionQueueArbitrage.FindBy(x => x.PairID == PairId && x.Status == 1 && x.SettledDate >= _basePage.UTC_To_IST().AddDays(-1)).ToList();//TrnDate Rita 11-4-19 only settle txn , not created txn

                var tradeRateData = SettledData.OrderByDescending(x => x.Id).FirstOrDefault();
                if (tradeRateData != null)
                {
                    if (tradeRateData.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
                        LastRate = tradeRateData.BidPrice;
                    else if (tradeRateData.TrnType == Convert.ToInt16(enTrnType.Sell_Trade))
                        LastRate = tradeRateData.AskPrice;
                }
                else
                    LastRate = 0;

                var tradedata = SettledData.OrderBy(x => x.Id).FirstOrDefault();
                if (tradedata != null)
                {
                    if (tradedata.TrnType == 4)
                        tradeprice = tradedata.BidPrice;
                    else if (tradedata.TrnType == 5)
                        tradeprice = tradedata.AskPrice;
                    if (LastRate > 0 && tradeprice > 0)
                    {
                        ChangePer = ((LastRate * 100) / tradeprice) - 100;
                        //Calculate ChangeValue
                        ChangeValue = LastRate - tradeprice;
                    }
                    else if (LastRate > 0 && tradeprice == 0)
                    {
                        ChangePer = 100;
                        ChangeValue = LastRate;
                    }
                    else
                    {
                        ChangePer = 0;
                        ChangeValue = 0;
                    }
                }
                else
                {
                    ChangePer = 0;
                    ChangeValue = 0;
                }

                //Calculate Volume24
                tradeprice = 0;
                decimal tradeqty = 0, sum = 0;
                var tradedata1 = SettledData;
                if (tradedata1 != null && tradedata1.Count() > 0)
                {
                    foreach (var trade in tradedata1)//Rita 11-4-19 taken settledQty instead of total Qty
                    {
                        if (trade.TrnType == 4)
                            tradeqty = trade.SettledSellQty;
                        else if (trade.TrnType == 5)
                            tradeqty = trade.SettledBuyQty;
                        else
                            tradeqty = 0;
                        sum += tradeqty;
                    }
                    Volume24 = sum;
                }
                else
                    Volume24 = 0;

                //Insert In GraphDetail Only BidPrice
                var DataDate = TranDate;
                var tradegraph = new TradeGraphDetailArbitrage()
                {
                    PairId = PairId,
                    TranNo = TrnNo,
                    DataDate = DataDate,
                    ChangePer = ChangePer,
                    Volume = Volume24,
                    BidPrice = CurrentRate,
                    LTP = CurrentRate,
                    Quantity = Quantity,
                    CreatedBy = 1,
                    CreatedDate = _basePage.UTC_To_IST()
                };

                try
                {
                    tradegraph = _graphDetailRepositoryArbitrage.Add(tradegraph);
                }
                catch (Exception ex)
                {
                    //Uday 08-01-2019 add Trnno in errorlog. check which trnno has been duplicate
                    HelperForLog.WriteLogIntoFile(MethodName + " #TradeGraphDetail# #DuplicateTrnNo# : " + TrnNo, "FrontService", "Duplicate TrnNo in TradeGraphDetail");
                    HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " #TrnNo# : " + TrnNo + "  ", this.GetType().Name, ex);
                }
                finally
                {
                    //Calculate High Low Data For 24Hr
                    var tardeTrabDetail = SettledData.OrderByDescending(x => x.Id).ToList();
                    High24Hr = LastRate;
                    Low24Hr = LastRate;
                    if (tardeTrabDetail.Count > 0)
                    {
                        foreach (SettledTradeTransactionQueueArbitrage type in tardeTrabDetail)
                        {
                            decimal price = 0;
                            if (type.TrnType == Convert.ToInt16(enTrnType.Buy_Trade))
                                price = type.BidPrice;
                            else if (type.TrnType == Convert.ToInt16(enTrnType.Sell_Trade))
                                price = type.AskPrice;

                            if (price > High24Hr)
                                High24Hr = price;
                            if (price < Low24Hr)
                                Low24Hr = price;
                        }
                    }

                    //Calculate High Low Data For Week
                    var WeekData = _frontTrnRepository.GetHighLowValueArbitrage(PairId, -7);
                    if (WeekData != null)
                    {
                        WeekHigh = WeekData.HighPrice;
                        WeekLow = WeekData.LowPrice;
                    }

                    //Calculate High Low Data For 52Week
                    var Week52Data = _frontTrnRepository.GetHighLowValueArbitrage(PairId, -365);
                    if (Week52Data != null)
                    {
                        Week52High = Week52Data.HighPrice;
                        Week52Low = Week52Data.LowPrice;
                    }
                    var pairData = _tradePairStasticsArbitrage.GetSingle(x => x.PairId == PairId);
                    HelperForLog.WriteLogIntoFile(MethodName + " #GetLastPairData# #PairId : " + PairId + " #TrnNo# : " + TrnNo + " UpDownBit : " + pairData.UpDownBit + " LTP : " + pairData.LTP + " Last TrnDate : " + pairData.TranDate, "Object Data : ");

                    if (CurrentRate > pairData.High24Hr) //komal 13-11-2018 Change code sequence cos got 0 every time
                        UpDownBit = 1;
                    else if (CurrentRate < pairData.Low24Hr)
                        UpDownBit = 0;
                    else
                    {
                        if (CurrentRate < pairData.LTP)
                        {
                            UpDownBit = 0;
                        }
                        else if (CurrentRate > pairData.LTP)
                        {
                            UpDownBit = 1;
                        }
                        else if (CurrentRate == pairData.LTP)//komal 13-11-2018 if no change then set as it is
                        {
                            UpDownBit = pairData.UpDownBit;
                        }
                    }

                    Task.Run(() => HelperForLog.WriteLogIntoFile(MethodName + " #UpdatePairLTPStart# " + " UpDownBit : " + UpDownBit + " TrnNo : " + TrnNo + " CurrentRate : " + pairData.CurrentRate, "Object Data : "));
                    pairData.ChangePer24 = ChangePer;
                    pairData.ChangeVol24 = Volume24;
                    pairData.High24Hr = High24Hr;
                    pairData.Low24Hr = Low24Hr;
                    pairData.LTP = CurrentRate;
                    pairData.CurrentRate = CurrentRate;
                    pairData.HighWeek = WeekHigh;
                    pairData.LowWeek = WeekLow;
                    pairData.High52Week = Week52High;
                    pairData.Low52Week = Week52Low;
                    pairData.TranDate = TranDate;
                    pairData.UpDownBit = UpDownBit;
                    pairData.ChangeValue = ChangeValue;
                    _tradePairStasticsArbitrage.Update(pairData);
                    Task.Run(() => HelperForLog.WriteLogIntoFile(MethodName + " #UpdatePairLTEND# " + " UpDownBit : " + UpDownBit + " TrnNo : " + TrnNo + " CurrentRate : " + pairData.CurrentRate, "Object Data : "));

                    //komal 16-07-2019 add local LTP change to CryptoWatcher
                    ArbitrageLTPCls ArbitrageLTP = new ArbitrageLTPCls();
                    ArbitrageLTP.ChangePer = pairData.ChangePer24;
                    ArbitrageLTP.Fees = 0;
                    ArbitrageLTP.LpType = (short)enAppType.COINTTRADINGLocal;
                    ArbitrageLTP.Pair = _trnMasterConfiguration.GetTradePairMasterArbitrage().Where(e => e.Id == PairId).FirstOrDefault().PairName;
                    ArbitrageLTP.PairID = PairId;
                    ArbitrageLTP.Price = pairData.CurrentRate;
                    ArbitrageLTP.Volume = pairData.ChangeVol24;
                    ArbitrageLTP.UpDownBit = pairData.UpDownBit;
                    var Res = _frontTrnRepository.UpdateLTPDataArbitrage(ArbitrageLTP);
                    if (Res == null)
                        _frontTrnRepository.InsertLTPDataArbitrage(ArbitrageLTP);
                    HelperForLog.WriteLogIntoFile(MethodName , " LTP Update To CryptoWatcher Call #PairId : " + ArbitrageLTP.PairID + " #TrnNo# : " + TrnNo + " UpDownBit : " + ArbitrageLTP.UpDownBit + " LTP : " + ArbitrageLTP.Price + " Volume : " + ArbitrageLTP.Volume);


                    //komal 16-11-2018 Set Volume Data avoid DB call
                    VolumeDataRespose VolumeData = new VolumeDataRespose();
                    VolumeData.PairId = PairId;
                    //VolumeData.PairName = _tradeMasterRepository.GetById(PairId).PairName;
                    VolumeData.PairName = _trnMasterConfiguration.GetTradePairMasterArbitrage().Where(e => e.Id == PairId).FirstOrDefault().PairName;
                    VolumeData.Currentrate = pairData.CurrentRate;
                    VolumeData.ChangePer = pairData.ChangePer24;
                    VolumeData.Volume24 = pairData.ChangeVol24;
                    VolumeData.High24Hr = pairData.High24Hr;
                    VolumeData.Low24Hr = pairData.Low24Hr;
                    VolumeData.HighWeek = pairData.HighWeek;
                    VolumeData.LowWeek = pairData.LowWeek;
                    VolumeData.High52Week = pairData.High52Week;
                    VolumeData.Low52Week = pairData.Low52Week;
                    VolumeData.UpDownBit = pairData.UpDownBit;


                    //komal 16-11-2018 Set MArket Data avoid DB call
                    MarketCapData MarketData = new MarketCapData();
                    MarketData.Change24 = pairData.High24Hr - pairData.Low24Hr;
                    MarketData.ChangePer = pairData.ChangePer24;
                    MarketData.High24 = pairData.High24Hr;
                    MarketData.Low24 = pairData.Low24Hr;
                    MarketData.LastPrice = pairData.LTP;
                    MarketData.Volume24 = pairData.ChangeVol24;

                    Task.Run(() => _signalRService.OnVolumeChangeArbitrage(VolumeData, MarketData, UserID));
                    //Uday 25-12-2018  SignalR Call For Market Ticker
                    var PairDetailMarketTicker = _tradeDetailRepositoryArbitrage.GetSingle(x => x.PairId == PairId);
                    if (PairDetailMarketTicker != null)
                    {
                        if (PairDetailMarketTicker.IsMarketTicker == 1)
                        {
                            List<VolumeDataRespose> MarketTickerData = new List<VolumeDataRespose>();
                            MarketTickerData.Add(VolumeData);
                            Task.Run(() => HelperForLog.WriteLogIntoFile(MethodName + " #MarketTickerSocket# #PairId# : " + PairId + " #TrnNo# : " + TrnNo, "FrontService", "Object Data : "));
                            Task.Run(() => _signalRService.MarketTickerArbitrage(MarketTickerData, UserID, "", IsMargin: 0));
                        }
                    }

                    

                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetPairAdditionalValArbitrage ##TrnNo:" + TrnNo, "FrontTrnService", ex);
            }
        }

        public List<GetGraphDetailInfo> GetGraphDetail(long PairId, int IntervalTime, string IntervalData, short IsMargin = 0)//Rita 22-2-19 for Margin Trading Data bit
        {
            try
            {
                IOrderedEnumerable<GetGraphDetailInfo> list;
                if (IsMargin == 1)
                    list = _frontTrnRepository.GetGraphDataMargin(PairId, IntervalTime, IntervalData, _basePage.UTC_To_IST()).OrderBy(x => x.DataDate);
                else
                    list = _frontTrnRepository.GetGraphData(PairId, IntervalTime, IntervalData, _basePage.UTC_To_IST()).OrderBy(x => x.DataDate);
                return list.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool addSetteledTradeTransaction(SettledTradeTransactionQueue queueData)
        {
            try
            {
                var model = _settelTradeTranQueue.Add(queueData);
                if (model.Id != 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Trading Data Method

        //public GetBuySellBookResponse GetBuyerBook(long id, short IsMargin = 0)//Rita 22-2-19 for Margin Trading Data bit
        //{
        //    GetBuySellBookResponse _Res = new GetBuySellBookResponse();
        //    try
        //    {
        //        List<GetBuySellBook> list;
        //        if (IsMargin == 1)
        //            list = _frontTrnRepository.GetBuyerBookMargin(id, Price: -1);
        //        else
        //            list = _frontTrnRepository.GetBuyerBook(id, Price: -1);
        //        if(list.Count ==0)
        //        {
        //            _Res.ReturnCode = enResponseCode.Fail;
        //            _Res.ErrorCode = enErrorCode.NoDataFound;
        //            _Res.ReturnMsg = "Fail";
        //            _Res.response = list;
        //            return _Res;
        //        }
        //        _Res.ReturnCode = enResponseCode.Success;
        //        _Res.ErrorCode = enErrorCode.Success;
        //        _Res.ReturnMsg = "Success";
        //        _Res.response = list;
        //        return _Res;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}
        //public GetBuySellBookResponse GetSellerBook(long id, short IsMargin = 0)//Rita 22-2-19 for Margin Trading Data bit
        //{
        //    GetBuySellBookResponse _Res = new GetBuySellBookResponse();
        //    try
        //    {
        //        List<GetBuySellBook> list;
        //        if (IsMargin == 1)
        //            list = _frontTrnRepository.GetSellerBookMargin(id, Price: -1);
        //        else
        //            list = _frontTrnRepository.GetSellerBook(id, Price: -1);
        //        if (list.Count == 0)
        //        {
        //            _Res.ReturnCode = enResponseCode.Fail;
        //            _Res.ErrorCode = enErrorCode.NoDataFound;
        //            _Res.ReturnMsg = "Fail";
        //            _Res.response = list;
        //            return _Res;
        //        }
        //        _Res.ReturnCode = enResponseCode.Success;
        //        _Res.ErrorCode = enErrorCode.Success;
        //        _Res.ReturnMsg = "Success";
        //        _Res.response = list;
        //        return _Res;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        public GetBuySellBookResponse GetBuyerSellerBookV1(long id,short TrnType, short IsMargin = 0)//Rita 22-2-19 for Margin Trading Data bit
        {
            GetBuySellBookResponse _Res = new GetBuySellBookResponse();
            try
            {
                List<GetBuySellBook> list = new List<GetBuySellBook>();
                if (TrnType == 4)
                {
                    if (IsMargin == 1)
                        list = _frontTrnRepository.GetBuyerBookMargin(id, Price: -1);
                    else
                        list = _frontTrnRepository.GetBuyerBook(id, Price: -1);
                }
                else if(TrnType == 5)
                {
                    if (IsMargin == 1)
                        list = _frontTrnRepository.GetSellerBookMargin(id, Price: -1);
                    else
                        list = _frontTrnRepository.GetSellerBook(id, Price: -1);
                }
                if (list.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnMsg = "Fail";
                    _Res.response = list;
                    return _Res;
                }
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                _Res.response = list;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<BasePairResponse> GetTradePairAsset()
        {
            List<BasePairResponse> responsedata;
            try
            {
                responsedata = new List<BasePairResponse>();
                IEnumerable<Market> basePairData = _trnMasterConfiguration.GetMarket().Where(x => x.Status == 1).OrderBy(x => x.Priority);//rita 23-2-19 taken from cache Implemented
                var TradePairList = _frontTrnRepository.GetTradePairAsset();
                if (basePairData != null)
                {
                    foreach (var bpair in basePairData)
                    {
                        BasePairResponse basePair = new BasePairResponse();
                        //Rita 01-05-19 added priority
                        var pairData = TradePairList.Where(x => x.BaseId == bpair.ServiceID).OrderBy(x => x.Priority);
                        if (pairData.Count() > 0)
                        {
                            basePair.BaseCurrencyId = pairData.FirstOrDefault().BaseId;
                            basePair.BaseCurrencyName = pairData.FirstOrDefault().BaseName;
                            basePair.Abbrevation = pairData.FirstOrDefault().BaseCode;

                            List<TradePairRespose> pairList = new List<TradePairRespose>();
                            pairList = pairData.Select(pair => new TradePairRespose
                            {
                                PairId = pair.PairId,
                                Pairname = pair.Pairname,
                                Currentrate = pair.Currentrate,
                                BuyFees = pair.BuyFees,
                                SellFees = pair.SellFees,
                                ChildCurrency = pair.ChildCurrency,
                                Abbrevation = pair.Abbrevation,
                                ChangePer = pair.ChangePer,
                                Volume = pair.Volume,
                                High24Hr = pair.High24Hr,
                                Low24Hr = pair.Low24Hr,
                                HighWeek = pair.HighWeek,
                                LowWeek = pair.LowWeek,
                                High52Week = pair.High52Week,
                                Low52Week = pair.Low52Week,
                                UpDownBit = pair.UpDownBit,
                                AmtLength = pair.AmtLength,
                                PriceLength = pair.PriceLength,
                                QtyLength = pair.QtyLength,
                                PairPercentage = pair.PairPercentage
                            }).ToList();

                            basePair.PairList = pairList;
                            responsedata.Add(basePair);
                        }
                    }
                    return responsedata;
                }
                else
                {
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 23-2-19 for margin trading
        public List<BasePairResponse> GetTradePairAssetMargin()
        {
            List<BasePairResponse> responsedata;
            try
            {
                responsedata = new List<BasePairResponse>();
                //Rita 01-05-19 added priority,also added status condition
                IEnumerable<MarketMargin> basePairData = _trnMasterConfiguration.GetMarketMargin().Where(x => x.Status == 1).OrderBy(x => x.Priority);

                var TradePairList = _frontTrnRepository.GetTradePairAssetMargin();

                if (basePairData != null)
                {
                    foreach (var bpair in basePairData)
                    {
                        BasePairResponse basePair = new BasePairResponse();
                        //Rita 01-05-19 added priority
                        var pairData = TradePairList.Where(x => x.BaseId == bpair.ServiceID).OrderBy(x => x.Priority);
                        if (pairData.Count() > 0)
                        {
                            basePair.BaseCurrencyId = pairData.FirstOrDefault().BaseId;
                            basePair.BaseCurrencyName = pairData.FirstOrDefault().BaseName;
                            basePair.Abbrevation = pairData.FirstOrDefault().BaseCode;

                            List<TradePairRespose> pairList = new List<TradePairRespose>();
                            pairList = pairData.Select(pair => new TradePairRespose
                            {
                                PairId = pair.PairId,
                                Pairname = pair.Pairname,
                                Currentrate = pair.Currentrate,
                                BuyFees = pair.BuyFees,
                                SellFees = pair.SellFees,
                                ChildCurrency = pair.ChildCurrency,
                                Abbrevation = pair.Abbrevation,
                                ChangePer = pair.ChangePer,
                                Volume = pair.Volume,
                                High24Hr = pair.High24Hr,
                                Low24Hr = pair.Low24Hr,
                                HighWeek = pair.HighWeek,
                                LowWeek = pair.LowWeek,
                                High52Week = pair.High52Week,
                                Low52Week = pair.Low52Week,
                                UpDownBit = pair.UpDownBit,
                            }).ToList();

                            basePair.PairList = pairList;
                            responsedata.Add(basePair);
                        }
                    }
                    return responsedata;
                }
                else
                {
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        
        public List<VolumeDataRespose> GetVolumeData(long BasePairId, short IsMargin = 0)//Rita 22-2-19 for Margin Trading Data bit
        {
            List<VolumeDataRespose> responsedata;
            try
            {
                responsedata = new List<VolumeDataRespose>();
                List<TradePairTableResponse> TradePairList;
                if (IsMargin == 1)//Rita 22-2-19 for Margin Trading Data bit
                    TradePairList = _frontTrnRepository.GetTradePairAssetMargin(BasePairId);
                else
                    TradePairList = _frontTrnRepository.GetTradePairAsset(BasePairId);

                if (TradePairList != null && TradePairList.Count() > 0)
                {
                    foreach (var pmdata in TradePairList)
                    {
                        VolumeDataRespose volumedata = new VolumeDataRespose();
                        volumedata.PairId = pmdata.PairId;
                        volumedata.PairName = pmdata.Pairname;
                        volumedata.Currentrate = pmdata.Currentrate;
                        volumedata.ChangePer = pmdata.ChangePer;
                        volumedata.Volume24 = pmdata.Volume;
                        volumedata.High24Hr = pmdata.High24Hr;
                        volumedata.Low24Hr = pmdata.Low24Hr;
                        volumedata.HighWeek = pmdata.HighWeek;
                        volumedata.LowWeek = pmdata.LowWeek;
                        volumedata.High52Week = pmdata.High52Week;
                        volumedata.Low52Week = pmdata.Low52Week;
                        volumedata.UpDownBit = pmdata.UpDownBit;
                        responsedata.Add(volumedata);
                    }
                    return responsedata;
                }
                else
                {
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public VolumeDataRespose GetVolumeDataByPair(long PairId)
        {
            VolumeDataRespose responsedata;
            try
            {
                responsedata = new VolumeDataRespose();
                var pairMasterData = _tradeMasterRepository.GetActiveById(PairId);
                if (pairMasterData != null)
                {
                    var pairDetailData = _tradeDetailRepository.GetSingle(x => x.PairId == pairMasterData.Id);
                    var pairStastics = _tradePairStastics.GetSingle(x => x.PairId == pairMasterData.Id);
                    responsedata.PairId = pairMasterData.Id;
                    responsedata.PairName = pairMasterData.PairName;
                    responsedata.Currentrate = pairStastics.CurrentRate;
                    responsedata.ChangePer = pairStastics.ChangePer24;
                    responsedata.Volume24 = pairStastics.ChangeVol24;
                    responsedata.High24Hr = pairStastics.High24Hr;
                    responsedata.Low24Hr = pairStastics.Low24Hr;
                    responsedata.HighWeek = pairStastics.HighWeek;
                    responsedata.LowWeek = pairStastics.LowWeek;
                    responsedata.High52Week = pairStastics.High52Week;
                    responsedata.Low52Week = pairStastics.Low52Week;
                    responsedata.UpDownBit = pairStastics.UpDownBit;
                    return responsedata;
                }
                else
                    return responsedata;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 23-2-19 for margin trading
        public VolumeDataRespose GetVolumeDataByPairMargin(long PairId)
        {
            VolumeDataRespose responsedata;
            try
            {
                responsedata = new VolumeDataRespose();
                TradePairMasterMargin pairMasterData = _trnMasterConfiguration.GetTradePairMasterMargin().Where(e => e.Id == PairId && e.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();

                if (pairMasterData != null)
                {
                    TradePairDetailMargin pairDetailData = _trnMasterConfiguration.GetTradePairDetailMargin().Where(x => x.PairId == pairMasterData.Id).FirstOrDefault();
                    TradePairStasticsMargin pairStastics = _tradePairStasticsMargin.GetSingle(x => x.PairId == pairMasterData.Id);
                    responsedata.PairId = pairMasterData.Id;
                    responsedata.PairName = pairMasterData.PairName;
                    responsedata.Currentrate = pairStastics.CurrentRate;
                    responsedata.ChangePer = pairStastics.ChangePer24;
                    responsedata.Volume24 = pairStastics.ChangeVol24;
                    responsedata.High24Hr = pairStastics.High24Hr;
                    responsedata.Low24Hr = pairStastics.Low24Hr;
                    responsedata.HighWeek = pairStastics.HighWeek;
                    responsedata.LowWeek = pairStastics.LowWeek;
                    responsedata.High52Week = pairStastics.High52Week;
                    responsedata.Low52Week = pairStastics.Low52Week;
                    responsedata.UpDownBit = pairStastics.UpDownBit;
                    return responsedata;
                }
                else
                    return responsedata;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public PairRatesResponse GetPairRates(long PairId, short IsMargin = 0)//Rita 23-2-19 for Margin Trading Data bit
        {
            try
            {
                PairRatesResponse responseData = new PairRatesResponse();
                if (IsMargin == 1)
                    responseData = _frontTrnRepository.GetPairRatesMargin(PairId);
                else
                    responseData = _frontTrnRepository.GetPairRates(PairId);

                return responseData;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public GetTradePairByName GetTradePairByName(long id)
        {
            GetTradePairByName responsedata = new GetTradePairByName();
            try
            {
                var pairMasterData = _tradeMasterRepository.GetById(id);
                if (pairMasterData != null)
                {
                    var pairDetailData = _tradeDetailRepository.GetSingle(x => x.PairId == pairMasterData.Id);
                    var baseService = _serviceMasterRepository.GetSingle(x => x.Id == pairMasterData.BaseCurrencyId);
                    var chidService = _serviceMasterRepository.GetSingle(x => x.Id == pairMasterData.SecondaryCurrencyId);
                    var pairStastics = _tradePairStastics.GetSingle(x => x.PairId == pairMasterData.Id);
                    responsedata.PairId = pairMasterData.Id;
                    responsedata.Pairname = pairMasterData.PairName;
                    responsedata.Currentrate = pairStastics.CurrentRate;
                    responsedata.BuyFees = pairDetailData.BuyFees;
                    responsedata.SellFees = pairDetailData.SellFees;
                    responsedata.ChildCurrency = chidService.Name;
                    responsedata.Abbrevation = chidService.SMSCode;
                    responsedata.ChangePer = pairStastics.ChangePer24;
                    responsedata.Volume = pairStastics.ChangeVol24;
                    responsedata.High24Hr = pairStastics.High24Hr;
                    responsedata.Low24Hr = pairStastics.Low24Hr;
                    responsedata.HighWeek = pairStastics.HighWeek;
                    responsedata.LowWeek = pairStastics.LowWeek;
                    responsedata.High52Week = pairStastics.High52Week;
                    responsedata.Low52Week = pairStastics.Low52Week;
                    responsedata.UpDownBit = pairStastics.UpDownBit;
                    responsedata.BaseCurrencyId = baseService.Id;
                    responsedata.BaseCurrencyName = baseService.Name;
                    responsedata.BaseAbbrevation = baseService.SMSCode;
                }
                return responsedata;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetTradePairByName GetTradePairByNameMargin(long id)
        {
            GetTradePairByName responsedata = new GetTradePairByName();
            try
            {
                TradePairMasterMargin pairMasterData = _trnMasterConfiguration.GetTradePairMasterMargin().Where(e => e.Id == id).FirstOrDefault();
                if (pairMasterData != null)
                {
                    TradePairDetailMargin pairDetailData = _trnMasterConfiguration.GetTradePairDetailMargin().Where(x => x.PairId == pairMasterData.Id).FirstOrDefault();
                    ServiceMasterMargin baseService = _trnMasterConfiguration.GetServicesMargin().Where(x => x.Id == pairMasterData.BaseCurrencyId).FirstOrDefault();
                    ServiceMasterMargin chidService = _trnMasterConfiguration.GetServicesMargin().Where(x => x.Id == pairMasterData.SecondaryCurrencyId).FirstOrDefault();
                    TradePairStasticsMargin pairStastics = _tradePairStasticsMargin.GetSingle(x => x.PairId == pairMasterData.Id);

                    responsedata.PairId = pairMasterData.Id;
                    responsedata.Pairname = pairMasterData.PairName;
                    responsedata.Currentrate = pairStastics.CurrentRate;
                    responsedata.BuyFees = pairDetailData.BuyFees;
                    responsedata.SellFees = pairDetailData.SellFees;
                    responsedata.ChildCurrency = chidService.Name;
                    responsedata.Abbrevation = chidService.SMSCode;
                    responsedata.ChangePer = pairStastics.ChangePer24;
                    responsedata.Volume = pairStastics.ChangeVol24;
                    responsedata.High24Hr = pairStastics.High24Hr;
                    responsedata.Low24Hr = pairStastics.Low24Hr;
                    responsedata.HighWeek = pairStastics.HighWeek;
                    responsedata.LowWeek = pairStastics.LowWeek;
                    responsedata.High52Week = pairStastics.High52Week;
                    responsedata.Low52Week = pairStastics.Low52Week;
                    responsedata.UpDownBit = pairStastics.UpDownBit;
                    responsedata.BaseCurrencyId = baseService.Id;
                    responsedata.BaseCurrencyName = baseService.Name;
                    responsedata.BaseAbbrevation = baseService.SMSCode;
                }
                return responsedata;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public MarketCapData GetMarketCap(long PairId)
        {
            try
            {
                MarketCapData dataRes = new MarketCapData();
                VolumeDataRespose res = new VolumeDataRespose();
                var pairMasterData = _tradeMasterRepository.GetById(PairId);
                if (pairMasterData != null)
                {
                    var pairStastics = _tradePairStastics.GetSingle(x => x.PairId == pairMasterData.Id);
                    if (pairStastics != null)
                    {
                        dataRes.Change24 = pairStastics.High24Hr - pairStastics.Low24Hr;
                        dataRes.ChangePer = pairStastics.ChangePer24;
                        dataRes.High24 = pairStastics.High24Hr;
                        dataRes.Low24 = pairStastics.Low24Hr;
                        dataRes.LastPrice = pairStastics.LTP;
                        dataRes.Volume24 = pairStastics.ChangeVol24;
                    }
                }
                return dataRes;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public MarketCapData GetMarketCapMargin(long PairId)
        {
            try
            {
                MarketCapData dataRes = new MarketCapData();
                VolumeDataRespose res = new VolumeDataRespose();
                TradePairMasterMargin pairMasterData = _trnMasterConfiguration.GetTradePairMasterMargin().Where(e => e.Id == PairId).FirstOrDefault();
                if (pairMasterData != null)
                {
                    var pairStastics = _tradePairStasticsMargin.GetSingle(x => x.PairId == pairMasterData.Id);
                    if (pairStastics != null)
                    {
                        dataRes.Change24 = pairStastics.High24Hr - pairStastics.Low24Hr;
                        dataRes.ChangePer = pairStastics.ChangePer24;
                        dataRes.High24 = pairStastics.High24Hr;
                        dataRes.Low24 = pairStastics.Low24Hr;
                        dataRes.LastPrice = pairStastics.LTP;
                        dataRes.Volume24 = pairStastics.ChangeVol24;
                    }
                }
                return dataRes;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long GetPairIdByName(string pair, short IsMargin = 0)//Rita 22-2-19 for Margin Trading Data bit
        {
            long PairID = 0;
            try
            {
                if (IsMargin == 1)
                {
                    TradePairMasterMargin TradePairMarginObj = _trnMasterConfiguration.GetTradePairMasterMargin().Where(p => p.PairName == pair && p.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                    if (TradePairMarginObj == null)
                        return 0;
                    PairID = TradePairMarginObj.Id;
                }
                else
                {
                    TradePairMaster TradePairObj = _trnMasterConfiguration.GetTradePairMaster().Where(p => p.PairName == pair && p.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                    if (TradePairObj == null)
                        return 0;
                    PairID = TradePairObj.Id;
                }
                return PairID;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public long GetBasePairIdByName(string BasePair, short IsMargin = 0)//Rita 22-2-19 for Margin Trading Data bit
        {
            long BasePairID = 0;
            try
            {

                if (IsMargin == 1)
                {
                    ServiceMasterMargin ServiceMasterMarginObj = _trnMasterConfiguration.GetServicesMargin().Where(p => p.SMSCode == BasePair).FirstOrDefault();
                    if (ServiceMasterMarginObj == null)
                        return 0;
                    BasePairID = ServiceMasterMarginObj.Id;
                }
                else
                {
                    ServiceMaster ServiceMasterObj = _trnMasterConfiguration.GetServices().Where(p => p.SMSCode == BasePair).FirstOrDefault();
                    if (ServiceMasterObj == null)
                        return 0;
                    BasePairID = ServiceMasterObj.Id;
                }
                return BasePairID;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<VolumeDataRespose> GetMarketTicker(short IsMargin = 0)//Rita 23-2-19 for Margin Trading Data bit
        {
            try
            {
                List<VolumeDataRespose> list;
                if (IsMargin == 1)
                    list = _backOfficeTrnRepository.GetUpdatedMarketTickerMargin();
                else
                    list = _backOfficeTrnRepository.GetUpdatedMarketTicker();

                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public int GetMarketTickerSignalR(short IsMargin = 0)//Rita 23-2-19 for Margin Trading Data bit
        {
            try
            {
                List<VolumeDataRespose> list;
                if (IsMargin == 1)
                    list = _backOfficeTrnRepository.GetUpdatedMarketTickerMargin();
                else
                    list = _backOfficeTrnRepository.GetUpdatedMarketTicker();

                if (list.Count != 0)
                {
                    _signalRService.MarketTicker(list, "");
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public GetBuySellMarketBook GetMarketDepthChart(long PairId)
        {
            GetBuySellMarketBook Response = new GetBuySellMarketBook();
            try
            {
                decimal UpdatedAmount = 0;
                var BuyerBookData = _frontTrnRepository.GetBuyerBook(PairId, Price: -1); // Uday 08-01-2019 Change Order For Data From Ascending to Descending
                var SellerBookData = _frontTrnRepository.GetSellerBook(PairId, Price: -1);

                List<GetBuySellMarketBookData> BuySellBookData = new List<GetBuySellMarketBookData>();
                if (BuyerBookData.Count < 0 && SellerBookData.Count < 0) // if both have no data than give error no data found
                {
                    return null;
                }
                else
                {
                    //Buyer Book Calculation
                    BuySellBookData = new List<GetBuySellMarketBookData>();
                    if (BuyerBookData.Count > 0)  //Bid array calculation for market depth chart
                    {
                        foreach (var BuyerData in BuyerBookData)
                        {
                            GetBuySellMarketBookData Data = new GetBuySellMarketBookData();
                            UpdatedAmount = UpdatedAmount + BuyerData.Amount;
                            Data.Price = BuyerData.Price;
                            Data.Amount = UpdatedAmount;
                            BuySellBookData.Add(Data); //Add buyer book object after market depth calculation.
                        }
                    }
                    Response.Bid = BuySellBookData;
                    //Seller Book Calculation
                    UpdatedAmount = 0;  // For Seller Book reinitialize the amount.
                    BuySellBookData = new List<GetBuySellMarketBookData>();
                    if (SellerBookData.Count > 0)  //Bid array calculation for market depth chart
                    {
                        foreach (var SellerData in SellerBookData)
                        {
                            GetBuySellMarketBookData Data = new GetBuySellMarketBookData();
                            UpdatedAmount = UpdatedAmount + SellerData.Amount;
                            Data.Price = SellerData.Price;
                            Data.Amount = UpdatedAmount;
                            BuySellBookData.Add(Data); //Add buyer book object after market depth calculation.
                        }
                    }
                    Response.Ask = BuySellBookData;
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 23-2-19 for Margin Trading Data bit
        public GetBuySellMarketBook GetMarketDepthChartMargin(long PairId)
        {
            GetBuySellMarketBook Response = new GetBuySellMarketBook();
            try
            {
                decimal UpdatedAmount = 0;
                var BuyerBookData = _frontTrnRepository.GetBuyerBookMargin(PairId, Price: -1); // Uday 08-01-2019 Change Order For Data From Ascending to Descending
                var SellerBookData = _frontTrnRepository.GetSellerBookMargin(PairId, Price: -1);
                List<GetBuySellMarketBookData> BuySellBookData = new List<GetBuySellMarketBookData>();
                if (BuyerBookData.Count < 0 && SellerBookData.Count < 0) // if both have no data than give error no data found
                {
                    return null;
                }
                else
                {
                    //Buyer Book Calculation
                    BuySellBookData = new List<GetBuySellMarketBookData>();
                    if (BuyerBookData.Count > 0)  //Bid array calculation for market depth chart
                    {
                        foreach (var BuyerData in BuyerBookData)
                        {
                            GetBuySellMarketBookData Data = new GetBuySellMarketBookData();
                            UpdatedAmount = UpdatedAmount + BuyerData.Amount;

                            Data.Price = BuyerData.Price;
                            Data.Amount = UpdatedAmount;

                            BuySellBookData.Add(Data); //Add buyer book object after market depth calculation.
                        }
                    }
                    Response.Bid = BuySellBookData;

                    //Seller Book Calculation
                    UpdatedAmount = 0;  // For Seller Book reinitialize the amount.
                    BuySellBookData = new List<GetBuySellMarketBookData>();
                    if (SellerBookData.Count > 0)  //Bid array calculation for market depth chart
                    {
                        foreach (var SellerData in SellerBookData)
                        {
                            GetBuySellMarketBookData Data = new GetBuySellMarketBookData();
                            UpdatedAmount = UpdatedAmount + SellerData.Amount;
                            Data.Price = SellerData.Price;
                            Data.Amount = UpdatedAmount;
                            BuySellBookData.Add(Data); //Add buyer book object after market depth calculation.
                        }
                    }
                    Response.Ask = BuySellBookData;
                }

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<HistoricalPerformanceYear> GetHistoricalPerformance(long UserId)
        {
            try
            {
                string[] FullMonths = { "null", "January", "February", "March", "April", "May", "June",
                                            "July", "August", "September", "October", "November", "December" };

                int StartingMonth = 0, EndingMonth = 0;
                decimal Value1 = 0, Value2 = 0, Value3 = 0, Value4 = 0, YearlyPerformance = 0;
                decimal MonthStartValue = 0, MonthEndValue = 0, DepositValue = 0, WithdrwalValue = 0;
                var userAvailable = _userService.GetUserById(UserId);

                if (userAvailable == false)
                {
                    return null;
                }

                var JoiningDate = _userService.GetUserJoiningDate(UserId);
                var CurrentDate = Helpers.UTC_To_IST();
                List<HistoricalPerformanceYear> Response = new List<HistoricalPerformanceYear>();

                for (short i = Convert.ToInt16(CurrentDate.Year); i >= Convert.ToInt16(JoiningDate.Year); i--)
                {
                    HistoricalPerformanceYear YearData = new HistoricalPerformanceYear();
                    decimal[] Monthdata = new decimal[12];
                    YearlyPerformance = 1;StartingMonth = 1;EndingMonth = 12;

                    for (int j = StartingMonth; j <= EndingMonth; j++)
                    {
                        var MonthStasticsData = _walletService.GetMonthwiseWalletStatistics(UserId, Convert.ToInt16(j), i);
                        if (MonthStasticsData.ReturnCode == 0)
                        {
                            var MonthTranData = MonthStasticsData.Balances.TranAmount;
                            MonthStartValue = MonthStasticsData.Balances.StartingBalance;
                            MonthEndValue = MonthStasticsData.Balances.EndingBalance;

                            if (MonthTranData[0].TrnTypeId == 9) // Withdrwal
                                WithdrwalValue = MonthTranData[0].TotalAmount;
                            else // Deposit
                                DepositValue = MonthTranData[0].TotalAmount;


                            if (MonthTranData[1].TrnTypeId == 9) // Withdrwal
                                WithdrwalValue = MonthTranData[1].TotalAmount;
                            else // Deposit
                                DepositValue = MonthTranData[1].TotalAmount;

                            //Calculate Performance Value
                            Value1 = (MonthEndValue + WithdrwalValue);
                            Value2 = (MonthStartValue + DepositValue);
                            Value3 = Value1 - Value2;

                            if (Value2 != 0)
                            {
                                Monthdata[j - 1] = Helpers.DoRoundForTrading(((Value3 / Value2) * 100), 3);
                                Value4 = Helpers.DoRoundForTrading(((Value3 / Value2) * 100), 3);
                            }
                            else
                            {
                                Monthdata[j - 1] = 0;
                                Value4 = 0;
                            }
                        }
                        else
                            Monthdata[j - 1] = 0;
                        YearlyPerformance *= (1 + Value4);
                    }

                    YearData.Year = i;
                    if (YearlyPerformance == 1)
                        YearData.Total = 0;
                    else
                        YearData.Total = Helpers.DoRoundForTrading(YearlyPerformance, 3);
                    YearData.Data = Monthdata;
                    Response.Add(YearData);
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<HistoricalPerformanceYear> GetHistoricalPerformanceV1(long UserId)
        {
            try
            {
                string[] FullMonths = { "null", "January", "February", "March", "April", "May", "June",
                                            "July", "August", "September", "October", "November", "December" };

                var userAvailable = _userService.GetUserById(UserId);
                if (userAvailable == false)
                {
                    return null;
                }

                var JoiningDate = _userService.GetUserJoiningDate(UserId);
                var CurrentDate = Helpers.UTC_To_IST();
                List<HistoricalPerformanceYear> Response = new List<HistoricalPerformanceYear>();

                for (short i = Convert.ToInt16(CurrentDate.Year); i >= Convert.ToInt16(JoiningDate.Year); i--)
                {
                    HistoricalPerformanceYear YearData = new HistoricalPerformanceYear();
                    YearData.Year = i;
                    decimal[] Monthdata = new decimal[13];
                    var monthObjs = _walletRepository.GetHistoricalPerformanceYearWise(UserId, i);
                    foreach (var m in monthObjs)
                        Monthdata[m.AutoNo] = m.ProfitPer;

                    Monthdata[0] = 0;
                    YearData.Data = Monthdata;
                    Response.Add(YearData);
                }
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public GetWithdrawalTransactionData GetWithdrawalTransaction(string RefId)
        {
            try
            {
                GetWithdrawalTransactionData Response = new GetWithdrawalTransactionData();
                var Data = _transactionQueue.GetSingle(x => x.GUID.ToString() == RefId);
                if (Data != null)
                {
                    Response.TrnNo = Data.Id;
                    Response.TransactionAddress = Data.TransactionAccount;
                    Response.TrnDate = Data.TrnDate;
                    Response.Amount = Data.Amount;
                    Response.Currency = Data.SMSCode;
                    Response.Status = Data.Status;
                    Response.Fee = Convert.ToDecimal(Data.ChargeRs);
                    Response.IsVerified = Data.IsVerified;
                    Response.CurrencyName = Data.ChargeCurrency;

                    if (Data.Status == 4 || Data.Status == 6)
                        Response.StatusMsg = ((EnWithdrwalConfirmationStatus)Data.IsVerified).ToString();
                    else
                    {
                        if (Response.IsVerified == 9)
                        {
                            Response.StatusMsg = ((EnWithdrwalConfirmationStatus)9).ToString();
                        }
                        else
                        {
                            if (Data.Status == 2)
                                Response.StatusMsg = "ProviderFail";
                            else
                                Response.StatusMsg = ((enTransactionStatus)Data.Status).ToString();
                        }
                    }
                    return Response;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region FavPair Methods

        public int AddToFavouritePair(long PairId, long UserId)
        {
            try
            {
                var pairData = _tradeMasterRepository.GetById(PairId);
                if (pairData == null)
                {
                    return 2;
                }
                var favouritePair = _favouritePairRepository.GetSingle(x => x.PairId == PairId && x.UserId == UserId);
                if (favouritePair == null)
                {
                    //Add With First Time
                    favouritePair = new FavouritePair()
                    {
                        PairId = PairId,
                        UserId = UserId,
                        Status = 1,
                        CreatedBy = UserId,
                        CreatedDate = _basePage.UTC_To_IST()
                    };
                    favouritePair = _favouritePairRepository.Add(favouritePair);
                }
                else if (favouritePair != null)
                {
                    if (favouritePair.Status == 1)
                    {
                        return 1;  // already added as favourite pair
                    }
                    else if (favouritePair.Status == 9)
                    {
                        favouritePair.Status = 1;
                        _favouritePairRepository.Update(favouritePair);
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public int RemoveFromFavouritePair(long PairId, long UserId)
        {
            try
            {
                var favouritePair = _favouritePairRepository.GetSingle(x => x.PairId == PairId && x.UserId == UserId);
                if (favouritePair == null)
                {
                    return 1;
                }
                else if (favouritePair != null)
                {
                    favouritePair.Status = 9;
                    _favouritePairRepository.Update(favouritePair);
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<FavouritePairInfo> GetFavouritePair(long UserId, short IsMargin = 0)//Rita 23-2-19 for Margin Trading Data bit
        {
            List<FavouritePairInfo> responsedata = new List<FavouritePairInfo>();
            try
            {
                if (IsMargin == 1)
                    responsedata = _frontTrnRepository.GetFavouritePairsMargin(UserId);
                else
                    responsedata = _frontTrnRepository.GetFavouritePairs(UserId);
                return responsedata;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 23-2-19 for Margin Trading Data bit
        public int AddToFavouritePairMargin(long PairId, long UserId)
        {
            try
            {
                TradePairMasterMargin pairData = _trnMasterConfiguration.GetTradePairMasterMargin().Where(e => e.Id == PairId).FirstOrDefault();
                if (pairData == null)
                {
                    return 2;
                }
                FavouritePairMargin favouritePair = _favouritePairRepositoryMargin.GetSingle(x => x.PairId == PairId && x.UserId == UserId);
                if (favouritePair == null)
                {
                    //Add With First Time
                    favouritePair = new FavouritePairMargin()
                    {
                        PairId = PairId,
                        UserId = UserId,
                        Status = 1,
                        CreatedBy = UserId,
                        CreatedDate = _basePage.UTC_To_IST()
                    };
                    favouritePair = _favouritePairRepositoryMargin.Add(favouritePair);
                }
                else if (favouritePair != null)
                {
                    if (favouritePair.Status == 1)
                    {
                        return 1;  // already added as favourite pair
                    }
                    else if (favouritePair.Status == 9)
                    {
                        favouritePair.Status = 1;
                        _favouritePairRepositoryMargin.Update(favouritePair);
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public int RemoveFromFavouritePairMargin(long PairId, long UserId)
        {
            try
            {
                var favouritePair = _favouritePairRepositoryMargin.GetSingle(x => x.PairId == PairId && x.UserId == UserId);
                if (favouritePair == null)
                {
                    return 1;
                }
                else if (favouritePair != null)
                {
                    favouritePair.Status = 9;
                    _favouritePairRepositoryMargin.Update(favouritePair);
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region parameterValidation

        public bool IsValidPairName(string Pair)
        {
            try
            {
                String Pattern = "^[A-Z_]{5,20}$";//rita 10-1-19 change lengt hvalidation
                if (Regex.IsMatch(Pair, Pattern, RegexOptions.IgnoreCase))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public Int16 IsValidTradeType(string Type)
        {
            try
            {
                if (Type.ToUpper().Equals("BUY"))
                    return Convert.ToInt16(enTrnType.Buy_Trade);
                else if (Type.ToUpper().Equals("SELL"))
                    return Convert.ToInt16(enTrnType.Sell_Trade);
                else if (Type.ToUpper().Equals("Withdraw"))
                    return Convert.ToInt16(enTrnType.Withdraw);
                else
                    return 999;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public Int16 IsValidMarketType(string Type)
        {
            try
            {
                if (Type.ToUpper().Equals("LIMIT"))
                    return Convert.ToInt16(enTransactionMarketType.LIMIT);
                else if (Type.ToUpper().Equals("MARKET"))
                    return Convert.ToInt16(enTransactionMarketType.MARKET);
                else if (Type.ToUpper().Equals("STOP_LIMIT"))
                    return Convert.ToInt16(enTransactionMarketType.STOP_Limit);
                else if (Type.ToUpper().Equals("STOP"))
                    return Convert.ToInt16(enTransactionMarketType.STOP);
                else if (Type.ToUpper().Equals("SPOT"))
                    return Convert.ToInt16(enTransactionMarketType.SPOT);
                else
                    return 999;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public short IsValidStatus(string status)
        {
            try
            {
                if (status.ToUpper().Equals("SETTLED"))
                    return Convert.ToInt16(enTransactionStatus.Success);
                if (status.ToUpper().Equals("CURRENT"))
                    return Convert.ToInt16(enTransactionStatus.Hold);
                else
                    return 999;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public bool IsValidDateFormate(string date)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public void GetIntervalTimeValue(string Interval, ref int IntervalTime, ref string IntervalData)
        {
            try
            {
                switch (Interval)
                {
                    case "1m":
                        IntervalTime = 1;
                        IntervalData = "MINUTE";
                        break;
                    case "3m":
                        IntervalTime = 3;
                        IntervalData = "MINUTE";
                        break;
                    case "5m":
                        IntervalTime = 5;
                        IntervalData = "MINUTE";
                        break;
                    case "15m":
                        IntervalTime = 15;
                        IntervalData = "MINUTE";
                        break;
                    case "30m":
                        IntervalTime = 30;
                        IntervalData = "MINUTE";
                        break;
                    case "1H":
                        IntervalTime = 1;
                        IntervalData = "HOUR";
                        break;
                    case "2H":
                        IntervalTime = 2;
                        IntervalData = "HOUR";
                        break;
                    case "4H":
                        IntervalTime = 4;
                        IntervalData = "HOUR";
                        break;
                    case "6H":
                        IntervalTime = 6;
                        IntervalData = "HOUR";
                        break;
                    case "12H":
                        IntervalTime = 12;
                        IntervalData = "HOUR";
                        break;
                    case "1D":
                        IntervalTime = 1;
                        IntervalData = "DAY";
                        break;
                    case "1W":
                        IntervalTime = 1;
                        IntervalData = "WEEK";
                        break;
                    case "1M":
                        IntervalTime = 1;
                        IntervalData = "MONTH";
                        break;
                    default:
                        IntervalTime = 0;
                        break;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public BizResponseClass ValidatePairCommonMethod(string PairName, ref long PairId, short IsMargin)
        {
            try
            {
                if (!IsValidPairName(PairName))
                {
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InvalidPairName, ReturnMsg = "Fail" };
                }
                PairId = GetPairIdByName(PairName, IsMargin);
                if (PairId == 0)
                {
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InvalidPairName, ReturnMsg = "Fail" };
                }
                return new BizResponseClass() { ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" };
            }
            catch (Exception ex)
            {
                return new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError };
            }
        }
        public BizResponseClass ValidateFromDateToDateCommonMethod(string FromDate, string ToDate)
        {
            try
            {
                if (string.IsNullOrEmpty(ToDate))
                {
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InvalidFromDateFormate, ReturnMsg = "Fail" };
                }
                if (!IsValidDateFormate(FromDate))
                {
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InvalidFromDateFormate, ReturnMsg = "Fail"};
                }
                if (!IsValidDateFormate(ToDate))
                {
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InvalidToDateFormate, ReturnMsg = "Fail"};
                }
                return new BizResponseClass() { ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" };
            }
            catch (Exception ex)
            {
                return new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError };
            }
        }

        public BizResponseClass ValidatePairCommonMethodArbitrage(string PairName, ref long PairId, short IsMargin)
        {
            try
            {
                if (!IsValidPairName(PairName))
                {
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InvalidPairName, ReturnMsg = "Fail" };
                }
                PairId = GetPairIdByNameArbitrage(PairName, IsMargin);
                if (PairId == 0)
                {
                    return new BizResponseClass() { ReturnCode = enResponseCode.Fail, ErrorCode = enErrorCode.InvalidPairName, ReturnMsg = "Fail" };
                }
                return new BizResponseClass() { ReturnCode = enResponseCode.Success, ErrorCode = enErrorCode.Success, ReturnMsg = "Success" };
            }
            catch (Exception ex)
            {
                return new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError };
            }
        }
        #endregion

        #region TopGainer And ToLooser Pair
        public List<TopLooserGainerPairData> GetFrontTopGainerPair(int Type)
        {
            try
            {
                return _frontTrnRepository.GetFrontTopGainerPair(Type);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TopLooserGainerPairData> GetFrontTopLooserPair(int Type)
        {
            try
            {
                return _frontTrnRepository.GetFrontTopLooserPair(Type);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TopLooserGainerPairData> GetFrontTopLooserGainerPair()
        {
            try
            {
                return _frontTrnRepository.GetFrontTopLooserGainerPair();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TopLeadersListResponse TopLeadersList()
        {
            TopLeadersListResponse _Res = new TopLeadersListResponse();
            List<TopLeaderListInfo> leaderList = new List<TopLeaderListInfo>();
            int cnt = 0;
            try
            {
                var list = _profileConfigurationService.GetFrontLeaderList(0, 0, 2);
                list = _profileConfigurationService.GetFrontLeaderList(0, list.TotalCount, 2);
                foreach (var obj in list.LeaderList.OrderByDescending(e => e.NoOfFollowerFollow))
                {
                    leaderList.Add(new TopLeaderListInfo()
                    {
                        IsFollow = obj.IsFollow,
                        IsWatcher = obj.IsWatcher,
                        LeaderId = obj.LeaderId,
                        LeaderName = obj.LeaderName,
                        NoOfFollowers = obj.NoOfFollowerFollow,
                        UserDefaultVisible = obj.UserDefaultVisible
                    });
                    cnt++;
                    if (cnt >= 5)
                        break;
                }
                _Res.Response = leaderList;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TradeWatchListResponse getTradeWatchList(long userId)
        {
            List<TradeWatchLists> WatcherTrade = new List<TradeWatchLists>();
            try
            {
                TradeWatchListResponse _Res = new TradeWatchListResponse();
                var watchList = _profileConfigurationService.GetWatcherWiseLeaderList(0, 50, Convert.ToInt32(userId));
                if (watchList.TotalCount > 50)
                    watchList = _profileConfigurationService.GetWatcherWiseLeaderList(0, watchList.TotalCount, Convert.ToInt32(userId));

                foreach (var obj in watchList.WatcherList)
                {
                    WatcherTrade.Add(new TradeWatchLists()
                    {
                        LeaderId = obj.LeaderId,
                        LeaderName = obj.LeaderName,
                    });
                }
                _Res.Response = _frontTrnRepository.getTradeWatchList(WatcherTrade);
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TopProfitGainerLoserResponse GetTopProfitGainer(DateTime date, int size)
        {
            TopProfitGainerLoserResponse _Res = new TopProfitGainerLoserResponse();
            List<TopProfitGainerLoser> profitGainers = new List<TopProfitGainerLoser>();
            try
            {
                var list = _profileConfigurationService.GetFrontLeaderList(0, 0, 2);
                list = _profileConfigurationService.GetFrontLeaderList(0, list.TotalCount, 2);
                long[] leaderArray = list.LeaderList.Select(x => (long)x.LeaderId).ToArray();
                ListLeaderBoardRes profitList = new ListLeaderBoardRes();
                if (leaderArray.Count() > 0)
                {
                    profitList = _walletService.LeaderBoardWeekWiseTopFive(leaderArray, date, 1, size);
                }
                if (profitList.Data == null)
                {
                    _Res.Response = profitGainers;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnMsg = "Success";
                    return _Res;
                }
                foreach (var obj in profitList.Data)
                {
                    var leaderData = list.LeaderList.SingleOrDefault(e => e.LeaderId == obj.UserId);
                    profitGainers.Add(new TopProfitGainerLoser()
                    {
                        LeaderId = obj.UserId,
                        LeaderName = leaderData.LeaderName,
                        Profit = obj.ProfitAmount,
                        Email = obj.Email,
                        ProfitPer = obj.ProfitPer
                    });
                }
                _Res.Response = profitGainers;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TopProfitGainerLoserResponse TopProfitLoser(DateTime date, int size)
        {
            TopProfitGainerLoserResponse _Res = new TopProfitGainerLoserResponse();
            List<TopProfitGainerLoser> profitGainers = new List<TopProfitGainerLoser>();
            try
            {
                var list = _profileConfigurationService.GetFrontLeaderList(0, 0, 2);
                list = _profileConfigurationService.GetFrontLeaderList(0, list.TotalCount, 2);
                long[] leaderArray = list.LeaderList.Select(x => (long)x.LeaderId).ToArray();
                ListLeaderBoardRes profitList = new ListLeaderBoardRes();
                if (leaderArray.Count() > 0)
                {
                    profitList = _walletService.LeaderBoardWeekWiseTopFive(leaderArray, date, 1, size);
                }
                if (profitList.Data == null)
                {
                    _Res.Response = profitGainers;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnMsg = "Success";
                    return _Res;
                }
                foreach (var obj in profitList.Data)
                {
                    var leaderData = list.LeaderList.SingleOrDefault(e => e.LeaderId == obj.UserId);
                    profitGainers.Add(new TopProfitGainerLoser()
                    {
                        LeaderId = obj.UserId,
                        LeaderName = leaderData.LeaderName,
                        Profit = obj.ProfitAmount,
                        //AutoId = obj.AutoId,
                        Email = obj.Email,
                        ProfitPer = obj.ProfitPer
                    });
                }
                _Res.Response = profitGainers;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region SiteTokenConversion

        public SiteTokenConvertFundResponse GetSiteTokenConversionData(long? UserID, string SourceCurrency = "", string TargetCurrency = "", string FromDate = "", string ToDate = "", short IsMargin = 0)
        {
            SiteTokenConvertFundResponse _Res = new SiteTokenConvertFundResponse();
            List<SiteTokenConvertInfo> convertInfos = new List<SiteTokenConvertInfo>();
            try
            {
                var list = _frontTrnRepository.GetSiteTokenConversionData(UserID, SourceCurrency, TargetCurrency, FromDate, ToDate, IsMargin);
                if (list.Count == 0)
                {
                    _Res.Response = convertInfos;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                foreach (var obj in list)
                {
                    convertInfos.Add(new SiteTokenConvertInfo()
                    {
                        SourceCurrency = obj.SourceCurrency,
                        SourceCurrencyID = obj.SourceCurrencyID,
                        SourceCurrencyQty = obj.SourceCurrencyQty,
                        SourceToBasePrice = obj.SourceToBasePrice,
                        SourceToBaseQty = obj.SourceToBaseQty,
                        TargerCurrency = obj.TargerCurrency,
                        TargerCurrencyID = obj.TargerCurrencyID,
                        TargetCurrencyQty = obj.TargerCurrencyQty,
                        TokenPrice = obj.TokenPrice,
                        UserID = obj.UserID,
                        TrnDate = obj.CreatedDate
                    });
                }
                _Res.Response = convertInfos;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region trading configuration

        public TradingConfigurationList TradingConfiguration()
        {
            TradingConfigurationList _Res = new TradingConfigurationList();
            List<TradingConfigurationViewModel> convertInfos = new List<TradingConfigurationViewModel>();
            try
            {
                var list = _tradingConfigurationRepository.GetAllList();
                if (list.Count == 0)
                {
                    _Res.Data = convertInfos;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }

                foreach (var obj in list)
                {
                    convertInfos.Add(new TradingConfigurationViewModel()
                    {
                        CreatedDate = obj.CreatedDate,
                        Id = obj.Id,
                        Name = obj.Name,
                        Status = obj.Status
                    });
                }
                _Res.Data = convertInfos;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = EnResponseMessage.FindRecored;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public BizResponseClass ChangeTradingConfigurationStatus(long ConfigID, short Status, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                var Data = _tradingConfigurationRepository.GetById(ConfigID);
                if (Data == null)
                {
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                // khushali  26-07-2019 change logic and  optimize code
                //mansi - add bit for liquidity & marketmaking provider                
                if (Data.Name == enTradingType.Liquidity.ToString())
                {                    
                    Data.Status = Status;
                    if (Status == (short)ServiceStatus.Active)
                    {
                        TradingConfiguration MarketMakingData = _tradingConfigurationRepository.FindBy(e => e.Name == enTradingType.MarketMaking.ToString()).FirstOrDefault();
                        if (MarketMakingData != null)
                        {
                            MarketMakingData.Status = Convert.ToInt16(ServiceStatus.InActive);
                            _tradingConfigurationRepository.Update(MarketMakingData);
                        }                        
                    }                    
                }
                else if (Data.Name == enTradingType.MarketMaking.ToString())
                {
                    Data.Status = Status;
                    if (Status == (short)ServiceStatus.Active)
                    {
                        TradingConfiguration LiquidityData = _tradingConfigurationRepository.FindBy(e => e.Name == enTradingType.Liquidity.ToString()).FirstOrDefault();
                        if (LiquidityData != null)
                        {
                            LiquidityData.Status = Convert.ToInt16(ServiceStatus.InActive);
                            _tradingConfigurationRepository.Update(LiquidityData);
                        }                        
                    }                    
                }
                else
                {
                    Data.Status = Status;
                }
                Data.UpdatedBy = UserID;
                Data.UpdatedDate = Helpers.UTC_To_IST();
                _tradingConfigurationRepository.Update(Data);
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.RecordUpdatedSuccessfully;
                _Res.ReturnMsg = EnResponseMessage.RecordUpdated;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        
        #endregion

        #region Arbitrage Trading Data Method
        public long GetPairIdByNameArbitrage(string pair, short IsMargin = 0)
        {
            long PairID = 0;
            try
            {
                if (IsMargin == 1)
                {
                }
                else
                {
                    TradePairMasterArbitrage TradePairObj = _trnMasterConfiguration.GetTradePairMasterArbitrage().Where(p => p.PairName == pair && p.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                    if (TradePairObj == null)
                        return 0;
                    PairID = TradePairObj.Id;
                }
                return PairID;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetBuySellBook> GetBuyerSellerBookArbitrageV1(long id,short TrnType,short IsMargin = 0)
        {
            try
            {
                if(TrnType==4)
                    return _frontTrnRepository.GetBuyerBookArbitrage(id, Price: -1);
                else
                    return _frontTrnRepository.GetSellerBookArbitrage(id, Price: -1);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetGraphDetailInfo> GetGraphDetailArbitrage(long PairId, int IntervalTime, string IntervalData, short IsMargin = 0)//Rita 22-2-19 for Margin Trading Data bit
        {
            try
            {
                IOrderedEnumerable<GetGraphDetailInfo> list = _frontTrnRepository.GetGraphDataArbitrage(PairId, IntervalTime, IntervalData, _basePage.UTC_To_IST()).OrderBy(x => x.DataDate);
                return list.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ProfitIndicatorInfo GetProfitIndicatorArbitrage(long PairId, short IsMargin = 0)
        {
            try
            {
                ProfitIndicatorInfo responseData = new ProfitIndicatorInfo();
                //Rita 16-07-19 For Profit with fees
                TradePairDetailArbitrage TradePairDetailObj = _trnMasterConfiguration.GetTradePairDetailArbitrage().Where(item => item.PairId == PairId && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();

                List<ExchangeProviderListArbitrage> list = _frontTrnRepository.GetExchangeProviderListArbitrage(PairId);
                if (list == null || list.Count == 0)
                    return null;

                List<ExchangeProviderListArbitrage> Buylist;
                List<ExchangeProviderListArbitrage> Selllist;
                Buylist = list.Where(e => e.TrnType == 4).Where(e => e.LTP != 0).ToList();
                Selllist = list.Where(e => e.TrnType == 5).Where(e => e.LTP != 0).ToList();
                List<LeftProvider> LeftProListBuy = new List<LeftProvider>();
                foreach (ExchangeProviderListArbitrage sBuylist in Buylist)
                {
                    LeftProvider LeftProvider = new LeftProvider();
                    LeftProvider.RouteID = sBuylist.RouteID;
                    LeftProvider.RouteName = sBuylist.RouteName;
                    LeftProvider.SerProID = sBuylist.ProviderID;
                    LeftProvider.ProviderName = sBuylist.ProviderName;
                    LeftProvider.LTP = sBuylist.LTP;
                    List<Provider> Providers = new List<Provider>();
                    foreach (ExchangeProviderListArbitrage ssBuylist in Buylist)
                    {
                        Provider ToProvider = new Provider();
                        ToProvider.RouteID = ssBuylist.RouteID;
                        ToProvider.RouteName = ssBuylist.RouteName;
                        ToProvider.SerProID = ssBuylist.ProviderID;
                        ToProvider.ProviderName = ssBuylist.ProviderName;
                        ToProvider.LTP = ssBuylist.LTP;

                        var diff = LeftProvider.LTP - ToProvider.LTP;
                        ToProvider.GrossProfit = diff * 100 / Math.Min(LeftProvider.LTP, ToProvider.LTP);
                        ToProvider.NetProfit = ToProvider.GrossProfit - TradePairDetailObj.BuyFees; //(Provider + Local Fee)/2 //Rita 16-7-19 Per after Fee deduction

                        Providers.Add(ToProvider);
                    }
                    LeftProvider.Providers = Providers;
                    LeftProListBuy.Add(LeftProvider);
                }

                List<LeftProvider> LeftProListSell = new List<LeftProvider>();
                foreach (ExchangeProviderListArbitrage sSelllist in Selllist)
                {
                    LeftProvider LeftProvider = new LeftProvider();
                    LeftProvider.RouteID = sSelllist.RouteID;
                    LeftProvider.RouteName = sSelllist.RouteName;
                    LeftProvider.SerProID = sSelllist.ProviderID;
                    LeftProvider.ProviderName = sSelllist.ProviderName;
                    LeftProvider.LTP = sSelllist.LTP;
                    List<Provider> Providers = new List<Provider>();
                    foreach (ExchangeProviderListArbitrage ssSelllist in Selllist)
                    {
                        Provider ToProvider = new Provider();
                        ToProvider.RouteID = ssSelllist.RouteID;
                        ToProvider.RouteName = ssSelllist.RouteName;
                        ToProvider.SerProID = ssSelllist.ProviderID;
                        ToProvider.ProviderName = ssSelllist.ProviderName;
                        ToProvider.LTP = ssSelllist.LTP;

                        var diff = ToProvider.LTP - LeftProvider.LTP;
                        ToProvider.GrossProfit = diff * 100 / Math.Min(LeftProvider.LTP, ToProvider.LTP);
                        ToProvider.NetProfit = ToProvider.GrossProfit - TradePairDetailObj.SellFees; //(Provider + Local Fee)/2 //Rita 16-7-19 Per after Fee deduction

                        Providers.Add(ToProvider);
                    }
                    LeftProvider.Providers = Providers;
                    LeftProListSell.Add(LeftProvider);
                }
                responseData.BUY = LeftProListBuy;
                responseData.SELL = LeftProListSell;
                return responseData;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name + " ##PairId:" + PairId, ex);
                return null;
            }
        }

        public List<ExchangeProviderListArbitrage> ExchangeProviderListArbitrage(long PairId, short IsMargin = 0)
        {
            try
            {
                List<ExchangeProviderListArbitrage> list = _frontTrnRepository.GetExchangeProviderListArbitrage(PairId);
                if (list == null || list.Count == 0)
                    return null;
                return list.Where(e => e.TrnType == 4).ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rita 12-6-19 for display profit data in smart arbitrage screen in front
        public List<ExchangeListSmartArbitrage> ExchangeListSmartArbitrageService(long PairId, string PairName, short ProviderCount, short IsMargin = 0)
        {
            try
            {

                List<ExchangeListSmartArbitrage> responseData = new List<ExchangeListSmartArbitrage>();
                //Rita 16-07-19 For Profit with fees
                TradePairDetailArbitrage TradePairDetailObj = _trnMasterConfiguration.GetTradePairDetailArbitrage().Where(item => item.PairId == PairId && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();

                List<ExchangeProviderListArbitrage> list = _frontTrnRepository.GetExchangeProviderListArbitrage(PairId);
                if (list == null || list.Count == 0)
                    return null;

                List<ExchangeProviderListArbitrage> Buylist;
                List<ExchangeProviderListArbitrage> Selllist;

                //here LTP zero then gives error 
                Buylist = list.Where(e => e.TrnType == 4).Where(e => e.LTP != 0).OrderBy(e => e.LTP).Take(ProviderCount).ToList();//take top 5 provider
                Selllist = list.Where(e => e.TrnType == 5).Where(e => e.LTP != 0).OrderByDescending(e => e.LTP).Take(ProviderCount).ToList();//take top 5 provider

                foreach (ExchangeProviderListArbitrage sSelllist in Selllist)//High seller common for differnt low Buyer
                {
                    Providers ProviderSELL = new Providers();
                    ProviderSELL.RouteID = sSelllist.RouteID;
                    ProviderSELL.RouteName = sSelllist.RouteName;
                    ProviderSELL.SerProID = sSelllist.ProviderID;
                    ProviderSELL.ProviderName = sSelllist.ProviderName;
                    ProviderSELL.LPType = sSelllist.LPType;
                    ProviderSELL.LTP = sSelllist.LTP;

                    foreach (ExchangeProviderListArbitrage sBuylist in Buylist)
                    {
                        if (ProviderSELL.LPType == sBuylist.LPType)
                            continue;
                        ExchangeListSmartArbitrage TableList = new ExchangeListSmartArbitrage();
                        Providers ProviderBuy = new Providers();

                        TableList.Pair = PairName;
                        TableList.ProviderSELL = ProviderSELL;

                        ProviderBuy.RouteID = sBuylist.RouteID;
                        ProviderBuy.RouteName = sBuylist.RouteName;
                        ProviderBuy.SerProID = sBuylist.ProviderID;
                        ProviderBuy.ProviderName = sBuylist.ProviderName;
                        ProviderBuy.LPType = sBuylist.LPType;
                        ProviderBuy.LTP = sBuylist.LTP;

                        var diff = ProviderSELL.LTP - ProviderBuy.LTP;
                        if (diff <= 0)
                            continue;
                       
                        TableList.GrossProfitPer = diff * 100 / ProviderBuy.LTP;
                        //Rita 16-7-19 Per after Fee deduction , Provider + Local Fee
                        //Rita 18-7-19 differ Gross-total , and Net with Fee deduction , amount and value
                        TableList.GrossProfitValue = diff;
                        TableList.NetProfitPer = TableList.GrossProfitPer - (TradePairDetailObj.BuyFees + TradePairDetailObj.SellFees);
                        TableList.NetProfitValue = TableList.NetProfitPer * ProviderBuy.LTP / 100;
                        //if (TableList.NetProfitPer <= 0)
                        //    continue;

                        TableList.ProviderBuy = ProviderBuy;
                        responseData.Add(TableList);
                    }

                }
                return responseData.OrderByDescending(e => e.GrossProfitPer).ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name + " ##PairId:" + PairId, ex);
                return null;
                //throw ex;
            }
        }

        public List<BasePairResponse> GetTradePairAssetArbitrage()
        {
            //decimal ChangePer = 0;
            //decimal Volume24 = 0;
            List<BasePairResponse> responsedata;
            try
            {
                responsedata = new List<BasePairResponse>();
                //var basePairData = _marketRepository.GetAll();
                //Rita 01-05-19 added priority ,also added status condition
                IEnumerable<MarketArbitrage> basePairData = _trnMasterConfiguration.GetMarketArbitrage().Where(x => x.Status == 1).OrderBy(x => x.Priority);//rita 23-2-19 taken from cache Implemented

                var TradePairList = _frontTrnRepository.GetTradePairAssetArbitrageInfo();

                if (basePairData != null)
                {
                    foreach (var bpair in basePairData)
                    {
                        BasePairResponse basePair = new BasePairResponse();
                        var pairData = TradePairList.Where(x => x.BaseId == bpair.ServiceID).OrderBy(x => x.Priority);
                        if (pairData.Count() > 0)
                        {
                            basePair.BaseCurrencyId = pairData.FirstOrDefault().BaseId;
                            basePair.BaseCurrencyName = pairData.FirstOrDefault().BaseName;
                            basePair.Abbrevation = pairData.FirstOrDefault().BaseCode;
                            List<TradePairRespose> pairList = new List<TradePairRespose>();
                            pairList = pairData.Select(pair => new TradePairRespose
                            {
                                PairId = pair.PairId,
                                Pairname = pair.Pairname,
                                Currentrate = pair.Currentrate,
                                BuyFees = pair.BuyFees,
                                SellFees = pair.SellFees,
                                ChildCurrency = pair.ChildCurrency,
                                Abbrevation = pair.Abbrevation,
                                ChangePer = pair.ChangePer,
                                Volume = pair.Volume,
                                High24Hr = pair.High24Hr,
                                Low24Hr = pair.Low24Hr,
                                HighWeek = pair.HighWeek,
                                LowWeek = pair.LowWeek,
                                High52Week = pair.High52Week,
                                Low52Week = pair.Low52Week,
                                UpDownBit = pair.UpDownBit,
                                QtyLength = pair.QtyLength,
                                PriceLength = pair.PriceLength,
                                AmtLength = pair.AmtLength                        
                            }).ToList();

                            basePair.PairList = pairList;
                            responsedata.Add(basePair);
                        }
                    }
                    return responsedata;
                }
                else
                {
                    return responsedata;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ArbitrageBuySellViewModel> GetExchangeProviderBuySellBookArbitrage(long PairId, short TrnType)
        {
            try
            {
                return _frontTrnRepository.GetExchangeProviderBuySellBookArbitrage(PairId, TrnType);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<SmartArbitrageHistoryInfo> SmartArbitrageHistoryList(long PairId, long MemberID, string FromDat, string ToDate, short IsMargin = 0)
        {
            try
            {
                List<SmartArbitrageHistoryInfo> list = _frontTrnRepository.SmartArbitrageHistoryList(PairId, MemberID, FromDat, ToDate);
                if (list == null || list.Count == 0)
                    return null;

                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion =========================================

        #region ArbitrageTradingAllowToUser CRUD

        public AllowArbitrageTradingViewModel GetAllowedUserArbitrageTrading(long UserID)
        {
            AllowArbitrageTradingViewModel _Res = new AllowArbitrageTradingViewModel();
            try
            {
                ArbitrageTradingAllowToUser ModelData = _arbitrageTradingAllowToUserRepository.FindBy(e => e.UserId == UserID).FirstOrDefault();
                if (ModelData == null)
                    _Res.TradingType = (Enum.GetName(typeof(enArbitrageTradingAllowToUser), enArbitrageTradingAllowToUser.None));
                else
                    _Res.TradingType = (Enum.GetName(typeof(enArbitrageTradingAllowToUser),ModelData.SmaartTradePriority));

                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                _Res.ErrorCode = enErrorCode.Success;         
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(ControllerName, "GetAllowArbitrageTrading", ex);
                throw ex;
            }
        }

        public BizResponseClass SetAllowedUserArbitrageTrading(enArbitrageTradingAllowToUser TradingType, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                ArbitrageTradingAllowToUser ModelData = _arbitrageTradingAllowToUserRepository.FindBy(e => e.UserId == UserID).FirstOrDefault();
                if(ModelData==null)
                {
                    _arbitrageTradingAllowToUserRepository.Add(new ArbitrageTradingAllowToUser()
                    {
                        CreatedBy=UserID,
                        CreatedDate=DateTime.UtcNow,
                        SmaartTradePriority= (short)TradingType,
                        Status=1,
                        UserId=UserID
                    });
                }
                else
                {
                    ModelData.SmaartTradePriority =(short)TradingType;
                    ModelData.UpdatedBy = UserID;
                    ModelData.UpdatedDate=DateTime.UtcNow;
                    _arbitrageTradingAllowToUserRepository.Update(ModelData);
                }
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                _Res.ErrorCode = enErrorCode.Success;
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(ControllerName, "SetAllowedUserArbitrageTrading", ex);
                throw ex;
            }
        }

        #endregion

        //18-07-2019 komal this is test method
        public async Task<string> CCXTBalanceCheckAsync(string exchangeName, string id, string APIKey, string SecretKey)
        {
            string APIResponse = string.Empty;
            try
            {
                String Token =await ConnectToExchangeAsync("binance", "MyKey", "", "");
                if(!string.IsNullOrEmpty(Token))
                {
                    Uri uri = new Uri("http://172.20.65.133:3000/exchange/" + exchangeName+ "/balances");
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Accept= "application/json";
                    httpWebRequest.Headers.Add("Authorization", "Bearer "+Token);
                    httpWebRequest.Method = "GET";

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        APIResponse = streamReader.ReadToEnd();
                    }
                }
                return APIResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return "";
            }
        }
        public async Task<string> ConnectToExchangeAsync(string exchangeName,string id,string APIKey,string SecretKey)
        {
            string APIResponse = string.Empty;
            try
            {
                Uri uri = new Uri("http://172.20.65.133:3000/exchange/"+ exchangeName);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json ="{\"id\":\""+ id + "\",\"apiKey\": \""+ APIKey + "\",\"secret\": \""+ SecretKey + "\",\"enableRateLimit\": true}";
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    APIResponse = streamReader.ReadToEnd();
                }
                if (string.IsNullOrEmpty(APIResponse))
                    return "";

                CCXTTokenResponse tokenResponse = JsonConvert.DeserializeObject<CCXTTokenResponse>(APIResponse);
                return tokenResponse.token;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return "";
            }
        }
        public class CCXTTokenResponse
        {
            public string token { get; set; }
        }

        public int GetMarketMakerUserRole()
        {
            try
            {
                return _frontTrnRepository.GetMarketMakerUserRole();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(ControllerName, "GetMarketMakerUserRole", ex);
                return 0;
            }
        }
    }
}
