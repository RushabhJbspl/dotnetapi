using Worldex.Core.ApiModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Configuration
{
    public class LimitViewModel
    {
        public long Id { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmt { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaxAmt { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmtDaily { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaxAmtDaily { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmtWeekly { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaxAmtWeekly { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmtMonthly { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public long MaxAmtMonthly { get; set; }

        public long MinRange { get; set; }
        public long Maxrange { get; set; }
        public long MinRangeDaily { get; set; }
        public long MaxRangeDaily { get; set; }
        public long MinRangeWeekly { get; set; }
        public long MaxRangeWeekly { get; set; }
        public long MinRangeMonthly { get; set; }
        public long MaxRangeMonthly { get; set; }
        public short Status { get; set; }
    }

    public class LimitRequest : LimitViewModel
    {
        
    }
    public class LimitResponseAllData : BizResponseClass
    {
        public List<LimitViewModel> Response { get; set; }
    }
    public class LimitResponse : BizResponseClass
    {
        public LimitViewModel Response { get; set; }
    }
}
