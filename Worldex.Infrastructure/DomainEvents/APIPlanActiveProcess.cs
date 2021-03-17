using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.FeedConfiguration;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.DomainEvents
{
    public class APIPlanActiveProcess : IRequestHandler<InvokeAPIPlanCroneObj>
    {
        private readonly ICommonRepository<UserSubscribeAPIPlan> _SubScribePlanRepository;
        private readonly IUserSubscribeAPIPlanProcess _IUserSubscribeAPIPlanProcess;
        private readonly IAPIConfigurationRepository _APIConfigurationRepository;

        public APIPlanActiveProcess(ICommonRepository<UserSubscribeAPIPlan> SubScribePlanRepository,
            IUserSubscribeAPIPlanProcess IUserSubscribeAPIPlanProcess, IAPIConfigurationRepository APIConfigurationRepository)
        {
            _SubScribePlanRepository = SubScribePlanRepository;
            _IUserSubscribeAPIPlanProcess = IUserSubscribeAPIPlanProcess;
            _APIConfigurationRepository = APIConfigurationRepository;
        }

        public Task<Unit> Handle(InvokeAPIPlanCroneObj request, CancellationToken cancellationToken)
        {
            //InvokeAPIPlanCroneObj response = new InvokeAPIPlanCroneObj();
            int IsActivePlan = 0;
            try
            {
                //ForAutoRenew 
                var AutoRenewPlanList = _SubScribePlanRepository.FindBy(e => e.Status == 1 && e.ExpiryDate.Value.Date <= request.Date.AddDays(e.RenewDays).Date && e.IsAutoRenew==1 && e.RenewStatus==0).ToList();
                if (AutoRenewPlanList.Count > 0)
                {
                    foreach (var obj in AutoRenewPlanList)
                    {
                        _IUserSubscribeAPIPlanProcess.PlanAutoRenewEntry(obj);
                    }
                }

                //Set Active, InActive
                var ActivePlanList = _SubScribePlanRepository.FindBy(e => e.Status == 1 && e.ExpiryDate.Value.Date <= request.Date.AddDays(-1).Date).ToList();
                if(ActivePlanList.Count>0)
                {
                    foreach (var Obj in ActivePlanList)
                    {
                        //var rDate = Obj.RenewDate.Value.Date;
                        if (Obj.RenewStatus==1)//Next renew plan
                        {
                            if (Obj.NextAutoRenewId != null && Obj.RenewDate.Value.Date == request.Date.Date)
                            {
                                var NextRenewPlan = _SubScribePlanRepository.FindBy(e => e.Id == Obj.NextAutoRenewId && e.Status == 0 && e.ActivationDate.Value.Date == request.Date.Date).FirstOrDefault();
                                if (NextRenewPlan != null)
                                {
                                    NextRenewPlan.Perticuler += " On " + request.Date.ToString();
                                    NextRenewPlan.Status = (short)enTransactionStatus.Success;
                                    NextRenewPlan.UpdatedBy = 999;
                                    _SubScribePlanRepository.Update(NextRenewPlan);
                                    IsActivePlan = 1;
                                }
                            }
                        }
                        Obj.Perticuler += " Expire On " + request.Date.ToString();
                        Obj.UpdatedBy = 999;
                        Obj.Status = (short)enTransactionStatus.InActive;
                        _SubScribePlanRepository.Update(Obj);
                        if (IsActivePlan == 0)
                            _APIConfigurationRepository.ExpireAllPlanAPIKey(Obj.APIPlanMasterID, Obj.UserID);
                    }
                }
                return Task.FromResult(new Unit());
            }
            catch (Exception ex)
            {
                return Task.FromResult(new Unit());
            }
        }
    }
}
