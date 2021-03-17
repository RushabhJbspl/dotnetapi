using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using Newtonsoft.Json;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class MarginWalletTypeMaster : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Key] //ntrivedi key added 18-04-2019 
        [Required]
        [StringLength(7)]
        [JsonProperty(PropertyName = "CoinName")]
        public string WalletTypeName { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        [Required]
        public short IsDepositionAllow { get; set; }

        [Required]
        public short IsWithdrawalAllow { get; set; }

        [Required]
        public short IsTransactionWallet { get; set; }

        
        public short? IsDefaultWallet { get; set; }

        public short? ConfirmationCount { get; set; }

        public short? IsLocal { get; set; }//add for Call ERC-20 API

        public void DisableStatus()
        {
            Status  = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new WalletStatusDisable<MarginWalletTypeMaster>(this));
        }
        public long CurrencyTypeID { get; set; } //ntrivedi 17-04-2019

    }


    public class ArbitrageWalletTypeMaster : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Key] //ntrivedi key added 18-04-2019 
        [Required]
        [StringLength(7)]
        [JsonProperty(PropertyName = "CoinName")]
        public string WalletTypeName { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        [Required]
        public short IsDepositionAllow { get; set; }

        [Required]
        public short IsWithdrawalAllow { get; set; }

        [Required]
        public short IsTransactionWallet { get; set; }

        public short IsLeaverageAllow { get; set; }

        public short? IsDefaultWallet { get; set; }

        public short? ConfirmationCount { get; set; }

        public short? IsLocal { get; set; }//add for Call ERC-20 API

        public void DisableStatus()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new WalletStatusDisable<ArbitrageWalletTypeMaster>(this));
        }
        public long CurrencyTypeID { get; set; } //ntrivedi 17-04-2019

    }
}
