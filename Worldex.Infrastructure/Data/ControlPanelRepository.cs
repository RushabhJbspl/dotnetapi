using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.ControlPanel;
using Worldex.Core.ViewModels.ControlPanel;
using System;
using System.Linq;
using System.Collections.Generic;
using Worldex.Core.Entities.NewWallet;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;
using Worldex.Core.Enums;
using Worldex.Core.ApiModels;
using Worldex.Infrastructure.Interfaces;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletOperations;
using Worldex.Core.Entities;
using Worldex.Core.ViewModels.RoleConfig;
using Worldex.Core.Entities.UserRoleManagement;
using Worldex.Core.Entities.Affiliate;
using System.Data.SqlClient;
using System.Data;
using Worldex.Core.ViewModels.BackOffice;
using Worldex.Core.Entities.Wallet;

namespace Worldex.Infrastructure.Data
{
    public class ControlPanelRepository : IControlPanelRepository
    {
        #region Constructor
        private readonly WorldexContext _dbContext;

        private readonly IWalletConfiguration _walletConfiguration;

        public ControlPanelRepository(WorldexContext dbContext, IWalletConfiguration walletConfiguration)
        {
            _dbContext = dbContext;
            _walletConfiguration = walletConfiguration;
        }
        #endregion

        #region TransactionBlockedChannel Methods

