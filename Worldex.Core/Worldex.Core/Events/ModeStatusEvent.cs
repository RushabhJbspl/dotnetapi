using Worldex.Core.Entities.Modes;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Events
{
    public class ModeStatusEvent : BaseDomainEvent
    {
        public Mode ModeUpdateItem { get; set; }

        public ModeStatusEvent(Mode modeUpdateItem)
        {
            ModeUpdateItem = modeUpdateItem;
        }
    }
}
