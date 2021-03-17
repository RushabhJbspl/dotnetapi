using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldex.Core.ViewModels.Transaction.MarketMaker;

namespace Worldex.Infrastructure.Services.Transaction
{

    public class BackOfficeTrnService : IBackOfficeTrnService
    {
        private readonly ILogger<FrontTrnService> _logger;
        private readonly IBackOfficeTrnRepository _backOfficeTrnRepository;
        private readonly ICommonRepository<TransactionQueue> _transactionQueueRepository;
        private readonly ICommonRepository<TradeTransactionQueue> _tradeTransactionRepository;
        private readonly ICommonRepository<TradeBuyRequest> _tradeBuyRepository;
        private readonly ICommonRepository<TradeCancelQueue> _tradeCancelQueueRepository;
        private readonly ICommonRepository<TransactionRequest> _TransactionRequest;

        private readonly IBasePage _basePage;
        private readonly ICommonRepository<PoolOrder> _poolOrderRepository;
        private readonly ICancelOrderProcessV1 _cancelOrderProcess;//Rita 5-2-19 for API cancellation mane new Class
        private readonly IWalletService _WalletService;
        private readonly ICommonRepository<ServiceMaster> _serviceMasterRepository;
        private readonly ICommonRepository<RouteConfiguration> _RouteConfiguration;
        private readonly ICommonRepository<WithdrawHistory> _WithdrawHistory;
        private readonly ICommonRepository<WithdrawERCTokenQueue> _WithdrawERCTokenQueue;

        TransactionRecon tradeReconObj;
        private readonly IWalletTransactionCrDr _walletTransactionCrDr;
        private readonly ITradeReconProcessV1 _tradeReconProcessV1; // khushali 18-03-2019 for Trade Recond
       // private readonly ITradeReconProcessArbitrageV1 _tradeReconProcessArbitrageV1;
        private readonly ICommonRepository<TransactionQueueArbitrage> _transactionQueueArbitrageRepository;

        public BackOfficeTrnService(ILogger<FrontTrnService> logger,
            IBackOfficeTrnRepository backOfficeTrnRepository,
            ICommonRepository<TransactionQueue> transactionQueueRepository,
            ICommonRepository<TradeTransactionQueue> tradeTransactionRepository,
            ICommonRepository<TradeBuyRequest> tradeBuyRepository,
            ICommonRepository<TradeCancelQueue> tradeCancelQueueRepository,
            IBasePage basePage, ICommonRepository<RouteConfiguration> RouteConfiguration,
            ICommonRepository<TransactionRequest> TransactionRequest,
            ICommonRepository<WithdrawHistory> WithdrawHistory,
            ICommonRepository<PoolOrder> poolOrderRepository,
            ICancelOrderProcessV1 cancelOrderProcess,
            IWalletService WalletService,
            ICommonRepository<ServiceMaster> serviceMasterRepository,
            IWalletTransactionCrDr walletTransactionCrDr,
            ICommonRepository<WithdrawERCTokenQueue> WithdrawERCTokenQueue,
            ITradeReconProcessV1 TradeReconProcessV1, // khushali 18-03-2019 for Trade Recond,
          //  ITradeReconProcessArbitrageV1 tradeReconProcessArbitrageV1,
            ICommonRepository<TransactionQueueArbitrage> transactionQueueArbitrageRepository
            )
        {
            _logger = logger;
            _WithdrawHistory = WithdrawHistory;
            _backOfficeTrnRepository = backOfficeTrnRepository;
            _transactionQueueRepository = transactionQueueRepository;
            _tradeTransactionRepository = tradeTransactionRepository;
            _tradeBuyRepository = tradeBuyRepository;
            _tradeCancelQueueRepository = tradeCancelQueueRepository;
            _basePage = basePage;
            _TransactionRequest = TransactionRequest;
            _poolOrderRepository = poolOrderRepository;
            _cancelOrderProcess = cancelOrderProcess;
            _WalletService = WalletService;
            _serviceMasterRepository = serviceMasterRepository;
            _walletTransactionCrDr = walletTransactionCrDr;
            _tradeReconProcessV1 = TradeReconProcessV1;
            _WithdrawERCTokenQueue = WithdrawERCTokenQueue;
            _RouteConfiguration = RouteConfiguration;
           // _tradeReconProcessArbitrageV1 = tradeReconProcessArbitrageV1;
            _transactionQueueArbitrageRepository = transactionQueueArbitrageRepository;
        }

        #region history methods
        
        public TradeSettledHistoryResponse TradeSettledHistory(int PageSize, int PageNo, long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, long TrnNo = 0, short IsMargin = 0)//Rita 23-2-19 for Margin Trading Data bit
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;

