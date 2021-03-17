using Microsoft.AspNetCore.Http;
using AspNet.Security.OpenIdConnect.Primitives;
using Worldex.Infrastructure.Data;
using Worldex.Infrastructure;

namespace Worldex.Infrastructure.Data
{
    public class HttpUnitOfWork : UnitOfWork
    {
        public HttpUnitOfWork(WorldexContext context, IHttpContextAccessor httpAccessor) : base(context)
        {
            context.CurrentUserId = httpAccessor.HttpContext.User.FindFirst(OpenIdConnectConstants.Claims.Subject)?.Value?.Trim();
        }
    }
}
