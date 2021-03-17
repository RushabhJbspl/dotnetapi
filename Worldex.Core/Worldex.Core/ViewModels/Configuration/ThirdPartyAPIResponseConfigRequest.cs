using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class ThirdPartyAPIResponseConfigRequest : ThirdPartyAPIResponseConfigViewModel
    {
        
    }
    public class ThirdPartyAPIResponseConfigResponse : BizResponseClass
    {
        public ThirdPartyAPIResponseConfigViewModel Response { get; set; }
    }
    public class ThirdPartyAPIResponseConfigResponseAllData : BizResponseClass
    {
        public List<ThirdPartyAPIResponseConfigViewModel> Response { get; set; }
        //Darshan dholakiya added parameters for pagination
        public long TotalPage { get; set; }
        public long PageSize { get; set; }
        public long Count { get; set; }
    }

    public class ThirdPartyAPIConfigurationResponseAllData : BizResponseClass //Create by jagdish 12-02-2020
    {
        public List<ThirdPartyAPIConfigurationResponseViewModel> Response { get; set; }
        //Darshan dholakiya added parameters for pagination
        public long TotalPage { get; set; }
        public long PageSize { get; set; }
        public long Count { get; set; }
    }
    public class ThirdPartyAPIConfigurationResponse : BizResponseClass // create by jagdish 12-02-2020
    {
        public ThirdPartyAPIConfigurationResponseViewModel Response { get; set; }
    }
    public class ThirdPartyResponseConfigurationRequest : ThirdPartyAPIConfigurationResponseViewModel // Add by jagdish 12-02-2020
    {

    }
}
