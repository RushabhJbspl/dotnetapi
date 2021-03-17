using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.Arbitrage;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Data.Transaction;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BackofficeWorldex.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly IBasePage _basePage;
        private readonly ILogger<TransactionController> _logger;
        private readonly IFrontTrnService _frontTrnService;
        
        public TransactionController(ILogger<TransactionController> logger, IBasePage basePage, IFrontTrnService frontTrnService)
        {
            _logger = logger;
            _basePage = basePage;
            _frontTrnService = frontTrnService;
        }

        [HttpGet("GetMarketTicker")]
        public ActionResult<GetMarketTickerResponse> GetMarketTicker(short IsMargin = 0)
        {
            try
            {
                var responsedata = _frontTrnService.GetMarketTicker(IsMargin);
                if (responsedata != null && responsedata.Count != 0)
                {
                    return new GetMarketTickerResponse() { ErrorCode= enErrorCode.Success ,ReturnCode= enResponseCode.Success ,ReturnMsg= "Success" ,Response= responsedata };
                }
                else
                {
                    return new GetMarketTickerResponse() { ErrorCode = enErrorCode.NoDataFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail", Response=new List<VolumeDataRespose>()};
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

    }
}