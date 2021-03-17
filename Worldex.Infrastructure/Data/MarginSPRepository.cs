using Worldex.Core.ApiModels;
using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.MarginWallet;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Wallet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Data
{
    public class MarginSPRepository : IMarginSPRepositories
    {
        private readonly WorldexContext _dbContext;

        public MarginSPRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public BizResponseClass Callsp_HoldWallet(MarginWalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enMarginWalletTrnType walletTrnType, ref long trnNo, enWalletDeductionType enWalletDeductionType)
        {
            try
            {
                BizResponseClass bizResponseClass = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@TrnNo",SqlDbType.BigInt, 10, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default,0),
                new SqlParameter("@ChannelType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,Convert.ToInt64(channelType)),
                new SqlParameter("@timeStamp",SqlDbType.VarChar, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, timestamp),
                new SqlParameter("@serviceType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt32(serviceType)),
                new SqlParameter("@Coin",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, coin),
                new SqlParameter("@WalletType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, WalletType) ,
                new SqlParameter("@Amount",SqlDbType.Decimal, 18, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, amount) ,
                new SqlParameter("@TrnRefNo",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,TrnRefNo) ,
                new SqlParameter("@DrWalletID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, walletID) ,
                new SqlParameter("@UserID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, UserID) ,
                new SqlParameter("@TrnType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, TrnType) ,
                new SqlParameter("@WalletTrnType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, walletTrnType) ,
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)  ,
                new SqlParameter("@WalletDeductionType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, enWalletDeductionType)
                };
                var res = _dbContext.Database.ExecuteSqlCommand("sp_MarginHoldWalletWithCharge @TrnNo output,@ChannelType ,@timeStamp  ,@serviceType ,@Coin ,@WalletType ,@Amount ,@TrnRefNo ,@DrWalletID ,@UserID  ,@TrnType ,@WalletTrnType ,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@WalletDeductionType", param1);
                //Task.Run(() => HelperForLog.WriteLogIntoFileAsync("sp_MarginHoldWallet", "MarginSPRepository", "timestamp:" + timestamp + " ,ReturnCode=" + bizResponseClass.ReturnCode + ",Message=" + bizResponseClass.ReturnMsg + ",ErrorCode=" + bizResponseClass.ErrorCode));
                //ntrivedi moved below 09-07-2019 for response logging
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
                    trnNo = Convert.ToInt64(@param1[0].Value);
                }
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("sp_MarginHoldWalletWithCharge", "MarginSPRepository", "timestamp:" + timestamp + " ,ReturnCode=" + bizResponseClass.ReturnCode + ",Message=" + bizResponseClass.ReturnMsg + ",ErrorCode=" + bizResponseClass.ErrorCode));
                return bizResponseClass;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass Callsp_CrDrWalletForHold(MarginPNL PNLObj, MarginCommonClassCrDr firstCurrObj, MarginCommonClassCrDr secondCurrObj, string timestamp, enServiceType serviceType, long firstCurrWalletType, long secondCurrWalletType, long channelType = (long)EnAllowedChannels.Web)
        {
            try
            {
                BizResponseClass bizResponseClass = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@timeStamp",SqlDbType.VarChar, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, timestamp),
                new SqlParameter("@serviceType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,Convert.ToInt32(serviceType)),
                new SqlParameter("@firstCurrCoin",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.Coin),
                new SqlParameter("@secondCurrCoin",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.Coin),
                new SqlParameter("@firstCurrWalletType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrWalletType) ,
                new SqlParameter("@secondCurrWalletType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,secondCurrWalletType) ,
                new SqlParameter("@firstCurrAmount",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, firstCurrObj.Amount) ,
                new SqlParameter("@secondCurrAmount",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, secondCurrObj.Amount) ,
                new SqlParameter("@firstCurrDrTQTrnno",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.debitObject.WTQTrnNo) ,
                new SqlParameter("@secondCurrDrTQTrnno",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.debitObject.WTQTrnNo) ,
                new SqlParameter("@firstCurrCrTrnRefNo",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,firstCurrObj.creditObject.TrnRefNo) ,
                new SqlParameter("@firstCurrDrTrnRefNo",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.debitObject.TrnRefNo) ,
                new SqlParameter("@secondCurrCrTrnRefNo",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.creditObject.TrnRefNo) ,
                new SqlParameter("@secondCurrDrTrnRefNo",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.debitObject.TrnRefNo) ,
                new SqlParameter("@firstCurrCrWalletID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.creditObject.WalletId) ,
                new SqlParameter("@firstCurrDrWalletID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.debitObject.WalletId) ,
                new SqlParameter("@secondCurrCrWalletID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.creditObject.WalletId) ,
                new SqlParameter("@secondCurrDrWalletID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.debitObject.WalletId) ,
                new SqlParameter("@firstCurrCrUserID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.creditObject.UserID) ,
                new SqlParameter("@firstCurrDrUserID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.debitObject.UserID) ,
                new SqlParameter("@secondCurrCrUserID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.creditObject.UserID) ,
                new SqlParameter("@secondCurrDrUserID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.debitObject.UserID) ,
                new SqlParameter("@firstCurrCrisFullSettled",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.creditObject.isFullSettled) ,
                new SqlParameter("@firstCurrDrisFullSettled",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.debitObject.isFullSettled) ,
                new SqlParameter("@secondCurrCrisFullSettled",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.creditObject.isFullSettled) ,
                new SqlParameter("@secondCurrDrisFullSettled",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.debitObject.isFullSettled) ,
                new SqlParameter("@firstCurrCrTrnType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.creditObject.trnType) ,
                new SqlParameter("@firstCurrDrTrnType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.debitObject.trnType) ,
                new SqlParameter("@secondCurrCrTrnType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.creditObject.trnType) ,
                new SqlParameter("@secondCurrDrTrnType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.debitObject.trnType) ,
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode) ,
                new SqlParameter("@ChannelType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,Convert.ToInt64(channelType)),
                new SqlParameter("@secondCurrCrIsMaker",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.creditObject.IsMaker) ,
                new SqlParameter("@secondCurrDrIsMaker",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, secondCurrObj.debitObject.IsMaker) ,
                new SqlParameter("@PairID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, PNLObj.PairID),  //profit loss position 02-04-2019
                new SqlParameter("@Qty",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, PNLObj.Qty) ,
                new SqlParameter("@BidPrice",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, PNLObj.BidPrice) ,
                new SqlParameter("@LandingPrice",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, PNLObj.LandingPrice) ,
                new SqlParameter("@BaseCurrency",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, PNLObj.BaseCurrency),
                new SqlParameter("@SecondCurrency",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, PNLObj.SecondCurrency),
                new SqlParameter("@firstCurrDrOrderType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.debitObject.OrderType ) ,
                new SqlParameter("@secondCurrDrOrderType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, firstCurrObj.debitObject.OrderType)
            };
                var res = _dbContext.Database.ExecuteSqlCommand("sp_MarginCrDrWalletForHoldWithCharge @timeStamp ,@serviceType ,@firstCurrCoin ,@secondCurrCoin ,@firstCurrWalletType ,@secondCurrWalletType ,@firstCurrAmount ,@secondCurrAmount ,@firstCurrDrTQTrnno  ,@secondCurrDrTQTrnno  ,@firstCurrCrTrnRefNo ,@firstCurrDrTrnRefNo ,@secondCurrCrTrnRefNo ,@secondCurrDrTrnRefNo ,@firstCurrCrWalletID ,@firstCurrDrWalletID ,@secondCurrCrWalletID ,@secondCurrDrWalletID ,@firstCurrCrUserID ,@firstCurrDrUserID, @secondCurrCrUserID ,@secondCurrDrUserID ,@firstCurrCrisFullSettled ,@firstCurrDrisFullSettled ,@secondCurrCrisFullSettled ,@secondCurrDrisFullSettled ,@firstCurrCrTrnType ,@firstCurrDrTrnType ,@secondCurrCrTrnType ,@secondCurrDrTrnType ,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@ChannelType,@secondCurrCrIsMaker,@secondCurrDrIsMaker,@PairID,@Qty,@BidPrice,@LandingPrice,@BaseCurrency,@SecondCurrency,@firstCurrDrOrderType,@secondCurrDrOrderType", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("Callsp_MarginCrDrWalletForHold", "MarginSPRepository", "timestamp:" + timestamp + " ,ReturnCode" + bizResponseClass.ReturnCode + ",Message=" + bizResponseClass.ReturnMsg + ",ErrorCode=" + bizResponseClass.ErrorCode));
                bizResponseClass.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[30].Value);
                bizResponseClass.ReturnMsg = @param1[31].Value.ToString();
                bizResponseClass.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[32].Value);

                return bizResponseClass;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass Callsp_ReleaseHoldWallet(MarginWalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enMarginWalletTrnType walletTrnType, ref long trnNo)
        {
            try
            {
                BizResponseClass bizResponseClass = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@TrnNo",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,0),
                new SqlParameter("@ChannelType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,Convert.ToInt64(channelType)),
                new SqlParameter("@timeStamp",SqlDbType.VarChar, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, timestamp),
                new SqlParameter("@serviceType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt32(serviceType)),
                new SqlParameter("@Coin",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, coin),
                new SqlParameter("@WalletType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, WalletType) ,
                new SqlParameter("@Amount",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, amount) ,
                new SqlParameter("@TrnRefNo",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,TrnRefNo) ,
                new SqlParameter("@DrWalletID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, walletID) ,
                new SqlParameter("@UserID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, UserID) ,
                new SqlParameter("@TrnType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, TrnType) ,
                new SqlParameter("@WalletTrnType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, walletTrnType) ,
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode) ,
            };
                var res = _dbContext.Database.ExecuteSqlCommand("sp_MarginReleaseHoldWalletWithCharge @TrnNo ,@ChannelType ,@timeStamp  ,@serviceType ,@Coin ,@WalletType ,@Amount ,@TrnRefNo ,@DrWalletID ,@UserID  ,@TrnType ,@WalletTrnType ,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("sp_MarginReleaseHoldWallet", "MarginSPRepository", "timestamp:" + timestamp + " ,ReturnCode" + bizResponseClass.ReturnCode + ",Message=" + bizResponseClass.ReturnMsg + ",ErrorCode=" + bizResponseClass.ErrorCode));

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
                trnNo = Convert.ToInt64(@param1[0].Value);


                return bizResponseClass;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass Callsp_HoldWallet_MarketTrade(MarginWalletMaster dWalletobj, string timestamp, enServiceType serviceType, decimal amount, string coin, EnAllowedChannels channelType, long WalletType, long TrnRefNo, long walletID, long UserID, enTrnType TrnType, enMarginWalletTrnType walletTrnType, enWalletDeductionType enWalletDeductionType)
        {
            try
            {
                BizResponseClass bizResponseClass = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@ChannelType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,Convert.ToInt64(channelType)),
                new SqlParameter("@timeStamp",SqlDbType.VarChar, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, timestamp),
                new SqlParameter("@serviceType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt32(serviceType)),
                new SqlParameter("@Coin",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, coin),
                new SqlParameter("@WalletType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, WalletType) ,
                new SqlParameter("@Amount",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, amount) ,
                new SqlParameter("@TQTrnNo",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,TrnRefNo) ,
                new SqlParameter("@DrWalletID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, walletID) ,
                new SqlParameter("@UserID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, UserID) ,
                new SqlParameter("@TrnType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, TrnType) ,
                new SqlParameter("@WalletTrnType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, walletTrnType) ,
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)
                };
                var res = _dbContext.Database.ExecuteSqlCommand("sp_MarginHoldWallet_MarketTrade @ChannelType ,@timeStamp  ,@serviceType ,@Coin ,@WalletType ,@Amount ,@TQTrnNo ,@DrWalletID ,@UserID  ,@TrnType ,@WalletTrnType ,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("sp_MarginHoldWallet_MarketTrade", "MarginSPRepository", "timestamp:" + timestamp + " ,ReturnCode=" + bizResponseClass.ReturnCode + ",Message=" + bizResponseClass.ReturnMsg + ",ErrorCode=" + bizResponseClass.ErrorCode));

                try
                {
                    _dbContext.Entry(dWalletobj).Reload();
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                }

                bizResponseClass.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[11].Value);
                bizResponseClass.ReturnMsg = @param1[12].Value.ToString();
                bizResponseClass.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[13].Value);

                return bizResponseClass;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass Callsp_IsValidWalletTransaction(long WalletID, long UserID, long WalletTypeID, long ChannelID, long WalletTrnType)
        {
            try
            {
                BizResponseClass bizResponseClass = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@WalletID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,WalletID),
                new SqlParameter("@UserID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,UserID),
                new SqlParameter("@WalletTypeID",SqlDbType.BigInt, 10, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, WalletTypeID),
                new SqlParameter("@ChannelID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, ChannelID),
                new SqlParameter("@WalletTrnType",SqlDbType.BigInt, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, WalletTrnType),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)
                };
                var res = _dbContext.Database.ExecuteSqlCommand("sp_MarginIsValidWalletTransaction @WalletID,@UserID ,@WalletTypeID,@ChannelID ,@WalletTrnType,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("sp_MarginIsValidWalletTransaction", "MarginSPRepository", "timestamp:" + Helpers.UTC_To_IST() + " ,ReturnCode" + bizResponseClass.ReturnCode + ",Message=" + bizResponseClass.ReturnMsg + ",ErrorCode=" + bizResponseClass.ErrorCode));


                bizResponseClass.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[5].Value);
                bizResponseClass.ReturnMsg = @param1[6].Value.ToString();
                bizResponseClass.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[7].Value);


                return bizResponseClass;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public MarginPreConfirmationRes CallSP_MarginFundTransferCalculation(long WalletTypeId, decimal Amount, long UserID, long WalletID, short LeverageChargeDeductionType, decimal Levarage)
        {
            try
            {
                MarginPreConfirmationRes Res = new MarginPreConfirmationRes();
                BizResponseClass bizResponseClass = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@WalletTypeId",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,WalletTypeId),
                new SqlParameter("@Amount",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default,Amount),
                new SqlParameter("@UserID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,UserID),
                new SqlParameter("@WalletID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,WalletID),
                new SqlParameter("@ChannelID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,21),
                new SqlParameter("@TrnType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,Convert.ToInt64(enMarginWalletTrnType.MarginWalletTransfer)),
                new SqlParameter("@LeveragePer",SqlDbType.Decimal, 28, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default,Res.LeveragePer),
                new SqlParameter("@LeverageAmount",SqlDbType.Decimal, 28, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default,Res.LeverageAmount),
                new SqlParameter("@SafetyMarginAmount",SqlDbType.Decimal, 28, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default,Res.SafetyMarginAmount),
                new SqlParameter("@IsAutoApproveEnabled",SqlDbType.SmallInt, 5, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default,Res.IsAutoApproveEnabled),
                new SqlParameter("@FinalCreditAmount",SqlDbType.Decimal, 28, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default,Res.FinalCreditAmount),
                new SqlParameter("@ChargePer",SqlDbType.Decimal, 28, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default,Res.ChargePer),
                new SqlParameter("@ChargeAmount",SqlDbType.Decimal, 28, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default,Res.ChargeAmount),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ErrorCode),
                new SqlParameter("@ToWalletId",SqlDbType.BigInt, 10, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default,Res.ToWalletId),
                new SqlParameter("@LeverageId",SqlDbType.BigInt, 10, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default,Res.LeverageId),
                new SqlParameter("@Leverage",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default,Levarage)

                };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_MarginFundTransferCalculation @WalletTypeId, @Amount,@UserID,@WalletID,@ChannelID,@TrnType,@LeveragePer OUTPUT,@LeverageAmount OUTPUT,@SafetyMarginAmount OUTPUT,@IsAutoApproveEnabled OUTPUT,@FinalCreditAmount OUTPUT,@ChargePer OUTPUT,@ChargeAmount OUTPUT,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@ToWalletId OUTPUT ,@LeverageId OUTPUT,@Leverage", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("CallSP_MarginFundTransferCalculation", "WalletService", " ReturnCode:" + @param1[13].Value.ToString() + " ReturnMsg:" + @param1[14].Value.ToString() + " ErrorCode :" + @param1[15].Value.ToString()));

                Res.LeveragePer = Convert.ToDecimal(@param1[6].Value);
                Res.LeverageAmount = Convert.ToDecimal(@param1[7].Value);
                Res.SafetyMarginAmount = Convert.ToDecimal(@param1[8].Value);
                Res.IsAutoApproveEnabled = Convert.ToInt16(@param1[9].Value);
                Res.FinalCreditAmount = Convert.ToDecimal(@param1[10].Value);
                Res.ChargePer = Convert.ToDecimal(@param1[11].Value);
                Res.ChargeAmount = Convert.ToDecimal(@param1[12].Value);
                Res.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[13].Value);
                Res.ReturnMsg = @param1[14].Value.ToString();
                Res.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[15].Value);
                Res.ToWalletId = Convert.ToInt64(@param1[16].Value);
                Res.LeverageId = Convert.ToInt64(@param1[17].Value);
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass CallSP_MarginProcess(long WalletTypeId, decimal Amount, long UserID, long WalletID, string TimeStamp, short LeverageChargeDeductionType, ref long RequestId, decimal Leverage)
        {
            try
            {
                BizResponseClass Res = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@WalletTypeId",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,WalletTypeId),
                new SqlParameter("@Amount",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default,Amount),
                new SqlParameter("@UserID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,UserID),
                new SqlParameter("@WalletID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,WalletID),
                new SqlParameter("@TimeStamp",SqlDbType.VarChar, 20, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,TimeStamp),
                new SqlParameter("@TrnType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,Convert.ToInt64(enMarginWalletTrnType.MarginWalletTransfer)),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ErrorCode),
                new SqlParameter("@RequestId",SqlDbType.BigInt, 10, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default,RequestId),
                new SqlParameter("@Leverage",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default,Leverage)
                };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_MarginProcess @WalletTypeId, @Amount,@UserID,@WalletID,@TimeStamp,@TrnType,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@RequestId OUTPUT,@Leverage", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("SP_MarginProcess", "WalletService", " ReturnCode:" + @param1[6].Value.ToString() + " ReturnMsg:" + @param1[7].Value.ToString() + " ErrorCode :" + @param1[8].Value.ToString()));

                Res.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[6].Value);
                Res.ReturnMsg = @param1[7].Value.ToString();
                Res.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[8].Value);
                RequestId = Convert.ToInt64(@param1[9].Value);
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass CreateMarginWallet(long WalletTypeId, long UserId)
        {
            try
            {
                BizResponseClass Res = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@UserID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,UserId),
                new SqlParameter("@WalletTypeId",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,WalletTypeId),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ErrorCode)
                };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_CreateMarginWallet @UserID,@WalletTypeId,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("SP_CreateMarginWallet", "WalletService", " ReturnCode:" + @param1[2].Value.ToString() + " ReturnMsg:" + @param1[3].Value.ToString() + " ErrorCode :" + @param1[4].Value.ToString()));

                Res.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[2].Value);
                Res.ReturnMsg = @param1[3].Value.ToString();
                Res.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[4].Value);
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass CreateAllMarginWallet(long UserId)
        {
            try
            {
                BizResponseClass Res = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@UserId",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,UserId),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ErrorCode)
                };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_CreateMarginWalletForAllWalletType @UserId,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("SP_CreateMarginWalletForAllWalletType", "WalletService", " ReturnCode:" + @param1[1].Value.ToString() + " ReturnMsg:" + @param1[2].Value.ToString() + " ErrorCode :" + @param1[3].Value.ToString()));

                Res.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[1].Value);
                Res.ReturnMsg = @param1[2].Value.ToString();
                Res.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[3].Value);
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public GetMemberBalRes Callsp_MarginGetMemberBalance(long walletID, long UserID, long WalletMasterID, short BalanceType, decimal Amount, int WalletUsageType)
        {
            try
            {
                GetMemberBalRes Res = new GetMemberBalRes();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@WalletID",SqlDbType.BigInt, 10, ParameterDirection.InputOutput, false, 0, 0, String.Empty, DataRowVersion.Default,walletID),
                new SqlParameter("@UserID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,UserID),
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
                var res = _dbContext.Database.ExecuteSqlCommand("sp_MarginGetMemberBalance @WalletID OUTPUT,@UserID,@WalletMasterID,@WalletBalance OUTPUT,@WalletOutboundBalance OUTPUT,@WalletInboundBalance OUTPUT,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@BalanceType,@Amount,@WalletUsageType", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("sp_MarginGetMemberBalance", "MarginWalletService", " ReturnCode:" + @param1[6].Value.ToString() + " ReturnMsg:" + @param1[7].Value.ToString() + " ErrorCode :" + @param1[8].Value.ToString()));

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

        public BizResponseClass CallSP_AdminMarginChargeRequestApproval(short IsApproved, long ReuestId, string TimeStamp, string Remarks)
        {
            try
            {
                BizResponseClass Res = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@IsApproved",SqlDbType.SmallInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,IsApproved),
                new SqlParameter("@ReuestId",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,ReuestId),
                new SqlParameter("@TimeStamp",SqlDbType.VarChar, 20, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,TimeStamp),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ErrorCode),
                new SqlParameter("@Remarks",SqlDbType.VarChar,50, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,(Remarks==null?"Remarks":Remarks)),
                };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_AdminMarginChargeRequestApproval @IsApproved,@ReuestId,@TimeStamp,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@Remarks", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("SP_AdminMarginChargeRequestApproval", "WalletService", " ReturnCode:" + @param1[3].Value.ToString() + " ReturnMsg:" + @param1[4].Value.ToString() + " ErrorCode :" + @param1[5].Value.ToString()));

                Res.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[3].Value);
                Res.ReturnMsg = @param1[4].Value.ToString();
                Res.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[5].Value);
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass CallSP_CreateMarginWalletForAllWalletType(long UserId)
        {
            try
            {
                BizResponseClass Res = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@UserId",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,UserId),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ErrorCode)
                };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_CreateMarginWalletForAllWalletType @UserId,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("SP_CreateMarginWalletForAllWalletType", "WalletService", " ReturnCode:" + @param1[1].Value.ToString() + " ReturnMsg:" + @param1[2].Value.ToString() + " ErrorCode :" + @param1[3].Value.ToString()));

                Res.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[1].Value);
                Res.ReturnMsg = @param1[2].Value.ToString();
                Res.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[3].Value);
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass Callsp_MarginChargeWalletCallBGTaskNew(ref long BatchNo)//ntrivedi 09-04-2019
        {
            try
            {
                BizResponseClass Res = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ErrorCode),
                new SqlParameter("@BatchNo",SqlDbType.BigInt, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, 0),

                };
                var res = _dbContext.Database.ExecuteSqlCommand("sp_MarginEODChargeCollection @ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@BatchNo OUTPUT", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("sp_MarginEODChargeCollection", "WalletService", " ReturnCode:" + @param1[0].Value.ToString() + " ReturnMsg:" + @param1[1].Value.ToString() + " ErrorCode :" + @param1[2].Value.ToString() + " BatchNo :" + @param1[3].Value.ToString()));

                Res.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[0].Value);
                Res.ReturnMsg = @param1[1].Value.ToString();
                Res.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[2].Value);
                BatchNo = Convert.ToInt64(@param1[3].Value);
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public BizResponseClass Callsp_MarginProcessLeverageAccountEOD(long loanID, long BatchNo, short ActionType)//ntrivedi 09-04-2019
        {
            try
            {
                BizResponseClass Res = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@LoanID",SqlDbType.BigInt, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, loanID),
                new SqlParameter("@BatchNo",SqlDbType.BigInt, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, BatchNo),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ErrorCode),
                new SqlParameter("@ActionType",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, ActionType)

                };
                var res = _dbContext.Database.ExecuteSqlCommand("sp_MarginProcessLeverageAccountEOD @LoanID,@BatchNo,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@ActionType", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("sp_MarginProcessLeverageAccountEOD", "WalletService", " ReturnCode:" + @param1[2].Value.ToString() + " ReturnMsg:" + @param1[3].Value.ToString() + " ErrorCode :" + @param1[4].Value.ToString()));

                Res.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[2].Value);
                Res.ReturnMsg = @param1[3].Value.ToString();
                Res.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[4].Value);
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public MarginWithdrawPreConfirmResponse CallSP_MarginWithdrawCalc(long UserId, string currency)
        {
            try
            {
                MarginWithdrawPreConfirmResponse Res = new MarginWithdrawPreConfirmResponse();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@Currency",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,currency),
                new SqlParameter("@UserId",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,UserId),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ErrorCode),
                new SqlParameter("@TotalAmount",SqlDbType.Decimal, 10, ParameterDirection.Output, false,28,18,String.Empty, DataRowVersion.Default,0),
                new SqlParameter("@ProfitWalletBal",SqlDbType.Decimal, 10, ParameterDirection.Output, false,28,18,String.Empty, DataRowVersion.Default,0),
                new SqlParameter("@SafetyWalletBal",SqlDbType.Decimal, 10, ParameterDirection.Output, false,28,18,String.Empty, DataRowVersion.Default,0),
                new SqlParameter("@LoanId",SqlDbType.BigInt, 10, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default,0),
                new SqlParameter("@ChargeAmount",SqlDbType.Decimal, 10, ParameterDirection.Output, false,28,18,String.Empty, DataRowVersion.Default,0),
                };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_MarginWithdrawCalc @Currency,@UserId,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@TotalAmount OUTPUT,@ProfitWalletBal OUTPUT,@SafetyWalletBal OUTPUT,@LoanId OUTPUT ,@ChargeAmount OUTPUT", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("SP_MarginWithdrawCalc", "MarginWalletService", " ReturnCode:" + @param1[2].Value.ToString() + " ReturnMsg:" + @param1[3].Value.ToString() + " ErrorCode :" + @param1[4].Value.ToString()));

                Res.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[2].Value);
                Res.ReturnMsg = @param1[3].Value.ToString();
                Res.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[4].Value);
                Res.TotalAmount = Convert.ToDecimal(@param1[5].Value);
                Res.ProfitAmount = Convert.ToDecimal(@param1[6].Value);
                Res.SafetyAmount = Convert.ToDecimal(@param1[7].Value);
                Res.LoanID = Convert.ToInt64(@param1[8].Value);
                Res.ChargeAmount = Convert.ToDecimal(@param1[9].Value);
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public MarginWithdrawPreConfirmResponse CallSP_MarginWithdraw(long UserId, string currency)
        {
            try
            {
                MarginWithdrawPreConfirmResponse ResMrgn = new MarginWithdrawPreConfirmResponse();

                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@Currency",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,currency),
                new SqlParameter("@UserId",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,UserId),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, ResMrgn.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, ResMrgn.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, ResMrgn.ErrorCode),
                new SqlParameter("@TotalAmount",SqlDbType.Decimal, 10, ParameterDirection.Output, false,28,18,String.Empty, DataRowVersion.Default,0),
                new SqlParameter("@ProfitWalletAmount",SqlDbType.Decimal, 10, ParameterDirection.Output, false,28,18,String.Empty, DataRowVersion.Default,0),
                new SqlParameter("@SafetyWalletAmount",SqlDbType.Decimal, 10, ParameterDirection.Output, false,28,18,String.Empty, DataRowVersion.Default,0)
                };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_MarginWithdraw @Currency,@UserId,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@TotalAmount OUTPUT,@ProfitWalletAmount OUTPUT,@SafetyWalletAmount OUTPUT", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("SP_MarginWithdraw", "MarginWalletService", " ReturnCode:" + @param1[2].Value.ToString() + " ReturnMsg:" + @param1[3].Value.ToString() + " ErrorCode :" + @param1[4].Value.ToString()));

                ResMrgn.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[2].Value);
                ResMrgn.ReturnMsg = @param1[3].Value.ToString();
                ResMrgn.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[4].Value);
                ResMrgn.TotalAmount = Convert.ToDecimal(@param1[5].Value);
                ResMrgn.ProfitAmount = Convert.ToDecimal(@param1[6].Value);
                ResMrgn.SafetyAmount = Convert.ToDecimal(@param1[7].Value);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("SP_MarginWithdraw", "MarginWalletService", "Rewrite TotalAmount:" + Convert.ToDecimal(@param1[5].Value).ToString()));

                return ResMrgn;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass CallSP_UpgradeLoan(long UserID, long loanID, decimal leverageX)//ntrivedi 15-05-2019
        {
            try
            {
                BizResponseClass Res = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@LoanID",SqlDbType.BigInt, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, loanID),
                new SqlParameter("@LeverageX",SqlDbType.Decimal, 8, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, leverageX),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ErrorCode),
                new SqlParameter("@UserID",SqlDbType.BigInt, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, UserID)
                };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_UpgradeLoan @LoanID,@LeverageX,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@UserID", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("SP_UpgradeLoan", "WalletService", " ReturnCode:" + @param1[2].Value.ToString() + " ReturnMsg:" + @param1[3].Value.ToString() + " ErrorCode :" + @param1[4].Value.ToString()));

                Res.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[2].Value);
                Res.ReturnMsg = @param1[3].Value.ToString();
                Res.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[4].Value);
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<WalletLedgerResponse> CallGetMarginWalletLedger(DateTime FromDate, DateTime ToDate, long WalletId, int page, int? PageSize, ref int TotalCount)
        {
            try
            {
                List<WalletLedgerResponse> res = new List<WalletLedgerResponse>();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@WalletId",SqlDbType.BigInt, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, WalletId),
                new SqlParameter("@FromDate",SqlDbType.DateTime, 10, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, FromDate),
                new SqlParameter("@ToDate",SqlDbType.DateTime, 10, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, ToDate),
                new SqlParameter("@PageNo",SqlDbType.Int, 10, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, page),
                new SqlParameter("@PageSize",SqlDbType.Int, 10, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, PageSize),
                new SqlParameter("@RecordCount",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default,TotalCount),
                };
                res = _dbContext.WalletLedgerResponse.FromSql("SP_GetMarginWalletLedger   @WalletId,@FromDate,@ToDate,@PageNo,@PageSize,@RecordCount OUTPUT", param1).ToListAsync().GetAwaiter().GetResult();

                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("CallGetWalletLedger", "WalletService", Convert.ToInt32(@param1[5].Value) + "", ""));
                TotalCount = Convert.ToInt32(@param1[5].Value);

                res.ForEach(i =>
                { i.TrnDate = Convert.ToDateTime(i.TrnDate).ToString("dd-MM-yyyy h:mm:ss tt"); });

                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass CallSP_AdminMarginChargeRequestApprovalv2(short IsApproved, string ReuestId, string TimeStamp, string Remarks)
        {
            try
            {
                BizResponseClass Res = new BizResponseClass();
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@IsApproved",SqlDbType.SmallInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,IsApproved),
                new SqlParameter("@ReuestId",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,ReuestId),
                new SqlParameter("@TimeStamp",SqlDbType.VarChar, 20, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,TimeStamp),
                new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnCode) ,
                new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ReturnMsg) ,
                new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Res.ErrorCode),
                new SqlParameter("@Remarks",SqlDbType.VarChar,50, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,(Remarks==null?"Remarks":Remarks)),
                };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_AdminMarginChargeRequestApprovalGUID @IsApproved,@ReuestId,@TimeStamp,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT,@Remarks", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("SP_AdminMarginChargeRequestApprovalV2", "WalletService", " ReturnCode:" + @param1[3].Value.ToString() + " ReturnMsg:" + @param1[4].Value.ToString() + " ErrorCode :" + @param1[5].Value.ToString()));

                Res.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[3].Value);
                Res.ReturnMsg = @param1[4].Value.ToString();
                Res.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[5].Value);
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

