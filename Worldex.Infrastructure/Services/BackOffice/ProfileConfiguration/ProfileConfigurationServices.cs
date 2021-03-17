using AutoMapper.Configuration;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Profile_Management;
using Worldex.Core.Interfaces.BackOffice.ProfileConfiguration;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Profile_Management;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.BackOffice.PasswordPolicy
{
    public class ProfileConfigurationServices : IProfileConfigurationData
    {
        private readonly ICustomRepository<ProfileMaster> _profileRepository;

        private readonly WorldexContext _dbContext;
        public ProfileConfigurationServices(ICustomRepository<ProfileMaster> profileRepository, WorldexContext dbContext)
        {
            _profileRepository = profileRepository;
           
            _dbContext = dbContext;
        }
        public long AddProfileConfiguration(ProfileConfigurationAddReqViewModel profileConfiguration)
        {
            try
            {
                ProfileMaster profileMaster = new ProfileMaster()
                {
                    IsProfileExpiry = profileConfiguration.IsProfileExpiry,
                    IsRecursive = profileConfiguration.IsRecursive,
                    KYCLevel = profileConfiguration.KYCLevel,
                    LevelName = profileConfiguration.LevelName,
                    DepositFee = profileConfiguration.DepositFee,
                    ProfileFree = profileConfiguration.ProfileFree,
                    Profilelevel = profileConfiguration.Profilelevel,
                    Status = 1,
                    SubscriptionAmount = profileConfiguration.SubscriptionAmount,
                    TradeLimit = profileConfiguration.TradeLimit,
                    Tradingfee = profileConfiguration.Tradingfee,
                    TransactionLimit = profileConfiguration.TransactionLimit,
                    TypeId = profileConfiguration.TypeId,
                    DepositLimit = profileConfiguration.DepositLimit,
                    Withdrawalfee = profileConfiguration.Withdrawalfee,
                    WithdrawalLimit = profileConfiguration.WithdrawalLimit,
                    Description = profileConfiguration.Description,
                    CreatedBy = profileConfiguration.UserId,
                    CreatedDate = DateTime.UtcNow

                };
                _profileRepository.Insert(profileMaster);
                return profileMaster.Id;

            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }

        }
        public long UpdateProfileConfiguration(ProfileConfigurationUpdateReqViewModel profileConfiguration)
        {
            try
            {
                var IsprofileMasterExist = _profileRepository.Table.FirstOrDefault(i => i.Id == profileConfiguration.Id && i.Status != 9);
               
                

                if (IsprofileMasterExist != null)
                {
                    IsprofileMasterExist.IsProfileExpiry = profileConfiguration.IsProfileExpiry;
                    IsprofileMasterExist.IsRecursive = profileConfiguration.IsRecursive;
                    IsprofileMasterExist.KYCLevel = profileConfiguration.KYCLevel;
                    IsprofileMasterExist.LevelName = profileConfiguration.LevelName;
                    IsprofileMasterExist.DepositFee = profileConfiguration.DepositFee;
                    IsprofileMasterExist.ProfileFree = profileConfiguration.ProfileFree;
                    IsprofileMasterExist.Profilelevel = profileConfiguration.Profilelevel;
                   
                    IsprofileMasterExist.SubscriptionAmount = profileConfiguration.SubscriptionAmount;
                    IsprofileMasterExist.TradeLimit = profileConfiguration.TradeLimit;
                    IsprofileMasterExist.Tradingfee = profileConfiguration.Tradingfee;
                    IsprofileMasterExist.TransactionLimit = profileConfiguration.TransactionLimit;
                    IsprofileMasterExist.TypeId = profileConfiguration.TypeId;
                    IsprofileMasterExist.DepositLimit = profileConfiguration.DepositLimit;
                    IsprofileMasterExist.Withdrawalfee = profileConfiguration.Withdrawalfee;
                    IsprofileMasterExist.WithdrawalLimit = profileConfiguration.WithdrawalLimit;
                    IsprofileMasterExist.Description = profileConfiguration.Description;
                    IsprofileMasterExist.UpdatedBy = profileConfiguration.UserId;
                    IsprofileMasterExist.UpdatedDate = DateTime.UtcNow;
                    _profileRepository.Update(IsprofileMasterExist);
                    return IsprofileMasterExist.Id;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }

        }
        public long DeleteProfileConfiguration(ProfileConfigurationDeleteReqViewModel profileConfiguration)
        {
            try
            {
                var IsprofileMasterExist = _profileRepository.Table.FirstOrDefault(i => i.Id == profileConfiguration.Id && i.Status != 9);
                if (IsprofileMasterExist != null)
                {
                    IsprofileMasterExist.Status = 9;
                    IsprofileMasterExist.UpdatedDate = DateTime.UtcNow;
                    IsprofileMasterExist.UpdatedBy = profileConfiguration.UserId;
                    _profileRepository.Update(IsprofileMasterExist);
                    return IsprofileMasterExist.Id;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public ProfileConfigurationGetResponseViewmodel GetProfileConfiguration(int PageIndex = 0, int Page_Size = 0, long Typeid = 0, bool IsRecursive = true, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            try
            {
                IQueryable<GetProfileConfigurationViewModel> Result;

                string Query = "select pm.id,tm.SubType as TypeName ,pm.Typeid,pm.KYCLevel,pm.LevelName,km.KYCName,pm.CreatedDate,pm.SubscriptionAmount,pm.Description,pm.DepositFee,pm.Withdrawalfee,pm.IsRecursive,";
                Query += " pm.IsProfileExpiry,pm.ProfileFree,pm.Profilelevel,pm.Tradingfee,PLM.ProfileName,pm.TransactionLimit,pm.WithdrawalLimit,pm.DepositLimit,pm.TradeLimit from ProfileMaster Pm  inner join kYCLevelMaster km on Pm.KYCLevel =km.Level Inner join typemaster TM On Pm.TypeId=tm.Id and Tm.type = 'Profile' Inner join ProfileLevelMaster PLM on PLM.id=pm.Profilelevel  Where pm.Status=1 and pm.Profilelevel>0 and ";
                if (FromDate != null && ToDate != null)
                {
                    string FromDatestr = String.Format("{0:yyyy/MM/dd}", FromDate);
                    string ToDatestr = String.Format("{0:yyyy/MM/dd}", ToDate);
                    Query += " pm.CreatedDate between '" + FromDatestr + " 00:00:00.0000000' And '" + ToDatestr + " 23:59:59.999' and ";
                }
                else
                {
                    var date = DateTime.Now.ToString("yyyy-MM-dd");
                    Query += " pm.CreatedDate between '" + date + " 00:00:00.0000000' and '" + date + " 23:59:59.999' and  ";
                }

                if (Typeid > 0)
                    Query += " tm.Id=" + Typeid + " and";
                if (IsRecursive)
                    Query += " pm.IsRecursive=1";
                else
                    Query += " pm.IsRecursive=0";

                Result = _dbContext.GetProfileConfiguration.FromSql(Query);
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }

                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);
                var ProfileConfiguration = Result.Skip(skip).Take(Page_Size).OrderBy(i => i.Profilelevel).ToList();
                List<GetProfileConfigurationWithJsonViewModel> getProfileConfigurationList = new List<GetProfileConfigurationWithJsonViewModel>();
                foreach (var item in ProfileConfiguration)
                {
                    GetProfileConfigurationWithJsonViewModel getProfileConfiguration = new GetProfileConfigurationWithJsonViewModel();
                    getProfileConfiguration.Id = item.Id;
                    getProfileConfiguration.IsProfileExpiry = item.IsProfileExpiry;
                    getProfileConfiguration.IsRecursive = item.IsRecursive;
                    getProfileConfiguration.ProfileFree = item.ProfileFree;
                    getProfileConfiguration.Profilelevel = item.Profilelevel;
                    getProfileConfiguration.ProfileName = item.ProfileName;
                    getProfileConfiguration.SubscriptionAmount = item.SubscriptionAmount;
                    getProfileConfiguration.Tradingfee = item.Tradingfee;
                    getProfileConfiguration.TypeName = item.TypeName;
                    getProfileConfiguration.Typeid = item.Typeid;
                   
                    getProfileConfiguration.KYCLevel = item.KYCLevel;
                    getProfileConfiguration.LevelName = item.LevelName;
                    getProfileConfiguration.KYCName = item.KYCName;
                    getProfileConfiguration.Withdrawalfee = item.Withdrawalfee;
                    getProfileConfiguration.CreatedDate = item.CreatedDate;
                    getProfileConfiguration.DepositFee = item.DepositFee;
                    getProfileConfiguration.Description = item.Description;
                    var TransactionLimit = JsonConvert.DeserializeObject<Dictionary<string, List<TransactionLimitViewModel>>>(item.TransactionLimit);
                    getProfileConfiguration.TransactionLimit = TransactionLimit.SelectMany(kvp => kvp.Value).ToList();
                    var WithdrawalLimit = JsonConvert.DeserializeObject<Dictionary<string, List<TransactionLimitViewModel>>>(item.WithdrawalLimit);
                    getProfileConfiguration.WithdrawalLimit = WithdrawalLimit.SelectMany(kvp => kvp.Value).ToList();
                    var TradeLimit = JsonConvert.DeserializeObject<Dictionary<string, List<TransactionLimitViewModel>>>(item.TradeLimit);
                    getProfileConfiguration.TradeLimit = TradeLimit.SelectMany(kvp => kvp.Value).ToList();
                    var DepositLimit = JsonConvert.DeserializeObject<Dictionary<string, List<TransactionLimitViewModel>>>(item.DepositLimit);
                    getProfileConfiguration.DepositLimit = DepositLimit.SelectMany(kvp => kvp.Value).ToList();
                    getProfileConfigurationList.Add(getProfileConfiguration);
                }
                ProfileConfigurationGetResponseViewmodel UserProfileConfiguration = new ProfileConfigurationGetResponseViewmodel()
                {
                    getProfileConfiguration = getProfileConfigurationList,
                    TotalCount = Result.Count()
                };
                return UserProfileConfiguration;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public ProfileConfigurationGetResponseByIdViewmodel GetProfileConfigurationById(long id)
        {
            try
            {
                IQueryable<GetProfileConfigurationByIdViewModel> Result;

                string Query = "Select Id,CreatedDate,Typeid,SubscriptionAmount,Description,KYCLevel,LevelName,DepositFee,Withdrawalfee,";
                Query += " Tradingfee,WithdrawalLimit,IsRecursive,depositlimit,IsProfileExpiry,ProfileFree,Profilelevel,TradeLimit,TransactionLimit";
                Query += " From profilemaster where id=" + id;
             

                Result = _dbContext.GetProfileConfigurationById.FromSql(Query);
                var response = Result.ToList();
                List<GetProfileConfigurationbyidWithJsonViewModel> getProfileConfigurationList = new List<GetProfileConfigurationbyidWithJsonViewModel>();
                foreach (var item in response)
                {
                    GetProfileConfigurationbyidWithJsonViewModel getProfileConfiguration = new GetProfileConfigurationbyidWithJsonViewModel();
                    getProfileConfiguration.Id = item.Id;
                    getProfileConfiguration.IsProfileExpiry = item.IsProfileExpiry;
                    getProfileConfiguration.IsRecursive = item.IsRecursive;
                    getProfileConfiguration.ProfileFree = item.ProfileFree;
                    getProfileConfiguration.Profilelevel = item.Profilelevel;
                    getProfileConfiguration.Typeid = item.Typeid;
                    getProfileConfiguration.SubscriptionAmount = item.SubscriptionAmount;
                    getProfileConfiguration.Tradingfee = item.Tradingfee;
                    getProfileConfiguration.KYCLevel = item.KYCLevel;
                    getProfileConfiguration.LevelName = item.LevelName;
                    getProfileConfiguration.Withdrawalfee = item.Withdrawalfee;
                    getProfileConfiguration.CreatedDate = item.CreatedDate;
                    getProfileConfiguration.DepositFee = item.DepositFee;
                    getProfileConfiguration.Description = item.Description;
                    var TransactionLimit = JsonConvert.DeserializeObject<Dictionary<string, List<TransactionLimitViewModel>>>(item.TransactionLimit);
                    getProfileConfiguration.TransactionLimit = TransactionLimit.SelectMany(kvp => kvp.Value).ToList();
                    var WithdrawalLimit = JsonConvert.DeserializeObject<Dictionary<string, List<TransactionLimitViewModel>>>(item.WithdrawalLimit);
                    getProfileConfiguration.WithdrawalLimit = WithdrawalLimit.SelectMany(kvp => kvp.Value).ToList();
                    var TradeLimit = JsonConvert.DeserializeObject<Dictionary<string, List<TransactionLimitViewModel>>>(item.TradeLimit);
                    getProfileConfiguration.TradeLimit = TradeLimit.SelectMany(kvp => kvp.Value).ToList();
                    var DepositLimit = JsonConvert.DeserializeObject<Dictionary<string, List<TransactionLimitViewModel>>>(item.DepositLimit);
                    getProfileConfiguration.DepositLimit = DepositLimit.SelectMany(kvp => kvp.Value).ToList();
                    getProfileConfigurationList.Add(getProfileConfiguration);
                }
                ProfileConfigurationGetResponseByIdViewmodel UserProfileConfiguration = new ProfileConfigurationGetResponseByIdViewmodel()
                {
                    getProfileConfiguration = getProfileConfigurationList,
             
                };
                return UserProfileConfiguration;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }


        public List<ProfilelevelCountViewmodel> GetProfilelevelmaster()
        {
            try
            {
                IQueryable<ProfilelevelCountViewmodel> Result;
                string Query = "Select pm.Id As ProfileId,plm.ProfileName, Count(sm.profileId) as LevelCount from profilemaster pm";
                Query += " inner join ProfileLevelMaster plm on pm.Profilelevel = plm.Id and plm.status=1";
                Query += " left join SubscriptionMaster sm on pm.Id = sm.ProfileId";
                Query += " group by  pm.Id,plm.ProfileName";
                Result = _dbContext.ProfilelevelCount.FromSql(Query);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public ProfilewiseuserlistResponseVoewmodel Profilewiseuserlist(long ProfileId, int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                IQueryable<ProfilewiseuserlistViewmodel> Result;
                string Query = "select sm.CreatedDate,sm.Status,Isnull(bu.UserName,'') As UserName ,Isnull(bu.email,'') as [Email],";
                Query += " Isnull(bu.Mobile,'') as [Mobile] from SubscriptionMaster sm inner join bizuser bu on sm.UserId=bu.Id";
                Query += " where ProfileId=" + ProfileId;

                Result = _dbContext.Profilewiseuserlist.FromSql(Query);
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }

                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }
                var skip = Page_Size * (PageIndex - 1);
                Result.Skip(skip).Take(Page_Size).ToList();
                ProfilewiseuserlistResponseVoewmodel profilewiseuserlistResponse = new ProfilewiseuserlistResponseVoewmodel();
                profilewiseuserlistResponse.profilewiseuserlist = Result.Skip(skip).Take(Page_Size).ToList();
                profilewiseuserlistResponse.TotalCount = Result.Count();
                return profilewiseuserlistResponse;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }


        public GetProfilelevelmasterResponse GetProfilelevelmaster(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                IQueryable<GetProfilelevelmaster> Result;
                string Query = "Select id,createddate,profilename From Profilelevelmaster where Status=1";

                Result = _dbContext.GetProfilelevelmaster.FromSql(Query);
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }

                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }
                var skip = Page_Size * (PageIndex - 1);
                Result.Skip(skip).Take(Page_Size).ToList();
                GetProfilelevelmasterResponse Response = new GetProfilelevelmasterResponse();
                Response.GetProfilelevelmasters = Result.Skip(skip).Take(Page_Size).ToList();
                Response.TotalCount = Result.Count();
                return Response;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public List<GetProfilelevelmaster> GetProfilelevelmasterDropDownList()
        {
            try
            {
                IQueryable<GetProfilelevelmaster> Result;
                string Query = "Select id,createddate,profilename From Profilelevelmaster where Status=1";

                Result = _dbContext.GetProfilelevelmaster.FromSql(Query);

                return Result.ToList();
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        public string CreateJson(string Jsonname, string JsonData)
        {
            try
            {
                string JsonString = string.Empty;
                JsonString = "{" + "\"" + Jsonname + "\"" + ":[";
                JsonString += JsonData;
                JsonString += "]}";
                return JsonString;
            }
            catch (Exception ex)
            {
                ex.ToString();

                throw ex;
            }
        }

        public long IsProfilelevelExistConfiguration(long ProfileLevel)
        {
            try
            {
                var IsprofileMasterExist = _profileRepository.Table.FirstOrDefault(i => i.Profilelevel == ProfileLevel && i.Status != 9 && i.Status == 1);
                if (IsprofileMasterExist != null)
                {
                    return IsprofileMasterExist.Id;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }

        }


    }
}
