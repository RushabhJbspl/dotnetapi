using Worldex.Core.Enums.Modes;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Modes
{
    public class Mode : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string ModeType { get; set; }
        public bool Status { get; private set; } = false;

        public void EndTimeUpdated()
        {
            Status = Convert.ToBoolean(ModeStatus.True);
            Events.Add(new ModeStatusEvent(this));
        }
    }
}
