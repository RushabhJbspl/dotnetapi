using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Entities.NewWallet;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.MarginWallet;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletConfiguration;
using Worldex.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services
{
    public class MarginWalletService : IMarginWalletService 
    {
        #region COTR

        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IMarginWalletRepository _walletRepository1;
        private readonly ISignalRService _signalRService;
        private readonly IWalletService _walletService;
        private readonly ICommonRepository<WalletMaster> _commonRepository;
        private readonly ICommonRepository<MarginWalletMaster> _MarginWalletMaster;
        private readonly ICommonRepository<MarginLoanRequest> _MarginWalletTopupRequest;
        private readonly ICommonRepository<LeverageMaster> _LeverageMaster;
        private readonly ICommonRepository<WalletTypeMaster> _WalletTypeMaster;
        private readonly ICommonRepository<MarginWalletTypeMaster> _WalletTypeMasterRepository;
        private readonly ICommonRepository<UserLoanMaster> _UserLoanMaster;

        private readonly ICommonRepository<MarginCloseUserPositionWallet> _ClosePosition;
        private readonly IMarginSPRepositories _walletSPRepositories;

        public MarginWalletService(Microsoft.Extensions.Configuration.IConfiguration configuration, IMarginWalletRepository walletRepository1, 
            ICommonRepository<WalletMaster> commonRepository, IMarginSPRepositories walletSPRepositories, ICommonRepository<MarginWalletTypeMaster> WalletTypeMasterRepository,
            ICommonRepository<LeverageMaster> LeverageMaster, ICommonRepository<WalletTypeMaster> WalletTypeMaster, ICommonRepository<MarginLoanRequest> MarginWalletTopupRequest,
            IWalletService walletService, ICommonRepository<MarginWalletMaster> MarginWalletMaster, ISignalRService signalRService, ICommonRepository<UserLoanMaster> userLoanMaster, ICommonRepository<MarginCloseUserPositionWallet> ClosePosition)
        {
            _walletRepository1 = walletRepository1;
            _commonRepository = commonRepository;
            _walletSPRepositories = walletSPRepositories;
            _WalletTypeMasterRepository = WalletTypeMasterRepository;
            _LeverageMaster = LeverageMaster;
            _WalletTypeMaster = WalletTypeMaster;
            _MarginWalletTopupRequest = MarginWalletTopupRequest;
            _walletService = walletService;
            _MarginWalletMaster = MarginWalletMaster;
            _signalRService = signalRService;
            _configuration = configuration;
            _UserLoanMaster = userLoanMaster;
            _ClosePosition = ClosePosition;
        }

        #endregion

        #region Margin Trading

        public MarginPreConfirmationRes GetMarginPreConfirmationData(long WalletTypeId, decimal Amount, string AccWalletID, long UserId, short LeverageChargeDeductionType,decimal Leverage)
        {
            try
            {
                MarginPreConfirmationRes Res = new MarginPreConfirmationRes();
                //2019-2-18 added condi for only used trading wallet
                var wallet = _commonRepository.GetSingle(i => i.AccWalletID == AccWalletID && i.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (wallet == null)
                {
                    Res.ReturnCode = enResponseCode.Fail;
                    Res.ReturnMsg = EnResponseMessage.InvalidWallet;
                    Res.ErrorCode = enErrorCode.InvalidWallet;
                    return Res;
                }
                Res = _walletSPRepositories.CallSP_MarginFundTransferCalculation(WalletTypeId, Amount, UserId, wallet.Id, LeverageChargeDeductionType, Leverage); //ntrivedi levarage added is now user choice 

                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetMarginPreConfirmationData", "MarginWalletService", ex);
                throw ex;
            }
        }

        public BizResponseClass InsertMarginRequest(long WalletTypeId, decimal Amount, string AccWalletID, long UserId, short LeverageChargeDeductionType, decimal Leverage)
        {
            try
            {
                string Timestamp = Helpers.GetTimeStamp();
                BizResponseClass Res = new BizResponseClass();
                var wallet = _commonRepository.GetSingle(i => i.AccWalletID == AccWalletID && i.WalletUsageType == Convert.ToInt64(EnWalletUsageType.Trading_Wallet));
                if (wallet == null)
                {
                    Res.ReturnCode = enResponseCode.Fail;
                    Res.ReturnMsg = EnResponseMessage.InvalidWallet;
                    Res.ErrorCode = enErrorCode.InvalidWallet;
                    return Res;
                }
                long RequestId = 0;

                Res = _walletSPRepositories.CallSP_MarginProcess(WalletTypeId, Amount, UserId, wallet.Id, Timestamp, LeverageChargeDeductionType,ref RequestId,Leverage);
                var data = _MarginWalletTopupRequest.GetById(RequestId);
                
                if(data!=null)
                {
                    var wallettype = _WalletTypeMaster.GetById(WalletTypeId).WalletTypeName;
                    var leverage = _LeverageMaster.GetById(data.LeverageID);
                    var walletObj = _MarginWalletMaster.GetById(data.ToWalletID);
                    var debitwalletObj = _MarginWalletMaster.GetById(data.FromWalletID);
                    if (walletObj != null && debitwalletObj != null && leverage != null && wallettype != null)
                    {
                        if (Res.ErrorCode == enErrorCode.SP_MarginProcess_Pending)
                        {
                            //pending
                            _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_Leverage_Req_Pending, UserId.ToString(), Convert.ToDateTime(data.UpdatedDate).ToString("MM/dd/yyyy hh:mm tt"), data.Id.ToString());
                        }
                        else if (Res.ErrorCode == enErrorCode.SP_MarginProcess_Sucess)
                        {
                           
                            _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_Leverage_Req_DirectAccept, UserId.ToString(), wallettype.ToString(), data.Amount.ToString(), Helpers.DoRoundForTrading((leverage.LeveragePer), 2).ToString() + "X", Helpers.DoRoundForTrading(data.LeverageAmount, 8).ToString(), Helpers.DoRoundForTrading(data.ChargeAmount, 8).ToString(), Helpers.DoRoundForTrading(walletObj.Balance, 8).ToString()); //ntrivedi /100 removed 26-03-2019

                            _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_WalletDebited, UserId.ToString(), Helpers.DoRoundForTrading(data.CreditAmount, 8).ToString(), wallettype, Helpers.UTC_To_IST().ToString(), data.Id.ToString(), enMarginWalletTrnType.MarginWalletTransfer.ToString());

                            WalletMasterResponse walletMasterObjCr = new WalletMasterResponse();
                            walletMasterObjCr.AccWalletID = walletObj.AccWalletID;
                            walletMasterObjCr.Balance = walletObj.Balance;
                            walletMasterObjCr.WalletName = walletObj.Walletname;
                            walletMasterObjCr.PublicAddress = (walletObj.PublicAddress == null ? "N/A" : walletObj.PublicAddress);
                            walletMasterObjCr.IsDefaultWallet = walletObj.IsDefaultWallet;
                            walletMasterObjCr.CoinName = wallettype;
                            walletMasterObjCr.OutBoundBalance = walletObj.OutBoundBalance;

                            _signalRService.OnWalletBalChange(walletMasterObjCr, wallettype, walletObj.UserID.ToString(), 2, data.Id + " timestamp : " + Helpers.GetTimeStamp(),1);

                            WalletMasterResponse walletMasterObjDr = new WalletMasterResponse();
                            walletMasterObjDr.AccWalletID = debitwalletObj.AccWalletID;
                            walletMasterObjDr.Balance = debitwalletObj.Balance;
                            walletMasterObjDr.WalletName = debitwalletObj.Walletname;
                            walletMasterObjDr.PublicAddress = (debitwalletObj.PublicAddress == null ? "N/A" : debitwalletObj.PublicAddress);
                            walletMasterObjDr.IsDefaultWallet = debitwalletObj.IsDefaultWallet;
                            walletMasterObjDr.CoinName = wallettype;
                            walletMasterObjDr.OutBoundBalance = debitwalletObj.OutBoundBalance;

                            _signalRService.OnWalletBalChange(walletMasterObjDr, wallettype, debitwalletObj.UserID.ToString(), 2, data.Id + " timestamp : " + Helpers.GetTimeStamp(),1);
                        }
                    }
                }
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertMarginRequest", "MarginWalletService", ex);
                throw ex;
            }
        }

        public BizResponseClass AdminMarginChargeRequestApproval(short IsApproved, long ReuestId,string Remarks)
        {
            try
            {
                string Timestamp = Helpers.GetTimeStamp();
                BizResponseClass Res = new BizResponseClass();
                Res = _walletSPRepositories.CallSP_AdminMarginChargeRequestApproval(IsApproved, ReuestId, Timestamp, Remarks);
                var data = _MarginWalletTopupRequest.GetById(ReuestId);
                if (data != null)
                {
                    var wallettype = _WalletTypeMasterRepository.GetById(data.WalletTypeID).WalletTypeName; //ntrivedi 27-03-2019 taking marginwallettype
                    var leverage = _LeverageMaster.GetById(data.LeverageID);
                    var walletObj = _MarginWalletMaster.GetById(data.ToWalletID);
                    var debitwalletObj = _MarginWalletMaster.GetById(data.FromWalletID);

                    if (walletObj != null && debitwalletObj != null && leverage != null && wallettype != null)
                    {
                        if (Res.ErrorCode == enErrorCode.SP_AdminMarginChargeRequestApproval_Admin_approval_rejected)
                        {
                            //reject admin
                            _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_Leverage_Req_AdminReject, data.UserID.ToString(), wallettype.ToString(), Helpers.DoRoundForTrading(data.Amount, 8).ToString(), Helpers.DoRoundForTrading((leverage.LeveragePer), 2).ToString() + "X", Helpers.DoRoundForTrading(data.LeverageAmount, 8).ToString(), Helpers.DoRoundForTrading(data.ChargeAmount, 8).ToString(), Helpers.DoRoundForTrading(walletObj.Balance, 8).ToString()); //ntrivedi removed 26-03-2019
                        }
                        else if (Res.ErrorCode == enErrorCode.SP_AdminMarginChargeRequestApproval_Success)
                        {
                            //success admin
                            _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_Leverage_Req_AdminAccept, data.UserID.ToString(), wallettype.ToString(), Helpers.DoRoundForTrading(data.Amount, 8).ToString(), Helpers.DoRoundForTrading((leverage.LeveragePer), 2).ToString() + "X", Helpers.DoRoundForTrading(data.LeverageAmount, 8).ToString(), Helpers.DoRoundForTrading(data.ChargeAmount, 8).ToString(), Helpers.DoRoundForTrading(walletObj.Balance, 8).ToString()); //ntrivedi removed 26-03-2019

                            WalletMasterResponse walletMasterObjCr = new WalletMasterResponse();
                            walletMasterObjCr.AccWalletID = walletObj.AccWalletID;
                            walletMasterObjCr.Balance = walletObj.Balance;
                            walletMasterObjCr.WalletName = walletObj.Walletname;
                            walletMasterObjCr.PublicAddress = (walletObj.PublicAddress == null ? "N/A" : walletObj.PublicAddress);
                            walletMasterObjCr.IsDefaultWallet = walletObj.IsDefaultWallet;
                            walletMasterObjCr.CoinName = wallettype;
                            walletMasterObjCr.OutBoundBalance = walletObj.OutBoundBalance;

                            _signalRService.OnWalletBalChange(walletMasterObjCr, wallettype, walletObj.UserID.ToString(), 2, data.Id + " timestamp : " + Helpers.GetTimeStamp(),1);

                            WalletMasterResponse walletMasterObjDr = new WalletMasterResponse();
                            walletMasterObjDr.AccWalletID = debitwalletObj.AccWalletID;
                            walletMasterObjDr.Balance = debitwalletObj.Balance;
                            walletMasterObjDr.WalletName = debitwalletObj.Walletname;
                            walletMasterObjDr.PublicAddress = (debitwalletObj.PublicAddress == null ? "N/A" : debitwalletObj.PublicAddress);
                            walletMasterObjDr.IsDefaultWallet = debitwalletObj.IsDefaultWallet;
                            walletMasterObjDr.CoinName = wallettype;
                            walletMasterObjDr.OutBoundBalance = debitwalletObj.OutBoundBalance;

                            _signalRService.OnWalletBalChange(walletMasterObjDr, wallettype, debitwalletObj.UserID.ToString(), 2, data.Id + " timestamp : " + Helpers.GetTimeStamp(),1);
                        }
                    }
                    
                }
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AdminMarginChargeRequestApproval", "MarginWalletService", ex);
                throw ex;
            }
        }

        public BizResponseClass AdminMarginChargeRequestApprovalv2(short IsApproved, string RequestId, string Remarks)
        {
            try
            {
                string Timestamp = Helpers.GetTimeStamp();
                BizResponseClass Res = new BizResponseClass();
                Res = _walletSPRepositories.CallSP_AdminMarginChargeRequestApprovalv2(IsApproved, RequestId, Timestamp, Remarks);
                var data = _MarginWalletTopupRequest.GetSingle(i=>i.Guid== RequestId);
                if (data != null)
                {
                    var wallettype = _WalletTypeMasterRepository.GetById(data.WalletTypeID).WalletTypeName; //ntrivedi 27-03-2019 taking marginwallettype
                    var leverage = _LeverageMaster.GetById(data.LeverageID);
                    var walletObj = _MarginWalletMaster.GetById(data.ToWalletID);
                    var debitwalletObj = _MarginWalletMaster.GetById(data.FromWalletID);

                    if (walletObj != null && debitwalletObj != null && leverage != null && wallettype != null)
                    {
                        if (Res.ErrorCode == enErrorCode.SP_AdminMarginChargeRequestApproval_Admin_approval_rejected)
                        {
                            //reject admin
                            _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_Leverage_Req_AdminReject, data.UserID.ToString(), wallettype.ToString(), Helpers.DoRoundForTrading(data.Amount, 8).ToString(), Helpers.DoRoundForTrading((leverage.LeveragePer), 2).ToString() + "X", Helpers.DoRoundForTrading(data.LeverageAmount, 8).ToString(), Helpers.DoRoundForTrading(data.ChargeAmount, 8).ToString(), Helpers.DoRoundForTrading(walletObj.Balance, 8).ToString()); //ntrivedi removed 26-03-2019
                        }
                        else if (Res.ErrorCode == enErrorCode.SP_AdminMarginChargeRequestApproval_Success)
                        {
                            //success admin
                            _walletService.EmailSendAsyncV1(EnTemplateType.EMAIL_Leverage_Req_AdminAccept, data.UserID.ToString(), wallettype.ToString(), Helpers.DoRoundForTrading(data.Amount, 8).ToString(), Helpers.DoRoundForTrading((leverage.LeveragePer), 2).ToString() + "X", Helpers.DoRoundForTrading(data.LeverageAmount, 8).ToString(), Helpers.DoRoundForTrading(data.ChargeAmount, 8).ToString(), Helpers.DoRoundForTrading(walletObj.Balance, 8).ToString()); //ntrivedi removed 26-03-2019

                            WalletMasterResponse walletMasterObjCr = new WalletMasterResponse();
                            walletMasterObjCr.AccWalletID = walletObj.AccWalletID;
                            walletMasterObjCr.Balance = walletObj.Balance;
                            walletMasterObjCr.WalletName = walletObj.Walletname;
                            walletMasterObjCr.PublicAddress = (walletObj.PublicAddress == null ? "N/A" : walletObj.PublicAddress);
                            walletMasterObjCr.IsDefaultWallet = walletObj.IsDefaultWallet;
                            walletMasterObjCr.CoinName = wallettype;
                            walletMasterObjCr.OutBoundBalance = walletObj.OutBoundBalance;

                            _signalRService.OnWalletBalChange(walletMasterObjCr, wallettype, walletObj.UserID.ToString(), 2, data.Id + " timestamp : " + Helpers.GetTimeStamp(), 1);

                            WalletMasterResponse walletMasterObjDr = new WalletMasterResponse();
                            walletMasterObjDr.AccWalletID = debitwalletObj.AccWalletID;
                            walletMasterObjDr.Balance = debitwalletObj.Balance;
                            walletMasterObjDr.WalletName = debitwalletObj.Walletname;
                            walletMasterObjDr.PublicAddress = (debitwalletObj.PublicAddress == null ? "N/A" : debitwalletObj.PublicAddress);
                            walletMasterObjDr.IsDefaultWallet = debitwalletObj.IsDefaultWallet;
                            walletMasterObjDr.CoinName = wallettype;
                            walletMasterObjDr.OutBoundBalance = debitwalletObj.OutBoundBalance;

                            _signalRService.OnWalletBalChange(walletMasterObjDr, wallettype, debitwalletObj.UserID.ToString(), 2, data.Id + " timestamp : " + Helpers.GetTimeStamp(), 1);
                        }
                    }

                }
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AdminMarginChargeRequestApproval", "MarginWalletService", ex);
                throw ex;
            }
        }
        #endregion

        #region Margin Create Wallet

        public BizResponseClass CreateMarginWallet(long WalletTypeId, long UserId)
        {
            try
            {
                var res = _walletSPRepositories.CreateMarginWallet(WalletTypeId, UserId);
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateMarginWallet", "MarginWalletService", ex);
                throw ex;
            }
        }

        public BizResponseClass CreateAllMarginWallet(long UserId)
        {
            try
            {
                var res = _walletSPRepositories.CreateAllMarginWallet( UserId);
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateAllMarginWallet", "MarginWalletService", ex);
                throw ex;
            }
        }

        public async Task<ListMarginWallet> ListMarginWalletMaster(long? WalletTypeId, EnWalletUsageType? WalletUsageType, short? Status, string AccWalletId, long? UserId)
        {
            ListMarginWallet Resp = new ListMarginWallet();
            try
            {
                int TotalCount = 0;
                var data = _walletRepository1.ListMarginWalletMaster(WalletTypeId, WalletUsageType, Status, AccWalletId, UserId);
                Resp.Data = data;
                if (data.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListMarginWallet2> ListMarginWallet(int PageNo, int PageSize,long? WalletTypeId, EnWalletUsageType? WalletUsageType, short? Status, string AccWalletId, long? UserId)
        {
            ListMarginWallet2 Resp = new ListMarginWallet2();
            try
            {
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                int TotalCount = 0;
                var data = _walletRepository1.ListMarginWallet(PageNo+1,PageSize,WalletTypeId, WalletUsageType, Status, AccWalletId, UserId,ref TotalCount);
                Resp.Data = data;
                Resp.TotalCount = TotalCount;
                if (data.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListMarginWalletByUserIdRes> GetMarginWalletByUserId(long UserId)
        {
            ListMarginWalletByUserIdRes Resp = new ListMarginWalletByUserIdRes();
            try
            {
                var data = _walletRepository1.GetMarginWalletByUserId(UserId);
                Resp.Data = data;
                if (data.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }
        #endregion

        #region WalletType Master

        public ListMarginWalletTypeMasterResponse ListAllWalletTypeMaster()
        {
            ListMarginWalletTypeMasterResponse listWalletTypeMasterResponse = new ListMarginWalletTypeMasterResponse();
            try
            {
                IEnumerable<MarginWalletTypeMaster> coin = new List<MarginWalletTypeMaster>();
                coin = _WalletTypeMasterRepository.FindBy(item => item.Status != Convert.ToInt16(ServiceStatus.Disable));
                if (coin == null)
                {
                    listWalletTypeMasterResponse.ReturnCode = enResponseCode.Fail;
                    listWalletTypeMasterResponse.ReturnMsg = EnResponseMessage.NotFound;
                    listWalletTypeMasterResponse.ErrorCode = enErrorCode.RecordNotFound;

                }
                else
                {
                    listWalletTypeMasterResponse.walletTypeMasters = coin;
                    listWalletTypeMasterResponse.ReturnCode = enResponseCode.Success;
                    listWalletTypeMasterResponse.ErrorCode = enErrorCode.Success;
                    listWalletTypeMasterResponse.ReturnMsg = EnResponseMessage.FindRecored;
                }

                return listWalletTypeMasterResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                listWalletTypeMasterResponse.ReturnCode = enResponseCode.InternalError;
                return listWalletTypeMasterResponse;
            }
        }
        #endregion

        #region Leverage Report

        public ListLeaverageRes LeverageRequestReport(long? WalletTypeId, long UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize, short? Status)
        {
            try
            {
                ListLeaverageRes Resp = new ListLeaverageRes();
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                int TotalCount = 0;
                var data = _walletRepository1.LeverageRequestReport(WalletTypeId, UserId,FromDate, ToDate, PageNo + 1, PageSize, Status, ref TotalCount);
                Resp.TotalCount = TotalCount;
                if (data.Count != 0)
                {
                    Resp.Data = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LeaverageReport", this.GetType().Name, ex);
                throw;
            }
        }

        public ListLeaverageResv2 LeverageRequestReportv2(long? WalletTypeId, long UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize, short? Status)
        {
            try
            {
                ListLeaverageResv2 Resp = new ListLeaverageResv2();
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                int TotalCount = 0;
                var data = _walletRepository1.LeverageRequestReportv2(WalletTypeId, UserId, FromDate, ToDate, PageNo + 1, PageSize, Status, ref TotalCount);
                Resp.TotalCount = TotalCount;
                if (data.Count != 0)
                {
                    Resp.Data = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LeaverageReport", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region Leaverage Report

        public ListLeaverageReportRes LeveragePendingReport(long? WalletTypeId, long? UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize)
        {
            try
            {
                ListLeaverageReportRes Resp = new ListLeaverageReportRes();
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                int TotalCount = 0;
                var data = _walletRepository1.LeveragePendingReport(WalletTypeId, UserId, FromDate, ToDate, PageNo + 1, PageSize, ref TotalCount);
                Resp.TotalCount = TotalCount;
                if (data.Count != 0)
                {
                    Resp.Data = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LeveragePendingReport", this.GetType().Name, ex);
                throw;
            }
        }

        public ListLeaverageReportResv2 LeveragePendingReportv2(long? WalletTypeId, long? UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize)
        {
            try
            {
                ListLeaverageReportResv2 Resp = new ListLeaverageReportResv2();
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                int TotalCount = 0;
                var data = _walletRepository1.LeveragePendingReportv2(WalletTypeId, UserId, FromDate, ToDate, PageNo + 1, PageSize, ref TotalCount);
                Resp.TotalCount = TotalCount;
                if (data.Count != 0)
                {
                    Resp.Data = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LeveragePendingReport", this.GetType().Name, ex);
                throw;
            }
        }

        public ListLeaverageRes LeverageReport(long? WalletTypeId, long? UserId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, short? Status)
        {
            try
            {
                ListLeaverageRes Resp = new ListLeaverageRes();
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                int TotalCount = 0;
                FromDate = FromDate.AddHours(00).AddMinutes(00).AddSeconds(00);
                ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                var data = _walletRepository1.LeverageReport(WalletTypeId, UserId, FromDate, ToDate, PageNo + 1, PageSize, Status, ref TotalCount);
                Resp.TotalCount = TotalCount;
                if (data.Count != 0)
                {
                    Resp.Data = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LeaverageReport", this.GetType().Name, ex);
                throw;
            }
        }

        public ListLeaverageResv2 LeverageReportv2(long? WalletTypeId, long? UserId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, short? Status)
        {
            try
            {
                ListLeaverageResv2 Resp = new ListLeaverageResv2();
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                int TotalCount = 0;
                FromDate = FromDate.AddHours(00).AddMinutes(00).AddSeconds(00);
                ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                var data = _walletRepository1.LeverageReportv2(WalletTypeId, UserId, FromDate, ToDate, PageNo + 1, PageSize, Status, ref TotalCount);
                Resp.TotalCount = TotalCount;
                if (data.Count != 0)
                {
                    Resp.Data = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LeaverageReport", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region LevarageMaster

        //2018-12-28
        public async Task<ListLeverageRes> ListLeverage(long? WalletTypeId, short? Status)
        {
            ListLeverageRes Resp = new ListLeverageRes();
            try
            {
                var obj = await _walletRepository1.ListLeverage(WalletTypeId, Status);
                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListLeverageRes", this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-28
        public async Task<BizResponseClass> InserUpdateLeverage(InserUpdateLeverageReq Request, long UserId)
        {
            try
            {             
                //ntrivedi 27-03-2019
                if (Request.LeveragePer > Convert.ToInt64(_configuration["MaxLeverageX"]))
                {
                    string ReturnMessage = EnResponseMessage.LevrageValueExceeded.Replace("##Lev##", Convert.ToInt64(_configuration["MaxLeverageX"]).ToString());
                    return new BizResponseClass { ErrorCode = enErrorCode.LeverageValueExceeded, ReturnCode = enResponseCode.Fail, ReturnMsg = ReturnMessage };
                }

                BizResponseClass Resp = new BizResponseClass();
                var IsExist = _LeverageMaster.GetSingle(item => item.Id == Request.Id);

                var levrageObj = _LeverageMaster.GetSingle(i => i.WalletTypeID == Request.WalletTypeId);

                var walletTypeObj = _WalletTypeMasterRepository.GetSingle(i => i.Id == Request.WalletTypeId);

                if (walletTypeObj == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidWalletType, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWalletType };
                }
                if (Request.Id == 0)
                {
                    if (levrageObj == null)
                    {
                        //insert
                        LeverageMaster Obj = new LeverageMaster();
                        Obj.WalletTypeID = Request.WalletTypeId;
                        Obj.LeveragePer = Request.LeveragePer;
                        Obj.Status = Request.Status;
                        Obj.CreatedBy = UserId;
                        Obj.CreatedDate = Helpers.UTC_To_IST();
                        Obj.UpdatedDate = Helpers.UTC_To_IST();
                        Obj.SafetyMarginPer = Request.SafetyMarginPer;
                        Obj.MarginChargePer = Request.MarginChargePer;
                        Obj.IsAutoApprove = Request.IsAutoApprove;
                        Obj.LeverageChargeDeductionType = Request.LeverageChargeDeductionType;
                        Obj.InstantChargePer = Request.InstantChargePer; //ntrivedi 27-03-2019
                        _LeverageMaster.Add(Obj);
                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                    }
                    else if (levrageObj.Status == 9)
                    {
                        //update
                        levrageObj.Status = Request.Status;
                        levrageObj.UpdatedBy = UserId;
                        levrageObj.UpdatedDate = Helpers.UTC_To_IST();
                        levrageObj.LeveragePer = Request.LeveragePer;
                        levrageObj.SafetyMarginPer = Request.SafetyMarginPer;
                        levrageObj.MarginChargePer = Request.MarginChargePer;
                        levrageObj.IsAutoApprove = Request.IsAutoApprove;
                        levrageObj.LeverageChargeDeductionType = Request.LeverageChargeDeductionType;
                        levrageObj.InstantChargePer = Request.InstantChargePer; //ntrivedi 27-03-2019
                        _LeverageMaster.UpdateWithAuditLog(levrageObj);
                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                    }
                    else
                    {
                        //exist
                        return new BizResponseClass { ErrorCode = enErrorCode.Alredy_Exist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Alredy_Exist };
                    }
                }
                else
                {
                    if (IsExist == null)
                    {
                        return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                    }
                    //update
                    IsExist.Status = Request.Status;
                    IsExist.LeveragePer = Request.LeveragePer;
                    IsExist.UpdatedBy = UserId;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    IsExist.SafetyMarginPer = Request.SafetyMarginPer;
                    IsExist.MarginChargePer = Request.MarginChargePer;
                    IsExist.IsAutoApprove = Request.IsAutoApprove;
                    IsExist.LeverageChargeDeductionType = Request.LeverageChargeDeductionType;
                    IsExist.InstantChargePer = Request.InstantChargePer;//ntrivedi 27-03-2019
                    _LeverageMaster.UpdateWithAuditLog(IsExist);
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InserUpdateLeverage", this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-28
        public async Task<BizResponseClass> ChangeLeverageStatus(short Status, long UserId, long Id)
        {
            try
            {
                var ISExist = _LeverageMaster.GetSingle(i => i.Id == Id);
                if (ISExist != null)
                {
                    //update
                    ISExist.Status = Status;
                    ISExist.UpdatedBy = UserId;
                    ISExist.UpdatedDate = Helpers.UTC_To_IST();
                    _LeverageMaster.UpdateWithAuditLog(ISExist);
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordDeleted };
                }
                return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ChangeLeverageStatus", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListLeverageRes> ListLeverageBaseCurrency(long? WalletTypeId, short? Status)
        {
            ListLeverageRes Resp = new ListLeverageRes();
            try
            {
                var obj = await _walletRepository1.ListLeverageBaseCurrency(WalletTypeId, Status);
                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListLeverageRes", this.GetType().Name, ex);
                throw;
            }
        }
        #endregion

        #region Charge

        public ListChargesTypeWise ListMarginChargesTypeWise(string WalletTypeName, long? TrnTypeId)
        {
            var typeObj = _WalletTypeMasterRepository.GetSingle(i => i.WalletTypeName == WalletTypeName);
            ListChargesTypeWise Resp = new ListChargesTypeWise();
            List<ChargeWalletType> walletTypes = new List<ChargeWalletType>();
            try
            {
                long? Id = null;
                if (typeObj != null)
                {
                    Id = typeObj.Id;
                }
                var res = _walletRepository1.GetMarginChargeWalletType(Id);
                for (int i = 0; i <= res.Count - 1; i++)
                {
                    ChargeWalletType a = new ChargeWalletType();
                    a.WalletTypeName = res[i].WalletTypeName;
                    a.WalletTypeId = res[i].WalletTypeId;
                    a.Charges = new List<ChargesTypeWise>();
                    var data = _walletRepository1.ListMarginChargesTypeWise(res[i].WalletTypeId, TrnTypeId);
                    a.Charges = data;
                    walletTypes.Add(a);
                }
                if (walletTypes.Count == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.Data = walletTypes;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public ListTrnChargeLogRes MarginTrnChargeLogReport(int PageNo, int PageSize, short? Status, long? TrnTypeID, long? WalleTypeId, short? SlabType, DateTime? FromDate, DateTime? ToDate, long? TrnNo)
        {
            try
            {
                ListTrnChargeLogRes Resp = new ListTrnChargeLogRes();
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                long TotalCount = 0;
                var data = _walletRepository1.MarginTrnChargeLogReport(PageNo + 1, PageSize, Status, TrnTypeID, WalleTypeId, SlabType, FromDate, ToDate, TrnNo, ref TotalCount);
                Resp.TotalCount = TotalCount;
                if (data.Count == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.Data = data;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region Margin Wallet Ledger

        public ListWalletLedgerResv1 GetMarginWalletLedger(DateTime FromDate, DateTime ToDate, string WalletId, int page, int PageSize)
        {
            try
            {
                var wallet = _MarginWalletMaster.GetSingle(item => item.AccWalletID == WalletId);

                ListWalletLedgerResv1 Response = new ListWalletLedgerResv1();
                Response.PageNo = page;
                Response.PageSize = PageSize;
                if (wallet == null)
                {
                    Response.ErrorCode = enErrorCode.InvalidWallet;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return Response;
                }
                DateTime newToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                int TotalCount = 0;
                var wl = _walletRepository1.GetMarginWalletLedger(FromDate, newToDate, wallet.Id, page + 1, PageSize, ref TotalCount);
                Response.TotalCount = TotalCount;
                if (wl.Count == 0)
                {
                    Response.ErrorCode = enErrorCode.NotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.WalletLedgers = wl;
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = EnResponseMessage.FindRecored;
                Response.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ListWalletLedgerResponse GetMarginWalletLedgerv2(DateTime FromDate, DateTime ToDate, string WalletId, int page, int PageSize)
        {
            try
            {
                var wallet = _MarginWalletMaster.GetSingle(item => item.AccWalletID == WalletId);

                ListWalletLedgerResponse Response = new ListWalletLedgerResponse();
                Response.PageNo = page;
                Response.PageSize = PageSize;
                if (wallet == null)
                {
                    Response.ErrorCode = enErrorCode.InvalidWallet;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.InvalidWallet;
                    return Response;
                }
                DateTime newToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                int TotalCount = 0;
                //var wl = _walletRepository1.GetMarginWalletLedger(FromDate, newToDate, wallet.Id, page + 1, PageSize, ref TotalCount);
                var wl = _walletSPRepositories.CallGetMarginWalletLedger(FromDate, newToDate, wallet.Id, page + 1, PageSize, ref TotalCount);
                Response.TotalCount = TotalCount;
                if (wl.Count == 0)
                {
                    Response.ErrorCode = enErrorCode.NotFound;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.NotFound;
                    return Response;
                }
                Response.WalletLedgers = wl;
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = EnResponseMessage.FindRecored;
                Response.ErrorCode = enErrorCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region GetPairLeverageDetail
        public PairLeverageDetailRes GetPairLeverageDetail(string FirstCurr,string SecondCurrency,long userID)
        {
            MarginWalletTypeMaster walletMasterFirstCurr, walletMasterSecondCurrency;
            MarginWalletMaster userMarginWalletMasterFirstCurr, userMarginWalletMasterSecondCurr;
            PairLeverageDetailRes leverageDetailRes;
            LeveragePairDetail leveragePairDetailFirstCurr, LeveragePairDetailSecondCurr;
            PairLeverageDetail PairLeverageDetailFirstCurr, PairLeverageDetailSecondCurr;

            try
            {
                leverageDetailRes = new PairLeverageDetailRes();
                PairLeverageDetailFirstCurr = new PairLeverageDetail();
                PairLeverageDetailSecondCurr = new PairLeverageDetail();

                walletMasterFirstCurr = _WalletTypeMasterRepository.GetSingle(e => e.WalletTypeName == FirstCurr && e.Status == 1);
                if(walletMasterFirstCurr == null)
                {
                    PairLeverageDetailFirstCurr = null;
                }
                else
                {
                    userMarginWalletMasterFirstCurr = _MarginWalletMaster.GetSingle(e => e.UserID == userID && e.WalletTypeID == walletMasterFirstCurr.Id && e.Status == 1 && e.WalletUsageType == EnWalletUsageType.Margin_Trading_Wallet);
                    if (userMarginWalletMasterFirstCurr == null)
                    {
                        PairLeverageDetailFirstCurr.CurrentBalance = 0;
                        PairLeverageDetailFirstCurr.LastLeverageAmount = 0;
                        PairLeverageDetailFirstCurr.LastLeverageTime = null;
                        PairLeverageDetailFirstCurr.Leverage = 0;
                        PairLeverageDetailFirstCurr.IsLeverageTaken = 0;
                        PairLeverageDetailFirstCurr.LeverageCharge = 0;
                    }
                    else
                    {
                        leveragePairDetailFirstCurr = _walletRepository1.GetPairLeverageDetail(userMarginWalletMasterFirstCurr.Id);
                        PairLeverageDetailFirstCurr = new PairLeverageDetail();
                        PairLeverageDetailFirstCurr.CurrentBalance = userMarginWalletMasterFirstCurr.Balance;
                        PairLeverageDetailFirstCurr.LastLeverageAmount = leveragePairDetailFirstCurr.Amount;
                        PairLeverageDetailFirstCurr.LastLeverageTime = leveragePairDetailFirstCurr.ApprovedDate;
                        PairLeverageDetailFirstCurr.Leverage = leveragePairDetailFirstCurr.Leverage;
                        PairLeverageDetailFirstCurr.IsLeverageTaken = leveragePairDetailFirstCurr.IsLeverageTaken;
                        PairLeverageDetailFirstCurr.LeverageCharge = leveragePairDetailFirstCurr.LeverageCharge;
                    }
                }
                walletMasterSecondCurrency = _WalletTypeMasterRepository.GetSingle(e => e.WalletTypeName == SecondCurrency && e.Status == 1);
                if (walletMasterSecondCurrency == null)
                {
                    PairLeverageDetailSecondCurr = null;
                }
                else
                {
                    userMarginWalletMasterSecondCurr = _MarginWalletMaster.GetSingle(e => e.UserID == userID && e.WalletTypeID == walletMasterSecondCurrency.Id && e.Status == 1 && e.WalletUsageType == EnWalletUsageType.Margin_Trading_Wallet);
                    if (userMarginWalletMasterSecondCurr == null)
                    {
                        PairLeverageDetailSecondCurr.CurrentBalance = 0;
                        PairLeverageDetailSecondCurr.LastLeverageAmount = 0;
                        PairLeverageDetailSecondCurr.LastLeverageTime = null;
                        PairLeverageDetailSecondCurr.Leverage = 0;
                        PairLeverageDetailSecondCurr.IsLeverageTaken = 0;
                        PairLeverageDetailSecondCurr.LeverageCharge = 0;
                    }
                    else
                    {
                        LeveragePairDetailSecondCurr = _walletRepository1.GetPairLeverageDetail(userMarginWalletMasterSecondCurr.Id);
                        PairLeverageDetailSecondCurr.CurrentBalance = userMarginWalletMasterSecondCurr.Balance;
                        PairLeverageDetailSecondCurr.LastLeverageAmount = LeveragePairDetailSecondCurr.Amount;
                        PairLeverageDetailSecondCurr.LastLeverageTime = LeveragePairDetailSecondCurr.ApprovedDate;
                        PairLeverageDetailSecondCurr.Leverage = LeveragePairDetailSecondCurr.Leverage;
                        PairLeverageDetailSecondCurr.IsLeverageTaken = LeveragePairDetailSecondCurr.IsLeverageTaken;
                        PairLeverageDetailSecondCurr.LeverageCharge = LeveragePairDetailSecondCurr.LeverageCharge;
                    }
                }
                leverageDetailRes.FirstCurrency = PairLeverageDetailFirstCurr;
                leverageDetailRes.SecondCurrency = PairLeverageDetailSecondCurr;
                leverageDetailRes.ReturnCode = enResponseCode.Success;
                leverageDetailRes.ReturnMsg = EnResponseMessage.MarginLeverageDetailFoundSuccess;
                leverageDetailRes.ErrorCode = enErrorCode.MarginLeverageDetailFoundSuccess;
                return leverageDetailRes;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        public async Task<StopLimitOrderPrice> ReCalculateInternalOrderPrice(long UserID, long PairID, string baseCurrency)
        {
            StopLimitOrderPrice stopLimitOrderPriceObj = new StopLimitOrderPrice();
            BizResponseClass BizResponseClassObj = new BizResponseClass();
            decimal dailyCharge = 0, dailyChargePer = 0;
            try
            {
                //Every Settlement find the Internal Order BidPrice

                // Average Qty Average BidPrice Average Landing
                //sum(buy Qty) - sum(sell Qty)      sum(buy bidprice) - sum(sell bidprice)            sum(buy Landing) - sum(sell Landing)

                //    100                     5                                               500

                //SAfety balance = 10 - today charge balance = 2 => 10 - 2 = 8 safety amount
                //500 - 8 = 492 / 100 Qty = 4.92  price e settle thay to aapan ne 8 to loss jay
                stopLimitOrderPriceObj.PairID = PairID;
                stopLimitOrderPriceObj.UserID = UserID;
                stopLimitOrderPriceObj.baseCurrency = baseCurrency;

                OpenPositionMaster openPositionMasterObj = _walletRepository1.GetPositionMasterValue(PairID, UserID);
                if (openPositionMasterObj == null)
                {
                    BizResponseClassObj.ReturnCode = enResponseCode.Fail;
                    BizResponseClassObj.ReturnMsg = EnResponseMessage.OpenPositionNotFound;
                    BizResponseClassObj.ErrorCode = enErrorCode.OpenPositionNotFound;
                    stopLimitOrderPriceObj.BizResponseClass = BizResponseClassObj;
                    HelperForLog.WriteLogIntoFileAsync("ReCalculateInternalOrderPrice", Helpers.JsonSerialize(stopLimitOrderPriceObj));
                    return stopLimitOrderPriceObj;
                }
                PositionValue posValueDetailObj = _walletRepository1.GetPositionDetailValue(openPositionMasterObj.Id);
                stopLimitOrderPriceObj.AvgBidPrice = posValueDetailObj.BidPrice;
                stopLimitOrderPriceObj.AvgLanding = posValueDetailObj.LandingPrice;
                stopLimitOrderPriceObj.AvgQty = posValueDetailObj.Qty;

                MarginWalletTypeMaster marginWalletTypeObj = _WalletTypeMasterRepository.GetSingle(x => x.WalletTypeName == baseCurrency && x.Status == 1);
                if (marginWalletTypeObj == null)
                {
                    BizResponseClassObj.ReturnCode = enResponseCode.Fail;
                    BizResponseClassObj.ReturnMsg = EnResponseMessage.InvalidBaseCurrency;
                    BizResponseClassObj.ErrorCode = enErrorCode.InvalidBaseCurrency;
                    stopLimitOrderPriceObj.BizResponseClass = BizResponseClassObj;
                    HelperForLog.WriteLogIntoFileAsync("ReCalculateInternalOrderPrice", Helpers.JsonSerialize(stopLimitOrderPriceObj));
                    return stopLimitOrderPriceObj;
                }
                Core.Entities.NewWallet.LeverageMaster LeverageMasterObj = _LeverageMaster.GetSingle(x => x.WalletTypeID == marginWalletTypeObj.Id);
                if (LeverageMasterObj == null)
                {
                    BizResponseClassObj.ReturnCode = enResponseCode.Fail;
                    BizResponseClassObj.ReturnMsg = EnResponseMessage.InvalidBaseCurrency;
                    BizResponseClassObj.ErrorCode = enErrorCode.InvalidBaseCurrency;
                    stopLimitOrderPriceObj.BizResponseClass = BizResponseClassObj;
                    HelperForLog.WriteLogIntoFileAsync("ReCalculateInternalOrderPrice", Helpers.JsonSerialize(stopLimitOrderPriceObj));

                    return stopLimitOrderPriceObj;
                }
                if (LeverageMasterObj.LeverageChargeDeductionType == enLeverageChargeDeductionType.Monthly)
                {
                    dailyChargePer = Helpers.DoRoundForTrading(LeverageMasterObj.MarginChargePer / 30, 18);
                }
                else
                {
                    dailyChargePer = LeverageMasterObj.MarginChargePer;
                }
                MarginWalletMaster marginWalletMasterObj = _MarginWalletMaster.GetSingle(x => x.Status == 1 && x.WalletUsageType == EnWalletUsageType.Margin_Safety_Wallet && x.UserID == UserID && x.WalletTypeID == marginWalletTypeObj.Id);
                if (marginWalletMasterObj == null)
                {
                    BizResponseClassObj.ReturnCode = enResponseCode.Fail;
                    BizResponseClassObj.ReturnMsg = EnResponseMessage.SafetyWalletNotFound;
                    BizResponseClassObj.ErrorCode = enErrorCode.SafetyWalletNotFound;
                    stopLimitOrderPriceObj.BizResponseClass = BizResponseClassObj;
                    HelperForLog.WriteLogIntoFileAsync("ReCalculateInternalOrderPrice", Helpers.JsonSerialize(stopLimitOrderPriceObj));

                    return stopLimitOrderPriceObj;
                }
                UserLoanMaster userLoanMaster = _UserLoanMaster.GetById(openPositionMasterObj.LoanID);
                if (userLoanMaster == null)
                {
                    BizResponseClassObj.ReturnCode = enResponseCode.Fail;
                    BizResponseClassObj.ReturnMsg = EnResponseMessage.LoanNotFound;
                    BizResponseClassObj.ErrorCode = enErrorCode.LoanNotFound;
                    stopLimitOrderPriceObj.BizResponseClass = BizResponseClassObj;
                    HelperForLog.WriteLogIntoFileAsync("ReCalculateInternalOrderPrice", Helpers.JsonSerialize(stopLimitOrderPriceObj));

                    return stopLimitOrderPriceObj;
                }
               
                stopLimitOrderPriceObj.ProfitBalance = userLoanMaster.ProfitAmount;//ntrivedi 17-04-2019 added 
                stopLimitOrderPriceObj.SafetyBalance = marginWalletMasterObj.Balance;
                stopLimitOrderPriceObj.Charge = Helpers.DoRoundForTrading(Helpers.DoRoundForTrading(userLoanMaster.LeverageAmount * dailyChargePer, 18) / 100, 18); //ntrivedi 09-05-2019 instead of balance fetch from loan amount 09-05-2019 
                stopLimitOrderPriceObj.SafetyBalanceAferCharge = stopLimitOrderPriceObj.SafetyBalance - stopLimitOrderPriceObj.Charge;
                stopLimitOrderPriceObj.MinLanding = stopLimitOrderPriceObj.AvgLanding - stopLimitOrderPriceObj.SafetyBalanceAferCharge - stopLimitOrderPriceObj.ProfitBalance; //ntrivedi profit added 17-04-2019  profit minus from the safety 
                stopLimitOrderPriceObj.FinalBidPrice = Helpers.DoRoundForTrading(stopLimitOrderPriceObj.MinLanding / stopLimitOrderPriceObj.AvgQty, 18);

                if (posValueDetailObj.SellQty > 0) // divided by 0 issue fixing 07-05-2019
                {
                    stopLimitOrderPriceObj.BuyProfitLanding = Helpers.DoRoundForTrading((posValueDetailObj.BuyQty * posValueDetailObj.BuyLandingPrice) / posValueDetailObj.SellQty, 18); //if open position loss more than safety wallet
                }
                else
                {
                    stopLimitOrderPriceObj.BuyProfitLanding = 0;
                }
               

                BizResponseClassObj.ReturnCode = enResponseCode.Success;
                BizResponseClassObj.ReturnMsg = EnResponseMessage.PriceCalculationisSuccess;
                BizResponseClassObj.ErrorCode = enErrorCode.MarginPriceCalculationisSuccess;
                if ((stopLimitOrderPriceObj.BuyProfitLanding - posValueDetailObj.SellLandingPrice > stopLimitOrderPriceObj.SafetyBalanceAferCharge) && posValueDetailObj.SellQty > 0) // if open position loss more than safety wallet
                {
                    BizResponseClassObj.ErrorCode = enErrorCode.OpenPositionLossMoreThanSafetyWallet;
                }
                stopLimitOrderPriceObj.BizResponseClass = BizResponseClassObj;
                HelperForLog.WriteLogIntoFileAsync("ReCalculateInternalOrderPrice", Helpers.JsonSerialize(stopLimitOrderPriceObj));
                return stopLimitOrderPriceObj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ReCalculateInternalOrderPrice", this.GetType().Name, ex);
                BizResponseClassObj.ReturnCode = enResponseCode.InternalError;
                BizResponseClassObj.ReturnMsg = EnResponseMessage.InternalError;
                BizResponseClassObj.ErrorCode = enErrorCode.InternalError;
                stopLimitOrderPriceObj.BizResponseClass = BizResponseClassObj;
                return stopLimitOrderPriceObj;
            }
        }

        public MarginWithdrawPreConfirmResponse MarginWithdrawPreConfirm(long UserId,string Currency)
        {
            try
            {
                MarginWithdrawPreConfirmResponse preConfirmResponse = new MarginWithdrawPreConfirmResponse();

                if (string.IsNullOrEmpty(Currency))
                {
                    preConfirmResponse.ErrorCode = enErrorCode.MarginCurrencyNull;
                    preConfirmResponse.ReturnCode = enResponseCode.Fail;
                    preConfirmResponse.ReturnMsg = EnResponseMessage.MarginCurrencyNull;
                    return preConfirmResponse;
                }
                preConfirmResponse = _walletSPRepositories.CallSP_MarginWithdrawCalc(UserId,Currency);
                return preConfirmResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MarginWithdrawPreConfirm", "MarginWalletService", ex);
                throw ex;
            }
        }
        public MarginWithdrawPreConfirmResponse MarginWithdraw(long UserId, string Currency)
        {
            try
            {
                MarginWithdrawPreConfirmResponse preConfirmResponse = new MarginWithdrawPreConfirmResponse();

                if (string.IsNullOrEmpty(Currency))
                {
                    preConfirmResponse.ErrorCode = enErrorCode.MarginCurrencyNull;
                    preConfirmResponse.ReturnCode = enResponseCode.Fail;
                    preConfirmResponse.ReturnMsg = EnResponseMessage.MarginCurrencyNull;
                    return preConfirmResponse;
                }
                preConfirmResponse = _walletSPRepositories.CallSP_MarginWithdraw(UserId, Currency);
                return preConfirmResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("MarginWithdraw", "MarginWalletService", ex);
                throw ex;
            }
        }
        public BizResponseClass UpgradeLoan(long UserID,long LoanID,decimal LeverageX)
        {
            try
            {
                BizResponseClass UpgradeLoanResponse = new BizResponseClass();

                if (LoanID == 0)
                {
                    UpgradeLoanResponse.ErrorCode = enErrorCode.LoanIDNull;
                    UpgradeLoanResponse.ReturnCode = enResponseCode.Fail;
                    UpgradeLoanResponse.ReturnMsg = EnResponseMessage.LoanIDNull;
                    return UpgradeLoanResponse;
                }

                UpgradeLoanResponse = _walletSPRepositories.CallSP_UpgradeLoan(UserID, LoanID, LeverageX);
                return UpgradeLoanResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpgradeLoan", "MarginWalletService", ex);
                throw ex;
            }
        }

        public PNLAccountRes GetProfitNLossData(int pageNo, int? pageSize,long? pairId, string currencyName, long id)
        {
            PNLAccountRes Resp = new PNLAccountRes();
            try
            {                
                int newpageSize = Convert.ToInt32(pageSize == null ? Helpers.PageSize : pageSize);
                var data = _walletRepository1.GetProfitNLossData(pairId, currencyName, id);
                if(data.Count > 0)
                {
                    Resp.Data = data;
                    Resp.TotalCount = data.Count;
                    Resp.PageNo = pageNo;
                    Resp.PageSize = newpageSize;
                    pageNo = pageNo + 1;
                    if (pageNo > 0)
                    {
                        int skip = newpageSize * (pageNo - 1);
                        Resp.Data = data.Skip(skip).Take(newpageSize).ToList();
                    }
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.ReportDataNotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }
        public OpenPositionRes GetOpenPosition(long? pairId, long? Userid)
        {
            OpenPositionRes Resp = new OpenPositionRes();
            try
            {
                
                var data = _walletRepository1.GetOpenPosition(Convert.ToInt64(pairId == null ? 0 : pairId), Convert.ToInt64(Userid == null ? 0 : Userid));
                if (data.Count > 0)
                {
                    Resp.Data = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.ReportDataNotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public ListMarginWalletTypeMasterResponse ListAllWalletTypeMasterV2()
        {
            ListMarginWalletTypeMasterResponse listWalletTypeMasterResponse = new ListMarginWalletTypeMasterResponse();
            try
            {
                IEnumerable<MarginWalletTypeMaster> coin = new List<MarginWalletTypeMaster>();
                coin = _WalletTypeMasterRepository.FindBy(item => item.Status != Convert.ToInt16(ServiceStatus.Disable));
                if (coin == null)
                {
                    listWalletTypeMasterResponse.ReturnCode = enResponseCode.Fail;
                    listWalletTypeMasterResponse.ReturnMsg = EnResponseMessage.NotFound;
                    listWalletTypeMasterResponse.ErrorCode = enErrorCode.RecordNotFound;

                }
                else
                {
                    listWalletTypeMasterResponse.walletTypeMasters = coin;
                    listWalletTypeMasterResponse.ReturnCode = enResponseCode.Success;
                    listWalletTypeMasterResponse.ErrorCode = enErrorCode.Success;
                    listWalletTypeMasterResponse.ReturnMsg = EnResponseMessage.FindRecored;
                }

                return listWalletTypeMasterResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                listWalletTypeMasterResponse.ReturnCode = enResponseCode.InternalError;
                return listWalletTypeMasterResponse;
            }
        }

        public MarginWalletTypeMasterResponse AddWalletTypeMaster(WalletTypeMasterRequest addWalletTypeMasterRequest, long Userid)
        {
            MarginWalletTypeMasterResponse walletTypeMasterResponse = new MarginWalletTypeMasterResponse();
            MarginWalletTypeMaster _walletTypeMaster = new MarginWalletTypeMaster();
            try
            {
                if (addWalletTypeMasterRequest == null)
                {
                    walletTypeMasterResponse.ReturnCode = enResponseCode.Fail;
                    walletTypeMasterResponse.ReturnMsg = EnResponseMessage.NotFound;
                    return walletTypeMasterResponse;
                }
                else
                {
                    //Rushabh 04-04-2019 Added Duplicate Record Condition
                    var IsExist = _WalletTypeMasterRepository.GetSingle(i => i.WalletTypeName == addWalletTypeMasterRequest.WalletTypeName);
                    if (IsExist != null)
                    {
                        if (IsExist.Status == Convert.ToInt16(ServiceStatus.Disable))
                        {
                            IsExist.Status = addWalletTypeMasterRequest.Status;
                            IsExist.IsDepositionAllow = addWalletTypeMasterRequest.IsDepositionAllow;
                            IsExist.IsWithdrawalAllow = addWalletTypeMasterRequest.IsWithdrawalAllow;
                            IsExist.IsTransactionWallet = addWalletTypeMasterRequest.IsTransactionWallet;
                            IsExist.Description = addWalletTypeMasterRequest.Description;
                            IsExist.UpdatedBy = Userid;
                            IsExist.UpdatedDate = Helpers.UTC_To_IST();
                            _WalletTypeMasterRepository.UpdateWithAuditLog(IsExist);

                            walletTypeMasterResponse.walletTypeMaster = IsExist;
                            walletTypeMasterResponse.ReturnCode = enResponseCode.Success;
                            walletTypeMasterResponse.ReturnMsg = EnResponseMessage.RecordAdded;
                            walletTypeMasterResponse.ErrorCode = enErrorCode.Success;
                            return walletTypeMasterResponse;
                        }
                        else
                        {
                            walletTypeMasterResponse.walletTypeMaster = _walletTypeMaster;
                            walletTypeMasterResponse.ReturnCode = enResponseCode.Fail;
                            walletTypeMasterResponse.ReturnMsg = EnResponseMessage.DuplicateRecord;
                            walletTypeMasterResponse.ErrorCode = enErrorCode.DuplicateRecord;
                            return walletTypeMasterResponse;
                        }
                    }
                    _walletTypeMaster.CreatedBy = Userid;
                    _walletTypeMaster.CreatedDate = Helpers.UTC_To_IST();
                    _walletTypeMaster.IsDepositionAllow = addWalletTypeMasterRequest.IsDepositionAllow;
                    _walletTypeMaster.IsTransactionWallet = addWalletTypeMasterRequest.IsTransactionWallet;
                    _walletTypeMaster.IsWithdrawalAllow = addWalletTypeMasterRequest.IsWithdrawalAllow;
                    _walletTypeMaster.WalletTypeName = addWalletTypeMasterRequest.WalletTypeName;
                    _walletTypeMaster.Description = addWalletTypeMasterRequest.Description;
                    _walletTypeMaster.Status = Convert.ToInt16(ServiceStatus.Active);
                    _WalletTypeMasterRepository.Add(_walletTypeMaster);
                    walletTypeMasterResponse.walletTypeMaster = _walletTypeMaster;
                    walletTypeMasterResponse.ReturnCode = enResponseCode.Success;
                    walletTypeMasterResponse.ReturnMsg = EnResponseMessage.RecordAdded;
                    walletTypeMasterResponse.ErrorCode = enErrorCode.Success;
                    return walletTypeMasterResponse;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);

                walletTypeMasterResponse.ReturnCode = enResponseCode.InternalError;
                return walletTypeMasterResponse;
            }
        }

        public MarginWalletTypeMasterResponse UpdateWalletTypeMaster(WalletTypeMasterUpdateRequest updateWalletTypeMasterRequest, long Userid, long WalletTypeId)
        {
            MarginWalletTypeMasterResponse walletTypeMasterResponse = new MarginWalletTypeMasterResponse();
            try
            {
                var _walletTypeMaster = _WalletTypeMasterRepository.GetById(WalletTypeId);
                if (_walletTypeMaster == null)
                {
                    walletTypeMasterResponse.ReturnCode = enResponseCode.Fail;
                    walletTypeMasterResponse.ReturnMsg = EnResponseMessage.NotFound;
                    return walletTypeMasterResponse;
                }
                else
                {
                    _walletTypeMaster.UpdatedBy = Userid;
                    _walletTypeMaster.UpdatedDate = Helpers.UTC_To_IST();

                    _walletTypeMaster.IsDepositionAllow = updateWalletTypeMasterRequest.IsDepositionAllow;
                    _walletTypeMaster.IsTransactionWallet = updateWalletTypeMasterRequest.IsTransactionWallet;
                    _walletTypeMaster.IsWithdrawalAllow = updateWalletTypeMasterRequest.IsWithdrawalAllow;
                    _walletTypeMaster.Description = updateWalletTypeMasterRequest.Description;
                    _walletTypeMaster.Status = updateWalletTypeMasterRequest.Status;

                    _WalletTypeMasterRepository.Update(_walletTypeMaster);
                    walletTypeMasterResponse.walletTypeMaster = _walletTypeMaster;
                    walletTypeMasterResponse.ReturnCode = enResponseCode.Success;
                    walletTypeMasterResponse.ReturnMsg = EnResponseMessage.RecordUpdated;
                    walletTypeMasterResponse.ErrorCode = enErrorCode.Success;
                    return walletTypeMasterResponse;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);

                walletTypeMasterResponse.ReturnCode = enResponseCode.InternalError;
                return walletTypeMasterResponse;
            }
        }

        public BizResponseClass DisableWalletTypeMaster(long WalletTypeId)
        {
            try
            {
                var _walletTypeMaster = _WalletTypeMasterRepository.GetById(WalletTypeId);
                if (_walletTypeMaster == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidWallet, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet };
                }
                else
                {
                    _walletTypeMaster.DisableStatus();
                    _WalletTypeMasterRepository.Update(_walletTypeMaster);
                    return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordDisable, ErrorCode = enErrorCode.Success };
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return new BizResponseClass { ReturnCode = enResponseCode.InternalError, };
            }
        }

        public MarginWalletTypeMasterResponse GetWalletTypeMasterById(long WalletTypeId)
        {
            MarginWalletTypeMasterResponse walletTypeMasterResponse = new MarginWalletTypeMasterResponse();
            try
            {
                var _walletTypeMaster = _WalletTypeMasterRepository.GetSingle(item => item.Id == WalletTypeId && item.Status != Convert.ToInt16(ServiceStatus.Disable));
                if (_walletTypeMaster == null)
                {
                    walletTypeMasterResponse.ReturnCode = enResponseCode.Fail;
                    walletTypeMasterResponse.ReturnMsg = EnResponseMessage.NotFound;
                    return walletTypeMasterResponse;
                }
                else
                {
                    walletTypeMasterResponse.walletTypeMaster = _walletTypeMaster;
                    walletTypeMasterResponse.ReturnCode = enResponseCode.Success;
                    walletTypeMasterResponse.ErrorCode = enErrorCode.Success;
                    walletTypeMasterResponse.ReturnMsg = EnResponseMessage.FindRecored;
                    return walletTypeMasterResponse;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                walletTypeMasterResponse.ReturnCode = enResponseCode.InternalError;
                return walletTypeMasterResponse;
            }
        }

        //khushali 11-04-2019 Process for Release Stuck Order - wallet side   
        public enTransactionStatus CheckTransactionSuccessOrNot(long TrnRefNo)
        {
            try
            {
                return _walletRepository1.CheckTransactionSuccessOrNot(TrnRefNo);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita for check settlement process status
        public bool CheckSettlementProceed(long MakerTrnNo, long TakerTrnNo)
        {
            try
            {
                return _walletRepository1.CheckSettlementProceed(MakerTrnNo, TakerTrnNo);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
    }
}
