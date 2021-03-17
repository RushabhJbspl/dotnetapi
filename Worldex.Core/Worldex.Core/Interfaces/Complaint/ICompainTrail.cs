using Worldex.Core.ViewModels.BackOfficeComplain;
using Worldex.Core.ViewModels.Complaint;

namespace Worldex.Core.Interfaces.Complaint
{
   public interface ICompainTrail
    {
        long AddCompainTrail(CompainTrailReqVirewModel compainTrail);
        long AddBackOffComMaster(BackOffAddCom model,int UserId);
    }
}
