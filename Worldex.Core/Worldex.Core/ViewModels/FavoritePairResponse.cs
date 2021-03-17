using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels
{
    public class FavoritePairResponse : BizResponseClass
    {
        public List<FavouritePairInfo> response { get; set; }
    }
    public class FavouritePairInfo : TradePairRespose
    {
        public string BaseCurrency { get; set; }
        public string BaseAbbrevation { get; set; }
    }
}
