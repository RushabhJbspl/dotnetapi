using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.LiquidityProvider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Worldex.Core.ViewModels.FiatBankIntegration;
using Worldex.Infrastructure.Data;

namespace BackofficeWorldex.Web.Api
{
    [Route("api/[controller]")]
    public class CacheController : Controller
    {
        private readonly IMessageConfiguration _messageConfiguration;
        private readonly ITrnMasterConfiguration _trnMasterConfiguration;
         private readonly IFrontTrnRepository _frontTrnRepository;
        private FiatIntegrateRepository _fiatIntegrateRepository;
        private IMemoryCache _cache { get; set; }
        public CacheController( IMessageConfiguration messageConfiguration, 
            ITrnMasterConfiguration trnMasterConfiguration, IFrontTrnRepository frontTrnRepository,
            IMemoryCache Cache, FiatIntegrateRepository fiatIntegrateRepository)
        {
            _messageConfiguration = messageConfiguration;
            _trnMasterConfiguration = trnMasterConfiguration;
            _cache = Cache;
            _frontTrnRepository = frontTrnRepository;
            _fiatIntegrateRepository = fiatIntegrateRepository;
        }
        
        [HttpGet("ReloadEmailMasterCache")]
        public async Task<IActionResult> ReloadEmailMasterCache()
        {
            try
            {

                IQueryable Result = await _messageConfiguration.GetAPIConfigurationAsync(Convert.ToInt32(enWebAPIRouteType.CommunicationAPI), Convert.ToInt32(enCommunicationServiceType.Email));
                var ConfigurationList = Result.Cast<CommunicationProviderList>().ToList().AsReadOnly();
                _cache.Set<IReadOnlyList<CommunicationProviderList>>("EmailConfiguration", ConfigurationList);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }

        [HttpGet("ReloadtemplateMasterCache")]
        public async Task<IActionResult> ReloadtemplateMasterCache()
        {
            try
            {
                IList<TemplateMasterData> Result = _messageConfiguration.GetTemplateConfigurationAsyncV1();
                IReadOnlyList<TemplateMasterData> ConfigurationList = Result.ToList().AsReadOnly();
                _cache.Set<IReadOnlyList<TemplateMasterData>>("TemplateConfiguration", ConfigurationList);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }

        [HttpGet("ReloadSMSMasterCache")]
        public async Task<IActionResult> ReloadSMSMasterCache()
        {
            try
            {

                IQueryable Result = await _messageConfiguration.GetAPIConfigurationAsync(Convert.ToInt32(enWebAPIRouteType.CommunicationAPI), Convert.ToInt32(enCommunicationServiceType.SMS));
                var ConfigurationList = Result.Cast<CommunicationProviderList>().ToList().AsReadOnly();
                _cache.Set<IReadOnlyList<CommunicationProviderList>>("SMSConfiguration", ConfigurationList);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }

        [HttpGet("LpfeedConfiguration")]
        public async Task<IActionResult> LpfeedConfiguration()
        {
            try
            {

                _trnMasterConfiguration.LPFeedConfigurationList();
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }

        [HttpGet("TradingConfiguration")]
        public async Task<IActionResult> TradingConfiguration()
        {
            try
            {

                _trnMasterConfiguration.TradingConfigurationList();
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }

        [HttpGet("ReloadCronMasterCache")]
        public async Task<IActionResult> ReloadCronMasterCache()
        {
            try
            {
                _trnMasterConfiguration.UpdateCronMasterList();
                return Ok(_trnMasterConfiguration.GetCronMaster());
            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }

        [HttpGet("TradePairConfigurationArbitrageV1Cache")]
        public async Task<IActionResult> TradePairConfigurationArbitrageV1Cache()
        {
            try
            {
                var PairList = _cache.Get<ConfigureLPArbitrage[]>("TradePairConfigurationArbitrageV1Cache");
                 PairList = _frontTrnRepository.GetLiquidityConfigurationDataArbitrage(0).ToArray();
                 _cache.Set<ConfigureLPArbitrage[]>("TradePairConfigurationArbitrageV1Cache", PairList);
             
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }

        [HttpGet("ReloadLTPdataCache")]
        public async Task<IActionResult> ReloadLTPdataCache()
        {
            try
            {
                List<LPTPairFiat> Result = _fiatIntegrateRepository.GetPairForBinnance();
                List<LPTPairFiat> ConfigurationList = Result.ToList();
                _cache.Set<List<LPTPairFiat>>("LPTPairFiat", ConfigurationList);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }
    }
}