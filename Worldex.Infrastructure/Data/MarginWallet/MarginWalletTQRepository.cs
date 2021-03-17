using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.MarginWallet;
using Microsoft.EntityFrameworkCore;
using System;

namespace Worldex.Infrastructure
{
    public class MarginWalletTQRepository : IMarginWalletTQInsert
    {
        private readonly WorldexContext _dbContext;
        public MarginWalletTQRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public MarginWalletTransactionQueue AddIntoWalletTransactionQueue(MarginWalletTransactionQueue wtq, byte AddorUpdate)//1=add,2=update
        {
            try
            {
                if (AddorUpdate == 1)
                {
                    _dbContext.MarginWalletTransactionQueue.Add(wtq);
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
