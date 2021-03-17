using Worldex.Core.ApiModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Configuration
{
    public class GetAllTradeRouteConfiguration : BizResponseClass
    {
        public List<GetTradeRouteConfigurationData> Response { get; set; }
    }

    public class GetTradeRouteConfiguration : BizResponseClass
    {
        public GetTradeRouteConfigurationData Response { get; set; }
    }

    public class GetTradeRouteConfigurationData
    {
        public long Id { get; set; }
        public string TradeRouteName { get; set; }
        public long PairId { get; set; }
        public string PairName { get; set; }
        public long OrderType { get; set; }
        public string OrderTypeText { get; set; }
        public int TrnType { get; set; }
        public string TrnTypeText { get; set; }
        public short Status { get; set; }
        public string StatusText { get; set; }
        public long RouteUrlId { get; set; }
        public string RouteUrl { get; set; }
        public string AssetName { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal ConvertAmount { get; set; }
        public int ConfirmationCount { get; set; }
        public short Priority { get; set; }
    }
}
