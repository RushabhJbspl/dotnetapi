using Worldex.Core.Entities;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Transaction;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces
{
    public interface ISettlementRepository<T>
    {
        Task<T> PROCESSSETLLEMENT(T _Resp, TradeBuyRequest TradeBuyRequestObj, ParallelProcessTrns ParallelTrnsObj, string accesstocken = "", short IsCancel = 0);
    }
    public interface ISettlementRepositoryV1<T>
    {
        Task<T> PROCESSSETLLEMENTBuy(T _Resp, TradeTransactionQueue TradeTransactionQueueObj, TransactionQueue TransactionQueueObj, TradeStopLoss TradeStopLossObj, TradeBuyerListV1 CurrentTradeBuyerListObj, string accessToken = "", short IsCancel = 0);
        Task<T> PROCESSSETLLEMENTSell(T _Resp, TradeTransactionQueue TradeTransactionQueueObj, TransactionQueue TransactionQueueObj, TradeStopLoss TradeStopLossObj, TradeSellerListV1 CurrentTradeSellerListObj, string accessToken = "", short IsCancel = 0);
        Task<bool> ReleaseWalletAmountBuy(TradeBuyerListV1 CurrentTradeBuyerListObj, TransactionQueue TransactionQueueObj, TradeTransactionQueue TradeTransactionQueueObj, short IsPartial = 0);
        Task<bool> ReleaseWalletAmountSell(TradeSellerListV1 CurrentTradeSellerListObj, TransactionQueue TransactionQueueObj, TradeTransactionQueue TradeTransactionQueueObj, short IsPartial = 0);
        Task<bool> Callsp_TradeSettlement(long TrnNo, long MakerTrnNo, decimal SettlementQty, decimal BaseCurrQty, decimal SettlementPrice, short ActionType, short ActionStage, long ErrorCode = 0, long UpdatedBy = 0);
    }
    public interface ISettlementRepositoryMarginV1<T>
    {
        Task<T> PROCESSSETLLEMENTBuy(T _Resp, TradeTransactionQueueMargin TradeTransactionQueueObj, TransactionQueueMargin TransactionQueueObj, TradeStopLossMargin TradeStopLossObj, TradeBuyerListMarginV1 CurrentTradeBuyerListObj, string accessToken = "", short IsCancel = 0);
        Task<T> PROCESSSETLLEMENTSell(T _Resp, TradeTransactionQueueMargin TradeTransactionQueueObj, TransactionQueueMargin TransactionQueueObj, TradeStopLossMargin TradeStopLossObj, TradeSellerListMarginV1 CurrentTradeSellerListObj, string accessToken = "", short IsCancel = 0);
        Task<bool> SystemCreatedOrderAlsoFromSiteToken(long CurrObjTrnNo, long ProcessTrnNo, long PairID, string PairName, long UserID, string baseCurrency, string DebitAccountID, string CreditAccountID, decimal LTP, string MemberMobile, string accessToken);
        Task<bool> Callsp_TradeSettlement(long TrnNo, long MakerTrnNo, decimal SettlementQty, decimal BaseCurrQty, decimal SettlementPrice, short ActionType, short ActionStage, long ErrorCode = 0, long UpdatedBy = 0);
    }
    public interface ISettlementRepositoryArbitrageV1<T>
    {
        Task<T> PROCESSSETLLEMENTBuy(T _Resp, TradeTransactionQueueArbitrage TradeTransactionQueueObj, TransactionQueueArbitrage TransactionQueueObj, TradeStopLossArbitrage TradeStopLossObj, TradeBuyerListArbitrageV1 CurrentTradeBuyerListObj, string accessToken = "", short IsCancel = 0);
        Task<T> PROCESSSETLLEMENTSell(T _Resp, TradeTransactionQueueArbitrage TradeTransactionQueueObj, TransactionQueueArbitrage TransactionQueueObj, TradeStopLossArbitrage TradeStopLossObj, TradeSellerListArbitrageV1 CurrentTradeSellerListObj, string accessToken = "", short IsCancel = 0);
        Task<bool> ReleaseWalletAmountBuy(TradeBuyerListArbitrageV1 CurrentTradeBuyerListObj, TransactionQueueArbitrage TransactionQueueObj, TradeTransactionQueueArbitrage TradeTransactionQueueObj, short IsPartial = 0);
        Task<bool> ReleaseWalletAmountSell(TradeSellerListArbitrageV1 CurrentTradeSellerListObj, TransactionQueueArbitrage TransactionQueueObj, TradeTransactionQueueArbitrage TradeTransactionQueueObj, short IsPartial = 0);
        Task<bool> Callsp_TradeSettlement(long TrnNo, long MakerTrnNo, decimal SettlementQty, decimal BaseCurrQty, decimal SettlementPrice, short ActionType, short ActionStage, long ErrorCode = 0, long UpdatedBy = 0);
    }
    public interface IMarginCreateOrderFromWallet
    {
        //Rita 3-4-19 added for placing order from charge canculation in day end crone
        Task<RespnseToWallet> PlaceMarketSELLOrder(EnAllowedChannels TrnMode, long UserID, string MobileNo, long PairID, decimal Qty, string DebitAccountID, string CreditAccountID, string TrnRefNo, string accessToken);
        Task<RespnseToWallet> ReleaseOrderForNoOpenPosition(EnAllowedChannels TrnMode, long UserID, decimal Qty, long BatchNo, string BaseCurrency, string TrnRefNo, string accessToken);
        //Task<CloseOpenPostionResponseMargin> CloseOpenPostionMargin(long PairID, long UserID, string accessToken);
    }
    public interface IMarginClosePosition
    {
        Task<CloseOpenPostionResponseMargin> CloseOpenPostionMargin(long PairID, long UserID, string accessToken);
    }

    public interface ISettlementRepositoryAPI<T>
    {
        Task<T> PROCESSSETLLEMENTAPI(T _Resp, long TrnNo, decimal APIRemainQty, decimal APISettledQty, decimal APIPrice);
        Task<T> PROCESSSETLLEMENTAPIFromInit(T _Resp, long TrnNo, decimal APIRemainQty, decimal APISettledQty, decimal APIPrice, TransactionQueue TransactionQueueObj, TradeTransactionQueue TradeTranQueueObj, TradeStopLoss TradeStopLossObj, TradeBuyerListV1 TradeBuyerListObj, TradeSellerListV1 TradeSellerListObj);
    }
    //public interface ISettlementRepositoryArbitrageAPI<T>
    //{
    //    Task<T> PROCESSSETLLEMENTAPI(T _Resp, long TrnNo, decimal APIRemainQty, decimal APISettledQty, decimal APIPrice);
    //    Task<T> PROCESSSETLLEMENTAPIFromInit(T _Resp, long TrnNo, decimal APIRemainQty, decimal APISettledQty, decimal APIPrice, TransactionQueueArbitrage TransactionQueueObj, TradeTransactionQueueArbitrage TradeTranQueueObj, TradeStopLossArbitrage TradeStopLossObj, TradeBuyerListArbitrageV1 TradeBuyerListObj, TradeSellerListArbitrageV1 TradeSellerListObj);
    //}

    public interface ICancelOrderRepository
    {
        bool UpdateDataObjectWithBeginTransaction(TradeCancelQueue tradeCancelQueue, TradeTransactionQueue TradeTranQueueObj, PoolOrder PoolOrderObj, TradeBuyRequest NewBuyRequestObj);
        bool UpdateDataObjectWithBeginTransactionV1(TradeCancelQueue tradeCancelQueue, TransactionQueue TransactionQueueObj, TradeTransactionQueue TradeTransactionQueueObj, TradeBuyerListV1 BuyerListObj, TradeSellerListV1 SellerListObj, SettledTradeTransactionQueue SettledTradeTQObj, short ISPartialSettled);
    }
    public interface ICancelOrderRepositoryMargin
    {
        bool UpdateDataObjectWithBeginTransactionV1(TradeCancelQueueMargin tradeCancelQueue, TransactionQueueMargin TransactionQueueObj, TradeTransactionQueueMargin TradeTransactionQueueObj, TradeBuyerListMarginV1 BuyerListObj, TradeSellerListMarginV1 SellerListObj, SettledTradeTransactionQueueMargin SettledTradeTQObj, short ISPartialSettled);
    }
    public interface ICancelOrderRepositoryArbitrage
    {
        bool UpdateDataObjectWithBeginTransactionV1(TradeCancelQueueArbitrage tradeCancelQueue, TransactionQueueArbitrage TransactionQueueObj, TradeTransactionQueueArbitrage TradeTransactionQueueObj, TradeBuyerListArbitrageV1 BuyerListObj, TradeSellerListArbitrageV1 SellerListObj, SettledTradeTransactionQueueArbitrage SettledTradeTQObj, short ISPartialSettled);
    }
}
