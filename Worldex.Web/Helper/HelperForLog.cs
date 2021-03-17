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
                "/api/TransactionBackOffice/PairTradeSummary,/api/TransactionBackOffice/TradingSummary,/api/TransactionBackOffice/TransactionChargeSummary,/Chat,/chat/negotiate,/Chat/negotiate,/Market/negotiate," +
                "/api/Transaction/GetTradeHistory,/api/Transaction/TradeSettledHistory,/api/TransactionBackOffice/TradeSettledHistory,"+
                "/api/TransactionConfiguration/ListPair,/api/Manage/GetLoginHistory,/api/WalletControlPanel/GetDetailTypeWise," +
                "/api/ProfileConfiguration/GetProfileConfiguration,/api/TransactionConfiguration/GetAllRouteConfiguration,/api/Transaction/GetMarketTicker," +
                "/api/TransactionConfiguration/ListCurrency,/api/WalletControlPanel/ListWalletTypeDetails,/Chat,/api/TransactionBackOfficeCount/GetTradeSummaryCount," +
                "/api/TransactionConfiguration/GetAllPairConfiguration,/api/TransactionConfiguration/GetAllServiceConfigurationData,/connect/token," +
                "/api/APIConfiguration/ViewUserActivePlan,/api/MarginWalletControlPanel/ListLeverage,/api/WalletControlPanel/ListChargesTypeWise,/api/APIConfiguration/ViewAPIPlanDetail,/api/WalletControlPanel/ListChargeConfiguration,/api/WalletControlPanel/ListChargeConfigurationDetail," +
                "/api/Referral/ListAdminReferralChannelWithChannelType,/api/Manage/userinfo,/api/Affiliate/GetAffiliatePromotionLink,/api/BackOfficeComplain/GetAllUserData," +
                "/api/Wallet/GetAvailbleBalTypeWise,/api/WalletControlPanel/ListAllUserDetails,/api/BackofficeRoleManagement/GetAccessRightsByUserV1,/api/TransactionBackOffice/TradingReconHistory,/api/Chat/GetUserList,/api/WalletControlPanel/GetStackingHistory,/api/WalletOperations/GetStackingHistory,/api/MarginWallet/ListMarginWalletMaster,/api/MarginWallet/ListAllWalletTypeMaster" +
                "/api/Transaction/GetTradeHistoryArbitrage,/api/Transaction/GetActiveOrderArbitrage,/api/Transaction/GetRecentOrderArbitrage,/api/Transaction/GetOrderhistoryArbitrage"+
                "/api/Transaction/GetTradePairAssetArbitrage,/api/Transaction/ExchangeProviderListArbitrage,/api/Transaction/GetProfitIndicatorArbitrage"+
                "/api/Transaction/SmartArbitrageHistory,/api/TransactionBackOffice/TradingSummaryArbitrage,/api/TransactionBackOffice/TradingSummaryLPWiseArbitrage"+
                "/api/TransactionBackOffice/TradeSettledHistoryArbitrage,/api/TransactionBackOffice/ChangeTradingConfigurationStatus,/api/Wallet/GetTradingChartData,/api/Wallet/AddTradingChartData,/api/ArbitrageWallet/ListCurrency,/api/ArbitrageWallet/ListWalletMaster," +
                "/api/Transaction/GetFavouritePair,/api/BackOffice/GetComplaintPriority,/api/Wallet/ListChargesTypeWise,/api/WalletOperations/GetAllBeneficiaries,/api/Manage/DayNightModePreference,/api/MarginWallet/ListLeverageBaseCurrency,/api/FiatIntegration/ListFiatCurrency";

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
               || Controllername.Contains("GetGraphDetail") || Controllername.Contains("GetBackOfficeGraphDetail") || Controllername.Contains("GetBuyerBookArbitrage") || Controllername.Contains("GetSellerBookArbitrage")
               || Controllername.Contains("AdminAssets") || Controllername.Contains("GetGraphDetailArbitrage") || Controllername.Contains("ExchangeListSmartArbitrage")|| Controllername.Contains("ExchangeProviderListArbitrage")
               || Controllername.Contains("GetStackingPolicyDetail")|| Controllername.Contains("GetIpAddress")|| Controllername.Contains("/api/Manage/GetDeviceId/")|| Controllername.Contains("/api/Manage/GetIpHistory")
                || Controllername.Contains("/api/Wallet/GetAllBalances/") || Controllername.Contains("/api/Wallet/GetWalletByType/") || Controllername.Contains("/api/Wallet/GetServiceLimitChargeValue/") || Controllername.Contains("/api/WalletOperations/GetWhitelistedBeneficiaries/")
               || Controllername.Contains("/api/Transaction/GetProfitIndicatorArbitrage/") || Controllername.Contains("/api/Transaction/GetMarketCap/") || Controllername.Contains("/api/WalletOperations/GetDefaultWalletAddress/")))
               {
                    if (CheckReq_Res == 1)
                    {
                        //Req_Res = null;
                        if (accessToken != null)
                            logger.Info(Environment.NewLine + "\nDate:" + Date + ",MethodName:" + MethodName + ", Controllername: " + Controllername + Environment.NewLine + "AccessToken: " + "" + Environment.NewLine + "Request: " + Req_Res + Environment.NewLine + "===================================================================================================================");
                        else
                        {
                            if (Controllername.Trim() != "/Market" && !Controllername.Contains("/api/Signin/StandardLoginV1"))
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
