using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Transaction
{
    public class ProfitIndicatorResponse : BizResponseClass
    {
        public ProfitIndicatorInfo response { get; set; }
    }
    public class ProfitIndicatorInfo
    {
        public List<LeftProvider> BUY { get; set; }
        public List<LeftProvider> SELL { get; set; }
    }
    public class CommonProData
    {
        public long RouteID { get; set; }
        public string RouteName { get; set; }
        public long SerProID { get; set; }
        public string ProviderName { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LTP { get; set; }
    }
    public class LeftProvider : CommonProData
    {
        public List<Provider> Providers { get; set; }
    }
    public class Provider : CommonProData
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal GrossProfit { get; set; }//Rita 16-7-19 gives percentage value
        public decimal NetProfit { get; set; }
        //public decimal Profit { get; set; }
    }


    //====================================
    public class ExchangeProviderListArbitrage
    {
        //C.Volume,C.ChangePer,C.UpDownBit
        public long LPType { get; set; }
        public long RouteID { get; set; }
        //public long ordertype { get; set; }
        public string RouteName { get; set; }
        public long ProviderID { get; set; }
        public string ProviderName { get; set; }
        public long SerProDetailID { get; set; }
        public long  TrnType { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LTP { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Volume { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChangePer { get; set; }
        public short UpDownBit { get; set; }

    }

    public class LocalPairStatisticsQryRes
    {
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Volume { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChangePer { get; set; }
        public short UpDownBit { get; set; }
    }

    public class ExchangeProviderListResponse : BizResponseClass
    {
        public List<ExchangeProviderListArbitrage> response { get; set; }
    }
    //public class ExchangeProviderListInfo
    //{
    //}
    //====================================

    //Rita 12-6-19 for smart Arbitrage
    public class ExchangeListSmartArbitrageResponse : BizResponseClass
    {
        public List<ExchangeListSmartArbitrage> response { get; set; }
    }
    public class ExchangeListSmartArbitrage
    {
        public string Pair { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        //public decimal ProfitPer { get; set; }
        public decimal GrossProfitPer { get; set; }//Rita 18-7-19 Differe TotalPer and Net Per
        public decimal NetProfitPer { get; set; }
        //public decimal ProfitValue { get; set; } //Rita 16-7-19 gives percentage value
        public decimal GrossProfitValue { get; set; }//Rita 18-7-19 Differe TotalValue and NetValue
        public decimal NetProfitValue { get; set; }
        public Providers ProviderBuy { get; set; }
        public Providers ProviderSELL { get; set; }
    }
    public class Providers
    {
        public long RouteID { get; set; }
        public string RouteName { get; set; }
        public long SerProID { get; set; }
        public string ProviderName { get; set; }
        public long LPType { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LTP { get; set; }
    }

    //Rita 13-6-19 smart arbitrage 
    public class SmartArbitrageHistoryRequest
    {
        public string Pair { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        //public int Page { get; set; }
        public short IsMargin { get; set; } = 0;
    }
    public class SmartArbitrageHistoryResponse : BizResponseClass
    {
        public List<SmartArbitrageHistoryInfo> response { get; set; }
    }
    public class SmartArbitrageHistoryInfo
    {
        public Guid GUID { get; set; }
        public string PairName { get; set; }
        public DateTime TrnDate { get; set; }        
        public string Market { get; set; }        
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Profit { get; set; }
        public string ProfitCurrency { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal FundUsed { get; set; }
        public string FundUsedCurrency { get; set; }


    }
}
