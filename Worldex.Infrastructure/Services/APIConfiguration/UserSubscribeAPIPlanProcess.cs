using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.FeedConfiguration;
using Worldex.Core.ViewModels.APIConfiguration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.APIConfiguration
{
    public class UserSubscribeAPIPlanProcess : IUserSubscribeAPIPlanProcess
    {
        private readonly ILogger<UserSubscribeAPIPlanProcess> _logger;
        private ICommonRepository<APIPlanMaster> _APIPlanMasterRepository;
        private ICommonRepository<UserSubscribeAPIPlan> _SubScribePlanRepository;
        private UserSubscribeAPIPlan UserSubscribeAPIPlanObj = new UserSubscribeAPIPlan();
        APIPlanSubscribeProcess PlanSubscribeObj = new APIPlanSubscribeProcess();
        private ICommonRepository<UserAPIKeyDetails> _UserAPIKeyDetailRepository;
        private ICommonRepository<WhiteListIPEndPoint> _WhiteListIPEndPointRepository;
        private readonly IAPIConfigurationRepository _configurationRepository;
        private readonly IWalletSPRepositories _IWalletSPRepositories;

        public UserSubscribeAPIPlanProcess(ILogger<UserSubscribeAPIPlanProcess> logger,
            ICommonRepository<APIPlanMaster> APIPlanMasterRepository, ICommonRepository<UserSubscribeAPIPlan> SubScribePlanRepository,
            ICommonRepository<UserAPIKeyDetails> UserAPIKeyDetailRepository, ICommonRepository<WhiteListIPEndPoint> WhiteListIPEndPointRepository,
            IAPIConfigurationRepository configurationRepository, IWalletSPRepositories IWalletSPRepositories)
        {
            _logger = logger;
            _APIPlanMasterRepository = APIPlanMasterRepository;
            _SubScribePlanRepository = SubScribePlanRepository;
            _UserAPIKeyDetailRepository = UserAPIKeyDetailRepository;
            _WhiteListIPEndPointRepository = WhiteListIPEndPointRepository;
            _configurationRepository = configurationRepository;
            _IWalletSPRepositories = IWalletSPRepositories;
        }

        #region Subscribe plan
        public async Task<BizResponseClass> UserAPIPlanSubscribe(UserAPIPlanSubscribeRequest Request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            UserSubscribeAPIPlan OldPlanHistoryObj = null;
            try
            {
                if(Request.ChannelID!=(short)EnAllowedChannels.App && Request.ChannelID != (short)EnAllowedChannels.Web)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidChannelType;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = " In Valid Channel";
                    return _Res;
                }
                APIPlanMaster PlanDetails = _APIPlanMasterRepository.FindBy(e => e.Id == Request.SubscribePlanID && e.Status == 1 ).SingleOrDefault();
                if (PlanDetails == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidRequestedPlan;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = " In Valid Requested Plan";
                    return _Res;
                }
                
                if (String.IsNullOrEmpty(Request.OldPlanID.ToString())) //Case for subscribe plan
                {
                    UserSubscribeAPIPlan PlanHistoryObj1 = _SubScribePlanRepository.FindBy(e => e.UserID == UserID && (e.Status == 1 || e.Status == 0 || e.Status == 4) && e.APIPlanMasterID==Request.SubscribePlanID).FirstOrDefault();
                    if (PlanHistoryObj1 != null)
                    {
                        _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_RequestedPlanIsActivated;
                        _Res.ReturnCode = enResponseCode.Fail;
                        _Res.ReturnMsg = "Requested Plan Is Activated";
                        return _Res;
                    }
                }
                //Check for Requested plan is Active/InProcess or not
                UserSubscribeAPIPlan PlanHistoryObj = _SubScribePlanRepository.FindBy(e => e.APIPlanMasterID == Request.SubscribePlanID && e.UserID == UserID && (e.Status == 1 || e.Status == 0 || e.Status == 4)).FirstOrDefault();

                if(PlanHistoryObj != null)
                {
                    switch (PlanHistoryObj.Status)
                    {
                        case (short)enTransactionStatus.Success:
                            _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_RequestedPlanIsActivated;
                            _Res.ReturnCode = enResponseCode.Fail;
                            _Res.ReturnMsg = "Requested Plan Is Activated";
                            return _Res;
                        case (short)enTransactionStatus.Hold:
                            _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_RequestedPlanInProcess;
                            _Res.ReturnCode = enResponseCode.Fail;
                            _Res.ReturnMsg = "Requested Plan In Process";
                            return _Res;
                        case (short)enTransactionStatus.Pending:
                            _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_RequestedPlanIsInPending;
                            _Res.ReturnCode = enResponseCode.Fail;
                            _Res.ReturnMsg = "Requested Plan Is In Pending";
                            return _Res;
                    }
                }

                PlanSubscribeObj.PlanName = PlanDetails.PlanName;
                PlanSubscribeObj.PlanID = PlanDetails.Id;
                PlanSubscribeObj.Price = PlanDetails.Price;
                PlanSubscribeObj.PlanValidity = PlanDetails.PlanValidity;
                PlanSubscribeObj.PlanValidityType = PlanDetails.PlanValidityType;
                PlanSubscribeObj.UserID = UserID;
                PlanSubscribeObj.DebitedCurrency = PlanDetails.Currency;
                PlanSubscribeObj.Charge = 0;// PlanDetails.Charge;
                PlanSubscribeObj.Perticuler = PlanSubscribeObj.PlanName + " Plan Subscribe";
                PlanSubscribeObj.TotalAmt = PlanSubscribeObj.Price +PlanSubscribeObj.Charge;
                PlanSubscribeObj.ServiceID = PlanDetails.ServiceID;
                PlanSubscribeObj.ChannelID = Request.ChannelID;

                if (!String.IsNullOrEmpty(Request.OldPlanID.ToString()))//for Upgrade-Downgrade plan
                {
                    OldPlanHistoryObj = _SubScribePlanRepository.FindBy(e => e.APIPlanMasterID == Request.OldPlanID && e.UserID == PlanSubscribeObj.UserID && e.Status == 1).SingleOrDefault();
                    if (OldPlanHistoryObj != null)
                    {
                        var oldPlan = _APIPlanMasterRepository.FindBy(e => e.Id == OldPlanHistoryObj.APIPlanMasterID).SingleOrDefault();

                        PlanSubscribeObj.Perticuler = PlanDetails.Priority > oldPlan.Priority ? " Downgrade " + PlanDetails.PlanName + " To Plan " + oldPlan.PlanName + " Requested On" : "Upgrade " + PlanDetails.PlanName + " To Plan " + oldPlan.PlanName + " Requested On";
                        PlanSubscribeObj.Perticuler += "  " + DateTime.UtcNow.ToString();
                        
                       
                    }
                    else
                    {
                        _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_CurrentPlanNotFound;
                        _Res.ReturnCode = enResponseCode.Fail;
                        _Res.ReturnMsg = " Current Plan NotFound";
                        return _Res;
                    }
                }
                //Add into UserSubscribeAPIPlan with status=0
                PlanSubscribeObj.SubscribePlanID = this.InsertSubScribePlanHistory();
                //Check Wallet balance
                var ServiceID = PlanSubscribeObj.ServiceID;
                var WalletDebit = await DebitWalletProcess(UserID, ServiceID,Request.ChannelID, PlanSubscribeObj.TotalAmt,(long)PlanSubscribeObj.SubscribePlanID,(short)enWalletTrnType.PublicAPIPlan);
                //if success update status=1
                if (WalletDebit.ErrorCode == enErrorCode.Success)
                {
                    if (!String.IsNullOrEmpty(Request.OldPlanID.ToString()))
                    {
                        OldPlanHistoryObj.Status = (short)enTransactionStatus.InActive;
                        OldPlanHistoryObj.UpdatedBy = 999;
                        OldPlanHistoryObj.UpdatedDate = DateTime.UtcNow;
                        _SubScribePlanRepository.Update(OldPlanHistoryObj);
                        this.setDisableUserAPIKeys(OldPlanHistoryObj.APIPlanMasterID, UserID, OldPlanHistoryObj.Id);
                        if (OldPlanHistoryObj.NextAutoRenewId != null)
                            _configurationRepository.ExpireOldRenewPlan(Convert.ToInt64(Request.OldPlanID), UserID);
                    }
                    UserSubscribeAPIPlanObj.PaymentStatus = 1;
                    UserSubscribeAPIPlanObj.ActivationDate = DateTime.UtcNow;
                    UserSubscribeAPIPlanObj.DebitedAccountID = WalletDebit.AccountID;
                    //UserSubscribeAPIPlanObj.DebitedCurrency = WalletDebit.Currency;
                    UserSubscribeAPIPlanObj.Perticuler = PlanSubscribeObj.Perticuler;
                    UserSubscribeAPIPlanObj.ExpiryDate = PlanSubscribeObj.PlanValidityType == 2 ? DateTime.UtcNow.AddMonths(PlanSubscribeObj.PlanValidity) : DateTime.UtcNow.AddYears(PlanSubscribeObj.PlanValidity);
                    UserSubscribeAPIPlanObj.Status = (short)enTransactionStatus.Success;
                    
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "Success";
                }
                else
                {
                    UserSubscribeAPIPlanObj.Status = (short)enTransactionStatus.SystemFail;
                    UserSubscribeAPIPlanObj.Perticuler = WalletDebit.ReturnMsg;
                    UserSubscribeAPIPlanObj.DebitedAccountID = WalletDebit.ErrorCode.ToString();

                    _Res.ErrorCode = WalletDebit.ErrorCode;
                    _Res.ReturnCode = WalletDebit.ReturnCode;
                    _Res.ReturnMsg = WalletDebit.ReturnMsg;
                }
                this.UpdateSubScribePlanHistory();
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public long InsertSubScribePlanHistory()
        {
            try
            {
                UserSubscribeAPIPlanObj = _SubScribePlanRepository.Add(new UserSubscribeAPIPlan()
                {
                    APIPlanMasterID= PlanSubscribeObj.PlanID,
                    Charge= PlanSubscribeObj.Charge,
                    CreatedBy= PlanSubscribeObj.UserID,
                    CreatedDate=DateTime.UtcNow,
                    DebitedAccountID="",
                    DebitedCurrency= PlanSubscribeObj.DebitedCurrency,
                    Perticuler= PlanSubscribeObj.Perticuler,
                    Price= PlanSubscribeObj.Price,
                    Status=0,
                    UserID= PlanSubscribeObj.UserID,
                    TotalAmt= PlanSubscribeObj.TotalAmt,
                    ServiceID= PlanSubscribeObj.ServiceID,
                    ChannelID=PlanSubscribeObj.ChannelID
                });
                return UserSubscribeAPIPlanObj.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public void UpdateSubScribePlanHistory()
        {
            try
            {
                _SubScribePlanRepository.Update(UserSubscribeAPIPlanObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public async Task<APIPlanSubscribeWalletDeduct> DebitWalletProcess(long UserID,long ServiceID,short ChannelID,decimal Amount,long TrnRefNo,short TrnType)
        {
            APIPlanSubscribeWalletDeduct _Res = new APIPlanSubscribeWalletDeduct();
            try
            {
                var log = " UserID = "+ UserID + ", ServiceID = "+ ServiceID + " , ChannelID = "+ ChannelID + " , Amount = "+ Amount + ",  TrnRefNo = "+TrnRefNo+", TrnType = "+ TrnType;
                HelperForLog.WriteLogIntoFile("DebitWalletProcess", "UserSubscribeAPIPlanProcess", log);
                var res = _IWalletSPRepositories.Callsp_APIPlanDepositProcess(UserID, ServiceID, ChannelID, Amount, TrnRefNo, TrnType);
                if (res.ErrorCode != enErrorCode.sp_APIPlanDepositProcess_Success)
                {
                    _Res.ErrorCode = res.ErrorCode;
                    _Res.ReturnCode = res.ReturnCode;
                    _Res.ReturnMsg = res.ReturnMsg;
                    return _Res;
                }
                else
                {
                    _Res.AccountID = res.AccountID;
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "Success";
                    return _Res;
                }
                //_Res.AccountID = "7208665602002741";
                //_Res.ErrorCode = enErrorCode.Success;
                //_Res.ReturnCode = enResponseCode.Success;
                //_Res.ReturnMsg = "Success";
                //return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public void setDisableUserAPIKeys(long PlanID,long UserID,long SubscribeID)
        {
            long[] KeyArr;
            string Keys = "";
            try
            {
                var Keylist = _UserAPIKeyDetailRepository.FindBy(e => e.APIPlanMasterID == PlanID && e.UserID == UserID && e.Status==1).ToList();
                if(Keylist.Count>0)
                {
                    KeyArr = new long[Keylist.Count];
                    KeyArr= Array.ConvertAll<UserAPIKeyDetails, long>(Keylist.ToArray(), x => (long)x.Id);
                    Keys = string.Join(",", KeyArr);
                    _configurationRepository.DisablePlanService(PlanID, UserID, SubscribeID, Keys);
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        

        #endregion
        #region AutoRenew 
        public async Task<BizResponseClass> APIPlanAutoRenewProcess(AutoRenewPlanRequest request,long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            APIPlanSubscribeProcess PlanSubscribeObj2 = new APIPlanSubscribeProcess();
            try
            {
                //get current plan
                var CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Id == request.SubscribePlanID && e.UserID==UserID && e.Status==1).SingleOrDefault();
                if(CurrentPlanHistory==null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidSubscribeID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "InValid Subscribe ID";
                    return _Res;
                }
                if (CurrentPlanHistory.IsAutoRenew == 1)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_PlanAlreadyAutoRenew;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Plan Already Auto Renew";
                    return _Res;
                }
                var PlanDetail = _APIPlanMasterRepository.FindBy(e => e.Id == CurrentPlanHistory.APIPlanMasterID && e.Status==1).SingleOrDefault();
                if (PlanDetail == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidRequestedPlanOrDisable;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "In Valid Requested Plan Or Disable";
                    return _Res;
                }
                if(PlanDetail.IsPlanRecursive==0)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_PlanIsNotRecursive;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Requested Plan is Not Recursive";
                    return _Res;
                }
                //UserSubscribeAPIPlan PlanHistoryObj1 = _SubScribePlanRepository.FindBy(e => e.UserID == UserID && (e.Status == 0 || e.Status == 4)).FirstOrDefault();
                //if (PlanHistoryObj1 != null)
                //{
                //    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_PlanAlreadyRenew;
                //    _Res.ReturnCode = enResponseCode.Fail;
                //    _Res.ReturnMsg = "Plan Already Renew";
                //    return _Res;
                //}

                //createdDate,createdby,status,planid,expirydate,price,charge,totalamt,userid,perticuler,debiaccountid,currency,paymentstatus
                //PlanSubscribeObj2.PlanName = PlanDetail.PlanName;
                //PlanSubscribeObj2.PlanID = PlanDetail.Id;
                //PlanSubscribeObj2.Price = PlanDetail.Price;
                //PlanSubscribeObj2.PlanValidity = PlanDetail.PlanValidity;
                //PlanSubscribeObj2.PlanValidityType = PlanDetail.PlanValidityType;
                //PlanSubscribeObj2.UserID = UserID;
                //PlanSubscribeObj2.DebitedCurrency = "BTC";
                //PlanSubscribeObj2.Charge = 0;//PlanDetails.Charge;
                //PlanSubscribeObj2.Perticuler ="Auto Renew "+ PlanSubscribeObj2.PlanName + " Plan";
                //PlanSubscribeObj2.TotalAmt = PlanSubscribeObj2.Price;
                //PlanSubscribeObj2.RenewDays = request.DaysBeforeExpiry;
                //PlanSubscribeObj2.CustomeLimitId = CurrentPlanHistory.CustomeLimitId;

                //var UserSubscribeAPIPlanObj2= this.InsertAutoRenewEntry(PlanSubscribeObj2);
                //var WalletDebit = await DebitWalletProcess(UserID, PlanSubscribeObj2.DebitedCurrency);

                //if (WalletDebit.ErrorCode == enErrorCode.Success)
                //{
                //    var Days = "-" + request.DaysBeforeExpiry;
                //    UserSubscribeAPIPlanObj2.ActivationDate = CurrentPlanHistory.ExpiryDate.Value.AddDays(1);
                //    UserSubscribeAPIPlanObj2.DebitedAccountID = WalletDebit.AccountID;
                //    UserSubscribeAPIPlanObj2.DebitedCurrency = WalletDebit.Currency;
                //    UserSubscribeAPIPlanObj2.ExpiryDate = PlanSubscribeObj2.PlanValidityType == 2 ? UserSubscribeAPIPlanObj2.ActivationDate.Value.AddMonths(PlanSubscribeObj2.PlanValidity) : UserSubscribeAPIPlanObj2.ActivationDate.Value.AddYears(PlanSubscribeObj2.PlanValidity);
                //    UserSubscribeAPIPlanObj2.PaymentStatus = 1;

                var renewStatus = CurrentPlanHistory.RenewStatus;
                CurrentPlanHistory.RenewDays = request.DaysBeforeExpiry;
                CurrentPlanHistory.IsAutoRenew = 1;
                _SubScribePlanRepository.Update(CurrentPlanHistory);

                while (renewStatus == 1)
                {
                    CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Id == CurrentPlanHistory.NextAutoRenewId && e.Status == 0).SingleOrDefault();
                    if (CurrentPlanHistory == null)
                        break;
                    CurrentPlanHistory.RenewDays = request.DaysBeforeExpiry;
                    CurrentPlanHistory.IsAutoRenew = 1;
                    renewStatus = CurrentPlanHistory.RenewStatus;
                    _SubScribePlanRepository.Update(CurrentPlanHistory);
                }


                //CurrentPlanHistory.NextAutoRenewId = UserSubscribeAPIPlanObj2.Id;
                //CurrentPlanHistory.RenewDate = UserSubscribeAPIPlanObj2.ActivationDate;
                
                //}
                //else
                //{
                //    UserSubscribeAPIPlanObj2.Status = (short)enTransactionStatus.SystemFail;
                //    UserSubscribeAPIPlanObj2.Perticuler = WalletDebit.ReturnMsg;
                //}
                //_SubScribePlanRepository.Update(UserSubscribeAPIPlanObj2);
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public UserSubscribeAPIPlan InsertAutoRenewEntry(APIPlanSubscribeProcess obj)
        {
            try
            {
                var Model = _SubScribePlanRepository.Add(new UserSubscribeAPIPlan()
                {
                    APIPlanMasterID = obj.PlanID,
                    Charge = obj.Charge,
                    CreatedBy = obj.UserID,
                    CreatedDate = DateTime.UtcNow,
                    DebitedAccountID = "",
                    DebitedCurrency = obj.DebitedCurrency,
                    ServiceID= obj.ServiceID,
                    Perticuler = obj.Perticuler,
                    Price = obj.Price,
                    Status = 0,
                    UserID = obj.UserID,
                    TotalAmt = obj.TotalAmt,
                    RenewDays=obj.RenewDays,
                    CustomeLimitId=obj.CustomeLimitId,
                    IsAutoRenew=obj.IsAutoRenew,
                    ChannelID=obj.ChannelID
                });
                return Model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public async Task<BizResponseClass> StopAutoRenewProcess(StopAutoRenewRequest request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                var CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Id == request.SubscribeID && e.UserID == UserID && e.Status == 1).SingleOrDefault();
                if (CurrentPlanHistory == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidSubscribeID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "InValid Subscribe ID";
                    return _Res;
                }
                if (CurrentPlanHistory.IsAutoRenew == 0)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_PlanIsNotAutoRenew;
;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Plan Is Not Auto Renew";
                    return _Res;
                }

                var renewStatus = CurrentPlanHistory.RenewStatus;
                CurrentPlanHistory.RenewDays = 0;
                CurrentPlanHistory.IsAutoRenew = 0;
                _SubScribePlanRepository.Update(CurrentPlanHistory);

                while (renewStatus == 1)
                {
                    CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Id == CurrentPlanHistory.NextAutoRenewId && e.Status == 0 ).SingleOrDefault();
                    if (CurrentPlanHistory == null)
                        break;
                    CurrentPlanHistory.RenewDays =0;
                    CurrentPlanHistory.IsAutoRenew = 0;
                    renewStatus = CurrentPlanHistory.RenewStatus;
                    _SubScribePlanRepository.Update(CurrentPlanHistory);
                }


                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public async Task<BizResponseClass> PlanAutoRenewEntry(UserSubscribeAPIPlan Obj)
        {
            APIPlanSubscribeProcess PlanSubscribeObj2 = new APIPlanSubscribeProcess();
            BizResponseClass _Res = new BizResponseClass();
            try
            {
                var CurrentPlanHistory = Obj;
                PlanSubscribeObj2.PlanID = CurrentPlanHistory.APIPlanMasterID;
                PlanSubscribeObj2.UserID = CurrentPlanHistory.UserID;
                PlanSubscribeObj2.ChannelID = CurrentPlanHistory.ChannelID;
                
                var PlanDetail = _APIPlanMasterRepository.FindBy(e => e.Id == CurrentPlanHistory.APIPlanMasterID).SingleOrDefault();
                if (PlanDetail == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidRequestedPlanOrDisable;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = " In Valid Requested Plan";
                    PlanSubscribeObj2.Perticuler = _Res.ReturnMsg;
                    this.InsertAutoRenewEntry(PlanSubscribeObj2);
                    return _Res;
                }
                if (PlanDetail.IsPlanRecursive == 0)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_PlanIsNotRecursive;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Requested Plan is Not Recursive";
                    PlanSubscribeObj2.Perticuler = _Res.ReturnMsg;
                    this.InsertAutoRenewEntry(PlanSubscribeObj2);
                    return _Res;
                }

                PlanSubscribeObj2.ServiceID = PlanDetail.ServiceID;
                PlanSubscribeObj2.DebitedCurrency = PlanDetail.Currency;
                PlanSubscribeObj2.PlanName = PlanDetail.PlanName;
                PlanSubscribeObj2.PlanValidity = PlanDetail.PlanValidity;
                PlanSubscribeObj2.PlanValidityType = PlanDetail.PlanValidityType;
                PlanSubscribeObj2.Price = PlanDetail.Price;
                PlanSubscribeObj2.Charge = 0;//PlanDetails.Charge;
                PlanSubscribeObj2.TotalAmt = PlanSubscribeObj2.Price;
                PlanSubscribeObj2.Perticuler = "Auto Renew " + PlanSubscribeObj2.PlanName + " Plan";
                PlanSubscribeObj2.IsAutoRenew = CurrentPlanHistory.IsAutoRenew;
                PlanSubscribeObj2.RenewDays = CurrentPlanHistory.RenewDays;
                PlanSubscribeObj2.CustomeLimitId = CurrentPlanHistory.CustomeLimitId;
                

                var UserSubscribeAPIPlanObj2 = this.InsertAutoRenewEntry(PlanSubscribeObj2);

                PlanSubscribeObj2.ExpiryDate = CurrentPlanHistory.ExpiryDate;
                PlanSubscribeObj2.ActivationDate = CurrentPlanHistory.ActivationDate;

                var ServiceID = PlanSubscribeObj2.ServiceID;
                var WalletDebit = await DebitWalletProcess(PlanSubscribeObj2.UserID, ServiceID,(short)PlanSubscribeObj2.ChannelID, PlanSubscribeObj2.TotalAmt, (long)UserSubscribeAPIPlanObj2.Id, (short)enWalletTrnType.PublicAPIPlan);

                if (WalletDebit.ErrorCode == enErrorCode.Success)
                {
                    UserSubscribeAPIPlanObj2.PaymentStatus = 1;
                    UserSubscribeAPIPlanObj2.ActivationDate = PlanSubscribeObj2.ExpiryDate.Value.AddDays(1);
                    UserSubscribeAPIPlanObj2.DebitedAccountID = WalletDebit.AccountID;
                    //UserSubscribeAPIPlanObj2.DebitedCurrency = WalletDebit.Currency;
                    UserSubscribeAPIPlanObj2.ExpiryDate = PlanSubscribeObj2.PlanValidityType == 2 ? UserSubscribeAPIPlanObj2.ActivationDate.Value.AddMonths(PlanSubscribeObj2.PlanValidity) : UserSubscribeAPIPlanObj2.ActivationDate.Value.AddYears(PlanSubscribeObj2.PlanValidity);
                    UserSubscribeAPIPlanObj2.Status = (short)enTransactionStatus.Initialize;

                    //CurrentPlanHistory.IsAutoRenew = 1;
                    CurrentPlanHistory.NextAutoRenewId = UserSubscribeAPIPlanObj2.Id;
                    CurrentPlanHistory.RenewDate = UserSubscribeAPIPlanObj2.ActivationDate;
                    CurrentPlanHistory.RenewStatus = 1;
                    _SubScribePlanRepository.Update(CurrentPlanHistory);
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "Success";
                }
                else
                {
                    UserSubscribeAPIPlanObj2.Status = (short)enTransactionStatus.SystemFail;
                    UserSubscribeAPIPlanObj2.Perticuler = WalletDebit.ReturnMsg;
                    UserSubscribeAPIPlanObj2.DebitedAccountID = WalletDebit.ErrorCode.ToString();
                    _Res.ErrorCode = WalletDebit.ErrorCode;
                    _Res.ReturnCode = WalletDebit.ReturnCode;
                    _Res.ReturnMsg = WalletDebit.ReturnMsg;

                }
                _SubScribePlanRepository.Update(UserSubscribeAPIPlanObj2);
                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        #endregion

        #region ManualRenew Process
        public async Task<BizResponseClass> ManualRenewAPIPlan(ManualRenewAPIPlanRequest request, long UserID)
        {
            BizResponseClass _Res = new BizResponseClass();
            APIPlanSubscribeProcess PlanSubscribeObj2 = new APIPlanSubscribeProcess();
            try
            {
                if (request.ChannelID != (short)EnAllowedChannels.App && request.ChannelID != (short)EnAllowedChannels.Web)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidChannelType;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = " In Valid Channel";
                    return _Res;
                }
                var CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Id == request.SubscribePlanID && e.UserID == UserID && e.Status == 1).SingleOrDefault();
                if (CurrentPlanHistory == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidSubscribeID;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "InValid Subscribe ID";
                    return _Res;
                }
                //if (CurrentPlanHistory.ExpiryDate > DateTime.UtcNow || CurrentPlanHistory.Status==1)
                //{
                //    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_CurrentPlanIsActive;
                //    _Res.ReturnCode = enResponseCode.Fail;
                //    _Res.ReturnMsg = "Current Plan is Active ";
                //    return _Res;
                //}
                //if (CurrentPlanHistory.RenewStatus == 1)
                //{
                //    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_PlanAlreadyRenew;
                //    _Res.ReturnCode = enResponseCode.Fail;
                //    _Res.ReturnMsg = "InValid Subscribe ID";
                //    return _Res;
                //}
                var PlanDetail = _APIPlanMasterRepository.FindBy(e => e.Id == CurrentPlanHistory.APIPlanMasterID && e.Status == 1).SingleOrDefault();
                if (PlanDetail == null)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_InValidRequestedPlanOrDisable;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = " In Valid Requested Plan";
                    return _Res;
                }
                if (PlanDetail.IsPlanRecursive == 0)
                {
                    _Res.ErrorCode = enErrorCode.SubscribeAPIPlan_PlanIsNotRecursive;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "Requested Plan is Not Recursive";
                    return _Res;
                }

                PlanSubscribeObj2.ExpiryDate = CurrentPlanHistory.ExpiryDate;
                PlanSubscribeObj2.ActivationDate = CurrentPlanHistory.ActivationDate;
                PlanSubscribeObj2.CustomeLimitId = CurrentPlanHistory.CustomeLimitId;
                PlanSubscribeObj2.IsAutoRenew = CurrentPlanHistory.IsAutoRenew;
                PlanSubscribeObj2.RenewDays = CurrentPlanHistory.RenewDays;
                PlanSubscribeObj2.ServiceID = PlanDetail.ServiceID;
                PlanSubscribeObj2.DebitedCurrency = PlanDetail.Currency;
                var renewStatus = CurrentPlanHistory.RenewStatus;

                while(renewStatus==1)
                {
                    CurrentPlanHistory = _SubScribePlanRepository.FindBy(e => e.Id== CurrentPlanHistory.NextAutoRenewId && e.Status==0).SingleOrDefault();
                    if (CurrentPlanHistory == null)
                        break;

                    renewStatus = CurrentPlanHistory.RenewStatus;
                    PlanSubscribeObj2.ExpiryDate = CurrentPlanHistory.ExpiryDate;
                    PlanSubscribeObj2.ActivationDate = CurrentPlanHistory.ActivationDate;
                }

                PlanSubscribeObj2.PlanName = PlanDetail.PlanName;
                PlanSubscribeObj2.PlanID = PlanDetail.Id;
                PlanSubscribeObj2.Price = PlanDetail.Price;
                PlanSubscribeObj2.PlanValidity = PlanDetail.PlanValidity;
                PlanSubscribeObj2.PlanValidityType = PlanDetail.PlanValidityType;
                PlanSubscribeObj2.UserID = UserID;
                //PlanSubscribeObj2.DebitedCurrency = "BTC";
                PlanSubscribeObj2.Charge = 0;//PlanDetails.Charge;
                PlanSubscribeObj2.Perticuler = "Manual Renew " + PlanSubscribeObj2.PlanName + " Plan";
                PlanSubscribeObj2.TotalAmt = PlanSubscribeObj2.Price;
                PlanSubscribeObj2.ChannelID = request.ChannelID;
                
                //PlanSubscribeObj2.ActivationDate = DateTime.UtcNow;//change 

                var UserSubscribeAPIPlanObj2 = this.InsertAutoRenewEntry(PlanSubscribeObj2);
                var ServiceID = PlanSubscribeObj2.ServiceID;
                var WalletDebit = await DebitWalletProcess(UserID, ServiceID, request.ChannelID, PlanSubscribeObj2.TotalAmt, (long)UserSubscribeAPIPlanObj2.Id, (short)enWalletTrnType.PublicAPIPlan);

                if (WalletDebit.ErrorCode == enErrorCode.Success)
                {
                    UserSubscribeAPIPlanObj2.PaymentStatus = 1;
                    UserSubscribeAPIPlanObj2.ActivationDate = PlanSubscribeObj2.ExpiryDate.Value.AddDays(1);
                    UserSubscribeAPIPlanObj2.DebitedAccountID = WalletDebit.AccountID;
                    //UserSubscribeAPIPlanObj2.DebitedCurrency = WalletDebit.Currency;
                    UserSubscribeAPIPlanObj2.ExpiryDate = PlanSubscribeObj2.PlanValidityType == 2 ? UserSubscribeAPIPlanObj2.ActivationDate.Value.AddMonths(PlanSubscribeObj2.PlanValidity) : UserSubscribeAPIPlanObj2.ActivationDate.Value.AddYears(PlanSubscribeObj2.PlanValidity);
                    UserSubscribeAPIPlanObj2.Status = (short)enTransactionStatus.Initialize;

                    //CurrentPlanHistory.IsAutoRenew = 1;
                    CurrentPlanHistory.NextAutoRenewId = UserSubscribeAPIPlanObj2.Id;
                    CurrentPlanHistory.RenewDate = UserSubscribeAPIPlanObj2.ActivationDate;
                    CurrentPlanHistory.RenewStatus = 1;
                    _SubScribePlanRepository.Update(CurrentPlanHistory);
                    _Res.ErrorCode = enErrorCode.Success;
                    _Res.ReturnCode = enResponseCode.Success;
                    _Res.ReturnMsg = "Success";
                }
                else
                {
                    UserSubscribeAPIPlanObj2.Status = (short)enTransactionStatus.SystemFail;
                    UserSubscribeAPIPlanObj2.Perticuler = WalletDebit.ReturnMsg;
                    UserSubscribeAPIPlanObj2.DebitedAccountID = WalletDebit.ErrorCode.ToString();
                    _Res.ErrorCode = WalletDebit.ErrorCode;
                    _Res.ReturnCode = WalletDebit.ReturnCode;
                    _Res.ReturnMsg = WalletDebit.ReturnMsg;
                    
                }
                _SubScribePlanRepository.Update(UserSubscribeAPIPlanObj2);
                return _Res;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        #endregion
    }
}
