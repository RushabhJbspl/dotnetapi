using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.CCXT;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.Services.CCXT
{
    public class CCXTCommonService : ICCXTCommonService
    {
        private readonly ILogger<CCXTCommonService> _logger;
        private readonly ICCXTCommonRepository _iCCXTCommonRepository;

        public CCXTCommonService(ILogger<CCXTCommonService> logger, ICCXTCommonRepository iCCXTCommonRepository)
        {
            _logger = logger;
            _iCCXTCommonRepository = iCCXTCommonRepository;
        }

        public List<CCXTTickerExchange> GetCCXTExchange()
        {
            try
            {
                return _iCCXTCommonRepository.GetCCXTExchange();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CCXTCommonService", "GetCCXTExchange ", ex);
                return null;
            }
        }

        public CCXTTickerQryObj InsertUpdateTickerData(CCXTTickerResObj TickerData)
        {
            try
            {
                return _iCCXTCommonRepository.InsertUpdateTickerData(TickerData);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CCXTCommonService", "InsertUpdateTickerData ", ex);
                return null;
            }
        }
    }
}
