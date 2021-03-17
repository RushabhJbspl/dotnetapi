using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Worldex.Core.ApiModels;

namespace Worldex.Core.ViewModels.Wallet
{
    public class BuyTopUpRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14150")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14160"), DataType(DataType.Currency)]
        //[Column(TypeName = "decimal(28, 18)")]
        public decimal FromAmount { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14151")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14161"), DataType(DataType.Currency)]
        //[Column(TypeName = "decimal(28, 18)")]
        public decimal ToAmount { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14152")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14162"), DataType(DataType.Currency)]
        // [Column(TypeName = "decimal(28, 18)")]
        public decimal CoinRate { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14153")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14163"), DataType(DataType.Currency)]
        // [Column(TypeName = "decimal(28, 18)")]
        public decimal FiatConverationRate { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14154")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14164"), DataType(DataType.Currency)]
        //[Column(TypeName = "decimal(28, 18)")]
        public decimal Fee { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14155")]
        public string FromCurrency { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14156")]
        public string ToCurrency { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14157")]
        public string Address { get; set; }
        //[Required]
        public string TransactionHash { get; set; }
        // [Required]
        public string TransactionId { get; set; }
        // [Required]
        public string TransactionCode { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14158")]
        public string Platform { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14159")]
        public string Code { get; set; }
    }

    public class BuyTopUpResponse : BizResponseClass
    {
        public string TrnId { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string UserName { get; set; }
        public string DateOfBirth { get; set; }
        public string NotifyURL { get; set; }
        public string ResponseTag { get; set; }
    }

    public class GetLTP
    {
        public string PairName { get; set; }
        public string RatePairName { get; set; }
        public decimal LTP { get; set; }
        public decimal MinQty { get; set; }
        public decimal MaxQty { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal MinRate { get; set; }
        public decimal Rate { get; set; }
        public short TransactionType { get; set; }
    }
    public class ListGetLTP : BizResponseClass
    {
        public List<GetLTP> Data { get; set; }
    }

    public class NotifyDepositReq
    {
        public string Data { get; set; }
        public bool Status { get; set; }
    }

    public class FiatBuyHistory
    {
        public string TrnId { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal CoinRate { get; set; }
        public decimal FiatConverationRate { get; set; }
        public decimal Fee { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public string Address { get; set; }
        public string TransactionHash { get; set; }
        public string TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public string Platform { get; set; }
        public string Code { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public string Remarks { get; set; }
        public string NotifyUrl { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class ListFiatBuyHistory : BizResponseClass
    {
        public List<FiatBuyHistory> Data { get; set; }
    }
    public class FiatSellHistory
    {
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public string Email { get; set; }
        public string Guid { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal CoinRate { get; set; }
        public decimal FiatConverationRate { get; set; }
        public long UserId { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public string Address { get; set; }
        public string TransactionHash { get; set; }
        public string NotifyUrl { get; set; }
        public string TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public string UserGuid { get; set; }
        public string Platform { get; set; }
        public short Type { get; set; }
        public long FromBankId { get; set; }
        public long ToBankId { get; set; }
        public string Code { get; set; }
        public string Remarks { get; set; }
        public string BankName { get; set; }
        public string CurrencyName { get; set; }
        public string BankId { get; set; }
        public string CurrencyId { get; set; }
        public string user_bank_name { get; set; }
        public string user_bank_account_number { get; set; }
        public string user_bank_acount_holder_name { get; set; }
        public string user_currency_code { get; set; }
        public string payus_transaction_id { get; set; }
        public decimal payus_amount_usd { get; set; }
        public decimal payus_amount_crypto { get; set; }
        public decimal payus_mining_fees { get; set; }
        public decimal payus_total_payable_amount { get; set; }
        public decimal payus_fees { get; set; }
        public decimal payus_total_fees { get; set; }
        public decimal payus_usd_rate { get; set; }
        public DateTime payus_expire_datetime { get; set; }
        public string payus_payment_tracking { get; set; }
    }
    public class ListFiatSellHistory : BizResponseClass
    {
        public List<FiatSellHistory> Data { get; set; }
    }

    public class ListFiatCurrencyInfo : BizResponseClass
    {
        public List<GetFiatCurrencyInfo> Data { get; set; }
    }
    public class GetFiatCurrencyInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }
        public string CurrencyCode { get; set; }
        public decimal USDRate { get; set; }
        public decimal BuyFee { get; set; }
        public decimal SellFee { get; set; }
        public short BuyFeeType { get; set; }//1 -fixed 2 -percentage
        public short SellFeeType { get; set; }
    }

    public class GetFiatTradeInfo
    {
        //public decimal BuyFee { get; set; }
        //public decimal SellFee { get; set; }
        public string TermsAndCondition { get; set; }
        public short IsBuyEnable { get; set; }//1 -enable 0 -disable
        public short IsSellEnable { get; set; }//1 -enable 0 -disable
        //public short BuyFeeType { get; set; }//1-fixed 2 Percetage
        //public short SellFeeType { get; set; }//1-fixed 2 Percetage
        public string SellCallBackURL { get; set; }
        public string WithdrawURL { get; set; }
        public string Platform { get; set; }
        //public decimal FiatCurrencyRate { get; set; }
        //public string FiatCurrencyName { get; set; }
        //public decimal MinLimit { get; set; }
        //public decimal MaxLimit { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ListGetFiatTradeInfo : BizResponseClass
    {
        public GetFiatTradeInfo Data { get; set; }
    }

    //public class BuyCallBackUpdateReq
    //{
    //    public string coin_name { get; set; }
    //    public decimal coin_amount { get; set; }
    //    public string currency_code { get; set; }
    //    public decimal total { get; set; }
    //    public string email { get; set; }
    //    public string phone { get; set; }
    //    public string user_id { get; set; }
    //    public string platform { get; set; }
    //    public string transaction_id { get; set; }
    //    public string coin_address { get; set; }
    //    public string code { get; set; }
    //    public string bank { get; set; }
    //    public DateTime created_at { get; set; }
    //    public string status { get; set; }
    //    public string currency { get; set; }
    //}

    public class Bank
    {
        public string _id { get; set; }
        public string bank_name { get; set; }
    }

    public class Currency
    {
        public string _id { get; set; }
        public string currency { get; set; }
    }

    public class BuyCallBackUpdateReq
    {
        public string _id { get; set; }
        public string coin_name { get; set; }
        public decimal coin_amount { get; set; }
        public string currency_code { get; set; }
        public decimal total { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string user_id { get; set; }
        public string platform { get; set; }
        public string transaction_id { get; set; }
        public string coin_address { get; set; }
        public string code { get; set; }
        public Bank bank { get; set; }
        public DateTime created_at { get; set; }
        public string status { get; set; }
        public Currency currency { get; set; }
        public string notes { get; set; }
    }


    public class SellCallBackUpdateReq
    {
        public string _id { get; set; }
        public string payus_destination_tag { get; set; }
        public string coin_name { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal coin_amount { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal total { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string user_id { get; set; }
        public string platform { get; set; }
        public string user_bank_name { get; set; }
        public string user_bank_account_number { get; set; }
        public string user_bank_acount_holder_name { get; set; }
        public string user_currency_code { get; set; }
        public string notify_url { get; set; }
        public string transaction_id { get; set; }
        public string status { get; set; }
        public string payus_transaction_id { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_amount_usd { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_amount_crypto { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_mining_fees { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_total_payable_amount { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_fees { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_total_fees { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_usd_rate { get; set; }
        public DateTime payus_expire_datetime { get; set; }
        public string payus_payment_tracking { get; set; }
        public string coin_address { get; set; }
        public string notes { get; set; }
    }




    public class InputBuyCallBackUpdateReq
    {
        public string data { get; set; }
    }

    public class RootObjectBuyRequest
    {
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal total { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal coin_amount { get; set; }
        public string user_id { get; set; }
        public string platform { get; set; }
        public string coin_name { get; set; }
        public string coin_address { get; set; }
        public string transaction_hash { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string currency_code { get; set; }
        public string notify_url { get; set; }
        public string return_url { get; set; }
        public string transaction_id { get; set; }
    }

    public class SellRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14150")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14160"), DataType(DataType.Currency)]
        public decimal FromAmount { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14151")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14161"), DataType(DataType.Currency)]
        public decimal ToAmount { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14155")]
        public string FromCurrency { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14156")]
        public string ToCurrency { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14185")]
        public string BankId { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14152")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14162"), DataType(DataType.Currency)]
        public decimal CoinRate { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14153")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14163"), DataType(DataType.Currency)]
        public decimal FiatConverationRate { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14154")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14164"), DataType(DataType.Currency)]
        public decimal Fee { get; set; }
    }

    public class InternalSellTopUpRequest
    {
        public decimal total { get; set; }
        public decimal coin_amount { get; set; }
        public long user_id { get; set; }
        public string platform { get; set; }
        public string coin_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string user_bank_name { get; set; }
        public string user_bank_account_number { get; set; }
        public string user_bank_acount_holder_name { get; set; }
        public string user_currency_code { get; set; }
        public string notify_url { get; set; }
        public string transaction_id { get; set; }
        public string return_url { get; set; }
    }

    public class InternalSellTopUpReq
    {
        public string order { get; set; }
    }

    public class InternalSellTopUpRes
    {
        public bool status { get; set; }
        public string data { get; set; }
    }

    public class RootObjectInternalSellTopUpRes
    {
        public string _id { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal total { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal coin_amount { get; set; }
        public string user_id { get; set; }
        public string platform { get; set; }
        public string coin_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string user_bank_name { get; set; }
        public string user_bank_account_number { get; set; }
        public string user_bank_acount_holder_name { get; set; }
        public string user_currency_code { get; set; }
        public string notify_url { get; set; }
        public string transaction_id { get; set; }
        public string return_url { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string status { get; set; }
        public bool edited { get; set; }
        public bool deposit { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_amount_usd { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_amount_crypto { get; set; }
        public string payus_destination_tag { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_mining_fees { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_total_payable_amount { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_fees { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_total_fees { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        public decimal payus_usd_rate { get; set; }
        public DateTime payus_expire_datetime { get; set; }
        public string payus_payment_tracking { get; set; }
        public string coin_address { get; set; }
        public int __v { get; set; }
    }

    public class SellResponse : BizResponseClass
    {
        public string Address { get; set; }
        public string TrnId { get; set; }
        public string TransactionId { get; set; }
    }
    public class SellResponseV2 : BizResponseClass
    {
        public string TrnId { get; set; }
    }

    public class FiatSellConfirmReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14212")]
        public string TrnId { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,14213")]
        public short TransactionBit { get; set; }//1-succss,2-fial or cancel
    }

    public class RemarksClassObj
    {
        public string status { get; set; }
        public string message { get; set; }
        public int code { get; set; }
        public List<object> data { get; set; }
    }
}
