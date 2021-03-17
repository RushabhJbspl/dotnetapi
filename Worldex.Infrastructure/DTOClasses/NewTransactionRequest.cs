using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Worldex.Infrastructure.DTOClasses
{
    [Serializable]
    public class NewTransactionCommonCls
    {
        public long TrnNo { get; set; }

        public Guid GUID { get; set; }

        public short TrnMode { get; set; }

        public enTrnType TrnType { get; set; }

        public enTransactionMarketType ordertype { get; set; } // market type
        public enTransactionMarketType1 ordertype1 { get; set; }//add for huobi
        public long AccountID { get; set; }
       

        public long MemberID { get; set; }

        public string MemberMobile { get; set; }


        [StringLength(10)]
        public string SMSCode { get; set; }


        [StringLength(200)]
        public string TransactionAccount { get; set; }//Mob for txn , address for crypto


        [Range(0, 9999999999.999999999999999999)]
        public decimal Amount { get; set; }

        public long PairID { get; set; } = 0;

        [Range(0, 9999999999.999999999999999999)]
        public decimal Price { get; set; } = 0;

        [Range(0, 9999999999.999999999999999999)]
        public decimal Qty { get; set; } = 0;

        public string TrnRefNo { get; set; }
        public string AdditionalInfo { get; set; }
        public enTransactionStatus Status { get; set; }
        public long StatusCode { get; set; }
        public string StatusMsg { get; set; }
        //public long WalletID { get; set; }
        //public long DeliveryWalletID { get; set; } //use only in case of Trading

        public string DebitAccountID { get; set; }

        public string CreditAccountID { get; set; }

        public long DebitWalletID { get; set; }

        public long CreditWalletID { get; set; }

        public enServiceType ServiceType { get; set; }
        public enWalletTrnType WalletTrnType { get; set; }

        public enWhiteListingBit WhitelistingBit { get; set; }

        public string AddressLabel { get; set; }
        public string accessToken { get; set; }

        [Range(0, 9999999999.999999999999999999)]
        public decimal StopPrice { get; set; }

        public short ISFollowersReq { get; set; } = 0;//Rita 12-1-19 main req always 0
        public long FollowingTo { get; set; } = 0;//Rita 21-1-19 main req always 0
        public long LeaderTrnNo { get; set; } = 0;//Rita 21-1-19 main req always 0
        public string FollowTradeType { get; set; } = "";//Rita 22-1-19 main req always blank
        public short LPType { get; set; } = 0; //khushali 10-06-2019 for Route Trade on Specific LP
        public short IsPrivateAPITrade { get; set; } = 0; //komal 3-10-2019 for Private API
    }

    [Serializable]
    public class NewTransactionRequestCls : NewTransactionCommonCls, IRequest
    {
        //https://www.codeproject.com/Articles/28952/%2FArticles%2F28952%2FShallow-Copy-vs-Deep-Copy-in-NET
        public object Clone()//for copy object , shallow copy,value copy but inner class object refere same object reference
        {
            return MemberwiseClone();
        }
        public NewTransactionRequestCls CreateDeepCopy(NewTransactionRequestCls inputcls)//rita 9-1-19 , this not used as im using Helper's common method,DeepCopy
        {
            MemoryStream m = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(m, inputcls);
            m.Position = 0;
            return (NewTransactionRequestCls)b.Deserialize(m);
        }
    }

    public class NewTradeTransactionRequestCls
    {
        //public long TrnNo { get; set; }
        public string TrnTypeName { get; set; } = "";
        public string PairName { get; set; } = "";
        public long OrderWalletID { get; set; } = 0;
        public long DeliveryWalletID { get; set; } = 0;
        [Range(0, 9999999999.999999999999999999)]
        public decimal BuyQty { get; set; } = 0;
        [Range(0, 9999999999.999999999999999999)]
        public decimal BidPrice { get; set; } = 0;
        [Range(0, 9999999999.999999999999999999)]
        public decimal SellQty { get; set; } = 0;
        [Range(0, 9999999999.999999999999999999)]
        public decimal AskPrice { get; set; } = 0;
        public string Order_Currency { get; set; } = "";
        [Range(0, 9999999999.999999999999999999)]
        public decimal OrderTotalQty { get; set; } = 0;
        public string Delivery_Currency { get; set; } = "";
        [Range(0, 9999999999.999999999999999999)]
        public decimal DeliveryTotalQty { get; set; } = 0;
        //public long? TrnRefNo { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal SettledBuyQty { get; set; } = 0;
        [Range(0, 9999999999.999999999999999999)]
        public decimal SettledSellQty { get; set; } = 0;
        //public decimal TakerPer { get; set; }
    }

    //Rita 15-2-19 for margin trading=====
    [Serializable]
    public class NewTransactionRequestMarginCls : NewTransactionCommonCls, IRequest
    {
        public short IsWithoutAmtHold { get; set; } = 0;//Rita 29-3-19 default 0 , 1 for NO amount hold and direct Cr/Dr as settlement time
        public short ISOrderBySystem { get; set; } = 0;//Rita 29-3-19 default 0 , 1 for NO amount hold and direct Cr/Dr as settlement time

        //https://www.codeproject.com/Articles/28952/%2FArticles%2F28952%2FShallow-Copy-vs-Deep-Copy-in-NET
        public object Clone()//for copy object , shallow copy,value copy but inner class object refere same object reference
        {
            return MemberwiseClone();
        }
        public NewTransactionRequestMarginCls CreateDeepCopy(NewTransactionRequestMarginCls inputcls)//rita 9-1-19 , this not used as im using Helper's common method,DeepCopy
        {
            MemoryStream m = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(m, inputcls);
            m.Position = 0;
            return (NewTransactionRequestMarginCls)b.Deserialize(m);
        }
    }
    public class NewTradeTransactionRequestMarginCls : NewTradeTransactionRequestCls
    {

    }
    //====================================
    //Rita 04-06-19 for Arbitrage trading=====
    [Serializable]
    public class NewTransactionRequestArbitrageCls : NewTransactionCommonCls, IRequest
    {
        public short IsSmartArbitrage { get; set; } = 0;//for profit log  

        //https://www.codeproject.com/Articles/28952/%2FArticles%2F28952%2FShallow-Copy-vs-Deep-Copy-in-NET
        public object Clone()//for copy object , shallow copy,value copy but inner class object refere same object reference
        {
            return MemberwiseClone();
        }
        public NewTransactionRequestArbitrageCls CreateDeepCopy(NewTransactionRequestArbitrageCls inputcls)
        {
            MemoryStream m = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(m, inputcls);
            m.Position = 0;
            return (NewTransactionRequestArbitrageCls)b.Deserialize(m);
        }
    }
    public class NewTradeTransactionRequestArbitrageCls : NewTradeTransactionRequestCls
    {

    }
    //====================================
    public class ProcessTransactionCls
    {
        [Range(0, 9999999999.999999999999999999)]
        public decimal BidPrice_TQ { get; set; }

        [Range(0, 9999999999.999999999999999999)]
        public decimal BidPrice_BuyReq { get; set; }

        public long Delivery_ServiceID { get; set; }
        public long Order_ServiceID { get; set; }
        //public long Pool_OrderID { get; set; }

        public long TransactionRequestID { get; set; }

        public string APIResponse { get; set; }

    }

    public class NewWithdrawRequestCls : IRequest
    {
        public long TrnNo { get; set; }

        public Guid GUID { get; set; }

        public short TrnMode { get; set; }

        public enTrnType TrnType { get; set; }

        public long MemberID { get; set; }

        public string MemberMobile { get; set; }

        [StringLength(10)]
        public string SMSCode { get; set; }

        [StringLength(200)]
        public string TransactionAccount { get; set; }//Mob for txn , address for crypto

        [Range(0, 9999999999.999999999999999999)]
        public decimal Amount { get; set; }

        public string TrnRefNo { get; set; }
        public string AdditionalInfo { get; set; }
        public enTransactionStatus Status { get; set; }
        public long StatusCode { get; set; }
        public string StatusMsg { get; set; }
        //public long WalletID { get; set; }
        //public long DeliveryWalletID { get; set; } //use only in case of Trading

        public string DebitAccountID { get; set; }
        public long DebitWalletID { get; set; }

        public enServiceType ServiceType { get; set; }
        public enWalletTrnType WalletTrnType { get; set; }

        public enWhiteListingBit WhitelistingBit { get; set; }

        public string AddressLabel { get; set; }
        public string accessToken { get; set; }
        public short IsInternalTrn { get; set; }  //Uday 11-01-2019 Check For Withdrwal Interanl Transaction
    }

    public class NewCancelOrderRequestCls : IRequest
    {
        public long TranNo { get; set; }
        public string accessToken { get; set; }
        public long MemberID { get; set; }//Rita 3-1-19 instead of access tocken
        public short CancelAll { get; set; }//0-singal trx,1-All,2-Marketwise. //komal 28-01-2018 
        public enTransactionMarketType OrderType { get; set; }
        public short IsMargin { get; set; } = 0;//1-Margin trading Rita 21-2-19
        public string TrnRefNo { get; set; } = "";//Rita 4-10-19 for margin trade , inform wallet team for cancellation success
        public EnAllowedChannels TrnMode { get; set; } = 0;//Rita 4-10-19 for margin trade , inform wallet team for cancellation success
        public int CancelOrderCount { get; set; } = 0;//Rushabh 19-08-2020 For CancelAll Activity Notification Handling
    }
    public class CancelOrderRequestEvent : IRequest
    {
        public NewCancelOrderRequestCls Request { get; set; }
        public int CancelOrderCount { get; set; } = 0;//Rushabh 19-08-2020 For CancelAll Activity Notification Handling
    }

    //komal 07-06-2019 cancel arbitrage Trade
    public class NewCancelOrderArbitrageRequestCls : IRequest
    {
        public long TranNo { get; set; }
        public string accessToken { get; set; }
        public long MemberID { get; set; }//Rita 3-1-19 instead of access tocken
        public short CancelAll { get; set; }//0-singal trx,1-All,2-Marketwise. //komal 28-01-2018 
        public enTransactionMarketType OrderType { get; set; }
        //public short IsMargin { get; set; } = 0;//1-Margin trading Rita 21-2-19
        public string TrnRefNo { get; set; } = "";//Rita 4-10-19 for margin trade , inform wallet team for cancellation success
        public EnAllowedChannels TrnMode { get; set; } = 0;//Rita 4-10-19 for margin trade , inform wallet team for cancellation success
        public short IsFromStuckOrder { get; set; } = 0;
    }

    public class FollowersOrderRequestCls : IRequest
    {
        public NewTransactionRequestCls Req { get; set; }

        public string Order_Currency { get; set; } = "";
        public string Delivery_Currency { get; set; } = "";
    }
    public class PairDataCalculationCls : IRequest //Rita 23-1-19 for backgoup pairdata calculation
    {
        public string TempVar { get; set; } = "";
    }
    public class LPCancelOrdedrResponse : BizResponse
    {
        public long TrnNo { get; set; }
        public string TrnRefNo { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal price { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal origQty { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal executedQty { get; set; }
        [Range(0, 9999999999.999999999999999999)]
        public decimal cummulativeQuoteQty { get; set; }
        public enTransactionStatus status { get; set; }
    }

    public class LPProcessTransactionCls
    {
        [Range(0, 9999999999.999999999999999999)]
        public decimal RemainingQty { get; set; }

        [Range(0, 9999999999.999999999999999999)]
        public decimal SettledQty { get; set; }

        [Range(0, 9999999999.999999999999999999)]
        public decimal TotalQty { get; set; }

    }

    //khushali 26-04-2019 for trade recon
    public class NewCancelOrderRequestClsV2 : IRequest<NewCancelOrderResponseClsV2>
    {
        public long TranNo { get; set; }
        public string accessToken { get; set; }
        public long MemberID { get; set; }//Rita 3-1-19 instead of access tocken
        public short CancelAll { get; set; }//0-singal trx,1-All,2-Marketwise. //komal 28-01-2018 
        public enTransactionMarketType OrderType { get; set; }
        public short IsMargin { get; set; } = 0;//1-Margin trading Rita 21-2-19
        public string TrnRefNo { get; set; } = "";//Rita 4-10-19 for margin trade , inform wallet team for cancellation success
        public EnAllowedChannels TrnMode { get; set; } = 0;//Rita 4-10-19 for margin trade , inform wallet team for cancellation success
    }
    public class NewCancelOrderResponseClsV2 : BizResponse, IRequest
    {

    }

    public class NewCancelOrderRequestArbitrageClsV2 : IRequest<NewCancelOrderResponseArbitrageClsV2>
    {
        public long TranNo { get; set; }
        public string accessToken { get; set; }
        public long MemberID { get; set; }//Rita 3-1-19 instead of access tocken
        public short CancelAll { get; set; }//0-singal trx,1-All,2-Marketwise. //komal 28-01-2018 
        public enTransactionMarketType OrderType { get; set; }
        public short IsMargin { get; set; } = 0;//1-Margin trading Rita 21-2-19
        public string TrnRefNo { get; set; } = "";//Rita 4-10-19 for margin trade , inform wallet team for cancellation success
        public EnAllowedChannels TrnMode { get; set; } = 0;//Rita 4-10-19 for margin trade , inform wallet team for cancellation success
    }
    public class NewCancelOrderResponseArbitrageClsV2 : BizResponse, IRequest
    {

    }
}
