using Worldex.Core.Entities;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.Data.Transaction
{
    public class CancelOrderRepository : ICancelOrderRepository
    {
        private readonly WorldexContext _dbContext;

        public CancelOrderRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool UpdateDataObjectWithBeginTransaction(TradeCancelQueue tradeCancelQueue, TradeTransactionQueue TradeTranQueueObj, PoolOrder PoolOrderObj,TradeBuyRequest NewBuyRequestObj)
        {
            try
            {
                _dbContext.Database.BeginTransaction();
                _dbContext.Set<TradeCancelQueue>().Add(tradeCancelQueue);
                _dbContext.Set<PoolOrder>().Add(PoolOrderObj);
                //_dbContext.Entry(PoolOrderObj).State = EntityState.Modified;
                _dbContext.Entry(NewBuyRequestObj).State = EntityState.Modified;
                _dbContext.Entry(TradeTranQueueObj).State = EntityState.Modified;
                _dbContext.SaveChanges();
                _dbContext.Database.CommitTransaction();
                return true;
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateDataObjectWithBeginTransaction", "ICancelOrderRepository", ex);
                return false;
            }
        }
        public bool UpdateDataObjectWithBeginTransactionV1(TradeCancelQueue tradeCancelQueue,TransactionQueue TransactionQueueObj,TradeTransactionQueue TradeTransactionQueueObj,TradeBuyerListV1 BuyerListObj,TradeSellerListV1 SellerListObj, SettledTradeTransactionQueue SettledTradeTQObj,short ISPartialSettled)
        {
            try
            {
                _dbContext.Database.BeginTransaction();
                _dbContext.Set<TradeCancelQueue>().Add(tradeCancelQueue);
                _dbContext.Entry(TransactionQueueObj).State = EntityState.Modified;
                _dbContext.Entry(TradeTransactionQueueObj).State = EntityState.Modified;
                _dbContext.Entry(tradeCancelQueue).State = EntityState.Added;
                if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Buy_Trade) && BuyerListObj!=null)//Rita 24-4-19 may be no entry found
                    _dbContext.Entry(BuyerListObj).State = EntityState.Modified;
                else if(TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Sell_Trade) && SellerListObj != null)
                    _dbContext.Entry(SellerListObj).State = EntityState.Modified;

                //if (ISPartialSettled == 1)//IF success
                //    _dbContext.Set<SettledTradeTransactionQueue>().Add(SettledTradeTQObj);

                _dbContext.SaveChanges();
                _dbContext.Database.CommitTransaction();

                try//Rita 19-7-19 bcz of Violation of PRIMARY KEY constraint 'PK_SettledTradeTransactionQueue'
                {
                    if (ISPartialSettled == 1)//IF success
                    {
                        _dbContext.Set<SettledTradeTransactionQueue>().Add(SettledTradeTQObj);
                        _dbContext.SaveChanges();
                    }                        
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog("UpdateDataObjectWithBeginTransactionV1 SettledTradeTQObj Entry", "ICancelOrderRepository", ex);
                }
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateDataObjectWithBeginTransactionV1", "ICancelOrderRepository", ex);
                return false;
            }
        }
    }

    //========================================Rita 21-2-19 for margin trading====================================
    public class CancelOrderRepositoryMargin : ICancelOrderRepositoryMargin
    {
        private readonly WorldexContext _dbContext;

        public CancelOrderRepositoryMargin(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool UpdateDataObjectWithBeginTransactionV1(TradeCancelQueueMargin tradeCancelQueue, TransactionQueueMargin TransactionQueueObj, TradeTransactionQueueMargin TradeTransactionQueueObj, TradeBuyerListMarginV1 BuyerListObj, TradeSellerListMarginV1 SellerListObj, SettledTradeTransactionQueueMargin SettledTradeTQObj, short ISPartialSettled)
        {
            try
            {
                _dbContext.Database.BeginTransaction();
                _dbContext.Set<TradeCancelQueueMargin>().Add(tradeCancelQueue);
                _dbContext.Entry(TransactionQueueObj).State = EntityState.Modified;
                _dbContext.Entry(TradeTransactionQueueObj).State = EntityState.Modified;
                _dbContext.Entry(tradeCancelQueue).State = EntityState.Added;
                if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Buy_Trade) && BuyerListObj != null)//Rita 24-4-19 may be no entry found
                    _dbContext.Entry(BuyerListObj).State = EntityState.Modified;
                else if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Sell_Trade) && SellerListObj != null)
                    _dbContext.Entry(SellerListObj).State = EntityState.Modified;

                if (ISPartialSettled == 1)//IF success
                    _dbContext.Set<SettledTradeTransactionQueueMargin>().Add(SettledTradeTQObj);

                _dbContext.SaveChanges();
                _dbContext.Database.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateDataObjectWithBeginTransactionV1", "CancelOrderRepositoryMargin", ex);
                return false;
            }
        }
    }

    public class CancelOrderRepositoryArbitrage : ICancelOrderRepositoryArbitrage
    {
        private readonly WorldexContext _dbContext;

        public CancelOrderRepositoryArbitrage(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool UpdateDataObjectWithBeginTransactionV1(TradeCancelQueueArbitrage tradeCancelQueue, TransactionQueueArbitrage TransactionQueueObj, TradeTransactionQueueArbitrage TradeTransactionQueueObj, TradeBuyerListArbitrageV1 BuyerListObj, TradeSellerListArbitrageV1 SellerListObj, SettledTradeTransactionQueueArbitrage SettledTradeTQObj, short ISPartialSettled)
        {
            try
            {
                _dbContext.Database.BeginTransaction();
                _dbContext.Set<TradeCancelQueueArbitrage>().Add(tradeCancelQueue);
                _dbContext.Entry(TransactionQueueObj).State = EntityState.Modified;
                _dbContext.Entry(TradeTransactionQueueObj).State = EntityState.Modified;
                _dbContext.Entry(tradeCancelQueue).State = EntityState.Added;
                if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Buy_Trade) && BuyerListObj != null)//Rita 24-4-19 may be no entry found
                    _dbContext.Entry(BuyerListObj).State = EntityState.Modified;
                else if (TradeTransactionQueueObj.TrnType == Convert.ToInt16(enTrnType.Sell_Trade) && SellerListObj != null)
                    _dbContext.Entry(SellerListObj).State = EntityState.Modified;

                if (ISPartialSettled == 1)//IF success
                    _dbContext.Set<SettledTradeTransactionQueueArbitrage>().Add(SettledTradeTQObj);

                _dbContext.SaveChanges();
                _dbContext.Database.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateDataObjectWithBeginTransactionV1", "ICancelOrderRepository", ex);
                return false;
            }
        }
    }
}
