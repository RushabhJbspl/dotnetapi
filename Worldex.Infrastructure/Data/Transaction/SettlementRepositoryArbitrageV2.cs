//using Worldex.Core.ApiModels;
//using Worldex.Core.ApiModels.Chat;
//using Worldex.Core.Entities;
//using Worldex.Core.Entities.Transaction;
//using Worldex.Core.Entities.User;
//using Worldex.Core.Enums;
//using Worldex.Core.Helpers;
//using Worldex.Core.Interfaces;
//using Worldex.Core.Interfaces.Repository;
//using Worldex.Core.ViewModels;
//using Worldex.Core.ViewModels.Wallet;
//using Worldex.Infrastructure.BGTask;
//using Worldex.Infrastructure.DTOClasses;
//using Worldex.Infrastructure.Interfaces;
//using Worldex.Infrastructure.Services;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;

//namespace Worldex.Infrastructure.Data.Transaction
//{
//    public class SettlementRepositoryArbitrageV2: ISettlementRepositoryArbitrageV1<BizResponse>
//    {
//        private readonly WorldexContext _dbContext;
//        private readonly WorldexContext _dbContext1;
//        private readonly WorldexContext _dbContext2;
//        private readonly ICommonRepository<TradeBuyerListArbitrageV1> _TradeBuyerList;
//        private readonly ICommonRepository<TradeSellerListArbitrageV1> _TradeSellerList;
//        private readonly ICommonRepository<TradePairStasticsArbitrage> _tradePairStastics;
//        private readonly ITransactionRepository<TradeStopLossArbitrage> _TradeStopLoss;
//        private readonly ISignalRService _ISignalRService;
//        private readonly IFrontTrnService _IFrontTrnService;
//        private readonly SettledTradeTransactionQueueArbitrage _SettledTradeTQ;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue; //24-11-2018 komal make Email Enqueue
//        private readonly IArbitrageWalletService _WalletService;
//        string ControllerName = "SettlementRepositoryArbitrageV2";
//        TradePoolQueueArbitrageV1 TradePoolQueueObj;     
//        private readonly IMessageConfiguration _messageConfiguration;
//        private readonly IMessageService _messageService;
//        private readonly IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
//        WalletDrCrResponse CreditWalletResult;//Rita 25-3-19 for by ref not possible
//        //private readonly IGetWebRequest _IGetWebRequest;
//        //private readonly IWebApiSendRequest _IWebApiSendRequest;
//        //WebApiParseResponse _WebApiParseResponseObj;

//        public SettlementRepositoryArbitrageV2(WorldexContext dbContext,
//            ICommonRepository<TransactionQueue> TransactionRepository, IArbitrageWalletService WalletService,
//            ISignalRService ISignalRService, IFrontTrnService IFrontTrnService, ITransactionRepository<TradeStopLossArbitrage> TradeStopLoss,
//            IMessageConfiguration messageConfiguration, UserManager<ApplicationUser> userManager,
//            ICommonRepository<TradeBuyerListArbitrageV1> TradeBuyerList, ICommonRepository<TradeSellerListArbitrageV1> TradeSellerList,
//            IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, SettledTradeTransactionQueueArbitrage SettledTradeTQ,
//            ICommonRepository<TradePairStasticsArbitrage> tradePairStastics, IMessageService messageService, IPushNotificationsQueue<SendSMSRequest> pushSMSQueue
//            )//IGetWebRequest IGetWebRequest, IWebApiSendRequest IWebApiSendRequest, WebApiParseResponse WebApiParseResponseObj
//        {
//            _dbContext = dbContext;
//            _dbContext1 = dbContext;
//            _dbContext2 = dbContext;
//            _TradeBuyerList = TradeBuyerList;
//            _TradeSellerList = TradeSellerList;
//            _WalletService = WalletService;
//            _ISignalRService = ISignalRService;
//            _IFrontTrnService = IFrontTrnService;
//            _TradeStopLoss = TradeStopLoss;
//            _messageConfiguration = messageConfiguration;
//            _userManager = userManager;
//            _pushNotificationsQueue = pushNotificationsQueue;
//            _SettledTradeTQ = SettledTradeTQ;
//            _tradePairStastics = tradePairStastics;
//            _messageService = messageService;
//            _pushSMSQueue = pushSMSQueue;
//            //_IGetWebRequest = IGetWebRequest;
//            //_IWebApiSendRequest = IWebApiSendRequest;
//            //_WebApiParseResponseObj = WebApiParseResponseObj;
//        }

//        #region ==============================PROCESS SETLLEMENT========================      

//        public async Task<BizResponse> PROCESSSETLLEMENTBuy(BizResponse _Resp, TradeTransactionQueueArbitrage TradeTransactionQueueObj, TransactionQueueArbitrage TransactionQueueObj, TradeStopLossArbitrage TradeStopLossObj, TradeBuyerListArbitrageV1 CurrentTradeBuyerListObj,string accessToken = "",short IsCancel=0)
//        {           
//            short TrackBit=0;
//            short TrackDebitBit = 0;
//            decimal GSettlementQty = 0;
//            decimal GBaseCurrQty = 0;
//            TradeSellerListArbitrageV1 CurrenetSellerList=new TradeSellerListArbitrageV1();
//            //List<ProccessBuyWith> ProccessBuyWithList=new List<ProccessBuyWith>();
            
//            try
//            {
//                Task<IEnumerable<TradeSellerListArbitrageV1>> MatchSellerListBaseAllResult = _TradeSellerList.FindByAsync(e=>(e.Status == Convert.ToInt16(enTransactionStatus.Initialize) || e.Status == Convert.ToInt16(enTransactionStatus.Hold)) && e.IsAPITrade==0);//Rita 30-1-19 do not pick API trade
//                if (TradeTransactionQueueObj.IsCancelled == 1 || IsCancel == 1)
//                {
//                    _Resp.ErrorCode = enErrorCode.CancelOrder_OrderalreadyCancelled;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Transaction Cancellation request is already in processing";
//                    return _Resp;
//                }

//                if (CurrentTradeBuyerListObj.RemainQty == 0)
//                {
//                    _Resp.ErrorCode = enErrorCode.Settlement_AlreadySettled;
//                    _Resp.ReturnCode = enResponseCodeService.Success;
//                    _Resp.ReturnMsg = "ALready Settled";
//                    return _Resp;
//                }
//                _Resp.ErrorCode = enErrorCode.TransactionInsertSuccess;
//                _Resp.ReturnCode = enResponseCodeService.Success;
//                _Resp.ReturnMsg = "Initialise and settlement start";

//                IEnumerable<TradeSellerListArbitrageV1> MatchSellerListBase;
//                IEnumerable<TradeSellerListArbitrageV1> MatchSellerListBaseAll = await MatchSellerListBaseAllResult;
//                TradePairStasticsArbitrage pairStastics = await _dbContext.Set<TradePairStasticsArbitrage>().FirstOrDefaultAsync(pair => pair.PairId == CurrentTradeBuyerListObj.PairID);

//                if (CurrentTradeBuyerListObj.OrderType == 2)
//                {
//                    CurrentTradeBuyerListObj.Price = pairStastics.LTP;
//                    //TradeTransactionQueueObj.BidPrice = pairStastics.LTP; //rita 17-12-18 do not update as needed in TQ Zero for Buyer and seller List
//                }
//                //rita 3-4-19 here update LTP only for related pair data only, added pair condition
//                (from u in MatchSellerListBaseAll where u.OrderType == 2 && u.PairID == CurrentTradeBuyerListObj.PairID select u).ToList()
//                        .ForEach(u => u.Price = pairStastics.LTP);

//                MatchSellerListBase = MatchSellerListBaseAll.Where(item => (item.Price <= CurrentTradeBuyerListObj.Price)//|| item.OrderType==2 Rita 28-12-18 after updating price no need of this //&& item.IsProcessing == 0
//                                                     && item.PairID == CurrentTradeBuyerListObj.PairID && item.OrderType != 3
//                                                     //&& (item.Status == Convert.ToInt16(enTransactionStatus.Initialize) || item.Status == Convert.ToInt16(enTransactionStatus.Hold))
//                                                     && item.RemainQty > 0).OrderBy(x => x.Price).ThenBy(x => x.TrnNo);
//                if (CurrentTradeBuyerListObj.OrderType == 3)//SPOT Order,check Total Qty
//                {
//                    //decimal RemainQtySum1 = MatchSellerListBase.Sum(e=>e.RemainQty);//https://www.toptal.com/c-sharp/top-10-mistakes-that-c-sharp-programmers-make
//                    decimal RemainQtySum = Helpers.DoRoundForTrading(Enumerable.Sum(MatchSellerListBase, a => a.RemainQty),18);

//                    if (CurrentTradeBuyerListObj.RemainQty> RemainQtySum)//No sufficient Qty Found
//                    {
//                        goto SkipLoop;
//                    }
//                }
//                foreach (TradeSellerListArbitrageV1 SellerList in MatchSellerListBase)//MatchSellerList
//                {
//                    Task<bool> IsProResult = CheckIsProcessingBUY(CurrentTradeBuyerListObj.TrnNo, SellerList, pairStastics.LTP);

//                    TrackDebitBit = 0;
//                    CurrenetSellerList = SellerList;
//                    decimal SettlementQty = 0;
//                    decimal SettlementPrice = 0;
//                    decimal BaseCurrQty = 0;
//                    if(await IsProResult==false)
//                    {
//                        continue;
//                    }
//                    TrackBit = 1;//rita 26-12-18 move here if continue then not proceed this record                   
             
//                    SellerList.IsProcessing = 1;
//                    SellerList.UpdatedBy = CurrentTradeBuyerListObj.TrnNo;//rita 8-1-19 for log
//                    SellerList.CreatedBy = 800;//rita 10-1-19 for sp and code separate                  
//                     _TradeSellerList.UpdateField(SellerList,e=>e.IsProcessing, e => e.Price, e => e.UpdatedBy,e=>e.CreatedBy);
//                    TradeTransactionQueueArbitrage TradeTQMakerObj = _dbContext2.Set<TradeTransactionQueueArbitrage>().FirstOrDefault(item => item.TrnNo == SellerList.TrnNo);
//                    _dbContext1.Entry(SellerList).Reload();//Rita 18-1-19 here we get lates remain Qty
//                    Task<TransactionQueueArbitrage> TQMakerObjResult = _dbContext1.Set<TransactionQueueArbitrage>().FirstOrDefaultAsync(item => item.Id == SellerList.TrnNo);
//                    //====================================Partial SETTLEMENT TO MEMBER
//                    if (SellerList.RemainQty < CurrentTradeBuyerListObj.RemainQty && SellerList.RemainQty != 0)//Do not Release Qty here //rita 8-2-18 release may occured so add <>0 Condition//Do not Release Qty here
//                    {
//                        SettlementQty = Helpers.DoRoundForTrading(SellerList.RemainQty, 18);//Take all Seller's Qty
//                        GSettlementQty = SettlementQty;
//                        SettlementPrice = SellerList.Price;
//                        BaseCurrQty = Helpers.DoRoundForTrading(SettlementQty * SettlementPrice, 18);
//                        GBaseCurrQty = BaseCurrQty;

//                        TransactionQueueArbitrage TQMakerObj = await TQMakerObjResult;
//                        if (BaseCurrQty == 0)//Rita 29-1-19 release amount , here Qty used of Seller so make fail seller
//                        {
//                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTBuy Continue Seller has BaseCurrQty:" + BaseCurrQty + " SettlementQty:" + SettlementQty + " SettlementPrice:" + SettlementPrice + " Maker TrnNo: " + SellerList.TrnNo, ControllerName, "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
//                            await ReleaseWalletAmountSell(SellerList, TQMakerObj, TradeTQMakerObj,1);
//                            continue;
//                        }

//                        var CallspResult = Callsp_TradeSettlement(CurrentTradeBuyerListObj.TrnNo, SellerList.TrnNo, SettlementQty, BaseCurrQty, SettlementPrice, 11,1);
                        
//                        decimal TakeDisc = 0;
//                        //rita 4-2-19 no need to change as settlement price of maker-seller and taker settle on same price
//                        await InsertTradePoolQueue(TransactionQueueObj.MemberID, CurrentTradeBuyerListObj.PairID, SellerList.TrnNo, SettlementQty, SettlementPrice, CurrentTradeBuyerListObj.TrnNo, SettlementQty, SettlementPrice, TakeDisc, 0, "SELL", "BUY");// SellerList.RemainQty//CurrentTradeBuyerListObj.Price , buy on price of seller's price 
                                                
//                        if (await CallspResult == false)
//                        {
//                            _dbContext2.Entry(SellerList).Reload();//Rita 11-1-19 reload as incase of fail, currecnt IsProcessing set 1 as above assign
//                            continue;
//                        }

//                        TrackDebitBit = 1;//refund if error occured
//                        _dbContext2.Entry(SellerList).Reload();
//                        _dbContext2.Entry(TradeTQMakerObj).Reload();

//                        CurrentTradeBuyerListObj.RemainQty = Helpers.DoRoundForTrading(CurrentTradeBuyerListObj.RemainQty - SettlementQty, 18);
//                        CurrentTradeBuyerListObj.DeliveredQty = Helpers.DoRoundForTrading(CurrentTradeBuyerListObj.DeliveredQty + SettlementQty, 18);
                       
//                        Task<bool> WalletResult = WalletProcessBuy(TradeTransactionQueueObj, TradeTQMakerObj, SettlementPrice, SettlementQty, BaseCurrQty, 0, 1, TransactionQueueObj.TrnMode, CreditWalletResult);
                      
//                        Task<SettledTradeTransactionQueueArbitrage> SettledTradeTQMakerResult = MakeTransactionSettledEntry(TradeTQMakerObj, SettlementPrice);                        

//                        TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
//                        TransactionQueueObj.SetTransactionStatusMsg("Partial Settlement Done");
//                        TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
//                        TradeTransactionQueueObj.SetTransactionStatusMsg("Partial Settlement Done");
//                        TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + SettlementQty,18);
//                        TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + BaseCurrQty,18);
                          
//                        TQMakerObj.MakeTransactionSuccess();
//                        TQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
//                        TQMakerObj.SetTransactionStatusMsg("Full Settlement Done");
//                        SettledTradeTransactionQueueArbitrage SettledTradeTQMakerObj = await SettledTradeTQMakerResult;
//                        Task<TradeStopLossArbitrage> MakeTradeStopLossObjResult = _dbContext.Set<TradeStopLossArbitrage>().FirstOrDefaultAsync(e => e.TrnNo == TQMakerObj.Id);
                        

//                        if (await WalletResult == false)
//                        {
//                            await MakeTradeStopLossObjResult;//for no contex error
//                            var CallspReverseResult = Callsp_TradeSettlement(CurrentTradeBuyerListObj.TrnNo, SellerList.TrnNo, SettlementQty, BaseCurrQty, SettlementPrice, 11, 2, Convert.ToInt64(CreditWalletResult.ErrorCode));
//                            _dbContext.Entry(CurrentTradeBuyerListObj).Reload();
//                            _dbContext.Entry(TransactionQueueObj).Reload();
//                            _dbContext.Entry(TradeTransactionQueueObj).Reload();
                         
//                            var CallspReverseResult1 =await CallspReverseResult;
//                            _dbContext.Entry(SellerList).Reload();
//                            _dbContext.Entry(TradeTQMakerObj).Reload();
//                            _dbContext.Entry(TQMakerObj).Reload();
//                            Task WalletFail = WalletFailOrderProcessBUY(CurrentTradeBuyerListObj.TrnNo, SellerList, TradeTQMakerObj, TQMakerObj, CreditWalletResult);
//                            TrackDebitBit = 0;
//                            _Resp.ErrorCode = enErrorCode.Settlement_PartialSettlementRollback;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Partial Settlement RollBack ##TrnNo " + CurrentTradeBuyerListObj.TrnNo + " With: TrnNo " + SellerList.TrnNo;
//                            Task.Run(()=> HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT reverse result"+ CallspReverseResult1, ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
//                            await WalletFail;
//                        }
//                        else
//                        {
//                            TradeStopLossArbitrage MakeTradeStopLossObj = await MakeTradeStopLossObjResult;//Rita 27-4-19 as second operation error if not wait
//                            _dbContext.Database.BeginTransaction();
                            
//                            _dbContext.Entry(CurrentTradeBuyerListObj).State = EntityState.Modified;
//                            _dbContext.Entry(TransactionQueueObj).State = EntityState.Modified;
//                            _dbContext.Entry(TradeTransactionQueueObj).State = EntityState.Modified;
//                            _dbContext.Entry(TQMakerObj).State = EntityState.Modified;
                          
//                            TradePoolQueueObj.UpdatedDate = Helpers.UTC_To_IST();
//                            TradePoolQueueObj.Status = 1;//rita 23-3-19 update status as entry added from SP
                                                       

