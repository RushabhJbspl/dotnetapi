using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.WalletConfiguration
{
    public class WalletTypeMasterRequest 
    {
        [Required(ErrorMessage = "1,Please Enter Required parameters,4200")]
        [StringLength(50, ErrorMessage = "1,Please enter a valid parameters,4201")]
        public string WalletTypeName { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required parameters,4202")]
        [StringLength(100, ErrorMessage = "1,Please enter a valid parameters,4203")]
        public string Description { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required parameters,4204")]
        public short IsDepositionAllow { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required parameters,4205")]
        public short IsWithdrawalAllow { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required parameters,4206")]
        public short IsTransactionWallet { get; set; }

        public short Status { get; set; }

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14161"), DataType(DataType.Currency)]
        //[Required(ErrorMessage = "1,Please Enter Required parameters,4206")]
        public decimal Rate { get; set; }

        //[Required(ErrorMessage = "1,Please Enter Required parameters,4206")]
        public long CurrencyTypeId { get; set; }
    }

    public class WalletTypeMasterUpdateRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required parameters,4202")]
        [StringLength(100, ErrorMessage = "1,Please enter a valid parameters,4203")]
        public string Description { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required parameters,4204")]
        public short IsDepositionAllow { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required parameters,4205")]
        public short IsWithdrawalAllow { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required parameters,4206")]
        public short IsTransactionWallet { get; set; }

        public short Status { get; set; }

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14167"), DataType(DataType.Currency)]
        [Required(ErrorMessage = "1,Please Enter Required parameters,14168")]
        public decimal Rate { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required parameters,14169")]
        public long CurrencyTypeId { get; set; }
    }
}
