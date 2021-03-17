using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.LiquidityProvider
{

    public class CancelOrderResult
    {
        public string uuid { get; set; }
    }

    public class BittrexCancelOrderResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public CancelOrderResult result { get; set; }
    }

    public class BittrexStatusCheckResult
    {
        public string Uuid { get; set; }
        public string OrderUuid { get; set; }
        public string Exchange { get; set; }
        public string OrderType { get; set; }
        public int Quantity { get; set; }
        public int QuantityRemaining { get; set; }
        public double Limit { get; set; }
        public int CommissionPaid { get; set; }
        public int Price { get; set; }
        public object PricePerUnit { get; set; }
        public DateTime Opened { get; set; }
        public object Closed { get; set; }
        public bool CancelInitiated { get; set; }
        public bool ImmediateOrCancel { get; set; }
        public bool IsConditional { get; set; }
    }

    public class BittrexStatusCheckResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<BittrexStatusCheckResult> result { get; set; }
    }
}
