using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.Services.Handlers
{
    public class MustHaveAccess : IAuthorizationRequirement
    {
        public MustHaveAccess()
        {

        }
    }
}
