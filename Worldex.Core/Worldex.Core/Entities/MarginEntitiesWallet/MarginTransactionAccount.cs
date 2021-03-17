using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Worldex.Core.Enums;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class MarginTransactionAccount : BizBase
    {
        [Required]
        public long BatchNo { get; set; }

        [Required]
        public long RefNo { get; set; }

        [Required]
        public DateTime TrnDate { get; set; }

        [Required]
        public long WalletID { get; set; } // accountid fk of walletmaster          

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal CrAmt { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal DrAmt { get; set; }

        [Required]
        [StringLength(1000)]
        public string Remarks { get; set; }

        [Required]
        public short IsSettled { get; set; }

        //ntrivedi added 30-11-2018
        [Required]
        [DefaultValue(1)]
        public enBalanceType Type { get; set; }

        public void SetAsSettled()
        {
            IsSettled = 1;
            Events.Add(new ServiceStatusEvent<MarginTransactionAccount>(this));
        }

        public MarginTransactionAccount() //ntrivedi 04-12-2018
        {
            Type = enBalanceType.AvailableBalance; // adding default type as available balance
        }

    }

}
