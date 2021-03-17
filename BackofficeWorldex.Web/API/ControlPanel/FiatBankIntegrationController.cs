using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.ViewModels.FiatBankIntegration;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Infrastructure.Interfaces;

namespace BackofficeWorldex.Web.API.ControlPanel
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class FiatBankIntegrationController : Controller
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly IFiatIntegration _FiatIntegration;
        private readonly IFiatIntegrateService _fiatIntegrateService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        public FiatBankIntegrationController(UserManager<ApplicationUser> UserManager, IFiatIntegration FiatIntegration, IFiatIntegrateService fiatIntegrateService, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _UserManager = UserManager;
            _FiatIntegration = FiatIntegration;
            _fiatIntegrateService = fiatIntegrateService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> ListUserBankRequest(short? Status, EnOperationType? RequestType)
        {
            ListUserBankReq Response = new ListUserBankReq();
            try
            {
                ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {
                    Response = await _FiatIntegration.ListUserBankDetail(Status, Convert.ToInt16(RequestType), 0);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRejectUserBankReq([FromBody] [Required] AdminApprovalReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else if (Req.Guid.Equals("00000000-0000-0000-0000-000000000000"))
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.InvalidInput;
                    Response.ErrorCode = enErrorCode.InvalidGUIDValue;
                }
                else
                {
                    Response = await _FiatIntegration.AcceptRejectUserBankRequest(Req, user.Id);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUpdateFiatTradeConfiguration([FromBody] [Required] FiatTradeConfigurationReq Req)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    if (Req.MinLimit > Req.MaxLimit)
                    {
                        Response.ReturnCode = enResponseCode.Fail;
                        Response.ReturnMsg = EnResponseMessage.FiatInvalidMinLimit;
                        Response.ErrorCode = enErrorCode.FiatInvalidMinLimit;
                    }
                    else
                    {
                        Response = await _FiatIntegration.AddUpdateFiatTradeConfiguration(Req, user.Id);
                    }
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListFiatTradeConfiguration(short? Status)
        {
            ListFiatTradeConfigurationRes Response = new ListFiatTradeConfigurationRes();
            try
            {
                ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                }
                else
                {
                    Response = await _FiatIntegration.GetFiatTradeConfiguration(Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> FiatBuyHistory(string FromCurrency, string ToCurrency, short? Status, string TrnId, DateTime? FromDate, DateTime? ToDate, string Email)
        {
            ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);//new ApplicationUser(); user.Id = 35; user.Email = "nishant@jbspl.com";//
            ListFiatBuyHistory Response = new ListFiatBuyHistory();
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
                    Response = _fiatIntegrateService.FiatBuyHistory(FromCurrency, ToCurrency, Status, TrnId, Email, FromDate, ToDate);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass
                {
                    ReturnCode = enResponseCode.InternalError,
                    ReturnMsg = ex.ToString(),
                    ErrorCode = enErrorCode.Status500InternalServerError
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> FiatSellHistory(string FromCurrency, string ToCurrency, short? Status, string TrnId, DateTime? FromDate, DateTime? ToDate, string Email)
        {
            ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);//new ApplicationUser(); user.Id = 35; user.Email = "nishant@jbspl.com";//
            ListFiatSellHistory Response = new ListFiatSellHistory();
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
                    Response = _fiatIntegrateService.FiatSellHistory(FromCurrency, ToCurrency, Status, TrnId, Email, FromDate, ToDate);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass
                {
                    ReturnCode = enResponseCode.InternalError,
                    ReturnMsg = ex.ToString(),
                    ErrorCode = enErrorCode.Status500InternalServerError
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertUpdateCryptoCurrencyConfiguration([FromBody][Required] ListFiatCoinConfigurationReq Req)
        {
            InsertUpdateCoinRes Response = new InsertUpdateCoinRes();
            try
            {
                ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {
                    Response = _FiatIntegration.InsertUpdateFiatConfiguration(Req, user.Id);
                    return Ok(Response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<IActionResult> ListCryptoCoinConfiguration(short? TransactionType)
        {
            ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);//new ApplicationUser();user.Id = 35;//await _UserManager.GetUserAsync(HttpContext.User);
            ListFiatCoinConfigurationRes Response = new ListFiatCoinConfigurationRes();
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
                    Response = _FiatIntegration.ListFiatConfiguration(null, null, null, TransactionType);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass
                {
                    ReturnCode = enResponseCode.InternalError,
                    ReturnMsg = ex.ToString(),
                    ErrorCode = enErrorCode.Status500InternalServerError
                });
            }
        }


        [HttpPost]
        [RequestSizeLimit(573741824)]
        public async Task<IActionResult> InsertUpdateFiatCurrency()
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ReturnMsg = EnResponseMessage.StandardLoginfailed;
                    Response.ErrorCode = enErrorCode.StandardLoginfailed;
                    return Ok(Response);
                }
                else
                {
                    ///
                    var httpRequest = Request.Form;
                    string fileName = "";
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
                    FiatCurrencyConfigurationReq Req = JsonConvert.DeserializeObject<FiatCurrencyConfigurationReq>(req);

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
                        string webRootPath = _configuration["CurrencyLogoPath"].ToString();
                        if (!Directory.Exists(webRootPath))
                        {
                            Directory.CreateDirectory(webRootPath);
                        }
                        string Extension = System.IO.Path.GetExtension(file.FileName);
                        fileName = ContentDispositionHeaderValue.Parse(postedFile.ContentDisposition).FileName.Trim('"');
                        string RootPath = Req.CurrencyCode;
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
                    ///
                    Response = _FiatIntegration.InsertUpdateFiatCurrency(Req, user.Id);
                    return Ok(Response);

                }
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListFiatCurrency(short? Status)
        {
            ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);
            ListFiatCurrencyInfo Response = new ListFiatCurrencyInfo();
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
                    Response = _fiatIntegrateService.GetFiatCurrencyInfoBO(Status);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass
                {
                    ReturnCode = enResponseCode.InternalError,
                    ReturnMsg = ex.ToString(),
                    ErrorCode = enErrorCode.Status500InternalServerError
                });
            }
        }
    }
}