using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.MarginWallet;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Infrastructure.Data.LPWallet;
using Worldex.Infrastructure.Interfaces;
using MediatR;
using System;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Wallet
{
    public class LPWalletTransactionService : ILPWalletTransaction
    {
        #region cotr
        private readonly ICommonRepository<LPWalletMaster> _LPWalletMaster;
        private readonly ICommonRepository<WalletTypeMaster> _WalletTypeMasterRepository;
        private readonly IWalletTQInsert _WalletTQInsert;
        private readonly ILPSPRepositories _walletSPRepositories;
        private readonly ICommonRepository<WalletMaster> _UserWalletMaster;
        private readonly LPWalletRepository _LPWalletRepository;
        private readonly IMediator _mediator;
        public LPWalletTransactionService(WorldexContext dbContext, ICommonRepository<LPWalletMaster> LPWalletMaster, ICommonRepository<WalletTypeMaster> WalletTypeMasterRepository,
            IWalletTQInsert WalletTQInsert, ILPSPRepositories lPWalletRepository, ICommonRepository<WalletMaster> UserWalletMaster, LPWalletRepository LPWalletRepository, IMediator mediator)
        {
            _LPWalletMaster = LPWalletMaster;
            _WalletTypeMasterRepository = WalletTypeMasterRepository;
            _WalletTQInsert = WalletTQInsert;//ntrivedi 22-01-2018
            _walletSPRepositories = lPWalletRepository;
            _UserWalletMaster = UserWalletMaster;
            _LPWalletRepository = LPWalletRepository;
            _mediator = mediator;

        }
        #endregion

        public async Task<WalletDrCrResponse> LPGetWalletHoldNew(LPHoldDr LPObj)
        {
            try
            {

                WalletDrCrResponse resp = new WalletDrCrResponse();
                enWalletTranxOrderType orderType = enWalletTranxOrderType.Debit;
                WalletTypeMaster walletTypeMaster;
                long userID = 0, TrnNo = 0;
                WalletTransactionQueue objTQ;
                LPWalletMaster dWalletobj;

               
                HelperForLog.WriteLogIntoFileAsync("LPGetWalletHoldNew", "MarginTransactionWalletService", Helpers.JsonSerialize(LPObj));
                if (LPObj.SerProID == 0 || LPObj.CoinName == string.Empty)
                {
                    return new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidWalletOrUserIDorCoinName, TimeStamp = LPObj.Timestamp };
                }
                walletTypeMaster = _WalletTypeMasterRepository.GetForceSingle(e => e.WalletTypeName == LPObj.CoinName);
                if (walletTypeMaster == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidCoinName, TimeStamp = LPObj.Timestamp }, "LPGetWalletHoldNew");
                }
                //2019-2-18 added condi for only used trading wallet
                var dWalletobjTask = _LPWalletMaster.GetForceSingleAsync(e => e.WalletTypeID == walletTypeMaster.Id && e.SerProID == LPObj.SerProID);

                if (LPObj.TrnRefNo == 0) // sell 13-10-2018
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, LPObj.Amount, LPObj.TrnRefNo, Helpers.UTC_To_IST(), null, 0, LPObj.CoinName, userID, LPObj.Timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidTradeRefNo, LPObj.trnType, enErrorCode.InvalidTradeRefNo);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNo, ErrorCode = enErrorCode.InvalidTradeRefNo, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = LPObj.Timestamp }, "LPGetWalletHoldNew");
                }
                if (LPObj.Amount <= 0)
                {
                    // insert with status=2 system failed
                    objTQ = InsertIntoWalletTransactionQueue(Guid.NewGuid(), orderType, LPObj.Amount, LPObj.TrnRefNo, Helpers.UTC_To_IST(), null, 0, LPObj.CoinName, userID, LPObj.Timestamp, enTransactionStatus.SystemFail, EnResponseMessage.InvalidAmt, LPObj.trnType, enErrorCode.InvalidAmount);
                    objTQ = _WalletTQInsert.AddIntoWalletTransactionQueue(objTQ, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAmt, ErrorCode = enErrorCode.InvalidAmount, TrnNo = objTQ.TrnNo, Status = objTQ.Status, StatusMsg = objTQ.StatusMsg, TimeStamp = LPObj.Timestamp }, "LPGetWalletHoldNew");
                }
                dWalletobj = await dWalletobjTask;
                if (dWalletobj == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidWallet, TimeStamp = LPObj.Timestamp }, "LPGetWalletHoldNew");
                }
                LPObj.WalletID = dWalletobj.Id;
                HelperForLog.WriteLogIntoFileAsync("LPGetWalletHoldNew", "CheckUserBalance Pre sp call TrnNo=" + LPObj.TrnRefNo.ToString() + " timestamp:" + LPObj.Timestamp);
                try
                {
                    HelperForLog.WriteLogIntoFileAsync("LPGetWalletHoldNew", "GetLPProviderBalanceMediatR" + LPObj.TrnRefNo.ToString() + " timestamp:" + LPObj.Timestamp);
                    Task.Run(() => CheckBalanceMediatR(LPObj.SerProID, LPObj.CoinName, dWalletobj.Balance, LPObj.TrnRefNo, "LPGetWalletHoldNew"));
                }
                catch (Exception exM)
                {
                    HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + " GetLPProviderBalanceMediatR Exception " + LPObj.TrnRefNo.ToString() + " timestamp:" + LPObj.Timestamp, this.GetType().Name, exM);
                    //HelperForLog.WriteLogIntoFileAsync("LPGetWalletHoldNew", "GetLPProviderBalanceMediatR Exception" + LPObj.TrnRefNo.ToString() + " timestamp:" + LPObj.Timestamp);
                }

                BizResponseClass bizResponse = _walletSPRepositories.Callsp_HoldWallet(LPObj, dWalletobj);
                HelperForLog.WriteLogIntoFileAsync("LPGetWalletHoldNew", "CheckUserBalance Post sp call TrnNo=" + LPObj.TrnRefNo.ToString() + " timestamp:" + LPObj.Timestamp);
                if (bizResponse.ReturnCode == enResponseCode.Success)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessCredit, ErrorCode = enErrorCode.Success, TrnNo = LPObj.TrnNo, Status = 0, StatusMsg = bizResponse.ReturnMsg, TimeStamp = LPObj.Timestamp }, "LPGetWalletHoldNew");
                }
                else
                {
                    // ntrivedi 12-02-2018 status message changed
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = bizResponse.ReturnMsg, ErrorCode = bizResponse.ErrorCode, TrnNo = TrnNo, Status = enTransactionStatus.Initialize, StatusMsg = bizResponse.ReturnMsg, TimeStamp = LPObj.Timestamp }, "LPGetWalletHoldNew");
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.InternalError, TrnNo = 0, Status = 0, StatusMsg = EnResponseMessage.InternalError, TimeStamp = LPObj.Timestamp }, "LPGetWalletHoldNew");
                //throw ex;
            }
        }

        public WalletDrCrResponse GetCrDRResponse(WalletDrCrResponse obj, string extras)
        {
            try
            {
                Task.Run(() => HelperForLog.WriteLogIntoFile(extras, "MarginTransactionWalletService", Helpers.JsonSerialize(obj)));
                return obj;
            }
            catch (Exception ex)
            {
                return obj;
            }
        }

        public WalletTransactionQueue InsertIntoWalletTransactionQueue(Guid Guid, enWalletTranxOrderType TrnType, decimal Amount, long TrnRefNo, DateTime TrnDate, DateTime? UpdatedDate,
           long WalletID, string WalletType, long MemberID, string TimeStamp, enTransactionStatus Status, string StatusMsg, enWalletTrnType enWalletTrnType, enErrorCode enErrorCodeObj, LPOrderType LPType = LPOrderType.LPHoldUser)
        {
            try
            {
                WalletTransactionQueue walletTransactionQueue = new WalletTransactionQueue();
                walletTransactionQueue.Guid = Guid;
                walletTransactionQueue.TrnType = TrnType;
                walletTransactionQueue.Amount = Amount;
                walletTransactionQueue.TrnRefNo = TrnRefNo;
                walletTransactionQueue.TrnDate = TrnDate;
                walletTransactionQueue.UpdatedDate = UpdatedDate;
                walletTransactionQueue.WalletID = WalletID;
                walletTransactionQueue.WalletType = WalletType;
                walletTransactionQueue.MemberID = MemberID;
                walletTransactionQueue.TimeStamp = TimeStamp;
                walletTransactionQueue.Status = Status;
                walletTransactionQueue.StatusMsg = StatusMsg;
                walletTransactionQueue.WalletTrnType = enWalletTrnType;
                walletTransactionQueue.IsProcessing = 0;
                walletTransactionQueue.LPType = LPType;
                walletTransactionQueue.ErrorCode = Convert.ToInt64(enErrorCodeObj);
                return walletTransactionQueue;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        public async Task<WalletDrCrResponse> GetLPWalletCreditDrForHoldNewAsyncFinal(ArbitrageCommonClassCrDr firstCurrObj, string timestamp, enServiceType serviceType, EnAllowedChannels allowedChannels = EnAllowedChannels.Web, enWalletDeductionType enWalletDeductionType = enWalletDeductionType.Normal)
        {
            try
            {
                WalletTransactionQueue tqObj;
                WalletMaster firstCurrObjCrWM, firstCurrObjDrWM;
                LPWalletMaster LPCurrObjCrWM;
                WalletTypeMaster walletTypeFirstCurr, walletTypeSecondCurr;
                bool checkDebitRefNo, checkDebitRefNo1;
                Task<bool> checkDebitRefNoTask;
                Task<bool> checkDebitRefNoTask1;

                Task.Run(() => HelperForLog.WriteLogIntoFile("GetLPWalletCreditDrForHoldNewAsyncFinal first currency", "LPWalletTransactionService", "timestamp:" + timestamp + Helpers.JsonSerialize(firstCurrObj)));
                //2019-2-18 added condi for only used trading wallet
                Task<WalletTypeMaster> walletTypeFirstCurrTask = _WalletTypeMasterRepository.GetSingleAsync(e => e.WalletTypeName == firstCurrObj.Coin);
                Task<WalletTypeMaster> walletTypeSecondCurrTask = _WalletTypeMasterRepository.GetSingleAsync(e => e.WalletTypeName == firstCurrObj.HoldCoin);
                var firstCurrObjCrWMTask = _UserWalletMaster.GetSingleAsync(item => item.Id == firstCurrObj.WalletId && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                walletTypeFirstCurr = await walletTypeFirstCurrTask;
                walletTypeSecondCurr = await walletTypeSecondCurrTask;

                if (walletTypeFirstCurr == null || walletTypeSecondCurr == null)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidCoinName }, "Credit");
                }
                var LPCurrObjCrWMTask = _LPWalletMaster.GetSingleAsync(item => item.WalletTypeID == walletTypeFirstCurr.Id && item.SerProID == firstCurrObj.SerProID && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));

                //2019-2-18 added condi for only used trading wallet
                var firstCurrObjDrWMTask = _UserWalletMaster.GetSingleAsync(item => item.WalletTypeID == walletTypeSecondCurr.Id && item.UserID == firstCurrObj.UserID && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
             
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("GetLPWalletCreditDrForHoldNewAsyncFinal before await1", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.TrnRefNo.ToString()));

                firstCurrObjCrWM = await firstCurrObjCrWMTask;
                if (firstCurrObjCrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjCrWM.Id, firstCurrObj.Coin, firstCurrObjCrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, firstCurrObj.trnType, enErrorCode.FirstCurrCrWalletNotFound);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.FirstCurrCrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }
                LPCurrObjCrWM = await LPCurrObjCrWMTask;
                if (LPCurrObjCrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjCrWM.Id, firstCurrObj.Coin, firstCurrObjCrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, firstCurrObj.trnType, enErrorCode.FirstCurrCrWalletNotFound);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.InvalidArbitarageWallet, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }
                firstCurrObjDrWM = await firstCurrObjDrWMTask;
                if (firstCurrObjDrWM == null)
                {
                    tqObj = InsertIntoWalletTransactionQueue(Guid.NewGuid(), enWalletTranxOrderType.Debit, firstCurrObj.Amount, firstCurrObj.TrnRefNo, Helpers.UTC_To_IST(), null, firstCurrObjCrWM.Id, firstCurrObj.Coin, firstCurrObjCrWM.UserID, timestamp, enTransactionStatus.SystemFail, EnResponseMessage.WalletNotMatch, firstCurrObj.trnType, enErrorCode.FirstCurrDrWalletNotFound);
                    tqObj = _WalletTQInsert.AddIntoWalletTransactionQueue(tqObj, 1);
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet, ErrorCode = enErrorCode.FirstCurrDrWalletNotFound, TrnNo = tqObj.TrnNo, Status = tqObj.Status, StatusMsg = tqObj.StatusMsg, TimeStamp = timestamp }, "Credit");
                }

                Task.Run(() => HelperForLog.WriteLogIntoFile("GetLPWalletCreditDrForHoldNewAsyncFinal before await2", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.TrnRefNo.ToString()));

                Task.Run(() => HelperForLog.WriteLogIntoFile("GetLPWalletCreditDrForHoldNewAsyncFinal after await2", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.TrnRefNo.ToString()));

                if (firstCurrObj.Coin == string.Empty || firstCurrObj.HoldCoin == string.Empty)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidReq, ErrorCode = enErrorCode.InvalidWalletOrUserIDorCoinName }, "Credit");
                }
                if (firstCurrObj.Amount <= 0 || firstCurrObj.HoldAmount <= 0) // ntrivedi amount -ve check
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidAmt, ErrorCode = enErrorCode.InvalidAmt }, "Credit");
                }
                if (firstCurrObj.TrnRefNo == 0)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTradeRefNoCr, ErrorCode = enErrorCode.InvalidTradeRefNoCr }, "Credit");
                }
               
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("GetLPWalletCreditDrForHoldNewAsyncFinal before await3", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.TrnRefNo.ToString()));

                Task.Run(() => HelperForLog.WriteLogIntoFile("GetLPWalletCreditDrForHoldNewAsyncFinal before Wallet operation", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.TrnRefNo.ToString()));

                BizResponseClass bizResponse = _walletSPRepositories.Callsp_LPCrDrWalletForHold(firstCurrObj, timestamp, serviceType, walletTypeFirstCurr.Id, walletTypeSecondCurr.Id, (long)allowedChannels);

                _LPWalletRepository.ReloadEntitySingle(firstCurrObjCrWM, LPCurrObjCrWM, firstCurrObjDrWM);

                if (bizResponse.ReturnCode != enResponseCode.Success)
                {
                    return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = bizResponse.ReturnMsg, ErrorCode = bizResponse.ErrorCode, TrnNo = 0, Status = enTransactionStatus.Initialize, StatusMsg = bizResponse.ReturnMsg, TimeStamp = timestamp }, "Credit");
                }
                decimal ChargefirstCur = 0, ChargesecondCur = 0;
              
                Task.Run(() => HelperForLog.WriteLogIntoFile("LPGetArbitrageWalletCreditDrForHoldNewAsyncFinal after Wallet operation", "WalletService", "timestamp:" + timestamp + " ,Cr TrnNo=" + firstCurrObj.TrnRefNo.ToString() + ", Dr TrnNo=" + firstCurrObj.TrnRefNo.ToString()));
                //ntrivedi LP no need notification
                try
                {
                    HelperForLog.WriteLogIntoFileAsync("LPGetWalletCreditDrForHoldNewAsyncFinal", "GetArbitrageProviderBalanceMediatR" + firstCurrObj.TrnRefNo.ToString() + " timestamp:" + timestamp);
                    Task.Run(() => CheckBalanceMediatR(firstCurrObj.SerProID, firstCurrObj.Coin, LPCurrObjCrWM.Balance, firstCurrObj.TrnRefNo, "GetLPArbitrageWalletCreditDrForHoldNewAsyncFinal"));
                }
                catch (Exception exM)
                {
                    HelperForLog.WriteLogIntoFileAsync("LPGetWalletCreditDrForHoldNewAsyncFinal", "GetArbitrageProviderBalanceMediatR Exception" + firstCurrObj.TrnRefNo.ToString() + " timestamp:" + timestamp);
                }
                Task.Run(() => HelperForLog.WriteLogIntoFile("LPGetWalletCreditDrForHoldNewAsyncFinal:Without Token done", "WalletService", ",timestamp =" + timestamp));
                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessCredit, ErrorCode = enErrorCode.Success, TrnNo = 0, Status = 0, StatusMsg = "", TimeStamp = timestamp }, "GetWalletCreditDrForHoldNewAsyncFinal");
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LPGetLPWalletCreditDrForHoldNewAsyncFinal Timestamp" + timestamp, this.GetType().Name, ex);
                return GetCrDRResponse(new WalletDrCrResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.InternalError, TrnNo = 0, Status = 0, StatusMsg = "", TimeStamp = timestamp }, "GetWalletCreditDrForHoldNewAsyncFinal");
            }
        }

        public async Task<ArbitrageServiceProviderBalance> CheckBalanceMediatR(long SerProID, string Currency, decimal balance, long RefNo, String Type)
        {
            LPBalanceCheck mediatrReq;
            ArbitrageServiceProviderBalance responseCls = new ArbitrageServiceProviderBalance();
            try
            {
                mediatrReq = new LPBalanceCheck();
                mediatrReq.SerProID = SerProID;
                mediatrReq.Currency = Currency;
                mediatrReq.RefNo = RefNo; //logging activity 11-07-2019
                mediatrReq.Type = Type;
                mediatrReq.SystemBal = balance;
                _mediator.Send(mediatrReq);
                return responseCls;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public BizResponseClass ArbitrageRecon(ReconRequest Request, long UserId)
        {
            try
            {
                var Resp = _walletSPRepositories.callsp_LPWalletRecon(Request, UserId);
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
    }
}
