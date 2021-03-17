using Worldex.Core.ApiModels;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class GenerateAPIKeyRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11108")]
        public long PlanID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11109")]
        [StringLength(30)]
        public string AliasName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11110")]
        public short APIAccess { get; set; }
    }
    public class UpdateDeleteAPIKeyRequest
    {
        public long KeyID { get; set; }
    }
    public class GenerateAPIKeyResponse : BizResponseClass
    {
       public GenerateAPIKeyResponseInfo Response { get; set; }
    }
    public class GenerateAPIKeyResponseInfo
    {
        public string AliasName { get; set; }
        public string APIKey { get; set; }
        public string SecretKey { get; set; }
        public string QRCode { get; set; }
    }
}
