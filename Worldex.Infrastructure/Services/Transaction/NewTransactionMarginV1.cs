using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.MarginWallet;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.WalletOperations;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Transaction
{
    public class NewTransactionMarginV1 : ITransactionProcessMarginV1
    {
        private readonly ICommonRepository<TransactionQueueMargin> _TransactionRepository;
        private readonly ICommonRepository<TradeTransactionQueueMargin> _TradeTransactionRepository;
        private readonly ICommonRepository<TradeStopLossMargin> _TradeStopLoss;
        private readonly ICommonRepository<TradeSellerListMarginV1> _TradeSellerList;
        private readonly ICommonRepository<TradeBuyerListMarginV1> _TradeBuyerList;
        private readonly ICommonRepository<TradePairStasticsMargin> _tradePairStastics;
        private readonly IMarginTransactionWallet _WalletService;
        private readonly IWebApiRepository _WebApiRepository;
        private readonly ISettlementRepositoryMarginV1<BizResponse> _SettlementRepositoryV1;
        private readonly ISignalRService _ISignalRService;
        private readonly ITrnMasterConfiguration _trnMasterConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMessageService _messageService;
        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly IMediator _mediator;
        //private readonly ICommonWalletFunction _commonWalletFunction;
        private readonly IMarginWalletRepository _IMarginWalletRepository;
        private readonly ICommonRepository<MarginTradingAllowToUser> _MarginTradingAllowToUser;

        public BizResponse _Resp;
        public TradePairMasterMargin _TradePairObj;
        public TradePairDetailMargin _TradePairDetailObj;
        public List<TransactionProviderResponse> TxnProviderList;
        TransactionQueueMargin Newtransaction;
        TradeTransactionQueueMargin NewTradetransaction;
        NewTransactionRequestMarginCls Req;
        NewTradeTransactionRequestMarginCls _TradeTransactionObj = new NewTradeTransactionRequestMarginCls();
        ServiceMasterMargin _BaseCurrService;
        ServiceMasterMargin _SecondCurrService;
        TradeSellerListMarginV1 TradeSellerListObj;
        TradeBuyerListMarginV1 TradeBuyerListObj;
        TradeStopLossMargin TradeStopLossObj;
        private string ControllerName = "NewTransactionMarginV1";
        short STOPLimitWithSameLTP = 0;


        public NewTransactionMarginV1(
            ICommonRepository<TransactionQueueMargin> TransactionRepository,
            ICommonRepository<TradeTransactionQueueMargin> TradeTransactionRepository,
            ICommonRepository<TradeStopLossMargin> tradeStopLoss, IMarginTransactionWallet WalletService,IWebApiRepository WebApiRepository,
            ICommonRepository<TradeSellerListMarginV1> TradeSellerList,
            ICommonRepository<TradeBuyerListMarginV1> TradeBuyerList, ISettlementRepositoryMarginV1<BizResponse> SettlementRepositoryV1,
            ISignalRService ISignalRService, ICommonRepository<TradePairStasticsMargin> tradePairStastics,//IServiceProvider services, 
            ITrnMasterConfiguration trnMasterConfiguration, UserManager<ApplicationUser> userManager, IMessageService messageService, 
            IPushNotificationsQueue<SendSMSRequest> pushSMSQueue, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, IMediator mediator,
            IMarginWalletRepository IMarginWalletRepository, ICommonRepository<MarginTradingAllowToUser> MarginTradingAllowToUser)//ICommonWalletFunction commonWalletFunction
        {
            _TransactionRepository = TransactionRepository;
            _TradeTransactionRepository = TradeTransactionRepository;
            _TradeStopLoss = tradeStopLoss;
            _WalletService = WalletService;
            _WebApiRepository = WebApiRepository;
            _TradeSellerList = TradeSellerList;
            _TradeBuyerList = TradeBuyerList;
            _SettlementRepositoryV1 = SettlementRepositoryV1;
            _ISignalRService = ISignalRService;
            _tradePairStastics = tradePairStastics;
            _trnMasterConfiguration = trnMasterConfiguration;
             _userManager = userManager;
            _messageService = messageService;
            _pushSMSQueue = pushSMSQueue;
            _pushNotificationsQueue = pushNotificationsQueue;
            _mediator = mediator;
            //_commonWalletFunction = commonWalletFunction;
            _IMarginWalletRepository = IMarginWalletRepository;
            _MarginTradingAllowToUser = MarginTradingAllowToUser;
        }
        public async Task<BizResponse> ProcessNewTransactionAsync(NewTransactionRequestMarginCls Req1)
        {
            Req = Req1;

            _Resp = await CreateTransaction();
            if (_Resp.ReturnCode != enResponseCodeService.Success)
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ProcessNewTransactionAsync", ControllerName, _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo + " GUID:" + Req.GUID, Helpers.UTC_To_IST()));
                return _Resp;
            }
            _Resp = await CombineAllInitTransactionAsync();

            return _Resp;
        }

        public async Task<BizResponse> CombineAllInitTransactionAsync()
        {
            _Resp = new BizResponse();
            try
            {
                //Deduct balance here
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync Wallet", ControllerName, "Balance Deduction Start " + Req.GUID + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));

                if (Req.ISOrderBySystem == 1 && Req.IsWithoutAmtHold == 1)//Rita 1-4-19 for system internal order for sell qty
                    goto skipHold;

                var DebitResult1 = _WalletService.MarginGetWalletHoldNew(Req.MemberID, Req.SMSCode, Helpers.GetTimeStamp(), Req.Amount,
                    Req.DebitAccountID, Req.TrnNo, enServiceType.Trading, Req.TrnType == enTrnType.Buy_Trade ? enMarginWalletTrnType.BuyTrade : enMarginWalletTrnType.SellTrade, Req.TrnType, (EnAllowedChannels)Req.TrnMode, (enWalletDeductionType)((short)Req.ordertype)); //NTRIVEDI 07-12-2018               

                //TradingDataInsert(_Resp);
                var DebitResult = await DebitResult1;

                //2019-4-29 addedd charge 
                Newtransaction.ChargeRs = DebitResult.Charge;
                Newtransaction.ChargeCurrency = DebitResult.ChargeCurrency;

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync Wallet", ControllerName, "Deduction End" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));

                if (DebitResult.ReturnCode != enResponseCode.Success)
                {
                    _Resp.ReturnMsg = DebitResult.ReturnMsg;//EnResponseMessage.ProcessTrn_WalletDebitFailMsg;
                    _Resp.ReturnCode = enResponseCodeService.Fail;
                    _Resp.ErrorCode = DebitResult.ErrorCode;//enErrorCode.ProcessTrn_WalletDebitFail;
                    MarkTransactionSystemFail(_Resp.ReturnMsg, _Resp.ErrorCode);
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "Balance Deduction Fail" + _Resp.ReturnMsg + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                    return _Resp;
                }

                skipHold:
                //===================================Make txn HOLD as balance debited=======================                
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "HOLD Start" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                MarkTransactionHold(EnResponseMessage.ProcessTrn_HoldMsg, enErrorCode.ProcessTrn_Hold);//Rita 4-3-19 remove task.run for speed execution status not update as settlement reverse

                //Rita 8-1-19 for followers trading
                //if (Req.ISFollowersReq == 0 && Req.FollowingTo == 0 && Req.LeaderTrnNo == 0)//only for leader's allow this
                //{
                //    FollowersOrderRequestCls request = new FollowersOrderRequestCls { Req = Req, Delivery_Currency = _TradeTransactionObj.Delivery_Currency, Order_Currency = _TradeTransactionObj.Order_Currency };
                //    Task.Run(() => _mediator.Send(request));
                //}
                //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "Trading Data Entry Start " + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));

                if (Req.TrnType == enTrnType.Buy_Trade)
                {
                    //await InsertBuyerList();
                    TradeBuyerListObj.TrnNo = Req.TrnNo;
                    TradeBuyerListObj = _TradeBuyerList.Add(TradeBuyerListObj);
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "END Buyer" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                    if (Req.ordertype != enTransactionMarketType.STOP_Limit || STOPLimitWithSameLTP == 1)
                    {
                        _Resp = await _SettlementRepositoryV1.PROCESSSETLLEMENTBuy(_Resp, NewTradetransaction, Newtransaction, TradeStopLossObj, TradeBuyerListObj, Req.accessToken, 0);
                    }
                }
                else
                {
                    TradeSellerListObj.TrnNo = Req.TrnNo;
                    TradeSellerListObj = _TradeSellerList.Add(TradeSellerListObj);
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync", ControllerName, "END Seller" + "##TrnNo:" + Req.TrnNo, Helpers.UTC_To_IST()));
                    if (Req.ordertype != enTransactionMarketType.STOP_Limit || STOPLimitWithSameLTP == 1)
                    {
                        _Resp = await _SettlementRepositoryV1.PROCESSSETLLEMENTSell(_Resp, NewTradetransaction, Newtransaction, TradeStopLossObj, TradeSellerListObj, Req.accessToken, 0);
                    }
                }
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CombineAllInitTransactionAsync Settlement END Now wait for 20 sec", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
                Task.Delay(10000).Wait();//rita 3-1-19 wait for all operations done //rita 27-2-19 change from 20000 to 10000
                return _Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CombineAllInitTransactionAsync Internal Error:##TrnNo " + Req.TrnNo, ControllerName, ex);
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionProcessInternalError });
            }
        }

        #region RegionInitTransaction    
        public async Task<BizResponse> CreateTransaction()
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Transaction Process For" + Req.TrnType + "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));
                _TradePairObj = _trnMasterConfiguration.GetTradePairMasterMargin().Where(item => item.Id == Req.PairID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                if (_TradePairObj == null)
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnNoPairSelectedMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrn_NoPairSelected);
                }
                var pairStasticsResult = _tradePairStastics.GetSingleAsync(pair => pair.PairId == Req.PairID);
                _TradeTransactionObj.PairName = _TradePairObj.PairName;
                var LoadDataResult = LoadAllMasterDataParaller();

                //var GetWalletIDResult1 = _WalletService.GetWalletID(Req.DebitAccountID);
                if (Req.TrnType == enTrnType.Buy_Trade)
                {
                    _TradeTransactionObj.TrnTypeName = "BUY";
                }
                else
                {
                    _TradeTransactionObj.TrnTypeName = "SELL";
                }
                if (Convert.ToInt16(Req.ordertype) < 1 || Convert.ToInt16(Req.ordertype) > 5)
                {
                    Req.StatusMsg = EnResponseMessage.InValidOrderTypeMsg;
                    return MarkSystemFailTransaction(enErrorCode.InValidOrderType);
                }
                //var GetWalletIDResult2 = _WalletService.GetWalletID(Req.CreditAccountID);

                //Req.DebitWalletID = await GetWalletIDResult1;
                //if (Req.DebitWalletID == 0)
                //{
                //    Req.StatusMsg = EnResponseMessage.InValidDebitAccountIDMsg;
                //    return MarkSystemFailTransaction(enErrorCode.InValidDebitAccountID);
                //}
                //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Debit WalletID" + Req.DebitWalletID + "##TrnNo:" + Req.GUID,Helpers.UTC_To_IST()));
                var pairStastics = await pairStasticsResult;
                if (Req.ordertype == enTransactionMarketType.MARKET)
                {
                    Req.Price = pairStastics.LTP;
                }
                //_TradeTransactionObj.OrderWalletID = Req.DebitWalletID;

                //Req.CreditWalletID = await GetWalletIDResult2;
                //if (Req.CreditWalletID == 0)
                //{
                //    Req.StatusMsg = EnResponseMessage.InValidCreditAccountIDMsg;
                //    return MarkSystemFailTransaction(enErrorCode.InValidCreditAccountID);
                //}
                //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Credit WalletID" + Req.CreditWalletID + "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));

                //_TradeTransactionObj.DeliveryWalletID = Req.CreditWalletID;
             
                if (Req.Qty <= 0 || (Req.Price <= 0 && Req.ordertype != enTransactionMarketType.MARKET)|| (Req.StopPrice == 0 && Req.ordertype == enTransactionMarketType.STOP_Limit))
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnInvalidQtyPriceMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrnInvalidQtyPrice);
                }
                await LoadDataResult;
                
                if (_TradePairDetailObj == null)
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnNoPairSelectedMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrn_NoPairSelected);
                }
                if (Req.TrnType == enTrnType.Buy_Trade)
                {
                    _TradeTransactionObj.BuyQty = Req.Qty;
                    _TradeTransactionObj.BidPrice = Req.ordertype == enTransactionMarketType.MARKET ? 0 : Req.Price;
                    var AssRes = AssignDataBuy();                   
                    if (_TradeTransactionObj.BuyQty < _TradePairDetailObj.BuyMinQty || _TradeTransactionObj.BuyQty > _TradePairDetailObj.BuyMaxQty)
                    {
                        Req.StatusMsg = EnResponseMessage.ProcessTrn_AmountBetweenMinMaxMsg.Replace("@MIN", _TradePairDetailObj.BuyMinQty.ToString()).Replace("@MAX", _TradePairDetailObj.BuyMaxQty.ToString());
                        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_AmountBetweenMinMax, _TradePairDetailObj.BuyMinQty.ToString(), _TradePairDetailObj.BuyMaxQty.ToString());
                    }
                    if ((_TradeTransactionObj.BidPrice < _TradePairDetailObj.BuyMinPrice || _TradeTransactionObj.BidPrice > _TradePairDetailObj.BuyMaxPrice) && Req.ordertype != enTransactionMarketType.MARKET)
                    {
                        Req.StatusMsg = EnResponseMessage.ProcessTrn_PriceBetweenMinMaxMsg.Replace("@MIN", _TradePairDetailObj.BuyMinPrice.ToString()).Replace("@MAX", _TradePairDetailObj.BuyMaxPrice.ToString());
                        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_PriceBetweenMinMax, _TradePairDetailObj.BuyMinPrice.ToString(), _TradePairDetailObj.BuyMaxPrice.ToString());
                    }
                    await AssRes;
                    InsertBuyerList();
                }
                else if (Req.TrnType == enTrnType.Sell_Trade)
                {
                    _TradeTransactionObj.SellQty = Req.Qty;
                    _TradeTransactionObj.AskPrice = Req.ordertype == enTransactionMarketType.MARKET ? 0 : Req.Price;
                    var AssRes=AssignDataSell();                   
                    if (_TradeTransactionObj.SellQty < _TradePairDetailObj.SellMinQty || _TradeTransactionObj.SellQty > _TradePairDetailObj.SellMaxQty)
                    {
                        Req.StatusMsg = EnResponseMessage.ProcessTrn_AmountBetweenMinMaxMsg.Replace("@MIN", _TradePairDetailObj.SellMinQty.ToString()).Replace("@MAX", _TradePairDetailObj.SellMaxQty.ToString());
                        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_AmountBetweenMinMax, _TradePairDetailObj.SellMinQty.ToString(), _TradePairDetailObj.SellMaxQty.ToString());
                    }
                    if ((_TradeTransactionObj.AskPrice < _TradePairDetailObj.SellMinPrice || _TradeTransactionObj.AskPrice > _TradePairDetailObj.SellMaxPrice) && Req.ordertype != enTransactionMarketType.MARKET)
                    {
                        Req.StatusMsg = EnResponseMessage.ProcessTrn_PriceBetweenMinMaxMsg.Replace("@MIN", _TradePairDetailObj.SellMinPrice.ToString()).Replace("@MAX", _TradePairDetailObj.SellMaxPrice.ToString());
                        return MarkSystemFailTransaction(enErrorCode.ProcessTrn_PriceBetweenMinMax, _TradePairDetailObj.SellMinPrice.ToString(), _TradePairDetailObj.SellMaxPrice.ToString());
                    }
                    await AssRes;
                    InsertSellerList();
                }

                //komal 30-9-2019 Validate user wallet
                var OrderWalletDetail = _WalletService.GetTransactionWalletByCoin(Req.MemberID, _TradeTransactionObj.Order_Currency);
                var DeliveryWalletDetail = await _WalletService.GetTransactionWalletByCoin(Req.MemberID, _TradeTransactionObj.Delivery_Currency);
                if (DeliveryWalletDetail.ReturnCode != enResponseCode.Success)
                {
                    Req.StatusMsg = EnResponseMessage.InValidCreditAccountIDMsg;
                    return MarkSystemFailTransaction(enErrorCode.InValidCreditAccountID);
                }
                Req.CreditAccountID = DeliveryWalletDetail.Wallet.AccWalletID;
                Req.CreditWalletID = DeliveryWalletDetail.Wallet.id;
                _TradeTransactionObj.DeliveryWalletID = DeliveryWalletDetail.Wallet.id;
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Credit WalletID" + Req.CreditWalletID + "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));

                var OrderWalletDetail2 = OrderWalletDetail.GetAwaiter().GetResult();
                if (OrderWalletDetail2.ReturnCode != enResponseCode.Success)
                {
                    Req.StatusMsg = EnResponseMessage.InValidDebitAccountIDMsg;
                    return MarkSystemFailTransaction(enErrorCode.InValidDebitAccountID);
                }
                Req.DebitAccountID = OrderWalletDetail2.Wallet.AccWalletID;
                Req.DebitWalletID = OrderWalletDetail2.Wallet.id;
                _TradeTransactionObj.OrderWalletID = OrderWalletDetail2.Wallet.id;
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CreateTransaction", ControllerName, "Debit WalletID" + Req.DebitWalletID + "##TrnNo:" + Req.GUID, Helpers.UTC_To_IST()));

                //Rita 28-9-19 if same Wallet id then fail order
                if (_TradeTransactionObj.DeliveryWalletID == _TradeTransactionObj.OrderWalletID)
                {
                    Req.StatusMsg = EnResponseMessage.InValidCreditAccountIDMsg + "..";
                    return MarkSystemFailTransaction(enErrorCode.InValidCreditAccountID);
                }




                //Rita 10-8-19 for total Qty validation
                if (_TradePairDetailObj.MinNotional > Req.Price * Req.Qty || _TradePairDetailObj.MaxNotional < Req.Price * Req.Qty)
                {
                    Req.StatusMsg = EnResponseMessage.ProcessTrn_TotalFailure.Replace("@MIN", _TradePairDetailObj.MinNotional.ToString()).Replace("@MAX", _TradePairDetailObj.MaxNotional.ToString());
                    return MarkSystemFailTransaction(enErrorCode.ProcessTrn_TotalFailure, _TradePairDetailObj.MinNotional.ToString(), _TradePairDetailObj.MaxNotional.ToString());
                }

                //Rita 28-3-19 does not required here
                //var GetProListResult = _WebApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest {amount = Req.Amount,SMSCode = Req.SMSCode, APIType = enWebAPIRouteType.TransactionAPI,
                //                        trnType = Req.TrnType == enTrnType.Sell_Trade ? Convert.ToInt32(enTrnType.Buy_Trade) : Convert.ToInt32(Req.TrnType) });

                //Rita 23-2-19 remove sa need to take this limit saperately
                // var walletLimit = _commonWalletFunction.CheckWalletLimitAsyncV1(enWalletLimitType.TradingLimit, Req.DebitWalletID, Req.Amount);

                if (_TradeTransactionObj.OrderTotalQty < (decimal)(0.000000000000000001) || _TradeTransactionObj.DeliveryTotalQty < (decimal)(0.000000000000000001))
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnInvalidQtyNAmountMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrnInvalidQtyNAmount);
                }
                if (Req.Amount <= 0)
                {
                    Req.StatusMsg = EnResponseMessage.CreateTrnInvalidAmountMsg;
                    return MarkSystemFailTransaction(enErrorCode.CreateTrnInvalidAmount);
                }
                //WalletTrnLimitResponse WalletLimitRes = await walletLimit;//komal 25-01-2019 check transaction limit
                //if ( WalletLimitRes.ErrorCode != enErrorCode.Success)
                //{                    
                //    Req.StatusMsg = WalletLimitRes.ReturnMsg.ToString();
                //    return MarkSystemFailTransaction(WalletLimitRes.ErrorCode);
                //}
                //Rita 28-3-19 for at a time one position open in any pair
                TradeTransactionQueueMargin OpenPositionEntry = _TradeTransactionRepository.GetSingle(e =>e.Status==4 && e.PairID != Req.PairID && e.MemberID==Req.MemberID);
                if (OpenPositionEntry!=null)//IF Entry found does not proceed
                {
                    Req.StatusMsg = "Already Open Position in pair: " + OpenPositionEntry.PairName;
                    return MarkSystemFailTransaction(enErrorCode.MarginTxn_AlreadyOpenPosition);
                }
                OpenPositionMaster OpenPositionObj = _IMarginWalletRepository.GetPositionOpenInOtherPair(Req.PairID, Req.MemberID);//Position open by Site tiken conversion
                if (OpenPositionObj != null)//IF Entry found does not proceed
                {
                    Req.StatusMsg = "Already Open Position in pair: " + _trnMasterConfiguration.GetTradePairMasterMargin().Where(item => item.Id == OpenPositionObj.PairID).FirstOrDefault().PairName;
                    return MarkSystemFailTransaction(enErrorCode.MarginTxn_AlreadyOpenPosition);
                }
                MarginTradingAllowToUser MarginAllowRecord = _MarginTradingAllowToUser.GetSingle(e => e.UserId == Req.MemberID && e.Status==1);
                if(MarginAllowRecord==null)//Not allowed
                {
                    Req.StatusMsg = "Margin Trading not allowed";
                    return MarkSystemFailTransaction(enErrorCode.Margin_TradingNotAllowed);
                }

                Req.Status = enTransactionStatus.Initialize;
                Req.StatusCode = Convert.ToInt64(enErrorCode.TransactionInsertSuccess);
                await InsertTransactionInQueue();
                await InsertTradeTransactionInQueue();
                await InsertTradeStopLoss(pairStastics.LTP);
                return new BizResponse { ReturnMsg = "", ReturnCode = enResponseCodeService.Success };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateTransaction:##TrnNo " + Req.TrnNo + " GUID:"+ Req.GUID, ControllerName, ex);
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionInsertInternalError });
            }

        }
        public async Task AssignDataBuy()
        {
            try
            {
                _TradeTransactionObj.DeliveryTotalQty = Req.Qty;
                _TradeTransactionObj.OrderTotalQty = Helpers.DoRoundForTrading(Req.Qty * Req.Price, 18);//235.415001286,8 =  235.41500129                         
                _TradeTransactionObj.Order_Currency = _BaseCurrService.SMSCode;
                _TradeTransactionObj.Delivery_Currency = _SecondCurrService.SMSCode;
                Req.SMSCode = _TradeTransactionObj.Order_Currency;
                Req.Amount = _TradeTransactionObj.OrderTotalQty;
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("AssignDataBuy:##TrnNo " + Req.TrnNo, ControllerName, ex));
                throw ex;
            }
        }
        public async Task AssignDataSell()
        {
            try
            {
                _TradeTransactionObj.OrderTotalQty = Req.Qty;
                _TradeTransactionObj.DeliveryTotalQty = Helpers.DoRoundForTrading(Req.Qty * Req.Price, 18);//235.415001286,8 =  235.41500129                        
                _TradeTransactionObj.Order_Currency = _SecondCurrService.SMSCode;
                _TradeTransactionObj.Delivery_Currency = _BaseCurrService.SMSCode;
                Req.SMSCode = _TradeTransactionObj.Order_Currency;
                Req.Amount = _TradeTransactionObj.OrderTotalQty;
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("AssignDataBuy:##TrnNo " + Req.TrnNo, ControllerName, ex));
                throw ex;
            }
        }
        public async Task LoadAllMasterDataParaller()
        {
            try
            {
                _BaseCurrService = _trnMasterConfiguration.GetServicesMargin().Where(item => item.Id == _TradePairObj.BaseCurrencyId).FirstOrDefault();
                _SecondCurrService = _trnMasterConfiguration.GetServicesMargin().Where(item => item.Id == _TradePairObj.SecondaryCurrencyId).FirstOrDefault();
                _TradePairDetailObj = _trnMasterConfiguration.GetTradePairDetailMargin().Where(item => item.PairId == Req.PairID && item.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LoadAllMasterDataParaller:##TrnNo " + Req.TrnNo, ControllerName, ex);
                throw ex;
            }
        }
        public BizResponse MarkSystemFailTransaction(enErrorCode ErrorCode,string Param2="",string Param3="")
        {
            try
            {
                Req.Status = enTransactionStatus.SystemFail;
                Req.StatusCode = Convert.ToInt64(ErrorCode);
                InsertTransactionInQueue();
                InsertTradeStopLoss(0);
                try//as some para null in starting so error occured here ,only in case of system fail
                {
                    InsertTradeTransactionInQueue();
                }
                catch (Exception ex)
                {
                    Task.Run(() => HelperForLog.WriteErrorLog("MarkSystemFailTransaction Trade TQ Error:##TrnNo " + Req.TrnNo, ControllerName, ex));
                }
                //DI of SMS here
                //Uday 06-12-2018  Send SMS When Transaction is Failed
                SMSSendTransactionHoldOrFailed(Newtransaction.Id, Newtransaction.MemberMobile, Req.Price, Req.Qty, 2);

                //Uday 06-12-2018  Send Email When Transaction is Failed
                EmailSendTransactionHoldOrFailed(Newtransaction.Id, Newtransaction.MemberID + "",Req.PairID,Req.Qty, Newtransaction.TrnDate + "", Req.Price, 0, 2, Convert.ToInt16(Req.ordertype),Convert.ToInt16(Req.TrnType));

                if (Req.ISOrderBySystem == 1 && Req.ordertype == enTransactionMarketType.STOP_Limit && Req.IsWithoutAmtHold == 1)//Rita 29-4-19 does not send notification to user
                {
                    goto skipNotofication;
                }
                try
                {
                    //Rita 26-11-2018 add Activity Notifiation v2
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    //notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);
                    notification.MsgCode = Convert.ToInt32(ErrorCode); //komal 05-02-2019 set validation error code
                    notification.Param1 = Req.TrnNo.ToString();
                    notification.Param2 = Param2;
                    notification.Param3 = Param3;
                    notification.Type = Convert.ToInt16(EnNotificationType.Fail);
                    _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2);//Req.accessToken
                    //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("MarkSystemFailTransaction Notification Send " + notification.MsgCode, ControllerName, "##TrnNo:" + Newtransaction.Id, Helpers.UTC_To_IST()));
                }
                catch (Exception ex)
                {
                    Task.Run(() => HelperForLog.WriteErrorLog("ISignalRService Notification Error-MarkSystemFailTransaction ##TrnNo:" + Newtransaction.Id, ControllerName, ex));
                }
                skipNotofication:
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.Fail, ErrorCode = ErrorCode });
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("MarkSystemFailTransaction:##TrnNo " + Req.TrnNo, ControllerName, ex));
                return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError, ErrorCode = enErrorCode.TransactionInsertInternalError });
            }
        }
        public async Task InsertTransactionInQueue()//ref long TrnNo
        {
            try
            {
                Newtransaction = new TransactionQueueMargin()
                {
                    TrnDate = Helpers.UTC_To_IST(),
                    //GUID = Guid.NewGuid(),
                    GUID = Req.GUID,
                    TrnMode = Req.TrnMode,
                    TrnType = Convert.ToInt16(Req.TrnType),
                    MemberID = Req.MemberID,
                    MemberMobile = Req.MemberMobile,
                    TransactionAccount = Req.TransactionAccount,
                    SMSCode = Req.SMSCode,
                    Amount = Req.Amount,
                    Status = Convert.ToInt16(Req.Status),
                    StatusCode = Req.StatusCode,
                    StatusMsg = Req.StatusMsg,
                    TrnRefNo = Req.TrnRefNo,
                    AdditionalInfo = Req.AdditionalInfo,
                    DebitAccountID = Req.DebitAccountID//rita 03-12-18 added as required in withdraw process
                };
                Newtransaction = _TransactionRepository.Add(Newtransaction);
                Req.TrnNo = Newtransaction.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertTransactionInQueue:##TrnNo " + Req.TrnNo, ControllerName, ex);                
                throw ex;
            }

        }
        public async Task InsertTradeTransactionInQueue()
        {
            try
            {
                NewTradetransaction = new TradeTransactionQueueMargin()
                {
                    TrnDate = Helpers.UTC_To_IST(),
                    TrnType = Convert.ToInt16(Req.TrnType),
                    TrnTypeName = _TradeTransactionObj.TrnTypeName,
                    MemberID = Req.MemberID,
                    PairID = Req.PairID,
                    PairName = _TradeTransactionObj.PairName,
                    OrderWalletID = _TradeTransactionObj.OrderWalletID,
                    DeliveryWalletID = _TradeTransactionObj.DeliveryWalletID,
                    OrderAccountID = Req.DebitAccountID,
                    DeliveryAccountID = Req.CreditAccountID,
                    BuyQty = _TradeTransactionObj.BuyQty,
                    BidPrice = _TradeTransactionObj.BidPrice,
                    SellQty = _TradeTransactionObj.SellQty,
                    AskPrice = _TradeTransactionObj.AskPrice,
                    Order_Currency = _TradeTransactionObj.Order_Currency,
                    OrderTotalQty = _TradeTransactionObj.OrderTotalQty,
                    Delivery_Currency = _TradeTransactionObj.Delivery_Currency,
                    DeliveryTotalQty = _TradeTransactionObj.DeliveryTotalQty,
                    SettledBuyQty = _TradeTransactionObj.SettledBuyQty,
                    SettledSellQty = _TradeTransactionObj.SettledSellQty,
                    Status = Convert.ToInt16(Req.Status),
                    StatusCode = Req.StatusCode,
                    StatusMsg = Req.StatusMsg,
                    ordertype = Convert.ToInt16(Req.ordertype),
                    TrnNo = Req.TrnNo,//NewTradetransactionReq.TrnNo,
                    IsAPITrade = 0,
                    IsExpired = 0,//Rita 30-1-19 for API level changes
                    APIStatus = "",
                    IsAPICancelled = 0,
                    IsWithoutAmtHold = Req.IsWithoutAmtHold,//rita 29-3-19 added for auto order by system with no HOLD Amount
                    ISOrderBySystem = Req.ISOrderBySystem,
                    CreatedBy = Req.ISOrderBySystem == 1 ? Convert.ToInt64(Req.TransactionAccount) : Req.MemberID,//rita 4-2-19 system order create from which Trnno
                };
                NewTradetransaction = _TradeTransactionRepository.Add(NewTradetransaction);
            }
            catch (Exception ex)
            {
               HelperForLog.WriteErrorLog("InsertTradeTransactionInQueue:##TrnNo " + Req.TrnNo, ControllerName, ex);
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
                throw ex;
            }

        }
        public async Task InsertTradeStopLoss(decimal LTP)
        {
            try
            {
                TradeStopLossObj = new TradeStopLossMargin()
                {                  
                    ordertype = Convert.ToInt16(Req.ordertype),
                    StopPrice = Req.StopPrice,
                    Status = Convert.ToInt16(enTransactionStatus.Success),
                    LTP = LTP,
                    PairID=Req.PairID,
                    TrnNo = Req.TrnNo,
                    ISFollowersReq=Req.ISFollowersReq,//Rita 12-1-19 main req always 0
                    FollowingTo =Req.FollowingTo,//Rita 12-1-19 main req always 0
                    LeaderTrnNo = Req.LeaderTrnNo,//Rita 21-1-19 main req always 0
                    FollowTradeType = Req.FollowTradeType//Rita 22-1-19 main req always blank
                };
                if (Req.ordertype == enTransactionMarketType.STOP_Limit)
                {
                    if (Req.StopPrice <= LTP)//250 - 300 Low
                    {
                        if (Req.StopPrice == LTP)
                            STOPLimitWithSameLTP = 1;

                        TradeStopLossObj.RangeMin = Req.StopPrice;
                        TradeStopLossObj.RangeMax = LTP;
                        TradeStopLossObj.MarketIndicator = 0;
                    }
                    else if (Req.StopPrice > LTP)//300 - 350 High
                    {
                        TradeStopLossObj.RangeMin = LTP;
                        TradeStopLossObj.RangeMax = Req.StopPrice;
                        TradeStopLossObj.MarketIndicator = 1;
                    }
                }               
                TradeStopLossObj =_TradeStopLoss.Add(TradeStopLossObj);
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommSuccessMsgInternal, ReturnCode = enResponseCodeService.Success });
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("InsertTradeStopLoss:##TrnNo " + Req.TrnNo, ControllerName, ex));
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
                throw ex;
            }

        }
        #endregion        

        #region RegionProcessTransaction        
        public async void MarkTransactionSystemFail(string StatusMsg, enErrorCode ErrorCode)
        {
            try
            {
                //var Txn = _TransactionRepository.GetById(Req.TrnNo);
                Newtransaction.MakeTransactionSystemFail();
                Newtransaction.SetTransactionStatusMsg(StatusMsg);
                Newtransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
                _TransactionRepository.UpdateAsync(Newtransaction);

                //var TradeTxn = _TradeTransactionRepository.GetById(Req.TrnNo);
                NewTradetransaction.MakeTransactionSystemFail();
                NewTradetransaction.SetTransactionStatusMsg(StatusMsg);
                NewTradetransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
                _TradeTransactionRepository.Update(NewTradetransaction);
                try
                {
                    //Rita 26-11-2018 add Activity Notifiation v2
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);

                    if (ErrorCode == enErrorCode.sp_InsufficientBalanceForCharge)//Rita 13-03-19 In this case only send diff ErrorCode as per front and wallet
                        notification.MsgCode = Convert.ToInt32(ErrorCode);

                    //notification.MsgCode = Convert.ToInt32(ErrorCode);
                    notification.Param1 = Req.TrnNo.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Fail);
                    _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(),2);//Req.accessToken
                   //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("MarkTransactionSystemFail Notification Send " + notification.MsgCode, ControllerName, "##TrnNo:" + Newtransaction.Id, Helpers.UTC_To_IST()));
                }
                catch (Exception ex)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ISignalRService Notification Error-MarkTransactionSystemFail", ControllerName, ex.Message + "##TrnNo:" + NewTradetransaction.TrnNo, Helpers.UTC_To_IST()));
                }
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("MarkTransactionSystemFail:##TrnNo " + Req.TrnNo, ControllerName, ex));
                // throw ex;
            }
        }
        public async Task MarkTransactionHold(string StatusMsg, enErrorCode ErrorCode)
        {
            try
            {
                if (Newtransaction.Status != 0)//Rita 22-2-19 this update after settlement so overrights status, error solved
                    return;

                Newtransaction.MakeTransactionHold();
                Newtransaction.SetTransactionStatusMsg(StatusMsg);
                Newtransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
                _TransactionRepository.UpdateAsync(Newtransaction);

                //var Txn = _TransactionRepository.GetById(Req.TrnNo);
                //rita 28-12-18 remove active inactive as txn considers in settlement time
                //if (Req.ordertype == enTransactionMarketType.STOP_Limit && STOPLimitWithSameLTP == 0)//Rita 26-12-18 for STOP & limit Order
                //{
                //    NewTradetransaction.MakeTransactionInActive();
                //}
                //else
                //{
                //    NewTradetransaction.MakeTransactionHold();
                //}
                if (NewTradetransaction.Status != 0)
                    return;

                NewTradetransaction.MakeTransactionHold();
                NewTradetransaction.SetTransactionStatusMsg(StatusMsg);
                NewTradetransaction.SetTransactionCode(Convert.ToInt64(ErrorCode));
                if (NewTradetransaction.Status == 1)
                    return;
                _TradeTransactionRepository.Update(NewTradetransaction);

                //if (Req.ordertype == enTransactionMarketType.STOP_Limit && STOPLimitWithSameLTP == 0)//Rita 26-12-18 for STOP & limit Order
                //{
                //    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                //    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRTrnSuccessfullyCreated);
                //    notification.Param1 = Req.Price.ToString();
                //    notification.Param2 = Req.Qty.ToString();
                //    notification.Type = Convert.ToInt16(EnNotificationType.Success);
                //    _ISignalRService.SendActivityNotificationV2(notification, Req.MemberID.ToString(), 2);//Req.accessToken    
                //    return;//for Inactive Order no need to send Book,history etc
                //}

                try
                {
                    var CopyNewtransaction = new TransactionQueueMargin();
                    CopyNewtransaction = (TransactionQueueMargin)Newtransaction.Clone();
                    //CopyNewtransaction.MakeTransactionHold();

                    var CopyNewTradetransaction = new TradeTransactionQueueMargin();
                    CopyNewTradetransaction = (TradeTransactionQueueMargin)NewTradetransaction.Clone();
                    //CopyNewTradetransaction.MakeTransactionHold();
                    //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ISignalRService", ControllerName, "parallel execution pre ##TrnNo:",Helpers.UTC_To_IST()));
                    Parallel.Invoke(() => _ISignalRService.OnStatusHoldMargin(Convert.ToInt16(enTransactionStatus.Success), CopyNewtransaction, CopyNewTradetransaction,"", TradeStopLossObj.ordertype));
                    //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ISignalRService", ControllerName, "parallel execution complete ##TrnNo:",Helpers.UTC_To_IST()));

                    //Uday 06-12-2018  Send SMS When Transaction is Hold
                    SMSSendTransactionHoldOrFailed(Newtransaction.Id, Newtransaction.MemberMobile, Req.Price, Req.Qty, 1);
                }
                catch (Exception ex)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ISignalRService", ControllerName, "Trading Hold Error " + ex.Message + "##TrnNo:" + NewTradetransaction.TrnNo, Helpers.UTC_To_IST()));
                }
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("MarkTransactionHold:##TrnNo " + Req.TrnNo, ControllerName, ex));
                throw ex;
            }
        }        
        #endregion

        #region Settlement Insert Data        
        public async Task InsertSellerList()
        {
            try
            {
                TradeSellerListObj = new TradeSellerListMarginV1()
                {
                    CreatedBy = Req.MemberID,
                    TrnNo = Req.TrnNo,
                    PairID = Req.PairID,
                    PairName = _TradeTransactionObj.PairName,
                    Price = Req.Price,
                    Qty = Req.Qty,
                    ReleasedQty = Req.Qty,
                    SelledQty = 0,
                    RemainQty = Req.Qty,
                    IsProcessing = 1,
                    OrderType = Convert.ToInt16(Req.ordertype),
                    Status = Convert.ToInt16(enTransactionStatus.Initialize),//txn type status
                    IsAPITrade = 0,//Rita 30-1-19 for API level changes in settlement , do not pick in local settlement
                };
                if (Req.ordertype == enTransactionMarketType.STOP_Limit && STOPLimitWithSameLTP == 0)
                {
                    //TradeSellerListObj.Status = Convert.ToInt16(enTransactionStatus.InActive);
                    TradeSellerListObj.IsProcessing = 0;
                }
                //TradeSellerListObj =_TradeSellerList.Add(TradeSellerListObj);
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommSuccessMsgInternal, ReturnCode = enResponseCodeService.Success });
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("Tradepoolmaster:##TrnNo " + Req.TrnNo, ControllerName, ex));
                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
                throw ex;
            }
        }
        
        public async Task InsertBuyerList()
        {
            try
            {
                TradeBuyerListObj = new TradeBuyerListMarginV1()
                {
                    CreatedBy = Req.MemberID,
                    TrnNo = Req.TrnNo,
                    PairID = Req.PairID,
                    PairName = _TradeTransactionObj.PairName,
                    Price = Req.Price,
                    Qty = Req.Qty, //same as request as one entry per one request
                    DeliveredQty = 0,
                    RemainQty = Req.Qty,
                    IsProcessing = 1,
                    OrderType = Convert.ToInt16(Req.ordertype),
                    Status = Convert.ToInt16(enTransactionStatus.Initialize),//txn type status
                    IsAPITrade = 0,//Rita 30-1-19 for API level changes in settlement , do not pick in local settlement
                };
                if (Req.ordertype == enTransactionMarketType.STOP_Limit && STOPLimitWithSameLTP == 0)
                {
                    TradeBuyerListObj.IsProcessing = 0;
                }
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("InsertBuyerList:##TrnNo " + Req.TrnNo, ControllerName, ex));
                throw ex;
            }

        }

        public async Task<BizResponse> TradingDataInsert(BizResponse _Resp)
        {
            try
            {                
                _Resp.ReturnMsg = "success";
                _Resp.ReturnCode = enResponseCodeService.Success;
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("TradingDataInsert:##TrnNo " + Req.TrnNo, ControllerName, ex));
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = ex.Message;
            }
            return _Resp;
        }

        public async Task<BizResponse> TradingDataInsertV2(BizResponse _Resp)
        {
            try
            {
                foreach (TransactionProviderResponse Provider in TxnProviderList)//route transcation on every API - khushali
                {
                    switch (Enum.Parse<enWebAPIRouteType>(Provider.ServiceProID.ToString()))
                    {
                        case enWebAPIRouteType.TradeServiceLocal:
                           // 
                            break;
                        case enWebAPIRouteType.LiquidityProvider:

                            Newtransaction.SetServiceProviderData(Provider.ServiceID, Provider.ServiceProID, Provider.ProductID, Provider.RouteID);
                            _TransactionRepository.Update(Newtransaction);
                            break;
                        case enWebAPIRouteType.MarketData:

                            break;
                        case enWebAPIRouteType.TransactionAPI:

                            break;
                    }
                }
                _Resp.ReturnMsg = "success";
                _Resp.ReturnCode = enResponseCodeService.Success;
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("TradingDataInsert:##TrnNo " + Req.TrnNo, ControllerName, ex));
                _Resp.ReturnCode = enResponseCodeService.Fail;
                _Resp.ReturnMsg = ex.Message;
            }
            return _Resp;
        }
        #endregion

        #region Send SMS And Email
        public async Task SMSSendTransactionHoldOrFailed(long TrnNo, string MobileNumber, decimal Price, decimal Qty, int type)
        {
            try
            {
                if (!string.IsNullOrEmpty(MobileNumber))
                {
                    TemplateMasterData SmsData = new TemplateMasterData();
                    SendSMSRequest SendSMSRequestObj = new SendSMSRequest();
                    ApplicationUser User = new ApplicationUser();

                    CommunicationParamater communicationParamater = new CommunicationParamater();
                    communicationParamater.Param1 = TrnNo + "";
                    communicationParamater.Param2 = Price + "";
                    communicationParamater.Param3 = Qty + "";

                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SendSMSTransaction - SMSSendTransactionHoldOrFailed", ControllerName, " ##TrnNo : " + TrnNo + " ##MobileNo : " + MobileNumber + " ##Price : " + Price + " ##Qty : " + Qty + " ##Type : " + type, Helpers.UTC_To_IST()));

                    if (type == 1) // Transaction Created
                    {
                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_TransactionCreated, communicationParamater, enCommunicationServiceType.SMS).Result;
                    }
                    else if (type == 2) // Transaction Failed
                    {
                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_TransactionFailed, communicationParamater, enCommunicationServiceType.SMS).Result;
                    }

                    if (SmsData != null)
                    {
                        if (SmsData.IsOnOff == 1)
                        {
                            SendSMSRequestObj.Message = SmsData.Content;
                            SendSMSRequestObj.MobileNo = Convert.ToInt64(MobileNumber);
                            _pushSMSQueue.Enqueue(SendSMSRequestObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("SMSSendTransactionHold:##TrnNo " + TrnNo, ControllerName, ex));
            }
        }

        public async Task EmailSendTransactionHoldOrFailed(long TrnNo, string UserId,long pairid,decimal qty, string datetime, decimal price, decimal fee, int Type,short OrderType,short TrnType)
        {
            try
            {
                SendEmailRequest Request = new SendEmailRequest();
                ApplicationUser User = new ApplicationUser();
                TemplateMasterData EmailData = new TemplateMasterData();
                CommunicationParamater communicationParamater = new CommunicationParamater();

                User = await _userManager.FindByIdAsync(UserId);
                if (!string.IsNullOrEmpty(User.Email))
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SendEmailTransaction - EmailSendWithdrwalTransaction", ControllerName, " ##TrnNo : " + TrnNo  + " ##Type : " + Type, Helpers.UTC_To_IST()));

                    var pairdata = _trnMasterConfiguration.GetTradePairMasterMargin().Where(x => x.Id == pairid).FirstOrDefault();
                    
                    if(pairdata != null)
                    {
                        communicationParamater.Param1 = pairdata.PairName + "";
                        communicationParamater.Param3 = pairdata.PairName.Split("_")[1];
                    }

                    communicationParamater.Param8 = User.Name + "";
                    communicationParamater.Param2 = Helpers.DoRoundForTrading(qty, 18).ToString();
                    communicationParamater.Param4 = datetime;
                    communicationParamater.Param5 = Helpers.DoRoundForTrading(price, 18).ToString();
                    communicationParamater.Param6 = Helpers.DoRoundForTrading(fee, 18).ToString();
                    communicationParamater.Param7 = Helpers.DoRoundForTrading(0, 18).ToString();  //Uday 01-01-2019  In failed transaction final price as 0
                    communicationParamater.Param9 = ((enTransactionMarketType)OrderType).ToString();  //Uday 01-01-2019 Add OrderType In Email
                    communicationParamater.Param10 = ((enTrnType)TrnType).ToString();  //Uday 01-01-2019 Add TranType In Email
                    communicationParamater.Param11 = TrnNo.ToString(); //Uday 01-01-2019 Add TrnNo In Email

                    //if (CancelType == 1) // Hold
                    //{
                    //    EmailData = _messageService.SendMessageAsync(EnTemplateType.EMAIL_OrderCancel, communicationParamater, enCommunicationServiceType.Email).Result;
                    //}
                    if (Type == 2) // Failed
                    {
                        EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_TransactionFailed, communicationParamater, enCommunicationServiceType.Email).Result;
                    }
                  
                    if (EmailData != null)
                    {
                        Request.Body = EmailData.Content;
                        Request.Subject = EmailData.AdditionalInfo;
                        Request.Recepient = User.Email;
                        Request.EmailType = 0;
                        _pushNotificationsQueue.Enqueue(Request);
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Run(() => HelperForLog.WriteErrorLog("EmailSendCancelTransaction:##TrnNo " + TrnNo, ControllerName, ex));
            }
        }
        #endregion
    }
}