//                            _dbContext.SaveChanges();
//                            _dbContext.Database.CommitTransaction();
//                            //Rita 13-2-19 for error  : Violation of PRIMARY KEY constraint 'PK_SettledTradeTransactionQueueArbitrage'. Cannot insert duplicate key in object 'dbo.SettledTradeTransactionQueueArbitrage'. The duplicate key value is
//                            try
//                            {
//                                _dbContext.Set<SettledTradeTransactionQueueArbitrage>().Add(SettledTradeTQMakerObj);
//                                _dbContext.SaveChanges();
//                            }
//                            catch(Exception ex)
//                            {
//                                HelperForLog.WriteErrorLog("PROCESSSETLLEMENTBuy Partial Settle Entry Error:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                            }

//                            TrackDebitBit = 0;
                                                       
//                            Task.Run(() => PartilaSettleBuySignalR(CurrentTradeBuyerListObj.TrnNo, TransactionQueueObj, TradeTransactionQueueObj, TradeStopLossObj, MakeTradeStopLossObj, TQMakerObj, TradeTQMakerObj, accessToken, SettlementPrice,SettlementQty, CurrentTradeBuyerListObj.OrderType));
                                                      
//                            _Resp.ErrorCode = enErrorCode.Settlement_PartialSettlementDone;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Partial Settlement Done of ##TrnNo " + CurrentTradeBuyerListObj.TrnNo + " Settled: " + CurrentTradeBuyerListObj.DeliveredQty + " Remain:" + CurrentTradeBuyerListObj.RemainQty;
//                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));                           
//                        }
//                    }
//                    //====================================FULL SETTLEMENT TO MEMBER
//                    else if (SellerList.RemainQty >= CurrentTradeBuyerListObj.RemainQty && CurrentTradeBuyerListObj.RemainQty != 0)
//                    {
//                        SettlementQty = Helpers.DoRoundForTrading(CurrentTradeBuyerListObj.RemainQty, 18);
//                        GSettlementQty = SettlementQty;
//                        SettlementPrice = SellerList.Price;
//                        BaseCurrQty = Helpers.DoRoundForTrading(SettlementQty * SettlementPrice, 18);
//                        GBaseCurrQty = BaseCurrQty;
//                        short IsFullSettledBit = 0;
//                        if(SellerList.RemainQty == SettlementQty)
//                            IsFullSettledBit = 1;
                     
//                        if (BaseCurrQty == 0)//Rita 29-1-19 release amount , here Qty used of Current Buyer Object , so release
//                        {
//                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTBuy Continue Current Buyer has BaseCurrQty:" + BaseCurrQty + " SettlementQty:" + SettlementQty + " SettlementPrice:" + SettlementPrice + " Maker TrnNo: " + SellerList.TrnNo, ControllerName, "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
//                            await ReleaseWalletAmountBuy(CurrentTradeBuyerListObj, TransactionQueueObj, TradeTransactionQueueObj, 1);
//                            SellerList.IsProcessing = 0;
//                            _TradeSellerList.UpdateField(SellerList, e => e.IsProcessing);
//                            break; //break loop
//                        }

//                        TransactionQueueArbitrage TQMakerObj = await TQMakerObjResult;
//                        var CallspResult = Callsp_TradeSettlement(CurrentTradeBuyerListObj.TrnNo, SellerList.TrnNo, SettlementQty, BaseCurrQty, SettlementPrice, 12,1);
                       
//                        decimal TakeDisc = 0;
//                        await InsertTradePoolQueue(TransactionQueueObj.MemberID, CurrentTradeBuyerListObj.PairID, SellerList.TrnNo, SellerList.RemainQty, SettlementPrice, CurrentTradeBuyerListObj.TrnNo, SettlementQty, SettlementPrice, TakeDisc, 0, "SELL", "BUY");//+ SettlementQty//CurrentTradeBuyerListObj.Price , buy on price of seller's price
                                                
//                        if (await CallspResult == false)
//                        {
//                            _dbContext2.Entry(SellerList).Reload();//Rita 11-1-19 reload as incase of fail, currecnt IsProcessing set 1 as above assign
//                            continue;
//                        }

//                        TrackDebitBit = 1;//refund if error occured
//                        _dbContext2.Entry(SellerList).Reload();
//                        _dbContext2.Entry(TradeTQMakerObj).Reload();

//                        Task<bool> WalletResult = WalletProcessBuy(TradeTransactionQueueObj, TradeTQMakerObj, SettlementPrice, SettlementQty, BaseCurrQty, 1, IsFullSettledBit == 1 ? (short)1 : (short)0, TransactionQueueObj.TrnMode, CreditWalletResult);//SellerList.RemainQty == 0 //Rita 4-1-18 change condition based on bit
                      
//                        TQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
//                        TQMakerObj.SetTransactionStatusMsg("Partial Settlement Done");

//                        if (IsFullSettledBit == 1)
//                        {//Make Seller Order Success
//                            TQMakerObj.MakeTransactionSuccess();
//                            TQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
//                            TQMakerObj.SetTransactionStatusMsg("Full Settlement Done");
//                        }
//                        Task<SettledTradeTransactionQueueArbitrage> SettledTradeTQMakerResult = MakeTransactionSettledEntry(TradeTQMakerObj, SettlementPrice);
                       
//                        CurrentTradeBuyerListObj.RemainQty = Helpers.DoRoundForTrading(CurrentTradeBuyerListObj.RemainQty - SettlementQty,18);
//                        CurrentTradeBuyerListObj.DeliveredQty = Helpers.DoRoundForTrading(CurrentTradeBuyerListObj.DeliveredQty + SettlementQty,18);
//                        CurrentTradeBuyerListObj.MakeTransactionSuccess();
//                        TransactionQueueObj.MakeTransactionSuccess();
//                        TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
//                        TransactionQueueObj.SetTransactionStatusMsg("Full Settlement Done");

//                        TradeTransactionQueueObj.MakeTransactionSuccess();
//                        TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
//                        TradeTransactionQueueObj.SetTransactionStatusMsg("Full Settlement Done");
//                        TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + SettlementQty,18);
//                        TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + BaseCurrQty,18);
//                        Task<SettledTradeTransactionQueueArbitrage> SettledTradeTQResult = MakeTransactionSettledEntry(TradeTransactionQueueObj, SettlementPrice);
                       
//                        Task<TradeStopLossArbitrage> MakeTradeStopLossObjResult = _dbContext.Set<TradeStopLossArbitrage>().FirstOrDefaultAsync(e => e.TrnNo == TQMakerObj.Id);


//                        if (await WalletResult == false)
//                        {
//                            await MakeTradeStopLossObjResult;//for no contex error
//                            var CallspReverseResult = Callsp_TradeSettlement(CurrentTradeBuyerListObj.TrnNo, SellerList.TrnNo, SettlementQty, BaseCurrQty, SettlementPrice, 12, 2, Convert.ToInt64(CreditWalletResult.ErrorCode));
//                            _dbContext.Entry(CurrentTradeBuyerListObj).Reload();
//                            _dbContext.Entry(TransactionQueueObj).Reload();
//                            _dbContext.Entry(TradeTransactionQueueObj).Reload();
                          
//                            var CallspReverseResult1 = await CallspReverseResult;
//                            _dbContext.Entry(SellerList).Reload();
//                            _dbContext.Entry(TradeTQMakerObj).Reload();
//                            _dbContext.Entry(TQMakerObj).Reload();
//                            Task WalletFail =WalletFailOrderProcessBUY(CurrentTradeBuyerListObj.TrnNo, SellerList, TradeTQMakerObj, TQMakerObj, CreditWalletResult);
//                            TrackDebitBit = 0;
//                            _Resp.ErrorCode = enErrorCode.Settlement_FullSettlementRollback;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Full Settlement Rollback ##TrnNo:" + TradeTransactionQueueObj.TrnNo + " With: TrnNo " + SellerList.TrnNo;
//                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT reverse Result " + CallspReverseResult1, ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
//                            await WalletFail;
//                        }
//                        else
//                        {
//                            TradeStopLossArbitrage MakeTradeStopLossObj = await MakeTradeStopLossObjResult;//Rita 27-4-19 as second operation error if not wait
//                            _dbContext.Database.BeginTransaction();
                           
//                            _dbContext.Entry(CurrentTradeBuyerListObj).State = EntityState.Modified;
//                            _dbContext.Entry(TransactionQueueObj).State = EntityState.Modified;
//                            _dbContext.Entry(TradeTransactionQueueObj).State = EntityState.Modified;
//                            TradePoolQueueObj.UpdatedDate = Helpers.UTC_To_IST();
//                            TradePoolQueueObj.Status = 1;//rita 23-3-19 update status as entry added from SP
                          
//                            SettledTradeTransactionQueueArbitrage SettledTradeTQObj = await SettledTradeTQResult;
                                                   
//                            _dbContext.Entry(TQMakerObj).State = EntityState.Modified;
                          
//                            _dbContext.SaveChanges();
//                            _dbContext.Database.CommitTransaction();
//                            TrackDebitBit = 0;
//                            //Rita 13-2-19 for error  : Violation of PRIMARY KEY constraint 'PK_SettledTradeTransactionQueueArbitrage'. Cannot insert duplicate key in object 'dbo.SettledTradeTransactionQueueArbitrage'. The duplicate key value is
//                            try
//                            {
//                                if (IsFullSettledBit == 1)
//                                {
//                                    SettledTradeTransactionQueueArbitrage SettledTradeTQMakerObj = await SettledTradeTQMakerResult;
//                                    _dbContext.Set<SettledTradeTransactionQueueArbitrage>().Add(SettledTradeTQMakerObj);
//                                }
//                                _dbContext.Set<SettledTradeTransactionQueueArbitrage>().Add(SettledTradeTQObj);
//                                _dbContext.SaveChanges();
//                            }
//                            catch (Exception ex)
//                            {
//                                HelperForLog.WriteErrorLog("PROCESSSETLLEMENTBuy FULL Settle Entry Error:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                            }                            
                          
//                            Task.Run(() => FullSettleBuySignalR(CurrentTradeBuyerListObj.TrnNo, TransactionQueueObj, TradeTransactionQueueObj, TradeStopLossObj, MakeTradeStopLossObj, TQMakerObj, TradeTQMakerObj, accessToken, SettlementPrice, SettlementQty, SellerList, CurrentTradeBuyerListObj.OrderType));                            

//                            _Resp.ErrorCode = enErrorCode.Settlement_FullSettlementDone;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Full Settlement Done of ##TrnNo " + CurrentTradeBuyerListObj.TrnNo + " Settled: " + CurrentTradeBuyerListObj.DeliveredQty + " Remain:" + CurrentTradeBuyerListObj.RemainQty;
//                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
//                            break;//record settled                           
//                        }
                       
//                    }
                    
//                }
              
//                SkipLoop:
//                CurrentTradeBuyerListObj.IsProcessing = 0;//Release Buy Order                 
//                _TradeBuyerList.UpdateField(CurrentTradeBuyerListObj, e => e.IsProcessing);              
               
//                if (TrackBit == 0)//No any record Process
//                {
//                    if (CurrentTradeBuyerListObj.OrderType == 3)//Reverse Order , for LP integration remove release call
//                    {
//                        await ReleaseWalletAmountBuy(CurrentTradeBuyerListObj, TransactionQueueObj, TradeTransactionQueueObj);
                        
