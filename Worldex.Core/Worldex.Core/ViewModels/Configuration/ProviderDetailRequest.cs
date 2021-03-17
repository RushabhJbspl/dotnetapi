using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class ProviderDetailRequest : ProviderDetailViewModel  
    {
    }
    public class ProviderDetailResponse : BizResponseClass
    {
        public ProviderDetailGetAllResponse Response { get; set; }
    }
    public class ProviderDetailResponseList : BizResponseClass
    {
        public IEnumerable<ProviderDetailGetAllResponse> Response { get; set; }
    }
    public class ProviderDetailGetAllResponse
    {
        public long Id { get; set; }
        
        //Darshan dholakiya add ServiceProvider detail name:27-07-2019
        public string SerProDetailName { get; set; }

        //Darshan dholakiya add ServiceProvider detail name:27-07-2019
        public short Status { get; set; }    
        public ServiceProviderViewModel Provider { get; set;}
        public ProviderTypeViewModel ProviderType { get; set; }
        public AppTypeViewModel AppType { get; set; }

        //Rushabh 17-01-2020 return TrnType obj instead of id
        public long TrnType { get; set; }
        //public TrnTypeViewModel TrnType { get; set; }

        public LimitViewModel Limit { get; set; }
        public DemonconfigurationViewModel DemonConfiguration { get; set; }
        public ProviderConfigurationViewModel ProviderConfiguration { get; set; }
        public ThirdPartyAPIConfigViewModel   thirdParty { get; set; }

    }

    public class ProviderDetailResponse2 : BizResponseClass
    {
        public ProviderDetailGetAllResponse2 Response { get; set; }
    }
    public class ProviderDetailResponseList2 : BizResponseClass
    {
        public IEnumerable<ProviderDetailGetAllResponse2> Response { get; set; }
    }


    public class ProviderDetailGetAllResponse2
    {
        public long Id { get; set; }

        //Darshan dholakiya add ServiceProvider detail name:27-07-2019
        public string SerProDetailName { get; set; }

        //Darshan dholakiya add ServiceProvider detail name:27-07-2019
        public short Status { get; set; }
        public ServiceProviderViewModel Provider { get; set; }
        public ProviderTypeViewModel ProviderType { get; set; }
        public AppTypeViewModel AppType { get; set; }

        //Rushabh 17-01-2020 return TrnType obj instead of id
        //public long TrnType { get; set; }
        public TrnTypeViewModel TrnType { get; set; }

        public LimitViewModel Limit { get; set; }
        public DemonconfigurationViewModel DemonConfiguration { get; set; }
        public ProviderConfigurationViewModel ProviderConfiguration { get; set; }
        public ThirdPartyAPIConfigViewModel thirdParty { get; set; }

    }
}
