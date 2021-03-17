using Worldex.Core.Enums;

namespace Worldex.Core.ApiModels
{
    public class CreditWalletDrArryTrnID
    {
        public long DrTrnRefNo { get; set; }
        public decimal Amount { get; set; }
        public long dWalletId { get; set; }
        public long OrderID { get; set; }
        public long DrTQTrnNo { get; set; }
    }

    public class WalletCrDr
    {
        public string token { get; set; } // currently not in use
        public long TrnRefNo { get; set; } // tq trnno 
        public long WalletId { get; set; } //
        public enWalletTrnType trnType { get; set; }
        public short isFullSettled;
        public long OrderID { get; set; }
        public long WTQTrnNo { get; set; }
        public long UserID { get; set; } // for notification
        public short isMarketTrade { get; set; }
        public decimal differenceAmount { get; set; }
        public short IsMaker { get; set; }
        public decimal Charge { get; set; }
        public string RefGuid { get; set; } = "";
    }

    public class CommonClassCrDr
    {
        public decimal Amount { get; set; }
        public string Coin { get; set; }
        public WalletCrDr creditObject;
        public WalletCrDr debitObject;
    }
    
    public class TempEntity
    {
        public long TrnNo { get; set; }
        public decimal SetteledAmount { get; set; }
        public decimal Amount { get; set; }
    }


    public class MarginWalletCrDr
    {
        public string token { get; set; } // currently not in use
        public long TrnRefNo { get; set; } // tq trnno 
        public long WalletId { get; set; } //
        public enMarginWalletTrnType trnType { get; set; }
        public short isFullSettled;
        public long OrderID { get; set; }
        public long WTQTrnNo { get; set; }
        public long UserID { get; set; } // for notification
        public short isMarketTrade { get; set; }
        public decimal differenceAmount { get; set; }
        public short IsMaker { get; set; }
        public decimal Charge { get; set; }
        public enWalletDeductionType OrderType;
        public string RefGuid { get; set; } = "";//2019-7-17 added refguid
    }

    public class MarginCommonClassCrDr
    {
        public decimal Amount { get; set; }
        public string Coin { get; set; }
        public MarginWalletCrDr creditObject;
        public MarginWalletCrDr debitObject;
    }

    public class MarginPNL
    {
        public decimal Qty { get; set; }
        public decimal BidPrice { get; set; } //base currency
        public decimal LandingPrice { get; set; } //first currency
        public long PairID { get; set; }
        public string BaseCurrency { get; set; } // leverage currency BTC
        public string SecondCurrency { get; set; } // currency ATCC
    }
    //ntrivedi 02-04-2019
    public class StopLimitOrderPrice
    {
        public decimal ProfitBalance { get; set; }//pROFIT 17-04-2019 LOAN 
        public decimal SafetyBalance { get; set; }
        public decimal SafetyBalanceAferCharge { get; set; }
        public decimal Charge { get; set; }
        public decimal AvgLanding { get; set; }
        public decimal AvgQty { get; set; }
        public decimal AvgBidPrice { get; set; }
        public decimal MinLanding { get; set; }
        public decimal FinalBidPrice { get; set; }
        public BizResponseClass BizResponseClass { get; set; }
        public long UserID { get; set; }
        public long PairID { get; set; }
        public string baseCurrency { get; set; }
        public decimal BuyProfitLanding { get; set; } // till date profit open position

    }

    public class LPHoldDr
    {
        public long SerProID { get; set; }
        public string CoinName { get; set; }
        public string Timestamp { get; set; }
        public decimal Amount { get; set; }
        public long TrnRefNo { get; set; }
        public enWalletTrnType trnType { get; set; }
        public enWalletDeductionType enWalletDeductionType { get; set; }
        public long WalletID { get; set; }
        public long TrnNo { get; set; }
        public long PairId { get; set; }
        public string RefGuid { get; set; } = "";

        public LPHoldDr()
        {
            enWalletDeductionType = enWalletDeductionType.Normal;
        }
    }
    public class ArbitrageCommonClassCrDr // arbitrage class added
    {
        public long SerProID { get; set; }
        public decimal Amount { get; set; }
        public decimal HoldAmount { get; set; }
        public string Coin { get; set; }
        public string HoldCoin { get; set; }
        public string token { get; set; } // currently not in use
        public long TrnRefNo { get; set; } // tq trnno 
        public long WalletId { get; set; } //
        public enWalletTrnType trnType { get; set; }
        public short isFullSettled;
        public long OrderID { get; set; }
        public long WTQTrnNo { get; set; }
        public long UserID { get; set; } // for notification
        public short isMarketTrade { get; set; }
        public decimal differenceAmount { get; set; }
        public short IsMaker { get; set; }
        public decimal Charge { get; set; }
        public enWalletDeductionType OrderType;
        public string RefGuid { get; set; } = "";
    }
}
