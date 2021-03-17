using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration.FeedConfiguration
{
    public class SocketMethods : BizBase
    {
        [Required]
        [StringLength(30)]
        public string MethodName { get; set; }

        [Required]
        [StringLength(30)]
        public string ReturnMethodName { get; set; }

        public short PublicOrPrivate { get; set; }

        public short EnumCode { get; set; }
    }
}
