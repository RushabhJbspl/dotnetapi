using System;
using System.Threading;
using System.Configuration;
using System.Timers;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Worldex.Infrastructure.Interfaces;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Worldex.Infrastructure.Data;
using Worldex.Core.Interfaces.Repository;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Worldex.Core.Interfaces;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Wallet;
using Worldex.Infrastructure.Data.Transaction;
using Worldex.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Worldex.Core.Services;
using Microsoft.AspNetCore.Http;
using Worldex.Core.Services.RadisDatabase;
using MediatR;
using Worldex.Infrastructure.BGTask;
using Worldex.Core.ViewModels;
using Worldex.Core.Interfaces.ControlPanel;
namespace DepositStatusCheckApp
{
    /// <summary>
    /// NOTE : Comment "TransactionTick.Start();" of CallAPI()'s finally statement for testing purpose 
    /// </summary>
    /// <param name="args"></param>

    class StatusCheckApp
    {
        static System.Timers.Timer TransactionTick = new System.Timers.Timer();
        public static IConfiguration Configuration { get; set; }
        static bool IsProcessing = false;
        static void Main(string[] args)
        {
            try
            {
                Console.Write("start");
                var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                TransactionTick.Elapsed += new ElapsedEventHandler(transaction_tick);
                TransactionTick.Start();
                Console.WriteLine("Press \'q\' to quit");
                while (Console.Read() != 'q') ;
                CommonFunctions.WriteRequestLog("Application Starts On : " + UTC_To_IST(), "transaction_tick", "");
            }
            catch (Exception ex)
            {
                CommonFunctions.WriteErrorLog(ex, "Program", "Main");
                ex = null;
            }
        }

        #region TimerTick
        private static void transaction_tick(object sender, System.EventArgs e)
        {
            try
            {
                TransactionTick.Stop();
                TransactionTick.Interval = Convert.ToInt64(Configuration["Interval"]); //18000000; // For Testing temperory ntrivedi //ntrivedi 03-05-019 dynamic time interval
                CallAPI();
                TransactionTick.Start();  //ntrivedi 27-10-2018 temperory test
            }
            catch (Exception ex)
            {
                CommonFunctions.WriteErrorLog(ex, "Program", "transaction_tick");
            }

        }
        #endregion

        #region classDeclaration
        public class walletServiceData
        {
            public Int32 ServiceID { get; set; }
            public string SMSCode { get; set; }
            public int WallletStatus { get; set; }
            public int ServiceStatus { get; set; }
            public int RecordCount { get; set; }
            public int AppType { get; set; }
            public long SerProID { get; set; }
            public string RouteTag { get; set; }
        }
        public class CommonMethods
        {
            public string TransactionFile;
            public string trnID;
            public string Authorization;
            public string enterprise;
            public string line;
            public string LeftTitle = null;
            public string[] StaticArray = new string[20];
            public char MainSaperator = ':';
            public int StaticCnt = 1;
            public string TrnLeftTitle = null;
            public StreamReader MsgFile;
            public string RequestType;
            public string Path_AddressGenerate, Path_CustomerDetail, Path_CustomerValidate, Path_CustomerRegistration, Path_BeneRegistration, Path_VerifyBeneficiary, Path_DeleteBeneficiary, Path_VerifyDeleteBeneficiary, PubKey;
            public string Str_URL = string.Empty;
            public string Str_RequestType;
            public string ContentType;
            public string Address1, Address2;
            public string ReturnMsg1, ReturnMsg2;
            public string SuccAPICodes;
            public string[] SuccAPICodesArray = new string[10];
            public string MatchRegex2;
            public string BeneActiveCode;
            public string DepositHistoryResponse;
            public List<RespTransfers> Transfers { get; set; }
            public string SqlStr = string.Empty;
            public DataSet dSet = new DataSet();
            public DataRow dRows = null;
            public string SMSCode = string.Empty;
            public int Category;
            public string UserName;
            public string AssetName;
            public string Password;
            public string RequestBody;
            public string AssetFromRequest;
            public long DMemberID;
            public List<RespLocalCoin> RespLocalCoins { get; set; }
            public decimal ConvertAmt { get; set; } // ntrivedi
            public string ProviderWalletID { get; set; }
            public int ConfirmationCount { get; set; }
        }
        enum EnAppType
        {
            BitGoAPI = 1,
            CryptoAPI = 2,
            EtherScan = 3
        }

        public class RespTransfers
        {
            public long id { get; set; } // ntrivedi 28-05-2018 string to long
            public string coin { get; set; }
            public string wallet { get; set; }
            public string txid { get; set; }
            public string address { get; set; }
            public string fromAddress { get; set; }
            public long confirmations { get; set; }
            public long value { get; set; }
            public string state { get; set; }
            public string confirmedTime { get; set; }
            public string unconfirmedTime { get; set; }
            public string createdTime { get; set; }
            public bool IsValid { get; set; }
            public decimal Amount { get; set; }
            public long OrderId { get; set; }
            public string valueStr { get; set; }
        }

        public class RespLocalCoin
        {
            public long confirmations { get; set; }
            public string txid { get; set; }
            public string address { get; set; }
            public string confirmedTime { get; set; }
            public string unconfirmedTime { get; set; }
            public int value { get; set; }
            public decimal Amount { get; set; }
        }
        #endregion

