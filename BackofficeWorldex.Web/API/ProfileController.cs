using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Interfaces.Profile_Management;
using CleanArchitecture.Core.PasswordPolicy;
using CleanArchitecture.Core.ViewModels.Profile_Management;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        #region Field
        private readonly IProfileMaster _IprofileMaster;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISubscriptionMaster _IsubscriptionMaster;
        #endregion

        #region Ctore
        public ProfileController(IProfileMaster IprofileMaster, UserManager<ApplicationUser> userManager, ISubscriptionMaster IsubscriptionMaster)
        {
            _IprofileMaster = IprofileMaster;
            _userManager = userManager;
            _IsubscriptionMaster = IsubscriptionMaster;
        }
        #endregion

        #region Methods
        [HttpGet("GetProfileData")]
        public async Task<ActionResult> GetProfileData()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new ProfileMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });
                //var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items["UserId"] = user.Id;
                var ProfileList = _IprofileMaster.GetProfileData(user.Id);
                if (ProfileList != null)
                    return Ok(new ProfileMasterResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessGetProfilePlan, ProfileList = ProfileList });
                else
                    return BadRequest(new ProfileMasterResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ProfilePlan, ErrorCode = enErrorCode.Status4112ProfilePlan });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProfileMasterResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("AddProfile")]
        public async Task<ActionResult> AddProfile(MultipalSubscriptionReqViewModel model)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });
                //var UserId = await HttpContext.GetTokenAsync("access_token");
                HttpContext.Items["UserId"] = user.Id;
                if (model.ProfileId > 0)
                {
                    //bool Status = _IsubscriptionMaster.GetSubscriptionData(user.Id, model.ProfileId);
                    //if (Status)
                    //{
                    _IsubscriptionMaster.AddMultiSubscription(user.Id, model.ProfileId);
                    return Ok(new SubscriptionResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessUpdateProfile });
                    //}
                    //else
                    //    return Ok(new SubscriptionResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotAddedProfile, ErrorCode = enErrorCode.Status4122NotAddedProfile });
                }
                else
                    return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidProfileId, ErrorCode = enErrorCode.Status4129InvalidProfileId });
            }
            catch (Exception ex)
            {
                return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        // Feature & transaction access are based on subscribed profile 
        // added by nirav savariya for current active profile data on 18-01-2019
        [HttpGet("CurrentUserProfileData")]
        public async Task<IActionResult> CurrentUserProfileData()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });
                HttpContext.Items["UserId"] = user.Id;
                var UserData = _IsubscriptionMaster.CurrentUserProfileData(user.Id);
                if (UserData != null)
                    return Ok(new GetActiveSubResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.UserWiseGetProfile, ActiveSubscriptionData = UserData });
                else
                    return Ok(new SubscriptionResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.UserWiseGetProfileNotavailable, ErrorCode = enErrorCode.Status4165UserWiseGetProfileNotavailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        // Disabled current and down-level profiles 
        // Current profile and down level profile disable list and uper level profile avtive list show on 18-01-2019
        [HttpGet("GetAllUserProfileData/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetAllUserProfileData(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });
                HttpContext.Items["UserId"] = user.Id;
                var UserData = _IprofileMaster.GetAllUserProfileData(user.Id, PageIndex, Page_Size);
                if (UserData != null)
                    return Ok(new GetProfileDataResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetUserWiseProfileData, GetUserWiseProfileData = UserData.GetUserWiseProfileData, TotalCount = UserData.TotalCount });
                else
                    return Ok(new SubscriptionResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.UserWiseGetProfileNotavailable, ErrorCode = enErrorCode.Status4165UserWiseGetProfileNotavailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetUserWiseProfileHistory/{PageIndex}/{Page_Size}")]
        public async Task<IActionResult> GetUserWiseProfileHistory(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null) // khushali 05-04-2019 for check use exist or not
                    return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.UserNotExist, ErrorCode = enErrorCode.Status4037UserNotAvailable });
                HttpContext.Items["UserId"] = user.Id;
                var UserWiseProfileData = _IsubscriptionMaster.GetUserProfileHistoryData(user.Id, passwordPolicy.AddProfile, PageIndex, Page_Size);
                if (UserWiseProfileData != null)
                    return Ok(new UserProfileHistoryDataResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetUserWiseProfileHistory, UserProfileHistory = UserWiseProfileData.UserProfileHistory, TotalCount = UserWiseProfileData.TotalCount });
                else
                    return Ok(new SubscriptionResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.UserWiseProfileHistoryNotavailable, ErrorCode = enErrorCode.Status4166UserWiseProfileHistoryNotavailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new SubscriptionResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Helpers
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        #endregion
    }
}