using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Transaction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Data.Transaction
{
    public class WithdrawTransactionRepository : IWithdrawTransactionRepository
    {
        private readonly WorldexContext _dbContext;

        public WithdrawTransactionRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<WithdrawERCAdminAddress> GetERCAdminAddress(string Coin)
        {
            try
            {
                IQueryable<WithdrawERCAdminAddress> Result = null;

                Result = _dbContext.WithdrawERCAdminAddress.FromSql(
                                @"select Am.Id as [AddressId],OriginalAddress as [Address],Am.GUID as RefKey
                                from  AddressMasters AM inner join WalletMasters wm on wm.id=am.WalletId
                                inner join WalletTypeMasters WTM on WTM.Id= wm.wallettypeid    
                                where wm.UserID = (select top 1 UserID from BizUserTypeMapping where UserType=0)  
                                and WTM.WalletTypeName={0} and wm.isdefaultwallet=1 and Am.Status = 1 and AM.AddressType=1 Order By Am.Id asc", Coin); //ntrivedi and AM.AddressType=1 added for admin address identification 18-04-2019 , for commisssion deposition in admin wallet balance need to add condition for AM.AddressType=1

                return Result.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BizResponseClass WithdrwalInteranlTransferProcess(string RefId, string timestamp, int ChannelId)
        {
            try
            {
                BizResponseClass Response = new BizResponseClass();

                SqlParameter[] param1 = new SqlParameter[]{
                        new SqlParameter("@GUID",SqlDbType.VarChar, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, RefId),
                        new SqlParameter("@timeStamp",SqlDbType.VarChar, 50, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,timestamp),
                        new SqlParameter("@ChannelID",SqlDbType.Int, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, ChannelId),
                        new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Response.ReturnCode) ,
                        new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Response.ReturnMsg) ,
                        new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Response.ErrorCode) ,
                };
                var res = _dbContext.Database.ExecuteSqlCommand("sp_WithdrawInternalTransfer @GUID ,@timeStamp ,@ChannelID ,@ReturnCode  OUTPUT,@ReturnMsg OUTPUT , @ErrorCode  OUTPUT", param1);

                Response.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[3].Value);
                Response.ReturnMsg = @param1[4].Value.ToString();
                Response.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[5].Value);

                return Response;
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
    }
}
