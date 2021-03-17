using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Infrastructure.DTOClasses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Worldex.Core.Entities.User;

namespace Worldex.Infrastructure.Interfaces
{
    public interface ITransactionProcess
    {
        Task<BizResponse> ProcessNewTransactionAsync(NewTransactionRequestCls Req);
    }
    public interface ITransactionProcessV1
    {
        Task<BizResponse> ProcessNewTransactionAsync(NewTransactionRequestCls Req);
    }
    public interface ITransactionProcessMarginV1//Rita 15-2-19 for margin trading saperate
    {
        Task<BizResponse> ProcessNewTransactionAsync(NewTransactionRequestMarginCls Req);
    }
    //Rita 04-06-19 for Arbitrage Trading
    //public interface ITransactionProcessArbitrageV1
    //{
    //    Task<BizResponse> ProcessNewTransactionArbitrageAsync(NewTransactionRequestArbitrageCls Req);
    //    Task<string> ConnectToExchangeAsync(TransactionProviderArbitrageResponse Provider, TransactionQueueArbitrage TQ, long TrnNo);
    //}
    public interface IWithdrawTransaction
    {
        Task<BizResponse> WithdrawTransactionTransactionAsync(NewWithdrawRequestCls Req);
    }
    public interface IWithdrawTransactionV1
    {
        Task<BizResponse> WithdrawTransactionTransactionAsync(NewWithdrawRequestCls Req);
        Task<BizResponse> WithdrawTransactionAPICallProcessAsync(WithdrawalConfirmationRequest Request,long UserId,short IsReqFromAdmin,ApplicationUser user=null);
        Task<BizResponse> ResendEmailWithdrawalConfirmation(long TrnNo, long UserId);
        Task MarkTransactionOperatorFailv2(string StatusMsg, enErrorCode ErrorCode, TransactionQueue Newtransaction);
        Task MarkTransactionOperatorFail(string StatusMsg, enErrorCode ErrorCode, TransactionQueue Newtransaction);
        long InsertIntoTransactionRequest(InsertIntoTransactionRequyest listObj);
        void UpdateTransactionRequest(long ApiId, string RequestBody);
        void UpdateIntoTransactionRequest(long ApiId, string RequestBody);
        Task<BizResponse> FiatWithdrawTransactionTransactionAsync(NewWithdrawRequestCls Req1);
        Task<BizResponse> FiatWithdrawTransactionAPICallProcessAsync(WithdrawalConfirmationRequest Request, long UserId, short IsReqFromAdmin, ApplicationUser user = null);
    }
    //Rita 17-1-19 Added for trading inserts of Followers
    public interface IFollowersTrading
    {
        Task<BizResponse> ProcessFollowersNewTransactionAsync(FollowersOrderRequestCls request);
    }

    //Rita 7-2-19 Site Token Conversion Service
    public interface ISiteTokenConversion
    {
        Task<SiteTokenConversionResponse> SiteTokenConversionAsync(SiteTokenConversionRequest Request, long UserID, string accesstoken);
        Task<SiteTokenCalculationResponse> SiteTokenCalculation(SiteTokenCalculationRequest Request, long UserID, string accesstoken);
        //Rita 16-4-19 added for margin trading
        Task<SiteTokenConversionResponse> SiteTokenConversionAsyncMargin(SiteTokenConversionRequest Request, long UserID, string accesstoken);
        Task<SiteTokenCalculationResponse> SiteTokenCalculationMargin(SiteTokenCalculationRequest Request, long UserID, string accesstoken);
    }
}