        #region CallBackProcess
        public static void CallAPI()
        {
            string SqlStr = string.Empty;
            DataSet dSet = new DataSet();
            try
            {
                SqlStr = " SELECT DC.AppType As AppType,SM.Id as ServiceID , SM.SMSCode,WM.Status AS WalletStatus , SM.Status AS ServiceStatus ,PM.ProviderName,DC.SerProId " +
                " FROM MarginWalletTypeMaster WM INNER JOIN ServiceMasterMargin SM ON SM.WalletTypeID=WM.id  inner join MarginDepositCounterMaster DC on DC.WalletTypeID = sm.WalletTypeID  inner join ServiceProviderMaster PM on PM.Id= DC.SerProId WHERE WM.status = 1 and WM.IsDepositionAllow = 1  and DC.Status = 1";
                dSet = (new DataCommon()).OpenDataSet("MarginWalletTypeMaster", SqlStr, dSet, 30);
                if (dSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dRow in dSet.Tables[0].Rows)
                    {
                        // Need common row object for each SMScode
                        walletServiceData walletServiceDataObj = new walletServiceData();
                        walletServiceDataObj.ServiceID = Convert.ToInt32(dRow["ServiceID"]);
                        walletServiceDataObj.SMSCode = dRow["SMSCode"].ToString();
                        walletServiceDataObj.WallletStatus = Convert.ToInt16(dRow["WalletStatus"]);
                        walletServiceDataObj.ServiceStatus = Convert.ToInt16(dRow["ServiceStatus"]);
                        walletServiceDataObj.AppType = Convert.ToInt16(dRow["AppType"]); // 2 for Local Coin
                        walletServiceDataObj.RecordCount = 1; //komal 10/9/2018  temperory ntrivedi
                        walletServiceDataObj.SerProID = Convert.ToInt64(dRow["SerProID"]); // ntrivedi added 14-12-2018
                        walletServiceDataObj.RouteTag = dRow["ProviderName"].ToString(); // ntrivedi added 14-12-2018
                        Console.Title = walletServiceDataObj.SMSCode + " Topup "; // ntrivedi easy to find 13-09-2018

                        CommonFunctions.WriteRequestLog("new timer call ", "CallAPI", walletServiceDataObj.SMSCode);
                        lock (walletServiceDataObj)
                        {
                            if (IsProcessing == false) //NTRIVEDI TEMPERORY
                            {
                                WaitCallback callBack;
                                callBack = new WaitCallback(CallAPISingle); // create thread for each SMSCode
                                ThreadPool.QueueUserWorkItem(callBack, walletServiceDataObj);
                            }
                            else
                            {
                                CommonFunctions.WriteRequestLog("IsProcessing = true so return", "CallAPI", walletServiceDataObj.SMSCode, Action: 2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.WriteErrorLog(ex, "Program", "CallAPI");
            }
            finally
            {
                 TransactionTick.Start(); //ntrivedi temperory 28-06-2018
            }
        }
        #endregion

        #region CommonFunction

        private static void GetHistory(ref CommonMethods CommonMethod, walletServiceData walletServiceDataObj, string trnID)
        {
            DataSet dSet = new DataSet();
            string SqlStr = string.Empty;
            decimal amount = 0;
            long autono = 0;
            try
            {
                // ntrivedi 28-05-2018 temperory top 1
                CommonFunctions.WriteRequestLog("GetHistory ", "GetHistory", CommonMethod.SMSCode, Action: 2);
                SqlStr = "SELECT top " + walletServiceDataObj.RecordCount.ToString() + " ID, TrnID , Address, SMSCode, Amount, Confirmations,OrderID,isNull(FromAddress,'') as FromAddress From MarginDepositHistory WHERE SMSCode = '" + CommonMethod.SMSCode + "' AND Status = 0 AND IsProcessing = 0 and SerProID=" + walletServiceDataObj.SerProID + " order by updateddate"; // ntrivedi temperory order by updateddate
                dSet = (new DataCommon()).OpenDataSet("MarginDepositHistory", SqlStr, dSet, 30);
                CommonFunctions.WriteRequestLog("GetHistory ", "GetHistory Query: " + SqlStr, CommonMethod.SMSCode, Action: 2);
                if (dSet.Tables[0].Rows.Count > 0)
                {
                    Console.WriteLine("Fetch Rows:" + dSet.Tables[0].Rows.Count + " for Coin" + CommonMethod.SMSCode);
                    IsProcessing = true;
                    CommonFunctions.WriteRequestLog("IsProcessing = true + Fetching Rows :  " + dSet.Tables[0].Rows.Count, "GetHistory", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2); // komal 10/9/2018 log for IsProcessing = true
                    foreach (DataRow dRow in dSet.Tables[0].Rows)
                    {
                        SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 1,UpdatedDate=dbo.getistdate() WHERE Id = " + dRow["ID"].ToString() + ""; //ntrivedi 28-05-2018 trnid instead of AutoNo due to slow searching on TrnID
                        (new DataCommon()).ExecuteQuery(SqlStr);
                    }
                    CommonFunctions.WriteRequestLog("Total Count :" + dSet.Tables[0].Rows.Count + " IsProcessing :" + IsProcessing.ToString(), "GetHistory", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);//komal 10/9/2018 test
                    foreach (DataRow dRow in dSet.Tables[0].Rows)
                    {
                        trnID = dRow["TrnID"].ToString();
                        amount = 0;
                        autono = Convert.ToInt64(dRow["ID"]);
                        CommonFunctions.WriteRequestLog("New Loop TrnID :" + dRow["TrnID"].ToString() + " IsProcessing :" + IsProcessing.ToString() + "", "GetHistory", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);

                        String Response = CallThirdPartyAPI(ref CommonMethod, trnID, CommonMethod.Authorization, CommonMethod.enterprise, dSet); // Generate ThirdParty API Response
                        if (Response == string.Empty) // ntrivedi 28-05-2018
                        {
                            SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 0,updateddate=dbo.getistdate() WHERE ID = " + dRow["ID"].ToString() + "";
                            (new DataCommon()).ExecuteQuery(SqlStr);
                            CommonFunctions.WriteRequestLog("Thirdparty Response Null for TrnID :  " + dRow["TrnID"].ToString(), "GetHistory", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                            continue;
                        }
                        BitGoResponses TPGenerateResponse = new BitGoResponses();
                        RespTransfers transferObj = new RespTransfers();
                        TPGenerateResponse = JsonConvert.DeserializeObject<BitGoResponses>(Response);
                        if (TPGenerateResponse != null)
                        {
                            if (TPGenerateResponse.coin.ToLower() == CommonMethod.SMSCode.ToLower()) // For BTC , BTG , BCH Response remove type receive condition ntrivedi 07-01-2018
                            {
                                if (TPGenerateResponse.entries != null)
                                {
                                    string trnid = dRow["TrnID"].ToString();
                                    trnid = dRow["SMSCode"].ToString();

                                    if (TPGenerateResponse.txid == (dRow["TrnID"]).ToString() && TPGenerateResponse.coin.ToLower() == (dRow["SMSCode"]).ToString().ToLower())
                                    {
                                        if (TPGenerateResponse.entries != null)
                                        {
                                            foreach (var iv in TPGenerateResponse.entries)
                                            {
                                                if (!string.IsNullOrEmpty(iv.address) && iv.value > 0)
                                                {
                                                    if (iv.value <= 0)
                                                    {
                                                        SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 0,Status=9,StatusMsg='negative amount' WHERE ID = " + dRow["ID"].ToString() + "";
                                                        (new DataCommon()).ExecuteQuery(SqlStr);
                                                        continue;
                                                    }
                                                    if (iv.address.ToString() == dRow["address"].ToString() && iv.wallet == CommonMethod.ProviderWalletID)
                                                    {
                                                        amount = Math.Round((iv.value / CommonMethod.ConvertAmt), 8);
                                                        transferObj.id = Convert.ToInt64(dRow["ID"]);
                                                        transferObj.Amount = Convert.ToDecimal(amount);
                                                        transferObj.coin = CommonMethod.SMSCode;
                                                        transferObj.txid = TPGenerateResponse.txid;
                                                        // transferObj.Value = Convert.ToDecimal(dRow["Value"]);
                                                        transferObj.OrderId = Convert.ToInt64(dRow["OrderID"]);
                                                        transferObj.address = dRow["Address"].ToString();
                                                        transferObj.fromAddress = dRow["FromAddress"].ToString();
                                                        transferObj.confirmations = TPGenerateResponse.confirmations;
                                                        if (transferObj.confirmations < 0) //ntrivedi treat as failed 26-07-2018
                                                        {
                                                            CommonFunctions.WriteRequestLog("confirmations < 0 :  " + transferObj.txid, "GetHistory", CommonMethod.SMSCode, Action: 2);
                                                            SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 0,Status=9,StatusMsg='negative confirmation' WHERE ID = " + dRow["ID"].ToString() + "";
                                                            (new DataCommon()).ExecuteQuery(SqlStr);
                                                            continue;
                                                        }
                                                        transferObj.valueStr = TPGenerateResponse.valueString;
                                                        transferObj.unconfirmedTime = TPGenerateResponse.unconfirmedTime.ToString();
                                                        transferObj.confirmedTime = TPGenerateResponse.confirmedTime.ToString();
                                                        CommonMethod.Transfers.Add(transferObj);
                                                    }
                                                    else
                                                    {
                                                        SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 1,Status=9,StatusMsg='mismatch walletid or address' WHERE Address='" + iv.address.ToString() + "' and ID = " + dRow["ID"].ToString() + "";
                                                        (new DataCommon()).ExecuteQuery(SqlStr);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 1,Status=9,StatusMsg='thirparty amount not found' WHERE ID = " + dRow["ID"].ToString() + "";
                                    (new DataCommon()).ExecuteQuery(SqlStr);
                                }
                            }

                            else
                            {
                                SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 1,Status=9,StatusMsg='error not null Thirpdarty response' WHERE ID = " + dRow["ID"].ToString() + "";
                                (new DataCommon()).ExecuteQuery(SqlStr);
                            }
                            CommonFunctions.WriteRequestLog("Loop done :" + dRow["TrnID"].ToString() + " IsProcessing :" + IsProcessing.ToString() + "", "GetHistory", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                        }

                        Console.WriteLine(CommonMethod.SMSCode + " Count : " + CommonMethod.Transfers.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 0 WHERE ID = " + autono + "";
                (new DataCommon()).ExecuteQuery(SqlStr);
                CommonFunctions.WriteErrorLog(ex, "Program", "GetHistory");
            }
        }

        //Uday 09-02-2019  For EtherScan API Get History
        private static void GetHistoryEtherScan(ref CommonMethods CommonMethod, walletServiceData walletServiceDataObj, string trnID)
        {
            DataSet dSet = new DataSet();
            string SqlStr = string.Empty;
            long autono = 0;
            try
            {

                CommonFunctions.WriteRequestLog("GetHistoryEtherScan ", "GetHistoryEtherScan", CommonMethod.SMSCode, Action: 2);
                SqlStr = "SELECT top " + walletServiceDataObj.RecordCount.ToString() + " ID, TrnID , Address, SMSCode, Amount, Confirmations,OrderID,isNull(FromAddress,'') as FromAddress From MarginDepositHistory WHERE SMSCode = '" + CommonMethod.SMSCode + "' AND Status = 0 AND IsProcessing = 0 and SerProID=" + walletServiceDataObj.SerProID + " order by updateddate"; // ntrivedi temperory order by updateddate
                dSet = (new DataCommon()).OpenDataSet("MarginDepositHistory", SqlStr, dSet, 30);
                CommonFunctions.WriteRequestLog("GetHistoryEtherScan ", "GetHistoryEtherScan Query: " + SqlStr, CommonMethod.SMSCode, Action: 2);
                if (dSet.Tables[0].Rows.Count > 0)
                {
                    Console.WriteLine("Fetch Rows:" + dSet.Tables[0].Rows.Count + " for Coin" + CommonMethod.SMSCode);
                    IsProcessing = true;
                    CommonFunctions.WriteRequestLog("IsProcessing = true + Fetching Rows :  " + dSet.Tables[0].Rows.Count, "GetHistory", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2); // komal 10/9/2018 log for IsProcessing = true
                    foreach (DataRow dRow in dSet.Tables[0].Rows)
                    {
                        SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 1,UpdatedDate=dbo.getistdate() WHERE Id = " + dRow["ID"].ToString() + ""; //ntrivedi 28-05-2018 trnid instead of AutoNo due to slow searching on TrnID
                        (new DataCommon()).ExecuteQuery(SqlStr);
                    }
                    CommonFunctions.WriteRequestLog("Total Count :" + dSet.Tables[0].Rows.Count + " IsProcessing :" + IsProcessing.ToString(), "GetHistoryEtherScan", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);//komal 10/9/2018 test
                    foreach (DataRow dRow in dSet.Tables[0].Rows)
                    {
                        trnID = dRow["TrnID"].ToString();
                        autono = Convert.ToInt64(dRow["ID"]);
                        CommonFunctions.WriteRequestLog("New Loop TrnID :" + dRow["TrnID"].ToString() + " IsProcessing :" + IsProcessing.ToString() + "", "GetHistoryEtherScan", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);

                        String Response = CallThirdPartyEtherScanAPI(ref CommonMethod, trnID, CommonMethod.Authorization, CommonMethod.enterprise, dSet); // Generate ThirdParty API Response

                        if (Response == string.Empty) // ntrivedi 28-05-2018
                        {
                            SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 0,updateddate=dbo.getistdate() WHERE ID = " + dRow["ID"].ToString() + "";
                            (new DataCommon()).ExecuteQuery(SqlStr);
                            CommonFunctions.WriteRequestLog("Thirdparty Response Null for TrnID :  " + dRow["TrnID"].ToString(), "GetHistoryEtherScan", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                            continue;
                        }
                        ERC20Response TPGenerateResponse = new ERC20Response();
                        RespTransfers transferObj = new RespTransfers();
                        TPGenerateResponse = JsonConvert.DeserializeObject<ERC20Response>(Response);

                        if (TPGenerateResponse != null)
                        {
                            if (TPGenerateResponse.isError == false)
                            {
                                string trnid = dRow["TrnID"].ToString();
                                trnid = dRow["SMSCode"].ToString();

                                if (trnID == TPGenerateResponse.txnid)
                                {
                                    transferObj.id = Convert.ToInt64(dRow["ID"]);
                                    transferObj.Amount = Convert.ToDecimal(dRow["Amount"]);
                                    transferObj.coin = CommonMethod.SMSCode;
                                    transferObj.txid = TPGenerateResponse.txnid;
                                    transferObj.OrderId = Convert.ToInt64(dRow["OrderID"]);
                                    transferObj.address = dRow["Address"].ToString();
                                    transferObj.fromAddress = dRow["FromAddress"].ToString();
                                    transferObj.confirmations = TPGenerateResponse.confirmations;
                                    if (transferObj.confirmations < 0) //ntrivedi treat as failed 26-07-2018
                                    {
                                        CommonFunctions.WriteRequestLog("confirmations < 0 :  " + transferObj.txid, "GetHistoryEtherScan", CommonMethod.SMSCode, Action: 2);
                                        SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 0,Status=9,StatusMsg='negative confirmation' WHERE ID = " + dRow["ID"].ToString() + "";
                                        (new DataCommon()).ExecuteQuery(SqlStr);
                                        continue;
                                    }
                                    CommonMethod.Transfers.Add(transferObj);
                                }
                                else
                                {
                                    SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 1,Status=9,StatusMsg='mismatch transaction id' WHERE ID = " + dRow["ID"].ToString() + "";
                                    (new DataCommon()).ExecuteQuery(SqlStr);
                                }
                            }
                            else
                            {
                                SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 1,Status=9,StatusMsg='error not null Thirpdarty response' WHERE ID = " + dRow["ID"].ToString() + "";
                                (new DataCommon()).ExecuteQuery(SqlStr);
                            }
                            CommonFunctions.WriteRequestLog("Loop done :" + dRow["TrnID"].ToString() + " IsProcessing :" + IsProcessing.ToString() + "", "GetHistoryEtherScan", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                        }
                        Console.WriteLine(CommonMethod.SMSCode + " Count : " + CommonMethod.Transfers.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 0 WHERE ID = " + autono + "";
                (new DataCommon()).ExecuteQuery(SqlStr);
                CommonFunctions.WriteErrorLog(ex, "Program", "GetHistoryEtherScan");
            }
        }

        public static DateTime UTC_To_IST()
        {
            DateTime myUTC = DateTime.UtcNow;
            DateTime istdate = TimeZoneInfo.ConvertTimeFromUtc(myUTC, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            return istdate;
        }

        private static void TradeDepositHistoryUpdationForBitgo(ref CommonMethods CommonMethod)
        {
            DataSet dSet = new DataSet();
            string SqlStr = string.Empty;
            string queryResult;
            long CrWalletID;
            try
            {
                if (CommonMethod.Transfers.Count > 0)
                {
                    foreach (var item in CommonMethod.Transfers)
                    {
                        // update 
                        CommonMethod.SqlStr = "UPDATE MarginDepositHistory SET Amount=" + item.Amount + ",Confirmations =" + item.confirmations + ", ConfirmedTime ='" + item.confirmedTime + "', epochtimepure  ='" + item.unconfirmedTime + "', UpdatedDate = dbo.GetISTDate(),SystemRemarks='Str Amt:" + item.valueStr + "' WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                        (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                        CommonFunctions.WriteRequestLog("Update Deposit History :  " + item.txid, "TradeMarginDepositHistoryUpdationForBitgo", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                        // ntrivedi temperory
                        if ((new DataCommon()).ExecuteScalarWDM("Select Count(TrnID) From TradeDepositCompletedTrn Where TrnID='" + item.txid + "' and Address='" + item.address + "'") != "0")
                        {
                            continue;
                        }
                       
                        SqlStr = "select top 1 UserID from BizUserTypeMapping where UserType=0 order by UserID";
                        queryResult = (new DataCommon()).ExecuteScalarWDM(SqlStr);// organization
                        if (string.IsNullOrEmpty(queryResult))
                        {
                            SqlStr = "UPDATE MarginDepositHistory SET StatusMsg='Org Record Not found',Status=9,IsProcessing = 0 WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                            (new DataCommon()).ExecuteQuery(SqlStr);
                            CommonMethod.DMemberID = 0;
                            continue;
                        }
                        else
                        {
                            CommonMethod.DMemberID = Convert.ToInt64(queryResult);
                        }


                        SqlStr = "select WalletID from MarginAddressMaster  AM " +
                                 " inner join MarginWalletMaster WM on wm.Id = am.WalletId " +
                                 " inner join MarginWalletTypeMaster WTM on WTM.Id = wm.WalletTypeID " +
                                 " Where OriginalAddress = @PublicAddress and AM.Status = 1 and WTM.WalletTypeName = @CoinName";
                        queryResult = (new DataCommon()).ExecuteScalarWDMParameterize("@PublicAddress,@CoinName", item.address + "," + item.coin, SqlStr);
                        if (string.IsNullOrEmpty(queryResult))
                        {
                            // ntrivedi temperory 29-10-2018
                            SqlStr = "UPDATE MarginDepositHistory SET StatusMsg='Address Not found',Status=9,IsProcessing = 0 WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                            (new DataCommon()).ExecuteQuery(SqlStr);
                            CommonMethod.DMemberID = 0;
                            continue;
                        }
                        else
                        {
                            CrWalletID = Convert.ToInt64(queryResult);
                            SqlStr = "select UserID from MarginWalletMaster where ID=@WalletID";
                            string userID = (new DataCommon()).ExecuteScalarWDMParameterize("@WalletID", queryResult, SqlStr);
                            SqlStr = "UPDATE MarginDepositHistory SET UserID=" + userID + " WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                            (new DataCommon()).ExecuteQuery(SqlStr);
                        }
                        // Delivery Process Order
                        if (item.confirmations >= CommonMethod.ConfirmationCount && CommonMethod.ConfirmationCount > 0) ////ntrivedi 11-05-2018 
                        {
                            SqlStr = "INSERT INTO TradeDepositCompletedTrn(TrnID,Address, Status, CreatedTime) VALUES('" + item.txid + "','" + item.address + "', 1 , dbo.GetISTDate())";
                            (new DataCommon()).ExecuteQuery(SqlStr);
                            Int32 ReturnCode = 0;
                            string ReturnMsg = string.Empty;
                            Int64 errorcode = 0;
                            long TrnNo = 0;
                            sp_MarginDepositionProcess(item.txid, item.coin, item.Amount, item.id, ref ReturnMsg, ref ReturnCode, ref errorcode, ref TrnNo);
                            if (ReturnCode == 0)
                            {
                                SqlStr = "UPDATE MarginDepositHistory SET status = 1,StatusMsg='Success',OrderID='" + TrnNo + "' WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                                (new DataCommon()).ExecuteQuery(SqlStr);
                            }
                            else
                            {
                                SqlStr = "UPDATE MarginDepositHistory SET status = 9,StatusMsg='" + ReturnMsg + "',OrderID='" + TrnNo + "' WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                                (new DataCommon()).ExecuteQuery(SqlStr);
                            }
                        }
                        SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 0 WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                        (new DataCommon()).ExecuteQuery(SqlStr);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.WriteErrorLog(ex, "Program", "TradeMarginDepositHistoryUpdationForBitgo");
            }
        }

        //Uday 09-02-2019  For EtherScan Update Deposit History
        private static void TradeDepositHistoryUpdationForEtherScan(ref CommonMethods CommonMethod)
        {
            DataSet dSet = new DataSet();
            string SqlStr = string.Empty;
            string queryResult;
            long CrWalletID;
            try
            {
                if (CommonMethod.Transfers.Count > 0)
                {
                    foreach (var item in CommonMethod.Transfers)
                    {
                        // update 
                        CommonMethod.SqlStr = "UPDATE MarginDepositHistory SET Amount=" + item.Amount + ",Confirmations =" + item.confirmations + ",UpdatedDate = dbo.GetISTDate(),SystemRemarks='Str Amt:" + item.value + "' WHERE ID=" + item.id;
                        (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                        CommonFunctions.WriteRequestLog("Update Deposit History :  " + item.txid, "TradeMarginDepositHistoryUpdationForEtherScan", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                        // ntrivedi temperory
                        if ((new DataCommon()).ExecuteScalarWDM("Select Count(TrnID) From TradeDepositCompletedTrn Where TrnID='" + item.txid + "' and Address='" + item.address + "'") != "0")
                        {
                            continue;
                        }
                        SqlStr = "select top 1 UserID from BizUserTypeMapping where UserType=0 order by UserID";
                        queryResult = (new DataCommon()).ExecuteScalarWDM(SqlStr);// organization
                        if (string.IsNullOrEmpty(queryResult))
                        {
                            SqlStr = "UPDATE MarginDepositHistory SET StatusMsg='Org Record Not found',Status=9,IsProcessing = 0 WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                            (new DataCommon()).ExecuteQuery(SqlStr);
                            CommonMethod.DMemberID = 0;
                            continue;
                        }
                        else
                        {
                            CommonMethod.DMemberID = Convert.ToInt64(queryResult);
                        }

                        //Uday 07-02-2019 Check For Admin Address
                        SqlStr = "select top 1 Am.Id as [AddressId] " +
                                 " from MarginAddressMaster AM inner join MarginWalletMaster wm on wm.id = am.WalletId" +
                                 " inner join MarginWalletTypeMaster WTM on WTM.Id = wm.wallettypeid" +
                                 " where wm.UserID = (select top 1 UserID from BizUserTypeMapping where UserType = 0)" +
                                 " and (OriginalAddress = '" + item.address + "' or Address = '" + item.address + "') and AM.AddressType=1"; //ntrivedi 18-04-2018 for commisssion deposition in admin wallet balance need to add condition for AM.AddressType=1
                        queryResult = (new DataCommon()).ExecuteScalarWDM(SqlStr);
                        if (!string.IsNullOrEmpty(queryResult))
                        {
                            SqlStr = "UPDATE MarginDepositHistory SET StatusMsg='Admin Address Not Allowed',Status=9,IsProcessing = 0 WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                            (new DataCommon()).ExecuteQuery(SqlStr);
                            CommonMethod.DMemberID = 0;
                            continue;
                        }


                        SqlStr = "select WalletID from MarginAddressMaster  AM " +
                                 " inner join MarginWalletMaster WM on wm.Id = am.WalletId " +
                                 " inner join MarginWalletTypeMaster WTM on WTM.Id = wm.WalletTypeID " +
                                 " Where OriginalAddress = @PublicAddress and AM.Status = 1 and WTM.WalletTypeName = @CoinName";
                        queryResult = (new DataCommon()).ExecuteScalarWDMParameterize("@PublicAddress,@CoinName", item.address + "," + item.coin, SqlStr);
                        if (string.IsNullOrEmpty(queryResult))
                        {
                            // ntrivedi temperory 29-10-2018
                            SqlStr = "UPDATE MarginDepositHistory SET StatusMsg='Address Not found',Status=9,IsProcessing = 0 WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                            (new DataCommon()).ExecuteQuery(SqlStr);
                            CommonMethod.DMemberID = 0;
                            continue;
                        }
                        else
                        {
                            CrWalletID = Convert.ToInt64(queryResult);
                            SqlStr = "select UserID from MarginWalletMaster where ID=@WalletID";
                            string userID = (new DataCommon()).ExecuteScalarWDMParameterize("@WalletID", queryResult, SqlStr);
                            SqlStr = "UPDATE MarginDepositHistory SET UserID=" + userID + " WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                            (new DataCommon()).ExecuteQuery(SqlStr);
                        }
                        // Delivery Process Order
                        if (item.confirmations >= CommonMethod.ConfirmationCount && CommonMethod.ConfirmationCount > 0) ////ntrivedi 11-05-2018 
                        {
                            SqlStr = "INSERT INTO TradeDepositCompletedTrn(TrnID,Address, Status, CreatedTime) VALUES('" + item.txid + "','" + item.address + "', 1 , dbo.GetISTDate())";
                            (new DataCommon()).ExecuteQuery(SqlStr);
                            // intentionally added code 04-11-2018 for checking the exception from dependency side 
                            Int32 ReturnCode = 0;
                            string ReturnMsg = string.Empty;
                            Int64 errorcode = 0;
                            long TrnNo = 0;

                            sp_MarginDepositionProcess(item.txid, item.coin, item.Amount, item.id, ref ReturnMsg, ref ReturnCode, ref errorcode, ref TrnNo);
                            if (ReturnCode == 0)
                            {
                                SqlStr = "UPDATE MarginDepositHistory SET status = 1,StatusMsg='Success',OrderID='" + TrnNo + "' WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                                (new DataCommon()).ExecuteQuery(SqlStr);
                            }
                            else
                            {
                                SqlStr = "UPDATE MarginDepositHistory SET status = 9,StatusMsg='" + ReturnMsg + "',OrderID='" + TrnNo + "' WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                                (new DataCommon()).ExecuteQuery(SqlStr);
                            }
                        }
                        
                        SqlStr = "UPDATE MarginDepositHistory SET IsProcessing = 0 WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                        (new DataCommon()).ExecuteQuery(SqlStr);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.WriteErrorLog(ex, "Program", "TradeMarginDepositHistoryUpdationForEtherScan");
            }
        }

        private static void CallAPISingle(object RefObj)
        {
            string smscode = "";/*string trnID= "ddaa745649f8f09564daa1aaa51716ef49175b328c8d87aba4b207b3f178d620"; string Authorization = "Bearer v2x5e587227b1085bdcd4d5bd3421f0a702fff86d2137af0cf3a8624655d4e42f71"; string enterprise = "5a79359692f6f738073dffcbbb0c22df";*/
            try
            {
                walletServiceData walletServiceDataObj = (walletServiceData)RefObj;
                CommonMethods CommonMethod = new CommonMethods();
                CommonMethod.Transfers = new List<RespTransfers>(); // Need object of RespTransfers  for Thirdparty ApI response
                //CommonMethod.RespLocalCoins = new List<RespLocalCoin>(); // Need object of RespLocalCoins  for Thirdparty ApI response
                CommonMethod.SMSCode = walletServiceDataObj.SMSCode;
                smscode = CommonMethod.SMSCode;
                ReadMasterFile(walletServiceDataObj.SMSCode, ref CommonMethod); // Read  Master File
                if (!string.IsNullOrEmpty(CommonMethod.Path_AddressGenerate))
                {
                    ReadTransactionalFile(CommonMethod.Path_AddressGenerate, ref CommonMethod); // Read Transaction file for specific coin               
                    if (!string.IsNullOrEmpty(CommonMethod.Str_URL) && !string.IsNullOrEmpty(CommonMethod.Str_RequestType) && !string.IsNullOrEmpty(CommonMethod.ContentType))
                    {
                        switch (walletServiceDataObj.AppType)
                        {
                            case (int)EnAppType.BitGoAPI:
                                GetHistory(ref CommonMethod, walletServiceDataObj, CommonMethod.trnID);
                                TradeDepositHistoryUpdationForBitgo(ref CommonMethod);
                                break;
                            case (int)EnAppType.CryptoAPI:
                                break;
                            case (int)EnAppType.EtherScan:      //Uday  Integrate Status Check For EtherScan ERC20 Provider API
                                GetHistoryEtherScan(ref CommonMethod, walletServiceDataObj, CommonMethod.trnID); // Get History From Deposit History SMScode Wise 
                                TradeDepositHistoryUpdationForEtherScan(ref CommonMethod); // Update Crypto coin into Trade Deposit History
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        CommonFunctions.WriteRequestLog("Transaction Detail not found", "CallAPISingle", CommonMethod.SMSCode, Action: 2);
                    }
                }
                else
                {
                    CommonFunctions.WriteRequestLog("Master File Detail not found", "CallAPISingle", CommonMethod.SMSCode, Action: 2);
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.WriteErrorLog(ex, "Program", "CallAPISingle");
            }
            finally
            {
                IsProcessing = false; //komal 10/9/2018 finally stop thread 
                CommonFunctions.WriteRequestLog("IsProcessing = false ", "CallAPISingle", smscode, Action: 2);
            }
        }
        #endregion

        #region "ReadMasterFile"

        public static void ReadMasterFile(string APIName, ref CommonMethods CommonMethod)
        {
            string FilePath = Configuration["MainPath"] + "\\MasterFile_" + APIName + ".txt";

            try
            {
                if (System.IO.File.Exists(FilePath) == true)
                {
                    CommonMethod.StaticArray[0] = "0";
                    CommonMethod.TransactionFile = Configuration["MainPath"]; //FilePath

                    string[] lines = System.IO.File.ReadAllLines(FilePath);
                    foreach (string line in lines)
                    {
                        CommonMethod.LeftTitle = line.Substring(0, line.IndexOf(CommonMethod.MainSaperator)).ToLower();

                        if (CommonMethod.LeftTitle.ToUpper().Contains("STATIC"))
                        {
                            //Start with #1 Position
                            CommonMethod.StaticArray[CommonMethod.StaticCnt++] = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                            //StaticCnt++;
                        }
                        else if (CommonMethod.LeftTitle == "apiname")
                        {
                            CommonMethod.TransactionFile = CommonMethod.TransactionFile + "\\" + APIName + "\\";
                        }
                        else if (CommonMethod.LeftTitle == "requesttype")
                        {
                            CommonMethod.RequestType = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.LeftTitle == "transactionfilepath")
                        {
                            CommonMethod.Path_AddressGenerate = CommonMethod.TransactionFile + line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }

                    }

                    CommonFunctions.WriteRequestLog("Transaction File Path :  " + CommonMethod.Path_AddressGenerate, "ReadMasterFile", CommonMethod.SMSCode);
                }
                else
                {

                    CommonFunctions.WriteRequestLog(FilePath + " File Not Found", "ReadMasterFile", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.WriteErrorLog(ex, "Program", "ReadMasterFile");
            }
        }

        public static void ReadTransactionalFile(string Path, ref CommonMethods CommonMethod)
        {

            try
            {
                if (System.IO.File.Exists(Path) == true)
                {
                    string[] lines = System.IO.File.ReadAllLines(Path);
                    foreach (string line in lines)
                    {
                        CommonMethod.TrnLeftTitle = line.Substring(0, line.IndexOf(CommonMethod.MainSaperator)).ToLower();

                        if (CommonMethod.TrnLeftTitle.ToUpper().Contains("URL")) //Read URL
                        {
                            CommonMethod.Str_URL = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("requesttype")) //Read Request Type 
                        {
                            CommonMethod.Str_RequestType = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("contenttype")) //Read Content Type
                        {
                            CommonMethod.ContentType = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("category")) //Read Category Type
                        {
                            CommonMethod.Category = Convert.ToInt16(line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1));
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("assetname")) //Read AssetName
                        {
                            CommonMethod.AssetName = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("username")) //Read Username
                        {
                            CommonMethod.UserName = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("password")) //Read Password
                        {
                            CommonMethod.Password = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("requestbody")) //Read RequestBody
                        {
                            CommonMethod.RequestBody = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("assetfromrequest")) //Read RequestBody
                        {
                            CommonMethod.AssetFromRequest = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("authorization"))
                        {
                            CommonMethod.Authorization = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("trnID"))
                        {
                            CommonMethod.trnID = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("enterprise"))
                        {
                            CommonMethod.enterprise = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("convertamt")) //Read RequestBody
                        {
                            CommonMethod.ConvertAmt = Convert.ToInt64(line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1));
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("providerwalletid")) //Read RequestBody
                        {
                            CommonMethod.ProviderWalletID = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("confirmation")) //Read RequestBody
                        {
                            CommonMethod.ConfirmationCount = Convert.ToInt32(line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1));
                        }
                    }

                    CommonFunctions.WriteRequestLog("Transaction URL :  " + CommonMethod.Str_URL + " Request Type : " + CommonMethod.Str_RequestType + " Content Type : " + CommonMethod.ContentType, "ReadTransactionalFile", CommonMethod.SMSCode);
                }
                else
                {
                    CommonFunctions.WriteRequestLog(Path + " File Not Found", "ReadTransactionalFile", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.WriteErrorLog(ex, "Program", "ReadTransactionalFile");
            }
        }
        #endregion

        #region "CallThirdPartyAPI"

        private static string CallThirdPartyAPI(ref CommonMethods CommonMethod, string trnID, string Authorization, string enterprise, DataSet dSet)
        {
            try
            {
                //CommonMethods CommonMethod = (CommonMethods)CommonMethodObj;
                if (!string.IsNullOrEmpty(CommonMethod.Str_URL) && !string.IsNullOrEmpty(CommonMethod.Str_RequestType) && !string.IsNullOrEmpty(CommonMethod.ContentType))
                {
                    //vsolanki

                    if (CommonMethod.Str_URL.Contains("#trnID#"))
                    {
                        CommonMethod.Str_URL = CommonMethod.Str_URL.Replace("#trnID#", trnID);
                    }
                    CommonMethod.Str_URL = CommonMethod.Str_URL.Replace("#ProviderWalletID#", CommonMethod.ProviderWalletID);
                    CommonFunctions.WriteRequestLog("Request :  " + CommonMethod.Str_URL, "CallThirdPartyAPI", CommonMethod.SMSCode);

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(CommonMethod.Str_URL);
                    httpWebRequest.ContentType = CommonMethod.ContentType;
                    httpWebRequest.Method = CommonMethod.Str_RequestType;
                    httpWebRequest.KeepAlive = false;
                    httpWebRequest.Timeout = 180000;
                    //vsolanki
                    httpWebRequest.Headers.Add("Authorization", Authorization);
                    httpWebRequest.Headers.Add("enterprise", enterprise);

                    HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
                    using (StreamReader StreamReaderObj = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        CommonMethod.DepositHistoryResponse = StreamReaderObj.ReadToEnd();
                        StreamReaderObj.Close();
                        StreamReaderObj.Dispose();

                    }
                    httpWebResponse.Close();
                    CommonFunctions.WriteRequestLog("Response :  " + CommonMethod.DepositHistoryResponse, "CallThirdPartyAPI", CommonMethod.SMSCode);
                    CommonFunctions.WriteRequestLog("Generate Response :  " + JsonConvert.SerializeObject(CommonMethod.Transfers), "CallThirdPartyAPI", CommonMethod.SMSCode);
                    return CommonMethod.DepositHistoryResponse;
                }
                else
                {
                    CommonFunctions.WriteRequestLog("Generate Response :  Transaction Detail not found", "CallThirdPartyAPI", CommonMethod.SMSCode, Action: 2);
                    return "";
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.WriteErrorLog(ex, "Program", "CallThirdPartyAPI");
                return "";
            }
        }

        private static string CallThirdPartyCryptoAPI(ref CommonMethods CommonMethod, string Address, string TrnID, string AutoNo)
        {
            try
            {
                //CommonMethods CommonMethod = (CommonMethods)CommonMethodObj;
                if (!string.IsNullOrEmpty(CommonMethod.Str_URL) && !string.IsNullOrEmpty(CommonMethod.Str_RequestType) && !string.IsNullOrEmpty(CommonMethod.ContentType))
                {

                    string authInfo = CommonMethod.UserName + ":" + CommonMethod.Password;
                    authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                    WebRequest myReqrpc = WebRequest.Create(CommonMethod.Str_URL);
                    myReqrpc.Headers["Authorization"] = "Basic " + authInfo;
                    myReqrpc.Method = CommonMethod.Str_RequestType;

                    string ReqStr = @"" + CommonMethod.RequestBody; //@"{""id"":""" + 1 + @""",""method"":""getnewaddress"",""params"":[]}";
                    ReqStr = ReqStr.Replace("#Address#", Address);
                    ReqStr = ReqStr.Replace("#TrnID#", TrnID);
                    ReqStr = ReqStr.Replace("#AutoNo#", AutoNo);

                    CommonFunctions.WriteRequestLog("RPC Address generate Request :  " + ReqStr, "CallThirdPartyCryptoAPI", CommonMethod.SMSCode);
                    StreamWriter sw = new StreamWriter(myReqrpc.GetRequestStream());
                    sw.Write(ReqStr);
                    sw.Close();

                    WebResponse response = myReqrpc.GetResponse();

                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    CommonMethod.DepositHistoryResponse = StreamReader.ReadToEnd();
                    StreamReader.Close();
                    response.Close();

                    CommonFunctions.WriteRequestLog("RPC Address Response :  " + CommonMethod.DepositHistoryResponse, "CallThirdPartyCryptoAPI", CommonMethod.SMSCode);
                    return CommonMethod.DepositHistoryResponse;
                }
                else
                {
                    CommonFunctions.WriteRequestLog("Generate Response :  Transaction Detail not found", "CallThirdPartyCryptoAPI", CommonMethod.SMSCode);
                    return CommonMethod.DepositHistoryResponse;
                }
            }
            catch (WebException webex)
            {
                WebResponse errResp = webex.Response;
                using (Stream respStream = errResp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream);
                    string Text = reader.ReadToEnd();
                    if (Text.ToLower().Contains("code"))
                        CommonMethod.DepositHistoryResponse = Text;
                    webex = null;
                    CommonFunctions.WriteRequestLog("BlockChainTransfer exception : " + CommonMethod.DepositHistoryResponse, "CallThirdPartyCryptoAPI", CommonMethod.SMSCode, Action: 2);
                }

                CommonFunctions.WriteRequestLog("webex : " + webex, "CallThirdPartyCryptoAPI", CommonMethod.SMSCode, Action: 2);
                return CommonMethod.DepositHistoryResponse;
            }
        }

        //Uday 09-02-2019  Call EtherScan Confirmation Check API
        private static string CallThirdPartyEtherScanAPI(ref CommonMethods CommonMethod, string trnID, string Authorization, string enterprise, DataSet dSet)
        {
            try
            {
                string requestBody;
                if (!string.IsNullOrEmpty(CommonMethod.Str_URL) && !string.IsNullOrEmpty(CommonMethod.Str_RequestType) && !string.IsNullOrEmpty(CommonMethod.ContentType))
                {
                    CommonFunctions.WriteRequestLog("Request :  " + CommonMethod.Str_URL, "CallThirdPartyEtherScanAPI", CommonMethod.SMSCode);

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(CommonMethod.Str_URL);
                    httpWebRequest.ContentType = CommonMethod.ContentType;
                    httpWebRequest.Method = CommonMethod.Str_RequestType;
                    httpWebRequest.KeepAlive = false;
                    httpWebRequest.Timeout = 180000;

                    requestBody = CommonMethod.RequestBody.Replace("#Username#", CommonMethod.UserName);
                    requestBody = requestBody.Replace("#Password#", CommonMethod.Password);
                    requestBody = requestBody.Replace("#trnID#", trnID);
                    requestBody = requestBody.Replace("#AssetName#", CommonMethod.AssetName);
                    requestBody = requestBody.Replace("#PageNo#", "0");
                    requestBody = requestBody.Replace("#Limit#", "1000");

                    CommonFunctions.WriteRequestLog("Request :  " + requestBody, "CallThirdPartyEtherScanAPI", CommonMethod.SMSCode);


                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(requestBody);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
                    using (StreamReader StreamReaderObj = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        CommonMethod.DepositHistoryResponse = StreamReaderObj.ReadToEnd();
                        StreamReaderObj.Close();
                        StreamReaderObj.Dispose();

                    }
                    httpWebResponse.Close();
                    CommonFunctions.WriteRequestLog("Response :  " + CommonMethod.DepositHistoryResponse, "CallThirdPartyEtherScanAPI", CommonMethod.SMSCode);

                    CommonFunctions.WriteRequestLog("Generate Response :  " + JsonConvert.SerializeObject(CommonMethod.Transfers), "CallThirdPartyEtherScanAPI", CommonMethod.SMSCode);
                    return CommonMethod.DepositHistoryResponse;
                }
                else
                {
                    CommonFunctions.WriteRequestLog("Generate Response :  Transaction Detail not found", "CallThirdPartyEtherScanAPI", CommonMethod.SMSCode, Action: 2);
                    return "";
                }

            }
            catch (Exception ex)
            {

                CommonFunctions.WriteErrorLog(ex, "Program", "CallThirdPartyEtherScanAPI TrnID=" + trnID);
                return "";
            }
        }

        private static void sp_MarginDepositionProcess(string trnID, string SMSCode, decimal amount, long autoid, ref string RetMsg, ref Int32 RetCode, ref Int64 ErrorCode, ref long TrnNo)
        {
            try
            {
                SqlParameter[] Params = new SqlParameter[]
                {
                    new SqlParameter("@TrnNo",SqlDbType.BigInt, 10, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default,0),
                    new SqlParameter("@ChannelType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,21),
                    new SqlParameter("@timeStamp",SqlDbType.VarChar, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, trnID),
                    new SqlParameter("@Coin",SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, SMSCode),

                    new SqlParameter("@TrnRefNo",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,autoid) ,
                    new SqlParameter("@WalletTrnType",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, 2) ,
                    new SqlParameter("@Amount",SqlDbType.Decimal, 28, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, amount) ,
                    new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, 0) ,
                    new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default,"") ,
                    new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, 0)
                };
                (new DataCommon()).ExecuteSP("sp_MarginDepositionProcess", ref Params);

                if (Params[7].Value != DBNull.Value)
                    RetCode = Convert.ToInt16(Params[7].Value);

                if (Params[8].Value != DBNull.Value)
                    RetMsg = Convert.ToString(Params[8].Value);

                if (Params[9].Value != DBNull.Value)
                    ErrorCode = Convert.ToInt64(Params[9].Value);

                if (Params[0].Value != DBNull.Value)
                    TrnNo = Convert.ToInt64(Params[0].Value);

                CommonFunctions.WriteRequestLog("sp_MarginDepositionProcess Completed TrnID=trnID " + trnID + ",RetCode:" + RetCode.ToString() + " RetMsg:" + RetMsg, "ReconAction", SMSCode);
            }
            catch (Exception ex)
            {
                CommonFunctions.WriteErrorLog(ex, "Program", "ReconAction");
                throw ex;
            }
        }

        #endregion
    }
}