//                    }
//                    _Resp.ErrorCode = enErrorCode.Settlement_NoSettlementRecordFound;
//                    _Resp.ReturnCode = enResponseCodeService.Success;//Rita 13-6-19 for arbitrage no route so make hold as going in local
//                    _Resp.ReturnMsg = "No Any Match Record Found";
//                }
//                else if (CurrentTradeBuyerListObj.OrderType == 3)
//                {
//                    if (CurrentTradeBuyerListObj.RemainQty != 0)//Revert all process of above
//                    {
//                        //only revert ramain amount
//                        bool ReleaseResult =await ReleaseWalletAmountBuy(CurrentTradeBuyerListObj, TransactionQueueObj, TradeTransactionQueueObj,1);
                                   
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("PROCESSSETLLEMENTBuy:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                CurrentTradeBuyerListObj.IsProcessing = 0;//Release Buy Order             
//                _dbContext.Entry(CurrentTradeBuyerListObj).Property(p=>p.IsProcessing).IsModified = true;
//                _dbContext.SaveChanges();
//                if (CurrenetSellerList.TrnNo!=0)//here null condition not apply as this declare with new tag
//                {
//                    if (TrackDebitBit == 1)//rita 1-1-18 only if data updated
//                    {
//                        bool CallspReverseResult = await Callsp_TradeSettlement(CurrentTradeBuyerListObj.TrnNo, CurrenetSellerList.TrnNo, GSettlementQty, GBaseCurrQty, 0, 11, 2);
//                    }
//                    else
//                    {
//                        CurrenetSellerList.IsProcessing = 0;
//                        _dbContext.Entry(CurrenetSellerList).Property(p => p.IsProcessing).IsModified = true;                        
//                        _dbContext.SaveChanges();
//                    }
//                }
//                if(CurrentTradeBuyerListObj.RemainQty==0)//Rita 8-3-19 full pending then move on next LP
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                else
//                    _Resp.ReturnCode = enResponseCodeService.Success;

//                _Resp.ReturnMsg = ex.Message;
//                _Resp.ErrorCode = enErrorCode.Settlement_SettlementInternalError;
//            }
//            return _Resp;
//        }

//        public async Task<BizResponse> PROCESSSETLLEMENTSell(BizResponse _Resp, TradeTransactionQueueArbitrage TradeTransactionQueueObj, TransactionQueueArbitrage TransactionQueueObj, TradeStopLossArbitrage TradeStopLossObj,TradeSellerListArbitrageV1 CurrentTradeSellerListObj, string accessToken = "", short IsCancel = 0)
//        {
//            short TrackBit = 0;
//            short TrackDebitBit = 0;
//            decimal GSettlementQty =0;
//            decimal GBaseCurrQty = 0;
//            TradeBuyerListArbitrageV1 CurrenetBuyerList = new TradeBuyerListArbitrageV1();
//            //List<ProccessSellWith> ProccessSellWithList = new List<ProccessSellWith>();
//            try
//            {
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTSell", ControllerName, "Start Seller Process" + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
//                Task<IEnumerable<TradeBuyerListArbitrageV1>> MatchSellerListBaseAllResult = _TradeBuyerList.FindByAsync(e => (e.Status == Convert.ToInt16(enTransactionStatus.Initialize) || e.Status == Convert.ToInt16(enTransactionStatus.Hold)) && e.IsAPITrade == 0);//Rita 30-1-19 do not pick API trade
//                if (TradeTransactionQueueObj.IsCancelled == 1 || IsCancel == 1)
//                {
//                    _Resp.ErrorCode = enErrorCode.CancelOrder_OrderalreadyCancelled;
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                    _Resp.ReturnMsg = "Transaction Cancellation request is already in processing";
//                    return _Resp;
//                }
//                if (CurrentTradeSellerListObj.RemainQty == 0)
//                {
//                    _Resp.ErrorCode = enErrorCode.Settlement_AlreadySettled;
//                    _Resp.ReturnCode = enResponseCodeService.Success;
//                    _Resp.ReturnMsg = "ALready Settled";
//                    return _Resp;
//                }
//                _Resp.ErrorCode = enErrorCode.TransactionInsertSuccess;
//                _Resp.ReturnCode = enResponseCodeService.Success;
//                _Resp.ReturnMsg = "Initialise and settlement start";

//                IEnumerable<TradeBuyerListArbitrageV1> MatchBuyerListBase;
//                IEnumerable<TradeBuyerListArbitrageV1> MatchBuyerListBaseAll = await MatchSellerListBaseAllResult;
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTSell", ControllerName, "End Selection" + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
//                TradePairStasticsArbitrage pairStastics = await _dbContext.Set<TradePairStasticsArbitrage>().FirstOrDefaultAsync(pair => pair.PairId == CurrentTradeSellerListObj.PairID);

//                if (CurrentTradeSellerListObj.OrderType == 2)
//                {
//                    CurrentTradeSellerListObj.Price = pairStastics.LTP;
//                }
//                (from u in MatchBuyerListBaseAll where u.OrderType == 2 && u.PairID== CurrentTradeSellerListObj.PairID select u).ToList()
//                        .ForEach(u => u.Price = pairStastics.LTP);

//                MatchBuyerListBase = MatchBuyerListBaseAll.Where(item => (item.Price >= CurrentTradeSellerListObj.Price)//||item.OrderType==2 Rita 28-12-18 after updating price no need of this //&& item.IsProcessing == 0
//                                                     && item.PairID == CurrentTradeSellerListObj.PairID && item.OrderType != 3
//                                                     //&& (item.Status == Convert.ToInt16(enTransactionStatus.Initialize) || item.Status == Convert.ToInt16(enTransactionStatus.Hold))
//                                                     && item.RemainQty > 0).OrderByDescending(x => x.Price).ThenBy(x => x.TrnNo);
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTSell", ControllerName, "Where Done" + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
//                if (CurrentTradeSellerListObj.OrderType == 3)//SPOT Order,check Total Qty
//                {
//                    decimal RemainQtySum = Helpers.DoRoundForTrading(MatchBuyerListBase.Sum(e => e.RemainQty),18);
//                    if (CurrentTradeSellerListObj.RemainQty > RemainQtySum)//No sufficient Qty Found
//                    {
//                        goto SkipLoop;
//                    }
//                }
//                foreach (TradeBuyerListArbitrageV1 BuyerList in MatchBuyerListBase)//MatchSellerList
//                {
//                    Task<bool> IsProResult = CheckIsProcessingSELL(CurrentTradeSellerListObj.TrnNo, BuyerList, pairStastics.LTP);
//                    TrackDebitBit = 0;
//                    CurrenetBuyerList = BuyerList;
                    
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTSell", ControllerName, "Loop Validate Done" + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
                    
//                    decimal SettlementQty = 0;
//                    decimal SettlementPrice = 0;
//                    decimal BaseCurrQty = 0;
//                    if (await IsProResult == false)
//                    {
//                        continue;
//                    }
//                    TrackBit = 1;
//                    BuyerList.IsProcessing = 1;
//                    BuyerList.UpdatedBy = CurrentTradeSellerListObj.TrnNo;//rita 8-1-19 for log
//                    BuyerList.CreatedBy = 800;//rita 10-1-19 for sp and code separate
//                    _TradeBuyerList.UpdateField(BuyerList, e => e.IsProcessing, e => e.Price,e => e.UpdatedBy, e => e.CreatedBy);

//                    TradeTransactionQueueArbitrage TradeTQMakerObj = _dbContext2.Set<TradeTransactionQueueArbitrage>().FirstOrDefault(item => item.TrnNo == BuyerList.TrnNo);
//                    _dbContext1.Entry(BuyerList).Reload();//Rita 18-1-19 here we get lates remain Qty
//                    Task<TransactionQueueArbitrage> TQMakerObjResult = _dbContext1.Set<TransactionQueueArbitrage>().FirstOrDefaultAsync(item => item.Id == BuyerList.TrnNo);
                   
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTSell", ControllerName, "Loop outer Done" + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
//                    //====================================Partial SETTLEMENT TO MEMBER
//                    if (BuyerList.RemainQty < CurrentTradeSellerListObj.RemainQty && BuyerList.RemainQty != 0)//Do not Release Qty here //rita 8-2-18 release may occured so add <>0 Condition
//                    {
//                        SettlementQty = Helpers.DoRoundForTrading(BuyerList.RemainQty,18);//Give to Buyer , buyer's full settlement done here
//                        GSettlementQty = SettlementQty;
//                        SettlementPrice = CurrentTradeSellerListObj.Price;
//                        BaseCurrQty = Helpers.DoRoundForTrading(SettlementQty * SettlementPrice,18);
//                        GBaseCurrQty = BaseCurrQty;
//                        TransactionQueueArbitrage TQMakerObj = await TQMakerObjResult;//this gives error some times , so do not add below wallet call
//                        if (BaseCurrQty == 0)//Rita 29-1-19 release amount , here Qty used of Seller so make fail seller
//                        {
//                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTSell Continue Buyer has BaseCurrQty:" + BaseCurrQty + " SettlementQty:" + SettlementQty + " SettlementPrice:" + SettlementPrice + " Maker TrnNo: " + BuyerList.TrnNo, ControllerName, "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
//                            await ReleaseWalletAmountBuy(BuyerList, TQMakerObj, TradeTQMakerObj, 1);
//                            continue;
//                        }
//                        var CallspResult = Callsp_TradeSettlement(CurrentTradeSellerListObj.TrnNo, BuyerList.TrnNo, SettlementQty, BaseCurrQty, SettlementPrice, 13, 1);
                       
//                        //Here Bid Price of Seller always low then user given in Order , base on above Query
//                        decimal TakeDisc = 0;
//                        //rita 4-2-19 change SettlementPrice to BuyerList.Price , as maker price different
//                        await InsertTradePoolQueue(TransactionQueueObj.MemberID,CurrentTradeSellerListObj.PairID, BuyerList.TrnNo, SettlementQty, BuyerList.Price, CurrentTradeSellerListObj.TrnNo, SettlementQty, SettlementPrice, TakeDisc, 0, "BUY", "SELL");//BuyerList.RemainQty//BuyerList.Price selled base on seller's price


//                        _dbContext2.Entry(BuyerList).Reload();//Rita 11-1-19 reload as incase of fail, currecnt IsProcessing set 1 as above assign
//                        if (await CallspResult == false)
//                        {
//                            _dbContext2.Entry(BuyerList).Reload();//Rita 11-1-19 reload as incase of fail, currecnt IsProcessing set 1 as above assign
//                            continue;
//                        }
                        
//                        TrackDebitBit = 1;//refund if error occured
//                        _dbContext2.Entry(BuyerList).Reload();
//                        _dbContext2.Entry(TradeTQMakerObj).Reload();

//                        CurrentTradeSellerListObj.RemainQty = Helpers.DoRoundForTrading(CurrentTradeSellerListObj.RemainQty - SettlementQty, 18);
//                        CurrentTradeSellerListObj.SelledQty = Helpers.DoRoundForTrading(CurrentTradeSellerListObj.SelledQty + SettlementQty, 18);
                     
//                        Task<bool> WalletResult = WalletProcessSell(TradeTransactionQueueObj, TradeTQMakerObj, SettlementPrice, SettlementQty, BaseCurrQty, 0, 1, TransactionQueueObj.TrnMode, CreditWalletResult);
                       
//                        Task<SettledTradeTransactionQueueArbitrage> SettledTradeTQMakerResult = MakeTransactionSettledEntry(TradeTQMakerObj, SettlementPrice);                        

//                        TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
//                        TransactionQueueObj.SetTransactionStatusMsg("Partial Settlement Done");
//                        TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
//                        TradeTransactionQueueObj.SetTransactionStatusMsg("Partial Settlement Done");
//                        TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + SettlementQty,18);
//                        TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + BaseCurrQty,18);

                     
//                        TQMakerObj.MakeTransactionSuccess();
//                        TQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
//                        TQMakerObj.SetTransactionStatusMsg("Full Settlement Done");
//                        SettledTradeTransactionQueueArbitrage SettledTradeTQMakerObj = await SettledTradeTQMakerResult;
//                        Task<TradeStopLossArbitrage> MakeTradeStopLossObjResult = _dbContext.Set<TradeStopLossArbitrage>().FirstOrDefaultAsync(e => e.TrnNo == TQMakerObj.Id);


//                        if (await WalletResult == false)
//                        {
//                            await MakeTradeStopLossObjResult;//for no contex error
//                            var CallspReverseResult = Callsp_TradeSettlement(CurrentTradeSellerListObj.TrnNo, BuyerList.TrnNo, SettlementQty, BaseCurrQty, SettlementPrice, 13, 2, Convert.ToInt64(CreditWalletResult.ErrorCode));
//                            _dbContext.Entry(CurrentTradeSellerListObj).Reload();
//                            _dbContext.Entry(TransactionQueueObj).Reload();
//                            _dbContext.Entry(TradeTransactionQueueObj).Reload();
                           
//                            var CallspReverseResult1 = await CallspReverseResult;

//                            _dbContext.Entry(BuyerList).Reload();
//                            _dbContext.Entry(TradeTQMakerObj).Reload();
//                            _dbContext.Entry(TQMakerObj).Reload();
//                            Task WalletFail = WalletFailOrderProcessSELL(CurrentTradeSellerListObj.TrnNo, BuyerList, TradeTQMakerObj, TQMakerObj, CreditWalletResult);
//                            TrackDebitBit = 0;
//                            _Resp.ErrorCode = enErrorCode.Settlement_PartialSettlementRollback;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Partial Settlement SELL RollBack ##TrnNo " + CurrentTradeSellerListObj.TrnNo + " With: TrnNo " + BuyerList.TrnNo;
//                            Task.Run(()=> HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT reverse result " + CallspReverseResult1, ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
//                            await WalletFail;
//                        }
//                        else
//                        {//last loop records does not revert, and commit in currecnt txn , as it is present in contex
//                            TradeStopLossArbitrage MakeTradeStopLossObj = await MakeTradeStopLossObjResult;//Rita 27-4-19 as second operation error if not wait
//                            _dbContext.Database.BeginTransaction();
//                            _dbContext.Entry(CurrentTradeSellerListObj).State = EntityState.Modified;
//                            _dbContext.Entry(TransactionQueueObj).State = EntityState.Modified;
//                            _dbContext.Entry(TradeTransactionQueueObj).State = EntityState.Modified;
//                            _dbContext.Entry(TQMakerObj).State = EntityState.Modified;
//                            TradePoolQueueObj.UpdatedDate = Helpers.UTC_To_IST();
//                            TradePoolQueueObj.Status = 1;
//                            _dbContext.SaveChanges();
//                            _dbContext.Database.CommitTransaction();
//                            TrackDebitBit = 0;
//                            //Rita 13-2-19 for error  : Violation of PRIMARY KEY constraint 'PK_SettledTradeTransactionQueueArbitrage'. Cannot insert duplicate key in object 'dbo.SettledTradeTransactionQueueArbitrage'. The duplicate key value is
//                            try
//                            {                               
//                                _dbContext.Set<SettledTradeTransactionQueueArbitrage>().Add(SettledTradeTQMakerObj);
//                                _dbContext.SaveChanges();
//                            }
//                            catch (Exception ex)
//                            {
//                                HelperForLog.WriteErrorLog("PROCESSSETLLEMENTSELL Partial Settle Entry Error:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                            }
                           
//                            //ProccessSellWithList.Add(new ProccessSellWith { BuyerList = BuyerList, MakeTradeStopLossObj = MakeTradeStopLossObj, TQMakerObj = TQMakerObj, TradeTQMakerObj = TradeTQMakerObj, SettlementQty = SettlementQty, SettlementPrice = SettlementPrice, BaseCurrQty = BaseCurrQty });
                            
//                            Task.Run(() => PartilaSettleSellSignalR(CurrentTradeSellerListObj.TrnNo, TransactionQueueObj, TradeTransactionQueueObj, TradeStopLossObj, MakeTradeStopLossObj, TQMakerObj, TradeTQMakerObj, accessToken, SettlementPrice, SettlementQty, CurrentTradeSellerListObj.OrderType));
                            
//                            _Resp.ErrorCode = enErrorCode.Settlement_PartialSettlementDone;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Partial Settlement SELL Done of ##TrnNo " + CurrentTradeSellerListObj.TrnNo + " Settled: " + CurrentTradeSellerListObj.SelledQty + " Remain:" + CurrentTradeSellerListObj.RemainQty;
//                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));                           
//                        }
//                    }
//                    //====================================FULL SETTLEMENT TO MEMBER
//                    else if (BuyerList.RemainQty >= CurrentTradeSellerListObj.RemainQty && CurrentTradeSellerListObj.RemainQty != 0)
//                    {
//                        SettlementQty = Helpers.DoRoundForTrading(CurrentTradeSellerListObj.RemainQty,18);
//                        GSettlementQty = SettlementQty;
//                        SettlementPrice = CurrentTradeSellerListObj.Price;
//                        BaseCurrQty = Helpers.DoRoundForTrading(SettlementQty * SettlementPrice,18);
//                        GBaseCurrQty = BaseCurrQty;
//                        short IsFullSettledBit = 0;
//                        if (BuyerList.RemainQty == SettlementQty)
//                            IsFullSettledBit = 1;
//                        if (BaseCurrQty == 0)//Rita 29-1-19 release amount , here Qty used of Current Buyer Object , so release
//                        {
//                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTSell Continue Current Seller has BaseCurrQty:" + BaseCurrQty + " SettlementQty:" + SettlementQty + " SettlementPrice:" + SettlementPrice + " Maker TrnNo: " + BuyerList.TrnNo, ControllerName, "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
//                            await ReleaseWalletAmountSell(CurrentTradeSellerListObj, TransactionQueueObj, TradeTransactionQueueObj, 1);
//                            BuyerList.IsProcessing = 0;
//                            _TradeBuyerList.UpdateField(BuyerList, e => e.IsProcessing);
//                            break; //break loop
//                        }
//                        TransactionQueueArbitrage TQMakerObj = await TQMakerObjResult;//this gives error some times , so do not add below wallet call
//                        Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENTSell", ControllerName, "Else SP Start" + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
//                        var CallspResult = Callsp_TradeSettlement(CurrentTradeSellerListObj.TrnNo, BuyerList.TrnNo, SettlementQty, BaseCurrQty, SettlementPrice, 14, 1);
                      
//                        decimal TakeDisc = 0;
//                        //rita 4-2-19 change SettlementPrice to BuyerList.Price , as maker price different
//                        await InsertTradePoolQueue(TransactionQueueObj.MemberID, CurrentTradeSellerListObj.PairID, BuyerList.TrnNo, BuyerList.RemainQty, BuyerList.Price, CurrentTradeSellerListObj.TrnNo, SettlementQty, SettlementPrice, TakeDisc, 0, "BUY", "SELL");//+SettlementQty//BuyerList.Price selled base on seller's price
                        
//                        if (await CallspResult == false)
//                        {
//                            _dbContext2.Entry(BuyerList).Reload();//Rita 11-1-19 reload as incase of fail, currecnt IsProcessing set 1 as above assign
//                            continue;
//                        }

//                        TrackDebitBit = 1;//refund if error occured
//                        _dbContext2.Entry(BuyerList).Reload();
//                        _dbContext2.Entry(TradeTQMakerObj).Reload();
//                        Task<bool> WalletResult = WalletProcessSell(TradeTransactionQueueObj, TradeTQMakerObj, SettlementPrice, SettlementQty, BaseCurrQty, 1, IsFullSettledBit == 1 ? (short)1 : (short)0, TransactionQueueObj.TrnMode, CreditWalletResult);//BuyerList.RemainQty rita 4-1-19 issue when two time debited from original buyer and this make wallet settled then last txn may fail due to insufficient balance

                        
//                        TQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_PartialSettlementDone));
//                        TQMakerObj.SetTransactionStatusMsg("Partial Settlement Done");                      

//                        if (IsFullSettledBit == 1)
//                        {//Make Seller Order Success
                            
//                            TQMakerObj.MakeTransactionSuccess();
//                            TQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
//                            TQMakerObj.SetTransactionStatusMsg("Full Settlement Done");
//                        }
//                        //make entry in case of success seller txn
//                        Task<SettledTradeTransactionQueueArbitrage> SettledTradeTQMakerResult = MakeTransactionSettledEntry(TradeTQMakerObj, SettlementPrice);
                        

//                        CurrentTradeSellerListObj.RemainQty = Helpers.DoRoundForTrading(CurrentTradeSellerListObj.RemainQty - SettlementQty,18);
//                        CurrentTradeSellerListObj.SelledQty = Helpers.DoRoundForTrading(CurrentTradeSellerListObj.SelledQty + SettlementQty,18);
//                        CurrentTradeSellerListObj.MakeTransactionSuccess();
//                        TransactionQueueObj.MakeTransactionSuccess();
//                        TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
//                        TransactionQueueObj.SetTransactionStatusMsg("Full Settlement Done");

//                        TradeTransactionQueueObj.MakeTransactionSuccess();
//                        TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_FullSettlementDone));
//                        TradeTransactionQueueObj.SetTransactionStatusMsg("Full Settlement Done");
//                        TradeTransactionQueueObj.SettledSellQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledSellQty + SettlementQty,18);
//                        TradeTransactionQueueObj.SettledBuyQty = Helpers.DoRoundForTrading(TradeTransactionQueueObj.SettledBuyQty + BaseCurrQty,18);
//                        Task<SettledTradeTransactionQueueArbitrage> SettledTradeTQResult = MakeTransactionSettledEntry(TradeTransactionQueueObj, SettlementPrice);
//                        Task<TradeStopLossArbitrage> MakeTradeStopLossObjResult = _dbContext.Set<TradeStopLossArbitrage>().FirstOrDefaultAsync(e => e.TrnNo == TQMakerObj.Id);                        

//                        if (await WalletResult == false)
//                        {                            
//                            await MakeTradeStopLossObjResult;//for no contex error
//                            var CallspReverseResult = Callsp_TradeSettlement(CurrentTradeSellerListObj.TrnNo, BuyerList.TrnNo, SettlementQty, BaseCurrQty, SettlementPrice, 14, 2, Convert.ToInt64(CreditWalletResult.ErrorCode));
//                            _dbContext.Entry(CurrentTradeSellerListObj).Reload();
//                            _dbContext.Entry(TransactionQueueObj).Reload();
//                            _dbContext.Entry(TradeTransactionQueueObj).Reload();
//                            var CallspReverseResult1=await CallspReverseResult;
//                            _dbContext.Entry(BuyerList).Reload();
//                            _dbContext.Entry(TradeTQMakerObj).Reload();
//                            _dbContext.Entry(TQMakerObj).Reload();
//                            Task WalletFail = WalletFailOrderProcessSELL(CurrentTradeSellerListObj.TrnNo, BuyerList, TradeTQMakerObj, TQMakerObj, CreditWalletResult);
//                            TrackDebitBit = 0;
//                            _Resp.ErrorCode = enErrorCode.Settlement_FullSettlementRollback;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Full Settlement SELL Rollback ##TrnNo:" + TradeTransactionQueueObj.TrnNo + " With: TrnNo " + BuyerList.TrnNo;
//                            Task.Run(()=> HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT reverse result "+ CallspReverseResult1, ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
//                            await WalletFail;
//                        }
//                        else
//                        {
//                            TradeStopLossArbitrage MakeTradeStopLossObj = await MakeTradeStopLossObjResult;//Rita 27-4-19 as second operation error if not wait
//                            _dbContext.Database.BeginTransaction();
                            
//                            _dbContext.Entry(CurrentTradeSellerListObj).State = EntityState.Modified;
//                            _dbContext.Entry(TransactionQueueObj).State = EntityState.Modified;
//                            _dbContext.Entry(TradeTransactionQueueObj).State = EntityState.Modified;                            

//                            _dbContext.Entry(TQMakerObj).State = EntityState.Modified;
//                            TradePoolQueueObj.UpdatedDate = Helpers.UTC_To_IST();
//                            TradePoolQueueObj.Status = 1;//rita 23-3-19 update status as entry added from SP
//                            SettledTradeTransactionQueueArbitrage SettledTradeTQObj = await SettledTradeTQResult;
                           
//                            _dbContext.SaveChanges();
//                            _dbContext.Database.CommitTransaction();
//                            TrackDebitBit = 0;
//                            //Rita 13-2-19 for error  : Violation of PRIMARY KEY constraint 'PK_SettledTradeTransactionQueueArbitrage'. Cannot insert duplicate key in object 'dbo.SettledTradeTransactionQueueArbitrage'. The duplicate key value is
//                            try
//                            {
//                                if (IsFullSettledBit == 1)
//                                {
//                                    SettledTradeTransactionQueueArbitrage SettledTradeTQMakerObj = await SettledTradeTQMakerResult;
//                                    _dbContext.Set<SettledTradeTransactionQueueArbitrage>().Add(SettledTradeTQMakerObj);
//                                }
//                                _dbContext.Set<SettledTradeTransactionQueueArbitrage>().Add(SettledTradeTQObj);
//                                _dbContext.SaveChanges();
//                            }
//                            catch (Exception ex)
//                            {
//                                HelperForLog.WriteErrorLog("PROCESSSETLLEMENTSELL FULL Settle Entry Error:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                            }
                           
//                            //ProccessSellWithList.Add(new ProccessSellWith { BuyerList = BuyerList, MakeTradeStopLossObj = MakeTradeStopLossObj, TQMakerObj = TQMakerObj, TradeTQMakerObj = TradeTQMakerObj, SettlementQty = SettlementQty, SettlementPrice = SettlementPrice, BaseCurrQty = BaseCurrQty });
                            
//                            Task.Run(() => FullSettleSellSignalR(CurrentTradeSellerListObj.TrnNo, TransactionQueueObj, TradeTransactionQueueObj, TradeStopLossObj, MakeTradeStopLossObj, TQMakerObj, TradeTQMakerObj, accessToken, SettlementPrice, SettlementQty, BuyerList, CurrentTradeSellerListObj.OrderType));

//                            _Resp.ErrorCode = enErrorCode.Settlement_FullSettlementDone;
//                            _Resp.ReturnCode = enResponseCodeService.Success;
//                            _Resp.ReturnMsg = "Full Settlement SELL Done of ##TrnNo " + CurrentTradeSellerListObj.TrnNo + " Settled: " + CurrentTradeSellerListObj.SelledQty + " Remain:" + CurrentTradeSellerListObj.RemainQty + " With TrnNo:"+ BuyerList.TrnNo;
//                            Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT", ControllerName, _Resp.ReturnMsg, Helpers.UTC_To_IST()));
//                            break;//record settled                           
//                        }
//                    }
//                }
//                SkipLoop:
//                CurrentTradeSellerListObj.IsProcessing = 0;//Release Sell Order             
//                _TradeSellerList.UpdateField(CurrentTradeSellerListObj,e=>e.IsProcessing);
                
//                if (TrackBit == 0)//No any record Process
//                {
//                    if (CurrentTradeSellerListObj.OrderType == 3)//Reverse Order
//                    {                        
//                        await ReleaseWalletAmountSell(CurrentTradeSellerListObj, TransactionQueueObj, TradeTransactionQueueObj);
//                    }
//                    _Resp.ErrorCode = enErrorCode.Settlement_NoSettlementRecordFound;
//                    _Resp.ReturnCode = enResponseCodeService.Success;//Rita 13-6-19 for arbitrage no route so make hold as going in local
//                    _Resp.ReturnMsg = "No Any Match Record Found";
//                }
//                else if (CurrentTradeSellerListObj.OrderType == 3)
//                {
//                    if (CurrentTradeSellerListObj.RemainQty != 0)//Revert all process of above
//                    {
//                        //only revert ramain amount
//                        bool ReleaseResult = await ReleaseWalletAmountSell(CurrentTradeSellerListObj, TransactionQueueObj, TradeTransactionQueueObj,1);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("PROCESSSETLLEMENTSell:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                CurrentTradeSellerListObj.IsProcessing = 0;//Release Sell Order   
//                _dbContext.Entry(CurrentTradeSellerListObj).Property(p => p.IsProcessing).IsModified = true;
//                _dbContext.SaveChanges();
//                if (CurrenetBuyerList.TrnNo != 0)//here null condition not apply as this declare with new tag
//                {
//                    if (TrackDebitBit == 1)//rita 1-1-18 only if data updated
//                    {
//                        var CallspReverseResult = await Callsp_TradeSettlement(CurrentTradeSellerListObj.TrnNo, CurrenetBuyerList.TrnNo, GSettlementQty, GBaseCurrQty, 0, 13, 2);
//                    }
//                    else
//                    {
//                        CurrenetBuyerList.IsProcessing = 0;
//                        _dbContext.Entry(CurrenetBuyerList).Property(p => p.IsProcessing).IsModified = true;
//                        _dbContext.SaveChanges();
//                    }
                    
//                }
//                if (CurrentTradeSellerListObj.RemainQty == 0)//Rita 3-8-19 full pending then move on next LP
//                    _Resp.ReturnCode = enResponseCodeService.Fail;
//                else
//                    _Resp.ReturnCode = enResponseCodeService.Success;
                
//                _Resp.ReturnMsg = ex.Message;
//                _Resp.ErrorCode = enErrorCode.Settlement_SettlementInternalError;
//            }
//            return _Resp;
//        }

//        public async Task WalletFailOrderProcessBUY(long TrnNo, TradeSellerListArbitrageV1 SellerList, TradeTransactionQueueArbitrage TradeTQMakerObj, TransactionQueueArbitrage TQMakerObj, WalletDrCrResponse CreditWalletResult)//Rita 26-3-19 do not take global as object come here with original values which required,may global obj create an issue
//        {//Rita 11-1-19 as Order has mismatch issue , make it disable , also operation only with Maker while settlement , currecnt object skip if error occured
//            try
//            {
//                //InsufficientMarketInternalBalanceCheckFirstCurrencyForDifferentialAmountFailed = 4347
//                //InsufficientMarketInternalBalanceCheckSecondCurrencyForDifferentialAmountFailed = 4348
//                //InsufficientOutgoingBalFirstCur = 4345
//                //InsufficietOutgoingBalSecondCur = 4346
//                //CrDrCredit_SettledBalMismatchCrWallet = 4364
//                //CrDrCredit_SettledBalMismatchCrWalletSecCur = 4365
//                //CrDrCredit_SettledBalMismatchDrWallet = 4366
//                //CrDrCredit_SettledBalMismatchDrWalletSecDr = 4367
//                if (CreditWalletResult.ReturnCode != enResponseCode.Success)
//                {
//                    //Incase of BUY First/Base Currecy -- CURRENCT TxnOnj - Debited , so skip
//                    //Rita 7-3-19 Here First Means Second Currecny-ATCC, for Maker ,Second Currency total Hold , so not possible for Sell-Maker Order
//                    if (CreditWalletResult.ErrorCode == enErrorCode.InsufficientMarketInternalBalanceCheckFirstCurrencyForDifferentialAmountFailed)
//                    { }
//                    //Incase of BUY Second Currecy -- MAKER TxnObj- Debited, make Maker cancel
//                    //this error not possible for Seller, as his total Sell Qty/amount is hold
//                    //Rita 7-3-19 Here Second Means First/Base Currecny, for Maker-Seller ,Base Currency Credit, so not possible for Sell Order
//                    //For Current Object this poassible as BTC hold,but doest not process for it , so remove below code for Maker
//                    else if (CreditWalletResult.ErrorCode == enErrorCode.InsufficientMarketInternalBalanceCheckSecondCurrencyForDifferentialAmountFailed 
//                        || CreditWalletResult.ErrorCode == enErrorCode.InsufficietOutgoingBalSecondCur)
//                    {
//                        //TQMakerObj.MakeTransactionSuccess();
//                        //TQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                        //TQMakerObj.SetTransactionStatusMsg("Success with partial cancellation-System");
//                        //TQMakerObj.UpdatedDate = Helpers.UTC_To_IST();

//                        //TradeTQMakerObj.MakeTransactionSuccess();
//                        //TradeTQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                        //TradeTQMakerObj.SetTransactionStatusMsg("Success with partial cancellation-System");
//                        //TradeTQMakerObj.IsCancelled = 1;
//                        //TradeTQMakerObj.UpdatedDate = Helpers.UTC_To_IST();
//                        //TradeTQMakerObj.SettledDate = Helpers.UTC_To_IST();

//                        //SellerList.MakeTransactionSuccess();
//                        //SellerList.UpdatedDate = Helpers.UTC_To_IST();
//                        //SellerList.CreatedBy = 700;
//                        ////_TradeSellerList.UpdateField(SellerList, e => e.Status);
//                        //_dbContext.Entry(SellerList).Property(p => p.Status).IsModified = true;
//                        //_dbContext.Entry(SellerList).Property(p => p.CreatedBy).IsModified = true;
//                        //_dbContext.Entry(TQMakerObj).Property(p => p.Status).IsModified = true;
//                        //_dbContext.Entry(TQMakerObj).Property(p => p.StatusCode).IsModified = true;
//                        //_dbContext.Entry(TQMakerObj).Property(p => p.StatusMsg).IsModified = true;
//                        //_dbContext.Entry(TQMakerObj).Property(p => p.UpdatedDate).IsModified = true;
//                        //_dbContext.Entry(TradeTQMakerObj).Property(p => p.Status).IsModified = true;
//                        //_dbContext.Entry(TradeTQMakerObj).Property(p => p.StatusCode).IsModified = true;
//                        //_dbContext.Entry(TradeTQMakerObj).Property(p => p.StatusMsg).IsModified = true;
//                        //_dbContext.Entry(TradeTQMakerObj).Property(p => p.UpdatedDate).IsModified = true;
//                        //_dbContext.SaveChanges();
//                        //try
//                        //{
//                        //    _ISignalRService.OnStatusCancel(TradeTQMakerObj.Status, TQMakerObj, TradeTQMakerObj, "", TradeTQMakerObj.ordertype, 1);
//                        //}
//                        //catch (Exception ex)
//                        //{
//                        //    HelperForLog.WriteErrorLog("WalletFailOrderProcessBUY -signalR Error ##TrnNo:" + TrnNo + " with TrnNo:" + TradeTQMakerObj.TrnNo + " ErrorCode:" + CreditWalletResult.ErrorCode, ControllerName, ex);
//                        //}
//                    }
//                    else if (CreditWalletResult.ErrorCode == enErrorCode.CrDrCredit_SettledBalMismatchCrWalletSecCur 
//                        || CreditWalletResult.ErrorCode == enErrorCode.CrDrCredit_SettledBalMismatchDrWalletSecDr)
//                    {
//                        TQMakerObj.MakeTransactionInActive();
//                        TQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                        TQMakerObj.SetTransactionStatusMsg("Make InActive as Bal MisMatch-System");
//                        TQMakerObj.UpdatedDate = Helpers.UTC_To_IST();

//                        TradeTQMakerObj.MakeTransactionInActive();
//                        TradeTQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                        TradeTQMakerObj.SetTransactionStatusMsg("Make InActive as Bal MisMatch-System");
//                       // TradeTQMakerObj.IsCancelled = 1;
//                        TradeTQMakerObj.UpdatedDate = Helpers.UTC_To_IST();
//                        TradeTQMakerObj.SettledDate = Helpers.UTC_To_IST();

//                        SellerList.MakeTransactionInActive();
//                        SellerList.UpdatedDate = Helpers.UTC_To_IST();
//                        SellerList.CreatedBy = 700;
//                        //_TradeSellerList.UpdateField(SellerList, e => e.Status);
//                        _dbContext.Entry(SellerList).Property(p => p.Status).IsModified = true;
//                        _dbContext.Entry(SellerList).Property(p => p.CreatedBy).IsModified = true;
//                        _dbContext.Entry(TQMakerObj).Property(p => p.Status).IsModified = true;
//                        _dbContext.Entry(TQMakerObj).Property(p => p.StatusCode).IsModified = true;
//                        _dbContext.Entry(TQMakerObj).Property(p => p.StatusMsg).IsModified = true;
//                        _dbContext.Entry(TQMakerObj).Property(p => p.UpdatedDate).IsModified = true;
//                        _dbContext.Entry(TradeTQMakerObj).Property(p => p.Status).IsModified = true;
//                        _dbContext.Entry(TradeTQMakerObj).Property(p => p.StatusCode).IsModified = true;
//                        _dbContext.Entry(TradeTQMakerObj).Property(p => p.StatusMsg).IsModified = true;
//                        _dbContext.Entry(TradeTQMakerObj).Property(p => p.UpdatedDate).IsModified = true;
//                        _dbContext.SaveChanges();

//                        try
//                        {
//                            //fully cancel as fail order do not need to add in history
//                            _ISignalRService.OnStatusCancelArbitrage(TradeTQMakerObj.Status, TQMakerObj, TradeTQMakerObj, "", TradeTQMakerObj.ordertype,0);
//                        }
//                        catch (Exception ex)
//                        {
//                            HelperForLog.WriteErrorLog("WalletFailOrderProcessBUY -signalR Error ##TrnNo:" + TrnNo + " with TrnNo:" + TradeTQMakerObj.TrnNo + " ErrorCode:" + CreditWalletResult.ErrorCode, ControllerName, ex);
//                        }
//                    }


//                }

//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("WalletFailOrderProcessBUY:##TrnNo " + TrnNo + " with TrnNo:" + TradeTQMakerObj.TrnNo, ControllerName, ex);
//            }
//        }
//        public async Task WalletFailOrderProcessSELL(long TrnNo,TradeBuyerListArbitrageV1 BuyerList, TradeTransactionQueueArbitrage TradeTQMakerObj, TransactionQueueArbitrage TQMakerObj, WalletDrCrResponse CreditWalletResult)
//        {//Rita 11-1-19 as Order has mismatch issue , make it disable , also operation only with Maker while settlement , currecnt object skip if error occured
//            try
//            {
//                //InsufficientMarketInternalBalanceCheckFirstCurrencyForDifferentialAmountFailed = 4347
//                //InsufficientMarketInternalBalanceCheckSecondCurrencyForDifferentialAmountFailed = 4348
//                //InsufficientOutgoingBalFirstCur = 4345
//                //InsufficietOutgoingBalSecondCur = 4346
//                //CrDrCredit_SettledBalMismatchCrWallet = 4364
//                //CrDrCredit_SettledBalMismatchCrWalletSecCur = 4365
//                //CrDrCredit_SettledBalMismatchDrWallet = 4366
//                //CrDrCredit_SettledBalMismatchDrWalletSecDr = 4367
//                if (CreditWalletResult.ReturnCode != enResponseCode.Success)
//                {
//                    //Incase of SELL First/Base Currecy -- MAKER TxnObj- Debited, make Maker cancel   
//                    //Rita 7-3-19 Here First Means Second Currecny-ATCC, Debited from Current SELLER Object , so not possible
//                    if (CreditWalletResult.ErrorCode == enErrorCode.InsufficientMarketInternalBalanceCheckFirstCurrencyForDifferentialAmountFailed 
//                        || CreditWalletResult.ErrorCode == enErrorCode.InsufficientOutgoingBalFirstCur)
//                    {
//                        //TQMakerObj.MakeTransactionSuccess();
//                        //TQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                        //TQMakerObj.SetTransactionStatusMsg("Success with partial cancellation-System");
//                        //TQMakerObj.UpdatedDate = Helpers.UTC_To_IST();

//                        //TradeTQMakerObj.MakeTransactionSuccess();
//                        //TradeTQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                        //TradeTQMakerObj.SetTransactionStatusMsg("Success with partial cancellation-System");
//                        //TradeTQMakerObj.IsCancelled =1;
//                        //TradeTQMakerObj.UpdatedDate = Helpers.UTC_To_IST();
//                        //TradeTQMakerObj.SettledDate = Helpers.UTC_To_IST();

//                        //BuyerList.MakeTransactionSuccess();
//                        //BuyerList.UpdatedDate = Helpers.UTC_To_IST();
//                        //BuyerList.CreatedBy = 700;
//                        ////_TradeSellerList.UpdateField(SellerList, e => e.Status);
//                        //_dbContext.Entry(BuyerList).Property(p => p.Status).IsModified = true;
//                        //_dbContext.Entry(BuyerList).Property(p => p.CreatedBy).IsModified = true;
//                        //_dbContext.Entry(TQMakerObj).Property(p => p.Status).IsModified = true;
//                        //_dbContext.Entry(TQMakerObj).Property(p => p.StatusCode).IsModified = true;
//                        //_dbContext.Entry(TQMakerObj).Property(p => p.StatusMsg).IsModified = true;
//                        //_dbContext.Entry(TQMakerObj).Property(p => p.UpdatedDate).IsModified = true;
//                        //_dbContext.Entry(TradeTQMakerObj).Property(p => p.Status).IsModified = true;
//                        //_dbContext.Entry(TradeTQMakerObj).Property(p => p.StatusCode).IsModified = true;
//                        //_dbContext.Entry(TradeTQMakerObj).Property(p => p.StatusMsg).IsModified = true;
//                        //_dbContext.Entry(TradeTQMakerObj).Property(p => p.UpdatedDate).IsModified = true;
//                        //_dbContext.SaveChanges();
//                        //try
//                        //{
//                        //    _ISignalRService.OnStatusCancel(TradeTQMakerObj.Status, TQMakerObj, TradeTQMakerObj, "", TradeTQMakerObj.ordertype, 1);
//                        //}
//                        //catch (Exception ex)
//                        //{
//                        //    HelperForLog.WriteErrorLog("WalletFailOrderProcessBUY -signalR Error ##TrnNo:" + TrnNo + " with TrnNo:" + TradeTQMakerObj.TrnNo + " ErrorCode:" + CreditWalletResult.ErrorCode, ControllerName, ex);
//                        //}
//                    }
//                    //Incase of SELL Second Currecy -- CURRENCT TxnOnj - Debited , so skip
//                    //Rita 4-3-19 incase of Maker-buyer has insufficient balance for Market order make inactivate that order
//                    //Rita 7-3-19 Here Second Means First/Base Currecny , so this may come in case of HOLD BTC and may market up and need extra balance for Maker
//                    else if (CreditWalletResult.ErrorCode == enErrorCode.InsufficientMarketInternalBalanceCheckSecondCurrencyForDifferentialAmountFailed || CreditWalletResult.ErrorCode == enErrorCode.InsufficietOutgoingBalSecondCur)//Rita 15-3-19 as Buyer Maker debit BTC so it may nagative
//                    {
//                       var releaseResult = await ReleaseWalletAmountBuy(BuyerList, TQMakerObj, TradeTQMakerObj, 1);//Release Maker
//                    }
//                    else if (CreditWalletResult.ErrorCode == enErrorCode.CrDrCredit_SettledBalMismatchCrWallet
//                        || CreditWalletResult.ErrorCode == enErrorCode.CrDrCredit_SettledBalMismatchDrWallet)
//                    {
//                        TQMakerObj.MakeTransactionInActive();
//                        TQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                        TQMakerObj.SetTransactionStatusMsg("Make InActive as Bal MisMatch-System");
//                        TQMakerObj.UpdatedDate = Helpers.UTC_To_IST();

//                        TradeTQMakerObj.MakeTransactionInActive();
//                        TradeTQMakerObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                        TradeTQMakerObj.SetTransactionStatusMsg("Make InActive as Bal MisMatch-System");
//                        //TradeTQMakerObj.IsCancelled = 1;
//                        TradeTQMakerObj.UpdatedDate = Helpers.UTC_To_IST();
//                        TradeTQMakerObj.SettledDate = Helpers.UTC_To_IST();

//                        BuyerList.MakeTransactionInActive();
//                        BuyerList.UpdatedDate = Helpers.UTC_To_IST();
//                        BuyerList.CreatedBy = 700;
//                        //_TradeSellerList.UpdateField(SellerList, e => e.Status);
//                        _dbContext.Entry(BuyerList).Property(p => p.Status).IsModified = true;
//                        _dbContext.Entry(BuyerList).Property(p => p.CreatedBy).IsModified = true;
//                        _dbContext.Entry(TQMakerObj).Property(p => p.Status).IsModified = true;
//                        _dbContext.Entry(TQMakerObj).Property(p => p.StatusCode).IsModified = true;
//                        _dbContext.Entry(TQMakerObj).Property(p => p.StatusMsg).IsModified = true;
//                        _dbContext.Entry(TQMakerObj).Property(p => p.UpdatedDate).IsModified = true;
//                        _dbContext.Entry(TradeTQMakerObj).Property(p => p.Status).IsModified = true;
//                        _dbContext.Entry(TradeTQMakerObj).Property(p => p.StatusCode).IsModified = true;
//                        _dbContext.Entry(TradeTQMakerObj).Property(p => p.StatusMsg).IsModified = true;
//                        _dbContext.Entry(TradeTQMakerObj).Property(p => p.UpdatedDate).IsModified = true;
//                        _dbContext.SaveChanges();

//                        try
//                        {
//                            //fully cancel as fail order do not need to add in history
//                            _ISignalRService.OnStatusCancelArbitrage(TradeTQMakerObj.Status, TQMakerObj, TradeTQMakerObj, "", TradeTQMakerObj.ordertype, 0);
//                        }
//                        catch (Exception ex)
//                        {
//                            HelperForLog.WriteErrorLog("WalletFailOrderProcessBUY -signalR Error ##TrnNo:" + TrnNo + " with TrnNo:" + TradeTQMakerObj.TrnNo + " ErrorCode:" + CreditWalletResult.ErrorCode, ControllerName, ex);
//                        }
//                    }


//                }

//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("WalletFailOrderProcessBUY:##TrnNo " + TrnNo + " with TrnNo:" + TradeTQMakerObj.TrnNo, ControllerName, ex);
//            }
//        }

//        public async Task<bool> ReleaseWalletAmountBuy(TradeBuyerListArbitrageV1 CurrentTradeBuyerListObj, TransactionQueueArbitrage TransactionQueueObj, TradeTransactionQueueArbitrage TradeTransactionQueueObj,short IsPartial=0)
//        {
//            try
//            {
//                decimal ReleaseAmt = TradeTransactionQueueObj.OrderTotalQty - TradeTransactionQueueObj.SettledSellQty;//always full amount,incase of error it should be partial-remain
//                Task<WalletDrCrResponse> WalletResult = _WalletService.ArbitrageGetReleaseHoldNew(TradeTransactionQueueObj.Order_Currency, Helpers.GetTimeStamp(), ReleaseAmt,
//                                                 TradeTransactionQueueObj.OrderAccountID, TradeTransactionQueueObj.TrnNo, enServiceType.Trading,
//                                                 enWalletTrnType.BuyTrade, (enTrnType)TransactionQueueObj.TrnType, (EnAllowedChannels)TransactionQueueObj.TrnMode,"");
//                var WalletResp = await WalletResult;
//                if (WalletResp.ReturnCode != enResponseCode.Success && ReleaseAmt!=0)//Rita 29-1-19 if no amount remain direct update records
//                {
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ReleaseWalletAmountBuy Fail " + WalletResp.ErrorCode + " Msg " + WalletResp.ReturnMsg, ControllerName, "ReleaseAmt " + ReleaseAmt + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
//                    return false;
//                }
//                else
//                {
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ReleaseWalletAmountBuy Success " + WalletResp.ErrorCode + " Msg " + WalletResp.ReturnMsg, ControllerName, "ReleaseAmt " + ReleaseAmt + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));                   
                    
//                    if(IsPartial==0)//regular
//                    {
//                        TransactionQueueObj.MakeTransactionOperatorFail();
//                        TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_NoSettlementRecordFound));
//                        TransactionQueueObj.SetTransactionStatusMsg("No Any Match Record Found for SPOT Order");

//                        TradeTransactionQueueObj.MakeTransactionOperatorFail();
//                        TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_NoSettlementRecordFound));
//                        TradeTransactionQueueObj.SetTransactionStatusMsg("No Any Match Record Found for SPOT Order");
//                        TradeTransactionQueueObj.SettledDate = Helpers.UTC_To_IST();//Rita 17-1-19 update date for Reporting
//                        TradeTransactionQueueObj.UpdatedDate = Helpers.UTC_To_IST();

//                        CurrentTradeBuyerListObj.MakeTransactionOperatorFail();
//                    }
//                    else//In case of error , make partial settlement done
//                    {
//                        //Rita 29-1-19 make success entry for partial success-cancel
//                        Task<SettledTradeTransactionQueueArbitrage> SettledTradeTQMakerResult = MakeTransactionSettledEntry(TradeTransactionQueueObj, 0,1);

//                        TransactionQueueObj.MakeTransactionSuccess();
//                        TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                        TransactionQueueObj.SetTransactionStatusMsg("Success with partial cancellation");

//                        TradeTransactionQueueObj.MakeTransactionSuccess();
//                        TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                        TradeTransactionQueueObj.SetTransactionStatusMsg("Success with partial cancellation");
//                        TradeTransactionQueueObj.SettledDate = Helpers.UTC_To_IST();//Rita 17-1-19 update date for Reporting
//                        TradeTransactionQueueObj.UpdatedDate = Helpers.UTC_To_IST();
//                        TradeTransactionQueueObj.IsCancelled = 1;

//                        CurrentTradeBuyerListObj.MakeTransactionSuccess();
//                        CurrentTradeBuyerListObj.IsProcessing = 0;

//                        SettledTradeTransactionQueueArbitrage SettledTradeTQMakerObj = await SettledTradeTQMakerResult;
//                        _dbContext.Set<SettledTradeTransactionQueueArbitrage>().Add(SettledTradeTQMakerObj);
//                    }
//                    _dbContext.Entry(TransactionQueueObj).State = EntityState.Modified;
//                    _dbContext.Entry(TradeTransactionQueueObj).State = EntityState.Modified;
//                    //_dbContext.Entry(CurrentTradeBuyerListObj).State = EntityState.Modified;
//                    _dbContext.Entry(CurrentTradeBuyerListObj).Property(p => p.Status).IsModified = true;
//                    _dbContext.Entry(CurrentTradeBuyerListObj).Property(p => p.IsProcessing).IsModified = true;

//                    _dbContext.SaveChanges();
//                    try
//                    {                       
//                        _ISignalRService.OnStatusCancelArbitrage(TradeTransactionQueueObj.Status, TransactionQueueObj, TradeTransactionQueueObj, "", TradeTransactionQueueObj.ordertype, IsPartial);
                        
//                        Task.Run(() => EmailSendPartialSettledTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID.ToString(), TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.BuyQty,
//                        TradeTransactionQueueObj.TrnDate.ToString(), 0, 0, TradeTransactionQueueObj.BidPrice, 0, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType, 2));

//                    }
//                    catch (Exception ex)
//                    {                        
//                        HelperForLog.WriteErrorLog("ReleaseWalletAmountBuy Error ##TrnNo:" + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                    }
//                    return true;

//                }
//            }
//            catch(Exception ex)
//            {
//                HelperForLog.WriteErrorLog("ReleaseWalletAmountBuy:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                return false;
//            }
//        }

//        public async Task<bool> ReleaseWalletAmountSell(TradeSellerListArbitrageV1 CurrentTradeSellerListObj, TransactionQueueArbitrage TransactionQueueObj, TradeTransactionQueueArbitrage TradeTransactionQueueObj,short IsPartial = 0)
//        {
//            try
//            {
//                decimal ReleaseAmt = TradeTransactionQueueObj.OrderTotalQty - TradeTransactionQueueObj.SettledSellQty;//always full amount , or take here RemainAmt , ,incase of error it should be partial-remain
//                Task<WalletDrCrResponse> WalletResult = _WalletService.ArbitrageGetReleaseHoldNew(TradeTransactionQueueObj.Order_Currency, Helpers.GetTimeStamp(), ReleaseAmt,
//                                                 TradeTransactionQueueObj.OrderAccountID, TradeTransactionQueueObj.TrnNo, enServiceType.Trading,
//                                                 enWalletTrnType.SellTrade, (enTrnType)TransactionQueueObj.TrnType, (EnAllowedChannels)TransactionQueueObj.TrnMode, "");
//                var WalletResp = await WalletResult;
//                if (WalletResp.ReturnCode != enResponseCode.Success && ReleaseAmt != 0)//Rita 29-1-19 if no amount remain direct update records
//                {
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ReleaseWalletAmountSell Fail " + WalletResp.ErrorCode + " Msg " + WalletResp.ReturnMsg, ControllerName, "ReleaseAmt " + ReleaseAmt + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
//                    return false;
//                }
//                else
//                {
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("ReleaseWalletAmountSell Success " + WalletResp.ErrorCode + " Msg " + WalletResp.ReturnMsg, ControllerName, "ReleaseAmt " + ReleaseAmt + "##TrnNo:" + TradeTransactionQueueObj.TrnNo, Helpers.UTC_To_IST()));
//                    if (IsPartial == 0)//regular
//                    {
//                        TransactionQueueObj.MakeTransactionOperatorFail();
//                        TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_NoSettlementRecordFound));
//                        TransactionQueueObj.SetTransactionStatusMsg("No Any Match Record Found for SPOT Order");

