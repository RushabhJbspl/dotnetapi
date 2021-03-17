using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using Worldex.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services
{
    public class WithdrawRecon : IWithdrawRecon
    {
        private readonly IBasePage _basePage;
        private readonly ICommonRepository<TransactionQueue> _transactionQueueRepository;
        private readonly ICommonRepository<ServiceMaster> _serviceMasterRepository;
        private readonly IWalletTransactionCrDr _WalletService;
        private readonly IBackOfficeTrnRepository _backOfficeTrnRepository;
        TransactionRecon tradeReconObj;

        public WithdrawRecon(IBasePage basePage, ICommonRepository<TransactionQueue> transactionQueueRepository, ICommonRepository<ServiceMaster> serviceMasterRepository, IWalletTransactionCrDr WalletService, IBackOfficeTrnRepository backOfficeTrnRepository)
        {
            _basePage = basePage;
            _transactionQueueRepository = transactionQueueRepository;
            _serviceMasterRepository = serviceMasterRepository;
            _WalletService = WalletService;
            _backOfficeTrnRepository = backOfficeTrnRepository;
        }

        public async Task<BizResponseClass> WithdrawalReconV1(WithdrawalReconRequest request, long UserId, string accessToken)
        {
            BizResponseClass Response = new BizResponseClass();
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

                        //Refund Process(Credit To User Wallet)
                        List<CreditWalletDrArryTrnID> CreditWalletDrArryTrnIDList = new List<CreditWalletDrArryTrnID>();
                        CreditWalletDrArryTrnIDList.Add(new CreditWalletDrArryTrnID { DrTrnRefNo = request.TrnNo, Amount = TransactionQueueObj.Amount });

                        var _TrnService = _serviceMasterRepository.GetSingle(item => item.SMSCode == TransactionQueueObj.SMSCode && item.Status == Convert.ToInt16(ServiceStatus.Active));
                        var ServiceType = (enServiceType)_TrnService.ServiceType;
                        var CreditResult1 = _WalletService.GetWalletCreditNewAsync(TransactionQueueObj.SMSCode, Helpers.GetTimeStamp(), enWalletTrnType.Refund, TransactionQueueObj.Amount, TransactionQueueObj.MemberID,
                            TransactionQueueObj.DebitAccountID, CreditWalletDrArryTrnIDList.ToArray(), request.TrnNo, 1, enWalletTranxOrderType.Credit, ServiceType, (enTrnType)TransactionQueueObj.TrnType,"", TransactionQueueObj.GUID.ToString());

                        var CreditResult = await CreditResult1;

                        if (CreditResult.ReturnCode != enResponseCode.Success)
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

                        //Debit From User Wallet
                        var DebitResult = _WalletService.GetWalletDeductionNew(TransactionQueueObj.SMSCode, Helpers.GetTimeStamp(), enWalletTranxOrderType.Debit, TransactionQueueObj.Amount, TransactionQueueObj.MemberID,
                            TransactionQueueObj.DebitAccountID, request.TrnNo, ServiceType, enWalletTrnType.Withdrawal, (enTrnType)TransactionQueueObj.TrnType, accessToken, TransactionQueueObj.GUID.ToString());

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

                var ResultBool = _backOfficeTrnRepository.WithdrawalRecon(tradeReconObj, TransactionQueueObj);
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
    }
}
