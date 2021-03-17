using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.IEO
{
    public class IEOCurrencyPairMapping : BizBase
    {
        [Required]
        public string Guid { get; set; }
        [Required]
        public long IEOWalletTypeId { get; set; }//fk of wallettypemasters
        [Required]
        public long PaidWalletTypeId { get; set; }//fk	wallettypemasters
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal PurchaseRate { get; set; }
        [Required]
        public short ConvertCurrencyType { get; set; }//1-purchaserate,2-pair,3-usd
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal InstantPercentage { get; set; }
        [Required]
        public long RoundId { get; set; }
    }
}
