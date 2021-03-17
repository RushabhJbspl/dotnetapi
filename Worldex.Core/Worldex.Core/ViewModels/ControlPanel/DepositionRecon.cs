using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.ControlPanel
{
    public class DepositionHistoryRes : BizResponseClass
    {
        public List<Dipositions> Deposit { get; set; }
        public long TotalCount { get; set; }
        public int PageNo { get; set; }
        public long PageSize { get; set; }
    }

    public class Dipositions
    {
        public long TrnNo { get; set; }
        public string TrnId { get; set; }
        public string CoinName { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public long Confirmations { get; set; }
        public string ConfirmedTime { get; set; }
        public string FromAddress { get; set; }
        public long ServiceProviderId { get; set; }
        public string ProviderName { get; set; }
        public short IsInternalTrn { get; set; }
        public string Email { get; set; }
        public string ExplorerLink { get; set; }
        public short Status { get; set; }
        public string StatusStr { get; set; }
        public string StatusMsg { get; set; }
    }
    public class ActionTypes
    {
        public short ActionStage { get; set; }
        public string ActionMessage { get; set; }
    }
    public class ListTradingChargeTypeRes : BizResponseClass
    {
        public List<TradingChargeTypeRes> Data { get; set; }        
    }
    public class ListMarketCurrencyRes : BizResponseClass
    {
        public List<MarketCurrencyRes> Data { get; set; }
    }
    public class MarketCurrencyRes
    {
        public long Id { get; set; }
        public string MarketCurrency { get; set; }
        public short Status { get; set; }
    }

    public class TradingChargeTypeRes
    {
        public long Id { get; set; }
        public short Type { get; set; }
        public string TypeName { get; set; }
        public short IsChargeFreeMarketEnabled { get; set; }//1 enable,0-disable
        public short IsCommonCurrencyDeductEnable { get; set; }// enable,0-disable
        public decimal DiscountPercent { get; set; }
        public string DeductCurrency { get; set; }
        public short IsDeductChargeMarketCurrency { get; set; }
        public short Status { get; set; }
    }

    public class InsertTradingChargeTypeReq
    {
        [Required(ErrorMessage = "1,Please enter required parameter,17002")]
        public EnTradingChargeType Type { get; set; }
        [Required(ErrorMessage = "1,Please enter required parameter,17003")]
        public string TypeName { get; set; }
        [Required(ErrorMessage = "1,Please enter required parameter,4914")]
        public short Status { get; set; }
        public short IsChargeFreeMarketEnabled { get; set; }//1 enable,0-disable
        public short IsCommonCurrencyDeductEnable { get; set; }// enable,0-disable
        public decimal DiscountPercent { get; set; }
        public string DeductCurrency { get; set; }
        public short IsDeductChargeMarketCurrency { get; set; }
    }
    public class UpdateTradingChargeTypeReq
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "1,Please enter required parameter,17003")]
        public string TypeName { get; set; }
        [Required(ErrorMessage = "1,Please enter required parameter,4914")]
        public short Status { get; set; }
        public short IsChargeFreeMarketEnabled { get; set; }//1 enable,0-disable
        public short IsCommonCurrencyDeductEnable { get; set; }// enable,0-disable
        public decimal DiscountPercent { get; set; }
        public string DeductCurrency { get; set; }
        public short IsDeductChargeMarketCurrency { get; set; }
    }

    public class DepositionReconReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,17008")]
        public long[] TrnNo { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17009")]
        public int ActionType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17010")]
        public string ActionRemarks { get; set; }
    }

    public class DepositionReconRes
    {
        public long TrnNo { get; set; }
        public enResponseCode ReturnCode { get; set; }
        public string ReturnMsg { get; set; }
        public enErrorCode ErrorCode { get; set; }
    }

    public class ListDepositionReconRes : BizResponseClass
    {
        public List<DepositionReconRes> ResponseList { get; set; }
    }

}
