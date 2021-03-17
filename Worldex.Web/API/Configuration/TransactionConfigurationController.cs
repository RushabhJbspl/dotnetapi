using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Worldex.Web.API.Configuration
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] //for testing only //komal 10-06-2019 make authorize
    public class TransactionConfigurationController : ControllerBase
    {
        private readonly ITransactionConfigService _transactionConfigService;
        //private readonly ILogger<TransactionConfigurationController> _logger; //komal 03 May 2019, Cleanup
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBasePage _basePage;
        private readonly ITrnMasterConfiguration _trnMasterConfiguration;
        private IMemoryCache _cache;

        public TransactionConfigurationController(ITransactionConfigService transactionConfigService, UserManager<ApplicationUser> userManager,
            IBasePage basePage, ITrnMasterConfiguration trnMasterConfiguration, IMemoryCache cache)
        {
            _transactionConfigService = transactionConfigService;
            _userManager = userManager;
            _basePage = basePage;
            _trnMasterConfiguration = trnMasterConfiguration;
            _cache = cache;
        }

        #region Service
        [AllowAnonymous]
        [HttpGet("GetServiceConfiguration/{ServiceId}")]
        public ActionResult<ServiceConfigurationGetResponse> GetServiceConfiguration(long ServiceId, short IsMargin = 0)
        {
            ServiceConfigurationGetResponse Response = new ServiceConfigurationGetResponse();
            try
            {
                if (ServiceId == 0)
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.InValid_ID;
                    return Ok(Response);
                }
                ServiceConfigurationRequest responsedata = new ServiceConfigurationRequest();//_transactionConfigService.GetServiceConfiguration(ServiceId);
                if (IsMargin == 1)
                    responsedata = _transactionConfigService.GetServiceConfigurationMargin(ServiceId);
                else
                    responsedata = _transactionConfigService.GetServiceConfiguration(ServiceId);

                if (responsedata != null)
                {
                    Response.ReturnCode = enResponseCode.Success;
                    Response.Response = responsedata;
                    Response.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [AllowAnonymous]
        [HttpGet("GetAllServiceConfiguration")]
        public ActionResult<ServiceConfigurationGetAllResponse> GetAllServiceConfiguration(short IsMargin = 0)
        {
            ServiceConfigurationGetAllResponse Response = new ServiceConfigurationGetAllResponse();
            try
            {
                var responsedata = _transactionConfigService.GetAllServiceConfiguration(0, IsMargin);
                if (responsedata != null && responsedata.Count != 0)
                {
                    Response.ReturnCode = enResponseCode.Success;
                    Response.Response = responsedata;
                    Response.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        [AllowAnonymous]
        [HttpGet("GetAllServiceConfigurationData")]
        public ActionResult<ServiceConfigurationGetAllResponse> GetAllServiceConfigurationData(short IsMargin = 0, long CurrencyTypeId = 999)
        {
            ServiceConfigurationGetAllResponse Response = new ServiceConfigurationGetAllResponse();
            try
            {
                var responsedata = _transactionConfigService.GetAllServiceConfiguration(1, IsMargin, CurrencyTypeId);
                if (responsedata != null && responsedata.Count != 0)
                {
                    Response.ReturnCode = enResponseCode.Success;
                    Response.Response = responsedata;
                    Response.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NoDataFound;
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpPost("ListCurrency")]
        public ActionResult<GetServiceByBaseReasponse> ListCurrency(short IsMargin = 0, short ActiveOnly = 0)
        {
            GetServiceByBaseReasponse Response = new GetServiceByBaseReasponse();
            try
            {
                if (IsMargin == 1)
                    Response = _transactionConfigService.GetCurrencyMargin();
                else
                    Response = _transactionConfigService.GetCurrency(ActiveOnly);

                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        #endregion

        //Rita 16-7-19 added reload method , also  [AllowAnonymous] for run without Authorization
        #region Reload Master Configuration
        [AllowAnonymous]
        [HttpGet("UpdateServiceMasterInMemory")]
        public ActionResult UpdateServiceMasterInMemory(short IsMargin = 0)
        {
            try
            {
                if (IsMargin == 1)
                    _trnMasterConfiguration.UpdateServiceMarginList();
                else
                    _trnMasterConfiguration.UpdateServiceList();

                BizResponseClass Response = new BizResponseClass();
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Successfully Reloaded";
                return Ok(Response);

                //return Ok(_trnMasterConfiguration.GetServices());

            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [AllowAnonymous]
        [HttpGet("UpdateServiceProividerMasterInMemory")]
        public ActionResult UpdateServiceProividerMasterInMemory()
        {
            try
            {
                _trnMasterConfiguration.UpdateServiceProividerMasterList();

                BizResponseClass Response = new BizResponseClass();
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Successfully Reloaded";
                return Ok(Response);

                //return Ok(_trnMasterConfiguration.GetServiceProviderMaster());

            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [AllowAnonymous]
        [HttpGet("UpdateServiceProividerDetailInMemory")]
        public ActionResult UpdateServiceProividerDetailInMemory()
        {
            try
            {
                _trnMasterConfiguration.UpdateServiceProviderDetailList();

                BizResponseClass Response = new BizResponseClass();
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Successfully Reloaded";
                return Ok(Response);

                //return Ok(_trnMasterConfiguration.GetServiceProviderDetail());

            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [AllowAnonymous]
        [HttpGet("UpdateRouteConfigurationInMemory")]
        public ActionResult UpdateRouteConfigurationInMemory()
        {
            try
            {
                _trnMasterConfiguration.UpdateRouteConfigurationList();

                BizResponseClass Response = new BizResponseClass();
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Successfully Reloaded";
                return Ok(Response);

            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [AllowAnonymous]
        [HttpGet("UpdateTradePairMasterInMemory")]
        public ActionResult UpdateTradePairMasterInMemory(short IsMargin = 0)
        {
            try
            {
                if (IsMargin == 1)
                    _trnMasterConfiguration.UpdateTradePairMasterMarginList();
                else
                    _trnMasterConfiguration.UpdateTradePairMasterList();

                BizResponseClass Response = new BizResponseClass();
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Successfully Reloaded";
                return Ok(Response);

            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [AllowAnonymous]
        [HttpGet("UpdateTradePairDetailInMemory")]
        public ActionResult UpdateTradePairDetailInMemory(short IsMargin = 0)
        {
            try
            {
                if (IsMargin == 1)
                    _trnMasterConfiguration.UpdateTradePairDetailMarginList();
                else
                    _trnMasterConfiguration.UpdateTradePairDetailList();

                BizResponseClass Response = new BizResponseClass();
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Successfully Reloaded";
                return Ok(Response);

            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }
        //Rita 6-3-19 added for update cache data for Market
        [AllowAnonymous]
        [HttpGet("UpdateMarketInMemory")]
        public ActionResult UpdateMarketInMemory(short IsMargin = 0)
        {
            try
            {
                if (IsMargin == 1)
                    _trnMasterConfiguration.UpdateMarketMargin();
                else
                    _trnMasterConfiguration.UpdateMarket();

                BizResponseClass Response = new BizResponseClass();
                Response.ReturnCode = enResponseCode.Success;
                Response.ReturnMsg = "Successfully Reloaded";
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