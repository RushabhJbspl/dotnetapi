using Worldex.Core.ApiModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Worldex.Core.ViewModels
{
    public class TradePairAssetResponce : BizResponseClass
    {
        public List<BasePairResponse> response { get; set; } 
    }
    public class BasePairResponse
    {
        public long BaseCurrencyId { get; set; }
        public string BaseCurrencyName { get; set; }
        public string Abbrevation { get; set; }
        public List<TradePairRespose> PairList { get; set; }
    }
    public class TradePairRespose
    {
        public long PairId { get; set; }
        [JsonProperty(PropertyName = "PairName")]
        public string Pairname { get; set; }
        [JsonProperty(PropertyName = "CurrentRate")]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Currentrate { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Volume { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal SellFees { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal BuyFees { get; set; }
        public string ChildCurrency { get; set; }
        public string Abbrevation { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChangePer { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal High24Hr { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Low24Hr { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal HighWeek { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LowWeek { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal High52Week { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Low52Week { get; set; }
        public short UpDownBit { get; set; }
        public int QtyLength { get; set; } // khushali 17-07-2019 for precision validation currency wise 
        public int PriceLength { get; set; } // khushali 17-07-2019 for precision validation currency wise 
        public int AmtLength { get; set; } // khushali 17-07-2019 for precision validation currency wise        
        public decimal PairPercentage { get; set; } //Rushabh 06-01-2020 Added As Per Front Requirement.
    }

    public class TradePairTableResponse : TradePairRespose
    {        
        public long BaseId { get; set; }
        public string BaseCode { get; set; }
        public string BaseName { get; set; }
        public short Priority { get; set; }//rita 01-5-19 for manage list         
    }
}
