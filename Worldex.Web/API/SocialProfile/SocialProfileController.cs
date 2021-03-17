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

namespace Worldex.Web.API.SocialProfile
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

        public SocialProfileController(IProfileConfigurationService profileConfigurationService, UserManager<ApplicationUser> userManager, IUserService userdata, IProfileMaster profileMaster, ISubscriptionMaster IsubscriptionMaster, IGroupMasterService IgroupMasterService, Microsoft.Extensions.Configuration.IConfiguration configuration)
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

        [HttpGet("GetSocialProfile")]
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
        
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
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
    }

}