//                        TradeTransactionQueueObj.MakeTransactionOperatorFail();
//                        TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.Settlement_NoSettlementRecordFound));
//                        TradeTransactionQueueObj.SetTransactionStatusMsg("No Any Match Record Found for SPOT Order");
//                        TradeTransactionQueueObj.SettledDate = Helpers.UTC_To_IST();//Rita 17-1-19 update date for Reporting
//                        TradeTransactionQueueObj.UpdatedDate = Helpers.UTC_To_IST();

//                        CurrentTradeSellerListObj.MakeTransactionOperatorFail();
//                    }
//                    else//In case of error , make partial settlement done
//                    {
//                        Task<SettledTradeTransactionQueueArbitrage> SettledTradeTQMakerResult = MakeTransactionSettledEntry(TradeTransactionQueueObj, 0,1);
//                        TransactionQueueObj.MakeTransactionSuccess();
//                        TransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                        TransactionQueueObj.SetTransactionStatusMsg("Success with partial cancellation");

//                        TradeTransactionQueueObj.MakeTransactionSuccess();
//                        TradeTransactionQueueObj.SetTransactionCode(Convert.ToInt64(enErrorCode.CancelOrder_ProccedSuccess));
//                        TradeTransactionQueueObj.SetTransactionStatusMsg("Success with partial cancellation");
//                        TradeTransactionQueueObj.SettledDate = Helpers.UTC_To_IST();//Rita 17-1-19 update date for Reporting
//                        TradeTransactionQueueObj.UpdatedDate = Helpers.UTC_To_IST();
//                        TradeTransactionQueueObj.IsCancelled = 1;

