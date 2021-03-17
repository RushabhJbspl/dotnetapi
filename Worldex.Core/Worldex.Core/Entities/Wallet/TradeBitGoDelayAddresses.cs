using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.Events;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Wallet
{
    public class TradeBitGoDelayAddresses : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new  long Id { get; set; }

        [Required]
        public long WalletId { get; set; }

        [Required]
        public long WalletTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Key]
        public string TrnID { get; set; }

        [Required]
        [StringLength(100)]      
        public string Address { get; set; }

        [Required]
        public byte GenerateBit { get; set; }

        [Required]
        [StringLength(5)]
        public string CoinName { get; set; }

        [Required]
        [StringLength(100)]
        public string BitgoWalletId { get; set; }

        [Required]
        [StringLength(250)]
        public string CoinSpecific { get; set; }
               
        public void GetAddressInStatusCheck(byte generateBit,string address,string coinSpecific,long WalletID)
        {
            GenerateBit = generateBit;
            Address = address;
            CoinSpecific = coinSpecific;
            WalletId = WalletID;
            Events.Add(new ServiceStatusEvent<TradeBitGoDelayAddresses>(this));
        }
    }

}
