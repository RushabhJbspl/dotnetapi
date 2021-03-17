using Worldex.Core.Events;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities
{
    public class ToDoItem : BaseEntity
    {
        public string Title { get; set; } 
        public string Description { get; set; }
        public bool IsDone { get; private set; } = false;
        public void MarkComplete()
        {
            IsDone = true;
            Events.Add(new ToDoItemCompletedEvent(this));
        }
    }
}