using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.KYCConfiguration;
using Worldex.Core.Services;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletConfiguration;
using Worldex.Core.ViewModels.WalletOperations;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BackofficeWorldex.Web.API.ControlPanel
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class WalletControlPanelController : Controller
    {
        #region Constructor
        private readonly EncyptedDecrypted _encdecAEC;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IControlPanelServices _controlPanelServices;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IDocumentMaster _documentMaster;
        Worldex.Infrastructure.Data.MultiChainClient _client = null;
        private readonly ILPWalletTransaction _lPWalletTransaction;
        private readonly ICommunicationService _communicationService;

        public WalletControlPanelController(
            UserManager<ApplicationUser> userManager, IControlPanelServices controlPanelServices, IDocumentMaster documentMaster,
            Microsoft.Extensions.Configuration.IConfiguration configuration, EncyptedDecrypted encdecAEC, ICommunicationService communicationService,
            IPushNotificationsQueue<SendEmailRequest> PushNotificationsQueue, ILPWalletTransaction lPWalletTransaction)
        {
            _userManager = userManager;
            _controlPanelServices = controlPanelServices;
            _configuration = configuration;
            _encdecAEC = encdecAEC;
            _pushNotificationsQueue = PushNotificationsQueue;
            _documentMaster = documentMaster;
            _lPWalletTransaction = lPWalletTransaction;
            _communicationService = communicationService;
        }

        #endregion

        #region TransactionBlockedChannel

        [HttpPost]
        public async Task<IActionResult> BlockTransactionForChannel([FromBody] TransactionBlockedChannelReq Request)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.BlockTranForChannel(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{ID}")]
        public async Task<IActionResult> GetBlockTransactionForChannel(long ID, long? ChannelID, short? Status)
        {
            ListTransactionBlockedChannelRes Response = new ListTransactionBlockedChannelRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetBlockTranForChannel(ID, Status, ChannelID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListBlockTransactionForChannel(enWalletTrnType? TrnType, long? ChannelID, short? Status)
        {
            ListTransactionBlockedChannelRes Response = new ListTransactionBlockedChannelRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListBlockTranForChannel(TrnType, ChannelID, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region WalletPolicyAllowedDay

        [HttpPost]
        public async Task<IActionResult> AddWalletPolicyAllowedDay([FromBody] WalletPolicyAllowedDayReq Request)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.AddWPolicyAllowedDay(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{ID}")]
        public async Task<IActionResult> GetWalletPolicyAllowedDay(long ID, EnWeekDays? DayNo, long? PolicyID, short? Status)
        {
            ListWalletPolicyAllowedDayRes Response = new ListWalletPolicyAllowedDayRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetWPolicyAllowedDay(ID, DayNo, PolicyID, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListWalletPolicyAllowedDay(EnWeekDays? DayNo, long? PolicyID, short? Status)
        {
            ListWalletPolicyAllowedDayRes Response = new ListWalletPolicyAllowedDayRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListWPolicyAllowedDay(DayNo, PolicyID, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        #endregion

        #region User Methods

        [HttpGet]
        public async Task<IActionResult> GetUserCount(long? OrgID, long? UserTypeID, short? Status, long? RoleID)
        {
            TodaysCount Response = new TodaysCount();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetUserCount(OrgID, UserTypeID, Status, RoleID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCountStatusWise()
        {
            StatusWiseRes Response = new StatusWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetUCntStatusWise();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCountTypeWise()
        {
            UTypeWiseRes Response = new UTypeWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetUCntTypeWise();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCountOrgWise()
        {
            OrgWiseRes Response = new OrgWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetUCntOrgWise();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCountRoleWise()
        {
            RolewiseUserCount Response = new RolewiseUserCount();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetUCntRoleWise();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTodaysUserCount(short? Status)
        {
            CountRes Response = new CountRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetTodayUserCount(Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListAllUserDetails(long? OrgID, long? UserType, short? Status, int? PageNo, int? PageSize)
        {
            ListUserDetailRes Response = new ListUserDetailRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListAllUserDetails(OrgID, UserType, Status, PageNo, PageSize);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 2018-11-27
        [HttpGet]
        public async Task<IActionResult> ListUserLastFive()
        {
            UserList Response = new UserList();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListUserLast5();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region User Type Methods

        [HttpGet]
        public async Task<IActionResult> ListUserTypes(short? Status)
        {
            ListUserTypeRes Response = new ListUserTypeRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListAllUserType(Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Organization Methods
        [HttpGet]
        public async Task<IActionResult> GetOrgCount(short? Status)
        {
            TodaysCount Response = new TodaysCount();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetOrganizationCount(Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{OrgID}")]
        public async Task<IActionResult> SetDefaultOrg(long OrgID)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.SetDefaultOrganization(OrgID, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTodaysOrgCount(short? Status)
        {
            CountRes Response = new CountRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetTodayOrganizationCount(Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListOrgDetails(short? Status, long? OrgID)
        {
            OrgList Response = new OrgList();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ListOrgDetail(Status, OrgID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Wallet Methods

        [HttpGet]
        public async Task<IActionResult> GetWalletCount(long? WalletTypeID, short? Status, long? OrgID, long? UserID)
        {
            CountRes Response = new CountRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetWalletCount(WalletTypeID, Status, OrgID, UserID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletCountTypeWise()
        {
            WalletTypeWiseRes Response = new WalletTypeWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetWCntTypeWise();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletCountStatusWise()
        {
            WalletTypeWiseRes Response = new WalletTypeWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetWCntStatusWise();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletCountOrgWise()
        {
            WalletTypeWiseRes Response = new WalletTypeWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetWCntOrgWise();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletCountUserWise()
        {
            WalletTypeWiseRes Response = new WalletTypeWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetWCntUserWise();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageSize}/{Page}")]
        public async Task<IActionResult> ListAllWallet(DateTime? FromDate, DateTime? ToDate, short? Status, int PageSize, int Page, long? UserId, long? OrgId, string WalletType)
        {
            ListWalletResV1 Response = new ListWalletResV1();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListAllWallet(FromDate, ToDate, Status, PageSize, Page, UserId, OrgId, WalletType);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{AccWalletId}")]
        public async Task<IActionResult> GetWalletIdWise(string AccWalletId)
        {
            WalletRes1 Response = new WalletRes1();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetWalletIdWise(AccWalletId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Authorized Apps

        [HttpPost]
        public async Task<IActionResult> AddAuthorizeApps([FromBody] AuthAppReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.AddAuthorizedApps(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpGet]
        public async Task<IActionResult> ListAuthorizedAppDetails(short? Status)
        {
            ListAuthAppRes Response = new ListAuthAppRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListAuthorizedAppDetail(Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{AppID}")]
        public async Task<IActionResult> GetAuthorizedAppDetails(long AppID, short? Status)
        {
            ListAuthAppRes Response = new ListAuthAppRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetAuthorizedAppDetail(AppID, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region TranTypeWiseReport

        [HttpGet("{Type}")]
        public async Task<IActionResult> GetBlockedTrnTypeWiseWalletDetails(enWalletTrnType Type, int? PageNo, int? PageSize)
        {
            ListBlockTrnTypewiseReport Response = new ListBlockTrnTypewiseReport();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetBlockedTrnTypeWiseWalletDetail(Type, PageNo, PageSize);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Wallet Authorized User Methods

        [HttpGet]
        public async Task<IActionResult> GetWalletAuthUserCount(short? Status, long? OrgID, long? UserID, long? RoleID)
        {
            CountRes Response = new CountRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetWalletAuthUserCount(Status, OrgID, UserID, RoleID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletAuthUserCountStatusWise()
        {
            WalletTypeWiseRes Response = new WalletTypeWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetWAUCntStatusWise();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletAuthUserCountOrgWise()
        {
            WalletTypeWiseRes Response = new WalletTypeWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetWAUCntOrgWise();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletAuthUserCountRoleWise()
        {
            RolewiseUserCount Response = new RolewiseUserCount();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetWAUCntRoleWise();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Log Listing Methods

        [HttpGet]
        public async Task<IActionResult> GetUserActivityLog(long? UserID, DateTime? FromDate, DateTime? ToDate)
        {
            ListUserActivityLoging Response = new ListUserActivityLoging();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetUserActivities(UserID, FromDate, ToDate);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region User Role Methods

        [HttpGet]
        public async Task<IActionResult> GetUserRoleCount(short? Status)
        {
            CountRes Response = new CountRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetUserRoleCount(Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListRoleDetails(short? Status)
        {
            RoleList Response = new RoleList();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListRoleDetail(Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Currency Method

        [HttpGet]
        public async Task<IActionResult> GetCurrencyCount(short? Status)
        {
            CountRes Response = new CountRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetCurrencyCount(Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListCurrencyDetails(short? Status)
        {
            CurrencyList Response = new CurrencyList();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListCurrencyDetail(Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCurrencyType([FromBody] CurrencyMasterReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.AddNewCurrencyType(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Wallet Type Methods

        [HttpPost]
        public async Task<IActionResult> AddWalletTypeDetail([FromBody] WalletTypeMasterReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.AddWalletTypeDetails(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletTypeCount(short? Status, long? CurrencyTypeID)
        {
            CountRes Response = new CountRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetWalletTypeCount(Status, CurrencyTypeID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{CurrencyTypeID}")]
        public async Task<IActionResult> GetWalletTypeDetails(long CurrencyTypeID)
        {
            WalletTypeMasterResp Response = new WalletTypeMasterResp();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetWalletTypeDetail(CurrencyTypeID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListWalletTypeDetails(short? Status, long? ServiceProviderId, long? CurrencyTypeID, short isMargin = 0)
        {
            WalletTypeList Response = new WalletTypeList();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListWalletTypeDetail(Status, ServiceProviderId, CurrencyTypeID, isMargin);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region ChargeTypeMaster Methods

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> GetChargeTypeCount(short? Status, long? ChargeTypeID)
        {
            CountRes Response = new CountRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetChargeTypeCount(Status, ChargeTypeID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> ListChargeTypeDetail(short? Status, long? ChargeTypeID)
        {
            ChargeTypeList Response = new ChargeTypeList();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListChargeTypeDetail(Status, ChargeTypeID);
                }

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-14
        [HttpPost]
        public async Task<IActionResult> InsertUpdateChargeType([FromBody]ChargeTypeReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.InsertUpdateChargeType(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-14
        [HttpPost("{Id}/{Status}")]
        public async Task<IActionResult> ChangeChargeTypeStatus(long Id, ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ChangeChargeTypeStatus(Id, Convert.ToInt16(Status), user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region CommissionTypeMaster Methods

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> GetCommisssionTypeCount(short? Status, long? TypeID)
        {
            CountRes Response = new CountRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetCommissionTypeCount(Status, TypeID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> ListCommisssionTypeDetail(short? Status, long? TypeID)
        {
            CommisssionTypeList Response = new CommisssionTypeList();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListCommissionTypeDetail(Status, TypeID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-13
        [HttpPost]
        public async Task<IActionResult> InsertUpdateCommisssionType([FromBody]CommisssionTypeReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.InsertUpdateCommisssionType(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-13
        [HttpPost("{Id}/{Status}")]
        public async Task<IActionResult> ChangeCommisssionTypeReqStatus(long Id, ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ChangeCommisssionTypeReqStatus(Id, Convert.ToInt16(Status), user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Charge Policy Methods 

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> GetChargePolicyRecCount(long? WalletTypeID, short? Status, long? WalletTrnTypeID)
        {
            CountRes Response = new CountRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetChargePolicyRecCount(WalletTypeID, Status, WalletTrnTypeID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> GetChargePolicyCountWalletTypeWise()
        {
            WalletTypeWiseRes Response = new WalletTypeWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetChargePolicyWalletTypeWiseCount();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> GetChargePolicyCountStatusWise()
        {
            StatusWiseRes Response = new StatusWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetChargePolicyStatusWiseCount();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> GetChargePolicyCountWalletTrnTypeWise()
        {
            WalletTrnTypeWiseRes Response = new WalletTrnTypeWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetChargePolicyWalletTrnTypeWiseCount();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> GetChargePolicyDetail(short? Status, long? WalletType, long? WalletTrnType)
        {
            ListChargePolicy Response = new ListChargePolicy();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetChargePolicyList(Status, WalletType, WalletTrnType);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 27-11-2018
        [HttpGet]
        public async Task<IActionResult> ListChargePolicyLastFive()
        {
            ListChargePolicy Response = new ListChargePolicy();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListChargePolicyLast5();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolnki 28-11-2018
        [HttpPost]
        public async Task<IActionResult> InsertChargePolicy([FromBody]ChargePolicyReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.InsertChargePolicy(Req);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolnki 28-11-2018
        [HttpPost]
        public async Task<IActionResult> UpdateChargePolicy(long Id, [FromBody]UpdateChargePolicyReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.UpdateChargePolicy(Id, Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Commission Policy Methods 

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> GetCommissionPolicyRecCount(long? WalletTypeID, short? Status, long? WalletTrnTypeID)
        {
            CountRes Response = new CountRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetCommissionPolicyRecCount(WalletTypeID, Status, WalletTrnTypeID);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> GetCommissionPolicyCountWalletTypeWise()
        {
            WalletTypeWiseRes Response = new WalletTypeWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetCommissionPolicyWalletTypeWiseCount();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> GetCommissionPolicyCountStatusWise()
        {
            StatusWiseRes Response = new StatusWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetCommissionPolicyStatusWiseCount();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> GetCommissionPolicyCountWalletTrnTypeWise()
        {
            WalletTrnTypeWiseRes Response = new WalletTrnTypeWiseRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetCommissionPolicyWalletTrnTypeWiseCount();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 24-11-2018
        [HttpGet]
        public async Task<IActionResult> GetCommissionPolicyDetail(short? Status, long? WalletType, long? WalletTrnType)
        {
            ListCommissionPolicy Response = new ListCommissionPolicy();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetCommissionPolicyList(Status, WalletType, WalletTrnType);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 27-11-2018
        [HttpGet]
        public async Task<IActionResult> ListCommissionPolicyLastFive()
        {
            ListCommissionPolicy Response = new ListCommissionPolicy();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListCommissionPolicy();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //Rushabh 28/11/2018
        [HttpPost]
        public async Task<IActionResult> AddCommissionPolicy([FromBody] CommPolicyReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.AddCommPolicy(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{CommPolicyID}")]
        public async Task<IActionResult> UpdateCommissionPolicyDetail(long CommPolicyID, [FromBody] UpdateCommPolicyReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.UpdateCommPolicyDetail(CommPolicyID, Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Graph API

        //vsolanki 2018-11-27
        [HttpGet]
        public async Task<IActionResult> GetMonthwiseUserCount()
        {
            ListWalletGraphRes Response = new ListWalletGraphRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GraphForUserCount();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthwiseOrgCount()
        {
            ListWalletGraphRes Response = new ListWalletGraphRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GraphForOrgCount();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTranCountTypewise()
        {
            ListTransactionTypewiseCount Response = new ListTransactionTypewiseCount();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GraphForTrnTypewiseCount();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Wallet Usage Policy

        //Rushabh 19-12-2018
        [HttpGet]
        public async Task<IActionResult> ListUsagePolicy(long? WalletTypeID, short? Status)
        {
            ListWalletusagePolicy2 Response = new ListWalletusagePolicy2();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListUsagePolicy(WalletTypeID, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        //vsolanki 2018-11-27
        [HttpGet]
        public async Task<IActionResult> ListUsagePolicyLastFive()
        {
            ListWalletusagePolicy Response = new ListWalletusagePolicy();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListUsagePolicyLast5();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-14
        [HttpPost]
        public async Task<IActionResult> InsertUpdateWalletUsagePolicy([FromBody]AddWalletUsagePolicyReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.InsertUpdateWalletUsagePolicy(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-14
        [HttpPost("{Id}/{Status}")]
        public async Task<IActionResult> ChangeWalletUsagePolicyStatus(long Id, ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ChangeWalletUsagePolicyStatus(Id, Convert.ToInt16(Status), user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region WTrnTypeMaster
        //vsoalnki 2018-11-28
        [HttpPost("{TrnTypeId}/{Status}")]
        public async Task<IActionResult> UpdateWTrnTypeStatus(long TrnTypeId, ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.UpdateWTrnTypeStatus(TrnTypeId, Convert.ToInt16(Status), user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 2018-11-28
        [HttpGet]
        public async Task<IActionResult> ListWalletTrnType()
        {
            ListTypeRes Response = new ListTypeRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListWalletTrnType();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region BlockWalletTrnTypeMaster
        //vsoalnki 2018-11-28
        [HttpPost]
        public async Task<IActionResult> InsertBlockWalletTrnType([FromBody]BlockWalletTrnTypeReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.InsertBlockWalletTrnType(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //vsolanki 2018-11-28
        [HttpGet("{WalletType}")]
        public async Task<IActionResult> GetBlockWTypewiseTrnTypeList(long WalletType)
        {
            ListTypeRes Response = new ListTypeRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetBlockWTypewiseTrnTypeList(WalletType);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Wallet Type

        //vsolanki 2018-11-28
        [HttpGet]
        public async Task<IActionResult> ListWalletType()
        {
            ListTypeRes Response = new ListTypeRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListWalletType();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region AllowedChannel Methods

        [HttpGet("{ChannelID}")]
        public async Task<IActionResult> GetAllowedChannelDetail(long ChannelID, short? Status)
        {
            ListChannels Response = new ListChannels();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.GetChannels(ChannelID, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAllowedChannel([FromBody] AllowedChannelReq Request)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.AddAllowedChannels(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListChannelDetail()
        {
            ListChannels Response = new ListChannels();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListChannels();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListChannelwiseTransactionCount()
        {
            ListChannelwiseTrnCount Response = new ListChannelwiseTrnCount();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListChannelwiseTrnCnt();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region other

        [HttpGet]
        public async Task<IActionResult> GetOrgAllDetail()
        {
            ListOrgDetail Response = new ListOrgDetail();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.GetOrgAllDetail();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ListTypeWiseDetail>> GetDetailTypeWise()
        {
            ListTypeWiseDetail Response = new ListTypeWiseDetail();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.GetDetailTypeWise();
                }
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region TransactionPolicy
        //2018-12-12
        [HttpGet]
        public async Task<IActionResult> ListTransactionPolicy()
        {
            ListTransactionPolicyRes Response = new ListTransactionPolicyRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListTransactionPolicy();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-12
        [HttpPost]
        public async Task<IActionResult> InsertTransactionPolicy([FromBody]AddTransactionPolicyReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.InsertTransactionPolicy(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-12
        [HttpPost("{TrnPolicyId}")]
        public async Task<IActionResult> UpdateTransactionPolicy([FromBody]UpdateTransactionPolicyReq Req, long TrnPolicyId)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.UpdateTransactionPolicy(Req, user.Id, TrnPolicyId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-12
        [HttpPost("{TrnPolicyId}/{Status}")]
        public async Task<IActionResult> ChangeTransactionPolicyStatus(long TrnPolicyId, ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.UpdateTransactionPolicyStatus(Convert.ToInt16(Status), user.Id, TrnPolicyId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region UserWalletAllowTrnType

        //2018-12-13
        [HttpGet]
        public async Task<IActionResult> ListUserWalletBlockTrnType(string WalletId, long? TrnTypeId)
        {
            ListUserWalletBlockTrnType Response = new ListUserWalletBlockTrnType();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListUserWalletBlockTrnType(WalletId, TrnTypeId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-13
        [HttpPost]
        public async Task<IActionResult> InsertUpdateUserWalletBlockTrnType([FromBody]InsertUpdateUserWalletBlockTrnTypeReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.InsertUpdateUserWalletBlockTrnType(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-13
        [HttpPost("{Id}/{Status}")]
        public async Task<IActionResult> ChangeUserWalletBlockTrnTypeStatus(long Id, ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ChangeUserWalletBlockTrnTypeStatus(Id, Convert.ToInt16(Status), user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region WalletAuthorizeUserMaster
        //2018-12-13
        [HttpGet("{AccWalletId}")]
        public async Task<IActionResult> ListWalletAuthorizeUser(string AccWalletId)
        {
            ListWalletAuthorizeUserRes Response = new ListWalletAuthorizeUserRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListWalletAuthorizeUser(AccWalletId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region AllowTrnTypeRoleWise

        //2018-12-22
        [HttpGet]
        public async Task<IActionResult> ListAllowTrnTypeRoleWise(long? RoleId, long? TrnTypeId, short? Status)
        {
            ListAllowTrnTypeRoleWise Response = new ListAllowTrnTypeRoleWise();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ListAllowTrnTypeRoleWise(RoleId, TrnTypeId, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-12
        [HttpPost]
        public async Task<IActionResult> InserUpdateAllowTrnTypeRole([FromBody]InserUpdateAllowTrnTypeRoleReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.InserUpdateAllowTrnTypeRole(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-12
        [HttpPost("{Id}/{Status}")]
        public async Task<IActionResult> ChangeAllowTrnTypeRoleStatus(long Id, ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ChangeAllowTrnTypeRoleStatus(Convert.ToInt16(Status), user.Id, Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Staking Configuration

        [HttpPost]
        public async Task<IActionResult> AddStakingPolicy([FromBody] StakingPolicyReq Request)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.AddStakingPolicy(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{PolicyDetailId}")]
        public async Task<IActionResult> UpdateStakingPolicyDetail(long PolicyDetailId, [FromBody] UpdateStakingDetailReq Request)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.UpdateStakingPolicy(PolicyDetailId, Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{PolicyDetailId}/{Status}")]
        public async Task<IActionResult> ChangeStakingPolicyDetailStatus(long PolicyDetailId, ServiceStatus Status)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (User == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ChangeStakingPolicyStatus(PolicyDetailId, Status, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{StackingPolicyMasterId}")]
        public async Task<IActionResult> ListStackingPolicyDetail(long StackingPolicyMasterId, EnStakingType? StakingType, EnStakingSlabType? SlabType, short? Status)
        {
            ListStakingPolicyDetailRes2 Response = new ListStakingPolicyDetailRes2();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _controlPanelServices.ListStakingPolicy(StackingPolicyMasterId, StakingType, SlabType, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PolicyDetailID}")]
        public async Task<IActionResult> GetStackingPolicyDetail(long PolicyDetailID, short? Status)
        {
            ListStakingPolicyDetailRes Response = new ListStakingPolicyDetailRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _controlPanelServices.GetStakingPolicy(PolicyDetailID, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListUnStackingRequest(long? Userid, short? Status, EnUnstakeType UnStakingType)
        {
            ListUnStakingHistory Response = new ListUnStakingHistory();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _controlPanelServices.ListUnStakingHistory(Userid, Status, UnStakingType);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListUnStackingRequestv2(long? Userid, short? Status, EnUnstakeType UnStakingType)
        {
            ListUnStakingHistoryv2 Response = new ListUnStakingHistoryv2();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _controlPanelServices.ListUnStakingHistoryv2(Userid, Status, UnStakingType);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{AdminReqID}/{Bit}")]
        public async Task<IActionResult> AcceptRejectUnstakingRequest(long AdminReqID, ServiceStatus Bit, UserUnstakingReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _controlPanelServices.AdminUnstakeRequest(AdminReqID, Bit, user.Id, Request);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{AdminReqID}/{Bit}")]
        public async Task<IActionResult> AcceptRejectUnstakingRequestv2(string AdminReqID, ServiceStatus Bit, UserUnstakingReqv2 Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _controlPanelServices.AdminUnstakeRequestv2(AdminReqID, Bit, user.Id, Request);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageSize}/{PageNo}")]
        public async Task<IActionResult> GetStackingHistory(DateTime? FromDate, long? UserId, DateTime? ToDate, EnStakeUnStake? Type, int PageSize, int PageNo, EnStakingSlabType? Slab, EnStakingType? StakingType)
        {
            ListStakingHistoryRes Response = new ListStakingHistoryRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (User == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _controlPanelServices.GetStackingHistoryData(FromDate, ToDate, Type, PageSize, PageNo, Slab, StakingType, UserId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageSize}/{PageNo}")]
        public async Task<IActionResult> GetStackingHistoryv2(DateTime? FromDate, long? UserId, DateTime? ToDate, EnStakeUnStake? Type, int PageSize, int PageNo, EnStakingSlabType? Slab, EnStakingType? StakingType)
        {
            ListStakingHistoryResv2 Response = new ListStakingHistoryResv2();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (User == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _controlPanelServices.GetStackingHistoryDatav2(FromDate, ToDate, Type, PageSize, PageNo, Slab, StakingType, UserId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region StopLossMaster

        //2018-12-28
        [HttpGet]
        public async Task<IActionResult> ListStopLoss(long? WalletTypeId, short? Status)
        {
            ListStopLossRes Response = new ListStopLossRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ListStopLoss(WalletTypeId, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-28
        [HttpPost]
        public async Task<IActionResult> InserUpdateStopLoss([FromBody]InserUpdateStopLossReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.InserUpdateStopLoss(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-28
        [HttpPost("{Id}/{Status}")]
        public async Task<IActionResult> ChangeStopLossStatus(long Id, ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ChangeStopLossStatus(Convert.ToInt16(Status), user.Id, Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        //2019-02-23 move to margin control panel
        #region LeverageMaster

        //2018-12-28
        [HttpGet]
        public async Task<IActionResult> ListLeverage(long? WalletTypeId, short? Status)
        {
            ListLeverageRes Response = new ListLeverageRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ListLeverage(WalletTypeId, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-28
        [HttpPost]
        public async Task<IActionResult> InserUpdateLeverage([FromBody]InserUpdateLeverageReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.InserUpdateLeverage(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //2018-12-28
        [HttpPost("{Id}/{Status}")]
        public async Task<IActionResult> ChangeLeverageStatus(long Id, ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ChangeLeverageStatus(Convert.ToInt16(Status), user.Id, Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Import-Export Address

        [HttpPost]
        public async Task<IActionResult> ImportAddress()
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                var httpRequest = Request.Form;
                if (httpRequest.Files.Count == 0)
                {
                    return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FileNotFound, ErrorCode = enErrorCode.FileRequiredToImport });
                }
                var file = httpRequest.Files[0];
                string data = System.IO.Path.GetExtension(file.FileName);
                data = data.ToUpper();
                data = data.Substring(1);

                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {
                    var postedFile = httpRequest.Files[file.Name];
                    string folderDirctory = user.Id.ToString();
                    string webRootPath = _configuration["ImportedFilePath"].ToString();
                    string newPath = webRootPath + folderDirctory;
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    string Extension = System.IO.Path.GetExtension(file.FileName);
                    string fileName = ContentDispositionHeaderValue.Parse(postedFile.ContentDisposition).FileName.Trim('"');
                    fileName = fileName.Replace(fileName, "ImportAddress_" + Helpers.UTC_To_IST().ToString("yyyyMMddHHmmssfff") + Extension);

                    string fullPath = newPath + "/" + fileName;
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(stream);
                    }
                    Response = await _controlPanelServices.ImportAddressDetails(newPath, fullPath);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetExportAddressDetails(long? ServiceProviderID, long? UserID, long? WalletTypeID)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ExportAddressDetails(ServiceProviderID, UserID, WalletTypeID, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageNo}/{PageSize}")]
        public async Task<IActionResult> ListAddressDetails(long? ServiceProviderID, long? UserID, long? WalletTypeID, string Address, int PageNo, int PageSize)
        {
            ListAddressRes Response = new ListAddressRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = _controlPanelServices.ListAddressDetails(ServiceProviderID, UserID, WalletTypeID, Address, PageNo, PageSize);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmAddExport(string emailConfirmCode)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                if (!string.IsNullOrEmpty(emailConfirmCode))
                {
                    byte[] DecpasswordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
                    var bytes = Convert.FromBase64String(emailConfirmCode);
                    var encodedString = Encoding.UTF8.GetString(bytes);
                    string DecryptToken = EncyptedDecrypted.Decrypt(encodedString, DecpasswordBytes);
                    ExpAddress_EmailLinkTokenViewModel dmodel = JsonConvert.DeserializeObject<ExpAddress_EmailLinkTokenViewModel>(DecryptToken);
                    if (dmodel?.Expirytime >= DateTime.UtcNow)   /// Check the link expiration time 
                    {
                        if (dmodel.Id == 0)
                        {
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ReturnMsg = "Invalid Link";
                            Response.ErrorCode = enErrorCode.InvalidConfirmationId;
                            return Ok(Response);
                        }
                        else
                        {
                            var user = await _userManager.FindByEmailAsync(dmodel.Email);
                            if (dmodel.DownloadLink == null || dmodel.DownloadLink == string.Empty)
                            {
                                Response.ReturnCode = enResponseCode.Fail;
                                Response.ReturnMsg = "Invalid Link";
                                Response.ErrorCode = enErrorCode.DownloadFilePathNotFound;
                                return Ok(Response);
                            }
                            if (user == null)
                            {
                                Response.ReturnCode = enResponseCode.Fail;
                                Response.ReturnMsg = "Invalid Link";
                                Response.ErrorCode = enErrorCode.UserDetailNotFound;
                                return Ok(Response);
                            }
                            else
                            {
                                if (System.IO.File.Exists(dmodel.DownloadLink))
                                {
                                    var content = new FileStream(dmodel.DownloadLink, FileMode.Open, FileAccess.Read, FileShare.Read);
                                    var response = File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");//FileStreamResult //"application/octet-stream"
                                    return response;
                                }
                                Response.ReturnCode = enResponseCode.Fail;
                                Response.ReturnMsg = "Invalid Link";
                                Response.ErrorCode = enErrorCode.DownloadFileNotFound;
                                return Ok(Response);
                            }
                        }
                    }
                    else
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Link Expired";
                        Response.ErrorCode = enErrorCode.LinkExpired;
                        return Ok(Response);
                    }
                }
                Response.ReturnCode = enResponseCode.Fail;
                Response.ReturnMsg = "Confirmation Code Required";
                Response.ErrorCode = enErrorCode.emailConfirmCodeRequired;
                return Ok(Response);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ConfirmAddExport", "WalletControlPanelController", ex);
                throw;
            }
        }

        #endregion

        #region Export Wallet

        [HttpPost("{FileName}/{Coin}")]
        public async Task<IActionResult> ExportWallet(string FileName, string Coin)
        {
            try
            {
                var Response = await _controlPanelServices.ExportWallet(FileName, Coin);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region StakingPolicyMaster

        // 15-1-2019
        [HttpGet]
        public async Task<IActionResult> ListStakingPolicyMaster(long? WalletTypeID, short? Status, EnStakingSlabType? SlabType, EnStakingType? StakingType)
        {
            ListStakingPolicyRes Response = new ListStakingPolicyRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListStakingPolicyMaster(WalletTypeID, Status, (SlabType == null ? Convert.ToInt16(0) : Convert.ToInt16(SlabType)), (StakingType == null ? Convert.ToInt16(0) : Convert.ToInt16(StakingType)));
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        // 15-1-2019
        [HttpPost]
        public async Task<IActionResult> InsertUpdateStakingPolicy([FromBody]InsertUpdateStakingPolicyReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.InsertUpdateStakingPolicy(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        // 15-1-2019
        [HttpPost("{Id}/{Status}")]
        public async Task<IActionResult> ChangeStakingPolicyMasterStatus(long Id, ServiceStatus Status)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ChangeStakingPolicyStatus(Id, Convert.ToInt16(Status), user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Charge
        [HttpGet]
        public async Task<IActionResult> ListChargesTypeWise(string WalletTypeName, long? TrnTypeId)
        {
            try
            {
                ListChargesTypeWise Response = new ListChargesTypeWise();
                Response = _controlPanelServices.ListChargesTypeWise(WalletTypeName, TrnTypeId);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageNo}/{PageSize}")]
        public async Task<IActionResult> TrnChargeLogReport(int PageNo, int PageSize, short? Status, long? TrnTypeID, long? WalleTypeId, short? SlabType, DateTime? FromDate, DateTime? ToDate, long? TrnNo)
        {
            try
            {
                ListTrnChargeLogRes Response = new ListTrnChargeLogRes();
                Response = _controlPanelServices.TrnChargeLogReport(PageNo, PageSize, Status, TrnTypeID, WalleTypeId, SlabType, FromDate, ToDate, TrnNo);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageNo}/{PageSize}")]
        public async Task<IActionResult> TrnChargeLogReportv2(int PageNo, int PageSize, short? Status, long? TrnTypeID, long? WalleTypeId, short? SlabType, DateTime? FromDate, DateTime? ToDate, string TrnNo)
        {
            try
            {
                ListTrnChargeLogResv2 Response = new ListTrnChargeLogResv2();
                Response = _controlPanelServices.TrnChargeLogReportv2(PageNo, PageSize, Status, TrnTypeID, WalleTypeId, SlabType, FromDate, ToDate, TrnNo);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region DepositCounterMaster

        [HttpGet("{PageNo}/{PageSize}")]
        public async Task<IActionResult> GetDepositCounter(long? WalletTypeID, long? SerProId, int PageNo, int PageSize)
        {
            try
            {
                ListDepositeCounterRes Response = new ListDepositeCounterRes();
                Response = _controlPanelServices.GetDepositCounter(WalletTypeID, SerProId, PageNo, PageSize);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertUpdateDepositCounter([FromBody]InsertUpdateDepositCounterReq Request)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                Response = _controlPanelServices.InsertUpdateDepositCounter(Request);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{Id}/{Status}")]
        public async Task<IActionResult> ChangeDepositCounterStatus(long Id, ServiceStatus Status)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                Response = _controlPanelServices.ChangeDepositCounterStatus(Id, Convert.ToInt16(Status));
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Admin Assets

        [HttpGet("{PageNo}/{PageSize}")]
        public async Task<IActionResult> AdminAssets(long? WalletTypeId, EnWalletUsageType? WalletUsageType, int PageNo, int PageSize)
        {
            try
            {
                long userid = 1;
                ListAdminAssetsres Response = new ListAdminAssetsres();
                Response = _controlPanelServices.AdminAssets(WalletTypeId, WalletUsageType, userid, PageNo, PageSize);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Organization Ledger

        [HttpGet]
        public async Task<IActionResult> ListOrganizationWallet(long? WalletTypeId, EnWalletUsageType? WalletUsageType)
        {
            try
            {
                ListOrgLedger Response = new ListOrgLedger();
                Response = _controlPanelServices.OrganizationLedger(WalletTypeId, WalletUsageType);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region ChargeConfigurationMaster

        [HttpPost]
        public async Task<IActionResult> AddChargeConfiguration([FromBody]ChargeConfigurationMasterReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.AddNewChargeConfiguration(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{MasterId}")]
        public async Task<IActionResult> UpdateChargeConfiguration(long MasterId, [FromBody]ChargeConfigurationMasterReq2 Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.UpdateChargeConfiguration(MasterId, Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{MasterId}")]
        public async Task<IActionResult> GetChargeConfiguration(long MasterId)
        {
            ChargeConfigurationMasterRes2 Response = new ChargeConfigurationMasterRes2();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.GetChargeConfiguration(MasterId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListChargeConfiguration(long? WalletTypeId, long? TrnType, short? SlabType, short? Status, int? PageSize, int? PageNo)
        {
            ListChargeConfigurationMasterRes Response = new ListChargeConfigurationMasterRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ListChargeConfiguration(WalletTypeId, TrnType, SlabType, Status, PageSize, PageNo);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region ChargeConfigurationDetail

        [HttpPost]
        public async Task<IActionResult> AddChargeConfigurationDeatil([FromBody]ChargeConfigurationDetailReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.AddNewChargeConfigurationDetail(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{DetailId}")]
        public async Task<IActionResult> UpdateChargeConfigurationDetail(long DetailId, [FromBody]ChargeConfigurationDetailReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.UpdateChargeConfigurationDetail(DetailId, Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{DetailId}")]
        public async Task<IActionResult> GetChargeConfigurationDetail(long DetailId)
        {
            ChargeConfigurationDetailRes2 Response = new ChargeConfigurationDetailRes2();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.GetChargeConfigurationDetail(DetailId);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListChargeConfigurationDetail(long? MasterId, long? ChargeType, short? ChargeValueType, short? ChargeDistributionBasedOn, short? Status)
        {
            ListChargeConfigurationDetailRes Response = new ListChargeConfigurationDetailRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ListChargeConfigurationDetail(MasterId, ChargeType, ChargeValueType, ChargeDistributionBasedOn, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region WalletTrnLimitConfiguration

        [HttpPost]
        public async Task<IActionResult> AddMasterLimitConfiguration([FromBody]WalletTrnLimitConfigurationInsReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.AddMasterLimitConfiguration(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMasterLimitConfiguration([FromBody]WalletTrnLimitConfigurationUpdReq Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.UpdateMasterLimitConfiguration(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeMasterLimitConfigStatus([FromBody]ChangeServiceStatus Request)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ChangeMasterLimitConfigStatus(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListMasterLimitConfiguration(long? WalletTypeId, long? TrnType, EnIsKYCEnable? IsKYCEnable, short? Status)
        {
            ListWalletTrnLimitConfigResp Response = new ListWalletTrnLimitConfigResp();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ListMasterLimitConfiguration(WalletTypeId, TrnType, IsKYCEnable, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetMasterLimitConfiguration(long Id)
        {
            GetWalletTrnLimitConfigResp Response = new GetWalletTrnLimitConfigResp();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.GetMasterLimitConfiguration(Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        ////2019-02-23 move to margin control panel
        //#region Leaverage Report 

        //[HttpGet("{PageNo}/{PageSize}")]
        //public async Task<IActionResult> LeveragePendingReport(long? WalletTypeId, long? UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize)
        //{
        //    try
        //    {
        //        ListLeaverageReportRes Response = new ListLeaverageReportRes();
        //        ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            Response = _controlPanelServices.LeveragePendingReport(WalletTypeId, UserId, FromDate, ToDate, PageNo, PageSize);
        //        }
        //        return Ok(Response);

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpGet("{FromDate}/{ToDate}/{PageNo}/{PageSize}")]
        //public async Task<IActionResult> LeverageReport(long? WalletTypeId, long? UserId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, short? Status)
        //{
        //    try
        //    {
        //        ListLeaverageRes Response = new ListLeaverageRes();
        //        ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            Response = _controlPanelServices.LeverageReport(WalletTypeId, UserId, FromDate, ToDate, PageNo, PageSize, Status);
        //        }
        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //#endregion

        #region Deposition Recon

        [HttpGet("{PageNo}")]
        public async Task<IActionResult> DepositionReport(string Trnid, string Address, short? IsInternal, long? UserID, string CoinName, long? Provider, int PageNo, int? PageSize)
        {
            try
            {
                DepositionHistoryRes Response = new DepositionHistoryRes();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.DepositionReport(Trnid, Address, IsInternal, UserID, CoinName, Provider, PageNo, Convert.ToInt32(PageSize == null ? Helpers.PageSize : PageSize));
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DepositionReconProcess([FromBody]DepositionReconReq Request)
        {
            try
            {
                ListDepositionReconRes Response = new ListDepositionReconRes();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.DepositionReconProcess(Request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region TradingChargeTypeMaster

        [HttpGet]
        public async Task<IActionResult> ListTradingChargeType()
        {
            try
            {
                ListTradingChargeTypeRes Response = new ListTradingChargeTypeRes();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListTradingChargeTypeMaster();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertTradingChargeType([FromBody]InsertTradingChargeTypeReq Request)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.InsertTradingChargeType(Request);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTradingChargeType([FromBody]UpdateTradingChargeTypeReq Request)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.UpdateTradingChargeType(Request);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Currency Logo

        [HttpPost]
        public async Task<IActionResult> AddCurrencyLogo()
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                var httpRequest = Request.Form;
                if (httpRequest.Files.Count == 0)
                {
                    Response.ErrorCode = enErrorCode.ImageNotFoundInRequest;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = "Please Upload Image";
                    return Ok(Response);
                }
                if (String.IsNullOrEmpty(httpRequest["CurrencyName"].ToString()))
                {
                    Response.ErrorCode = enErrorCode.ImageUploadCurrencyNameRequired;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = "Currency Name Not Found";
                    return Ok(Response);
                }
                string CurrencyName = httpRequest["CurrencyName"].ToString();
                var file = httpRequest.Files[0];
                string data = System.IO.Path.GetExtension(file.FileName);
                data = data.ToUpper();
                data = data.Substring(1);
                if (data == "PNG" || data == "JPG" || data == "JPEG")
                {
                    ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                    if (user == null)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                        Response.ErrorCode = enErrorCode.StandardLoginfailed;
                        return Ok(Response);
                    }
                    else
                    {
                        var postedFile = httpRequest.Files[file.Name];
                        string webRootPath = _configuration["CurrencyLogoPath"].ToString();
                        if (!Directory.Exists(webRootPath))
                        {
                            Directory.CreateDirectory(webRootPath);
                        }
                        string Extension = System.IO.Path.GetExtension(file.FileName);
                        string fileName = ContentDispositionHeaderValue.Parse(postedFile.ContentDisposition).FileName.Trim('"');
                        fileName = fileName.Replace(fileName, CurrencyName + Extension);

                        string fullPath = webRootPath + "/" + fileName;
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await postedFile.CopyToAsync(stream);
                        }
                        Response.ErrorCode = enErrorCode.ImageUploadedSuccessfully;
                        Response.ReturnCode = enResponseCode.Success;
                        Response.ReturnMsg = "Image Uploaded Successfully";
                        return Ok(Response);
                    }
                }
                else
                {
                    Response.ErrorCode = enErrorCode.InvalidImageFormat;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = "You Can Not Upload An Image Other Than PNG/JPG/JPEG";
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        // Develop by Pratik 14-3-2019
        #region Deposition Interval 

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddDepositionInterval(DepositionIntervalViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

                var obj = new DepositionIntervalViewModel()
                {
                    DepositHistoryFetchListInterval = model.DepositHistoryFetchListInterval,
                    DepositStatusCheckInterval = model.DepositStatusCheckInterval,
                    Status = model.Status,
                };
                long id = _controlPanelServices.AddDepositionInterval(obj, user.Id);
                if (id != 0)
                {
                    return new OkObjectResult(
                    new DepositionIntervalResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.SuccessfullAddOrUpdateDepositionInterval
                    });
                }
                else
                {
                    return BadRequest(new DepositionIntervalResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailAddOrUpdateDepositionInterval, ErrorCode = enErrorCode.Status33001FailAddOrUpdateDepositionInterval });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new DepositionIntervalResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> ListDepositionInterval()
        {
            try
            {
                var getData = await Task.FromResult(_controlPanelServices.ListDepositionInterval());
                if (getData != null)
                    return Ok(new ListDepositionIntervalResponse { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.DepositionIntervalList, ListDepositionInterval = getData.ListDepositionInterval });
                else
                    return BadRequest(new ListDepositionIntervalResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailDepositionIntervalList, ErrorCode = enErrorCode.Status33002FailDepositionIntervalList });
            }
            catch (Exception ex)
            {
                return BadRequest(new ListDepositionIntervalResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DisableDepositionInterval(DepositionIntervalStatusViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var obj = new DepositionIntervalStatusViewModel()
                {
                    Id = model.Id
                };
                bool id = _controlPanelServices.DisableDepositionInterval(obj, user.Id);
                if (id == true)
                {
                    return new OkObjectResult(
                    new DepositionIntervalResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.DisableDepositionIntervalStatus
                    });
                }
                else
                {
                    return BadRequest(new DepositionIntervalResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailDisableDepositionIntervalStatus, ErrorCode = enErrorCode.Status33003FailDisableDepositionIntervalStatus });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new DepositionIntervalResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EnableDepositionInterval(DepositionIntervalStatusViewModel model)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                var obj = new DepositionIntervalStatusViewModel()
                {
                    Id = model.Id
                };
                bool id = _controlPanelServices.EnableDepositionInterval(obj, user.Id);
                if (id == true)
                {
                    return new OkObjectResult(
                    new DepositionIntervalResponse
                    {
                        ReturnCode = enResponseCode.Success,
                        ReturnMsg = EnResponseMessage.EnableDepositionIntervalStatus
                    });
                }
                else
                {
                    return BadRequest(new DepositionIntervalResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailEnableDepositionIntervalStatus, ErrorCode = enErrorCode.Status33004FailEnableDepositionIntervalStatus });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new DepositionIntervalResponse { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> ListMultichainAddress(string chainName, int PageIndex, int PageSize)
        {
            try
            {
                var connection = _controlPanelServices.MultichainConnection(chainName);
                if (connection != null)
                {
                    _client = new Worldex.Infrastructure.Data.MultiChainClient(connection.hostname, Convert.ToInt32(connection.port), connection.username, connection.password, connection.chainName);
                    if (_client != null)
                    {
                        var TotalCountAddress = await Task.FromResult(_client.MultichainLP.ListAddressesTotalCount().Result);
                        var getData = await Task.FromResult(_client.MultichainLP.ListAddresses(PageSize, PageIndex).Result);
                        List<string> listStringAddress = new List<string>();
                        foreach (object data in getData)
                        {
                            string myString = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                            var ms = new MemoryStream(Encoding.Unicode.GetBytes(myString));
                            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(MultichainViewModel));
                            MultichainViewModel obj = (MultichainViewModel)deserializer.ReadObject(ms);
                            listStringAddress.Add(obj.address);
                        }

                        if (getData != null)
                            return Ok(new ListMultichainAddressViewModel { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.GetMultichainAddressList, listaddressitem = listStringAddress, TotalCount = TotalCountAddress.Count });
                        else
                            return BadRequest(new ListMultichainAddressViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailMultichainAddressList, ErrorCode = enErrorCode.Status33005FailMultichainAddressList });
                    }
                    else
                    {
                        return BadRequest(new ListMultichainAddressViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailMultichainConnectionNode, ErrorCode = enErrorCode.Status33007FailMultichainConnectionNode });
                    }
                }
                else
                {
                    return BadRequest(new ListMultichainAddressViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailMultichainConnection, ErrorCode = enErrorCode.Status33006FailMultichainConnection });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ListMultichainAddressViewModel { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.FailMultichainConnectionNode, ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #region Service Provider Balance

        [HttpGet("{ServiceProviderID}/{CurrencyName}")]
        public async Task<IActionResult> GetServiceProviderBalance(long ServiceProviderID, enTrnType? TransactionType, string CurrencyName)
        {
            ServiceProviderBalanceResponse Response = new ServiceProviderBalanceResponse();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.GetSerProviderBalance(ServiceProviderID, TransactionType, CurrencyName);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceProviderBalanceResponse { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InternalError, ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpGet("{SMSCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLPProviderBalance(string SMSCode, long SerProID = 0)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            LPServiceProBalanceResponse Response = new LPServiceProBalanceResponse();
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.GetLPProviderBalance(SerProID, SMSCode);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        #endregion

        #region ERC-223 Integration

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BlockUnblockUserAddress([FromBody]BlockUserReqRes Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.BlockUnblockUserAddress(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ListBlockUnblockUserAddress(long? UserId, string Address, DateTime? FromDate, DateTime? ToDate, short? Status)
        {
            ListBlockUserRes Response = new ListBlockUserRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.ListBlockUnblockUserAddress(UserId, Address, FromDate, ToDate, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DestroyBlackFund([FromBody]DestroyBlackFundReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.DestroyBlackFund(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DestroyedBlackFundHistory(DateTime? FromDate, DateTime? ToDate, string Address)
        {
            ListDestroyBlackFundRes Response = new ListDestroyBlackFundRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.DestroyedBlackFundHistory(Address, FromDate, ToDate);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> TokenTransfer([FromBody]TokenTransferReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.TokenTransfer(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> TokenTransferHistory(DateTime? FromDate, DateTime? ToDate)
        {
            ListTokenTransferRes Response = new ListTokenTransferRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.TokenTransferHistory(FromDate, ToDate);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> IncreaseTokenSupply([FromBody]TokenSupplyReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.IncreaseTokenSupply(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> IncreaseDecreaseTokenSupplyHistory(DateTime? FromDate, DateTime? ToDate, long? WalletTypeId, short? ActionType)
        {
            ListTokenSupplyRes Response = new ListTokenSupplyRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.IncreaseDecreaseTokenSupplyHistory(WalletTypeId, ActionType, FromDate, ToDate);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DecreaseTokenSupply([FromBody]TokenSupplyReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.DecreaseTokenSupply(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetTransferFee([FromBody]SetTransferFeeReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.SetTransferFee(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> TransferFeeHistory(DateTime? FromDate, DateTime? ToDate, long? WalletTypeId)
        {
            ListSetTransferFeeRes Response = new ListSetTransferFeeRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.TransferFeeHistory(WalletTypeId, FromDate, ToDate);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        #endregion

        #region Testing Purpose

        [HttpGet]
        public async Task<IActionResult> GetAuthToken()
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                Response = await _controlPanelServices.GetToken();
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Withdraw Admin Accept/Reject Methods

        [HttpGet]
        public async Task<IActionResult> ListWithdrawalRequest(long? TrnNo, DateTime? FromDate, DateTime? ToDate, short? Status)
        {
            ListWithdrawalAdminRes Response = new ListWithdrawalAdminRes();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _controlPanelServices.ListWithdrawalRequest(TrnNo, FromDate, ToDate, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRejectWithdrawalRequest([FromBody]WithdrwalAdminReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.StandardLoginfailed, ErrorCode = enErrorCode.StandardLoginfailed };
                }
                else
                {
                    Response = await _controlPanelServices.AdminWithdrawalRequest(Req.AdminReqId, Req.Bit, user.Id, Req.Remarks);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region ChargeFreeMarketCurrencyMaster
        [HttpGet]
        public async Task<IActionResult> ListChargeFreeMarketCurrencyMaster()
        {
            try
            {
                ListMarketCurrencyRes Response = new ListMarketCurrencyRes();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListChargeFreeMarketCurrencyMaster();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("{MarketCurrency}/{Status}")]
        public async Task<IActionResult> InsertChargeFreeMarketCurrencyMaster(string MarketCurrency, short Status)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.InsertChargeFreeMarketCurrencyMaster(MarketCurrency, user.Id, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        [HttpGet("{FromDate}/{ToDate}/{Page}/{PageSize}")]
        public async Task<IActionResult> ListLPWalletMismatch(DateTime FromDate, DateTime ToDate, int Page, int PageSize, long WalletId = 0, long SerProID = 0, Int16 Status = 999)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListLPWalletMismatchRes Response = new ListLPWalletMismatchRes();
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _controlPanelServices.ListLPWalletMismatch(FromDate, ToDate, Page, PageSize, WalletId, SerProID, Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> LPWalletRecon([FromBody]ReconRequest Request)
        //{
        //    ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        var accessToken = await HttpContext.GetTokenAsync("access_token");
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            Response = _lPWalletTransaction.ArbitrageRecon(Request, user.Id);
        //        }
        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        //[HttpGet("{SMSCode}")]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetLPProviderBalance(string SMSCode, long SerProID = 0, int GenerateMismatch = 0)
        //{
        //    ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //    LPServiceProBalanceResponse Response = new LPServiceProBalanceResponse();
        //    try
        //    {
        //        var accessToken = await HttpContext.GetTokenAsync("access_token");
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            Response = await _controlPanelServices.GetLPProviderBalance(SerProID, SMSCode, GenerateMismatch);
        //        }
        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}


        /// <summary>
        /// /vsolanki 2019-11-23 added new method for update pay u wallet coin limit per trnsaction
        /// </summary>
        /// <param name="Reqest"></param>
        /// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> UpdatePayUCoinLimitConfiguration([FromBody]ReqUpdatePayUCoinLimitConfiguration Request)
        //{
        //    try
        //    {
        //        BizResponseClass Response = new BizResponseClass();
        //        ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
        //        if (user == null)
        //        {
        //            Response.ReturnCode = enResponseCode.Fail;
        //            Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
        //            Response.ErrorCode = enErrorCode.StandardLoginfailed;
        //        }
        //        else
        //        {
        //            Response = _controlPanelServices.UpdatePayUCoinLimitConfiguration(Reqest);
        //        }
        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
        //    }
        //}

        #region
        [HttpPost]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateTextFile(CreateTextFileReq textFileReq)
        {
            BizResponseClass Response = new BizResponseClass();
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            //ApplicationUser user = new ApplicationUser(); user.Id = 3;
            try
            {
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _controlPanelServices.CreateTextFile(textFileReq);
                }
                return Ok(Response);
            }
            catch(Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region User Suspicious Activity Report
        //Rushabh 09-05-2020        
        [HttpGet]
        public async Task<IActionResult> GetTotalWithdrawalHistory(string CurrencyName)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                UserWithdrawalResponse Response = new UserWithdrawalResponse();
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {
                    Response = await _controlPanelServices.GetTotalWithdrawalHistory(user.Id, CurrencyName);
                }
                return Ok(Response);
            }
            catch (Exception Ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = Ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetTotalDepositionHistory(string CurrencyName)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                //user.Id = 67;
                UserWithdrawalResponse Response = new UserWithdrawalResponse();
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {
                    Response = await _controlPanelServices.GetTotalDepositionHistory(user.Id, CurrencyName);
                }
                return Ok(Response);
            }
            catch (Exception Ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = Ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllWalletBalance(string CurrencyName)
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                //user.Id = 17;
                UserAllWalletBalanceResp Response = new UserAllWalletBalanceResp();
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {
                    Response = await _controlPanelServices.GetAllWalletBalanceUserWise(user.Id, CurrencyName);
                }
                return Ok(Response);
            }
            catch (Exception Ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = Ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserTradeSummary()
        {
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                //user.Id = 17;
                UserTradingSummaryResp Response = new UserTradingSummaryResp();
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {
                    Response = await _controlPanelServices.GetUserTradeSummary(user.Id);
                }
                return Ok(Response);
            }
            catch (Exception Ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = Ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //Chirag 07/05/2020
        [HttpGet]
        public async Task<dynamic> GetActivityDataForSuspiciousUser(long UserID)
        {
            SuspiciousUserActivityResp Response = new SuspiciousUserActivityResp();
            try
            {
                //ApplicationUser user = new ApplicationUser(); user.Id = 1;
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _communicationService.GetSuspiciousUserData(UserID);
                }
                var respObj = JsonConvert.SerializeObject(Response);
                dynamic respObjJson = JObject.Parse(respObj);
                return Ok(respObjJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        //Chirag 08/05/2020
        [HttpGet]
        public async Task<IActionResult> GetCurrencyCummulativeData()
        {
            CurrencyCummulativeData Response = new CurrencyCummulativeData();
            try
            {
                //ApplicationUser user = new ApplicationUser(); user.Id = 1;
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _communicationService.GetCurrencyCummulativeData();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

    }
}