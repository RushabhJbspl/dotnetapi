using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities;
using Worldex.Core.Entities.NewWallet;
using Worldex.Core.Entities.User;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.BackOfficeReports;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletOperations;
using Microsoft.EntityFrameworkCore;

namespace Worldex.Infrastructure.Data
{
    public class WalletRepository : IWalletRepository
    {
        private readonly WorldexContext _dbContext;

        public WalletRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public WalletMaster GetById(long id)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                return _dbContext.Set<WalletMaster>().FirstOrDefault(e => e.Id == id && e.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TradeBitGoDelayAddresses GetUnassignedETH()
        {
            try
            {
                return _dbContext.Set<TradeBitGoDelayAddresses>().Where(e => e.GenerateBit == 1 && e.WalletId == 0).OrderBy(e => e.Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool WalletOperation(WalletLedger wl1, WalletLedger wl2, TransactionAccount ta1, TransactionAccount ta2, WalletMaster wm2, WalletMaster wm1)
        {
            try
            {
                _dbContext.Database.BeginTransaction();
                _dbContext.Set<WalletLedger>().Add(wl1);
                _dbContext.Set<WalletLedger>().Add(wl2);
                _dbContext.Set<TransactionAccount>().Add(ta1);
                _dbContext.Set<TransactionAccount>().Add(ta2);
                _dbContext.Entry(wm1).State = EntityState.Modified;
                _dbContext.Entry(wm2).State = EntityState.Modified;
                _dbContext.SaveChanges();
                _dbContext.Database.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                _dbContext.Database.RollbackTransaction();
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<WalletMasterResponse> ListWalletMasterResponse(long UserId)
        {
            try
            {
                //2019-2-15 added condi for only used trading wallet
                var items = _dbContext.WalletMasterResponse.FromSql(@"select u.AccWalletID,u.ExpiryDate,ISNULL(u.OrgID,0) AS OrgID,u.Walletname as WalletName,c.WalletTypeName as CoinName,u.PublicAddress,u.Balance,u.IsDefaultWallet,u.InBoundBalance,u.OutBoundBalance from WalletMasters u inner join WalletTypeMasters c on c.Id= u.WalletTypeID where u.Status < 9 and c.Status < 9 and Walletusagetype=0 AND u.UserID={0}", UserId).ToList(); //ntrivedi 23-04-2019  added c.status condition added
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<WalletMasterResponse> GetWalletMasterResponseByCoin(long UserId, string coin)
        {
            try
            {
                //2019-2-15 added condi for only used trading wallet
                var items = _dbContext.WalletMasterResponse.FromSql(@"select u.AccWalletID,u.ExpiryDate,ISNULL(u.OrgID,0) AS OrgID,u.Walletname as WalletName,c.WalletTypeName as CoinName,u.PublicAddress,u.Balance,u.IsDefaultWallet,u.InBoundBalance,u.OutBoundBalance from WalletMasters u inner join WalletTypeMasters c on c.Id= u.WalletTypeID where u.Status < 9 AND Walletusagetype=0 and  u.UserID={0} and c.WalletTypeName ={1}", UserId, coin).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<WalletMasterResponse> GetWalletMasterResponseById(long UserId, string coin, string walletId)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                List<WalletMasterResponse> items = (from u in _dbContext.WalletMasters
                                                    join c in _dbContext.WalletTypeMasters
                                                           on u.WalletTypeID equals c.Id
                                                    where u.Status < 9 && u.UserID == UserId && c.WalletTypeName == coin && u.AccWalletID == walletId && u.WalletUsageType == 0
                                                    select new WalletMasterResponse
                                                    {
                                                        AccWalletID = u.AccWalletID,
                                                        WalletName = u.Walletname,
                                                        CoinName = c.WalletTypeName,
                                                        PublicAddress = u.PublicAddress,
                                                        Balance = u.Balance,
                                                        ExpiryDate = u.ExpiryDate,
                                                        OrgID = Convert.ToInt64(u.OrgID == null ? 0 : u.OrgID),
                                                        IsDefaultWallet = u.IsDefaultWallet
                                                    }).AsEnumerable().ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int CheckTrnRefNo(long TrnRefNo, enWalletTranxOrderType TrnType, enWalletTrnType walletTrnType)
        {
            try
            {
                int response;
                if (walletTrnType != enWalletTrnType.Deposit)
                {
                    response = (from u in _dbContext.WalletTransactionQueues
                                where u.TrnRefNo == TrnRefNo && u.TrnType == TrnType && u.WalletTrnType == walletTrnType // ntrivedi added 09-01-2018
                                && u.WalletTrnType == walletTrnType
                                select u).Count();
                }
                else
                {
                    response = (from u in _dbContext.WalletTransactionQueues
                                where u.TrnRefNo == TrnRefNo && u.TrnType == TrnType && u.WalletTrnType == walletTrnType
                                select u).Count();
                }
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rushabh 06-12-2018
        public async Task<CheckTrnRefNoRes> CheckTranRefNoAsync(long TrnRefNo, enWalletTranxOrderType TrnType, enWalletTrnType walletTrnType)
        {
            try
            {
                CheckTrnRefNoRes response;
                string query = "SELECT COUNT(TrnNo) AS 'TotalCount' FROM WalletTransactionQueues WHERE TrnRefNo = {0} AND TrnType = {1}";
                if (walletTrnType != enWalletTrnType.Deposit)
                {
                    query += " AND WalletTrnType = {2}";
                    IQueryable<CheckTrnRefNoRes> Result = _dbContext.CheckTrnRefNoRes.FromSql(query, TrnRefNo, TrnType, walletTrnType);
                    response = Result.FirstOrDefault();
                }
                else
                {
                    IQueryable<CheckTrnRefNoRes> Result = _dbContext.CheckTrnRefNoRes.FromSql(query, TrnRefNo, TrnType);
                    response = Result.FirstOrDefault();
                }
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CheckTranRefNoAsync", this.GetType().Name, ex);
                throw ex;
            }
        }

        public int CheckTrnRefNoForCredit(long TrnRefNo, enWalletTranxOrderType TrnType) // need to check whether walleet is pre deducted for this order
        {
            try
            {
                int response = (from u in _dbContext.WalletTransactionQueues
                                where u.TrnRefNo == TrnRefNo && u.TrnType == TrnType && (u.Status == enTransactionStatus.Hold || u.Status == enTransactionStatus.Success)
                                select u).Count();
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CheckTrnRefNoForCredit", this.GetType().Name, ex);
                throw ex;
            }
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

        public WalletTransactionOrder AddIntoWalletTransactionOrder(WalletTransactionOrder wo, byte AddorUpdate)//1=add,2=update)
        {
            try
            {
                if (AddorUpdate == 1)
                {
                    _dbContext.WalletTransactionOrders.Add(wo);
                }
                else
                {
                    _dbContext.Entry(wo).State = EntityState.Modified;
                }
                _dbContext.SaveChanges();
                return wo;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool CheckarryTrnID(CreditWalletDrArryTrnID[] arryTrnID, string coinName)
        {
            try
            {
                bool i = false;
                decimal totalAmtDrTranx;
                for (int t = 0; t <= arryTrnID.Length - 1; t++)
                {
                    var response = (from u in _dbContext.WalletTransactionQueues
                                    where u.TrnRefNo == arryTrnID[t].DrTrnRefNo && u.Status == enTransactionStatus.Hold && u.TrnType == Core.Enums.enWalletTranxOrderType.Debit
                                    && u.WalletType == coinName
                                    select u);
                    if (response.Count() != 1)
                    {
                        i = false;
                        return i;
                    }
                    totalAmtDrTranx = response.ToList()[0].Amount;
                    decimal deliveredAmt = (from p in _dbContext.WalletTransactionOrders
                                            join u in _dbContext.WalletTransactionQueues on p.DTrnNo equals u.TrnNo
                                            where u.TrnRefNo == arryTrnID[t].DrTrnRefNo && u.TrnType == Core.Enums.enWalletTranxOrderType.Debit
                                            && u.WalletType == coinName && p.Status != enTransactionStatus.SystemFail
                                            select p).Sum(e => e.Amount);
                    if (!(totalAmtDrTranx - deliveredAmt - arryTrnID[t].Amount >= 0))
                    {
                        i = false;
                        return i;
                    }
                    arryTrnID[t].dWalletId = response.ToList()[0].WalletID;
                    arryTrnID[t].DrTQTrnNo = response.ToList()[0].TrnNo;

                    i = true;
                }
                return i;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<AddressMasterResponse> ListAddressMasterResponse(string AccWalletID)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                List<AddressMasterResponse> items = (from u in _dbContext.AddressMasters
                                                     join c in _dbContext.WalletMasters
                                                     on u.WalletId equals c.Id
                                                     where u.Status < 9 && c.AccWalletID == AccWalletID && u.Status == Convert.ToInt16(ServiceStatus.Active) && c.WalletUsageType == 0
                                                     select new AddressMasterResponse
                                                     {
                                                         AddressLabel = u.AddressLable,
                                                         Address = u.Address,
                                                         IsDefaultAddress = u.IsDefaultAddress,
                                                     }).AsEnumerable().ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsolanki 16-10-2018
        public WithdrawHistoryResponse DepositHistoy(DateTime FromDate, DateTime ToDate, string Coin, string TrnNo, decimal? Amount, byte? Status, long Userid, int PageNo)
        {
            List<HistoryObject> items = new List<HistoryObject>();
            //RUSHABH 11-12-2018
            try
            {
                if (ToDate < FromDate)
                {
                    return new WithdrawHistoryResponse()
                    {
                        Histories = items,
                        ReturnCode = enResponseCode.Fail,
                        ReturnMsg = EnResponseMessage.InvalidFromDate_ToDate,
                        ErrorCode = enErrorCode.InvalidFromDate_ToDate
                    };
                }
                if (TrnNo != null)
                {
                    items = _dbContext.HistoryObject.FromSql(@"SELECT D.Id AS 'TrnNo',ISNULL(D.TrnID,0) AS 'TrnId',D.SMSCode AS 'CoinName',D.Status,
                            D.StatusMsg AS 'Information',D.Amount,D.CreatedDate AS 'Date',D.Address,
                            ISNULL(D.Confirmations,0) AS 'Confirmations',
                            (CASE D.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 2 THEN 'OperatorFail' 
                            WHEN 3 THEN 'SystemFail' WHEN 4 THEN 'Hold' WHEN 5 THEN 'Refunded' ELSE 'Pending' END) AS 'StatusStr', 
                            ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink',Isnull(IsInternalTrn,2) As IsInternalTrn FROM DepositHistory D 
                            INNER JOIN ServiceMaster SM ON D.SMSCode = SM.SMSCode
                            INNER JOIN ServiceDetail SD ON SD.ServiceId = SM.Id
                            WHERE D.UserId={0} AND (D.TrnId={1} OR {1}='') 
                            ORDER BY D.CreatedDate DESC,D.ID DESC", Userid, (TrnNo == null ? "" : TrnNo)).ToList();
                }
                else
                {
                    items = _dbContext.HistoryObject.FromSql(@"SELECT D.Id AS 'TrnNo',ISNULL(D.TrnID,0) AS 'TrnId',D.SMSCode AS 'CoinName',D.Status,
                            D.StatusMsg AS 'Information',D.Amount,D.CreatedDate AS 'Date',D.Address,
                            ISNULL(D.Confirmations,0) AS 'Confirmations',
                            (CASE D.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 2 THEN 'OperatorFail' 
                            WHEN 3 THEN 'SystemFail' WHEN 4 THEN 'Hold' WHEN 5 THEN 'Refunded' ELSE 'Pending' END) AS 'StatusStr', 
                            ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink',Isnull(IsInternalTrn,2) As IsInternalTrn FROM DepositHistory D 
                            INNER JOIN ServiceMaster SM ON D.SMSCode = SM.SMSCode
                            INNER JOIN ServiceDetail SD ON SD.ServiceId = SM.Id
                            WHERE D.UserId={0} AND D.CreatedDate BETWEEN {1} AND {2} AND (D.Status={3} OR {3}=0) 
                            AND ({4}='' OR D.SMSCode={4}) AND (D.Amount={5} OR {5}=0) AND (D.TrnId={6} OR {6}='') 
                            ORDER BY D.CreatedDate DESC,D.ID DESC", Userid, FromDate, ToDate, (Status == null ? 0 : Status), (Coin == null ? "" : Coin), (Amount == null ? 0 : Amount), (TrnNo == null ? "" : TrnNo)).ToList();
                }
                if (items.Count() == 0)
                {
                    return new WithdrawHistoryResponse()
                    {
                        ReturnCode = enResponseCode.Fail,
                        ReturnMsg = EnResponseMessage.NotFound,
                        ErrorCode = enErrorCode.NotFound
                    };
                }
                if (PageNo > 0)
                {
                    int skip = Helpers.PageSize * (PageNo - 1);
                    items = items.Skip(skip).Take(Helpers.PageSize).ToList();
                }
                return new WithdrawHistoryResponse()
                {
                    ReturnCode = enResponseCode.Success,
                    ReturnMsg = EnResponseMessage.FindRecored,
                    ErrorCode = enErrorCode.Success,
                    Histories = items
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public WithdrawHistoryResponsev2 DepositHistoyv2(DateTime FromDate, DateTime ToDate, string Coin, string TrnId, decimal? Amount, byte? Status, long Userid, int PageNo)
        {
            List<HistoryObjectv2> items = new List<HistoryObjectv2>();
            //RUSHABH 11-12-2018
            try
            {
                if (TrnId != null)
                {
                    items = _dbContext.HistoryObjectv2.FromSql(@"SELECT D.GUID AS 'TrnNo',ISNULL(D.TrnID,0) AS 'TrnId',D.SMSCode AS 'CoinName',D.Status,
                            D.StatusMsg AS 'Information',D.Amount,D.CreatedDate AS 'Date',D.Address,
                            ISNULL(D.Confirmations,0) AS 'Confirmations',
                            (CASE D.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 2 THEN 'OperatorFail' 
                            WHEN 3 THEN 'SystemFail' WHEN 4 THEN 'Hold' WHEN 5 THEN 'Refunded' ELSE 'Pending' END) AS 'StatusStr', 
                            ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink',Isnull(IsInternalTrn,2) As IsInternalTrn FROM DepositHistory D 
                            INNER JOIN ServiceMaster SM ON D.SMSCode = SM.SMSCode
                            INNER JOIN ServiceDetail SD ON SD.ServiceId = SM.Id
                            WHERE D.UserId={0} AND (D.TrnId={1} OR {1}='') 
                            ORDER BY D.CreatedDate DESC,D.ID DESC", Userid, (TrnId == null ? "" : TrnId)).ToList();
                }
                else
                {
                    items = _dbContext.HistoryObjectv2.FromSql(@"SELECT D.GUID AS 'TrnNo',ISNULL(D.TrnID,0) AS 'TrnId',D.SMSCode AS 'CoinName',D.Status,
                            D.StatusMsg AS 'Information',D.Amount,D.CreatedDate AS 'Date',D.Address,
                            ISNULL(D.Confirmations,0) AS 'Confirmations',
                            (CASE D.Status WHEN 0 THEN 'Initialize' WHEN 1 THEN 'Success' WHEN 2 THEN 'OperatorFail' 
                            WHEN 3 THEN 'SystemFail' WHEN 4 THEN 'Hold' WHEN 5 THEN 'Refunded' ELSE 'Pending' END) AS 'StatusStr', 
                            ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink',Isnull(IsInternalTrn,2) As IsInternalTrn FROM DepositHistory D 
                            INNER JOIN ServiceMaster SM ON D.SMSCode = SM.SMSCode
                            INNER JOIN ServiceDetail SD ON SD.ServiceId = SM.Id
                            WHERE D.UserId={0} AND D.CreatedDate BETWEEN {1} AND {2} AND (D.Status={3} OR {3}=0) 
                            AND ({4}='' OR D.SMSCode={4}) AND (D.Amount={5} OR {5}=0) AND (D.TrnId={6} OR {6}='') 
                            ORDER BY D.CreatedDate DESC,D.ID DESC", Userid, FromDate, ToDate, (Status == null ? 0 : Status), (Coin == null ? "" : Coin), (Amount == null ? 0 : Amount), (TrnId == null ? "" : TrnId)).ToList();
                }
                if (items.Count() == 0)
                {
                    return new WithdrawHistoryResponsev2()
                    {
                        ReturnCode = enResponseCode.Fail,
                        ReturnMsg = EnResponseMessage.NotFound,
                        ErrorCode = enErrorCode.NotFound
                    };
                }
                if (PageNo > 0)
                {
                    int skip = Helpers.PageSize * (PageNo - 1);
                    items = items.Skip(skip).Take(Helpers.PageSize).ToList();
                }
                return new WithdrawHistoryResponsev2()
                {
                    ReturnCode = enResponseCode.Success,
                    ReturnMsg = EnResponseMessage.FindRecored,
                    ErrorCode = enErrorCode.Success,
                    Histories = items
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsolanki 16-10-2018
        public WithdrawHistoryNewResponse WithdrawalHistoy(DateTime FromDate, DateTime ToDate, string Coin, decimal? Amount, byte? Status, long Userid, int PageNo, short? IsInternalTransfer)
        {
            List<WithdrawHistoryObject> items = new List<WithdrawHistoryObject>();
            //RUSHABH 11-12-2018
            try
            {
                if (ToDate < FromDate)
                {
                    return new WithdrawHistoryNewResponse()
                    {
                        Histories = items,
                        ReturnCode = enResponseCode.Fail,
                        ReturnMsg = EnResponseMessage.InvalidFromDate_ToDate,
                        ErrorCode = enErrorCode.InvalidFromDate_ToDate
                    };
                }
                items = _dbContext.WithdrawHistoryObject.FromSql(@"SELECT  ISNULL( wt.WalletTypeName,'') AS ChargeCurrency,ISNULL(u.ChargeRs,0) as ChargeRs,isNull(W.TrnID,'') as 'TrnID',u.Id as 'TrnNo',u.SMSCode as 'CoinName' ,
                                u.Status,ISNULL( u.StatusMsg,'Not Found') as 'Information',ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink',
                                ISNULL(u.TransactionAccount,'Not Available') as 'Address',u.Amount,u.CreatedDate as 'Date',
                                ISNULL(w.Confirmations,0) as 'Confirmations',
                                CASE When u.Status = 4 or u.Status = 6 Then 
                                Case When u.IsVerified = 0 Then 'ConfirmationPending' When u.IsVerified = 1 Then 'Confirm' When u.IsVerified = 9 Then 'Cancelled' End
                                Else CASE  WHEN u.Status = 0 THEN 'Initialize' WHEN u.Status = 1 THEN 'Success' WHEN u.Status = 2 THEN 'ProviderFail' 
                                WHEN u.Status = 3 THEN 'SystemFail'  WHEn u.Status = 4 THEN 'Hold' WHEN u.Status = 5 And u.IsVerified = 9 THEN 'Cancelled' WHEN u.Status = 5
		                            THEN 'Refunded' WHEN u.Status = 6 THEN 'Pending' 
                                ELSE 'Other' END End AS 'StatusStr',u.IsVerified as 'IsVerified',u.EmailSendDate as 'EmailSendDate',u.IsInternalTrn FROM TransactionQueue u LEFT JOIN WithdrawHistory w ON w.id=(select max(id) from WithdrawHistory where trnno= u.Id )
                                LEFT JOIN ServiceDetail SD ON u.ServiceID = SD.ServiceId
	                            LEFT JOIN TrnChargeLog tc ON tc.TrnRefNo=u.Id and tc.Status=1 LEFT JOIN ChargeConfigurationDetail cd on cd.Id=tc.ChargeConfigurationDetailID
                                LEFT JOIN  WalletTypeMasters wt on wt.Id=cd.DeductionWalletTypeId WHERE u.TrnType = 6 and u.TrnDate >={0} and  u.TrnDate <= {1} and (u.Status={2} or {2}=0) and 
                                (u.SMSCode={3} or {3}='') and  (u.MemberID={4}) and (u.IsInternalTrn={5} or {5}=999) ORDER BY u.CreatedDate DESC,u.TrnRefNo DESC",
                                FromDate, ToDate, (Status == null ? 0 : Status), (Coin == null ? "" : Coin), Userid, (IsInternalTransfer == null ? 999 : IsInternalTransfer)).ToList();

                if (items.Count() == 0)
                {
                    return new WithdrawHistoryNewResponse()
                    {
                        ReturnCode = enResponseCode.Fail,
                        ReturnMsg = EnResponseMessage.NotFound,
                        ErrorCode = enErrorCode.NotFound
                    };
                }
                if (PageNo > 0)
                {
                    int skip = Helpers.PageSize * (PageNo - 1);
                    items = items.Skip(skip).Take(Helpers.PageSize).ToList();
                }
                return new WithdrawHistoryNewResponse()
                {
                    ReturnCode = enResponseCode.Success,
                    ReturnMsg = EnResponseMessage.FindRecored,
                    ErrorCode = enErrorCode.Success,
                    Histories = items
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public WithdrawHistoryNewResponsev2 WithdrawalHistoyv2(DateTime FromDate, DateTime ToDate, string Coin, decimal? Amount, byte? Status, long Userid, int PageNo, short? IsInternalTransfer)
        {
            try
            {
                var items = _dbContext.WithdrawHistoryObjectv2.FromSql(@"SELECT  ISNULL( wt.WalletTypeName,'') AS ChargeCurrency,ISNULL(u.ChargeRs,0) as ChargeRs,isNull(W.TrnID,'') as 'TrnID',cast(u.GUID as varchar(50)) as 'TrnNo',u.SMSCode as 'CoinName' ,
                                u.Status,ISNULL( u.StatusMsg,'Not Found') as 'Information',ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink',
                                ISNULL(u.TransactionAccount,'Not Available') as 'Address',u.Amount,u.CreatedDate as 'Date',
                                ISNULL(w.Confirmations,0) as 'Confirmations',
                                CASE When u.Status = 4 or u.Status = 6 Then 
                                Case When u.IsVerified = 0 Then 'ConfirmationPending' When u.IsVerified = 1 Then 'Confirm' When u.IsVerified = 9 Then 'Cancelled' End
                                Else CASE  WHEN u.Status = 0 THEN 'Initialize' WHEN u.Status = 1 THEN 'Success' WHEN u.Status = 2 THEN 'ProviderFail' 
                                WHEN u.Status = 3 THEN 'SystemFail'  WHEn u.Status = 4 THEN 'Hold' WHEN u.Status = 5 And u.IsVerified = 9 THEN 'Cancelled' WHEN u.Status = 5
		                            THEN 'Refunded' WHEN u.Status = 6 THEN 'Pending' 
                                ELSE 'Other' END End AS 'StatusStr',u.IsVerified as 'IsVerified',u.EmailSendDate as 'EmailSendDate',u.IsInternalTrn FROM TransactionQueue u LEFT JOIN WithdrawHistory w ON w.id=(select max(id) from WithdrawHistory where trnno= u.Id )
                                LEFT JOIN ServiceDetail SD ON u.ServiceID = SD.ServiceId
	                            LEFT JOIN TrnChargeLog tc ON tc.TrnRefNo=u.Id and tc.Status=1 LEFT JOIN ChargeConfigurationDetail cd on cd.Id=tc.ChargeConfigurationDetailID
                                LEFT JOIN  WalletTypeMasters wt on wt.Id=cd.DeductionWalletTypeId WHERE u.TrnType = 6 and u.TrnDate >={0} and  u.TrnDate <= {1} and (u.Status={2} or {2}=0) and 
                                (u.SMSCode={3} or {3}='') and  (u.MemberID={4})  and (u.IsInternalTrn={5} or {5}=999) ORDER BY u.CreatedDate DESC,u.TrnRefNo DESC",
                                FromDate, ToDate, (Status == null ? 0 : Status), (Coin == null ? "" : Coin), Userid, (IsInternalTransfer == null ? 999 : IsInternalTransfer)).ToList();

                if (items.Count() == 0)
                {
                    return new WithdrawHistoryNewResponsev2()
                    {
                        ReturnCode = enResponseCode.Fail,
                        ReturnMsg = EnResponseMessage.NotFound,
                        ErrorCode = enErrorCode.NotFound
                    };
                }
                if (PageNo > 0)
                {
                    int skip = Helpers.PageSize * (PageNo - 1);
                    items = items.Skip(skip).Take(Helpers.PageSize).ToList();
                }
                return new WithdrawHistoryNewResponsev2()
                {
                    ReturnCode = enResponseCode.Success,
                    ReturnMsg = EnResponseMessage.FindRecored,
                    ErrorCode = enErrorCode.Success,
                    Histories = items
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public bool WalletCreditwithTQ(WalletLedger wl1, TransactionAccount ta1, WalletMaster wm2, WalletTransactionQueue wtq, CreditWalletDrArryTrnID[] arryTrnID, decimal TotalAmount)
        {
            try
            {
                WalletMaster walletMasterReloaded = new WalletMaster();
                _dbContext.Database.BeginTransaction();
                var arrayObj = (from p in _dbContext.WalletTransactionOrders
                                join q in arryTrnID on p.OrderID equals q.OrderID
                                select p).ToList();
                arrayObj.ForEach(e => e.Status = enTransactionStatus.Success);
                arrayObj.ForEach(e => e.StatusMsg = "Success");
                arrayObj.ForEach(e => e.UpdatedDate = Helpers.UTC_To_IST()); // ntrivedi update updateddate

                var arrayObjTQ = (from p in _dbContext.WalletTransactionQueues
                                  join q in arryTrnID on p.TrnNo equals q.DrTQTrnNo
                                  select new { p, q }).ToList();
                arrayObjTQ.ForEach(e => e.p.SettedAmt = e.p.SettedAmt + e.q.Amount);
                arrayObjTQ.ForEach(e => e.p.UpdatedDate = Helpers.UTC_To_IST());
                arrayObjTQ.Where(d => d.p.SettedAmt >= d.p.Amount).ToList().ForEach(e => e.p.Status = enTransactionStatus.Success);
                arrayObjTQ.Where(d => d.p.SettedAmt >= d.p.Amount).ToList().ForEach(e => e.p.StatusMsg = "Success"); // ntrivedi update statusmsg
                arrayObjTQ.Where(d => d.p.SettedAmt >= d.p.Amount).ToList().ForEach(e => e.p.UpdatedDate = Helpers.UTC_To_IST()); // ntrivedi update updateddate

                walletMasterReloaded = GetById(wm2.Id);  // ntrivedi to fetch fresh balance 
                _dbContext.Entry(walletMasterReloaded).Reload();

                walletMasterReloaded.CreditBalance(TotalAmount); // credit balance here to update fresh balance
                _dbContext.Set<WalletLedger>().Add(wl1);
                _dbContext.Set<TransactionAccount>().Add(ta1);

                _dbContext.Entry(walletMasterReloaded).State = EntityState.Modified;

                _dbContext.Entry(wtq).State = EntityState.Modified;
                _dbContext.SaveChanges();
                _dbContext.Database.CommitTransaction();

                _dbContext.Entry(walletMasterReloaded).Reload();

                return true;
            }

            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                _dbContext.Database.RollbackTransaction();
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rushabh 16-10-2018
        public List<WalletLimitConfigurationRes> GetWalletLimitResponse(string AccWaletID)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                List<WalletLimitConfigurationRes> items = (from u in _dbContext.WalletLimitConfiguration
                                                           join c in _dbContext.WalletMasters
                                                           on u.WalletId equals c.Id
                                                           where u.Status < 9 && c.AccWalletID == AccWaletID && u.Status == Convert.ToInt16(ServiceStatus.Active) && c.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)
                                                           select new WalletLimitConfigurationRes
                                                           {
                                                               TrnType = u.TrnType,
                                                               LimitPerDay = u.LimitPerDay,
                                                               LimitPerHour = u.LimitPerHour,
                                                               LimitPerTransaction = u.LimitPerTransaction,
                                                               AccWalletID = c.AccWalletID,
                                                               EndTime = u.EndTimeUnix,
                                                               LifeTime = u.LifeTime != null ? u.LifeTime : 0,
                                                               StartTime = u.StartTimeUnix
                                                           }).AsEnumerable().ToList();

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AddressMasterResponse> GetAddressMasterResponse(string AccWalletID)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                List<AddressMasterResponse> items = (from u in _dbContext.AddressMasters
                                                     join c in _dbContext.WalletMasters
                                                     on u.WalletId equals c.Id
                                                     join m in _dbContext.ProviderWalletTypeMapping
                                                     on u.SerProID equals m.ServiceProviderId
                                                     where u.Status < 9 && c.AccWalletID == AccWalletID && u.IsDefaultAddress == 1 && u.Status == Convert.ToInt16(ServiceStatus.Active) && c.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet) && m.Status == 1 && m.WalletTypeId == c.WalletTypeID

                                                     select new AddressMasterResponse
                                                     {
                                                         AddressLabel = u.AddressLable,
                                                         Address = u.Address
                                                     }).AsEnumerable().ToList();
                if (items.Count() == 0)
                {
                    //2019-2-18 added condi for only used trading wallet
                    List<AddressMasterResponse> items1 = (from u in _dbContext.AddressMasters
                                                          join c in _dbContext.WalletMasters
                                                          on u.WalletId equals c.Id
                                                          join m in _dbContext.ProviderWalletTypeMapping
                           on u.SerProID equals m.ServiceProviderId
                                                          where u.Status < 9 && c.AccWalletID == AccWalletID && u.Status == Convert.ToInt16(ServiceStatus.Active) && c.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet) && m.Status == 1 && m.WalletTypeId == c.WalletTypeID
                                                          orderby u.CreatedDate descending

                                                          select new AddressMasterResponse
                                                          {
                                                              AddressLabel = u.AddressLable,
                                                              Address = u.Address,
                                                          }).AsEnumerable().Take(1).ToList();
                    return items1;
                }
                else
                {
                    return items;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsolanki 24-10-2018
        public List<BalanceResponse> GetAvailableBalance(long userid, long walletId)
        {
            try
            {
                var items = _dbContext.BalanceResponse.FromSql("select Balance,w.Id as WalletId,WalletTypeName as WalletType from WalletMasters w inner join wallettypemasters wt on wt.id = w.wallettypeid where w.Status=1 and w.userid={0} and w.WalletUsageType=0 and w.id={1}", userid, walletId).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //vsolanki 24-10-2018
        public List<BalanceResponse> GetAllAvailableBalance(long userid)
        {
            try
            {
                var items = _dbContext.BalanceResponse.FromSql("select Balance,w.Id as WalletId,WalletTypeName as WalletType from WalletMasters w inner join wallettypemasters wt on wt.id = w.wallettypeid where w.Status=1 and w.userid={0} and w.WalletUsageType=0", userid).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //convert
        public decimal NewGetTotalAvailbleBal(long userid)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var total = (from w in _dbContext.WalletMasters
                             join wt in _dbContext.WalletTypeMasters on w.WalletTypeID equals wt.Id
                             where w.UserID == userid && w.Status == Convert.ToInt16(ServiceStatus.Active) && wt.IsDefaultWallet == 1 && w.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)
                             select w.Balance
                            ).Sum();

                var t = _dbContext.BalanceTotal.FromSql("select ISNULL(cast(Round((sum(w.Balance*ts.LTP)),18) as decimal (28,18)),0) as TotalBalance from WalletMasters w inner join  wallettypemasters wt on wt.id = w.wallettypeid inner join servicemaster s on s.wallettypeid =wt.id inner join Tradepairmaster t on t.SecondaryCurrencyId=s.id inner join TradePairStastics ts on ts.PairId=t.id where userid={0}  and t.basecurrencyid=(select s.id from wallettypemasters wt inner join servicemaster s on s.wallettypeid =wt.id where wt.IsDefaultWallet=1)", userid).FirstOrDefault();
                if (t == null)
                {
                    return total;
                }
                var amt = total + t.TotalBalance;
                return amt;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsolanki 24-10-2018
        public List<BalanceResponse> GetUnSettledBalance(long userid, long walletid)
        {
            try
            {
                var result = (from w in _dbContext.WalletTransactionQueues
                              where w.WalletID == walletid && w.MemberID == userid && w.Status == enTransactionStatus.Hold || w.Status == enTransactionStatus.Pending
                              group w by new { w.WalletType } into g
                              select new BalanceResponse
                              {
                                  Balance = g.Sum(order => order.Amount),
                                  WalletType = g.Key.WalletType,
                                  WalletId = walletid
                              }).AsEnumerable().ToList();

                return result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //vsolanki 24-10-2018
        public List<BalanceResponse> GetAllUnSettledBalance(long userid)
        {
            try
            {
                var result = (from w in _dbContext.WalletTransactionQueues
                              where w.MemberID == userid && w.Status == enTransactionStatus.Hold || w.Status == enTransactionStatus.Pending
                              group w by new { w.WalletType, w.WalletID } into g
                              select new BalanceResponse
                              {
                                  Balance = g.Sum(order => order.Amount),
                                  WalletType = g.Key.WalletType,
                                  WalletId = g.Key.WalletID
                              }).AsEnumerable().ToList();
                return result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //vsolanki 24-10-2018
        public List<BalanceResponse> GetUnClearedBalance(long userid, long walletid)
        {
            try
            {
                var result = (from w in _dbContext.DepositHistory
                              join wt in _dbContext.AddressMasters
                              on w.Address equals wt.Address
                              where wt.WalletId == walletid && w.UserId == userid && w.Status == Convert.ToInt16(ServiceStatus.InActive)
                              select new BalanceResponse
                              {
                                  Balance = w.Amount,
                                  WalletType = w.SMSCode,
                                  WalletId = walletid
                              }).AsEnumerable().ToList();

                return result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //vsolanki 24-10-2018
        public List<BalanceResponse> GetUnAllClearedBalance(long userid)
        {
            try
            {
                var result = (from w in _dbContext.DepositHistory
                              join wt in _dbContext.AddressMasters
                              on w.Address equals wt.Address
                              where w.UserId == userid && w.Status == Convert.ToInt16(ServiceStatus.InActive)
                              select new BalanceResponse
                              {
                                  Balance = w.Amount,
                                  WalletType = w.SMSCode,
                                  WalletId = wt.WalletId
                              }).AsEnumerable().ToList();
                return result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //vsolanki 24-10-2018
        public List<StackingBalanceRes> GetStackingBalance(long userid, long walletid)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var result = (from u in _dbContext.UserStacking
                              join w in _dbContext.WalletMasters
                              on u.WalletId equals w.Id
                              where u.WalletId == walletid && w.UserID == userid && u.Status == Convert.ToInt16(ServiceStatus.InActive) && w.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)
                              select new StackingBalanceRes
                              {
                                  StackingAmount = u.StackingAmount,
                                  WalletType = u.WalletType,
                                  WalletId = walletid
                              }).AsEnumerable().ToList();

                if (result.Count() == 0)
                {
                    //2019-2-18 added condi for only used trading wallet
                    var result1 = (from u in _dbContext.StckingScheme
                                   join w in _dbContext.WalletMasters
                                  on u.WalletType equals w.WalletTypeID
                                   join wt in _dbContext.WalletTypeMasters
                                   on u.WalletType equals wt.Id
                                   where w.Id == walletid && w.UserID == userid && u.Status == Convert.ToInt16(ServiceStatus.InActive) && w.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)
                                   select new StackingBalanceRes
                                   {
                                       MaxLimitAmount = u.MaxLimitAmount,
                                       MinLimitAmount = u.MinLimitAmount,
                                       WalletType = wt.WalletTypeName,
                                       WalletId = walletid
                                   }).AsEnumerable().ToList();
                    return result1;
                }

                return result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //vsolanki 24-10-2018
        public List<StackingBalanceRes> GetAllStackingBalance(long userid)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var result = (from u in _dbContext.UserStacking
                              join w in _dbContext.WalletMasters
                              on u.WalletId equals w.Id
                              where w.UserID == userid && u.Status == Convert.ToInt16(ServiceStatus.InActive) && w.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)
                              select new StackingBalanceRes
                              {
                                  StackingAmount = u.StackingAmount,
                                  WalletType = u.WalletType,
                                  WalletId = w.Id
                              }).AsEnumerable().ToList();

                if (result.Count() == 0)
                {
                    //2019-2-18 added condi for only used trading wallet
                    var result1 = (from u in _dbContext.StckingScheme
                                   join w in _dbContext.WalletMasters
                                  on u.WalletType equals w.WalletTypeID
                                   join wt in _dbContext.WalletTypeMasters
                                   on u.WalletType equals wt.Id
                                   where w.UserID == userid && u.Status == Convert.ToInt16(ServiceStatus.InActive) && w.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)
                                   select new StackingBalanceRes
                                   {
                                       MaxLimitAmount = u.MaxLimitAmount,
                                       MinLimitAmount = u.MinLimitAmount,
                                       WalletType = wt.WalletTypeName,
                                       WalletId = w.Id
                                   }).AsEnumerable().ToList();
                    return result1;
                }
                return result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //vsolanki 24-10-2018
        public List<BalanceResponse> GetShadowBalance(long userid, long walletid)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var result = (from u in _dbContext.MemberShadowBalance
                              join w in _dbContext.WalletMasters
                              on u.WalletID equals w.Id
                              join wt in _dbContext.WalletTypeMasters
                              on u.WalletTypeId equals wt.Id
                              where u.WalletID == walletid && w.UserID == userid && u.Status == Convert.ToInt16(ServiceStatus.InActive) && w.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)
                              select new BalanceResponse
                              {
                                  Balance = u.ShadowAmount,
                                  WalletType = wt.WalletTypeName,
                                  WalletId = walletid
                              }).AsEnumerable().ToList();

                if (result.Count() == 0)
                {
                    //2019-2-18 added condi for only used trading wallet
                    var result1 = (from u in _dbContext.MemberShadowLimit
                                   join w in _dbContext.BizUserTypeMapping
                                   on u.MemberTypeId equals w.UserType
                                   join wt in _dbContext.WalletMasters
                                   on walletid equals wt.Id
                                   join wtm in _dbContext.WalletTypeMasters
                                                      on wt.WalletTypeID equals wtm.Id
                                   where u.WalletType == wt.WalletTypeID && w.UserID == userid && u.Status == Convert.ToInt16(ServiceStatus.InActive) && wt.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)
                                   select new BalanceResponse
                                   {
                                       Balance = u.ShadowLimitAmount,
                                       WalletType = wtm.WalletTypeName,
                                       WalletId = walletid
                                   }).AsEnumerable().ToList();
                    return result1;
                }

                return result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //vsolanki 24-10-2018
        public List<BalanceResponse> GetAllShadowBalance(long userid)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var result = (from u in _dbContext.MemberShadowBalance
                              join w in _dbContext.WalletMasters
                              on u.WalletID equals w.Id
                              join wt in _dbContext.WalletTypeMasters
                                                       on u.WalletTypeId equals wt.Id
                              where w.UserID == userid && u.Status == Convert.ToInt16(ServiceStatus.InActive) && w.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)
                              select new BalanceResponse
                              {
                                  Balance = u.ShadowAmount,
                                  WalletType = wt.WalletTypeName,
                                  WalletId = w.Id
                              }).AsEnumerable().ToList();

                if (result.Count() == 0)
                {
                    //2019-2-18 added condi for only used trading wallet
                    var result1 = (from u in _dbContext.MemberShadowLimit
                                   join w in _dbContext.BizUserTypeMapping
                                   on u.MemberTypeId equals w.UserType
                                   join wt in _dbContext.WalletMasters
                                   on u.WalletType equals wt.WalletTypeID
                                   join wtm in _dbContext.WalletTypeMasters
                                   on wt.WalletTypeID equals wtm.Id
                                   where u.WalletType == wt.WalletTypeID && w.UserID == userid && u.Status == Convert.ToInt16(ServiceStatus.InActive) && wt.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)
                                   select new BalanceResponse
                                   {
                                       Balance = u.ShadowLimitAmount,
                                       WalletType = wtm.WalletTypeName,
                                       WalletId = wt.Id
                                   }).AsEnumerable().ToList();
                    return result1;
                }

                return result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //vsolanki 24-10-2018
        public Balance GetAllBalances(long userid, long walletid)
        {
            try
            {
                var items = _dbContext.Balance.FromSql(@"select ISNULL((select sum(Amount) from WalletTransactionQueues where WalletID ={0} AND MemberID = {1} AND (Status=4 or Status=6)),0 )as UnSettledBalance ,ISnull((select sum(w.Balance) from  WalletMasters w inner join WalletTypeMasters wt on wt.Id=w.WalletTypeId where w.Id = {0} and w.UserID = {1} and w.Status =1),0) as AvailableBalance,ISNULL((select sum(w.Amount) from DepositHistory w inner join AddressMasters wt on wt.Address=w.Address where w.Id = {0} and w.UserID = {1} and w.Status =0),0 )as UnClearedBalance,ISNULL((select sum(u.ShadowAmount) from MemberShadowBalance u inner join WalletMasters w on w.Id=u.WalletID inner join WalletTypeMasters wt on wt.Id= u.WalletTypeId where w.Id = {0} and w.UserID = {1} and w.Status =0),0) as ShadowBalance,ISNULL((select SUM(StakingAmount) from TokenStakingHistory where WalletID={0} and UserId={1} And Status in (1,4)),0) as StackingBalance", walletid, userid).First();

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsolanki 25-10-2018
        public List<BalanceResponseLimit> GetAvailbleBalTypeWise(long userid)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var result = _dbContext.BalanceResponseLimit.FromSql(@"select wt.WalletTypeName as WalletType,ISNULL(SUM(w.Balance),0) as Balance from WalletTypeMasters wt left join WalletMasters w on w.WalletTypeID=wt.Id and w.UserID={0} and w.status=1 and w.WalletUsageType=0 where wt.Status=1  group by wt.WalletTypeName", userid).ToList();

                return result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<BeneficiaryMasterRes1> GetAllWhitelistedBeneficiaries(long WalletTypeID, long UserID, short? IsInternalAddress, long WalletId)
        {
            try
            {
                List<BeneficiaryMasterRes1> Resp = new List<BeneficiaryMasterRes1>();
                //string query = "SELECT B.Name,B.Id AS 'BeneficiaryID',B.Address,B.Status FROM BeneficiaryMaster B WHERE B.Status < 9 AND B.UserID = {0} AND B.WalletTypeID = {1} AND B.IsWhiteListed = 1 ORDER BY B.CreatedDate DESC";
                //2019-7-22 added IsInternal address bit in request / response param
                //string query = "SELECT CAST(CASE WHEN (AM.Id) > 0 THEN 1 ELSE 2 END AS SMALLINT) AS IsInternalAddress ,B.Name,B.Id AS 'BeneficiaryID',B.Address,B.Status FROM BeneficiaryMaster B LEFT Join AddressMasters AM On AM.Address=B.Address AND AM.Status=1 and walletid={2} LEFT JOIN WalletMasters W ON W.Id=AM.WalletId  WHERE B.Status < 9 AND B.UserID = {0} AND B.WalletTypeID = {1} AND B.IsWhiteListed = 1 ORDER BY B.CreatedDate DESC";
                string query = "SELECT distinct CAST(CASE WHEN (AM.Id) > 0 THEN 1 ELSE 2 END AS SMALLINT) AS IsInternalAddress ,B.Name,B.Id AS 'BeneficiaryID',B.Address,B.Status FROM BeneficiaryMaster B LEFT Join AddressMasters AM On AM.Address=B.Address AND AM.Status=1 LEFT JOIN WalletMasters W ON W.Id=AM.WalletId WHERE B.Status < 9 AND B.UserID = {0} AND B.WalletTypeID = {1} AND B.IsWhiteListed = 1";

                IQueryable<BeneficiaryMasterRes1> Result = _dbContext.BeneficiaryMasterRes1.FromSql(query, UserID, WalletTypeID, WalletId);
                Resp = Result.ToList();
                if (IsInternalAddress != null && IsInternalAddress != 0)
                {
                    Resp = Resp.FindAll(i => i.IsInternalAddress == IsInternalAddress);
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<BeneficiaryMasterRes> GetAllBeneficiaries(long UserID)
        {
            try
            {
                List<BeneficiaryMasterRes> Resp = new List<BeneficiaryMasterRes>();
                string query = "SELECT B.Name,B.Id AS 'BeneficiaryID',B.Address,W.WalletTypeName AS 'CoinName',B.IsWhiteListed,B.Status FROM BeneficiaryMaster B INNER JOIN WalletTypeMasters W ON B.WalletTypeID = W.Id WHERE B.UserID = {0} AND B.Status < 9 ORDER BY B.CreatedDate DESC";
                IQueryable<BeneficiaryMasterRes> Result = _dbContext.BeneficiaryMasterRes.FromSql(query, UserID);
                Resp = Result.ToList();
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BeneUpdate BeneficiaryBulkEdit(string id, short bit)
        {
            try
            {
                BeneUpdate res = new BeneUpdate();
                string Query = "UPDATE BeneficiaryMaster SET IsWhiteListed = {0}";
                if (bit == 9)
                {
                    Query += ", Status=9";
                }
                Query += " WHERE ID IN(" + id + ") SELECT @@ROWCOUNT as 'AffectedRows'";
                IQueryable<BeneUpdate> Result = _dbContext.BeneUpdate.FromSql(Query, bit);
                res = Result.FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public DateTime UTC_To_IST(DateTime dateTime)
        {
            try
            {
                DateTime istdate = TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                return istdate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal GetTodayAmountOfTQ(long userId, long WalletId)
        {
            try
            {
                DateTime startDateTime = UTC_To_IST(DateTime.UtcNow); //Today at 12:00:00
                DateTime endDateTime = UTC_To_IST(DateTime.UtcNow.AddDays(-1).AddTicks(-1));

                var total = _dbContext.BalanceTotal.FromSql("select isnull(sum(case Status when 4 then OrderTotalQty else SettledSellQty end) ,0) as TotalBalance from TradeTransactionQueue where TrnDate <= {0} AND TrnDate >= {1} and OrderWalletID ={2} and status in(1, 4)", startDateTime.Date, endDateTime.Date, WalletId).FirstOrDefault();
                if (total == null)
                {
                    return 0;
                }
                return total.TotalBalance;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsoalnki 26-10-2018
        public List<WalletLedgerRes> GetWalletLedger(DateTime FromDate, DateTime ToDate, long WalletId, int page)
        {
            try
            {
                List<WalletLedgerRes> wl = (from w in _dbContext.WalletLedgers
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
                                            }).Take(1).Union((from w in _dbContext.WalletLedgers
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

                if (page > 0)
                {
                    int skip = Helpers.PageSize * (page - 1);
                    wl = wl.Skip(skip).Take(Helpers.PageSize).ToList();
                }
                decimal DrAmount = 0, CrAmount = 0, Amount = 0;
                return wl;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsoalnki 26-10-2018
        public List<WalletLedgerRes> GetWalletLedgerV1(DateTime FromDate, DateTime ToDate, long WalletId, int page, int PageSize, ref int TotalCount)
        {
            try
            {
                List<WalletLedgerRes> wl = (from w in _dbContext.WalletLedgers
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
                                            }).Take(1).Union((from w in _dbContext.WalletLedgers
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
                decimal Amount = 0;
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

        //vsolanki 2018-10-27
        public async Task<int> CreateDefaulWallet(long UserId)
        {
            try
            {
                //Craete wallet
                var WalletTypeObj = (from p in _dbContext.WalletTypeMasters
                                     where p.Status == Convert.ToInt16(ServiceStatus.Active)
                                     select p).ToList();

                var Wallets = from WalletTypearray in WalletTypeObj
                              select new WalletMaster
                              {
                                  CreatedBy = UserId,
                                  CreatedDate = Helpers.UTC_To_IST(),
                                  Status = Convert.ToInt16(ServiceStatus.Active),
                                  UpdatedDate = Helpers.UTC_To_IST(),
                                  Balance = 0,
                                  WalletTypeID = WalletTypearray.Id,
                                  UserID = UserId,
                                  Walletname = WalletTypearray.WalletTypeName + " DefaultWallet",
                                  AccWalletID = RandomGenerateWalletId(UserId, 1),
                                  IsDefaultWallet = 1,
                                  IsValid = true,
                                  PublicAddress = "",
                                  OrgID = 1
                              };
                _dbContext.WalletMasters.AddRange(Wallets);
                _dbContext.SaveChanges();

                var walletObj = (from wm in _dbContext.WalletMasters
                                 where wm.UserID == UserId && wm.IsDefaultWallet == 1
                                 select wm).ToList();


                var authObj = from ww in walletObj
                              select new WalletAuthorizeUserMaster
                              {
                                  RoleID = 1,
                                  UserID = UserId,
                                  Status = 1,
                                  CreatedBy = UserId,
                                  CreatedDate = Helpers.UTC_To_IST(),
                                  UpdatedDate = Helpers.UTC_To_IST(),
                                  WalletID = ww.Id,
                                  OrgID = Convert.ToInt64(ww.OrgID),
                              };
                _dbContext.WalletAuthorizeUserMaster.AddRange(authObj);
                _dbContext.SaveChanges();

                return 1;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CreateDefaulWallet", this.GetType().Name, ex);
                return 0;
            }
        }

        private static Random random = new Random((int)DateTime.Now.Ticks);

        public string RandomGenerateWalletId(long userID, byte isDefaultWallet)
        {
            try
            {
                long maxValue = 999999999;
                long minValue = 100000000;
                long x = (long)Math.Round(random.NextDouble() * (maxValue - minValue - 1)) + minValue;
                string userIDStr = x.ToString() + userID.ToString().PadLeft(6, '0') + isDefaultWallet.ToString();
                return userIDStr;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int CreateWalletForAllUser_NewService(string WalletType)
        {
            try
            {
                var WalletTypeObj = (from p in _dbContext.WalletTypeMasters
                                     where p.Status == Convert.ToInt16(ServiceStatus.Active) && p.WalletTypeName == WalletType
                                     select p).FirstOrDefault();

                var Users = (from s in _dbContext.Users
                             where !_dbContext.WalletMasters.Any(es => (es.UserID == s.Id) && (es.WalletTypeID == WalletTypeObj.Id) && (es.IsDefaultWallet == 1))
                             select s).ToList();
                var Wallets = from U in Users
                              select new WalletMaster
                              {
                                  CreatedBy = U.Id,
                                  CreatedDate = Helpers.UTC_To_IST(),
                                  Status = Convert.ToInt16(ServiceStatus.Active),
                                  UpdatedDate = Helpers.UTC_To_IST(),
                                  Balance = 0,
                                  WalletTypeID = WalletTypeObj.Id,
                                  UserID = U.Id,
                                  Walletname = WalletTypeObj.WalletTypeName + " DefaultWallet",
                                  AccWalletID = RandomGenerateWalletId(U.Id, 1),
                                  IsDefaultWallet = 1,
                                  IsValid = true,
                                  PublicAddress = ""
                              };
                _dbContext.WalletMasters.AddRange(Wallets);
                _dbContext.SaveChanges();

                var walletObj = (from wm in _dbContext.WalletMasters
                                 join U in Users on wm.UserID equals U.Id
                                 where wm.UserID == U.Id && wm.IsDefaultWallet == 1 && wm.WalletTypeID == WalletTypeObj.Id
                                 select wm).ToList();

                var authObj = from ww in walletObj
                              select new WalletAuthorizeUserMaster
                              {
                                  RoleID = 1,
                                  UserID = ww.UserID,
                                  Status = 1,
                                  CreatedBy = ww.UserID,
                                  CreatedDate = Helpers.UTC_To_IST(),
                                  UpdatedDate = Helpers.UTC_To_IST(),
                                  WalletID = ww.Id,
                                  OrgID = Convert.ToInt64(ww.OrgID),
                              };
                _dbContext.WalletAuthorizeUserMaster.AddRange(authObj);
                _dbContext.SaveChanges();

                return 1;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int AddBizUserTypeMapping(BizUserTypeMapping bizUser)
        {
            try
            {
                var UserTypeMap = _dbContext.BizUserTypeMapping.Add(bizUser);
                _dbContext.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return 0;
            }
        }

        //vsolanki 2018-10-29
        public List<IncomingTrnRes> GetIncomingTransaction(long Userid, string Coin)
        {
            if (Coin == null)
            {
                Coin = "";
            }
            try
            {
                var test = _dbContext.IncomingTrnRes.FromSql(@"SELECT  trn.Id as TrnNo,trn.CreatedDate as Date,ROW_NUMBER() OVER (ORDER BY trn.Id ) AS AutoNo,trn.TrnID,trn.SMSCode AS WalletType,trn.Confirmations,trn.Amount,trn.Address,wt.ConfirmationCount,ISNULL(JSON_QUERY(CAST(sd.ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]')AS ExplorerLink FROM DepositHistory trn INNER JOIN WalletTypeMasters wt ON wt.WalletTypeName=trn.SMSCode INNER JOIN ServiceMaster   s ON s.SMSCode=trn.SMSCode INNER JOIN ServiceDetail sd ON sd.ServiceId=s.Id WHERE trn.Status = 0 and trn.UserId = {0} and ({1} ='' or trn.SMSCode={1})", Userid, Coin).ToList();

                return test;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<IncomingTrnResv2> GetIncomingTransactionv2(long Userid, string Coin)
        {
            if (Coin == null)
            {
                Coin = "";
            }
            try
            {
                var test = _dbContext.IncomingTrnResv2.FromSql(@"SELECT  trn.GUID as TrnNo,trn.CreatedDate as Date,ROW_NUMBER() OVER (ORDER BY trn.Id ) AS AutoNo,trn.TrnID,trn.SMSCode AS WalletType,trn.Confirmations,trn.Amount,trn.Address,wt.ConfirmationCount,ISNULL(JSON_QUERY(CAST(sd.ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]')AS ExplorerLink FROM DepositHistory trn INNER JOIN WalletTypeMasters wt ON wt.WalletTypeName=trn.SMSCode INNER JOIN ServiceMaster   s ON s.SMSCode=trn.SMSCode INNER JOIN ServiceDetail sd ON sd.ServiceId=s.Id WHERE trn.Status = 0 and trn.UserId = {0} and ({1} ='' or trn.SMSCode={1})", Userid, Coin).ToList();

                return test;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

        }
        public long getOrgID()
        {
            try
            {
                var orgObj = _dbContext.BizUserTypeMapping.Where(u => u.UserType == 0).FirstOrDefault();
                if (orgObj == null)
                {
                    return 0;
                }
                else
                {
                    return orgObj.UserID;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public WalletTransactionQueue GetTransactionQueue(long TrnNo)
        {
            try
            {
                WalletTransactionQueue tq = _dbContext.WalletTransactionQueues.Where(u => u.TrnNo == TrnNo).SingleOrDefault();
                return tq;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public long GetTypeMappingObj(long userid)
        {
            try
            {
                var UserTypeObj = _dbContext.BizUserTypeMapping.Where(u => u.UserID == userid).SingleOrDefault();
                if (UserTypeObj == null)
                {
                    return -1; //ntrivedi usertype can be 0
                }
                else
                {
                    return UserTypeObj.UserType;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<long> GetTypeMappingObjAsync(long userid)
        {
            try
            {
                Task<BizUserTypeMapping> UserTypeObj1 = _dbContext.BizUserTypeMapping.Where(u => u.UserID == userid).FirstOrDefaultAsync();
                BizUserTypeMapping UserTypeObj = await UserTypeObj1;
                if (UserTypeObj == null)
                {
                    return -1; //ntrivedi usertype can be 0
                }
                else
                {
                    return UserTypeObj.UserType;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTypeMappingObjAsync", this.GetType().Name, ex);
                throw ex;
            }
        }

        public decimal GetLedgerLastPostBal(long walletId)
        {
            try
            {
                var ledgers = (from ledger in _dbContext.WalletLedgers
                               where ledger.WalletId == walletId
                               orderby ledger.TrnDate descending
                               select ledger).Take(1).First();
                if (ledgers != null)
                {
                    var bal = ledgers.PostBal;
                    return bal;
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rushabh 2018-12-04
        public List<OutgoingTrnRes> GetOutGoingTransaction(long Userid, string Coin)
        {
            try
            {
                List<OutgoingTrnRes> res = new List<OutgoingTrnRes>();
                string str = "SELECT WH.TrnNo as TrnNo,WH.CreatedDate as Date,ROW_NUMBER() OVER (ORDER BY WH.ID) AS 'AutoNo',WH.TrnID,WH.SMSCode AS 'WalletType',WH.Confirmations,WH.Amount,WH.Address,WT.ConfirmationCount,ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink' FROM WithdrawHistory WH INNER JOIN WalletTypeMasters WT ON WH.SMSCode = WT.WalletTypeName INNER JOIN ServiceMaster SM ON WH.SMSCode = SM.SMSCode INNER JOIN ServiceDetail SD ON SM.Id = SD.ServiceId WHERE WH.UserId = {0} AND WH.Status = {1}";
                if (Coin != null && Coin != "")
                {
                    str += " AND WH.SMSCode={2}";
                }
                IQueryable<OutgoingTrnRes> Result = _dbContext.OutgoingTrnRes.FromSql(str, Userid, Convert.ToInt16(enTransactionStatus.Pending), Coin);
                res = Result.ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<OutgoingTrnResv2> GetOutGoingTransactionv2(long Userid, string Coin)
        {
            try
            {
                List<OutgoingTrnResv2> res = new List<OutgoingTrnResv2>();
                string str = "SELECT cast(u.GUID as varchar(50)) as TrnNo,WH.CreatedDate as Date,ROW_NUMBER() OVER (ORDER BY WH.ID) AS 'AutoNo',WH.TrnID,WH.SMSCode AS 'WalletType',WH.Confirmations,WH.Amount,WH.Address,WT.ConfirmationCount,ISNULL(JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer'),'[]') AS 'ExplorerLink' FROM WithdrawHistory WH INNER JOIN TransactionQueue u ON u.Id= WH.TrnNo INNER JOIN WalletTypeMasters WT ON WH.SMSCode = WT.WalletTypeName INNER JOIN ServiceMaster SM ON WH.SMSCode = SM.SMSCode INNER JOIN ServiceDetail SD ON SM.Id = SD.ServiceId WHERE WH.UserId = {0} AND WH.Status = {1}";
                if (Coin != null && Coin != "")
                {
                    str += " AND WH.SMSCode={2}";
                }
                IQueryable<OutgoingTrnResv2> Result = _dbContext.OutgoingTrnResv2.FromSql(str, Userid, Convert.ToInt16(enTransactionStatus.Pending), Coin);
                res = Result.ToList();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //vsolanki 2018-11-02
        public List<TransfersRes> GetTransferIn(string Coin, int Page, int PageSize, long? UserId, string Address, string TrnID, long? OrgId, ref int TotalCount)
        {
            try
            {
                List<TransfersRes> trns = new List<TransfersRes>();
                trns = _dbContext.TransfersRes.FromSql(@"SELECT  o.Id as OrgId,o.OrganizationName,ROW_NUMBER() OVER (ORDER BY trn.Id ) AS AutoNo,trn.TrnID,trn.SMSCode AS WalletType,  JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer') AS 'ExplorerLink',trn.Confirmations,trn.Amount,trn.Address,wt.ConfirmationCount,trn.ConfirmedTime,u.id as UserId,u.UserName AS 'UserName',ISNULL(u.Email,'') as Email,cast(trn.Amount as varchar) as StrAmount  FROM DepositHistory trn INNER JOIN WalletTypeMasters wt ON wt.WalletTypeName=trn.SMSCode INNER JOIN ServiceMaster   s ON s.SMSCode=trn.SMSCode INNER JOIN ServiceDetail sd ON sd.ServiceId=s.Id INNER JOIN BizUser u ON u.Id= trn.UserId inner join WalletMasters wu on wu.UserID=trn.UserId and wt.id=wu.wallettypeid inner join Organizationmaster o on o.id=wu.OrgID  WHERE (trn.Status = 0) AND  (trn.SMSCode={1})  AND (trn.UserId={2} or {2}=0) and (trn.Address={3} or {3}='') and  (trn.TrnID={4} or {4}='') and  (wu.OrgId={0} or {0}=0)", (OrgId == null ? 0 : OrgId), Coin, (UserId == null ? 0 : UserId), (Address == null ? "" : Address), (TrnID == null ? "" : TrnID)).ToList();
                TotalCount = trns.Count();
                if (Page > 0)
                {
                    int skip = PageSize * (Page - 1);
                    trns = trns.Skip(skip).Take(PageSize).ToList();
                }
                return trns;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //RUSHABH 04-12-2018
        public List<TransfersRes> TransferOutHistory(string CoinName, int Page, int PageSize, long? UserId, string Address, string TrnID, long? OrgId, ref int TotalCount)
        {
            try
            {
                List<TransfersRes> History = new List<TransfersRes>();
                string str = "SELECT o.Id as OrgId,o.OrganizationName,ROW_NUMBER() OVER (ORDER BY WH.ID) AS 'AutoNo',WH.TrnID,WH.SMSCode AS 'WalletType', WH.Confirmations,JSON_QUERY(CAST(ServiceDetailJson as varchar(8000)), '$.Explorer') AS 'ExplorerLink',WH.confirmedTime,WH.Amount,cast(WH.Amount as varchar) as StrAmount,WH.Address,WT.ConfirmationCount,U.UserName AS 'UserName',U.id as UserId,ISNULL(u.Email,'') as Email FROM WithdrawHistory WH INNER JOIN WalletTypeMasters WT ON  WT.WalletTypeName=WH.SMSCode  INNER JOIN BizUser U ON  U.Id=WH.UserId inner join WalletMasters wu on wu.UserID=WH.UserId AND WT.Id=wu.WalletTypeId  INNER JOIN ServiceMaster   s ON s.SMSCode=WH.SMSCode INNER JOIN ServiceDetail sd ON sd.ServiceId=s.Id inner join Organizationmaster o on o.id=wu.OrgID WHERE WH.SMSCode = {0} AND WH.Status = 6 AND (WH.UserId={2} or {2}=0) and (WH.Address={3} or {3}='') and  (WH.TrnID={4} or {4}='') and (wu.OrgId={1} or {1}=0) ";

                History = _dbContext.TransfersRes.FromSql(str, CoinName, (OrgId == null ? 0 : OrgId), (UserId == null ? 0 : UserId), (Address == null ? "" : Address), (TrnID == null ? "" : TrnID)).ToList();
                TotalCount = History.Count();
                if (Page > 0)
                {
                    int skip = PageSize * (Page - 1);
                    History = History.Skip(skip).Take(PageSize).ToList();
                }
                return History;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<bool> CheckTrnIDDrForHoldAsync(CommonClassCrDr arryTrnID)
        {
            try
            {
                var response = (from u in _dbContext.WalletTransactionQueues
                                where u.TrnRefNo == arryTrnID.debitObject.TrnRefNo && (u.Status == enTransactionStatus.Initialize || u.Status == enTransactionStatus.Hold)
                                && u.TrnType == Core.Enums.enWalletTranxOrderType.Debit
                                && u.Amount - u.SettedAmt >= arryTrnID.Amount
                                select new TempEntity { TrnNo = u.TrnNo, SetteledAmount = u.SettedAmt, Amount = u.Amount }).ToList();
                if (response.Count != 1)
                {
                    return false;
                }
                arryTrnID.debitObject.WTQTrnNo = response[0].TrnNo;

                var deliveredAmt = (from p in _dbContext.WalletTransactionOrders
                                    where p.DTrnNo == arryTrnID.debitObject.WTQTrnNo && p.Status != enTransactionStatus.SystemFail
                                    select p.Amount).Sum();

                if (!(response[0].Amount - deliveredAmt - arryTrnID.Amount >= 0))
                {
                    //i = false;
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

        public void ReloadEntity(WalletMaster wm1, WalletMaster wm2, WalletMaster wm3, WalletMaster wm4)
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
            }
        }

        public bool CheckUserBalanceV1(long WalletId, enBalanceType enBalance = enBalanceType.AvailableBalance, EnWalletUsageType enWalletUsageType = EnWalletUsageType.Trading_Wallet)
        {
            try
            {
                decimal wObjBal;

                //2019-2-18 added condi for only used trading wallet
                WalletMaster walletObject = (from w in _dbContext.WalletMasters
                                             where w.Id == WalletId && w.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)
                                             select w).First();
                IQueryable<SumAmount> Result1 = _dbContext.SumAmounts.FromSql(@"SELECT ISNULL((SUM(CrAmt)  - SUM(DrAmt)),0) AS 'DifferenceAmount'  FROM TransactionAccounts WHERE WalletID = {0} AND IsSettled = 1 AND Type = {1}", WalletId, enBalance); var temp = Result1.FirstOrDefault();

                //ntrivedi 13-02-2019 added so margin wallet do not use in other transaction
                if (walletObject.WalletUsageType != Convert.ToInt16(enWalletUsageType))
                {
                    HelperForLog.WriteLogIntoFileAsync("CheckUserBalance", "WalletId=" + WalletId.ToString() + "WalletUsageType Mismatching :" + enWalletUsageType);
                    return false;
                }
                if (enBalance == enBalanceType.AvailableBalance)
                {
                    wObjBal = walletObject.Balance;
                }
                else if (enBalance == enBalanceType.OutBoundBalance)
                {
                    wObjBal = walletObject.OutBoundBalance;
                }
                else if (enBalance == enBalanceType.InBoundBalance)
                {
                    wObjBal = walletObject.InBoundBalance;
                }
                else
                {
                    return false;
                }
                if (wObjBal < 0) //ntrivedi 04-01-2018
                {
                    return false;
                }
                if (temp.DifferenceAmount == wObjBal && temp.DifferenceAmount > 0)
                {
                    return true;
                }
                HelperForLog.WriteLogIntoFileAsync("CheckUserBalance", "WalletId=" + WalletId.ToString() + ",Total=" + temp.DifferenceAmount.ToString() + ",dbbalance=" + wObjBal.ToString());
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<bool> CheckTrnIDDrForMarketAsync(CommonClassCrDr arryTrnID)
        {
            try
            {
                GetCount count;
                TQTrnAmt sumAmount;

                IQueryable<GetCount> Result1 = _dbContext.GetCount.FromSql(@"SELECT count(TrnNo)  as 'Count' FROM WalletTransactionQueues WHERE TrnRefNo = {0} and Status=4 and TrnType={1} and WalletDeductionType={2}", arryTrnID.debitObject.TrnRefNo, enWalletTranxOrderType.Debit, enWalletDeductionType.Market);
                IQueryable<TQTrnAmt> Result2 = _dbContext.TQTrnAmt.FromSql(@"SELECT Amount-SettedAmt as 'DifferenceAmount',TrnNo  FROM WalletTransactionQueues WHERE TrnRefNo = {0} and Status=4 and TrnType={1} and WalletDeductionType={2}", arryTrnID.debitObject.TrnRefNo, enWalletTranxOrderType.Debit, enWalletDeductionType.Market);
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

        public async Task<ApplicationUser> GetUserById(long id)
        {
            try
            {
                return _dbContext.Set<ApplicationUser>().FirstOrDefault(e => e.Id == id);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        public async Task<AllSumAmount> GetSumForPolicy(long WalletType, long TrnType)
        {
            try
            {
                SumAmountAndCount Daily = new SumAmountAndCount();
                SumAmountAndCount Monthly = new SumAmountAndCount();
                SumAmountAndCount Hourly = new SumAmountAndCount();
                SumAmountAndCount Weekly = new SumAmountAndCount();
                SumAmountAndCount LifeTime = new SumAmountAndCount();
                SumAmountAndCount Yearly = new SumAmountAndCount();

                AllSumAmount obj = new AllSumAmount();

                string sqlDaily = "SELECT ISNULL(SUM(Amount),0) AS 'TotalAmount',ISNULL(SUM(Count),0) AS 'TotalCount' FROM Statastics WHERE  CreatedDate BETWEEN DATEADD(DD, 0, DATEDIFF(DD, 0, dbo.GetISTDate())) AND dbo.GetISTDate() AND ";

                string sqlMonthly = "SELECT ISNULL(SUM(Amount),0) AS 'TotalAmount',ISNULL(SUM(Count),0) AS 'TotalCount' FROM Statastics WHERE CreatedDate BETWEEN DATEADD(MM, 0, DATEDIFF(MM, 0, dbo.GetISTDate())) AND dbo.GetISTDate() AND ";

                string sqlYearly = "SELECT ISNULL(SUM(Amount),0) AS 'TotalAmount',ISNULL(SUM(Count),0) AS 'TotalCount' FROM Statastics WHERE  CreatedDate BETWEEN DATEADD(YY, 0, DATEDIFF(YY, 0, dbo.GetISTDate())) AND dbo.GetISTDate() AND ";

                string sqlHourly = "SELECT ISNULL(SUM(Amount),0) AS 'TotalAmount',ISNULL(SUM(Count),0) AS 'TotalCount' FROM Statastics WHERE  CreatedDate BETWEEN DATEADD(HOUR, 0, DATEDIFF(HOUR, 0, dbo.GetISTDate())) AND dbo.GetISTDate() AND ";

                string sqlWeekly = "SELECT ISNULL(SUM(Amount),0) AS 'TotalAmount',ISNULL(SUM(Count),0) AS 'TotalCount' FROM Statastics WHERE CreatedDate BETWEEN DATEADD(WEEK, 0, DATEDIFF(WEEK, 0, dbo.GetISTDate())) AND dbo.GetISTDate() AND ";

                string sqlLife = "SELECT ISNULL(SUM(Amount),0) AS 'TotalAmount',ISNULL(SUM(Count),0) AS 'TotalCount' FROM Statastics WHERE ";

                if (WalletType != 0)
                {
                    sqlDaily = sqlDaily + "WalletType={0}";
                    Daily = _dbContext.SumAmountAndCount.FromSql(sqlDaily, WalletType).FirstOrDefault();

                    sqlHourly = sqlHourly + "WalletType={0}";
                    Hourly = _dbContext.SumAmountAndCount.FromSql(sqlHourly, WalletType).FirstOrDefault();

                    sqlMonthly = sqlMonthly + "WalletType={0}";
                    Monthly = _dbContext.SumAmountAndCount.FromSql(sqlMonthly, WalletType).FirstOrDefault();

                    sqlYearly = sqlYearly + "WalletType={0}";
                    Yearly = _dbContext.SumAmountAndCount.FromSql(sqlYearly, WalletType).FirstOrDefault();

                    sqlLife = sqlLife + "WalletType={0}";
                    LifeTime = _dbContext.SumAmountAndCount.FromSql(sqlLife, WalletType).FirstOrDefault();

                    sqlWeekly = sqlWeekly + "WalletType={0}";
                    Weekly = _dbContext.SumAmountAndCount.FromSql(sqlWeekly, WalletType).FirstOrDefault();
                }
                else if (TrnType != 0)
                {
                    sqlDaily = sqlDaily + "TrnType={0}";
                    Daily = _dbContext.SumAmountAndCount.FromSql(sqlDaily, TrnType).FirstOrDefault();

                    sqlHourly = sqlDaily + "TrnType={0}";
                    Hourly = _dbContext.SumAmountAndCount.FromSql(sqlHourly, TrnType).FirstOrDefault();

                    sqlMonthly = sqlMonthly + "TrnType={0}";
                    Monthly = _dbContext.SumAmountAndCount.FromSql(sqlMonthly, TrnType).FirstOrDefault();

                    sqlYearly = sqlYearly + "TrnType={0}";
                    Yearly = _dbContext.SumAmountAndCount.FromSql(sqlYearly, TrnType).FirstOrDefault();

                    sqlLife = sqlLife + "TrnType={0}";
                    LifeTime = _dbContext.SumAmountAndCount.FromSql(sqlLife, TrnType).FirstOrDefault();

                    sqlWeekly = sqlWeekly + "TrnType={0}";
                    Weekly = _dbContext.SumAmountAndCount.FromSql(sqlWeekly, TrnType).FirstOrDefault();
                }

                obj.DailyAmount = Convert.ToDecimal(Daily.TotalAmount);
                obj.MonthlyAmount = Convert.ToDecimal(Monthly.TotalAmount);
                obj.WeeklyAmount = Convert.ToDecimal(Weekly.TotalAmount);
                obj.HourlyAmount = Convert.ToDecimal(Hourly.TotalAmount);
                obj.LifeTimeAmount = Convert.ToDecimal(LifeTime.TotalAmount);
                obj.YearlyAmount = Convert.ToDecimal(Yearly.TotalAmount);

                obj.DailyCount = Convert.ToInt64(Daily.TotalCount);
                obj.MonthlyCount = Convert.ToInt64(Monthly.TotalCount);
                obj.WeeklyCount = Convert.ToInt64(Weekly.TotalCount);
                obj.HourlyCount = Convert.ToInt64(Hourly.TotalCount);
                obj.LifeTimeCount = Convert.ToInt64(LifeTime.TotalCount);
                obj.YearlyCount = Convert.ToInt64(Yearly.TotalCount);

                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public BeneUpdate UpdateDefaultWallets(long WalletTypeID, long UserID)
        {
            try
            {
                BeneUpdate res = new BeneUpdate();
                //2019-2-15 added condi for only used trading wallet
                string Query = "UPDATE WalletMasters SET IsDefaultWallet = 0 WHERE UserID = {0} AND Walletusagetype=0 and WalletTypeID = {1} SELECT @@ROWCOUNT as 'AffectedRows'";
                IQueryable<BeneUpdate> Result = _dbContext.BeneUpdate.FromSql(Query, UserID, WalletTypeID);
                res = Result.FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #region AddUserWalletRequest

        //2018-12-20
        public List<AddWalletRequestRes> ListAddUserWalletRequest(long UserId)
        {
            try
            {
                var items = _dbContext.AddWalletRequestRes.FromSql(@"SELECT CASE a.Type  WHEN 1 THEN 'AddRequest' ELSE 'RemoveRequest' END AS 'RequestType',a.OwnerApprovalStatus,CASE a.OwnerApprovalStatus WHEN 0 THEN 'Pending' WHEN 1 THEN 'Accepted' ELSE 'Rejected' END AS 'StrOwnerApprovalStatus',a.Message,a.Id AS RequestId,w.Walletname AS WalletName,wt.WalletTypeName AS WalletType,a.Status AS Status,r.RoleType AS RoleName,a.ReceiverEmail AS ToEmail,b.Email AS FromEmail,CASE a.Status WHEN 0 THEN 'Pending' WHEN 1 THEN 'Accepted' ELSE 'Rejected' END AS 'StrStatus'  FROM AddRemoveUserWalletRequest a INNER JOIN WalletMasters w ON w.Id = a.WalletID INNER JOIN WalletTypeMasters wt ON wt.Id = w.WalletTypeID INNER JOIN BizUser b ON b.Id =a.FromUserId INNER JOIN UserRoleMaster r ON r.Id =a.RoleId  WHERE a.Status=0 AND w.WalletUsageType=0 AND
                    ((a.FromUserId={0}  and a.OwnerApprovalStatus=0) 
                    OR (a.ToUserId={0}  AND a.OwnerApprovalStatus=1) 
                    OR (a.WalletOwnerUserID={0} AND a.OwnerApprovalStatus=0))", UserId).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<List<UserWalletWise>> ListUserWalletWise(long WalletId)
        {
            try
            {
                var items = _dbContext.UserWalletWise.FromSql(@"SELECT r.id as RoleID,r.RoleType as RoleName,b.UserName as UserName,Isnull(b.Email,'') as Email,w.Walletname as WalletName,wt.WalletTypeName as WalletType from WalletAuthorizeUserMaster a INNER JOIN UserRoleMaster r on r.Id=a.RoleId  INNER JOIN BizUser b on b.Id = a.UserID inner join WalletMasters w on w.id = a.WalletID INNER JOIN WalletTypeMasters wt on wt.Id=w.WalletTypeID where a.status=1 and a.WalletID={0} and w.WalletUsageType=0", WalletId).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListUserWalletWise", "WalletRepository", ex);
                throw ex;
            }
        }

        #endregion

        #region Staking Policy

        public List<StakingPolicyDetailRes> GetStakingPolicyData(short statkingTypeID, short currencyTypeID)
        {
            try
            {
                string Query = "SELECT SPD.ID AS 'PolicyDetailID',SPM.StakingType, " +
                    "CASE SPM.StakingType WHEN 1 THEN 'FD' WHEN 2 THEN 'Charge' END AS 'StakingTypeName',SPM.SlabType," +
                    "CASE SPM.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' END AS 'SlabTypeName',SPM.WalletTypeID," +
                    "(SELECT WTM.WalletTypeName FROM WalletTypeMasters WTM WHERE WTM.id=SPM.WalletTypeID) AS 'StakingCurrency'," +
                    "SPD.StakingDurationWeek AS 'DurationWeek',SPD.StakingDurationMonth AS 'DurationMonth',SPD.InterestType," +
                    "ISNULL(CASE SPD.InterestType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Percentage' END,'-') AS 'InterestTypeName'," +
                    "CASE SPM.SlabType WHEN 1 THEN CAST(SPD.MinAmount AS varchar) WHEN 2 THEN CAST(MinAmount AS varchar) +'-' + CAST(MaxAmount AS varchar) END AS 'AvailableAmount', " +
                    "SPD.MinAmount,SPD.MaxAmount,SPD.InterestValue,SPD.InterestWalletTypeID AS 'MaturityCurrencyID',ISNULL(WT.WalletTypeName,'-') AS 'MaturityCurrencyName'," +
                    "ISNULL(SPD.MakerCharges, 0) AS 'MakerCharges',ISNULL(SPD.TakerCharges, 0) AS 'TakerCharges',SPD.Status," +
                    "SPD.EnableAutoUnstaking,CASE SPD.EnableAutoUnstaking WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableAutoUnstaking'," +
                    "SPD.EnableStakingBeforeMaturity," +
                    "SPD.RenewUnstakingEnable,CASE SPD.RenewUnstakingEnable WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrRenewUnstakingEnable',SPD.RenewUnstakingPeriod," +
                    "CASE SPD.EnableStakingBeforeMaturity WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableStakingBeforeMaturity'," +
                    "SPD.EnableStakingBeforeMaturityCharge " +
                    "FROM StakingPolicyDetail SPD " +
                    "INNER JOIN StakingPolicyMaster SPM ON SPD.StakingPolicyID = SPM.Id " +
                    "LEFT JOIN WalletTypeMasters WT ON WT.Id = SPD.InterestWalletTypeID " +
                    "WHERE SPD.Status = 1  AND SPM.Status = 1 AND (SPM.StakingType = {0} or {0} = 0) AND (SPM.WalletTypeID = {1} or {1} = 0) ";
                var data = _dbContext.StakingPolicyDetailRes.FromSql(Query, statkingTypeID, currencyTypeID).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<StakingPolicyDetailResV2> GetStakingPolicyDataV2(short statkingTypeID, short currencyTypeID)
        {
            try
            {
                string Query = "SELECT SPD.ID AS 'PolicyDetailID',SPM.StakingType, " +
                    "CASE SPM.StakingType WHEN 1 THEN 'FD' WHEN 2 THEN 'Charge' END AS 'StakingTypeName',SPM.SlabType," +
                    "CASE SPM.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' END AS 'SlabTypeName',SPM.WalletTypeID," +
                    "(SELECT WTM.WalletTypeName FROM WalletTypeMasters WTM WHERE WTM.id=SPM.WalletTypeID) AS 'StakingCurrency'," +
                    "SPD.StakingDurationWeek AS 'DurationWeek',SPD.StakingDurationMonth AS 'DurationMonth',SPD.InterestType," +
                    "ISNULL(CASE SPD.InterestType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Percentage' END,'-') AS 'InterestTypeName'," +
                    "CASE SPM.SlabType WHEN 1 THEN CAST(SPD.MinAmount AS varchar) WHEN 2 THEN CAST(MinAmount AS varchar) +'-' + CAST(MaxAmount AS varchar) END AS 'AvailableAmount', " +
                    "SPD.MinAmount,SPD.MaxAmount,SPD.InterestValue,SPD.InterestWalletTypeID AS 'MaturityCurrencyID',ISNULL(WT.WalletTypeName,'-') AS 'MaturityCurrencyName'," +
                    "ISNULL(SPD.MakerCharges, 0) AS 'MakerCharges',ISNULL(SPD.TakerCharges, 0) AS 'TakerCharges',SPD.Status," +
                    "SPD.EnableAutoUnstaking,CASE SPD.EnableAutoUnstaking WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableAutoUnstaking'," +
                    "SPD.EnableStakingBeforeMaturity," +
                    "SPD.RenewUnstakingEnable,CASE SPD.RenewUnstakingEnable WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrRenewUnstakingEnable',SPD.RenewUnstakingPeriod," +
                    "CASE SPD.EnableStakingBeforeMaturity WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableStakingBeforeMaturity'," +
                    "SPD.EnableStakingBeforeMaturityCharge " +
                    "FROM StakingPolicyDetail SPD " +
                    "INNER JOIN StakingPolicyMaster SPM ON SPD.StakingPolicyID = SPM.Id " +
                    "LEFT JOIN WalletTypeMasters WT ON WT.Id = SPD.InterestWalletTypeID " +
                    "WHERE SPD.Status = 1  AND SPM.Status = 1 AND SPM.StakingType = {0} AND SPM.WalletTypeID = {1}";
                var data = _dbContext.StakingPolicyDetailResV2.FromSql(Query, statkingTypeID, currencyTypeID).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public PreStackingConfirmationRes GetPreStackingData(long PolicyDetailID)
        {
            try
            {
                PreStackingConfirmationRes data = new PreStackingConfirmationRes();
                string Query = "SELECT SPD.ID AS 'PolicyDetailID',SPM.StakingType, " +
                    "CASE SPM.StakingType WHEN 1 THEN 'FD' WHEN 2 THEN 'Charge' END AS 'StakingTypeName',SPM.SlabType," +
                    "CASE SPM.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' END AS 'SlabTypeName',SPM.WalletTypeID,SPD.InterestWalletTypeID," +
                    "ISNULL(WT.WalletTypeName,'-') AS 'MaturityCurrencyName',SPD.InterestType," +
                    "ISNULL(CASE SPD.InterestType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Percentage' END,'-') AS 'InterestTypeName'," +
                    "SPD.InterestValue,SPD.StakingDurationWeek AS 'DurationWeek',SPD.StakingDurationMonth AS 'DurationMonth',SPD.MinAmount,SPD.MaxAmount," +
                    "ISNULL(SPD.MakerCharges, 0) AS 'MakerCharges',ISNULL(SPD.TakerCharges, 0) AS 'TakerCharges',SPD.EnableAutoUnstaking," +
                    "CASE SPD.EnableAutoUnstaking WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableAutoUnstaking',SPD.EnableStakingBeforeMaturity," +
                    "CASE SPD.EnableStakingBeforeMaturity WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableStakingBeforeMaturity'," +
                    "SPD.EnableStakingBeforeMaturityCharge " +
                    "FROM StakingPolicyDetail SPD " +
                    "INNER JOIN StakingPolicyMaster SPM ON SPD.StakingPolicyID = SPM.Id " +
                    "LEFT JOIN WalletTypeMasters WT ON WT.Id = SPD.InterestWalletTypeID " +
                    "WHERE SPD.Status = 1 AND SPD.Id = {0}";
                data = _dbContext.PreStackingConfirmationRes.FromSql(Query, PolicyDetailID).FirstOrDefault();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetPreStackingData", "WalletRepository", ex);
                throw ex;
            }
        }

        #endregion

        public async Task<List<WalletMasterRes>> ListWalletMasterResponseNew(long UserId, string Coin)
        {
            try
            {
                //2019-2-15 added condi for only used trading wallet
                var data = _dbContext.WalletMasterRes.FromSql("select r.Id as RoleId ,r.RoleName as RoleName ,u.AccWalletID,u.ExpiryDate,ISNULL(u.OrgID,0) AS OrgID,u.Walletname as WalletName,c.WalletTypeName as CoinName,u.PublicAddress,u.Balance,u.IsDefaultWallet,u.InBoundBalance,u.OutBoundBalance from WalletAuthorizeUserMaster wa inner join WalletMasters u on u.Id=wa.WalletID inner join WalletTypeMasters c on c.Id= u.WalletTypeID inner join UserRoleMaster r on r.id=wa.RoleID where wa.Status = 1 AND wa.UserID={0} AND Walletusagetype=0 AND (c.wallettypename={1} or {1}='')", UserId, (Coin == null ? "" : Coin)).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListWalletMasterResponseNew", "WalletRepository", ex);
                throw ex;
            }
        }

        public async Task<List<WalletMasterRes>> GetWalletMasterResponseByCoinNew(long UserId, string coin)
        {
            try
            {
                //2019-2-15 added condi for only used trading wallet
                var data = _dbContext.WalletMasterRes.FromSql("select r.Id as RoleId ,r.RoleName as RoleName ,u.AccWalletID,u.ExpiryDate,ISNULL(u.OrgID,0) AS OrgID,u.Walletname as WalletName,c.WalletTypeName as CoinName,u.PublicAddress,u.Balance,u.IsDefaultWallet,u.InBoundBalance,u.OutBoundBalance from WalletAuthorizeUserMaster wa inner join WalletMasters u on u.Id=wa.WalletID inner join WalletTypeMasters c on c.Id= u.WalletTypeID inner join UserRoleMaster r on r.id=wa.RoleID where wa.Status = 1 AND wa.UserID={0} AND Walletusagetype=0 and c.WalletTypeName={1}", UserId, coin).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWalletMasterResponseByCoinNew", "WalletRepository", ex);
                throw ex;
            }
        }

        public async Task<List<WalletMasterRes>> GetWalletMasterResponseByIdNew(long UserId, string walletId)
        {
            try
            {
                var data = _dbContext.WalletMasterRes.FromSql("select r.Id as RoleId ,r.RoleName as RoleName ,u.AccWalletID,u.ExpiryDate,ISNULL(u.OrgID,0) AS OrgID,u.Walletname as WalletName,c.WalletTypeName as CoinName,u.PublicAddress,u.Balance,u.IsDefaultWallet,u.InBoundBalance,u.OutBoundBalance from WalletAuthorizeUserMaster wa inner join WalletMasters u on u.Id=wa.WalletID inner join WalletTypeMasters c on c.Id= u.WalletTypeID inner join UserRoleMaster r on r.id=wa.RoleID where wa.Status = 1  AND Walletusagetype=0 AND wa.UserID={0} AND  u.AccWalletID={1}", UserId, walletId).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWalletMasterResponseByIdNew", "WalletRepository", ex);
                throw ex;
            }
        }

        // NEW balance API
        public Balance GetAllBalancesNew(long userid, long walletid)
        {
            try
            {
                var items = _dbContext.Balance.FromSql(@"select ISNULL((select sum(Amount) from WalletTransactionQueues where WalletID ={0} AND MemberID = {1} AND (Status=4 or Status=6)),0 )as UnSettledBalance ,ISnull((select sum(w.Balance) from WalletAuthorizeUserMaster wa inner join WalletMasters w on w.Id=wa.WalletID join WalletTypeMasters wt on wt.Id=w.WalletTypeId where wa.WalletID = {0} and wa.UserID = {1} and wa.Status =1),0) as AvailableBalance,ISNULL((select sum(w.Amount) from DepositHistory w inner join AddressMasters wt on wt.Address=w.Address where w.Id = {0} and w.UserID = {1} and w.Status =0),0 )as UnClearedBalance,ISNULL((select sum(u.ShadowAmount) from MemberShadowBalance u inner join WalletMasters w on w.Id=u.WalletID inner join WalletTypeMasters wt on wt.Id= u.WalletTypeId where w.Id = {0} and w.UserID = {1} and w.Status =0),0) as ShadowBalance,ISNULL((select SUM(StakingAmount) from TokenStakingHistory where WalletID={0} and UserId={1} And Status in (1,4)),0) as StackingBalance", walletid, userid).First();

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public Balance GetAllBalancesV1(long userid, long walletid)
        {

            try
            {
                var items = _dbContext.Balance.FromSql(@"select ISNULL((select (sum(wtq.Amount)) from WalletTransactionQueues wtq inner join WalletMasters w on w.id=wtq.WalletID where WalletID ={0} AND MemberID = {1} AND (wtq.Status=4 or wtq.Status=6)),0 )as UnSettledBalance ,ISnull((select sum(w.Balance) from WalletAuthorizeUserMaster wa inner join WalletMasters w on w.Id=wa.WalletID join WalletTypeMasters wt on wt.Id=w.WalletTypeId where wa.WalletID = {0} and wa.UserID = {1} and wa.Status =1),0) as AvailableBalance,ISNULL((select sum(w.Amount) from DepositHistory w inner join AddressMasters wt on wt.Address=w.Address where w.Id = {0} and w.UserID = {1} and w.Status =0),0 )as UnClearedBalance,ISNULL((select sum(u.ShadowAmount) from MemberShadowBalance u inner join WalletMasters w on w.Id=u.WalletID inner join WalletTypeMasters wt on wt.Id= u.WalletTypeId where w.Id = {0} and w.UserID = {1} and w.Status =0),0) as ShadowBalance,ISNULL((select SUM(StakingAmount) from TokenStakingHistory where WalletID={0} and UserId={1} And Status in (1,4)),0) as StackingBalance", walletid, userid).First();

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<BalanceResponse> GetAvailableBalanceNew(long userid, long walletId)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                List<BalanceResponse> items = (from wa in _dbContext.WalletAuthorizeUserMaster
                                               join w in _dbContext.WalletMasters on wa.WalletID equals w.Id
                                               join wt in _dbContext.WalletTypeMasters
                                                       on w.WalletTypeID equals wt.Id
                                               where wa.WalletID == walletId && wa.UserID == userid && wa.Status == Convert.ToInt16(ServiceStatus.Active) && w.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)
                                               select new BalanceResponse
                                               {
                                                   Balance = w.Balance,
                                                   WalletId = w.Id,
                                                   WalletType = wt.WalletTypeName
                                               }).AsEnumerable().ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<BalanceResponse> GetAllAvailableBalanceNew(long userid)
        {
            try
            {
                List<BalanceResponse> items = (from wa in _dbContext.WalletAuthorizeUserMaster
                                               join w in _dbContext.WalletMasters on wa.WalletID equals w.Id
                                               join wt in _dbContext.WalletTypeMasters
                                                       on w.WalletTypeID equals wt.Id
                                               where wa.UserID == userid && wa.Status == Convert.ToInt16(ServiceStatus.Active)
                                               select new BalanceResponse
                                               {
                                                   Balance = w.Balance,
                                                   WalletId = w.Id,
                                                   WalletType = wt.WalletTypeName,
                                               }).AsEnumerable().ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public decimal GetTotalAvailbleBalNew(long userid)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var total = (from wa in _dbContext.WalletAuthorizeUserMaster
                             join w in _dbContext.WalletMasters on wa.WalletID equals w.Id
                             where wa.UserID == userid && wa.Status == Convert.ToInt16(ServiceStatus.Active) && w.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet)
                             select w.Balance
                            ).Sum();
                return total;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<BalanceResponseLimit> GetAvailbleBalTypeWiseNew(long userid)
        {
            try
            {
                //2019-2-18 added condi for only used trading wallet
                var result = _dbContext.BalanceResponseLimit.FromSql(@"select wt.WalletTypeName as WalletType,ISNULL(SUM(w.Balance),0) as Balance from WalletTypeMasters wt  left join WalletMasters w on w.WalletTypeID=wt.Id and w.UserID={0} and w.status=1 and w.WalletUsageType=0  group by wt.WalletTypeName", userid).ToList();

                return result;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<StakingHistoryRes> GetStackingHistoryData(DateTime? fromDate, DateTime? toDate, EnStakeUnStake? type, int pageSize, int pageNo, EnStakingSlabType? slab, EnStakingType? stakingType, long userID, ref int TotalCount)
        {
            try
            {
                string Query = "SELECT TSH.Id AS 'StakingHistoryId', TSH.CreatedDate AS 'StakingDate',  TSH.StakingPolicyDetailID AS 'PolicyDetailID',TSH.StakingType,CASE TSH.StakingType WHEN 1 THEN 'FD' WHEN 2 THEN 'Charge' END AS 'StakingTypeName',TSH.SlabType,CASE TSH.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' END AS 'SlabTypeName',TSH.WalletTypeID,ISNULL(WT.WalletTypeName, '-') AS 'StakingCurrency',TSH.InterestType,ISNULL(CASE TSH.InterestType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Percentage' END, '-') AS 'InterestTypeName',TSH.InterestValue,SPD.StakingDurationWeek AS 'DurationWeek',SPD.StakingDurationMonth AS 'DurationMonth',TSH.InterestWalletTypeID,ISNULL((SELECT WTM.WalletTypeName FROM WalletTypeMasters WTM WHERE WTM.id = TSH.InterestWalletTypeID),'-')AS 'MaturityCurrency',CASE TSH.SlabType WHEN 1 THEN CAST(TSH.MinAmount AS varchar) WHEN 2 THEN CAST(TSH.MinAmount AS varchar)+'-' + CAST(TSH.MaxAmount AS varchar) END AS 'AvailableAmount',TSH.MakerCharges,TSH.TakerCharges,TSH.EnableAutoUnstaking,CASE TSH.EnableAutoUnstaking WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableAutoUnstaking',TSH.EnableStakingBeforeMaturity,CASE TSH.EnableStakingBeforeMaturity WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableStakingBeforeMaturity',TSH.EnableStakingBeforeMaturityCharge,TSH.RenewUnstakingEnable,CASE TSH.Status WHEN 0 THEN 'Inactive' WHEN 1 THEN 'Active' WHEN 4 THEN 'Unstaking Request To Admin' WHEN 5 THEN 'Unstake' END AS 'StrStatus',CASE TSH.RenewUnstakingEnable WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrRenewUnstakingEnable',TSH.RenewUnstakingPeriod,TSH.StakingAmount,TSH.MaturityDate,TSH.MaturityAmount,TSH.ChannelID,CH.ChannelName,TSH.UserID,(BU.FirstName + ' ' + BU.LastName) AS 'UserName',TSH.WalletID,ISNULL(WM.WalletName, '-') AS 'WalletName',TSH.WalletOwnerID,TSH.Status,ISNULL(TUH.TokenStakingHistoryID,0) AS TokenStakingHistoryID,ISNULL(TUH.AmountCredited,0) AS AmountCredited,ISNULL(TUH.UnstakeType,0) AS UnstakeType,ISNULL(TUH.InterestCreditedValue,0) AS InterestCreditedValue,ISNULL( TUH.ChargeBeforeMaturity,0) AS ChargeBeforeMaturity,CASE TUH.UnstakeType WHEN 1 THEN 'Full' WHEN 2 THEN 'Partial' ELSE 'N/A' END AS 'StrUnstakeType',ISNULL(TUH.DegradeStakingHistoryRequestID,0) AS DegradeStakingHistoryRequestID,ISNULL(TUH.CreatedDate,DBO.GetISTDate()) AS 'UnstakingDate'FROM TokenStakingHistory TSH INNER JOIN StakingPolicyDetail SPD ON SPD.Id = TSH.StakingPolicyDetailID LEFT JOIN WalletTypeMasters WT ON WT.Id = TSH.WalletTypeID LEFT JOIN BizUser BU ON BU.Id = TSH.UserID LEFT JOIN TokenUnStakingHistory TUH ON TUH.TokenStakingHistoryID = TSH.Id INNER JOIN AllowedChannels CH ON CH.ChannelID = TSH.ChannelID LEFT JOIN WalletMasters WM ON WM.Id = TSH.WalletID " +
  "WHERE TSH.Status < 9 AND (TSH.UserID = {0} OR {0}=0) AND (TSH.SlabType = {1} OR {1}=0) " +
  "AND (TSH.StakingType = {2} OR {2}=0) AND (TSH.Status = {3} OR {3}=0) ";
                if (fromDate != null && toDate != null)
                {
                    toDate = Convert.ToDateTime(toDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += "AND (TSH.CreatedDate BETWEEN {4} AND {5}) ORDER BY TSH.ID DESC ";
                    var data = _dbContext.StakingHistoryRes.FromSql(Query, userID, (slab == null ? 0 : Convert.ToInt16(slab)), (stakingType == null ? 0 : Convert.ToInt16(stakingType)), (type == null ? 0 : Convert.ToInt16(type)), fromDate, toDate).ToList();
                    TotalCount = data.Count();
                    if (pageNo > 0)
                    {
                        int skip = pageSize * (pageNo - 1);
                        data = data.Skip(skip).Take(pageSize).ToList();
                    }
                    return data;
                }
                else
                {
                    Query += " ORDER BY TSH.ID DESC";
                    var data = _dbContext.StakingHistoryRes.FromSql(Query, userID, (slab == null ? 0 : Convert.ToInt16(slab)), (stakingType == null ? 0 : Convert.ToInt16(stakingType)), (type == null ? 0 : Convert.ToInt16(type))).ToList();
                    TotalCount = data.Count();
                    if (pageNo > 0)
                    {
                        int skip = pageSize * (pageNo - 1);
                        data = data.Skip(skip).Take(pageSize).ToList();
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<StakingHistoryResv2> GetStackingHistoryDatav2(DateTime? fromDate, DateTime? toDate, EnStakeUnStake? type, int pageSize, int pageNo, EnStakingSlabType? slab, EnStakingType? stakingType, long userID, ref int TotalCount)
        {
            try
            {
                string Query = "SELECT TSH.GUID AS 'StakingHistoryId',TSH.StakingPolicyDetailID AS 'PolicyDetailID',TSH.StakingType,CASE TSH.StakingType WHEN 1 THEN 'FD' WHEN 2 THEN 'Charge' END AS 'StakingTypeName',TSH.SlabType,CASE TSH.SlabType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Range' END AS 'SlabTypeName',TSH.WalletTypeID,ISNULL(WT.WalletTypeName, '-') AS 'StakingCurrency',TSH.InterestType,ISNULL(CASE TSH.InterestType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Percentage' END, '-') AS 'InterestTypeName',TSH.InterestValue,SPD.StakingDurationWeek AS 'DurationWeek',SPD.StakingDurationMonth AS 'DurationMonth',TSH.InterestWalletTypeID,ISNULL((SELECT WTM.WalletTypeName FROM WalletTypeMasters WTM WHERE WTM.id = TSH.InterestWalletTypeID),'-')AS 'MaturityCurrency',CASE TSH.SlabType WHEN 1 THEN CAST(TSH.MinAmount AS varchar) WHEN 2 THEN CAST(TSH.MinAmount AS varchar)+'-' + CAST(TSH.MaxAmount AS varchar) END AS 'AvailableAmount',TSH.MakerCharges,TSH.TakerCharges,TSH.EnableAutoUnstaking,CASE TSH.EnableAutoUnstaking WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableAutoUnstaking',TSH.EnableStakingBeforeMaturity,CASE TSH.EnableStakingBeforeMaturity WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrEnableStakingBeforeMaturity',TSH.EnableStakingBeforeMaturityCharge,TSH.RenewUnstakingEnable,CASE TSH.Status WHEN 0 THEN 'Inactive' WHEN 1 THEN 'Active' WHEN 4 THEN 'Unstaking Request To Admin' WHEN 5 THEN 'Unstake' END AS 'StrStatus',CASE TSH.RenewUnstakingEnable WHEN 1 THEN 'Yes' ELSE 'No' END AS 'StrRenewUnstakingEnable',TSH.RenewUnstakingPeriod,TSH.StakingAmount,TSH.MaturityDate,TSH.MaturityAmount,TSH.ChannelID,CH.ChannelName,TSH.UserID,(BU.FirstName + ' ' + BU.LastName) AS 'UserName',TSH.WalletID,ISNULL(WM.WalletName, '-') AS 'WalletName',TSH.WalletOwnerID,TSH.Status,ISNULL(TUH.TokenStakingHistoryID,0) AS TokenStakingHistoryID,ISNULL(TUH.AmountCredited,0) AS AmountCredited,ISNULL(TUH.UnstakeType,0) AS UnstakeType,ISNULL(TUH.InterestCreditedValue,0) AS InterestCreditedValue,ISNULL( TUH.ChargeBeforeMaturity,0) AS ChargeBeforeMaturity,CASE TUH.UnstakeType WHEN 1 THEN 'Full' WHEN 2 THEN 'Partial' ELSE 'N/A' END AS 'StrUnstakeType',ISNULL(TUH.DegradeStakingHistoryRequestID,0) AS DegradeStakingHistoryRequestID,TSH.CreatedDate AS 'StakingDate', ISNULL(TUH.CreatedDate,DBO.GetISTDate()) AS 'UnstakingDate'FROM TokenStakingHistory TSH INNER JOIN StakingPolicyDetail SPD ON SPD.Id = TSH.StakingPolicyDetailID LEFT JOIN WalletTypeMasters WT ON WT.Id = TSH.WalletTypeID LEFT JOIN BizUser BU ON BU.Id = TSH.UserID LEFT JOIN TokenUnStakingHistory TUH ON TUH.TokenStakingHistoryID = TSH.Id INNER JOIN AllowedChannels CH ON CH.ChannelID = TSH.ChannelID LEFT JOIN WalletMasters WM ON WM.Id = TSH.WalletID " +
  "WHERE TSH.Status < 9 AND (TSH.UserID = {0} OR {0}=0) AND (TSH.SlabType = {1} OR {1}=0) " +
  "AND (TSH.StakingType = {2} OR {2}=0) AND (TSH.Status = {3} OR {3}=0) ";
                if (fromDate != null && toDate != null)
                {
                    toDate = Convert.ToDateTime(toDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                    Query += "AND (TUH.CreatedDate BETWEEN {4} AND {5} OR TSH.CreatedDate BETWEEN {4} AND {5}) ";
                    var data = _dbContext.StakingHistoryResv2.FromSql(Query, userID, (slab == null ? 0 : Convert.ToInt16(slab)), (stakingType == null ? 0 : Convert.ToInt16(stakingType)), (type == null ? 0 : Convert.ToInt16(type)), fromDate, toDate).ToList();
                    TotalCount = data.Count();
                    if (pageNo > 0)
                    {
                        int skip = pageSize * (pageNo - 1);
                        data = data.Skip(skip).Take(pageSize).ToList();
                    }
                    return data;
                }
                else
                {
                    var data = _dbContext.StakingHistoryResv2.FromSql(Query, userID, (slab == null ? 0 : Convert.ToInt16(slab)), (stakingType == null ? 0 : Convert.ToInt16(stakingType)), (type == null ? 0 : Convert.ToInt16(type))).ToList();
                    TotalCount = data.Count();
                    if (pageNo > 0)
                    {
                        int skip = pageSize * (pageNo - 1);
                        data = data.Skip(skip).Take(pageSize).ToList();
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int IsSelfAddress(string address, long userID, string smscode)
        {
            try
            {

                GetCount count;
                string query = "select count(AM.ID) As Count from AddressMasters AM inner join WalletMasters WM on wm.id = am.WalletId " +
                "inner join WalletTypeMasters wtm on wtm.Id=wm.WalletTypeID where wtm.WalletTypeName = {0} and wm.UserID = {1} and AM.Address = {2}";
                IQueryable<GetCount> Result1 = _dbContext.GetCount.FromSql(query, smscode, userID, address);
                count = Result1.First();
                return count.Count;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public int IsInternalAddress(string address, long userID, string smscode)
        {
            try
            {

                GetCount count;
                string query = "select count(AM.ID) As Count from AddressMasters AM inner join WalletMasters WM on wm.id = am.WalletId " +
                "inner join WalletTypeMasters wtm on wtm.Id=wm.WalletTypeID where wtm.WalletTypeName = {0} and wm.UserID <> {1} and AM.Address = {2}";
                IQueryable<GetCount> Result1 = _dbContext.GetCount.FromSql(query, smscode, userID, address);
                count = Result1.First();
                return count.Count;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<WalletTransactiondata> GetWalletStatisticsdata(long userID, short month, short year)
        {
            try
            {
                string Query = "SELECT TM.TrnTypeID AS 'TrnTypeId',TrnTypeName,((ISNULL(SUM(ST.Amount),0)) * isNull(CRM.CurrentRate,0)) AS TotalAmount," +
                                "ISNULL(SUM(ST.Count), 0) AS 'TotalCount'" +
                                "FROM WTrnTypeMaster TM left join Statastics ST ON ST.TrnType = TM.TrnTypeId AND ST.Status = 1 AND ST.Month = {2}  AND ST.UserId = {3} AND ST.Year = {4} " +
                                "left JOIN CurrencyRateMaster CRM ON CRM.WalletTypeId = ST.WalletType " +
                                "WHERE TM.TrnTypeID IN({0},{1})  GROUP BY TM.TrnTypeID,TrnTypeName,CurrentRate";

                var BalanceData = _dbContext.WalletTransactiondata.FromSql(Query, Convert.ToInt16(enWalletTrnType.Deposit), Convert.ToInt16(enWalletTrnType.Withdrawal), month, userID, year);
                return BalanceData.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWalletStatisticsdata", "WalletRepository", ex);
                throw ex;
            }
        }

        public List<TranDetails> GetYearlyWalletStatisticsdata(long userID, short year)
        {
            try
            {
                //ntrivedi enum taken wrong 22-01-2018 and joining table WTrnTypeMaster instead of TrnTypeMaster 
                string Query = "SELECT CAST(ST.Month as bigint) AS 'Month',ST.TrnType AS 'TrnTypeId',TM.TrnTypeName, ((ISNULL(SUM(ST.Amount),0)) * CRM.CurrentRate) AS TotalAmount," +
                    "ISNULL(SUM(ST.Count), 0) AS 'TotalCount' FROM Statastics ST " +
                    "INNER JOIN WTrnTypeMaster TM ON ST.TrnType = TM.TrnTypeId " +
                    "INNER JOIN CurrencyRateMaster CRM ON CRM.WalletTypeId = ST.WalletType " +
                    "WHERE ST.Status = 1 AND TrnType IN({0}, {1}) AND ST.UserId = {2} AND ST.Year = {3} " +
                    "GROUP BY ST.Month,ST.Year,ST.TrnType,TM.TrnTypeName,CRM.CurrentRate " +
                    "ORDER BY ST.Month DESC, ST.Year DESC";
                var BalanceData = _dbContext.TranDetails.FromSql(Query, Convert.ToInt16(enWalletTrnType.Deposit), Convert.ToInt16(enWalletTrnType.Withdrawal), userID, year);//Convert.ToInt16(enTrnType.Deposit)  , Convert.ToInt16(enTrnType.Withdraw)
                return BalanceData.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetYearlyWalletStatisticsdata", "WalletRepository", ex);
                throw ex;
            }
        }

        public bool AddAddressIntoDB(long userID, string Address, string TxnID, string Key, long SerProDetailId, short Islocal)
        {
            try
            {
                var serPro = (from s in _dbContext.ServiceProviderDetail
                              where s.Id == SerProDetailId
                              select s).FirstOrDefault();

                var walletObj = (from w in _dbContext.WalletMasters
                                 join wt in _dbContext.WalletTypeMasters on w.WalletTypeID equals wt.Id
                                 where w.UserID == userID && w.WalletTypeID == wt.Id && wt.IsLocal == Islocal && (w.PublicAddress == null || w.PublicAddress == "")
                                 select new WalletMaster
                                 {
                                     AccWalletID = w.AccWalletID,
                                     Walletname = w.Walletname,
                                     CreatedBy = w.CreatedBy,
                                     CreatedDate = w.CreatedDate,
                                     Balance = w.Balance,
                                     InBoundBalance = w.InBoundBalance,
                                     OutBoundBalance = w.OutBoundBalance,
                                     ExpiryDate = w.ExpiryDate,
                                     IsDefaultWallet = w.IsDefaultWallet,
                                     IsValid = w.IsValid,
                                     OrgID = w.OrgID,
                                     Status = w.Status,
                                     UserID = w.UserID,
                                     WalletTypeID = w.WalletTypeID,
                                     WalletUsageType = w.WalletUsageType,
                                     PublicAddress = Address,
                                     UpdatedBy = userID,
                                     UpdatedDate = Helpers.UTC_To_IST()
                                 });
                _dbContext.WalletMasters.UpdateRange(walletObj);

                var addObj = (from w in _dbContext.WalletMasters
                              join wt in _dbContext.WalletTypeMasters on w.WalletTypeID equals wt.Id
                              where w.UserID == userID && w.WalletTypeID == wt.Id && wt.IsLocal == Islocal
                              select new AddressMaster
                              {
                                  Address = Address,
                                  TxnID = TxnID,
                                  UpdatedBy = userID,
                                  UpdatedDate = Helpers.UTC_To_IST(),
                                  CreatedBy = userID,
                                  CreatedDate = Helpers.UTC_To_IST(),
                                  Status = 1,
                                  AddressLable = "Self Address: " + wt.WalletTypeName,
                                  GUID = Key,
                                  WalletId = w.Id,
                                  IsDefaultAddress = 0,
                                  SerProID = serPro.ServiceProID,
                                  OriginalAddress = Address
                              });
                _dbContext.AddressMasters.AddRange(addObj);
                _dbContext.SaveChanges();

                if (Islocal == 5)
                {
                    var NeoAddressObj = (from a in _dbContext.AddressMasters
                                         join w in _dbContext.WalletMasters on a.WalletId equals w.Id
                                         join wt in _dbContext.WalletTypeMasters on w.WalletTypeID equals wt.Id
                                         where w.UserID == userID && w.WalletTypeID == wt.Id && wt.IsLocal == Islocal && a.Status == 1
                                         select new NEODepositCounter
                                         {
                                             UpdatedBy = userID,
                                             UpdatedDate = Helpers.UTC_To_IST(),
                                             CreatedBy = userID,
                                             CreatedDate = Helpers.UTC_To_IST(),
                                             Status = 1,
                                             AddressId = a.Id,
                                             PickUpDate = Helpers.UTC_To_IST(),
                                             RecordCount = 1,
                                             Limit = 0,
                                             LastTrnID = "",
                                             MaxLimit = 0,
                                             WalletTypeID = wt.Id,
                                             SerProId = serPro.ServiceProID,
                                             PreviousTrnID = "",
                                             prevIterationID = "",
                                             FlushAddressEnable = 0,
                                             TPSPickupStatus = 0,
                                             AppType = 7,
                                             StartTime = 0,
                                             EndTime = 0
                                         });
                    _dbContext.NEODepositCounter.AddRange(NeoAddressObj);
                    _dbContext.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddAddressIntoDB", "WalletRepository", ex);
                return false;
            }
        }

        public List<LeaderBoardRes> LeaderBoard(int? UserCount, long[] LeaderId)
        {
            try
            {
                UserCount = (UserCount == null ? Convert.ToInt32(25) : Convert.ToInt32(UserCount));
                string id = string.Join(",", LeaderId);
                string sql = "SELECT TOP " + UserCount + " b.UserName,b.Email,u.UserId, SUM(ProfitPer) AS ProfitPer,SUM(ProfitAmount) AS ProfitAmount,ROW_NUMBER() OVER(ORDER BY SUM(u.ProfitPer)) AS AutoId  FROM  UserProfitStatistics u INNER JOIN BizUser b ON b.Id=u.UserId  where u.userid in (" + id + ")  GROUP BY u.UserId,b.UserName,b.Email ORDER BY ProfitPer DESC";

                var data = _dbContext.LeaderBoardRes.FromSql(sql).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LeaderBoard", "WalletRepository", ex);
                throw ex;
            }
        }

        public List<LeaderBoardRes> LeaderBoardWeekWiseTopFive(long[] LeaderId, DateTime Date, short IsGainer, int Count)
        {
            try
            {
                DateTime ToDate = new DateTime();
                ToDate = Date.AddDays(-7);//before 7 days record from Date

                int ToDay = ToDate.Day;
                int ToMonth = ToDate.Month;
                int ToYear = ToDate.Year;

                int FromDay = Date.Day;
                int FromMonth = Date.Month;
                int FromYear = Date.Year;
                string id = string.Join(",", LeaderId);
                string IsDesc = "";
                if (IsGainer == 1)
                {
                    IsDesc = " DESC";
                }
                string sql = "SELECT TOP(" + Count + ") b.UserName,b.Email,u.UserId, SUM(u.ProfitPer) AS ProfitPer,ROW_NUMBER() OVER(ORDER BY sum(u.ProfitPer) " + IsDesc + ") AS AutoId ,SUM(u.ProfitAmount) AS ProfitAmount FROM  UserProfitStatistics u INNER JOIN BizUser b ON b.Id=u.UserId where u.userid in (" + id + ") AND  (u.Day <=" + FromDay + " AND u.Day >= " + ToDay + ") AND (u.Month <=" + FromMonth + " AND u.Month >= " + ToMonth + ") AND (u.Year <=" + FromYear + " AND u.Year >= " + ToYear + ") GROUP BY u.UserId,b.UserName,b.Email ORDER BY SUM(u.ProfitPer) " + IsDesc;
                var data = _dbContext.LeaderBoardRes.FromSql(sql).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LeaderBoard", "WalletRepository", ex);
                throw ex;
            }
        }

        public List<HistoricalPerformanceTemp> GetHistoricalPerformanceYearWise(long UserId, int Year)
        {
            try
            {
                string sql = "SELECT A.AutoNo,isNull(SUM(ProfitPer),0) as ProfitPer from (SELECT top 12 ROW_NUMBER() OVER (ORDER BY (SELECT 12)) as AutoNo FROM WalletTypeMasters) as A left join UserProfitStatistics u on u.Month = a.AutoNo and u.userid={0} and year={1}  GROUP BY A.AutoNo";
                var data = _dbContext.HistoricalPerformanceTemp.FromSql(sql, UserId, Year).ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("LeaderBoard", "WalletRepository", ex);
                throw ex;
            }
        }

        public decimal FindChargeValueHold(string Timestamp, long TrnRefNo)
        {
            try
            {
                var charge = _dbContext.BalanceTotal.FromSql("SELECT ISNULL(DeductedChargeAmount,isNull(HoldChargeAmount,0)) AS TotalBalance  FROM WalletTransactionqueues WHERE TimeStamp='" + Timestamp + "' AND trnrefno= {0}", TrnRefNo).FirstOrDefault();
                if (charge == null) //ntrivedi null condition 26-02-2019
                {
                    return 0;
                }
                return charge.TotalBalance;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("FindChargeValueHold", "WalletRepository", ex);
                throw ex;
            }
        }

        public long FindChargeValueWalletId(string Timestamp, long TrnRefNo)
        {
            try
            {
                var charge = _dbContext.ChargeWalletId.FromSql("SELECT top 1 ISNULL(Dwalletid,0) as Id FROM TrnChargeLog WHERE trnno=(SELECT trnno FROM WalletTransactionqueues WHERE TimeStamp='" + Timestamp + "' and trnrefno= {0}) and trnrefno= {1} order by id desc", TrnRefNo, TrnRefNo).FirstOrDefault();
                if (charge == null) //ntrivedi null condition 26-02-2019
                {
                    return 0;
                }
                return charge.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("FindChargeValueWalletId", "WalletRepository", ex);
                throw ex;
            }
        }

        public long FindChargeValueReleaseWalletId(string Timestamp, long TrnRefNo)
        {
            try
            {
                HelperForLog.WriteLogIntoFileAsync("FindChargeValueReleaseWalletId", "Get walletid and currency walletid=" + "TrnRefNo: " + TrnRefNo.ToString() + "timestamp : " + Timestamp.ToString());

                var charge = _dbContext.ChargeWalletId.FromSql("SELECT top 1 ISNULL(OWalletID,0) as Id FROM TrnChargeLog tc WHERE tc.trnno=(SELECT trnno FROM WalletTransactionqueues WHERE TimeStamp='" + Timestamp + "' and trnrefno= {1}  and LPType<> 1 ) and tc.trnrefno= {0} order by tc.id desc", TrnRefNo, TrnRefNo).FirstOrDefault();//ntrivedi bug fixing 07-08-2019 two entry for lp order
                if (charge == null) //ntrivedi null condition 26-02-2019
                {
                    return 0;
                }
                return charge.Id;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("FindChargeValueReleaseWalletId", "WalletRepository", ex);
                throw ex;
            }
        }

        public decimal FindChargeValueDeduct(string Timestamp, long TrnRefNo)
        {
            try
            {
                HelperForLog.WriteLogIntoFileAsync("FindChargeValueDeduct", "Get walletid and currency walletid=" + "TrnRefNo: " + TrnRefNo.ToString() + "timestamp : " + Timestamp.ToString());

                var charge = _dbContext.BalanceTotal.FromSql(" select Charge AS TotalBalance from TrnChargeLog where TrnNo in ( SELECT TrnNo FROM WalletTransactionqueues WHERE TimeStamp = '" + Timestamp + "' AND trnrefno = {1}) and trnrefno = {0}", TrnRefNo, TrnRefNo).FirstOrDefault();
                if (charge == null) //ntrivedi null condition 26-02-2019
                {
                    return 0;
                }
                return charge.TotalBalance;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("FindChargeValueDeduct", "WalletRepository", ex);
                throw ex;
            }
        }

        public decimal FindChargeValueRelease(string Timestamp, long TrnRefNo)
        {
            try
            {
                var charge = _dbContext.BalanceTotal.FromSql("SELECT ISNULL(Charge,0) as TotalBalance FROM TrnChargeLog WHERE trnno=(SELECT trnno FROM WalletTransactionqueues WHERE TimeStamp='" + Timestamp + "' and trnrefno= {0})", TrnRefNo).FirstOrDefault();
                if (charge == null) //ntrivedi null condition 26-02-2019
                {
                    return 0;
                }
                return charge.TotalBalance;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("FindChargeValue", "WalletRepository", ex);
                throw ex;
            }
        }

        public string FindChargeCurrencyDeduct(long TrnRefNo)
        {
            try
            {
                HelperForLog.WriteLogIntoFileAsync("FindChargeCurrencyDeduct", "Get walletid and currency walletid=" + "TrnRefNo: " + TrnRefNo.ToString());
                var chargeWalletType = _dbContext.ChargeCurrency.FromSql("select top 1 ISnull(wt.WalletTypeName,'') as Name from Trnchargelog tc inner join ChargeConfigurationDetail cd on cd.Id=tc.ChargeConfigurationDetailID inner join WalletTypeMasters wt on wt.Id=cd.DeductionWalletTypeId where trnrefno={0} order by tc.id desc", TrnRefNo).FirstOrDefault();
                if (chargeWalletType == null)
                {
                    return "";
                }
                return chargeWalletType.Name;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("FindChargeCurrencyDeduct", "WalletRepository", ex);
                throw ex;
            }
        }

        public TransactionPolicyRes ListTransactionPolicy(long TrnType, long userId)
        {
            try
            {
                int IsKYCEnable = 0;
                var obj = (from p in _dbContext.PersonalVerification
                           where p.UserID == userId
                           select p.VerifyStatus).FirstOrDefault();
                if (obj != null)
                {
                    IsKYCEnable = obj;
                }
                var items = _dbContext.TransactionPolicyRes.FromSql(@"SELECT  t.IsKYCEnable,t.Id ,w.TrnTypeName AS StrTrnType,t.Status,CASE t.Status WHEN 1 THEN 'Enable' WHEN 0 THEN 'Disable' ELSE 'Deleted' END AS StrStatus,t.TrnType,t.AllowedIP,t.AllowedLocation,t.AuthenticationType,t.StartTime,t.EndTime,t.DailyTrnCount,t.DailyTrnAmount,t.MonthlyTrnCount,t.MonthlyTrnAmount,t.WeeklyTrnCount,t.WeeklyTrnAmount,t.YearlyTrnCount,t.YearlyTrnAmount,t.MinAmount,t.MaxAmount,t.AuthorityType,t.AllowedUserType,r.Id as RoleId,r.RoleType as RoleName FROM TransactionPolicy t inner join  WTrnTypeMaster w ON w.TrnTypeId = t.TrnType inner join UserRoleMaster r on r.Id=t.RoleId WHERE t.Status < 9 and t.TrnType={0} and t.IsKYCEnable={1}", TrnType, IsKYCEnable).FirstOrDefault();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<WalletType> GetChargeWalletType(long? WalletTypeId)
        {
            try
            {
                var _WalletType = (from w in _dbContext.ChargeConfigurationMaster
                                   join wt in _dbContext.WalletTypeMasters
                                   on w.WalletTypeID equals wt.Id
                                   where wt.Status == 1 && (WalletTypeId == null || (w.WalletTypeID == WalletTypeId && WalletTypeId != null))
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

        public List<ChargesTypeWise> ListChargesTypeWise(long WalletTypeId, long? TrntypeId)
        {
            List<ChargesTypeWise> Resp = new List<ChargesTypeWise>();
            try
            {
                var ChargeData = _dbContext.ChargesTypeWise.FromSql("SELECT cd.ChargeValue,wtm.WalletTypeName as DeductWalletTypeName,wt.TrnTypeName, cd.MakerCharge, cd.TakerCharge, wt.TrnTypeId FROM ChargeConfigurationDetail cd inner join ChargeConfigurationMaster cm ON cm.id = cd.ChargeConfigurationMasterID INNER JOIN WTrnTypeMaster wt ON wt.TrnTypeId = cm.TrnType inner join WalletTypemasters wtm on wtm.id = cd.DeductionWalletTypeId WHERE cm.TrnType in (3, 8,9) AND cd.Status = 1 AND cm.Status = 1 AND cm.WalletTypeID ={0} and(cm.TrnType ={1} or {1}= 0)", WalletTypeId, (TrntypeId == null ? 0 : Convert.ToInt64(TrntypeId))).ToList();

                return ChargeData;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public enTransactionStatus CheckTransactionSuccessOrNot(long TrnRefNo)
        {
            try
            {
                IQueryable<CheckTransactionSuccessOrNotRes> Result =
                    _dbContext.CheckTransactionSuccessOrNotRes.FromSql("SELECT Status from wallettransactionqueues where trnrefno={0} and TrnType=1 and wallettrntype in(3,8) ", TrnRefNo);

                return Result.FirstOrDefault().Status;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        //komal 3-07-2019 get Settled Qty
        public Decimal GetTransactionSettledQty(long TrnRefNo)
        {
            try
            {
                IQueryable<GetTransactionSettledQty> Result =
                    _dbContext.GetTransactionSettledQty.FromSql("SELECT SettedAmt as SettledAmt from wallettransactionqueues where trnrefno={0} and TrnType=1 and wallettrntype in(3,8) ", TrnRefNo);

                return Result.FirstOrDefault().SettledAmt;
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
            {
                IQueryable<WalletTransactionQueue> Result = _dbContext.WalletTransactionQueues.FromSql("select * from wallettransactionqueues where trnrefno={0} and " +
                                                                                "timestamp in(select timestamp from wallettransactionqueues where trnrefno={1}) and status=1 and wallettrntype in(3,8)  ", TakerTrnNo, MakerTrnNo);
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

        public List<UserUnstakingReq2> GetStakingdataForChrone()
        {
            try
            {
                DateTime FromDate = Helpers.UTC_To_IST().Date;
                DateTime ToDate = FromDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                string Query = "SELECT Id,UserID,StakingPolicyDetailID,ChannelID,StakingAmount,MaturityDate,EnableAutoUnstaking FROM TokenStakingHistory WHERE Status IN(1,4) AND EnableAutoUnstaking = 1 AND MaturityDate>={0} AND MaturityDate<={1}";
                var data = _dbContext.UserUnstakingReq2.FromSql(Query, FromDate, ToDate);
                return data.ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ValidationWithdrawal CheckActivityLog(long UserId, int Type)
        {
            try
            {
                var data = _dbContext.ValidationWithdrawal.FromSql("SELECT TOP(1) ActivityDate as Date FROM ActivityTypeLog WHERE userid={0} and ActivityType={1} ORDER BY ActivityDate DESC", UserId, Type).FirstOrDefault();
                return data;
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
                var items = _dbContext.WalletMasterResponsev2.FromSql(@"select u.id, u.AccWalletID,u.ExpiryDate,ISNULL(u.OrgID,0) AS OrgID,u.Walletname as WalletName,c.WalletTypeName as CoinName,u.PublicAddress,u.Balance,u.IsDefaultWallet,u.InBoundBalance,u.OutBoundBalance from WalletMasters u inner join WalletTypeMasters c on c.Id= u.WalletTypeID where u.Status < 9 AND Walletusagetype=0 and  u.UserID={0} and c.WalletTypeName ={1}", UserId, coin).ToList();
                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //Rushabh 13-12-2019 Added New Method As Per Client's New Requirement
        public BalanceLat GetAllBalancesV1Lat(long userid, long walletid)
        {

            try
            {
                var items = _dbContext.BalanceLat.FromSql(@"select ISNULL((select (sum(wtq.Amount)) from WalletTransactionQueues wtq inner join WalletMasters w on w.id=wtq.WalletID where WalletID ={0} AND MemberID = {1} AND (wtq.Status=4 or wtq.Status=6)),0 )as UnSettledBalance ,ISnull((select sum(w.Balance) from WalletAuthorizeUserMaster wa inner join WalletMasters w on w.Id=wa.WalletID join WalletTypeMasters wt on wt.Id=w.WalletTypeId where wa.WalletID = {0} and wa.UserID = {1} and wa.Status =1),0) as AvailableBalance,ISNULL((select sum(w.Amount) from DepositHistory w inner join AddressMasters wt on wt.Address=w.Address where w.Id = {0} and w.UserID = {1} and w.Status =0),0 )as UnClearedBalance,ISNULL((select sum(u.ShadowAmount) from MemberShadowBalance u inner join WalletMasters w on w.Id=u.WalletID inner join WalletTypeMasters wt on wt.Id= u.WalletTypeId where w.Id = {0} and w.UserID = {1} and w.Status =0),0) as ShadowBalance,ISNULL((select SUM(StakingAmount) from TokenStakingHistory where WalletID={0} and UserId={1} And Status in (1,4)),0) as StackingBalance", walletid, userid).First();

                var btcBala = _dbContext.BalanceTotal.FromSql("SELECT Balance as TotalBalance FROM WalletMasters C Inner Join WalletTypeMasters W On W.Id = C.WalletTypeId And W.IsdefaultWallet = 1 Where C.Id ={0}", walletid).FirstOrDefault();

                if (btcBala != null)
                {
                    if (btcBala.TotalBalance > 0)
                    {
                        items.BTCAvailableBalance = items.AvailableBalance;
                    }
                    else
                    {
                        items.BTCAvailableBalance = 0;
                    }
                }
                else
                {
                    var t = _dbContext.BalanceTotal.FromSql("select ISNULL(cast(Round((sum(w.Balance*ts.LTP)),18) as decimal (28,18)),0) as TotalBalance from WalletMasters w inner join  wallettypemasters wt on wt.id = w.wallettypeid inner join servicemaster s on s.wallettypeid =wt.id inner join Tradepairmaster t on t.SecondaryCurrencyId=s.id inner join TradePairStastics ts on ts.PairId=t.id where w.id={0}  and t.basecurrencyid=(select s.id from wallettypemasters wt inner join servicemaster s on s.wallettypeid =wt.id where wt.IsDefaultWallet=1)", walletid).FirstOrDefault();
                    if (t == null)
                    {
                        var Rate = _dbContext.BalanceTotal.FromSql("SELECT ISNULL(CurrentRate,0) as TotalBalance FROM CurrencyRateMaster C Inner Join WalletTypeMasters W On W.WalletTypeName=C.CurrencyName And W.IsdefaultWallet=1").FirstOrDefault();
                        if (Rate != null)
                        {
                            items.BTCAvailableBalance = Rate.TotalBalance * items.AvailableBalance;
                        }
                        else
                        {
                            items.BTCAvailableBalance = 0;
                        }
                    }
                    else
                    {
                        items.BTCAvailableBalance = t.TotalBalance;
                    }
                }
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
