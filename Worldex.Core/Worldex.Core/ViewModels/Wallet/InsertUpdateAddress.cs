using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Wallet
{
    class InsertUpdateAddress
    {
    }

    public class InsertUpdateAddressReq
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17082")]
        [StringLength(70, ErrorMessage = "1,Please Enter a User Name,17083")]
        public string Address { get; set; }

        [Required(ErrorMessage = "1Please Enter Required Parameter,17084")]
        public byte IsDefaultAddress { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17085")]
        public long ServiceProviderID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17086")]
        public long WalletTypeId { get; set; }
    }

    public class InsertUpdateAddressRes : BizResponseClass
    {

    }

    public class ListArbitrageAddressRes : BizResponseClass
    {
        public List<ArbitrageAddressRes> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class ArbitrageAddressRes
    {
        public long Id { get; set; }
        public long WalletTypeId { get; set; }
        public string WalletTypeName { get; set; }
        public long ServiceProviderId { get; set; }
        public string ServiceProviderName { get; set; }
        public string Address { get; set; }
        public byte IsDefaultAddress { get; set; }
    }

    public class ArbitrageWithdrawReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,17087")]
        public long FromServiceProviderId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17088")]
        public long ToServiceProviderId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17089")]
        public string Address { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17090")]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17091")]
        public string SMSCode { get; set; }
    }

    public class ArbitrageWithdrawRes : BizResponseClass
    {
        public string TransactionId { get; set; }
    }

    public class ArbitrageTopUpHistory
    {
        public string TrnNo { get; set; }
        public long FromServiceProviderId { get; set; }
        public long ToServiceProviderId { get; set; }
        public string FromServiceProviderName { get; set; }
        public string ToServiceProviderName { get; set; }
        public string CoinName { get; set; }
        public string TrnId { get; set; }
        public DateTime TrnDate { get; set; }
        public short Status { get; set; }
        public string StatusStr { get; set; }
        public string Remarks { get; set; }
        public decimal Amount { get; set; }
        public string Address { get; set; }
        public string ExplorerLink { get; set; }
    }

    public class ListArbitrageTopUpHistory : BizResponseClass
    {
        public List<ArbitrageTopUpHistory> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class AnalyticsGraphRes : BizResponseClass
    {
        public string[] DayMonth { get; set; }
        public decimal[] AmountArray { get; set; }
        public decimal[] USDAmountArray { get; set; }
        public decimal Amount { get; set; }
        public decimal USDTotalAmount { get; set; }
        public ChangeClass TotalChange { get; set; }
        public ChangeClass TotalChange24H { get; set; }
        public ChangeClass TotalChange7D { get; set; }
        public ChangeClass TotalChange30D { get; set; }
    }

    public class ChangeClass
    {
        public decimal Percentage { get; set; }
        public decimal Change { get; set; }
        public decimal USDChange { get; set; }
        public short IsProfit { get; set; }
    }

    public class sqlGraphRes
    {
        public string AmountString { get; set; }
        public string USDAmountString { get; set; }
    }

    public class sqlGraphAmountRes
    {
        public decimal StartingBalance { get; set; }
        public decimal EndingBalance { get; set; }
    }

    public class FundTransferRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,17092")]
        public string DebitAccWalletId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17093")]
        public string CreditAccWalletId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17094")]
         [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17095")]
        public string CurrencyName { get; set; }
    }

    public class FundTransferResponse : BizResponseClass
    {

    }

    public class ProviderFundTransferRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,17096")]
        public long ServiceProviderId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17094")]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17095")]
        public string CurrencyName { get; set; }
    }

    public class NewGraphRes
    {
        public decimal EndingBalance { get; set; }
        public decimal USDEndingBalance { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }

    public class ListNewGraphRes
    {
       public List<NewGraphRes> data { get; set; }
    }

    public class ArbitrageWithdrawReconReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,17087")]
        public long ReconID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17090")]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ProviderBalance { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17091")]
        public string SMSCode { get; set; }
    }

    public class ReconRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,17097")]
        public string RequestId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17090")]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17098")]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17099")]
        public short IsAccept { get; set; }

    }
}
