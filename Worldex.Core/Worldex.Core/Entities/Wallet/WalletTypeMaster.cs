﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using Newtonsoft.Json;

namespace Worldex.Core.Entities.Wallet
{
    public class WalletTypeMaster : BizBase
    {        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        [Key]
        [StringLength(10)]
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

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]//always in USD prize
        public decimal Rate { get; set; }

        public void DisableStatus()
        {
            Status  = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new WalletStatusDisable<WalletTypeMaster>(this));
        }
                
        public long CurrencyTypeID { get; set; }
    }
   
}
