using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.Interfaces.Referral;
using Microsoft.AspNetCore.Identity;
using Worldex.Core.ViewModels.Referral;
using Worldex.Core.Entities.Referral;
using Worldex.Core.Interfaces.Repository;

namespace Worldex.Infrastructure.Services.Referral
{
    public class ReferralUserServices : IReferralUser
    {
        private readonly WorldexContext _dbContext;       
        private readonly ICustomRepository<ReferralUser> _ReferralUserRepository;
        private readonly ICustomRepository<ReferralUserLevelMapping> _ReferralUserLevelMapping;

        public ReferralUserServices(ICustomRepository<ReferralUser> ReferralUserRepository, ICustomRepository<ReferralUserLevelMapping> ReferralUserLevelMapping, WorldexContext dbContext)
        {
            _dbContext = dbContext;
            _ReferralUserLevelMapping = ReferralUserLevelMapping;
            _ReferralUserRepository = ReferralUserRepository;
        }

        public string GenerateRandomReferralCode(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 0,
                RequireDigit = true,
                RequireLowercase = false,
                RequireNonAlphanumeric = false,
                RequireUppercase = true,

            };
            string[] randomChars = new[] {
             "ABCDEFGHJKLMNPQRSTUVWXYZ",    // uppercase             
        "123456789"                   // digits         
    };
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);
            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }
            return new string(chars.ToArray());
        }

        public bool FindDuplicateReferralCode(string ReferralCode)
        {
            var userdata = _dbContext.Users.Any(i => i.ReferralCode == ReferralCode);
            return userdata;
        }

        public async Task<int> GetReferCodeUser(string ReferralCode)
        {
            var userdata =await Task.FromResult(_dbContext.Users.Where(i => i.ReferralCode == ReferralCode).FirstOrDefault());
            if (userdata != null)
            {
                return Convert.ToInt32(userdata.Id);
            }
            else
            {
                return 0;
            }
        }

        public long AddReferralUser(ReferralUserViewModel ReferralUserInsert)
        {
            try
            {
                ReferralUser ObjReferralUser = new ReferralUser()
                {
                    UserId = ReferralUserInsert.UserId,
                    ReferUserId = ReferralUserInsert.ReferUserId,
                    ReferralServiceId = ReferralUserInsert.ReferralServiceId,
                    ReferralChannelTypeId= ReferralUserInsert.ReferralChannelTypeId,
                    IsCommissionCredited= ReferralUserInsert.IsCommissionCredited,
                    CreatedBy = ReferralUserInsert.ReferUserId,
                    CreatedDate = DateTime.UtcNow,
                    Status = 1
                };

                ReferralUserLevelMapping ObjReferralUserLevelMapping = new ReferralUserLevelMapping()
                {
                    UserId = ReferralUserInsert.UserId,
                    ReferUserId = ReferralUserInsert.ReferUserId,
                    IsCommissionCredited = 0,
                    CreatedBy = ReferralUserInsert.ReferUserId,
                    CreatedDate = DateTime.UtcNow,
                    Level=1,
                    Status = 1
                };

                _ReferralUserLevelMapping.Insert(ObjReferralUserLevelMapping);
                _ReferralUserRepository.Insert(ObjReferralUser);

                var ObjLevel =(from l in _dbContext.ReferralUserLevelMapping
                               where l.ReferUserId== ReferralUserInsert.UserId
                               select new ReferralUserLevelMapping
                               {
                                   UserId = l.UserId,
                                   ReferUserId = ReferralUserInsert.ReferUserId,
                                   IsCommissionCredited = 0,
                                   CreatedBy = ReferralUserInsert.ReferUserId,
                                   CreatedDate = DateTime.UtcNow,
                                   Level = l.Level+1,
                                   Status = 1
                               }).ToList();
                if(ObjLevel.Count()!=0)
                {
                    _dbContext.AddRange(ObjLevel);
                    _dbContext.SaveChanges();
                }

                return ObjReferralUser.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReferralUserListResponse ListAdminParticipateReferralUser(int PageIndex = 0, int Page_Size = 0, long ReferralServiceId = 0, string UserName = null, string ReferUserName = null, DateTime? FromDate = null, DateTime? ToDate = null, int ReferralChannelTypeId=0)
        {
            try
            {
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }
                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }
                var skip = Page_Size * (PageIndex - 1);

                var items = (from rs in _dbContext.ReferralUser
                             join st in _dbContext.ReferralService on rs.ReferralServiceId equals st.Id
                             join us in _dbContext.Users on rs.UserId equals us.Id
                             join rus in _dbContext.Users on rs.ReferUserId equals rus.Id 
                             join cht in _dbContext.ReferralChannelType on rs.ReferralChannelTypeId equals cht.Id
                             select new ReferralUserListViewModel
                             {
                                 Id = rs.Id,
                                 UserId = rs.UserId,
                                 ReferUserId = rs.ReferUserId,                                
                                 UserName = us.UserName,
                                 ReferUserName = rus.UserName,
                                 UserReferralCode = us.ReferralCode,
                                 ReferUserReferralCode = rus.ReferralCode,
                                 ReferralChannelTypeId=rs.ReferralChannelTypeId,
                                 ReferralChanneTypeName=cht.ChannelTypeName,
                                 ReferralServiceId = rs.ReferralServiceId,
                                 ReferralServiceDescription = st.Description,
                                 IsCommissionCreditedName=rs.IsCommissionCredited == 0 ? "No": "Yes",
                                 CreatedDate = rs.CreatedDate
                             }
                            ).ToList();

                if (ReferralServiceId != 0)
                {
                    items = items.Where(x => x.ReferralServiceId == ReferralServiceId).ToList();
                }
                if (ReferralChannelTypeId != 0)
                {
                    items= items.Where(x => x.ReferralChannelTypeId == ReferralChannelTypeId).ToList();
                }
                if (!String.IsNullOrEmpty(UserName))
                {
                    items = items.Where(x => x.UserName == UserName).ToList();
                }

                if (!String.IsNullOrEmpty(ReferUserName))
                {
                    items = items.Where(x => x.ReferUserName == ReferUserName).ToList();
                }

                if (FromDate != null && ToDate != null)
                {
                    items = items.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }

                int TotalCount = items.Count();

                ReferralUserListResponse obj = new ReferralUserListResponse();
                obj.TotalCount = TotalCount;
                obj.ReferralUserList = items.OrderBy(x => x.Id).Skip(skip).Take(Page_Size).ToList();
                return obj;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        
        public ReferralUserListResponse ListUserParticipateReferralUser(int UserId,int PageIndex = 0, int Page_Size = 0, long ReferralServiceId = 0, string ReferUserName = null, DateTime? FromDate = null, DateTime? ToDate = null, int ReferralChannelTypeId = 0)
        {
            try
            {
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }
                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);

                var items = (from rs in _dbContext.ReferralUser
                             join st in _dbContext.ReferralService on rs.ReferralServiceId equals st.Id
                             join us in _dbContext.Users on rs.UserId equals us.Id
                             join rus in _dbContext.Users on rs.ReferUserId equals rus.Id
                             join cht in _dbContext.ReferralChannelType on rs.ReferralChannelTypeId equals cht.Id
                             where rs.UserId.Equals(UserId)
                             select new ReferralUserListViewModel
                             {
                                 Id = rs.Id,
                                 UserId = rs.UserId,
                                 ReferUserId = rs.ReferUserId,
                                 UserName = us.UserName,
                                 ReferUserName = rus.UserName,
                                 UserReferralCode = us.ReferralCode,
                                 ReferUserReferralCode = rus.ReferralCode,
                                 ReferralChannelTypeId = rs.ReferralChannelTypeId,
                                 ReferralChanneTypeName = cht.ChannelTypeName,
                                 ReferralServiceId = rs.ReferralServiceId,
                                 ReferralServiceDescription = st.Description,
                                 IsCommissionCreditedName = rs.IsCommissionCredited == 0 ? "No" : "Yes",
                                 CreatedDate = rs.CreatedDate
                             }
                            ).ToList();


                if (ReferralServiceId != 0)
                {
                    items = items.Where(x => x.ReferralServiceId == ReferralServiceId).ToList();
                }
                if (ReferralChannelTypeId != 0)
                {
                    items = items.Where(x => x.ReferralChannelTypeId == ReferralChannelTypeId).ToList();
                }
                if (!String.IsNullOrEmpty(ReferUserName))
                {
                    items = items.Where(x => x.ReferUserName == ReferUserName).ToList();
                }

                if (FromDate != null && ToDate != null)
                {
                    items = items.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }

                int TotalCount = items.Count();

                ReferralUserListResponse obj = new ReferralUserListResponse();
                obj.TotalCount = TotalCount;
                obj.ReferralUserList = items.OrderBy(x => x.Id).Skip(skip).Take(Page_Size).ToList();
                return obj;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public async Task<int> GetUserReferralCount(int UserId)
        {
            int UserReferralCount = await Task.FromResult(_dbContext.ReferralUser.Where(i => i.UserId == UserId).Count());
            return UserReferralCount;
        }

        public async Task<int> GetAdminReferralCount()
        {
            int AdminReferralCount = await Task.FromResult(_dbContext.ReferralUser.Count());
            return AdminReferralCount;
        }

        public async Task<string> GetUserReferralCode(int UserId)
        {
            var ReferralData = await Task.FromResult(_dbContext.Users.Where(i => i.Id == UserId).FirstOrDefault());
            return Convert.ToString(ReferralData.Email);// Display Referrel code as Email Address add by jagdish : 03-09-2019
        }

    }
}