        public List<TransactionBlockedChannelRes> GetBlockTranChannelDetail(long ID, short? Status, long? ChannelID)
        {
            try
            {
                string Query = "SELECT ROW_NUMBER() OVER (ORDER BY TBC.Id) AS 'AutoNo',TBC.ID,TBC.ChannelID," +
                                "TBC.TrnType,WTM.TrnTypeName,TBC.Status," +
                                "CASE TBC.Status WHEN 0 THEN 'Inactive' WHEN 1 THEN 'Active' ELSE 'Deleted' END AS 'StrStatus' " +
                                "FROM TransactionBlockedChannel TBC " +
                                "LEFT JOIN WTrnTypeMaster WTM ON WTM.TrnTypeId = TrnType " +
                                "WHERE TBC.Status < 9 AND TBC.ID={0} AND (TBC.ChannelID={1} OR {1}=999) AND (TBC.Status={2} OR {2}=999)";

                var items = _dbContext.TransactionBlockedChannelRes.FromSql(Query, ID, (ChannelID == null ? 999 : ChannelID), (Status == null ? 999 : Status));

                return items.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TransactionBlockedChannelRes> ListBlockTranChannelDetail(enWalletTrnType? TrnType, long? ChannelID, short? Status)
        {
            try
            {
                string Query = "SELECT ROW_NUMBER() OVER (ORDER BY TBC.Id) AS 'AutoNo',TBC.ID,TBC.ChannelID," +
                                "TBC.TrnType,WTM.TrnTypeName,TBC.Status," +
                                "CASE TBC.Status WHEN 0 THEN 'Inactive' WHEN 1 THEN 'Active' ELSE 'Deleted' END AS 'StrStatus' " +
                                "FROM TransactionBlockedChannel TBC " +
                                "LEFT JOIN WTrnTypeMaster WTM ON WTM.TrnTypeId = TrnType " +
                                "WHERE TBC.Status < 9 AND (TBC.TrnType={0} OR {0}=999) AND (TBC.ChannelID={1} OR {1}=999) AND (TBC.Status={2} OR {2}=999)";

                var items = _dbContext.TransactionBlockedChannelRes.FromSql(Query, (TrnType == null ? 999 : Convert.ToInt16(TrnType)), (ChannelID == null ? 999 : ChannelID), (Status == null ? 999 : Status));

                return items.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region User Methods

        public List<CommonCountClass> GetOrgWiseCount()
        {
            try
            {
                //var items = (from p in _dbContext.OrganizationUserMaster.Where(p => p.Status < 9)
                //             .GroupBy(p => p.OrganizationID)
                //             select new CommonCountClass
                //             {
                //                 Count = p.Count(),
                //                 Key = p.First().OrganizationID
                //             }
                //            ).ToList();
                var items = _dbContext.CommonCountClass.FromSql("select cast(Count(*) as INT) as Count,CAST(OrganizationID AS INT) as 'Key','' AS Name from OrganizationUserMaster where status<9 group by OrganizationID").ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<CommonCountClass> GetStatusWiseCount()
        {
            try
            {
                var items = _dbContext.CommonCountClass.FromSql("select cast(Count(*) as INT) as Count,cast(status as INT) as 'Key',case status when 1 then 'Active' when 2 then 'Inactive' when 9 then 'Delete' else 'Unknown' end AS Name from OrganizationUserMaster where status<9 group by status").ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<CommonCountClass> GetTypeWiseCount()
        {
            try
            {
                var items = _dbContext.CommonCountClass.FromSql("select CAST(Count(*) AS INT) as Count,case UserType when 0 then 'Admin' when 1 then 'User'  else 'Unknown' end as Name,CAST(UserType AS INT) as 'Key' from BizUserTypeMapping group by UserType").ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long GetTodaysUserRecCount(short? status, DateTime FromDate, DateTime Todate)
        {
            try
            {
                var items = (from o in _dbContext.OrganizationUserMaster
                             where o.Status < 9 && (status == null || (o.Status == status && status != null)) && o.CreatedDate >= FromDate && o.CreatedDate <= Todate
                             select o
                            ).Count();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long GetUserRecCount(long? OrgID, long? UserType, short? Status, long? RoleID)
        {
            try
            {
                var items = (from u in _dbContext.OrganizationUserMaster
                             join t in _dbContext.BizUserTypeMapping on u.UserID equals t.UserID
                             where u.Status < 9 && (Status == null || (u.Status == Status && Status != null))
                             && (OrgID == null || (u.OrganizationID == OrgID && OrgID != null))
                             && (RoleID == null || (u.RoleID == RoleID && RoleID != null))
                             && (UserType == null || (t.UserType == UserType && UserType != null))
                             select u.Id
                            ).Count();
                #region Trials
                //var items = (from u in _dbContext.UserMaster
                //             join t in _dbContext.BizUserTypeMapping on u.Id equals t.UserID
                //             join o in _dbContext.OrganizationUserMaster on u.Id equals o.UserID
                //             where (Status == null || (u.Status == Status && Status != null))
                //             && (OrgID == null || (o.OrganizationID == OrgID && OrgID != null))
                //             && (UserType == null || (t.UserType == UserType && UserType != null))
                //             select u
                //             ).Count();
                //IQueryable<UserMaster> obj;
                //obj = _dbContext.UserCountInfo.FromSql(
                //    @"SELECT U.Id,BizUserID,U.Status FROM UserMaster U 
                //        INNER JOIN BizUserTypeMapping T ON T.UserID=U.ID
                //        INNER JOIN OrganizationUserMaster O ON O.UserID=U.ID 
                //        WHERE (Status = NULL OR (U.Status={0} AND Status IS NOT NULL)) 
                //        AND (UserType=NULL OR (T.UserType={1} AND UserType IS NOT NULL))
                //        AND (OrganizationID=NULL OR (O.OrganizationID={2} AND OrganizationID IS NOT NULL))", Status, UserType,OrgID);
                #endregion
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TodaysCounts GetUserRecCounts(long? OrgID, long? UserType, short? Status, long? RoleID, DateTime FromDate, DateTime Todate)
        {
            try
            {
                string condition = "OM.Status < 9 AND 1=1";
                //string qry = "SELECT COUNT(OM.UserID) AS 'TotalCount' FROM OrganizationUserMaster OM INNER JOIN BizUserTypeMapping BT ON OM.UserID = BT.UserID WHERE ";
                string qry = "SELECT COUNT(OM.UserID) AS 'TotalCount',(SELECT COUNT(Id) FROM OrganizationUserMaster WHERE CreatedDate BETWEEN {0} AND {1}) AS 'TodayCount' FROM OrganizationUserMaster OM INNER JOIN BizUserTypeMapping BT ON OM.UserID = BT.UserID WHERE ";
                if ((OrgID != null) && (OrgID > 0))
                {
                    condition += " AND OM.OrganizationID=" + OrgID.ToString();
                }
                if ((UserType != null) && (UserType > 0))
                {
                    condition += " AND BT.UserType=" + UserType.ToString();
                }
                if ((Status != null))
                {
                    condition += " AND OM.Status=" + Status.ToString();
                }
                if ((RoleID != null) && (RoleID > 0))
                {
                    condition += " AND OM.RoleID=" + RoleID.ToString();
                }
                qry += condition;
                IQueryable<TodaysCounts> Result = _dbContext.TodaysCounts.FromSql(qry, FromDate, Todate);
                var abc = Result.FirstOrDefault();
                return abc;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<CommonCountClass> GetRoleWiseCount()
        {
            try
            {
                var items = (from p in _dbContext.OrganizationUserMaster.Where(p => p.Status < 9).GroupBy(p => p.RoleID)
                             select new CommonCountClass
                             {
                                 Count = p.Count(),
                                 Name = (p.First().RoleID == 1) ? "Owner" : (p.First().RoleID == 2) ? "Admin" : (p.First().RoleID == 3) ? "Spender" : (p.First().RoleID == 4) ? "Viewer" : "Unknown",
                                 Key = Convert.ToInt32(p.First().RoleID)
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

        //vsoalnki 218-11-27
        public List<UserRes> ListUserLast5()
        {
            try
            {
                var obj = _dbContext.UserRes.FromSql(@"select top 5 bu.UserName,bu.Email,bu.FirstName,bu.LastName,bu.Mobile from BizUser bu inner join UserMaster u on u.Id=bu.Id where u.Status < 9 order by u.CreatedDate desc").ToList();
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ListUserDetailRes ListAllUserDetail(long? orgID, long? userType, short? status, int? pageNo, int? pageSize)
        {
            //Change in CreatedDate For BizUser BY Rushali = BU.CreatedDate(17-12-2019)
            try
            {
                ListUserDetailRes res = new ListUserDetailRes();
                string str = "SELECT ROW_NUMBER() OVER (ORDER BY BU.Id) AS 'AutoNo',ISNULL(BU.FirstName, '-') AS 'FirstName',ISNULL(BU.LastName, '-') AS 'LastName'," +
                                "ISNULL(BU.Mobile, '-') AS 'Mobile',ISNULL(BU.Email, '-') AS 'Email',ISNULL(OM.OrganizationName, '-') AS 'OrganizationName'," +
                                "ISNULL(BU.CreatedDate, '') AS 'CreatedDate',ISNULL(BU.CountryCode, '-') AS 'CountryCode'," +
                                "CASE(BU.IsEnabled) WHEN 0 THEN 'Yes' ELSE 'No' END AS 'IsEnabled'," +
                                "CASE(BU.EmailConfirmed) WHEN 1 THEN 'Yes' ELSE 'No' END AS 'EmailConfirmed'," +
                                "CASE(BT.UserType) WHEN 0 THEN 'Admin' WHEN 1 THEN 'User' WHEN 2 THEN 'Org-Admin' ELSE 'Unknown' END AS 'UserType'," +
                                "(SELECT ISNULL(SUM(Balance),0) FROM WalletMasters WHERE UserID=BU.Id) AS 'TotalBalance' FROM BizUser BU " +
                                "LEFT JOIN OrganizationUserMaster OU ON OU.UserID = BU.Id " +
                                "LEFT JOIN OrganizationMaster OM ON OM.Id = OU.OrganizationID " +
                                "LEFT JOIN BizUserTypeMapping BT ON BT.UserID = BU.Id " +
                                "WHERE BU.IsEnabled < 9 AND (OM.Id={0} OR {0}=0) AND (BT.UserType={1} OR {1}=999) AND (BU.IsEnabled={2} OR {2}=999) ";

                var Result = _dbContext.UserDetailRes.FromSql(str, (orgID == null ? 0 : orgID), (userType == null ? 999 : userType), (status == null ? 999 : status)).ToList();
                var pagesize = (pageSize == null) ? Helpers.PageSize : Convert.ToInt64(pageSize);
                var it = Convert.ToDouble(Result.Count) / pagesize;
                var fl = Math.Ceiling(it);
                res.TotalPages = Convert.ToInt64(fl);
                if (pageNo > 0)
                {
                    if (pagesize == 0)
                    {
                        int skip = Convert.ToInt32(Helpers.PageSize * (pageNo - 1));
                        Result = Result.Skip(skip).Take(Helpers.PageSize).ToList();
                    }
                    else
                    {
                        int skip = Convert.ToInt32((pagesize) * (pageNo - 1));
                        Result = Result.Skip(skip).Take(Convert.ToInt32(pagesize)).ToList();
                    }
                }
                res.Details = Result.ToList();
                res.PageSize = (pageSize == null || pagesize == 0) ? Helpers.PageSize : Convert.ToInt64(pagesize);
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Organization Methods

        public long GetOrgRecCount(short? status)
        {
            try
            {
                var items = (from o in _dbContext.OrganizationMaster
                             where o.Status < 9 && (status == null || (o.Status == status && status != null))
                             select o
                            ).Count();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long GetTodaysOrgRecCount(short? status, DateTime FromDate, DateTime Todate)
        {
            try
            {
                var items = (from o in _dbContext.OrganizationMaster
                             where o.Status < 9 && (status == null || (o.Status == status && status != null)) && o.CreatedDate >= FromDate && o.CreatedDate <= Todate
                             select o
                            ).Count();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<List<OrgMasterRes>> ListOrgDetail(short? status, long? orgID)
        {
            try
            {
                List<OrgMasterRes> res = new List<OrgMasterRes>();
                string condition = " O.Status < 9 AND 1=1";
                string str = "SELECT O.Id AS 'OrgID',O.OrganizationName AS 'OrgName',O.Status,O.IsDefault,ISNULL(SUM(U.Balance), 0) AS 'TotalBalance',COUNT(U.Id) AS 'TotalWallets',(SELECT COUNT(UserID) FROM OrganizationUserMaster WHERE OrganizationID = O.Id) AS 'TotalUsers' FROM OrganizationMaster O LEFT JOIN WalletMasters U ON O.ID = U.OrgID WHERE";
                if ((status != null) && (status > 0))
                {
                    condition += " AND O.Status=" + status.ToString();
                }
                if ((orgID != null) && (orgID > 0))
                {
                    condition += " AND O.Id=" + orgID.ToString();
                }
                str += condition + " GROUP BY O.Id,O.OrganizationName,O.Status,O.IsDefault";
                IQueryable<OrgMasterRes> Result = _dbContext.OrgMasterRes.FromSql(str);

                res = Result.ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListOrgDetail", this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool SetDefaultZero(long userid, DateTime UpdatedDate)
        {
            var res = _dbContext.OrganizationMaster.Where(x => x.IsDefault == 1).ToList();
            try
            {
                res.ForEach(a =>
                    {
                        a.IsDefault = 0;
                        a.UpdatedBy = userid;
                        a.UpdatedDate = UpdatedDate;
                    }
                );
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<int> GetWCount(long OrgID)
        {
            try
            {
                var items = (from w in _dbContext.WalletMasters
                             where w.OrgID == OrgID && w.Status < 9
                             select w.Id
                            ).CountAsync();
                return await items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWCount", this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<int> GetUCount(long OrgID)
        {
            try
            {
                var items = (from w in _dbContext.OrganizationUserMaster
                             where w.OrganizationID == OrgID && w.Status < 9
                             select w.Id
                            ).CountAsync();
                return await items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetUCount", this.GetType().Name, ex);
                throw ex;
            }
            throw new NotImplementedException();
        }

        #endregion

        #region Wallet Methods

        public long GetWalletRecCount(long? walletTypeID, short? status, long? orgID, long? userID)
        {
            try
            {
                var items = (from o in _dbContext.WalletMasters
                             where o.Status < 9 && (status == null || (o.Status == status && status != null))
                             && (walletTypeID == null || (o.WalletTypeID == walletTypeID && walletTypeID != null))
                             && (orgID == null || (o.OrgID == orgID && orgID != null))
                             && (userID == null || (o.UserID == userID && userID != null))
                             select o
                            ).Count();

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TempCount GetWalletRecCounts(long? walletTypeID, short? status, long? orgID, long? userID)
        {
            try
            {
                string condition = "Status < 9 AND 1=1";
                string qry = "SELECT COUNT(Id) AS 'TotalCount' FROM WalletMasters WHERE ";
                if ((orgID != null) && (orgID > 0))
                {
                    condition += " AND OrgID=" + orgID.ToString();
                }
                if ((userID != null) && (userID > 0))
                {
                    condition += " AND UserID=" + userID.ToString();
                }
                if ((status != null) && (status > 0))
                {
                    condition += " AND Status=" + status.ToString();
                }
                if ((walletTypeID != null) && (walletTypeID > 0))
                {
                    condition += " AND WalletTypeID=" + walletTypeID.ToString();
                }
                qry += condition;
                IQueryable<TempCount> Result = _dbContext.TempCount.FromSql(qry);
                var ReturnRes = Result.FirstOrDefault();
                return ReturnRes;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<CommonCountClass> GetWalletTypeWiseCount()
        {
            try
            {
                var ctx = (from p in _dbContext.WalletMasters
                           join q in _dbContext.WalletTypeMasters
                           on p.WalletTypeID equals q.Id
                           where p.Status < 9
                           group new { p, q } by new { q.WalletTypeName, q.Id } into grp
                           select new CommonCountClass
                           {
                               Key = Convert.ToInt32(grp.Key.Id),
                               Name = grp.Key.WalletTypeName,
                               Count = grp.Select(x => x.p.WalletTypeID).Count()//grp.Count() //
                           }
                ).ToList();
                // Only Return Count
                //var items = (from p in _dbContext.UserWalletMaster.GroupBy(p => p.WalletTypeID)
                //             select new CommonCountClass
                //             {
                //                 Count = p.Count(),
                //                 Key = p.First().WalletTypeID
                //             }
                //            ).ToList();
                return ctx;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<CommonCountClass> GetWalletStatusWiseCount()
        {
            try
            {
                //var items = (from p in _dbContext.WalletMasters.Where(p => p.Status < 9).GroupBy(p => p.Status)
                //             select new CommonCountClass
                //             {
                //                 Count = p.Count(),
                //                 Name = (p.First().Status == 1) ? "Active" : (p.First().Status == 2) ? "Inactive" : (p.First().Status == 3) ? "Freeze" : (p.First().Status == 4) ? "Inoperative" : (p.First().Status == 5) ? "Suspended" : (p.First().Status == 6) ? "Blocked" : (p.First().Status == 9) ? "Deleted" : "Unknown",
                //                 Key = p.First().Status
                //             }
                //            ).ToList();
                var items = _dbContext.CommonCountClass.FromSql("select CAST(Count(*) AS INT) as Count,CAST(Status AS INT) as 'Key',case Status when 1 then 'Active' when 2 then 'Inactive' when 3 then 'Freeze' when 4 then 'Inoperative' when 5 then 'Suspended' when 6 then 'Blocked' when 9 then 'Deleted' else 'Unknown' end as Name from WalletMasters where Status<9 group by Status").ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<CommonCountClass> GetWalletOrgWiseCount()
        {
            try
            {
                var items = (from p in _dbContext.WalletMasters.Where(p => p.Status < 9).GroupBy(p => p.OrgID)
                             select new CommonCountClass
                             {
                                 Count = p.Count(),
                                 Key = Convert.ToInt32(p.First().OrgID)
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

        public List<CommonCountClass> GetWalletUserWiseCount()
        {
            try
            {
                var items = (from p in _dbContext.WalletMasters.Where(p => p.Status < 9).GroupBy(p => p.UserID)
                             select new CommonCountClass
                             {
                                 Count = p.Count(),
                                 Key = Convert.ToInt32(p.First().UserID)
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

        //2018-12-17
        public List<WalletResV1> ListAllWallet(DateTime? FromDate, DateTime? ToDate, short? Status, int PageSize, int Page, long? UserId, long? OrgId, string WalletType, ref long TotalCount)
        {
            List<WalletResV1> items = new List<WalletResV1>();
            try
            {
                string sqlQuery = "SELECT w.CreatedDate as Date,w.Status,CASE w.Status WHEN 0 THEN 'Disable' WHEN 1 THEn 'Enable' ELSE 'Deleted' END AS 'StrStatus',w.Balance,wt.WalletTypeName,w.IsValid,o.OrganizationName,b.UserName,w.Walletname AS 'WalletName',w.AccWalletID,w.IsDefaultWallet,ISNULL(w.PublicAddress,'Not Found') AS PublicAddress,w.InBoundBalance,w.OutBoundBalance FROM WalletMasters w INNER JOIN WalletTypeMasters wt ON wt.Id = w.WalletTypeID INNER JOIN BizUser b ON b.id =w.userid INNER JOIN organizationmaster o ON o.Id =w.OrgID where w.Status < 9 AND (w.UserId ={0} OR {0}=0)  AND (w.Status={1} OR {1}=999) AND (w.OrgId={2} OR {2}=0) AND (wt.WalletTypeName={3} OR {3}='')";

                if (FromDate != null && ToDate != null)
                {
                    FromDate = Convert.ToDateTime(FromDate).AddHours(0).AddMinutes(0).AddSeconds(0);
                    ToDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                    sqlQuery = sqlQuery + "AND w.CreatedDate BETWEEN {4} AND {5}";
                    items = _dbContext.WalletResV1.FromSql(@sqlQuery, (UserId == null ? 0 : UserId), (Status == null ? 999 : Status), (OrgId == null ? 0 : OrgId), (WalletType == null ? "" : WalletType), FromDate, ToDate).ToList();
                }
                else
                {
                    items = _dbContext.WalletResV1.FromSql(@sqlQuery, (UserId == null ? 0 : UserId), (Status == null ? 999 : Status), (OrgId == null ? 0 : OrgId), (WalletType == null ? "" : WalletType)).ToList();
                }
                TotalCount = items.Count();
                if (items != null)
                {
                    if (Page > 0)
                    {
                        int skip = PageSize * (Page - 1);
                        items = items.Skip(skip).Take(PageSize).ToList();
                    }
                }
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //2018-12-17
        public WalletRes GetWalletIdWise(string AccWalletId)
        {
            WalletRes items = new WalletRes();
            try
            {
                string sqlQuery = "SELECT w.Status,CASE w.Status WHEN 0 THEN 'Disable' WHEN 1 THEn 'Enable' ELSE 'Deleted' END AS 'StrStatus',w.Balance,wt.WalletTypeName,w.IsValid,o.OrganizationName,b.UserName,w.Walletname AS 'WalletName',w.AccWalletID,w.IsDefaultWallet,ISNULL(w.PublicAddress,'Not Found') AS PublicAddress,w.InBoundBalance,w.OutBoundBalance FROM WalletMasters w INNER JOIN WalletTypeMasters wt ON wt.Id = w.WalletTypeID INNER JOIN BizUser b ON b.id =w.userid INNER JOIN organizationmaster o ON o.Id =w.OrgID where w.Status < 9 AND w.AccWalletID={0}";

                items = _dbContext.WalletRes.FromSql(@sqlQuery, AccWalletId).ToList().FirstOrDefault();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Wallet Authorized User Methods

        public long GetWalletAuthUserCount(short? status, long? orgID, long? userID, long? RoleID)
        {
            try
            {
                var items = (from o in _dbContext.WalletAuthorizeUserMaster
                             where o.Status < 9 && (status == null || (o.Status == status && status != null))
                             && (orgID == null || (o.OrgID == orgID && orgID != null))
                             && (RoleID == null || (o.RoleID == RoleID && RoleID != null))
                             && (userID == null || (o.UserID == userID && userID != null))
                             select o
                            ).Count();

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<CommonCountClass> GetWalletAuthUserStatusWiseCount()
        {
            try
            {
                var items = (from p in _dbContext.WalletAuthorizeUserMaster.Where(p => p.Status < 9).GroupBy(p => p.Status)
                             select new CommonCountClass
                             {
                                 Count = p.Count(),
                                 Name = (p.First().Status == 1) ? "Active" : (p.First().Status == 2) ? "Inactive" : (p.First().Status == 9) ? "Delete" : "Unknown",
                                 Key = p.First().Status
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

        public List<CommonCountClass> GetWalletAuthUserOrgWiseCount()
        {
            try
            {
                var items = (from p in _dbContext.WalletAuthorizeUserMaster.Where(p => p.Status < 9).GroupBy(p => p.OrgID)
                             select new CommonCountClass
                             {
                                 Count = p.Count(),
                                 Key = Convert.ToInt32(p.First().OrgID)
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

        public List<CommonCountClass> GetWalletAuthUserRoleWiseCount()
        {
            try
            {
                var items = _dbContext.CommonCountClass.FromSql("select CAST(Count(*) AS INT) as Count,case roleid when 1 then 'Owner' when 2 then 'Admin' when 3 then 'Spender' when 4 then 'Viewer' else 'Unknown' end as Name,CAST(RoleId AS INT) as 'Key' from WalletAuthorizeUserMaster where Status<9 group by RoleID").ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region User Role Methods

        public long GetUserRoleCount(short? status)
        {
            try
            {
                var items = (from o in _dbContext.UserRoleMaster
                             where o.Status < 9 && (status == null || (o.Status == status && status != null))
                             select o
                            ).Count();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<RoleMasterRes> ListRoleDetail(short? status)
        {
            try
            {
                List<RoleMasterRes> obj = (from u in _dbContext.UserRoleMaster
                                           where u.Status < 9 && (status == null || (u.Status == status && status != null))
                                           select new RoleMasterRes
                                           {
                                               ID = u.Id,
                                               RoleName = u.RoleName,
                                               Status = u.Status
                                           }
                                           ).AsEnumerable().ToList();
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Authorized Apps

        public List<AuthAppRes> GetAllAuthAppDetail(short? status)
        {
            try
            {
                var data = _dbContext.AuthAppRes.FromSql(@"SELECT ROW_NUMBER() OVER (ORDER BY Id) AS 'AutoNo',Id AS 'AppID',AppName,SecretKey," +
                    "SiteURL,Status,CASE Status WHEN 1 THEN 'Active' WHEN 9 THEN 'Delete' ELSE 'Inactive' END AS 'StrStatus' " +
                    "FROM AutorizedApps WHERE Status < 9 AND (Status={0} OR {0}=999)", (status == null ? 999 : status));
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        public List<AuthAppRes> GetAuthAppDetail(long appId, short? status)
        {
            try
            {
                IQueryable<AuthAppRes> data = _dbContext.AuthAppRes.FromSql(@"SELECT ROW_NUMBER() OVER (ORDER BY Id) AS 'AutoNo',Id AS 'AppID',AppName,SecretKey," +
                    "SiteURL,Status,CASE Status WHEN 1 THEN 'Active' WHEN 9 THEN 'Delete' ELSE 'Inactive' END AS 'StrStatus' " +
                    "FROM AutorizedApps WHERE Id={0} AND Status < 9 AND (Status={1} OR {1}=999)", appId, (status == null ? 999 : status));
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Currency Methods

        public long GetCurrencyCount(short? status)
        {
            try
            {
                var items = (from o in _dbContext.CurrencyTypeMaster
                             where o.Status < 9 && (status == null || (o.Status == status && status != null))
                             select o
                            ).Count();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<CurrencyMasterRes> ListCurrencyDetail(short? status)
        {
            try
            {
                List<CurrencyMasterRes> obj = (from u in _dbContext.CurrencyTypeMaster
                                               where u.Status < 9 && (status == null || (u.Status == status && status != null))
                                               select new CurrencyMasterRes
                                               {
                                                   ID = u.Id,
                                                   CurrencyName = u.CurrencyTypeName,
                                                   Status = u.Status
                                               }
                                           ).AsEnumerable().ToList();
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long GetMaxPlusOne()
        {
            try
            {
                var data = _dbContext.CurrencyTypeMaster.Max(item => item.CurrencyTypeId);
                return Convert.ToInt64(data + 1);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long GetMaxCommissionId()
        {
            try
            {
                var data = _dbContext.CommissionTypeMaster.Max(item => item.Id);
                return Convert.ToInt64(data + 1);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region WalletPolicyAllowedDay

        public List<WalletPolicyAllowedDayRes> GetWPolicyAllowedDays(long ID, EnWeekDays? DayNo, long? PolicyID, short? Status)
        {
            try
            {
                string Query = "SELECT ROW_NUMBER() OVER(ORDER BY WPA.Id) AS 'AutoNo',WPA.Id,WPA.WalletPolicyID," +
                                "ISNULL(WUP.PolicyName, '-') AS 'PolicyName',ISNULL(WUP.WalletType, '-') AS 'WalletType',WPA.DayNo," +
                                "WPA.Status,CASE WPA.Status WHEN 0 THEN 'Inactive' WHEN 1 THEN 'Active' ELSE 'Delete' END AS 'StrStatus' " +
                                "FROM WalletPolicyAllowedDay WPA INNER JOIN WalletUsagePolicy WUP ON WUP.Id = WPA.WalletPolicyID " +
                                "WHERE WPA.Status < 9 AND WPA.Id = {0} AND (WPA.Status={1} OR {1}=999) AND (WPA.DayNo={2} OR {2}=0) AND (WPA.WalletPolicyID={3} OR {3}=0)";

                var items = _dbContext.WalletPolicyAllowedDayRes.FromSql(Query, ID, (Status == null ? 999 : Status), (DayNo == null ? 0 : DayNo), (PolicyID == null ? 0 : PolicyID));

                return items.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        public List<WalletPolicyAllowedDayRes> ListWPolicyAllowedDays(EnWeekDays? DayNo, long? PolicyID, short? Status)
        {
            try
            {
                string Query = "SELECT ROW_NUMBER() OVER(ORDER BY WPA.Id) AS 'AutoNo',WPA.Id,WPA.WalletPolicyID," +
                                "ISNULL(WUP.PolicyName, '-') AS 'PolicyName',ISNULL(WUP.WalletType, '-') AS 'WalletType',WPA.DayNo," +
                                "WPA.Status,CASE WPA.Status WHEN 0 THEN 'Inactive' WHEN 1 THEN 'Active' ELSE 'Delete' END AS 'StrStatus' " +
                                "FROM WalletPolicyAllowedDay WPA INNER JOIN WalletUsagePolicy WUP ON WUP.Id = WPA.WalletPolicyID " +
                                "WHERE WPA.Status < 9 AND (WPA.Status={0} OR {0}=999) AND (WPA.DayNo={1} OR {1}=0) AND (WPA.WalletPolicyID={2} OR {2}=0)";

                var items = _dbContext.WalletPolicyAllowedDayRes.FromSql(Query, (Status == null ? 999 : Status), (DayNo == null ? 0 : DayNo), (PolicyID == null ? 0 : PolicyID));

                return items.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass AddWPolicyAllowedDay(short[] DayNo, long WalletPolicyID, long UserId, short type)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                short[] internalDay = { 1, 2, 3, 4, 5, 6, 7 };
                var PolicyObj = from a in internalDay
                                where !_dbContext.WalletPolicyAllowedDay.Any(i => i.WalletPolicyID == WalletPolicyID && i.DayNo == a)
                                select new WalletPolicyAllowedDay
                                {
                                    CreatedBy = UserId,
                                    CreatedDate = Helpers.UTC_To_IST(),
                                    Status = 1,
                                    UpdatedDate = Helpers.UTC_To_IST(),
                                    WalletPolicyID = WalletPolicyID,
                                    DayNo = a
                                };
                _dbContext.WalletPolicyAllowedDay.AddRange(PolicyObj);
                _dbContext.SaveChanges();

                var existObj = (from p in _dbContext.WalletPolicyAllowedDay
                                where p.WalletPolicyID == WalletPolicyID
                                select p).ToList();

                if (type == 2)
                {
                    if (existObj.Count() != 0)
                    {
                        existObj.ForEach(a =>
                        {
                            a.Status = 0;
                            a.UpdatedBy = UserId;
                            a.UpdatedDate = Helpers.UTC_To_IST();
                        }
                       );
                        _dbContext.SaveChanges();
                    }
                }
                var IsExist = (from p in existObj
                               join q in DayNo on p.DayNo equals q
                               where p.DayNo == q
                               select p).ToList();

                if (IsExist != null)
                {
                    IsExist.ForEach(a =>
                    {
                        a.Status = 1;
                        a.UpdatedBy = UserId;
                        a.UpdatedDate = Helpers.UTC_To_IST();
                    }
            );
                    _dbContext.SaveChanges();
                }
                #region OldCode
                //var IsExist = (from p in _dbContext.WalletPolicyAllowedDay
                //               join q in DayNo on p.DayNo equals q
                //               where p.WalletPolicyID == WalletPolicyID && p.DayNo == q
                //               select p).ToList();

                //if (IsExist.Any(i => i.Status == 9 || i.Status == 0))
                //{
                //    //var PolicyObjUp = from a in IsExist
                //    //                  where _dbContext.WalletPolicyAllowedDay.Any(i => i.DayNo == a.DayNo && i.WalletPolicyID == WalletPolicyID)
                //    //                  select new WalletPolicyAllowedDay
                //    //                  {
                //    //                      UpdatedBy = UserId,
                //    //                      Status = Convert.ToInt16(1),
                //    //                      UpdatedDate = Helpers.UTC_To_IST()
                //    //                  };
                //    //_dbContext.WalletPolicyAllowedDay.UpdateRange(PolicyObjUp);
                //    //_dbContext.SaveChanges();
                //    IsExist.ForEach(a =>
                //      {
                //          a.Status = 1;
                //          a.UpdatedBy = UserId;
                //          a.UpdatedDate = Helpers.UTC_To_IST();
                //      }
                //    );
                //    _dbContext.SaveChanges();
                //    Resp.ReturnCode = enResponseCode.Success;
                //    Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                //    Resp.ErrorCode = enErrorCode.Success;
                //}
                //if (IsExist.Any(i => i.Status == 1))
                //{
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = EnResponseMessage.Alredy_Exist;
                //    Resp.ErrorCode = enErrorCode.Alredy_Exist;
                //}

                //var PolicyObj = from a in DayNo
                //                where !_dbContext.WalletPolicyAllowedDay.Any(i => i.DayNo == a && i.WalletPolicyID == WalletPolicyID)
                //                select new WalletPolicyAllowedDay
                //                {
                //                    CreatedBy = UserId,
                //                    CreatedDate = Helpers.UTC_To_IST(),
                //                    Status = 1,
                //                    UpdatedDate = Helpers.UTC_To_IST(),
                //                    WalletPolicyID = WalletPolicyID,
                //                    DayNo = a
                //                };
                //_dbContext.WalletPolicyAllowedDay.AddRange(PolicyObj);
                //var affcted = _dbContext.SaveChanges();
                //if (affcted == 0)
                //{
                //    Resp.ReturnCode = enResponseCode.Fail;
                //    Resp.ReturnMsg = EnResponseMessage.Alredy_Exist;
                //    Resp.ErrorCode = enErrorCode.Alredy_Exist;
                //    return Resp;
                //}
                #endregion
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                Resp.ErrorCode = enErrorCode.Success;

                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddWPolicyAllowedDay", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion


        #region Wallet Type Methods

        public WalletTypeMasterResp GetWalletTypeDetails(long typeID)
        {
            try
            {
                var data = from c in _dbContext.WalletTypeMasters
                           where c.Status < 9 && c.Id == typeID
                           select new WalletTypeMasterResp
                           {
                               ID = c.Id,
                               TypeName = c.WalletTypeName,
                               CurrencyTypeID = c.CurrencyTypeID,
                               Description = c.Description,
                               Status = c.Status,
                           };
                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long GetWalletTypeCount(short? status, long? CurrencyType)
        {
            try
            {
                var items = (from o in _dbContext.WalletTypeMasters
                             where o.Status < 9 && (status == null || (o.Status == status && status != null))
                             && (CurrencyType == null || (o.CurrencyTypeID == CurrencyType && CurrencyType != null))
                             select o
                            ).Count();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<WalletTypeMasterRes> ListWalletTypeDetail(short? status, long? ServiceProviderId, long? CurrencyType, short IsMargin)
        {
            try
            {
                List<WalletTypeMasterRes> obj;
                if (IsMargin == 1)
                {
                    obj = (from u in _dbContext.MarginWalletTypeMaster ////join w in _dbContext.Wallet_TypeMaster on u.WalletTypeName equals w.WalletTypeName
                           where u.Status < 9 && (status == null || (u.Status == status && status != null))
                           && (CurrencyType == null || (u.CurrencyTypeID == CurrencyType && CurrencyType != null))
                           select new WalletTypeMasterRes
                           {
                               ID = u.Id,
                               TypeName = u.WalletTypeName,
                               CurrencyTypeID = u.CurrencyTypeID,
                               Description = u.Description,
                               Status = u.Status
                           }
                                               ).AsEnumerable().ToList();
                }
                else
                {
                    if (ServiceProviderId != null)
                    {
                        obj = (from u in _dbContext.WalletTypeMasters ////join w in _dbContext.Wallet_TypeMaster on u.WalletTypeName equals w.WalletTypeName
                               join p in _dbContext.ProviderWalletTypeMapping on u.Id equals p.WalletTypeId
                               where u.Status < 9 && (status == null || (u.Status == status && status != null))
                               && (CurrencyType == null || (u.CurrencyTypeID == CurrencyType && CurrencyType != null)) && p.ServiceProviderId == ServiceProviderId
                               select new WalletTypeMasterRes
                               {
                                   ID = u.Id,
                                   TypeName = u.WalletTypeName,
                                   CurrencyTypeID = u.CurrencyTypeID,
                                   Description = u.Description,
                                   Status = u.Status
                               }
                                             ).AsEnumerable().ToList();
                    }
                    else
                    {
                        obj = (from u in _dbContext.WalletTypeMasters ////join w in _dbContext.Wallet_TypeMaster on u.WalletTypeName equals w.WalletTypeName
                               where u.Status < 9 && (status == null || (u.Status == status && status != null))
                               && (CurrencyType == null || (u.CurrencyTypeID == CurrencyType && CurrencyType != null))
                               select new WalletTypeMasterRes
                               {
                                   ID = u.Id,
                                   TypeName = u.WalletTypeName,
                                   CurrencyTypeID = u.CurrencyTypeID,
                                   Description = u.Description,
                                   Status = u.Status
                               }
                                             ).AsEnumerable().ToList();
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Charge Type Methods

        //vsolanki 24-11-2018
        public long GetChargeTypeCount(short? status, long? ChargeTypeID)
        {
            try
            {
                var items = (from o in _dbContext.ChargeTypeMaster
                             where o.Status < 9 && (status == null || (o.Status == status && status != null))
                             && (ChargeTypeID == null || (o.Id == ChargeTypeID && ChargeTypeID != null))
                             select o
                            ).Count();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsolanki 24-11-2018
        public List<ChargeTypeMasterRes> ListChargeTypeDetail(short? status, long? ChargeTypeID)
        {
            try
            {
                List<ChargeTypeMasterRes> obj = (from u in _dbContext.ChargeTypeMaster
                                                 where u.Status < 9 && (status == null || (u.Status == status && status != null))
                                                 && (ChargeTypeID == null || (u.Id == ChargeTypeID && ChargeTypeID != null))
                                                 select new ChargeTypeMasterRes
                                                 {
                                                     TypeName = u.ChargeName,
                                                     ChargeTypeID = u.Id,
                                                     Status = u.Status
                                                 }
                                           ).AsEnumerable().ToList();
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //2018-12-14
        public BizResponseClass InsertUpdateChargeType(ChargeTypeReq Req, long UserId)
        {
            ChargeTypeMaster Obj = new ChargeTypeMaster();
            try
            {
                if (Req == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidInput, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput };
                }
                var ChargeTypeObj = (from c in _dbContext.ChargeTypeMaster
                                     where c.Id == Req.TypeId
                                     select c).FirstOrDefault();

                if (ChargeTypeObj != null)
                {
                    //update
                    ChargeTypeObj.Status = Req.Status;
                    ChargeTypeObj.ChargeName = Req.TypeName;
                    ChargeTypeObj.UpdatedBy = UserId;
                    ChargeTypeObj.UpdatedDate = Helpers.UTC_To_IST();
                    _dbContext.ChargeTypeMaster.Update(ChargeTypeObj);
                    _walletConfiguration.UpdateChargeTypeMasterList();
                    _dbContext.SaveChanges();
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
                var data = _dbContext.ChargeTypeMaster.Max(item => item.Id);
                Obj.Id = Convert.ToInt64(data + 1);
                Obj.Status = Req.Status;
                Obj.UpdatedBy = null;
                Obj.UpdatedDate = Helpers.UTC_To_IST();
                Obj.CreatedBy = UserId;
                Obj.CreatedDate = Helpers.UTC_To_IST();
                Obj.ChargeName = Req.TypeName;
                //insert
                _dbContext.ChargeTypeMaster.Add(Obj);
                _dbContext.SaveChanges();

                return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public BizResponseClass ChangeChargeTypeStatus(long Id, short Status, long UserId)
        {
            try
            {
                var ChargeTypeObj = (from c in _dbContext.ChargeTypeMaster
                                     where c.Id == Id
                                     select c).FirstOrDefault();
                if (ChargeTypeObj != null)
                {
                    //update
                    ChargeTypeObj.Status = Status;
                    ChargeTypeObj.UpdatedBy = UserId;
                    ChargeTypeObj.UpdatedDate = Helpers.UTC_To_IST();
                    _dbContext.ChargeTypeMaster.Update(ChargeTypeObj);
                    _dbContext.SaveChanges();
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
                return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region Commission Type Methods

        //vsolanki 24-11-2018
        public long GetCommissionTypeCount(short? status, long? CommissionTypeID)
        {
            try
            {
                var items = (from o in _dbContext.CommissionTypeMaster
                             where o.Status < 9 && (status == null || (o.Status == status && status != null))
                             && (CommissionTypeID == null || (o.Id == CommissionTypeID && CommissionTypeID != null))
                             select o
                            ).Count();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsolanki 24-11-2018
        public List<CommissionTypeMasterRes> ListCommisssionTypeDetail(short? status, long? CommissionTypeID)
        {
            try
            {
                List<CommissionTypeMasterRes> obj = (from u in _dbContext.CommissionTypeMaster
                                                     where u.Status < 9 && (status == null || (u.Status == status && status != null))
                                                     && (CommissionTypeID == null || (u.Id == CommissionTypeID && CommissionTypeID != null))
                                                     select new CommissionTypeMasterRes
                                                     {
                                                         TypeName = u.TypeName,
                                                         TypeID = u.Id,
                                                         Status = u.Status
                                                     }
                                           ).AsEnumerable().ToList();
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Charge Policy Methods

        //vsolanki 24-11-2018
        public long GetChargePolicyRecCount(long? WalletTypeID, short? status, long? WalletTrnTypeID)
        {
            try
            {
                var items = (from o in _dbContext.ChargePolicy
                             where o.Status < 9 && (status == null || (o.Status == status && status != null))
                             && (WalletTrnTypeID == null || (o.WalletTrnType == WalletTrnTypeID && WalletTrnTypeID != null))
                              && (WalletTypeID == null || (o.WalletType == WalletTypeID && WalletTypeID != null))
                             select o
                ).Count();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsolanki 24-11-2018
        public List<CommonCountClass> GetChargePolicyWalletTypeWiseCount()
        {
            try
            {
                var items = (from p in _dbContext.ChargePolicy.Where(p => p.Status < 9).GroupBy(p => p.WalletType)
                             select new CommonCountClass
                             {
                                 Count = p.Count(),
                                 Key = Convert.ToInt32(p.First().WalletType)
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

        //vsolanki 24-11-2018
        public List<CommonCountClass> GetChargePolicyStatusWiseCount()
        {
            try
            {
                var items = (from p in _dbContext.ChargePolicy.Where(p => p.Status < 9).GroupBy(p => p.Status)
                             select new CommonCountClass
                             {
                                 Count = p.Count(),
                                 Key = p.First().Status
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

        //vsolanki 24-11-2018
        public List<CommonCountClass> GetChargePolicyWalletTrnTypeWiseCount()
        {
            try
            {
                var items = (from p in _dbContext.ChargePolicy.Where(p => p.Status < 9).GroupBy(p => p.WalletTrnType)
                             select new CommonCountClass
                             {
                                 Count = p.Count(),
                                 Key = Convert.ToInt32(p.First().WalletTrnType)
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

        //vsolanki 24-11-2018
        public List<ChargePolicyRes> GetChargePolicyList(short? status, long? WalletType, long? WalletTrnType)
        {
            try
            {
                List<ChargePolicyRes> obj = (from u in _dbContext.ChargePolicy
                                             where u.Status < 9 && (status == null || (u.Status == status && status != null))
                                             && (WalletType == null || (u.WalletType == WalletType && WalletType != null))
                                              && (WalletTrnType == null || (u.WalletTrnType == WalletTrnType && WalletTrnType != null))
                                             select new ChargePolicyRes
                                             {
                                                 Status = u.Status,
                                                 StrStatus = (u.Status == 0) ? "Inactive" : (u.Status == 1) ? "Active" : "Deleted",
                                                 Id = u.Id,
                                                 PolicyName = u.PolicyName,
                                                 WalletTrnType = u.WalletTrnType,
                                                 MinAmount = u.MinAmount,
                                                 MaxAmount = u.MaxAmount,
                                                 WalletType = u.WalletType,
                                                 Type = u.Type,
                                                 ChargeType = u.ChargeType,
                                                 ChargeValue = u.ChargeValue
                                             }
                                           ).AsEnumerable().ToList();
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsolanki 27-11-2018
        public List<ChargePolicyRes> ListChargePolicyLast5()
        {
            try
            {
                List<ChargePolicyRes> obj = (from u in _dbContext.ChargePolicy.Where(u => u.Status < 9)
                                             orderby u.CreatedDate descending
                                             select new ChargePolicyRes
                                             {
                                                 Status = u.Status,
                                                 StrStatus = (u.Status == 0) ? "Inactive" : (u.Status == 1) ? "Active" : "Deleted",
                                                 Id = u.Id,
                                                 PolicyName = u.PolicyName,
                                                 WalletTrnType = u.WalletTrnType,
                                                 MinAmount = u.MinAmount,
                                                 MaxAmount = u.MaxAmount,
                                                 WalletType = u.WalletType,
                                                 Type = u.Type,
                                                 ChargeType = u.ChargeType,
                                                 ChargeValue = u.ChargeValue
                                             }
                                           ).Take(5).AsEnumerable().ToList();
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region Commission Policy Methods

        //vsolanki 24-11-2018
        public long GetCommissionPolicyRecCount(long? WalletTypeID, short? status, long? WalletTrnTypeID)
        {
            try
            {
                var items = (from o in _dbContext.CommissionPolicy
                             where o.Status < 9 && (status == null || (o.Status == status && status != null))
                             && (WalletTrnTypeID == null || (o.WalletTrnType == WalletTrnTypeID && WalletTrnTypeID != null))
                              && (WalletTypeID == null || (o.WalletType == WalletTypeID && WalletTypeID != null))
                             select o
                                ).Count();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsolanki 24-11-2018
        public List<CommonCountClass> GetCommissionPolicyWalletTypeWiseCount()
        {
            try
            {
                var items = (from p in _dbContext.CommissionPolicy.Where(p => p.Status < 9).GroupBy(p => p.WalletType)
                             select new CommonCountClass
                             {
                                 Count = p.Count(),
                                 Key = Convert.ToInt32(p.First().WalletType)
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

        //vsolanki 24-11-2018
        public List<CommonCountClass> GetCommissionPolicyStatusWiseCount()
        {
            try
            {
                var items = (from p in _dbContext.CommissionPolicy.Where(p => p.Status < 9).GroupBy(p => p.Status)
                             select new CommonCountClass
                             {
                                 Count = p.Count(),
                                 Key = p.First().Status
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

        //vsolanki 24-11-2018
        public List<CommonCountClass> GetCommissionPolicyWalletTrnTypeWiseCount()
        {
            try
            {
                var items = (from p in _dbContext.CommissionPolicy.Where(p => p.Status < 9).GroupBy(p => p.WalletTrnType)
                             select new CommonCountClass
                             {
                                 Count = p.Count(),
                                 Key = Convert.ToInt32(p.First().WalletTrnType)
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

        //vsolanki 24-11-2018
        public List<CommissionPolicyRes> GetCommissionPolicyList(short? status, long? WalletType, long? WalletTrnType)
        {
            try
            {
                List<CommissionPolicyRes> obj = (from u in _dbContext.CommissionPolicy
                                                 where u.Status < 9 && (status == null || (u.Status == status && status != null))
                                                 && (WalletType == null || (u.WalletType == WalletType && WalletType != null))
                                                  && (WalletTrnType == null || (u.WalletTrnType == WalletTrnType && WalletTrnType != null))
                                                 select new CommissionPolicyRes
                                                 {
                                                     Status = u.Status,
                                                     StrStatus = (u.Status == 0) ? "Inactive" : (u.Status == 1) ? "Active" : "Deleted",
                                                     Id = u.Id,
                                                     PolicyName = u.PolicyName,
                                                     WalletTrnType = u.WalletTrnType,
                                                     MinAmount = u.MinAmount,
                                                     MaxAmount = u.MaxAmount,
                                                     WalletType = u.WalletType,
                                                     Type = u.Type,
                                                     CommissionType = u.CommissionType,
                                                     CommissionValue = u.CommissionValue
                                                 }
                                           ).AsEnumerable().ToList();
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsolanki 27-11-2018
        public List<CommissionPolicyRes> ListCommissionPolicy()
        {
            try
            {
                List<CommissionPolicyRes> obj = (from u in _dbContext.CommissionPolicy.Where(p => p.Status < 9)
                                                 orderby u.CreatedDate descending
                                                 select new CommissionPolicyRes
                                                 {
                                                     Status = u.Status,
                                                     StrStatus = (u.Status == 0) ? "Inactive" : (u.Status == 1) ? "Active" : "Deleted",
                                                     Id = u.Id,
                                                     PolicyName = u.PolicyName,
                                                     WalletTrnType = u.WalletTrnType,
                                                     MinAmount = u.MinAmount,
                                                     MaxAmount = u.MaxAmount,
                                                     WalletType = u.WalletType,
                                                     Type = u.Type,
                                                     CommissionType = u.CommissionType,
                                                     CommissionValue = u.CommissionValue
                                                 }
                                           ).Take(5).AsEnumerable().ToList();
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Graph API

        //vsoalnki 2018-11-27
        public ListWalletGraphRes GraphForUserCount()
        {
            try
            {
                List<WalletGraphRes> res = new List<WalletGraphRes>();

                IQueryable<WalletGraphRes> Result = _dbContext.WalletGraphRes.FromSql(
                  @"SELECT DATEPART(mm,CreatedDate) Month,count(Id) AS TotalCount FROM UserMaster U WHERE U.Status < 9 AND U.CreatedDate > DATEADD(year,-1,dbo.GetISTDate()) GROUP BY DATEPART(mm,CreatedDate),DATEPART(yy,CreatedDate) ORDER BY  DATEPART(yy,CreatedDate) DESC,DATEPART(mm,CreatedDate) DESC");
                res = Result.ToList();
                List<int> monthlist = new List<int>();
                List<int> countlist = new List<int>();
                List<string> monthStrlist = new List<string>();
                foreach (var a in res)
                {
                    monthlist.Add(a.Month);
                    countlist.Add(a.TotalCount);
                    monthStrlist.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(a.Month));
                }
                ListWalletGraphRes GraphList = new ListWalletGraphRes();
                GraphList.TotalCount = countlist;
                GraphList.Month = monthStrlist;
                return GraphList;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }

        }

        //Rushabh 27/11/2018
        public ListWalletGraphRes GraphForOrgCount()
        {
            try
            {
                List<WalletGraphRes> res = new List<WalletGraphRes>();

                IQueryable<WalletGraphRes> Result = _dbContext.WalletGraphRes.FromSql(
                  @"SELECT DATEPART(mm,CreatedDate) Month,count(Id) AS TotalCount FROM OrganizationMaster O WHERE O.Status < 9 AND O.CreatedDate > DATEADD(year,-1,dbo.GetISTDate()) GROUP BY DATEPART(mm,CreatedDate),DATEPART(yy,CreatedDate) ORDER BY  DATEPART(yy,CreatedDate) DESC,DATEPART(mm,CreatedDate) DESC");
                res = Result.ToList();
                List<int> monthlist = new List<int>();
                List<int> countlist = new List<int>();
                List<string> monthStrlist = new List<string>();
                foreach (var a in res)
                {
                    monthlist.Add(a.Month);
                    countlist.Add(a.TotalCount);
                    monthStrlist.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(a.Month));
                }
                ListWalletGraphRes GraphList = new ListWalletGraphRes();
                GraphList.TotalCount = countlist;
                GraphList.Month = monthStrlist;
                return GraphList;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }

        }

        //Rushabh 28/11/2018
        public ListTransactionTypewiseCount GraphForTrnTypewiseCount()
        {
            try
            {
                List<TransactionTypewiseCount> res = new List<TransactionTypewiseCount>();

                IQueryable<TransactionTypewiseCount> Result = _dbContext.TransactionTypewiseCount.FromSql(
                        @"SELECT WalletTrnType AS 'TranType',count(TrnNo) AS TotalCount FROM WalletTransactionQueues O 
                        WHERE O.TrnDate > DATEADD(year,-1,dbo.GetISTDate()) AND O.Status=1 AND O.WalletTrnType <> 0
                        GROUP BY DATEPART(yy,TrnDate),WalletTrnType
                        ORDER BY  DATEPART(yy,TrnDate) DESC");

                res = Result.ToList();
                List<int> countlist = new List<int>();
                List<int> TrnTypelist = new List<int>();
                List<string> StrType = new List<string>();
                foreach (var a in res)
                {
                    countlist.Add(a.TotalCount);
                    TrnTypelist.Add(a.TranType);
                    StrType.Add((a.TranType == 1) ? "Topup Credit" : (a.TranType == 2) ? "Deposit Credit" : (a.TranType == 3) ? "BuyTrade Credit" : (a.TranType == 4) ? "Refund Credit" : (a.TranType == 5) ? "Commission Credit" : (a.TranType == 6) ? "PartialCancel Credit" : (a.TranType == 7) ? "TransferIn Credit" : (a.TranType == 8) ? "SellTrade Debit" : (a.TranType == 9) ? "Withdrawal Debit" : (a.TranType == 10) ? "Ecommerce Debit" : (a.TranType == 11) ? "Charges Debit" : (a.TranType == 12) ? "TransferOut Debit" : (a.TranType == 13) ? "Freez Debit" : (a.TranType == 14) ? "Stacking Debit" : (a.TranType == 15) ? "Escrow Debit" : (a.TranType == 16) ? "Bonus Credit" : (a.TranType == 17) ? "Deposit Debit" : "Unknown");
                }
                ListTransactionTypewiseCount GraphList = new ListTransactionTypewiseCount();
                GraphList.TotalCount = countlist;
                GraphList.TypeName = StrType;
                return GraphList;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }

        }

        #endregion

        #region Wallet Usage Policy

        public List<WalletusagePolicyRes2> ListUsagePolicyLast5()
        {
            try
            {
                string Query = "SELECT top 5 WT.Id as WalletTypeId,WUP.Id,ROW_NUMBER() OVER (ORDER BY WUP.CreatedDate DESC) 'AutoNo',WUP.PolicyName,WUP.AllowedIP,WUP.AllowedLocation,WUP.AuthenticationType,ISNULL(WUP.StartTime,0) AS 'StartTime',ISNULL(WUP.EndTime,0) AS 'EndTime',WUP.HourlyTrnCount,WUP.HourlyTrnAmount,WUP.DailyTrnCount,WUP.DailyTrnAmount,                WUP.MonthlyTrnCount,WUP.MonthlyTrnAmount,WUP.WeeklyTrnCount,WUP.WeeklyTrnAmount,WUP.YearlyTrnCount,WUP.YearlyTrnAmount,WUP.LifeTimeTrnCount,WUP.LifeTimeTrnAmount,WUP.MinAmount, WUP.MaxAmount,WT.WalletTypeName AS 'WalletType',WUP.Status, CASE WUP.Status WHEN 0 THEN 'Inactive' WHEN 1 THEN 'Active' ELSE 'Delete' END AS 'StrStatus',(select (SUBSTRING((SELECT ','+ cast (dayno as varchar)  AS [text()] FROM WalletPolicyAllowedDay WHERE Status=1 and walletpolicyid=WUP.Id FOR XML PATH ('')), 2, 1000) ) as [DayNo]) as DayNo FROM WalletUsagePolicy WUP INNER JOIN WalletTypeMasters WT ON WT.Id = WUP.WalletType WHERE WUP.Status < 9  ORDER BY WUP.CreatedDate DESC";
                var data = _dbContext.WalletusagePolicyRes.FromSql(Query).ToList();
                //int[] DayNo = data.FirstOrDefault().DayNo.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                return data;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<WalletusagePolicyRes2> ListUsagePolicyData(long? walletTypeID, short? status)
        {
            try
            {
                string Query = "SELECT WT.Id as WalletTypeId,WUP.Id,ROW_NUMBER() OVER (ORDER BY WUP.CreatedDate DESC) 'AutoNo',WUP.PolicyName,WUP.AllowedIP,WUP.AllowedLocation,WUP.AuthenticationType,ISNULL(WUP.StartTime,0) AS 'StartTime',ISNULL(WUP.EndTime,0) AS 'EndTime',WUP.HourlyTrnCount,WUP.HourlyTrnAmount,WUP.DailyTrnCount,WUP.DailyTrnAmount,                WUP.MonthlyTrnCount,WUP.MonthlyTrnAmount,WUP.WeeklyTrnCount,WUP.WeeklyTrnAmount,WUP.YearlyTrnCount,WUP.YearlyTrnAmount,WUP.LifeTimeTrnCount,WUP.LifeTimeTrnAmount,WUP.MinAmount, WUP.MaxAmount,WT.WalletTypeName AS 'WalletType',WUP.Status, CASE WUP.Status WHEN 0 THEN 'Inactive' WHEN 1 THEN 'Active' ELSE 'Delete' END AS 'StrStatus',(select (SUBSTRING((SELECT ','+ cast (dayno as varchar)  AS [text()] FROM WalletPolicyAllowedDay WHERE Status=1 and walletpolicyid=WUP.Id FOR XML PATH ('')), 2, 1000) ) as [DayNo]) as DayNo FROM WalletUsagePolicy WUP INNER JOIN WalletTypeMasters WT ON WT.Id = WUP.WalletType WHERE WUP.Status < 9 AND (WUP.WalletType={0} OR {0}=0) AND (WUP.Status={1} OR {1}=999) ORDER BY WUP.CreatedDate DESC";
                var data = _dbContext.WalletusagePolicyRes.FromSql(Query, (walletTypeID == null ? 0 : walletTypeID), (status == null ? 999 : status));
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region WalletType

        //vsoalnki 2018-11-28
        public List<TypeRes> ListWalletType()
        {
            try
            {
                var items = (from w in _dbContext.WalletTypeMasters
                             where w.Status < 9
                             select new TypeRes
                             {
                                 Status = w.Status,
                                 TypeId = w.Id,
                                 TypeName = w.WalletTypeName
                             }).AsEnumerable().ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region TranTypeWiseReport

        public ListBlockTrnTypewiseReport GetBlockedTrnTypeWiseWalletData(enWalletTrnType type, int? PageNo, int? PageSize)
        {
            try
            {
                ListBlockTrnTypewiseReport res = new ListBlockTrnTypewiseReport();
                string Query = "SELECT ROW_NUMBER() OVER (ORDER BY UWB.Id) AS 'AutoNo',ISNULL(UWB.WalletID, 0) AS 'WalletID',ISNULL(WM.AccWalletID, '-') AS 'AccWalletID'," +
                    "ISNULL(WM.Walletname, '-') AS 'Walletname',ISNULL(WM.WalletTypeID, 0) AS 'WalletTypeID'," +
                    "ISNULL(WT.WalletTypeName, '-') AS 'Currency',ISNULL(WM.UserID, 0) AS 'UserID'," +
                    "ISNULL(BU.FirstName + ' ' + BU.LastName, '-')  AS 'UserName',ISNULL(WM.Balance, 0) AS 'Balance'," +
                    "ISNULL(WM.InBoundBalance, 0) AS 'InBoundBalance',ISNULL(WM.OutBoundBalance, 0) AS 'OutBoundBalance'," +
                    "ISNULL(UWB.Remarks,'-') AS 'Remarks',ISNULL(WM.Status, 0) AS 'Status' FROM UserWalletBlockTrnTypeMaster UWB " +
                    "LEFT JOIN WalletMasters WM ON WM.Id = UWB.WalletID " +
                    "LEFT JOIN WalletTypeMasters WT ON WT.Id = WM.WalletTypeID " +
                    "LEFT JOIN BizUser BU ON BU.Id = WM.UserID WHERE UWB.Status < 9 AND UWB.WTrnTypeMasterID = {0}";
                var Result = _dbContext.BlockTrnTypewiseReport.FromSql(Query, Convert.ToInt16(type));
                var pagesize = (PageSize == null) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                var it = Convert.ToDouble(Result.Count()) / pagesize;
                var fl = Math.Ceiling(it);
                res.TotalPages = Convert.ToInt64(fl);
                if (PageNo > 0)
                {
                    if (pagesize == 0)
                    {
                        int skip = Convert.ToInt32(Helpers.PageSize * (PageNo - 1));
                        Result = Result.Skip(skip).Take(Helpers.PageSize);
                    }
                    else
                    {
                        int skip = Convert.ToInt32((pagesize) * (PageNo - 1));
                        Result = Result.Skip(skip).Take(Convert.ToInt32(pagesize));
                    }
                }
                res.Details = Result.ToList();
                res.PageSize = (PageSize == null || pagesize == 0) ? Helpers.PageSize : Convert.ToInt64(PageSize);
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region WalletTrnType

        //vsoalnki 2018-11-28
        public List<TypeRes> ListWalletTrnType()
        {
            try
            {
                var items = (from w in _dbContext.WTrnTypeMaster
                             where w.Status < 9
                             select new TypeRes
                             {
                                 Status = w.Status,
                                 TypeId = w.TrnTypeId,
                                 TypeName = w.TrnTypeName
                             }).AsEnumerable().ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region BlockWalletTrnTypeMaster

        //vsoalnki 2018-11-28
        public List<TypeRes> GetBlockWTypewiseTrnTypeList(long WalletType)
        {
            try
            {
                var items = (from w in _dbContext.BlockWalletTrnTypeMaster
                             join q in _dbContext.WTrnTypeMaster
                             on w.TrnTypeID equals q.TrnTypeId
                             where w.Status < 9 && w.WalletTypeID == WalletType
                             select new TypeRes
                             {
                                 Status = (w.Status == Convert.ToInt16(0) ? Convert.ToInt16(1) : Convert.ToInt16(0)),
                                 TypeId = w.Id,
                                 TypeName = q.TrnTypeName
                             }).AsEnumerable().ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        #endregion

        #region Allowed Channels

        public List<ChannelMasterRes> GetChannelDetail(long channelID, short? status)
        {
            try
            {
                string Query = "SELECT ChannelID,ChannelName,Status,CASE Status WHEN 0 THEN 'Inactive' WHEN 1 THEN 'Active' ELSE 'Deleted' END AS 'StrStatus' " +
                                "FROM AllowedChannels WHERE Status < 9 AND ChannelID = {0} AND (Status = {1} OR {1}=999)";
                var items = _dbContext.ChannelMasterRes.FromSql(Query, channelID, (status == null ? 999 : status));

                return items.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ListChannels ListChannels()
        {
            ListChannels res = new ListChannels();
            try
            {
                List<ChannelMasterRes> items = (from c in _dbContext.AllowedChannels
                                                where c.Status < 9
                                                select new ChannelMasterRes
                                                {
                                                    ChannelId = c.ChannelID,
                                                    ChannelName = c.ChannelName,
                                                    Status = c.Status,
                                                    StrStatus = (c.Status == 1) ? "Active" : (c.Status == 2) ? "Inactive" : (c.Status == 9) ? "Deleted" : "Unknown"
                                                }
                                                ).AsEnumerable().ToList();
                res.Channels = items;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ProviderRes> ListProvider()
        {
            try
            {
                List<ProviderRes> items = (from c in _dbContext.ServiceProvider
                                           where c.Status < 9
                                           select new ProviderRes
                                           {
                                               ID = c.Id,
                                               ProviderName = c.ServiceProviderName,
                                               Status = c.Status,
                                               StrStatus = (c.Status == 1) ? "Active" : (c.Status == 2) ? "Inactive" : (c.Status == 9) ? "Deleted" : "Unknown"
                                           }
                                                ).AsEnumerable().ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long GetMaxPlusOneChannelID()
        {
            try
            {
                var data = _dbContext.AllowedChannels.Max(item => item.ChannelID);
                return Convert.ToInt64(data + 1);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Other Method

        public async Task<long> GetTypecount()
        {
            try
            {
                var WalletTypeCount = (from o in _dbContext.WalletTypeMasters
                                       where o.Status < 9

                                       select o
                ).Count();

                return WalletTypeCount;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTypecount", this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<long> GetWalletcount()
        {
            try
            {
                var WalletCount = (from o in _dbContext.WalletMasters
                                   where o.Status < 9
                                   select o
                ).Count();
                return WalletCount;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWalletcount", this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<long> GetUcount()
        {
            try
            {
                var UserCount = (from u in _dbContext.OrganizationUserMaster
                                 where u.Status < 9
                                 select u.Id
                                ).Count();

                return UserCount;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetUcount", this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<decimal> GetTotalBal()
        {
            try
            {
                var ToatalBalance = (from w in _dbContext.WalletMasters
                                     where w.Status < 9
                                     select w.Balance).Sum();

                return ToatalBalance;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTotalBal", this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<decimal> GetTotalBalTypeWise(long WalletType)
        {
            try
            {
                var ToatalBalance = (from w in _dbContext.WalletMasters
                                     where w.Status < 9 && w.WalletTypeID == WalletType
                                     select w.Balance).Sum();

                return ToatalBalance;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTotalBalTypeWise", this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<long> GetUserTypeWise(long WalletType)
        {
            try
            {
                var ToatalUser = (from w in _dbContext.WalletMasters
                                  where w.Status < 9 && w.WalletTypeID == WalletType
                                  select w.UserID).Count();

                return ToatalUser;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetUserTypeWise", this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<long> GetTranTypeWise(string WalletType)
        {
            try
            {
                var Toataltras = (from w in _dbContext.WalletTransactionQueues
                                  where w.WalletType == WalletType && w.Status == enTransactionStatus.Success
                                  select w.TrnNo).Count();

                return Toataltras;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTranTypeWise", this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<long> GetWalletTypeWise(long WalletType)
        {
            try
            {
                var wallet = (from w in _dbContext.WalletMasters //ntrivedi correction of table
                              where w.Status < 9 && w.WalletTypeID == WalletType
                              select w.Id).Count();

                return wallet;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWalletTypeWise", this.GetType().Name, ex);
                throw ex;
            }

        }
        //vsolanki 2018-12-3
        public List<TypeWiseDetail> GetDetailTypeWise()
        {
            try
            {   //ntrivedi 19-04-2019 walletmasters table instead of userwalletmaster
                List<TypeWiseDetail> res = new List<TypeWiseDetail>();
                IQueryable<TypeWiseDetail> Result = _dbContext.TypeWiseDetail.FromSql(
                  @"select wt.WalletTypeName as 'WalletType',wt.Id as 'WalletTypeId',COUNT(w.UserID) as 'UserCount',ISNULL(SUM(w.Balance),0) as 'ToatalBalance' ,Count(w.Id) as 'WalletCount',(select COUNT(TrnNo) from WalletTransactionQueues where WalletType=wt.WalletTypeName and Status=1) as 'TransactionCount' from WalletTypeMasters wt left join  WalletMasters w on w.WalletTypeID = wt.Id
  where wt.Status < 9 group by wt.WalletTypeName,wt.Id");
                res = Result.ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 2018-12-3
        public OrgDetail GetOrgAllDetail()
        {
            try
            {
                OrgDetail res = new OrgDetail();
                //2019-7-27 change count query
                IQueryable<OrgDetail> Result = _dbContext.OrgDetail.FromSql(
                  @"select(select Count(Id) from WalletTypeMasters where  Status < 9 ) as 'WalletTypeCount', (select Count(Id) from BizUser where IsEnabled<9 ) as 'UserCount',(SELECT count(*) FROM WalletMasters w INNER JOIN WalletTypeMasters wt ON wt.Id = w.WalletTypeID INNER JOIN BizUser b ON b.id =w.userid INNER JOIN organizationmaster o ON o.Id =w.OrgID where w.Status < 9 ) as  'WalletCount',isnull((SELECT sum(Balance) FROM WalletMasters w INNER JOIN WalletTypeMasters wt ON wt.Id = w.WalletTypeID INNER JOIN BizUser b ON b.id =w.userid INNER JOIN organizationmaster o ON o.Id =w.OrgID where w.Status < 9  ),0) as  'ToatalBalance'");
                res = Result.ToList().First();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }
        #endregion

        #region Channelwise Transaction Count

        public List<ChannelwiseTranCount> ChannelwiseTranCount()
        {
            try
            {
                List<ChannelwiseTranCount> res = new List<ChannelwiseTranCount>();
                IQueryable<ChannelwiseTranCount> Result = _dbContext.ChannelwiseTranCount.FromSql(
                  //@"SELECT U.AllowedChannelID AS 'ChannelID',C.ChannelName,count(U.TrnNo) AS 'TotalCount' FROM WalletTransactionQueues U INNER JOIN AllowedChannels C ON U.AllowedChannelID=C.ChannelID WHERE U.TrnDate BETWEEN DATEADD(MONTH,-1,dbo.GetISTDate()) AND dbo.GetISTDate() GROUP BY U.AllowedChannelID,C.ChannelName ");
                  @"SELECT C.ChannelID AS 'ChannelID',C.ChannelName,count(U.TrnNo) AS 'TotalCount' FROM AllowedChannels C LEFT JOIN WalletTransactionQueues U ON C.ChannelID = U.AllowedChannelID AND U.TrnDate BETWEEN DATEADD(MONTH,-1,dbo.GetISTDate()) AND dbo.GetISTDate() WHERE C.Status < 9 GROUP BY C.ChannelID,C.ChannelName");
                res = Result.ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region TransactionPolicy
        //2018-12-12
        public List<TransactionPolicyRes> ListTransactionPolicy()
        {
            try
            {
                var items = _dbContext.TransactionPolicyRes.FromSql(@"SELECT t.IsKYCEnable,t.Id ,w.TrnTypeName AS StrTrnType,t.Status,CASE t.Status WHEN 1 THEN 'Enable' WHEN 0 THEN 'Disable' ELSE 'Deleted' END AS StrStatus,t.TrnType,t.AllowedIP,t.AllowedLocation,t.AuthenticationType,t.StartTime,t.EndTime,t.DailyTrnCount,t.DailyTrnAmount,t.MonthlyTrnCount,t.MonthlyTrnAmount,t.WeeklyTrnCount,t.WeeklyTrnAmount,t.YearlyTrnCount,t.YearlyTrnAmount,t.MinAmount,t.MaxAmount,t.AuthorityType,t.AllowedUserType,r.Id as RoleId,r.RoleType as RoleName FROM TransactionPolicy t inner join  WTrnTypeMaster w ON w.TrnTypeId = t.TrnType inner join UserRoleMaster r on r.Id=t.RoleId WHERE t.Status < 9").ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        #endregion

        #region UserWalletAllowTrnType
        //2018-12-13
        public List<UserWalletBlockTrnTypeRes> ListUserWalletBlockTrnType(string WalletId, long? TrnTypeId)
        {
            try
            {
                var items = _dbContext.UserWalletBlockTrnTypeRes.FromSql(@"SELECT ISNULL(ut.Remarks,'Not Added') as Remarks ,ut.Id,wtn.TrnTypeId,wtn.TrnTypeName,w.AccWalletID as WalletId,w.Walletname AS WalletName,case ut.Status when 0 then CAST (1 AS smallint)  when 1 then CAST (0 AS smallint) end as  Status ,CASE ut.Status WHEN 0 THEN 'Enable' WHEN 1 THEN 'Disable' ELSE 'Deleted' END AS StrStatus,wt.WalletTypeName as WalletType FROM UserWalletBlockTrnTypeMaster ut INNER JOIN wTrnTypeMaster wtn ON wtn.TrnTypeId =ut.WTrnTypeMasterID INNER JOIN WalletMasters w ON w.Id =ut.WalletID INNER JOIN WalletTypeMasters wt ON wt.Id =w.WalletTypeID WHERE ut.Status < 9  AND (wtn.TrnTypeId={0} OR {0}=0 ) AND (w.AccWalletID={1} OR {1}='')", (TrnTypeId == null ? 0 : Convert.ToInt64(TrnTypeId)), (WalletId == null ? "" : WalletId)).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region User Types

        public List<UserTypeRes> ListAllUserTypes(short? status)
        {
            try
            {
                var items = _dbContext.UserTypeRes.FromSql(@"SELECT UserTypeId,UserType,Status,CASE Status WHEN 1 THEN 'Active' WHEN 9 THEN 'Delete' ELSE 'Inactive' END AS 'StrStatus' FROM UserTypeMaster WHERE Status < 9 AND (Status={0} OR {0}=0)", (status == null ? 0 : status));
                return items.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Log Listing Methods

        public List<UserActivityLoging> ListUserActivityData(long? userID, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                string Query = "SELECT ROW_NUMBER() OVER(ORDER BY UA.Id) AS 'AutoNo'," +
                                    "ISNULL(UA.ActivityType, 0) AS 'ActivityType'," +
                                    "ISNULL(UA.UserID, 0) AS 'UserID'," +
                                    "ISNULL(UA.WalletID, 0) AS 'WalletID'," +
                                    "ISNULL(UA.TrnNo, 0) AS 'TranNo'," +
                                    "ISNULL(UA.Remarks, '-') AS 'Remarks'," +
                                    "ISNULL(UA.WalletTrnType, 0) AS 'WalletTrnType'," +
                                    "ISNULL(WT.TrnTypeName, '-') AS 'WalletTrnTypeName'" +
                                    "FROM UserActivityLog UA " +
                                    "LEFT JOIN WTrnTypeMaster WT ON WT.TrnTypeId = UA.WalletTrnType " +
                                    "WHERE UA.Status < 9 AND (UA.UserID = {0} OR {0}=0) ";
                if (fromDate != null && toDate != null)
                {
                    toDate = Convert.ToDateTime(toDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += "AND UA.CreatedDate BETWEEN {1} AND {2}";
                    // var items = _dbContext.UserActivityLoging.FromSql(Query, (userID == null ? 0 : userID));
                }
                var items = _dbContext.UserActivityLoging.FromSql(Query, (userID == null ? 0 : userID), fromDate, toDate);
                return items.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region WalletAuthorizeUser
        //2018-12-13
        public List<WalletAuthorizeUserRes> ListWalletAuthorizeUser(string WalletId)
        {
            try
            {
                var items = _dbContext.WalletAuthorizeUserRes.FromSql(@"SELECT w.Walletname AS  WalletName,wt.WalletTypeName AS WalletType,o.OrganizationName AS OrgName,b.UserName AS UserName,ur.RoleName AS UserRoleName,wu.Status,CASE wu.Status WHEN 0 THEN 'Disable' WHEN 1 THEN 'Enable' ELSE 'Deleted' END AS StrStatus FROM WalletAuthorizeUserMaster wu INNER JOIN WalletMasters w ON w.Id=wu.WalletID INNER JOIN WalletTypeMasters wt ON wt.Id= w.WalletTypeID INNER JOIN OrganizationMaster o ON o.Id = wu.OrgID INNER JOIN BizUser b ON b.Id=wu.UserID INNER JOIN UserRoleMaster ur ON ur.Id =wu.RoleID WHERE wu.Status < 9 AND w.AccWalletID={0}", WalletId).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region AllowTrnTypeRoleWise

        //2018-12-22
        public async Task<List<AllowTrnTypeRoleWiseRes>> ListAllowTrnTypeRoleWise(long? RoleId, long? TrnTypeId, short? Status)
        {
            try
            {
                var items = _dbContext.AllowTrnTypeRoleWiseRes.FromSql(@"SELECT a.Id,a.RoleId,a.TrnTypeId,r.RoleType as RoleName,w.TrnTypeName,a.Status,CASE a.Status WHEN 0 THEN 'Disable' WHEN 1 THEN 'Enable' ELSE 'Deleted' END AS StrStatus FROM AllowTrnTypeRoleWise a INNER JOIN UserRoleMaster r ON r.Id=a.RoleId INNER JOIN WTrnTypeMaster w ON w.TrnTypeId=a.TrnTypeId WHERE a.Status < 9 AND (a.RoleId={0} OR {0}=0) AND  (a.TrnTypeId={1} OR {1}=0) AND (a.Status={2} OR {2}=999)", (RoleId == null ? 0 : RoleId), (TrnTypeId == null ? 0 : TrnTypeId), (Status == null ? 999 : Status)).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Staking Policy

        public List<UnStakingHistoryData> ListUnStakingHistoryData(long? userid, short? status, EnUnstakeType unStakingType, string HistoryId)
        {
            try
            {
                string query = "SELECT TSH.ID AS 'StakingHistoryId',TSH.StakingPolicyDetailID AS 'PolicyDetailID',TSH.StakingType," +
                            "CASE TSH.StakingType WHEN 1 THEN 'FD' WHEN 2 THEN 'Charge' END AS 'StakingTypeName',TSH.SlabType," +
                            "CASE TSH.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' END AS 'SlabTypeName',TSH.WalletTypeID," +
                            "ISNULL(WT.WalletTypeName, '-') AS 'StakingCurrency',TSH.InterestType," +
                            "ISNULL(CASE TSH.InterestType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Percentage' END, '-') AS 'InterestTypeName'," +
                            "TSH.InterestValue,SPD.StakingDurationWeek AS 'DurationWeek',SPD.StakingDurationMonth AS 'DurationMonth'," +
                            "TSH.InterestWalletTypeID,TSH.DeductionAmount," +
                            "ISNULL((SELECT WTM.WalletTypeName FROM WalletTypeMasters WTM WHERE WTM.id = TSH.InterestWalletTypeID),'-') AS 'MaturityCurrency'," +
                            "CASE TSH.SlabType WHEN 1 THEN CAST(TSH.MinAmount AS varchar) " +
                            "WHEN 2 THEN CAST(TSH.MinAmount AS varchar) +'-' + CAST(TSH.MaxAmount AS varchar) END AS 'AvailableAmount'," +
                            "TSH.MakerCharges,TSH.TakerCharges,TSH.EnableAutoUnstaking," +
                            "CASE TSH.EnableAutoUnstaking WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableAutoUnstaking',TSH.EnableStakingBeforeMaturity," +
                            "CASE TSH.EnableStakingBeforeMaturity WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableStakingBeforeMaturity'," +
                            "TSH.EnableStakingBeforeMaturityCharge,TSH.RenewUnstakingEnable," +
                            "CASE TSH.RenewUnstakingEnable WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrRenewUnstakingEnable',TSH.RenewUnstakingPeriod," +
                            "TSH.StakingAmount,TSH.MaturityDate,TSH.MaturityAmount,ChannelID,TSH.UserID," +
                            "(BU.FirstName + ' ' + BU.LastName) AS 'UserName',TSH.WalletID,ISNULL(WM.WalletName, '-') AS 'WalletName'," +
                            "TSH.WalletOwnerID,TSH.Status,TUH.TokenStakingHistoryID,TUH.AmountCredited,TUH.UnstakeType,TUH.InterestCreditedValue,TUH.ChargeBeforeMaturity," +
                            "CASE TUH.UnstakeType WHEN 1 THEN 'Full' WHEN 2 THEN 'Partial' END AS 'StrUnstakeType'," +
                            "TUH.DegradeStakingHistoryRequestID,TUH.CreatedDate AS 'UnstakingDate'" +
                            "FROM TokenStakingHistory TSH " +
                            "INNER JOIN StakingPolicyDetail SPD ON SPD.Id = TSH.StakingPolicyDetailID " +
                            "LEFT JOIN WalletTypeMasters WT ON WT.Id = TSH.WalletTypeID " +
                            "LEFT JOIN BizUser BU ON BU.Id = TSH.UserID " +
                            "LEFT JOIN TokenUnStakingHistory TUH ON TUH.TokenStakingHistoryID = TSH.Id " +
                            "LEFT JOIN WalletMasters WM ON WM.Id = TSH.WalletID " +
                            "WHERE TSH.Id IN(" + HistoryId + ") AND (TSH.Status={0} OR {0}=999) AND (TSH.UserID = {1} OR {1}=0) ORDER BY TUH.ID DESC ";
                //"AND (TSH.SlabType = {1} OR {1}=0) AND (TSH.StakingType = {2} OR {2}=0) AND (TSH.Status = {3} OR {3}=0) ";
                var data = _dbContext.UnStakingHistoryData.FromSql(query, Convert.ToInt16(status == null ? 999 : status), Convert.ToInt64(userid == null ? 0 : userid));
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<UnStakingHistoryDatav2> ListUnStakingHistoryDatav2(long? userid, short? status, EnUnstakeType unStakingType, string HistoryId)
        {
            try
            {
                string query = "SELECT ISNULL(TSH.GUID,'') AS 'StakingHistoryId',TSH.StakingPolicyDetailID AS 'PolicyDetailID',TSH.StakingType," +
                            "CASE TSH.StakingType WHEN 1 THEN 'FD' WHEN 2 THEN 'Charge' END AS 'StakingTypeName',TSH.SlabType," +
                            "CASE TSH.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' END AS 'SlabTypeName',TSH.WalletTypeID," +
                            "ISNULL(WT.WalletTypeName, '-') AS 'StakingCurrency',TSH.InterestType," +
                            "ISNULL(CASE TSH.InterestType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Percentage' END, '-') AS 'InterestTypeName'," +
                            "TSH.InterestValue,SPD.StakingDurationWeek AS 'DurationWeek',SPD.StakingDurationMonth AS 'DurationMonth'," +
                            "TSH.InterestWalletTypeID,TSH.DeductionAmount," +
                            "ISNULL((SELECT WTM.WalletTypeName FROM WalletTypeMasters WTM WHERE WTM.id = TSH.InterestWalletTypeID),'-') AS 'MaturityCurrency'," +
                            "CASE TSH.SlabType WHEN 1 THEN CAST(TSH.MinAmount AS varchar) " +
                            "WHEN 2 THEN CAST(TSH.MinAmount AS varchar) +'-' + CAST(TSH.MaxAmount AS varchar) END AS 'AvailableAmount'," +
                            "TSH.MakerCharges,TSH.TakerCharges,TSH.EnableAutoUnstaking," +
                            "CASE TSH.EnableAutoUnstaking WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableAutoUnstaking',TSH.EnableStakingBeforeMaturity," +
                            "CASE TSH.EnableStakingBeforeMaturity WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableStakingBeforeMaturity'," +
                            "TSH.EnableStakingBeforeMaturityCharge,TSH.RenewUnstakingEnable," +
                            "CASE TSH.RenewUnstakingEnable WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrRenewUnstakingEnable',TSH.RenewUnstakingPeriod," +
                            "TSH.StakingAmount,TSH.MaturityDate,TSH.MaturityAmount,ChannelID,TSH.UserID," +
                            "(BU.FirstName + ' ' + BU.LastName) AS 'UserName',TSH.WalletID,ISNULL(WM.WalletName, '-') AS 'WalletName'," +
                            "TSH.WalletOwnerID,TSH.Status,TUH.TokenStakingHistoryID,TUH.AmountCredited,TUH.UnstakeType,TUH.InterestCreditedValue,TUH.ChargeBeforeMaturity," +
                            "CASE TUH.UnstakeType WHEN 1 THEN 'Full' WHEN 2 THEN 'Partial' END AS 'StrUnstakeType'," +
                            "TUH.DegradeStakingHistoryRequestID,TUH.CreatedDate AS 'UnstakingDate'" +
                            "FROM TokenStakingHistory TSH " +
                            "INNER JOIN StakingPolicyDetail SPD ON SPD.Id = TSH.StakingPolicyDetailID " +
                            "LEFT JOIN WalletTypeMasters WT ON WT.Id = TSH.WalletTypeID " +
                            "LEFT JOIN BizUser BU ON BU.Id = TSH.UserID " +
                            "LEFT JOIN TokenUnStakingHistory TUH ON TUH.TokenStakingHistoryID = TSH.Id " +
                            "LEFT JOIN WalletMasters WM ON WM.Id = TSH.WalletID " +
                            "WHERE TSH.Id IN(" + HistoryId + ") AND (TSH.Status={0} OR {0}=999) AND (TSH.UserID = {1} OR {1}=0) ORDER BY TSH.ID DESC";

                var data = _dbContext.UnStakingHistoryDatav2.FromSql(query, Convert.ToInt16(status == null ? 999 : status), Convert.ToInt64(userid == null ? 0 : userid));
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<UnStakingHistory> ListUnStakingHistory(long? userid, short? status, EnUnstakeType? UnStackType)
        {
            try
            {
                string Query = "SELECT TU.ID AS 'AdminReqID',TH.StakingAmount, BU.UserName,BU.Email,TU.TokenStakingHistoryID,TU.AmountCredited,TU.UnstakeType," +
                    "CASE TU.UnstakeType WHEN 1 THEN 'Full' WHEN 2 THEN 'Partial' ELSE 'Unknown' END AS 'UnstackTypeName'," +
                    "TU.InterestCreditedValue,TU.ChargeBeforeMaturity,TU.DegradeStakingHistoryRequestID,TU.Status,TU.CreatedDate AS 'RequestDate',TU.DegradeStakingAmount," +
                    "CASE TU.Status WHEN 0 THEN 'Pending' WHEN 1 THEN 'Approved' WHEN 9 THEN 'Deleted' ELSE 'Unknown' END AS 'StrStatus' " +
                    "FROM TokenUnStakingHistory TU " +
                    "INNER JOIN TokenStakingHistory TH ON TH.ID = TU.TokenStakingHistoryID " +
                    "INNER JOIN BizUser BU ON BU.Id = TH.UserID " +
                    "WHERE (TU.Status = {0} OR {0}=999) AND (TU.CreatedBy = {1} OR {1}=0) AND (TU.UnstakeType = {2} OR {2}=0) order by TU.ID desc";
                var Resp = _dbContext.UnStakingHistory.FromSql(Query, Convert.ToInt16(status == null ? 999 : status), Convert.ToInt64(userid == null ? 0 : userid), Convert.ToInt16(UnStackType == null ? 0 : UnStackType));
                return Resp.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<UnStakingHistoryv2> ListUnStakingHistoryv2(long? userid, short? status, EnUnstakeType? UnStackType)
        {
            try
            {
                string Query = "SELECT ISNULL(TU.GUID,'') AS 'AdminReqID',BU.UserName,BU.Email,TU.TokenStakingHistoryID,TU.AmountCredited,TU.UnstakeType," +
                    "CASE TU.UnstakeType WHEN 1 THEN 'Full' WHEN 2 THEN 'Partial' ELSE 'Unknown' END AS 'UnstackTypeName'," +
                    "TU.InterestCreditedValue,TU.ChargeBeforeMaturity,TU.DegradeStakingHistoryRequestID,TU.Status,TU.CreatedDate AS 'RequestDate',TU.DegradeStakingAmount," +
                    "CASE TU.Status WHEN 0 THEN 'Pending' WHEN 1 THEN 'Approved' WHEN 9 THEN 'Deleted' ELSE 'Unknown' END AS 'StrStatus' " +
                    "FROM TokenUnStakingHistory TU " +
                    "INNER JOIN TokenStakingHistory TH ON TH.ID = TU.TokenStakingHistoryID " +
                    "INNER JOIN BizUser BU ON BU.Id = TH.UserID " +
                    "WHERE (TU.Status = {0} OR {0}=999) AND (TU.CreatedBy = {1} OR {1}=0) AND (TU.UnstakeType = {2} OR {2}=0)";
                var Resp = _dbContext.UnStakingHistoryv2.FromSql(Query, Convert.ToInt16(status == null ? 999 : status), Convert.ToInt64(userid == null ? 0 : userid), Convert.ToInt16(UnStackType == null ? 0 : UnStackType));
                return Resp.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public MinMaxRanges GetMinMaxRange(long newID)
        {
            try
            {
                var data = _dbContext.MinMaxRanges.FromSql("SELECT MIN(MinAmount) AS 'MinRange',MAX(MaxAmount) AS 'MaxRange' FROM StakingPolicyDetail WHERE StakingPolicyID = {0}", newID);
                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //public int GetRange(decimal MaxRange, decimal MinRange, long StackingPolicyId)
        //{
        //    try
        //    {
        //        var dataCount1 = _dbContext.Counts.FromSql("SELECT count(*) as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {1}>=MinAmount", StackingPolicyId, MinRange).FirstOrDefault();
        //        var dataCount2 = _dbContext.Counts.FromSql("SELECT count(*)as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {1}>=MinAmount ", StackingPolicyId, MaxRange).FirstOrDefault();
        //        var dataCount3 = _dbContext.Counts.FromSql("SELECT count(*)as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {2}>=MinAmount ", StackingPolicyId, MinRange, MaxRange).FirstOrDefault();
        //        if (dataCount1.Count == 0 && dataCount2.Count == 0 && dataCount3.Count == 0)
        //        {
        //            return 1;//for insert into db
        //        }
        //        return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        //ControlPanelRepository



        public int GetRange(decimal MaxRange, decimal MinRange, long StackingPolicyId, short InterestType, decimal InterestValue, short DurationMonth, short DurationWeek, long PolicyDetailID)
        {
            try
            {
                if (PolicyDetailID > 0)
                {
                    if (DurationMonth > 0)
                    {
                        var dataCount1 = _dbContext.Counts.FromSql("SELECT count(*) as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {1}>=MinAmount and InterestType = {2} and InterestValue = {3} and StakingDurationMonth = {4} and Id <> {5}", StackingPolicyId, MinRange, InterestType, InterestValue, DurationMonth, PolicyDetailID).FirstOrDefault();
                        var dataCount2 = _dbContext.Counts.FromSql("SELECT count(*)as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {1}>=MinAmount and InterestType = {2} and InterestValue = {3} and StakingDurationMonth = {4} and Id <> {5}", StackingPolicyId, MaxRange, InterestType, InterestValue, DurationMonth, PolicyDetailID).FirstOrDefault();
                        var dataCount3 = _dbContext.Counts.FromSql("SELECT count(*)as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {2}>=MinAmount and InterestType = {3} and InterestValue = {4} and StakingDurationMonth = {5} and Id <> {6}", StackingPolicyId, MinRange, MaxRange, InterestType, InterestValue, DurationMonth, PolicyDetailID).FirstOrDefault();
                        if (dataCount1.Count == 0 && dataCount2.Count == 0 && dataCount3.Count == 0)
                        {
                            return 1;//for insert into db
                        }
                        return 0;
                    }
                    else
                    {
                        var dataCount4 = _dbContext.Counts.FromSql("SELECT count(*) as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {1}>=MinAmount and InterestType = {2} and InterestValue = {3} and StakingDurationWeek = {4} and Id <> {5}", StackingPolicyId, MinRange, InterestType, InterestValue, DurationWeek, PolicyDetailID).FirstOrDefault();
                        var dataCount5 = _dbContext.Counts.FromSql("SELECT count(*)as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {1}>=MinAmount and InterestType = {2} and InterestValue = {3} and StakingDurationWeek = {4} and Id <> {5}", StackingPolicyId, MaxRange, InterestType, InterestValue, DurationWeek, PolicyDetailID).FirstOrDefault();
                        var dataCount6 = _dbContext.Counts.FromSql("SELECT count(*)as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {2}>=MinAmount and InterestType = {3} and InterestValue = {4} and StakingDurationWeek = {5} and Id <> {6}", StackingPolicyId, MinRange, MaxRange, InterestType, InterestValue, DurationWeek, PolicyDetailID).FirstOrDefault();
                        if (dataCount4.Count == 0 && dataCount5.Count == 0 && dataCount6.Count == 0)
                        {
                            return 1;//for insert into db
                        }
                        return 0;
                    }
                }
                else
                {
                    if (DurationMonth > 0)
                    {
                        var dataCount1 = _dbContext.Counts.FromSql("SELECT count(*) as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {1}>=MinAmount and InterestType = {2} and InterestValue = {3} and StakingDurationMonth = {4} ", StackingPolicyId, MinRange, InterestType, InterestValue, DurationMonth).FirstOrDefault();
                        var dataCount2 = _dbContext.Counts.FromSql("SELECT count(*)as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {1}>=MinAmount and InterestType = {2} and InterestValue = {3} and StakingDurationMonth = {4} ", StackingPolicyId, MaxRange, InterestType, InterestValue, DurationMonth).FirstOrDefault();
                        var dataCount3 = _dbContext.Counts.FromSql("SELECT count(*)as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {2}>=MinAmount and InterestType = {3} and InterestValue = {4} and StakingDurationMonth = {5} ", StackingPolicyId, MinRange, MaxRange, InterestType, InterestValue, DurationMonth).FirstOrDefault();
                        if (dataCount1.Count == 0 && dataCount2.Count == 0 && dataCount3.Count == 0)
                        {
                            return 1;//for insert into db
                        }
                        return 0;
                    }
                    else
                    {
                        var dataCount4 = _dbContext.Counts.FromSql("SELECT count(*) as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {1}>=MinAmount and InterestType = {2} and InterestValue = {3} and StakingDurationWeek = {4} ", StackingPolicyId, MinRange, InterestType, InterestValue, DurationWeek).FirstOrDefault();
                        var dataCount5 = _dbContext.Counts.FromSql("SELECT count(*)as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {1}>=MinAmount and InterestType = {2} and InterestValue = {3} and StakingDurationWeek = {4} ", StackingPolicyId, MaxRange, InterestType, InterestValue, DurationWeek).FirstOrDefault();
                        var dataCount6 = _dbContext.Counts.FromSql("SELECT count(*)as Count FROM StakingPolicyDetail WHERE status=1 and StakingPolicyID = {0} and {1}<=MaxAmount and {2}>=MinAmount and InterestType = {3} and InterestValue = {4} and StakingDurationWeek = {5} ", StackingPolicyId, MinRange, MaxRange, InterestType, InterestValue, DurationWeek).FirstOrDefault();
                        if (dataCount4.Count == 0 && dataCount5.Count == 0 && dataCount6.Count == 0)
                        {
                            return 1;//for insert into db
                        }
                        return 0;
                    }
                }

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        public ListStakingPolicyDetailRes2 ListStakingPolicyDetails(long StackingPolicyMasterId, EnStakingType? StakingType, EnStakingSlabType? SlabType, short? status)
        {
            try
            {
                ListStakingPolicyDetailRes2 resp = new ListStakingPolicyDetailRes2();
                string Query = "SELECT SPD.ID AS 'PolicyDetailID',SPM.StakingType, " +
                                "CASE SPM.StakingType WHEN 1 THEN 'FD' WHEN 2 THEN 'Charge' END AS 'StakingTypeName',SPM.SlabType," +
                                "CASE SPM.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' END AS 'SlabTypeName',SPM.WalletTypeID," +
                                "(SELECT WTM.WalletTypeName FROM WalletTypeMasters WTM WHERE WTM.id=SPM.WalletTypeID) AS 'StakingCurrency'," +
                                "SPD.MinAmount,SPD.MaxAmount,SPD.StakingDurationWeek AS 'DurationWeek',SPD.StakingDurationMonth AS 'DurationMonth',SPD.InterestType," +
                                "ISNULL(CASE SPD.InterestType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Percentage' END,'-') AS 'InterestTypeName'," +
                                "CASE SPM.SlabType WHEN 1 THEN CAST(SPD.MinAmount AS varchar) WHEN 2 THEN " +
                                "CAST(MinAmount AS varchar) +'-' + CAST(MaxAmount AS varchar) END AS 'AvailableAmount', " +
                                "SPD.InterestValue,SPD.InterestWalletTypeID AS 'MaturityCurrencyID'," +
                                "SPD.RenewUnstakingEnable,CASE SPD.RenewUnstakingEnable WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrRenewUnstakingEnable',SPD.RenewUnstakingPeriod," +
                                "ISNULL(WT.WalletTypeName,'-') AS 'MaturityCurrencyName'," +
                                "ISNULL(SPD.MakerCharges, 0) AS 'MakerCharges',ISNULL(SPD.TakerCharges, 0) AS 'TakerCharges',SPD.Status," +
                                "SPD.EnableAutoUnstaking,CASE SPD.EnableAutoUnstaking WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableAutoUnstaking'," +
                                "SPD.EnableStakingBeforeMaturity," +
                                "CASE SPD.EnableStakingBeforeMaturity WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableStakingBeforeMaturity'," +
                                "SPD.EnableStakingBeforeMaturityCharge " +
                                "FROM StakingPolicyDetail SPD " +
                                "INNER JOIN StakingPolicyMaster SPM ON SPD.StakingPolicyID = SPM.Id " +
                                "LEFT JOIN WalletTypeMasters WT ON WT.Id = SPD.InterestWalletTypeID " +
                                "WHERE SPD.StakingPolicyID={3} AND SPM.Status < 9 AND SPD.Status < 9 AND (SPM.SlabType = {0} OR {0}=999) " + //AND (SPD.Id = {0} OR {0}=999) 
                                "AND (SPM.StakingType = {1} OR {1}=999) AND (SPD.Status = {2} OR {2}=999) AND (SPM.Status = {2} OR {2}=999)";

                var DetailData = _dbContext.StakingPolicyDetailRes.FromSql(Query, (SlabType == null ? 999 : Convert.ToInt16(SlabType)), (StakingType == null ? 999 : Convert.ToInt16(StakingType)), (status == null ? 999 : status), StackingPolicyMasterId);

                string MasterQuery = "SELECT Id,WalletTypeID,SlabType,StakingType,Status FROM StakingPolicyMaster " +
                                     "WHERE Status < 9 AND (SlabType = {0} OR {0}=999) AND (Status = {1} OR {1}=999) AND (StakingType = {2} OR {2}=999) AND Id = {3}";
                var MasterData = _dbContext.StakingPolicyMasterRes.FromSql(MasterQuery, (SlabType == null ? 999 : Convert.ToInt16(SlabType)), (status == null ? 999 : status), (StakingType == null ? 999 : Convert.ToInt16(StakingType)), StackingPolicyMasterId);

                resp.Details = DetailData.ToList();
                resp.MasterDetail = MasterData.ToList();

                return resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<StakingPolicyDetailRes> GetStakingPolicy(long PolicyDetailID, short? Status)
        {
            try
            {
                ListStakingPolicyDetailRes resp = new ListStakingPolicyDetailRes();
                string Query = "SELECT SPD.ID AS 'PolicyDetailID',SPM.StakingType, " +
                                "CASE SPM.StakingType WHEN 1 THEN 'FD' WHEN 2 THEN 'Charge' END AS 'StakingTypeName',SPM.SlabType," +
                                "CASE SPM.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' END AS 'SlabTypeName',SPM.WalletTypeID," +
                                "(SELECT WTM.WalletTypeName FROM WalletTypeMasters WTM WHERE WTM.id=SPM.WalletTypeID) AS 'StakingCurrency'," +
                                "SPD.StakingDurationWeek AS 'DurationWeek',SPD.StakingDurationMonth AS 'DurationMonth',SPD.InterestType," +
                                "CASE SPD.InterestType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Percentage' END AS 'InterestTypeName'," +
                                "CASE SPM.SlabType WHEN 1 THEN CAST(SPD.MinAmount AS varchar) WHEN 2 THEN CAST(MinAmount AS varchar) +'-' + CAST(MaxAmount AS varchar) END AS 'AvailableAmount', " +
                                "SPD.MinAmount,SPD.MaxAmount,SPD.InterestValue,SPD.InterestWalletTypeID AS 'MaturityCurrencyID'," +
                                "ISNULL(WT.WalletTypeName,'-') AS 'MaturityCurrencyName'," +
                                "ISNULL(SPD.MakerCharges, 0) AS 'MakerCharges',ISNULL(SPD.TakerCharges, 0) AS 'TakerCharges',SPD.Status," +
                                "SPD.EnableAutoUnstaking,CASE SPD.EnableAutoUnstaking WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableAutoUnstaking'," +
                                "SPD.EnableStakingBeforeMaturity," +
                                "SPD.RenewUnstakingEnable,CASE SPD.RenewUnstakingEnable WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrRenewUnstakingEnable',SPD.RenewUnstakingPeriod," +
                                "CASE SPD.EnableStakingBeforeMaturity WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableStakingBeforeMaturity'," +
                                "SPD.EnableStakingBeforeMaturityCharge " +
                                "FROM StakingPolicyDetail SPD " +
                                "INNER JOIN StakingPolicyMaster SPM ON SPD.StakingPolicyID = SPM.Id " +
                                "LEFT JOIN WalletTypeMasters WT ON WT.Id = SPD.InterestWalletTypeID " +
                                "WHERE SPM.Status < 9 AND SPD.Status < 9 AND (SPD.Id = {0} OR {0}=999) " +
                                "AND (SPM.Status = {1} OR {1}=999) AND (SPD.Status = {1} OR {1}=999)";

                var DetailData = _dbContext.StakingPolicyDetailRes.FromSql(Query, (PolicyDetailID == 0 ? 999 : Convert.ToInt16(PolicyDetailID)), (Status == null ? 999 : Status));
                return DetailData.ToList();

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region StopLossMaster

        public async Task<List<StopLossRes>> ListStopLoss(long? WalletTypeId, short? Status)
        {
            try
            {
                var data = _dbContext.StopLossRes.FromSql("select sl.Status,case sl.Status when 0 then 'Disable' when 1 then 'Enable' else 'Deleted' end as StrStatus,sl.Id,sl.WalletTypeId,sl.StopLossPer,wt.WalletTypeName from stoplossmaster sl inner join WalletTypeMasters wt on wt.id = sl.WalletTypeID where sl.Status<9 and (sl.WalletTypeId={0} or {0}=0) and (sl.Status={1} or {1}=999)", (WalletTypeId == null ? 0 : WalletTypeId), (Status == null ? 999 : Status)).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region LeverageMaster

        public async Task<List<LeverageRes>> ListLeverage(long? WalletTypeId, short? Status)
        {
            try
            {
                var data = _dbContext.LeverageRes.FromSql("select sl.InstantChargePer,sl.Status,case sl.Status when 0 then 'Disable' when 1 then 'Enable' else 'Deleted' end as StrStatus,sl.Id,sl.WalletTypeId,sl.LeveragePer,wt.WalletTypeName,sl.IsAutoApprove,sl.SafetyMarginPer,sl.MarginChargePer,sl.LeverageChargeDeductionType,case sl.LeverageChargeDeductionType when 0 then 'TradingToMarginWallet' when 1 then 'EndOfDay' when 2 then 'MarginWalletToTradingWallet' when 3 then 'EndOfDay_Or_MarginWalletToTradingWallet' else 'N/A' END AS LeverageChargeDeductionTypeName from LeverageMaster sl inner join WalletTypeMasters wt on wt.id = sl.WalletTypeID where sl.Status<9 and (sl.WalletTypeId={0} or {0}=0) and (sl.Status={1} or {1}=999)", (WalletTypeId == null ? 0 : WalletTypeId), (Status == null ? 999 : Status)).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Import Export Address

        public List<ImpExpAddressRes> GetExportAddressList(long? ServiceProviderID, long? UserID, long? WalletTypeID)
        {
            try
            {
                string Query = "SELECT WTM.WalletTypeName,AM.Address,AM.AddressLable,BU.Email,AM.IsDefaultAddress,SPM.ProviderName AS 'ServiceProviderName' " +
                                "FROM AddressMasters AM " +
                                "INNER JOIN ServiceProviderMaster SPM ON SPM.Id = AM.SerProID " +
                                "INNER JOIN WalletMasters WM ON WM.Id = AM.WalletId " +
                                "INNER JOIN WalletTypeMasters WTM ON WTM.Id = WM.WalletTypeID " +
                                "INNER JOIN BizUser BU ON BU.Id = WM.UserID " +
                                "WHERE (WTM.ID = {0} OR {0}=0) AND (AM.CreatedBy = {1} OR {1}=0) AND (SPM.Id = {2} OR {2}=0)";
                var data = _dbContext.ImpExpAddressRes.FromSql(Query, (WalletTypeID == null ? 0 : WalletTypeID), (UserID == null ? 0 : UserID), (ServiceProviderID == null ? 0 : ServiceProviderID)).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<AddressRes> ListAddressDetails(long? ServiceProviderID, long? UserID, long? WalletTypeID, string Address, int PageNo, int PageSize, ref int TotalCount)
        {
            try
            {
                string Query = "SELECT WTM.WalletTypeName,AM.Address,AM.AddressLable,BU.Email,AM.IsDefaultAddress,SPM.ProviderName AS 'ServiceProviderName' FROM AddressMasters AM INNER JOIN ServiceProviderMaster SPM ON SPM.Id = AM.SerProID INNER JOIN WalletMasters WM ON WM.Id = AM.WalletId INNER JOIN WalletTypeMasters WTM ON WTM.Id = WM.WalletTypeID INNER JOIN BizUser BU ON BU.Id = WM.UserID WHERE (AM.Address= {3} OR {3}='') and  (WTM.ID = {0} OR {0}=0) AND (AM.CreatedBy = {1} OR {1}=0) AND (SPM.Id = {2} OR {2}=0) AND AM.Status=1";
                var data = _dbContext.AddressRes.FromSql(Query, (WalletTypeID == null ? 0 : WalletTypeID), (UserID == null ? 0 : UserID), (ServiceProviderID == null ? 0 : ServiceProviderID), (Address == null ? "" : Address)).ToList();
                TotalCount = data.Count();
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32(PageSize * (PageNo - 1));
                    data = data.Skip(skip).Take(PageSize).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //public BizResponseClass AddBulkData(List<ImpExpAddressRes> Details)
        //{
        //    try
        //    {
        //        BizResponseClass res = new BizResponseClass();
        //        ListImpExpAddressRes ListData = new ListImpExpAddressRes();
        //        ListData.Details = Details;

        //        #region Comments
        //        //var test1 = (from a in _dbContext.ServiceProviderMaster
        //        //             join li in ListData.Details on a.ProviderName equals li.ServiceProviderName
        //        //             where a.ProviderName == li.ServiceProviderName
        //        //             select a).ToList();

        //        //var use = (from b in ListData.Details
        //        //           join t in  test1 on b.ServiceProviderName equals t.ProviderName
        //        //           join li in _dbContext.Users on b.Email equals li.Email
        //        //           where b.Email == li.Email
        //        //           select b).ToList();

        //        //var Obj = (from t in _dbContext.AddressMasters
        //        //           join li in ListData.Details on t.Address equals  li.Address
        //        //           where t.Address == li.Address
        //        //          select t).ToList();



        //        //var add = from li in ListData.Details
        //        //          from o in Obj 
        //        //              join array in use on li.Email equals array.Email
        //        //          //from array in use
        //        //          from ww in test1
        //        //             // join ww in test1 on li.ServiceProviderName  equals ww.ProviderName
        //        //          where li.Address!=o.Address
        //        //          select new AddressMaster
        //        //          {
        //        //              CreatedBy = 1,
        //        //              CreatedDate = Helpers.UTC_To_IST(),
        //        //              WalletId = ww.Id,
        //        //              UpdatedDate = Helpers.UTC_To_IST()
        //        //          };
        //        //_dbContext.AddressMasters.AddRange();
        //        //_dbContext.SaveChanges();
        //        //-----------------------------------

        //        #endregion

        //        var listObj = (from li in ListData.Details
        //                       join sp in _dbContext.ServiceProviderMaster on li.ServiceProviderName equals sp.ProviderName
        //                       join user in _dbContext.Users on li.Email equals user.Email
        //                       join wt in _dbContext.WalletTypeMasters on li.WalletTypeName equals wt.WalletTypeName
        //                       join wm in _dbContext.WalletMasters on user.Id equals wm.UserID
        //                       where wt.Id == wm.WalletTypeID && wm.IsDefaultWallet == 1
        //                       //select li).ToList().Distinct();
        //                       select new AddressMaster//ImpExpAddressRes2
        //                       {
        //                           Address = li.Address,
        //                           OriginalAddress = li.Address,
        //                           AddressLable = li.AddressLable,
        //                           CreatedBy = 1,
        //                           CreatedDate = Helpers.UTC_To_IST(),
        //                           Status = 1,
        //                           WalletId = wm.Id,
        //                           //Email = li.Email,                                   
        //                           IsDefaultAddress = li.IsDefaultAddress,
        //                           SerProID = sp.Id,
        //                           //ServiceProviderName = li.ServiceProviderName,
        //                           //UserID = user.Id,
        //                           //WalletTypeID = wt.Id,
        //                           //WalletTypeName = li.WalletTypeName
        //                       }).ToList().Distinct();
        //        if (listObj != null || listObj.Count() > 0)
        //        {
        //            var AddObj = (from obj in listObj
        //                          where !_dbContext.AddressMasters.Any(i => i.Address == obj.Address && i.OriginalAddress == obj.OriginalAddress)
        //                          select obj);
        //            _dbContext.AddressMasters.AddRange(AddObj);
        //            _dbContext.SaveChanges();

        //            res.ErrorCode = enErrorCode.Success;
        //            res.ReturnCode = enResponseCode.Success;
        //            res.ReturnMsg = EnResponseMessage.RecordAdded;
        //        }
        //        else
        //        {
        //            res.ErrorCode = enErrorCode.NoDataFound;
        //            res.ReturnCode = enResponseCode.Fail;
        //            res.ReturnMsg = EnResponseMessage.NotFound;
        //        }
        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}

        public BizResponseClass AddBulkData(List<ImpExpAddressRes3> Details)
        {
            try
            {
                BizResponseClass res = new BizResponseClass();
                ListImpExpAddressRes3 ListData = new ListImpExpAddressRes3();
                ListData.Details = Details;

                #region Comments
                //var test1 = (from a in _dbContext.ServiceProviderMaster
                //             join li in ListData.Details on a.ProviderName equals li.ServiceProviderName
                //             where a.ProviderName == li.ServiceProviderName
                //             select a).ToList();

                //var use = (from b in ListData.Details
                //           join t in  test1 on b.ServiceProviderName equals t.ProviderName
                //           join li in _dbContext.Users on b.Email equals li.Email
                //           where b.Email == li.Email
                //           select b).ToList();

                //var Obj = (from t in _dbContext.AddressMasters
                //           join li in ListData.Details on t.Address equals  li.Address
                //           where t.Address == li.Address
                //          select t).ToList();



                //var add = from li in ListData.Details
                //          from o in Obj 
                //              join array in use on li.Email equals array.Email
                //          //from array in use
                //          from ww in test1
                //             // join ww in test1 on li.ServiceProviderName  equals ww.ProviderName
                //          where li.Address!=o.Address
                //          select new AddressMaster
                //          {
                //              CreatedBy = 1,
                //              CreatedDate = Helpers.UTC_To_IST(),
                //              WalletId = ww.Id,
                //              UpdatedDate = Helpers.UTC_To_IST()
                //          };
                //_dbContext.AddressMasters.AddRange();
                //_dbContext.SaveChanges();
                //-----------------------------------

                #endregion

                var listObj = (from li in ListData.Details
                               select new AddressMaster
                               {
                                   Address = li.Address,
                                   OriginalAddress = li.Address,
                                   AddressLable = li.AddressLable,
                                   CreatedBy = 1,
                                   CreatedDate = Helpers.UTC_To_IST(),
                                   Status = 1,
                                   WalletId = li.WalletId,
                                   IsDefaultAddress = li.IsDefaultAddress,
                                   SerProID = li.SerProId
                               }).ToList().Distinct();
                if (listObj != null || listObj.Count() > 0)
                {
                    var AddObj = (from obj in listObj
                                  where !_dbContext.AddressMasters.Any(i => i.Address == obj.Address && i.OriginalAddress == obj.OriginalAddress)
                                  select obj);
                    _dbContext.AddressMasters.AddRange(AddObj);
                    _dbContext.SaveChanges();

                    res.ErrorCode = enErrorCode.Success;
                    res.ReturnCode = enResponseCode.Success;
                    res.ReturnMsg = EnResponseMessage.RecordAdded;
                }
                else
                {
                    res.ErrorCode = enErrorCode.NoDataFound;
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.NotFound;
                }
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long getProviderId(string ProviderName)
        {
            try
            {
                var SerProData = (from m in _dbContext.ServiceProviderMaster
                                  where m.ProviderName == ProviderName
                                  select m).FirstOrDefault();
                return SerProData.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region StakingPolicyMaster
        public async Task<List<StakingPolicyRes>> ListStakingPolicyMaster(long? WalletTypeId, short? Status, short? enStakingSlabType, short? enStakingType)
        {
            try
            {
                var data = _dbContext.StakingPolicyRes.FromSql("select case sl.StakingType when 1 then 'FixedDeposit' when 2 then 'Charge' else 'no found' end as StakingTypeName,case sl.slabtype when 1 then 'Fixed' when 2 then 'Range' else 'no found' end as SlabTypeName,sl.Status,case sl.Status when 0 then 'Disable' when 1 then 'Enable' else 'Deleted' end as StrStatus,sl.Id,sl.WalletTypeId,sl.StakingType,wt.WalletTypeName,sl.SlabType from StakingPolicyMaster sl inner join WalletTypeMasters wt on wt.id = sl.WalletTypeID where sl.Status<9 and (sl.WalletTypeId={0} or {0}=0) and (sl.Status={1} or {1}=999) and (sl.SlabType={2} or {2}=0) and (sl.StakingType={3} or {3}=0)", (WalletTypeId == null ? 0 : WalletTypeId), (Status == null ? 999 : Status), enStakingSlabType, enStakingType).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BeneUpdate BulkUpdateDetail(long id, short bit)
        {
            try
            {
                BeneUpdate res = new BeneUpdate();
                string Query = "UPDATE stakingpolicydetail SET Status={0}";
                //if (bit == 9)
                //{
                //    Query += "Status=9";
                //}
                Query += " WHERE stakingpolicyid = (select id from stakingpolicymaster where id={1}) SELECT @@ROWCOUNT as 'AffectedRows'";
                IQueryable<BeneUpdate> Result = _dbContext.BeneUpdate.FromSql(Query, bit, id);
                res = Result.FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                //_dbContext.Database.RollbackTransaction(); ntrivedi 14-12-2018 not needed 
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region Charge

        public List<ChargesTypeWise> ListChargesTypeWise(long WalletTypeId, long? TrntypeId)
        {
            List<ChargesTypeWise> Resp = new List<ChargesTypeWise>();
            try
            {
                var ChargeData = _dbContext.ChargesTypeWise.FromSql("SELECT cd.ChargeValue,wtm.WalletTypeName as DeductWalletTypeName,wt.TrnTypeName, cd.MakerCharge, cd.TakerCharge, wt.TrnTypeId FROM ChargeConfigurationDetail cd inner join ChargeConfigurationMaster cm ON cm.id = cd.ChargeConfigurationMasterID INNER JOIN WTrnTypeMaster wt ON wt.TrnTypeId = cm.TrnType inner join WalletTypemasters wtm on wtm.id = cd.DeductionWalletTypeId WHERE cm.TrnType in (3, 8,9) AND cd.Status = 1 AND cm.Status = 1 AND cm.WalletTypeID ={0} and(cm.TrnType ={1} or {1}= 0)", WalletTypeId, (TrntypeId == null ? 0 : Convert.ToInt64(TrntypeId))).ToList();

                return ChargeData;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public List<WalletType> GetChargeWalletType(long? WalletTypeId)
        {
            //List<ChargesTypeWise> Resp = new List<ChargesTypeWise>();
            try
            {
                var _WalletType = (from w in _dbContext.ChargeConfigurationMaster
                                   join wt in _dbContext.WalletTypeMasters
                                   on w.WalletTypeID equals wt.Id
                                   where wt.Status == 1 && (WalletTypeId == null || (w.WalletTypeID == WalletTypeId && WalletTypeId != null))
                                   group w by new { w.WalletTypeID, wt.WalletTypeName } into g
                                   select new WalletType { WalletTypeId = g.Key.WalletTypeID, WalletTypeName = g.Key.WalletTypeName }).ToList();

                return _WalletType;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public List<TrnChargeLogRes> TrnChargeLogReport(int PageNo, int PageSize, short? Status, long? TrnTypeID, long? WalleTypeId, short? SlabType, DateTime? FromDate, DateTime? ToDate, long? TrnNo, ref long TotalCount)
        {
            try
            {
                List<TrnChargeLogRes> ChargeLogData = new List<TrnChargeLogRes>();
                string sqlQuery = "SELECT tch.Status,CASE tch.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 6 THEN 'Hold' WHEN 5 THEN 'Refunded' WHEN 9 THEN 'FAIL' ELSE 'Other' END AS StrStatus,tch.BatchNo,tch.TrnNo,tch.TrnType as TrnTypeID,wtrn.Trntypename AS TrnTypeName,tch.Amount,CONVERT(VARCHAR,tch.Amount) AS StrAmount,tch.Charge,ISNULL(tch.StakingChargeMasterID,0) AS StakingChargeMasterID,tch.ChargeConfigurationDetailID,tch.WalletTypeID,wt.wallettypename AS WalletTypeName,cd.Remarks as ChargeConfigurationDetailRemarks,tch.TimeStamp,tch.DWalletID,(SELECT Accwalletid FROM walletmasters WHERE id=tch.DWalletID) AS DAccWalletId,(SELECT Walletname FROM walletmasters WHERE id=tch.DWalletID) AS DWalletName ,tch.OWalletID,w.Walletname AS OWalletName,w.AccWalletID AS OAccWalletID,tch.DUserID,(SELECT username FROM BizUser WHERE ID=tch.DUserID) AS DUserName,(SELECT Email FROM BizUser WHERE ID=tch.DUserID) AS DEmail,tch.OUserID,ISNULL(b.UserName,'OrgUsername') AS OUserName,ISNULL(b.email,'OrgEmail') as OEmail,tch.SlabType,CASE tch.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' END AS SlabTypeName ,tch.Remarks AS TrnChargeLogRemarks ,tch.ChargeConfigurationMasterID ,ISNULL(cm.SpecialChargeConfigurationID,0) AS SpecialChargeConfigurationID ,ISNULL(s.Remarks,'N/A') AS SpecialChargeConfigurationRemarks,ISNULL(tch.Ismaker,2) as Ismaker,CASE tch.Ismaker WHEN 0 THEN 'Taker' WHEN 1 THEn 'Maker' ELSE 'N/A' END AS StrIsmaker,tch.Createddate AS Date FROM TrnChargeLog tch INNER JOIN WTrnTypeMaster wtrn ON wtrn.TrnTypeId=tch.trntype INNER JOIN WalletTypeMasters wt ON wt.Id=tch.WalletTypeID LEFT JOIN ChargeConfigurationDetail cd ON cd.id=tch.ChargeConfigurationDetailID INNER JOIN Walletmasters w ON w.id=tch.OWalletID LEFT JOIN BizUser b on b.id =tch.OUserID LEFT JOIN ChargeConfigurationMaster cm on cm.id= tch.ChargeConfigurationMasterid LEFT JOIN SpecialChargeConfiguration s ON s.id =cm.SpecialChargeConfigurationID  WHERE tch.Status>0 AND (tch.Status = {0} OR {0}=999)  AND (tch.trntype={1} OR {1}=0) AND (tch.WalletTypeID={2} or {2}=0) AND (tch.slabtype={3} OR {3}=0) AND (tch.trnNo={4} OR {4}=0)";

                if (FromDate != null && ToDate != null)
                {
                    FromDate = Convert.ToDateTime(FromDate).AddHours(0).AddMinutes(0).AddSeconds(0);
                    ToDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                    sqlQuery = sqlQuery + " AND tch.CreatedDate BETWEEN {5} AND {6}";
                    ChargeLogData = _dbContext.TrnChargeLogRes.FromSql(sqlQuery, (Status == null ? 999 : Status), (TrnTypeID == null ? 0 : TrnTypeID), (WalleTypeId == null ? 0 : WalleTypeId), (SlabType == null ? 0 : SlabType), (TrnNo == null ? 0 : TrnNo), FromDate, ToDate).ToList();
                }
                else
                {
                    ChargeLogData = _dbContext.TrnChargeLogRes.FromSql(sqlQuery, (Status == null ? 999 : Status), (TrnTypeID == null ? 0 : TrnTypeID), (WalleTypeId == null ? 0 : WalleTypeId), (SlabType == null ? 0 : SlabType), (TrnNo == null ? 0 : TrnNo)).ToList();
                }
                TotalCount = ChargeLogData.Count();
                if (ChargeLogData.Count != 0)
                {
                    if (PageNo > 0)
                    {
                        int skip = PageSize * (PageNo - 1);
                        ChargeLogData = ChargeLogData.Skip(skip).Take(PageSize).ToList();
                    }
                }
                return ChargeLogData;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public List<TrnChargeLogResv2> TrnChargeLogReportv2(int PageNo, int PageSize, short? Status, long? TrnTypeID, long? WalleTypeId, short? SlabType, DateTime? FromDate, DateTime? ToDate, string TrnNo, ref long TotalCount)
        {
            try
            {
                List<TrnChargeLogResv2> ChargeLogData = new List<TrnChargeLogResv2>();
                string sqlQuery = "SELECT ISNULL(tch.DeductCurrency,'') as MarketCurrency,ISNULL(tch.DiscountPercent,0) as DiscountPercent,ISNULL(tch.Type,0) as TradingChargeType,tch.Status,CASE tch.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 6 THEN 'Hold' WHEN 5 THEN 'Refunded' WHEN 9 THEN 'FAIL' ELSE 'Other' END AS StrStatus,tch.BatchNo,cast(ISNULL(u.GUID,'00000000-0000-0000-0000-000000000000')as varchar(50)) as TrnNo,tch.TrnType as TrnTypeID,wtrn.Trntypename AS TrnTypeName,tch.Amount,CONVERT(VARCHAR,tch.Amount) AS StrAmount,tch.Charge,ISNULL(tch.StakingChargeMasterID,0) AS StakingChargeMasterID,tch.ChargeConfigurationDetailID,tch.WalletTypeID,wt.wallettypename AS WalletTypeName,cd.Remarks as ChargeConfigurationDetailRemarks,tch.TimeStamp,tch.DWalletID,(SELECT Accwalletid FROM walletmasters WHERE id=tch.DWalletID) AS DAccWalletId,(SELECT Walletname FROM walletmasters WHERE id=tch.DWalletID) AS DWalletName ,tch.OWalletID,w.Walletname AS OWalletName,w.AccWalletID AS OAccWalletID,tch.DUserID,(SELECT username FROM BizUser WHERE ID=tch.DUserID) AS DUserName,(SELECT Email FROM BizUser WHERE ID=tch.DUserID) AS DEmail,tch.OUserID,ISNULL(b.UserName,'OrgUsername') AS OUserName,ISNULL(b.email,'OrgEmail') as OEmail,tch.SlabType,CASE tch.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' END AS SlabTypeName ,tch.Remarks AS TrnChargeLogRemarks ,tch.ChargeConfigurationMasterID ,ISNULL(cm.SpecialChargeConfigurationID,0) AS SpecialChargeConfigurationID ,ISNULL(s.Remarks,'N/A') AS SpecialChargeConfigurationRemarks,ISNULL(tch.Ismaker,2) as Ismaker,CASE tch.Ismaker WHEN 0 THEN 'Taker' WHEN 1 THEn 'Maker' ELSE 'N/A' END AS StrIsmaker,tch.Createddate AS Date FROM TrnChargeLog tch INNER JOIN WTrnTypeMaster wtrn ON wtrn.TrnTypeId=tch.trntype INNER JOIN WalletTypeMasters wt ON wt.Id=tch.WalletTypeID LEFT JOIN TransactionQueue u ON u.Id=tch.TrnrefNo LEFT JOIN ChargeConfigurationDetail cd ON cd.id=tch.ChargeConfigurationDetailID INNER JOIN Walletmasters w ON w.id=tch.OWalletID LEFT JOIN BizUser b on b.id =tch.OUserID LEFT JOIN ChargeConfigurationMaster cm on cm.id= tch.ChargeConfigurationMasterid LEFT JOIN SpecialChargeConfiguration s ON s.id =cm.SpecialChargeConfigurationID WHERE tch.Status>0 AND (tch.Status = {0} OR {0}=999)  AND (tch.trntype={1} OR {1}=0) AND (tch.WalletTypeID={2} or {2}=0) AND (tch.slabtype={3} OR {3}=0) AND (u.[GUID]={4} OR {4}='00000000-0000-0000-0000-000000000000')";

                if (FromDate != null && ToDate != null)
                {
                    FromDate = Convert.ToDateTime(FromDate).AddHours(0).AddMinutes(0).AddSeconds(0);
                    ToDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                    sqlQuery = sqlQuery + " AND tch.CreatedDate BETWEEN {5} AND {6}";
                    ChargeLogData = _dbContext.TrnChargeLogResv2.FromSql(sqlQuery, (Status == null ? 999 : Status), (TrnTypeID == null ? 0 : TrnTypeID), (WalleTypeId == null ? 0 : WalleTypeId), (SlabType == null ? 0 : SlabType), (TrnNo == null ? Guid.Empty : Guid.Parse(TrnNo)), FromDate, ToDate).ToList();
                }
                else
                {
                    ChargeLogData = _dbContext.TrnChargeLogResv2.FromSql(sqlQuery, (Status == null ? 999 : Status), (TrnTypeID == null ? 0 : TrnTypeID), (WalleTypeId == null ? 0 : WalleTypeId), (SlabType == null ? 0 : SlabType), (TrnNo == null ? Guid.Empty : Guid.Parse(TrnNo))).ToList();
                }
                TotalCount = ChargeLogData.Count();
                if (ChargeLogData.Count != 0)
                {
                    if (PageNo > 0)
                    {
                        int skip = PageSize * (PageNo - 1);
                        ChargeLogData = ChargeLogData.Skip(skip).Take(PageSize).ToList();
                    }
                }
                return ChargeLogData;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }
        #endregion

        #region DepositCounterMaster

        public List<DepositeCounterRes> GetDepositCounter(long? WalletTypeID, long? SerProId, int PageNo, int PageSize, ref int Total)
        {
            try
            {
                var data = _dbContext.DepositeCounterRes.FromSql("SELECT  dm.id AS Id, ISNULL(dm.UpdatedDate, '') as UpdatedDate, dm.Status as Status, CASE dm.Status WHEN 0 THEN 'Disable' WHEN 1 THEN 'Enable' END AS StrStatus, dm.RecordCount AS RecordCount, dm.Limit AS Limit, dm.MaxLimit AS MaxLimit, dm.TPSPickupStatus AS TPSPickupStatus, dm.WalletTypeID AS WalletTypeID, wt.WalletTypeName AS WalletTypeName, dm.SerProId AS SerProId, sp.ProviderName AS SerProName, dm.LastTrnID AS LastTrnID, ISNULL(dm.PreviousTrnID, 'N/A') AS PreviousTrnID, ISNULL(dm.PrevIterationID, 'N/A') AS PrevIterationID FROM DepositCounterMaster dm INNER JOIN WalletTypeMasters wt ON wt.id = dm.wallettypeid INNER JOIN ServiceProviderMaster sp ON sp.id = dm.SerProId WHERE dm.Status < 9  AND (dm.WalletTypeID={0} OR {0}=0) AND (dm.SerProId={1} OR {1}=0)", (WalletTypeID == null ? 0 : WalletTypeID), (SerProId == null ? 0 : SerProId)).ToList();
                Total = data.Count();

                if (data.Count != 0)
                {
                    if (PageNo > 0)
                    {
                        int skip = PageSize * (PageNo - 1);
                        data = data.Skip(skip).Take(PageSize).ToList();
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region Admin Assets

        public List<AdminAssetsres> AdminAssets(long? WalletTypeId, EnWalletUsageType? WalletUsageType, long Userid, int PageNo, int PageSize, ref int TotalCount)
        {
            List<AdminAssetsres> Resp = new List<AdminAssetsres>();
            try
            {
                string sqlQuery = "SELECT  w.UserId as UserId,w.Id,w.WalletName,wt.WalletTypeName,w.WalletTypeId,w.Balance,w.InBoundBalance,w.OutBoundBalance,w.WalletUsageType,case w.WalletUsageType when 0 then 'Trading Wallet' when 1 then 'Market Wallet' when 2 then 'Cold Wallet' when 3 then 'Charge Cr Wallet ORG' end as StrWalletUsageType,o.OrganizationName,w.OrgId,w.Status, case w.Status when 0 then 'Disable' when 1 then 'Active' when 2 then 'Inactive' when 3 then 'Freeze' when 4 then 'Inoperative' when 5 then 'Suspended' when 6 then 'Blocked' when 6 then 'Deleted' end as StrStatus,b.Email,w.AccWalletId from Walletmasters w inner join wallettypemasters wt on wt.id=w.wallettypeid inner join Bizuser b on b.id=w.userid  inner join OrganizationMaster o on o.id =w.OrgID where w.status<9 and userid in (select  UserID from BizUserTypeMapping where UserType=0)  and (walletusagetype={1} or {1}=999) AND (WalletTypeId={0} or {0}=0)";
                var Data = _dbContext.AdminAssetsres.FromSql(sqlQuery, (WalletTypeId == null ? 0 : Convert.ToInt64(WalletTypeId)), (WalletUsageType == null ? 999 : Convert.ToInt16(WalletUsageType))).ToList();
                TotalCount = Data.Count();
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32((PageSize) * (PageNo - 1));
                    Data = Data.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region Org Ledger

        public List<OrgWalletres> OrganizationLedger(long? WalletTypeId, EnWalletUsageType? WalletUsageType, ref int TotalCount)
        {
            List<OrgWalletres> Resp = new List<OrgWalletres>();
            try
            {
                string sqlQuery = "SELECT  w.UserId as UserId,w.Id,w.WalletName,wt.WalletTypeName,w.WalletTypeId,w.WalletUsageType,case w.WalletUsageType when 0 then 'Trading Wallet' when 1 then 'Market Wallet' when 2 then 'Cold Wallet' when 3 then 'Charge Cr Wallet ORG' end as StrWalletUsageType,o.OrganizationName,w.OrgId,w.Status, case w.Status when 0 then 'Disable' when 1 then 'Active' when 2 then 'Inactive' when 3 then 'Freeze' when 4 then 'Inoperative' when 5 then 'Suspended' when 6 then 'Blocked' when 6 then 'Deleted' end as StrStatus,b.Email,w.AccWalletId from Walletmasters w inner join wallettypemasters wt on wt.id=w.wallettypeid inner join Bizuser b on b.id=w.userid  inner join OrganizationMaster o on o.id =w.OrgID where w.status<9 and userid in (select  UserID from BizUserTypeMapping where UserType=0)  and (walletusagetype={1} or {1}=999) AND (WalletTypeId={0} or {0}=0)";
                var Data = _dbContext.OrgWalletres.FromSql(sqlQuery, (WalletTypeId == null ? 0 : Convert.ToInt64(WalletTypeId)), (WalletUsageType == null ? 999 : Convert.ToInt16(WalletUsageType))).ToList();
                TotalCount = Data.Count();
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }





        #endregion

        #region Market Cap

        public List<WalletTypeMaster> MarketCapIsLocal()
        {
            var listWalletType = new List<WalletTypeMaster>(); // khushali  08-05-2019 handle null reference exception
            listWalletType = (from w in _dbContext.WalletTypeMasters
                              where w.IsLocal == 1
                              select w).ToList();
            return listWalletType;
        }

        public decimal GetLTP(long WalletType)
        {
            var LtpObj = _dbContext.BalanceTotal.FromSql("select Isnull(ts.LTP,0) as TotalBalance from  wallettypemasters wt  inner join servicemaster s on s.wallettypeid =wt.id inner join Tradepairmaster t on t.SecondaryCurrencyId=s.id inner join TradePairStastics ts on ts.PairId=t.id where wt.id={0} and t.basecurrencyid=(select s.id from wallettypemasters wt inner join servicemaster s on s.wallettypeid =wt.id where wt.IsDefaultWallet=1)", WalletType).FirstOrDefault();
            return LtpObj == null ? 0 : LtpObj.TotalBalance; // khushali  08-05-2019 handle null reference exception
        }
        #endregion

        #region ChargeConfigurationMaster

        public List<ChargeConfigurationMasterRes> GetChargeConfigMasterList(long? walletTypeId, long? trnType, short? slabType, short? status, int? PageSize, int? PageNo, ref int TotalCount)
        {
            try
            {
                string Query = "SELECT CCM.Id,CCM.WalletTypeID,WT.WalletTypeName,CCM.TrnType,TM.TrnTypeName,CCM.KYCComplaint," +
                    "CASE CCM.KYCComplaint WHEN 0 THEN 'No' WHEN 1 THEN 'Yes' ELSE 'Unknown' END AS 'StrKYCComplaint'," +
                    "CCM.SlabType,CASE CCM.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' ELSE 'Unknown' END AS 'StrSlabType'," +
                    "CCM.SpecialChargeConfigurationID,CCM.Remarks,CCM.Status," +
                    "CASE CCM.Status WHEN 0 THEN 'Inactive' WHEN 1 THEN 'Active' WHEN 9 THEN 'Delete' ELSE 'Unknown' END AS 'StrStatus' " +
                    "FROM ChargeConfigurationMaster CCM " +
                    "INNER JOIN WalletTypeMasters WT ON CCM.WalletTypeID = WT.Id " +
                    "INNER JOIN Wtrntypemaster TM ON CCM.TrnType = TM.TrnTypeId " +
                    "WHERE CCM.Status < 9 AND (CCM.SlabType = {0} OR {0}=0) AND (CCM.WalletTypeID = {1} OR {1}=0) AND (CCM.TrnType = {2} OR {2}=0) AND (CCM.Status={3} OR {3}=999)";
                var data = _dbContext.ChargeConfigurationMasterRes.FromSql(Query, Convert.ToInt16(slabType == null ? 0 : slabType), Convert.ToInt64(walletTypeId == null ? 0 : walletTypeId), Convert.ToInt64(trnType == null ? 0 : trnType), Convert.ToInt16(status == null ? 999 : status)).ToList();
                TotalCount = data.Count();
                if (PageNo != null && PageSize != null)
                {
                    if (PageNo > 0)
                    {
                        int skip = Convert.ToInt32((PageSize) * (PageNo - 1));
                        data = data.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ChargeConfigurationMasterRes GetChargeConfigMasterbyId(long masterID)
        {
            try
            {
                string Query = "SELECT CCM.Id,CCM.WalletTypeID,WT.WalletTypeName,CCM.TrnType,TM.TrnTypeName,CCM.KYCComplaint," +
                    "CASE CCM.KYCComplaint WHEN 0 THEN 'No' WHEN 1 THEN 'Yes' ELSE 'Unknown' END AS 'StrKYCComplaint'," +
                    "CCM.SlabType,CASE CCM.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' ELSE 'Unknown' END AS 'StrSlabType'," +
                    "CCM.SpecialChargeConfigurationID,CCM.Remarks,CCM.Status," +
                    "CASE CCM.Status WHEN 0 THEN 'Inactive' WHEN 1 THEN 'Active' WHEN 9 THEN 'Delete' ELSE 'Unknown' END AS 'StrStatus' " +
                    "FROM ChargeConfigurationMaster CCM " +
                    "INNER JOIN WalletTypeMasters WT ON CCM.WalletTypeID = WT.Id " +
                    "INNER JOIN Wtrntypemaster TM ON CCM.TrnType = TM.TrnTypeId " +
                    "WHERE CCM.Status < 9 AND CCM.Id={0}";
                var data = _dbContext.ChargeConfigurationMasterRes.FromSql(Query, masterID);
                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ChargeConfigurationDetailRes> GetChargeConfigDetailList(long? masterId, long? chargeType, short? chargeValueType, short? chargeDistributionBasedOn, short? status)
        {
            try
            {
                string Query = "SELECT CD.Id AS ChargeConfigDetailId,CD.ChargeConfigurationMasterID,CD.ChargeDistributionBasedOn," +
                    "CASE CD.ChargeDistributionBasedOn WHEN 1 THEN 'Regular' WHEN 2 THEN 'Volume' WHEN 3 THEN 'DayEndBalance' ELSE 'Unknown' END AS 'StrChargeDistributionBasedOn'," +
                    "CD.ChargeType,CASE CD.ChargeType WHEN 1 THEN 'Regular' WHEN 2 THEN 'Recurring' ELSE 'Unknown' END AS 'StrChargeType'," +
                    "CD.DeductionWalletTypeId,WT.WalletTypeName,CD.ChargeValue," +
                    "CD.ChargeValueType,CASE CD.ChargeValueType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Percentage' ELSE 'Unknown' END AS 'StrChargeValueType' ," +
                    "CD.MakerCharge,CD.TakerCharge,CD.MinAmount,CD.MaxAmount,CD.Remarks,CD.Status," +
                    "CASE CD.Status WHEN 0 THEN 'Inactive' WHEN 1 THEN 'Active' WHEN 9 THEN 'Delete' ELSE 'Unknown' END AS 'StrStatus' " +
                    "FROM ChargeConfigurationDetail CD " +
                    "INNER JOIN WalletTypeMasters WT ON CD.DeductionWalletTypeId = WT.ID " +
                    "WHERE CD.Status < 9 AND (CD.ChargeConfigurationMasterID = {0} OR {0}=0) AND (CD.ChargeType = {1} OR {1}=0) AND (CD.ChargeValueType = {2} OR {2}=0) AND (CD.Status={3} OR {3}=999) AND (CD.ChargeDistributionBasedOn={4} OR {4}=0)";
                var data = _dbContext.ChargeConfigurationDetailRes.FromSql(Query, Convert.ToInt64(masterId == null ? 0 : masterId), Convert.ToInt64(chargeType == null ? 0 : chargeType), Convert.ToInt16(chargeValueType == null ? 0 : chargeValueType), Convert.ToInt16(status == null ? 999 : status), Convert.ToInt16(chargeDistributionBasedOn == null ? 0 : chargeDistributionBasedOn));
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ChargeConfigurationDetailRes GetChargeConfigDetailbyId(long detailID)
        {
            try
            {
                string Query = "SELECT CD.Id AS ChargeConfigDetailId,CD.ChargeConfigurationMasterID,CD.ChargeDistributionBasedOn," +
                    "CASE CD.ChargeDistributionBasedOn WHEN 1 THEN 'Regular' WHEN 2 THEN 'Volume' WHEN 3 THEN 'DayEndBalance' ELSE 'Unknown' END AS 'StrChargeDistributionBasedOn'," +
                    "CD.ChargeType,CASE CD.ChargeType WHEN 1 THEN 'Regular' WHEN 2 THEN 'Recurring' ELSE 'Unknown' END AS 'StrChargeType'," +
                    "CD.DeductionWalletTypeId,WT.WalletTypeName,CD.ChargeValue," +
                    "CD.ChargeValueType,CASE CD.ChargeValueType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Percentage' ELSE 'Unknown' END AS 'StrChargeValueType' ," +
                    "CD.MakerCharge,CD.TakerCharge,CD.MinAmount,CD.MaxAmount,CD.Remarks,CD.Status," +
                    "CASE CD.Status WHEN 0 THEN 'Inactive' WHEN 1 THEN 'Active' WHEN 9 THEN 'Delete' ELSE 'Unknown' END AS 'StrStatus' " +
                    "FROM ChargeConfigurationDetail CD " +
                    "INNER JOIN WalletTypeMasters WT ON CD.DeductionWalletTypeId = WT.ID " +
                    "WHERE CD.Status < 9 AND CD.Id = {0}";
                var data = _dbContext.ChargeConfigurationDetailRes.FromSql(Query, detailID);
                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Role and Acccess Rights Methods - Account updated by khushali 04-03-2019

        public List<GetRoleDetail2> ListRoleDetails(short? Status)
        {
            try
            {
                var Data = (from i in _dbContext.Roles
                            where (Status == null || (i.Status == Status && Status != null))
                            orderby i.Id descending
                            select new GetRoleDetail2
                            {
                                RoleID = i.Id,
                                RoleName = i.Name,
                                RoleDescription = i.Description,
                                //PermissionGroupID = i.PermissionGroupID,
                                Status = i.Status,
                                StrStatus = (i.Status == 1 ? "Enable" : i.Status == 0 ? "disable" : i.Status == 9 ? "Delete" : "Unknown")
                            });
                return Data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ApplicationGroupRoles> CheckGroupInUse(long PermissionGrpId)
        {
            try
            {
                var data = (from i in _dbContext.GroupRoleMapping
                            where i.PermissionGroupId == PermissionGrpId
                            select i).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<ApplicationGroupRoles> IsWithDuplicateRole(long id)
        {
            try
            {
                var data = (from i in _dbContext.GroupRoleMapping
                            where i.RoleId == id
                            select i).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool AddGroupRoleMappingData(long PermissionGrpID, long RoleID)
        {
            try
            {
                ApplicationGroupRoles NewObj = new ApplicationGroupRoles
                {
                    PermissionGroupId = PermissionGrpID,
                    RoleId = RoleID
                };
                _dbContext.GroupRoleMapping.AddAsync(NewObj);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetPermissionGroup> ListPermissionGrpDetail(DateTime? FromDate, DateTime? Todate, long? RoleId, short? Status)
        {
            try
            {
                string Query = "SELECT PG.Id,PG.CreatedDate,PG.Status,PG.GroupName,PG.GroupDescription,ISNULL(BR.Name,'') AS 'LinkedRole',ISNULL(BR.Id , 0) AS 'LinkedRoleID',PG.IPAddress," +
                    "ISNULL(BU.Username,'') AS 'CreatedBy',ISNULL((SELECT Username FROM BizUser WHERE Id = PG.UpdatedBy),'') AS 'UpdatedBy' " +
                    "FROM PermissionGroupMaster PG " +
                    "LEFT JOIN BizUser BU ON BU.Id = PG.CreatedBy " +
                    "LEFT JOIN GroupRoleMapping GRM ON GRM.PermissionGroupId = PG.Id " +
                    "LEFT JOIN BizRoles BR ON GRM.RoleId = BR.Id " +
                    "WHERE (PG.Status={0} OR {0}=999) ";
                if (FromDate != null && Todate != null)
                {
                    FromDate = Convert.ToDateTime(FromDate).AddHours(00).AddMinutes(00).AddSeconds(00);
                    Todate = Convert.ToDateTime(Todate).AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += "AND PG.CreatedDate >= {2} AND PG.CreatedDate <= {3} AND (GRM.RoleId={1} OR {1}=0) ORDER BY PG.Id DESC";
                    var data1 = _dbContext.ListPermissionGroup.FromSql(Query, (Status == null ? 999 : Status), (RoleId == null ? 0 : RoleId), FromDate, Todate);
                    return data1.ToList();
                }
                Query += " AND (GRM.RoleId={1} OR {1}=0) ORDER BY PG.Id DESC";
                var data = _dbContext.ListPermissionGroup.FromSql(Query, (Status == null ? 999 : Status), (RoleId == null ? 0 : RoleId));
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetRoleHistoryData> GetRoleHistoryData(long? UserId, DateTime? FromDate, DateTime? ToDate, long? ModuleId, short? Status)
        {
            try
            {
                string Query = "SELECT RH.Id,ISNULL(RH.UpdatedDate,'') AS 'UpdatedDate',RH.Status,RH.ModificationDetail," +
                                "CASE RH.Module WHEN 1 THEN 'Role' WHEN 2 THEN 'Group' WHEN 3 THEN 'Permission' WHEN 4 THEN 'User' ELSE 'Unknown' END AS 'Module'," +
                                "ISNULL(RH.IPAddress,'N/A') AS 'IPAddress',BU.UserName FROM RoleHistory RH " +
                                "INNER JOIN BizUser BU ON RH.UserId = BU.Id " +
                                //"INNER JOIN ModuleMaster MM ON RH.Module = MM.Id " +
                                "WHERE (RH.UserId = {0} OR {0}=0) AND (RH.Module={1} OR {1}=0) AND (RH.Status = {2} OR {2}=999)";
                if (FromDate != null && ToDate != null)
                {
                    ToDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += " AND (RH.CreatedDate BETWEEN {3} AND {4})";
                    var data = _dbContext.GetRoleHistoryData.FromSql(Query, Convert.ToInt64(UserId == null ? 0 : UserId), Convert.ToInt64(ModuleId == null ? 0 : ModuleId), Convert.ToInt16(Status == null ? 999 : Status), Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate));
                    return data.ToList();
                }
                else
                {
                    var data = _dbContext.GetRoleHistoryData.FromSql(Query, Convert.ToInt64(UserId == null ? 0 : UserId), Convert.ToInt64(ModuleId == null ? 0 : ModuleId), Convert.ToInt16(Status == null ? 999 : Status));
                    return data.ToList();
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetPermissionGroup2 GetPermissionGrpDetail(long permissionGroupId)
        {
            try
            {
                string Query = "SELECT PG.Id AS 'PermissionGroupID',PG.CreatedDate,PG.Status,PG.GroupName,PG.GroupDescription AS 'Description',ISNULL(BR.Name , '') AS 'LinkedRole',ISNULL(BR.Id , 0) AS 'LinkedRoleId',PG.IPAddress, " +
                    "ISNULL(BU.Username,'') AS 'CreatedBy',ISNULL((SELECT Username FROM BizUser WHERE Id = PG.UpdatedBy),'') AS 'UpdatedBy' " +
                    "FROM PermissionGroupMaster PG " +
                    "LEFT JOIN BizUser BU ON BU.Id = PG.CreatedBy LEFT JOIN GroupRoleMapping GRM ON GRM.PermissionGroupId = PG.Id LEFT JOIN BizRoles BR ON GRM.RoleId = BR.Id WHERE PG.Id={0}";
                var data = _dbContext.GetPermissionGroup.FromSql(Query, permissionGroupId);
                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetUserDetail> GetUserListData(string Username = null, string Mobile = null, string Email = null, DateTime? FromDate = null, DateTime? ToDate = null, int IsAdmin = 9) //change in parameter for filtration by mansi -02-09-2019
        {
            try
            {
                //string Query = "SELECT BU.Id AS 'UserId',BU.UserName,BU.Email,ISNULL(BU.FirstName, '') AS 'FirstName',ISNULL(BU.LastName, '') AS 'LastName'," +
                //                "ISNULL(BU.Mobile, '') AS 'Mobile',ISNULL(BR.Id,0) AS 'RoleId',ISNULL(BR.Name,'') AS 'RoleName',ISNULL(PGM.GroupName,'') AS 'PermissionGroup',BU.Status, BU.GroupID " +
                //                "FROM BizUser BU " +
                //                "LEFT JOIN BizUserRole BUR ON BUR.UserId = BU.Id " +
                //                "LEFT JOIN BizRoles BR ON BUR.RoleId = BR.Id " +
                //                "LEFT JOIN GroupRoleMapping PM ON PM.RoleId = BUR.RoleId " +
                //                "LEFT JOIN PermissionGroupMaster PGM ON PM.PermissionGroupId = PGM.Id ORDER BY BU.Id DESC";//+
                //"WHERE BU.IsEnabled = 1";//AND BU.Status = 1
                // Mansi 31-07-2019 Added groupname from modulegroupmaster table
                string Condition = "1=1";

                if (Username != null)
                {
                    Condition += " and Username='" + Username + "'";
                }
                if (Mobile != null)
                {
                    Condition += " and Mobile='" + Mobile + "'";
                }
                if (Email != null)
                {
                    Condition += " and Email='" + Email + "'";
                }

                if (FromDate != null && ToDate != null)
                {
                    FromDate = Convert.ToDateTime(FromDate).Date.AddHours(0).AddMinutes(0).AddSeconds(0);
                    ToDate = Convert.ToDateTime(ToDate).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    Condition += " and BU.CreatedDate BETWEEN {0} AND {1} ";
                }
                if (IsAdmin != 9)
                {
                    Condition += " and IsCreatedByAdmin=" + IsAdmin;
                }





                string Query = @"SELECT BU.Id AS 'UserId',BU.UserName,BU.Email,ISNULL(BU.FirstName, '') AS 'FirstName',ISNULL(BU.LastName, '') AS 'LastName',
                                ISNULL(BU.Mobile, '') AS 'Mobile',ISNULL(BR.Id, 0) AS 'RoleId',ISNULL(BR.Name, '') AS 'RoleName',BU.Status, BU.GroupID ,ISNULL(mgp.GroupName, '-') AS 'PermissionGroup',BU.CreatedDate as  CreatedDate,
                                BU.IsCreatedByAdmin AS 'IsAdmin',CASE BU.IsCreatedByAdmin WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrIsAdmin', case when BU.Lockoutend is null then cast(0 as smallint)  when BU.Lockoutend <= getdate() then cast(0 as smallint) when BU.Lockoutend >=getdate() then cast(1 as smallint) end as IsLockOut ,cast(BU.TwoFactorEnabled as smallint)  AS TwoFactorEnabled FROM BizUser BU 
                                LEFT JOIN BizUserRole BUR ON BUR.UserId = BU.Id LEFT JOIN BizRoles BR ON BUR.RoleId = BR.Id
                                LEFT JOIN GroupRoleMapping PM ON PM.RoleId = BUR.RoleId LEFT Join modulegroupmaster mgp on BU.GroupID = mgp.Id where " + Condition + " ORDER BY BU.Id DESC";

                var data = _dbContext.GetUserDetail.FromSql(Query, FromDate, ToDate);

                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetUserDetail GetUserDataById(long userId)
        {
            try
            {
                string Query = "SELECT BU.Id AS 'UserId',BU.UserName,BU.Email,ISNULL(BU.FirstName, '') AS 'FirstName',ISNULL(BU.LastName, '') AS 'LastName'," +
                                  "ISNULL(BU.Mobile, '') AS 'Mobile', ISNULL(BR.Id,0) AS 'RoleId',ISNULL(BR.Name,'') AS 'RoleName',ISNULL(PGM.GroupName,'') AS 'PermissionGroup',BU.CreatedDate as  CreatedDate,BU.Status, BU.GroupID , " +
                                  "BU.IsCreatedByAdmin AS 'IsAdmin',CASE BU.IsCreatedByAdmin WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrIsAdmin', case when BU.Lockoutend is null then cast(0 as smallint)  when BU.Lockoutend <= getdate() then cast(0 as smallint) when BU.Lockoutend >=getdate() then cast(1 as smallint) end as IsLockOut ,cast(BU.TwoFactorEnabled as smallint)  AS TwoFactorEnabled  " +
                                  "FROM BizUser BU " +
                                  "LEFT JOIN BizUserRole BUR ON BUR.UserId = BU.Id " +
                                  "LEFT JOIN BizRoles BR ON BUR.RoleId = BR.Id " +
                                  "LEFT JOIN GroupRoleMapping PM ON PM.RoleId = BUR.RoleId " +
                                  "LEFT JOIN PermissionGroupMaster PGM ON PM.PermissionGroupId = PGM.Id " +
                                  "WHERE BU.Id={0}"; //BU.IsEnabled = 1 AND BU.Status = 1 AND 
                var data = _dbContext.GetUserDetail.FromSql(Query, userId);
                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetUserDetail> SearchUserData(string searchText)
        {
            try
            {
                string Query = "SELECT BU.Id AS 'UserId',BU.UserName,BU.Email,ISNULL(BU.FirstName, '') AS 'FirstName',ISNULL(BU.LastName, '') AS 'LastName'," +
                                "ISNULL(BU.Mobile, '') AS 'Mobile', ISNULL(BR.Id,0) AS 'RoleId',ISNULL(BR.Name,'') AS 'RoleName',ISNULL(PGM.GroupName,'') AS 'PermissionGroup',BU.CreatedDate as  CreatedDate," +
                                "BU.Status, BU.GroupID,BU.IsCreatedByAdmin AS 'IsAdmin',CASE BU.IsCreatedByAdmin WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrIsAdmin', case when BU.Lockoutend is null then cast(0 as smallint)  when BU.Lockoutend <= getdate() then cast(0 as smallint) when BU.Lockoutend >=getdate() then cast(1 as smallint) end as IsLockOut,cast(BU.TwoFactorEnabled as smallint)  AS TwoFactorEnabled  " +
                                "FROM BizUser BU " +
                                "LEFT JOIN BizUserRole BUR ON BUR.UserId = BU.Id " +
                                "LEFT JOIN BizRoles BR ON BUR.RoleId = BR.Id " +
                                "LEFT JOIN GroupRoleMapping PM ON PM.RoleId = BUR.RoleId " +
                                "LEFT JOIN PermissionGroupMaster PGM ON PM.PermissionGroupId = PGM.Id " +
                                "WHERE BU.Status = 1 AND BU.UserName LIKE '%" + searchText + "%' OR BU.Email LIKE '%" + searchText + "%'";//BU.IsEnabled = 1
                var data = _dbContext.GetUserDetail.FromSql(Query);
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //khuhsali 29-04-2019 initial level master entry for group access right
        public async Task<bool> Callsp_AddGroupAccessRight(long GroupId, long UserID, long ErrorCode = 0)
        {
            bool ReturnCode = false;
            try
            {
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@GroupID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Default,GroupId),
                new SqlParameter("@CreatedBy",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Default, UserID),
                new SqlParameter("@ReturnCode",SqlDbType.Bit, 10, ParameterDirection.Output, false, 28, 8, string.Empty, DataRowVersion.Default, ReturnCode),
                new SqlParameter("@ErrorCode",SqlDbType.BigInt, 10, ParameterDirection.InputOutput, false, 0, 0, string.Empty, DataRowVersion.Default, ErrorCode),

            };
                var res = _dbContext.Database.ExecuteSqlCommand("Sp_AddGroupAccessRight @GroupID ,@CreatedBy, @ReturnCode OUTPUT, @ErrorCode OUTPUT", param1);
                ReturnCode = Convert.ToBoolean(@param1[2].Value);
                ErrorCode = Convert.ToInt64(@param1[3].Value);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("Sp_AddGroupAccessRight", "ControlPanelRepository", "##GroupId:" + GroupId + " ,CreatedBy=" + UserID + ",ReturnCode=" + ReturnCode + ",ErrorCode=" + ErrorCode));

                return ReturnCode;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CallSp_AddGroupAccessRight ##GroupId " + GroupId, "ControlPanelRepository", ex);
                throw ex;
            }
        }

        public List<MenuSubDetailViewModelV2> GetGroupAccessRightsGroupWise(long GroupID, string ParentID, bool IscCheckStatus = false)
        {
            try
            {
                string Query = "select MG.ID AS ModuleGroupAccessID, SM.ModuleID AS ModuelID,SM.ID,MG.Status,ISNULL(SM.SubModuleName,'') AS ModuleName,SM.ParentID, " +
                    "(SELECT STRING_AGG(Result1.UtilityValue, ',') FROM (Select UtilityValue From ModuleUtilityMaster Where ID in (SELECT value FROM STRING_SPLIT(MG.UtilityTypes, ','))) as Result1) As Utility, " +
                    "(SELECT STRING_AGG(Result1.OptValue, ',') FROM (Select OptValue From ModuleCRUDOptMaster Where ID in (SELECT value FROM STRING_SPLIT(MG.CrudTypes, ','))) as Result1) As CrudOption, " +
                    "SM.GUID,SM.ParentGUID , ISNULL((SELECT TOP 1 TypeValue FROM ModuleTypeMaster WHERE Id = SM.Type),'') As Type, " +
                    "case when (SELECT COUNT(ID) FROM SubModuleMaster WHERE ParentGUID = SM.GUID) > 0 Then 1 else 0 End As HasChild, " +
                    "case when (SELECT COUNT(ID) FROM FieldMaster WHERE SubModuleID = SM.Id) > 0 Then  1 else  0 End As HasField " +
                    "from SubModuleMaster SM INNER JOIN ModuleGroupAccess MG ON MG.SubModuleID = SM.ID " +
                    "WHere SM.ParentGUID = {0} and GroupID =  {1}  ";
                if (IscCheckStatus)
                {
                    Query = Query + " and MG.Status = 1";
                }
                //string Query = "SELECT SM.ModuleID,SM.ID,MG.Status,ISNULL(SM.SubModuleName,'') AS ModuleName,SM.ParentID," +
                //    "ISNULL(MG.UtilityTypes,'') As  Utility, ISNULL(MG.CrudTypes,'') AS CrudOption,SM.GUID,SM.ParentGUID ," +
                //    " ISNULL((SELECT TOP 1 TypeValue FROM ModuleTypeMaster WHERE Id = SM.Id),'') As Type, " +
                //    "case when (SELECT COUNT(ID) FROM SubModuleMaster WHERE ParentGUID = SM.GUID) > 0 Then 1 else 0 End As HasChild, " +
                //    "case when (SELECT COUNT(ID) FROM FieldMaster WHERE SubModuleID = SM.Id) > 0 Then 1 else 0 End As HasField " +
                //    "from SubModuleMaster SM INNER JOIN ModuleGroupAccess MG ON MG.SubModuleID = SM.ID " +
                //    "WHere SM.ParentGUID = {0} and GroupID =  {1} ";
                var data = _dbContext.MenuSubDetailViewModelV2.FromSql(Query, ParentID, GroupID).ToList();

                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool InsertFieldEntryGroupWise(long FieldID)
        {
            try
            {
                string Query = @"INSERT INTO SubModuleFormMaster 
                    (CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, Status, ModuleGroupAccessID, CrudTypes, FieldID, Visibility) 
                    SELECT dbo.GetISTDate(),{0},null,null,FM.AccressRight AS Status,MG.Id As ModuleGroupAccessID,100,FM.Id AS FieldID,1 
                    FROM FieldMaster FM INNER JOIN ModuleGroupAccess MG ON MG.SubModuleID = FM.SubModuleID where FM.ID = {1}";

                var data = _dbContext.Database.ExecuteSqlCommand(Query, 999, FieldID);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        public bool InsertSubmoduleEntryGroupWise(long SubModuleID, long GroupID)
        {
            try
            {
                string Query = @"INSERT INTO ModuleGroupAccess 
                    (CreatedDate,CreatedBy,UpdatedBy,UpdatedDate,Status,GroupID,UtilityTypes,CrudTypes,SubModuleID) 
                    SELECT dbo.GetISTDate(),{0},null,null,1,{1},UtilityTypes,CrudTypes,SM.Id AS SubModuleID 
                    FROM SubModuleMaster SM where SM.Id = {2}";

                var data = _dbContext.Database.ExecuteSqlCommand(Query, 999, GroupID, SubModuleID);
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        public List<ChildNodeFiledViewModel> GetFormAccessRightsGroupWise(long ModuleGroupAccessID, bool IscCheckStatus = false)
        {
            try
            {
                string Query = "SELECT SM.Id AS ModuleFormAccessID, FM.ID As FiledID ,ISNULL(FM.FieldName,'') AS FieldName  , " +
                    "ISNULL((SELECT TOP 1 Value FROM ModuleFieldRequirerMaster WHERE Id = FM.Required),'') As Required ," +
                    " ISNULL((SELECT TOP 1 VisibilityValue FROM ModuleVisibilityMaster WHERE Id = SM.Visibility),'') As Visibility," +
                    " ISNULL((SELECT TOP 1 AccessRightValue FROM ModuleAccessRightsMaster WHERE Id = SM.Status),'') As AccessRight , " +
                    "FM.GUID FROM SubModuleFormMaster SM INNER JOIN FieldMaster FM ON FM.ID = SM.FieldID where SM.ModuleGroupAccessID = {0}";
                if (IscCheckStatus)
                {
                    Query = Query + " and FM.Status = 1";
                }
                var data = _dbContext.ChildNodeFiledViewModel.FromSql(Query, ModuleGroupAccessID).ToList();

                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int UpdateModuleGroupAccess(long UserID, long GroupID, long ModuleGroupAccessID, short Status, string CrudOption, string Utility)
        {
            try
            {
                string Query = "Update ModuleGroupAccess SET UpdatedBy={0}, UpdatedDate=dbo.GetISTDate() , Status = {1} ," +
                    " CrudTypes = {2} , UtilityTypes = {3} WHere ID = {4} and GroupID =  {5}";
                var RowAffected = _dbContext.Database.ExecuteSqlCommand(Query, UserID, Status, CrudOption, Utility, ModuleGroupAccessID, GroupID);

                return RowAffected;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int UpdateModuleFieldpAccess(long UserID, long ModuleGroupAccessID, long ModuleFormAccessID, short Visibility, short AccessRights)
        {
            try
            {
                string Query = "Update SubModuleFormMaster SET UpdatedBy={0}, UpdatedDate=dbo.GetISTDate() , Visibility = {1} , Status = {2} " +
                    "WHere ID = {3}  and ModuleGroupAccessID =  {4}";
                var RowAffected = _dbContext.Database.ExecuteSqlCommand(Query, UserID, Visibility, AccessRights, ModuleFormAccessID, ModuleGroupAccessID);

                return RowAffected;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Deposition Recon

        public List<Dipositions> GetDepositionHistorydata(string trnid, string address, short? isInternal, long? userID, string coin, long? provider, int pageNo, int pageSize, ref int TotalCount)
        {
            try
            {
                //CAST(CASE DH.Status WHEN 1 THEN '{\"Data\":[{\"ActionStage\":\"1\",\"ActionMsg\":\"Fail\"}]}' WHEN 0 THEN '{\"Data\":[{\"ActionStage\":\"3\",\"ActionMsg\": \"Success\"},{\"ActionStage\":\"4\",\"ActionMsg\":\"Fail\"}]' WHEN 9 THEN '{\"Data\":[{\"ActionStage\":\"5\",\"ActionMsg\":\"Success\"}]}' END AS VARCHAR(800)) AS 'AvailableAction'"
                string Query = "SELECT DH.Id AS 'TrnNo',DH.TrnID,DH.SMSCode AS 'CoinName',DH.Address,DH.Amount,DH.Confirmations,DH.ConfirmedTime," +
                                "ISNULL(DH.FromAddress, '-') AS 'FromAddress',DH.SerProId AS 'ServiceProviderId',ISNULL(SPM.ProviderName, '-') AS 'ProviderName'," +
                                "ISNULL(DH.IsInternalTrn, 0) AS 'IsInternalTrn',ISNULL(BU.Email, '-') AS 'Email'," +
                                "(CASE DH.Status WHEN 0 THEN 'Processing' WHEN 1 THEN 'Success' WHEN 9 THEN 'Failed' else 'Other' END) AS 'StatusStr'," +
                                "ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'), '[]') AS 'ExplorerLink'," +
                                "DH.Status,DH.StatusMsg " +
                                "FROM DepositHistory DH " +
                                "LEFT JOIN ServiceProviderMaster SPM ON DH.SerProID = SPM.Id " +
                                "LEFT JOIN BizUser BU ON DH.UserId = BU.Id " +
                                "LEFT JOIN ServiceMaster SM ON DH.SMSCode = SM.SMSCode " +
                                "LEFT JOIN ServiceDetail SD ON SM.Id = SD.ServiceId " +
                                "WHERE (DH.TrnID={0} OR {0}='0') AND (DH.Address={1} OR {1}='0') AND (DH.IsInternalTrn={2} OR {2}=999) " +
                                "AND (DH.UserId={3} OR {3}=0) AND (DH.SMSCode={4} OR {4}='0') AND (DH.SerProID={5} OR {5}=0)";
                var data = _dbContext.Dipositions.FromSql(Query, (trnid ?? "0"), (address ?? "0"), (isInternal == null ? 999 : isInternal), (userID == null ? 0 : userID), (coin ?? "0"), (provider == null ? 0 : provider)).ToList();
                TotalCount = data.Count();
                if (pageNo > 0)
                {
                    int skip = Convert.ToInt32((pageSize) * (pageNo - 1));
                    data = data.Skip(skip).Take(Convert.ToInt32(pageSize)).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool CheckUserExistWithAnyRole(long id)
        {
            try
            {
                string Query = "SELECT RoleId FROM BizUserRole WHERE UserId = {0}";
                var data = _dbContext.GetRoleId.FromSql(Query, id);
                if (data != null)
                {
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

        // khushali 04-03-2019  -- Get permission Group ID by Linked Role
        public List<ApplicationGroupRoles> GetPermissionGroupIDByLinkedRole(long RoleID)
        {
            try
            {
                var data = (from i in _dbContext.GroupRoleMapping
                            where i.RoleId == RoleID
                            select i).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //return null;
                throw ex;
            }
        }

        // khushali 04-03-2019  -- View Unassigned Users
        public List<ViewUnAssignedUserRes> ViewUnassignedUsers(string UserName, DateTime? FromDate, DateTime? ToDate, short? Status)
        {
            try
            {
                string Query = "select Id,ISnull(Email,'') AS Email, " +
                                "ISnull(UserName, '') AS UserName, ISnull(CreatedDate, '') AS CreatedDate, ISnull(Status, 0) AS Status, '' AS RoleName  " +
                                "from bizuser where (Status={0} OR {0}=999) ";
                if (!string.IsNullOrEmpty(UserName))
                {
                    Query += " AND UserName LIKE '%" + UserName + "%'";
                }
                Query += " AND id in (select Id from bizuser EXCEPT  select userid from bizuserrole)";
                if (FromDate != null && ToDate != null)
                {
                    DateTime NewFrmDate = Convert.ToDateTime(FromDate);
                    DateTime NewToDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += " AND CreatedDate BETWEEN {1} AND {2}";
                    var data = _dbContext.ViewUsers.FromSql(Query, Convert.ToInt16(Status == null ? 999 : Status), NewFrmDate, NewToDate);
                    return data.ToList();
                }
                else
                {
                    var data = _dbContext.ViewUsers.FromSql(Query, Convert.ToInt16(Status == null ? 999 : Status));
                    return data.ToList();
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region WalletTrnLimitConfiguration

        public List<WalletTrnLimitConfigResp> ListMasterLimitConfig(long? walletTypeId, long? trnType, EnIsKYCEnable? isKYCEnable, short? status)
        {
            try
            {
                string Query = "SELECT LC.Id,LC.TrnType,CASE LC.TrnType WHEN 1 THEN 'Trading' WHEN 2 THEN 'Deposit' WHEN 4 THEN 'API Call' WHEN 9 THEN 'Withdraw' ELSE 'Unknown' END AS StrTrnType," +
                                "LC.WalletType,WTM.WalletTypeName,LC.StartTime,LC.EndTime,LC.MinAmount AS 'PerTranMinAmount',LC.MaxAmount AS 'PerTranMaxAmount'," +
                                "LC.HourlyTrnCount,LC.HourlyTrnAmount,LC.DailyTrnCount,LC.DailyTrnAmount,LC.WeeklyTrnCount,LC.WeeklyTrnAmount," +
                                "LC.MonthlyTrnCount,LC.MonthlyTrnAmount,LC.YearlyTrnCount,LC.YearlyTrnAmount,LC.IsKYCEnable,LC.Status " +
                                "FROM WalletTrnLimitConfiguration LC " +
                                "LEFT JOIN WalletTypeMasters WTM ON WTM.ID = LC.WalletType " +
                                "WHERE LC.Status < 9 AND (LC.WalletType={0} OR {0}=0) AND (LC.TrnType={1} OR {1}=0) AND (LC.IsKYCEnable={2} OR {2}=999) AND (LC.Status={3} OR {3}=999)";
                var data = _dbContext.WalletTrnLimitConfig.FromSql(Query, (walletTypeId == null ? 0 : walletTypeId), (trnType == null ? 0 : trnType), Convert.ToInt16(isKYCEnable == null ? 999 : Convert.ToInt16(isKYCEnable)), (status == null ? 999 : status));
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public WalletTrnLimitConfigResp GetMasterLimitConfig(long id)
        {
            try
            {
                string Query = "SELECT LC.Id,LC.TrnType,CASE LC.TrnType WHEN 1 THEN 'Trading' WHEN 2 THEN 'Deposit' WHEN 4 THEN 'API Call' WHEN 9 THEN 'Withdraw' ELSE 'Unknown' END AS StrTrnType," +
                                "LC.WalletType,WTM.WalletTypeName,LC.StartTime,LC.EndTime,LC.MinAmount AS 'PerTranMinAmount',LC.MaxAmount AS 'PerTranMaxAmount'," +
                                "LC.HourlyTrnCount,LC.HourlyTrnAmount,LC.DailyTrnCount,LC.DailyTrnAmount,LC.WeeklyTrnCount,LC.WeeklyTrnAmount," +
                                "LC.MonthlyTrnCount,LC.MonthlyTrnAmount,LC.YearlyTrnCount,LC.YearlyTrnAmount,LC.IsKYCEnable,LC.Status " +
                                "FROM WalletTrnLimitConfiguration LC " +
                                "LEFT JOIN WalletTypeMasters WTM ON WTM.ID = LC.WalletType " +
                                "WHERE LC.Id={0}";
                var data = _dbContext.WalletTrnLimitConfig.FromSql(Query, id);
                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region TradingChargeTypeMaster

        public List<TradingChargeTypeRes> ListTradingChargeTypeMaster()
        {
            try
            {
                var data = (from t in _dbContext.TradingChargeTypeMaster
                            where t.Status < 9
                            select new TradingChargeTypeRes
                            {
                                Id = t.Id,
                                TypeName = t.TypeName,
                                Type = Convert.ToInt16(t.Type),
                                IsChargeFreeMarketEnabled = t.IsChargeFreeMarketEnabled,
                                IsCommonCurrencyDeductEnable = t.IsCommonCurrencyDeductEnable,
                                IsDeductChargeMarketCurrency = t.IsDeductChargeMarketCurrency,
                                DiscountPercent = t.DiscountPercent,
                                DeductCurrency = t.DeductCurrency,
                                Status = t.Status
                            }).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<MarketCurrencyRes> ListChargeFreeMarketCurrencyMaster()
        {
            try
            {
                var data = (from t in _dbContext.ChargeFreeMarketCurrencyMaster
                            select new MarketCurrencyRes
                            {
                                Id = t.Id,
                                Status = t.Status,
                                MarketCurrency = t.MarketCurrency
                            }).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Deposit Interval
        public List<DepositionIntervalListViewModel> ListDepositionInterval()
        {
            try
            {
                var data = (from t in _dbContext.DepositionInterval
                            select new DepositionIntervalListViewModel
                            {
                                Id = t.Id,
                                CreatedDate = t.CreatedDate,
                                Status = t.Status,
                                CreatedBy = t.CreatedBy,
                                DepositHistoryFetchListInterval = t.DepositHistoryFetchListInterval,
                                DepositStatusCheckInterval = t.DepositStatusCheckInterval

                            }).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public DepositionIntervalListViewModel FirstDepositionInterval()
        {
            try
            {
                var data = (from t in _dbContext.DepositionInterval
                            select new DepositionIntervalListViewModel
                            {
                                Id = t.Id,
                                CreatedDate = t.CreatedDate,
                                CreatedBy = t.CreatedBy,
                                Status = t.Status,
                                DepositHistoryFetchListInterval = t.DepositHistoryFetchListInterval,
                                DepositStatusCheckInterval = t.DepositStatusCheckInterval

                            }).FirstOrDefault();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public DepositionIntervalListViewModel GetDepositionInterval(long Id)
        {
            try
            {
                var data = (from t in _dbContext.DepositionInterval
                            where t.Id.Equals(Id)
                            select new DepositionIntervalListViewModel
                            {
                                Id = t.Id,
                                CreatedDate = t.CreatedDate,
                                CreatedBy = t.CreatedBy,
                                Status = t.Status,
                                DepositHistoryFetchListInterval = t.DepositHistoryFetchListInterval,
                                DepositStatusCheckInterval = t.DepositStatusCheckInterval

                            }).FirstOrDefault();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //public DepositionIntervalListViewModel GetFirstActiveDepositionInterval()
        //{
        //    try
        //    {
        //        var data = (from t in _dbContext.DepositionInterval
        //                    where t.Status.Equals('1')
        //                    select new DepositionIntervalListViewModel
        //                    {
        //                        Id = t.Id,
        //                        CreatedDate = t.CreatedDate,
        //                        Status = t.Status,
        //                        DepositHistoryFetchListInterval = t.DepositHistoryFetchListInterval,
        //                        DepositStatusCheckInterval = t.DepositStatusCheckInterval

        //                    }).FirstOrDefault();
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        throw ex;
        //    }
        //}
        #endregion

        #region BGTASK

        public AffiliateCommissionCron GetCronData()
        {
            try
            {
                var data = (from p in _dbContext.AffiliateCommissionCron
                            where p.SchemeMappingId == 999
                            orderby p.Id descending
                            select p).FirstOrDefault();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        public List<TransactionProviderResponse> GetProviderDataListAsync(long ServiceProviderId, enTrnType? TransactionType, string CurrencyName)
        {
            try
            {
                string Query = "select SC.ID as ServiceID,SC.Name as ServiceName,Prc.ID as SerProDetailID,Prc.ServiceProID,RC.ID as RouteID," +
                    "PC.ID as ProductID,RC.RouteName,cast(SC.ServiceType as int) as ServiceType,Prc.ThirPartyAPIID,Prc.AppTypeID," +
                    "LC.MinAmt as MinimumAmountItem,LC.MaxAmt as MaximumAmountItem,RC.ProviderWalletID,RC.OpCode,cast(1 as bigint) as ProTypeID ," +
                    "'' as APIBalURL,'' as APISendURL,'' as APIValidateURL,'' as ContentType,'' as MethodType,0 as ParsingDataID," +
                    "RC.AccNoStartsWith,RC.AccNoValidationRegex,RC.AccountNoLen " +
                    "from ServiceMaster SC " +
                    "inner join  ProductConfiguration PC on PC.ServiceID = SC.Id " +
                    "inner join RouteConfiguration RC on RC.ProductID = PC.Id " +
                    "inner join ServiceProviderDetail PrC on Prc.Id = RC.SerProDetailID AND (Prc.TrnTypeID ={0} OR {0}=0) " +
                    "inner join Limits LC on LC.ID = RC.LimitID " +
                    "where (RC.TrnType ={0} OR {0}=0)	" +
                    "and SC.Status = 1 and RC.Status = 1 and Prc.Status = 1 and PrC.ServiceProID = {1}";
                if (!String.IsNullOrEmpty(CurrencyName))
                {
                    Query += "AND SC.SMSCode={2}";
                }
                Query += " order by RC.Priority";
                var Result = _dbContext.TransactionProviderResponse.FromSql(Query, (TransactionType == null ? 0 : Convert.ToInt32(TransactionType)), ServiceProviderId, CurrencyName);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<BlockUserRes> ListBlockUserAddresses(long? userId, string address, DateTime? fromDate, DateTime? toDate, short? status)
        {
            try
            {
                string Query = "SELECT BUA.Id,BUA.CreatedDate,BUA.CreatedBy AS 'BlockedByUserId'," +
                    "(select FirstName + ' ' + LastName FROM BizUser WHERE Id = BUA.CreatedBy) AS 'BlockedByUserName',BUA.Status," +
                    "CASE BUA.Status WHEN 1 THEN 'Blocked' WHEN 2 THEN 'Unblocked' ELSE 'Unknown' END AS 'StrStatus'," +
                    "BUA.UserID AS 'BlockedForUserId',BU.FirstName + ' ' + BU.LastName AS 'BlockedForUserName',BUA.WalletID," +
                    "BUA.Address,BUA.WalletTypeID,WT.WalletTypeName,BUA.Remarks " +
                    "FROM BlockUnblockUserAddress BUA " +
                    "INNER JOIN WalletTypeMasters WT ON WT.Id = BUA.WalletTypeID " +
                    "INNER JOIN BizUser BU ON BU.Id = BUA.UserID " +
                    "WHERE BUA.Status < 3 AND (BUA.UserId={0} OR {0}=0) AND (BUA.Address={1} OR {1}='') AND (BUA.Status={2} OR {2}=999) ";
                if (fromDate != null && toDate != null)
                {
                    fromDate = Convert.ToDateTime(fromDate).Date.AddHours(0).AddMinutes(0).AddSeconds(0);
                    toDate = Convert.ToDateTime(toDate).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += "AND BUA.CreatedDate BETWEEN {3} AND {4} ";
                }
                string add = String.IsNullOrEmpty(address) ? "" : address.ToString();
                var Result = _dbContext.BlockUserRes.FromSql(Query, (userId == null ? 0 : userId), add, Convert.ToInt16(status == null ? 999 : status), fromDate, toDate).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<WithdrwalAdminReqRes> ListWithdrawalReqData(long? trnNo, DateTime? fromDate, DateTime? toDate, short? status)
        {
            try
            {
                string Query = "SELECT WR.Id AS 'AdminReqId',WR.TrnNo,WR.CreatedBy AS 'ActionByUserId',WR.Status," +
                    "CASE WR.Status WHEN 0 THEN 'Pending' WHEN 1 THEN 'Approved' WHEN 9 THEN 'Reject' ELSE '-' END AS 'StrStatus',ISNULL(BU.FirstName + ' ' + BU.LastName, '-') AS 'ActionByUserName',WR.CreatedDate AS 'ActionDate',WR.Remarks " +
                    ",TQ.TrnDate,TQ.Amount,TQ.SMSCode AS 'Currency' FROM WithdrawAdminRequest WR " +
                    "LEFT JOIN BizUser BU ON BU.Id = WR.ApprovedBy LEFT JOIN TransactionQueue TQ ON TQ.Id = WR.TrnNo " +
                    "WHERE (WR.TrnNo={0} OR {0}=0) AND (WR.Status={1} OR {1}=999)";
                if (fromDate != null && toDate != null)
                {
                    fromDate = Convert.ToDateTime(fromDate).Date;
                    toDate = Convert.ToDateTime(toDate).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += " AND WR.CreatedDate BETWEEN {2} AND {3} ";
                }
                Query += " ORDER BY WR.CreatedDate DESC";
                var data = _dbContext.WithdrwalAdminReqRes.FromSql(Query, (trnNo == null ? 0 : trnNo), (status == null ? 999 : status), fromDate, toDate);
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TokenSupplyRes> ListIncreaseDecreaseTokenSupply(long? walletTypeId, short? actionType, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                string Query = "SELECT TH.Id,TH.WalletTypeId,ISNULL(WT.WalletTypeName,'-') AS 'WalletTypeName'," +
                    "TH.Amount,TH.CreatedBy AS 'ActionByUserId',ISNULL(TH.TrnHash, '-') AS 'TrnHash'," +
                    "ISNULL(BU.FirstName + ' ' + BU.LastName, '-') AS 'ActionByUserName'," +
                    "TH.IsIncrease AS 'ActionType'," +
                    "CASE TH.IsIncrease WHEN 1 THEN 'Increase' WHEN 2 THEN 'Decrease' ELSE '-' END AS 'ActionTypeName'," +
                    "TH.CreatedDate AS 'ActionDate',TH.ContractAddress,TH.Remarks " +
                    "FROM TokenSupplyHistory TH " +
                    "LEFT JOIN BizUser BU ON BU.Id = TH.CreatedBy " +
                    "LEFT JOIN WalletTypeMasters WT ON WT.Id = TH.WalletTypeId " +
                    "WHERE (TH.WalletTypeId={0} OR {0}=0) AND (TH.IsIncrease={1} OR {1}=0) ";
                if (fromDate != null && toDate != null)
                {
                    fromDate = Convert.ToDateTime(fromDate).Date;
                    toDate = Convert.ToDateTime(toDate).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += " AND TH.CreatedDate BETWEEN {2} AND {3}";
                }
                Query += " ORDER BY TH.CreatedDate DESC";
                var data = _dbContext.TokenSupplyRes.FromSql(Query, (walletTypeId == null ? 0 : walletTypeId), (actionType == null ? 0 : actionType), fromDate, toDate);
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<DestroyBlackFundRes> ListDestroyedBlackFund(string address, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                if (String.IsNullOrEmpty(address))
                {
                    address = "";
                }
                string Query = "SELECT DR.Id,DR.Address,DR.CreatedBy AS 'ActionByUserId',ISNULL(DR.TrnHash,'-') AS 'TrnHash'," +
                    "ISNULL(BU.FirstName + ' ' + BU.LastName, '-') AS 'ActionByUserName',DR.CreatedDate AS 'ActionDate',DR.Remarks" +
                    " FROM DestroyFundRequest DR " +
                    "LEFT JOIN BizUser BU ON BU.Id = DR.CreatedBy " +
                    "WHERE (DR.Address={0} OR {0}='') ";
                if (fromDate != null && toDate != null)
                {
                    fromDate = Convert.ToDateTime(fromDate).Date;
                    toDate = Convert.ToDateTime(toDate).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += " AND DR.CreatedDate BETWEEN {1} and {2}";
                }
                Query += " ORDER BY DR.CreatedDate DESC";
                var data = _dbContext.DestroyBlackFundRes.FromSql(Query, address.ToString(), fromDate, toDate);
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TokenTransferRes> ListTokenTransferHistory(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                string Query = "SELECT TTH.Id,TTH.CreatedDate AS 'ActionDate',TTH.CreatedBy AS 'ActionByUserId'," +
                    "ISNULL(BU.FirstName + ' ' + BU.LastName, '-') AS 'ActionByUserName'," +
                    "TTH.FromAddress,TTH.ToAddress,TTH.Amount,TTH.TrnHash,TTH.Remarks " +
                    "FROM TokenTransferHistory TTH " +
                    "LEFT JOIN BizUser BU ON BU.Id = TTH.CreatedBy ";
                if (fromDate != null && toDate != null)
                {
                    fromDate = Convert.ToDateTime(fromDate).Date;
                    toDate = Convert.ToDateTime(toDate).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += " WHERE TTH.CreatedDate BETWEEN {0} AND {1} ";
                }
                Query += " ORDER BY TTH.CreatedDate DESC";
                var data = _dbContext.TokenTransferRes.FromSql(Query, fromDate, toDate);
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<SetTransferFeeRes> ListTransferFeeHistory(long? walletTypeId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                string Query = "SELECT TFH.Id,TFH.CreatedBy AS 'ActionByUserId'," +
                    "ISNULL(BU.FirstName + ' ' + BU.LastName, '-') AS 'ActionByUserName',TFH.CreatedDate AS 'ActionDate'," +
                    "TFH.WalletTypeId,ISNULL(WT.WalletTypeName, '-') AS 'WalletTypeName',TFH.ContractAddress," +
                    "TFH.BasePoint,TFH.Maxfee,TFH.Minfee,TFH.Remarks,TFH.TrnHash " +
                    "FROM TransferFeeHistory TFH " +
                    "LEFT JOIN BizUser BU ON BU.Id = TFH.CreatedBy " +
                    "LEFT JOIN WalletTypeMasters WT ON WT.Id = TFH.WalletTypeId " +
                    "WHERE (TFH.WalletTypeId={0} OR {0}=0) ";
                if (fromDate != null && toDate != null)
                {
                    fromDate = Convert.ToDateTime(fromDate).Date;
                    toDate = Convert.ToDateTime(toDate).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += " AND TFH.CreatedDate BETWEEN {1} AND {2} ";
                }
                Query += " ORDER BY TFH.CreatedDate DESC";
                var data = _dbContext.SetTransferFeeRes.FromSql(Query, (walletTypeId == null ? 0 : walletTypeId), fromDate, toDate);
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ListLPWalletMismatchRes ListLPWalletMismatchM(DateTime FromDate, DateTime ToDate, int Page, int PageSize, long WalletId, long SerProID, Int16 Status)
        {
            ListLPWalletMismatchRes ListLPWalletMismatchMResp = new ListLPWalletMismatchRes();
            try
            {
                if (FromDate != null && ToDate != null)
                {
                    FromDate = Convert.ToDateTime(FromDate).Date;
                    ToDate = Convert.ToDateTime(ToDate).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                var Query = "select isNull(MIS.guid,'') as Id,MIS.Status,CASE MIS.Status WHEN 1 THEN 'Success' WHEN 9 THEN 'Reject' ELSE 'InProcess' END as 'StrStatus'," +
                " MIS.WalletID as 'WalletTypeID',AWM.Walletname as 'WalletTypeName',AWM.Walletname as 'WalletTypeName',wtm.WalletTypeName as currencyName,AWM.SerProID,SPMA.ProviderName as 'SerProIDName',MIS.MismatchaingAmount " +
                " as 'MisMatchingAmount',MIS.SettledAmount,MIS.ResolvedBy,isNull(BZ.UserName,'') as 'ResolvedByName',MIS.ResolvedDate,isNull(MIS.ResolvedRemarks,'') as 'Remarks' " +
                " from LPWalletMismatch MIS INNER JOIN LPWalletMaster AWM On AWM.Id=MIS.WalletID INNER JOIN ServiceProviderMaster SPMA " +
                " On SPMA.Id=AWM.SerProID LEFT JOIN bizuser BZ On BZ.Id=MIS.ResolvedBy inner join WAlletTypeMasters wtm on AWM.WalletTypeID=wtm.Id  Where MIS.CreatedDate Between {0} And {1} " +
                " And (MIS.WalletID ={2} OR {2}=0) And (SPMA.Id={3} OR {3}=0) And (MIS.Status={4} OR {4}=999)";
                var items = _dbContext.LPWalletMismatchRes.FromSql(Query, FromDate, ToDate, WalletId, SerProID, Status).ToList();
                ListLPWalletMismatchMResp.TotalCount = items.Count;
                if (Page > 0)
                {
                    int skip = PageSize * (Page - 1);
                    items = items.Skip(skip).Take(PageSize).ToList();
                }
                ListLPWalletMismatchMResp.Data = items;
                return ListLPWalletMismatchMResp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #region User Suspicious Activity Report
        //Rushabh 09-05-2020

        public Task<List<UserWithdrawalDTO>> GetTotalWithdrawalHistory(long userId, short TrnType, short Status, string CurrencyName)
        {
            try
            {
                string Query_Internal = @"SELECT TOP 100 TQ.SMSCode,SUM(TQ.Amount) AS 'TotalAmount',dbo.ConvertRateUSD(TQ.SMSCode,SUM(TQ.Amount)) AS 'EquivalentUSDAmt' FROM TransactionQueue TQ
                                    INNER JOIN WithdrawHistory WH ON WH.TrnNo = TQ.Id AND WH.Status = {0}
                                    WHERE TQ.Status = {0} AND TQ.MemberId = {1} AND (TQ.IsInternalTrn = {2} OR {2} = 0) AND (TQ.SMSCode = {3} OR {3} = '0')
                                    GROUP BY TQ.SMSCode,TQ.MemberId ORDER BY TQ.MemberId";
                List<UserWithdrawalDTO> Resp_Internal = _dbContext.GetUserWithdrawalHistory.FromSql(Query_Internal, Status, userId, TrnType, (string.IsNullOrEmpty(CurrencyName) ? "0" : CurrencyName)).ToList();
                return Task.FromResult(Resp_Internal);
            }
            catch (Exception Ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, GetType().Name, Ex);
                throw Ex;
            }
        }

        public Task<List<UserWithdrawalDTO>> GetTotalDepositionHistory(long UserId, short TrnType, short Status, string CurrencyName)
        {
            try
            {
                string Query_Internal = @"SELECT SMSCode,SUM(Amount) AS 'TotalAmount',dbo.ConvertRateUSD(SMSCode,SUM(Amount)) AS 'EquivalentUSDAmt' FROM DepositHistory WHERE Status = {0} AND UserId = {1} AND (IsInternalTrn = {2} OR {2} = 0) AND (SMSCode = {3} OR {3} = '0' ) GROUP BY SMSCode,UserId";
                List<UserWithdrawalDTO> Resp_Internal = _dbContext.GetUserWithdrawalHistory.FromSql(Query_Internal, Status, UserId, TrnType, (string.IsNullOrEmpty(CurrencyName) ? "0" : CurrencyName)).ToList();
                return Task.FromResult(Resp_Internal);
            }
            catch (Exception Ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, GetType().Name, Ex);
                throw Ex;
            }
        }

        public Task<List<UserWalletBalDTO>> GetAllWalletBalance(long UserId, bool IsMainWallet, string CurrencyName)
        {
            try
            {
                if (IsMainWallet == true)
                {
                    string Query = @"SELECT WM.Balance AS 'MainBalance',WM.OutBoundBalance,WTM.WalletTypeName,dbo.ConvertRateUSD(WTM.WalletTypeName,WM.Balance) AS 'EquivalentUSDAmt' 
                                    FROM WalletMasters WM INNER JOIN WalletTypeMasters WTM ON WTM.Id = WM.WalletTypeID 
                                    WHERE WM.Status = 1 AND WM.UserID = {0} AND (WTM.WalletTypeName = {1} OR {1} = '0')";
                    List<UserWalletBalDTO> Response = _dbContext.GetAllUserWalletBalance.FromSql(Query, UserId, (string.IsNullOrEmpty(CurrencyName) ? "0" : CurrencyName)).ToList();
                    return Task.FromResult(Response);
                }
                else//ArbitrageWallet
                {
                    string Query = @"SELECT WM.Balance AS 'MainBalance',WM.OutBoundBalance,WTM.WalletTypeName,dbo.ConvertRateUSD(WTM.WalletTypeName,WM.Balance) AS 'EquivalentUSDAmt' 
                                    FROM ArbitrageWalletMaster WM INNER JOIN ArbitrageWalletTypeMaster WTM ON WTM.Id = WM.WalletTypeID 
                                    WHERE WM.Status = 1 AND WM.UserID = {0} AND (WTM.WalletTypeName = {1} OR {1} = '0')";
                    List<UserWalletBalDTO> Response = _dbContext.GetAllUserWalletBalance.FromSql(Query, UserId, (string.IsNullOrEmpty(CurrencyName) ? "0" : CurrencyName)).ToList();
                    return Task.FromResult(Response);
                }
            }
            catch (Exception Ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, GetType().Name, Ex);
                throw Ex;
            }
        }

        public Task<List<UserTradingSummaryDTO>> GetUserTradingSummary(long UserId, bool IsBuyTrn)
        {
            if (IsBuyTrn == true)//Buy Trade
            {
                string Query = @"SELECT PairName,Order_Currency,Delivery_Currency,SUM(SettledSellQty) AS 'TotalOrderQty',
                                dbo.ConvertRateUSD(Order_Currency,SUM(SettledSellQty)) AS 'TotalOrderQtyUSD',
                                SUM(settledBuyQty) AS 'TotalDeliveryQty',
                                dbo.ConvertRateUSD(Delivery_Currency,SUM(settledBuyQty)) AS 'TotalDeliveryQtyUSD' 
                                FROM TradeTransactionQueue WHERE status=1 AND TrnType={0} AND MemberId={1} 
                                GROUP BY Order_Currency,Delivery_Currency,PairName";
                List<UserTradingSummaryDTO> Response = _dbContext.GetUserTradingSummary.FromSql(Query, 4, UserId).ToList();
                return Task.FromResult(Response);
            }
            else//Sell Trade
            {
                string Query = @"SELECT PairName,Order_Currency,Delivery_Currency,SUM(SettledSellQty) AS 'TotalOrderQty',
                                dbo.ConvertRateUSD(Order_Currency,SUM(SettledSellQty)) AS 'TotalOrderQtyUSD',
                                SUM(settledBuyQty) AS 'TotalDeliveryQty',
                                dbo.ConvertRateUSD(Delivery_Currency,SUM(settledBuyQty)) AS 'TotalDeliveryQtyUSD' 
                                FROM TradeTransactionQueue WHERE status=1 AND TrnType={0} AND MemberId={1} 
                                GROUP BY Order_Currency,Delivery_Currency,PairName";
                List<UserTradingSummaryDTO> Response = _dbContext.GetUserTradingSummary.FromSql(Query, 5, UserId).ToList();
                return Task.FromResult(Response);
            }
        }

        #endregion
    }
}
