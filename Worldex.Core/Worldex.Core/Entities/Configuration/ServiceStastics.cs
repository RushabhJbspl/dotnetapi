using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Configuration
{
    public class ServiceStastics : BizBase
    {
        public long ServiceId { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MarketCap { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal VolGlobal { get; set; }
        public long MaxSupply { get; set; }
        public long CirculatingSupply { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal IssuePrice { get; set; }
        public DateTime IssueDate { get; set; }
    }
    public class ServiceStasticsMargin : BizBase
    {
        public long ServiceId { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MarketCap { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal VolGlobal { get; set; }
        public long MaxSupply { get; set; }
        public long CirculatingSupply { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal IssuePrice { get; set; }
        public DateTime IssueDate { get; set; }
    }

    //Darshan Dholakiya added this entity for the arbitrage service config. related changes:10-06-2019
    public class ServiceStasticsArbitrage : BizBase
    {
        public long ServiceId { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MarketCap { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal VolGlobal { get; set; }
        public long MaxSupply { get; set; }
        public long CirculatingSupply { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal IssuePrice { get; set; }
        public DateTime IssueDate { get; set; }
    }
}
