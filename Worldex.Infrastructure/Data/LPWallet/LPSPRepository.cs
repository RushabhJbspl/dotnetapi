using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.MarginWallet;
using Worldex.Core.ViewModels.Wallet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Data
{
    public class LPSPRepository : ILPSPRepositories
    {
        private readonly WorldexContext _dbContext;

        public LPSPRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public GetMemberBalRes Callsp_LPGetMemberBalance(long walletID, long SerProID, long WalletMasterID, short BalanceType, decimal Amount, int WalletUsageType)
        {
            try
            {
                GetMemberBalRes Res = new GetMemberBalRes();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@WalletID",SqlDbType.BigInt, 10, ParameterDirection.InputOutput, false, 0, 0, String.Empty, DataRowVersion.Default,walletID),
                new SqlParameter("@SerProID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,SerProID),
                new SqlParameter("@WalletMasterID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,WalletMasterID),
                new SqlParameter("@WalletBalance",SqlDbType.Decimal, 10, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default,Res.WalletBalance),
                new SqlParameter("@WalletOutboundBalance",SqlDbType.Decimal, 10, ParameterDirection.Output, false,28,18,String.Empty, DataRowVersion.Default,Res.WalletOutboundBalance),
                new SqlParameter("@WalletInboundBalance",SqlDbType.Decimal, 10, ParameterDirection.Output, false,28,18,String.Empty, DataRowVersion.Default,Res.WalletInboundBalance),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ErrorCode),
                new SqlParameter("@BalanceType",SqlDbType.SmallInt, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, BalanceType),
                new SqlParameter("@Amount",SqlDbType.Decimal, 12, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, Amount),
                new SqlParameter("@WalletUsageType",SqlDbType.SmallInt, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, WalletUsageType)
                };
                var res = _dbContext.Database.ExecuteSqlCommand("sp_LPGetMemberBalance @WalletID OUTPUT,@SerProID,@WalletMasterID,@WalletBalance OUTPUT,@WalletOutboundBalance OUTPUT,@WalletInboundBalance OUTPUT,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@BalanceType,@Amount,@WalletUsageType", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("sp_LPGetMemberBalance", "LPSPRepository", " ReturnCode:" + @param1[6].Value.ToString() + " ReturnMsg:" + @param1[7].Value.ToString() + " ErrorCode :" + @param1[8].Value.ToString()));

                Res.WalletID = Convert.ToInt64(@param1[0].Value);
                Res.WalletBalance = Convert.ToDecimal(@param1[3].Value);
                Res.WalletOutboundBalance = Convert.ToDecimal(@param1[4].Value);
                Res.WalletInboundBalance = Convert.ToDecimal(@param1[5].Value);
                Res.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[6].Value);
                Res.ReturnMsg = @param1[7].Value.ToString();
                Res.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[8].Value);
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass Callsp_HoldWallet(LPHoldDr lPHoldDr,LPWalletMaster dWalletobj)
        {
            try
            {
                BizResponseClass bizResponseClass = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@TrnNo",SqlDbType.BigInt, 10, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default,0),
                new SqlParameter("@ChannelType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,EnAllowedChannels.Web),
                new SqlParameter("@timeStamp",SqlDbType.VarChar, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, lPHoldDr.Timestamp),
                new SqlParameter("@serviceType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, 0),
                new SqlParameter("@Coin",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, lPHoldDr.CoinName),
                new SqlParameter("@WalletType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, 0) ,
                new SqlParameter("@Amount",SqlDbType.Decimal, 18, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, lPHoldDr.Amount) ,
                new SqlParameter("@TrnRefNo",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,lPHoldDr.TrnRefNo) ,
                new SqlParameter("@DrWalletID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, lPHoldDr.WalletID) ,
                new SqlParameter("@SerProID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, lPHoldDr.SerProID) ,
                new SqlParameter("@TrnType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, 1) ,
                new SqlParameter("@WalletTrnType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, lPHoldDr.trnType) ,
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)  ,
                new SqlParameter("@WalletDeductionType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, lPHoldDr.enWalletDeductionType)
                };
                var res = _dbContext.Database.ExecuteSqlCommand("sp_LPHoldWalletWithCharge @TrnNo output,@ChannelType ,@timeStamp  ,@serviceType ,@Coin ,@WalletType ,@Amount ,@TrnRefNo ,@DrWalletID ,@SerProID  ,@TrnType ,@WalletTrnType ,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@WalletDeductionType", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("sp_LPHoldWalletWithCharge", "LPSPRepository", "timestamp:" + lPHoldDr.Timestamp + " ,ReturnCode=" + bizResponseClass.ReturnCode + ",Message=" + bizResponseClass.ReturnMsg + ",ErrorCode=" + bizResponseClass.ErrorCode));

                try
                {
                    _dbContext.Entry(dWalletobj).Reload();
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                }
                bizResponseClass.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[12].Value);
                bizResponseClass.ReturnMsg = @param1[13].Value.ToString();
                bizResponseClass.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[14].Value);
                if (bizResponseClass.ReturnCode == enResponseCode.Success)
                {
                    lPHoldDr.TrnNo = Convert.ToInt64(@param1[0].Value);
                }
                return bizResponseClass;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass Callsp_LPCrDrWalletForHold(ArbitrageCommonClassCrDr firstCurrObj, string timestamp, enServiceType serviceType, long firstCurrWalletType, long secondCurrWalletType, long channelType = (long)EnAllowedChannels.Web)
        {
            try
            {
                BizResponseClass bizResponseClass = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@SerProID",SqlDbType.BigInt, 10, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.SerProID),
                new SqlParameter("@timeStamp",SqlDbType.VarChar, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, timestamp),
                new SqlParameter("@serviceType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,Convert.ToInt32(serviceType)),
                new SqlParameter("@firstCurrCoin",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.Coin),
                new SqlParameter("@HoldCurrCoin",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.HoldCoin),
                new SqlParameter("@firstCurrWalletType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrWalletType) ,
                new SqlParameter("@holdCurrWalletType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,secondCurrWalletType) ,
                new SqlParameter("@firstCurrAmount",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, firstCurrObj.Amount) ,
                new SqlParameter("@HoldCurrAmount",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, firstCurrObj.HoldAmount) ,
                new SqlParameter("@firstCurrCrTrnRefNo",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,firstCurrObj.TrnRefNo) ,
                new SqlParameter("@firstCurrCrUserID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,firstCurrObj.UserID) ,
                new SqlParameter("@firstCurrCrisFullSettled",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.isFullSettled) ,
                new SqlParameter("@firstCurrCrTrnType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.trnType) ,
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode) ,
                new SqlParameter("@ChannelType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,Convert.ToInt64(channelType)),
                new SqlParameter("IsMaker",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.IsMaker)

            };
                var res = _dbContext.Database.ExecuteSqlCommand("sp_LPCrDrWalletForHoldWithCharge @SerProID,@timeStamp ,@serviceType ,@firstCurrCoin ,@HoldCurrCoin ,@firstCurrWalletType ,@holdCurrWalletType ,@firstCurrAmount ,@HoldCurrAmount  ,@firstCurrCrTrnRefNo ,@firstCurrCrUserID ,@firstCurrCrisFullSettled ,@firstCurrCrTrnType ,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@ChannelType,@IsMaker", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("sp_LPCrDrWalletForHoldWithCharge", "LPSPREpositiory", "timestamp:" + timestamp + " ,ReturnCode" + bizResponseClass.ReturnCode + ",Message=" + bizResponseClass.ReturnMsg + ",ErrorCode=" + bizResponseClass.ErrorCode));
                bizResponseClass.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[13].Value);
                bizResponseClass.ReturnMsg = @param1[14].Value.ToString();
                bizResponseClass.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[15].Value);

                return bizResponseClass;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass callsp_LPWalletRecon(ReconRequest recon, long UserId)
        {
            try
            {
                BizResponseClass Res = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@RequestId",SqlDbType.VarChar, 50, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,recon.RequestId),
                new SqlParameter("@UserId",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,UserId),
                new SqlParameter("@Amount",SqlDbType.Decimal, 10, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default,recon.Amount),
                new SqlParameter("@Remarks",SqlDbType.VarChar, 200, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,recon.Remarks),
                new SqlParameter("@IsAccept",SqlDbType.SmallInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,recon.IsAccept),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ErrorCode)
                };
                var res = _dbContext.Database.ExecuteSqlCommand("sp_LPWalletRecon @RequestId,@UserId,@Amount,@Remarks,@IsAccept,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("sp_LPWalletRecon", "ArbitrageSPRepository", " ReturnCode:" + @param1[5].Value.ToString() + " ReturnMsg:" + @param1[6].Value.ToString() + " ErrorCode :" + @param1[7].Value.ToString()));

                Res.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[5].Value);
                Res.ReturnMsg = @param1[6].Value.ToString();
                Res.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[7].Value);
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
    }
}
