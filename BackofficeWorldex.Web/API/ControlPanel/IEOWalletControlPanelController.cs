using Worldex.Core.ApiModels;
using System.Threading.Tasks;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Worldex.Core.ViewModels.ControlPanel;
using Microsoft.AspNetCore.Http;
using System;
using Worldex.Core.ViewModels.IEOWallet;
using System.Net.Http.Headers;
using Worldex.Core.Helpers;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace BackofficeWorldex.Web.API.ControlPanel
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class IEOWalletControlPanelController : Controller
    {
        #region Cotr
        private readonly IIEOWalletService _IEOService;
        Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        public IEOWalletControlPanelController(UserManager<ApplicationUser> userManager, IIEOWalletService IEOService, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _userManager = userManager;
            _IEOService = IEOService;
            _configuration = configuration;
        }
        #endregion

        #region Banner Configuration
        [HttpPost]
        [RequestSizeLimit(573741824)] // e.g. 500 MB request limit
        public async Task<IActionResult> InsertUpdateBannerConfiguration()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
            IEOBannerRequest request = new IEOBannerRequest();
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
                    var httpRequest = Request.Form;

                    if (String.IsNullOrEmpty(httpRequest["Id"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.NotFoundBannerId;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter Id";
                        return Ok(Response);
                    }
                    if (String.IsNullOrEmpty(httpRequest["BannerName"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.NotFoundBannerName;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter BannerName";
                        return Ok(Response);
                    }
                    if (String.IsNullOrEmpty(httpRequest["Description"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.NotFoundBannerDescription;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter Description";
                        return Ok(Response);
                    }
                    if (String.IsNullOrEmpty(httpRequest["Message"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.NotFoundBannerMessage;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter Message";
                        return Ok(Response);
                    }
                    if (String.IsNullOrEmpty(httpRequest["TermsAndCondition"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.NotFoundBannerTermsAndCondition;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter TermsAndCondition";
                        return Ok(Response);
                    }
                    if (String.IsNullOrEmpty(httpRequest["IsKYCReuired"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.NotFoundBannerIsKYCReuired;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter IsKYCReuired";
                        return Ok(Response);
                    }
                    if (String.IsNullOrEmpty(httpRequest["Status"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.NotFoundBannerStatus;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter Status";
                        return Ok(Response);
                    }
                    request.Id = Convert.ToInt64(httpRequest["Id"]);
                    string fileName = "";
                    if (request.Id == 0)
                    {
                        if (httpRequest.Files.Count == 0)
                        {
                            Response.ErrorCode = enErrorCode.BannerNotUpload;
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ReturnMsg = "Please Upload Image";
                            return Ok(Response);
                        }
                        var file = httpRequest.Files[0];
                        string data = System.IO.Path.GetExtension(file.FileName);
                        data = data.ToUpper();
                        data = data.Substring(1);

                        if (data != "PNG" && data != "JPG" && data != "JPEG")
                        {
                            Response.ErrorCode = enErrorCode.NotFound;
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ReturnMsg = "You Can Not Upload An Image Other Than PNG/JPG/JPEG";
                            return Ok(Response);
                        }

                        var postedFile = httpRequest.Files[file.Name];
                        string webRootPath = _configuration["IEOBannerImagePath"].ToString();
                        if (!Directory.Exists(webRootPath))
                        {
                            Directory.CreateDirectory(webRootPath);
                        }
                        string Extension = System.IO.Path.GetExtension(file.FileName);
                        fileName = ContentDispositionHeaderValue.Parse(postedFile.ContentDisposition).FileName.Trim('"');
                        string RootPath = "Banner" + "_" + Helpers.GetTimeStamp();
                        fileName = fileName.Replace(fileName, RootPath + Extension);

                        string fullPath = webRootPath + fileName;
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await postedFile.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        if (httpRequest.Files.Count > 0)
                        {
                            var file = httpRequest.Files[0];
                            string data = System.IO.Path.GetExtension(file.FileName);
                            data = data.ToUpper();
                            data = data.Substring(1);

                            if (data != "PNG" && data != "JPG" && data != "JPEG")
                            {
                                Response.ErrorCode = enErrorCode.NotFound;
                                Response.ReturnCode = enResponseCode.Fail;
                                Response.ReturnMsg = "You Can Not Upload An Image Other Than PNG/JPG/JPEG";
                                return Ok(Response);
                            }

                            var postedFile = httpRequest.Files[file.Name];
                            string webRootPath = _configuration["IEOBannerImagePath"].ToString();
                            if (!Directory.Exists(webRootPath))
                            {
                                Directory.CreateDirectory(webRootPath);
                            }
                            string Extension = System.IO.Path.GetExtension(file.FileName);
                            fileName = ContentDispositionHeaderValue.Parse(postedFile.ContentDisposition).FileName.Trim('"');
                            string RootPath = "Banner" + "_" + Helpers.GetTimeStamp();
                            fileName = fileName.Replace(fileName, RootPath + Extension);

                            string fullPath = webRootPath + fileName;
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                            {
                                await postedFile.CopyToAsync(stream);
                            }
                        }
                    }

                    request.Description = httpRequest["Description"].ToString();
                    request.BannerName = httpRequest["BannerName"].ToString();
                    request.Message = httpRequest["Message"].ToString();
                    request.TermsAndCondition = httpRequest["TermsAndCondition"].ToString();
                    request.IsKYCReuired = Convert.ToInt16(httpRequest["IsKYCReuired"]);
                    request.Status = Convert.ToInt16(httpRequest["Status"]);

                    Response = _IEOService.InsertUpdateBannerConfiguration(request, user.Id, fileName);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBannerConfiguration()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            GetIEOBannerRes Response = new GetIEOBannerRes();
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
                    Response = _IEOService.GetBannerConfiguration();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Admin Wallet configuration

        [HttpPost]
        [RequestSizeLimit(573741824)] // e.g. 500 MB request limit   
        public async Task<IActionResult> InsertUpdateAdminWalletConfiguration()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
            IEOAdminWalletRequest request = new IEOAdminWalletRequest();
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
                    var httpRequest = Request.Form;

                    if (String.IsNullOrEmpty(httpRequest["Id"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.NotFoundId;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter Id";
                        return Ok(Response);
                    }
                    if (String.IsNullOrEmpty(httpRequest["Description"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.IEONotFoundDescription;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter Description";
                        return Ok(Response);
                    }
                    if (String.IsNullOrEmpty(httpRequest["Rate"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.IEONotFoundRate;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter Rate";
                        return Ok(Response);
                    }
                    if (String.IsNullOrEmpty(httpRequest["WalletName"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.NotFoundWalletName;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter WalletName";
                        return Ok(Response);
                    }
                    if (String.IsNullOrEmpty(httpRequest["ShortCode"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.NotFoundShortCode;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter ShortCode";
                        return Ok(Response);
                    }
                    if (String.IsNullOrEmpty(httpRequest["CoinType"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.NotFoundCoinType;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter CoinType";
                        return Ok(Response);
                    }
                    if (String.IsNullOrEmpty(httpRequest["Status"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.NotFoundStatus;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter Status";
                        return Ok(Response);
                    }
                    if (String.IsNullOrEmpty(httpRequest["Rounds"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.NotFoundRounds;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter Rounds";
                        return Ok(Response);
                    }
                    request.Id = Convert.ToInt64(httpRequest["Id"]);
                    string fileName = "";
                    if (request.Id == 0)
                    {
                        if (httpRequest.Files.Count == 0)
                        {
                            Response.ErrorCode = enErrorCode.WalletImageNotUpload;
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ReturnMsg = "Please Upload Image";
                            return Ok(Response);
                        }
                        var file = httpRequest.Files[0];
                        string data = System.IO.Path.GetExtension(file.FileName);
                        data = data.ToUpper();
                        data = data.Substring(1);

                        if (data != "PNG" && data != "JPG" && data != "JPEG")
                        {
                            Response.ErrorCode = enErrorCode.NotFound;
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ReturnMsg = "You Can Not Upload An Image Other Than PNG/JPG/JPEG";
                            return Ok(Response);
                        }

                        var postedFile = httpRequest.Files[file.Name];
                        string webRootPath = _configuration["IEOWalletImagePath"].ToString();
                        if (!Directory.Exists(webRootPath))
                        {
                            Directory.CreateDirectory(webRootPath);
                        }
                        string Extension = System.IO.Path.GetExtension(file.FileName);
                        fileName = ContentDispositionHeaderValue.Parse(postedFile.ContentDisposition).FileName.Trim('"');
                        string RootPath = "Wallet" + "_" + Helpers.GetTimeStamp();
                        fileName = fileName.Replace(fileName, RootPath + Extension);

                        string fullPath = webRootPath + fileName;
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await postedFile.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        if (httpRequest.Files.Count > 0)
                        {
                            var file = httpRequest.Files[0];
                            string data = System.IO.Path.GetExtension(file.FileName);
                            data = data.ToUpper();
                            data = data.Substring(1);

                            if (data != "PNG" && data != "JPG" && data != "JPEG")
                            {
                                Response.ErrorCode = enErrorCode.NotFound;
                                Response.ReturnCode = enResponseCode.Fail;
                                Response.ReturnMsg = "You Can Not Upload An Image Other Than PNG/JPG/JPEG";
                                return Ok(Response);
                            }

                            var postedFile = httpRequest.Files[file.Name];
                            string webRootPath = _configuration["IEOWalletImagePath"].ToString();
                            if (!Directory.Exists(webRootPath))
                            {
                                Directory.CreateDirectory(webRootPath);
                            }
                            string Extension = System.IO.Path.GetExtension(file.FileName);
                            fileName = ContentDispositionHeaderValue.Parse(postedFile.ContentDisposition).FileName.Trim('"');
                            string RootPath = "Wallet" + "_" + Helpers.GetTimeStamp();
                            fileName = fileName.Replace(fileName, RootPath + Extension);

                            string fullPath = webRootPath + fileName;
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                            {
                                await postedFile.CopyToAsync(stream);
                            }
                        }
                    }
                    request.WalletName = httpRequest["WalletName"].ToString();
                    request.ShortCode = httpRequest["ShortCode"].ToString();
                    request.CoinType = httpRequest["CoinType"].ToString();
                    request.Description = httpRequest["Description"].ToString();
                    request.WalletPath = fileName;
                    request.Status = Convert.ToInt16(httpRequest["Status"]);
                    request.Rounds = Convert.ToInt16(httpRequest["Rounds"]);
                    request.Rate = Convert.ToDecimal(httpRequest["Rate"]);
                    if (request.ShortCode.Length > 7)
                    {
                        Response.ErrorCode = enErrorCode.MaxShortCode;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Maximum 7 character allowed.";
                        return Ok(Response);
                    }
                    Response = _IEOService.InsertUpdateAdminWalletConfiguration(request, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAdminWalletConfiguration()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListGetIEOAdminWalletRes Response = new ListGetIEOAdminWalletRes();
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
                    Response = _IEOService.GetAdminWalletConfiguration(user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Round Configuration
        [HttpPost]
        public async Task<IActionResult> InsertRoundConfiguration()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
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
                    var httpRequest = Request.Form;

                    if (String.IsNullOrEmpty(httpRequest["Request"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.InValidRoundRequest;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter Valid Request.";
                        return Ok(Response);
                    }
                    var req = httpRequest["Request"].ToString();
                    bool returnValue;
                    try
                    {
                        var js = JToken.Parse(req);
                        returnValue = true;
                    }
                    catch
                    {
                        returnValue = false;
                    }
                    if (returnValue == false)
                    {
                        Response.ErrorCode = enErrorCode.InValidRoundJsonRequest;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter Valid Request Format.";
                        return Ok(Response);
                    }
                    InsertRoundConfigurationReq request = JsonConvert.DeserializeObject<InsertRoundConfigurationReq>(req);

                    string fileName = "";

                    if (httpRequest.Files.Count == 0)
                    {
                        Response.ErrorCode = enErrorCode.WalletImageNotUpload;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Upload Image";
                        return Ok(Response);
                    }
                    var file = httpRequest.Files[0];
                    string data = System.IO.Path.GetExtension(file.FileName);
                    data = data.ToUpper();
                    data = data.Substring(1);

                    if (data != "PNG" && data != "JPG" && data != "JPEG")
                    {
                        Response.ErrorCode = enErrorCode.NotFound;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "You Can Not Upload An Image Other Than PNG/JPG/JPEG";
                        return Ok(Response);
                    }

                    var postedFile = httpRequest.Files[file.Name];
                    string webRootPath = _configuration["IEOCurrencyImagePath"].ToString();
                    if (!Directory.Exists(webRootPath))
                    {
                        Directory.CreateDirectory(webRootPath);
                    }
                    string Extension = System.IO.Path.GetExtension(file.FileName);
                    fileName = ContentDispositionHeaderValue.Parse(postedFile.ContentDisposition).FileName.Trim('"');
                    string RootPath = "BGCurrency" + "_" + Helpers.GetTimeStamp();
                    fileName = fileName.Replace(fileName, RootPath + Extension);

                    string fullPath = webRootPath + fileName;
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(stream);
                    }
                    Response = _IEOService.InsertRoundConfiguration(request, user.Id, fileName);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoundConfiguration()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
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
                    var httpRequest = Request.Form;

                    if (String.IsNullOrEmpty(httpRequest["Request"].ToString()))
                    {
                        Response.ErrorCode = enErrorCode.InValidRoundRequest;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter Valid Request.";
                        return Ok(Response);
                    }
                    var req = httpRequest["Request"].ToString();
                    bool returnValue;
                    try
                    {
                        var js = JToken.Parse(req);
                        returnValue = true;
                    }
                    catch
                    {
                        returnValue = false;
                    }
                    if (returnValue == false)
                    {
                        Response.ErrorCode = enErrorCode.InValidRoundJsonRequest;
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = "Please Enter Valid Request Format.";
                        return Ok(Response);
                    }
                    UpdateRoundConfigurationReq request = JsonConvert.DeserializeObject<UpdateRoundConfigurationReq>(req);

                    string fileName = "";

                    if (httpRequest.Files.Count > 0)
                    {
                        var file = httpRequest.Files[0];
                        string data = System.IO.Path.GetExtension(file.FileName);
                        data = data.ToUpper();
                        data = data.Substring(1);

                        if (data != "PNG" && data != "JPG" && data != "JPEG")
                        {
                            Response.ErrorCode = enErrorCode.NotFound;
                            Response.ReturnCode = enResponseCode.Fail;
                            Response.ReturnMsg = "You Can Not Upload An Image Other Than PNG/JPG/JPEG";
                            return Ok(Response);
                        }

                        var postedFile = httpRequest.Files[file.Name];
                        string webRootPath = _configuration["IEOCurrencyImagePath"].ToString();
                        if (!Directory.Exists(webRootPath))
                        {
                            Directory.CreateDirectory(webRootPath);
                        }
                        string Extension = System.IO.Path.GetExtension(file.FileName);
                        fileName = ContentDispositionHeaderValue.Parse(postedFile.ContentDisposition).FileName.Trim('"');
                        string RootPath = "BGCurrency" + "_" + Helpers.GetTimeStamp();
                        fileName = fileName.Replace(fileName, RootPath + Extension);

                        string fullPath = webRootPath + fileName;
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await postedFile.CopyToAsync(stream);
                        }
                    }

                    Response = _IEOService.UpdateRoundConfiguration(request, user.Id, fileName);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region ListIEOWallet
        [HttpGet]
        public async Task<IActionResult> ListRoundConfiguration(Int16 Status = 999)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListRoundConfigurationResponse Response = new ListRoundConfigurationResponse();
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
                    Response = _IEOService.ListIEORoundConfiguration(Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        #endregion

        #region Admin wallet depoosit

        [HttpPost]
        public async Task<IActionResult> IEOAdminWalletDeposit([FromBody]IEOAdminWalletCreditReq Req)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            BizResponseClass Response = new BizResponseClass();
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
                    Response = _IEOService.IEOAdminWalletDeposit(Req);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Count Method

        [HttpGet("{IsAllocate}")]
        public async Task<IActionResult> IEOTokenCount(short IsAllocate)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListAllocateTokenCountRes Response = new ListAllocateTokenCountRes();
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
                    Response = _IEOService.IEOTokenCount(IsAllocate);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> IEOTradeTokenCount()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListTokenCountRes Response = new ListTokenCountRes();
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
                    Response = _IEOService.IEOTradeTokenCount();
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region IEO Report

        [HttpGet("{PageNo}/{PageSize}")]
        public async Task<IActionResult> IEOTokenReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, string Email, string PaidCurrency, string DeliveredCurrency, short? Status, string TrnRefNo)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListIEOTokenReportDataRes Response = new ListIEOTokenReportDataRes();
            try
            {
                if (Email != null)
                {
                    Regex regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
                    bool isValid = regex.IsMatch(Email.Trim());
                    if (!isValid)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.EmailFail;
                        Response.ErrorCode = enErrorCode.Status4087EmailFail;
                        return Ok(Response);
                    }
                }
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _IEOService.IEOTokenReport(PageNo, PageSize, FromDate, ToDate, Email, PaidCurrency, DeliveredCurrency, Status, TrnRefNo);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("{PageNo}/{PageSize}")]
        public async Task<IActionResult> IEOAllocatedTokenReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, string Email, string PaidCurrency, string DeliveredCurrency, short? Status, string TrnRefNo)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListIEOAllocatedTokenReportDataRes Response = new ListIEOAllocatedTokenReportDataRes();
            try
            {
                if (Email != null)
                {
                    Regex regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
                    bool isValid = regex.IsMatch(Email.Trim());
                    if (!isValid)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.EmailFail;
                        Response.ErrorCode = enErrorCode.Status4087EmailFail;
                        return Ok(Response);
                    }
                }
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _IEOService.IEOAllocatedTokenReport(PageNo, PageSize, FromDate, ToDate, Email, PaidCurrency, DeliveredCurrency, Status, TrnRefNo);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        #region Purchase History

        [HttpGet("{FromDate}/{ToDate}/{Page}/{PageSize}")]
        public async Task<IActionResult> TransactionHistory(DateTime FromDate, DateTime ToDate, int Page, int PageSize, Int64 PaidCurrency = 0, Int64 DeliveryCurrency = 0, string Email = null)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ListIEOPurchaseHistoryResponseBO Response = new ListIEOPurchaseHistoryResponseBO();
            try
            {
                if (Email != null)
                {
                    Regex regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
                    bool isValid = regex.IsMatch(Email.Trim());
                    if (!isValid)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.EmailFail;
                        Response.ErrorCode = enErrorCode.Status4087EmailFail;
                        return Ok(Response);
                    }
                }
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = _IEOService.ListPurchaseHistoryBO(FromDate, ToDate, Page, PageSize, PaidCurrency, DeliveryCurrency, Email);
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
