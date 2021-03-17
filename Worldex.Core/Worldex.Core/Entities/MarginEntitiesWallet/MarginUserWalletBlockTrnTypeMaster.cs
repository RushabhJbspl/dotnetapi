using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class MarginUserWalletBlockTrnTypeMaster: BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Key]
        [Required]
        public long WalletID { get; set; }

        [Key]
        [Required]
        public long WTrnTypeMasterID { get; set; }

        [StringLength(150)]
        public string Remarks { get; set; }

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<MarginUserWalletBlockTrnTypeMaster>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<MarginUserWalletBlockTrnTypeMaster>(this));
        }
    }
}
