using System.ComponentModel.DataAnnotations;
using Worldex.Core.Enums;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Wallet
{
    public class AddressMaster : BizBase
    {
        [Required]
        public long WalletId { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        public byte IsDefaultAddress { get; set; }

        [Required]
        public long SerProID { get; set; }

        [Required]
        [StringLength(50)]
        public string AddressLable { get; set; }

        [Required]
        [StringLength(200)]
        public string OriginalAddress { get; set; }

        public string GUID { get; set; }//used for store key (response from ERC-20 Api)

        public enAddressType AddressType { get; set; }//ntrivedi 18-04-2019

        [StringLength(150)]
        public string TxnID { get; set; }

        public string DestinationTag { get; set; }
    }

}
