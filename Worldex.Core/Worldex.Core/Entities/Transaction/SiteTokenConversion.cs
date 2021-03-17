using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Transaction
{
    public class SiteTokenConversion : BizBase
    {  
        public Guid GUID { get; set; }

        public long UserID { get; set; }

        public long SourceCurrencyID { get; set; }

        public string SourceCurrency { get; set; }//Currency Code

        public long TargerCurrencyID { get; set; }

        public string TargerCurrency { get; set; }//Currency Code

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SourceCurrencyQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TargerCurrencyQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SourceToBasePrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SourceToBaseQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TokenPrice { get; set; }

        public long SiteTokenMasterID { get; set; }

        public string TimeStamp { get; set; }

        public string StatusMsg { get; set; }
    }

    public class SiteTokenConversionMargin : BizBase
    {
        public Guid GUID { get; set; }

        public long UserID { get; set; }

        public long SourceCurrencyID { get; set; }

        public string SourceCurrency { get; set; }//Currency Code

        public long TargerCurrencyID { get; set; }

        public string TargerCurrency { get; set; }//Currency Code

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SourceCurrencyQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TargerCurrencyQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SourceToBasePrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SourceToBaseQty { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TokenPrice { get; set; }

        public long SiteTokenMasterID { get; set; }

        public string TimeStamp { get; set; }

        public string StatusMsg { get; set; }
    }
}
