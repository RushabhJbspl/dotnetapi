using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Profile_Management;
using Worldex.Core.Entities.SocialProfile;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Interfaces.SocialProfile;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.Profile_Management;
using Worldex.Core.ViewModels.SocialProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Configuration
{
    public class ProfileConfigurationService : IProfileConfigurationService
    {
        private readonly ICustomRepository<ProfileConfiguration> _profileConfigurationRepository;
        private readonly ICustomRepository<UserProfileConfig> _userProfileConfigRepository;
        private readonly ICustomRepository<FollowerMaster> _followerMasterRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly WorldexContext _dbContext;
        private readonly IGroupMasterService _groupMasterService;
        private readonly ICustomRepository<WatchMaster> _watchcustomRepository;
        // private readonly ICustomRepository<SubscriptionMaster> _subscriptionRepository;
        private readonly ICustomRepository<ProfileMaster> _profileRepository;

        public ProfileConfigurationService(ICustomRepository<ProfileConfiguration> profileConfigurationRepository, UserManager<ApplicationUser> userManager, WorldexContext dbContext, ICustomRepository<UserProfileConfig> userProfileConfigRepository, ICustomRepository<FollowerMaster> followerMasterRepository, IGroupMasterService groupMasterService, ICustomRepository<ProfileMaster> profileRepository,
            ICustomRepository<WatchMaster> watchcustomRepository)
        {
            this._profileConfigurationRepository = profileConfigurationRepository;
            this._userManager = userManager;
            this._dbContext = dbContext;
            this._userProfileConfigRepository = userProfileConfigRepository;
            this._followerMasterRepository = followerMasterRepository;
            this._groupMasterService = groupMasterService;
            this._profileRepository = profileRepository;
            this._watchcustomRepository = watchcustomRepository;
        }

        public List<ProfileConfigurationModel> GetProfileConfiguration(string ConfigType = null)
        {
            var ProfileConfig = (dynamic)null;
            if (!string.IsNullOrEmpty(ConfigType))
            {
                ProfileConfig = _profileConfigurationRepository.Table.Where(i => i.ConfigType == ConfigType && i.Status == Convert.ToInt16(ServiceStatus.Active)).OrderByDescending(i => i.CreatedDate).ToList();
            }
            else
            { ProfileConfig = _profileConfigurationRepository.Table.Where(i => i.ConfigType == ConfigType && i.Status == Convert.ToInt16(ServiceStatus.Active)).OrderByDescending(i => i.CreatedDate).ToList(); }

            if (ProfileConfig == null)
            {
                return null;
            }

            var ProfilconfigList = new List<ProfileConfigurationModel>();
            foreach (var item in ProfileConfig)
            {
                ProfileConfigurationModel model = new ProfileConfigurationModel();
                model.Id = item.Id;
                model.ConfigType = item.ConfigType;
                model.ConfigKey = item.ConfigKey;
                model.ConfigValue = item.ConfigValue;

                ProfilconfigList.Add(model);
            }


            return ProfilconfigList;


        }

        public void SetLeaderProfileConfiguration(LeaderAdminPolicyModel model, string ConfigType = null, int UserId = 0)
        {

            var ProfileConfig = _profileConfigurationRepository.Table.Where(i => i.ConfigType == ConfigType && i.Status == Convert.ToInt16(ServiceStatus.Active)).OrderByDescending(i => i.CreatedDate).ToList();

            foreach (var item in ProfileConfig)
            {
                //string val = EnProfileConfigKey.Default_Visibility_of_Profile.ToString();
                switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), item.ConfigKey))
                {
                    case EnProfileConfigKey.Default_Visibility_of_Profile:
                        item.ConfigValue = Convert.ToString((EnVisibleProfile)model.Default_Visibility_of_Profile);
                        break;
                    case EnProfileConfigKey.Max_Number_Followers_can_Follow:
                        item.ConfigValue = model.Max_Number_Followers_can_Follow.ToString();
                        break;
                    case EnProfileConfigKey.Min_Number_of_Followers_can_Follow:
                        item.ConfigValue = model.Min_Number_of_Followers_can_Follow.ToString();
                        break;

                        #region
                        //case EnProfileConfigKey.Min_Balance_Require_in_Follower_Account_to_Follow:
                        //    item.ConfigValue = model.Min_Balance_Require_in_Follower_Account_to_Follow.ToString();
                        //    break;
                        //case EnProfileConfigKey.Min_Trade_Volume_Requir:
                        //    item.ConfigValue = model.Min_Trade_Volume_Requir.ToString();
                        //    break;
                        //case EnProfileConfigKey.Min_Trade_Volume_Requir_in_Time:
                        //    item.ConfigValue = Convert.ToString((EnProfileVolumeTime)model.Min_Trade_Volume_Requir_in_Time);
                        //    break;
                        //case EnProfileConfigKey.Subscription_Charge_Leader:
                        //    item.ConfigValue = model.Subscription_Charge.ToString();
                        //    break;
                        //case EnProfileConfigKey.Subscription_Charge_Frequency_Leader:
                        //    item.ConfigValue = Convert.ToString((EnSubscriptionChangeFrequency)model.Subscription_Charge_Frequency);
                        //    break;
                        //case EnProfileConfigKey.Can_Add_Pair_to_Watchlist_Leader:
                        //    item.ConfigValue = Convert.ToString((EnYesNo)model.Can_Add_Pair_to_Watchlist);
                        //    break;
                        //case EnProfileConfigKey.Max_Number_of_Pairs_to_Allow_in_Watchlist_Leader:
                        //    item.ConfigValue = model.Max_Number_of_Pairs_to_Allow_in_Watchlist.ToString();
                        //    break;
                        #endregion
                }

                item.UpdatedBy = UserId;
                item.UpdatedDate = DateTime.UtcNow;

                _profileConfigurationRepository.Update(item);

            }
        }

        public void SetFolowerProfileConfiguration(FollowerAdminModel model, string ConfigType = null, int UserId = 0)
        {

            var ProfileConfig = _profileConfigurationRepository.Table.Where(i => i.ConfigType == ConfigType && i.Status == Convert.ToInt16(ServiceStatus.Active)).OrderByDescending(i => i.CreatedDate).ToList();

            foreach (var item in ProfileConfig)
            {
                switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), item.ConfigKey))
                {
                    case EnProfileConfigKey.Can_Copy_Trade:
                        item.ConfigValue = Convert.ToString((EnYesNo)model.Can_Copy_Trade);
                        break;
                    case EnProfileConfigKey.Can_Mirror_Trade:
                        item.ConfigValue = Convert.ToString((EnYesNo)model.Can_Mirror_Trade);
                        break;

                    case EnProfileConfigKey.Maximum_Number_of_Leaders_to_Allow_Follow:
                        item.ConfigValue = model.Maximum_Number_of_Leaders_to_Allow_Follow.ToString();
                        break;
                    case EnProfileConfigKey.Can_Add_Leader_to_Watchlist_Follower:
                        item.ConfigValue = Convert.ToString((EnYesNo)model.Can_Add_Leader_to_Watchlist);
                        break;
                    case EnProfileConfigKey.Max_Number_of_Leader_to_Allow_in_Watchlist_Follower:
                        item.ConfigValue = model.Max_Number_of_Leader_to_Allow_in_Watchlist.ToString();
                        break;

                        #region
                        //case EnProfileConfigKey.Enable_Auto_Copy_Trade_Functionality:
                        //    item.ConfigValue = Convert.ToString((EnYesNo)model.Enable_Auto_Copy_Trade_Functionality);
                        //    break;
                        //case EnProfileConfigKey.Minimum_Copy_Trade_Percentage:
                        //    item.ConfigValue = model.Minimum_Copy_Trade_Percentage.ToString();
                        //    break;
                        //case EnProfileConfigKey.Default_Copy_Trade_Percentage:
                        //    item.ConfigValue = model.Default_Copy_Trade_Percentage.ToString();
                        //    break;
                        //case EnProfileConfigKey.Maximum_Copy_Trade_Percentage:
                        //    item.ConfigValue = model.Maximum_Copy_Trade_Percentage.ToString();
                        //    break;
                        //case EnProfileConfigKey.Maximum_Transaction_Amount_Limit:
                        //    item.ConfigValue = model.Maximum_Transaction_Amount_Limit.ToString();
                        //    break;
                        //case EnProfileConfigKey.Maximum_Number_of_Transactions_Limit:
                        //    item.ConfigValue = model.Maximum_Number_of_Transactions_Limit.ToString();
                        //    break;
                        //case EnProfileConfigKey.Subscription_Charge_Follower:
                        //    item.ConfigValue = model.Subscription_Charge.ToString();
                        //    break;
                        //case EnProfileConfigKey.Subscription_Charge_Frequency_Follower:
                        //    item.ConfigValue = Convert.ToString((EnSubscriptionChangeFrequency)model.Subscription_Charge_Frequency);
                        //    break;
                        #endregion
                }
                item.UpdatedBy = UserId;
                item.UpdatedDate = DateTime.UtcNow;
                _profileConfigurationRepository.Update(item);


            }
        }

        public async Task<LeaderAdminPolicyGetModel> GetLeaderProfileConfiguration()
        {
            var ProfileConfig = _profileConfigurationRepository.Table.Where(i => i.ConfigType == ProfileSocialCongifType.Leader_Admin_Policy.ToString() && i.Status == Convert.ToInt16(ServiceStatus.Active)).OrderByDescending(i => i.CreatedDate).ToList();

            LeaderAdminPolicyGetModel model = new LeaderAdminPolicyGetModel();
            int userid = 0;
            foreach (var item in ProfileConfig)
            {
                //string val = EnProfileConfigKey.Default_Visibility_of_Profile.ToString();
                switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), item.ConfigKey))
                {
                    case EnProfileConfigKey.Default_Visibility_of_Profile:
                        //item.ConfigValue = Convert.ToString((EnVisibleProfile)model.Default_Visibility_of_Profile);                        
                        model.Default_Visibility_of_Profile = (int)(EnVisibleProfile)Enum.Parse(typeof(EnVisibleProfile), item.ConfigValue);
                        break;
                    case EnProfileConfigKey.Min_Number_of_Followers_can_Follow:
                        model.Min_Number_of_Followers_can_Follow = Convert.ToInt16(item.ConfigValue);
                        break;
                    case EnProfileConfigKey.Max_Number_Followers_can_Follow:
                        model.Max_Number_Followers_can_Follow = Convert.ToInt16(item.ConfigValue);
                        break;

                        #region
                        //case EnProfileConfigKey.Min_Balance_Require_in_Follower_Account_to_Follow:
                        //    model.Min_Balance_Require_in_Follower_Account_to_Follow = Convert.ToDecimal(item.ConfigValue);
                        //    break;
                        //case EnProfileConfigKey.Min_Trade_Volume_Requir:
                        //    model.Min_Trade_Volume_Requir = Convert.ToDecimal(item.ConfigValue);
                        //    break;
                        //case EnProfileConfigKey.Min_Trade_Volume_Requir_in_Time:
                        //    model.Min_Trade_Volume_Requir_in_Time = (int)(EnProfileVolumeTime)Enum.Parse(typeof(EnProfileVolumeTime), item.ConfigValue);
                        //    break;
                        //case EnProfileConfigKey.Subscription_Charge_Leader:
                        //    model.Subscription_Charge = Convert.ToDecimal(item.ConfigValue);
                        //    break;
                        //case EnProfileConfigKey.Subscription_Charge_Frequency_Leader:
                        //    model.Subscription_Charge_Frequency = (int)(EnSubscriptionChangeFrequency)Enum.Parse(typeof(EnSubscriptionChangeFrequency), item.ConfigValue);
                        //    break;
                        //case EnProfileConfigKey.Can_Add_Pair_to_Watchlist_Leader:
                        //    model.Can_Add_Pair_to_Watchlist = (int)(EnYesNo)Enum.Parse(typeof(EnYesNo), item.ConfigValue);
                        //    break;
                        //case EnProfileConfigKey.Max_Number_of_Pairs_to_Allow_in_Watchlist_Leader:
                        //    model.Max_Number_of_Pairs_to_Allow_in_Watchlist = Convert.ToInt16(item.ConfigValue);
                        //    break;
                        #endregion
                }

                model.UpdatedDate = (DateTime)item.UpdatedDate;
                userid = (int)item.UpdatedBy;


            }

            var user = await _userManager.FindByIdAsync(userid.ToString());
            if (user != null)
                model.UserName = user?.UserName;
            return model;
        }

        public async Task<FollowerAdminPolicyGetModel> GetFollowerProfileConfiguration()
        {
            var ProfileConfig = _profileConfigurationRepository.Table.Where(i => i.ConfigType == ProfileSocialCongifType.Follower_Admin_Policy.ToString() && i.Status == Convert.ToInt16(ServiceStatus.Active)).OrderByDescending(i => i.CreatedDate).ToList();

            FollowerAdminPolicyGetModel model = new FollowerAdminPolicyGetModel();
            int userid = 0;
            foreach (var item in ProfileConfig)
            {
                switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), item.ConfigKey))
                {
                    case EnProfileConfigKey.Can_Copy_Trade:
                        model.Can_Copy_Trade = (int)(EnYesNo)Enum.Parse(typeof(EnYesNo), item.ConfigValue);
                        break;
                    case EnProfileConfigKey.Can_Mirror_Trade:
                        model.Can_Mirror_Trade = (int)(EnYesNo)Enum.Parse(typeof(EnYesNo), item.ConfigValue);
                        break;
                    case EnProfileConfigKey.Maximum_Number_of_Leaders_to_Allow_Follow:
                        model.Maximum_Number_of_Leaders_to_Allow_Follow = Convert.ToInt16(item.ConfigValue);
                        break;
                    case EnProfileConfigKey.Can_Add_Leader_to_Watchlist_Follower:
                        model.Can_Add_Leader_to_Watchlist = (int)(EnYesNo)Enum.Parse(typeof(EnYesNo), item.ConfigValue);
                        break;
                    case EnProfileConfigKey.Max_Number_of_Leader_to_Allow_in_Watchlist_Follower:
                        model.Max_Number_of_Leader_to_Allow_in_Watchlist = Convert.ToInt16(item.ConfigValue);
                        break;

                        #region
                        //case EnProfileConfigKey.Enable_Auto_Copy_Trade_Functionality:
                        //    model.Enable_Auto_Copy_Trade_Functionality = (int)(EnYesNo)Enum.Parse(typeof(EnYesNo), item.ConfigValue);
                        //    break;
                        //case EnProfileConfigKey.Minimum_Copy_Trade_Percentage:
                        //    model.Minimum_Copy_Trade_Percentage = Convert.ToInt16(item.ConfigValue);
                        //    break;
                        //case EnProfileConfigKey.Default_Copy_Trade_Percentage:
                        //    model.Default_Copy_Trade_Percentage = Convert.ToInt16(item.ConfigValue);
                        //    break;
                        //case EnProfileConfigKey.Maximum_Copy_Trade_Percentage:
                        //    model.Maximum_Copy_Trade_Percentage = Convert.ToInt16(item.ConfigValue);
                        //    break;

                        //case EnProfileConfigKey.Maximum_Transaction_Amount_Limit:
                        //    model.Maximum_Transaction_Amount_Limit = Convert.ToDecimal(item.ConfigValue);
                        //    break;
                        //case EnProfileConfigKey.Maximum_Number_of_Transactions_Limit:
                        //    model.Maximum_Number_of_Transactions_Limit = Convert.ToInt16(item.ConfigValue);
                        //    break;
                        //case EnProfileConfigKey.Subscription_Charge_Follower:
                        //    model.Subscription_Charge = Convert.ToDecimal(item.ConfigValue);
                        //    break;
                        //case EnProfileConfigKey.Subscription_Charge_Frequency_Follower:
                        //    model.Subscription_Charge_Frequency = (int)(EnSubscriptionChangeFrequency)Enum.Parse(typeof(EnSubscriptionChangeFrequency), item.ConfigValue);
                        //    break;
                        #endregion
                }

                model.UpdatedDate = (DateTime)item.UpdatedDate;
                userid = (int)item.UpdatedBy;
            }

            var user = await _userManager.FindByIdAsync(userid.ToString());
            if (user != null)
                model.UserName = user?.UserName;
            return model;
        }

        public GetLeaderFrontPolicyModel GetUserLeaderProfileFrontConfiguration(int UserId = 0) //(LeaderAdminPolicyModel model, string ConfigType = null, int UserId = 0)
        {

            string Qry = "";
            //int userID = 1;
            IQueryable<FrontLeaderProfile> frontLeadersProfile;
            //Qry = @" Select IsNull(pc.Id,0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id,0) as UserProfileConfigId,Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId,0) as UserId from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId ";
            //Qry += " where pc.ConfigType = '" + ProfileSocialCongifType.Leader_Admin_Policy + "' and pc.Status = 1 and (ISNULL(uc.UserID,0) = " + UserId + " or Isnull(uc.UserID,0) = 0)";

            Qry = @" Select IsNull(pc.Id,0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id, 0) as UserProfileConfigId, Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId, 0) as UserId ";
            Qry += " from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId   and ISNULL(uc.UserID, 0) = " + UserId + " And uc.Status = 1 ";
            Qry += " where pc.ConfigType = 'Leader_Admin_Policy' and pc.Status = 1 ";

            frontLeadersProfile = _dbContext.FrontLeaderProfile.FromSql(Qry);
            List<FrontLeaderProfile> Profile = new List<FrontLeaderProfile>();
            Profile = frontLeadersProfile.ToList();

            //var ProfileConfig = _userProfileConfigRepository.Table.Where(i => i.UserId == UserId && !i.IsEnable && !i.IsDeleted).OrderByDescending(i => i.CreatedDate).ToList();
            GetLeaderFrontPolicyModel model = new GetLeaderFrontPolicyModel();
            foreach (var item in Profile)
            {
                //string val = EnProfileConfigKey.Default_Visibility_of_Profile.ToString();
                switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), item.ConfigKey))
                {
                    case EnProfileConfigKey.Default_Visibility_of_Profile:
                        model.Default_Visibility_of_Profile = (int)(EnVisibleProfile)Enum.Parse(typeof(EnVisibleProfile), item.configValue);
                        break;
                    //if (!string.IsNullOrEmpty(item.UserConfigValue))
                    //    //item.ConfigValue = Convert.ToString((EnVisibleProfile)model.Default_Visibility_of_Profile);                        
                    //    model.Default_Visibility_of_Profile = (int)(EnVisibleProfile)Enum.Parse(typeof(EnVisibleProfile), item.UserConfigValue);
                    //else
                    //    model.Default_Visibility_of_Profile = (int)(EnVisibleProfile)Enum.Parse(typeof(EnVisibleProfile), item.configValue);
                    //break;
                    case EnProfileConfigKey.Max_Number_Followers_can_Follow:
                        model.Max_Number_Followers_can_Follow = Convert.ToInt32(item.configValue);
                        break;
                    //if (!string.IsNullOrEmpty(item.UserConfigValue))
                    //    model.Max_Number_Followers_can_Follow = Convert.ToInt32(item.UserConfigValue);
                    //else
                    //    model.Max_Number_Followers_can_Follow = Convert.ToInt32(item.configValue);
                    //break;
                    case EnProfileConfigKey.Min_Number_of_Followers_can_Follow:
                        model.Min_Number_of_Followers_can_Follow = Convert.ToInt32(item.configValue);
                        break;
                        //if (!string.IsNullOrEmpty(item.UserConfigValue))
                        //    model.Min_Number_of_Followers_can_Follow = Convert.ToInt32(item.UserConfigValue);
                        //else
                        //    model.Min_Number_of_Followers_can_Follow = Convert.ToInt32(item.configValue);
                        //break;
                }
            }
            return model;
        }



        public GetLeaderFrontPolicyModel GetUserLeaderProfileConfiguration(int UserId = 0) //(LeaderAdminPolicyModel model, string ConfigType = null, int UserId = 0)
        {

            string Qry = "";
            //int userID = 1;
            IQueryable<FrontLeaderProfile> frontLeadersProfile;
            //Qry = @" Select IsNull(pc.Id,0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id,0) as UserProfileConfigId,Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId,0) as UserId from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId ";
            //Qry += " where pc.ConfigType = '" + ProfileSocialCongifType.Leader_Admin_Policy + "' and pc.Status = 1 and (ISNULL(uc.UserID,0) = " + UserId + " or Isnull(uc.UserID,0) = 0)";

            Qry = @" Select IsNull(pc.Id,0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id, 0) as UserProfileConfigId, Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId, 0) as UserId ";
            Qry += " from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId   and ISNULL(uc.UserID, 0) = " + UserId + " And uc.Status = 1 ";
            Qry += " where pc.ConfigType = 'Leader_Admin_Policy' and pc.Status = 1 ";

            frontLeadersProfile = _dbContext.FrontLeaderProfile.FromSql(Qry);
            List<FrontLeaderProfile> Profile = new List<FrontLeaderProfile>();
            Profile = frontLeadersProfile.ToList();

            //var ProfileConfig = _userProfileConfigRepository.Table.Where(i => i.UserId == UserId && !i.IsEnable && !i.IsDeleted).OrderByDescending(i => i.CreatedDate).ToList();
            GetLeaderFrontPolicyModel model = new GetLeaderFrontPolicyModel();
            foreach (var item in Profile)
            {
                //string val = EnProfileConfigKey.Default_Visibility_of_Profile.ToString();
                switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), item.ConfigKey))
                {
                    case EnProfileConfigKey.Default_Visibility_of_Profile:
                        if (!string.IsNullOrEmpty(item.UserConfigValue))
                            //item.ConfigValue = Convert.ToString((EnVisibleProfile)model.Default_Visibility_of_Profile);                        
                            model.Default_Visibility_of_Profile = (int)(EnVisibleProfile)Enum.Parse(typeof(EnVisibleProfile), item.UserConfigValue);
                        else
                            model.Default_Visibility_of_Profile = (int)(EnVisibleProfile)Enum.Parse(typeof(EnVisibleProfile), item.configValue);
                        break;
                    case EnProfileConfigKey.Max_Number_Followers_can_Follow:
                        if (!string.IsNullOrEmpty(item.UserConfigValue))
                            model.Max_Number_Followers_can_Follow = Convert.ToInt32(item.UserConfigValue);
                        else
                            model.Max_Number_Followers_can_Follow = Convert.ToInt32(item.configValue);
                        break;
                    case EnProfileConfigKey.Min_Number_of_Followers_can_Follow:
                        if (!string.IsNullOrEmpty(item.UserConfigValue))
                            model.Min_Number_of_Followers_can_Follow = Convert.ToInt32(item.UserConfigValue);
                        else
                            model.Min_Number_of_Followers_can_Follow = Convert.ToInt32(item.configValue);
                        break;
                }
            }
            return model;
        }


        public GetLeaderFollowerCountModel GetLeaderFollowNuber(int LeaderId = 0) //(LeaderAdminPolicyModel model, string ConfigType = null, int UserId = 0)
        {
            GetLeaderFollowerCountModel lmodel = new GetLeaderFollowerCountModel();
            lmodel.TotalFollower = _followerMasterRepository.Table.Where(f => f.LeaderId == LeaderId && f.Status == Convert.ToInt16(ServiceStatus.Active)).Count();
            string Qry = "";
            //int userID = 1;
            IQueryable<FrontLeaderProfile> frontLeadersProfile;
            //Qry = @" Select IsNull(pc.Id,0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id,0) as UserProfileConfigId,Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId,0) as UserId from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId ";
            //Qry += " where pc.ConfigType = '" + ProfileSocialCongifType.Leader_Admin_Policy + "' and pc.Status = 1 and (ISNULL(uc.UserID,0) = " + UserId + " or Isnull(uc.UserID,0) = 0)";

            Qry = @" Select IsNull(pc.Id,0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id, 0) as UserProfileConfigId, Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId, 0) as UserId ";
            Qry += " from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId   and ISNULL(uc.UserID, 0) = " + LeaderId + " And uc.Status = 1 ";
            Qry += " where pc.ConfigType = 'Leader_Admin_Policy' and pc.Status = 1 ";

            frontLeadersProfile = _dbContext.FrontLeaderProfile.FromSql(Qry);
            List<FrontLeaderProfile> Profile = new List<FrontLeaderProfile>();
            Profile = frontLeadersProfile.ToList();

            //var ProfileConfig = _userProfileConfigRepository.Table.Where(i => i.UserId == UserId && !i.IsEnable && !i.IsDeleted).OrderByDescending(i => i.CreatedDate).ToList();
            // GetLeaderFrontPolicyModel model = new GetLeaderFrontPolicyModel();
            foreach (var item in Profile)
            {
                //string val = EnProfileConfigKey.Default_Visibility_of_Profile.ToString();
                switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), item.ConfigKey))
                {
                    //case EnProfileConfigKey.Default_Visibility_of_Profile:
                    //    model.Default_Visibility_of_Profile = (int)(EnVisibleProfile)Enum.Parse(typeof(EnVisibleProfile), item.configValue);
                    //    break;
                    ////if (!string.IsNullOrEmpty(item.UserConfigValue))
                    ////    //item.ConfigValue = Convert.ToString((EnVisibleProfile)model.Default_Visibility_of_Profile);                        
                    ////    model.Default_Visibility_of_Profile = (int)(EnVisibleProfile)Enum.Parse(typeof(EnVisibleProfile), item.UserConfigValue);
                    ////else
                    ////    model.Default_Visibility_of_Profile = (int)(EnVisibleProfile)Enum.Parse(typeof(EnVisibleProfile), item.configValue);
                    ////break;
                    case EnProfileConfigKey.Max_Number_Followers_can_Follow:
                        //model.Max_Number_Followers_can_Follow = Convert.ToInt32(item.configValue);
                        //break;
                        if (!string.IsNullOrEmpty(item.UserConfigValue))
                            lmodel.Max_Number_Followers_can_Follow = Convert.ToInt32(item.UserConfigValue);
                        else
                            lmodel.Max_Number_Followers_can_Follow = Convert.ToInt32(item.configValue);
                        break;
                        //case EnProfileConfigKey.Min_Number_of_Followers_can_Follow:
                        //    model.Min_Number_of_Followers_can_Follow = Convert.ToInt32(item.configValue);
                        //    break;
                        //    //if (!string.IsNullOrEmpty(item.UserConfigValue))
                        //    //    model.Min_Number_of_Followers_can_Follow = Convert.ToInt32(item.UserConfigValue);
                        //    //else
                        //    //    model.Min_Number_of_Followers_can_Follow = Convert.ToInt32(item.configValue);
                        //    //break;
                }
            }
            return lmodel;
        }

        public void SetLeaderFrontProfileConfiguration(LeaderFrontPolicyModel model, int UserId = 0)
        {

            try
            {
                string Qry = "";
                //int userID = 1;
                IQueryable<FrontLeaderProfile> frontLeadersProfile;
                //Qry = @" Select IsNull(pc.Id,0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id,0) as UserProfileConfigId,Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId,0) as UserId from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId ";
                //Qry += " where pc.ConfigType = '" + ProfileSocialCongifType.Leader_Admin_Policy + "' and pc.Status = 1 and (ISNULL(uc.UserID,0) = " + UserId + " or Isnull(uc.UserID,0) = 0)"; // and uc.IsEnable = 0 and uc.IsDeleted = 0"; //+ UserId;

                Qry = @" Select IsNull(pc.Id,0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id, 0) as UserProfileConfigId, Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId, 0) as UserId ";
                Qry += " from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId   and ISNULL(uc.UserID, 0) = " + UserId + " And uc.Status = 1 ";
                Qry += " where pc.ConfigType = 'Leader_Admin_Policy' and pc.Status = 1 ";


                frontLeadersProfile = _dbContext.FrontLeaderProfile.FromSql(Qry);
                List<FrontLeaderProfile> ProfileConfig = new List<FrontLeaderProfile>();
                ProfileConfig = frontLeadersProfile.ToList();

                foreach (var item in ProfileConfig)
                {
                    bool datastatus = false;
                    //string val = EnProfileConfigKey.Default_Visibility_of_Profile.ToString();
                    switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), item.ConfigKey))
                    {
                        case EnProfileConfigKey.Default_Visibility_of_Profile:
                            item.UserConfigValue = Convert.ToString((EnVisibleProfile)model.Default_Visibility_of_Profile);
                            datastatus = true;
                            break;
                        case EnProfileConfigKey.Max_Number_Followers_can_Follow:
                            item.UserConfigValue = model.Max_Number_Followers_can_Follow.ToString();
                            datastatus = true;
                            break;
                    }

                    if (datastatus)
                    {
                        if (item.UserProfileConfigId == 0)
                        {
                            var userprofile = new UserProfileConfig
                            {
                                UserId = UserId,
                                //LeaderId = 0,
                                ProfileConfigId = item.ConfigId,
                                ConfigValue = item.UserConfigValue,
                                //IsEnable = false,
                                // IsDeleted = false,
                                CreatedBy = Convert.ToInt32(UserId),
                                CreatedDate = DateTime.UtcNow,
                                Status = Convert.ToInt16(ServiceStatus.Active),
                            };
                            _userProfileConfigRepository.Insert(userprofile);
                        }
                        else
                        {
                            var userConfig = _userProfileConfigRepository.Table.Where(u => u.Id == item.UserProfileConfigId && u.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                            if (userConfig != null)
                            {
                                //UserProfileConfig uprofile = new UserProfileConfig();
                                userConfig.Id = item.UserProfileConfigId;
                                //userConfig.CreatedBy = userConfig.CreatedBy;
                                //userConfig.CreatedDate = userConfig.CreatedDate;
                                userConfig.UpdatedBy = UserId;
                                userConfig.UpdatedDate = DateTime.UtcNow;
                                userConfig.Status = Convert.ToInt16(ServiceStatus.Active);
                                userConfig.UserId = UserId;
                                userConfig.ProfileConfigId = item.ConfigId;
                                userConfig.ConfigValue = item.UserConfigValue;
                                userConfig.LeaderId = userConfig.LeaderId;
                                _userProfileConfigRepository.Update(userConfig);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public void UnSubscribeLeaderFrontProfileConfiguration(int UserId = 0, long ProfileId = 0)
        {


            var profile = _profileRepository.Table.Where(p => p.Id == ProfileId && p.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
            if (profile != null)
            {
                // subscription.
                if (profile.LevelName == ProfileSocialCongifType.Leader.ToString())
                {
                    //string Qry = "";
                    ////int userID = 1;
                    //IQueryable<FrontLeaderProfile> frontLeadersProfile;
                    ////Qry = @" Select IsNull(pc.Id,0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id,0) as UserProfileConfigId,Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId,0) as UserId from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId ";
                    ////Qry += " where pc.ConfigType = '" + ProfileSocialCongifType.Leader_Admin_Policy + "' and pc.Status = 1 and (ISNULL(uc.UserID,0) = " + UserId + " or Isnull(uc.UserID,0) = 0)"; // and uc.IsEnable = 0 and uc.IsDeleted = 0"; //+ UserId;

                    //Qry = @" Select IsNull(pc.Id,0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id, 0) as UserProfileConfigId, Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId, 0) as UserId ";
                    //Qry += " from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId   and ISNULL(uc.UserID, 0) = " + UserId + " ";
                    //Qry += " where pc.ConfigType = 'Leader_Admin_Policy' and pc.Status = 1 ";


                    //frontLeadersProfile = _dbContext.FrontLeaderProfile.FromSql(Qry);
                    //List<FrontLeaderProfile> ProfileConfig = new List<FrontLeaderProfile>();
                    //ProfileConfig = frontLeadersProfile.ToList();

                    //foreach (var item in ProfileConfig)
                    //{
                    //    //bool datastatus = false;
                    //    ////string val = EnProfileConfigKey.Default_Visibility_of_Profile.ToString();
                    //    //switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), item.ConfigKey))
                    //    //{
                    //    //    case EnProfileConfigKey.Default_Visibility_of_Profile:
                    //    //        item.UserConfigValue = Convert.ToString((EnVisibleProfile)model.Default_Visibility_of_Profile);
                    //    //        datastatus = true;
                    //    //        break;
                    //    //    case EnProfileConfigKey.Max_Number_Followers_can_Follow:
                    //    //        item.UserConfigValue = model.Max_Number_Followers_can_Follow.ToString();
                    //    //        datastatus = true;
                    //    //        break;
                    //    //}

                    //    var userProfileConfig = _userProfileConfigRepository.GetById(item.UserProfileConfigId);
                    //    userProfileConfig.SetUnDeleteStatus();
                    //    _userProfileConfigRepository.Update(userProfileConfig);



                    //    ////if (datastatus)
                    //    ////{
                    //    //if (item.UserProfileConfigId == 0)
                    //    //    {
                    //    //        var userprofile = new UserProfileConfig
                    //    //        {


                    //    //            UserId = UserId,
                    //    //            //LeaderId = 0,
                    //    //            ProfileConfigId = item.ConfigId,
                    //    //            ConfigValue = item.UserConfigValue,
                    //    //            //IsEnable = false,
                    //    //            // IsDeleted = false,
                    //    //            CreatedBy = Convert.ToInt32(UserId),
                    //    //            CreatedDate = DateTime.UtcNow,
                    //    //            Status = Convert.ToInt16(ServiceStatus.Active),
                    //    //        };
                    //    //        _userProfileConfigRepository.Insert(userprofile);
                    //    //    }
                    //    //    else
                    //    //    {
                    //    //        UserProfileConfig uprofile = new UserProfileConfig();
                    //    //        uprofile.Id = item.UserProfileConfigId;
                    //    //        uprofile.UserId = UserId;
                    //    //        //uprofile.LeaderId = 0;
                    //    //        uprofile.ProfileConfigId = item.ConfigId;
                    //    //        uprofile.ConfigValue = item.UserConfigValue;
                    //    //        //uprofile.IsEnable = false;
                    //    //        //uprofile.IsDeleted = false;                       
                    //    //        uprofile.UpdatedBy = UserId;
                    //    //        uprofile.UpdatedDate = DateTime.UtcNow;
                    //    //        _userProfileConfigRepository.Update(uprofile);
                    //    //    }
                    //    // }

                    //}

                    var followerlist = _followerMasterRepository.Table.Where(f => f.LeaderId == Convert.ToUInt32(UserId) && f.Status == Convert.ToInt16(ServiceStatus.Active)).ToList();
                    foreach (var fItem in followerlist)
                    {
                        var followMaster = _followerMasterRepository.GetById(fItem.Id);
                        followMaster.SetInActiveStatus();
                        _followerMasterRepository.Update(followMaster);
                    }


                    ///// User config 
                    var userConfig = _userProfileConfigRepository.Table.Where(u => u.Status == Convert.ToInt16(ServiceStatus.Active) && (u.UserId == UserId || u.LeaderId == UserId)).ToList();
                    foreach (var item in userConfig)
                    {
                        var userProfileConfig = _userProfileConfigRepository.GetById(item.Id);
                        userProfileConfig.SetUnDeleteStatus();
                        _userProfileConfigRepository.Update(userProfileConfig);
                    }
                }
                else if (profile.LevelName == ProfileSocialCongifType.Follower.ToString())
                {
                    var followerlist = _followerMasterRepository.Table.Where(f => f.FolowerId == Convert.ToUInt32(UserId) && f.Status == Convert.ToInt16(ServiceStatus.Active)).ToList();
                    foreach (var fItem in followerlist)
                    {
                        var followMaster = _followerMasterRepository.GetById(fItem.Id);
                        followMaster.SetInActiveStatus();
                        _followerMasterRepository.Update(followMaster);
                    }

                    ///// User config 
                    var userConfig = _userProfileConfigRepository.Table.Where(u => u.UserId == UserId && u.Status == Convert.ToInt16(ServiceStatus.Active)).ToList();
                    foreach (var item in userConfig)
                    {
                        var userProfileConfig = _userProfileConfigRepository.GetById(item.Id);
                        userProfileConfig.SetUnDeleteStatus();
                        _userProfileConfigRepository.Update(userProfileConfig);
                    }
                }

            }
        }

        public FollowerServiceFrontModel GetFollowerProfileFrontConfiguration(int UserId = 0, string Leaderid = null) //(LeaderAdminPolicyModel model, string ConfigType = null, int UserId = 0)
        {
            string Qry = "";
            //int userID = 1;
            IQueryable<FrontLeaderProfile> frontLeadersProfile;
            //Qry = @" Select IsNull(pc.Id,0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id,0) as UserProfileConfigId,Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId,0) as UserId from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId ";
            //Qry += " where pc.ConfigType = '" + ProfileSocialCongifType.Follower_Admin_Policy + "' and pc.IsEnable = 0 and pc.IsDeleted = 0 and (ISNULL(uc.UserID,0) = " + UserId + " or Isnull(uc.UserID,0) = 0)"; // and uc.IsEnable = 0 and uc.IsDeleted = 0"; //+ UserId;
            Qry = @" Select IsNull(pc.Id, 0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id, 0) as UserProfileConfigId,Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId, 0) as UserId "; //,Isnull(uc.LeaderId, 0) as LeaderId ";
            Qry += " from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId and uc.UserID = " + UserId + " and uc.LeaderId in (" + Leaderid + ") and uc.Status=1 ";
            Qry += " where pc.ConfigType = '" + ProfileSocialCongifType.Follower_Admin_Policy + "' and pc.Status = 1";

            frontLeadersProfile = _dbContext.FrontLeaderProfile.FromSql(Qry);
            List<FrontLeaderProfile> Profile = new List<FrontLeaderProfile>();
            Profile = frontLeadersProfile.ToList();

            //var ProfileConfig = _userProfileConfigRepository.Table.Where(i => i.UserId == UserId && !i.IsEnable && !i.IsDeleted).OrderByDescending(i => i.CreatedDate).ToList();
            FollowerServiceFrontModel model = new FollowerServiceFrontModel();
            foreach (var item in Profile)
            {
                switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), item.ConfigKey))
                {
                    case EnProfileConfigKey.Can_Copy_Trade:
                        if (!string.IsNullOrEmpty(item.UserConfigValue))
                            model.Can_Copy_Trade = (int)(EnYesNo)Enum.Parse(typeof(EnYesNo), item.UserConfigValue);
                        else
                            model.Can_Copy_Trade = (int)(EnYesNo)Enum.Parse(typeof(EnYesNo), item.configValue);
                        break;
                    case EnProfileConfigKey.Can_Mirror_Trade:
                        if (!string.IsNullOrEmpty(item.UserConfigValue))
                            model.Can_Mirror_Trade = (int)(EnYesNo)Enum.Parse(typeof(EnYesNo), item.UserConfigValue);
                        else
                            model.Can_Mirror_Trade = (int)(EnYesNo)Enum.Parse(typeof(EnYesNo), item.configValue);
                        break;

                    case EnProfileConfigKey.Default_Copy_Trade_Percentage:
                        if (!string.IsNullOrEmpty(item.UserConfigValue))
                            model.Default_Copy_Trade_Percentage = Convert.ToDecimal(item.UserConfigValue);
                        else
                            model.Default_Copy_Trade_Percentage = Convert.ToDecimal(item.configValue);
                        break;

                        /*
                        //case EnProfileConfigKey.Enable_Auto_Copy_Trade_Functionality:
                        //    if (!string.IsNullOrEmpty(item.UserConfigValue))
                        //        model.Enable_Auto_Copy_Trade_Functionality = (int)(EnYesNo)Enum.Parse(typeof(EnYesNo), item.UserConfigValue);
                        //    else
                        //        model.Enable_Auto_Copy_Trade_Functionality = (int)(EnYesNo)Enum.Parse(typeof(EnYesNo), item.configValue);
                        //    break;
                        //case EnProfileConfigKey.Maximum_Copy_Trade_Percentage:
                        //    if (!string.IsNullOrEmpty(item.UserConfigValue))
                        //        model.Maximum_Copy_Trade_Percentage = Convert.ToInt16(item.UserConfigValue);
                        //    else
                        //        model.Maximum_Copy_Trade_Percentage = Convert.ToInt16(item.configValue);
                        //    break;
                        //case EnProfileConfigKey.Maximum_Transaction_Amount_Limit:
                        //    if (!string.IsNullOrEmpty(item.UserConfigValue))
                        //        model.Maximum_Transaction_Amount_Limit = Convert.ToDecimal(item.UserConfigValue);
                        //    else
                        //        model.Maximum_Transaction_Amount_Limit = Convert.ToDecimal(item.configValue);
                        //    break;
                        //case EnProfileConfigKey.Maximum_Number_of_Transactions_Limit:
                        //    if (!string.IsNullOrEmpty(item.UserConfigValue))
                        //        model.Maximum_Number_of_Transactions_Limit = Convert.ToInt16(item.UserConfigValue);
                        //    else
                        //        model.Maximum_Number_of_Transactions_Limit = Convert.ToInt16(item.configValue);
                        //    break;
                        */
                }
            }
            return model;
        }

        public void SetFollowerFrontProfileConfiguration(FollowerFrontModel model, int UserId = 0)
        {
            try
            {
                string Qry = "";
                //int userID = 1;

                string[] LeaderId = model.LeaderId.Split(",");

                foreach (var Itemleaderid in LeaderId)

                {
                    IQueryable<FrontLeaderProfile> frontLeadersProfile;
                    //Qry = @" Select IsNull(pc.Id,0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id,0) as UserProfileConfigId,Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId,0) as UserId from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId ";
                    //Qry += " where pc.ConfigType = '" + ProfileSocialCongifType.Follower_Admin_Policy + "' and pc.IsEnable = 0 and pc.IsDeleted = 0 and (ISNULL(uc.UserID,0) = " + UserId + " or Isnull(uc.UserID,0) = 0)"; // and uc.IsEnable = 0 and uc.IsDeleted = 0"; //+ UserId;

                    Qry = @" Select IsNull(pc.Id, 0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id, 0) as UserProfileConfigId,Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId, 0) as UserId ";
                    Qry += " from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId and uc.UserID = " + UserId + " and uc.LeaderId in (" + Itemleaderid + ") ";
                    Qry += " where pc.ConfigType = '" + ProfileSocialCongifType.Follower_Admin_Policy + "' and pc.Status = 1";



                    frontLeadersProfile = _dbContext.FrontLeaderProfile.FromSql(Qry);
                    List<FrontLeaderProfile> ProfileConfig = new List<FrontLeaderProfile>();
                    ProfileConfig = frontLeadersProfile.ToList();

                    foreach (var item in ProfileConfig)
                    {
                        bool datastatus = false;
                        //string val = EnProfileConfigKey.Default_Visibility_of_Profile.ToString();
                        switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), item.ConfigKey))
                        {
                            case EnProfileConfigKey.Can_Copy_Trade:
                                if (Enum.IsDefined(typeof(EnYesNo), model.Can_Copy_Trade))
                                {
                                    item.UserConfigValue = Convert.ToString((EnYesNo)model.Can_Copy_Trade);
                                    datastatus = true;
                                }
                                break;
                            case EnProfileConfigKey.Can_Mirror_Trade:
                                if (Enum.IsDefined(typeof(EnYesNo), model.Can_Mirror_Trade))
                                {
                                    item.UserConfigValue = Convert.ToString((EnYesNo)model.Can_Mirror_Trade);
                                    datastatus = true;
                                }
                                break;

                            case EnProfileConfigKey.Default_Copy_Trade_Percentage:
                                item.UserConfigValue = model.Trade_Percentage.ToString();
                                datastatus = true;
                                break;

                                #region "Comment"
                                //case EnProfileConfigKey.Enable_Auto_Copy_Trade_Functionality:
                                //    item.UserConfigValue = Convert.ToString((EnYesNo)model.Enable_Auto_Copy_Trade_Functionality);
                                //    datastatus = true;
                                //    break;
                                //case EnProfileConfigKey.Default_Copy_Trade_Percentage:
                                //    item.UserConfigValue = model.Default_Copy_Trade_Percentage.ToString();
                                //    datastatus = true;
                                //    break;
                                //case EnProfileConfigKey.Maximum_Copy_Trade_Percentage:
                                //    item.UserConfigValue = model.Maximum_Copy_Trade_Percentage.ToString();
                                //    datastatus = true;
                                //    break;
                                //case EnProfileConfigKey.Maximum_Transaction_Amount_Limit:
                                //    item.UserConfigValue = model.Maximum_Transaction_Amount_Limit.ToString();
                                //    datastatus = true;
                                //    break;
                                //case EnProfileConfigKey.Maximum_Number_of_Transactions_Limit:
                                //    item.UserConfigValue = model.Maximum_Number_of_Transactions_Limit.ToString();
                                //    datastatus = true;
                                //    break;
                                #endregion
                        }


                        if (datastatus)
                        {
                            if (item.UserProfileConfigId == 0)
                            {
                                var userprofile = new UserProfileConfig
                                {
                                    UserId = UserId,
                                    ProfileConfigId = item.ConfigId,
                                    ConfigValue = item.UserConfigValue,
                                    //IsEnable = false,
                                    //IsDeleted = false,
                                    CreatedBy = Convert.ToInt32(UserId),
                                    CreatedDate = DateTime.UtcNow,
                                    Status = Convert.ToInt16(ServiceStatus.Active),
                                    LeaderId = Convert.ToUInt32(Itemleaderid)
                                };
                                _userProfileConfigRepository.Insert(userprofile);
                            }
                            else
                            {
                                ////var uprofile = _userProfileConfigRepository.GetById(item.UserProfileConfigId);
                                ////uprofile.SetDisableStatus();
                                ////_userProfileConfigRepository.Update(uprofile);

                                //UserProfileConfig uprofile = new UserProfileConfig();
                                //uprofile.Id = item.UserProfileConfigId;
                                //uprofile.UserId = UserId;
                                //uprofile.ProfileConfigId = item.ConfigId;
                                //uprofile.ConfigValue = item.UserConfigValue;
                                //// uprofile.IsEnable = false;
                                //// uprofile.IsDeleted = false;
                                //uprofile.UpdatedBy = UserId;
                                //uprofile.UpdatedDate = DateTime.UtcNow;
                                //uprofile.Status = Convert.ToInt16(ServiceStatus.Active);
                                //uprofile.LeaderId = Convert.ToUInt32(Itemleaderid);
                                //_userProfileConfigRepository.Update(uprofile);


                                var userConfig = _userProfileConfigRepository.Table.Where(u => u.Id == item.UserProfileConfigId && u.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                                if (userConfig != null)
                                {
                                    //UserProfileConfig uprofile = new UserProfileConfig();
                                    userConfig.Id = item.UserProfileConfigId;
                                    //userConfig.CreatedBy = userConfig.CreatedBy;
                                    //userConfig.CreatedDate = userConfig.CreatedDate;
                                    userConfig.UpdatedBy = UserId;
                                    userConfig.UpdatedDate = DateTime.UtcNow;
                                    userConfig.Status = Convert.ToInt16(ServiceStatus.Active);
                                    userConfig.UserId = UserId;
                                    userConfig.ProfileConfigId = item.ConfigId;
                                    userConfig.ConfigValue = item.UserConfigValue;
                                    userConfig.LeaderId = userConfig.LeaderId;
                                    _userProfileConfigRepository.Update(userConfig);
                                }
                            }
                        }
                    }

                    var followerlist = _followerMasterRepository.Table.Where(f => f.LeaderId == Convert.ToUInt32(Itemleaderid) && f.FolowerId == Convert.ToInt32(UserId) && f.Status == Convert.ToInt16(ServiceStatus.Active)).ToList();
                    if (followerlist.Count() == 0)
                    {
                        FollowerMaster fmmodel = new FollowerMaster();
                        var FmModel = new FollowerMaster
                        {
                            CreatedBy = Convert.ToInt32(UserId),
                            CreatedDate = DateTime.UtcNow,
                            LeaderId = Convert.ToUInt32(Itemleaderid),
                            FolowerId = Convert.ToInt32(UserId),
                            Status = Convert.ToInt16(ServiceStatus.Active),
                            FllowerStatus = false,
                        };
                        _followerMasterRepository.Insert(FmModel);
                    }
                    else
                    {
                        var followMaster = _followerMasterRepository.GetById(followerlist.FirstOrDefault().Id);
                        followMaster.SetFollowStatus();
                        _followerMasterRepository.Update(followMaster);
                    }

                    var group = _groupMasterService.GetGroupbyUserId(UserId);
                    if (group == null)
                    {
                        GroupMasterViewModel gmodel = new GroupMasterViewModel();
                        gmodel.GroupName = EnmGroupName.MyGroup.ToString();
                        _groupMasterService.AddGroup(gmodel, UserId);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public LeaderListWithGroupModel GetFrontLeaderList(int pageIndex = 0, int pageSize = 0, int userId = 0)
        {
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            if (pageSize == 0)
            {
                pageSize = 10;
            }

            var followerList = _dbContext.FollowerMaster.Where(f => f.FolowerId == userId && f.Status == Convert.ToInt16(ServiceStatus.Active) && !f.FllowerStatus).ToList();

            var WatchList = _dbContext.WatchMaster.Where(w => w.WatcherId == userId && w.Status == Convert.ToInt16(ServiceStatus.Active)).ToList();  // added by nirav savariya for get user wise watch list data on 25-01-2019

            List<LeaderModel> LeaderList = (dynamic)null;
            LeaderList = _dbContext.FrontGetLeaderList.FromSql("dbo.Sp_GetLeaderList @PageIndex={0},@PageSize = {1}", pageIndex, pageSize).ToList();
            List<GetLeaderWithWatchModel> Listmodel = new List<GetLeaderWithWatchModel>();
            int totalCount = 0;
            foreach (var item in LeaderList)
            {
                GetLeaderWithWatchModel model = new GetLeaderWithWatchModel();
                totalCount = item.RowsCount;
                // model.RowNumber = item.RowNumber;
                model.LeaderId = item.LeaderId;
                model.LeaderName = item.UserName;
                model.NoOfFollowerFollow = item.NoOfFollowerFollow;
                //  model.MaxFollowerKey = item.MaxFollowerKey;
                //   model.adminFollowerLimit = Convert.ToInt32(item.adminFollowerLimit);
                //   model.UserMaxLimit = Convert.ToInt32(item.UserMaxLimit);
                //   model.DefaultVisibilityKey = item.DefaultVisibilityKey;
                //   model.AdminDefaultVisibility = item.AdminDefaultVisibility;
                if (!string.IsNullOrEmpty(item.UserDefaultVisible))
                    model.UserDefaultVisible = item.UserDefaultVisible;
                else
                    model.UserDefaultVisible = item.AdminDefaultVisibility;


                var lederID = followerList.Where(f => f.LeaderId == item.LeaderId).FirstOrDefault();
                if (lederID != null)
                    model.IsFollow = true;
                else
                    model.IsFollow = false;
                var Watcher = WatchList.Where(a => a.LeaderId == item.LeaderId).ToList();
                long[] GroupId = new long[Watcher.Count()];
                if (Watcher.Count > 0)
                {
                    model.IsWatcher = true;
                    for (int i = 0; i < Watcher.Count(); i++)
                    {
                       GroupId[i] = Watcher[i].GroupId;
                    }
                }
                else
                    model.IsWatcher = false;

                model.GroupId = GroupId;

                Listmodel.Add(model);
            }
            LeaderListWithGroupModel lmodel = new LeaderListWithGroupModel();
            lmodel.TotalCount = totalCount;
            lmodel.LeaderList = Listmodel;

            return lmodel;
        }

        public LeaderListModel GetLeaderListByFollowerId(int pageIndex = 0, int pageSize = 0, int userId = 0)
        {
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            if (pageSize == 0)
            {
                pageSize = 10;
            }

            var followerList = _dbContext.FollowerMaster.Where(f => f.FolowerId == userId && f.Status == Convert.ToInt16(ServiceStatus.Active) && !f.FllowerStatus).ToList();

            var WatchList = _dbContext.WatchMaster.Where(w => w.WatcherId == userId && w.Status == Convert.ToInt16(ServiceStatus.Active)).ToList();  // added by nirav savariya for get user wise watch list data on 25-01-2019


            List<LeaderModel> LeaderList = (dynamic)null;
            LeaderList = _dbContext.FollowerWiseLeaders.FromSql("dbo.Sp_GetLeaderList_FollowerWise @FolloWerId = {0},@PageIndex = {1},@PageSize = {2}", userId, pageIndex, pageSize).ToList();
            List<GetLeaderModel> Listmodel = new List<GetLeaderModel>();
            int totalCount = 0;
            foreach (var item in LeaderList)
            {
                GetLeaderModel model = new GetLeaderModel();
                totalCount = item.RowsCount;
                // model.RowNumber = item.RowNumber;
                model.LeaderId = item.LeaderId;
                //model.FollowerId = item.FollowerId;
                model.LeaderName = item.UserName;
                model.NoOfFollowerFollow = item.NoOfFollowerFollow;
                //  model.MaxFollowerKey = item.MaxFollowerKey;
                //   model.adminFollowerLimit = Convert.ToInt32(item.adminFollowerLimit);
                //   model.UserMaxLimit = Convert.ToInt32(item.UserMaxLimit);
                //   model.DefaultVisibilityKey = item.DefaultVisibilityKey;
                //   model.AdminDefaultVisibility = item.AdminDefaultVisibility;
                if (!string.IsNullOrEmpty(item.UserDefaultVisible))
                    model.UserDefaultVisible = item.UserDefaultVisible;
                else
                    model.UserDefaultVisible = item.AdminDefaultVisibility;


                var lederID = followerList.Where(f => f.LeaderId == item.LeaderId).FirstOrDefault();
                if (lederID != null)
                    model.IsFollow = true;
                else
                    model.IsFollow = false;

                var WatchId = WatchList.Where(a => a.LeaderId == item.LeaderId).FirstOrDefault();
                if (WatchId != null)
                    model.IsWatcher = true;
                else
                    model.IsWatcher = false;
                Listmodel.Add(model);
            }
            LeaderListModel lmodel = new LeaderListModel();
            lmodel.TotalCount = totalCount;
            lmodel.LeaderList = Listmodel;

            return lmodel;
        }

        public LeaderwiseFollower GetLeaderWiseFollowerCongfigList(long LeaderId = 0)
        {
            try
            {


                //int LeaderId = 0;
                string Qry = "";
                IQueryable<GetProfileType> ProfileType;
                Qry = @" select bu.Id,case When pm.LevelName = 'Leader' then cast('true' as bit) When pm.LevelName = 'Follower' then cast('False' as bit) End as ProfileType from bizuser bu ";
                Qry += " inner join SubscriptionMaster sm on bu.id = sm.UserId and sm.Status = 1 ";
                Qry += " inner join profilemaster pm on sm.profileId = pm.Id and pm.Id in (Select Id from profilemaster where Typeid in (Select Id from typemaster where type = 'SocialProfile' )) ";
                Qry += " Where bu.Id = " + LeaderId;

                ProfileType = _dbContext.ProfileType.FromSql(Qry);
                GetProfileType getprofiletype = new GetProfileType();
                getprofiletype = ProfileType.ToList().FirstOrDefault();


                string Qry_Follower = "";
                IQueryable<FollowerList> followeList;
                Qry_Follower = @" Select Id, LeaderId, FolowerId from followermaster where LeaderId =" + LeaderId + " And Status=1";

                followeList = _dbContext.FollowerList.FromSql(Qry_Follower);
                List<FollowerList> listfollower = new List<FollowerList>();
                listfollower = followeList.ToList();

                string Qry_Follower_Config = "";
                IQueryable<FollowerConfigList> followeCongifgList;
                Qry_Follower_Config = @" Select bu.UserName,bu.Email,bu.Mobile, pc.Id,pc.ConfigKey,pc.ConfigValue,uc.ConfigValue as userconfig, uc.UserId as follower, uc.LeaderId as Leader ";
                Qry_Follower_Config += " from ProfileConfiguration pc inner join UserProfileConfig uc on pc.Id= uc.ProfileConfigId and pc.Status = 1 and uc.Status=1 ";
                Qry_Follower_Config += " and pc.ConfigType = 'Follower_Admin_Policy' and uc.LeaderId =" + LeaderId;
                Qry_Follower_Config += " inner join bizuser bu on uc.UserId = bu.Id order by uc.Id,pc.Id";

                followeCongifgList = _dbContext.FollowerConfigList.FromSql(Qry_Follower_Config);
                List<FollowerConfigList> followerconfig = new List<FollowerConfigList>();
                followerconfig = followeCongifgList.ToList();



                LeaderwiseFollower model = new LeaderwiseFollower();

                if (getprofiletype != null)
                {
                    model.ProfileType = getprofiletype.ProfileType;
                    List<ConfigListFollower> cmodelList = new List<ConfigListFollower>();
                    foreach (var fitem in listfollower)
                    {
                        var userFollowerConfig = followerconfig.Where(fc => fc.Follower == fitem.FolowerId && (fc.ConfigKey == EnProfileConfigKey.Can_Copy_Trade.ToString() || fc.ConfigKey == EnProfileConfigKey.Can_Mirror_Trade.ToString() || fc.ConfigKey == EnProfileConfigKey.Default_Copy_Trade_Percentage.ToString())).ToList();
                        if (userFollowerConfig.Count > 0)
                        {
                            ConfigListFollower cmodel = new ConfigListFollower();
                            foreach (var citem in userFollowerConfig)
                            {
                                cmodel.Id = citem.Follower;
                                cmodel.UserName = citem.UserName;
                                cmodel.Mobile = citem.Mobile;
                                if (EnProfileConfigKey.Can_Copy_Trade.ToString() == citem.ConfigKey)
                                {
                                    if (citem.userconfig == "Yes")
                                    {
                                        cmodel.ConfigKey = citem.ConfigKey;
                                        cmodel.userconfig = citem.userconfig;
                                    }
                                }
                                if (EnProfileConfigKey.Can_Mirror_Trade.ToString() == citem.ConfigKey)
                                {
                                    if (citem.userconfig == "Yes")
                                    {
                                        cmodel.ConfigKey = citem.ConfigKey;
                                        cmodel.userconfig = citem.userconfig;
                                    }
                                }

                                if (EnProfileConfigKey.Default_Copy_Trade_Percentage.ToString() == citem.ConfigKey)
                                {
                                    if (!string.IsNullOrEmpty(citem.userconfig))
                                        cmodel.TradePercentage = Convert.ToDecimal(citem.userconfig);
                                    else
                                        cmodel.TradePercentage = Convert.ToDecimal(citem.ConfigValue);
                                }
                            }
                            cmodelList.Add(cmodel);
                        }
                    }
                    model.FollowerList = cmodelList;

                }
                return model;
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog("GetLeaderWiseFollowerCongfigList LeaderId:" + LeaderId, "ProfileConfigurationService", ex);
                return null;
            }           
        }

        public LeaderwiseFollowersList GetLeaderWiseFollowers(long LeaderId = 0, int pageIndex = 0, int pageSize = 0)
        {
            ////int LeaderId = 0;
            //string Qry = "";
            //IQueryable<GetProfileType> ProfileType;
            //Qry = @" select bu.Id,case When pm.LevelName = 'Leader' then cast('true' as bit) When pm.LevelName = 'Follower' then cast('False' as bit) End as ProfileType from bizuser bu ";
            //Qry += " inner join SubscriptionMaster sm on bu.id = sm.UserId and sm.Status = 1 ";
            //Qry += " inner join profilemaster pm on sm.profileId = pm.Id and pm.Id in (Select Id from profilemaster where Typeid in (Select Id from typemaster where type = 'SocialProfile' )) ";
            //Qry += " Where bu.Id = " + LeaderId;

            //ProfileType = _dbContext.ProfileType.FromSql(Qry);
            //GetProfileType getprofiletype = new GetProfileType();
            //getprofiletype = ProfileType.ToList().FirstOrDefault();


            string Qry_Follower = "";
            IQueryable<FollowerList> followeList;
            Qry_Follower = @" Select Id, LeaderId, FolowerId from followermaster where LeaderId =" + LeaderId + " And Status=1";

            followeList = _dbContext.FollowerList.FromSql(Qry_Follower);
            List<FollowerList> listfollower = new List<FollowerList>();
            listfollower = followeList.ToList();

            string Qry_Follower_Config = "";
            IQueryable<FollowerConfigList> followeCongifgList;
            Qry_Follower_Config = @" Select bu.UserName,bu.Email,bu.Mobile, pc.Id,pc.ConfigKey,pc.ConfigValue,uc.ConfigValue as userconfig, uc.UserId as follower, uc.LeaderId as Leader ";
            Qry_Follower_Config += " from ProfileConfiguration pc inner join UserProfileConfig uc on pc.Id= uc.ProfileConfigId and pc.Status = 1 and uc.Status=1 ";
            Qry_Follower_Config += " and pc.ConfigType = 'Follower_Admin_Policy' and uc.LeaderId =" + LeaderId;
            Qry_Follower_Config += " inner join bizuser bu on uc.UserId = bu.Id order by uc.Id,pc.Id";

            followeCongifgList = _dbContext.FollowerConfigList.FromSql(Qry_Follower_Config);
            List<FollowerConfigList> followerconfig = new List<FollowerConfigList>();
            followerconfig = followeCongifgList.ToList();



            LeaderwiseFollowersList model = new LeaderwiseFollowersList();

            //if (getprofiletype != null)
            //{
            //    model.ProfileType = getprofiletype.ProfileType;
            List<ConfigListFollower> cmodelList = new List<ConfigListFollower>();
            foreach (var fitem in listfollower)
            {
                var userFollowerConfig = followerconfig.Where(fc => fc.Follower == fitem.FolowerId && (fc.ConfigKey == EnProfileConfigKey.Can_Copy_Trade.ToString() || fc.ConfigKey == EnProfileConfigKey.Can_Mirror_Trade.ToString() || fc.ConfigKey == EnProfileConfigKey.Default_Copy_Trade_Percentage.ToString())).ToList();
                if (userFollowerConfig.Count > 0)
                {
                    ConfigListFollower cmodel = new ConfigListFollower();

                    foreach (var citem in userFollowerConfig)
                    {
                        cmodel.Id = citem.Follower;
                        cmodel.UserName = citem.UserName;
                        cmodel.Mobile = citem.Mobile;
                        if (EnProfileConfigKey.Can_Copy_Trade.ToString() == citem.ConfigKey)
                        {
                            if (citem.userconfig == "Yes")
                            {
                                cmodel.ConfigKey = citem.ConfigKey;
                                cmodel.userconfig = citem.userconfig;
                            }
                        }
                        if (EnProfileConfigKey.Can_Mirror_Trade.ToString() == citem.ConfigKey)
                        {
                            if (citem.userconfig == "Yes")
                            {
                                cmodel.ConfigKey = citem.ConfigKey;
                                cmodel.userconfig = citem.userconfig;
                            }
                        }

                        if (EnProfileConfigKey.Default_Copy_Trade_Percentage.ToString() == citem.ConfigKey)
                        {
                            if (!string.IsNullOrEmpty(citem.userconfig))
                                cmodel.TradePercentage = Convert.ToDecimal(citem.userconfig);
                            else
                                cmodel.TradePercentage = Convert.ToDecimal(citem.ConfigValue);
                        }
                    }
                    cmodelList.Add(cmodel);
                }
            }
            model.FollowerList = cmodelList;

            model.Totalcount = cmodelList.Count();
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            if (pageSize == 0)
            {
                pageSize = 10;
            }

            var skip = pageSize * (pageIndex - 1);

            model.FollowerList = cmodelList.Skip(skip).Take(pageSize).ToList();

            return model;
        }

        public void Unfollowleader(long FollowerId = 0, string LeaderID = null)
        {
            try
            {
                //string Qry = "";//komal 03 May 2019, Cleanup
                //int userID = 1;
                string[] arryLeaderId = LeaderID.Split(",");

                //bool Status = false;
                var Follow = _followerMasterRepository.Table.Where(f => f.FolowerId == FollowerId && arryLeaderId.Contains(f.LeaderId.ToString()) && !f.FllowerStatus && f.Status == Convert.ToInt16(ServiceStatus.Active)).ToList();
                foreach (var item in Follow)
                {
                    var followMaster = _followerMasterRepository.GetById(item.Id);
                    followMaster.SetUnFollowStatus();
                    followMaster.SetInActiveStatus();
                    followMaster.UpdatedBy = item.FolowerId;
                    followMaster.UpdatedDate = DateTime.UtcNow;

                    _followerMasterRepository.Update(followMaster);
                    //Status = true;


                    ///// User config 
                    var userConfig = _userProfileConfigRepository.Table.Where(u => u.UserId == FollowerId && u.LeaderId == item.LeaderId && u.Status == Convert.ToInt16(ServiceStatus.Active)).ToList();
                    foreach (var uitem in userConfig)
                    {
                        var userProfileConfig = _userProfileConfigRepository.GetById(uitem.Id);
                        userProfileConfig.SetUnDeleteStatus();
                        _userProfileConfigRepository.Update(userProfileConfig);
                    }
                }


                // return Status;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public bool AddWatch(WatchMasterViewModel model, int UserId = 0)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.LeaderId) && !string.IsNullOrEmpty(model.GroupId))
                {
                    string[] LeaderId = model.LeaderId.Split(",");
                    string[] GroupId = model.GroupId.Split(",");

                    foreach (var itemgroup in GroupId.ToArray())
                    {
                        foreach (var itemleder in LeaderId.ToList())
                        {
                            var getWatch = _watchcustomRepository.Table.Where(i => i.WatcherId == UserId && i.GroupId == Convert.ToInt64(itemgroup) && i.LeaderId == Convert.ToInt64(itemleder)).FirstOrDefault();
                            if (getWatch == null)
                            {
                                var Watch = new WatchMaster
                                {
                                    WatcherId = UserId,
                                    GroupId = Convert.ToInt64(itemgroup),
                                    LeaderId = Convert.ToInt64(itemleder),
                                    Status = Convert.ToInt16(ServiceStatus.Active),
                                    CreatedDate = DateTime.UtcNow,
                                    CreatedBy = UserId,
                                };
                                _watchcustomRepository.Insert(Watch);
                            }
                            else
                            {
                                getWatch.EnableWatch();
                                getWatch.UpdatedBy = UserId;
                                getWatch.UpdatedDate = DateTime.UtcNow;
                                _watchcustomRepository.Update(getWatch);
                            }
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public bool UnWatch(WatchMasterViewModel model, int UserId = 0)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.LeaderId) && !string.IsNullOrEmpty(model.GroupId))
                {
                    string[] LeaderId = model.LeaderId.Split(",");
                    string[] GroupId = model.GroupId.Split(",");

                    foreach (var itemgroup in GroupId.ToArray())
                    {
                        foreach (var itemleder in LeaderId.ToList())
                        {
                            var getWatch = _watchcustomRepository.Table.Where(i => i.WatcherId == UserId && i.GroupId == Convert.ToInt64(itemgroup) && i.LeaderId == Convert.ToInt64(itemleder) && i.Status == Convert.ToInt16(ServiceStatus.Active)).FirstOrDefault();
                            if (getWatch != null)
                            {
                                getWatch.DisableWatch();
                                getWatch.UpdatedBy = UserId;
                                getWatch.UpdatedDate = DateTime.UtcNow;
                                _watchcustomRepository.Update(getWatch);
                            }
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public WatcherWiseLeaderResponse GetWatcherWiseLeaderList(int pageIndex = 0, int pageSize = 0, int UserId = 0, int GroupId = 0)
        {
            try
            {
                //string Qry_Group = "";
                //IQueryable<GroupData> GroupList;
                //Qry_Group = @" Select Id,GroupName  from groupmaster where UserId =" + UserId + " and Status=" + Convert.ToInt16(ServiceStatus.Active) + "";

                //GroupList = _dbContext.GroupList.FromSql(Qry_Group);
                //List<GroupData> listGrouper = new List<GroupData>();
                //listGrouper = GroupList.ToList();

                List<GetWatcherModel> cmodelList = new List<GetWatcherModel>();

                //foreach (var item in listGrouper)
                //{
                //GetGroupModel modeldata = new GetGroupModel();
                //modeldata.Id = item.Id;
                //modeldata.GroupName = item.GroupName;

                string Qry = "";
                IQueryable<GetWatcherModel> followewiseWatchList;
                Qry = @" Select WM.GroupId,GM.GroupName,WM.LeaderId,Isnull(BU.UserName,'') as LeaderName ";
                Qry += " From WatchMaster WM Inner join Bizuser BU on WM.LeaderId=BU.Id  and wm.Status = 1 Inner Join groupmaster GM on WM.GroupId=GM.Id and GM.Status=1";
                Qry += " Where WM.WatcherId =" + UserId + "";
                if (GroupId > 0)
                    Qry += " And WM.GroupId =" + GroupId + "";

                followewiseWatchList = _dbContext.WatcherList.FromSql(Qry);
                List<GetWatcherModel> listfollowerwisewatch = new List<GetWatcherModel>();
                listfollowerwisewatch = followewiseWatchList.ToList();

                foreach (var itemdata in listfollowerwisewatch)
                {
                    GetWatcherModel getWatcherModel = new GetWatcherModel();
                    getWatcherModel.GroupId = itemdata.GroupId;
                    getWatcherModel.GroupName = itemdata.GroupName;
                    getWatcherModel.LeaderId = itemdata.LeaderId;
                    getWatcherModel.LeaderName = itemdata.LeaderName;
                    cmodelList.Add(getWatcherModel);
                }

                if (pageIndex == 0)
                    pageIndex = 1;

                if (pageSize == 0)
                    pageSize = 10;
                var skip = pageSize * (pageIndex - 1);

                WatcherWiseLeaderResponse response = new WatcherWiseLeaderResponse();
                response.WatcherList = cmodelList.Skip(skip).Take(pageSize).ToList();
                response.TotalCount = cmodelList.Count();
                return response;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        /*
        public GetLeaderFrontPolicyModel GetUserLeaderFrontProfile(int UserId = 0,int ProfileId = 0) //(LeaderAdminPolicyModel model, string ConfigType = null, int UserId = 0)
        {
            string Qry = "";

            IQueryable<SubscriptionProfile> Subscription;
            Qry = @" Select sm.ProfileId,tm.SubType as ProfileType from ProfileMaster pm inner join Typemaster tm on pm.TypeId = tm.Id  and  tm.[Type] = 'SocialProfile' and tm.Status = 1 ";
            Qry += " inner join subscriptionmaster sm on pm.Id = sm.ProfileId and sm.Status = 1 where sm.UserId =" + UserId;


            Subscription = _dbContext.SubscriptionProfileId.FromSql(Qry);
            List<SubscriptionProfile> Sprofile = new List<SubscriptionProfile>();
            Sprofile = Subscription.ToList();

            if (Sprofile.Count == 0)
                return null;
            else
            {
                var data  = from pm in _dbContext.ProfileMaster
                            join 
                Sprofile.ToList().FirstOrDefault()
            }

            _profilemaster
            string Qry = "";
            //int userID = 1;
            IQueryable<FrontLeaderProfile> frontLeadersProfile;
            Qry = @" Select IsNull(pc.Id,0) as ConfigId,pc.ConfigKey,pc.ConfigValue,Isnull(uc.Id,0) as UserProfileConfigId,Uc.ConfigValue as UserConfigValue,Isnull(uc.UserId,0) as UserId from ProfileConfiguration pc left join UserProfileConfig uc on pc.Id = uc.ProfileConfigId ";
            Qry += " where pc.ConfigType = '" + ProfileSocialCongifType.Leader_Admin_Policy + "' and pc.IsEnable = 0 and pc.IsDeleted = 0 and (ISNULL(uc.UserID,0) = " + UserId + " or Isnull(uc.UserID,0) = 0)"; // and uc.IsEnable = 0 and uc.IsDeleted = 0"; //+ UserId;


            frontLeadersProfile = _dbContext.FrontLeaderProfile.FromSql(Qry);
            List<FrontLeaderProfile> Profile = new List<FrontLeaderProfile>();
            Profile = frontLeadersProfile.ToList();

            //var ProfileConfig = _userProfileConfigRepository.Table.Where(i => i.UserId == UserId && !i.IsEnable && !i.IsDeleted).OrderByDescending(i => i.CreatedDate).ToList();
            GetLeaderFrontPolicyModel model = new GetLeaderFrontPolicyModel();
            foreach (var item in Profile)
            {
                //string val = EnProfileConfigKey.Default_Visibility_of_Profile.ToString();
                switch ((EnProfileConfigKey)Enum.Parse(typeof(EnProfileConfigKey), item.ConfigKey))
                {
                    case EnProfileConfigKey.Default_Visibility_of_Profile:
                        if (!string.IsNullOrEmpty(item.UserConfigValue))
                            //item.ConfigValue = Convert.ToString((EnVisibleProfile)model.Default_Visibility_of_Profile);                        
                            model.Default_Visibility_of_Profile = (int)(EnVisibleProfile)Enum.Parse(typeof(EnVisibleProfile), item.UserConfigValue);
                        else
                            model.Default_Visibility_of_Profile = (int)(EnVisibleProfile)Enum.Parse(typeof(EnVisibleProfile), item.configValue);
                        break;
                    case EnProfileConfigKey.Max_Number_Followers_can_Follow:
                        if (!string.IsNullOrEmpty(item.UserConfigValue))
                            model.Max_Number_Followers_can_Follow = Convert.ToInt32(item.UserConfigValue);
                        else
                            model.Max_Number_Followers_can_Follow = Convert.ToInt32(item.configValue);
                        break;
                    case EnProfileConfigKey.Min_Number_of_Followers_can_Follow:
                        if (!string.IsNullOrEmpty(item.UserConfigValue))
                            model.Min_Number_of_Followers_can_Follow = Convert.ToInt32(item.UserConfigValue);
                        else
                            model.Min_Number_of_Followers_can_Follow = Convert.ToInt32(item.configValue);
                        break;
                }
            }
            return model;
        }
        */
    }

}


