using Worldex.Infrastructure.DTOClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.Interfaces
{
    public interface IActivityLogProcess
    {
        void AddActivityLog(ActivityReqRes activityReqRes);
        void UpdateActivityLogAsync(ActivityRes activityReqRes);
    }
}
