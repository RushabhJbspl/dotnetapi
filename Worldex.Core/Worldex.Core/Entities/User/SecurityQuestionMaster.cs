using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.User
{
   public class SecurityQuestionMaster : BizBaseExtended
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(200)]
        public string SecurityQuestion { get; set; }
        [Required]
        [StringLength(200)]
        public string Answer { get; set; }
    }
}
