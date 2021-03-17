using System;
using System.Collections.Generic;
using Worldex.Core.ViewModels.Referral;
using Worldex.Core.Entities.Referral;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Interfaces.Referral;
using System.Linq;

namespace Worldex.Infrastructure.Services.Referral
{
    public class ReferralUserClickServices :IReferralUserClick
    {
        private readonly WorldexContext _dbContext;
        
        private readonly ICustomRepository<ReferralUserClick> _ReferralUserClickRepository;

        public ReferralUserClickServices(ICustomRepository<ReferralUserClick> ReferralUserClickRepository, WorldexContext dbContext)
        {
            _dbContext = dbContext;           
            _ReferralUserClickRepository = ReferralUserClickRepository;
        }

        public ReferralUserClickListResponse ListAdminReferralUserClick(int PageIndex = 0, int Page_Size = 0, long ReferralChannelTypeId = 0,long ReferralServiceId = 0, string UserName = null, DateTime? FromDate = null, DateTime? ToDate = null)
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

                var items = (from rs in _dbContext.ReferralUserClick
                             join st in _dbContext.ReferralService on rs.ReferralServiceId equals st.Id
                             join us in _dbContext.Users on rs.UserId equals us.Id                             
                             join cht in _dbContext.ReferralChannelType on rs.ReferralChannelTypeId equals cht.Id
                             select new ReferralUserClickListViewModel
                             {
                                 Id = rs.Id,
                                 UserId = rs.UserId,                                
                                 UserName = us.UserName,                                
                                 UserReferralCode = us.ReferralCode,                                
                                 ReferralChannelTypeId = rs.ReferralChannelTypeId,
                                 ReferralChanneTypeName = cht.ChannelTypeName,
                                 ReferralServiceId = rs.ReferralServiceId,
                                 ReferralServiceDescription = st.Description,
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
                if (!String.IsNullOrEmpty(UserName))
                {
                    items = items.Where(x => x.UserName == UserName).ToList();
                } 
                if (FromDate != null && ToDate != null)
                {
                    items = items.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }

                int TotalCount = items.Count();

                ReferralUserClickListResponse obj = new ReferralUserClickListResponse();
                obj.TotalCount = TotalCount;
                obj.ReferralUserClickList = items.OrderBy(x => x.Id).Skip(skip).Take(Page_Size).ToList();
                return obj;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public ReferralUserClickListResponse ListUserReferralUserClick(int UserId, int PageIndex = 0, int Page_Size = 0, long ReferralChannelTypeId = 0,long ReferralServiceId = 0, DateTime? FromDate = null, DateTime? ToDate = null)
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
                var items = (from rs in _dbContext.ReferralUserClick
                             join st in _dbContext.ReferralService on rs.ReferralServiceId equals st.Id
                             join us in _dbContext.Users on rs.UserId equals us.Id                           
                             join cht in _dbContext.ReferralChannelType on rs.ReferralChannelTypeId equals cht.Id
                             where rs.UserId.Equals(UserId)
                             select new ReferralUserClickListViewModel
                             {
                                 Id = rs.Id,
                                 UserId = rs.UserId,                                 
                                 UserName = us.UserName,                                 
                                 UserReferralCode = us.ReferralCode,                                
                                 ReferralChannelTypeId = rs.ReferralChannelTypeId,
                                 ReferralChanneTypeName = cht.ChannelTypeName,
                                 ReferralServiceId = rs.ReferralServiceId,
                                 ReferralServiceDescription = st.Description,
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
                if (FromDate != null && ToDate != null)
                {
                    items = items.Where(x => x.CreatedDate.Date >= FromDate && x.CreatedDate.Date <= ToDate).ToList();
                }

                int TotalCount = items.Count();
                ReferralUserClickListResponse obj = new ReferralUserClickListResponse();
                obj.TotalCount = TotalCount;
                obj.ReferralUserClickList = items.OrderBy(x => x.Id).Skip(skip).Take(Page_Size).ToList();
                return obj;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        
    }
}
