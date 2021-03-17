using Worldex.Core.SharedKernel;

namespace Worldex.Core.Events.Log
{
    public class LogTimeUpdateEvent<T> : BaseDomainEvent
    {
        public T LogUpdateItem { get; set; }

        public LogTimeUpdateEvent(T logUpdateItem)
        {
            LogUpdateItem = logUpdateItem;
        }
    }
}
