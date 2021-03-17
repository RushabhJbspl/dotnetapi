using Worldex.Core.ViewModels.BackOfficeComplain;
using Worldex.Core.ViewModels.Complaint;
using System;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.Complaint
{
     public interface IComplainmaster
    {
        long AddComplainmaster(ComplainmasterReqViewModel model);
        ComplainChildParentViewmodel GetComplain(int UserId);
        List<UserWiseCompaintDetailResponce> GetComplainByUserWise(int UserId, string Subject = null, DateTime? FromDate = null, DateTime? ToDate = null);
        GetBackOffComRptResponse GetBackofficeAllData(int UserId, int pageIndex, int pageSize, long ComplainId=0, string EmailId=null, string MobileNo = null, long Status = 0, long TypeId = 0, int PriorityId = 0, DateTime? FromDate = null, DateTime? ToDate = null);
        GetComplainAllData GetBackOfficeComplain(long ComplainId);
        GetTotalCountCom GetTotalComplainCount(long Type = 0, short ComplainStatus = 0,int UserId =0);
    }
}
