using Worldex.Core.Entities.Complaint;
using Worldex.Core.Entities.Organization;
using Worldex.Core.Interfaces.Activity_Log;
using Worldex.Core.Interfaces.Organization;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.BackOfficeComplain;
using Worldex.Core.ViewModels.Organization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.ActivityLog
{
    public class ActivityLogRegisterService : IActivityRegisterData
    {
        private readonly ICustomRepository<Typemaster> _IcommonRepository;
        //private readonly ICustomExtendedRepository<HostURLMaster> _serviceProviderMasterRepo;
        private readonly WorldexContext _dbContext;
        private readonly ICustomExtendedRepository<ActivityType_Master> _IcustomExtendedRepository;
        private readonly ICustomExtendedRepository<ActivityRegister> _IActivityRepository;
        private readonly IActivityMasterConfiguration _IactivityMasterConfiguration;

        public ActivityLogRegisterService(ICustomRepository<Typemaster> IcommonRepository, WorldexContext dbContext, ICustomExtendedRepository<ActivityType_Master> IcustomExtendedRepository,
            ICustomExtendedRepository<ActivityRegister> IActivityRepository,
            IActivityMasterConfiguration IactivityMasterConfiguration)
        {
            _IcommonRepository = IcommonRepository;
            _dbContext = dbContext;
            _IcustomExtendedRepository = IcustomExtendedRepository;
            _IActivityRepository = IActivityRepository;
            _IactivityMasterConfiguration = IactivityMasterConfiguration;

        }

        public GetActivityLogResponse GetBackofficeAllActivityLog(int UserId, int pageIndex, int pageSize, string IpAddress, string DeviceId, string ActivityAliasName, string ModuleType, long? StatusCode, DateTime? fromdate, DateTime? todate)
        {
            try
            {
                //var ActivityLogData1 = new List<GetActivityLogData>();
                string fromdatetime = string.Empty, todatetime = string.Empty;
                if (fromdate != null && todate != null)
                {
                    fromdatetime = fromdate.Value.Add(new TimeSpan(0, 00, 00, 00)).ToString("yyyy-MM-dd HH:mm:ss");
                    todatetime = todate.Value.Add(new TimeSpan(0, 23, 59, 59)).ToString("yyyy-MM-dd HH:mm:ss");
                }
                List<GetActivityLogData> userTypeList = (dynamic)null;
                if (fromdate != null && todate != null)
                    userTypeList = _dbContext.GetActivityLogData.FromSql("dbo.Sp_GetActivityData @UserId={0},@IpAddress = {1}, @DeviceId = {2}," +
                         "@ActivityAliasName ={3},@ModuleType ={4},@StatusCode ={5},@FromDate ={6}," +
                         "@ToDate ={7}", UserId, IpAddress, DeviceId, ActivityAliasName,  ModuleType, StatusCode, fromdatetime, todatetime).ToList();
                else
                    userTypeList = _dbContext.GetActivityLogData.FromSql("dbo.Sp_GetActivityData @UserId={0},@IpAddress = {1}, @DeviceId = {2}," +
                         "@ActivityAliasName ={3},@ModuleType ={4},@StatusCode ={5},@FromDate ={6}," +
                         "@ToDate ={7}", UserId, IpAddress, DeviceId, ActivityAliasName,  ModuleType, StatusCode, fromdate, todate).ToList();

                var ActivityList = new List<GetActivityLogData>();
                foreach (var item in userTypeList)
                {
                    GetActivityLogData imodel = new GetActivityLogData();
                    imodel.Id = item.Id;
                    //imodel.ApplicationId = item.ApplicationId;
                    //imodel.StatusCode = item.StatusCode;
                    imodel.DeviceId = item.DeviceId;
                    imodel.IPAddress = item.IPAddress;
                    //imodel.ReturnCode = item.ReturnCode;
                    //imodel.ReturnMsg = item.ReturnMsg;
                    //imodel.ErrorCode = item.ErrorCode;
                    //imodel.Session = item.Session;
                    //imodel.AccessToken = item.AccessToken;
                    imodel.AliasName = item.AliasName;
                    imodel.ActivityType = item.ActivityType;
                    imodel.ModuleTypeName = item.ModuleTypeName;
                    //imodel.HostURLName = item.HostURLName;
                    //imodel.Request = item.Request;
                    //imodel.Response = item.Response;
                    imodel.CreatedDate = item.CreatedDate;
                    imodel.CreatedBy = item.CreatedBy;
                    //imodel.UpdatedBy = item.UpdatedBy;
                    //imodel.UpdatedDate = item.UpdatedDate;
                    imodel.Remark = item.Remark;
                    //imodel.Connection = item.Connection;
                    imodel.UserName = item.UserName;
                    ActivityList.Add(imodel);
                }

                var total = ActivityList.Count();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }
                if (pageSize == 0)
                {
                    pageSize = 10;
                }

                var skip = pageSize * (pageIndex - 1);
                GetActivityLogResponse response = new GetActivityLogResponse();
                response.TotalCount = userTypeList.Count;
                response.GetActivityLogList = ActivityList.Skip(skip).Take(pageSize).ToList();
                return response;

                /*
                string Qry = "";
                IQueryable<GetActivityLogData> Result;
                Qry = @" Select AM.Id as [Id],AM.Remark,AM.Connection,AM.ApplicationId,AM.StatusCode,AM.DeviceId,AM.IPAddress,AM.ReturnCode,AM.ReturnMsg,AM.ErrorCode,AM.Session,";
                Qry += " AM.AccessToken,AM.AliasName,ATM.TypeMaster as [ActivityType],TM.SubType as [ModuleTypeName],HM.HostURL as [HostURLName],";
                Qry += " AD.Request,AD.Response,AM.CreatedDate,AM.CreatedBy,AM.UpdatedBy,AM.UpdatedDate From ActivityRegister AM";
                //Qry += " '' as Request,'' as Response,AM.CreatedDate,AM.CreatedBy,AM.UpdatedBy,AM.UpdatedDate From ActivityRegister AM";
                Qry += " Inner Join ActivityRegisterDetail AD on AM.Id=AD.ActivityId";
                Qry += " Left  Join typemaster TM on AM.ModuleTypeId=TM.Id Left Join ActivityType_Master ATM on AM.ActivityTypeId=ATM.Id";
                Qry += " Left join HostURLMaster HM ON AM.HostURLId=HM.Id ";

                if (UserId > 0)
                {
                    if (!string.IsNullOrEmpty(IpAddress))
                    {
                        Qry += " Where AM.IPAddress='" + IpAddress + "' and AM.CreatedBy=" + UserId + "";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                    if (!string.IsNullOrEmpty(DeviceId))
                    {
                        if (!string.IsNullOrEmpty(IpAddress))
                            Qry += " And AM.DeviceId='" + DeviceId + "'";
                        else
                            //Qry += "TUR.Mobile= '" + Mobile + "'";
                            Qry += " Where AM.DeviceId='" + DeviceId + "' and AM.CreatedBy=" + UserId + "";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                    if (!string.IsNullOrEmpty(ActivityAliasName))
                    {
                        if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId))
                            Qry += "AM.AliasName='" + ActivityAliasName + "'";
                        else

                            Qry += " Where AM.AliasName='" + ActivityAliasName + "' and AM.CreatedBy=" + UserId + "";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                    if (!string.IsNullOrEmpty(URL))
                    {
                        if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId) ||
                            !string.IsNullOrEmpty(ActivityAliasName))
                            Qry += "HM.HostURL='" + URL + "'";
                        else
                            Qry += " Where HM.HostURL='" + URL + "' and AM.CreatedBy=" + UserId + "";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                    if (!string.IsNullOrEmpty(ModuleType))
                    {
                        if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId) ||
                            !string.IsNullOrEmpty(ActivityAliasName) || !string.IsNullOrEmpty(URL))
                            Qry += "TM.SubType='" + ModuleType + "'";
                        else
                            Qry += " Where TM.SubType='" + ModuleType + "' and AM.CreatedBy=" + UserId + "";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                    if (StatusCode > 0)
                    {
                        if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId) ||
                            !string.IsNullOrEmpty(ActivityAliasName) || !string.IsNullOrEmpty(URL) || !string.IsNullOrEmpty(ModuleType))
                            Qry += "AM.StatusCode='" + StatusCode + "'";
                        else
                            Qry += " Where AM.StatusCode='" + StatusCode + "' and AM.CreatedBy=" + UserId + "";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId) ||
                            !string.IsNullOrEmpty(ActivityAliasName) || !string.IsNullOrEmpty(URL) || !string.IsNullOrEmpty(ModuleType)
                            || StatusCode > 0)
                            Qry += "Order by AM.CreatedDate Desc";
                        else
                            Qry += " Where AM.CreatedBy=" + UserId + " Order by AM.CreatedDate Desc";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(IpAddress))
                    {
                        Qry += " Where AM.IPAddress='" + IpAddress + "' Order by AM.CreatedDate Desc";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                    if (!string.IsNullOrEmpty(DeviceId))
                    {
                        if (!string.IsNullOrEmpty(IpAddress))
                            Qry += " And AM.DeviceId='" + DeviceId + "'";
                        else
                            Qry += " Where AM.DeviceId='" + DeviceId + "'";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                    if (!string.IsNullOrEmpty(ActivityAliasName))
                    {
                        if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId))
                            Qry += " And AM.AliasName='" + ActivityAliasName + "'";
                        else
                            Qry += " Where AM.AliasName='" + ActivityAliasName + "'";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                    if (!string.IsNullOrEmpty(URL))
                    {
                        if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId)
                            || !string.IsNullOrEmpty(ActivityAliasName))
                            Qry += " And HM.HostURL='" + URL + "'";
                        else
                            Qry += " Where HM.HostURL='" + URL + "'";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                    if (!string.IsNullOrEmpty(ModuleType))
                    {
                        if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId)
                          || !string.IsNullOrEmpty(ActivityAliasName) || !string.IsNullOrEmpty(URL))
                            Qry += " And TM.SubType='" + ModuleType + "'";
                        else
                            Qry += " Where TM.SubType='" + ModuleType + "'";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                    if (StatusCode > 0)
                    {
                        if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId)
                          || !string.IsNullOrEmpty(ActivityAliasName) || !string.IsNullOrEmpty(URL)
                          || !string.IsNullOrEmpty(ModuleType))
                            Qry += " And AM.StatusCode='" + StatusCode + "'";
                        else
                            Qry += " Where AM.StatusCode='" + StatusCode + "'";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                    else
                    {
                        Qry += " Order by AM.CreatedDate Desc";
                        Result = _dbContext.GetActivityLogData.FromSql(Qry);
                        if (Result != null)
                            ActivityLogData = Result.ToList();
                    }
                }
                */

                //bool ReturnCode = false;
                //string ReturnData = string.Empty;
                //SqlParameter[] param1 = new SqlParameter[]{
                //    new SqlParameter("@UserId",SqlDbType.Int, 10, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default,UserId =0),
                //    new SqlParameter("@IpAddress",SqlDbType.NVarChar, 60, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default,IpAddress=string.Empty),
                //    new SqlParameter("@DeviceId",SqlDbType.NVarChar, 4000, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, DeviceId=string.Empty),
                //    new SqlParameter("@ActivityAliasName",SqlDbType.NVarChar, 2000, ParameterDirection.Input, true, 0,0 , String.Empty, DataRowVersion.Default, ActivityAliasName=string.Empty),
                //    new SqlParameter("@URL",SqlDbType.NVarChar, 1000, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, URL=string.Empty),
                //    new SqlParameter("@ModuleType",SqlDbType.NVarChar, 300, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, ModuleType=string.Empty) ,
                //    new SqlParameter("@StatusCode",SqlDbType.BigInt, 10, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, StatusCode=0),
                //    new SqlParameter("@FromDate",SqlDbType.DateTime2, 50, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, fromdate) ,
                //    new SqlParameter("@ToDate",SqlDbType.DateTime2, 50, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, todate),
                //    new SqlParameter("@ReturnData",SqlDbType.NVarChar, 10000, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, ReturnData),
                //    new SqlParameter("@ReturnCode",SqlDbType.Bit, 10, ParameterDirection.Output, false, 18, 8, String.Empty, DataRowVersion.Default, ReturnCode)
                //    };
                //var ActivityLogData123 = _dbContext.Database.ExecuteSqlCommand("Sp_GetActivityData @UserId =0 ,@IpAddress=null,@DeviceId=null,@ActivityAliasName=null,@URL=null,@ModuleType=null,@StatusCode=0,@FromDate=null,@ToDate=null, @ReturnCode  OUTPUT", param1);



                //var ActivityLogData123 = _dbContext.Database.
                //    ExecuteSqlCommand("Sp_GetActivityData @UserId," +
                //    "@IpAddress,@DeviceId,@ActivityAliasName," +
                //    "@URL,@ModuleType,@StatusCode,@FromDate," +
                //    "@ToDate, @ReturnData OUTPUT,@ReturnCode  OUTPUT", 
                //    param1);
                //var ActivityLogData123 = _dbContext.Database.ExecuteSqlCommand("Sp_GetActivityData @UserId =0 ,@IpAddress=null,@DeviceId=null,@ActivityAliasName=null,@URL=null,@ModuleType=null,@StatusCode=0,@FromDate=null,@ToDate=null", param1);
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }



        //public GetActivityLogResponse GetBackofficeAllActivityLog_Old25_3_19(int UserId, int pageIndex, int pageSize, string IpAddress, string DeviceId, string ActivityAliasName, string URL, string ModuleType, long? StatusCode, DateTime? fromdate, DateTime? todate)
        //{
        //    try
        //    {
        //        //var ActivityLogData1 = new List<GetActivityLogData>();
        //        string fromdatetime = string.Empty, todatetime = string.Empty;
        //        if (fromdate != null && todate != null)
        //        {
        //            fromdatetime = fromdate.Value.Add(new TimeSpan(0, 00, 00, 00)).ToString("yyyy-MM-dd HH:mm:ss");
        //            todatetime = todate.Value.Add(new TimeSpan(0, 23, 59, 59)).ToString("yyyy-MM-dd HH:mm:ss");
        //        }
        //        List<GetActivityLogData> userTypeList = (dynamic)null;
        //        if (fromdate != null && todate != null)                
        //            userTypeList = _dbContext.GetActivityLogData.FromSql("dbo.Sp_GetActivityData @UserId={0},@IpAddress = {1}, @DeviceId = {2}," +
        //                 "@ActivityAliasName ={3},@URL ={4},@ModuleType ={5},@StatusCode ={6},@FromDate ={7}," +
        //                 "@ToDate ={8}", UserId, IpAddress, DeviceId, ActivityAliasName, URL, ModuleType, StatusCode, fromdatetime, todatetime).ToList();
        //        else
        //            userTypeList = _dbContext.GetActivityLogData.FromSql("dbo.Sp_GetActivityData @UserId={0},@IpAddress = {1}, @DeviceId = {2}," +
        //                 "@ActivityAliasName ={3},@URL ={4},@ModuleType ={5},@StatusCode ={6},@FromDate ={7}," +
        //                 "@ToDate ={8}", UserId, IpAddress, DeviceId, ActivityAliasName, URL, ModuleType, StatusCode, fromdate, todate).ToList();

        //        var ActivityList = new List<GetActivityLogData>();
        //        foreach (var item in userTypeList)
        //        {
        //            GetActivityLogData imodel = new GetActivityLogData();
        //            imodel.Id = item.Id;
        //            //imodel.ApplicationId = item.ApplicationId;
        //            //imodel.StatusCode = item.StatusCode;
        //            imodel.DeviceId = item.DeviceId;
        //            imodel.IPAddress = item.IPAddress;
        //            //imodel.ReturnCode = item.ReturnCode;
        //            //imodel.ReturnMsg = item.ReturnMsg;
        //            //imodel.ErrorCode = item.ErrorCode;
        //            //imodel.Session = item.Session;
        //            //imodel.AccessToken = item.AccessToken;
        //            imodel.AliasName = item.AliasName;
        //            imodel.ActivityType = item.ActivityType;
        //            imodel.ModuleTypeName = item.ModuleTypeName;
        //            //imodel.HostURLName = item.HostURLName;
        //            //imodel.Request = item.Request;
        //            //imodel.Response = item.Response;
        //            imodel.CreatedDate = item.CreatedDate;
        //            imodel.CreatedBy = item.CreatedBy;
        //            //imodel.UpdatedBy = item.UpdatedBy;
        //            //imodel.UpdatedDate = item.UpdatedDate;
        //            imodel.Remark = item.Remark;
        //            //imodel.Connection = item.Connection;
        //            imodel.UserName = item.UserName;
        //            ActivityList.Add(imodel);
        //        }

        //        var total = ActivityList.Count();
        //        if (pageIndex == 0)
        //        {
        //            pageIndex = 1;
        //        }
        //        if (pageSize == 0)
        //        {
        //            pageSize = 10;
        //        }

        //        var skip = pageSize * (pageIndex - 1);
        //        GetActivityLogResponse response = new GetActivityLogResponse();
        //        response.TotalCount = userTypeList.Count;
        //        response.GetActivityLogList = ActivityList.Skip(skip).Take(pageSize).ToList();
        //        return response;

        //        /*
        //        string Qry = "";
        //        IQueryable<GetActivityLogData> Result;
        //        Qry = @" Select AM.Id as [Id],AM.Remark,AM.Connection,AM.ApplicationId,AM.StatusCode,AM.DeviceId,AM.IPAddress,AM.ReturnCode,AM.ReturnMsg,AM.ErrorCode,AM.Session,";
        //        Qry += " AM.AccessToken,AM.AliasName,ATM.TypeMaster as [ActivityType],TM.SubType as [ModuleTypeName],HM.HostURL as [HostURLName],";
        //        Qry += " AD.Request,AD.Response,AM.CreatedDate,AM.CreatedBy,AM.UpdatedBy,AM.UpdatedDate From ActivityRegister AM";
        //        //Qry += " '' as Request,'' as Response,AM.CreatedDate,AM.CreatedBy,AM.UpdatedBy,AM.UpdatedDate From ActivityRegister AM";
        //        Qry += " Inner Join ActivityRegisterDetail AD on AM.Id=AD.ActivityId";
        //        Qry += " Left  Join typemaster TM on AM.ModuleTypeId=TM.Id Left Join ActivityType_Master ATM on AM.ActivityTypeId=ATM.Id";
        //        Qry += " Left join HostURLMaster HM ON AM.HostURLId=HM.Id ";

        //        if (UserId > 0)
        //        {
        //            if (!string.IsNullOrEmpty(IpAddress))
        //            {
        //                Qry += " Where AM.IPAddress='" + IpAddress + "' and AM.CreatedBy=" + UserId + "";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //            if (!string.IsNullOrEmpty(DeviceId))
        //            {
        //                if (!string.IsNullOrEmpty(IpAddress))
        //                    Qry += " And AM.DeviceId='" + DeviceId + "'";
        //                else
        //                    //Qry += "TUR.Mobile= '" + Mobile + "'";
        //                    Qry += " Where AM.DeviceId='" + DeviceId + "' and AM.CreatedBy=" + UserId + "";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //            if (!string.IsNullOrEmpty(ActivityAliasName))
        //            {
        //                if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId))
        //                    Qry += "AM.AliasName='" + ActivityAliasName + "'";
        //                else

        //                    Qry += " Where AM.AliasName='" + ActivityAliasName + "' and AM.CreatedBy=" + UserId + "";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //            if (!string.IsNullOrEmpty(URL))
        //            {
        //                if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId) ||
        //                    !string.IsNullOrEmpty(ActivityAliasName))
        //                    Qry += "HM.HostURL='" + URL + "'";
        //                else
        //                    Qry += " Where HM.HostURL='" + URL + "' and AM.CreatedBy=" + UserId + "";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //            if (!string.IsNullOrEmpty(ModuleType))
        //            {
        //                if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId) ||
        //                    !string.IsNullOrEmpty(ActivityAliasName) || !string.IsNullOrEmpty(URL))
        //                    Qry += "TM.SubType='" + ModuleType + "'";
        //                else
        //                    Qry += " Where TM.SubType='" + ModuleType + "' and AM.CreatedBy=" + UserId + "";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //            if (StatusCode > 0)
        //            {
        //                if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId) ||
        //                    !string.IsNullOrEmpty(ActivityAliasName) || !string.IsNullOrEmpty(URL) || !string.IsNullOrEmpty(ModuleType))
        //                    Qry += "AM.StatusCode='" + StatusCode + "'";
        //                else
        //                    Qry += " Where AM.StatusCode='" + StatusCode + "' and AM.CreatedBy=" + UserId + "";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //            else
        //            {
        //                if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId) ||
        //                    !string.IsNullOrEmpty(ActivityAliasName) || !string.IsNullOrEmpty(URL) || !string.IsNullOrEmpty(ModuleType)
        //                    || StatusCode > 0)
        //                    Qry += "Order by AM.CreatedDate Desc";
        //                else
        //                    Qry += " Where AM.CreatedBy=" + UserId + " Order by AM.CreatedDate Desc";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(IpAddress))
        //            {
        //                Qry += " Where AM.IPAddress='" + IpAddress + "' Order by AM.CreatedDate Desc";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //            if (!string.IsNullOrEmpty(DeviceId))
        //            {
        //                if (!string.IsNullOrEmpty(IpAddress))
        //                    Qry += " And AM.DeviceId='" + DeviceId + "'";
        //                else
        //                    Qry += " Where AM.DeviceId='" + DeviceId + "'";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //            if (!string.IsNullOrEmpty(ActivityAliasName))
        //            {
        //                if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId))
        //                    Qry += " And AM.AliasName='" + ActivityAliasName + "'";
        //                else
        //                    Qry += " Where AM.AliasName='" + ActivityAliasName + "'";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //            if (!string.IsNullOrEmpty(URL))
        //            {
        //                if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId)
        //                    || !string.IsNullOrEmpty(ActivityAliasName))
        //                    Qry += " And HM.HostURL='" + URL + "'";
        //                else
        //                    Qry += " Where HM.HostURL='" + URL + "'";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //            if (!string.IsNullOrEmpty(ModuleType))
        //            {
        //                if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId)
        //                  || !string.IsNullOrEmpty(ActivityAliasName) || !string.IsNullOrEmpty(URL))
        //                    Qry += " And TM.SubType='" + ModuleType + "'";
        //                else
        //                    Qry += " Where TM.SubType='" + ModuleType + "'";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //            if (StatusCode > 0)
        //            {
        //                if (!string.IsNullOrEmpty(IpAddress) || !string.IsNullOrEmpty(DeviceId)
        //                  || !string.IsNullOrEmpty(ActivityAliasName) || !string.IsNullOrEmpty(URL)
        //                  || !string.IsNullOrEmpty(ModuleType))
        //                    Qry += " And AM.StatusCode='" + StatusCode + "'";
        //                else
        //                    Qry += " Where AM.StatusCode='" + StatusCode + "'";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //            else
        //            {
        //                Qry += " Order by AM.CreatedDate Desc";
        //                Result = _dbContext.GetActivityLogData.FromSql(Qry);
        //                if (Result != null)
        //                    ActivityLogData = Result.ToList();
        //            }
        //        }
        //        */

        //        //bool ReturnCode = false;
        //        //string ReturnData = string.Empty;
        //        //SqlParameter[] param1 = new SqlParameter[]{
        //        //    new SqlParameter("@UserId",SqlDbType.Int, 10, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default,UserId =0),
        //        //    new SqlParameter("@IpAddress",SqlDbType.NVarChar, 60, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default,IpAddress=string.Empty),
        //        //    new SqlParameter("@DeviceId",SqlDbType.NVarChar, 4000, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, DeviceId=string.Empty),
        //        //    new SqlParameter("@ActivityAliasName",SqlDbType.NVarChar, 2000, ParameterDirection.Input, true, 0,0 , String.Empty, DataRowVersion.Default, ActivityAliasName=string.Empty),
        //        //    new SqlParameter("@URL",SqlDbType.NVarChar, 1000, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, URL=string.Empty),
        //        //    new SqlParameter("@ModuleType",SqlDbType.NVarChar, 300, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, ModuleType=string.Empty) ,
        //        //    new SqlParameter("@StatusCode",SqlDbType.BigInt, 10, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, StatusCode=0),
        //        //    new SqlParameter("@FromDate",SqlDbType.DateTime2, 50, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, fromdate) ,
        //        //    new SqlParameter("@ToDate",SqlDbType.DateTime2, 50, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, todate),
        //        //    new SqlParameter("@ReturnData",SqlDbType.NVarChar, 10000, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, ReturnData),
        //        //    new SqlParameter("@ReturnCode",SqlDbType.Bit, 10, ParameterDirection.Output, false, 18, 8, String.Empty, DataRowVersion.Default, ReturnCode)
        //        //    };
        //        //var ActivityLogData123 = _dbContext.Database.ExecuteSqlCommand("Sp_GetActivityData @UserId =0 ,@IpAddress=null,@DeviceId=null,@ActivityAliasName=null,@URL=null,@ModuleType=null,@StatusCode=0,@FromDate=null,@ToDate=null, @ReturnCode  OUTPUT", param1);



        //        //var ActivityLogData123 = _dbContext.Database.
        //        //    ExecuteSqlCommand("Sp_GetActivityData @UserId," +
        //        //    "@IpAddress,@DeviceId,@ActivityAliasName," +
        //        //    "@URL,@ModuleType,@StatusCode,@FromDate," +
        //        //    "@ToDate, @ReturnData OUTPUT,@ReturnCode  OUTPUT", 
        //        //    param1);
        //        //var ActivityLogData123 = _dbContext.Database.ExecuteSqlCommand("Sp_GetActivityData @UserId =0 ,@IpAddress=null,@DeviceId=null,@ActivityAliasName=null,@URL=null,@ModuleType=null,@StatusCode=0,@FromDate=null,@ToDate=null", param1);
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        throw;
        //    }
        //}

        public List<GetModuleData> GetAllModuleData()
        {
            try
            {
                var ModuleData = _IactivityMasterConfiguration.GetTypeMasterData().Where(i => i.Type.ToLower().ToString() == "activitytype").ToList();
                var ModuleDataList = new List<GetModuleData>();
                if (ModuleData != null)
                {
                    foreach (var item in ModuleData)
                    {
                        GetModuleData imodel = new GetModuleData();
                        imodel.Id = item.Id;
                        imodel.ModuleName = item.SubType;
                        ModuleDataList.Add(imodel);
                    }
                    return ModuleDataList.ToList();
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
    }
}
