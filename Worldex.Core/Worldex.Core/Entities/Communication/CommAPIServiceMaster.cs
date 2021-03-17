using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities
{
    public  class CommAPIServiceMaster : BizBase
    {
        [Required]
        public long CommServiceID { get; set; }

        [Required]
        [StringLength(200)]
        public string SenderID { get; set; }

        [Required]
        [StringLength(200)]
        public string SMSSendURL { get; set; }

        [StringLength(200)]
        public string SMSBalURL { get; set; }        

        [Required]
        public int Priority { get; set; }
    }
}
