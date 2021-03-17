using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.IEO
{
    public class IEOSlabMaster : BizBase
    {
        [Required]
        public string Guid { get; set; }
        [Required]
        public long RoundId { get; set; }
        [Required]
        public long Priority { get; set; }//1-Instant	,2-Percentage
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Value { get; set; }
        [Required]
        public long Duration { get; set; }//days
        [Required]
        public short DurationType { get; set; }//1-Instant,2-Days
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Bonus { get; set; }
    }
}
