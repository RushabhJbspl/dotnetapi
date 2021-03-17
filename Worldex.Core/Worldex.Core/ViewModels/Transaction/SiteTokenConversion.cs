using Worldex.Core.ApiModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Transaction
{
    public class SiteTokenConversionRequest
    {
        [Required]
        public long SourceCurrencyID { get; set; }

        [Required]
        public long SiteTokenMasterID { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SourceCurrencyQty { get; set; }

        public short TrnMode { get; set; }

        public short IsMargin { get; set; } = 0;//Rita 16-4-19,   1-for Margin trading -used for convert Qty Base to Second currency
    }

    public class SiteTokenConversionResponse : BizResponseClass
    {
        public SiteTokenConversionInfo response { get; set; }
    }
    public class SiteTokenConversionInfo
    {          
        public Guid TrnID { get; set; }       
    }

    //============================Calculation class request-response
    public class SiteTokenCalculationRequest
    {
        [Required]
        public long SourceCurrencyID { get; set; }

        [Required]
        public long SiteTokenMasterID { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Qty { get; set; }

        public short ISSiteTokenToCurrency { get; set; }

        public short IsMargin { get; set; } = 0;//Rita 16-4-19,   1-for Margin trading -used for convert Qty Base to Second currency
    }

    public class SiteTokenCalculationResponse : BizResponseClass
    {
        public SiteTokenCalculationInfo response { get; set; }
    }
    public class SiteTokenCalculationInfo
    {
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal ResultQty { get; set; }

        public string ResultCurrecy { get; set; }
    }
}  
