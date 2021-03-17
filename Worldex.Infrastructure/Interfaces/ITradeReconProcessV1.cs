using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Infrastructure.DTOClasses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Interfaces
{
    public interface ITradeReconProcessV1
    {
        Task<BizResponseClass> TradeReconProcessAsyncV1(enTradeReconActionType ActionType, long TranNo, string ActionMessage, long UserId, string AccessToken);
        //Rita 25-04-19 added this for calling from cancellation , as txn found in isprocessing=1
        Task<BizResponseClass> ProcessReleaseStuckOrderOrderAsync(BizResponseClass Response, TradeTransactionQueue TrnObj, long UserID,short IsFromCancellation=0);
        short CheckBuyerSellerListIsProcessing(TradeTransactionQueue TradeTransactionQueueObj);
    }

    public interface ITradeReconProcessMarginV1
    {
        Task<BizResponseClass> ProcessReleaseStuckOrderOrderAsync(BizResponseClass Response, TradeTransactionQueueMargin TrnObj, long UserID, short IsFromCancellation = 0);
        short CheckBuyerSellerListIsProcessing(TradeTransactionQueueMargin TradeTransactionQueueObj);
    }

    //public interface ITradeReconProcessArbitrageV1
    //{
    //    Task<BizResponseClass> TradeReconProcessArbitrageAsyncV1(enTradeReconActionType ActionType, long TranNo, string ActionMessage, long UserId, string AccessToken);
    //    Task<BizResponseClass> ProcessReleaseStuckOrderOrderArbitrageAsync(BizResponseClass Response, TradeTransactionQueueArbitrage TrnObj, long UserID, short IsFromCancellation = 0);
    //    short CheckBuyerSellerListIsProcessing(TradeTransactionQueueArbitrage TradeTransactionQueueObj);
    //}
}
