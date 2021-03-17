using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Infrastructure.Data;
using Worldex.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Entities.Communication;
using Worldex.Core.ViewModels.ControlPanel;

namespace Worldex.Infrastructure.Services.Configuration
{
    public class CommunicationService : BasePage, ICommunicationService
    {
        #region "Variables"

        private readonly ICommonRepository<CommServiceTypeMaster> _CommServiceTypeMaster;
        //private readonly ICommonRepository<LimitRuleMaster> _LimitRuleMaster;
        private readonly ICommonRepository<ChargeRuleMaster> _ChargeRuleMaster;
        private readonly ICommonRepository<WalletMaster> _WalletMaster;
        private readonly IWalletSPRepositories _walletSPRepositories;
        private readonly ICommonRepository<TemplateMaster> _TemplateMaster;
        private readonly ICommonRepository<TemplateCategoryMaster> _TemplateCategoryMaster;
        private readonly ILogger<CommunicationService> _log;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IMasterConfigurationRepository _masterConfigurationRepository;
        private readonly IMessageConfiguration _messageConfiguration;
        private readonly ICommonRepository<RequestFormatMaster> _RequestFormatAPIRepository;
        private readonly ICommonRepository<CommAPIServiceMaster> _CommAPIServiceMasterRepository;
        private readonly ICommonRepository<CommServiceMaster> _CommServiceMasterRepository;
        private readonly ICommonRepository<CommServiceproviderMaster> _CommServiceproviderMasterRepository;
        #endregion

        #region "cotr"

        public CommunicationService(ILogger<CommunicationService> log, ILogger<BasePage> logger, 
            ICommonRepository<CommServiceTypeMaster> CommServiceTypeMaster, ICommonRepository<TemplateMaster> TemplateMaster, IWalletSPRepositories walletSPRepositories,
            WorldexContext dbContext, Microsoft.Extensions.Configuration.IConfiguration configuration, 
            IMasterConfigurationRepository masterConfigurationRepository, //ICommonRepository<LimitRuleMaster> LimitRuleMaster, 
            ICommonRepository<ChargeRuleMaster> ChargeRuleMaster, ICommonRepository<WalletMaster> WalletMaster, 
            IMessageConfiguration messageConfiguration, ICommonRepository<TemplateCategoryMaster> TemplateCategoryMaster,
            ICommonRepository<CommAPIServiceMaster> CommAPIServiceMasterRepository,
            ICommonRepository<CommServiceMaster> CommServiceMasterRepository,
            ICommonRepository<CommServiceproviderMaster> CommServiceproviderMasterRepository,
                ICommonRepository<RequestFormatMaster> RequestFormatAPIRepository): base(logger)
        {
            _CommServiceTypeMaster = CommServiceTypeMaster;
            _TemplateMaster = TemplateMaster;
            _log = log;
            _masterConfigurationRepository = masterConfigurationRepository;
            _configuration = configuration;
            _walletSPRepositories = walletSPRepositories;
             _ChargeRuleMaster = ChargeRuleMaster;
            _WalletMaster = WalletMaster;
            _messageConfiguration = messageConfiguration;
            _TemplateCategoryMaster = TemplateCategoryMaster;
            _CommAPIServiceMasterRepository = CommAPIServiceMasterRepository;
            _CommServiceMasterRepository = CommServiceMasterRepository;
            _CommServiceproviderMasterRepository = CommServiceproviderMasterRepository;
            _RequestFormatAPIRepository = RequestFormatAPIRepository;
        }

        #endregion

        #region "CommServiceTypeMaster"

