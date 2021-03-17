using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class MarginWalletAuthorizeUserMaster:BizBase
    {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        [Key]
        public long WalletID { get; set; }

        [Required]
        [Key]
        public long UserID { get; set; }

        [Required]
        public long OrgID { get; set; }

        [Required]  
        public long RoleID { get; set; }

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<MarginWalletAuthorizeUserMaster>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<MarginWalletAuthorizeUserMaster>(this));
        }
    }

    public class ArbitrageWalletAuthorizeUserMaster : BizBase
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        [Key]
        public long WalletID { get; set; }

        [Required]
        [Key]
        public long UserID { get; set; }

        [Required]
        public long OrgID { get; set; }

        [Required]
        public long RoleID { get; set; }

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<ArbitrageWalletAuthorizeUserMaster>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<ArbitrageWalletAuthorizeUserMaster>(this));
        }
    }
}
