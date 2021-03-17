using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration.FeedConfiguration
{
    public class UserAPIKeyDetails :BizBase
    {
        [Required]
        [StringLength(50)]
        public string AliasName { get; set; }
        public short APIPermission { get; set; }
        public string SecretKey { get; set; }
        public string APIKey { get; set; }
        public short IPAccess { get; set; }
        public string QRCode { get; set; }
        public long APIPlanMasterID { get; set; }
        public long UserID { get; set; }
    }

    public class WhiteListIPEndPoint :BizBase
    {
        public long APIKeyDetailsID { get; set; }
        public long APIPlanID { get; set; }
        [Required]
        [StringLength(50)]
        public string AliasName { get; set; }
        [Required]
        public string IPAddress { get; set; }
        public short IPType { get; set; }
        public long UserID { get; set; }
    }
    public class APIKeyWhitelistIPConfig : BizBase
    {
        public long APIKeyID { get; set; }
        public long IPId { get; set; }
    }
}
