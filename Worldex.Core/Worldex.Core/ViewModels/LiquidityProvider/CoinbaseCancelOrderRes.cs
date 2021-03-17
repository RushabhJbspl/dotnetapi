using Worldex.Core.Enums;
using CoinbasePro.Services.Orders.Models.Responses;

namespace Worldex.Core.ViewModels.LiquidityProvider
{
    public class CoinbaseCancelOrderRes
    {
        public string ErrorMsg { get; set; }
        public enErrorCode ErrorCode { get; set; }
        public CancelOrderResponse Result { get; set; }
    }
}
