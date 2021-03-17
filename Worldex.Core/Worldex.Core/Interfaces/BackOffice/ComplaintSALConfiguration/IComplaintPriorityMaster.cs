using Worldex.Core.ViewModels.BackOffice.ComplaintSALConfiguration;

namespace Worldex.Core.Interfaces.BackOffice.ComplaintSALConfiguration
{
    public interface IComplaintPriorityMaster
    {
        long Add(ComplaintPriorityMasterreqViewModel ComplaintPriorityMaster);
        long IsComplaintPriorityExist(ComplaintPriorityMasterreqViewModel ComplaintPriorityMaster);
        long Update(ComplaintPriorityMasterupdatereqViewModel ComplaintPriorityMasterupdate);
        long Delete(ComplaintPriorityMasterDeleteReqViewModel ComplaintPriorityMasterDelete);
        ComplaintPrioritygetdataResponse GetComplaintPriority(int PageIndex = 0, int Page_Size = 0);
    }
}
