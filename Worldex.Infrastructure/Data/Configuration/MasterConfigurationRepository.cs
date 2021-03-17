using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.BackOfficeReports;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.Wallet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Worldex.Core.ViewModels.ControlPanel;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Worldex.Infrastructure.Data.Configuration
{
    public class MasterConfigurationRepository : IMasterConfigurationRepository
    {
        #region DI
        private readonly WorldexContext _dbContext;
        private readonly IConfiguration _configuration;

        public MasterConfigurationRepository(WorldexContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        #endregion

        #region Methods

        //khushali 17-01-2019
        #region "User list for android device"

        public List<DeviceUserResponse> GetDeviceList()
        {
            try
            {
                var items = (from tm in _dbContext.DeviceStore
                             join cm in _dbContext.Users
                             on tm.UserID equals cm.Id
                             select new DeviceUserResponse
                             {
                                 DeviceID = tm.DeviceID,
                                 UserID = cm.Id,
                                 FirstName = cm.FirstName,
                                 UserName = cm.UserName,
                                 LastName = cm.LastName
                             }
                             ).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        //khushali 18-01-2019
        public List<RequestFormatViewModel> GetAllRequestFormat()
        {
            try
            {

                var items = (from rm in _dbContext.RequestFormatMaster
                             select new RequestFormatViewModel
                             {
                                 RequestID = rm.Id,
                                 RequestFormat = rm.RequestFormat,
                                 RequestName = rm.RequestName,
                                 ContentType = rm.ContentType,
                                 MethodType = rm.MethodType,
                                 RequestType = rm.RequestType,
                                 Status = rm.Status
                             }
                             ).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //khushali 18-01-2019
        public List<CommunicationServiceConfigViewModel> GetCommunicationServiceConfiguration(long ServiceType)
        {
            try
            {

                var items = (from am in _dbContext.CommAPIServiceMaster
                             join sm in _dbContext.CommServiceMaster
                             on am.CommServiceID equals sm.Id
                             join sp in _dbContext.CommServiceproviderMaster
                             on sm.CommSerproID equals sp.Id
                             join cm in _dbContext.CommServiceTypeMaster
                             on sp.CommServiceTypeID equals cm.CommServiceTypeID
                             join tp in _dbContext.ThirdPartyAPIConfiguration
                             on sm.ParsingDataID equals tp.Id
                             join rf in _dbContext.RequestFormatMaster
                             on sm.RequestID equals rf.Id
                             where cm.CommServiceTypeID == ServiceType
                             select new CommunicationServiceConfigViewModel
                             {
                                 RequestID = sm.RequestID,
                                 APID = am.Id,
                                 SerproID = sp.Id,
                                 ServiceID = sm.Id,
                                 ServiceTypeID = cm.CommServiceTypeID,
                                 ParsingDataID = sm.ParsingDataID,
                                 Password = sp.Password,
                                 UserID = sp.UserID,
                                 Priority = am.Priority,
                                 SenderID = am.SenderID,
                                 SendURL = am.SMSSendURL,
                                 SerproName = sp.SerproName,
                                 ServiceName = sm.ServiceName,
                                 status = sp.Status,
                                 RequestName = rf.RequestName,
                                 ParsingDataName = tp.APIName
                             }
                             ).ToList();
                return items;
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

                var items = (from am in _dbContext.CommAPIServiceMaster
                             join sm in _dbContext.CommServiceMaster
                             on am.CommServiceID equals sm.Id
                             join sp in _dbContext.CommServiceproviderMaster
                             on sm.CommSerproID equals sp.Id
                             join cm in _dbContext.CommServiceTypeMaster
                             on sp.CommServiceTypeID equals cm.CommServiceTypeID
                             join tp in _dbContext.ThirdPartyAPIConfiguration
                             on sm.ParsingDataID equals tp.Id
                             join rf in _dbContext.RequestFormatMaster
                             on sm.RequestID equals rf.Id
                             where am.Id == APIID
                             select new CommunicationServiceConfigViewModel
                             {
                                 RequestID = sm.RequestID,
                                 APID = am.Id,
                                 SerproID = sp.Id,
                                 ServiceID = sm.Id,
                                 ServiceTypeID = cm.CommServiceTypeID,
                                 ParsingDataID = sm.ParsingDataID,
                                 Password = sp.Password,
                                 UserID = sp.UserID,
                                 Priority = am.Priority,
                                 SenderID = am.SenderID,
                                 SendURL = am.SMSSendURL,
                                 SerproName = sp.SerproName,
                                 ServiceName = sm.ServiceName,
                                 status = sp.Status,
                                 RequestName = rf.RequestName,
                                 ParsingDataName = tp.APIName
                             }
                             ).SingleOrDefault();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsoalnki 14-11-2018
        public List<TemplateResponse> GetAllTemplateMaster()
        {
            try
            {
                var items = (from tm in _dbContext.TemplateMaster
                             join cm in _dbContext.CommServiceTypeMaster
                             on tm.CommServiceTypeID equals cm.CommServiceTypeID
                             join tcm in _dbContext.TemplateCategoryMaster
                             on tm.TemplateID equals tcm.Id
                             select new TemplateResponse
                             {
                                 ID = tm.Id,
                                 Status = tm.Status,
                                 TemplateID = tm.TemplateID,
                                 TemplateType = tcm.TemplateName,
                                 CommServiceTypeID = tm.CommServiceTypeID,
                                 CommServiceType = cm.CommServiceTypeName,
                                 TemplateName = tm.TemplateName,
                                 Content = tm.Content,
                                 AdditionalInfo = tm.AdditionalInfo,
                                 ParameterInfo = GetParameters(tcm.ParameterInfo)// tm.ParameterInfo.Split(',')
                             }
                             ).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //khushali 12-01-2019
        public List<TemplateParameterInfoRes> TemplateParameterInfo(long? id = null)
        {
            try
            {
                List<TemplateParameterInfoRes> items = new List<TemplateParameterInfoRes>();
                if (id != null)
                {
                    items = (from tcm in _dbContext.TemplateCategoryMaster
                             join tm in _dbContext.TemplateMaster
                             on tcm.Id equals tm.TemplateID
                             where tm.Id == id
                             select new TemplateParameterInfoRes
                             {
                                 TemplateID = tcm.TemplateId,
                                 TemplateType = tcm.Id,
                                 IsOnOff = tcm.IsOnOff,
                                 ParameterInfo = GetParameters(tcm.ParameterInfo)// tm.ParameterInfo.Split(',')
                             }
                               ).ToList();
                }
                else
                {
                    items = (from tcm in _dbContext.TemplateCategoryMaster
                             join tm in _dbContext.TemplateMaster
                             on tcm.Id equals tm.TemplateID
                             select new TemplateParameterInfoRes
                             {
                                 TemplateID = tcm.TemplateId,
                                 TemplateType = tcm.Id,
                                 IsOnOff = tcm.IsOnOff,
                                 ParameterInfo = GetParameters(tcm.ParameterInfo)// tm.ParameterInfo.Split(',')
                             }
                            ).ToList();
                }

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ParameterInfo> GetParameters(string Parameter)
        {
            try
            {
                List<ParameterInfo> parameterInfos = new List<ParameterInfo>();
                if (Parameter != null)
                {
                    string[] ParameterDetail = Parameter.Split(',');

                    if (ParameterDetail != null && ParameterDetail.Length > 0)
                    {
                        foreach (var P in ParameterDetail)
                        {
                            string[] Param = P.Split('-');
                            if (Param != null && Param.Length == 2)
                            {
                                parameterInfos.Add(new ParameterInfo
                                {
                                    Name = Param[0],
                                    Aliasname = Param[1]
                                });
                            }
                        }
                    }
                }
                return parameterInfos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long GetTemplate(string tempName)
        {
            try
            {
                var items = Helpers.GetEnumList<EnTemplateType>();

                var id = (from p in items
                          where tempName == p.Key
                          select p.Value).FirstOrDefault();

                return Convert.ToInt64(id);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsoalnki 14-11-2018
        public TemplateResponse GetTemplateMasterById(long TemplateMasterId)
        {
            try
            {
                var template = (from tm in _dbContext.TemplateMaster
                                join cm in _dbContext.CommServiceTypeMaster
                                on tm.CommServiceTypeID equals cm.CommServiceTypeID
                                join tcm in _dbContext.TemplateCategoryMaster
                                on tm.TemplateID equals tcm.Id
                                // join q in AllowTrnType on tm.TemplateID equals q
                                where tm.Id == TemplateMasterId
                                select new TemplateResponse
                                {
                                    ID = tm.Id,
                                    Status = tm.Status,
                                    TemplateID = tm.TemplateID,
                                    TemplateType = tm.TemplateName,
                                    CommServiceTypeID = tm.CommServiceTypeID,
                                    CommServiceType = cm.CommServiceTypeName,
                                    TemplateName = tm.TemplateName,
                                    Content = tm.Content,
                                    AdditionalInfo = tm.AdditionalInfo,
                                    ParameterInfo = GetParameters(tcm.ParameterInfo)
                                }
                             ).FirstOrDefault();
                return template;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //khushali 10-01-2019
        public TemplateCategoryMasterRes GetTemplateMasterByCategory(long TemplateMasterId)
        {
            try
            {
                TemplateCategoryMasterRes TemplateCategory = new TemplateCategoryMasterRes();
                var template = (from tm in _dbContext.TemplateMaster
                                join cm in _dbContext.CommServiceTypeMaster
                                on tm.CommServiceTypeID equals cm.CommServiceTypeID
                                where tm.TemplateID == TemplateMasterId && tm.Status == 1
                                select new TemplateResponse
                                {
                                    ID = tm.Id,
                                    Status = tm.Status,
                                    TemplateID = tm.TemplateID,
                                    TemplateType = tm.TemplateName,
                                    CommServiceTypeID = tm.CommServiceTypeID,
                                    CommServiceType = cm.CommServiceTypeName,
                                    TemplateName = tm.TemplateName,
                                    Content = tm.Content,
                                    AdditionalInfo = tm.AdditionalInfo
                                }
                             ).ToList();

                var Result = (from tm in _dbContext.TemplateCategoryMaster
                              where tm.Id == TemplateMasterId
                              select new { tm.IsOnOff, tm.TemplateId }).Single();

                TemplateCategory.TemplateMasterObj = template;
                TemplateCategory.IsOnOff = Result.IsOnOff;
                TemplateCategory.TemplateID = Result.TemplateId;
                return TemplateCategory;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //khuhshali 12-01-2019
        public List<Template> ListTemplateType()
        {
            try
            {

                List<Template> TemplateCategory = new List<Template>();
                var template = (from tm in _dbContext.TemplateCategoryMaster
                                select new Template
                                {
                                    TemplateID = tm.TemplateId,
                                    IsOnOff = tm.IsOnOff,
                                    Key = tm.Id,
                                    Value = tm.TemplateName,
                                    ServiceType = tm.CommServiceTypeID
                                }
                             ).ToList();
                return template;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsoalnki 14-11-2018
        public ListMessagingQueueRes GetMessagingQueue(DateTime FromDate, DateTime ToDate, short? Status, long? MobileNo, int Page, int? PageSize)
        {
            try
            {
                ListMessagingQueueRes listMessagingQueueRes = new ListMessagingQueueRes();
                var items = (from u in _dbContext.MessagingQueue
                             where u.CreatedDate >= FromDate && u.CreatedDate <= ToDate && (Status == null || (u.Status == Status && Status != null)) && (MobileNo == null || (u.MobileNo == MobileNo && MobileNo != null))
                             orderby u.Id descending
                             select new MessagingQueueRes
                             {
                                 MessageID = u.Id, // khushali 17-01-2019 for resend function
                                 Status = u.Status,
                                 MobileNo = u.MobileNo,
                                 SMSDate = u.CreatedDate.ToString("dd-MM-yyyy h:mm:ss tt"),
                                 SMSText = u.SMSText,
                                 StrStatus = (u.Status == 0) ? "Initialize" : (u.Status == 1) ? "Success" : (u.Status == 6) ? "Pending" : (u.Status == 4) ? "Hold" : "Fail"
                             }
                             ).ToList();
                listMessagingQueueRes.Count = items.Count; // khushali 11-01-2019  list filter so first assign list count 
                var pagesize = (PageSize == null) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                var it = Convert.ToDouble(items.Count) / pagesize;
                var fl = Math.Ceiling(it);
                listMessagingQueueRes.TotalPage = Convert.ToInt64(fl);
                if (Page > 0)
                {
                    if (PageSize == null)
                    {
                        int skip = Helpers.PageSize * (Page - 1);
                        items = items.Skip(skip).Take(Helpers.PageSize).ToList();
                    }
                    else
                    {
                        int skip = Convert.ToInt32(PageSize) * (Page - 1);
                        items = items.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                    }
                }
                listMessagingQueueRes.MessagingQueueObj = items;
                listMessagingQueueRes.PageSize = (PageSize == null) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                return listMessagingQueueRes;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsoalnki 14-11-2018
        public ListEmailQueueRes GetEmailQueue(DateTime FromDate, DateTime ToDate, short? Status, string Email, int Page, int? PageSize)
        {
            try
            {
                ListEmailQueueRes listEmailQueueRes = new ListEmailQueueRes();
                var items = (from u in _dbContext.EmailQueue
                             where u.CreatedDate >= FromDate && u.CreatedDate <= ToDate && (Status == null || (u.Status == Status && Status != null)) && (Email == null || (u.Recepient == Email && Email != null))
                             orderby u.Id descending
                             select new EmailQueueRes
                             {
                                 EmailID = u.Id, // khushali 17-01-2019 for resend function
                                 Status = u.Status,
                                 RecepientEmail = u.Recepient,
                                 EmailDate = u.CreatedDate.ToString("dd-MM-yyyy h:mm:ss tt"),
                                 Body = u.Body,
                                 CC = u.CC,
                                 BCC = u.BCC,
                                 Subject = u.Subject,
                                 Attachment = u.Attachment,
                                 EmailType = u.EmailType.ToString(),
                                 StrStatus = (u.Status == 0) ? "Initialize" : (u.Status == 1) ? "Success" : (u.Status == 6) ? "Pending" : (u.Status == 4) ? "Hold" : "Fail"
                             }
                             ).ToList();

                listEmailQueueRes.Count = items.Count; // khushali 11-01-2019  list filter so first assign list count 
                var pagesize = (PageSize == null) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                var it = Convert.ToDouble(items.Count) / pagesize;
                var fl = Math.Ceiling(it);
                listEmailQueueRes.TotalPage = Convert.ToInt64(fl);

                if (Page > 0)
                {
                    if (PageSize == null)
                    {
                        int skip = Helpers.PageSize * (Page - 1);
                        items = items.Skip(skip).Take(Helpers.PageSize).ToList();
                    }
                    else
                    {
                        int skip = Convert.ToInt32(PageSize) * (Page - 1);
                        items = items.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                    }
                }
                listEmailQueueRes.EmailQueueObj = items;
                listEmailQueueRes.PageSize = (PageSize == null) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                return listEmailQueueRes;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsolanki 14-11-2018
        public List<WalletLedgerResponse> GetWalletLedger(DateTime FromDate, DateTime ToDate, long WalletId, int page, int? PageSize, ref int TotalCount)
        {

            List<WalletLedgerResponse> wl = (from w in _dbContext.WalletLedgers
                                             where w.WalletId == WalletId && w.TrnDate >= FromDate && w.TrnDate <= ToDate && w.Type == enBalanceType.AvailableBalance
                                             orderby w.TrnDate ascending
                                             select new WalletLedgerResponse
                                             {
                                                 LedgerId = w.Id,
                                                 PreBal = w.PreBal,
                                                 PostBal = w.PreBal,
                                                 Remarks = "Opening Balance",
                                                 Amount = 0,
                                                 CrAmount = 0,
                                                 DrAmount = 0,
                                                 TrnDate = w.TrnDate.ToString("dd-MM-yyyy h:mm:ss tt")
                                             }).Take(1).Union((from w in _dbContext.WalletLedgers
                                                               where w.WalletId == WalletId && w.TrnDate >= FromDate && w.TrnDate <= ToDate && w.Type == enBalanceType.AvailableBalance
                                                               orderby w.TrnDate ascending
                                                               select new WalletLedgerResponse
                                                               {
                                                                   LedgerId = w.Id,
                                                                   PreBal = w.PreBal,
                                                                   PostBal = w.PostBal,
                                                                   Remarks = w.Remarks,
                                                                   Amount = w.CrAmt > 0 ? w.CrAmt : w.DrAmt,
                                                                   CrAmount = w.CrAmt,
                                                                   DrAmount = w.DrAmt,
                                                                   TrnDate = w.TrnDate.ToString("dd-MM-yyyy h:mm:ss tt")
                                                               })).ToList();
            TotalCount = wl.Count();

            if (page > 0)
            {
                if (PageSize == null)
                {
                    int skip = Helpers.PageSize * (page - 1);
                    wl = wl.Skip(skip).Take(Helpers.PageSize).ToList();
                }
                else
                {
                    int skip = Convert.ToInt32(PageSize) * (page - 1);
                    wl = wl.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
            }
            decimal DrAmount = 0, CrAmount = 0, Amount = 0;
            wl.ForEach(e =>
            {
                Amount = e.PreBal + e.CrAmount - e.DrAmount;
                e.PostBal = Amount;
                e.PreBal = e.PostBal + e.DrAmount - e.CrAmount;

            });
            return wl;
        }

        public List<WalletLedgerResponse> GetWalletLedgerv2(DateTime FromDate, DateTime ToDate, long WalletId, int page, int? PageSize, ref int TotalCount)
        {

            List<WalletLedgerResponse> wl = (from w in _dbContext.WalletLedgers
                                             where w.WalletId == WalletId && w.TrnDate >= FromDate && w.TrnDate <= ToDate && w.Type == enBalanceType.AvailableBalance
                                             orderby w.TrnDate ascending
                                             select new WalletLedgerResponse
                                             {
                                                 //LedgerId = w.Id,
                                                 PreBal = w.PreBal,
                                                 PostBal = w.PreBal,
                                                 Remarks = "Opening Balance",
                                                 Amount = 0,
                                                 CrAmount = 0,
                                                 DrAmount = 0,
                                                 TrnDate = w.TrnDate.ToString("dd-MM-yyyy h:mm:ss tt")
                                             }).Take(1).Union((from w in _dbContext.WalletLedgers
                                                               where w.WalletId == WalletId && w.TrnDate >= FromDate && w.TrnDate <= ToDate && w.Type == enBalanceType.AvailableBalance
                                                               orderby w.TrnDate ascending
                                                               select new WalletLedgerResponse
                                                               {
                                                                   //LedgerId = w.Id,
                                                                   PreBal = w.PreBal,
                                                                   PostBal = w.PostBal,
                                                                   Remarks = w.Remarks,
                                                                   Amount = w.CrAmt > 0 ? w.CrAmt : w.DrAmt,
                                                                   CrAmount = w.CrAmt,
                                                                   DrAmount = w.DrAmt,
                                                                   TrnDate = w.TrnDate.ToString("dd-MM-yyyy h:mm:ss tt")
                                                               })).ToList();
            TotalCount = wl.Count();
            int rowIndexNumber = 0;
            wl.ForEach(i => { rowIndexNumber += 1; i.LedgerId = rowIndexNumber; });

            if (page > 0)
            {
                if (PageSize == null)
                {
                    int skip = Helpers.PageSize * (page - 1);
                    wl = wl.Skip(skip).Take(Helpers.PageSize).ToList();
                }
                else
                {
                    int skip = Convert.ToInt32(PageSize) * (page - 1);
                    wl = wl.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
            }
            decimal DrAmount = 0, CrAmount = 0, Amount = 0;
            wl.ForEach(e =>
            {
                Amount = e.PreBal + e.CrAmount - e.DrAmount;
                e.PostBal = Amount;
                e.PreBal = e.PostBal + e.DrAmount - e.CrAmount;

            });
            return wl;
        }

        public ListNotificationQueueRes GetNotificationQueue(DateTime FromDate, DateTime ToDate, short? Status, int Page, int? PageSize)
        {
            ListNotificationQueueRes listNotificationQueueRes = new ListNotificationQueueRes();
            try
            {
                var items = (from u in _dbContext.NotificationQueue
                             where u.CreatedDate >= FromDate && u.CreatedDate <= ToDate && (Status == null || (u.Status == Status && Status != null))
                             orderby u.Id descending
                             select new NotificationQueueRes
                             {
                                 NotificationID = u.Id, // khushali 17-01-2019 for resend function
                                 Status = u.Status,
                                 NotificationDate = u.CreatedDate.ToString("dd-MM-yyyy h:mm:ss tt"),
                                 StrStatus = (u.Status == 0) ? "Initialize" : (u.Status == 1) ? "Success" : (u.Status == 6) ? "Pending" : (u.Status == 4) ? "Hold" : "Fail",
                                 UserName = "-",
                                 Subject = u.Subject,
                                 Message = u.Message,
                                 DeviceID = u.DeviceID,
                                 ContentTitle = u.ContentTitle,
                                 TickerText = u.TickerText
                             }
                             ).ToList();

                listNotificationQueueRes.Count = items.Count; // khushali 11-01-2019  list filter so first assign list count 
                var pagesize = (PageSize == null) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                var it = Convert.ToDouble(items.Count) / pagesize;
                var fl = Math.Ceiling(it);
                listNotificationQueueRes.TotalPage = Convert.ToInt64(fl);

                if (Page > 0)
                {
                    if (PageSize == null)
                    {
                        int skip = Helpers.PageSize * (Page - 1);
                        items = items.Skip(skip).Take(Helpers.PageSize).ToList();
                    }
                    else
                    {
                        int skip = Convert.ToInt32(PageSize) * (Page - 1);
                        items = items.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                    }
                }
                listNotificationQueueRes.NotificationQueueObj = items;
                listNotificationQueueRes.PageSize = (PageSize == null) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                return listNotificationQueueRes;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long GetMaxPlusOneTemplate()
        {
            try
            {
                var data = _dbContext.TemplateMaster.Max(item => item.TemplateID);
                return Convert.ToInt64(data + 1);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public RptWithdrawalRes GetWithdrawalRpt(DateTime FromDate, DateTime ToDate, string CoinName, long? UserID, short? Status, int PageNo, int? PageSize, string Address, string TrnID, string TrnNo, long? OrgId, short? IsInternalTransfer)
        {
            RptWithdrawalRes Response = new RptWithdrawalRes();
            Response.PageNo = PageNo;
            try
            {
                var items = _dbContext.RptWithdrawal.FromSql(@"SELECT Isnull(StatusMsg,'') As Remarks,ISNULL(SPM.ProviderName,'') AS ProviderName,ISNULL(ResponseData,'') as TrnResponse,  ISNULL( u.ChargeCurrency,'') AS ChargeCurrency,ISNULL(u.ChargeRs,0) as ChargeRs,Isnull(o.Id,0) as OrgId,o.OrganizationName,u.SMSCode as 'CoinName' ,
                        ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink',                                                                
                        u.Status,bu.id as UserId,bu.UserName AS 'UserName',ISNULL(bu.Email,'') as Email ,
                        ISNULL( w.ToAddress,'Not Available') as 'FromAddress',ISNULL(u.TransactionAccount,'Not Available') as 'ToAddress',
                        ISNULL(w.TrnID,'Not Available') as TrnID,ISNULL(u.Id,'0') as TrnNo,u.Amount,u.CreatedDate as 'Date',
                        CASE When u.Status = 4 or u.Status = 6 Then 
                        Case When u.IsVerified = 0 Then 'ConfirmationPending' When u.IsVerified = 1 Then 'Confirm' When u.IsVerified = 9 Then 'Cancelled' End
                        Else CASE u.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 2 THEN 'OperatorFail' 
                        WHEN 3 THEN 'SystemFail'  WHEn 4 THEN 'Hold' WHEN 5 THEN 'Refunded' WHEN 6 THEN 'Pending' 
                        ELSE 'Other' END End AS 'StrStatus',u.IsVerified as 'IsVerified',u.EmailSendDate as 'EmailSendDate',
                        cast(u.Amount as varchar) as StrAmount FROM TransactionQueue u LEFT  JOIN Transactionrequest TR ON TR.id= (select max(id) from Transactionrequest where trnno= u.Id ) 
						LEFT JOIN ServiceProviderMaster SPM ON SPM.Id= u.SerproId 
                        LEFT JOIN WithdrawHistory w ON w.id=(select max(id) from WithdrawHistory where trnno= u.Id )
                        LEFT JOIN ServiceMaster s ON s.SMSCode=u.SMSCode 
                        inner JOIN BizUser bu ON bu.Id= u.MemberID 
                        inner join WalletMasters wM on wM.AccWalletID=u.DebitAccountID
                        LEFT JOIN ServiceDetail sd ON sd.ServiceId=u.serviceid 
                        LEFT join Organizationmaster o on o.id=wM.OrgID                     
                        WHERE u.TrnType = 6 and u.TrnDate between {0} and {1} and (u.Status={2} or {2}=0) and (u.SMSCode={3} or {3}='') and  (u.MemberID={4} or {4}=0) and (u.TransactionAccount={5} OR {5}='') and (w.trnid={6} or {6}='') and (u.Id={7} or {7}='') and (wm.OrgId={8} or {8}=0) and (u.IsInternalTrn={9} or {9}=999)", FromDate, ToDate, (Status == null ? 0 : Status), (CoinName == null ? "" : CoinName), (UserID == null ? 0 : UserID), (Address == null ? "" : Address), (TrnID == null ? "" : TrnID), (TrnNo == null ? "" : TrnNo), (OrgId == null ? 0 : OrgId), (IsInternalTransfer == null ? 999 : IsInternalTransfer)).ToList(); //TrnNo select inTQ table(select u.id as TrnNo) add by akshay kothari 26/12/2019 12:08pm
                        //change by jagdish on Postition [7] for filter 30-12-2019 11:53

                Response.TotalCount = items.Count();
                List<TotalBalanceResWithdraw> resList = new List<TotalBalanceResWithdraw>();
                items.Where(i => (i.Status == 1 || i.Status == 4)).GroupBy(x => x.CoinName).ToList().ForEach(i1 =>
                        {
                            TotalBalanceResWithdraw newojs = new TotalBalanceResWithdraw();
                            newojs.Currency = i1.Key;
                            newojs.TotalAmount = items.FindAll(i => (i.Status == 1 || i.Status == 4) && i.CoinName == i1.Key).Sum(i => i.Amount);
                            resList.Add(newojs);
                        }
                );
                Response.Data = resList;
                var pagesize = (PageSize == null || PageSize == 0) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                Response.PageSize = Convert.ToInt32(pagesize);
                var it = Convert.ToDouble(items.Count) / pagesize;
                var fl = Math.Ceiling(it);
                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    if (PageSize == null || PageSize == 0)
                    {
                        int skip = Helpers.PageSize * (PageNo - 1);
                        items = items.Skip(skip).Take(Helpers.PageSize).ToList();
                    }
                    else
                    {
                        int skip = Convert.ToInt32(PageSize) * (PageNo - 1);
                        items = items.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                    }
                }
                Response.Withdraw = items;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public RptWithdrawalResv2 GetWithdrawalRptv2(DateTime FromDate, DateTime ToDate, string CoinName, long? UserID, short? Status, int PageNo, int? PageSize, string Address, string TrnID, string TrnNo, long? OrgId, short? IsInternalTransfer)
        {
            RptWithdrawalResv2 Response = new RptWithdrawalResv2();
            Response.PageNo = PageNo;
            try
            {
                var items = _dbContext.RptWithdrawalv2.FromSql(@"SELECT Isnull(StatusMsg,'') As Remarks,ISNULL(SPM.ProviderName,'') AS ProviderName,ISNULL(ResponseData,'') as TrnResponse,  ISNULL( u.ChargeCurrency,'') AS ChargeCurrency,ISNULL(u.ChargeRs,0) as ChargeRs,Isnull(o.Id,0) as OrgId,o.OrganizationName,u.SMSCode as 'CoinName' ,
                        ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink',                                                                
                        u.Status,bu.id as UserId,bu.UserName AS 'UserName',ISNULL(bu.Email,'') as Email ,
                        ISNULL( w.ToAddress,'Not Available') as 'FromAddress',ISNULL(u.TransactionAccount,'Not Available') as 'ToAddress',
                        ISNULL(w.TrnID,'Not Available') as TrnID,cast(u.GUID as varchar(50)) as TrnNo,u.Amount,u.CreatedDate as 'Date',
                        CASE When u.Status = 4 or u.Status = 6 Then 
                        Case When u.IsVerified = 0 Then 'ConfirmationPending' When u.IsVerified = 1 Then 'Confirm' When u.IsVerified = 9 Then 'Cancelled' End
                        Else CASE u.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 2 THEN 'OperatorFail' 
                        WHEN 3 THEN 'SystemFail'  WHEn 4 THEN 'Hold' WHEN 5 THEN 'Refunded' WHEN 6 THEN 'Pending' 
                        ELSE 'Other' END End AS 'StrStatus',u.IsVerified as 'IsVerified',u.EmailSendDate as 'EmailSendDate',
                        cast(u.Amount as varchar) as StrAmount FROM TransactionQueue u LEFT  JOIN Transactionrequest TR ON TR.id= (select max(id) from Transactionrequest where trnno= u.Id ) 
						LEFT JOIN ServiceProviderMaster SPM ON SPM.Id= u.SerproId 
                        LEFT JOIN WithdrawHistory w ON w.id=(select max(id) from WithdrawHistory where trnno= u.Id )
                        LEFT JOIN ServiceMaster s ON s.SMSCode=u.SMSCode 
                        inner JOIN BizUser bu ON bu.Id= u.MemberID 
                        inner join WalletMasters wM on  wM.AccWalletID=u.DebitAccountID
                        LEFT JOIN ServiceDetail sd ON sd.ServiceId=u.serviceid 
                        LEFT join Organizationmaster o on o.id=wM.OrgID                     
                        WHERE u.TrnType = 6 and u.TrnDate between {0} and {1} and (u.Status={2} or {2}=0) and (u.SMSCode={3} or {3}='') and  (u.MemberID={4} or {4}=0) and (u.TransactionAccount={5} OR {5}='') and (w.trnid={6} or {6}='') and (u.GUID={7} or {7}='00000000-0000-0000-0000-000000000000') and (wm.OrgId={8} or {8}=0) and (u.IsInternalTrn={9} or {9}=999)", FromDate, ToDate, (Status == null ? 0 : Status), (CoinName == null ? "" : CoinName), (UserID == null ? 0 : UserID), (Address == null ? "" : Address), (TrnID == null ? "" : TrnID), (TrnNo == null ? Guid.Parse("00000000-0000-0000-0000-000000000000") : Guid.Parse(TrnNo)), (OrgId == null ? 0 : OrgId), (IsInternalTransfer == null ? 999 : IsInternalTransfer)).ToList();
                Response.TotalCount = items.Count();

                var pagesize = (PageSize == null || PageSize == 0) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                Response.PageSize = Convert.ToInt32(pagesize);
                var it = Convert.ToDouble(items.Count) / pagesize;
                var fl = Math.Ceiling(it);
                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    if (PageSize == null || PageSize == 0)
                    {
                        int skip = Helpers.PageSize * (PageNo - 1);
                        items = items.Skip(skip).Take(Helpers.PageSize).ToList();
                    }
                    else
                    {
                        int skip = Convert.ToInt32(PageSize) * (PageNo - 1);
                        items = items.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                    }
                }
                Response.Withdraw = items;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }


        public RptDepositionRes GetDepositionRpt(DateTime FromDate, DateTime ToDate, string CoinName, long? UserID, short? Status, int PageNo, int? PageSize, string Address, string TrnID, long? OrgId)
        {
            RptDepositionRes Response = new RptDepositionRes();
            Response.PageNo = PageNo;
            try
            {
                //ntrivedi for duplicate record showing added walletmaster join 23-03-2019
                List<HistoryObjectNew> items = new List<HistoryObjectNew>();
                if (TrnID != null)
                {
                    items = _dbContext.HistoryObjectNew.FromSql(@"SELECT  o.Id as OrgId,o.OrganizationName,D.Id AS 'TrnNo',ISNULL(D.TrnID,0) AS 'TrnId',D.SMSCode AS 'CoinName',D.Status,
                            D.StatusMsg AS 'Information',D.Amount,D.CreatedDate AS 'Date',D.Address,
                            ISNULL(D.Confirmations,0) AS 'Confirmations',
                            (CASE D.Status WHEN 0 THEN 'Processing' WHEN 1 THEN 'Success' WHEN 9 THEN 'Failed ' 
                            else 'other' END) AS 'StatusStr',ISNULL(SPM.ProviderName,'-') AS 'ProviderName',                             
                            ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink',                                                                
                            u.UserName AS 'UserName',u.id as UserId,ISNULL(u.Email,'') as Email ,cast(D.Amount as varchar) as StrAmount FROM DepositHistory D 
                            INNER JOIN ServiceMaster SM ON D.SMSCode = SM.SMSCode
                            INNER JOIN ServiceDetail SD ON SD.ServiceId = SM.Id
							LEFT JOIN ServiceProviderMaster SPM ON D.SerProID = SPM.Id
							INNER JOIN BizUser u ON u.Id= D.UserId  inner join AddressMasters AM on AM.OriginalAddress=D.Address
							inner join WalletMasters WM on WM.ID=AM.WalletID and wm.WalletTypeID=sm.WalletTypeID inner join Organizationmaster o on o.id=wm.OrgID 
                            WHERE  (D.TrnID={0} or {0}='')
                            ORDER BY D.CreatedDate DESC,D.ID DESC", (TrnID == null ? "" : TrnID)).ToList();
                }
                else
                {
                    items = _dbContext.HistoryObjectNew.FromSql(@"SELECT  o.Id as OrgId,o.OrganizationName,D.Id AS 'TrnNo',ISNULL(D.TrnID,0) AS 'TrnId',D.SMSCode AS 'CoinName',D.Status,
                            D.StatusMsg AS 'Information',D.Amount,D.CreatedDate AS 'Date',D.Address,
                            ISNULL(D.Confirmations,0) AS 'Confirmations',
                            (CASE D.Status WHEN 0 THEN 'Processing' WHEN 1 THEN 'Success' WHEN 9 THEN 'Failed ' 
                            else 'other' END) AS 'StatusStr',ISNULL(SPM.ProviderName,'-') AS 'ProviderName',                             
                            ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink',                                                                
                            u.UserName AS 'UserName',u.id as UserId,ISNULL(u.Email,'') as Email ,cast(D.Amount as varchar) as StrAmount FROM DepositHistory D 
                            INNER JOIN ServiceMaster SM ON D.SMSCode = SM.SMSCode
                            INNER JOIN ServiceDetail SD ON SD.ServiceId = SM.Id
							LEFT JOIN ServiceProviderMaster SPM ON D.SerProID = SPM.Id
							INNER JOIN BizUser u ON u.Id= D.UserId  inner join AddressMasters AM on AM.OriginalAddress=D.Address
							inner join WalletMasters WM on WM.ID=AM.WalletID and wm.WalletTypeID=sm.WalletTypeID inner join Organizationmaster o on o.id=wm.OrgID 
                            WHERE (D.UserId={0} OR {0}=0) AND (D.CreatedDate BETWEEN {1} AND {2}) AND (D.Status={3} OR {3}=999) 
                            AND ({4}='' OR D.SMSCode={4}) AND (D.Address={5} OR {5}='') AND (D.TrnID={6} or {6}='') and (wm.OrgId={7} or {7}=0)
                            ORDER BY D.CreatedDate DESC,D.ID DESC", (UserID == null ? 0 : UserID), FromDate, ToDate, (Status == null ? 999 : Status), (CoinName == null ? "" : CoinName), (Address == null ? "" : Address), (TrnID == null ? "" : TrnID), (OrgId == null ? 0 : OrgId)).ToList();
                }

                Response.TotalCount = items.Count();
                List<TotalBalanceResWithdraw> resList = new List<TotalBalanceResWithdraw>();
                items.Where(i => i.Status == 1).GroupBy(x => x.CoinName).ToList().ForEach(i1 =>
               {
                   TotalBalanceResWithdraw newojs = new TotalBalanceResWithdraw();
                   newojs.Currency = i1.Key;
                   newojs.TotalAmount = items.FindAll(i => (i.Status == 1) && i.CoinName == i1.Key).Sum(i => i.Amount);
                   resList.Add(newojs);
               }
                );
                Response.Data = resList;
                var pagesize = (PageSize == null || PageSize == 0) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                var it = Convert.ToDouble(items.Count()) / pagesize;
                var fl = Math.Ceiling(it);
                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    if (PageSize == null || PageSize == 0)
                    {
                        int skip = Helpers.PageSize * (PageNo - 1);
                        items = items.Skip(skip).Take(Helpers.PageSize).ToList();
                    }
                    else
                    {
                        int skip = Convert.ToInt32(PageSize) * (PageNo - 1);
                        items = items.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                    }
                }
                Response.Deposit = items;
                Response.PageSize = (PageSize == null) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public RptDepositionResv2 GetDepositionRptv2(DateTime FromDate, DateTime ToDate, string CoinName, long? UserID, short? Status, int PageNo, int? PageSize, string Address, string TrnID, long? OrgId)
        {
            RptDepositionResv2 Response = new RptDepositionResv2();
            Response.PageNo = PageNo;
            try
            {
                //ntrivedi for duplicate record showing added walletmaster join 23-03-2019
                List<HistoryObjectNewv2> items = new List<HistoryObjectNewv2>();
                if (TrnID != null)
                {
                    items = _dbContext.HistoryObjectNewv2.FromSql(@"SELECT  o.Id as OrgId,o.OrganizationName,D.GUID AS 'TrnNo',ISNULL(D.TrnID,0) AS 'TrnId',D.SMSCode AS 'CoinName',D.Status,
                            D.StatusMsg AS 'Information',D.Amount,D.CreatedDate AS 'Date',D.Address,
                            ISNULL(D.Confirmations,0) AS 'Confirmations',
                            (CASE D.Status WHEN 0 THEN 'Processing' WHEN 1 THEN 'Success' WHEN 9 THEN 'Failed ' 
                            else 'other' END) AS 'StatusStr',                            
                            ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink',                                                                
                            u.UserName AS 'UserName',u.id as UserId,ISNULL(u.Email,'') as Email ,cast(D.Amount as varchar) as StrAmount FROM DepositHistory D 
                            INNER JOIN ServiceMaster SM ON D.SMSCode = SM.SMSCode
                            INNER JOIN ServiceDetail SD ON SD.ServiceId = SM.Id
							
							INNER JOIN BizUser u ON u.Id= D.UserId  inner join AddressMasters AM on AM.OriginalAddress=D.Address
							inner join WalletMasters WM on WM.ID=AM.WalletID and wm.WalletTypeID=sm.WalletTypeID inner join Organizationmaster o on o.id=wm.OrgID 
                            WHERE  (D.TrnID={0} or {0}='')
                            ORDER BY D.CreatedDate DESC,D.ID DESC", (TrnID == null ? "" : TrnID)).ToList();
                }
                else
                {
                    items = _dbContext.HistoryObjectNewv2.FromSql(@"SELECT  o.Id as OrgId,o.OrganizationName,D.GUID AS 'TrnNo',ISNULL(D.TrnID,0) AS 'TrnId',D.SMSCode AS 'CoinName',D.Status,
                            D.StatusMsg AS 'Information',D.Amount,D.CreatedDate AS 'Date',D.Address,
                            ISNULL(D.Confirmations,0) AS 'Confirmations',
                            (CASE D.Status WHEN 0 THEN 'Processing' WHEN 1 THEN 'Success' WHEN 9 THEN 'Failed ' 
                            else 'other' END) AS 'StatusStr',                            
                            ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink',                                                                
                            u.UserName AS 'UserName',u.id as UserId,ISNULL(u.Email,'') as Email ,cast(D.Amount as varchar) as StrAmount FROM DepositHistory D 
                            INNER JOIN ServiceMaster SM ON D.SMSCode = SM.SMSCode
                            INNER JOIN ServiceDetail SD ON SD.ServiceId = SM.Id
							
							INNER JOIN BizUser u ON u.Id= D.UserId  inner join AddressMasters AM on AM.OriginalAddress=D.Address
							inner join WalletMasters WM on WM.ID=AM.WalletID and wm.WalletTypeID=sm.WalletTypeID inner join Organizationmaster o on o.id=wm.OrgID 
                            WHERE (D.UserId={0} OR {0}=0) AND D.CreatedDate BETWEEN {1} AND {2} AND (D.Status={3} OR {3}=999) 
                            AND ({4}='' OR D.SMSCode={4}) AND (D.Address={5} OR {5}='') AND (D.TrnID={6} or {6}='' and (wm.OrgId={7} or {7}=0))
                            ORDER BY D.CreatedDate DESC,D.ID DESC", (UserID == null ? 0 : UserID), FromDate, ToDate, (Status == null ? 999 : Status), (CoinName == null ? "" : CoinName), (Address == null ? "" : Address), (TrnID == null ? "" : TrnID), (OrgId == null ? 0 : OrgId)).ToList();
                }

                Response.TotalCount = items.Count();
                var pagesize = (PageSize == null || PageSize == 0) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                var it = Convert.ToDouble(items.Count()) / pagesize;
                var fl = Math.Ceiling(it);
                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    if (PageSize == null || PageSize == 0)
                    {
                        int skip = Helpers.PageSize * (PageNo - 1);
                        items = items.Skip(skip).Take(Helpers.PageSize).ToList();
                    }
                    else
                    {
                        int skip = Convert.ToInt32(PageSize) * (PageNo - 1);
                        items = items.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                    }
                }
                Response.Deposit = items;
                Response.PageSize = (PageSize == null) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }

        public List<EmailLists> GetEmailLists()
        {
            try
            {
                List<EmailLists> lists = new List<EmailLists>();
                lists = _dbContext.emailLists.FromSql("SELECT distinct(Email) AS emailid FROM Bizuser WHERE EmailConfirmed=1 AND Email<> ''").ToList();
                return lists;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public EmailSMSCountResp GetEmailSMSCount()
        {
            try
            {
                EmailSMSCountResp lists = new EmailSMSCountResp();
                lists = _dbContext.EmailSMSCount.FromSql("SELECT " +
                    "(SELECT CAST(COUNT(ID) AS bigint) FROM CommServiceproviderMaster WHERE CommServiceTypeID = 2 AND Status <> 9) AS TotalEmailAPIManager," +
                    "(SELECT CAST(COUNT(ID) AS bigint) FROM CommServiceproviderMaster WHERE CommServiceTypeID = 1 AND Status <> 9) AS TotalSMSAPIManager").FirstOrDefault();
                return lists;
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
            SuspiciousUserActivityResp Resp = new SuspiciousUserActivityResp();
            try
            {
                SuspiciousUserPersonalInfo PersonalInfo = new SuspiciousUserPersonalInfo();
                //dynamic PersonalInfo = new ExpandoObject();
                PersonalInfo = _dbContext.SuspiPersDetails.FromSql("select BZ.UserName,BZ.FirstName + ' ' + BZ.LastName as 'Name',BZ.NormalizedEmail as 'Email',BZ.Mobile as 'MobileNo',CASE BZ.IsEnabled When 1 Then 'Enabled' ELSE 'Disabled' END as 'MemberStatus',CASE UAD.Status WHEN 1 Then 'KYC' ELSE 'Non KYC' END as 'KYCStatus',CASE UAD.Status When 1 Then cast(cast(UAD.CreatedDate as datetime)as varchar) ELSE '' End as 'KYCDate',CASE isnull(cast(cast(Email.CreatedDate as datetime)as varchar),'') When '' Then 'UnConfirmed' ELSE 'Confirmed' END as 'EmailConfirmStatus',CASE isnull(cast(cast(Email.CreatedDate as datetime)as varchar),'') When '' Then '' ELSE CONVERT(VARCHAR,Email.CreatedDate) END as 'EmailConfirmDate',IsNull(BZ2.FirstName + ' ' + BZ2.LastName,'') as 'ReferredBy',isnull(cast(cast(BZ.CreatedDate as datetime)as varchar),'') as 'CreatedDate'  From BizUser BZ Left Join ReferralUser RU On RU.ReferUserId=BZ.Id And RU.ReferUserId<>RU.UserId Left Join BizUser BZ2 On BZ2.Id=RU.UserId Outer Apply (Select top 1 CreatedDate,Status From KYCUserInfoMapping where UserId=BZ.Id Order By Id Desc) as UAD Outer Apply (Select top 1 CreatedDate From UserActivityDetails Where UserId=BZ.Id And ActivityCode=5 Order By Id Desc) Email WHere BZ.Id={0}", UserID).FirstOrDefault();
                Resp.SuspiciousUserPersonalInfo = PersonalInfo;

                SqlParameter[] param1 = new SqlParameter[]{
                    new SqlParameter("@UserID",SqlDbType.BigInt, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, UserID) ,
                    new SqlParameter("@FromDate",SqlDbType.VarChar, -1, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, string.Empty) ,
                    new SqlParameter("@Todate",SqlDbType.VarChar, -1, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, string.Empty),
                    new SqlParameter("@Count",SqlDbType.BigInt, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, _configuration["SuspiciousReportCount"]) ,
                    new SqlParameter("@CurrencyID",SqlDbType.BigInt, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, 0) ,
                    new SqlParameter("@ActionType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, (long)EnUserActivity.Users_Who_Reset_Password)
                    };

                List<UserActivityReportActConfirmedEmailResp.UserActivityReportConfEmailResp> UserList = _dbContext.UserActivityReportConfEmailResp.FromSql("Sp_GetUserActivityData @UserID,@FromDate,@Todate,@Count,@CurrencyID,@ActionType", param1).ToList();
                Resp.PasswordChangeData = UserList;

                List<UserActivityReportActConfirmedEmailResp.UserActivityReportConfEmailResp> ReferredUsers = new List<UserActivityReportActConfirmedEmailResp.UserActivityReportConfEmailResp>();
                ReferredUsers = _dbContext.UserActivityReportConfEmailResp.FromSql("Select top ({1}) BZ.Id,BZ.FirstName + ' ' + BZ.LastName as 'Name',BZ.NormalizedEmail as 'Email',BZ.Mobile as 'MobileNo',isnull(cast(cast(BZ.CreatedDate as datetime)as varchar),'''') as 'Date' from ReferralUser RU inner Join BizUser BZ On BZ.Id=RU.ReferUserId Where RU.UserId={0} And RU.Status=1", UserID, Convert.ToInt16(_configuration["SuspiciousReportCount"])).ToList();
                Resp.RefferedUsers = ReferredUsers;

                List<UserActivityReportConfEmailResp2> DeviceChange = new List<UserActivityReportConfEmailResp2>();
                DeviceChange = _dbContext.UserActivityReportConfEmailResp2.FromSql("Select top ({1}) DM.Id,BZ.FirstName + ' ' + BZ.LastName as 'Name',BZ.NormalizedEmail as 'Email',BZ.Mobile as 'MobileNo',isnull(cast(cast(DM.CreatedDate as datetime)as varchar),'''') as 'Date',ISNULL(DM.IPAddress,'') as 'IPAddress',ISNULL(DM.Location,'') as 'Location',DM.Device from DeviceMaster DM inner Join BizUser BZ On BZ.Id=DM.UserId Where DM.UserId={0} And DM.Status=1 Order By DM.Id Desc", UserID, Convert.ToInt16(_configuration["SuspiciousReportCount"])).ToList();
                Resp.DeviceChange = DeviceChange;

                Resp.ErrorCode = enErrorCode.Success;//GetSuspiciousUserDataSuccess
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = "Success";

                return Resp;
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
                CurrencyCummulativeData lists = new CurrencyCummulativeData();
                lists.Data = _dbContext.CummulativeData.FromSql(@"Select WTM.WalletTypeName as 'Currency',IsNULL(SUM(DH.Amount),0) as 'DepositeQty',ISNULL(SUM(WH.Amount),0) as 'WithdrawQty',ISNULL(SUM(BAL.Balance),0) as 'UserQty',ISNULL(SUM(ADM.Balance),0) as 'AdminQty',ISNULL(SUM(ARBUSR.Balance),0) as 'ArbitrageUserQty',ISNULL(SUM(MARUSR.Balance),0) as 'MarginUserQty',ISNULL(SUM(MARADM.Balance),0) as 'MarginAdminQty'
                                from WalletTypeMasters WTM 
                                OUTER APPLY (Select SUM(DH.Amount) as Amount From DepositHistory DH Inner Join BizUser BZ On BZ.Id=DH.UserId Inner Join BizUserTypeMapping TM ON TM.UserId=BZ.Id And TM.UserType<>0 Where DH.SMSCode=WTM.WalletTypeName AND DH.Status=1) DH
                                OUTER APPLY (Select SUM(WH.Amount) as Amount From WithdrawHistory WH Inner Join BizUser BZ On BZ.Id=WH.UserId Inner Join BizUserTypeMapping TM ON TM.UserId=BZ.Id And TM.UserType<>0 Where WH.SMSCode=WTM.WalletTypeName And WH.Status=1) WH
                                OUTER APPLY (Select SUM(WM.Balance) as Balance From WalletMasters WM Inner Join BizUser BZ On BZ.Id=WM.UserId Inner Join BizUserTypeMapping TM ON TM.UserId=BZ.Id And TM.UserType<>0 Where WM.WalletTypeID=WTM.Id And WM.Status=1) BAL
                                OUTER APPLY (Select SUM(WM.Balance) as Balance From WalletMasters WM Inner Join BizUser BZ On BZ.Id=WM.UserId Inner Join BizUserTypeMapping TM ON TM.UserId=BZ.Id And TM.UserType=0 Where WM.WalletTypeID=WTM.Id And WM.Status=1) ADM
                                OUTER APPLY (Select SUM(WM.Balance) as Balance From ArbitrageWalletMaster WM Inner Join BizUser BZ On BZ.Id=WM.UserId Inner Join BizUserTypeMapping TM ON TM.UserId=BZ.Id And TM.UserType<>0 Where WM.WalletTypeID=WTM.Id And WM.Status=1) ARBUSR
                                OUTER APPLY (Select SUM(WM.Balance) as Balance From MarginWalletMaster WM Inner Join BizUser BZ On BZ.Id=WM.UserId Inner Join BizUserTypeMapping TM ON TM.UserId=BZ.Id And TM.UserType<>0 Where WM.WalletTypeID=WTM.Id And WM.Status=1) MARUSR
                                OUTER APPLY (Select SUM(WM.Balance) as Balance From MarginWalletMaster WM Inner Join BizUser BZ On BZ.Id=WM.UserId Inner Join BizUserTypeMapping TM ON TM.UserId=BZ.Id And TM.UserType=0 Where WM.WalletTypeID=WTM.Id And WM.Status=1) MARADM
                                Where WTM.Status=1
                                Group By WTM.WalletTypeName
                                Order By WTM.WalletTypeName").ToList();
                lists.ErrorCode = enErrorCode.Success;//GetCurrencyCummulativeDataSuccess
                lists.ReturnCode = enResponseCode.Success;
                lists.ReturnMsg = "Success";
                return lists;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
    }
}
