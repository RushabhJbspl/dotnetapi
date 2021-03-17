using Worldex.Core.Interfaces;
using Worldex.Core.ApiModels;
using Worldex.Infrastructure.Data.Transaction;

namespace Worldex.Infrastructure.Services
{   
    //Take Transaction Route Data
    public class TransactionWebAPIConfiguration : IWebApiData
    {
        private readonly WebApiDataRepository _webapiDataRepository;
        public TransactionWebAPIConfiguration(WebApiDataRepository webapiDataRepository)
        {
            _webapiDataRepository = webapiDataRepository;
        }

        public WebApiConfigurationResponse GetAPIConfiguration(long ThirPartyAPIID)
        {
            return _webapiDataRepository.GetThirdPartyAPIData(ThirPartyAPIID);            
        }            
    }
}