//                        CurrentTradeSellerListObj.MakeTransactionSuccess();
//                        CurrentTradeSellerListObj.IsProcessing = 0;

//                        SettledTradeTransactionQueueArbitrage SettledTradeTQMakerObj = await SettledTradeTQMakerResult;
//                        _dbContext.Set<SettledTradeTransactionQueueArbitrage>().Add(SettledTradeTQMakerObj);
//                    }                   

//                    _dbContext.Entry(TransactionQueueObj).State = EntityState.Modified;
//                    _dbContext.Entry(TradeTransactionQueueObj).State = EntityState.Modified;
//                    //_dbContext.Entry(CurrentTradeSellerListObj).State = EntityState.Modified;
//                    _dbContext.Entry(CurrentTradeSellerListObj).Property(p => p.Status).IsModified = true;
//                    _dbContext.Entry(CurrentTradeSellerListObj).Property(p => p.IsProcessing).IsModified = true;

//                    _dbContext.SaveChanges();
//                    try
//                    {
//                        _ISignalRService.OnStatusCancelArbitrage(TradeTransactionQueueObj.Status, TransactionQueueObj, TradeTransactionQueueObj, "", TradeTransactionQueueObj.ordertype, IsPartial);

//                        Task.Run(() => EmailSendPartialSettledTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID.ToString(), TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.SellQty,
//                           TradeTransactionQueueObj.TrnDate.ToString(), 0, 0, TradeTransactionQueueObj.AskPrice, 0, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType, 2));
//                    }
//                    catch (Exception ex)
//                    {
//                        HelperForLog.WriteErrorLog("ReleaseWalletAmountSell Error ##TrnNo:" + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                    }
//                    return true;
//                }
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("ReleaseWalletAmountSell:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                return false;
//            }
//        }

//        private async Task<bool> CheckIsProcessingBUY(long TrnNo,TradeSellerListArbitrageV1 SellerList,decimal LTP)
//        {
//            try
//            {
//                Task<TradeStopLossArbitrage> TradeStopLossObjResult = _TradeStopLoss.GetSingleAsync(e => e.TrnNo == SellerList.TrnNo && (
//                                                            (e.MarketIndicator == 0 && e.StopPrice >= LTP) ||// 250 to 300 scenario
//                                                            (e.MarketIndicator == 1 && e.StopPrice <= LTP)// 300 to 350 scenario
//                                                            ));
//                try
//                {
//                    _dbContext1.Entry(SellerList).Reload();
//                }
//                catch(Exception ex)
//                {
//                    await Task.Delay(300);
//                    try
//                    {
//                        _dbContext1.Entry(SellerList).Reload();
//                    }
//                    catch (Exception ex1)
//                    {
//                        await Task.Delay(300);
//                        try
//                        {
//                            _dbContext1.Entry(SellerList).Reload();
//                        }
//                        catch (Exception ex2)
//                        {

//                        }
//                    }
//                }

                
//                if (SellerList.IsProcessing == 1)
//                {
//                    DateTime tTime = Helpers.UTC_To_IST().Add(new TimeSpan(0, 0, 0, 5, 0));//loop for 5 second only
//                    while (Helpers.UTC_To_IST() < tTime)
//                    {
//                        await Task.Delay(300);
//                        _dbContext1.Entry(SellerList).Reload();
//                        if (SellerList.IsProcessing == 1)
//                            continue;//still wait
//                        else
//                            break;//exit loop
//                    }
//                    //Thread.Sleep(3000);
//                    //_dbContext1.Entry(SellerList).Reload();
//                }
//                if (SellerList.IsProcessing == 1 || SellerList.RemainQty <= 0 || (SellerList.Status != 4 && SellerList.Status != 0))//rita 31-1-19 for cancellation , this order does not pick
//                {
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CheckIsProcessingBUY Buy skip IsProcessing " + SellerList.IsProcessing + " Remain Qty " + SellerList.RemainQty + " Status:" + SellerList.Status + " With " + SellerList.TrnNo, ControllerName, "##TrnNo " + TrnNo, Helpers.UTC_To_IST()));
//                    return false;
//                }
//                if (SellerList.OrderType == 4)
//                {
//                    TradeStopLossArbitrage TradeStopLossObj = await TradeStopLossObjResult;
//                    if (TradeStopLossObj == null)//if record NOT-FOUND then not possible for Activation-with-Settlement
//                        return false;
//                }               

//                return true;
//            }
//            catch(Exception ex)
//            {
//                HelperForLog.WriteErrorLog("CheckIsProcessingBUY:##TrnNo " + TrnNo + " With TrnNo:" + SellerList.TrnNo, ControllerName, ex);
//                return false;
//            }
//        }
//        private async Task<bool> CheckIsProcessingSELL(long TrnNo, TradeBuyerListArbitrageV1 BuyerList,decimal LTP)
//        {
//            try
//            {
//                Task<TradeStopLossArbitrage> TradeStopLossObjResult = _TradeStopLoss.GetSingleAsync(e => e.TrnNo == BuyerList.TrnNo && (
//                                                           (e.MarketIndicator == 0 && e.StopPrice >= LTP) ||// 250 to 300 scenario
//                                                           (e.MarketIndicator == 1 && e.StopPrice <= LTP)// 300 to 350 scenario
//                                                           ));
                
//                try
//                {
//                    _dbContext1.Entry(BuyerList).Reload();
//                }
//                catch (Exception ex)
//                {
//                    await Task.Delay(300);
//                    try
//                    {
//                        _dbContext1.Entry(BuyerList).Reload();
//                    }
//                    catch (Exception ex1)
//                    {
//                        await Task.Delay(300);
//                        try
//                        {
//                            _dbContext1.Entry(BuyerList).Reload();
//                        }
//                        catch (Exception ex2)
//                        {

//                        }
//                    }
//                }
//                if (BuyerList.IsProcessing == 1)
//                {
//                    DateTime tTime = Helpers.UTC_To_IST().Add(new TimeSpan(0, 0, 0, 5, 0));//loop for 5 second only
//                    while (Helpers.UTC_To_IST() < tTime)
//                    {
//                        await Task.Delay(300);
//                        _dbContext1.Entry(BuyerList).Reload();
//                        if (BuyerList.IsProcessing == 1)
//                            continue;//still wait
//                        else
//                            break;//exit loop
//                    }
//                    //Thread.Sleep(3000);
//                    //_dbContext1.Entry(SellerList).Reload();
//                }
//                if (BuyerList.IsProcessing == 1 || BuyerList.RemainQty <= 0 || (BuyerList.Status != 4 && BuyerList.Status != 0))//rita 31-1-19 for cancellation , this order does not pick
//                {
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CheckIsProcessingSELL Sell skip IsProcessing " + BuyerList.IsProcessing + " Remain Qty " + BuyerList.RemainQty + " Status:" + BuyerList.Status + " With " + BuyerList.TrnNo, ControllerName, "##TrnNo " + TrnNo, Helpers.UTC_To_IST()));
//                    return false;
//                }
//                if (BuyerList.OrderType == 4)
//                {
//                    TradeStopLossArbitrage TradeStopLossObj = await TradeStopLossObjResult;
//                    if (TradeStopLossObj == null)//if record NOT-FOUND then not possible for Activation-with-Settlement
//                        return false;
//                }
//                return true;
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("CheckIsProcessingSELL:##TrnNo " + TrnNo + " With TrnNo:" + BuyerList.TrnNo, ControllerName, ex);
//                return false;
//            }
//        }
               
             
//        private async Task BuyFailTransactionNotification(TradeBuyerListArbitrageV1 CurrentTradeBuyerListObj,long MemberID)
//        {
//            try
//            {
//                ActivityNotificationMessage notification = new ActivityNotificationMessage();
//                notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);                
//                notification.Param1 = CurrentTradeBuyerListObj.TrnNo.ToString();
//                notification.Type = Convert.ToInt16(EnNotificationType.Fail);
//                _ISignalRService.SendActivityNotificationV2Arbitrage(notification, MemberID.ToString(), 2);//Req.accessToken
//                                                                                                                      //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);

//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("BuyFailTransactionNotification Notification Send No Qty Found" + notification.MsgCode, ControllerName, "##TrnNo:" + CurrentTradeBuyerListObj.TrnNo, Helpers.UTC_To_IST()));
//            }
//            catch (Exception ex)
//            {                
//                HelperForLog.WriteErrorLog("BuyFailTransactionNotification:##TrnNo " + CurrentTradeBuyerListObj.TrnNo, ControllerName, ex);
//            }
//        }

//        private async Task SellFailTransactionNotification(TradeSellerListArbitrageV1 CurrentTradeSellerListObj, long MemberID)
//        {
//            try
//            {
//                ActivityNotificationMessage notification = new ActivityNotificationMessage();
//                notification.MsgCode = Convert.ToInt32(enErrorCode.TransactionValidationFail);
//                notification.Param1 = CurrentTradeSellerListObj.TrnNo.ToString();
//                notification.Type = Convert.ToInt16(EnNotificationType.Fail);
//                _ISignalRService.SendActivityNotificationV2Arbitrage(notification, MemberID.ToString(), 2);//Req.accessToken
//                                                                                                  //_ISignalRService.SendActivityNotificationV2(notification, Req.accessToken);

//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SellFailTransactionNotification Notification Send No Qty Found" + notification.MsgCode, ControllerName, "##TrnNo:" + CurrentTradeSellerListObj.TrnNo, Helpers.UTC_To_IST()));
//            }
//            catch (Exception ex)
//            {                
//                HelperForLog.WriteErrorLog("SellFailTransactionNotification:##TrnNo " + CurrentTradeSellerListObj.TrnNo, ControllerName, ex);
//            }
//        }

//        public async Task PartilaSettleBuySignalR(long TrnNo, TransactionQueueArbitrage TransactionQueueObj,TradeTransactionQueueArbitrage TradeTransactionQueueObj,TradeStopLossArbitrage TradeStopLossObj, TradeStopLossArbitrage MakeTradeStopLossObj, TransactionQueueArbitrage TQMakerObj, TradeTransactionQueueArbitrage TradeTQMakerObj,string accessToken,decimal SettlementPrice,decimal SettlementQty,short OrderType)
//        {
//            try
//            {
//                if(OrderType!=3)//SPOT order no partial order possible , so no send any data
//                {//do not send buyer-seller book , bcz no any order with this type of order
//                   await _ISignalRService.OnStatusPartialSuccessArbitrage(Convert.ToInt16(enTransactionStatus.Success), TransactionQueueObj, TradeTransactionQueueObj, "", TradeStopLossObj.ordertype);
//                    Task.Run(() => SMSSendSettledTransaction(TransactionQueueObj.Id, TransactionQueueObj.MemberMobile, SettlementPrice, SettlementQty, 1));
//                    Task.Run(() => EmailSendPartialSettledTransaction(TransactionQueueObj.Id, TransactionQueueObj.MemberID.ToString(), TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.BuyQty, TradeTransactionQueueObj.TrnDate.ToString(),
//                        TradeTransactionQueueObj.BuyQty - TradeTransactionQueueObj.SettledBuyQty, TradeTransactionQueueObj.SettledBuyQty, TradeTransactionQueueObj.BidPrice, 0, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType));
//                }
//                await _ISignalRService.OnStatusSuccessArbitrage(Convert.ToInt16(enTransactionStatus.Success), TQMakerObj, TradeTQMakerObj, "", MakeTradeStopLossObj.ordertype, SettlementPrice);
               
//                await _IFrontTrnService.GetPairAdditionalValArbitrage(TradeTQMakerObj.PairID, SettlementPrice, TradeTQMakerObj.TrnNo, SettlementQty, TradeTQMakerObj.TrnDate);//12-3-18 this is sell order so bifprice and BuyQty is zero , take settlement price and Qty
//                _ISignalRService.OnLtpChangeArbitrage(SettlementPrice, TradeTQMakerObj.PairID, TradeTQMakerObj.PairName);//rita 11-1-19 for Stop and Limit add/remove in book
                
//                Task.Run(() => EmailSendPartialSettledTransaction(TradeTQMakerObj.TrnNo, TradeTQMakerObj.MemberID.ToString(), TradeTQMakerObj.PairName, TradeTQMakerObj.SellQty,
//                    TradeTQMakerObj.TrnDate.ToString(), 0, 0, TradeTQMakerObj.AskPrice, 0, TradeTQMakerObj.ordertype, TradeTQMakerObj.TrnType, 1));

//                Task.Run(() => SMSSendSettledTransaction(TradeTQMakerObj.TrnNo, TQMakerObj.MemberMobile, SettlementPrice, SettlementQty, 2));
             
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("PartilaSettleBuySignalR:##TrnNo " + TrnNo, ControllerName, ex);
//            }
//        }

//        public async Task FullSettleBuySignalR(long TrnNo, TransactionQueueArbitrage TransactionQueueObj, TradeTransactionQueueArbitrage TradeTransactionQueueObj,TradeStopLossArbitrage TradeStopLossObj, TradeStopLossArbitrage MakeTradeStopLossObj, TransactionQueueArbitrage TQMakerObj, TradeTransactionQueueArbitrage TradeTQMakerObj,string accessToken, decimal SettlementPrice, decimal SettlementQty,TradeSellerListArbitrageV1 SellerList, short OrderType)
//        {
//            try
//            {
//                //Seller Txn partial Success;
//                if (SellerList.RemainQty == 0)//Full Success
//                {
//                    await _IFrontTrnService.GetPairAdditionalValArbitrage(TradeTQMakerObj.PairID, SettlementPrice, TradeTQMakerObj.TrnNo, SettlementQty, TradeTQMakerObj.TrnDate);//rita 06-12-18 for success maker txn send call
//                    await _ISignalRService.OnStatusSuccessArbitrage(Convert.ToInt16(enTransactionStatus.Success), TQMakerObj, TradeTQMakerObj, "", MakeTradeStopLossObj.ordertype, SettlementPrice);

//                    Task.Run(() => SMSSendSettledTransaction(TradeTQMakerObj.TrnNo, TQMakerObj.MemberMobile, SettlementPrice, SettlementQty, 2));

//                    Task.Run(() => EmailSendPartialSettledTransaction(TradeTQMakerObj.TrnNo, TQMakerObj.MemberID.ToString(), TradeTQMakerObj.PairName, TradeTQMakerObj.SellQty,
//                    TradeTQMakerObj.TrnDate.ToString(), 0, 0, TradeTQMakerObj.AskPrice, 0, TradeTQMakerObj.ordertype, TradeTQMakerObj.TrnType, 1));

//                }
//                else
//                {
//                    await _ISignalRService.OnStatusPartialSuccessArbitrage(Convert.ToInt16(enTransactionStatus.Success), TQMakerObj, TradeTQMakerObj, "", MakeTradeStopLossObj.ordertype);

//                    Task.Run(() => SMSSendSettledTransaction(TradeTQMakerObj.TrnNo, TQMakerObj.MemberMobile, SettlementPrice, SettlementQty, 1));

//                    Task.Run(() => EmailSendPartialSettledTransaction(TQMakerObj.Id, TQMakerObj.MemberID.ToString(), TradeTQMakerObj.PairName, TradeTQMakerObj.SellQty, TradeTQMakerObj.TrnDate.ToString(),
//                        TradeTQMakerObj.SellQty - TradeTQMakerObj.SettledSellQty, TradeTQMakerObj.SettledSellQty, TradeTQMakerObj.AskPrice, 0, TradeTQMakerObj.ordertype, TradeTQMakerObj.TrnType));
                    
//                }
//                await _IFrontTrnService.GetPairAdditionalValArbitrage(TradeTransactionQueueObj.PairID, SettlementPrice, TradeTransactionQueueObj.TrnNo, SettlementQty, TradeTransactionQueueObj.TrnDate);//12-3-18 take settlement price and Qty
//                await _ISignalRService.OnStatusSuccessArbitrage(Convert.ToInt16(enTransactionStatus.Success), TransactionQueueObj, TradeTransactionQueueObj, "", TradeStopLossObj.ordertype, SettlementPrice);//komal
//                _ISignalRService.OnLtpChangeArbitrage(SettlementPrice, TradeTQMakerObj.PairID, TradeTQMakerObj.PairName);//rita 11-1-19 for Stop and Limit add/remove in book
//                Task.Run(() => SMSSendSettledTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberMobile, SettlementPrice, SettlementQty, 2));

//                Task.Run(() => EmailSendPartialSettledTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberID.ToString(), TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.BuyQty,
//                    TradeTransactionQueueObj.TrnDate.ToString(), 0, 0, TradeTransactionQueueObj.BidPrice, 0, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType, 1));

//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("FullSettleBuySignalR:##TrnNo " + TrnNo, ControllerName, ex);
//            }
//        }
        
//        public async Task PartilaSettleSellSignalR(long TrnNo, TransactionQueueArbitrage TransactionQueueObj, TradeTransactionQueueArbitrage TradeTransactionQueueObj,TradeStopLossArbitrage TradeStopLossObj, TradeStopLossArbitrage MakeTradeStopLossObj, TransactionQueueArbitrage TQMakerObj, TradeTransactionQueueArbitrage TradeTQMakerObj,string accessToken, decimal SettlementPrice, decimal SettlementQty, short OrderType)
//        {
//            try
//            {                
//                Task.Run(() => EmailSendPartialSettledTransaction(TradeTQMakerObj.TrnNo, TQMakerObj.MemberID.ToString(), TradeTQMakerObj.PairName, TradeTQMakerObj.BuyQty,
//                    TradeTQMakerObj.TrnDate.ToString(), 0, 0, TradeTQMakerObj.BidPrice, 0, TradeTQMakerObj.ordertype, TradeTQMakerObj.TrnType, 1));

//                if (OrderType != 3)//SPOT order no partial order possible , so no send any data
//                {//do not send buyer-seller book , bcz no any order with this type of order
//                    await _ISignalRService.OnStatusPartialSuccessArbitrage(Convert.ToInt16(enTransactionStatus.Success), TransactionQueueObj, TradeTransactionQueueObj, "", TradeStopLossObj.ordertype);
//                    Task.Run(() => SMSSendSettledTransaction(TransactionQueueObj.Id, TransactionQueueObj.MemberMobile, SettlementPrice, SettlementQty, 1));
//                    Task.Run(() => EmailSendPartialSettledTransaction(TransactionQueueObj.Id, TransactionQueueObj.MemberID.ToString(), TradeTransactionQueueObj.PairName, TradeTransactionQueueObj.SellQty, TradeTransactionQueueObj.TrnDate.ToString(),
//                        TradeTransactionQueueObj.SellQty - TradeTransactionQueueObj.SettledSellQty, TradeTransactionQueueObj.SettledSellQty, TradeTransactionQueueObj.AskPrice, 0, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType));
//                }

//                //==============Volume update only after success,Maker/Seller's Txn Success   
//                await _ISignalRService.OnStatusSuccessArbitrage(Convert.ToInt16(enTransactionStatus.Success), TQMakerObj, TradeTQMakerObj, "", MakeTradeStopLossObj.ordertype, SettlementPrice);

//                await _IFrontTrnService.GetPairAdditionalValArbitrage(TradeTQMakerObj.PairID, SettlementPrice, TradeTQMakerObj.TrnNo, SettlementQty, TradeTQMakerObj.TrnDate);//12-3-18 take settlement price and Qty
                
//                _ISignalRService.OnLtpChangeArbitrage(SettlementPrice, TradeTQMakerObj.PairID, TradeTQMakerObj.PairName);//rita 11-1-19 for Stop and Limit add/remove in book
//                Task.Run(() => SMSSendSettledTransaction(TradeTQMakerObj.TrnNo, TQMakerObj.MemberMobile, SettlementPrice, SettlementQty, 2));
                
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("PartilaSettleSellSignalR:##TrnNo " + TrnNo, ControllerName, ex);
//            }
//        }

//        public async Task FullSettleSellSignalR(long TrnNo, TransactionQueueArbitrage TransactionQueueObj, TradeTransactionQueueArbitrage TradeTransactionQueueObj,TradeStopLossArbitrage TradeStopLossObj, TradeStopLossArbitrage MakeTradeStopLossObj, TransactionQueueArbitrage TQMakerObj, TradeTransactionQueueArbitrage TradeTQMakerObj,string accessToken, decimal SettlementPrice, decimal SettlementQty, TradeBuyerListArbitrageV1 BuyerList, short OrderType)
//        {
//            try
//            {
//                //Seller Txn partial Success;
//                if (BuyerList.RemainQty == 0)//Full Success
//                {
//                    await _IFrontTrnService.GetPairAdditionalValArbitrage(TradeTQMakerObj.PairID, SettlementPrice, TradeTQMakerObj.TrnNo, SettlementQty, TradeTQMakerObj.TrnDate);//rita 06-12-18 for success maker txn send call
//                    await _ISignalRService.OnStatusSuccessArbitrage(Convert.ToInt16(enTransactionStatus.Success), TQMakerObj, TradeTQMakerObj, "", MakeTradeStopLossObj.ordertype,SettlementPrice);

//                    Task.Run(() => SMSSendSettledTransaction(TradeTQMakerObj.TrnNo, TQMakerObj.MemberMobile, SettlementPrice, SettlementQty, 2));

//                    Task.Run(() => EmailSendPartialSettledTransaction(TradeTQMakerObj.TrnNo, TQMakerObj.MemberID.ToString(), TradeTQMakerObj.PairName, TradeTQMakerObj.BuyQty,
//                    TradeTQMakerObj.TrnDate.ToString(), 0, 0, TradeTQMakerObj.BidPrice, 0, TradeTQMakerObj.ordertype, TradeTQMakerObj.TrnType, 1));

//                }
//                else
//                {
//                    await _ISignalRService.OnStatusPartialSuccessArbitrage(Convert.ToInt16(enTransactionStatus.Success), TQMakerObj, TradeTQMakerObj, "", MakeTradeStopLossObj.ordertype);

//                    Task.Run(() => SMSSendSettledTransaction(TradeTQMakerObj.TrnNo, TQMakerObj.MemberMobile, SettlementPrice, SettlementQty, 1));

//                    Task.Run(() => EmailSendPartialSettledTransaction(TQMakerObj.Id, TQMakerObj.MemberID.ToString(), TradeTQMakerObj.PairName, TradeTQMakerObj.BuyQty, TradeTQMakerObj.TrnDate.ToString(),
//                        TradeTQMakerObj.BuyQty - TradeTQMakerObj.SettledBuyQty, TradeTQMakerObj.SettledBuyQty, TradeTQMakerObj.BidPrice, 0, TradeTransactionQueueObj.ordertype, TradeTransactionQueueObj.TrnType));
                    
//                }
//                await _IFrontTrnService.GetPairAdditionalValArbitrage(TradeTransactionQueueObj.PairID, SettlementPrice, TradeTransactionQueueObj.TrnNo, SettlementQty, TradeTransactionQueueObj.TrnDate);//12-3-18 take settlement price and Qty
//                await _ISignalRService.OnStatusSuccessArbitrage(Convert.ToInt16(enTransactionStatus.Success), TransactionQueueObj, TradeTransactionQueueObj, "", TradeStopLossObj.ordertype,SettlementPrice);//komal
//                _ISignalRService.OnLtpChangeArbitrage(SettlementPrice, TradeTQMakerObj.PairID, TradeTQMakerObj.PairName);//rita 11-1-19 for Stop and Limit add/remove in book
//                Task.Run(() => SMSSendSettledTransaction(TradeTransactionQueueObj.TrnNo, TransactionQueueObj.MemberMobile, SettlementPrice, SettlementQty, 2));
                
//                Task.Run(() => EmailSendPartialSettledTransaction(TradeTQMakerObj.TrnNo, TQMakerObj.MemberID.ToString(), TradeTQMakerObj.PairName, TradeTQMakerObj.SellQty,
//                   TradeTQMakerObj.TrnDate.ToString(), 0, 0, TradeTQMakerObj.AskPrice, 0, TradeTQMakerObj.ordertype, TradeTQMakerObj.TrnType, 1));

//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("FullSettleSellSignalR:##TrnNo " + TrnNo, ControllerName, ex);
//            }
//        }

//        public async Task InsertTradePoolQueue(long MemberID, long PairID, long MakerTrnNo, decimal MakerQty, decimal MakerPrice, long TakerTrnNo, decimal TakerQty, decimal TakerPrice, decimal TakerDisc, decimal TakerLoss, string MakerType, string TakerType)
//        {
//            try
//            {
//                //Rita inserted from sp
//                //TradePoolQueueObj = new TradePoolQueueV1()
//                //{
//                //    CreatedDate = Helpers.UTC_To_IST(),
//                //    CreatedBy = MemberID,
//                //    PairID = PairID,
//                //    MakerTrnNo = MakerTrnNo,
//                //    MakerQty = MakerQty,
//                //    MakerPrice = MakerPrice,
//                //    TakerTrnNo = TakerTrnNo,
//                //    TakerQty = TakerQty,
//                //    TakerPrice = TakerPrice,
//                //    TakerDisc = TakerDisc,
//                //    TakerLoss = TakerLoss,
//                //    MakerType = MakerType,
//                //    TakerType = TakerType,
//                //    Status = Convert.ToInt16(enTransactionStatus.Success),//always etry after settlement done
//                //};
//                //TradePoolQueueObj = _TradePoolQueue.Add(TradePoolQueueObj);
//                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommSuccessMsgInternal, ReturnCode = enResponseCodeService.Success });
//                TradePoolQueueObj = await _dbContext.Set<TradePoolQueueArbitrageV1>().FirstOrDefaultAsync(item => item.MakerTrnNo == MakerTrnNo && item.TakerTrnNo == TakerTrnNo && item.Status==0);
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("InsertTradePoolQueue:##TrnNo " + TakerTrnNo, ControllerName, ex);
//                //return (new BizResponse { ReturnMsg = EnResponseMessage.CommFailMsgInternal, ReturnCode = enResponseCodeService.InternalError });
//                throw ex;
//            }

//        }

//        private async Task<bool> WalletProcessBuy(TradeTransactionQueueArbitrage TradeTQueueCurrentObj, TradeTransactionQueueArbitrage TradeTQueueMakerObj, decimal Price,decimal SettledQty,decimal BaseCurrQty,short CurrIsFullSettled,short MakerIsFullSettled, short TrnMode, WalletDrCrResponse CreditWalletResult1)
//        {
//            CreditWalletResult = new WalletDrCrResponse();//here object pass not working in async method
//            try
//            {
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT WalletProcessBuy", ControllerName, "Start Wallet Operation With TrnNo:" + TradeTQueueMakerObj.TrnNo + "##TrnNo:" + TradeTQueueCurrentObj.TrnNo, Helpers.UTC_To_IST()));
//                //===============First-Base Currency======================================               
//                CommonClassCrDr CrDrFirstCurrObj = new CommonClassCrDr();

//                //TradeTQueueMakerObj credit wallet = +Price * SettledQty
//                WalletCrDr CrFirstCurrObj = new WalletCrDr();
//                CrFirstCurrObj.TrnRefNo = TradeTQueueMakerObj.TrnNo;
//                CrFirstCurrObj.WalletId = TradeTQueueMakerObj.DeliveryWalletID;//Cr WalletID
//                CrFirstCurrObj.trnType = enWalletTrnType.SellTrade;//enWalletTrnType.Cr_Trade //Rita 16-1-19 change as wallet team changed Enum
//                CrFirstCurrObj.isFullSettled = MakerIsFullSettled;
//                CrFirstCurrObj.IsMaker = 1;//rita 5-2-19 for charge , 1-Maker,2-taker
//                //TradeTQueueCurrentObj Debit wallet = -Price * SettledQty
//                WalletCrDr DrFirstCurrObj = new WalletCrDr();
//                DrFirstCurrObj.TrnRefNo = TradeTQueueCurrentObj.TrnNo;
//                DrFirstCurrObj.WalletId = TradeTQueueCurrentObj.OrderWalletID;//Dr WalletID
//                DrFirstCurrObj.trnType = enWalletTrnType.BuyTrade;//enWalletTrnType.Dr_Trade
//                DrFirstCurrObj.isFullSettled = CurrIsFullSettled;
//                DrFirstCurrObj.IsMaker = 2;//rita 5-2-19 for charge , 1-Maker,2-taker
//                if (TradeTQueueCurrentObj.ordertype == 2) // ntrivedi 07-12-2018
//                    DrFirstCurrObj.isMarketTrade = 1;

//                CrDrFirstCurrObj.Amount = BaseCurrQty;
//                CrDrFirstCurrObj.Coin = TradeTQueueCurrentObj.Order_Currency;
//                CrDrFirstCurrObj.creditObject = CrFirstCurrObj;
//                CrDrFirstCurrObj.debitObject = DrFirstCurrObj;
//                //===============Second Currency===========================================                         
//                CommonClassCrDr CrDrSecondCurrObj = new CommonClassCrDr();//Main trading currency
//                //TradeTQueueCurrentObj credit wallet = +SettledQty    
//                WalletCrDr CrSecondCurrObj = new WalletCrDr();//Current Buy Object
//                CrSecondCurrObj.TrnRefNo = TradeTQueueCurrentObj.TrnNo;
//                CrSecondCurrObj.WalletId = TradeTQueueCurrentObj.DeliveryWalletID;//Cr WalletID
//                CrSecondCurrObj.trnType = enWalletTrnType.BuyTrade;//enWalletTrnType.Cr_Trade
//                CrSecondCurrObj.isFullSettled = CurrIsFullSettled;
//                CrSecondCurrObj.IsMaker = 2;//rita 5-2-19 for charge , 1-Maker,2-taker
//                //TradeTQueueMakerObj Debit wallet = -SettledQty
//                WalletCrDr DrSecondCurrObj = new WalletCrDr();
//                DrSecondCurrObj.TrnRefNo = TradeTQueueMakerObj.TrnNo;
//                DrSecondCurrObj.WalletId = TradeTQueueMakerObj.OrderWalletID;//Cr WalletID
//                DrSecondCurrObj.trnType = enWalletTrnType.SellTrade;//enWalletTrnType.Dr_Trade
//                DrSecondCurrObj.isFullSettled = MakerIsFullSettled;
//                DrSecondCurrObj.IsMaker = 1;//rita 5-2-19 for charge , 1-Maker,2-taker
//                if (TradeTQueueMakerObj.ordertype == 2) // ntrivedi 07-12-2018
//                    DrSecondCurrObj.isMarketTrade = 1;

//                CrDrSecondCurrObj.Amount = SettledQty;
//                CrDrSecondCurrObj.Coin = TradeTQueueCurrentObj.Delivery_Currency;
//                CrDrSecondCurrObj.creditObject = CrSecondCurrObj;
//                CrDrSecondCurrObj.debitObject = DrSecondCurrObj;

//                //WalletDrCrResponse //rita 10-1-19 for make order fail base on error code
//                //InsufficientMarketInternalBalanceCheckFirstCurrencyForDifferentialAmountFailed = 4347
//                //InsufficientMarketInternalBalanceCheckSecondCurrencyForDifferentialAmountFailed = 4348
//                //InsufficientOutgoingBalFirstCur = 4345
//                //InsufficietOutgoingBalSecondCur = 4346
//                //CrDrCredit_SettledBalMismatchCrWallet = 4364
//                //CrDrCredit_SettledBalMismatchCrWalletSecCur = 4365
//                //CrDrCredit_SettledBalMismatchDrWallet = 4366
//                //CrDrCredit_SettledBalMismatchDrWalletSecDr = 4367
//                CreditWalletResult = await _WalletService.GetArbitrageWalletCreditDrForHoldNewAsyncFinal(CrDrSecondCurrObj, CrDrFirstCurrObj,
//                                          Helpers.GetTimeStamp(), enServiceType.Trading, (EnAllowedChannels)TrnMode);

//                if (CreditWalletResult.ReturnCode != enResponseCode.Success)
//                {
//                    Task.Run(()=> HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT WalletProcessBuy", ControllerName, "Wallet Operation fail " + CreditWalletResult.ReturnMsg + " With TrnNo:" + TradeTQueueMakerObj.TrnNo + "##TrnNo:" + TradeTQueueCurrentObj.TrnNo, Helpers.UTC_To_IST()));
//                    return false;
//                }
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT WalletProcessBuy", ControllerName, "End Wallet Operation With TrnNo:" + TradeTQueueMakerObj.TrnNo + "##TrnNo:" + TradeTQueueCurrentObj.TrnNo, Helpers.UTC_To_IST()));

//                return true;
//            }
//            catch(Exception ex)
//            {
//                HelperForLog.WriteErrorLog("WalletProcessBuy:##TrnNo " + TradeTQueueCurrentObj.TrnNo, ControllerName, ex);
//                return false;
//            }
//        }
//        private async Task<bool> WalletProcessSell(TradeTransactionQueueArbitrage TradeTQueueCurrentObj, TradeTransactionQueueArbitrage TradeTQueueMakerObj, decimal Price, decimal SettledQty, decimal BaseCurrQty, short CurrIsFullSettled, short MakerIsFullSettled, short TrnMode,WalletDrCrResponse CreditWalletResult1)
//        {
//            CreditWalletResult = new WalletDrCrResponse();//here object pass not working in async method
//            try
//            {
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT WalletProcessSell", ControllerName, "Start Wallet Operation With TrnNo:" + TradeTQueueMakerObj.TrnNo + "##TrnNo:" + TradeTQueueCurrentObj.TrnNo, Helpers.UTC_To_IST()));
//                //===============First-Base Currency======================================               
//                CommonClassCrDr CrDrFirstCurrObj = new CommonClassCrDr();

//                //TradeTQueueCurrentObj credit wallet = -Price * SettledQty
//                WalletCrDr CrFirstCurrObj = new WalletCrDr();
//                CrFirstCurrObj.TrnRefNo = TradeTQueueCurrentObj.TrnNo;
//                CrFirstCurrObj.WalletId = TradeTQueueCurrentObj.DeliveryWalletID;//Cr WalletID to Current Seller
//                CrFirstCurrObj.trnType = enWalletTrnType.SellTrade;//enWalletTrnType.Cr_Trade //Rita 16-1-19 change as wallet team changed Enum
//                CrFirstCurrObj.isFullSettled = CurrIsFullSettled;
//                CrFirstCurrObj.IsMaker = 2;//rita 5-2-19 for charge , 1-Maker,2-taker
//                //TradeTQueueMakerObj Debit wallet = +Price * SettledQty
//                WalletCrDr DrFirstCurrObj = new WalletCrDr();
//                DrFirstCurrObj.TrnRefNo = TradeTQueueMakerObj.TrnNo;
//                DrFirstCurrObj.WalletId = TradeTQueueMakerObj.OrderWalletID;//Dr WalletID of Buyer
//                DrFirstCurrObj.trnType = enWalletTrnType.BuyTrade;
//                DrFirstCurrObj.isFullSettled = MakerIsFullSettled;
//                DrFirstCurrObj.IsMaker = 1;//rita 5-2-19 for charge , 1-Maker,2-taker
//                if (TradeTQueueMakerObj.ordertype == 2) // ntrivedi 07-12-2018
//                    DrFirstCurrObj.isMarketTrade = 1;

//                CrDrFirstCurrObj.Amount = BaseCurrQty;
//                CrDrFirstCurrObj.Coin = TradeTQueueCurrentObj.Delivery_Currency;
//                CrDrFirstCurrObj.creditObject = CrFirstCurrObj;
//                CrDrFirstCurrObj.debitObject = DrFirstCurrObj;
//                //===============Second Currency===========================================
//                CommonClassCrDr CrDrSecondCurrObj = new CommonClassCrDr();//Main trading currency
//                //TradeTQueueMakerObj credit wallet = +SettledQty
//                WalletCrDr CrSecondCurrObj = new WalletCrDr();//Current Buy Object
//                CrSecondCurrObj.TrnRefNo = TradeTQueueMakerObj.TrnNo;
//                CrSecondCurrObj.WalletId = TradeTQueueMakerObj.DeliveryWalletID;//Cr WalletID to Buyer
//                CrSecondCurrObj.trnType = enWalletTrnType.BuyTrade;
//                CrSecondCurrObj.isFullSettled = MakerIsFullSettled;
//                CrSecondCurrObj.IsMaker = 1;//rita 5-2-19 for charge , 1-Maker,2-taker
//                //TradeTQueueCurrentObj Debit wallet = -SettledQty
//                WalletCrDr DrSecondCurrObj = new WalletCrDr();
//                DrSecondCurrObj.TrnRefNo = TradeTQueueCurrentObj.TrnNo;
//                DrSecondCurrObj.WalletId = TradeTQueueCurrentObj.OrderWalletID;//Dr WalletID of Current Seller
//                DrSecondCurrObj.trnType = enWalletTrnType.SellTrade;
//                DrSecondCurrObj.isFullSettled = CurrIsFullSettled;
//                DrSecondCurrObj.IsMaker = 2;//rita 5-2-19 for charge , 1-Maker,2-taker
//                if (TradeTQueueCurrentObj.ordertype == 2) // ntrivedi 07-12-2018
//                    DrSecondCurrObj.isMarketTrade = 1;

//                CrDrSecondCurrObj.Amount = SettledQty;
//                CrDrSecondCurrObj.Coin = TradeTQueueCurrentObj.Order_Currency;
//                CrDrSecondCurrObj.creditObject = CrSecondCurrObj;
//                CrDrSecondCurrObj.debitObject = DrSecondCurrObj;

//                //WalletDrCrResponse //rita 10-1-19 for make order fail base on error code               

//                CreditWalletResult = await _WalletService.GetArbitrageWalletCreditDrForHoldNewAsyncFinal(CrDrSecondCurrObj, CrDrFirstCurrObj,
//                                           Helpers.GetTimeStamp(), enServiceType.Trading,(EnAllowedChannels)TrnMode);

//                if (CreditWalletResult.ReturnCode != enResponseCode.Success)
//                {
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT WalletProcessBuy", ControllerName, "Wallet Operation fail " + CreditWalletResult.ReturnMsg + " With TrnNo:" + TradeTQueueMakerObj.TrnNo + "##TrnNo:" + TradeTQueueCurrentObj.TrnNo, Helpers.UTC_To_IST()));
//                    return false;
//                }
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("PROCESSSETLLEMENT WalletProcessSell", ControllerName, "End Wallet Operation With TrnNo:" + TradeTQueueMakerObj.TrnNo + "##TrnNo:" + TradeTQueueCurrentObj.TrnNo, Helpers.UTC_To_IST()));
//                return true;
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("WalletProcessSell:##TrnNo " + TradeTQueueCurrentObj.TrnNo, ControllerName, ex);
//                return false;
//            }
//        }

