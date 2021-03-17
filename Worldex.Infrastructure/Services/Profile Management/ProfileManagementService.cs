using Worldex.Core.Entities.Complaint;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Profile_Management;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Organization;
using Worldex.Core.Interfaces.Profile_Management;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Profile_Management;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.Profile_Management
{
    public class ProfileManagementService : IProfileMaster
    {
        private readonly ICustomRepository<ProfileMaster> _profileRepository;
        private readonly ICustomRepository<Typemaster> _typemasterRepository;
        private readonly ICustomRepository<SubscriptionMaster> _subscriptionRepository;
        private readonly ICustomRepository<ProfileConfiguration> _profileConfigurationRepository;
        private readonly ICustomRepository<ProfileLevelMaster> _profilelevelRepository;
        private readonly IActivityMasterConfiguration _commontypemasterdata;
        private readonly WorldexContext _dbContext;
        public ProfileManagementService(ICustomRepository<ProfileMaster> profileRepository, ICustomRepository<Typemaster> typemasterRepository
            , ICustomRepository<SubscriptionMaster> subscriptionRepository,
            ICustomRepository<ProfileConfiguration> profileConfigurationRepository,
            ICustomRepository<ProfileLevelMaster> profilelevelRepository,
            IActivityMasterConfiguration commontypemasterdata,
            WorldexContext dbContext)
        {
            _profileRepository = profileRepository;
            _typemasterRepository = typemasterRepository;
            _subscriptionRepository = subscriptionRepository;
            _profileConfigurationRepository = profileConfigurationRepository;
            _profilelevelRepository = profilelevelRepository;
            _commontypemasterdata = commontypemasterdata;
            _dbContext = dbContext;
        }

        public List<ProfileMasterData> GetProfileData(int userid)
        {
            try
            {
                //var ProfileDataList = _profileRepository.Table.OrderBy(i => i.Level).ToList();

                var ProfileDataList = (from pm in _profileRepository.Table
                                       join tm in _typemasterRepository.Table on pm.TypeId equals tm.Id
                                       join ptm in _profilelevelRepository.Table on pm.Profilelevel equals ptm.Id
                                       where tm.Type == "Profile" && pm.Status == 1
                                       select new
                                       {
                                           Description = pm.Description,
                                           LevelName = pm.LevelName,
                                           ProfileFree = pm.ProfileFree,
                                           TypeId = pm.TypeId,
                                           DepositFee = pm.DepositFee,
                                           Withdrawalfee = pm.Withdrawalfee,
                                           Tradingfee = pm.Tradingfee,
                                           //WithdrawalLimit = pm.WithdrawalLimit,
                                           Id = pm.Id,
                                           KYCLevel = pm.KYCLevel,
                                           Profilelevel = pm.Profilelevel,
                                           SubscriptionAmount = pm.SubscriptionAmount,
                                           IsRecursive = pm.IsRecursive,
                                           IsProfileExpiry = pm.IsProfileExpiry
                                       }).OrderBy(i => i.KYCLevel).ToList();
                List<ProfileMasterData> listmodel = new List<ProfileMasterData>();
                if (ProfileDataList != null)
                {
                    foreach (var item in ProfileDataList)
                    {
                        ProfileMasterData model = new ProfileMasterData();
                        model.Description = item.Description;
                        model.LevelName = item.LevelName;
                        model.ProfileFree = item.ProfileFree;
                        //model.TypeId = _typemasterRepository.Table.Where(i => i.Id == item.TypeId).FirstOrDefault().SubType;
                        model.Type = _commontypemasterdata.GetTypeMasterData().Where(i => i.Id == item.TypeId).Count() > 0 ? _commontypemasterdata.GetTypeMasterData().Where(i => i.Id == item.TypeId).FirstOrDefault().SubType : string.Empty;
                        model.DepositFee = item.DepositFee;
                        model.Withdrawalfee = item.Withdrawalfee;
                        model.Tradingfee = item.Tradingfee;
                        //model.WithdrawalLimit = item.WithdrawalLimit;
                        long profileid = 0;
                        profileid = _subscriptionRepository.Table.Where(s => s.UserId == userid && s.Status == 1).FirstOrDefault().ProfileId;
                        if (item.Id == profileid)
                            model.ActiveStatus = true;
                        else
                            model.ActiveStatus = false;
                        model.ProfileLevel = item.Profilelevel;
                        model.SubscriptionAmount = item.SubscriptionAmount;
                        model.IsProfileExpiry = item.IsProfileExpiry;
                        model.IsRecursive = item.IsRecursive;
                        model.KYCLevel = item.KYCLevel;
                        model.ProfileId = item.Id;
                        listmodel.Add(model);
                    }
                }

                return listmodel;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }


        //        Select* from ProfileMaster pm
        //inner join Typemaster tm on pm.TypeId = tm.Id and  tm.[Type]= 'SocialProfile' and tm.EnableStatus = 0
        //left join subscriptionmaster sm on pm.Id = sm.ProfileId and sm.ActiveStatus = 1
        public List<SocialProfileModel> GetSocialProfileData(int userid = 0)
        {
            var ProfileData = (from pm in _profileRepository.Table
                               join tm in _typemasterRepository.Table on pm.TypeId equals tm.Id
                               where tm.Type == "SocialProfile" && tm.Status == 1 && pm.Status == 1
                               select new { Profile = pm });


            var LeaderProfileConfig = _profileConfigurationRepository.Table.Where(i => i.ConfigType == ProfileSocialCongifType.Leader_Profile.ToString() && !i.IsEnable && !i.IsDeleted).OrderByDescending(i => i.CreatedDate).ToList();
            var FollowerProfileConfig = _profileConfigurationRepository.Table.Where(i => i.ConfigType == ProfileSocialCongifType.Follower_Profile.ToString() && !i.IsEnable && !i.IsDeleted).OrderByDescending(i => i.CreatedDate).ToList();


            //int userid = 0;

            List<SocialProfileModel> ProfileList = new List<SocialProfileModel>();

            foreach (var item in ProfileData)
            {
                var profile = (dynamic)null;

                SocialProfileModel model = new SocialProfileModel();
                model.ProfileId = item.Profile.Id;
                model.ProfileType = item.Profile.LevelName;

                profile = _subscriptionRepository.Table.Where(s => s.UserId == userid && s.ProfileId == item.Profile.Id && s.Status == 1).FirstOrDefault();
                if (profile != null)
                {
                    if (item.Profile.Id == profile.ProfileId)
                        model.Subscribe = true;
                    else
                        model.Subscribe = false;
                }
                else
                    model.Subscribe = false;

                foreach (var litem in LeaderProfileConfig)
                {
                    switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), litem.ConfigKey))
                    {
                        case EnProfileConfigKey.Profile_Visiblity_Leader:
                            model.Profile_Visiblity = litem.ConfigValue;
                            break;
                        case EnProfileConfigKey.Can_Have_Followers_Leader:
                            model.Can_Have_Followers = litem.ConfigValue;
                            break;
                        case EnProfileConfigKey.Can_Follow_Leaders_Lead:
                            model.Can_Follow_Leaders = litem.ConfigValue;
                            break;
                        case EnProfileConfigKey.Can_Copy_Trade_Leader:
                            model.Can_Copy_Trade = litem.ConfigValue;
                            break;
                        case EnProfileConfigKey.Can_Mirror_Trade_Leader:
                            model.Can_Mirror_Trade = litem.ConfigValue;
                            break;
                        case EnProfileConfigKey.Minimum_Trade_Volume_Leader:
                            model.Minimum_Trade_Volume = litem.ConfigValue;
                            break;

                    }

                }
                foreach (var fitem in FollowerProfileConfig)
                {
                    switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), fitem.ConfigKey))
                    {
                        case EnProfileConfigKey.Profile_Visiblity_Follower:
                            model.Profile_Visiblity = fitem.ConfigValue;
                            break;
                        case EnProfileConfigKey.Can_Have_Followers_Follow:
                            model.Can_Have_Followers = fitem.ConfigValue;
                            break;
                        case EnProfileConfigKey.Can_Follow_Leaders_Follower:
                            model.Can_Follow_Leaders = fitem.ConfigValue;
                            break;
                        case EnProfileConfigKey.Can_Copy_Trade_Folower:
                            model.Can_Copy_Trade = fitem.ConfigValue;
                            break;
                        case EnProfileConfigKey.Can_Mirror_Trade_Follower:
                            model.Can_Mirror_Trade = fitem.ConfigValue;
                            break;
                        case EnProfileConfigKey.Minimum_Trade_Volume_Follower:
                            model.Minimum_Trade_Volume = fitem.ConfigValue;
                            break;

                    }
                }

                ProfileList.Add(model);
            }


            return ProfileList;
        }

        public bool GetSocialProfile(int ProfileId = 0)
        {
            var ProfileData = (from pm in _profileRepository.Table
                               join tm in _typemasterRepository.Table on pm.TypeId equals tm.Id
                               where tm.Type == "SocialProfile" && tm.Status == 1 && pm.Status == 1 && pm.Id == ProfileId
                               select new { Profile = pm }).FirstOrDefault();

            if (ProfileData != null)
            {
                return true;
            }
            return false;

        }

        public GetProfileDataResponse GetAllUserProfileData(int UserId = 0, int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                //long currentprofile = _subscriptionRepository.Table.Where(j => j.UserId == UserId && j.Status == 1).Count() > 0 ? _subscriptionRepository.Table.Where(j => j.UserId == UserId && j.Status == 1).FirstOrDefault().ProfileId : 0;

                IQueryable<UserWiseProfileData> UserProfileData;

                string Query = "Select tm.SubType as [Type],pm.ProfileFree,pm.Description,pm.KYCLevel,klm.KYCName as [KYCLevelName],plm.ProfileName as [LevelName],pm.DepositFee,pm.Withdrawalfee,pm.Tradingfee,";
                Query += " Case when Sm.status=1 then cast('true' as bit)   else cast('false' as bit)  end as [Status],pm.Id as [ProfileId],pm.Profilelevel,pm.IsProfileExpiry,pm.SubscriptionAmount,pm.IsRecursive  from profilemaster pm";
                Query += " Left outer join SubscriptionMaster sm on pm.Id =sm.ProfileId and sm.UserId=" + UserId + "";
                Query += " Inner join Typemaster tm on tm.id = pm.typeid and tm.Type='Profile'";
                Query += " Inner join ProfileLevelMaster plm on pm.Profilelevel=plm.Id";
                Query += " Inner join kYCLevelMaster klm on pm.KYCLevel=klm.level";
                //Query += " Where pm.Profilelevel > 0";
                UserProfileData = _dbContext.GetSubscriptionProfileData.FromSql(Query);

                List<UserWiseProfileData> UserData = new List<UserWiseProfileData>();

                UserData = UserProfileData.ToList();
                var CurrentProfile = UserData.Where(i => i.Status).FirstOrDefault();
                var AllUserProfileData = new List<UserWiseProfileData>();
                foreach (var item in UserData)
                {
                    //bool status = false;//komal 03 May 2019, Cleanup
                    UserWiseProfileData imodel = new UserWiseProfileData();
                    if (item.ProfileLevel <= CurrentProfile.ProfileLevel)
                        imodel.Status = false;
                    else
                        imodel.Status = true;
                    imodel.LevelName = item.LevelName;
                    imodel.ProfileLevel = item.ProfileLevel;
                    imodel.Tradingfee = item.Tradingfee;
                    imodel.Type = item.Type;
                    imodel.Withdrawalfee = item.Withdrawalfee;
                    imodel.IsProfileExpiry = item.IsProfileExpiry;
                    imodel.IsRecursive = item.IsRecursive;
                    imodel.KYCLevel = item.KYCLevel;
                    imodel.KYCLevelName = item.KYCLevelName;
                    imodel.DepositFee = item.DepositFee;
                    imodel.Description = item.Description;
                    imodel.SubscriptionAmount = item.SubscriptionAmount;
                    imodel.ProfileFree = item.ProfileFree;
                    imodel.ProfileId = item.ProfileId;
                    AllUserProfileData.Add(imodel);
                }

                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }

                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);
                GetProfileDataResponse UserProfileConfiguration = new GetProfileDataResponse()
                {
                    GetUserWiseProfileData = AllUserProfileData.Skip(skip).Take(Page_Size).OrderBy(i => i.ProfileLevel).ToList(),
                    TotalCount = AllUserProfileData.Count()
                };

                return UserProfileConfiguration;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
    }
}
