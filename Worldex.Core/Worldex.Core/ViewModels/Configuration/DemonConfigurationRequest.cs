using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class DemonConfigurationRequest : DemonconfigurationViewModel 
    {
       
    }
    public class DemonConfigurationResponce : BizResponseClass
    {
        public DemonconfigurationViewModel Response { get; set; }
    }

    public class DemonconfigurationViewModel 
    {
        public long Id { get; set; }

        [Required]
        [StringLength(15)]
        public String IPAdd { get; set; }

        [Required]
        public int PortAdd { get; set; }

        [Required]
        [StringLength(200)]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        [Required]
        public short Status { get; set; }

        public string StatusText { get; set; }
    }
    public class ListDemonConfigResponse : BizResponseClass
    {
        public List<DemonconfigurationViewModel> Response { get; set; }
    }
    public class ListDemonConfigResponseV1 : BizResponseClass
    {
        public List<ListDEmonConfig> Response { get; set; }
    }
    public class ListDEmonConfig
    {
        public string Name { get; set; }
        public long Id { get; set; }
    }
}
