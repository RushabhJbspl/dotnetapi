using Worldex.Core.ApiModels;

namespace Worldex.Core.ViewModels
{
    public class TradePairByNameResponse : BizResponseClass
    {
        public GetTradePairByName response { get; set; }
    }
    public class GetTradePairByName : TradePairRespose
    {
        public long BaseCurrencyId { get; set; }
        public string BaseCurrencyName { get; set; }
        public string BaseAbbrevation { get; set; }
    }
}
