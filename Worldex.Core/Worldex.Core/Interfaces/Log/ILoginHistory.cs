using Worldex.Core.ViewModels.AccountViewModels.Log;
using System;

namespace Worldex.Core.Interfaces.Log
{
   public interface ILoginHistory
    {
         long AddLoginHistory(LoginhistoryViewModel model);
        LoginHistoryResponse GetLoginHistoryByUserId(long UserId, int pageIndex, int pageSize, string IPAddress = null, string Device = null, string Location = null, DateTime? FromDate = null, DateTime? ToDate = null);
    }
}