//        public async Task<SettledTradeTransactionQueueArbitrage> MakeTransactionSettledEntry(TradeTransactionQueueArbitrage TradeTransactionQueueObj, decimal Price, short IsCancelled=0)
//        {
//            decimal BidPrice= TradeTransactionQueueObj.BidPrice;
//            decimal AskPrice= TradeTransactionQueueObj.AskPrice;
//            if (TradeTransactionQueueObj.ordertype == 2)
//            {
//                if (TradeTransactionQueueObj.TrnType == 4)//Buy
//                {
//                    BidPrice = Price;
//                }
//                else//Sell
//                {
//                    AskPrice = Price;
//                }
//            }
//            try
//            {
//                SettledTradeTransactionQueueArbitrage SettledTradeTQObj = new SettledTradeTransactionQueueArbitrage()
//                {
//                    CreatedDate = Helpers.UTC_To_IST(),//rita 29-1-19 added for log
//                    TrnNo = TradeTransactionQueueObj.TrnNo,
//                    TrnDate = TradeTransactionQueueObj.TrnDate,
//                    SettledDate = Helpers.UTC_To_IST(),
//                    TrnType = TradeTransactionQueueObj.TrnType,
//                    TrnTypeName = TradeTransactionQueueObj.TrnTypeName,
//                    MemberID = TradeTransactionQueueObj.MemberID,
//                    PairID = TradeTransactionQueueObj.PairID,
//                    PairName = TradeTransactionQueueObj.PairName,
//                    OrderWalletID = TradeTransactionQueueObj.OrderWalletID,
//                    DeliveryWalletID = TradeTransactionQueueObj.DeliveryWalletID,
//                    OrderAccountID = TradeTransactionQueueObj.OrderAccountID,
//                    DeliveryAccountID = TradeTransactionQueueObj.DeliveryAccountID,
//                    BuyQty = TradeTransactionQueueObj.BuyQty,
//                    BidPrice = BidPrice,
//                    SellQty = TradeTransactionQueueObj.SellQty,
//                    AskPrice = AskPrice,
//                    Order_Currency = TradeTransactionQueueObj.Order_Currency,
//                    OrderTotalQty = TradeTransactionQueueObj.OrderTotalQty,
//                    Delivery_Currency = TradeTransactionQueueObj.Delivery_Currency,
//                    DeliveryTotalQty = TradeTransactionQueueObj.DeliveryTotalQty,
//                    SettledBuyQty = TradeTransactionQueueObj.SettledBuyQty,
//                    SettledSellQty = TradeTransactionQueueObj.SettledSellQty,
//                    Status = 1,// TradeTransactionQueueObj.Status,//rita 3-1-19 every time only success save
//                    StatusCode = TradeTransactionQueueObj.StatusCode,
//                    StatusMsg = TradeTransactionQueueObj.StatusMsg,
//                    IsCancelled= IsCancelled
//                };
//                return SettledTradeTQObj;
//            }
//            catch(Exception ex)
//            {
//                HelperForLog.WriteErrorLog("MakeTransactionSettledEntry:##TrnNo " + TradeTransactionQueueObj.TrnNo, ControllerName, ex);
//                throw ex;
//            }
//        }        

