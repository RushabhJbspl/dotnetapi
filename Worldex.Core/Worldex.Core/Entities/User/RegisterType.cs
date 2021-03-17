using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.User
{
    public class RegisterType : BizBase
    {
        public string Type { get; set; }

        public bool ActiveStatus { get; set; }

        public bool IsDeleted { get; set; }
    }
}
