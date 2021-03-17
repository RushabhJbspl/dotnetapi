using Ardalis.GuardClauses;
using Worldex.Core.Events;
using Worldex.Core.Interfaces;

namespace Worldex.Core.Services
{
    public class ToDoItemService : IHandle<ToDoItemCompletedEvent>
    {
        public void Handle(ToDoItemCompletedEvent domainEvent)
        {
            Guard.Against.Null(domainEvent, nameof(domainEvent));
        }
    }
}
