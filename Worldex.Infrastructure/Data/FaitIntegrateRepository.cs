using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.FiatBankIntegration;
using Worldex.Core.ViewModels.Wallet;

namespace Worldex.Infrastructure.Data
{
    /// <summary>
    /// vsolanki 2019-10-9 Added New Repository Implementation for Fiat COnfiguration
    /// </summary>
    public class FiatIntegrateRepository : IFiatIntegrateRepository
    {
        #region COTR

        private WorldexContext _dbContext;

        public FiatIntegrateRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }
        #endregion

        public List<GetLTP> GetFiatLTP(short? TransactionType)
        {
            try
            {
                List<GetLTP> res = new List<GetLTP>();
                //res = _dbContext.GetLTP.FromSql("SELECT PairId,LTP,TPM.PairName,CurrentRate FROM TradePairStastics TPS inner join TradePairMaster TPM on TPM.Id = TPS.PairId WHERE TPM.pairname IN ('SOX_USDT','BTC_USDT','ETH_USDT','USDX_USDT')").ToListAsync().GetAwaiter().GetResult();
                SqlParameter[] param1 = new SqlParameter[]{
                        new SqlParameter("@TransactionType",SqlDbType.SmallInt, 20, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,(TransactionType==null?0:TransactionType)) };

                res = _dbContext.GetLTP.FromSql("Sp_GetFiatLTP @TransactionType", param1).ToList();
                // res = _dbContext.GetLTP.FromSql("Sp_GetFiatLTP").ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public List<FiatBuyHistory> FiatBuyHistory(string FromCurrency, string ToCurrency, short? Status, string TrnId, string Email, DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                string Query = "SELECT u.Email,case B.Status when 0 then 'Initialize' when 6 then 'Pending' when 1 then 'Success' when 2 then 'Rejected' else 'Other' end as StrStatus," +
                    "B.Status,Guid As TrnId,FromAmount,ToAmount,B.CreatedDate ,CoinRate,FiatConverationRate,Fee,FromCurrency,ToCurrency,Address,TransactionHash," +
                    "NotifyUrl,TransactionId,TransactionCode,Platform,Code,Remarks FROM BuySellTopUpRequest B Inner Join Bizuser u On u.id=B.UserId " +
                    "Where  Type=1 AND (u.Email={0} OR {0}='') AND (B.FromCurrency={1} OR {1}='') AND (B.ToCurrency={2} OR {2}='') AND (B.Guid={3} OR {3}='') " +
                    "AND (B.Status={4} OR {4}=999)";
                if (FromDate != null && ToDate != null)
                {
                    FromDate = Convert.ToDateTime(FromDate).Date.AddHours(0).AddMinutes(0).AddSeconds(0);
                    ToDate = Convert.ToDateTime(ToDate).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += " AND B.CreatedDate BETWEEN {5} AND {6} ";
                    //Query = string.Format(Query, string.Format("{0:yyyy-MM-dd HH:mm:ss}", FromDate), string.Format("{0:yyyy-MM-dd HH:mm:ss}", ToDate));
                }
                Query += "  order by B.id desc";
                List<FiatBuyHistory> res = new List<FiatBuyHistory>();
                res = _dbContext.FiatBuyHistory.FromSql(Query, (Email == null ? "" : Email), (FromCurrency == null ? "" : FromCurrency), (ToCurrency == null ? "" : ToCurrency), (TrnId == null ? "" : TrnId), (Status == null ? 999 : Status), FromDate, ToDate).ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public List<FiatSellHistory> FiatSellHistory(string FromCurrency, string ToCurrency, short? Status, string TrnId, string Email, DateTime? FromDate, DateTime? ToDate)
        {
            try
            {

                string Query = @"select u.Email,B.CreatedDate,case B.Status when 0 then 'Approval Pending' when 6 then 'Pending' when 1 then 'Success' when 2 then 'Operator Fail'  when 3 then 'System Fail' when 4 then 'Rejected' when 5 then 'Cancel' when 7 then 'Withdraw ReInitialize' when 8 then 'Completed' when 9 then 'Expired' else 'Other' end as  StrStatus,B.Status,Guid,FromAmount,ToAmount,CoinRate,FiatConverationRate,Fee,UserId,FromCurrency,ToCurrency,
                        Address,TransactionHash,NotifyUrl,TransactionId,TransactionCode,UserGuid,Platform,Type,FromBankId,ToBankId,
                        Code,Remarks,isnull(BankName,'') as BankName,isnull(CurrencyName,'') as CurrencyName,isnull(BankId,'') as BankId,isnull(CurrencyId,'') as CurrencyId,isnull(user_bank_name,'') as user_bank_name,isnull(user_bank_account_number,'') as user_bank_account_number,isnull(user_bank_acount_holder_name,'') as user_bank_acount_holder_name,isnull(user_currency_code,'') as user_currency_code,
                        isnull(payus_transaction_id,'') as payus_transaction_id,payus_amount_usd,payus_amount_crypto,payus_mining_fees,payus_total_payable_amount,payus_fees,payus_total_fees ,
                        payus_usd_rate ,payus_expire_datetime ,isnull(payus_payment_tracking,'') as payus_payment_tracking   from SellTopUpRequest  B Inner Join Bizuser u On u.id=B.UserId Where  Type=2 AND (u.Email={0} OR {0}='') AND (B.FromCurrency={1} OR {1}='') AND (B.ToCurrency={2} OR {2}='') AND (B.Guid={3} OR {3}='') " +
                    "AND (B.Status={4} OR {4}=999)";
                if (FromDate != null && ToDate != null)
                {
                    FromDate = Convert.ToDateTime(FromDate).Date.AddHours(0).AddMinutes(0).AddSeconds(0);
                    ToDate = Convert.ToDateTime(ToDate).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += " AND B.CreatedDate BETWEEN {5} AND {6} ";
                    //Query = string.Format(Query, string.Format("{0:yyyy-MM-dd HH:mm:ss}", FromDate), string.Format("{0:yyyy-MM-dd HH:mm:ss}", ToDate));
                }
                Query += "  order by B.id desc";
                List<FiatSellHistory> res = new List<FiatSellHistory>();
                res = _dbContext.FiatSellHistory.FromSql(Query, (Email == null ? "" : Email), (FromCurrency == null ? "" : FromCurrency), (ToCurrency == null ? "" : ToCurrency), (TrnId == null ? "" : TrnId), (Status == null ? 999 : Status), FromDate, ToDate).ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public GetFiatTradeInfo GetFiatTradeInfo()
        {
            try
            {
                var data = _dbContext.GetFiatTradeInfo.FromSql("SELECT TermsAndCondition,IsBuyEnable,IsSellEnable,ISNULL(Platform,'-')AS Platform,ISNULL(SellCallBackURL,'-')AS SellCallBackURL,ISNULL(WithdrawURL,'-')AS WithdrawURL,CreatedDate FROM FiatTradeConfigurationMaster WHERE Status=1").FirstOrDefault();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public List<GetFiatCurrencyInfo> GetFiatCurrencyInfo()
        {
            try
            {
                List<GetFiatCurrencyInfo> res = new List<GetFiatCurrencyInfo>();
                res = _dbContext.GetFiatCurrencyInfo.FromSql("select Id,CreatedDate,Name,Status,CurrencyCode,USDRate,BuyFee,SellFee,BuyFeeType,SellFeeType from FiatCurrencyMaster where Status =1").ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public List<GetFiatCurrencyInfo> GetFiatCurrencyInfoBO(short? Status)
        {
            try
            {
                List<GetFiatCurrencyInfo> res = new List<GetFiatCurrencyInfo>();
                res = _dbContext.GetFiatCurrencyInfo.FromSql("select Id,CreatedDate,Name,Status,CurrencyCode,USDRate,BuyFee,SellFee,BuyFeeType,SellFeeType from FiatCurrencyMaster where Status<9 AND (Status={0} OR {0}=999)", (Status == null ? Convert.ToInt16(999) : Convert.ToInt16(Status))).ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //komal 1 Nov 2019 1:40 for fiatSellWithdraw
        public List<FiatSellWithdrawTraxn> GetSellWithdrawTrxn()
        {
            try
            {
                List<FiatSellWithdrawTraxn> res = new List<FiatSellWithdrawTraxn>();
                //res = _dbContext.FiatSellWithdrawTraxn.FromSql("SELECT TQ.id as Trnno,TQ.Memberid,TQ.GUID as RefNo,ST.Guid as WithdrawRefNo FROM SellTopUpRequest ST INNER JOIN TransactionQueue TQ ON ST.Trnno=TQ.id WHERE ST.Status=6 AND ST.APIStatus=1 AND TQ.Status=4 AND TQ.IsVerified=1 AND ST.Address <> 'N/A' AND (ST.TransactionId Is Not Null or ST.TransactionId <> '')").ToList();
                res = _dbContext.FiatSellWithdrawTraxn.FromSql("select A.MemberID,A.RefNo,A.WithdrawRefNo,A.Trnno from (SELECT top 1 TQ.id as Trnno,TQ.Memberid,TQ.GUID as RefNo,ST.Guid as WithdrawRefNo FROM SellTopUpRequest ST INNER JOIN TransactionQueue TQ ON ST.Trnno=TQ.id WHERE ST.Status=6 AND ST.APIStatus=1 AND TQ.Status=4 AND TQ.IsVerified=1 AND ST.Address <> 'N/A' AND (ST.TransactionId Is Not Null or ST.TransactionId <> '') and TQ.Id not in (select TR.TrnNo from TransactionRequest TR where TQ.ID=TR.TrnNo and (TR.trnid is null or TR.trnid ='') and (TR.OprTrnID is  null and TR.OprTrnID = '') and (TR.ResponseData is not null or TR.ResponseData <> ''))) as A union select  B.MemberID,B.RefNo,B.WithdrawRefNo,B.Trnno from(SELECT top 1 TQ.id as Trnno,TQ.Memberid,TQ.GUID as RefNo,ST.Guid as WithdrawRefNo FROM SellTopUpRequest ST INNER JOIN TransactionQueue TQ ON ST.Trnno = TQ.id WHERE ST.Status = 7 AND ST.APIStatus = 1 AND TQ.Status = 4 AND TQ.IsVerified = 1 AND ST.Address <> 'N/A' AND(ST.TransactionId Is Not Null or ST.TransactionId <> '') and TQ.Id not in (select TrnNo from TransactionRequest TR where TQ.ID = TR.TrnNo and(TR.trnid is not null and TR.trnid <> '') and (TR.OprTrnID is not null and TR.OprTrnID <> '') and(TR.ResponseData is not null and TR.ResponseData <> '')) and ST.UpdatedDate < Dateadd(MINUTE, -15, dbo.GetISTDate()) order by ST.UpdatedDate) as B").ToList();
                if (res.Count > 0)
                {
                    var TrnnoList = res.Select(e => e.Trnno).ToArray();
                    var CommaseparatedTrnno = string.Join(",", TrnnoList);
                    _dbContext.Database.ExecuteSqlCommand("UPDATE SellTopUpRequest SET APIStatus=2 WHERE TrnNo IN (" + CommaseparatedTrnno + ")");
                }
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public List<FiatSellWithdrawTraxn> GetSellWithdrawPendingTrxn()
        {
            try
            {
                List<FiatSellWithdrawTraxn> res = new List<FiatSellWithdrawTraxn>();
               
                res = _dbContext.FiatSellWithdrawTraxn.FromSql("select top 1 t.id as Trnno,t.Memberid,t.GUID as RefNo,s.Guid as WithdrawRefNo from SellTopUprequest s inner join WithdrawHistory W on W.TrnNo =s.Trnno and (W.Trnid is not null and W.TrnId <> '') inner join Transactionqueue t on t.id=s.trnno where (s.Transactionhash is null or s.Transactionhash='') and s.status in (6,7) and s.apistatus=2").ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public List<LPTPairFiat> GetPairForBinnance()
        {
            try
            {
                List<LPTPairFiat> res = new List<LPTPairFiat>();
                res = _dbContext.LPTPairFiat.FromSql("select  distinct WTMF.WalletTypeName+'_'+WTMB.WalletTypeName as PairName from FiatCoinConfiguration FCC " +
                               " inner join Wallettypemasters WTMF on WTMF.id = FCC.FromCurrencyId " +
                               " inner join Wallettypemasters WTMB on WTMB.id = FCC.ToCurrencyID " +
                                " where FCC.status = 1  ").ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public string GetWithdrawTrnId(long TrnNo)
        {
            try
            {
                var data = _dbContext.GetWithdarwData.FromSql("select Top 1 Isnull(TrnId,'') as TrnId from WithdrawHistory where trnno={0} Order By Id desc", TrnNo).FirstOrDefault();
                if (data != null)
                {
                    if (!string.IsNullOrEmpty(data.TrnId))
                    {
                        return data.TrnId;
                    }
                    return "";
                }
                return "";
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
    }
}
