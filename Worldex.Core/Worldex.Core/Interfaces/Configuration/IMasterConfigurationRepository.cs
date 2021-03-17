using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.BackOfficeReports;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.Wallet;
using System;
using System.Collections.Generic;
using Worldex.Core.ViewModels.ControlPanel;

namespace Worldex.Core.Interfaces.Configuration
{
    public interface IMasterConfigurationRepository
    {
        #region Methods

        //khuhsali 17-01-2019 for device Id with user list
        List<DeviceUserResponse> GetDeviceList();

        //khushali 18-01-2019
        List<RequestFormatViewModel> GetAllRequestFormat();
        List<CommunicationServiceConfigViewModel> GetCommunicationServiceConfiguration(long ServiceType);
        CommunicationServiceConfigViewModel GetCommunicationServiceConfigurationById(long APIID);

        //khushali 12-01-2019
        List<TemplateParameterInfoRes> TemplateParameterInfo(long? id = null);

        //vsoalnki 14-11-2018
        List<TemplateResponse> GetAllTemplateMaster();

        //vsoalnki 14-11-2018
        TemplateResponse GetTemplateMasterById(long TemplateMasterId);

        //khushali 10-01-2019
        TemplateCategoryMasterRes GetTemplateMasterByCategory(long TemplateMasterId);

        //khushali 12-01-2019
        List<Template> ListTemplateType();

        //vsoalnki 14-11-2018
        ListMessagingQueueRes GetMessagingQueue(DateTime FromDate, DateTime ToDate, short? Status, long? MobileNo, int Page, int? PageSize);

        //vsoalnki 14-11-2018
        ListEmailQueueRes GetEmailQueue(DateTime FromDate, DateTime ToDate, short? Status, string Email, int Page, int? PageSize);

        //vsoalnki 14-11-2018
        List<WalletLedgerResponse> GetWalletLedger(DateTime FromDate, DateTime ToDate, long WalletId, int page, int? PageSize,ref int TotalCount);

        List<WalletLedgerResponse> GetWalletLedgerv2(DateTime FromDate, DateTime ToDate, long WalletId, int page, int? PageSize, ref int TotalCount);

        //vsoalnki 15-11-2018
        ListNotificationQueueRes GetNotificationQueue(DateTime FromDate, DateTime ToDate, short? Status, int Page, int? PageSize);

        #endregion

        long GetMaxPlusOneTemplate();

        RptWithdrawalRes GetWithdrawalRpt(DateTime FromDate, DateTime ToDate, string CoinName, long? UserID, short? Status, int PageNo, int? PageSize, string Address, string TrnID, string TrnNo, long? OrgId, short? IsInternalTransfer);

        RptWithdrawalResv2 GetWithdrawalRptv2(DateTime FromDate, DateTime ToDate, string CoinName, long? UserID, short? Status, int PageNo, int? PageSize, string Address, string TrnID, string TrnNo, long? OrgId, short? IsInternalTransfer);

        RptDepositionRes GetDepositionRpt(DateTime FromDate, DateTime ToDate, string CoinName, long? UserID, short? Status, int PageNo, int? PageSize,string Address, string TrnID, long? OrgId);

        RptDepositionResv2 GetDepositionRptv2(DateTime FromDate, DateTime ToDate, string CoinName, long? UserID, short? Status, int PageNo, int? PageSize, string Address, string TrnID, long? OrgId);

        long GetTemplate(string tempName);

        List<EmailLists> GetEmailLists();
        EmailSMSCountResp GetEmailSMSCount();

        dynamic GetSuspiciousUserData(long UserID);
        CurrencyCummulativeData GetCurrencyCummulativeData();
    }
}
