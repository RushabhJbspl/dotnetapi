using Worldex.Core.ApiModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class ServiceProviderViewModel
    {
        public long Id { get; set; }

        [Required]
        [StringLength(60)]
        public string ProviderName { get; set; }
        
        public short Status { get; set; }
    }
    //Darshan Dholakiya added changes for pagination=15/07/2019
    public class GetAllServiceProvideViewModel : BizResponseClass
    {
        public List<ServiceProviderViewModel> Response { get; set; }
        public long TotalPage { get; set; }
        public long PageSize { get; set; }
        public long Count { get; set; }
    }
}
