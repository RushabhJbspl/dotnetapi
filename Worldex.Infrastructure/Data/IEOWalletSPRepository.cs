using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.IEOWallet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Data
{
    public class IEOWalletSPRepository : IIEOWalletSPRepositories
    {
        private readonly WorldexContext _dbContext;

        public IEOWalletSPRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public PreConfirmResponse CallSP_ConfirmTrn(string PaidCurrencyWallet, decimal PaidQauntity, string PaidCurrency, string DeliveryCurrency, string RoundID, string Remarks, long UserID)
        {
            PreConfirmResponse bizResponseClass = new PreConfirmResponse();
            try
            {
                 SqlParameter[] param1 = new SqlParameter[]{
                        new SqlParameter("@UserId",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,UserID),
                        new SqlParameter("@PaidAccWalletId",SqlDbType.VarChar, 20, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,PaidCurrencyWallet),
                        new SqlParameter("@PaidQuantity",SqlDbType.Decimal, 20, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default,PaidQauntity),
                        new SqlParameter("@PaidCurrency",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,PaidCurrency),
                        new SqlParameter("@DeliveredCurrency",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,DeliveryCurrency),
                        new SqlParameter("@RoundGuid",SqlDbType.VarChar, 50, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,RoundID),
                        new SqlParameter("@Remarks",SqlDbType.VarChar, 500, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,Remarks),
                        new SqlParameter("@InstantDeliverdQuantity",SqlDbType.Decimal, 8, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default, bizResponseClass.InstantDeliverdQuantity) ,
                        new SqlParameter("@DeliveredCurrencyOP",SqlDbType.VarChar, 10, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.DeliveryCurrency) ,
                        new SqlParameter("@MaxDeliverQuantity",SqlDbType.Decimal, 8, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default, bizResponseClass.MaxDeliverQuantity) ,
                        new SqlParameter("@MinimumPurchaseAmt",SqlDbType.Decimal, 8, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default, bizResponseClass.MinimumPurchaseAmt) ,
                        new SqlParameter("@MaximumPurchaseAmt",SqlDbType.Decimal, 8, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default, bizResponseClass.MaximumPurchaseAmt) ,
                        new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                        new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                        new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode) ,
                        new SqlParameter("@RefGuid",SqlDbType.VarChar, 50, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode) ,
                        };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_IEOConfirmTransaction @UserId, @PaidAccWalletId,@PaidQuantity,@PaidCurrency,@DeliveredCurrency,@RoundGuid,@Remarks,@InstantDeliverdQuantity OUTPUT,@DeliveredCurrencyOP OUTPUT,@MaxDeliverQuantity OUTPUT,@MinimumPurchaseAmt OUTPUT,@MaximumPurchaseAmt OUTPUT,@ReturnCode OUTPUT,@ReturnMsg OUTPUT , @ErrorCode OUTPUT,@RefGuid OUTPUT", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("SP_IEOConfirmTransaction", "IEOWalletController", " ReturnCode:" + @param1[12].Value.ToString() + " ReturnMsg:" + @param1[13].Value.ToString() + " ErrorCode :" + @param1[14].Value.ToString()));

                bizResponseClass.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[12].Value);
                bizResponseClass.ReturnMsg = @param1[13].Value.ToString();
                bizResponseClass.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[14].Value);

                bizResponseClass.RefNo = @param1[15].Value.ToString();

                bizResponseClass.InstantDeliverdQuantity = Convert.ToDecimal(@param1[7].Value);
                bizResponseClass.DeliveryCurrency = @param1[8].Value.ToString();
                bizResponseClass.MaxDeliverQuantity = Convert.ToDecimal(@param1[9].Value);
                bizResponseClass.MinimumPurchaseAmt = Convert.ToDecimal(@param1[10].Value);
                bizResponseClass.MaximumPurchaseAmt = Convert.ToDecimal(@param1[11].Value);
                return bizResponseClass;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                bizResponseClass.ErrorCode = enErrorCode.InternalError;
                bizResponseClass.ReturnCode = enResponseCode.InternalError;
                return bizResponseClass;
            }
        }

        public PreConfirmResponseV2 CallSP_PreConfirm(string PaidCurrencyWallet, decimal PaidQauntity, string PaidCurrency, string DeliveryCurrency, string RoundID, string Remarks, Int64 UserID)
        {
            PreConfirmResponseV2 bizResponseClass = new PreConfirmResponseV2();
            try
            {
                SqlParameter[] param1 = new SqlParameter[]{
                        new SqlParameter("@UserId",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,UserID),
                        new SqlParameter("@PaidAccWalletId",SqlDbType.VarChar, 20, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,PaidCurrencyWallet),
                        new SqlParameter("@PaidQuantity",SqlDbType.Decimal, 20, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default,PaidQauntity),
                        new SqlParameter("@PaidCurrency",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,PaidCurrency),
                        new SqlParameter("@DeliveredCurrency",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,DeliveryCurrency),
                        new SqlParameter("@RoundGuid",SqlDbType.VarChar, 50, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,RoundID),
                        new SqlParameter("@Remarks",SqlDbType.VarChar, 500, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,Remarks),
                        new SqlParameter("@InstantDeliverdQuantity",SqlDbType.Decimal, 8, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default, bizResponseClass.InstantDeliverdQuantity) ,
                        new SqlParameter("@DeliveredCurrencyOP",SqlDbType.VarChar, 10, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.DeliveryCurrency) ,
                        new SqlParameter("@MaxDeliverQuantity",SqlDbType.Decimal, 8, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default, bizResponseClass.MaxDeliverQuantity) ,
                        new SqlParameter("@MinimumPurchaseAmt",SqlDbType.Decimal, 8, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default, bizResponseClass.MinimumPurchaseAmt) ,
                        new SqlParameter("@MaximumPurchaseAmt",SqlDbType.Decimal, 8, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default, bizResponseClass.MaximumPurchaseAmt) ,
                        new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                        new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                        new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode) ,
                        new SqlParameter("@PaidWalletId",SqlDbType.BigInt, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.PaidWalletId) ,
                        new SqlParameter("@DeliveredWalletId",SqlDbType.BigInt, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.DeliveredWalletId) ,
                        new SqlParameter("@DeliveredCurrencyId",SqlDbType.BigInt, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.DeliveredCurrencyId) ,
                        new SqlParameter("@PaidCurrencyId",SqlDbType.BigInt, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.PaidCurrencyId) ,
                        new SqlParameter("@RoundId",SqlDbType.BigInt, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.RoundId) ,
                        new SqlParameter("@InstantPercentage",SqlDbType.Decimal, 28, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default, bizResponseClass.InstantPercentage),
                        new SqlParameter("@BonusPercentage",SqlDbType.Decimal, 28, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default, bizResponseClass.InstantPercentage),
                        new SqlParameter("@BonusAmount",SqlDbType.Decimal, 28, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default, bizResponseClass.InstantPercentage),
                        new SqlParameter("@Rate",SqlDbType.Decimal, 28, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default, bizResponseClass.InstantPercentage),
                        new SqlParameter("@MaxDeliverQuantityWOBonus",SqlDbType.Decimal, 28, ParameterDirection.Output, false, 28, 18, String.Empty, DataRowVersion.Default, bizResponseClass.InstantPercentage)
                        };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_IEOTransactionPreConfirm @UserId, @PaidAccWalletId,@PaidQuantity,@PaidCurrency,@DeliveredCurrency,@RoundGuid,@Remarks,@InstantDeliverdQuantity OUTPUT,@DeliveredCurrencyOP OUTPUT,@MaxDeliverQuantity OUTPUT,@MinimumPurchaseAmt OUTPUT,@MaximumPurchaseAmt OUTPUT,@ReturnCode OUTPUT,@ReturnMsg OUTPUT , @ErrorCode OUTPUT,@PaidWalletId OUTPUT,@DeliveredWalletId OUTPUT,@DeliveredCurrencyId OUTPUT,@PaidCurrencyId OUTPUT,@RoundId OUTPUT,@InstantPercentage OUTPUT,@BonusPercentage OUTPUT,@BonusAmount OUTPUT,@Rate OUTPUT @MaxDeliverQuantityWOBonus OUTPUT ", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("SP_IEOTransactionPreConfirm", "IEOWalletController", " ReturnCode:" + @param1[12].Value.ToString() + " ReturnMsg:" + @param1[13].Value.ToString() + " ErrorCode :" + @param1[14].Value.ToString()));

                bizResponseClass.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[12].Value);
                bizResponseClass.ReturnMsg = @param1[13].Value.ToString();
                bizResponseClass.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[14].Value);
                bizResponseClass.InstantDeliverdQuantity = Convert.ToDecimal(@param1[7].Value);
                bizResponseClass.DeliveryCurrency = @param1[8].Value.ToString();
                bizResponseClass.MaxDeliverQuantity = Convert.ToDecimal(@param1[9].Value);
                bizResponseClass.MinimumPurchaseAmt = Convert.ToDecimal(@param1[10].Value);
                bizResponseClass.MaximumPurchaseAmt = Convert.ToDecimal(@param1[11].Value);

                bizResponseClass.PaidWalletId = Convert.ToInt64(@param1[15].Value);
                bizResponseClass.DeliveredWalletId = Convert.ToInt64(@param1[16].Value);
                bizResponseClass.DeliveredCurrencyId = Convert.ToInt64(@param1[17].Value);
                bizResponseClass.PaidCurrencyId = Convert.ToInt64(@param1[18].Value);
                bizResponseClass.RoundId = Convert.ToInt64(@param1[19].Value);
                bizResponseClass.InstantPercentage = Convert.ToDecimal(@param1[20].Value);
                return bizResponseClass;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                bizResponseClass.ErrorCode = enErrorCode.InternalError;
                bizResponseClass.ReturnCode = enResponseCode.InternalError;
                return bizResponseClass;
            }
        }

        public BizResponseClass Callsp_IEOAdminWalletCreditBalance(IEOAdminWalletCreditReq request)
        {
            BizResponseClass bizResponseClass = new BizResponseClass();
            try
            {
                SqlParameter[] param1 = new SqlParameter[]{
                        new SqlParameter("@WalletId",SqlDbType.VarChar, 20, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,request.AdminWalletId),
                         new SqlParameter("@Amount",SqlDbType.Decimal, 30, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default,request.Amount),
                         new SqlParameter("@Remarks",SqlDbType.VarChar, 250, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,request.Remarks),
                         new SqlParameter("@WalletTypeName",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,request.WalletTypeName),
                        new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                        new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                        new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode) 
                        };
                var res = _dbContext.Database.ExecuteSqlCommand("sp_IEOAdminWalletCreditBalance @WalletId,@Amount,@Remarks,@WalletTypeName, @ReturnCode OUTPUT,@ReturnMsg OUTPUT , @ErrorCode OUTPUT", param1);
                Task.Run(() => HelperForLog.WriteLogIntoFileAsync("sp_IEOAdminWalletCreditBalance", "IEOWalletController", " ReturnCode:" + @param1[4].Value.ToString() + " ReturnMsg:" + @param1[5].Value.ToString() + " ErrorCode :" + @param1[6].Value.ToString()));

                bizResponseClass.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[4].Value);
                bizResponseClass.ReturnMsg = @param1[5].Value.ToString();
                bizResponseClass.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[6].Value);
                return bizResponseClass;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                bizResponseClass.ErrorCode = enErrorCode.InternalError;
                bizResponseClass.ReturnCode = enResponseCode.InternalError;
                return bizResponseClass;
            }
        }
    }
}
