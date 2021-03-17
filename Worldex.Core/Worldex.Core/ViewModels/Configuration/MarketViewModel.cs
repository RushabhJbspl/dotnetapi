using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class MarketViewModel
    {
        public long ID { get; set; }
        [Required]
        public string CurrencyName { get; set; }
        public short isBaseCurrency { get; set; }
        [Required]
        public long ServiceID { get; set; }
        public string Status { get; set; }
        public String CurrencyDesc { get; set; }
    }
    public class MarketInfo
    {
        public long ID { get; set; }
        [Required]
        public long ServiceID { get; set; }
        [Required]
        public short Status { get; set; }
        [Required]
        public string CurrencyName { get; set; }
    }
    public class MarketDataRequest : MarketInfo
    {
        public short IsMargin { get; set; } = 0;//Rita 5-3-19 for margin trading
    }
    public class MarketDataResponse : BizResponseClass
    {
        public MarketInfo Response { get; set; }
    }
    public class MarketResponse : BizResponseClass
    {
        public List<MarketViewModel> Response { get; set; }
    }
}