                TradeSettledHistoryResponse _Res = new TradeSettledHistoryResponse();
                List<TradeSettledHistory> list = new List<TradeSettledHistory>();
                //Uday 12-01-2019 Add pagination parameter
                if (IsMargin == 1)
                    list = _backOfficeTrnRepository.TradeSettledHistoryMargin(PageSize, PageNo, ref TotalPages, ref TotalCount, ref PageSize1, PairID, TrnType, OrderType, FromDate, Todate, MemberID, TrnNo);
                else
                    list = _backOfficeTrnRepository.TradeSettledHistory(PageSize, PageNo, ref TotalPages, ref TotalCount, ref PageSize1, PairID, TrnType, OrderType, FromDate, Todate, MemberID, TrnNo);


                if (list.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                _Res.PageSize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                _Res.TotalCount = list.Count;
                _Res.TotalPages = (long)Math.Ceiling(Convert.ToDouble(_Res.TotalCount) / _Res.PageSize);
                int skip = _Res.PageSize * (PageNo - 1);
                _Res.PageNo = PageNo;

                _Res.Response = list.Skip(skip).Take(_Res.PageSize).ToList();
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

        public TradingSummaryResponse GetTradingSummaryV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market, int PageSize, int PageNo, short IsMargin = 0) //Rita 4-2-19 for Margin Trading
        {
            try
            {
                TradingSummaryResponse _Res = new TradingSummaryResponse();
                List<TradingSummaryViewModel> list = new List<TradingSummaryViewModel>();

                if (IsMargin == 1) //Rita 4-2-19 for Margin Trading
                    list = _backOfficeTrnRepository.GetTradingSummaryMarginV1(MemberID, FromDate, ToDate, TrnNo, status, SMSCode, PairID, trade, Market);
                else
                    list =  _backOfficeTrnRepository.GetTradingSummaryV1(MemberID, FromDate, ToDate, TrnNo, status, SMSCode, PairID, trade, Market);

                if (list.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                _Res.PageSize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                _Res.TotalCount = list.Count;
                _Res.TotalPages = (long)Math.Ceiling(Convert.ToDouble(_Res.TotalCount) / _Res.PageSize);
                int skip = _Res.PageSize * (PageNo - 1);
                _Res.PageNo = PageNo;
                _Res.Response = list.Skip(skip).Take(_Res.PageSize).ToList();
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

        public TradingSummaryLPResponse GetTradingSummaryLPV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market, int PageSize, int PageNo, string LPType)
        {
            try
            {
                TradingSummaryLPResponse _Res = new TradingSummaryLPResponse();
                var Modellist = _backOfficeTrnRepository.GetTradingSummaryLPV1(MemberID, FromDate, ToDate, TrnNo, status, SMSCode, PairID, trade, Market, LPType);
                if (Modellist.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                _Res.PageSize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                _Res.TotalCount = Modellist.Count;
                _Res.TotalPages = (long)Math.Ceiling(Convert.ToDouble(_Res.TotalCount) / _Res.PageSize);
                int skip = _Res.PageSize * (PageNo - 1);
                _Res.PageNo = PageNo;
                _Res.Response = Modellist.Skip(skip).Take(_Res.PageSize).ToList();
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

        public TradingReconHistoryResponse GetTradingReconHistoryV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, long PairID, short trade, short Market, int PageSize, int PageNo, int LPType, short? IsProcessing)
        {
            try
            {
                TradingReconHistoryResponse _Res = new TradingReconHistoryResponse();
                List<TradingReconHistoryViewModel> list = new List<TradingReconHistoryViewModel>();
                var Modellist = _backOfficeTrnRepository.GetTradingReconHistoryV1(MemberID, FromDate, ToDate, TrnNo, status, PairID, trade, Market, PageSize, PageNo, LPType, IsProcessing);
                if (Modellist.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                _Res.PageSize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                _Res.TotalCount = Modellist.Count;
                _Res.TotalPages = (long)Math.Ceiling(Convert.ToDouble(_Res.TotalCount) / _Res.PageSize);
                int skip = _Res.PageSize * (PageNo - 1);
                _Res.PageNo = PageNo;

                _Res.Response = Modellist.Skip(skip).Take(_Res.PageSize).ToList();
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

        public TradeSettledHistoryResponseV1 TradeSettledHistoryV1(int PageSize, int PageNo, long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, string TrnNo = "", short IsMargin = 0)//Rita 23-2-19 for Margin Trading Data bit
        {
            try
            {
                TradeSettledHistoryResponseV1 _Res = new TradeSettledHistoryResponseV1();
                List<TradeSettledHistoryV1> list = new List<TradeSettledHistoryV1>();
                //Uday 12-01-2019 Add pagination parameter
                if (IsMargin == 1)
                    list = _backOfficeTrnRepository.TradeSettledHistoryMarginV1(PairID, TrnType, OrderType, FromDate, Todate, MemberID, TrnNo);
                else
                    list = _backOfficeTrnRepository.TradeSettledHistoryV1(PairID, TrnType, OrderType, FromDate, Todate, MemberID, TrnNo);


                if (list.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                _Res.PageSize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                _Res.TotalCount = list.Count;
                _Res.TotalPages = (long)Math.Ceiling(Convert.ToDouble(_Res.TotalCount) / _Res.PageSize);
                int skip = _Res.PageSize * (PageNo - 1);
                _Res.PageNo = PageNo;

                _Res.Response = list.Skip(skip).Take(_Res.PageSize).ToList();
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

        #region Arbitrage History

        public TradeSettledHistoryResponse TradeSettledHistoryArbitrage(int PageSize, int PageNo, long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, long TrnNo = 0, short IsMargin = 0)
        {
            try
            {
                long TotalCount = 0;
                int PageSize1 = 0;
                long TotalPages = 0;
                TradeSettledHistoryResponse _Res = new TradeSettledHistoryResponse();
                List<TradeSettledHistory> list = new List<TradeSettledHistory>();
                //Uday 12-01-2019 Add pagination parameter
                list = _backOfficeTrnRepository.TradeSettledHistoryArbitrageInfo(PageSize, PageNo, ref TotalPages, ref TotalCount, ref PageSize1, PairID, TrnType, OrderType, FromDate, Todate, MemberID, TrnNo);
                if (list.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                _Res.PageSize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                _Res.TotalCount = list.Count;
                _Res.TotalPages = (long)Math.Ceiling(Convert.ToDouble(_Res.TotalCount) / _Res.PageSize);
                int skip = _Res.PageSize * (PageNo - 1);
                _Res.PageNo = PageNo;
                _Res.Response = list.Skip(skip).Take(_Res.PageSize).ToList();
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

        public TradingReconHistoryResponse GetTradingReconHistoryArbitrageV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, long PairID, short trade, short Market, int PageSize, int PageNo, int LPType, short? IsProcessing)
        {
            TradingReconHistoryResponse _Res = new TradingReconHistoryResponse();
            try
            {
                var Modellist = _backOfficeTrnRepository.GetTradingReconHistoryArbitrageV1(MemberID, FromDate, ToDate, TrnNo, status, PairID, trade, Market, LPType, IsProcessing);
                if (Modellist.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                _Res.PageSize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                _Res.TotalCount = Modellist.Count;
                _Res.TotalPages = (long)Math.Ceiling(Convert.ToDouble(_Res.TotalCount) / _Res.PageSize);
                int skip = _Res.PageSize * (PageNo - 1);
                _Res.PageNo = PageNo;
                _Res.Response = Modellist.Skip(skip).Take(_Res.PageSize).ToList();
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

        public TradingSummaryResponse GetTradingSummaryArbitrageV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market, int PageSize, int PageNo)
        {
            TradingSummaryResponse _Res = new TradingSummaryResponse();
            try
            {
                var Modellist = _backOfficeTrnRepository.GetTradingSummaryArbitrageInfoV1(MemberID, FromDate, ToDate, TrnNo, status, SMSCode, PairID, trade, Market);

                if (Modellist.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                _Res.PageSize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                _Res.TotalCount = Modellist.Count;
                _Res.TotalPages = (long)Math.Ceiling(Convert.ToDouble(_Res.TotalCount) / _Res.PageSize);
                int skip = _Res.PageSize * (PageNo - 1);
                _Res.PageNo = PageNo;
                _Res.Response = Modellist.Skip(skip).Take(_Res.PageSize).ToList();
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

        public TradingSummaryLPResponse GetTradingSummaryLPArbitrageV1(long MemberID, string FromDate, string ToDate, string TrnNo, short status, string SMSCode, long PairID, short trade, short Market, int PageSize, int PageNo, string LPType)
        {
            TradingSummaryLPResponse _Res = new TradingSummaryLPResponse();
            try
            {
                var Modellist = _backOfficeTrnRepository.GetTradingSummaryLPArbitrageInfoV1(MemberID, FromDate, ToDate, TrnNo, status, SMSCode, PairID, trade, Market, LPType);
                if (Modellist.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                _Res.PageSize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                _Res.TotalCount = Modellist.Count;
                _Res.TotalPages = (long)Math.Ceiling(Convert.ToDouble(_Res.TotalCount) / _Res.PageSize);
                int skip = _Res.PageSize * (PageNo - 1);
                _Res.PageNo = PageNo;
                _Res.Response = Modellist.Skip(skip).Take(_Res.PageSize).ToList();
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

        public TradeSettledHistoryResponseV1 TradeSettledHistoryArbitrageV1(int PageSize, int PageNo, long PairID = 999, short TrnType = 999, short OrderType = 999, string FromDate = "", string Todate = "", long MemberID = 0, string TrnNo = "", short IsMargin = 0)
        {
            TradeSettledHistoryResponseV1 _Res = new TradeSettledHistoryResponseV1();
            try
            {
                //Uday 12-01-2019 Add pagination parameter
                var list = _backOfficeTrnRepository.TradeSettledHistoryArbitrageInfoV1(PairID, TrnType, OrderType, FromDate, Todate, MemberID, TrnNo);
                if (list.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                _Res.PageSize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                _Res.TotalCount = list.Count;
                _Res.TotalPages = (long)Math.Ceiling(Convert.ToDouble(_Res.TotalCount) / _Res.PageSize);
                int skip = _Res.PageSize * (PageNo - 1);
                _Res.PageNo = PageNo;
                _Res.Response = list.Skip(skip).Take(_Res.PageSize).ToList();
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

        #region Recon method

        public async Task<BizResponseClass> TradeReconV1(enTradeReconActionType ActionType, long TranNo, string ActionMessage, long UserId, string accessToken)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                var transactionQueue = _transactionQueueRepository.GetById(TranNo);
                if (transactionQueue != null)
                {
                    var datediff = _basePage.UTC_To_IST() - transactionQueue.TrnDate;
                    if (UserId != 1 && datediff.Days > 7)
                    {
                        //After 7 days of transaction you can not take action, Please contact admin
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.TradeRecon_After7DaysTranDontTakeAction;
                        Response.ErrorCode = enErrorCode.TradeRecon_After7DaysTranDontTakeAction;
                        return Response;
                    }
                    else
                    {
                        Response = await _tradeReconProcessV1.TradeReconProcessAsyncV1(ActionType, TranNo, ActionMessage, UserId, accessToken);
                        return Response;
                    }
                }
                else
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionNo;
                    Response.ErrorCode = enErrorCode.TradeRecon_InvalidTransactionNo;
                    return Response;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        
        public BizResponseClass WithdrawalRecon(WithdrawalReconRequest request, long UserId, string accessToken)
        {
            BizResponseClass Response = new BizResponseClass();
            WithdrawHistory WithdrawHistoryObj = null;
            TransactionRequest TransactionRequestobj = null;
            WithdrawERCTokenQueue _WithdrawERCTokenQueueObj = null;
            short IsInsert = 2;
            try
            {
                var TransactionQueueObj = _transactionQueueRepository.GetById(request.TrnNo);
                if (TransactionQueueObj == null)
                {
                    Response.ErrorCode = enErrorCode.WithdrawalRecon_NoRecordFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.WithdrawalRecon_NoRecordFound;
                    return Response;
                }
                if (TransactionQueueObj.TrnType != Convert.ToInt16(enTrnType.Withdraw))
                {
                    Response.ErrorCode = enErrorCode.WithdrawalRecon_InvalidTrnType;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.WithdrawalRecon_InvalidTrnType;
                    return Response;
                }

                if (request.ActionType == enWithdrawalReconActionType.Refund) //Only For Success And Hold transaction are allowed
                {
                    if (!(TransactionQueueObj.Status == Convert.ToInt16(enTransactionStatus.Success) || TransactionQueueObj.Status == Convert.ToInt16(enTransactionStatus.Hold)))
                    {
                        Response.ErrorCode = enErrorCode.WithdrawalRecon_InvalidActionType;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.WithdrawalRecon_InvalidActionType;
                        return Response;
                    }
                    else
                    {
                        TransactionReconEntry(request.TrnNo, enTransactionStatus.Refunded, TransactionQueueObj.Status, TransactionQueueObj.SerProID, TransactionQueueObj.SerProID, request.ActionMessage, UserId);

                        TransactionQueueObj.Status = Convert.ToInt16(enTransactionStatus.Refunded);
                        TransactionQueueObj.StatusMsg = "Refunded";

                        List<CreditWalletDrArryTrnID> CreditWalletDrArryTrnIDList = new List<CreditWalletDrArryTrnID>();
                        CreditWalletDrArryTrnIDList.Add(new CreditWalletDrArryTrnID { DrTrnRefNo = request.TrnNo, Amount = TransactionQueueObj.Amount });

                        var _TrnService = _serviceMasterRepository.GetSingle(item => item.SMSCode == TransactionQueueObj.SMSCode && item.Status == Convert.ToInt16(ServiceStatus.Active));
                        var ServiceType = (enServiceType)_TrnService.ServiceType;

                        WithdrawHistoryObj = _WithdrawHistory.GetSingle(i => i.TrnNo == request.TrnNo);
                        if (WithdrawHistoryObj != null)
                        {
                            WithdrawHistoryObj.Status = Convert.ToInt16(enTransactionStatus.Refunded);
                            WithdrawHistoryObj.SystemRemarks = "Refunded";
                            IsInsert = 0;
                        }

                        _WithdrawERCTokenQueueObj = _WithdrawERCTokenQueue.GetSingle(i => i.TrnNo == request.TrnNo);
                        if (_WithdrawERCTokenQueueObj != null)
                        {
                            _WithdrawERCTokenQueueObj.Status = Convert.ToInt16(enTransactionStatus.InActive);
                        }
                        //2019-11-8 added return msg /...
                       // Response.ErrorCode = enErrorCode.CurrentlystoppedWithdrawRecon;
                        //Response.ReturnCode = enResponseCode.Fail;
                        //Response.ReturnMsg = "Currently stopped Withdraw recon process.";
                        //return Response;
                        //2019-11-8 remove to call recon
                        var CreditResult = _walletTransactionCrDr.GetWalletCreditNewAsync(TransactionQueueObj.SMSCode, Helpers.GetTimeStamp(), enWalletTrnType.Withdrawal, TransactionQueueObj.Amount, TransactionQueueObj.MemberID,
                       TransactionQueueObj.DebitAccountID, CreditWalletDrArryTrnIDList.ToArray(), request.TrnNo, 1, enWalletTranxOrderType.Credit, ServiceType, (enTrnType)TransactionQueueObj.TrnType, "", TransactionQueueObj.GUID.ToString());

                        if (CreditResult.Result.ReturnCode != enResponseCode.Success)
                        {
                            Response.ErrorCode = enErrorCode.WithdrawalRecon_ProcessFail;
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ReturnMsg = EnResponseMessage.WithdrawalRecon_ProcessFail;
                            return Response;
                        }
                    }
                }
                else if (request.ActionType == enWithdrawalReconActionType.SuccessAndDebit) //Only For OperatorFail,SystemFail,Refund transaction are allowed
                {
                    if (!(TransactionQueueObj.Status == Convert.ToInt16(enTransactionStatus.OperatorFail) || TransactionQueueObj.Status == Convert.ToInt16(enTransactionStatus.SystemFail) || TransactionQueueObj.Status == Convert.ToInt16(enTransactionStatus.Refunded)))
                    {
                        Response.ErrorCode = enErrorCode.WithdrawalRecon_InvalidActionType;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.WithdrawalRecon_InvalidActionType;
                        return Response;
                    }
                    else
                    {
                        TransactionReconEntry(request.TrnNo, enTransactionStatus.Success, TransactionQueueObj.Status, TransactionQueueObj.SerProID, TransactionQueueObj.SerProID, request.ActionMessage, UserId);

                        TransactionQueueObj.Status = Convert.ToInt16(enTransactionStatus.Success);
                        TransactionQueueObj.StatusMsg = "Success";

                        var _TrnService = _serviceMasterRepository.GetSingle(item => item.SMSCode == TransactionQueueObj.SMSCode && item.Status == Convert.ToInt16(ServiceStatus.Active));
                        var ServiceType = (enServiceType)_TrnService.ServiceType;

                        var DebitResult = _WalletService.GetWalletDeductionNew(TransactionQueueObj.SMSCode, Helpers.GetTimeStamp(), enWalletTranxOrderType.Debit, TransactionQueueObj.Amount, TransactionQueueObj.MemberID,
                            TransactionQueueObj.DebitAccountID, request.TrnNo, ServiceType, enWalletTrnType.Withdrawal, (enTrnType)TransactionQueueObj.TrnType, accessToken);

                        if (DebitResult.Result.ReturnCode != enResponseCode.Success)
                        {
                            Response.ErrorCode = enErrorCode.WithdrawalRecon_ProcessFail;
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ReturnMsg = EnResponseMessage.WithdrawalRecon_ProcessFail;
                            return Response;
                        }
                    }
                }

                else if (request.ActionType == enWithdrawalReconActionType.Success) //Only For Hold transaction are allowed
                {
                    if (!(TransactionQueueObj.Status == Convert.ToInt16(enTransactionStatus.Hold)))
                    {
                        Response.ErrorCode = enErrorCode.WithdrawalRecon_InvalidActionType;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.WithdrawalRecon_InvalidActionType;
                        return Response;
                    }
                    else
                    {
                        TransactionReconEntry(request.TrnNo, enTransactionStatus.Success, TransactionQueueObj.Status, TransactionQueueObj.SerProID, TransactionQueueObj.SerProID, request.ActionMessage, UserId);

                        TransactionQueueObj.Status = Convert.ToInt16(enTransactionStatus.Success);
                        TransactionQueueObj.StatusMsg = "Success";

                        if (request.TrnID != null)
                        {
                            TransactionRequestobj = _TransactionRequest.GetSingle(i => i.TrnNo == request.TrnNo);
                            if (TransactionRequestobj != null)
                            {
                                if (TransactionRequestobj.TrnID == null || TransactionRequestobj.TrnID == "")
                                {
                                    TransactionRequestobj.TrnID = request.TrnID;
                                }
                            }
                            WithdrawHistoryObj = _WithdrawHistory.GetSingle(i => i.TrnNo == request.TrnNo);
                            if (WithdrawHistoryObj == null)
                            {
                                var routeObj = _RouteConfiguration.GetSingle(i => i.Id == TransactionQueueObj.RouteID);
                                if (routeObj != null)
                                {
                                    WithdrawHistoryObj = new WithdrawHistory();
                                    WithdrawHistoryObj.SMSCode = TransactionQueueObj.SMSCode;
                                    WithdrawHistoryObj.TrnID = request.TrnID;
                                    WithdrawHistoryObj.WalletId = _WalletService.GetWalletID(TransactionQueueObj.DebitAccountID).Result;
                                    WithdrawHistoryObj.Address = TransactionQueueObj.TransactionAccount;
                                    WithdrawHistoryObj.ToAddress = "";
                                    WithdrawHistoryObj.Confirmations = 0;
                                    WithdrawHistoryObj.Value = 0;
                                    WithdrawHistoryObj.Amount = TransactionQueueObj.Amount;
                                    WithdrawHistoryObj.Charge = 0;
                                    WithdrawHistoryObj.Status = 5;
                                    WithdrawHistoryObj.confirmedTime = "";
                                    WithdrawHistoryObj.unconfirmedTime = "";
                                    WithdrawHistoryObj.CreatedDate = Helpers.UTC_To_IST();
                                    WithdrawHistoryObj.State = 0;
                                    WithdrawHistoryObj.IsProcessing = 0;
                                    WithdrawHistoryObj.TrnNo = TransactionQueueObj.Id;
                                    WithdrawHistoryObj.RouteTag = routeObj.RouteName;
                                    WithdrawHistoryObj.UserId = TransactionQueueObj.MemberID;
                                    WithdrawHistoryObj.SerProID = TransactionQueueObj.SerProID;
                                    WithdrawHistoryObj.TrnDate = Helpers.UTC_To_IST();
                                    WithdrawHistoryObj.APITopUpRefNo = "";
                                    WithdrawHistoryObj.createdTime = Helpers.UTC_To_IST().ToString();
                                    WithdrawHistoryObj.SystemRemarks = "Recon Process Refunded";
                                    WithdrawHistoryObj.ProviderWalletID = routeObj.ProviderWalletID;
                                    WithdrawHistoryObj.GUID = Guid.NewGuid().ToString().Replace("-", "");//2019-7-13 added GUID in insert

                                    IsInsert = 1;
                                }
                            }
                            else
                            {
                                WithdrawHistoryObj.TrnID = request.TrnID;

                                IsInsert = 0;
                            }
                        }
                    }
                }

                else if (request.ActionType == enWithdrawalReconActionType.FailedMark) //Only For Hold,Success transaction are allowed
                {
                    if (!(TransactionQueueObj.Status == Convert.ToInt16(enTransactionStatus.Hold) || TransactionQueueObj.Status == Convert.ToInt16(enTransactionStatus.Success)))
                    {
                        Response.ErrorCode = enErrorCode.WithdrawalRecon_InvalidActionType;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.WithdrawalRecon_InvalidActionType;
                        return Response;
                    }
                    else
                    {
                        TransactionReconEntry(request.TrnNo, enTransactionStatus.Success, TransactionQueueObj.Status, TransactionQueueObj.SerProID, TransactionQueueObj.SerProID, request.ActionMessage, UserId);

                        TransactionQueueObj.Status = Convert.ToInt16(enTransactionStatus.OperatorFail);
                        TransactionQueueObj.StatusMsg = "OperatorFail";
                    }
                }

                var ResultBool = _backOfficeTrnRepository.WithdrawalRecon(tradeReconObj, TransactionQueueObj, WithdrawHistoryObj, _WithdrawERCTokenQueueObj, TransactionRequestobj, IsInsert);
                if (!ResultBool)
                {
                    Response.ErrorCode = enErrorCode.WithdrawalRecon_ProcessFail;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.WithdrawalRecon_ProcessFail;
                    return Response;
                }
                else
                {
                    Response.ErrorCode = enErrorCode.WithdrawalRecon_Success;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ReturnMsg = EnResponseMessage.WithdrawalRecon_Success;
                    return Response;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        
        public void TransactionReconEntry(long TrnNo, enTransactionStatus NewStatus, short OldStatus, long SerProID, long ServiceID, string Remarks, long UserID)
        {
            try
            {
                tradeReconObj = new TransactionRecon()
                {
                    TrnNo = TrnNo,
                    NewStatus = Convert.ToInt16(NewStatus),
                    OldStatus = OldStatus,
                    SerProID = SerProID,
                    ServiceID = ServiceID,
                    Remarks = Remarks,
                    CreatedBy = UserID,
                    CreatedDate = _basePage.UTC_To_IST(),
                    Status = 1,
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("WithdrwalreconEntry:##TrnNo " + TrnNo, "BackOfficeTrnService", ex);
            }
        }

        public async Task<BizResponseClass> ArbitrageTradeReconV1(enTradeReconActionType ActionType, long TranNo, string ActionMessage, long UserId, string accessToken)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                var transactionQueue = _transactionQueueArbitrageRepository.GetById(TranNo);
                if (transactionQueue != null)
                {
                    var datediff = _basePage.UTC_To_IST() - transactionQueue.TrnDate;
                    if (UserId != 1 && datediff.Days > 7)
                    {
                        //After 7 days of transaction you can not take action, Please contact admin
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.TradeRecon_After7DaysTranDontTakeAction;
                        Response.ErrorCode = enErrorCode.TradeRecon_After7DaysTranDontTakeAction;
                        return Response;
                    }
                    else
                    {
                        //Response = await _tradeReconProcessArbitrageV1.TradeReconProcessArbitrageAsyncV1(ActionType, TranNo, ActionMessage, UserId, accessToken);
                        return Response;
                    }
                }
                else
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionNo;
                    Response.ErrorCode = enErrorCode.TradeRecon_InvalidTransactionNo;
                    return Response;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Top Gainer and Looser Method

        public List<TopLooserGainerPairData> GetTopGainerPair(int Type, short IsMargin = 0)//Rita 5-3-19 for Margin Trading
        {
            try
            {
                List<TopLooserGainerPairData> Data;
                if (IsMargin == 1)
                    Data = _backOfficeTrnRepository.GetTopGainerPairMargin(Type);
                else
                    Data = _backOfficeTrnRepository.GetTopGainerPair(Type);

                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TopLooserGainerPairData> GetTopLooserPair(int Type, short IsMargin = 0)//Rita 5-3-19 for Margin Trading
        {
            try
            {
                List<TopLooserGainerPairData> Data;
                if (IsMargin == 1)
                    Data = _backOfficeTrnRepository.GetTopLooserPairMargin(Type);
                else
                    Data = _backOfficeTrnRepository.GetTopLooserPair(Type);

                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TopLooserGainerPairData> GetTopLooserGainerPair(short IsMargin = 0)//Rita 5-3-19 for Margin Trading
        {
            try
            {
                List<TopLooserGainerPairData> Data;

                if (IsMargin == 1)
                    Data = _backOfficeTrnRepository.GetTopLooserGainerPairMargin();
                else
                    Data = _backOfficeTrnRepository.GetTopLooserGainerPair();

                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Not in Use

        public Task<BizResponseClass> TradeRecon(long TranNo, string ActionMessage, long UserId, string accessToken)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                var transactionQueue = _transactionQueueRepository.GetById(TranNo);
                if (transactionQueue != null)
                {
                    var datediff = _basePage.UTC_To_IST() - transactionQueue.TrnDate;
                    if (UserId != 1 && datediff.Days > 7)
                    {
                        //After 7 days of transaction you can not take action, Please contact admin
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.TradeRecon_After7DaysTranDontTakeAction;
                        Response.ErrorCode = enErrorCode.TradeRecon_After7DaysTranDontTakeAction;
                        return Task.FromResult(Response);
                    }
                    else
                    {
                        var cancelOrderRequest = new NewCancelOrderRequestCls()
                        {
                            TranNo = TranNo,
                            accessToken = accessToken
                        };
                        var response = _cancelOrderProcess.ProcessCancelOrderAsyncV1(cancelOrderRequest);

                        Response.ReturnCode = (enResponseCode)enResponseCodeService.Parse(typeof(enResponseCodeService), response.Result.ReturnCode.ToString());
                        Response.ErrorCode = response.Result.ErrorCode;
                        Response.ReturnMsg = response.Result.ReturnMsg;

                        return Task.FromResult(Response);
                    }
                }
                else
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.TradeRecon_InvalidTransactionNo;
                    Response.ErrorCode = enErrorCode.TradeRecon_InvalidTransactionNo;
                    return Task.FromResult(Response);
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TransactionChargeResponse ChargeSummary(string FromDate, string ToDate, short trade)
        {
            try
            {
                TransactionChargeResponse _Res = new TransactionChargeResponse();
                var Modellist = _backOfficeTrnRepository.ChargeSummary(FromDate, ToDate, trade);
                if (Modellist.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    return _Res;
                }
                _Res.response = Modellist;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public WithdrawalSummaryResponse GetWithdrawalSummary(WithdrawalSummaryRequest Request)
        {
            try
            {
                WithdrawalSummaryResponse _Res = new WithdrawalSummaryResponse();

                if (Request.Status == 81)
                {
                    Request.Status = 1;
                }
                else if (Request.Status == 82)
                {
                    Request.Status = 2;
                }
                else if (Request.Status == 83)
                {
                    Request.Status = 3;
                }
                else if (Request.Status == 84)
                {
                    Request.Status = 4;
                }
                else if (Request.Status == 85)
                {
                    Request.Status = 5;
                }
                else if (Request.Status == 86)
                {
                    Request.Status = 6;
                }

                var Modellist = _backOfficeTrnRepository.GetWithdrawalSummary(Request);

                if (Modellist.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnMsg = "Fail";
                    return _Res;
                }
                else
                {
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.response = Modellist;
                    _Res.ReturnMsg = "Success";
                    return _Res;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public PairTradeSummaryResponse pairTradeSummary(long PairID, short Market, short Range, short IsMargin = 0)
        {
            try
            {
                PairTradeSummaryResponse _Res = new PairTradeSummaryResponse();
                List<PairTradeSummaryViewModel> Modellist = new List<PairTradeSummaryViewModel>();
                List<PairTradeSummaryQryResponse> list;
                if (IsMargin == 1)
                    list = _backOfficeTrnRepository.PairTradeSummaryMargin(PairID, Market, Range);
                else
                    list = _backOfficeTrnRepository.PairTradeSummary(PairID, Market, Range);

                if (list.Count == 0)
                {
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnMsg = "Fail";
                    return _Res;
                }
                foreach (var model in list)
                {
                    decimal CalcChargePer = 0;
                    try
                    {
                        CalcChargePer = ((model.LTP * 100) / model.OpenP) - 100;
                    }
                    catch (Exception e)
                    {
                        CalcChargePer = 0;
                    }
                    Modellist.Add(new PairTradeSummaryViewModel()
                    {
                        PairId = model.Id,
                        buy = model.buy,
                        Cancelled = model.Cancelled,
                        CloseP = model.LTP,
                        high = model.high,
                        low = model.low,
                        LTP = model.LTP,
                        OpenP = model.OpenP,
                        PairName = model.PairName,
                        sell = model.sell,
                        Settled = model.Settled,
                        TradeCount = model.TradeCount,
                        Volume = model.Volume,
                        ChargePer = CalcChargePer,
                        OrderType = Enum.GetName(typeof(enTransactionMarketType), model.ordertype)
                    });
                }
                _Res.Response = Modellist;
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

        #region Market Maker PErformance
        public MarketMakerBalancePerformanceResponse GetMarketMakerBalancePerformance()
        {
            MarketMakerBalancePerformanceResponse _Res = new MarketMakerBalancePerformanceResponse();
            try
            {
                var MarketMakerID = _backOfficeTrnRepository.GetMarketMakerUser();
                if (MarketMakerID == 0)
                {
                    _Res.Response = new List<MarketMakerBalancePerformanceViewModel>();
                    _Res.ErrorCode = enErrorCode.MarketMakerUserDoesNotExist;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Market Maker User Not Found";
                    return _Res;
                }
                _Res.Response = _backOfficeTrnRepository.GetMarketMakerBalancePerformance(MarketMakerID);
                if (_Res.Response.Count > 0)
                {
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "Success";
                }
                else
                {
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                }
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public MarketMakerTradePerformanceResponse MarketMakerTradePerformance(long PairID, string FromDate, string ToDate)
        {
            MarketMakerTradePerformanceResponse _Res = new MarketMakerTradePerformanceResponse();
            try
            {
                var MarketMakerID = _backOfficeTrnRepository.GetMarketMakerUser();
                if (MarketMakerID == 0)
                {
                    _Res.Response = new List<MarketMakerTradePerformance>();
                    _Res.ErrorCode = enErrorCode.MarketMakerUserDoesNotExist;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Market Maker User Not Found";
                    return _Res;
                }

                _Res.Response = _backOfficeTrnRepository.MarketMakerTradePerformance(MarketMakerID, PairID, FromDate, ToDate);
                if (_Res.Response.Count > 0)
                {
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "Success";
                }
                else
                {
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                }
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion
    }
}
