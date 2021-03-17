using Worldex.Core.ViewModels.AccountViewModels.Log;
using System;

namespace Worldex.Core.Interfaces.Log
{
    public interface IActivityLog
    {
        long AddActivityLog(ActivityLogViewModel model);
        ActivityLogResponse GetActivityLogHistoryByUserId(int UserId, int pageIndex, int pageSize, string Device = null, string Mode = null, string Location = null, DateTime? FromDate = null, DateTime? ToDate = null);
        long UpdateActivityLog(long Id, long UserID);
        ActivityLogResponse GetActivityLogHistoryAdmin(int PageIndex = 0, int Page_Size = 0, string Action = null, string Mode = null, string IPAddress = null, string Location = null, string UserName = null, DateTime? FromDate = null, DateTime? ToDate = null);
    }
}
