using System;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.Enums;

namespace Worldex.Core.ViewModels.Fiat_Bank_Integration
{
    public class AddBankDetailReq
    {        
        public Guid? BankID { get; set; }

        [Required(ErrorMessage = "1,Please Provide Bank Name,17249")]
        [StringLength(100, ErrorMessage = "1,Maximum Length For Bank Name Is 100 Characters,17250")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "1,Please Provide Bank Code,17251")]
        [StringLength(50, ErrorMessage = "1,Maximum Length For Bank Code Is 50 Characters,17252")]
        public string BankCode { get; set; }

        [Required(ErrorMessage = "1,Please Provide Bank Account Number,17253")]
        [StringLength(50, ErrorMessage = "1,Maximum Length For Bank Acc. No. Is 50 Characters,17254")]
        public string BankAccountNumber { get; set; }

        [Required(ErrorMessage = "1,Please Provide Account Holder Name,17255")]
        [StringLength(100, ErrorMessage = "1,Maximum Length For Account Holder Name Is 100 Characters,17256")]
        public string BankAcountHolderName { get; set; }

        [Required(ErrorMessage = "1,Please Provide Currency Code,17257")]
        [StringLength(5, ErrorMessage = "1,Invalid Currency Code,17258")]
        public string CurrencyCode { get; set; }

        [Required(ErrorMessage = "1,Please Provide Country Code,17259")]
        [StringLength(5, ErrorMessage = "1,Invalid Contry Code,17260")]
        public string CountryCode { get; set; }
    }
}
