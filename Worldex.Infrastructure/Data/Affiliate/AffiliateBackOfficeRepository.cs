using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.AccountViewModels.Affiliate;
using Worldex.Core.ViewModels.BackOfficeAffiliate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Worldex.Infrastructure.Data.Affiliate
{
    public class AffiliateBackOfficeRepository : IAffiliateBackOfficeRepository
    {
        private readonly WorldexContext _dbContext;

        public AffiliateBackOfficeRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AffiliateDashboardCount GetAffiliateDashboardCount()
        {
            try
            {
                IQueryable<AffiliateDashboardCount> Result;

                Result = _dbContext.AffiliateDashboardCount.FromSql(
                            @"Select Cast((Select Count(Id) From AffiliateUserMaster) As bigint) As UserCount,
                            Cast((Select Count(Id) From AffiliateUserMaster Where DAY(CreatedDate) = DAY(dbo.GetISTDate()) and Month(CreatedDate) = Month(dbo.GetISTDate()) and Year(CreatedDate) = Year(dbo.GetISTDate())) As bigint) As UserTodayCount, 
                            Cast((Select Count(Id) From AffiliateUserMaster) As bigint) As AffiliateCount,
                            Cast((Select Count(Id) From AffiliateUserMaster Where DAY(CreatedDate) = DAY(dbo.GetISTDate()) and Month(CreatedDate) = Month(dbo.GetISTDate()) and Year(CreatedDate) = Year(dbo.GetISTDate())) As bigint) As AffiliateTodayUserCount,
                            Cast((Select Count(Id) From AffiliateCommissionHistory) As bigint) As CommissionCount,
                            Cast((Select Count(Id) From AffiliateCommissionHistory Where DAY(CreatedDate) = DAY(dbo.GetISTDate()) and Month(CreatedDate) = Month(dbo.GetISTDate()) and Year(CreatedDate) = Year(dbo.GetISTDate())) As bigint) As CommissionTodayCount,
                            Cast((Select Count(Id) From AffiliateLinkClick Where PromotionTypeId = 9 ) As bigint) As ReferralLinkCount,
                            Cast((Select Count(Id) From AffiliateLinkClick Where PromotionTypeId = 9 And DAY(CreatedDate) = DAY(dbo.GetISTDate()) and Month(CreatedDate) = Month(dbo.GetISTDate()) and Year(CreatedDate) = Year(dbo.GetISTDate())) As bigint) ReferralLinkTodayCount,
                            Cast((Select Count(Id) From AffiliateLinkClick Where PromotionTypeId = 6 ) As bigint) As FacebookLinkCount,
                            Cast((Select Count(Id) From AffiliateLinkClick Where PromotionTypeId = 6 And DAY(CreatedDate) = DAY(dbo.GetISTDate()) and Month(CreatedDate) = Month(dbo.GetISTDate()) and Year(CreatedDate) = Year(dbo.GetISTDate())) As bigint) FacebookLinkTodayCount,
                            Cast((Select Count(Id) From AffiliateLinkClick Where PromotionTypeId = 7 ) As bigint) As GooglePlusLinkCount,
                            Cast((Select Count(Id) From AffiliateLinkClick Where PromotionTypeId = 7 And DAY(CreatedDate) = DAY(dbo.GetISTDate()) and Month(CreatedDate) = Month(dbo.GetISTDate()) and Year(CreatedDate) = Year(dbo.GetISTDate())) As bigint) GooglePlusTodayCount,
                            Cast((Select Count(Id) From AffiliateLinkClick Where PromotionTypeId = 8 ) As bigint) As TwitterLinkCount,
                            Cast((Select Count(Id) From AffiliateLinkClick Where PromotionTypeId = 8 And DAY(CreatedDate) = DAY(dbo.GetISTDate()) and Month(CreatedDate) = Month(dbo.GetISTDate()) and Year(CreatedDate) = Year(dbo.GetISTDate())) As bigint) TwitterLinkTodayCount,
                            Cast((Select Count(Id) From AffiliatePromotionShare Where PromotionTypeId = 1) As bigint) As EmailSentCount,
                            Cast((Select Count(Id) From AffiliatePromotionShare Where PromotionTypeId = 1 And DAY(CreatedDate) = DAY(dbo.GetISTDate()) and Month(CreatedDate) = Month(dbo.GetISTDate()) and Year(CreatedDate) = Year(dbo.GetISTDate())) As bigint) As EmailSentTodayCount,
                            Cast((Select Count(Id) From AffiliatePromotionShare Where PromotionTypeId = 2) As bigint) As SMSSentCount,
                            Cast((Select Count(Id) From AffiliatePromotionShare Where PromotionTypeId = 2 And DAY(CreatedDate) = DAY(dbo.GetISTDate()) and Month(CreatedDate) = Month(dbo.GetISTDate()) and Year(CreatedDate) = Year(dbo.GetISTDate())) As bigint) As SMSSentTodayCount");

                return Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetAffiateUserRegisteredData> GetAffiateUserRegistered(string FromDate, string ToDate, int Status, int SchemeType, long ParentId, string SCondition, int PageSize, int PageNo, ref long TotalCount, ref int PageSize1, ref long TotalPages)
        {
            try
            {
                List<GetAffiateUserRegisteredData> list = new List<GetAffiateUserRegisteredData>();
                DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
                string Query = "";

                if (!string.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }

                Query = "Select AUM.UserId As UserId,AUM.Id As AffiliateId,BU.UserName,BU.Email,BU.Mobile," +
                        " BU.FirstName,BU.LastName,AUM.CreatedDate,AUM.Status,Case AUM.Status When 1 Then 'Confirm' When 0 Then 'Not Confirm' End As StatusMsg," +
                        " AUM.ParentId,IsNull(BU1.UserName,'') As ParentUserName,IsNull(BU1.Email,'') As ParentEmail,IsNull(BU1.Mobile,'') As ParentMobile," +
                        " ASM.SchemeName As SchemeType,AUM.CreatedDate As JoinDate from AffiliateUserMaster AUM" +
                        " Inner Join BizUser BU On BU.Id = AUM.UserId" +
                        " Inner Join AffiliateSchemeMaster ASM ON ASM.Id = AUM.SchemeMstId" +
                        " left outer Join BizUser BU1 On BU1.Id = AUM.ParentId" +
                        " Where(AUM.Status = {0} Or 999 = {0}) And(AUM.SchemeMstId = {1} Or 999 = {1}) And (AUM.ParentId = {2} Or 999 = {2})";

                Query = Query + SCondition;

                list = _dbContext.GetAffiateUserRegisteredData.FromSql(Query, Status, SchemeType, ParentId, fDate, tDate).ToList();

                var items = list;
                TotalCount = items.Count;
                var pagesize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                var it = Convert.ToDouble(items.Count) / pagesize;
                var fl = Math.Ceiling(it);
                var t1 = Convert.ToDouble(TotalCount) / pagesize;
                TotalPages = Convert.ToInt64(Math.Ceiling(t1));

                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    if (PageSize == 0)
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

                PageSize1 = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetReferralLinkClickData> GetReferralLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageSize, int PageNo, ref long TotalCount, ref int PageSize1, ref long TotalPages)
        {
            try
            {
                List<GetReferralLinkClickData> list = new List<GetReferralLinkClickData>();
                DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
                string Query = "";

                if (!string.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }

                //Query = "Select ALC.Id,AUM.UserId,AUM.Id As AffiliateId,ALC.IpAddress,ALC.CreatedDate As ClickTime," +
                //        " BU.UserName,BU.Email As UserEmail,BU.FirstName,BU.LastName From AffiliateLinkClick ALC" +
                //        " Inner Join AffiliateUserMaster AUM On AUM.Id = ALC.AffiliateUserId" +
                //        " Inner Join BizUser BU On BU.Id = AUM.UserId" +
                //        " Where ALC.PromotionTypeId = 9 And(ALC.AffiliateUserId = {0} Or 999 = {0})";

                Query = "Select ALC.Id,AUM.UserId,AUM.Id As AffiliateId,ALC.IpAddress,ALC.CreatedDate As ClickTime," +
                       " BU.UserName,BU.Email As UserEmail,BU.FirstName,BU.LastName From AffiliateLinkClick ALC" +
                       " Inner Join AffiliateUserMaster AUM On AUM.Id = ALC.AffiliateUserId" +
                       " Inner Join BizUser BU On BU.Id = AUM.UserId" +
                       " Where ALC.PromotionTypeId = 9 And(AUM.UserId = {0} Or 999 = {0})"; //changes -mansi 25-07-2019 Chnage column ALC.AffiliateUserId to AUM.UserId. 

                Query = Query + SCondition;

                list = _dbContext.GetReferralLinkClickData.FromSql(Query, UserId, fDate, tDate).ToList();

                var items = list;
                TotalCount = items.Count;
                var pagesize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                var it = Convert.ToDouble(items.Count) / pagesize;
                var fl = Math.Ceiling(it);
                var t1 = Convert.ToDouble(TotalCount) / pagesize;
                TotalPages = Convert.ToInt64(Math.Ceiling(t1));

                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    if (PageSize == 0)
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

                PageSize1 = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetFacebookLinkClickData> GetFacebookLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageSize, int PageNo, ref long TotalCount, ref int PageSize1, ref long TotalPages)
        {
            try
            {
                List<GetFacebookLinkClickData> list = new List<GetFacebookLinkClickData>();
                DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
                string Query = "";

                if (!string.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }

                //Query = "Select ALC.Id,AUM.UserId,AUM.Id As AffiliateId,ALC.IpAddress,ALC.CreatedDate As ClickTime," +
                //        " BU.UserName,BU.Email As UserEmail,BU.FirstName,BU.LastName From AffiliateLinkClick ALC" +
                //        " Inner Join AffiliateUserMaster AUM On AUM.Id = ALC.AffiliateUserId" +
                //        " Inner Join BizUser BU On BU.Id = AUM.UserId" +
                //        " Where ALC.PromotionTypeId = 6 And(ALC.AffiliateUserId = {0} Or 999 = {0})";
                Query = "Select ALC.Id,AUM.UserId,AUM.Id As AffiliateId,ALC.IpAddress,ALC.CreatedDate As ClickTime," +
                        " BU.UserName,BU.Email As UserEmail,BU.FirstName,BU.LastName From AffiliateLinkClick ALC" +
                        " Inner Join AffiliateUserMaster AUM On AUM.Id = ALC.AffiliateUserId" +
                        " Inner Join BizUser BU On BU.Id = AUM.UserId" +
                        " Where ALC.PromotionTypeId = 6 And(AUM.UserId = {0} Or 999 = {0})"; //changes -mansi 25-07-2019 Chnage column ALC.AffiliateUserId to AUM.UserId.

                Query = Query + SCondition;

                list = _dbContext.GetFacebookLinkClickData.FromSql(Query, UserId, fDate, tDate).ToList();

                var items = list;
                TotalCount = items.Count;
                var pagesize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                var it = Convert.ToDouble(items.Count) / pagesize;
                var fl = Math.Ceiling(it);
                var t1 = Convert.ToDouble(TotalCount) / pagesize;
                TotalPages = Convert.ToInt64(Math.Ceiling(t1));

                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    if (PageSize == 0)
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

                PageSize1 = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetTwitterLinkClickData> GetTwitterLinkClick(string FromDate, string ToDate, long UserId, string SCondition, int PageSize, int PageNo, ref long TotalCount, ref int PageSize1, ref long TotalPages)
        {
            try
            {
                List<GetTwitterLinkClickData> list = new List<GetTwitterLinkClickData>();
                DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
                string Query = "";

                if (!string.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }

                //Query = "Select ALC.Id,AUM.UserId,AUM.Id As AffiliateId,ALC.IpAddress,ALC.CreatedDate As ClickTime," +
                //       " BU.UserName,BU.Email As UserEmail,BU.FirstName,BU.LastName From AffiliateLinkClick ALC" +
                //       " Inner Join AffiliateUserMaster AUM On AUM.Id = ALC.AffiliateUserId" +
                //       " Inner Join BizUser BU On BU.Id = AUM.UserId" +
                //       " Where ALC.PromotionTypeId = 8 And(ALC.AffiliateUserId = {0} Or 999 = {0})";

                Query = "Select ALC.Id,AUM.UserId,AUM.Id As AffiliateId,ALC.IpAddress,ALC.CreatedDate As ClickTime," +
                       " BU.UserName,BU.Email As UserEmail,BU.FirstName,BU.LastName From AffiliateLinkClick ALC" +
                       " Inner Join AffiliateUserMaster AUM On AUM.Id = ALC.AffiliateUserId" +
                       " Inner Join BizUser BU On BU.Id = AUM.UserId" +
                       " Where ALC.PromotionTypeId = 8 And(AUM.UserId = {0} Or 999 = {0})"; //changes -mansi 25-07-2019 Chnage column ALC.AffiliateUserId to AUM.UserId. 

                Query = Query + SCondition;

                list = _dbContext.GetTwitterLinkClickData.FromSql(Query, UserId, fDate, tDate).ToList();

                var items = list;
                TotalCount = items.Count;
                var pagesize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                var it = Convert.ToDouble(items.Count) / pagesize;
                var fl = Math.Ceiling(it);
                var t1 = Convert.ToDouble(TotalCount) / pagesize;
                TotalPages = Convert.ToInt64(Math.Ceiling(t1));

                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    if (PageSize == 0)
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

                PageSize1 = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetEmailSentData> GetEmailSent(string FromDate, string ToDate, long UserId, string SCondition, int PageSize, int PageNo, ref long TotalCount, ref int PageSize1, ref long TotalPages)
        {
            try
            {
                List<GetEmailSentData> list = new List<GetEmailSentData>();
                DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
                string Query = "";

                if (!string.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }

                //Query = "Select APS.Id,AUM.UserId,AUM.Id As AffiliateId,APS.PromotionDetail As Email,APS.CreatedDate As SentTime," +
                //                " BU.UserName,BU.Email As UserEmail,BU.FirstName,BU.LastName From AffiliatePromotionShare APS" +
                //                " Inner Join AffiliateUserMaster AUM On AUM.Id = APS.AffiliateUserId" +
                //                " Inner Join BizUser BU On BU.Id = AUM.UserId" +
                //                " Where APS.PromotionTypeId = 1 And (APS.AffiliateUserId = {0} Or 999 = {0})";
                Query = "Select APS.Id,AUM.UserId,AUM.Id As AffiliateId,APS.PromotionDetail As Email,APS.CreatedDate As SentTime," +
                               " BU.UserName,BU.Email As UserEmail,BU.FirstName,BU.LastName From AffiliatePromotionShare APS" +
                               " Inner Join AffiliateUserMaster AUM On AUM.Id = APS.AffiliateUserId" +
                               " Inner Join BizUser BU On BU.Id = AUM.UserId" +
                               " Where APS.PromotionTypeId = 1 And (AUM.UserId = {0} Or 999 = {0})"; //changes -mansi 25-07-2019 Chnage column ALC.AffiliateUserId to AUM.UserId. 

                Query = Query + SCondition;

                list = _dbContext.GetEmailSentData.FromSql(Query, UserId, fDate, tDate).ToList();

                var items = list;
                TotalCount = items.Count;
                var pagesize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                var it = Convert.ToDouble(items.Count) / pagesize;
                var fl = Math.Ceiling(it);
                var t1 = Convert.ToDouble(TotalCount) / pagesize;
                TotalPages = Convert.ToInt64(Math.Ceiling(t1));

                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    if (PageSize == 0)
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

                PageSize1 = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetSMSSentData> GetSMSSent(string FromDate, string ToDate, long UserId, string SCondition, int PageSize, int PageNo, ref long TotalCount, ref int PageSize1, ref long TotalPages)
        {
            try
            {
                List<GetSMSSentData> list = new List<GetSMSSentData>();
                DateTime fDate = DateTime.UtcNow, tDate = DateTime.UtcNow;
                string Query = "";

                if (!string.IsNullOrEmpty(FromDate))
                {
                    fDate = DateTime.ParseExact(FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ToDate = ToDate + " 23:59:59";
                    tDate = DateTime.ParseExact(ToDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }

                //Query = "Select APS.Id,AUM.UserId,AUM.Id As AffiliateId,APS.PromotionDetail As Mobile,APS.CreatedDate As SentTime," +
                //          " BU.UserName,BU.Email As UserEmail,BU.FirstName,BU.LastName From AffiliatePromotionShare APS" +
                //          " Inner Join AffiliateUserMaster AUM On AUM.Id = APS.AffiliateUserId" +
                //          " Inner Join BizUser BU On BU.Id = AUM.UserId" +
                //          " Where APS.PromotionTypeId = 2 And (APS.AffiliateUserId = {0} Or 999 = {0})";


                Query = "Select APS.Id,AUM.UserId,AUM.Id As AffiliateId,APS.PromotionDetail As Mobile,APS.CreatedDate As SentTime," +
                         " BU.UserName,BU.Email As UserEmail,BU.FirstName,BU.LastName From AffiliatePromotionShare APS" +
                         " Inner Join AffiliateUserMaster AUM On AUM.Id = APS.AffiliateUserId" +
                         " Inner Join BizUser BU On BU.Id = AUM.UserId" +
                         " Where APS.PromotionTypeId = 2 And (AUM.UserId = {0} Or 999 = {0})"; //changes -mansi 25-07-2019 Chnage column ALC.AffiliateUserId to AUM.UserId. 

                Query = Query + SCondition;

                list = _dbContext.GetSMSSentData.FromSql(Query, UserId, fDate, tDate).ToList();

                var items = list;
                TotalCount = items.Count;
                var pagesize = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);
                var it = Convert.ToDouble(items.Count) / pagesize;
                var fl = Math.Ceiling(it);
                var t1 = Convert.ToDouble(TotalCount) / pagesize;
                TotalPages = Convert.ToInt64(Math.Ceiling(t1));

                PageNo = PageNo + 1;
                if (PageNo > 0)
                {
                    if (PageSize == 0)
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

                PageSize1 = (PageSize == 0) ? Helpers.PageSize : Convert.ToInt32(PageSize);

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetAllAffiliateUserData> GetAllAffiliateUser()
        {
            try
            {
                List<GetAllAffiliateUserData> list = new List<GetAllAffiliateUserData>();

                list = _dbContext.GetAllAffiliateUserData.FromSql(@"Select AUM.UserId AS Id,BU.UserName,BU.Email,BU.Mobile,BU.FirstName,BU.LastName,BU.CreatedDate As JoinDate 
                                                           From AffiliateUserMaster AUM Inner Join BizUser BU On BU.Id = AUM.UserId").ToList();

                return list;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetAllAffiliateSchemeMasterRes> GetAffiliateSchemeMasterdata()
        {
            try
            {
                var data = (from i in _dbContext.AffiliateSchemeMaster
                            orderby i.Id descending
                            select new GetAllAffiliateSchemeMasterRes
                            {
                                SchemeMasterId = i.Id,
                                SchemeName = i.SchemeName,
                                SchemeType = i.SchemeType,
                                Status = i.Status
                            }).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetAllAffiliateSchemeTypeMasterRes> GetAffiliateSchemeTypeMasterdata()
        {
            try
            {
                var data = (from i in _dbContext.AffiliateSchemeTypeMaster
                            orderby i.Id descending
                            select new GetAllAffiliateSchemeTypeMasterRes
                            {
                                SchemeTypeId = i.Id,
                                SchemeTypeName = i.SchemeTypeName,
                                Description = i.Description,
                                Status = i.Status
                            }).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<AffiliateSchemeTypeMappingListViewModel> ListSchemeTypeMappingData()
        {
            try
            {
                var items = _dbContext.AffiliateSchemeTypeMappingListViewModel.FromSql("select 0 as UserId,'' as UserName,st.SchemeName as SchemeName, rs.Id as MappingId,rs.SchemeMstId as SchemeMasterId,rs.SchemeTypeMstId as SchemeTypeMasterId, cht.SchemeTypeName as SchemeTypeName,rs.MinimumDepositionRequired as MinimumDepositionRequired,rs.DepositWalletTypeId as DepositWalletTypeId,rs.CommissionTypeInterval as CommissionTypeInterval,rs.Description as Description,rs.CommissionHour as CommissionHour, wt.WalletTypeName as DepositWalletTypeName,cast(rs.Status as INT) as Status from AffiliateSchemeTypeMapping rs inner join AffiliateSchemeMaster st on st.id=rs.SchemeMstId inner join AffiliateSchemeTypeMaster cht on cht.id=rs.SchemeTypeMstId inner join WalletTypeMasters wt on wt.id= rs.DepositWalletTypeId").ToList();
                //var items = (from rs in _dbContext.AffiliateSchemeTypeMapping
                //             join st in _dbContext.AffiliateSchemeMaster on rs.SchemeMstId equals st.Id
                //             join wt in _dbContext.WalletTypeMasters on rs.DepositWalletTypeId equals wt.Id
                //             join cht in _dbContext.AffiliateSchemeTypeMaster on rs.SchemeTypeMstId equals cht.Id
                //             orderby rs.Id descending
                //             select new AffiliateSchemeTypeMappingListViewModel
                //             {
                //                 MappingId = rs.Id,
                //                 SchemeMasterId = rs.SchemeMstId,
                //                 SchemeName = st.SchemeName,
                //                 SchemeTypeMasterId = rs.SchemeTypeMstId,
                //                 SchemeTypeName = cht.SchemeTypeName,
                //                 MinimumDepositionRequired = rs.MinimumDepositionRequired,
                //                 DepositWalletTypeId = rs.DepositWalletTypeId,
                //                 CommissionTypeInterval = rs.CommissionTypeInterval,
                //                 Description = rs.Description,
                //                 CommissionHour = rs.CommissionHour,
                //                 DepositWalletTypeName = wt.WalletTypeName,
                //                 //CreatedDate = rs.CreatedDate,
                //                 Status = rs.Status
                //             }
                //            ).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public AffiliateSchemeTypeMappingListViewModel GetSchemeTypeMappingData(long id)
        {
            try
            {
                var items = (from rs in _dbContext.AffiliateSchemeTypeMapping
                             join st in _dbContext.AffiliateSchemeMaster on rs.SchemeMstId equals st.Id
                             join cht in _dbContext.AffiliateSchemeTypeMaster on rs.SchemeTypeMstId equals cht.Id
                             join wt in _dbContext.WalletTypeMasters on rs.DepositWalletTypeId equals wt.Id
                             where rs.Id == id
                             select new AffiliateSchemeTypeMappingListViewModel
                             {
                                 MappingId = rs.Id,
                                 SchemeMasterId = rs.SchemeMstId,
                                 SchemeName = st.SchemeName,
                                 SchemeTypeMasterId = rs.SchemeTypeMstId,
                                 SchemeTypeName = cht.SchemeTypeName,
                                 MinimumDepositionRequired = rs.MinimumDepositionRequired,
                                 DepositWalletTypeId = rs.DepositWalletTypeId,
                                 CommissionTypeInterval = rs.CommissionTypeInterval,
                                 Description = rs.Description,
                                 CommissionHour = rs.CommissionHour,
                                 DepositWalletTypeName = wt.WalletTypeName,
                                 Status = rs.Status
                             }
                            ).FirstOrDefault();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetAllAffiliatePromotionMasterRes> ListAffiliatePromotion()
        {
            try
            {
                var data = (from i in _dbContext.AffiliatePromotionMaster
                            orderby i.Id descending
                            select new GetAllAffiliatePromotionMasterRes
                            {
                                PromotionId = i.Id,
                                PromotionType = i.PromotionType,
                                Status = i.Status
                            }).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<GetAffiliateShemeDetailRes> ListAffiliateSchemeDetail()
        {
            try
            {
                var data = (from d in _dbContext.AffiliateSchemeDetail
                            join wt in _dbContext.WalletTypeMasters on d.CreditWalletTypeId equals wt.Id
                            join wtm in _dbContext.WalletTypeMasters on d.TrnWalletTypeId equals wtm.Id
                            join tm in _dbContext.AffiliateSchemeTypeMapping on d.SchemeMappingId equals tm.Id
                            orderby d.Id descending
                            select new GetAffiliateShemeDetailRes
                            {
                                DetailId = d.Id,
                                SchemeMasterId = tm.SchemeMstId,
                                SchemeMappingId = d.SchemeMappingId,
                                SchemeMappingName = tm.Description,
                                Level = d.Level,
                                MinimumValue = d.MinimumValue,
                                MaximumValue = d.MaximumValue,
                                CreditWalletTypeId = d.CreditWalletTypeId,
                                CreditWalletTypeName = wt.WalletTypeName,
                                CommissionType = d.CommissionType,
                                CommissionTypeName = (d.CommissionType == 1) ? "Fix" : "Percentage",
                                CommissionValue = d.CommissionValue,
                                DistributionType = d.DistributionType,
                                TrnWalletTypeId = d.TrnWalletTypeId,
                                TrnWalletTypeName = wtm.WalletTypeName,
                                Status = d.Status,
                                DistributionTypeName = (d.DistributionType == 1 ? "DependOnTranxAmount" : d.DistributionType == 2 ? "CommissionBasedOnPreviousLevel" : d.DistributionType == 3 ? "CalculatedForEachTranx" : "-")
                            }
                            ).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetAffiliateShemeDetailRes GetAffiliateSchemeDetail(long Id)
        {
            try
            {
                var data = (from d in _dbContext.AffiliateSchemeDetail
                            join wt in _dbContext.WalletTypeMasters on d.CreditWalletTypeId equals wt.Id
                            join wtm in _dbContext.WalletTypeMasters on d.TrnWalletTypeId equals wtm.Id
                            join tm in _dbContext.AffiliateSchemeTypeMapping on d.SchemeMappingId equals tm.Id
                            where d.Id == Id
                            select new GetAffiliateShemeDetailRes
                            {
                                DetailId = d.Id,
                                SchemeMasterId = tm.SchemeMstId,
                                SchemeMappingId = d.SchemeMappingId,
                                SchemeMappingName = tm.Description,
                                Level = d.Level,
                                MinimumValue = d.MinimumValue,
                                MaximumValue = d.MaximumValue,
                                CreditWalletTypeId = d.CreditWalletTypeId,
                                CreditWalletTypeName = wt.WalletTypeName,
                                CommissionType = d.CommissionType,
                                CommissionTypeName = (d.CommissionType == 1) ? "Fix" : "Percentage",
                                CommissionValue = d.CommissionValue,
                                DistributionType = d.DistributionType,
                                TrnWalletTypeId = d.TrnWalletTypeId,
                                TrnWalletTypeName = wtm.WalletTypeName,
                                Status = d.Status,
                                DistributionTypeName = (d.DistributionType == 1 ? "DependOnTranxAmount" : d.DistributionType == 2 ? "CommissionBasedOnPreviousLevel" : d.DistributionType == 3 ? "CalculatedForEachTranx" : "-")
                            }
                            ).FirstOrDefault();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int GetRange(decimal MaxRange, decimal MinRange, long SchemeMappingId)
        {
            try
            {
                var dataCount1 = _dbContext.Counts.FromSql("SELECT count(*) as Count FROM AffiliateSchemeDetail WHERE status=1 and SchemeMappingId = {0} and {1}<=MaximumValue and {1}>=MinimumValue", SchemeMappingId, MinRange).FirstOrDefault();
                var dataCount2 = _dbContext.Counts.FromSql("SELECT count(*) as Count FROM AffiliateSchemeDetail WHERE status = 1 and SchemeMappingId = {0} and {1}<= MaximumValue and {1}>= MinimumValue", SchemeMappingId, MaxRange).FirstOrDefault();
                var dataCount3 = _dbContext.Counts.FromSql("SELECT count(*)as Count FROM AffiliateSchemeDetail WHERE status=1 and SchemeMappingId = {0} and {1}<=MaximumValue and {2}>=MinimumValue", SchemeMappingId, MaxRange, MinRange).FirstOrDefault();
                if (dataCount1.Count == 0 && dataCount2.Count == 0 && dataCount3.Count == 0)
                {
                    return 1;//for insert into db
                }
                return 0;


            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public int GetDetailId(long DetailID, long SchemeMappingId, decimal MaxRange, decimal MinRange)// add For UpdateSchemaDetail method -mansi
        {
            try
            {
               // var dataCount1 = _dbContext.Counts.FromSql("Select Count(*) as Count FROM AffiliateSchemeDetail WHERE status=1 and SchemeMappingId = {0} and Id ={1}", SchemeMappingId, DetailID).FirstOrDefault();
                var dataCount1 = _dbContext.Counts.FromSql("SELECT count(*) as Count FROM AffiliateSchemeDetail WHERE status=1 and SchemeMappingId = {1} and id <> {0} and {2}<=MaximumValue and {2}>=MinimumValue", DetailID, SchemeMappingId, MinRange).FirstOrDefault();
                var dataCount2 = _dbContext.Counts.FromSql("SELECT count(*) as Count FROM AffiliateSchemeDetail WHERE status = 1 and SchemeMappingId = {1}  and id <> {0} and {2}<=MaximumValue and {2}>=MinimumValue", DetailID, SchemeMappingId, MaxRange).FirstOrDefault();
                var dataCount3 = _dbContext.Counts.FromSql("SELECT count(*)as Count FROM AffiliateSchemeDetail WHERE status=1 and SchemeMappingId = {1}  and id <> {0} and {2}<=MaximumValue and {3}>=MinimumValue", DetailID, SchemeMappingId, MaxRange, MinRange).FirstOrDefault();
                if (dataCount1.Count == 0 && dataCount2.Count == 0 && dataCount3.Count == 0)
                {
                    return 1;//for insert into db
                }
                return 0;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        public List<AffiliateCommissionHistoryReport> AffiliateCommissionHistoryReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, long? TrnUserId, long? AffiliateUserId, long? SchemeMappingId, long? TrnRefNo, ref int TotalCount)
        {
            try
            {
                string sqlQuery = "select ACH.Status,CASE ACH.Status WHEN 1 THEN 'Success' ELSE '' END AS StrStatus,ACH.TrnRefNo,ACH.CronRefNo,ACH.FromWalletId,WM.Walletname AS FromWalletName,WM.AccWalletId AS FromAccWalletId,ACH.Amount,ACH.ToWalletId,(SELECT Walletname FROM WalletMasters WHERE id=ACH.ToWalletId) AS ToWalletName,(SELECT AccWalletId FROM WalletMasters WHERE id=ACH.ToWalletId) AS ToAccWalletId,AffiliateUserId,BU.UserName AS AffiliateUserName,BU.Email AS AffiliateEmail,SchemeMappingId,AST.Description AS SchemeMappingName,TrnUserId,(select username from bizuser where id=TrnUserId) AS TrnUserName,(select email from bizuser where id=TrnUserId) AS TrnEmail,ACH.Remarks,ACH.TrnWalletTypeId,WT.WalletTypeName AS TrnWalletTypeName,ACH.CommissionPer,ACH.Level,ACH.TransactionAmount,ACH.TrnDate FROM AffiliateCommissionHistory ACH INNER JOIN WalletTypeMasters WT ON WT.Id = ACH.TrnWalletTypeId INNER JOIN WalletMasters WM ON WM.Id=ACH.FromWalletId INNER JOIN BizUser BU ON BU.Id=ACH.AffiliateUserId INNER JOIN AffiliateSchemeTypeMapping AST ON AST.Id = ACH.SchemeMappingId WHERE (TrnUserId = {0} OR {0}=0) AND (AffiliateUserId={1} OR {1}=0) AND (SchemeMappingId = {2} OR {2}=0) AND (TrnRefNo={3} OR {3}=0)";

                if (FromDate != null && ToDate != null)
                {
                    ToDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(23).AddSeconds(59);
                    sqlQuery = sqlQuery + " AND ACH.CreatedDate Between {4} AND {5}";
                }
                var data = _dbContext.AffiliateCommissionHistoryReport.FromSql(sqlQuery, (TrnUserId == null ? 0 : TrnUserId), (AffiliateUserId == null ? 0 : AffiliateUserId), (SchemeMappingId == null ? 0 : SchemeMappingId), (TrnRefNo == null ? 0 : TrnRefNo), FromDate, ToDate).ToList();
                TotalCount = data.Count();
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32(PageSize) * (PageNo - 1);
                    data = data.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #region  Graph API

        public InviteFrdClaas GetAffiliateInviteFrieds()
        {
            try
            {
                List<InviteFrdClaas> dataObj = new List<InviteFrdClaas>();

                string sqlQuery = "select cast((SELECT COUNT(s.Id) FROM AffiliatePromotionMaster m INNER JOIN AffiliateLinkClick s  ON m.Id = s.PromotionTypeId WHERE m.Id=9) AS decimal(28,18))AS AffiliateLink,cast((SELECT COUNT(s.Id)  FROM AffiliatePromotionMaster m INNER JOIN AffiliateLinkClick s  ON m.Id = s.PromotionTypeId WHERE m.Id=6)AS decimal(28,18)) AS Facebook,cast((SELECT COUNT(s.Id)  FROM AffiliatePromotionMaster m INNER JOIN AffiliateLinkClick s  ON m.Id = s.PromotionTypeId WHERE m.Id=8)AS decimal(28,18)) AS Twitter,cast((SELECT COUNT(s.Id)  FROM AffiliatePromotionMaster m INNER JOIN AffiliatePromotionShare s  ON m.Id = s.PromotionTypeId WHERE m.Id=1)AS decimal(28,18)) AS Email,cast((SELECT COUNT(s.Id) FROM AffiliatePromotionMaster m INNER JOIN AffiliatePromotionShare s  ON m.Id = s.PromotionTypeId WHERE m.Id=2) AS decimal(28,18))AS SMS";

                InviteFrdClaas x = new InviteFrdClaas();

                x = _dbContext.InviteFrdClaas.FromSql(sqlQuery).FirstOrDefault();
                decimal sumofTotal = (x.Facebook + x.AffiliateLink + x.Email + x.Twitter + x.SMS);
                if (sumofTotal > 0)
                {
                    x.Facebook = Convert.ToDecimal((x.Facebook * 100) / sumofTotal);
                    x.AffiliateLink = Convert.ToDecimal((x.AffiliateLink * 100) / sumofTotal);
                    x.Email = Convert.ToDecimal((x.Email * 100) / sumofTotal);
                    x.Twitter = Convert.ToDecimal((x.Twitter * 100) / sumofTotal);
                    x.SMS = Convert.ToDecimal((x.SMS * 100) / sumofTotal);
                }
                return x;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetMonthWiseCommissionDataV1 GetMonthWiseCommissionChartDetail(int? Year)
        {
            try
            {
                if (Year == null)
                {
                    var date = Helpers.UTC_To_IST();
                    Year = date.Year;
                }

                string Query = " SELECT  T.Id AS SchemeTypeId,T.SchemeTypeName,CAST (cast( round(ISNULL(SUM(CASE MONTH(Ch.CreatedDate) WHEN 1 THEN (Amount) END),0),3) as decimal(28,3)) AS VARCHAR(50)) + ','+CAST ( cast( round(ISNULL(SUM(CASE MONTH(Ch.CreatedDate) WHEN 2 THEN (Amount) END),0),3) as decimal(28,3)) AS VARCHAR(50)) + ','+CAST ( cast( round(ISNULL(SUM(CASE MONTH(Ch.CreatedDate) WHEN 3 THEN (Amount) END),0),3) as decimal(28,3)) AS VARCHAR(50)) + ','+CAST ( cast( round(ISNULL(SUM(CASE MONTH(Ch.CreatedDate) WHEN 4 THEN (Amount) END),0),3) as decimal(28,3)) AS VARCHAR(50)) + ','+CAST ( cast( round(ISNULL(SUM(CASE MONTH(Ch.CreatedDate) WHEN 5 THEN (Amount) END),0),3) as decimal(28,3)) AS VARCHAR(50)) + ','+CAST ( cast( round(ISNULL(SUM(CASE MONTH(Ch.CreatedDate) WHEN 6 THEN (Amount) END),0),3) as decimal(28,3)) AS VARCHAR(50)) + ','+CAST ( cast( round(ISNULL(SUM(CASE MONTH(Ch.CreatedDate) WHEN 7 THEN (Amount) END),0),3) as decimal(28,3)) AS VARCHAR(50)) + ','+CAST ( cast( round(ISNULL(SUM(CASE MONTH(Ch.CreatedDate) WHEN 8 THEN (Amount) END),0),3) as decimal(28,3)) AS VARCHAR(50)) + ','+CAST ( cast( round(ISNULL(SUM(CASE MONTH(Ch.CreatedDate) WHEN 9 THEN (Amount) END),0),3) as decimal(28,3)) AS VARCHAR(50)) + ','+CAST ( cast( round(ISNULL(SUM(CASE MONTH(Ch.CreatedDate) WHEN 10 THEN (Amount)END),0),3) as decimal(28,3)) AS VARCHAR(50)) + ','+CAST ( cast( round(ISNULL(SUM(CASE MONTH(Ch.CreatedDate) WHEN 11 THEN (Amount)END),0),3) as decimal(28,3)) AS VARCHAR(50)) + ','+CAST ( cast( round(ISNULL(SUM(CASE MONTH(Ch.CreatedDate) WHEN 12 THEN (Amount)END),0),3) as decimal(28,3)) AS VARCHAR(50)) AS DataString FROM AffiliateCommissionHistory Ch INNER JOIN AffiliateSchemeTypeMapping M ON M.Id = Ch.SchemeMappingId INNER JOIN AffiliateSchemeTypeMaster T ON T.Id = M.SchemeTypeMstId WHERE YEAR(Ch.CreatedDate) = {0} AND Ch.Status=1 GROUP BY T.Id,T.SchemeTypeName";

                var NewObj = new GetMonthWiseCommissionDataV1();

                var data = _dbContext.GetMonthWiseCommissionData.FromSql(Query, Year).ToList();
                if (data.Count > 0)
                {
                    data.ForEach(x =>
                    {
                        if (x.DataString == null)
                        {
                            x.DataString = "0,0,0,0,0,0,0,0,0,0,0,0";
                        }
                        else { x.Data = Array.ConvertAll(x.DataString.Split(','), decimal.Parse); }
                    });
                    data.ForEach(c =>
                    {
                        if (c.SchemeTypeId == 1)
                        {
                            NewObj.Deposition = c.Data;
                        }
                        if (c.SchemeTypeId == 2)
                        {
                            NewObj.BuyTrading = c.Data;
                        }
                        if (c.SchemeTypeId == 3)
                        {
                            NewObj.SignUp = c.Data;
                        }
                        if (c.SchemeTypeId == 5)
                        {
                            NewObj.SellTrading = c.Data;
                        }
                    });

                }
                else
                {
                    decimal[] newOn = new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    NewObj.Deposition = newOn;
                    NewObj.BuyTrading = newOn;
                    NewObj.SignUp = newOn;
                    NewObj.SellTrading = newOn;
                }
                return NewObj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion
    }

}
