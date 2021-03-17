using Worldex.Core.SharedKernel;

namespace Worldex.Core.Interfaces
{
    public interface IHandle<T> where T : BaseDomainEvent
    {
        void Handle(T domainEvent);
    }
}