using Worldex.Core.ApiModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class ProviderConfigurationRequest : ProviderConfigurationViewModel
    {
        
    }
    public class ProviderConfigurationResponse : BizResponseClass
    {
        public ProviderConfigurationViewModel Response { get; set; }
    }

    public class AllProConfigResponse : BizResponseClass
    {
        public List<ProviderConfigurationViewModel> Response { get; set; }
    }

    public class ListProConfigResponse : BizResponseClass
    {
        public List<ListProviderConfigInfo> Response { get; set; }
    }
    public class ListProviderConfigInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class ProviderConfigurationViewModel
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string AppKey { get; set; }

        [Required]
        [StringLength(50)]
        public string APIKey { get; set; }

        [Required]
        [StringLength(50)]
        public string SecretKey { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [Required]
        public short status { get; set; }

        public string StatusText { get; set; }

    }
}
