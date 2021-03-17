using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class ServiceDetailJsonData
    {
        public string ImageUrl { get; set; }
        public long TotalSupply { get; set; }
        public long MaxSupply { get; set; }
        [Required]
        public string WebsiteUrl { get; set; }
        public List<ExplorerData> Explorer { get; set; }
        public List<CommunityData> Community { get; set; }
        [Required]
        public string Introduction { get; set; }
    }
}
