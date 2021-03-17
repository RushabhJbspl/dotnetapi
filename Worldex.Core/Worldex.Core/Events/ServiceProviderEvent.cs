using Worldex.Core.SharedKernel;

namespace Worldex.Core.Events
{
    public class ServiceProviderEvent<T> : BaseDomainEvent
    {
        public T ChangedProviderEvent { get; set; }

        public ServiceProviderEvent(T ChangedEvent)
        {
            ChangedProviderEvent = ChangedEvent;
        }
    }
}
