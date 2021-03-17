using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Interfaces.PasswordPolicy;
using CleanArchitecture.Core.ViewModels.BackOffice.PasswordPolicy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.API.BackOffice
{
    /// <summary>
    /// This controller Created by pankaj kathiriya for perforn the password policy configuration  option like insert,modify,delete and get
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PasswordPolicyController : ControllerBase
    {
       
        public readonly IUserPasswordPolicyMaster _UserPasswordPolicyMaster;
        private readonly UserManager<ApplicationUser> _userManager;
        public PasswordPolicyController(IUserPasswordPolicyMaster userPasswordPolicyMaster,
            UserManager<ApplicationUser> userManager)
        {
            _UserPasswordPolicyMaster = userPasswordPolicyMaster;
            _userManager = userManager;
        }
        [HttpPost("PasswordPolicyAdd")]
        public async Task<IActionResult> PasswordPolicyAdd(UserPasswordPolicyMasterViewModel UserPasswordPolicyMaster)
        {
            try
            {
                if (UserPasswordPolicyMaster.PwdExpiretime == 0)
                {
                    return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.PwdExpiretime, ErrorCode = enErrorCode.Status8057PwdExpiretime });
                }
               
                if (UserPasswordPolicyMaster.MaxfppwdDay == 0)
                {
                    return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.MaxfppwdDay, ErrorCode = enErrorCode.Status8054MaxfppwdDay });
                }
                if (UserPasswordPolicyMaster.MaxfppwdMonth == 0)
                {
                    return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.MaxfppwdMonth, ErrorCode = enErrorCode.Status8055MaxfppwdMonth });
                }
                if (UserPasswordPolicyMaster.LinkExpiryTime == 0)
                {
                    return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LinkExpiryTime, ErrorCode = enErrorCode.Status8053LinkExpiryTime });
                }
                if (UserPasswordPolicyMaster.OTPExpiryTime == 0)
                {
                    return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.OTPExpiryTime, ErrorCode = enErrorCode.Status8056OTPExpiryTime });
                }
               

                var user = await GetCurrentUserAsync();
                UserPasswordPolicyMasterReqViewModel userPasswordPolicy = new UserPasswordPolicyMasterReqViewModel()
                {
                    LinkExpiryTime = UserPasswordPolicyMaster.LinkExpiryTime,
                    MaxfppwdDay = UserPasswordPolicyMaster.MaxfppwdDay,
                    MaxfppwdMonth = UserPasswordPolicyMaster.MaxfppwdMonth,
                    OTPExpiryTime = UserPasswordPolicyMaster.OTPExpiryTime,
                    PwdExpiretime = UserPasswordPolicyMaster.PwdExpiretime,
                    UserId = user.Id
                };
                long Passwordpolicyid = _UserPasswordPolicyMaster.IsPasswordPolicyExist(userPasswordPolicy);
                if (Passwordpolicyid > 0)
                {
                    return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.PasswordPolicyExist, ErrorCode = enErrorCode.Status8044PasswordPolicyExist });
                }
                else
                {
                    long id = _UserPasswordPolicyMaster.Add(userPasswordPolicy);
                    if (id > 0)
                        return Ok(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.AddPasswordPolicy });
                    else
                        return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.PasswordPolicyNotInsert, ErrorCode = enErrorCode.Status8045NotAddPasswordPolicy });
                }

            }
            catch (Exception ex)
            {

                return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("PasswordPolicyupdate")]
        public async Task<IActionResult> PasswordPolicyupdate(UserPasswordPolicyMasterupdateViewModel UserPasswordPolicyMaster)
        {
            try
            {
                if (UserPasswordPolicyMaster.LinkExpiryTime == 0)
                {
                    return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LinkExpiryTime, ErrorCode = enErrorCode.Status8053LinkExpiryTime });
                }
                if (UserPasswordPolicyMaster.MaxfppwdDay == 0)
                {
                    return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.MaxfppwdDay, ErrorCode = enErrorCode.Status8054MaxfppwdDay });
                }
                if (UserPasswordPolicyMaster.MaxfppwdMonth == 0)
                {
                    return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.MaxfppwdMonth, ErrorCode = enErrorCode.Status8055MaxfppwdMonth });
                }
                if (UserPasswordPolicyMaster.OTPExpiryTime == 0)
                {
                    return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.OTPExpiryTime, ErrorCode = enErrorCode.Status8056OTPExpiryTime });
                }
                if (UserPasswordPolicyMaster.PwdExpiretime == 0)
                {
                    return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.PwdExpiretime, ErrorCode = enErrorCode.Status8057PwdExpiretime });
                }
                var user = await GetCurrentUserAsync();
                UserPasswordPolicyMasterupdatereqViewModel userPasswordPolicy = new UserPasswordPolicyMasterupdatereqViewModel()
                {
                    LinkExpiryTime = UserPasswordPolicyMaster.LinkExpiryTime,
                    MaxfppwdDay = UserPasswordPolicyMaster.MaxfppwdDay,
                    MaxfppwdMonth = UserPasswordPolicyMaster.MaxfppwdMonth,
                    OTPExpiryTime = UserPasswordPolicyMaster.OTPExpiryTime,
                    PwdExpiretime = UserPasswordPolicyMaster.PwdExpiretime,
                    UserId = user.Id,
                    Id = UserPasswordPolicyMaster.Id
                };

                long id = _UserPasswordPolicyMaster.Update(userPasswordPolicy);
                if (id > 0)
                    return Ok(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccessupdatePasswordPolicy });
                else
                    return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.updatePasswordPolicy, ErrorCode = enErrorCode.Status8046NotUpdatePasswordPolicy });
            }
            catch (Exception ex)
            {

                return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [HttpPost("PasswordPolicyDelete")]
        public async Task<IActionResult> PasswordPolicyDelete(UserPasswordPolicyMasterDeleteViewModel UserPasswordPolicyMaster)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                UserPasswordPolicyMasterDeletereqViewModel userPasswordPolicy = new UserPasswordPolicyMasterDeletereqViewModel()
                {
                    UserId = user.Id,
                    Id = UserPasswordPolicyMaster.Id
                };

                long id = _UserPasswordPolicyMaster.Delete(userPasswordPolicy);
                if (id > 0)
                    return Ok(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.SuccesseDeletePasswordPolicy });
                else
                    return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DeletePasswordPolicy, ErrorCode = enErrorCode.Status8047NotDeletePasswordPolicy });
            }
            catch (Exception ex)
            {
                return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetPasswordPolicy")]

        public async Task<IActionResult> GetPasswordPolicy(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                var PasswordPolicyList = _UserPasswordPolicyMaster.GetPasswordPolicy(PageIndex, Page_Size);
                return Ok(new UserPasswordPolicyMasterresponseListViewModel { ReturnCode = enResponseCode.Success, TotalCount=PasswordPolicyList.TotalCount,UserPasswordPolicyMaster = PasswordPolicyList.UserPasswordPolicyMaster, ReturnMsg = EnResponseMessage.PasswordPolicyGet });
            }
            catch (Exception ex)
            {
                return BadRequest(new UserPasswordPolicyMasterresponseViewModel { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }

}