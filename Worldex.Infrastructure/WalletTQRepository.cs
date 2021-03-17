using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace Worldex.Infrastructure
{
    public class WalletTQRepository : IWalletTQInsert
    {
        private readonly WorldexContext _dbContext;
        public WalletTQRepository(WorldexContext dbContext)
        {          
            _dbContext = dbContext;
        }

        public WalletTransactionQueue AddIntoWalletTransactionQueue(WalletTransactionQueue wtq, byte AddorUpdate)//1=add,2=update
        {
            try
            {
                if (AddorUpdate == 1)
                {
                    _dbContext.WalletTransactionQueues.Add(wtq);
                }
                else
                {
                    _dbContext.Entry(wtq).State = EntityState.Modified;
                }
                _dbContext.SaveChanges();
                return wtq;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }
        public ArbitrageWalletTransactionQueue AddIntoArbitrageWalletTransactionQueue(ArbitrageWalletTransactionQueue wtq, byte AddorUpdate)//1=add,2=update
        {
            try
            {
                if (AddorUpdate == 1)
                {
                    _dbContext.ArbitrageWalletTransactionQueue.Add(wtq);
                }
                else
                {
                    _dbContext.Entry(wtq).State = EntityState.Modified;
                }
                _dbContext.SaveChanges();
                return wtq;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }
    }
}
