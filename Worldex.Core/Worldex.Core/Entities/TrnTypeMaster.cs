using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities
{
    public class TrnTypeMaster :BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        [Key]
        public enTrnType TrnTypeId { get; set; }

        [Required]
        [StringLength(20)]
        public String TrnTypeName { get; set; }

        public void DisableTrnType()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<TrnTypeMaster>(this));
        }

        public void EnableTrnType()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<TrnTypeMaster>(this));
        }
    }
}
