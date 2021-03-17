using Worldex.Core.Entities;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Helpers;
using System;

namespace Worldex.Infrastructure.Data.LPWallet
{
    public class LPWalletRepository : Core.Interfaces.MarginWallet.ILPWalletRepository
    {
        private readonly WorldexContext _dbContext;

        public LPWalletRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ReloadEntitySingle(WalletMaster wm1, LPWalletMaster wm2, WalletMaster wm3)
        {
            try
            {
                try
                {
                    _dbContext.Entry(wm1).Reload();
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + "w1", this.GetType().Name, ex);
                }
                try
                {
                    _dbContext.Entry(wm2).Reload();
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + "w2", this.GetType().Name, ex);
                }
                try
                {
                    _dbContext.Entry(wm3).Reload();
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + "w3", this.GetType().Name, ex);
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }
    }
}
