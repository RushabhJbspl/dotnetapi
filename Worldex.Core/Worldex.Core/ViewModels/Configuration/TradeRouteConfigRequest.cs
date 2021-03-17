using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Configuration
{
    public class TradeRouteConfigRequest
    {
        public long Id { get; set; }
        public long PairId { get; set; }
        public long OrderType { get; set; }
        public long TrnType { get; set; }
        public int Status { get; set; }
        public long RouteUrlId { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4664")]
        public string AssetName { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal ConvertAmount { get; set; }
        public int ConfirmationCount { get; set; }
    }

    public class TradeRoutePriorityUpdateRequest
    {
        public List<TradeRoutePriorityData> TradeRoute { get; set; }
    }

    public class TradeRoutePriorityData
    {
        public long Id { get; set; }
        public short Priority { get; set; }
    }
}
