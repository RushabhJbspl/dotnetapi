using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.User
{
    public class SubscribeNewsLetter : BizBase
    {
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
    }
}
