using Worldex.Core.ApiModels;
using Worldex.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Worldex.Infrastructure.DTOClasses;

namespace Worldex.Infrastructure.Services
{
    //Take Transaction Service Provider Data
    class ProviderDataList : IProviderDataList<TransactionApiConfigurationRequest, TransactionProviderResponse>
    {
        public IEnumerable<TransactionProviderResponse> GetProviderDataList(TransactionApiConfigurationRequest Request)
        {            
            throw new NotImplementedException();
        }

    }
}
