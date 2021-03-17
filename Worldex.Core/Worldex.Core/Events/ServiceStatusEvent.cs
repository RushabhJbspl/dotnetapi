using Worldex.Core.SharedKernel;

namespace Worldex.Core.Events
{
    public class ServiceStatusEvent<T> : BaseDomainEvent
    {
        public T ChangedServiceStatus { get; set; }

        public ServiceStatusEvent(T ChangedStatus)
        {
            ChangedServiceStatus = ChangedStatus;
        }
    }
}