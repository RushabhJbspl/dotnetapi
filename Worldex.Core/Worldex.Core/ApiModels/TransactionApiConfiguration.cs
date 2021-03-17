using Worldex.Core.Enums;
using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ApiModels
{ 
    public class TransactionApiConfigurationRequest
    {
        [Required]
        public string SMSCode { get; set; }
        public enWebAPIRouteType APIType { get; set; }
        public int trnType { get; set; } // ntrivedi  added 03-11-2018
        public decimal amount { get; set; }// ntrivedi  added 03-11-2018
        public int OrderType { get; set; } // Khushali  added 29-12-2018
        public long PairID { get; set; } // Khushali  added 29-12-2018
        public short LPType { get; set; } // Khushali  added 11-06-2018
    }

    public class ArbitrageTransactionApiConfigurationRequest
    {
        [Required]
        public string SMSCode { get; set; }
        public enWebAPIRouteType APIType { get; set; }
        public int trnType { get; set; }   
        public decimal amount { get; set; }
        public int OrderType { get; set; } 
        public long PairID { get; set; }   
    }

    public class TransactionProviderResponseForWithdraw
    {
        public long ServiceID { get; set; }
        public string ServiceName { get; set; }
        public long ServiceProID { get; set; }
        public long SerProDetailID { get; set; }
        public long RouteID { get; set; }
        public long ProductID { get; set; } // ntrivedi added 03-11-2018
        public string RouteName { get; set; }
        public int ServiceType { get; set; }
        public long ThirPartyAPIID { get; set; }
        public long AppTypeID { get; set; } //Rushabh Updated 11-10-2018 oldName=AppType
        public decimal MinimumAmountItem { get; set; } //Rushabh Updated 11-10-2018 old datatype=long
        public decimal MaximumAmountItem { get; set; } //Rushabh Updated 11-10-2018 old datatype=long
        public string APIBalURL { get; set; } // Khushali  added 29-01-2018
        public string APISendURL { get; set; } // Khushali  added 29-01-2018
        public string APIValidateURL { get; set; } // Khushali  added 29-01-2018
        public string ContentType { get; set; } // Khushali  added 29-01-2018
        public string MethodType { get; set; } // Khushali  added 29-01-2018
        public string OpCode { get; set; } // Khushali  added 29-01-2018
        public int ParsingDataID { get; set; } // Khushali  added 29-01-2018
        public string ProviderWalletID { get; set; } // Khushali  added 29-01-2018
        public long ProTypeID { get; set; } // Khushali  added 29-01-2018        
        public string AccNoStartsWith { get; set; } // Rushabh 23-03-2019        
        public string AccNoValidationRegex { get; set; } // Rushabh 23-03-2019        
        public int AccountNoLen { get; set; } // Rushabh 23-03-2019
        public short IsAdminApprovalRequired { get; set; }
        public short IsOnlyIntAmountAllow { get; set; }
    }

    public class TransactionProviderResponse
    {
        public long ServiceID { get; set; }
        public string ServiceName { get; set; }
        public long ServiceProID { get; set; }
        public long SerProDetailID { get; set; }
        public long RouteID { get; set; }
        public long ProductID { get; set; } // ntrivedi added 03-11-2018
        public string RouteName { get; set; }
        public int ServiceType { get; set; }
        public long ThirPartyAPIID { get; set; }      
        public long AppTypeID { get; set; } //Rushabh Updated 11-10-2018 oldName=AppType
        public decimal MinimumAmountItem { get; set; } //Rushabh Updated 11-10-2018 old datatype=long
        public decimal MaximumAmountItem { get; set; } //Rushabh Updated 11-10-2018 old datatype=long
        public string APIBalURL { get; set; } // Khushali  added 29-01-2018
        public string APISendURL { get; set; } // Khushali  added 29-01-2018
        public string APIValidateURL { get; set; } // Khushali  added 29-01-2018
        public string ContentType { get; set; } // Khushali  added 29-01-2018
        public string MethodType { get; set; } // Khushali  added 29-01-2018
        public string OpCode { get; set; } // Khushali  added 29-01-2018
        public int ParsingDataID { get; set; } // Khushali  added 29-01-2018
        public string ProviderWalletID { get; set; } // Khushali  added 29-01-2018
        public long ProTypeID { get; set; } // Khushali  added 29-01-2018
        [NotMapped]
        public string AccNoStartsWith { get; set; } // Rushabh 23-03-2019
        [NotMapped]
        public string AccNoValidationRegex { get; set; } // Rushabh 23-03-2019
        [NotMapped]
        public int AccountNoLen { get; set; } // Rushabh 23-03-2019
        public short IsAdminApprovalRequired { get; set; }
        [NotMapped]
        public string APIKey { get; set; } //khushali 10-07-2019
        [NotMapped]
        public string SecretKey { get; set; } //khushali 10-07-2019
    }

    public class InsertIntoTransactionRequyest
    {
        public long TrnNo { get; set; }
        public string RequestBody { get; set; }
    }

    //khushali 11-07-2019 sperate from all other route response.
    public class TransactionProviderResponseV1
    {
        public long ServiceID { get; set; }
        public string ServiceName { get; set; }
        public long ServiceProID { get; set; }
        public long SerProDetailID { get; set; }
        public long RouteID { get; set; }
        public long ProductID { get; set; } // ntrivedi added 03-11-2018
        public string RouteName { get; set; }
        public int ServiceType { get; set; }
        public long ThirPartyAPIID { get; set; }
        public long AppTypeID { get; set; } //Rushabh Updated 11-10-2018 oldName=AppType
        public decimal MinimumAmountItem { get; set; } //Rushabh Updated 11-10-2018 old datatype=long
        public decimal MaximumAmountItem { get; set; } //Rushabh Updated 11-10-2018 old datatype=long
        public string APIBalURL { get; set; } // Khushali  added 29-01-2018
        public string APISendURL { get; set; } // Khushali  added 29-01-2018
        public string APIValidateURL { get; set; } // Khushali  added 29-01-2018
        public string ContentType { get; set; } // Khushali  added 29-01-2018
        public string MethodType { get; set; } // Khushali  added 29-01-2018
        public string OpCode { get; set; } // Khushali  added 29-01-2018
        public int ParsingDataID { get; set; } // Khushali  added 29-01-2018
        public string ProviderWalletID { get; set; } // Khushali  added 29-01-2018
        public long ProTypeID { get; set; } // Khushali  added 29-01-2018
        public short IsAdminApprovalRequired { get; set; }
        public string APIKey { get; set; } //khushali 10-07-2019
        public string SecretKey { get; set; } //khushali 10-07-2019
    }

    public class TransactionProviderArbitrageResponse
    {
        public long LPType { get; set; }
        public long RouteID { get; set; }
        public long ProTypeID { get; set; }
        public string RouteName { get; set; }
        public long ProviderID { get; set; }
        public string ProviderName { get; set; }
        public long SerProDetailID { get; set; }
        public int TrnType { get; set; }
        //[Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        //public decimal LTP { get; set; }
        public string APIKey { get; set; }
        public string SecretKey { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MinNotional { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxNotional { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinPrice { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaxPrice { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinQty { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaxQty { get; set; }

        public long ThirdPartyAPIID { get; set; }//Rita 23-7-19 taken from cache all data then filter on it
        public long PairId { get; set; }
        public long TrnTypeID { get; set; }
        public string PairName { get; set; }
        public string IsStoplossOrder { get; set; } // khushali 27-07-2019  Stop Price Logic
        public string LPProviderName { get; set; } // rita 12-8-19 for used for redis cache data
    }

    public class TransactionProviderResponse3
    {
        public long ServiceID { get; set; }
        public string ServiceName { get; set; }
        public long ServiceProID { get; set; }
        public long SerProDetailID { get; set; }
        public long RouteID { get; set; }
        public long ProductID { get; set; } // ntrivedi added 03-11-2018
        public string RouteName { get; set; }
        public int ServiceType { get; set; }
        public long ThirPartyAPIID { get; set; }
        public long AppTypeID { get; set; } //Rushabh Updated 11-10-2018 oldName=AppType
        public decimal MinimumAmountItem { get; set; } //Rushabh Updated 11-10-2018 old datatype=long
        public decimal MaximumAmountItem { get; set; } //Rushabh Updated 11-10-2018 old datatype=long
        public string APIBalURL { get; set; } // Khushali  added 29-01-2018
        public string APISendURL { get; set; } // Khushali  added 29-01-2018
        public string APIValidateURL { get; set; } // Khushali  added 29-01-2018
        public string ContentType { get; set; } // Khushali  added 29-01-2018
        public string MethodType { get; set; } // Khushali  added 29-01-2018
        public string OpCode { get; set; } // Khushali  added 29-01-2018
        public int ParsingDataID { get; set; } // Khushali  added 29-01-2018
        public string ProviderWalletID { get; set; } // Khushali  added 29-01-2018
        public long ProTypeID { get; set; } // Khushali  added 29-01-2018
        [NotMapped]
        public string AccNoStartsWith { get; set; } // Rushabh 23-03-2019
        [NotMapped]
        public string AccNoValidationRegex { get; set; } // Rushabh 23-03-2019
        [NotMapped]
        public int AccountNoLen { get; set; } // Rushabh 23-03-2019
        public decimal ConvertAmount { get; set; }
    }

    public class TransactionProviderResponse2
    {
        public long ServiceID { get; set; }
        public string ServiceName { get; set; }
        public long ServiceProID { get; set; }
        public long SerProDetailID { get; set; }
        public long RouteID { get; set; }
        public long ProductID { get; set; } // ntrivedi added 03-11-2018
        public string RouteName { get; set; }
        public int ServiceType { get; set; }
        public long ThirPartyAPIID { get; set; }
        public long AppTypeID { get; set; } //Rushabh Updated 11-10-2018 oldName=AppType
        public decimal MinimumAmountItem { get; set; } //Rushabh Updated 11-10-2018 old datatype=long
        public decimal MaximumAmountItem { get; set; } //Rushabh Updated 11-10-2018 old datatype=long
        public string APIBalURL { get; set; } // Khushali  added 29-01-2018
        public string APISendURL { get; set; } // Khushali  added 29-01-2018
        public string APIValidateURL { get; set; } // Khushali  added 29-01-2018
        public string ContentType { get; set; } // Khushali  added 29-01-2018
        public string MethodType { get; set; } // Khushali  added 29-01-2018
        public string OpCode { get; set; } // Khushali  added 29-01-2018
        public int ParsingDataID { get; set; } // Khushali  added 29-01-2018
        public string ProviderWalletID { get; set; } // Khushali  added 29-01-2018
        public long ProTypeID { get; set; } // Khushali  added 29-01-2018
        [NotMapped]
        public string AccNoStartsWith { get; set; } // Rushabh 23-03-2019
        [NotMapped]
        public string AccNoValidationRegex { get; set; } // Rushabh 23-03-2019
        [NotMapped]
        public int AccountNoLen { get; set; } // Rushabh 23-03-2019        
        public long AddressId { get; set; }        
        public string Address { get; set; }        
        public string RefKey { get; set; }
        public decimal ConvertAmount { get; set; }
    }


    public class WebApiConfigurationResponse
    { 
        public long ThirPartyAPIID { get; set; }
        public string APISendURL { get; set; }
        public string APIValidateURL { get; set; }
        public string APIBalURL { get; set; }
        public string APIStatusCheckURL { get; set; }
        public string APIRequestBody { get; set; }
        public string TransactionIdPrefix { get; set; }
        public string MerchantCode { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string AuthHeader { get; set; }
        public string ContentType { get; set; }
        public string MethodType { get; set; }
        public string HashCode { get; set; }
        public string HashCodeRecheck { get; set; }
        public short HashType { get; set; }
        public short AppType { get; set; }
    }
    //ntrivedi moved from webapiparseresponsecls.cs
    public class GetDataForParsingAPI
    {
        public string ResponseSuccess { get; set; } = "";
        public string ResponseFailure { get; set; } = "";
        public string ResponseHold { get; set; } = "";
        public string BalanceRegex { get; set; } = "";
        public string StatusRegex { get; set; } = "";
        public string StatusMsgRegex { get; set; } = "";
        public string ResponseCodeRegex { get; set; } = "";
        public string ErrorCodeRegex { get; set; } = "";
        public string TrnRefNoRegex { get; set; } = "";
        public string OprTrnRefNoRegex { get; set; } = "";
        public string Param1Regex { get; set; } = "";
        public string Param2Regex { get; set; } = "";
        public string Param3Regex { get; set; } = "";
        public string Param4Regex { get; set; } = "";
        public string Param5Regex { get; set; } = "";
        public string Param6Regex { get; set; } = "";
        public string Param7Regex { get; set; } = "";
    }
    public class GetTradeSettlePrice
    {
        public Decimal? SettlementPrice { get; set; }
    }

    public class ServiceMasterResponse
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string SMSCode { get; set; }
        public short ServiceType { get; set; }
        public string ServiceDetailJson { get; set; }
        public long CirculatingSupply { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal IssuePrice { get; set; }
        public short TransactionBit { get; set; }
        public short WithdrawBit { get; set; }
        public short DepositBit { get; set; }
        public short Status { get; set; }
        public long WalletTypeID { get; set; }
        public short IsOnlyIntAmountAllow { get; set; }
        public decimal Rate { get; set; }
        public long CurrencyTypeId { get; set; }
    }
    public class GetGraphResponsePairWise
    {
        public long DataDate { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal OpenVal { get; set; }
        public decimal CloseVal { get; set; }
        public decimal Volume { get; set; }
        public string PairName { get; set; }
    }

    public class LPStatusCheckCls : IRequest  /// khushali 23-01-2019  for LP status check
    {
        public Guid uuid { get; set; } = Guid.NewGuid();
    }

    public class LPStatusCheckClsArbitrage : IRequest  /// Rushabh 13-06-2019  for LP status check arbitrage
    {
        public Guid uuid { get; set; } = Guid.NewGuid();
    }

    public class StopLossClsArbitarge : IRequest  // khushali 27-07-2019  for stop loss cron
    {
        public Guid uuid { get; set; } = Guid.NewGuid();
    }

    public class TradeStopLossArbitarge  // khushali 27-07-2019  for stop loss cahce class - max try to cancel 3 time.
    {
        public long TrnNo { get; set; }
        public short MaxTry { get; set; }
    }

    public class LPStatusCheckData : IRequest
    {
        public long AppTypeID { get; set; }
        public long TrnNo { get; set; }
        public short Ordertype { get; set; }
        public short  TrnType { get; set; }
        public short  Status { get; set; }
        public decimal Price { get; set; }
        public decimal  Amount { get; set; }
        public DateTime  DateTime { get; set; }
        public string  TrnRefNo { get; set; }
        public string  Pair { get; set; }
        public long SerProDetailID { get; set; }
    }

    public class LPStatusCheckDataArbitrage : IRequest
    {
        public long AppTypeID { get; set; }
        public long TrnNo { get; set; }
        public short Ordertype { get; set; }
        public short TrnType { get; set; }
        public short Status { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }
        public string TrnRefNo { get; set; }
        public string Pair { get; set; }
        public long SerProDetailID { get; set; }
    }

    public class StopLossArbitargeResponse
    {
        public long TrnNo { get; set; }
        public string PairName { get; set; }        
        public string APIKey { get; set; }
        public string SecretKey { get; set; }
        public long LPType { get; set; }        
        public long LPOrderID { get; set; }
        public long MemberID { get; set; }
    }



    public class ReleaseOrdercls : IRequest
    {
        public DateTime DateTime { get; set; }
    }

    public class StuckOrdercls : IRequest
    {
        public DateTime DateTime { get; set; }
    }

    public class MarginStuckOrdercls : IRequest
    {
        public DateTime DateTime { get; set; }
    }

    public class ReleaseAndStuckOrdercls
    {
        public long TrnNo { get; set; }
    }

    public class ReleaseArbitrageLPOrder : IRequest
    {
        public DateTime DateTime { get; set; }
    }

    public class ArbitrageTransactionProviderResponse
    {
        public long ServiceID { get; set; }
        public string ServiceName { get; set; }
        public long ServiceProID { get; set; }
        public long SerProDetailID { get; set; }
        public long RouteID { get; set; }
        public long ProductID { get; set; } 
        public string RouteName { get; set; }
        public int ServiceType { get; set; }
        public long ThirPartyAPIID { get; set; }
        public long AppTypeID { get; set; } 
        public decimal MinimumAmountItem { get; set; }
        public decimal MaximumAmountItem { get; set; }
        public string APIBalURL { get; set; } 
        public string APISendURL { get; set; } 
        public string APIValidateURL { get; set; }
        public string ContentType { get; set; } 
        public string MethodType { get; set; } 
        public string OpCode { get; set; }
        public int ParsingDataID { get; set; } 
        public string ProviderWalletID { get; set; }
        public long ProTypeID { get; set; }  
        public string AccNoStartsWith { get; set; }       
        public string AccNoValidationRegex { get; set; }      
        public int AccountNoLen { get; set; } 
        public short IsAdminApprovalRequired { get; set; }
    }
    //Darshan Dholakiya--add class for Retrive CCXT data-25-07-2019
    public class CCXTTranNo
    {
        public long TrnNo { get; set; }
        public string TrnRefNo { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public decimal SettledQty { get; set; }
        public short Ordertype { get; set; }
        public string IsBulkOrder { get; set; }
        public long SerProDetailID { get; set; }
    }
}
