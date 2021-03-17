using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.IEOWallet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Data
{
    public class IEOWalletRepository : IIEOWalletRepository
    {
        private readonly WorldexContext _dbContext;

        public IEOWalletRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<IEOWalletResponse> ListIEOWallet(Int16 Status)
        {
            List<IEOWalletResponse> items = new List<IEOWalletResponse>();
            string Query = string.Empty;
            try
            {
                if (Status == 1)
                {
                    Query = "select RM.Bonus,WTM.Id,WTM.WalletTypeName as 'CurrencyName',WTM.Description as 'CurrencyFullName',CM.Description,RM.BGPath AS IconPath,RM.Status as 'Status',CM.IEOTokenTypeName,RM.TotalSupply,RM.AllocatedSupply,RM.MinimumPurchaseAmt,RM.MaximumPurchaseAmt,CM.Rate AS CurrencyRate,RM.GUID as 'RoundID',RM.StartDate,RM.EndDate,RM.OccurrenceLimit from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName=WTM.WalletTypeName Inner Join IEORoundMaster RM On RM.IEOCurrencyId=CM.Id And dbo.GetISTDate() Between RM.StartDate And RM.EndDate And CM.Status=1 WHERE RM.Status in (1)";
                     items = _dbContext.IEOWalletResponse.FromSql(Query, 1).ToList();
                }
                    
                else if (Status == 9)
                {
                    Query = "select RM.Bonus,WTM.Id,WTM.WalletTypeName as 'CurrencyName',WTM.Description as 'CurrencyFullName',CM.Description,RM.BGPath AS IconPath,RM.Status as 'Status',CM.IEOTokenTypeName,RM.TotalSupply,RM.AllocatedSupply,RM.MinimumPurchaseAmt,RM.MaximumPurchaseAmt,CM.Rate AS CurrencyRate,RM.GUID as 'RoundID',RM.StartDate,RM.EndDate,RM.OccurrenceLimit from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName=WTM.WalletTypeName Inner Join IEORoundMaster RM On RM.IEOCurrencyId=CM.Id And dbo.GetISTDate() > RM.EndDate And WTM.Id NOT in(select WTM.Id from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName = WTM.WalletTypeName Inner Join IEORoundMaster RM On RM.IEOCurrencyId = CM.Id And dbo.GetISTDate() Between RM.StartDate And RM.EndDate And CM.Status=1) WHERE RM.Status=9";
                     items = _dbContext.IEOWalletResponse.FromSql(Query, 1).ToList();
                }
                    
                else
                {
                    //Query = "select WTM.Id,WTM.WalletTypeName as 'CurrencyName',WTM.Description as 'CurrencyFullName',CM.Description,RM.BGPath AS IconPath,RM.Status as 'Status',CM.IEOTokenTypeName,RM.TotalSupply,RM.AllocatedSupply,RM.MinimumPurchaseAmt,RM.MaximumPurchaseAmt,CM.Rate AS CurrencyRate,RM.Bonus,RM.GUID as 'RoundID',RM.StartDate,RM.EndDate,RM.OccurrenceLimit from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName=WTM.WalletTypeName Inner Join IEORoundMaster RM On RM.IEOCurrencyId=CM.Id And dbo.GetISTDate() Between RM.StartDate And RM.EndDate And CM.Status=1 WHERE RM.Status in (1,9) Union select WTM.Id,WTM.WalletTypeName as 'CurrencyName',WTM.Description as 'CurrencyFullName',CM.Description,RM.BGPath AS IconPath,RM.Status as 'Status',CM.IEOTokenTypeName,RM.TotalSupply,RM.AllocatedSupply,RM.MinimumPurchaseAmt,RM.MaximumPurchaseAmt,RM.Bonus,CM.Rate AS CurrencyRate,RM.GUID as 'RoundID',RM.StartDate,RM.EndDate,RM.OccurrenceLimit from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName=WTM.WalletTypeName Inner Join IEORoundMaster RM On RM.IEOCurrencyId=CM.Id And dbo.GetISTDate() > RM.EndDate  And WTM.Id NOT in(select WTM.Id from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName=WTM.WalletTypeName Inner Join IEORoundMaster RM On RM.IEOCurrencyId=CM.Id And dbo.GetISTDate() Between RM.StartDate And RM.EndDate And CM.Status=1) WHERE RM.Status in (1,9)";

                     items = _dbContext.IEOWalletResponse.FromSql("Sp_GetIEOListWallet").ToList();
                }
                    

                //  var items = _dbContext.IEOWalletResponse.FromSql(Query, 1).ToList();
                foreach (IEOWalletResponse iEOWalletResponse in items)
                {
                    //var Items = _dbContext.PurchaseWallets.FromSql(@"select WTM.Id as 'PurchaseWalletID',WTM.WalletTypeName as 'PurchaseWalletName',(CRM.CurrentRate/PWM.PurchaseRate) as 'PurchaseRate',PWM.ConvertCurrencyType as 'CurrencyConvertType',CASE PWM.ConvertCurrencyType When 1 Then 'Purchase Rate' When 2 Then 'Pair' Else 'USD' END as 'CurrencyConvertTypeName',PWM.InstantPercentage,CRM.CurrentRate as 'USDRate' from IEOCurrencyPairMapping PWM Inner Join WalletTypeMasters WTM On WTM.Id=PWM.PaidWalletTypeId Inner Join WalletTypeMasters WTMW On WTMW.Id=PWM.IEOWalletTypeId Inner Join CurrencyRateMaster CRM On CRM.WalletTypeID=WTM.Id Inner Join IEORoundMaster RM On RM.Guid= Where WTMW.Id={0} And PWM.Status=1 And WTM.Status=1", iEOWalletResponse.Id).ToList();
                    var Items = _dbContext.PurchaseWallets.FromSql(@"select WTMP.Id as 'PurchaseWalletID',WTMP.WalletTypeName as 'PurchaseWalletName',CASE CPM.ConvertCurrencyType WHEN 1 THEN (ISNULL(CRM.CurrentRate,0)/CPM.PurchaseRate) WHEN 0 THEN CPM.PurchaseRate END as 'PurchaseRate',CPM.ConvertCurrencyType as 'CurrencyConvertType',CASE CPM.ConvertCurrencyType When 0 Then 'Purchase Rate' When 1 Then 'USD' Else 'N/A' END as 'CurrencyConvertTypeName',CPM.InstantPercentage,ISNULL(CRM.CurrentRate,0) as 'USDRate' from IEOCurrencyPairMapping CPM Inner Join IEORoundMaster RM On RM.Id=CPM.RoundID Inner Join WalletTypeMasters WTMP On WTMP.Id=CPM.PaidWalletTypeId Inner join IEOCUrrencyMaster WTMD On WTMD.Id=CPM.IEOWalletTypeID inner Join CurrencyRateMaster CRM On CRM.WalletTypeID=WTMP.Id Where RM.Guid={0} And CPM.Status=1 And WTMP.Status=1", iEOWalletResponse.RoundID).ToList();
                    iEOWalletResponse.PurchaseWallets = Items;

                    var ItemsAllocation = _dbContext.Allocation.FromSql(@"Select SM.Value as 'AllocationValue',SM.Duration from IEORoundMaster RM Inner Join IEOSlabMaster SM On SM.RoundID=RM.Id Where RM.Guid={0} And SM.Status=1 Order by SM.DUration", iEOWalletResponse.RoundID).ToList();
                    iEOWalletResponse.Allocation = ItemsAllocation;
                }

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<IEOPurchaseHistoryResponse> ListPurchaseHistory(DateTime FromDate, DateTime ToDate, int Page, int PageSize, long PaidCurrency, long DeliveryCurrency, int UserID, ref int TotalCount)
        {
            string Query = string.Empty;
            try
            {
                Query = "Select MaximumDeliveredQuantiy as MaxDeliveryQuantity,BonusAmount as BonusAmount,BonusPercentage as BonusPercentage,MaximumDeliveredQuantiyWOBonus as MaxDeliveryQuantityWOBonus,Ph.Id,PH.PaidQuantity,PH.MaximumDeliveredQuantiy as 'DeliveredQuantity',PH.PaidCurrency,PH.DeliveredCurrency,PH.RoundID,PH.InstantQuantity,SM.Value as 'InstantQuantityPer',PH.CreatedDate as 'TrnDate',PH.Rate as 'Rate' from IEOPurchaseHistory PH Inner Join IEORoundMaster RM On RM.Id=PH.RoundID Inner Join IEOSlabMaster SM On SM.RoundId=RM.Id And SM.Duration=0 Inner Join WalletTypeMasters WTMP On WTMP.WalletTypeName=PH.PaidCurrency Inner Join WalletTypeMasters WTMD On WTMD.WalletTypeName=PH.DeliveredCurrency  Where (PH.UserID={0} OR {0}=0) And PH.CreatedDate Between {1} And {2} And (WTMP.Id={3} OR 0={3}) And (WTMD.Id={4} OR 0={4})";
                var wl = _dbContext.IEOPurchaseHistoryResponse.FromSql(Query, UserID, FromDate, ToDate, PaidCurrency, DeliveryCurrency).ToList();

                TotalCount = wl.Count();
                if (Page > 0)
                {
                    int skip = PageSize * (Page - 1);
                    wl = wl.Skip(skip).Take(PageSize).ToList();
                }

                foreach (IEOPurchaseHistoryResponse IEOPurchaseHistoryResponse in wl)
                {
                    Query = "Select CASE CM.Status WHEN 0 Then 'Pending' WHEN 1 Then  'Success' ELSE 'N/A' END AS StrStatus ,CM.Status,CM.MaturityDate,CM.DeliveryQuantity,CM.DeliveryCurrency,CM.Guid as 'CMGUID',SM.Value as 'SlabPercentage' from IEOCronMaster CM INNER JOIN IEOSlabMaster SM ON SM.Id=CM.SlabID Where CM.IEOPurchaseHistoryId={0} ";//And CM.Status=1
                    var cm = _dbContext.IEOCronMasterResponse.FromSql(Query, IEOPurchaseHistoryResponse.Id).ToList();
                    IEOPurchaseHistoryResponse.IEOCronMasterList = cm;
                }

                return wl;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<IEOPurchaseHistoryResponseBO> ListPurchaseHistoryBO(DateTime FromDate, DateTime ToDate, int Page, int PageSize, long PaidCurrency, long DeliveryCurrency, string Email, ref int TotalCount)
        {
            string Query = string.Empty;
            try
            {
                Query = "Select MaximumDeliveredQuantiy as MaxDeliveryQuantity,BonusAmount as BonusAmount,BonusPercentage as BonusPercentage,MaximumDeliveredQuantiyWOBonus as MaxDeliveryQuantityWOBonus, B.Email,Ph.Id,PH.PaidQuantity,PH.MaximumDeliveredQuantiy as 'DeliveredQuantity',PH.PaidCurrency,PH.DeliveredCurrency,PH.RoundID,PH.InstantQuantity,SM.Value as 'InstantQuantityPer',PH.CreatedDate as 'TrnDate',PH.Rate as 'Rate' from IEOPurchaseHistory PH Inner Join IEORoundMaster RM On RM.Id=PH.RoundID Inner Join IEOSlabMaster SM On SM.RoundId=RM.Id And SM.Duration=0 Inner Join WalletTypeMasters WTMP On WTMP.WalletTypeName=PH.PaidCurrency Inner Join WalletTypeMasters WTMD On WTMD.WalletTypeName=PH.DeliveredCurrency INNER JOIN BizUser B ON B.Id =PH.UserID  Where (B.Email={0} OR {0}='') And PH.CreatedDate Between {1} And {2} And (WTMP.Id={3} OR 0={3}) And (WTMD.Id={4} OR 0={4}) order by PH.CreatedDate  desc";
                var wl = _dbContext.IEOPurchaseHistoryResponseBO.FromSql(Query, (Email == null ? "" : Email), FromDate, ToDate, PaidCurrency, DeliveryCurrency).ToList();

                TotalCount = wl.Count();
                if (Page > 0)
                {
                    int skip = PageSize * (Page - 1);
                    wl = wl.Skip(skip).Take(PageSize).ToList();
                }

                foreach (IEOPurchaseHistoryResponseBO IEOPurchaseHistoryResponse in wl)
                {
                    Query = "Select CASE CM.Status WHEN 0 Then 'Pending' WHEN 1 Then  'Success' ELSE 'N/A' END AS StrStatus ,CM.Status,CM.MaturityDate,CM.DeliveryQuantity,CM.DeliveryCurrency,CM.Guid as 'CMGUID',SM.Value as 'SlabPercentage' from IEOCronMaster CM INNER JOIN IEOSlabMaster SM ON SM.Id=CM.SlabID Where CM.IEOPurchaseHistoryId={0} ";//And CM.Status=1
                    var cm = _dbContext.IEOCronMasterResponse.FromSql(Query, IEOPurchaseHistoryResponse.Id).ToList();
                    IEOPurchaseHistoryResponse.IEOCronMasterList = cm;
                }

                return wl;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }


        public List<GetIEOAdminWalletRes> GetAdminWalletConfiguration(long UserId)
        {
            try
            {
                var data = _dbContext.GetIEOAdminWalletRes.FromSql("SELECT C.Rate,Cast(W.Balance As varchar(50)) as Balance,C.Id,C.IconPath AS WalletPath,C.IEOTokenTypeName AS CoinType,C.CurrencyName AS ShortCode,WT.Description AS WalletName,C.Description,C.Status,C.Rounds,W.AccWalletId AS AdminWalletId FROM IEOCurrencyMaster C INNER JOIN WalletTypeMasters WT ON WT.WalletTypeName=C.CurrencyName INNER JOIN WalletMasters W ON W.WalletTypeID=WT.Id AND  W.WalletUsageType=11 WHERE W.Status in (1,0) AND WT.Status in (1,0) AND C.Status in (1,0)").ToList();
                return data;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<IEORoundResponse> ListIEORounds(short Status)
        {
            string Query = string.Empty;
            try
            {
                //Query = "select WTM.Id,CM.Id as 'CMId',CM.Guid as 'CMGUID',WTM.WalletTypeName as 'CurrencyName',WTM.Description as 'CurrencyFullName',CM.Description,CM.IconPath,CM.Status,CASE CM.Status When 1 THEN 'Enabled' When 0 Then 'Disabled' ELSE 'Other' END as 'StatusStr',CM.IEOTokenTypeName,CM.Rounds from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName=WTM.WalletTypeName And CM.Status<>9 Where (CM.Status={0} OR 999={0})";
                //if (Status == 1)
                //    //Query = "select WTM.Id,CM.Guid as 'CMGUID',WTM.WalletTypeName as 'CurrencyName',WTM.Description as 'CurrencyFullName',CM.Description,CM.IconPath,1 as 'Status',CM.IEOTokenTypeName,CM.Rounds,RM.TotalSupply,RM.AllocatedSupply,RM.MinimumPurchaseAmt,RM.MaximumPurchaseAmt,RM.CurrencyRate,RM.GUID as 'RoundID',RM.StartDate,RM.EndDate,RM.OccurrenceLimit from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName=WTM.WalletTypeName Inner Join IEORoundMaster RM On RM.IEOCurrencyId=CM.Id And dbo.GetISTDate() Between RM.StartDate And RM.EndDate And CM.Status=1";
                //    Query = "select WTM.Id,CM.Guid as 'CMGUID',WTM.WalletTypeName as 'CurrencyName',WTM.Description as 'CurrencyFullName',CM.Description,CM.IconPath,CM.Status,CASE CM.Status When 1 THEN 'Enabled' When 9 Then 'Deleted' When 0 Then 'Disabled' ELSE 'Other' END as 'StatusStr',CM.IEOTokenTypeName,CM.Rounds from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName=WTM.WalletTypeName Where (CM.Status={0} OR 0={0})";
                //else if (Status == 9)
                //    Query = "select WTM.Id,CM.Guid as 'CMGUID',WTM.WalletTypeName as 'CurrencyName',WTM.Description as 'CurrencyFullName',CM.Description,CM.IconPath,9 as 'Status',CM.IEOTokenTypeName,CM.Rounds,RM.TotalSupply,RM.AllocatedSupply,RM.MinimumPurchaseAmt,RM.MaximumPurchaseAmt,RM.CurrencyRate,RM.GUID as 'RoundID',RM.StartDate,RM.EndDate,RM.OccurrenceLimit from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName=WTM.WalletTypeName Inner Join IEORoundMaster RM On RM.IEOCurrencyId=CM.Id And dbo.GetISTDate() > RM.EndDate And WTM.Id NOT in(select WTM.Id from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName = WTM.WalletTypeName Inner Join IEORoundMaster RM On RM.IEOCurrencyId = CM.Id And dbo.GetISTDate() Between RM.StartDate And RM.EndDate And CM.Status=1)";
                //else
                //    Query = "select WTM.Id,CM.Guid as 'CMGUID',WTM.WalletTypeName as 'CurrencyName',WTM.Description as 'CurrencyFullName',CM.Description,CM.IconPath,1 as 'Status',CM.IEOTokenTypeName,CM.Rounds,RM.TotalSupply,RM.AllocatedSupply,RM.MinimumPurchaseAmt,RM.MaximumPurchaseAmt,RM.CurrencyRate,RM.GUID as 'RoundID',RM.StartDate,RM.EndDate,RM.OccurrenceLimit from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName=WTM.WalletTypeName Inner Join IEORoundMaster RM On RM.IEOCurrencyId=CM.Id And dbo.GetISTDate() Between RM.StartDate And RM.EndDate And CM.Status=1 Union select WTM.Id,CM.Guid as 'CMGUID',WTM.WalletTypeName as 'CurrencyName',WTM.Description as 'CurrencyFullName',CM.Description,CM.IconPath,9 as 'Status',CM.IEOTokenTypeName,CM.Rounds,RM.TotalSupply,RM.AllocatedSupply,RM.MinimumPurchaseAmt,RM.MaximumPurchaseAmt,RM.CurrencyRate,RM.GUID as 'RoundID',RM.StartDate,RM.EndDate,RM.OccurrenceLimit from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName=WTM.WalletTypeName Inner Join IEORoundMaster RM On RM.IEOCurrencyId=CM.Id And dbo.GetISTDate() > RM.EndDate  And WTM.Id NOT in(select WTM.Id from WalletTypeMasters WTM Inner Join IEOCurrencyMaster CM On CM.CurrencyName=WTM.WalletTypeName Inner Join IEORoundMaster RM On RM.IEOCurrencyId=CM.Id And dbo.GetISTDate() Between RM.StartDate And RM.EndDate And CM.Status=1)";
                //var items = _dbContext.IEORoundResponse.FromSql(Query, Status).ToList();
                List<IEORoundResponse> items = _dbContext.RoundsMasterBack.FromSql(@"Select  CM.Id as 'CurrencyID',CM.CurrencyName,RM.Guid as 'RoundId',RM.StartDate as 'FromDate',RM.EndDate as 'ToDate',RM.Status,CASE RM.Status When 1 THEN 'Enabled' When 0 Then 'Disabled' ELSE 'Other' END as 'StatusStr',RM.TotalSupply,RM.MinimumPurchaseAmt as 'MinLimit',RM.MaximumPurchaseAmt as 'MaxLimit',RM.OccurrenceLimit as 'MaxOccurence',RM.Bonus,RM.BGPath from IEORoundMaster RM INNER JOIN IEOCurrencyMaster CM On CM.Id=RM.IEOCurrencyId Where RM.Status<>9 And (RM.Status={0} OR 999={0}) Order By CM.CurrencyName", Status).ToList();
                foreach (IEORoundResponse IEORoundResponse in items)
                {
                    var ItemsAllocation = _dbContext.AllocationBack.FromSql(@"Select SM.Guid as 'DetailId',SM.Value as 'AllocationPercentage',SM.Bonus,SM.Duration as 'AllocationNoofPeriod',SM.DurationType as 'AllocationPeriodType',CASE SM.DurationType WHEN 0 Then 'Instant' When 1 Then 'Days' When 2 Then 'Month' Else 'Other' End as 'AllocationPeriodTypeStr' from IEOSlabMaster SM Inner Join IEORoundMaster RM On RM.Id=SM.RoundId Where RM.Guid={0} And SM.Status in (0,1)", IEORoundResponse.RoundId).ToList();
                    IEORoundResponse.Allocation = ItemsAllocation;

                    var Items = _dbContext.PurchaseWalletsBack.FromSql(@"select CPM.Guid as 'ExchangeId',WTMP.WalletTypeName as 'PaidWalletName',WTMP.Id as 'PaidCurrencyId',CPM.ConvertCurrencyType as 'CurrencyConvertType',CASE CPM.ConvertCurrencyType When 0 Then 'Purchase Rate' When 1 Then 'USD' Else 'N/A' END as 'CurrencyConvertTypeName',CPM.PurchaseRate as 'Rate' from IEOCurrencyPairMapping CPM Inner Join IEORoundMaster RM On RM.Id=CPM.RoundId Inner Join WalletTypeMasters WTMP On WTMP.Id=CPM.PaidWalletTypeId Where RM.Guid={0} And CPM.Status in (0,1)", IEORoundResponse.RoundId).ToList();
                    IEORoundResponse.PurchaseWallets = Items;

                    //var ItemsRounds = _dbContext.RoundsMasterBack.FromSql(@"Select CM.CurrencyName,RM.Guid as 'ID',RM.StartDate,RM.EndDate,RM.Status,CASE RM.Status When 1 THEN 'Enabled' When 0 Then 'Disabled' ELSE 'Other' END as 'StatusStr',RM.TotalSupply,RM.MinimumPurchaseAmt,RM.MaximumPurchaseAmt,RM.CurrencyRate,RM.OccurrenceLimit,RM.Bonus,RM.BGPath from IEORoundMaster RM INNER JOIN IEOCurrencyMaster CM On CM.Id=RM.IEOCurrencyId Where RM.Status<>9 Order By CM.CurrencyName", 1).ToList();
                    //iEOWalletResponseBack.RoundDetails = ItemsRounds;

                    //foreach(RoundMasterBack roundMasterBack in iEOWalletResponseBack.RoundDetails)
                    //{
                    //    var ItemsAllocation = _dbContext.AllocationBack.FromSql(@"Select SM.Guid as 'ID',SM.Value as 'AllocationValue',SM.Bonus,SM.Duration,SM.DurationType,CASE SM.DurationType WHEN 1 Then 'Instant' When 2 Then 'Days' When 3 Then 'Month' Else 'Other' End as 'DurationTypeStr' from IEOSlabMaster SM Inner Join IEORoundMaster RM On RM.Id=SM.RoundId Where RM.Guid={0}", roundMasterBack.RMGUID).ToList();
                    //    roundMasterBack.Allocation = ItemsAllocation;
                    //}

                    //var Items = _dbContext.PurchaseWalletsBack.FromSql(@"select CPM.Guid as 'ID',WTMP.WalletTypeName as 'PurchaseWalletName',CPM.PurchaseRate,CPM.ConvertCurrencyType as 'CurrencyConvertType',CASE CPM.ConvertCurrencyType When 1 Then 'Purchase Rate' When 2 Then 'Pair' Else 'USD' END as 'CurrencyConvertTypeName',CPM.InstantPercentage from IEOCurrencyPairMapping CPM Inner Join IEORoundMaster RM On RM.Id=CPM.RoundId Inner Join WalletTypeMasters WTMP On WTMP.Id=CPM.PaidWalletTypeId Where RM.Guid={0} And CPM.Status=1", IEORoundResponse.ID).ToList();
                    //iEOWalletResponseBack.PurchaseWallets = Items;
                }

                return items;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ListIEOTokenReportDataRes IEOTokenReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, string Email, string PaidCurrency, string DeliveredCurrency, short? Status, string TrnRefNo)
        {
            ListIEOTokenReportDataRes res = new ListIEOTokenReportDataRes();
            try
            {
                string sql = "SELECT CASE H.Status WHEN 1 then 'Success' WHEN 5 THEN 'Comleted' ELSE 'N/A' END as StrStatus, H.Status,H.GUID AS TrnRefNo,PaidCurrency,DeliveredCurrency,B.Email,cast(DeliveredQuantity as varchar(50)) as DeliveredQuantity,CAST(PaidQuantity AS VARCHAR(50)) AS PaidQuantity,CAST((PaidQuantity / MaximumDeliveredQuantiy) AS VARCHAR(50)) AS Rate,CAST(MaximumDeliveredQuantiy AS VARCHAR(50)) AS MaximumDeliveredQuantiy ,H.CreatedDate AS Date,H.Remarks  FROM IEOPurchaseHistory H INNER JOIN BizUser B ON B.Id=H.UserId WHERE(B.Email={0} OR {0}='') AND (PaidCurrency={1} OR {1}='') AND (DeliveredCurrency={2} OR {2}='') AND (H.Status={3} OR {3}=999) AND (H.GUID={4} OR {4}='')";

                if (FromDate != null && ToDate != null)
                {
                    ToDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                    FromDate = Convert.ToDateTime(FromDate).AddHours(0).AddMinutes(0).AddSeconds(0);
                    sql = sql + " AND H.CreatedDate BETWEEN {5} AND {6}";
                }
                sql = sql + " Order By H.Id DESC";
                var data = _dbContext.IEOTokenReportDataRes.FromSql(sql, (Email == null ? "" : Email), (PaidCurrency == null ? "" : PaidCurrency), (DeliveredCurrency == null ? "" : DeliveredCurrency), (Status == null ? 999 : Status), (TrnRefNo == null ? "" : TrnRefNo), FromDate, ToDate).ToList();
                res.TotalCount = data.Count;
                if (PageNo > 0)
                {
                    int skip = PageSize * (PageNo - 1);
                    data = data.Skip(skip).Take(PageSize).ToList();
                }
                res.Data = data;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ListIEOAllocatedTokenReportDataRes IEOAllocatedTokenReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, string Email, string PaidCurrency, string DeliveredCurrency, short? Status, string TrnRefNo)
        {
            ListIEOAllocatedTokenReportDataRes res = new ListIEOAllocatedTokenReportDataRes();
            try
            {
                string sql = "SELECT CASE C.Status WHEN 1 then 'Success' WHEN 0 THEN 'Pending' ELSE 'N/A' END as StrStatus, C.Status,H.GUID AS TrnRefNo,PaidCurrency,DeliveredCurrency,B.Email,cast(DeliveryQuantity as varchar(50)) as DeliveredQuantity,CAST(PaidQuantity AS VARCHAR(50)) AS PaidQuantity,CAST((PaidQuantity / MaximumDeliveredQuantiy) AS VARCHAR(50)) AS Rate,CAST(MaximumDeliveredQuantiy AS VARCHAR(50)) AS MaximumDeliveredQuantiy ,C.MaturityDate AS Date,StatusMsg AS Remarks  FROM IEOPurchaseHistory H INNER JOIN BizUser B ON B.Id=H.UserId INNER JOIN IEOCronMaster C ON C.IEOPurchaseHistoryId=H.Id  WHERE (B.Email={0} OR {0}='') AND (PaidCurrency={1} OR {1}='') AND (DeliveredCurrency={2} OR {2}='') AND (C.Status={3} OR {3}=999) AND (H.GUID={4} OR {4}='')";
                if (FromDate != null && ToDate != null)
                {
                    ToDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                    FromDate = Convert.ToDateTime(FromDate).AddHours(0).AddMinutes(0).AddSeconds(0);
                    sql = sql + " AND C.MaturityDate BETWEEN {5} AND {6}";
                }
                sql = sql + " Order By C.Id DESC";
                var data = _dbContext.IEOAllocatedTokenReportDataRes.FromSql(sql, (Email == null ? "" : Email), (PaidCurrency == null ? "" : PaidCurrency), (DeliveredCurrency == null ? "" : DeliveredCurrency), (Status == null ? 999 : Status), (TrnRefNo == null ? "" : TrnRefNo), FromDate, ToDate).ToList();
                res.TotalCount = data.Count;
                if (PageNo > 0)
                {
                    int skip = PageSize * (PageNo - 1);
                    data = data.Skip(skip).Take(PageSize).ToList();
                }
                res.Data = data;
                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<AllocateTokenCountRes> IEOTokenCount(short IsAllocate)
        {
            try
            {
                if (IsAllocate == 1)
                {
                    var data = _dbContext.AllocateTokenCountRes.FromSql("select cast(sum(DeliveredQuantity) as varchar(50)) as QuantityTotal,DeliveredCurrency from IEOPurchaseHistory group by DeliveredCurrency").ToList();
                    return data;
                }
                else if (IsAllocate == 0)
                {
                    var data = _dbContext.AllocateTokenCountRes.FromSql("select cast(sum(MaximumDeliveredQuantiy) as varchar(50)) as QuantityTotal,DeliveredCurrency from IEOPurchaseHistory group by DeliveredCurrency").ToList();
                    return data;
                }
                else
                {
                    var data = new List<AllocateTokenCountRes>();
                    return data;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public List<TokenCountRes> IEOTradeTokenCount()
        {
            try
            {
                var data = _dbContext.TokenCountRes.FromSql("select cast(sum(PaidQuantity) as varchar(50)) as QuantityTotal,PaidCurrency,DeliveredCurrency from IEOPurchaseHistory group by PaidCurrency,DeliveredCurrency").ToList();
                return data;
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
    }
}
