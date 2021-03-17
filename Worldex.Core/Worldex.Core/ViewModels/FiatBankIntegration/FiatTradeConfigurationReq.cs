using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.ApiModels;
using Worldex.Core.Enums;

namespace Worldex.Core.ViewModels.FiatBankIntegration
{
    public class FiatTradeConfigurationReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,17268")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Range,17279"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BuyFee { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17269")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Range,17280"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SellFee { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17270")]
        public string TermsAndCondition { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17271")]
        public short IsBuyEnable { get; set; }//1 -enable 0 -disable

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17272")]
        public short IsSellEnable { get; set; }//1 -enable 0 -disable

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17273")]
        public short BuyFeeType { get; set; }//1-fixed 2 Percetage

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17274")]
        public short SellFeeType { get; set; }//1-fixed 2 Percetage

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17299")]
        [StringLength(250, ErrorMessage = "1,Invalid Length,17275")]
        public string BuyNotifyURL { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17300")]
        [StringLength(250, ErrorMessage = "1,Invalid Length,17276")]
        public string SellNotifyURL { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17301")]
        [StringLength(250, ErrorMessage = "1,Invalid Length,17277")]
        public string CallBackURL { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17302")]
        [StringLength(50, ErrorMessage = "1,Invalid Length,17278")]
        public string EncryptionKey { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17303")]
        [StringLength(250, ErrorMessage = "1,Invalid Length,17282")]
        public string SellCallBackURL { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17304")]
        [StringLength(250, ErrorMessage = "1,Invalid Length,17283")]
        public string WithdrawURL { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17305")]
        [StringLength(250, ErrorMessage = "1,Invalid Length,17284")]
        public string Platform { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17285")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Range,17286"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal FiatCurrencyRate { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17287")]
        [StringLength(50, ErrorMessage = "1,Invalid Length,17288")]
        public string FiatCurrencyName { get; set; }

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Range,17289"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinLimit { get; set; }

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Range,17290"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaxLimit { get; set; }

        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Invalid Parameter,17281")]
        public ServiceStatus? Status { get; set; }
    }

    public class FiatTradeConfigurationRes
    {
        public decimal BuyFee { get; set; }
        public decimal SellFee { get; set; }
        public string TermsAndCondition { get; set; }
        public short IsBuyEnable { get; set; }//1 -enable 0 -disable        
        public short IsSellEnable { get; set; }//1 -enable 0 -disable        
        public short BuyFeeType { get; set; }//1-fixed 2 Percetage  
        public string StrBuyFeeType { get; set; }
        public short SellFeeType { get; set; }//1-fixed 2 Percetage  
        public string StrSellFeeType { get; set; }
        public string BuyNotifyURL { get; set; }
        public string SellNotifyURL { get; set; }
        public string CallBackURL { get; set; }
        public string EncryptionKey { get; set; }
        public string SellCallBackURL { get; set; }
        public string WithdrawURL { get; set; }
        public string Platform { get; set; }
        public decimal FiatCurrencyRate { get; set; }
        public string FiatCurrencyName { get; set; }
        public decimal MinLimit { get; set; }
        public decimal MaxLimit { get; set; }
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }

    public class ListFiatTradeConfigurationRes : BizResponseClass
    {
        public List<FiatTradeConfigurationRes> Data { get; set; }
    }

    public class ListFiatCoinConfigurationReq
    {
        public List<FiatCoinConfigurationReq> Request { get; set; }
    }

    public class FiatCoinConfigurationReq
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14194")]
        public short TransactionType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14170")]
        public long FromCurrencyId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14171")]
        public long ToCurrencyId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14172")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14173"), DataType(DataType.Currency)]
        public decimal MinQty { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14174")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14175"), DataType(DataType.Currency)]
        public decimal MaxQty { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14176")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14177"), DataType(DataType.Currency)]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14178")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14179"), DataType(DataType.Currency)]
        public decimal MaxAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14180")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14181"), DataType(DataType.Currency)]
        public decimal MinRate { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14182")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14183"), DataType(DataType.Currency)]
        public decimal Rate { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14184")]
        public short Status { get; set; }
    }

    public class FiatCoinConfigurationRes
    {
        public long Id { get; set; }

        public long FromCurrencyId { get; set; }

        public string FromCurrencyName { get; set; }

        public long ToCurrencyId { get; set; }

        public string ToCurrencyName { get; set; }

        public decimal MinQty { get; set; }

        public decimal MaxQty { get; set; }

        public decimal MinAmount { get; set; }

        public decimal MaxAmount { get; set; }

        public decimal Rate { get; set; }

        public decimal MinRate { get; set; }

        public short Status { get; set; }

        public short TransactionType { get; set; }
    }

    public class ListFiatCoinConfigurationRes : BizResponseClass
    {
        public List<FiatCoinConfigurationRes> Data { get; set; }
    }

    public class UpdateTransactionHashViewModel
    {
        public string coin_address { get; set; }
        public string transaction_hash { get; set; }
    }
    public class TransactionHasAPIResponse
    {
        public string status { get; set; }
        public string data { get; set; }
    }
    public class FiatCurrencyConfigurationReq
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14201")]
        public short Status { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14202")]
        public string Name { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14203")]
        public string CurrencyCode { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14204")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14205"), DataType(DataType.Currency)]
        public decimal USDRate { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14206")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Range,14207"), DataType(DataType.Currency)]
        public decimal BuyFee { get; set; }


        [Required(ErrorMessage = "1,Please Enter Required Parameter,14208")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Invalid Range,14209"), DataType(DataType.Currency)]
        public decimal SellFee { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14210")]
        public short BuyFeeType { get; set; }//1 -fixed 2 -percentage

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14211")]
        public short SellFeeType { get; set; }


    }
    public class FiatSellWithdrawTraxn
    {
        public long Memberid { get; set; }
        public long Trnno { get; set; }
        public Guid RefNo { get; set; }
        public string WithdrawRefNo { get; set; }

    }

    public class LPTPairFiat
    {
        public string PairName { get; set; }
    }
}
