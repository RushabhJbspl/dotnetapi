using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Worldex.Web.Helper
{
    public class HelperForLog
    {
        //vsolanki 17-10-2018
        public static void WriteLogIntoFile(byte CheckReq_Res, DateTime Date, string MethodName, string Controllername, string Req_Res = null, string accessToken = null)
        {
            string IgnoreMethodsForLog = "/api/TransactionConfiguration/GetAllServiceConfiguration,/api/Transaction/GetRecentOrder,/api/Transaction/GetOpenOrder,/api/Transaction/GetSellerBook,/api/Transaction/GetBuyerBook,/api/Transaction/GetOrderhistory,/api/Transaction/GetActiveOrder,/api/Wallet/ListWallet,/api/Transaction/GetTradePairAsset," +
                "/api/TransactionBackOffice/PairTradeSummary,/api/TransactionBackOffice/TradingSummary,/api/TransactionBackOffice/TransactionChargeSummary,/Chat,/chat/negotiate,/Chat/negotiate,"+
                "/api/Transaction/GetTradeHistory,/api/Transaction/TradeSettledHistory,/api/TransactionBackOffice/TradeSettledHistory,"+
                "/api/TransactionConfiguration/ListPair,/api/Manage/GetLoginHistory,/api/WalletControlPanel/GetDetailTypeWise," +
                "/api/ProfileConfiguration/GetProfileConfiguration,/api/TransactionConfiguration/GetAllRouteConfiguration,/api/Transaction/GetMarketTicker," +
                "/api/TransactionConfiguration/ListCurrency,/api/WalletControlPanel/ListWalletTypeDetails,/Chat,/api/TransactionBackOfficeCount/GetTradeSummaryCount," +
                "/api/TransactionConfiguration/GetAllPairConfiguration,/api/TransactionConfiguration/GetAllServiceConfigurationData,/connect/token," +
                "/api/APIConfiguration/ViewUserActivePlan,/api/MarginWalletControlPanel/ListLeverage,/api/WalletControlPanel/ListChargesTypeWise,/api/APIConfiguration/ViewAPIPlanDetail," +
                "/api/Referral/ListAdminReferralChannelWithChannelType,/api/Manage/userinfo,/api/Affiliate/GetAffiliatePromotionLink,/api/BackOfficeComplain/GetAllUserData," +
                "/api/Wallet/GetAvailbleBalTypeWise,/api/WalletControlPanel/ListAllUserDetails";

            //Uday 05-03-2019 Comment This Code Because More log has been write so stop the unwantned log
            //if (!(IgnoreMethodsForLog.Contains(Controllername) || Controllername.Contains("GetBuyerBook") || Controllername.Contains("GetSellerBook")
            //    || Controllername.Contains("GetFrontTopGainerPair") || Controllername.Contains("GetLoginHistory") || Controllername.Contains("GetFrontTopLooserPair")|| Controllername.Contains("GetFrontTopLooserGainerPair")
            //    || Controllername.Contains("GetTopGainerPair") || Controllername.Contains("GetTopLooserPair") || Controllername.Contains("GetTopLooserGainerPair")
            //    || Controllername.Contains("GetGraphDetail") || Controllername.Contains("GetBackOfficeGraphDetail")
            //    || Controllername.Contains("AdminAssets")))
                //Req_Res = null;
            
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
               if (!(IgnoreMethodsForLog.Contains(Controllername) || Controllername.Contains("GetBuyerBook") || Controllername.Contains("GetSellerBook")
               || Controllername.Contains("GetFrontTopGainerPair") || Controllername.Contains("GetLoginHistory") || Controllername.Contains("GetFrontTopLooserPair") || Controllername.Contains("GetFrontTopLooserGainerPair")
               || Controllername.Contains("GetTopGainerPair") || Controllername.Contains("GetTopLooserPair") || Controllername.Contains("GetTopLooserGainerPair")
               || Controllername.Contains("GetGraphDetail") || Controllername.Contains("GetBackOfficeGraphDetail")
               || Controllername.Contains("AdminAssets")))
               {
                    if (CheckReq_Res == 1)
                    {
                        //Req_Res = null;
                        if (accessToken != null)
                            logger.Info(Environment.NewLine + "\nDate:" + Date + ",MethodName:" + MethodName + ", Controllername: " + Controllername + Environment.NewLine + "AccessToken: " + "" + Environment.NewLine + "Request: " + Req_Res + Environment.NewLine + "===================================================================================================================");
                        else
                        {
                            if (Controllername.Trim() != "/Market")
                                logger.Info(Environment.NewLine + "\nDate:" + Date + ",MethodName:" + MethodName + ", Controllername: " + Controllername + Environment.NewLine + "Request: " + Req_Res + Environment.NewLine + "===================================================================================================================");
                        }

                    }
                    else //if(CheckReq_Res==2)
                    {
                        if (Controllername.Trim() != "/Market")
                            logger.Info(Environment.NewLine + "Date:" + Date + ", MethodName:" + MethodName + ", Controllername: " + Controllername + Environment.NewLine + "Response: " + Req_Res + Environment.NewLine + "===================================================================================================================");
                    }
               }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        public static void WriteErrorLog(DateTime Date, string MethodName, string Controllername, string Error)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                logger.Error(Environment.NewLine + "Date:" + Date + ", MethodName:" + MethodName + ",Controllername: " + Controllername + "\nError: " + Error + Environment.NewLine + "===================================================================================================================");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
