using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.Interfaces.Profile_Management;
using Worldex.Core.Interfaces.SocialProfile;
using Worldex.Core.Interfaces.User;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.Profile_Management;
using Worldex.Core.ViewModels.SocialProfile;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackofficeWorldex.Web.API.SocialProfile
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SocialProfileController : ControllerBase
    {
        #region "Fields"

        private readonly IProfileConfigurationService _profileConfigurationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userdata;
        private readonly IProfileMaster _profileMaster;
        private readonly ISubscriptionMaster _IsubscriptionMaster;
        private readonly IGroupMasterService _IgroupMasterService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        #endregion

        #region "CTOR"

        public SocialProfileController(IProfileConfigurationService profileConfigurationService, UserManager<ApplicationUser> userManager, IUserService userdata, IProfileMaster profileMaster, ISubscriptionMaster IsubscriptionMaster, IGroupMasterService IgroupMasterService,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            this._profileConfigurationService = profileConfigurationService;
            this._userManager = userManager;
            this._userdata = userdata;
            this._profileMaster = profileMaster;
            this._IsubscriptionMaster = IsubscriptionMaster;
            this._IgroupMasterService = IgroupMasterService;
            _configuration = configuration;
        }

        #endregion

        #region Method

        #region Back office Get , Set And update Leader and follower Config
        // POST: api/SocialProfile
        [HttpPost("SetLeaderProfile")]
        //[AllowAnonymous]
        public async Task<IActionResult> SetLeaderProfile([FromBody] LeaderAdminPolicyModel model)
        {
            try
            {
                if (_configuration["IPByPassLogin"].ToString() == "False")
                {
                    if (HttpContext.Connection.RemoteIpAddress.ToString() != null)
                    {
                        model.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        return Ok(new LeaderAdminPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                ////// Ip Address Validate or not
                var ipmodel = await _userdata.GetIPWiseData(model.IPAddress);
                if (!string.IsNullOrEmpty(ipmodel?.CountryCode) && ipmodel?.CountryCode == "fail")
                {
                    return BadRequest(new LeaderAdminPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
                }

                //bool success = Enum.IsDefined(typeof(EnVisibleProfile), model.Default_Visibility_of_Profile);
                if (!Enum.IsDefined(typeof(EnVisibleProfile), model.Default_Visibility_of_Profile))
                {
                    // Please select valid visible profile type. 12007
                    return BadRequest(new LeaderAdminPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Default_Visibility_of_Profile, ErrorCode = enErrorCode.Status12007Default_Visibility_of_Profile });
                }

                if (model.Min_Number_of_Followers_can_Follow <= 0)
                {
                    // Please enter min Number follower can follow. 12029
                    return BadRequest(new LeaderAdminPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Min_Number_of_Followers_can_Follow, ErrorCode = enErrorCode.Status12029Min_Number_of_Followers_can_Follow });
                }

                if (model.Max_Number_Followers_can_Follow <= 0)
                {
                    // Please enter max Number follower can follow. 12002
                    return BadRequest(new LeaderAdminPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Max_Number_Followers_can_Follow, ErrorCode = enErrorCode.Status12002Max_Number_Followers_can_Follow });
                }

                //Task.Run(() => _profileConfigurationService.SetLeaderProfileConfiguration(model, "Leader Admin Policy"));
                _profileConfigurationService.SetLeaderProfileConfiguration(model, ProfileSocialCongifType.Leader_Admin_Policy.ToString(), user.Id);

                return Ok(new LeaderAdminPolicyResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessFullUpDateLeaderAdminProfile });
            }

            catch (Exception ex)
            {
                return BadRequest(new LeaderAdminPolicyResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        // POST: api/SocialProfile
        [HttpPost("SetFollowerProfile")]
        //[AllowAnonymous]
        public async Task<IActionResult> SetFollowerProfile([FromBody] FollowerAdminModel model)
        {
            try
            {
                if (_configuration["IPByPassLogin"].ToString() == "False")
                {
                    if (HttpContext.Connection.RemoteIpAddress.ToString() != null)
                    {
                        model.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        return Ok(new FollowerAdminPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                ////// Ip Address Validate or not
                var ipmodel = await _userdata.GetIPWiseData(model.IPAddress);
                if (!string.IsNullOrEmpty(ipmodel?.CountryCode) && ipmodel?.CountryCode == "fail")
                {
                    return BadRequest(new FollowerAdminPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
                }

                //bool success = Enum.IsDefined(typeof(EnVisibleProfile), model.Default_Visibility_of_Profile);
                if (!Enum.IsDefined(typeof(EnYesNo), model.Can_Copy_Trade))
                {
                    //Please select valid copy trade option.12017
                    return BadRequest(new FollowerAdminPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Can_Copy_Trade, ErrorCode = enErrorCode.Status12017Can_Copy_Trade });
                }

                if (!Enum.IsDefined(typeof(EnYesNo), model.Can_Mirror_Trade))
                {
                    //Please Select valid mirror trade option.12018
                    return BadRequest(new FollowerAdminPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Can_Mirror_Trade, ErrorCode = enErrorCode.Status12018Can_Mirror_Trade });
                }

                if (model.Maximum_Number_of_Leaders_to_Allow_Follow <= 0)
                {
                    // Please enter max number of leader to allow follower.,12030
                    return BadRequest(new FollowerAdminPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Maximum_Number_of_Leaders_to_Allow_Follow, ErrorCode = enErrorCode.Status12030Maximum_Number_of_Leaders_to_Allow_Follow });
                }

                if (!Enum.IsDefined(typeof(EnYesNo), model.Can_Add_Leader_to_Watchlist))
                {
                    // Please select valid leader watchlist. 12010
                    return BadRequest(new FollowerAdminPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Can_Add_Pair_to_Watchlist, ErrorCode = enErrorCode.Status12010Can_Add_Pair_to_Watchlist });
                }

                //Task.Run(() => _profileConfigurationService.SetFolowerProfileConfiguration(model, "Follower Admin Policy"));

                _profileConfigurationService.SetFolowerProfileConfiguration(model, ProfileSocialCongifType.Follower_Admin_Policy.ToString(), user.Id);


                return Ok(new FollowerAdminPolicyResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessFullUpDateFollowerAdminProfile });
            }

            catch (Exception ex)
            {
                return BadRequest(new FollowerAdminPolicyResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }


        // GET: api/SocialProfile
        [HttpGet("GetLeaderProfile")]
        public async Task<IActionResult> GetLeaderProfile()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                var leaderpolicy = await _profileConfigurationService.GetLeaderProfileConfiguration();
                return Ok(new LeaderPolicyResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessFullUpGetLeaderAdminProfile, LeaderAdminPolicy = leaderpolicy });
            }
            catch (Exception ex)
            {
                return BadRequest(new LeaderPolicyResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        // GET: api/SocialProfile
        [HttpGet("GetFollowerProfile")]
        public async Task<IActionResult> GetFollowerProfile()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                var Followerpolicy = await _profileConfigurationService.GetFollowerProfileConfiguration();

                return Ok(new FollowerPolicyResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessFullUpGetLeaderAdminProfile, FollowerAdminPolicy = Followerpolicy });
            }
            catch (Exception ex)
            {
                return BadRequest(new FollowerPolicyResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion


        #region Front Side

        #region Front side Get Social Profile List user wise , Subscrib and unsubscrib profile
        // GET: api/SocialProfile
        [HttpGet("GetSocialProfile")]
        //[AllowAnonymous]
        public async Task<IActionResult> GetSocialProfile()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                var Followerpolicy = _profileMaster.GetSocialProfileData(user.Id);
                return Ok(new SocialProfileResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessFullUpGetProfileList, SocialProfileList = Followerpolicy });
            }
            catch (Exception ex)
            {
                return BadRequest(new SocialProfileResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("SubscribSocialProfile/{ProfileId}")]
        public async Task<ActionResult> SubscribSocialProfile(int ProfileId)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;
                //if (user != null)
                //{
                if (ProfileId > 0)
                {
                    long Status = _IsubscriptionMaster.GetSpcialProfileSubscriptionData(user.Id, ProfileId);
                    if (Status > 0)
                    {
                        // please first unsubscrib other subscription .
                        return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UnsubscribOtherSubscription, ErrorCode = enErrorCode.Status12020UnsubscribOtherSubscription });
                    }
                    bool ProfileStatus = _profileMaster.GetSocialProfile(ProfileId);
                    if (ProfileStatus)
                    {
                        _IsubscriptionMaster.AddMultiSubscription(user.Id, ProfileId);
                        return Ok(new SubscriptionResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessAddProfile });
                    }
                    else
                        return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidSocialProfile, ErrorCode = enErrorCode.Status12021InvalidSocialProfile });
                }
                else
                    return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidSocialProfile, ErrorCode = enErrorCode.Status12021InvalidSocialProfile });
            }
            catch (Exception ex)
            {
                return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpPost("UnsibscribeSocialProfile/{ProfileId}")]
        public async Task<ActionResult> UnsibscribeSocialProfile(int ProfileId)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                if (ProfileId > 0)
                {
                    long Status = _IsubscriptionMaster.GetSpcialProfileSubscriptionData(user.Id, ProfileId);
                    if (Status > 0)
                    {
                        if (ProfileId == Status)
                        {
                            _profileConfigurationService.UnSubscribeLeaderFrontProfileConfiguration(user.Id, ProfileId);
                            _IsubscriptionMaster.UnsubscribeProfile(user.Id, ProfileId);

                            // please first unsubscrib other subscription .
                            return Ok(new SubscriptionResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessUnSubscribeProfile });

                        }

                    }
                    return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.SubscribeplanNotAvailable, ErrorCode = enErrorCode.Status12022SubscribeplanNotAvailable });
                }
                else
                    return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidSocialProfile, ErrorCode = enErrorCode.Status12021InvalidSocialProfile });
            }
            catch (Exception ex)
            {
                return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Front Set Leader Front profile and set Follower Front profile 
        // POST: api/SocialProfile
        [HttpPost("SetLeaderFrontProfile")]
        //[AllowAnonymous]
        public async Task<IActionResult> SetLeaderFrontProfile([FromBody]  LeaderFrontPolicyModel model)
        {
            try
            {
                if (_configuration["IPByPassLogin"].ToString() == "False")
                {
                    if (HttpContext.Connection.RemoteIpAddress.ToString() != null)
                    {
                        model.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        return Ok(new LeaderFrontPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                ////// Ip Address Validate or not
                var ipmodel = await _userdata.GetIPWiseData(model.IPAddress);
                if (!string.IsNullOrEmpty(ipmodel?.CountryCode) && ipmodel?.CountryCode == "fail")
                {
                    return BadRequest(new LeaderFrontPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
                }

                //bool success = Enum.IsDefined(typeof(EnVisibleProfile), model.Default_Visibility_of_Profile);
                if (!Enum.IsDefined(typeof(EnVisibleProfile), model.Default_Visibility_of_Profile))
                {
                    // Please select valid visible profile type. 12007
                    return BadRequest(new LeaderFrontPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Default_Visibility_of_Profile, ErrorCode = enErrorCode.Status12007Default_Visibility_of_Profile });
                }

                GetLeaderFrontPolicyModel lmodel = _profileConfigurationService.GetUserLeaderProfileFrontConfiguration(user.Id);

                if (model.Max_Number_Followers_can_Follow <= 0)
                {
                    // Please enter max Number follower can follow. 12002
                    //  string message = String.Format(EnResponseMessage.Max_Number_Followers_can_Follow, lmodel.Min_Number_of_Followers_can_Follow.ToString(), lmodel.Max_Number_Followers_can_Follow.ToString());
                    return BadRequest(new LeaderFrontPolicyResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Max_Number_Followers_can_Follow, ErrorCode = enErrorCode.Status12002Max_Number_Followers_can_Follow });
                }
                else if (lmodel.Max_Number_Followers_can_Follow < model.Max_Number_Followers_can_Follow)
                {
                    // Set only max number of follower you follow. 12023
                    string message = String.Format(EnResponseMessage.Set_only_max_number_of_follower, lmodel.Min_Number_of_Followers_can_Follow.ToString(), lmodel.Max_Number_Followers_can_Follow.ToString());
                    return BadRequest(new LeaderFrontPolicyerrorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = message, ErrorCode = enErrorCode.Status12023Set_only_max_number_of_follower, Min_Follower = lmodel.Min_Number_of_Followers_can_Follow, Max_Follower = lmodel.Max_Number_Followers_can_Follow });
                }
                else if (lmodel.Min_Number_of_Followers_can_Follow > model.Max_Number_Followers_can_Follow)
                {
                    // Set max number of follower you follow between max and min. 12031
                    // Set only max number of follower you follow. 12023
                    string message = String.Format(EnResponseMessage.Set_only_max_number_of_follower, lmodel.Min_Number_of_Followers_can_Follow.ToString(), lmodel.Max_Number_Followers_can_Follow.ToString());
                    return BadRequest(new LeaderFrontPolicyerrorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = message, ErrorCode = enErrorCode.Status12023Set_only_max_number_of_follower, Min_Follower = lmodel.Min_Number_of_Followers_can_Follow, Max_Follower = lmodel.Max_Number_Followers_can_Follow });

                    //string message = String.Format(EnResponseMessage.Set_only_max_number_of_follower, lmodel.Min_Number_of_Followers_can_Follow.ToString(), lmodel.Max_Number_Followers_can_Follow.ToString());
                    //return BadRequest(new LeaderFrontPolicyerrorResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = message, ErrorCode = enErrorCode.Status12031Min_Number_of_Followers_can_Follow_front, Min_Follower = lmodel.Min_Number_of_Followers_can_Follow, Max_Follower = lmodel.Max_Number_Followers_can_Follow });
                }

                //Task.Run(() => _profileConfigurationService.SetLeaderProfileConfiguration(model, "Leader Admin Policy"));

                _profileConfigurationService.SetLeaderFrontProfileConfiguration(model, user.Id);

                return Ok(new LeaderFrontPolicyResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessFullUpDateLeaderAdminProfile });
            }

            catch (Exception ex)
            {
                return BadRequest(new LeaderFrontPolicyResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        // POST: api/SocialProfile
        [HttpPost("SetFollowerFrontProfile")]
        //[AllowAnonymous]
        public async Task<IActionResult> SetFollowerFrontProfile([FromBody] FollowerFrontModel model)
        {
            try
            {
                if (_configuration["IPByPassLogin"].ToString() == "False")
                {
                    if (HttpContext.Connection.RemoteIpAddress.ToString() != null)
                    {
                        model.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    }
                    else
                    {
                        return Ok(new FollowerFrontResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4188RequestedRemoteIPNotFound });
                    }
                }
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                ////// Ip Address Validate or not
                var ipmodel = await _userdata.GetIPWiseData(model.IPAddress);
                if (!string.IsNullOrEmpty(ipmodel?.CountryCode) && ipmodel?.CountryCode == "fail")
                {
                    return BadRequest(new FollowerFrontResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.IpAddressInvalid, ErrorCode = enErrorCode.Status4020IpInvalid });
                }
                if (!Enum.IsDefined(typeof(EnYesNo), model.Can_Copy_Trade))
                {
                    return BadRequest(new FollowerFrontResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Copy_Mirror_Or_Trade_Any_One, ErrorCode = enErrorCode.Status12035Copy_Mirror_Or_Trade_Any_One });
                }
                if (!Enum.IsDefined(typeof(EnYesNo), model.Can_Mirror_Trade))
                {
                    return BadRequest(new FollowerFrontResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Copy_Mirror_Or_Trade_Any_One, ErrorCode = enErrorCode.Status12035Copy_Mirror_Or_Trade_Any_One });
                }
                if (!Enum.IsDefined(typeof(EnYesNo), model.Can_Copy_Trade) && !Enum.IsDefined(typeof(EnYesNo), model.Can_Mirror_Trade) || (model.Can_Copy_Trade == model.Can_Mirror_Trade))
                {
                    // any one trad type select
                    return BadRequest(new FollowerFrontResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Copy_Mirror_Or_Trade_Any_One, ErrorCode = enErrorCode.Status12035Copy_Mirror_Or_Trade_Any_One });
                }
                else if (Enum.IsDefined(typeof(EnYesNo), model.Can_Copy_Trade) && Convert.ToInt16(EnYesNo.Yes) == model.Can_Copy_Trade)
                {
                    if (model.Trade_Percentage < 0 || model.Trade_Percentage > 99)
                    {
                        //"Please input copy trade percentage between 1 to 99."; // 12033;
                        return BadRequest(new FollowerFrontResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Can_Copy_Trade_Percentage, ErrorCode = enErrorCode.Status12033Can_Copy_Trade_Percentage });
                    }
                }
                else if (Enum.IsDefined(typeof(EnYesNo), model.Can_Mirror_Trade) && Convert.ToInt16(EnYesNo.Yes) == model.Can_Mirror_Trade)
                {
                    if (model.Trade_Percentage < 100 || model.Trade_Percentage > 100)
                    {
                        // Please input mirror trade 100 percentage. 12034
                        return BadRequest(new FollowerFrontResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Can_Mirror_Trade_Percentage, ErrorCode = enErrorCode.Status12034Can_Mirror_Trade_Percentage });
                    }
                }

                string[] LeaderId = model.LeaderId.Split(",");

                foreach (var Itemleaderid in LeaderId)

                {
                    GetLeaderFollowerCountModel lmodel = _profileConfigurationService.GetLeaderFollowNuber(Convert.ToInt32(Itemleaderid));
                    if (lmodel.Max_Number_Followers_can_Follow < (lmodel.TotalFollower+1))
                    {

                        // Cannot follow the leader because leader follows limit exide.. 12034
                        return BadRequest(new FollowerFrontResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LederlimitExide, ErrorCode = enErrorCode.Status12046LederlimitExide });
                    }
                }
                //Task.Run(() => _profileConfigurationService.SetFolowerProfileConfiguration(model, "Follower Admin Policy"));               

                _profileConfigurationService.SetFollowerFrontProfileConfiguration(model, user.Id);

                return Ok(new FollowerFrontResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessFullUpDateFollowerAdminProfile });
            }

            catch (Exception ex)
            {
                return BadRequest(new FollowerFrontResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }

        #endregion

        #region Unfollow leader front side 
        [HttpPost("UnFollow")]
        //[AllowAnonymous]
        public async Task<ActionResult> UnFollow(string LeaderId)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                if (!string.IsNullOrEmpty(LeaderId))
                {
                    _profileConfigurationService.Unfollowleader(user.Id, LeaderId);
                    return Ok(new SubscriptionResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessUnFollowLeader });

                    //return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FollowerNotAvailable, ErrorCode = enErrorCode.Status12036FollowerNotAvailable });
                }
                else
                {
                    return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LeaderId_NotNull, ErrorCode = enErrorCode.Status12037LeaderId_NotNull });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region front Get Leader list with follow un follow
        [HttpGet("GetLeaderList/{PageIndex}/{Page_Size}")]
        // [AllowAnonymous]
        public async Task<ActionResult> GetLeaderList(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                LeaderListWithGroupModel lmodel = _profileConfigurationService.GetFrontLeaderList(PageIndex, Page_Size, user.Id);
                return Ok(new LeaderListWithGroupResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.Get_LeaderList_Front, TotalCount = lmodel.TotalCount, LeaderList = lmodel.LeaderList });
            }
            catch (Exception ex)
            {
                return BadRequest(new LeaderListWithGroupResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetFollowerWiseLeaderList/{PageIndex}/{Page_Size}")]
        // [AllowAnonymous]
        public async Task<ActionResult> GetFollowerWiseLeaderList(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                LeaderListModel lmodel = _profileConfigurationService.GetLeaderListByFollowerId(PageIndex, Page_Size, user.Id);
                return Ok(new LeaderListFrontResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.Get_LeaderList_Front, TotalCount = lmodel.TotalCount, LeaderList = lmodel.LeaderList });
            }
            catch (Exception ex)
            {
                return BadRequest(new LeaderListFrontResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region get Front leader config current user and Front follower configurations
        [HttpGet("GetLeaderFrontProfileConfiguration")]
        public async Task<ActionResult> GetLeaderFrontProfileConfiguration()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                var profile = _IsubscriptionMaster.GetSpcialProfiletype(user.Id);
                if (profile != null)
                {
                    if (profile.profiletype == ProfileSocialCongifType.Leader.ToString())
                    {
                        GetLeaderFrontPolicyModel lmodel = _profileConfigurationService.GetUserLeaderProfileConfiguration(user.Id);
                        return Ok(new LeaderFrontConfigResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetLeaderConfig, LeaderFrontConfiguration = lmodel });
                    }
                    else
                        return BadRequest(new LeaderFrontConfigResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotSubscibeSocialProfile, ErrorCode = enErrorCode.Status12038UserNotSubscibeSocialProfile });

                }
                return BadRequest(new LeaderFrontConfigResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotSubscribAnyProfile, ErrorCode = enErrorCode.Status12039UserNotSubscribAnyProfile });
            }
            catch (Exception ex)
            {
                return BadRequest(new LeaderFrontConfigResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetFollowerFrontProfileConfiguration/{LeaderId}")]
        public async Task<ActionResult> GetFollowerFrontProfileConfiguration(long LeaderId = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                var profile = _IsubscriptionMaster.GetSpcialProfiletype(user.Id);
                if (profile != null)
                {
                    if (profile.profiletype == ProfileSocialCongifType.Follower.ToString())
                    {
                        FollowerServiceFrontModel fmodel = _profileConfigurationService.GetFollowerProfileFrontConfiguration(user.Id, LeaderId.ToString());

                        return Ok(new FollowerFrontConfigResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetLeaderConfig, FollowerFrontConfiguration = fmodel });
                    }
                    else
                        return BadRequest(new FollowerFrontConfigResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotSubscibeSocialProfile, ErrorCode = enErrorCode.Status12038UserNotSubscibeSocialProfile });

                }
                return BadRequest(new FollowerFrontConfigResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotSubscribAnyProfile, ErrorCode = enErrorCode.Status12039UserNotSubscribAnyProfile });
            }
            catch (Exception ex)
            {
                return BadRequest(new FollowerFrontConfigResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion





        [HttpGet("GetLeaderWiseFollowerConfig/{PageIndex}/{Page_Size}")]
        //[AllowAnonymous]
        public async Task<ActionResult> GetLeaderWiseFollowerConfig(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                var lmodel = _profileConfigurationService.GetLeaderWiseFollowers(user.Id, PageIndex, Page_Size);

                return Ok(new LeaderwiseFollowerResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.Get_LeaderwisefollowerList, FollowerList = lmodel.FollowerList.ToList(), Totalcount = lmodel.Totalcount });
            }
            catch (Exception ex)
            {
                return BadRequest(new LeaderwiseFollowerResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion


        #region GroupName

        [HttpPost("AddGroup")]
        //[AllowAnonymous]
        public async Task<ActionResult> AddGroup(GroupMasterViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                if (_IgroupMasterService.AddGroup(model, user.Id))
                    return Ok(new GroupMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.Successfully_Add_Group });
                else
                    return BadRequest(new GroupMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Already_Exsits_Group, ErrorCode = enErrorCode.Status12041Already_Exsits_Group });
            }
            catch (Exception ex)
            {
                return BadRequest(new GroupMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetGroupList")]
        //[AllowAnonymous]
        public async Task<ActionResult> GetGroupList(/*GroupMasterViewModel model*/)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                var Group = _IgroupMasterService.GetGroupListByUserId(user.Id);
                return Ok(new GroupListResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.Successfully_Get_Group_List, GroupList = Group });

            }
            catch (Exception ex)
            {
                return BadRequest(new GroupMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region WatchMaster
        // added by nirav savariya on 24-01-2018
        [HttpPost("AddWatch")]
        public async Task<IActionResult> AddWatch(WatchMasterViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;
                if (_profileConfigurationService.AddWatch(model, user.Id))
                    return Ok(new GroupMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.Successfully_Add_Watch });
                else
                    return BadRequest(new WatchMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Already_Exsits_Watch, ErrorCode = enErrorCode.Status12042Already_Exsits_Watch });
            }
            catch (Exception ex)
            {
                return BadRequest(new WatchMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        // added by nirav savariya on 24-01-2018
        [HttpPost("UnFollowWatch")]
        public async Task<IActionResult> UnFollowWatch(WatchMasterViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items["UserId"] = user.Id;
                if (_profileConfigurationService.UnWatch(model, user.Id))
                    return Ok(new GroupMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessUnFollowWatch });
                else
                    return BadRequest(new WatchMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UnFollowWatchNotAvailable, ErrorCode = enErrorCode.Status12045UnFollowWatchNotAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new WatchMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        // added by nirav savariya on 25-01-2018
        [HttpGet("GetWatcherWiseLeaderList/{PageIndex}/{Page_Size}/{GroupId}")]
        public async Task<ActionResult> GetWatcherWiseLeaderList(int PageIndex = 0, int Page_Size = 0, int GroupId = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                HttpContext.Items[await HttpContext.GetTokenAsync("access_token")] = user.Id;

                var modeldata = _profileConfigurationService.GetWatcherWiseLeaderList(PageIndex, Page_Size, user.Id, GroupId);
                return Ok(new WatcherWiseLeaderResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.Get_WatchList_Front, WatcherList = modeldata.WatcherList, TotalCount = modeldata.TotalCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new LeaderListFrontResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #endregion


        #region Helpers
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        #endregion
    }

}
