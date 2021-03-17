using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.FeedConfiguration;
using Worldex.Core.ViewModels.APIConfiguration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Data.Configuration
{
    public class APIConfigurationRepository : IAPIConfigurationRepository
    {
        private readonly WorldexContext _dbContext;
        private readonly ILogger<APIConfigurationRepository> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public APIConfigurationRepository(WorldexContext dbContext, ILogger<APIConfigurationRepository> logger, 
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
        }
         
        #region APIPlan
        //public List<ViewAPIPlanDetailResponseInfo> ViewAPIPlanDetail()
        //{
        //    List<ViewAPIPlanDetailResponseInfo> _Res = new List<ViewAPIPlanDetailResponseInfo>();
        //    IQueryable<ViewAPIPlanDetailQryResponse> Result;
        //    String Qry = "";
        //    try
        //    {
        //        Qry = "SELECT PM.Id,PM.PlanName,PM.Price,PM.PlanValidity,PM.Charge,PM.Status as PlanStatus,PD.ConcurrentEndPoints,PD.HistoricalDataMonth,PD.Id as PlanDetailID,"+
        //                "PD.MaxOrderPerSec,PD.MaxPerDay,PD.MaxPerMinute,PD.MaxPerMonth,PD.MaxRecPerRequest,PD.MaxReqSize,PD.MaxResSize,PD.Status as PlanDetailStatus "+
        //                "FROM APIPlanMaster PM INNER JOIN APIPlanDetail PD ON PM.Id = PD.APIPlanMasterID WHERE PD.Status = 1 and PM.status = 1";
        //        Result = _dbContext.viewAPIPlans.FromSql(Qry);

        //        var PlanMethod = _dbContext.PlanMethodRes.FromSql(
        //                        @"select PC.ID,PC.RestMethodID,PC.APIPlanMasterID,RM.MethodName,RM.IsFullAccess,RM.IsReadOnly from APIPlanMethodConfiguration PC 
        //                        INNER JOIN RestMethods RM ON PC.RestMethodID = RM.ID WHERE RM.Status = 1 OR PC.Status = 1").ToList();

        //        foreach(var obj in Result.ToList())
        //        {
        //            var ReadOnly = PlanMethod.Where(e => e.IsReadOnly == 1 && e.APIPlanMasterID == obj.Id).Select(e=>e.MethodName).ToList();
        //            var FullAccecc = PlanMethod.Where(e => e.IsFullAccess == 1 && e.APIPlanMasterID == obj.Id).Select(e => e.MethodName).ToList();
        //            //_Res.Add(new ViewAPIPlanDetailResponseInfo() {
        //            //    Id = obj.Id,
        //            //    Charge =obj.Charge,
        //            //    Price=obj.Price,
        //            //    ConcurrentEndPoints=obj.ConcurrentEndPoints,
        //            //    HistoricalDataMonth=obj.HistoricalDataMonth,
        //            //    MaxOrderPerSec=obj.MaxOrderPerSec,
        //            //    MaxPerDay=obj.MaxPerDay,
        //            //    MaxPerMinute=obj.MaxPerMinute,
        //            //    MaxPerMonth=obj.MaxPerMonth,
        //            //    MaxRecPerRequest=obj.MaxRecPerRequest,
        //            //    MaxReqSize=obj.MaxReqSize,
        //            //    MaxResSize=obj.MaxResSize,
        //            //    PlanDetailID=obj.PlanDetailID,
        //            //    PlanDetailStatus=obj.PlanDetailStatus,
        //            //    PlanName=obj.PlanName,
        //            //    PlanStatus=obj.PlanStatus,
        //            //    PlanValidity=obj.PlanValidity,
        //            //    FullAccessAPI = FullAccecc,
        //            //    ReadOnlyAPI = ReadOnly
        //            //});
        //        }
        //        return _Res;
               
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //        throw ex;
        //    }
        //}
        //public List<APIPlanMasterResponseInfo> ViewAPIPlanDetailBackOffice()
        //{
        //    List<APIPlanMasterResponseInfo> _Res = new List<APIPlanMasterResponseInfo>();
        //    IQueryable<ViewAPIPlanDetailQryResponseV2> Result;
        //    String Qry = "";
        //    try
        //    {
        //        Qry = "SELECT PM.Id,PM.PlanName,PM.Price,PM.Priority,PM.PlanValidity,PM.Charge,PM.Status as PlanStatus,PD.ConcurrentEndPoints,PD.HistoricalDataMonth,PD.Id as PlanDetailID," +
        //                "PD.MaxOrderPerSec,PD.MaxPerDay,PD.MaxPerMinute,PD.MaxPerMonth,PD.MaxRecPerRequest,PD.MaxReqSize,PD.MaxResSize,PD.Status as PlanDetailStatus,PD.CreatedDate,PD.CreatedBy " +
        //                "FROM APIPlanMaster PM INNER JOIN APIPlanDetail PD ON PM.Id = PD.APIPlanMasterID ";
        //        Result = _dbContext.viewAPIPlansV2.FromSql(Qry);

        //        var PlanMethod = _dbContext.PlanMethodRes.FromSql(
        //                        @"select PC.ID,PC.RestMethodID,PC.APIPlanMasterID,RM.MethodName,RM.IsFullAccess,RM.IsReadOnly from APIPlanMethodConfiguration PC 
        //                        INNER JOIN RestMethods RM ON PC.RestMethodID = RM.ID ").ToList();

        //        foreach (var obj in Result.ToList())
        //        {
        //            var ReadOnly = PlanMethod.Where(e => e.IsReadOnly == 1 && e.APIPlanMasterID == obj.Id).Select(e => e.MethodName).ToList();
        //            var FullAccecc = PlanMethod.Where(e => e.IsFullAccess == 1 && e.APIPlanMasterID == obj.Id).Select(e => e.MethodName).ToList();
        //            //_Res.Add(new APIPlanMasterResponseInfo()
        //            //{
        //            //    ID = obj.Id,
        //            //    Charge = obj.Charge,
        //            //    Price = obj.Price,
        //            //    ConcurrentEndPoints = obj.ConcurrentEndPoints,
        //            //    HistoricalDataMonth = obj.HistoricalDataMonth,
        //            //    MaxOrderPerSec = obj.MaxOrderPerSec,
        //            //    MaxPerDay = obj.MaxPerDay,
        //            //    MaxPerMinute = obj.MaxPerMinute,
        //            //    MaxPerMonth = obj.MaxPerMonth,
        //            //    MaxRecPerRequest = obj.MaxRecPerRequest,
        //            //    MaxReqSize = obj.MaxReqSize,
        //            //    MaxResSize = obj.MaxResSize,
        //            //    //PlanDetailID = obj.PlanDetailID,
        //            //    //PlanDetailStatus = obj.PlanDetailStatus,
        //            //    PlanName = obj.PlanName,
        //            //    Status = obj.PlanStatus,
        //            //    Priority=obj.Priority,
        //            //    CreatedDate=obj.CreatedDate,
        //            //    PlanValidity = obj.PlanValidity,
        //            //    //FullAccessAPI = FullAccecc,
        //            //    //ReadOnlyAPI = ReadOnly,
        //            //    CreatedBy=obj.CreatedBy
        //            //});
        //        }
        //        return _Res;

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //        throw ex;
        //    }
        //}
        public List<PlanMethodsQryResponse> GetPlanMethods(long LimitID=0)
        {
            try
            {
                var PlanMethod = _dbContext.PlanMethodRes.FromSql(
                                @"select PC.ID,PC.RestMethodID,PC.APIPlanMasterID,RM.MethodName,RM.IsFullAccess,RM.IsReadOnly from APIPlanMethodConfiguration PC 
                                INNER JOIN APIMethods RM ON PC.RestMethodID = RM.ID WHERE RM.Status = 1 AND PC.Status = 1 AND CustomeLimitID={0} ", LimitID).ToList();
                return PlanMethod;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public void CancelAllPendingPlan(long PlanID, long UserID, short Status)
        {
            try
            {
                if(Status==0)
                    _dbContext.Database.ExecuteSqlCommand("UPDATE UserSubscribeAPIPlan SET status=2,UpdatedBy=999,UpdatedDate=dbo.GetISTDate(),Perticuler='Disable Plan' WHERE status=0 AND APIPlanMasterID={0}",  PlanID);
               else if (Status == 1)
                    _dbContext.Database.ExecuteSqlCommand("UPDATE UserSubscribeAPIPlan SET status=0,UpdatedBy=999,UpdatedDate=dbo.GetISTDate(),Perticuler='Enable Plan' WHERE status=2 AND APIPlanMasterID={0} AND UpdatedBy=999 AND ActivationDate>dbo.GetISTDate()",  PlanID);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public ViewActivePlanDetailInfo ViewUserActivePlan(long UserID)
        {
            try
            {
                //GetCustomLimitIDQry Res = _dbContext.GetCustomLimitIDQry.FromSql(
                //    @"SELECT top 1 PH.CustomeLimitId FROM UserSubscribeAPIPlan PH WHERE PH.UserID={0} AND PH.ActivationDate < dbo.GetISTDate() AND PH.status in(1,9) ORDER BY PH.ActivationDate desc", UserID).FirstOrDefault();
                //if (Res == null)
                //    return null;
                //if (Res.CustomeLimitId == 0)
                //{
                    var Result = _dbContext.viewActivePlanDetails.FromSql(
                      @"SELECT TOP 1 PH.Id AS SubscribeID,PM.PlanName,PM.Price AS PlanPrice,PM.Status AS PlanStatus,PM.Priority,PM.Id as PlanID,PM.MaxPerDay,PM.MaxPerMinute,PM.MaxPerMonth,PM.MaxOrderPerSec,
                    PM.MaxRecPerRequest,PM.WhitelistedEndPoints,PM.ConcurrentEndPoints,PM.MaxReqSize,PM.MaxResSize,PM.HistoricalDataMonth,
                    PH.TotalAmt AS PaidAmt,PH.PaymentStatus,PH.Status AS SubScribeStatus,PH.CreatedDate AS RequestedDate,PH.ActivationDate,PH.IsAutoRenew,PH.RenewStatus,PH.RenewDate,
                    PM.PlanValidity,PM.PlanValidityType,PM.IsPlanRecursive,PH.ExpiryDate,PH.Price,PH.Charge,PH.TotalAmt,PH.CustomeLimitId,PH.RenewDays
                    FROM UserSubscribeAPIPlan PH INNER JOIN APIPlanMaster PM ON PH.APIPlanMasterID=PM.Id 
                    WHERE PH.UserID={0} AND PH.ActivationDate < dbo.GetISTDate() AND PH.status in(1,9) ORDER BY PH.ActivationDate desc", UserID);
                    return Result.ToList().FirstOrDefault();
                //}
                //else
                //{
                //    var Result = _dbContext.viewActivePlanDetails.FromSql(
                //      @"SELECT top 1 PH.Id AS SubscribeID,PM.PlanName,PH.Price AS PlanPrice,PM.Status AS PlanStatus,PM.Priority,PM.Id as PlanID,PD.MaxPerDay,PD.MaxPerMinute,PD.MaxPerMonth,PD.MaxOrderPerSec,
                //        PD.MaxRecPerRequest,PD.WhitelistedEndPoints,PD.ConcurrentEndPoints,PD.MaxReqSize,PD.MaxResSize,PD.HistoricalDataMonth,
                //        PH.TotalAmt AS PaidAmt,PH.PaymentStatus,PH.Status AS SubScribeStatus,PH.CreatedDate AS RequestedDate,PH.ActivationDate,PH.IsAutoRenew,PH.RenewStatus,PH.RenewDate,
                //        PM.PlanValidity,PM.PlanValidityType,PM.IsPlanRecursive,PH.ExpiryDate,PH.Price,PH.Charge,PH.TotalAmt,PH.CustomeLimitId,PH.RenewDays
                //        FROM UserSubscribeAPIPlan PH INNER JOIN APIPlanMaster PM ON PH.APIPlanMasterID=PM.Id INNER JOIN APIPlanDetail PD ON PD.Id ={1}
                //        WHERE PH.UserID={0} AND PH.ActivationDate < dbo.GetISTDate() AND PH.status in(1,9) ORDER BY PH.ActivationDate desc", UserID, Res.CustomeLimitId);
                //    return Result.ToList().FirstOrDefault();
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public GetAutoRenewDetailInfo GetAutoRenewDetail(long UserID)
        {
            GetAutoRenewDetailInfo _Res = new GetAutoRenewDetailInfo();
            //GetActivationDate Result2 = new GetActivationDate();
            try
            {
                var Result = _dbContext.AutoRenewDetailQryRes.FromSql(
                    @"SELECT PH.Price,PH.Charge,PH.TotalAmt,PH.RenewDays,PH.Status,PM.PlanName,PH.ExpiryDate,PH.Id AS	SubscribeID,PH.NextAutoRenewId,PM.PlanValidity,PM.PlanValidityType
                        FROM UserSubscribeAPIPlan PH INNER JOIN APIPlanMaster PM ON PH.APIPlanMasterID=PM.Id WHERE PH.UserID={0} AND PH.Status=1 AND PH.IsAutoRenew=1", UserID).SingleOrDefault();
                if (Result == null)
                    return null;
                //Result2 = _dbContext.GetActivationDate.FromSql(
                //    @"select PH.ActivationDate,PH.RenewDays FROM UserSubscribeAPIPlan PH WHERE PH.id={0} ", Result.nextAutoRenewID).SingleOrDefault();
                //if (Result2 == null)
                //    return null;
                _Res.Amount = Result.Price;
                _Res.Fees = Result.Charge;
                _Res.TotalAmt = Result.TotalAmt;
                _Res.Days = Result.RenewDays;
                _Res.RenewID = Convert.ToInt64(Result.nextAutoRenewID);
                _Res.SubscribeID = Result.SubscribeID;
                _Res.PlanName = Result.PlanName;
                _Res.Status = Result.Status;
                _Res.Validity = Result.PlanValidity;
                _Res.PlanValidityType = Result.PlanValidityType;
                _Res.ExpiryDate = Result.ExpiryDate;
                _Res.NextRenewDate = Result.ExpiryDate.AddDays(-Result.RenewDays);

                return _Res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public void SetDisablePlanMethodsStatus(long PlanID, long UserID, long limitId = 0)
        {
            try
            {
                _dbContext.Database.ExecuteSqlCommand("UPDATE APIPlanmethodConfiguration set Status=0 where APIPlanMasterID={0} AND CustomeLimitID={1}", PlanID, limitId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public void SetDefaultConfigurationMethod(long PlanID, long LimitID)
        {
            try
            {
                _dbContext.Database.ExecuteSqlCommand("update APIPlanMethodConfiguration SET Status=1,UpdatedBy=999,UpdatedDate=dbo.GetISTDate()  where CustomeLimitId={0} and  RestMethodID in (select RestMethodID from APIPlanMethodConfiguration where APIPlanMasterID={1} AND CustomeLimitId=0 AND Status=1)", LimitID, PlanID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public void DisableAPIMethodsConfiguration(long MethodId)
        {
            try
            {
                _dbContext.Database.ExecuteSqlCommand("UPDATE APIMethodConfiguration SET Status=9,UpdatedBy=999,UpdatedDate=dbo.GetISTDate() where ParentID={0}", MethodId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public List<APIMethodConfigListQryRes> GetAPIMethodConfigListBK()
        {
            IQueryable<APIMethodConfigListQryRes> Result;
            try
            {
                Result = _dbContext.GetAPIMethodConfigList.FromSql(
                                @"SELECT AM.MethodID,AM.ParentID,AM.MethodType,RM.MethodName FROM APIMethodConfiguration AM INNER JOIN RestMethods RM ON AM.Methodid=RM.id WHERE MethodType=1 AND AM.status<>9
                                UNION ALL SELECT AM.MethodID,AM.ParentID,AM.MethodType,SM.MethodName FROM APIMethodConfiguration AM INNER JOIN SocketMethods SM on AM.Methodid=SM.id WHERE MethodType=2 AND AM.status<>9");

                return Result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public List<GetKeyWiseIPQryRes> GetKeyWiseIPList(long UserID, long PlanID)
        {
            IQueryable<GetKeyWiseIPQryRes> Result;
            try
            {
                Result = _dbContext.GetKeyWiseIPQryRes.FromSql(
                                @"SELECT KW.APIKeyID,WE.IPAddress,WE.IPType,WE.AliasName,WE.CreatedDate,WE.ID FROM APIKeyWhitelistIPConfig KW INNER JOIN WhiteListIPEndPoint WE ON KW.IPId=WE.Id
                                WHERE WE.Status=1 AND WE.UserID={0} AND WE.APIPlanID={1}", UserID, PlanID);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        #endregion

        #region Enable-disable key and ip
        public void DisablePlanService(long PlanID,long UserID,long SubscribeID,string Keys) //Upgrade-downgrade
        {
            try
            {
                _dbContext.Database.ExecuteSqlCommand("UPDATE UserAPIKeyDetails SET Status=9,UpdatedBy=997,UpdatedDate=dbo.GetISTDate() where userID={0} AND APIPlanMasterID={1} AND Status=1", UserID,PlanID);
                _dbContext.Database.ExecuteSqlCommand("Update WhiteListIPEndPoint SET Status=9,UpdatedBy=997,UpdatedDate=dbo.GetISTDate() where userID={0} AND apiplanid={1} AND Status=1", UserID, PlanID);
                if (!string.IsNullOrEmpty(Keys))
                    _dbContext.Database.ExecuteSqlCommand("UPDATE APIKeyWhitelistIPConfig SET Status=9,UpdatedBy=997,UpdatedDate=dbo.GetISTDate() where APIKeyID in ("+Keys+")");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public void ExpireOldRenewPlan(long PlanID, long UserID)
        {
            try
            {
                _dbContext.Database.ExecuteSqlCommand("UPDATE UserSubscribeAPIPlan set status=2,Updatedby=999,UpdatedDate=dbo.GetISTDate(),Perticuler='Cancel pending Renew Request' where UserID={0} and status=0 and APIPlanMasterid={1}", UserID, PlanID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public void DisableAllPlanAPIKey(long PlanID, long UserID) //Disable plan
        {
            try
            {
                _dbContext.Database.ExecuteSqlCommand("Update UserAPIKeyDetails set Status=9, UpdatedBy=999, UpdatedDate=dbo.GetISTDate() where APIPlanMasterID={0} AND Status=1", PlanID);
                _dbContext.Database.ExecuteSqlCommand("Update WhiteListIPEndPoint set Status=9, UpdatedBy=999, UpdatedDate=dbo.GetISTDate() where apiplanid={0} AND Status=1", PlanID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public void EnableAllPlanAPIKey(long PlanID, long UserID)//Enable plan
        {
            try
            {
                _dbContext.Database.ExecuteSqlCommand("Update UserAPIKeyDetails set Status=1, UpdatedBy=999, UpdatedDate=dbo.GetISTDate() where APIPlanMasterID={0} AND UpdatedBy=999", PlanID);
                _dbContext.Database.ExecuteSqlCommand("Update WhiteListIPEndPoint set Status=1, UpdatedBy=999, UpdatedDate=dbo.GetISTDate() where apiplanid={0} AND UpdatedBy=999", PlanID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public void DisableWhiteListIPConfigurationIPwise(long IPID, long UserID) //delete API Key
        {
            try
            {
                _dbContext.Database.ExecuteSqlCommand("Update APIKeyWhitelistIPConfig set Status=9, UpdatedBy={1}, UpdatedDate=dbo.GetISTDate() where IPId={0}", IPID, UserID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public void DisableWhiteListIPConfigurationKeywise(long APIKeyID, long UserID)//delete ip
        {
            try
            {
                _dbContext.Database.ExecuteSqlCommand("Update APIKeyWhitelistIPConfig set Status=9, UpdatedBy={1}, UpdatedDate=dbo.GetISTDate() where APIKeyID={0}", APIKeyID, UserID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public void ExpireAllPlanAPIKey(long PlanID, long UserID) //Expire plan
        {
            try
            {
                _dbContext.Database.ExecuteSqlCommand("Update UserAPIKeyDetails set Status=9, UpdatedBy=998, UpdatedDate=dbo.GetISTDate() where APIPlanMasterID={0} AND Status=1 AND userID={1}", PlanID, UserID);
                _dbContext.Database.ExecuteSqlCommand("Update WhiteListIPEndPoint set Status=9, UpdatedBy=998, UpdatedDate=dbo.GetISTDate() where apiplanid={0} AND Status=1 AND userID={1}", PlanID, UserID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        #endregion

        #region Backoffice method
        public List<UserAPIPlanHistoryQryRes> GetUserAPISubscribeHistory(long UserID,long? PlanID,short? PaymentStatus)
        {
            List<UserAPIPlanHistoryQryRes> Result = new List<UserAPIPlanHistoryQryRes>();
            string Qry = "";
            string Condition = "";
            try
            {
                if (PlanID != null)
                    Condition = " AND PM.Id="+ Convert.ToInt64(PlanID);
                if(PaymentStatus != null)
                    Condition += " AND PH.PaymentStatus=" + Convert.ToInt64(PaymentStatus);

                Qry = "SELECT PM.PlanName,PH.Status,PH.Perticuler,PH.ActivationDate,PH.ExpiryDate,PH.Price,PH.Charge,PH.TotalAmt,PH.PaymentStatus,PH.RenewStatus FROM UserSubscribeAPIPlan PH " +
                    "INNER JOIN APIPlanMaster PM ON PH.APIPlanMasterID = PM.Id WHERE PH.UserID ={0} AND PH.Status in (1, 0, 2, 9) "+Condition;
                Result = _dbContext.UserAPIPlanHistoryQryRes.FromSql(Qry,UserID).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
        public APIPlanUserCountResponse GetAPIPlanUserCount(long Pagesize, long PageNo, string FromDate, string ToDate,long? UserId,long? Status,long? PlanID)
        {
            APIPlanUserCountResponse _Res = new APIPlanUserCountResponse();
            string Condition = "";
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {
                if (UserId != null)
                    Condition += " AND PH.UserID="+UserId;
                if(Status!=null)
                    Condition += " AND PH.Status=" + Status;
                if (PlanID != null)
                    Condition += " AND PH.APIPlanMasterID=" + PlanID;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND PH.ActivationDate Between {0} AND {1} ";
                }

                string Qry = "SELECT Count(Distinct(PH.UserID)) AS Users,PH.APIPlanMasterID,PM.PlanName,count(PH.Id) AS PurchasePlan,sum(PH.TotalAmt) AS Earnings"+
                            " FROM UserSubscribeAPIPlan PH INNER JOIN APIPlanMaster PM ON PH.APIPlanMasterID = PM.Id WHERE PH.Status in (1, 9, 0) "+ Condition + " Group by PH.APIPlanMasterID,PM.PlanName" ;
                IQueryable<APIPlanUserCountResponseInfo> Result = _dbContext.APIPlanUserCountQryRes.FromSql(Qry , fDate, tDate);
                if (Result.ToList().Count == 0)
                {
                    _Res.Response = Result.ToList();
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                _Res.TotalCount = Result.Count();
                _Res.Response = Result.Skip(Convert.ToInt16(Pagesize * PageNo)).Take(Convert.ToInt16(Pagesize)).ToList();
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
        public UserSubscribeHistoryBKResponse GetUserSubscribeHistoryBK(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId, long? Status, long? PlanID)
        {
            UserSubscribeHistoryBKResponse _Res = new UserSubscribeHistoryBKResponse();
            List<UserSubscribeHistoryBKInfo> History = new List<UserSubscribeHistoryBKInfo>();
            List<UserSubscribeHistoryBKQryRes> UserInfo = new List<UserSubscribeHistoryBKQryRes>();
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            string Condition = "";
            try
            {
                if (UserId != null)
                    Condition += " AND PH.UserID=" + UserId;
                if (Status != null && Status!=0)
                    Condition += " AND PH.Status=" + Status;
                if (PlanID != null)
                    Condition += " AND PH.APIPlanMasterID=" + PlanID;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND PH.ActivationDate Between {0} AND {1} ";
                }
                string Qry = "SELECT PM.Id,PH.UserID,PM.PlanName,PH.Price,PH.Charge,PH.TotalAmt,PH.Status,PH.Perticuler, PH.IsAutoRenew, "+
                              " PM.PlanValidity,PM.PlanValidityType,PH.CreatedDate AS RequestedDate,PH.ActivationDate,PH.ExpiryDate,PH.PaymentStatus,PH.RenewStatus ," +
                              " PM.WhitelistedEndPoints,PM.ConcurrentEndPoints,(select top 1 AddMaxLimit from PublicAPIKeyPolicy where status = 1) as KeyTotCount"+
                              " FROM UserSubscribeAPIPlan PH INNER JOIN APIPlanMaster PM ON PH.APIPlanMasterID = PM.Id "+
                              " WHERE PH.Status in (1, 0, 2, 9) "+ Condition;

                UserInfo = _dbContext.userSubscribeHistories.FromSql(
                                Qry, fDate, tDate).ToList();
                if(UserInfo.Count==0)
                {
                    _Res.Response = History;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                _Res.TotalCount = UserInfo.Count();
                List<IPWhitelistCountQryRes> iPWhitelists = new List<IPWhitelistCountQryRes>();
                iPWhitelists = _dbContext.iPWhitelistCountQryRes.FromSql(@"select id,UserId,IPAddress,IPType,APIKeyDetailsID,Status,APIplanID from WhiteListIPEndPoint Where Status=1").ToList();

                List<APIKeyCountQryRes> aPIKeyCounts = new List<APIKeyCountQryRes>();
                aPIKeyCounts = _dbContext.aPIKeyCountQryRes.FromSql(@"select id,UserID,APIPlanMasterID,IPAccess from UserAPIKeyDetails Where Status=1").ToList();

                foreach(var obj in UserInfo.Skip(Convert.ToInt16(Pagesize * PageNo)).Take(Convert.ToInt16(Pagesize)))
                {
                    int WhitelistCount=0, KeyCount=0,ConcurrentCount=0;
                    KeyCount = aPIKeyCounts.Where(e => e.UserId == obj.UserID && e.APIPlanMasterID == obj.Id).Count();
                    WhitelistCount = iPWhitelists.Where(e => e.UserId == obj.UserID && e.APIplanID == obj.Id && e.IPType==1).Count();
                    ConcurrentCount = iPWhitelists.Where(e => e.UserId == obj.UserID && e.APIplanID == obj.Id && e.IPType == 2).Count();
                    
                    History.Add(new UserSubscribeHistoryBKInfo() {
                        ActivationDate=(DateTime)obj.ActivationDate,
                        Charge=obj.Charge,
                        ConcurrentEndPoints=obj.ConcurrentEndPoints,
                        ConcurrentEndPointsCount=ConcurrentCount,
                        ExpiryDate = (DateTime)obj.ExpiryDate,
                        IsAutoRenew=obj.IsAutoRenew,
                        KeyCount= KeyCount,
                        KeyTotCount=obj.KeyTotCount,
                        PaymentStatus=obj.PaymentStatus,
                        Perticuler=obj.Perticuler,
                        PlanName=obj.PlanName,
                        PlanValidity=obj.PlanValidity,
                        PlanValidityType=obj.PlanValidityType,
                        Price=obj.Price,
                        RenewStatus=obj.RenewStatus,
                        RequestedDate=obj.RequestedDate,
                        Status=obj.Status,
                        TotalAmt=obj.TotalAmt,
                        UserID=obj.UserID,
                        ConcurrentCount= ConcurrentCount,
                        WhitelistedEndPoints =obj.WhitelistedEndPoints,
                        WhitelistedEndPointsCount= WhitelistCount

                    });
                }
                _Res.PageCount = _Res.TotalCount / Pagesize;
                _Res.Response = History;
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
        public ViewAPIPlanConfigHistoryResponse ViewAPIPlanConfigHistoryBK(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId,long? PlanID)
        {
            ViewAPIPlanConfigHistoryResponse _Res = new ViewAPIPlanConfigHistoryResponse();
            List<ViewAPIPlanConfigHistoryInfo> historyInfos = new List<ViewAPIPlanConfigHistoryInfo>();
            List<APIPlanConfigurationHistory> list = new List<APIPlanConfigurationHistory>();
            List<APIPlanMethodConfigHistoryRes> MethodList;
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            string Condition = " 1=1 ";
            string Condition2 = " 1=1 ";
            try
            {
                if (UserId != null)
                    Condition += " AND LastModifyBy=" + UserId;
                if (PlanID != null)
                    Condition += " AND PlanID=" + PlanID;
                if (PlanID != null)
                    Condition2 += " AND APIPlanMasterID=" + PlanID;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND LastModifyDate Between {0} AND {1} ";
                }
                string Qry = "select *from APIPlanConfigurationHistory where " + Condition;

                list = _dbContext.APIPlanConfigurationHistory.FromSql(
                                Qry, fDate, tDate).ToList();

               
                if (list.Count==0)
                {
                    _Res.Response = historyInfos;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }

                var str2 = "select CreatedDate,CreatedBy,UpdatedBy,UpdatedDate,status,APIPlanMasterID,APIPlanHistoryID,RestMethodID  from APIPlanMethodConfigurationHistory where " + Condition2;
                var Methods = _dbContext.APIPlanMethodConfigHistoryRes.FromSql(str2).ToList();
                _Res.TotalCount = list.Count();
                _Res.PageCount = _Res.TotalCount / Pagesize;
                foreach (var obj in list.Skip(Convert.ToInt16(Pagesize * PageNo)).Take(Convert.ToInt16(Pagesize)))
                {
                    MethodList = new List<APIPlanMethodConfigHistoryRes>();
                    MethodList = Methods.Where(e => e.APIPlanHistoryID == obj.Id).ToList();
                    historyInfos.Add(new ViewAPIPlanConfigHistoryInfo()
                    {
                        Charge = obj.Charge,
                        ConcurrentEndPoints = obj.ConcurrentEndPoints,
                        CreatedBy = obj.CreatedBy,
                        CreatedDate = obj.CreatedDate,
                        CreatedIPAddress = obj.CreatedIPAddress,
                        HistoricalDataMonth = obj.HistoricalDataMonth,
                        Id = obj.Id,
                        IsPlanRecursive = obj.IsPlanRecursive,
                        LastModifyBy = obj.LastModifyBy,
                        LastModifyDate = obj.LastModifyDate,
                        MaxOrderPerSec = obj.MaxOrderPerSec,
                        MaxPerDay = obj.MaxPerDay,
                        MaxPerMinute = obj.MaxPerMinute,
                        MaxPerMonth = obj.MaxPerMonth,
                        MaxRecPerRequest = obj.MaxRecPerRequest,
                        MaxReqSize = obj.MaxReqSize,
                        MaxResSize = obj.MaxResSize,
                        ModifyDetails = obj.ModifyDetails,
                        PlanDesc = obj.PlanDesc,
                        PlanID = obj.PlanID,
                        PlanName = obj.PlanName,
                        PlanValidity = obj.PlanValidity,
                        PlanValidityType = obj.PlanValidityType,
                        Price = obj.Price,
                        Priority = obj.Priority,
                        Status = obj.Status,
                        UpdatedBy = obj.UpdatedBy,
                        UpdatedDate = obj.UpdatedDate,
                        WhitelistedEndPoints = obj.WhitelistedEndPoints,
                        MethodList= MethodList
                    });
                }
               
                _Res.Response = historyInfos;
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
        public ViewPublicAPIKeysResponse ViewPublicAPIKeysBK(long Pagesize, long PageNo, string FromDate, string ToDate, long? UserId, long? Status, long? PlanID)
        {
            ViewPublicAPIKeysResponse _Res = new ViewPublicAPIKeysResponse();
            List<ViewPublicAPIKeysInfo> KeyRes = new List<ViewPublicAPIKeysInfo>();
            List<APIKeysDetailsInfo> KeyDetails=new List<APIKeysDetailsInfo>();
            List<ViewPublicAPIKeysInfoQryRes> KeyResult = new List<ViewPublicAPIKeysInfoQryRes>();
            List<APIKeysDetailsInfoQryRes> iplist = new List<APIKeysDetailsInfoQryRes>();
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            string Condition = " 1=1 ";
            try
            {
                if (UserId != null)
                    Condition += " AND KD.UserID=" + UserId;
                if (Status != null)
                    Condition += " AND KD.Status=" + Status;
                if (PlanID != null)
                    Condition += " AND KD.APIPlanMasterID=" + PlanID;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND KD.CreatedDate Between {0} AND {1} ";
                }

                var Qry = "SELECT PM.PlanName,KD.Id,KD.UserID,KD.AliasName,KD.APIPermission,KD.CreatedDate,KD.UpdatedDate,KD.IPAccess ,KD.Status,KD.APIPlanMasterID FROM UserAPIKeyDetails KD inner join APIPlanMaster PM on KD.APIPlanMasterID=PM.Id WHERE " + Condition;
                KeyResult = _dbContext.publicAPIKeysInfoQryRes.FromSql(Qry,fDate,tDate).ToList();
                if (KeyResult.Count == 0)
                {
                    _Res.Response = KeyRes;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                _Res.TotalCount = KeyResult.Count();
                iplist = _dbContext.keysDetailsInfoQryRes.FromSql("select WE.AliasName,WE.IPAddress,WE.IPType,WE.CreatedDate,KG.APIKeyID from APIKeyWhitelistIPConfig KG INNER JOIN WhiteListIPEndPoint WE ON KG.IPId=WE.id").ToList();
                foreach (var obj in KeyResult.Skip(Convert.ToInt16(Pagesize * PageNo)).Take(Convert.ToInt16(Pagesize)))
                {
                    var list = iplist.Where(e => e.APIKeyID == obj.Id).ToList();
                    if(list.Count>0)
                    {
                        KeyDetails = new List<APIKeysDetailsInfo>();
                        foreach (var ipDetail in list)
                        {
                            KeyDetails.Add(new APIKeysDetailsInfo()
                            {
                                AliasName=ipDetail.AliasName,
                                CreatedDate=ipDetail.CreatedDate,
                                IPAddress=ipDetail.IPAddress,
                                IPType=ipDetail.IPType
                            });
                        }
                    }
                    KeyRes.Add(new ViewPublicAPIKeysInfo()
                    {
                        AliasName=obj.AliasName,
                        APIPermission=obj.APIPermission,
                        CreatedDate=obj.CreatedDate,
                        Id=obj.Id,
                        IPAccess=obj.IPAccess,
                        KeyDetails=KeyDetails,
                        Status=obj.Status,
                        UpdatedDate=obj.UpdatedDate,
                        UserID=obj.UserID,
                        PlanName=obj.PlanName
                    });
                }
                _Res.Response = KeyRes;
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
        #endregion

        #region DashbordMethod
        public APIRequestStatisticsCountResponse APIRequestStatisticsCount()
        {
            APIRequestStatisticsCountResponse _Res = new APIRequestStatisticsCountResponse();
            List<PlanUsersQryRes> PlanUsers = new List<PlanUsersQryRes>();
            List<PlanUsersQryRes> PurchasePlan = new List<PlanUsersQryRes>();
            List<APIReqStatusCodeCountQryRes> StatusCode = new List<APIReqStatusCodeCountQryRes>();
            List<APIReqBrowsweWiseQryRes> Browse = new List<APIReqBrowsweWiseQryRes>();
            List<HTTPErrorCodeQryRes> ErrorCodeList = new List<HTTPErrorCodeQryRes>();
            try
            {
                APIRequestStatisticsCountQryRes Result = _dbContext.APIRequestStatisticsCountQryRes.FromSql(
                            @"SELECT (select cast(sum(SuccessCount)as bigint) from APIReqResStatistics) as SuccessCount,(select cast(sum(FaliureCount)as bigint) from APIReqResStatistics)as FaliureCount,
                            (select cast(count(Distinct(UserID))as bigint) from UserSubscribeAPIPlan) as APIUsers, (select cast(count(Distinct(UserID))as bigint) from UserSubscribeAPIPlan Where CreatedDate > dbo.GetISTDate()) as RegisterToday").FirstOrDefault();

                PlanUsers = _dbContext.PlanUsersQryRes.FromSql(
                            @"select cast(count(Distinct(UP.UserID))as bigint) as APIUsers,UP.APIPlanMasterID,PM.PlanName from UserSubscribeAPIPlan  UP INNER JOIN APIPlanMaster PM ON UP.APIPlanMasterID=PM.Id
                            where UP.status in (0, 1, 9) Group by UP.APIPlanMasterID, PM.PlanName").ToList();
                PurchasePlan = _dbContext.PlanUsersQryRes.FromSql(
                            @"select cast(count((UP.UserID))as bigint) as APIUsers,UP.APIPlanMasterID,PM.PlanName from UserSubscribeAPIPlan  UP INNER JOIN APIPlanMaster PM ON UP.APIPlanMasterID=PM.Id
                            where UP.status in(0,1,9) Group by UP.APIPlanMasterID ,PM.PlanName").ToList();
                StatusCode = _dbContext.APIReqStatusCodeCountQryRes.FromSql(
                            @"select cast(count(id)as bigint) as ReqCount,HTTPStatusCode from PublicAPIReqResLog  group by HTTPStatusCode").ToList();
                Browse = _dbContext.APIReqBrowsweWiseQryRes.FromSql(
                            @"select cast(count(id)as bigint) as ReqCount,Browser from PublicAPIReqResLog  group by Browser").ToList();
                ErrorCodeList = _dbContext.HTTPErrorCodeQryRes.FromSql(
                            @"SELECT TOP 5 HTTPErrorCode,Path,Host,MethodType,CreatedDate FROM PublicAPIReqResLog ORDER BY CreatedDate DESC").ToList();
                _Res.SuccessCount = Result.SuccessCount;
                _Res.FaliureCount = Result.FaliureCount;
                _Res.APIUsers = Result.APIUsers;
                _Res.RegisterToday = Result.RegisterToday;
                _Res.PurchasePlan = PurchasePlan;
                _Res.PlanUsers = PlanUsers;
                _Res.StatusCode = StatusCode;
                _Res.Browser = Browse;
                _Res.ErrorCodeList = ErrorCodeList;
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

        public UserWiseAPIReqCounResponse UserWiseAPIReqCount(long Pagesize, long PageNo,short status)
        {
            UserWiseAPIReqCounResponse _Res = new UserWiseAPIReqCounResponse();
            List<UserWiseAPIReqCounResInfo> Response = new List<UserWiseAPIReqCounResInfo>();
            List<UserWiseAPIReqCountQryRes> Result = null;
            try
            {
                if (status == 0)//Fail
                    Result = _dbContext.UserWiseAPIReqCountQryRes.FromSql("SELECT cast(sum(FaliureCount)as bigint) as ReqCount,UserID FROM APIReqResStatistics GROUP BY UserID").ToList();
                else if (status == 1)//success
                    Result = _dbContext.UserWiseAPIReqCountQryRes.FromSql("SELECT cast(sum(SuccessCount)as bigint) as ReqCount,UserID FROM APIReqResStatistics GROUP BY UserID").ToList();

                if (Result.Count == 0)
                {
                    _Res.Response = Response;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                _Res.TotalCount = Result.Count();
                _Res.PageCount = _Res.TotalCount / Pagesize;

                foreach(var obj in Result.Skip(Convert.ToInt16(Pagesize * PageNo)).Take(Convert.ToInt16(Pagesize)))
                {
                    var user = _userManager.FindByIdAsync(obj.UserID.ToString()).Result;
                    Response.Add(new UserWiseAPIReqCounResInfo() {
                        ReqCount=obj.ReqCount,
                        UserID=obj.UserID,
                        EmailID=user.Email,
                        UserName=user.UserName
                    });
                }
                _Res.Response = Response;
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

        public FrequentUseAPIRespons GetFrequentUseAPI(long Pagesize,string FromDate, string ToDate)
        {
            FrequentUseAPIRespons _Res = new FrequentUseAPIRespons();
            List<FrequentUseAPIQryRes> APIList = new List<FrequentUseAPIQryRes>();
            List<FrequentUseAPIResponsInfo> APIRespons = new List<FrequentUseAPIResponsInfo>();
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            string Condition = " 1=1 ";
            try
            {
                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND CreatedDate Between {0} AND {1} ";
                }
                else
                {
                    Condition += " AND CreatedDate > CAST(dbo.GetISTDate() AS DATE) ";
                }

                var Qry = "SELECT TOP "+ Pagesize +" CAST(count(id) AS bigint) AS ReqCount,[path] FROM PublicAPIReqResLog WHERE  "+ Condition+" GROUP BY [Path] ORDER BY ReqCount DESC";
                APIList = _dbContext.FrequentUseAPIQryRes.FromSql(Qry,fDate,ToDate).ToList();
                if(APIList.Count==0)
                {
                    _Res.Response = APIRespons;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                foreach(var obj in APIList)
                {
                    var Str = "select top 1 CreatedDate,Path,HTTPErrorCode,HTTPStatusCode,Status,Host from PublicAPIReqResLog where path='"+obj.Path+"' order by CreatedDate desc";
                    var res = _dbContext.FrequentUseAPIResponsInfo.FromSql(Str).FirstOrDefault();
                    if (res != null)
                        APIRespons.Add(res);
                }
                _Res.Response = APIRespons;
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

        public MostActiveIPAddressResponse MostActiveIPAddress(long Pagesize, string FromDate, string ToDate)
        {
            MostActiveIPAddressResponse _Res = new MostActiveIPAddressResponse();
            List<MostActiveIPCountQryRes> IPCount = new List<MostActiveIPCountQryRes>();
            List<MostActiveIPDetailsInfo> IPDetails = new List<MostActiveIPDetailsInfo>();
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            MostActiveIPDetailsInfo info;
            string Condition = " 1=1 ";
            try
            {
                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND CreatedDate Between {0} AND {1} ";
                }
                else
                {
                    Condition += " AND CreatedDate > CAST(dbo.GetISTDate() AS DATE) ";
                }

                IPCount = _dbContext.MostActiveIPCountQryRes.FromSql("SELECT TOP "+Pagesize+" CAST(count(id) AS bigint) AS ReqCount,IPAddress,CreatedBy FROM PublicAPIReqResLog WHERE "+Condition+" GROUP BY IPAddress,CreatedBy ORDER BY ReqCount DESC",FromDate,ToDate).ToList();
                if(IPCount.Count==0)
                {
                    _Res.Response = IPDetails;
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                foreach(var obj in IPCount)
                {
                    MostActiveIPDetailsQryRes Details = _dbContext.MostActiveIPDetailsQryRes.FromSql("select top 1 CreatedDate,Path,Host,IPAddress,WhitelistIP,CreatedBy from PublicAPIReqResLog where IPAddress='"+ obj.IPAddress+"' order by CreatedDate desc").FirstOrDefault();
                    if(Details!=null)
                    {
                        var user = _userManager.FindByIdAsync(obj.CreatedBy.ToString()).Result;
                        info = new MostActiveIPDetailsInfo();
                        info.CreatedBy = Details.CreatedBy;
                        info.CreatedDate = Details.CreatedDate;
                        info.EmailID = user.Email;
                        info.Host = Details.Host;
                        info.IPAddress = Details.IPAddress;
                        info.Path = Details.Path;
                        info.UserName = user.UserName;
                        info.WhitelistIP = Details.WhitelistIP;
                        IPDetails.Add(info);
                    }
                }
                _Res.Response = IPDetails;
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

        public APIPlanConfigurationCountResponse APIPlanConfigurationCount()
        {
            APIPlanConfigurationCountResponse _Res = new APIPlanConfigurationCountResponse();
            try
            {
                IQueryable<APIPlanConfigurationCountQryRes> Result = null;
                Result = _dbContext.APIPlanConfigurationCountQryRes.FromSql(@"SELECT 
                                (SELECT CAST(COUNT(ID) AS bigint) FROM APIPlanMaster WHERE status <> 9) AS APIPlanCount,
                                (SELECT CAST(COUNT(ID) AS bigint) FROM UserSubscribeAPIPlan WHERE Status in (1, 0, 2, 9)) AS SubscriptionCount,
                                (SELECT CAST(COUNT(ID) AS bigint) FROM APIPlanConfigurationHistory) AS PlanConfigHistoryCount,
                                (SELECT CAST(COUNT(ID) AS bigint) FROM UserAPIKeyDetails WHERE status <> 9)AS KeyCount,
                                (SELECT CAST(COUNT(ID) AS bigint) FROM PublicAPIKeyPolicy)AS APIKeyPolicyCount,
                                (SELECT CAST(COUNT(ID) AS bigint) FROM PublicAPIReqResLog WHERE IPAddress IS NOT NULL) AS IPWiseRequestCount,
                                (SELECT CAST(COUNT(ID) AS bigint) FROM APIMethods WHERE status <> 9)AS APIMethodCount");
                _Res.Response = Result.FirstOrDefault();
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

        public HTTPErrorsReportResponse GetHTTPErrorReport(long Pagesize, long PageNo, string FromDate, string ToDate,long? ErrorCode)
        {
            HTTPErrorsReportResponse _Res = new HTTPErrorsReportResponse();
            string Condition = " ";
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            List<HTTPErrorCodeQryRes> Result = null;
            try
            {
                if(ErrorCode!=null)
                    Condition += " AND HTTPErrorCode="+ErrorCode;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND CreatedDate Between {0} AND {1} ";
                }

                Result = _dbContext.HTTPErrorCodeQryRes.FromSql("SELECT HTTPErrorCode,Path,Host,MethodType,CreatedDate FROM PublicAPIReqResLog WHERE HTTPErrorCode!=200 "+ Condition + " ORDER BY CreatedDate DESC", fDate, tDate).ToList();

                if(Result.Count==0)
                {
                    _Res.Response = Result.ToList();
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }

                _Res.TotalCount = Result.Count();
                _Res.Response = Result.Skip(Convert.ToInt16(Pagesize * PageNo)).Take(Convert.ToInt16(Pagesize)).ToList();
                _Res.PageCount = _Res.TotalCount / Pagesize;
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

        public MostActiveIPWiseReportResponse GetIPAddressWiseReport(long Pagesize, long PageNo, string FromDate, string ToDate, string IPAddress,long MemberID)
        {
            MostActiveIPWiseReportResponse _Res = new MostActiveIPWiseReportResponse();
            List<MostActiveIPWiseReportQryRes> Result = null;
            List<MostActiveIPWiseReportQryRes> Result2 = new List<MostActiveIPWiseReportQryRes>();
            string Condition = " ";
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            try
            {
                if (!string.IsNullOrEmpty( IPAddress))
                    Condition += " AND IPAddress='" + IPAddress + "'";

                if(MemberID!=0)
                    Condition += " AND CreatedBy="+MemberID;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND CreatedDate Between {0} AND {1} ";
                }

                Result = _dbContext.MostActiveIPWiseReportQryRes.FromSql(@"SELECT CreatedDate,Path,Host,IPAddress,WhitelistIP,CreatedBy AS MemberID,'' AS UserName,'' AS EmailID 
                    from PublicAPIReqResLog WHERE IPAddress is not NULL "+ Condition + " order by CreatedDate desc",fDate, tDate).ToList();

                if(Result.Count==0)
                {
                    _Res.Response = Result.ToList();
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                _Res.TotalCount = Result.Count();
                _Res.PageCount = _Res.TotalCount / Pagesize;
                //_Res.Response = Result.Skip(Convert.ToInt16(Pagesize * PageNo)).Take(Convert.ToInt16(Pagesize)).ToList();
                foreach(var obj in Result.Skip(Convert.ToInt16(Pagesize * PageNo)).Take(Convert.ToInt16(Pagesize)).ToList())
                {
                    var user = _userManager.FindByIdAsync(obj.MemberID.ToString()).Result;
                    Result2.Add(new MostActiveIPWiseReportQryRes() {
                        CreatedDate = obj.CreatedDate,
                        Host = obj.Host,
                        IPAddress = obj.IPAddress,
                        Path=obj.Path,
                        UserName= user.UserName,
                        WhitelistIP=obj.WhitelistIP,
                        MemberID = obj.MemberID,
                        EmailID = user.Email
                    });
                }
                _Res.Response = Result2;
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

        public FrequentUseAPIWiseReportResponse FrequentUseAPIReport(long Pagesize, long PageNo, string FromDate, string ToDate,long MemberID)
        {
            string Condition = " ";
            DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
            List<FrequentUseAPIWiseReportQryRes> Result = null;
            List<FrequentUseAPIWiseReportQryRes> Result2 = new List<FrequentUseAPIWiseReportQryRes>();
            FrequentUseAPIWiseReportResponse _Res = new FrequentUseAPIWiseReportResponse();
            try
            {
                if (MemberID != 0)
                    Condition += " AND CreatedBy=" + MemberID;

                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Condition += " AND CreatedDate Between {0} AND {1} ";
                }
                var str = "select CreatedDate,Path,HTTPErrorCode,HTTPStatusCode,Status,Host,CreatedBy AS MemberID,'' AS UserName,'' AS EmailID from PublicAPIReqResLog Where CreatedBy!=0 " + Condition + " order by CreatedDate desc";
                Result = _dbContext.FrequentUseAPIWiseReportQryRes.FromSql(@str, fDate, tDate).ToList();
                if (Result.Count == 0)
                {
                    _Res.Response = Result.ToList();
                    _Res.ErrorCode = enErrorCode.NoDataFound;
                    _Res.ReturnCode = enResponseCode.Fail;
                    _Res.ReturnMsg = "NoDataFound";
                    return _Res;
                }
                _Res.TotalCount = Result.Count();
                _Res.PageCount = _Res.TotalCount / Pagesize;
                foreach(var obj in Result.Skip(Convert.ToInt16(Pagesize * PageNo)).Take(Convert.ToInt16(Pagesize)).ToList())
                {
                    var user = _userManager.FindByIdAsync(obj.MemberID.ToString()).Result;
                    Result2.Add(new FrequentUseAPIWiseReportQryRes()
                    {
                        CreatedDate = obj.CreatedDate,
                        Host = obj.Host,
                        Path = obj.Path,
                        UserName = user.UserName,
                        MemberID = obj.MemberID,
                        EmailID = user.Email,
                        HTTPErrorCode=obj.HTTPErrorCode,
                        HTTPStatusCode=obj.HTTPStatusCode,
                        Status=obj.Status
                    });
                }
                _Res.Response = Result2;
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
        #endregion
    }
}