//        #region Repository SP
//        public async Task<bool> Callsp_TradeSettlement(long TrnNo,long MakerTrnNo,decimal SettlementQty,decimal BaseCurrQty,decimal SettlementPrice,short ActionType,short ActionStage, long ErrorCode = 0, long UpdatedBy = 0)
//        {
//            bool ReturnCode = false;
//            //long ErrorCode = 0;//rita 20-3-19 pass in sp in case of reverse wallet
//            try
//            {
//                SqlParameter[] param1 = new SqlParameter[]{
//                new SqlParameter("@TrnNo",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Default,TrnNo),
//                new SqlParameter("@MakerTrnNo",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Default,MakerTrnNo),
//                new SqlParameter("@SettlementQty",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, string.Empty, DataRowVersion.Default, SettlementQty),
//                new SqlParameter("@BaseCurrQty",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, string.Empty, DataRowVersion.Default, BaseCurrQty),
//                new SqlParameter("@SettlementPrice",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, string.Empty, DataRowVersion.Default, SettlementPrice),
//                new SqlParameter("@ActionType",SqlDbType.TinyInt, 10, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Default, ActionType) ,
//                new SqlParameter("@ActionStage",SqlDbType.TinyInt, 10, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Default, ActionStage) ,
//                new SqlParameter("@ReturnCode",SqlDbType.Bit, 10, ParameterDirection.Output, false, 28, 8, string.Empty, DataRowVersion.Default, ReturnCode),
//                new SqlParameter("@ErrorCode",SqlDbType.BigInt, 10, ParameterDirection.InputOutput, false, 0, 0, string.Empty, DataRowVersion.Default, ErrorCode),
//                new SqlParameter("@UpdatedBy",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Default, UpdatedBy)
//            };
//                var res = _dbContext2.Database.ExecuteSqlCommand("Sp_TradeSettlementArbitrage @TrnNo ,@MakerTrnNo,@SettlementQty,@BaseCurrQty,@SettlementPrice,@ActionType,@ActionStage,@ReturnCode  OUTPUT,@ErrorCode  OUTPUT,@UpdatedBy", param1);
//                ReturnCode = Convert.ToBoolean(@param1[7].Value);
//                ErrorCode = Convert.ToInt64(@param1[8].Value);
//                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("Sp_TradeSettlementArbitrage", ControllerName, "##TrnNo:" + TrnNo + " ,MakerTrnNo=" + MakerTrnNo + 
//                    ",SettlementQty=" + SettlementQty + ",BaseCurrQty=" + BaseCurrQty + ",SettlementPrice=" + SettlementPrice + ",ActionType=" + ActionType + ",ActionStage=" + ActionStage + ",ReturnCode=" + ReturnCode + ",ErrorCode=" + ErrorCode + ",UpdatedBy=" + UpdatedBy));


