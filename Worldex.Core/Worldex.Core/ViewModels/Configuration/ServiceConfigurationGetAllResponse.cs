using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class ServiceConfigurationGetAllResponse : BizResponseClass
    {
        public List<ServiceConfigurationRequest> Response { get; set; }
        public long TotalPage { get; set; }
        public long PageSize { get; set; }
        public long Count { get; set; }

    }
    public class GetServiceByBaseReasponse : BizResponseClass
    {
        public List<ServiceCurrencyData> Response { get; set; }
    }
    public class ServiceCurrencyData
    {
        public long ServiceId { get; set; }
        public string Name { get; set; }
        public string SMSCode { get; set; }
    }

}