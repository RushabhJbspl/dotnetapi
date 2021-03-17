using Microsoft.AspNetCore.Http;

namespace Worldex.Core.Services
{
   public class UserResolveService
    {
        private readonly IHttpContextAccessor _context;
        public UserResolveService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetUser()
        {
            return _context.HttpContext?.User?.Identity?.Name;
        }
    }
}
