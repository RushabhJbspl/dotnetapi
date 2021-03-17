using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class GetBuySellBookResponse : BizResponseClass
    {
      public List<GetBuySellBook> response { get; set; }
    }
    public class GetBuySellBook
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public Decimal Amount { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public Decimal Price { get; set; }
        public Guid OrderId { get; set; }
        public int RecordCount { get; set; }
        public short IsStopLimit { get; set; } //Rita 16-1-19 added fro front side separate array of Stop&Limit
    }
    public class StopLimitBuySellBook
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public Decimal Amount { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public Decimal Price { get; set; }
        public Guid OrderId { get; set; }
        public int RecordCount { get; set; }
        public short IsAdd { get; set; }
    }

    public class BulkBuySellBook //SignalR
    {
        public List<GetBuySellBook> OrderBook { get; set; }
        public int LP { get; set; }
    }

    public class UpbitGetBuySellBook
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public Decimal Amount { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public Decimal Price { get; set; }
    }
}