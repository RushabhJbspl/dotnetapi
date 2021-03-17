using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Backoffice.PasswordPolicy
{
   public class UserLinkMaster : BizBaseExtended
    {
        public string UserLinkData { get; set; }
        public int UserId { get; set; }
        public int LinkvalidTime { get; set; }
    }
}
