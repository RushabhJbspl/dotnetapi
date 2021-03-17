using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Communication
{
    public  class CommServiceproviderMaster : BizBase
    {
        [Required]
        public long CommServiceTypeID { get; set; }

        [Required]
        [StringLength(60)]
        public string SerproName { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        [Required]
        [StringLength(200)]
        public string Password { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Balance { get; set; }

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<CommServiceproviderMaster>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<CommServiceproviderMaster>(this));
        }
    }
}
