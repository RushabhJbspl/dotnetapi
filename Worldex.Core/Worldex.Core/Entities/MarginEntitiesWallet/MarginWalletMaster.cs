using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.MarginEntitiesWallet
{
    public class MarginWalletMaster : BizBase
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Walletname { get; set; }

        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Balance { get; set; }

        [Required]
        public long WalletTypeID { get; set; }

        [Required]
        public bool IsValid { get; set; }

        [Required]
        [StringLength(16)]
        [Key]
        public string AccWalletID { get; set; } // dynamically generated accountid 

        [Required]
        public long UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string PublicAddress { get; set; }

        public byte IsDefaultWallet { get; set; }

        public DateTime? ExpiryDate { get; set; } //ntrivedi 14-12-2018

        public long? OrgID {get;set;}

        public EnWalletUsageType WalletUsageType { get; set; }

        public void CreditBalance(decimal amount)
        {
            Balance = Balance + amount;
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            Events.Add(new WalletCrEvent<MarginWalletMaster>(this));

        }
        public void DebitBalance(decimal amount)
        {
            Balance = Balance - amount;
            UpdatedDate = Helpers.Helpers.UTC_To_IST();
            Events.Add(new WalletDrEvent<MarginWalletMaster>(this));

        }
        public void WalletPublicAddress(string publicAddress)
        {
            PublicAddress = publicAddress;
            Events.Add(new WalletPublicAddress<MarginWalletMaster>(this));
        }

        [Required]
        [DefaultValue(0)]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal OutBoundBalance { get; set; }


        [Required]
        [DefaultValue(0)]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal InBoundBalance { get; set; }
    }
}

