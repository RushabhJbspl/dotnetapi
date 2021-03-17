using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.IEO
{
    public class IEOCurrencyMaster : BizBase
    {
        [Required]
        public string Guid { get; set; }
        [Required]
        public string IEOTokenTypeName { get; set; }
        [Required]
        public string CurrencyName { get; set; }
        public string Description { get; set; }
        public short Rounds { get; set; }//max allow round	
        public string IconPath { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Rate { get; set; }
    }
}
