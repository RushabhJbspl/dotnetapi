using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.ViewModels.APIConfiguration;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces.FeedConfiguration
{
    public interface IUserSubscribeAPIPlanProcess
    {
        Task<BizResponseClass> UserAPIPlanSubscribe(UserAPIPlanSubscribeRequest Request, long UserID);
        Task<BizResponseClass> APIPlanAutoRenewProcess(AutoRenewPlanRequest request, long UserID);
        Task<BizResponseClass> ManualRenewAPIPlan(ManualRenewAPIPlanRequest request, long UserID);
        Task<BizResponseClass> StopAutoRenewProcess(StopAutoRenewRequest request, long UserID);
        Task<BizResponseClass> PlanAutoRenewEntry(UserSubscribeAPIPlan Obj);
    }
}
