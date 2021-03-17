using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class MarketCapResponse : BizResponseClass
    {
        public MarketCapData response { get; set; }
    }
    public class MarketCapData
    {
        [Range(0, 9999999999.999999999999999999)]
        public Decimal Volume24 { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public Decimal Low24 { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public Decimal High24 { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public Decimal Change24 { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public Decimal LastPrice { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public Decimal ChangePer { get; set; }
    }
    
}
