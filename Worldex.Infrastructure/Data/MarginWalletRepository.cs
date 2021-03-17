using Worldex.Core.ApiModels;
using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Entities.NewWallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.MarginWallet;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletOperations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Data
{
    public class MarginWalletRepository : IMarginWalletRepository
    {
        #region cotr

        private readonly WorldexContext _dbContext;
        private readonly ICommonRepository<LeverageMaster> _LeverageMaster;

        public MarginWalletRepository(WorldexContext dbContext, ICommonRepository<LeverageMaster> LeverageMaster)
        {
            _dbContext = dbContext;
            _LeverageMaster = LeverageMaster;
        }

        #endregion

        #region Method
        public List<MarginWalletMasterRes2> ListMarginWallet(int PageNo, int PageSize, long? WalletTypeId, EnWalletUsageType? WalletUsageType, short? Status, string AccWalletId, long? UserId, ref int TotalCount)
        {
            try
            {
                List<MarginWalletMasterRes2> res = new List<MarginWalletMasterRes2>();
                res = _dbContext.MarginWalletMasterRes2.FromSql("select ISNULL(b.UserName,'N/A') as UserName,ISNULL(b.Email,'N/A') as Email,r.Id as RoleId ,r.RoleName as RoleName ,u.AccWalletID,u.ExpiryDate,u.UserId,ISNULL(u.OrgID,0) AS OrgID,u.Walletname as WalletName,c.WalletTypeName as CoinName,ISNull(u.PublicAddress,'N/A') as PublicAddress,u.Balance,u.IsDefaultWallet,u.InBoundBalance,u.OutBoundBalance ,u.WalletUsageType,case u.WalletUsageType when 0 then 'Trading_Wallet' when 1 then 'Market_Wallet'WHEN 2 then 'Cold_Wallet' when 3 then 'Charge_Cr_Wallet_ORG' when 4 then 'Deposition_Admin_Wallet' when 5 then 'Margin_Trading_Wallet' when 6 then 'Margin_Safety_Wallet'when 7 then 'Margin_Profit_Wallet'else 'N/A'  end as WalletUsageTypeName,u.Status ,case u.Status when 0 then 'InActive' WHEN 1 then 'Active' When 9 then 'Delete' ELSE 'N/A' End as StrStatus from MarginWalletAuthorizeUserMaster wa inner join MarginWalletMaster u on u.Id=wa.WalletID inner join MarginWalletTypeMaster c on c.Id= u.WalletTypeID inner join BizUser b on b.id=wa.userid inner join MarginUserRoleMaster r on r.id=wa.RoleID where wa.Status = 1 AND (wa.UserID={3} or {3}=0) and  ( u.walletusagetype ={0} Or {0}=999 ) AND (u.wallettypeid ={1} Or {1}=0) AND (wa.status ={2} Or {2}=999) and (u.AccWalletId={4} or {4}='')", (WalletUsageType == null ? Convert.ToInt16(999) : Convert.ToInt16(WalletUsageType)), (WalletTypeId == null ? 0 : WalletTypeId), (Status == null ? 999 : Status), (UserId == null ? 0 : UserId), (AccWalletId == null ? "" : AccWalletId)).ToList();

                TotalCount = res.Count();
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32((PageSize) * (PageNo - 1));
                    res = res.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListMarginWalletMaster", "MarginWalletRepository", ex);
                throw ex;
            }
        }

        public List<MarginWalletMasterRes> ListMarginWalletMaster(long? WalletTypeId, EnWalletUsageType? WalletUsageType, short? Status, string AccWalletId, long? UserId)
        {
            try
            {
                List<MarginWalletMasterRes> res = new List<MarginWalletMasterRes>();
                res = _dbContext.MarginWalletMasterRes.FromSql("select r.Id as RoleId ,r.RoleName as RoleName ,u.AccWalletID,u.ExpiryDate,u.UserId,ISNULL(u.OrgID,0) AS OrgID,u.Walletname as WalletName,c.WalletTypeName as CoinName,ISNull(u.PublicAddress,'N/A') as PublicAddress,u.Balance,u.IsDefaultWallet,u.InBoundBalance,u.OutBoundBalance ,u.WalletUsageType,case u.WalletUsageType when 0 then 'Trading_Wallet' when 1 then 'Market_Wallet'WHEN 2 then 'Cold_Wallet' when 3 then 'Charge_Cr_Wallet_ORG' when 4 then 'Deposition_Admin_Wallet' when 5 then 'Margin_Trading_Wallet' when 6 then 'Margin_Safety_Wallet' when 7 then 'Margin_Profit_Wallet'else 'N/A'  end as WalletUsageTypeName,u.Status ,case u.Status when 0 then 'InActive' WHEN 1 then 'Active' When 9 then 'Delete' ELSE 'N/A' End as StrStatus from MarginWalletAuthorizeUserMaster wa inner join MarginWalletMaster u on u.Id=wa.WalletID inner join MarginWalletTypeMaster c on c.Id= u.WalletTypeID inner join MarginUserRoleMaster r on r.id=wa.RoleID where wa.Status = 1 AND (wa.UserID={3} or {3}=0) and  ( u.walletusagetype ={0} Or {0}=999 ) AND (u.wallettypeid ={1} Or {1}=0) AND (wa.status ={2} Or {2}=999) and (u.AccWalletId={4} or {4}='')", (WalletUsageType == null ? Convert.ToInt16(999) : Convert.ToInt16(WalletUsageType)), (WalletTypeId == null ? 0 : WalletTypeId), (Status == null ? 999 : Status), (UserId == null ? 0 : UserId), (AccWalletId == null ? "" : AccWalletId)).ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListMarginWalletMaster", "MarginWalletRepository", ex);
                throw ex;
            }
        }

        public List<MarginWalletByUserIdRes> GetMarginWalletByUserId(long UserId)
        {
            try
            {
                List<MarginWalletByUserIdRes> res = new List<MarginWalletByUserIdRes>();
                res = _dbContext.MarginWalletByUserIdRes.FromSql("select u.Id,u.AccWalletID,u.Walletname as WalletName from MarginWalletAuthorizeUserMaster wa inner join MarginWalletMaster u on u.Id=wa.WalletID where wa.Status = 1 AND (wa.UserID={0}) ", UserId).ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListMarginWalletMaster", "MarginWalletRepository", ex);
                throw ex;
            }
        }

        public async Task<bool> CheckTrnIDDrForHoldAsync(MarginCommonClassCrDr arryTrnID)
        {
            try
            {
                var response = _dbContext.TempEntity.FromSql("select TrnNo,SettedAmt AS SetteledAmount,Amount from MarginWalletTransactionQueue where TrnRefNo={0} and(Status = 4) and TrnType = {1} and (Amount - SettedAmt >= {2}) And WalletTrnType = {3}", arryTrnID.debitObject.TrnRefNo, Core.Enums.enWalletTranxOrderType.Debit, arryTrnID.Amount, arryTrnID.debitObject.trnType).ToList();

                if (response.Count != 1)
                {
                    return false;
                }
                arryTrnID.debitObject.WTQTrnNo = response[0].TrnNo;

                //var deliveredAmt = (from p in _dbContext.WalletTransactionOrders
                //                    where p.DTrnNo == arryTrnID.debitObject.WTQTrnNo && p.Status != enTransactionStatus.SystemFail
                //                    select p.Amount).Sum();

                var deliveredAmt = _dbContext.BalanceTotal.FromSql("select isNull(SUM(Amount),0) as TotalBalance from MarginWalletTransactionOrder where DTrnNo={0} and Status<>3", arryTrnID.debitObject.WTQTrnNo).FirstOrDefault().TotalBalance;

                if (!(response[0].Amount - deliveredAmt - arryTrnID.Amount >= 0))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CheckTrnIDDrForHoldAsync", this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<bool> CheckTrnIDDrForMarketAsync(MarginCommonClassCrDr arryTrnID)
        {
            try
            {
                GetCount count;
                TQTrnAmt sumAmount;

                IQueryable<GetCount> Result1 = _dbContext.GetCount.FromSql(@"SELECT count(TrnNo)  as 'Count' FROM MarginWalletTransactionQueue WHERE TrnRefNo = {0} and Status=4 and TrnType={1} and WalletDeductionType={2}", arryTrnID.debitObject.TrnRefNo, enWalletTranxOrderType.Debit, enWalletDeductionType.Market);
                IQueryable<TQTrnAmt> Result2 = _dbContext.TQTrnAmt.FromSql(@"SELECT Amount-SettedAmt as 'DifferenceAmount',TrnNo  FROM MarginWalletTransactionQueue WHERE TrnRefNo = {0} and Status=4 and TrnType={1} and WalletDeductionType={2}", arryTrnID.debitObject.TrnRefNo, enWalletTranxOrderType.Debit, enWalletDeductionType.Market);
                count = Result1.First();
                sumAmount = Result2.First();
                if (count.Count != 1)
                {
                    return false;
                }
                if (sumAmount.DifferenceAmount < arryTrnID.Amount)
                {
                    arryTrnID.debitObject.differenceAmount = arryTrnID.Amount - sumAmount.DifferenceAmount;
                }
                arryTrnID.debitObject.WTQTrnNo = sumAmount.TrnNo;
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public void ReloadEntity(MarginWalletMaster wm1, MarginWalletMaster wm2, MarginWalletMaster wm3, MarginWalletMaster wm4)
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
                try
                {
                    _dbContext.Entry(wm4).Reload();
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name + "w4", this.GetType().Name, ex);
                }

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        public List<LeaverageReport> LeverageRequestReport(long? WalletTypeId, long UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize, short? Status, ref int TotalCount)
        {
            try
            {
                List<LeaverageReport> Data = new List<LeaverageReport>();
                string sql = "SELECT mr.MaxLeverage,ISNULL(mr.ApprovedDate,mr.TrnDate) AS ApprovedDate,mr.ApprovedBy,case ISNULL(mr.ApprovedBy,0) WHEN 0  then 'N/A' else ISNULL((SELECT Username from BizUser where id=mr.ApprovedBy),'N/A') END  AS ApprovedByUserName,cast (round((mr.Leverage),2) as decimal(28,2)) as LeveragePer,mr.Id,mr.WalletTypeID,wt.WalletTypeName,mr.FromWalletID,w.WalletName AS FromWalletName,mr.ToWalletID,(SELECT WalletName FROM MarginWalletMaster WHERE Id=mr.ToWalletID) AS ToWalletName,mr.UserID,b.UserName,b.Email,mr.LeverageID,mr.IsAutoApprove,mr.RequestRemarks,mr.Amount,mr.TrnDate,mr.LeverageAmount,mr.ChargeAmount,mr.SafetyMarginAmount,mr.CreditAmount,mr.SystemRemarks,mr.Status,CASE mr.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Open'  WHEN 6 THEN 'Waiting for Admin Approve' WHEN 9 THEN 'Rejected' When 5 Then 'Withdrw' ELSE 'N/A' END AS StrStatus FROM MarginLoanRequest mr INNER JOIN LeverageMaster l ON l.Id=mr.LeverageID INNER JOIN BizUser b on b.Id=mr.UserID INNER JOIN MarginWalletTypeMaster wt ON wt.Id=mr.WalletTypeID INNER JOIN WalletMasters w ON w.Id=mr.FromWalletID WHERE  (mr.Status={0} OR {0}=999) AND (mr.Userid={1}) AND (mr.WalletTypeID={2} OR {2}=0)";

                if (FromDate != null && ToDate != null)
                {
                    FromDate = Convert.ToDateTime(FromDate).AddHours(00).AddMinutes(00).AddSeconds(00);
                    ToDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);

                    sql = sql + "AND (mr.TrnDate>={3} AND mr.TrnDate<={4})";
                    Data = _dbContext.LeaverageReport.FromSql(sql, (Status == null ? 999 : Status), UserId, (WalletTypeId == null ? 0 : WalletTypeId), FromDate, ToDate).ToList();
                }
                else
                {
                    Data = _dbContext.LeaverageReport.FromSql(sql, (Status == null ? 999 : Status), UserId, (WalletTypeId == null ? 0 : WalletTypeId)).ToList();
                }

                TotalCount = Data.Count();
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32((PageSize) * (PageNo - 1));
                    Data = Data.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<LeaverageReportv2> LeverageRequestReportv2(long? WalletTypeId, long UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize, short? Status, ref int TotalCount)
        {
            try
            {
                List<LeaverageReportv2> Data = new List<LeaverageReportv2>();
                string sql = "SELECT mr.MaxLeverage,ISNULL(mr.ApprovedDate,mr.TrnDate) AS ApprovedDate,mr.ApprovedBy,case ISNULL(mr.ApprovedBy,0) WHEN 0  then 'N/A' else ISNULL((SELECT Username from BizUser where id=mr.ApprovedBy),'N/A') END  AS ApprovedByUserName,cast (round((mr.Leverage),2) as decimal(28,2)) as LeveragePer,mr.Guid as Id,mr.WalletTypeID,wt.WalletTypeName,mr.FromWalletID,w.WalletName AS FromWalletName,mr.ToWalletID,(SELECT WalletName FROM MarginWalletMaster WHERE Id=mr.ToWalletID) AS ToWalletName,mr.UserID,b.UserName,b.Email,mr.LeverageID,mr.IsAutoApprove,mr.RequestRemarks,mr.Amount,mr.TrnDate,mr.LeverageAmount,mr.ChargeAmount,mr.SafetyMarginAmount,mr.CreditAmount,mr.SystemRemarks,mr.Status,CASE mr.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Open'  WHEN 6 THEN 'Waiting for Admin Approve' WHEN 9 THEN 'Rejected' When 5 Then 'Withdrw' ELSE 'N/A' END AS StrStatus FROM MarginLoanRequest mr INNER JOIN LeverageMaster l ON l.Id=mr.LeverageID INNER JOIN BizUser b on b.Id=mr.UserID INNER JOIN MarginWalletTypeMaster wt ON wt.Id=mr.WalletTypeID INNER JOIN WalletMasters w ON w.Id=mr.FromWalletID WHERE  (mr.Status={0} OR {0}=999) AND (mr.Userid={1}) AND (mr.WalletTypeID={2} OR {2}=0)";

                if (FromDate != null && ToDate != null)
                {
                    FromDate = Convert.ToDateTime(FromDate).AddHours(00).AddMinutes(00).AddSeconds(00);
                    ToDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);

                    sql = sql + "AND (mr.TrnDate>={3} AND mr.TrnDate<={4})";
                    Data = _dbContext.LeaverageReportv2.FromSql(sql, (Status == null ? 999 : Status), UserId, (WalletTypeId == null ? 0 : WalletTypeId), FromDate, ToDate).ToList();
                }
                else
                {
                    Data = _dbContext.LeaverageReportv2.FromSql(sql, (Status == null ? 999 : Status), UserId, (WalletTypeId == null ? 0 : WalletTypeId)).ToList();
                }

                TotalCount = Data.Count();
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32((PageSize) * (PageNo - 1));
                    Data = Data.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public decimal FindChargeValueHold(string Timestamp, long TrnRefNo)
        {
            try
            {
                var charge = _dbContext.BalanceTotal.FromSql("SELECT ISNULL(HoldChargeAmount,0) AS TotalBalance  FROM MarginWalletTransactionqueue WHERE TimeStamp={0} AND trnrefno= {1}", Timestamp, TrnRefNo).FirstOrDefault();
                if (charge == null)
                {
                    return 0;
                }
                return charge.TotalBalance;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("FindChargeValue", "MarginWalletRepository", ex);
                throw ex;
            }
        }

        public long FindChargeValueWalletId(string Timestamp, long TrnRefNo)
        {
            try
            {
                var charge = _dbContext.ChargeWalletId.FromSql("SELECT ISNULL(Dwalletid,0) as Id FROM MarginTrnChargeLog WHERE trnno=(SELECT trnno FROM MarginWalletTransactionqueue WHERE TimeStamp={0} and trnrefno= {1})", Timestamp, TrnRefNo).FirstOrDefault();
                if (charge == null)
                {
                    return 0;
                }
                return charge.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("FindChargeValue", "MarginWalletRepository", ex);
                throw ex;
            }
        }

        public decimal FindChargeValueDeduct(string Timestamp, long TrnRefNo)
        {
            try
            {
                var charge = _dbContext.BalanceTotal.FromSql("SELECT ISNULL(DeductedChargeAmount,0) AS TotalBalance  FROM MarginWalletTransactionqueue WHERE TimeStamp={0} AND trnrefno= {1}", Timestamp, TrnRefNo).FirstOrDefault();
                if (charge == null)
                {
                    return 0;
                }
                return charge.TotalBalance;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("FindChargeValue", "MarginWalletRepository", ex);
                throw ex;
            }
        }

        public decimal FindChargeValueRelease(string Timestamp, long TrnRefNo)
        {
            try
            {
                var charge = _dbContext.BalanceTotal.FromSql("SELECT ISNULL(Charge,0) AS TotalBalance FROM MarginTrnChargeLog WHERE trnno=(SELECT trnno FROM MarginWalletTransactionqueue WHERE TimeStamp={0} and trnrefno= {1})", Timestamp, TrnRefNo).FirstOrDefault();
                if (charge == null)
                {
                    return 0;
                }
                return charge.TotalBalance;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("FindChargeValue", "MarginWalletRepository", ex);
                throw ex;
            }
        }

        #endregion

        #region Leaverage Report

        public List<LeaverageReportRes> LeveragePendingReport(long? WalletTypeId, long? UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize, ref int TotalCount)
        {
            List<LeaverageReportRes> Data = new List<LeaverageReportRes>();
            try
            {
                string sql = "SELECT  mr.MaxLeverage,cast (round((mr.Leverage),2) as decimal(28,2)) as LeveragePer,mr.Id,mr.WalletTypeID,wt.WalletTypeName,mr.FromWalletID,w.WalletName AS FromWalletName,mr.ToWalletID,(SELECT WalletName FROM MarginWalletMaster WHERE Id=mr.ToWalletID) AS ToWalletName,mr.UserID,b.UserName,b.Email,mr.LeverageID,mr.IsAutoApprove,mr.RequestRemarks,mr.Amount,mr.TrnDate,mr.LeverageAmount,mr.ChargeAmount,mr.SafetyMarginAmount,mr.CreditAmount,ISNULL(mr.SystemRemarks ,'N/A') AS SystemRemarks,mr.Status,CASE mr.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 3 THEN 'Failed' WHEN 6 THEN 'Pending' WHEN 9 THEN 'Rejected' ELSE 'N/A' END AS StrStatus FROM MarginLoanRequest mr INNER JOIN LeverageMaster l ON l.Id=mr.LeverageID INNER JOIN BizUser b on b.Id=mr.UserID INNER JOIN MarginWalletTypeMaster wt ON wt.Id=mr.WalletTypeID INNER JOIN WalletMasters w ON w.Id=mr.FromWalletID WHERE mr.Status=6 AND (mr.Userid={0} OR {0}=0) AND (mr.WalletTypeID={1} OR {1}=0) ";
                if (FromDate != null && ToDate != null)
                {
                    FromDate = Convert.ToDateTime(FromDate).AddHours(00).AddMinutes(00).AddSeconds(00);
                    ToDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);

                    sql = sql + "AND (mr.TrnDate>={2} AND mr.TrnDate<={3})";
                    Data = _dbContext.LeaverageReportRes.FromSql(sql, (UserId == null ? 0 : UserId), (WalletTypeId == null ? 0 : WalletTypeId), FromDate, ToDate).ToList();
                }
                else
                {
                    Data = _dbContext.LeaverageReportRes.FromSql(sql, (UserId == null ? 0 : UserId), (WalletTypeId == null ? 0 : WalletTypeId)).ToList();
                }

                TotalCount = Data.Count();
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32((PageSize) * (PageNo - 1));
                    Data = Data.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<LeaverageReportResv2> LeveragePendingReportv2(long? WalletTypeId, long? UserId, DateTime? FromDate, DateTime? ToDate, int PageNo, int PageSize, ref int TotalCount)
        {
            List<LeaverageReportResv2> Data = new List<LeaverageReportResv2>();
            try
            {
                string sql = "SELECT  mr.MaxLeverage,cast (round((mr.Leverage),2) as decimal(28,2)) as LeveragePer,mr.Guid as Id,mr.WalletTypeID,wt.WalletTypeName,mr.FromWalletID,w.WalletName AS FromWalletName,mr.ToWalletID,(SELECT WalletName FROM MarginWalletMaster WHERE Id=mr.ToWalletID) AS ToWalletName,mr.UserID,b.UserName,b.Email,mr.LeverageID,mr.IsAutoApprove,mr.RequestRemarks,mr.Amount,mr.TrnDate,mr.LeverageAmount,mr.ChargeAmount,mr.SafetyMarginAmount,mr.CreditAmount,ISNULL(mr.SystemRemarks ,'N/A') AS SystemRemarks,mr.Status,CASE mr.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 3 THEN 'Failed' WHEN 6 THEN 'Pending' WHEN 9 THEN 'Rejected' ELSE 'N/A' END AS StrStatus FROM MarginLoanRequest mr INNER JOIN LeverageMaster l ON l.Id=mr.LeverageID INNER JOIN BizUser b on b.Id=mr.UserID INNER JOIN MarginWalletTypeMaster wt ON wt.Id=mr.WalletTypeID INNER JOIN WalletMasters w ON w.Id=mr.FromWalletID WHERE mr.Status=6 AND (mr.Userid={0} OR {0}=0) AND (mr.WalletTypeID={1} OR {1}=0) ";
                if (FromDate != null && ToDate != null)
                {
                    FromDate = Convert.ToDateTime(FromDate).AddHours(00).AddMinutes(00).AddSeconds(00);
                    ToDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);

                    sql = sql + "AND (mr.TrnDate>={2} AND mr.TrnDate<={3})";
                    Data = _dbContext.LeaverageReportResv2.FromSql(sql, (UserId == null ? 0 : UserId), (WalletTypeId == null ? 0 : WalletTypeId), FromDate, ToDate).ToList();
                }
                else
                {
                    Data = _dbContext.LeaverageReportResv2.FromSql(sql, (UserId == null ? 0 : UserId), (WalletTypeId == null ? 0 : WalletTypeId)).ToList();
                }

                TotalCount = Data.Count();
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32((PageSize) * (PageNo - 1));
                    Data = Data.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<LeaverageReport> LeverageReport(long? WalletTypeId, long? UserId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, short? Status, ref int TotalCount)
        {
            try
            {
                string sql = "SELECT mr.MaxLeverage,ISNULL(mr.ApprovedDate,mr.TrnDate) AS ApprovedDate,mr.ApprovedBy,case ISNULL(mr.ApprovedBy,0) WHEN 0  then 'N/A' else ISNULL((SELECT Username from BizUser where id=mr.ApprovedBy),'N/A') END  AS ApprovedByUserName,cast (round((mr.Leverage),2) as decimal(28,2)) as LeveragePer,mr.Id,mr.WalletTypeID,wt.WalletTypeName,mr.FromWalletID,w.WalletName AS FromWalletName,mr.ToWalletID,(SELECT WalletName FROM MarginWalletMaster WHERE Id=mr.ToWalletID) AS ToWalletName,mr.UserID,b.UserName,b.Email,mr.LeverageID,mr.IsAutoApprove,mr.RequestRemarks,mr.Amount,mr.TrnDate,mr.LeverageAmount,mr.ChargeAmount,mr.SafetyMarginAmount,mr.CreditAmount,mr.SystemRemarks,mr.Status,CASE mr.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 3 THEN 'Failed' WHEN 6 THEN 'Pending' WHEN 9 THEN 'Rejected' ELSE 'N/A' END AS StrStatus FROM MarginLoanRequest mr INNER JOIN LeverageMaster l ON l.Id=mr.LeverageID INNER JOIN BizUser b on b.Id=mr.UserID INNER JOIN MarginWalletTypeMaster wt ON wt.Id=mr.WalletTypeID INNER JOIN WalletMasters w ON w.Id=mr.FromWalletID WHERE (mr.TrnDate>={0} AND mr.TrnDate<={1}) AND  (mr.Status={4} OR {4}=999) AND (mr.Userid={2} OR {2}=0) AND (mr.WalletTypeID={3} OR {3}=0)";

                var Data = _dbContext.LeaverageReport.FromSql(sql, FromDate, ToDate, (UserId == null ? 0 : UserId), (WalletTypeId == null ? 0 : WalletTypeId), (Status == null ? 999 : Status)).ToList();

                TotalCount = Data.Count();
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32((PageSize) * (PageNo - 1));
                    Data = Data.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<LeaverageReportv2> LeverageReportv2(long? WalletTypeId, long? UserId, DateTime FromDate, DateTime ToDate, int PageNo, int PageSize, short? Status, ref int TotalCount)
        {
            try
            {
                string sql = "SELECT mr.MaxLeverage,ISNULL(mr.ApprovedDate,mr.TrnDate) AS ApprovedDate,mr.ApprovedBy,case ISNULL(mr.ApprovedBy,0) WHEN 0  then 'N/A' else ISNULL((SELECT Username from BizUser where id=mr.ApprovedBy),'N/A') END  AS ApprovedByUserName,cast (round((mr.Leverage),2) as decimal(28,2)) as LeveragePer,mr.Guid As Id,mr.WalletTypeID,wt.WalletTypeName,mr.FromWalletID,w.WalletName AS FromWalletName,mr.ToWalletID,(SELECT WalletName FROM MarginWalletMaster WHERE Id=mr.ToWalletID) AS ToWalletName,mr.UserID,b.UserName,b.Email,mr.LeverageID,mr.IsAutoApprove,mr.RequestRemarks,mr.Amount,mr.TrnDate,mr.LeverageAmount,mr.ChargeAmount,mr.SafetyMarginAmount,mr.CreditAmount,mr.SystemRemarks,mr.Status,CASE mr.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 3 THEN 'Failed' WHEN 6 THEN 'Pending' WHEN 9 THEN 'Rejected' ELSE 'N/A' END AS StrStatus FROM MarginLoanRequest mr INNER JOIN LeverageMaster l ON l.Id=mr.LeverageID INNER JOIN BizUser b on b.Id=mr.UserID INNER JOIN MarginWalletTypeMaster wt ON wt.Id=mr.WalletTypeID INNER JOIN WalletMasters w ON w.Id=mr.FromWalletID WHERE (mr.TrnDate>={0} AND mr.TrnDate<={1}) AND  (mr.Status={4} OR {4}=999) AND (mr.Userid={2} OR {2}=0) AND (mr.WalletTypeID={3} OR {3}=0)";

                var Data = _dbContext.LeaverageReportv2.FromSql(sql, FromDate, ToDate, (UserId == null ? 0 : UserId), (WalletTypeId == null ? 0 : WalletTypeId), (Status == null ? 999 : Status)).ToList();

                TotalCount = Data.Count();
                if (PageNo > 0)
                {
                    int skip = Convert.ToInt32((PageSize) * (PageNo - 1));
                    Data = Data.Skip(skip).Take(Convert.ToInt32(PageSize)).ToList();
                }
                return Data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region LeverageMaster

        public async Task<List<LeverageRes>> ListLeverage(long? WalletTypeId, short? Status)
        {
            try
            {
                var data = _dbContext.LeverageRes.FromSql("select sl.InstantChargePer,sl.Status,case sl.Status when 0 then 'Disable' when 1 then 'Enable' else 'Deleted' end as StrStatus,sl.Id,sl.WalletTypeId,sl.LeveragePer,wt.WalletTypeName,sl.IsAutoApprove,sl.SafetyMarginPer,sl.MarginChargePer,sl.LeverageChargeDeductionType,case sl.LeverageChargeDeductionType when 0 then 'TradingToMarginWallet' when 1 then 'EndOfDay' when 2 then 'MarginWalletToTradingWallet' when 3 then 'EndOfDay_Or_MarginWalletToTradingWallet' else 'N/A' END AS LeverageChargeDeductionTypeName from LeverageMaster sl inner join MarginWalletTypeMaster wt on wt.id = sl.WalletTypeID where sl.Status<9 and (sl.WalletTypeId={0} or {0}=0) and (sl.Status={1} or {1}=999)", (WalletTypeId == null ? 0 : WalletTypeId), (Status == null ? 999 : Status)).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<List<LeverageRes>> ListLeverageBaseCurrency(long? WalletTypeId, short? Status)
        {
            try
            {
                var data = _dbContext.LeverageRes.FromSql("select sl.InstantChargePer,sl.Status,case sl.Status when 0 then 'Disable' when 1 then 'Enable' else 'Deleted' end as StrStatus,sl.Id,sl.WalletTypeId,sl.LeveragePer,wt.WalletTypeName,sl.IsAutoApprove,sl.SafetyMarginPer,sl.MarginChargePer,sl.LeverageChargeDeductionType,case sl.LeverageChargeDeductionType when 0 then 'TradingToMarginWallet' when 1 then 'EndOfDay' when 2 then 'MarginWalletToTradingWallet' when 3 then 'EndOfDay_Or_MarginWalletToTradingWallet' else 'N/A' END AS LeverageChargeDeductionTypeName from LeverageMaster sl inner join MarginWalletTypeMaster wt on wt.id = sl.WalletTypeID  inner join MarketMargin MM on MM.CurrencyName=wt.WalletTypeName where sl.Status<9 and (sl.WalletTypeId={0} or {0}=0) and (sl.Status={1} or {1}=999) and MM.Status=1", (WalletTypeId == null ? 0 : WalletTypeId), (Status == null ? 999 : Status)).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Charge

        public List<ChargesTypeWise> ListMarginChargesTypeWise(long WalletTypeId, long? TrntypeId)
        {
            List<ChargesTypeWise> Resp = new List<ChargesTypeWise>();
            try
            {
                var ChargeData = _dbContext.ChargesTypeWise.FromSql("SELECT cd.ChargeValue,wtm.WalletTypeName as DeductWalletTypeName,wt.TrnTypeName, cd.MakerCharge, cd.TakerCharge, wt.TrnTypeId FROM MarginChargeConfigurationDetail cd inner join MarginChargeConfigurationMaster cm ON cm.id = cd.ChargeConfigurationMasterID INNER JOIN MarginWTrnTypeMaster wt ON wt.TrnTypeId = cm.TrnType inner join MarginWalletTypemaster wtm on wtm.id = cd.DeductionWalletTypeId WHERE cm.TrnType in (3, 8,9) AND cd.Status = 1 AND cm.Status = 1 AND cm.WalletTypeID ={0} and(cm.TrnType ={1} or {1}= 0)", WalletTypeId, (TrntypeId == null ? 0 : Convert.ToInt64(TrntypeId))).ToList();

                return ChargeData;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public List<WalletType> GetMarginChargeWalletType(long? WalletTypeId)
        {
            //List<ChargesTypeWise> Resp = new List<ChargesTypeWise>();
            try
            {
                var _WalletType = (from w in _dbContext.MarginChargeConfigurationMaster
                                   join wt in _dbContext.MarginWalletTypeMaster
                                   on w.WalletTypeID equals wt.Id
                                   where (WalletTypeId == null || (w.WalletTypeID == WalletTypeId && WalletTypeId != null))
                                   group w by new { w.WalletTypeID, wt.WalletTypeName } into g
                                   select new WalletType { WalletTypeId = g.Key.WalletTypeID, WalletTypeName = g.Key.WalletTypeName }).ToList();

                return _WalletType;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public List<TrnChargeLogRes> MarginTrnChargeLogReport(int PageNo, int PageSize, short? Status, long? TrnTypeID, long? WalleTypeId, short? SlabType, DateTime? FromDate, DateTime? ToDate, long? TrnNo, ref long TotalCount)
        {
            try
            {
                List<TrnChargeLogRes> ChargeLogData = new List<TrnChargeLogRes>();
                string sqlQuery = "SELECT tch.Status,CASE tch.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 6 THEN 'Hold' WHEN 5 THEN 'Refunded' WHEN 9 THEN 'FAIL' ELSE 'Other' END AS StrStatus,tch.BatchNo,tch.TrnNo,tch.TrnType as TrnTypeID,wtrn.Trntypename AS TrnTypeName,tch.Amount,CONVERT(VARCHAR,tch.Amount) AS StrAmount,tch.Charge,ISNULL(tch.StakingChargeMasterID,0) AS StakingChargeMasterID,tch.ChargeConfigurationDetailID,tch.WalletTypeID,wt.wallettypename AS WalletTypeName,cd.Remarks as ChargeConfigurationDetailRemarks,tch.TimeStamp,tch.DWalletID,(SELECT Accwalletid FROM walletmasters WHERE id=tch.DWalletID) AS DAccWalletId,(SELECT Walletname FROM walletmasters WHERE id=tch.DWalletID) AS DWalletName ,tch.OWalletID,w.Walletname AS OWalletName,w.AccWalletID AS OAccWalletID,tch.DUserID,(SELECT username FROM BizUser WHERE ID=tch.DUserID) AS DUserName,(SELECT Email FROM BizUser WHERE ID=tch.DUserID) AS DEmail,tch.OUserID,ISNULL(b.UserName,'OrgUsername') AS OUserName,ISNULL(b.email,'OrgEmail') as OEmail,tch.SlabType,CASE tch.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' END AS SlabTypeName ,tch.Remarks AS TrnChargeLogRemarks ,tch.ChargeConfigurationMasterID ,ISNULL(cm.SpecialChargeConfigurationID,0) AS SpecialChargeConfigurationID ,ISNULL(s.Remarks,'N/A') AS SpecialChargeConfigurationRemarks,ISNULL(tch.Ismaker,2) as Ismaker,CASE tch.Ismaker WHEN 0 THEN 'Taker' WHEN 1 THEn 'Maker' ELSE 'N/A' END AS StrIsmaker,tch.Createddate AS Date FROM MarginTrnChargeLog tch INNER JOIN MarginWTrnTypeMaster wtrn ON wtrn.TrnTypeId=tch.trntype INNER JOIN MarginWalletTypeMaster wt ON wt.Id=tch.WalletTypeID LEFT JOIN MarginChargeConfigurationDetail cd ON cd.id=tch.ChargeConfigurationDetailID INNER JOIN MarginWalletmaster w ON w.id=tch.OWalletID LEFT JOIN BizUser b on b.id =tch.OUserID LEFT JOIN MarginChargeConfigurationMaster cm on cm.id= tch.ChargeConfigurationMasterid LEFT JOIN MarginSpecialChargeConfiguration s ON s.id =cm.SpecialChargeConfigurationID  WHERE tch.Status>0 AND (tch.Status = {0} OR {0}=999)  AND (tch.trntype={1} OR {1}=0) AND (tch.WalletTypeID={2} or {2}=0) AND (tch.slabtype={3} OR {3}=0) AND (tch.trnNo={4} OR {4}=0)";

                if (FromDate != null && ToDate != null)
                {
                    FromDate = Convert.ToDateTime(FromDate).AddHours(0).AddMinutes(0).AddSeconds(0);
                    ToDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                    sqlQuery = sqlQuery + " AND tch.CreatedDate BETWEEN {5} AND {6}";
                    ChargeLogData = _dbContext.TrnChargeLogRes.FromSql(sqlQuery, (Status == null ? 999 : Status), (TrnTypeID == null ? 0 : TrnTypeID), (WalleTypeId == null ? 0 : WalleTypeId), (SlabType == null ? 0 : SlabType), (TrnNo == null ? 0 : TrnNo), FromDate, ToDate).ToList();
                }
                else
                {
                    ChargeLogData = _dbContext.TrnChargeLogRes.FromSql(sqlQuery, (Status == null ? 999 : Status), (TrnTypeID == null ? 0 : TrnTypeID), (WalleTypeId == null ? 0 : WalleTypeId), (SlabType == null ? 0 : SlabType), (TrnNo == null ? 0 : TrnNo)).ToList();
                }
                TotalCount = ChargeLogData.Count();
                if (ChargeLogData.Count != 0)
                {
                    if (PageNo > 0)
                    {
                        int skip = PageSize * (PageNo - 1);
                        ChargeLogData = ChargeLogData.Skip(skip).Take(PageSize).ToList();
                    }
                }
                return ChargeLogData;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }
        #endregion

        #region Margin Wallet Ledger

        public List<WalletLedgerRes> GetMarginWalletLedger(DateTime FromDate, DateTime ToDate, long WalletId, int page, int PageSize, ref int TotalCount)
        {
            try
            {
                List<WalletLedgerRes> wl = (from w in _dbContext.MarginWalletLedger
                                            where w.WalletId == WalletId && w.TrnDate >= FromDate && w.TrnDate <= ToDate && w.Type == enBalanceType.AvailableBalance
                                            orderby w.Id ascending
                                            select new WalletLedgerRes
                                            {
                                                LedgerId = w.Id,
                                                PreBal = w.PreBal,
                                                PostBal = w.PreBal,
                                                Remarks = "Opening Balance",
                                                Amount = 0,
                                                CrAmount = 0,
                                                DrAmount = 0,
                                                TrnDate = w.TrnDate
                                            }).Take(1).Union((from w in _dbContext.MarginWalletLedger
                                                              where w.WalletId == WalletId && w.TrnDate >= FromDate && w.TrnDate <= ToDate && w.Type == enBalanceType.AvailableBalance
                                                              orderby w.Id ascending
                                                              select new WalletLedgerRes
                                                              {
                                                                  LedgerId = w.Id,
                                                                  PreBal = w.PreBal,
                                                                  PostBal = w.PostBal,
                                                                  Remarks = w.Remarks,
                                                                  Amount = w.CrAmt > 0 ? w.CrAmt : w.DrAmt,
                                                                  CrAmount = w.CrAmt,
                                                                  DrAmount = w.DrAmt,
                                                                  TrnDate = w.TrnDate
                                                              })).ToList();

                TotalCount = wl.Count();
                if (page > 0)
                {
                    int skip = PageSize * (page - 1);
                    wl = wl.Skip(skip).Take(PageSize).ToList();
                }
                decimal DrAmount = 0, CrAmount = 0, Amount = 0;
                wl.ForEach(e =>
                {
                    Amount = e.PreBal + e.CrAmount - e.DrAmount;
                    e.PostBal = Amount;
                    e.PreBal = e.PostBal + e.DrAmount - e.CrAmount;

                });
                return wl;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region GetPairLeverageDetail
        public LeveragePairDetail GetPairLeverageDetail(long WalletID)
        {
            try
            {
                LeveragePairDetail pairLeverageDetailResp;
                var LeveragePairDetail = _dbContext.LeveragePairDetail.FromSql("select top 1 LeverageAmount as Amount,ApprovedDate,(LeverageAmount/Amount) as Leverage,ChargeAmount as LeverageCharge,1 as IsLeverageTaken " +
                    " from MarginLoanRequest where ToWalletID={0} and Status=1 order by ApprovedDate desc", WalletID).ToList();
                if (LeveragePairDetail == null)
                {
                    pairLeverageDetailResp = new LeveragePairDetail();
                    pairLeverageDetailResp.Amount = 0;
                    pairLeverageDetailResp.ApprovedDate = null;
                    pairLeverageDetailResp.Leverage = 0;
                    pairLeverageDetailResp.IsLeverageTaken = 0;
                    pairLeverageDetailResp.LeverageCharge = 0;
                    return pairLeverageDetailResp;
                }
                else if (LeveragePairDetail.Count == 0)
                {
                    pairLeverageDetailResp = new LeveragePairDetail();
                    pairLeverageDetailResp.Amount = 0;
                    pairLeverageDetailResp.ApprovedDate = null;
                    pairLeverageDetailResp.Leverage = 0;
                    pairLeverageDetailResp.IsLeverageTaken = 0;
                    pairLeverageDetailResp.LeverageCharge = 0;
                    return pairLeverageDetailResp;
                }
                pairLeverageDetailResp = LeveragePairDetail.FirstOrDefault();
                return pairLeverageDetailResp;
            }

            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        #endregion

        public OpenPositionMaster GetPositionMasterValue(long PairID, long UserID)
        {
            try
            {
                PositionValue PositionValueResponse;
                long OpenPositionMasterID;
                OpenPositionMaster OpenPositionMasterObj = _dbContext.OpenPositionMaster.Where(x => x.PairID == PairID && x.UserID == UserID && x.Status == 0).FirstOrDefault();
                return OpenPositionMasterObj;
                //if(OpenPositionMasterObj == null)
                //{
                //    return PositionValueResponse;
                //}
                //var PositionValueMasterBuy = _dbContext.PositionValue.FromSql("select isNull(sum(BidPrice),0),isNull(sum(Qty),0),isNull(sum(LandingPrice),0) from OpenPositionDetail Where OpenPositionMasterID = {0} and TrnType = 3", ).ToList();
                //var PositionValueMasterSell = _dbContext.PositionValue.FromSql("select isNull(sum(BidPrice),0),isNull(sum(Qty),0),isNull(sum(LandingPrice),0) from OpenPositionDetail Where OpenPositionMasterID = {0} and TrnType = 8", ).ToList();            }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public OpenPositionMaster GetPositionOpenInOtherPair(long PairID, long UserID)
        {
            try
            {
                PositionValue PositionValueResponse;
                long OpenPositionMasterID;
                OpenPositionMaster OpenPositionMasterObj = _dbContext.OpenPositionMaster.Where(x => x.PairID != PairID && x.UserID == UserID && x.Status == 0).FirstOrDefault();
                return OpenPositionMasterObj;
                //if(OpenPositionMasterObj == null)
                //{
                //    return PositionValueResponse;
                //}
                //var PositionValueMasterBuy = _dbContext.PositionValue.FromSql("select isNull(sum(BidPrice),0),isNull(sum(Qty),0),isNull(sum(LandingPrice),0) from OpenPositionDetail Where OpenPositionMasterID = {0} and TrnType = 3", ).ToList();
                //var PositionValueMasterSell = _dbContext.PositionValue.FromSql("select isNull(sum(BidPrice),0),isNull(sum(Qty),0),isNull(sum(LandingPrice),0) from OpenPositionDetail Where OpenPositionMasterID = {0} and TrnType = 8", ).ToList();            }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }
        public PositionValue GetPositionDetailValue(long OpenPositionMasterID)
        {
            try
            {
                PositionValue PositionValueResponse = new PositionValue();
                //long OpenPositionMasterID;
                //OpenPositionMaster OpenPositionMasterObj = _dbContext.OpenPositionMaster.Where(x => x.PairID == PairID && x.UserID == UserID && x.Status == 0).FirstOrDefault();
                //return OpenPositionMasterObj;
                //if(OpenPositionMasterObj == null)
                //{
                //    return PositionValueResponse;
                //}
                PositionValue PositionValueMasterBuy = _dbContext.PositionValue.FromSql("select isNull(sum(BidPrice),0) as BidPrice,isNull(sum(Qty),0) as Qty,isNull(sum(LandingPrice),0) as LandingPrice,0.00 as BuyBidPrice,0.00 as  BuyLandingPrice ,0.00 as BuyQty,0.00 as SellBidPrice,0.00 as  SellLandingPrice ,0.00 as SellQty from OpenPositionDetail Where OpenPositionMasterID = {0} and TrnType = 3", OpenPositionMasterID).FirstOrDefault();
                if (PositionValueMasterBuy == null)
                {
                    PositionValueMasterBuy.BidPrice = 0;
                    PositionValueMasterBuy.LandingPrice = 0;
                    PositionValueMasterBuy.Qty = 0;
                    PositionValueMasterBuy.BuyBidPrice = 0;
                    PositionValueMasterBuy.BuyLandingPrice = 0;
                    PositionValueMasterBuy.BuyQty = 0;
                }
                else
                {
                    PositionValueResponse.BuyBidPrice = PositionValueMasterBuy.BidPrice;
                    PositionValueResponse.BuyLandingPrice = PositionValueMasterBuy.LandingPrice;
                    PositionValueResponse.BuyQty = PositionValueMasterBuy.Qty;
                }
                PositionValue PositionValueMasterSell = _dbContext.PositionValue.FromSql("select isNull(sum(BidPrice),0) as BidPrice,isNull(sum(Qty),0) as Qty,isNull(sum(LandingPrice),0) as LandingPrice,0.00 as BuyBidPrice,0.00 as  BuyLandingPrice ,0.00 as BuyQty,0.00 as SellBidPrice,0.00 as  SellLandingPrice ,0.00 as SellQty from OpenPositionDetail Where OpenPositionMasterID = {0} and TrnType = 8", OpenPositionMasterID).FirstOrDefault();
                if (PositionValueMasterSell == null)
                {
                    PositionValueMasterSell.BidPrice = 0;
                    PositionValueMasterSell.LandingPrice = 0;
                    PositionValueMasterSell.Qty = 0;
                    PositionValueMasterSell.SellBidPrice = 0;
                    PositionValueMasterSell.SellLandingPrice = 0;
                    PositionValueMasterSell.SellQty = 0;
                }
                else
                {
                    PositionValueResponse.SellBidPrice = PositionValueMasterSell.BidPrice;
                    PositionValueResponse.SellLandingPrice = PositionValueMasterSell.LandingPrice;
                    PositionValueResponse.SellQty = PositionValueMasterSell.Qty;
                }                          

                PositionValueResponse.Qty = PositionValueMasterBuy.Qty - PositionValueMasterSell.Qty;
                PositionValueResponse.LandingPrice = PositionValueMasterBuy.LandingPrice - PositionValueMasterSell.LandingPrice;
                PositionValueResponse.BidPrice = PositionValueMasterBuy.BidPrice - PositionValueMasterSell.BidPrice;

                return PositionValueResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<PNLAccount> GetProfitNLossData(long? pairId, string currencyName, long id)
        {
            try
            {
                List<PNLAccount> data = new List<PNLAccount>();
                //  List<PNLAccount> obj = new List<PNLAccount>();
                string Query = "select BU.UserName, BU.Id,PL.OpenPositionMasterID ,PL.TrnNo, PL.ProfitAmount, PL.AvgLandingBuy, PL.AvgLandingSell, PL.SettledQty, PL.ProfitCurrencyName, PL.CreatedDate " +
                                "from MarginPNLAccount PL " +
                                "inner join OpenPositionMaster PM on PM.ID = PL.OpenPositionMasterID AND (PM.UserID={0} or {0} = 0 ) " +
                                "inner join BizUser BU on PM.UserID = BU.Id " +
                                "Where (PL.ProfitCurrencyName={1} OR {1}='') AND (PM.PairID={2} OR {2}=0)AND PL.Status=1";
                data = _dbContext.PNLAccount.FromSql(Query, id, Convert.ToString(currencyName ?? ""),Convert.ToInt64(pairId==null?0:pairId)).ToList();
                if (data.Count() > 0)
                {
                    //List<OpenPositionDetailVM> Sub = new List<OpenPositionDetailVM>();
                    for (int i = 0; i < data.Count; i++)
                    {
                        List<OpenPositionDetailVM> Sub = new List<OpenPositionDetailVM>();
                        string SubQuery = "select PD.Id,PM.BaseCurrency,PD.CreatedDate AS 'TrnDate',PM.PairID,TPM.PairName,PD.CurrencyName as SecondCurrency,PD.Qty,PD.BidPrice,PD.LandingPrice,TrnNo,Case TrnType when 3 Then 'Buy' when 8 Then 'Sell' else 'Unknown' end as OrderType " +
                            "from OpenPositionDetail PD " +
                            "inner join OpenPositionMaster PM on PM.ID = PD.OpenPositionMasterID " +
                            "inner join TradePairMasterMargin TPM on TPM.ID = PM.PairID " +
                            "where OpenPositionMasterID = {0}";
                        Sub = _dbContext.OpenPositionDetailVM.FromSql(SubQuery, data[i].OpenPositionMasterID).ToList();
                        data[i].DetailedData = Sub;
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        public List<OpenPosition> GetOpenPosition(long pairId,long userid)
        {
            try
            {
                List<OpenPosition> data = new List<OpenPosition>();
                //  List<PNLAccount> obj = new List<PNLAccount>();
                string Query = "select BU.UserName, PM.UserID, PM.PairID, TPM.PairName, PM.ID as MasterID, PM.CreatedDate as TrnDate " +
                " from OpenPositionMaster PM inner  join TradePairMasterMargin TPM on TPM.ID = PM.PairID " +
                " inner join OpenPositionDetail PD on PD.OpenPositionMasterID = PM.Id " +
                " inner join BizUser BU on PM.UserID = BU.Id " +
                " where PM.Status = 0 and(PM.PairID = {0} or {0}=0) and (PM.UserID = {1} or {1}=0) group by PM.PairID, TPM.PairName, PM.CreatedDate, PM.UserID, BU.UserName, PM.ID";
                data = _dbContext.OpenPosition.FromSql(Query, pairId, userid).ToList();
                if (data.Count() > 0)
                {
                    //List<OpenPositionDetailVM> Sub = new List<OpenPositionDetailVM>();
                    for (int i = 0; i < data.Count; i++)
                    {
                        List<OpenPositionDetailVM> Sub = new List<OpenPositionDetailVM>();
                        string SubQuery = "select PD.Id,PM.BaseCurrency,PD.CreatedDate AS 'TrnDate',PM.PairID,TPM.PairName,PD.CurrencyName as SecondCurrency,PD.Qty,PD.BidPrice,PD.LandingPrice,TrnNo,Case TrnType when 3 Then 'Buy' when 8 Then 'Sell' else 'Unknown' end as OrderType " +
                            "from OpenPositionDetail PD " +
                            "inner join OpenPositionMaster PM on PM.ID = PD.OpenPositionMasterID " +
                            "inner join TradePairMasterMargin TPM on TPM.ID = PM.PairID " +
                            "where OpenPositionMasterID = {0}";
                        Sub = _dbContext.OpenPositionDetailVM.FromSql(SubQuery, data[i].MasterID).ToList();
                        data[i].DetailedData = Sub;
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        public OpenPositionMaster GetPairPositionMasterValue(long UserID)
        {
            try
            {
                PositionValue PositionValueResponse;
                long OpenPositionMasterID;
                OpenPositionMaster OpenPositionMasterObj = _dbContext.OpenPositionMaster.Where(x=> x.UserID == UserID && x.Status == 0).FirstOrDefault();
                return OpenPositionMasterObj;
                //if(OpenPositionMasterObj == null)
                //{
                //    return PositionValueResponse;
                //}
                //var PositionValueMasterBuy = _dbContext.PositionValue.FromSql("select isNull(sum(BidPrice),0),isNull(sum(Qty),0),isNull(sum(LandingPrice),0) from OpenPositionDetail Where OpenPositionMasterID = {0} and TrnType = 3", ).ToList();
                //var PositionValueMasterSell = _dbContext.PositionValue.FromSql("select isNull(sum(BidPrice),0),isNull(sum(Qty),0),isNull(sum(LandingPrice),0) from OpenPositionDetail Where OpenPositionMasterID = {0} and TrnType = 8", ).ToList();            }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //khushali 11-04-2019 Process for Release Stuck Order - wallet side   

        public enTransactionStatus CheckTransactionSuccessOrNot(long TrnRefNo)
        {
            try
            {
                IQueryable<CheckTransactionSuccessOrNotRes> Result = _dbContext.CheckTransactionSuccessOrNotRes.FromSql("SELECT Status from Marginwallettransactionqueue where trnrefno={0} and TrnType=1 and " +
                    "wallettrntype in(3,8) ", TrnRefNo);
                return Result.FirstOrDefault().Status;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //Rita 25-4-19 if settlement proceed then does not revert txn
        public bool CheckSettlementProceed(long MakerTrnNo, long TakerTrnNo)
        {
            try
            {   //ntrivedi 11-06-2019 margin wallet TQ from WalletTQ
                IQueryable<MarginWalletTransactionQueue> Result = _dbContext.MarginWalletTransactionQueue.FromSql("select * from Marginwallettransactionqueue where trnrefno={0} and " +
                                                                                "timestamp in(select timestamp from Marginwallettransactionqueue where trnrefno={1}) and status=1 and wallettrntype in(3,8)  ", TakerTrnNo, MakerTrnNo);
                var FirstEntry = Result.FirstOrDefault();

                if (FirstEntry != null)//found entry then does not revert Txn
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //komal 13-09-2019 for getSystem user balance for settledbatch transaction
        public List<WalletMasterResponsev2> GetTransactionWalletMasterResponseByCoin(long UserId, string coin)
        {
            try
            {
                //2019-2-15 added condi for only used trading wallet
                var items = _dbContext.WalletMasterResponsev2.FromSql(
                    @"select u.id, u.AccWalletID,u.ExpiryDate,ISNULL(u.OrgID,0) AS OrgID,u.Walletname as WalletName,c.WalletTypeName as CoinName,
                    u.PublicAddress,u.Balance,u.IsDefaultWallet,u.InBoundBalance,u.OutBoundBalance 
                    from MarginWalletMaster u inner join MarginWalletTypeMaster c on c.Id= u.WalletTypeID 
                    where u.Status < 9 and u.WalletUsageType=5 and  u.UserID={0} and c.WalletTypeName ={1}", UserId, coin).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


    }
}
