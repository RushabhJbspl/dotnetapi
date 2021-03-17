using System.ComponentModel.DataAnnotations;
using Worldex.Core.Enums;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.FiatBankIntegration
{
    public class UserBankRequest : BizBase
    {
        public string GUID { get; set; }

        public short RequestType { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string BankName { get; set; }

        [Required]
        [StringLength(50)]
        public string BankCode { get; set; }

        [Required]
        [StringLength(50)]
        public string BankAccountNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string BankAcountHolderName { get; set; }

        [Required]
        [StringLength(5)]
        public string CurrencyCode { get; set; }

        [Required]
        [StringLength(5)]
        public string CountryCode { get; set; }

        public string Remarks { get; set; }
    }
}
