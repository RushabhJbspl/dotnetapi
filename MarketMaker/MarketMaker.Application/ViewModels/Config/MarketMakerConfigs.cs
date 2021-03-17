using System;
using System.Collections.Generic;
using System.Text;

namespace MarketMaker.Application.ViewModels.Config
{
    public class MarketMakerConfigs
    {
        public string clientId { get; set; }
        public string grantType { get; set; }
        public string user { get; set; }
        public string passWord { get; set; }
        public string scope { get; set; }
        public int tickerDataTimeLimit { get; set; }
    }
}