//                return ReturnCode;
//            }
//            catch (Exception ex)
//            {
//                HelperForLog.WriteErrorLog("Callsp_TradeSettlement Arbitrage ##TrnNo " + TrnNo, ControllerName, ex);
//                throw ex;
//            }
//        }
//        #endregion

//        #endregion

//        #region Send SMS And Email
//        public async Task SMSSendSettledTransaction(long TrnNo, string MobileNumber, decimal Price, decimal Qty, int SettleType)
//        {
//            try
//            {
//                if (!string.IsNullOrEmpty(MobileNumber))
//                {
//                    SendSMSRequest SendSMSRequestObj = new SendSMSRequest();
//                    ApplicationUser User = new ApplicationUser();
//                    TemplateMasterData SmsData = new TemplateMasterData();

//                    CommunicationParamater communicationParamater = new CommunicationParamater();
//                    communicationParamater.Param1 = TrnNo + "";
//                    communicationParamater.Param2 = Price + "";
//                    communicationParamater.Param3 = Qty + "";
//                    communicationParamater.Param4 = (Price * Qty) + "";

//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SendSMSTransaction - SMSSendSettledTransaction", ControllerName, " ##TrnNo : " + TrnNo + " ##MobileNo : " + MobileNumber + " ##Price : " + Price + " ##Qty : " + Qty + " ##Type : " + SettleType, Helpers.UTC_To_IST()));

//                    if (SettleType == 1) // Partially Settelement
//                    {
//                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_TransactionPartiallySettled, communicationParamater, enCommunicationServiceType.SMS).Result;
//                    }
//                    else if (SettleType == 2) // Full Sattelement
//                    {
//                        SmsData = _messageService.ReplaceTemplateMasterData(EnTemplateType.SMS_TransactionSettled, communicationParamater, enCommunicationServiceType.SMS).Result;
//                    }
//                    if (SmsData != null)
//                    {
//                        if (SmsData.IsOnOff == 1)
//                        {
//                            SendSMSRequestObj.Message = SmsData.Content;
//                            SendSMSRequestObj.MobileNo = Convert.ToInt64(MobileNumber);
//                            _pushSMSQueue.Enqueue(SendSMSRequestObj);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Task.Run(() => HelperForLog.WriteErrorLog("SMSSendSettledTransaction:##TrnNo " + TrnNo, ControllerName, ex));
//            }
//        }

//        public async Task EmailSendPartialSettledTransaction(long TrnNo, string UserId, string Pair, decimal Qty, string TrnDate, decimal RemainingQty, decimal SettleQty, decimal Amount, decimal Fee, short OrderType, short TrnType,short EmailType = 0)
//        {
//            try
//            {
//                SendEmailRequest Request = new SendEmailRequest();
//                ApplicationUser User = new ApplicationUser();
//                TemplateMasterData EmailData = new TemplateMasterData();
//                CommunicationParamater communicationParamater = new CommunicationParamater();

//                User = await _userManager.FindByIdAsync(UserId);
//                if (!string.IsNullOrEmpty(User.Email))
//                {
//                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("SendEmailTransaction - EmailSendPartialSettledTransaction", ControllerName, " ##TrnNo : " + TrnNo, Helpers.UTC_To_IST()));

//                    //Uday 03-01-2019 Transaction Full Success email merge with this function

//                    if (EmailType == 0)  // Partial Email
//                    {
//                        communicationParamater.Param1 = User.UserName;
//                        communicationParamater.Param2 = Pair;
//                        communicationParamater.Param3 = Helpers.DoRoundForTrading(Qty, 8).ToString();
//                        communicationParamater.Param4 = Pair.Split("_")[1];
//                        communicationParamater.Param5 = TrnDate;
//                        communicationParamater.Param6 = Helpers.DoRoundForTrading(RemainingQty, 8).ToString();
//                        communicationParamater.Param7 = Helpers.DoRoundForTrading(SettleQty, 8).ToString();
//                        communicationParamater.Param8 = Helpers.DoRoundForTrading(Amount, 8).ToString();
//                        communicationParamater.Param9 = Helpers.DoRoundForTrading(Fee, 8).ToString();
//                        communicationParamater.Param10 = Helpers.DoRoundForTrading(((Amount * SettleQty) + Fee), 8).ToString();  //Uday 01-01-2019 In Final Price Calculation Change
//                        communicationParamater.Param11 = ((enTransactionMarketType)OrderType).ToString();  //Uday 01-01-2019 Add OrderType In Email
//                        communicationParamater.Param12 = ((enTrnType)TrnType).ToString();  //Uday 01-01-2019 Add TranType In Email
//                        communicationParamater.Param13 = TrnNo.ToString(); //Uday 01-01-2019 Add TrnNo In Email

//                        if (OrderType == 2) //Uday 04-01-2019 if ordertype is market than not give price and total
//                        {
//                            EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.Email_MArket_TransactionPartialSuccess, communicationParamater, enCommunicationServiceType.Email).Result;
//                        }
//                        else
//                        {
//                            EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_TransactionPartialSuccess, communicationParamater, enCommunicationServiceType.Email).Result;
//                        }
//                     }
//                    else if (EmailType == 1)  // Full Success Email
//                    {
//                        communicationParamater.Param1 = User.UserName;
//                        communicationParamater.Param2 = Pair;
//                        communicationParamater.Param3 = Helpers.DoRoundForTrading(Qty, 8).ToString();
//                        communicationParamater.Param4 = Pair.Split("_")[1];
//                        communicationParamater.Param5 = TrnDate;
//                        communicationParamater.Param6 = "Success";
//                        communicationParamater.Param8 = Helpers.DoRoundForTrading(Amount, 8).ToString();
//                        communicationParamater.Param9 = Helpers.DoRoundForTrading(Fee, 8).ToString();
//                        communicationParamater.Param10 = Helpers.DoRoundForTrading(((Amount * Qty) + Fee), 8).ToString();
//                        communicationParamater.Param11 = ((enTransactionMarketType)OrderType).ToString();
//                        communicationParamater.Param12 = ((enTrnType)TrnType).ToString();
//                        communicationParamater.Param13 = TrnNo.ToString();

//                        if (OrderType == 2) //Uday 04-01-2019 if ordertype is market than not give price and total
//                        {
//                            EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.Email_Market_TransactionSuccess, communicationParamater, enCommunicationServiceType.Email).Result;
//                        }
//                        else
//                        {
//                            EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.TransactionSuccess, communicationParamater, enCommunicationServiceType.Email).Result;
//                        }
//                    }
//                    else if (EmailType == 2)  //Rita 25-1-19 Cancel Order for STOP Order
//                    {
//                        communicationParamater.Param8 = User.UserName + "";
//                        communicationParamater.Param1 = Pair + "";
//                        communicationParamater.Param2 = Helpers.DoRoundForTrading(Qty, 8).ToString();
//                        communicationParamater.Param3 = Pair.Split("_")[1];
//                        communicationParamater.Param4 = TrnDate;
//                        communicationParamater.Param5 = Helpers.DoRoundForTrading(Amount, 8).ToString();
//                        communicationParamater.Param6 = Helpers.DoRoundForTrading(Fee, 8).ToString();
//                        communicationParamater.Param7 = Helpers.DoRoundForTrading((Fee + (Amount * Qty)), 8).ToString();
//                        communicationParamater.Param11 = ((enTransactionMarketType)OrderType).ToString();
//                        communicationParamater.Param12 = ((enTrnType)TrnType).ToString();
//                        communicationParamater.Param13 = TrnNo.ToString();

//                        short CancelType = 1;//here used only for Fully cancelled
//                        if (CancelType == 1) // Cancel Success
//                        {
//                            EmailData = _messageService.ReplaceTemplateMasterData(EnTemplateType.EMAIL_OrderCancel, communicationParamater, enCommunicationServiceType.Email).Result;
//                        }
//                        //else if (CancelType == 2) // Cancel Partially
//                        //{
//                        //    communicationParamater.Param9 = Helpers.DoRoundForTrading(SettledQty, 8).ToString();
//                        //    communicationParamater.Param10 = Helpers.DoRoundForTrading(CancelQty, 8).ToString();
//                        //    communicationParamater.Param7 = Helpers.DoRoundForTrading((fee + (price * SettledQty)), 8).ToString();     //Uday 01-01-2019 In Final Price Calculation Change

//                        //    EmailData = _messageService.SendMessageAsync(EnTemplateType.EMAIL_PartialOrderCancel, communicationParamater, enCommunicationServiceType.Email).Result;
//                        //}
//                        //else if (CancelType == 3) // Cancel Failed
//                        //{
//                        //    communicationParamater.Param7 = Helpers.DoRoundForTrading(0, 8).ToString();
//                        //    EmailData = _messageService.SendMessageAsync(EnTemplateType.EMAIL_OrderCancelFailed, communicationParamater, enCommunicationServiceType.Email).Result;
//                        //}
//                    }

//                    if (EmailData != null)
//                    {
//                        Request.Body = EmailData.Content;
//                        Request.Subject = EmailData.AdditionalInfo;
//                        Request.Recepient = User.Email;
//                        Request.EmailType = 0;
//                        _pushNotificationsQueue.Enqueue(Request);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Task.Run(() => HelperForLog.WriteErrorLog("EmailSendCancelTransaction:##TrnNo " + TrnNo, ControllerName, ex));
//            }
//        }
//        #endregion
//    }

//    public class CCXTTokenResponseTxn
//    {
//        public string token { get; set; }
//    }
//}
