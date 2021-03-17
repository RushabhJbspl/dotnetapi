using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.UserChangeLog
{
    public class UserLogChange : BizBase
    {
        public long UserId { get; set; }
        public string Type { get; set; }
        public string Oldvalue { get; set; }
        public string Newvalue { get; set; }
    }
}
