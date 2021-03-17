using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.Wallet;
using System;
using System.Collections.Generic;
using Worldex.Core.ViewModels.ControlPanel;

namespace Worldex.Infrastructure.Interfaces
{
    public interface ICommunicationService
    {
        #region "CommServiceTypeMaster"

        //vsoalnki 13-11-2018
        CommServiceTypeRes GetAllCommServiceTypeMaster();

        #endregion

        #region "TemplateMaster"

        //vsoalnki 13-11-2018
        ListTemplateMasterRes GetAllTemplateMaster();
        BizResponseClass AddTemplateMaster(TemplateMasterReq Request, long userid);
        BizResponseClass UpdateTemplateMaster(TemplateMasterReq Request, long userid);
        BizResponseClass DisableTemplateMaster(long TemplateMasterId, short Status, long userid);
        TemplateMasterRes GetTemplateMasterById(long TemplateMasterId);

        // khushali 12-01-2019
        TemplateCategoryMasterRes GetTemplateMasterByCategory(long TemplateMasterId);
        BizResponseClass UpdateTemplateCategory(long TemplateMasterId, short Status,long TemplateID,long UserId);
        ListTemplateParameterInfoRes TemplateParameterInfo(long? id = null);
        //List<KeyValuePair<string, int>> ListTemplate();
        TemplateRes ListTemplate();
        DeviceUserResponseRes GetDeviceList();

        //khushali 18-01-2019
        List<RequestFormatViewModel> GetAllRequestFormat();
        RequestFormatViewModel GetRequestFormatById(long Id);
        long AddRequestFormat(RequestFormatRequest request, long UserId);
        bool UpdateRequestFormat(RequestFormatRequest request, long UserId);
        bool ChangeStatusRequestFormat(long id, short Status);
        List<CommunicationServiceConfigViewModel> GetCommunicationServiceConfiguration(long ServiceType);
        List<KeyValuePair<string, int>> GetCommunicationServiceType();
        CommunicationServiceConfigViewModel GetCommunicationServiceConfigurationById(long APIID);
        long AddCommunicationServiceConfig(CommunicationServiceConfigRequest request, long UserId);
        bool UpdateCommunicationServiceConfig(CommunicationServiceConfigRequest request, long UserId);
        bool ChangeStatusCommunicationServiceConfig(long APIID, long ServiceID, long SerProID, short Status);

        #endregion

        #region "MessagingQueue"

        //vsolanki 13-11-2018
        ListMessagingQueueRes GetMessagingQueue(DateTime FromDate, DateTime ToDate, short? Status, long? MobileNo, int Page, int? PageSize);

        #endregion

        #region "EmailQueue"

        //vsolanki 14-11-2018
        ListEmailQueueRes GetEmailQueue(DateTime FromDate, DateTime ToDate, short? Status, string Email, int Page, int? PageSize);

        #endregion

        #region "WalletLedger"

        //vsolanki 14-11-2018
        ListWalletLedgerResponse GetWalletLedger(DateTime FromDate, DateTime ToDate, string WalletId, int page, int? PageSize);

        ListWalletLedgerResponse GetWalletLedgerv2(DateTime FromDate, DateTime ToDate, string WalletId, int page, int? PageSize);
        #endregion

        //#region "LimitRuleMaster"

        ////vsoalnki 13-11-2018
        //ListLimitRuleMasterRes GetAllLimitRule();

        ////vsoalnki 13-11-2018
        //BizResponseClass AddLimitRule(LimitRuleMasterReq Request, long userid);

        ////vsoalnki 13-11-2018
        //BizResponseClass UpdateLimitRule(long LimitRuleMasterId, LimitRuleMasterReq Request, long userid);

        ////vsoalnki 13-11-2018
        //BizResponseClass DisableLimitRule(long LimitRuleMasterId);

        ////vsoalnki 13-11-2018
        //ListLimitRuleMasterRes GetLimitRuleById(long LimitRuleMasterId);

        //#endregion

        //#region "ChargeRuleMaster"

        ////vsoalnki 13-11-2018
        //ListChargeRuleMasterRes GetAllChargeRule();

        ////vsoalnki 13-11-2018
        //BizResponseClass AddChargeRule(ChargeRuleMasterReq Request, long userid);

        ////vsoalnki 13-11-2018
        //BizResponseClass UpdateChargeRule(long ChargeRuleMasterId, ChargeRuleMasterReq Request, long userid);

        ////vsoalnki 13-11-2018
        //BizResponseClass DisableChargeRule(long ChargeRuleMasterId);

        ////vsoalnki 13-11-2018
        //ListChargeRuleMasterRes GetChargeRuleById(long ChargeRuleMasterId);

        //#endregion

        #region "NotificationQueue"

        //vsolanki 15-11-2018
        ListNotificationQueueRes GetNotificationQueue(DateTime FromDate, DateTime ToDate, short? Status, int Page, int? PageSize);
        EmailSMSCountData GetEmailSMSAPIManagerCountData();

        #endregion

        dynamic GetSuspiciousUserData(long UserID);
        CurrencyCummulativeData GetCurrencyCummulativeData();
    }
}