        //vsoalnki 13-11-2018
        public CommServiceTypeRes GetAllCommServiceTypeMaster()
        {
            CommServiceTypeRes res = new CommServiceTypeRes();
            try
            {
                // var items = _CommServiceTypeMaster.FindBy(i => i.Status == Convert.ToInt16(ServiceStatus.Active));
                var items = _CommServiceTypeMaster.List();
                if (items.Count == 0)
                {
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.RecordNotFound;
                    res.ReturnMsg = EnResponseMessage.NotFound;
                    return res;
                }
                res.ReturnCode = enResponseCode.Success;
                res.ErrorCode = enErrorCode.Success;
                res.ReturnMsg = EnResponseMessage.FindRecored;
                res.Response = items;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        // khushali 17-01-2019
        public DeviceUserResponseRes GetDeviceList()
        {
            DeviceUserResponseRes res = new DeviceUserResponseRes();
            try
            {
                var items = _masterConfigurationRepository.GetDeviceList();
                if (items.Count == 0)
                {
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.RecordNotFound;
                    res.ReturnMsg = EnResponseMessage.NotFound;
                    return res;
                }
                res.ReturnCode = enResponseCode.Success;
                res.ErrorCode = enErrorCode.Success;
                res.ReturnMsg = EnResponseMessage.FindRecored;
                res.Result = items;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region "TemplateMaster"

        //vsoalnki 13-11-2018
        public ListTemplateMasterRes GetAllTemplateMaster()
        {
            ListTemplateMasterRes res = new ListTemplateMasterRes();
            try
            {
                var items = _masterConfigurationRepository.GetAllTemplateMaster();
                //var items = _TemplateMaster.List();
                // var items = _TemplateMaster.FindBy(i => i.Status == Convert.ToInt16(ServiceStatus.Active));
                if (items.Count == 0)
                {
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.RecordNotFound;
                    res.ReturnMsg = EnResponseMessage.NotFound;
                    return res;
                }
                // res.Template = _masterConfigurationRepository.ListTemplate();
                res.Template = Helpers.GetEnumList<EnTemplateType>();
                res.ReturnCode = enResponseCode.Success;
                res.ErrorCode = enErrorCode.Success;
                res.ReturnMsg = EnResponseMessage.FindRecored;
                res.TemplateMasterObj = items;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        //khushali 12-01-2019
        public ListTemplateParameterInfoRes TemplateParameterInfo(long? id = null)
        {
            ListTemplateParameterInfoRes res = new ListTemplateParameterInfoRes();
            try
            {
                var items = _masterConfigurationRepository.TemplateParameterInfo(id);
                //var items = _TemplateMaster.List();
                // var items = _TemplateMaster.FindBy(i => i.Status == Convert.ToInt16(ServiceStatus.Active));
                if (items.Count == 0)
                {
                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.RecordNotFound;
                    res.ReturnMsg = EnResponseMessage.NotFound;
                    return res;
                }
                // res.Template = _masterConfigurationRepository.ListTemplate();
                res.ReturnCode = enResponseCode.Success;
                res.ErrorCode = enErrorCode.Success;
                res.ReturnMsg = EnResponseMessage.FindRecored;
                res.templateParameterInfoList = items;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsoalnki 13-11-2018
        public BizResponseClass AddTemplateMaster(TemplateMasterReq Request, long userid)
        {
            try
            {
                TemplateMaster template = new TemplateMaster();
                var templateObj = _TemplateMaster.GetSingle(i => i.Id == Request.ID && i.Status != 9);
                if (templateObj != null)
                {
                    return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Alredy_Exist, ErrorCode = enErrorCode.Alredy_Exist };
                }
                if (Request != null)
                {
                    template.Status = Convert.ToInt16(ServiceStatus.Active);
                    template.UpdatedDate = UTC_To_IST();
                    template.CreatedDate = UTC_To_IST();
                    template.CreatedBy = userid;

                    
                    //template.TemplateID = _masterConfigurationRepository.GetTemplate(Request.TemplateName);
                    template.TemplateID = Request.TemplateID; // khushali 09-01-2019 Added for template category 
                    // template.TemplateID =Convert.ToInt64(Request.TemplateName);
                    template.TemplateName = Request.TemplateName; 
                    template.AdditionalInfo = Request.AdditionalInfo;
                    template.CommServiceTypeID = Request.CommServiceTypeID;
                    template.Content = Request.Content;
                    //template.IsOnOff = Request.IsOnOff;
                    _TemplateMaster.Add(template);
                    _messageConfiguration.ReloadTEmplateMaster();
                    return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded, ErrorCode = enErrorCode.Success };
                }
                return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput, ErrorCode = enErrorCode.InvalidInput };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsoalnki 13-11-2018
        public BizResponseClass UpdateTemplateMaster(TemplateMasterReq Request, long userid)
        {
            try
            {
                var template = _TemplateMaster.GetSingle(i => i.Id == Request.ID && i.Status != 9);
                //TemplateMaster template = new TemplateMaster();
                if (template == null)
                {
                    return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound, ErrorCode = enErrorCode.NotFound };
                }
                if (Request != null)
                {
                   // template.Status = Convert.ToInt16(ServiceStatus.Active);
                    template.UpdatedDate = UTC_To_IST();
                  //  template.CreatedDate = template.CreatedDate; //UTC_To_IST();
                  //  template.CreatedBy = template.CreatedBy;// userid;
                    template.UpdatedBy = userid;

                    //template.TemplateID = _masterConfigurationRepository.GetTemplate(Request.TemplateName);
                    template.TemplateID = Request.TemplateID; // khushali 09-01-2019 Added for template category 
                    template.TemplateName = Request.TemplateName;
                    template.AdditionalInfo = Request.AdditionalInfo;
                    template.CommServiceTypeID = Request.CommServiceTypeID;
                    template.Content = Request.Content;
                    //template.IsOnOff = Request.IsOnOff;
                    _TemplateMaster.Update(template);
                    _messageConfiguration.ReloadTEmplateMaster();
                    return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated, ErrorCode = enErrorCode.Success };
                }
                return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput, ErrorCode = enErrorCode.InvalidInput };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsoalnki 13-11-2018
        public BizResponseClass DisableTemplateMaster(long TemplateMasterId, short Status,long UserId)
        {
            try
            {
                var template = _TemplateMaster.GetById(TemplateMasterId);
                var templateCategoryDetail = _TemplateCategoryMaster.GetById(template.TemplateID);
                if (template != null && templateCategoryDetail != null)
                {
                    //disable status
                    // template.DisableService();
                    if(!(templateCategoryDetail.TemplateId == TemplateMasterId && Status == 0)) // khushali 12-01-2019 for tempalet already mapped so not disable
                    {
                        template.Status = Status;
                        template.UpdatedBy = UserId;
                        template.UpdatedDate = UTC_To_IST();
                        //update in DB
                        _TemplateMaster.UpdateWithAuditLog(template);
                        _messageConfiguration.ReloadTEmplateMaster();
                        return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated, ErrorCode = enErrorCode.Success };
                    }
                    else
                    {
                        return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.TempalteAlreadyMapwithCategory, ErrorCode = enErrorCode.TempalteAlreadyMapwithCategory };
                    }
                }
                return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput, ErrorCode = enErrorCode.InvalidInput };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        //khushali 10-01-2018 
        public BizResponseClass UpdateTemplateCategory(long TemplateMasterId, short Status,long TemplateID,long UserId)
        {
            try
            {
                var template = _TemplateCategoryMaster.GetById(TemplateMasterId);
                if (template != null)
                {
                    //disable status
                    // template.DisableService();
                    template.IsOnOff = Status;
                    template.TemplateId = TemplateID;
                    template.UpdatedBy = UserId;
                    template.UpdatedDate = UTC_To_IST();
                    //update in DB
                    _TemplateCategoryMaster.UpdateWithAuditLog(template);
                    _messageConfiguration.ReloadTEmplateMaster();
                    return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated, ErrorCode = enErrorCode.Success };
                }
                return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput, ErrorCode = enErrorCode.InvalidInput };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsoalnki 13-11-2018
        public TemplateMasterRes GetTemplateMasterById(long TemplateMasterId)
        {
            TemplateMasterRes res = new TemplateMasterRes();
            try
            {
                var template = _masterConfigurationRepository.GetTemplateMasterById(TemplateMasterId);
                // var template = _TemplateMaster.GetById(TemplateMasterId);
                if (template != null)
                {
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                    res.ReturnMsg = EnResponseMessage.FindRecored;
                    res.TemplateMasterObj = template;
                    return res;
                }
                res.ReturnCode = enResponseCode.Fail;
                res.ErrorCode = enErrorCode.RecordNotFound;
                res.ReturnMsg = EnResponseMessage.NotFound;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        // khushali 10-01-2019
        public TemplateCategoryMasterRes GetTemplateMasterByCategory(long TemplateMasterId)
        {
            TemplateCategoryMasterRes res = new TemplateCategoryMasterRes();
            try
            {
                var template = _masterConfigurationRepository.GetTemplateMasterByCategory(TemplateMasterId);
                // var template = _TemplateMaster.GetById(TemplateMasterId);
                if (template != null)
                {
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                    res.ReturnMsg = EnResponseMessage.FindRecored;
                    res.TemplateMasterObj = template.TemplateMasterObj;
                    res.IsOnOff = template.IsOnOff;
                    res.TemplateID = template.TemplateID;
                    return res;
                }
                res.ReturnCode = enResponseCode.Fail;
                res.ErrorCode = enErrorCode.RecordNotFound;
                res.ReturnMsg = EnResponseMessage.NotFound;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //public List<KeyValuePair<string, int>> ListTemplate()
        //{
        //    try
        //    {
        //        var Template = Helpers.GetEnumList<EnTemplateType>();
        //        return Template;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        public TemplateRes ListTemplate()
        {
            TemplateRes res = new TemplateRes();
            try
            {
                //var Template = Helpers.GetEnumList<EnTemplateType>();
                //return Template;
                var template = _masterConfigurationRepository.ListTemplateType();
                // var template = _TemplateMaster.GetById(TemplateMasterId);
                if (template != null)
                {
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                    res.ReturnMsg = EnResponseMessage.FindRecored;
                    res.Result = template;
                    return res;
                }
                res.ReturnCode = enResponseCode.Fail;
                res.ErrorCode = enErrorCode.RecordNotFound;
                res.ReturnMsg = EnResponseMessage.NotFound;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region "MessagingQueue"

        //vsolanki 13-11-2018
        public ListMessagingQueueRes GetMessagingQueue(DateTime FromDate, DateTime ToDate, short? Status, long? MobileNo, int Page, int? PageSize)
        {
            try
            {
                ListMessagingQueueRes res = new ListMessagingQueueRes();
                int Customday = Convert.ToInt32(_configuration["ReportValidDays"]);//2;
                double days = (ToDate - FromDate).TotalDays;
                if (days < 0 || Customday < days)
                {
                    var msg = EnResponseMessage.MoreDays;
                    msg = msg.Replace("#X#", Customday.ToString());

                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.MoreDays;
                    res.ReturnMsg = msg;
                    return res;
                }
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                var items = _masterConfigurationRepository.GetMessagingQueue(FromDate, ToDate, Status, MobileNo, Page, PageSize);
                if (items.MessagingQueueObj.Count != 0)
                {

                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                    res.ReturnMsg = EnResponseMessage.FindRecored;
                    res.MessagingQueueObj = items.MessagingQueueObj;
                    res.TotalPage = items.TotalPage;
                    res.PageSize = items.PageSize;
                    res.Count = items.Count;
                    return res;
                }
                res.ReturnCode = enResponseCode.Fail;
                res.ErrorCode = enErrorCode.RecordNotFound;
                res.ReturnMsg = EnResponseMessage.NotFound;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region "EmailQueue"

        //vsolanki 14-11-2018
        public ListEmailQueueRes GetEmailQueue(DateTime FromDate, DateTime ToDate, short? Status, string Email, int Page, int? PageSize)
        {
            try
            {
                ListEmailQueueRes res = new ListEmailQueueRes();
                int Customday = Convert.ToInt32(_configuration["ReportValidDays"]);//2;
                double days = (ToDate - FromDate).TotalDays;
                if (days < 0 || Customday < days)
                {
                    var msg = EnResponseMessage.MoreDays;
                    msg = msg.Replace("#X#", Customday.ToString());

                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.MoreDays;
                    res.ReturnMsg = msg;
                    return res;
                }
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                var items = _masterConfigurationRepository.GetEmailQueue(FromDate, ToDate, Status, Email, Page, PageSize);
                if (items.EmailQueueObj.Count != 0)
                {
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                    res.ReturnMsg = EnResponseMessage.FindRecored;
                    res.EmailQueueObj = items.EmailQueueObj;
                    res.TotalPage = items.TotalPage;
                    res.PageSize = items.PageSize;
                    res.Count = items.Count;
                    return res;
                }
                res.ReturnCode = enResponseCode.Fail;
                res.ErrorCode = enErrorCode.RecordNotFound;
                res.ReturnMsg = EnResponseMessage.NotFound;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region "WalletLedger"

        //vsolanki 14-11-2018
        public ListWalletLedgerResponse GetWalletLedger(DateTime FromDate, DateTime ToDate, string WalletId, int page, int? PageSize)
        {
            try
            {
                ListWalletLedgerResponse res = new ListWalletLedgerResponse();
                res.PageNo = page;
                res.PageSize =Convert.ToInt32(PageSize);
                page = page + 1;
                int Customday = Convert.ToInt32(_configuration["ReportValidDays"]);//2;
                double days = (ToDate - FromDate).TotalDays;
                if (days < 0 || Customday < days)
                {
                    var msg = EnResponseMessage.MoreDays;
                    msg = msg.Replace("#X#", Customday.ToString());

                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.MoreDays;
                    res.ReturnMsg = msg;
                    return res;
                }
                int TotalCount = 0;
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                var Wallets = _WalletMaster.GetSingle(i => i.AccWalletID == WalletId);
                var items = _masterConfigurationRepository.GetWalletLedger(FromDate, ToDate, Wallets.Id, page, PageSize,ref TotalCount);
                res.TotalCount = TotalCount;
                if (items.Count != 0)
                {
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                    res.ReturnMsg = EnResponseMessage.FindRecored;
                    res.WalletLedgers = items;
                    return res;
                }
                res.ReturnCode = enResponseCode.Fail;
                res.ErrorCode = enErrorCode.RecordNotFound;
                res.ReturnMsg = EnResponseMessage.NotFound;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ListWalletLedgerResponse GetWalletLedgerv2(DateTime FromDate, DateTime ToDate, string WalletId, int page, int? PageSize)
        {
            try
            {
                ListWalletLedgerResponse res = new ListWalletLedgerResponse();
                res.PageNo = page;
                res.PageSize = Convert.ToInt32(PageSize);
                page = page + 1;
                int Customday = Convert.ToInt32(_configuration["ReportValidDays"]);//2;
                double days = (ToDate - FromDate).TotalDays;
                if (days < 0 || Customday < days)
                {
                    var msg = EnResponseMessage.MoreDays;
                    msg = msg.Replace("#X#", Customday.ToString());

                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.MoreDays;
                    res.ReturnMsg = msg;
                    return res;
                }
                int TotalCount = 0;
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                var Wallets = _WalletMaster.GetSingle(i => i.AccWalletID == WalletId);
               // var items = _masterConfigurationRepository.GetWalletLedgerv2(FromDate, ToDate, Wallets.Id, page, PageSize, ref TotalCount);
                var items = _walletSPRepositories.CallGetWalletLedger(FromDate, ToDate, Wallets.Id, page, PageSize, ref TotalCount);
                res.TotalCount = TotalCount;
                if (items.Count != 0)
                {
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                    res.ReturnMsg = EnResponseMessage.FindRecored;
                    res.WalletLedgers = items;
                    return res;
                }
                res.ReturnCode = enResponseCode.Fail;
                res.ErrorCode = enErrorCode.RecordNotFound;
                res.ReturnMsg = EnResponseMessage.NotFound;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        //#region "LimitRuleMaster"

        ////vsoalnki 13-11-2018
        //public ListLimitRuleMasterRes GetAllLimitRule()
        //{
        //    ListLimitRuleMasterRes res = new ListLimitRuleMasterRes();
        //    try
        //    {
        //        var items = _masterConfigurationRepository.GetAllLimitRule();
        //        if (items.Count == 0)
        //        {
        //            res.ReturnCode = enResponseCode.Fail;
        //            res.ErrorCode = enErrorCode.RecordNotFound;
        //            res.ReturnMsg = EnResponseMessage.NotFound;
        //            return res;
        //        }
        //        res.ReturnCode = enResponseCode.Success;
        //        res.ErrorCode = enErrorCode.Success;
        //        res.ReturnMsg = EnResponseMessage.FindRecored;
        //        res.LimitRuleObj = items;
        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        ////vsoalnki 13-11-2018
        //public BizResponseClass AddLimitRule(LimitRuleMasterReq Request, long userid)
        //{
        //    try
        //    {
        //        LimitRuleMaster template = new LimitRuleMaster();
        //        if (Request != null)
        //        {
        //            template.Status = Convert.ToInt16(ServiceStatus.Active);
        //            template.UpdatedDate = UTC_To_IST();
        //            template.CreatedDate = UTC_To_IST();
        //            template.CreatedBy = userid;

        //            template.MaxAmount = Request.MaxAmount;
        //            template.MinAmount = Request.MinAmount;
        //            template.Name = Request.Name;
        //            template.TrnType = Request.TrnType;
        //            template.WalletType = Request.WalletType;

        //            _LimitRuleMaster.Add(template);

        //            return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded, ErrorCode = enErrorCode.Success };
        //        }
        //        return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput, ErrorCode = enErrorCode.InvalidInput };
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        ////vsoalnki 13-11-2018
        //public BizResponseClass UpdateLimitRule(long LimitRuleMasterId, LimitRuleMasterReq Request, long userid)
        //{
        //    try
        //    {
        //        var template = _LimitRuleMaster.GetSingle(i => i.Id == LimitRuleMasterId);
        //        if (Request != null)
        //        {
        //            template.Status = Convert.ToInt16(ServiceStatus.Active);
        //            template.UpdatedDate = UTC_To_IST();
        //            template.CreatedDate = template.CreatedDate; //UTC_To_IST();
        //            template.CreatedBy = template.CreatedBy;// userid;
        //            template.UpdatedBy = userid;

        //            template.MaxAmount = Request.MaxAmount;
        //            template.MinAmount = Request.MinAmount;
        //            template.Name = Request.Name;
        //            template.TrnType = Request.TrnType;
        //            template.WalletType = Request.WalletType;

        //            _LimitRuleMaster.Update(template);

        //            return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated, ErrorCode = enErrorCode.Success };
        //        }
        //        return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput, ErrorCode = enErrorCode.InvalidInput };
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        ////vsoalnki 13-11-2018
        //public BizResponseClass DisableLimitRule(long LimitRuleMasterId)
        //{
        //    try
        //    {
        //        var template = _LimitRuleMaster.GetById(LimitRuleMasterId);
        //        if (template != null)
        //        {
        //            //disable status
        //            template.DisableService();
        //            //update in DB
        //            _LimitRuleMaster.Update(template);

        //            return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordDisable, ErrorCode = enErrorCode.Success };
        //        }
        //        return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput, ErrorCode = enErrorCode.InvalidInput };
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        ////vsoalnki 13-11-2018
        //public ListLimitRuleMasterRes GetLimitRuleById(long LimitRuleMasterId)
        //{
        //    ListLimitRuleMasterRes res = new ListLimitRuleMasterRes();
        //    try
        //    {
        //        var template = _masterConfigurationRepository.GetLimitRuleById(LimitRuleMasterId);
        //        // var template = _TemplateMaster.GetById(TemplateMasterId);
        //        if (template != null)
        //        {
        //            res.ReturnCode = enResponseCode.Success;
        //            res.ErrorCode = enErrorCode.Success;
        //            res.ReturnMsg = EnResponseMessage.FindRecored;
        //            res.LimitRuleObj = template;
        //            return res;
        //        }
        //        res.ReturnCode = enResponseCode.Fail;
        //        res.ErrorCode = enErrorCode.RecordNotFound;
        //        res.ReturnMsg = EnResponseMessage.NotFound;
        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        //#endregion

        //#region "ChargeRuleMaster"

        ////vsoalnki 13-11-2018
        //public ListChargeRuleMasterRes GetAllChargeRule()
        //{
        //    ListChargeRuleMasterRes res = new ListChargeRuleMasterRes();
        //    try
        //    {
        //        var items = _masterConfigurationRepository.GetAllChargeRule();
        //        if (items.Count == 0)
        //        {
        //            res.ReturnCode = enResponseCode.Fail;
        //            res.ErrorCode = enErrorCode.RecordNotFound;
        //            res.ReturnMsg = EnResponseMessage.NotFound;
        //            return res;
        //        }
        //        res.ReturnCode = enResponseCode.Success;
        //        res.ErrorCode = enErrorCode.Success;
        //        res.ReturnMsg = EnResponseMessage.FindRecored;
        //        res.ChargeRuleObj = items;
        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        ////vsoalnki 13-11-2018
        //public BizResponseClass AddChargeRule(ChargeRuleMasterReq Request, long userid)
        //{
        //    try
        //    {
        //        ChargeRuleMaster template = new ChargeRuleMaster();
        //        if (Request != null)
        //        {
        //            template.Status = Convert.ToInt16(ServiceStatus.Active);
        //            template.UpdatedDate = UTC_To_IST();
        //            template.CreatedDate = UTC_To_IST();
        //            template.CreatedBy = userid;

        //            template.MaxAmount = Request.MaxAmount;
        //            template.MinAmount = Request.MinAmount;
        //            template.Name = Request.Name;
        //            template.TrnType = Request.TrnType;
        //            template.WalletType = Request.WalletType;
        //            template.ChargeType = Request.ChargeType;
        //            template.ChargeValue = Request.ChargeValue;

        //            _ChargeRuleMaster.Add(template);

        //            return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded, ErrorCode = enErrorCode.Success };
        //        }
        //        return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput, ErrorCode = enErrorCode.InvalidInput };
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        ////vsoalnki 13-11-2018
        //public BizResponseClass UpdateChargeRule(long ChargeRuleMasterId, ChargeRuleMasterReq Request, long userid)
        //{
        //    try
        //    {
        //        var template = _ChargeRuleMaster.GetSingle(i => i.Id == ChargeRuleMasterId);
        //        if (Request != null)
        //        {
        //            template.Status = Convert.ToInt16(ServiceStatus.Active);
        //            template.UpdatedDate = UTC_To_IST();
        //            template.CreatedDate = template.CreatedDate; //UTC_To_IST();
        //            template.CreatedBy = template.CreatedBy;// userid;
        //            template.UpdatedBy = userid;

        //            template.MaxAmount = Request.MaxAmount;
        //            template.MinAmount = Request.MinAmount;
        //            template.Name = Request.Name;
        //            template.TrnType = Request.TrnType;
        //            template.WalletType = Request.WalletType;
        //            template.ChargeType = Request.ChargeType;
        //            template.ChargeValue = Request.ChargeValue;

        //            _ChargeRuleMaster.Update(template);

        //            return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated, ErrorCode = enErrorCode.Success };
        //        }
        //        return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput, ErrorCode = enErrorCode.InvalidInput };
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        ////vsoalnki 13-11-2018
        //public BizResponseClass DisableChargeRule(long ChargeRuleMasterId)
        //{
        //    try
        //    {
        //        var template = _ChargeRuleMaster.GetById(ChargeRuleMasterId);
        //        if (template != null)
        //        {
        //            //disable status
        //            template.DisableService();
        //            //update in DB
        //            _ChargeRuleMaster.Update(template);

        //            return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordDisable, ErrorCode = enErrorCode.Success };
        //        }
        //        return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput, ErrorCode = enErrorCode.InvalidInput };
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        ////vsoalnki 13-11-2018
        //public ListChargeRuleMasterRes GetChargeRuleById(long ChargeRuleMasterId)
        //{
        //    ListChargeRuleMasterRes res = new ListChargeRuleMasterRes();
        //    try
        //    {
        //        var template = _masterConfigurationRepository.GetChargeRuleById(ChargeRuleMasterId);
        //        // var template = _TemplateMaster.GetById(TemplateMasterId);
        //        if (template != null)
        //        {
        //            res.ReturnCode = enResponseCode.Success;
        //            res.ErrorCode = enErrorCode.Success;
        //            res.ReturnMsg = EnResponseMessage.FindRecored;
        //            res.ChargeRuleObj = template;
        //            return res;
        //        }
        //        res.ReturnCode = enResponseCode.Fail;
        //        res.ErrorCode = enErrorCode.RecordNotFound;
        //        res.ReturnMsg = EnResponseMessage.NotFound;
        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        //#endregion

        #region "NotificationQueue"

        //vsolanki 15-11-2018
        public ListNotificationQueueRes GetNotificationQueue(DateTime FromDate, DateTime ToDate, short? Status, int Page, int? PageSize)
        {
            try
            {
                ListNotificationQueueRes res = new ListNotificationQueueRes();
                int Customday = Convert.ToInt32(_configuration["ReportValidDays"]);//2;
                double days = (ToDate - FromDate).TotalDays;
                if (days < 0 || Customday < days)
                {
                    var msg = EnResponseMessage.MoreDays;
                    msg = msg.Replace("#X#", Customday.ToString());

                    res.ReturnCode = enResponseCode.Fail;
                    res.ErrorCode = enErrorCode.MoreDays;
                    res.ReturnMsg = msg;
                    return res;
                }
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                var items = _masterConfigurationRepository.GetNotificationQueue(FromDate, ToDate, Status, Page, PageSize);
                if (items.NotificationQueueObj.Count != 0)
                {
                    res.ReturnCode = enResponseCode.Success;
                    res.ErrorCode = enErrorCode.Success;
                    res.ReturnMsg = EnResponseMessage.FindRecored;
                    res.NotificationQueueObj = items.NotificationQueueObj;
                    res.TotalPage = items.TotalPage;
                    res.PageSize = items.PageSize;
                    res.Count = items.Count;
                    return res;
                }
                res.ReturnCode = enResponseCode.Fail;
                res.ErrorCode = enErrorCode.RecordNotFound;
                res.ReturnMsg = EnResponseMessage.NotFound;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        //khushali 18-01-2019
        #region "Request format Master"

        public List<RequestFormatViewModel> GetAllRequestFormat()
        {
            try
            {
                List<RequestFormatViewModel> list = _masterConfigurationRepository.GetAllRequestFormat();
                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public RequestFormatViewModel GetRequestFormatById(long Id)
        {
            try
            {

                RequestFormatMaster Result = _RequestFormatAPIRepository.GetById(Id);
                if (Result != null)
                {
                    var model = new RequestFormatViewModel()
                    {
                        RequestName = Result.RequestName,
                        RequestID = Result.Id,
                        ContentType = Result.ContentType,
                        MethodType = Result.MethodType,
                        RequestFormat = Result.RequestFormat,
                        RequestType=Result.RequestType,
                        Status = Result.Status
                    };
                    return model;
                }
                return  null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long AddRequestFormat(RequestFormatRequest request, long UserId)
        {
            try
            {
                var model = new RequestFormatMaster()
                {
                    CreatedBy = UserId,
                    CreatedDate = UTC_To_IST(),
                    Status = request.Status,
                    RequestFormat = request.RequestFormat,
                    RequestName = request.RequestName,
                    ContentType = request.ContentType,
                    MethodType = request.MethodType,
                    RequestType= request.RequestType,
                };
                var newModel = _RequestFormatAPIRepository.Add(model);
                return newModel.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool UpdateRequestFormat(RequestFormatRequest request, long UserId)
        {
            try
            {
                var model = _RequestFormatAPIRepository.GetById(request.RequestID);
                if (model == null)
                {
                    return false;
                }

                model.RequestName = request.RequestName;
                model.RequestFormat = request.RequestFormat;
                model.MethodType = request.MethodType;
                model.RequestType = request.RequestType;
                model.Status = request.Status;
                model.ContentType = request.ContentType;
                model.UpdatedDate = UTC_To_IST();
                model.UpdatedBy = UserId;

                _RequestFormatAPIRepository.Update(model);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool ChangeStatusRequestFormat(long id,short Status)
        {
            try
            {
                RequestFormatMaster model = _RequestFormatAPIRepository.GetById(id);
                if (model != null)
                {
                    model.Status = Status;
                    _RequestFormatAPIRepository.Update(model);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region "Communication Service"

        public List<CommunicationServiceConfigViewModel> GetCommunicationServiceConfiguration(long ServiceType)
        {
            try
            {
                List<CommunicationServiceConfigViewModel> list = _masterConfigurationRepository.GetCommunicationServiceConfiguration(ServiceType);
                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<KeyValuePair<string, int>> GetCommunicationServiceType()
        {
            try
            {
                var model = Helpers.GetEnumList<enCommunicationServiceType>();
                return model;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        
        public CommunicationServiceConfigViewModel GetCommunicationServiceConfigurationById(long APIID)
        {
            try
            {
                CommunicationServiceConfigViewModel Result = _masterConfigurationRepository.GetCommunicationServiceConfigurationById(APIID);
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long AddCommunicationServiceConfig(CommunicationServiceConfigRequest request, long UserId)
        {
            try
            {
                var model1 = new CommServiceproviderMaster() // model1
                {
                    SerproName = request.SerproName,
                    Password = request.Password,
                    CommServiceTypeID = request.ServiceTypeID,
                    UserID = request.UserID,
                    Status = request.Status,
                    CreatedBy = UserId,
                    CreatedDate = DateTime.UtcNow
                };
                var newModel1 = _CommServiceproviderMasterRepository.Add(model1);

                var model2 = new CommServiceMaster() // model -2 
                {
                    RequestID = request.RequestID,
                    CommSerproID =  newModel1.Id, // CommServiceproviderMaster - ID // modle.3
                    ServiceName = request.ServiceName,
                    ParsingDataID = request.ParsingDataID,
                    CreatedBy = UserId,
                    CreatedDate = UTC_To_IST(),
                    Status = request.Status,
                };
                var newModel2 = _CommServiceMasterRepository.Add(model2);

                var model3 = new CommAPIServiceMaster() // model -3
                {
                    CreatedBy = UserId,
                    CreatedDate = UTC_To_IST(),
                    Status = request.Status,
                    Priority = request.Priority,
                    SenderID = request.SenderID,
                    SMSSendURL = request.SendURL,
                    CommServiceID = newModel2.Id,// CommServiceMaster - ID //model.id
                    UpdatedBy = UserId,
                    SMSBalURL = ""
                };
                var newModel3 = _CommAPIServiceMasterRepository.Add(model3);
                return newModel1.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool UpdateCommunicationServiceConfig(CommunicationServiceConfigRequest request, long UserId)
        {
            try
            {
                var Model1 = _CommAPIServiceMasterRepository.GetById(request.APID);
                var Model2 = _CommServiceMasterRepository.GetById(request.ServiceID);
                var Model3 = _CommServiceproviderMasterRepository.GetById(request.SerproID);
                if (Model1 == null || Model2 == null || Model3 == null)
                {
                    return false;
                }
                Model1.SenderID = request.SenderID;
                Model1.SMSSendURL = request.SendURL;
                Model1.Status = request.Status;
                Model1.UpdatedBy = UserId;
                Model1.UpdatedDate = UTC_To_IST();
                Model1.Priority = request.Priority;

                Model2.UpdatedBy = UserId;                
                Model2.UpdatedDate = UTC_To_IST();
                Model2.ParsingDataID = request.ParsingDataID;
                Model2.RequestID = request.RequestID;
                Model2.ServiceName = request.ServiceName;
                Model2.Status = request.Status;

                Model3.UpdatedBy = UserId;
                Model3.UpdatedDate = UTC_To_IST();
                Model3.Status = request.Status;
                Model3.UserID = request.UserID;
                Model3.Password = request.Password;
                Model3.SerproName = request.SerproName;

                _CommAPIServiceMasterRepository.Update(Model1);
                _CommServiceMasterRepository.Update(Model2);
                _CommServiceproviderMasterRepository.Update(Model3);

                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool ChangeStatusCommunicationServiceConfig(long APIID,long ServiceID,long SerProID, short Status)
        {
            try
            {
                CommServiceproviderMaster Result1 = _CommServiceproviderMasterRepository.GetById(SerProID);
                CommServiceMaster Result2 = _CommServiceMasterRepository.GetById(ServiceID);
                CommAPIServiceMaster Result3 = _CommAPIServiceMasterRepository.GetById(APIID);
                if (Result1 != null)
                {
                    Result1.Status = Status;
                    _CommServiceproviderMasterRepository.Update(Result1);
                    if (Result2 != null)
                    {
                        Result2.Status = Status;
                        _CommServiceMasterRepository.Update(Result2);
                        if (Result3 != null)
                        {
                            Result3.Status = Status;
                            _CommAPIServiceMasterRepository.Update(Result3);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public EmailSMSCountData GetEmailSMSAPIManagerCountData()
        {
            EmailSMSCountData Resp = new EmailSMSCountData();
            try
            {
                var data = _masterConfigurationRepository.GetEmailSMSCount();
                if(data != null)
                {
                    Resp.Data = new EmailSMSCountResp
                    {
                        TotalEmailAPIManager = data.TotalEmailAPIManager,
                        TotalSMSAPIManager = data.TotalSMSAPIManager
                    };
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                    Resp.ReturnMsg = EnResponseMessage.ReferralNoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        public dynamic GetSuspiciousUserData(long UserID)
        {
            try
            {
                dynamic data = _masterConfigurationRepository.GetSuspiciousUserData(UserID);
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public CurrencyCummulativeData GetCurrencyCummulativeData()
        {
            try
            {
                CurrencyCummulativeData data = _masterConfigurationRepository.GetCurrencyCummulativeData();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
    }
}
