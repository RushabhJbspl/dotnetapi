using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.FiatBankIntegration;

namespace Worldex.Infrastructure.Data
{
    public class FiatBankIntegrationRepository : IFiatBankIntegrationRepository
    {
        public static string ControllerName = "FiatBankIntegrationRepository";
        private WorldexContext _dbContext;
        public FiatBankIntegrationRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<FiatTradeConfigurationRes> ListFiatTradeConfiguration(short? status)
        {
            try
            {
                List<FiatTradeConfigurationRes> Res = new List<FiatTradeConfigurationRes>();
                Res = _dbContext.FiatTradeConfigurationRes.FromSql(@"SELECT Id,BuyFee,SellFee,TermsAndCondition,IsBuyEnable,IsSellEnable,BuyFeeType,
                                                        ISNULL(Platform,'-')AS Platform,ISNULL(SellCallBackURL,'-')AS SellCallBackURL,
                                                        ISNULL(WithdrawURL,'-')AS WithdrawURL,ISNULL(FiatCurrencyName,'-')AS FiatCurrencyName,
                                                        FiatCurrencyRate,MaxLimit,MinLimit,
                                                        CASE BuyFeeType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Percentage' ELSE 'Unknown' END AS 'StrBuyFeeType',
                                                        SellFeeType,CASE SellFeeType WHEN 1 THEN 'Fixed' WHEN 2 THEN 'Percentage' ELSE 'Unknown' END AS 'StrSellFeeType',
                                                        BuyNotifyURL,SellNotifyURL,CallBackURL,EncryptionKey,Status,CreatedDate,
                                                        CASE Status WHEN 0 THEN 'InActive' WHEN 1 THEN 'Active' WHEN 9 THEN 'Delete' ELSE 'Unknown' END AS 'StrStatus' 
                                                        FROM FiatTradeConfigurationMaster WHERE (Status={0} OR {0}=999)"
                                                        , status == null ? 999 : status).ToListAsync().GetAwaiter().GetResult();
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListFiatTradeConfiguration", ControllerName, ex);
                throw ex;
            }
        }

        public List<GetUserBankReq> ListUserBankDetail(short? status, short? RequestType, long UserId)
        {
            try
            {
                List<GetUserBankReq> Res = new List<GetUserBankReq>();
                //Res = _dbContext.GetUserBankReq.FromSql(@"SELECT Isnull(UB.Remarks,'') as Remarks,UB.GUID AS 'BankID',UB.UserId,UB.BankName,UB.BankCode,UB.BankAccountNumber,UB.BankAcountHolderName,UB.CreatedDate,
                //                                        UB.CurrencyCode,UB.CountryCode,ISNULL(BU.UserName,'-') AS 'UserName',UB.Status,
                //                                        CASE UB.Status WHEN 0 THEN 'Pending' WHEN 1 THEN 'Approved' WHEN 9 THEN 'Rejected' ELSE 'Unknown' END AS 'StrStatus',
                //                                        UB.RequestType,CASE UB.RequestType WHEN 1 THEN 'Added' WHEN 2 THEN 'Updated' ELSE '-' END AS 'RequestTypeName'
                //                                        FROM UserBankRequest UB LEFT JOIN BizUser BU ON UB.UserId = BU.Id 
                //                                        WHERE (UB.Status = {0} OR {0}=999) AND (UB.UserId={1} OR {1}=0) AND (UB.RequestType={2} OR {2}=0) ORDER BY UB.CreatedDate DESC",
                //                                        status == null ? 999 : status, UserId, RequestType == null ? 0 : RequestType).ToListAsync().GetAwaiter().GetResult();
                Res = _dbContext.GetUserBankReq.FromSql(@"SELECT case Isnull(P.Verifystatus,0) when 1 then 1 else 0 end as IsKycCompleted,isnull(BU.CountryCode,'') as UserCountryCode,ISNULL(BU.Mobile,'') as Mobile,isnull(BU.LastName,'') as LastName,isnull(BU.FirstName,'') as FirstName,BU.IsEnabled,BU.TwoFactorEnabled,isnull(BU.Email,'') as Email,BU.EmailConfirmed,Isnull(UB.Remarks,'') as Remarks,UB.GUID AS 'BankID',UB.UserId,UB.BankName,UB.BankCode,UB.BankAccountNumber,UB.BankAcountHolderName,UB.CreatedDate,
UB.CurrencyCode,UB.CountryCode,ISNULL(BU.UserName,'-') AS 'UserName',UB.Status,
CASE UB.Status WHEN 0 THEN 'Pending' WHEN 1 THEN 'Approved' WHEN 9 THEN 'Rejected' ELSE 'Unknown' END AS 'StrStatus',
UB.RequestType,CASE UB.RequestType WHEN 1 THEN 'Added' WHEN 2 THEN 'Updated' ELSE '-' END AS 'RequestTypeName'
FROM UserBankRequest UB INNER JOIN BizUser BU ON UB.UserId = BU.Id LEFT JOIN PersonalVerification P on P.Userid=BU.Id
                                                        WHERE (UB.Status = {0} OR {0}=999) AND (UB.UserId={1} OR {1}=0) AND (UB.RequestType={2} OR {2}=0) ORDER BY UB.CreatedDate DESC",
                                                      status == null ? 999 : status, UserId, RequestType == null ? 0 : RequestType).ToListAsync().GetAwaiter().GetResult();
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListUserBankDetail", ControllerName, ex);
                throw ex;
            }
        }

        public List<FiatCoinConfigurationRes> ListFiatConfiguration(long? FromCurrencyId, long? ToCurrencyId, short? Status, short? TransactionType)
        {
            try
            {
                List<FiatCoinConfigurationRes> Res = new List<FiatCoinConfigurationRes>();
                Res = _dbContext.FiatCoinConfigurationRes.FromSql(@"SELECT F.TransactionType,F.Id,FromCurrencyId,ToCurrencyId,MinQty,MaxQty,MinAmount,MaxAmount,F.Rate,MinRate,W1.WalletTypeName AS FromCurrencyName,W2.WalletTypeName AS ToCurrencyName,F.Status FROM FiatCoinConfiguration F Inner Join WalletTypeMasters W1 ON W1.Id=F.FromCurrencyId INNER JOIN WalletTypeMasters W2 ON W2.Id=F.ToCurrencyId Where F.Status<9 AND W1.Status=1  AND W2.Status=1 AND (F.TransactionType={0} OR {0}=0) ", (TransactionType == null ? 0 : TransactionType), (FromCurrencyId == null ? Convert.ToInt64(0) : Convert.ToInt64(FromCurrencyId)), (ToCurrencyId == null ? Convert.ToInt64(0) : Convert.ToInt64(ToCurrencyId)), (Status == null ? Convert.ToInt16(0) : Convert.ToInt16(Status))).ToList();
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListFiatConfiguration", ControllerName, ex);
                throw ex;
            }
        }
    }
}
