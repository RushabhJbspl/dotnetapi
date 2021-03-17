using Worldex.Core.ApiModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class APIPlanMethodConfigViewModel
    {
        public List<long> MethodID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11079")]
        public long APIPlanMasterID { get; set; }
        public short Status { get; set; }
    }
    public class APIPlanMethodConfigAddRequest : APIPlanMethodConfigViewModel
    {

    }
    public class APIPlanMethodConfigResponse : BizResponseClass
    {
        public List<APIPlanMethodConfigInfo> Response { get; set; }
    }
    public class APIPlanMethodConfigInfo
    {
        public long ID { get; set; }
        public string PlanName { get; set; }
        public string MethodName { get; set; }
        public short Status { get; set; }
    }
    public class APIRestMethodUpdateRequest
    {
        public long? ID { get; set; }
        public long MethodID { get; set; }
        public short Status { get; set; }
    }
}
