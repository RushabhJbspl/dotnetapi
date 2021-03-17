using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.Arbitrage;
using Worldex.Core.ViewModels.Wallet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces
{
    public interface ISignalRService
    {
        Task RemoveActiveOrder(List<long> TrnList, long UserID);
        Task OrderHistory(GetOrderHistoryInfo Data, string Pair, string UserID, short IsMargin = 0);
        Task ChartDataEveryLastMin(DateTime DateTime, short IsMargin = 0);
        Task BulkBuyerBook(List<GetBuySellBook> Data, string Pair, enLiquidityProvider LP, short IsMargin = 0);
        Task BulkSellerBook(List<GetBuySellBook> Data, string Pair, enLiquidityProvider LP, short IsMargin = 0);
        Task BulkOrderHistory(List<GetTradeHistoryInfoV1> Data, string Pair, enLiquidityProvider LP, short IsMargin = 0);
        Task MarketTicker(List<VolumeDataRespose> Data, string UserID, string Base="", short IsMargin = 0);

        //Event Call
        Task OnStatusPartialSuccess(short Status, TransactionQueue Newtransaction, TradeTransactionQueue NewTradeTransaction, string Token, short OrderType);
        Task OnStatusSuccess(short Status, TransactionQueue Newtransaction, TradeTransactionQueue NewTradeTransaction, string Token, short OrderType,decimal SettlementPrice);
        Task OnStatusHold(short Status, TransactionQueue Newtransaction, TradeTransactionQueue NewTradeTransaction, string Token, short OrderType);
        Task OnStatusCancel(short Status, TransactionQueue Newtransaction, TradeTransactionQueue NewTradeTransaction, string Token, short OrderType,short IsPartialCancel= 0, int OrderCount = 1);        
        Task OnVolumeChange(VolumeDataRespose volumeData, MarketCapData capData, string UserID);
        Task OnWalletBalChange(WalletMasterResponse Data, string WalletTypeName, string Token, short TokenType = 1, string TrnNo = "", short IsMargin = 0); //ntrivedi optional parameter added for margin wallet balance change
        
        Task OnLtpChange(Decimal LTP, long Pair, string PairName, short IsCancel = 0, short IsMargin = 0, string UserID = "");

        Task SendActivityNotificationV2(ActivityNotificationMessage ActivityNotification, string Token, short TokenType = 1, string TrnNo = "", short IsMargin = 0);
        string GetTokenByUserID(string ID);
        Task SendWalletActivityList(ListAddWalletRequest ActivityListRequest, string ID, short IsMargin = 0);
        //Rita 20-2-19 for Margin Trading
        Task OnStatusCancelMargin(short Status, TransactionQueueMargin Newtransaction, TradeTransactionQueueMargin NewTradeTransaction, string Token, short OrderType, short IsPartialCancel = 0);
        Task OnStatusPartialSuccessMargin(short Status, TransactionQueueMargin Newtransaction, TradeTransactionQueueMargin NewTradeTransaction, string Token, short OrderType);
        Task OnStatusHoldMargin(short Status, TransactionQueueMargin Newtransaction, TradeTransactionQueueMargin NewTradeTransaction, string Token, short OrderType);
        Task OnStatusSuccessMargin(short Status, TransactionQueueMargin Newtransaction, TradeTransactionQueueMargin NewTradeTransaction, string Token, short OrderType, decimal SettlementPrice);
        Task OnVolumeChangeMargin(VolumeDataRespose volumeData, MarketCapData capData, string UserID);
        Task SendOrderHistory(GetOrderHistoryInfo historyInfo, String PairName, string UserID, short IsMargin = 0);

        //============== Komal 3 June 2019 Arbitrange Trading
        Task MarketTickerArbitrage(List<VolumeDataRespose> Data, string UserID, string Base = "", short IsMargin = 0);
        Task LastPriceArbitrage(LastPriceViewModelArbitrage Data, string Pair, string UserID, short IsMargin = 0);
        Task SellerBookArbitrage(ArbitrageBuySellViewModel Data, string Pair, string UserID, short IsMargin = 0);
        Task BuyerBookArbitrage(ArbitrageBuySellViewModel Data, string Pair, string UserID, short IsMargin = 0);
        Task ProviderMarketDataArbitrage(ExchangeProviderListArbitrage Data, string Pair);
        Task ProfitIndicatorArbitrage(ProfitIndicatorInfo Data, string Pair);
        Task ExchangeListSmartArbitrage(List<ExchangeListSmartArbitrage> Data, string Pair);
   
        Task OnStatusSuccessArbitrage(short Status, TransactionQueueArbitrage Newtransaction, TradeTransactionQueueArbitrage NewTradeTransaction, string Token, short OrderType, decimal SettlementPrice);
        Task OnStatusPartialSuccessArbitrage(short Status, TransactionQueueArbitrage Newtransaction, TradeTransactionQueueArbitrage NewTradeTransaction, string Token, short OrderType);
        Task OnStatusHoldArbitrage(short Status, TransactionQueueArbitrage Newtransaction, TradeTransactionQueueArbitrage NewTradeTransaction, string Token, short OrderType);
        Task OnStatusCancelArbitrage(short Status, TransactionQueueArbitrage Newtransaction, TradeTransactionQueueArbitrage NewTradeTransaction, string Token, short OrderType, short IsPartialCancel = 0);
        Task OnVolumeChangeArbitrage(VolumeDataRespose volumeData, MarketCapData capData, string UserID);
        Task OnLtpChangeArbitrage(Decimal LTP, long Pair, string PairName, short IsCancel = 0, short IsMargin = 0, string UserID = "");
        Task SendActivityNotificationV2Arbitrage(ActivityNotificationMessage ActivityNotification, string Token, short TokenType = 1, string TrnNo = "", short IsMargin = 0);
        Task OnWalletBalChangeArbitrage(WalletMasterResponse Data, string WalletTypeName, string Token, short TokenType = 1, string TrnNo = "", short IsMargin = 0);
    }
}
