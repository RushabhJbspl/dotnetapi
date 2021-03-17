using Worldex.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class IPWhiteListRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11108")]
        public long PlanID { get; set; }
        public long APIKeyID { get; set; }
        public List<IPListRequest> IPList { get; set; }
    }
    public class IPListRequest
    {
        public long ID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11109")]
        [StringLength(30)]
        public string AliasName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11107")]
        public string IPAddress { get; set; }
        [Required]
        public enIPType IPType { get; set; }
    }
}
