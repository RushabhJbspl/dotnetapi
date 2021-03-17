using Worldex.Core.Interfaces.BackOffice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Handlers
{
    public class MustHaveAccessHandler : AuthorizationHandler<MustHaveAccess>
    {
        private readonly IRoleManagementServices _roleManagementServices;

        public MustHaveAccessHandler(IRoleManagementServices roleManagementServices)
        {
            _roleManagementServices = roleManagementServices;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustHaveAccess requirement)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;

            if (filterContext == null)
            {
                context.Fail();
                return Task.FromResult(0);
            }

            // Added manual check for user claim check as somehow RequiredAuthenticate Policy not working. -Nishit Jani on A 2019-03-30 2:02 PM
            var user = context.User;
            var userIsAnonymous = user?.Identity == null || !user.Identities.Any(i => i.IsAuthenticated);
            if (userIsAnonymous)
            {
                context.Fail();
                return Task.FromResult(0);
            }

            // get the sub claim
            var UserID = context.User.Claims.FirstOrDefault(x => x.Type == "sub").Value;
            var groupID = context.User.Claims.FirstOrDefault(c => c.Type == "GroupID").Value;

            var routeData = filterContext.RouteData.Values;
            string methodName = null;
            string controllerName = null;
            foreach (var data in routeData)
            {
                if (data.Key.ToLower() == "action")
                    methodName = data.Value.ToString();
                else if (data.Key.ToLower() == "controller")
                    controllerName = data.Value.ToString();
            }

            // Created below method to skip policy for methods which we supposed to use even Policy is not applicable. -Nishit Jani on A 2019-03-30 6:59 PM
            //if(_roleManagementServices.IsSkipAllow(methodName, controllerName))
            //{
            //    context.Succeed(requirement);
            //    return Task.FromResult(0);
            //}
            // Uncomment above code after discussion with Team BVN

            if (!_roleManagementServices.CheckUserHaveMenuAccess(Convert.ToInt64(groupID), methodName.ToUpper(), controllerName.ToUpper()))
            {
                context.Fail();
                return Task.FromResult(0);
            }

            context.Succeed(requirement);
            return Task.FromResult(0);
        }
    }
}
