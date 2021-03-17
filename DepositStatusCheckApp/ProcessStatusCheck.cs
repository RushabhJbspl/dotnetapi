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

namespace DepositStatusCheckApp
{

    public interface IProcessStatusCheck
    {
        void CallAPI();
    }

    public class ProcessStatusCheck : IProcessStatusCheck
    {
       
        public static IConfiguration Configuration { get; set; }
        static bool IsProcessing = false;
        private readonly IWalletService _walletService;

        
        public ProcessStatusCheck()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            //_walletService = new WalletService();
        }

        #region classDeclaration
        public class walletServiceData
        {
            public Int32 ServiceID { get; set; }
            public string SMSCode { get; set; }
            public int WallletStatus { get; set; }
            public int ServiceStatus { get; set; }
            public int RecordCount { get; set; }
            public int AppType { get; set; }
            // public int ValidDays { get; set; } 
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

            //private readonly IWalletService _walletService;




            //public CommonMethods(IWalletService walletService)
            //{
            //    _walletService = walletService;
            //}

            //public WalletDrCrResponse callDepencency()
            //{

            //    try
            //    {

            //    }
            //    catch (Exception ex)
            //    {
            //        //ex = null;
            //        WriteErrorLog(ex, "Program", "CallAPI");
            //    }
            //}

        }
        enum EnAppType
        {
            BitGoAPI = 1,
            CryptoAPI = 2
        }

        public class RespTransfers
        {
            public long id { get; set; } // ntrivedi 28-05-2018 string to long
            public string coin { get; set; }
            public string wallet { get; set; }
            public string txid { get; set; }
            public string address { get; set; }
            public string fromAddress { get; set; }
            //public int height { get; set; }
            //public DateTime date { get; set; }
            public long confirmations { get; set; }
            public long value { get; set; }
            //public string valueString { get; set; }
            //public string feeString { get; set; }
            //public int payGoFee { get; set; }
            //public string payGoFeeString { get; set; }
            //public double usd { get; set; }
            //public double usdRate { get; set; }
            public string state { get; set; }
            //public IList<string> tags { get; set; }
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
        public void CallAPI()
        {
            string SqlStr = string.Empty;
            DataSet dSet = new DataSet();
            try
            {

                SqlStr = " SELECT 1 As AppType,SM.Id as ServiceID , SM.SMSCode,WM.Status AS WalletStatus , SM.Status AS ServiceStatus FROM WalletTypeMasters WM INNER JOIN ServiceMaster SM ON SM.WalletTypeID=WM.id WHERE WM.status = 1 and WM.IsDepositionAllow = 1";
                dSet = (new DataCommon()).OpenDataSet("WalletTypeMasters", SqlStr, dSet, 30);
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
                                                                                         //   walletServiceDataObj.ValidDays = Convert.ToInt16(dRow["ValidDays"]); // komal 10/9/2018 get valid days from Table
                                                                                         //walletServiceDataObj.DeliveryRowsCount  = Convert.ToInt16(dRow["DeliveryRowsCount"]); // komal 10/9/2018 get Top count days from Table
                        walletServiceDataObj.RecordCount = 1; //komal 10/9/2018  temperory ntrivedi
                        Console.Title = walletServiceDataObj.SMSCode + " Topup "; // ntrivedi easy to find 13-09-2018
                       
                        WriteRequestLog("new timer call ", "CallAPI", walletServiceDataObj.SMSCode);
                        lock (walletServiceDataObj)
                        {
                            //isreturnwithfailure = false;
                            if (IsProcessing == false)
                            {
                                //isreturnwithfailure = true;
                                Console.WriteLine("new timer call");
                                WaitCallback callBack;
                                callBack = new WaitCallback(CallAPISingle); // create thread for each SMSCode
                                ThreadPool.QueueUserWorkItem(callBack, walletServiceDataObj);

                            }
                            else
                            {
                                WriteRequestLog("IsProcessing = true so return ", "CallAPI", walletServiceDataObj.SMSCode, Action: 2);
                            }

                            //Thread.Sleep(100);
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //ex = null;
                WriteErrorLog(ex, "Program", "CallAPI");
            }
            finally
            {
                //TransactionTick.Start(); //ntrivedi temperory 28-06-2018
            }
        }
        #endregion

        #region Logging
        public void WriteLogIntoFile(byte CheckReq_Res, DateTime Date, string MethodName, string Controllername, string Req_Res = null, string accessToken = null)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                if (CheckReq_Res == 1)
                {
                    logger.Info("\nDate:" + Date + "\nMethodName:" + MethodName + ", Controllername: " + Controllername + "\nAccessToken: " + accessToken + "\nRequest: " + Req_Res + "\n===================================================================================================================");
                }
                else //if(CheckReq_Res==2)
                {
                    logger.Info("\nDate:" + Date + "\nMethodName:" + MethodName + ", Controllername: " + Controllername + "\nResponse: " + Req_Res + "\n===================================================================================================================");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        public void WriteErrorLog(Exception ex, string methodName, string Source, string OtherInfo = "")
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                logger.Error("\nDate:" + UTC_To_IST() + "\nMethodName:" + methodName + ",Controllername: " + Source + "\nError: " + ex.Message + "\nOtherInfo:" + OtherInfo + "===================================================================================================================");
            }
            catch (Exception ex1)
            {
                logger.Error(ex1);
            }
        }
        public void WriteRequestLog(string inputString, string methodName, string APIName, int IsAllow = 1, int Action = 1)
        {
            string strFilePath = string.Empty;
            try
            {
                if (IsAllow == 1) //komal 12-9-2018 write log or not
                {
                    //OperationContext context = OperationContext.Current;
                    //MessageProperties prop = context.IncomingMessageProperties;
                    //RemoteEndpointMessageProperty endpoint =
                    //    prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                    string ip = "";
                    string dir = AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles\\";
                    //If Not Directory.Exists(dir) Then
                    //    Directory.CreateDirectory(dir)
                    //End If
                    strFilePath = (dir + APIName + "Log" + Convert.ToString("_")) + UTC_To_IST().ToString("yyyyMMdd") + ".txt";

                    dynamic maxRetry = 2;

                    //for Error: The process cannot access the file because it is being used by another process
                    for (int retry = 0; retry <= maxRetry - 1; retry++)
                    {
                        try
                        {
                            if (Action == 1) //komal 12-9-2018 multi line log
                            {
                                using (StreamWriter sw = new StreamWriter(strFilePath, true))
                                {
                                    //sw.WriteLine("----------------------------------------{0}---------------------------------------------------------", System.Web.HttpContext.Current.Request.UserHostAddress.ToString());
                                    sw.WriteLine("----------------------------------------{0}---------------------------------------------------------", ip);
                                    sw.WriteLine((Convert.ToString("-----> ") + methodName) + " " + " Time " + UTC_To_IST());
                                    sw.WriteLine(" Response : {0}", inputString);
                                    //sw.WriteLine("----------------------------------------{0}---------------------------------------------------------", ip);
                                    //sw.WriteLine("");
                                    sw.Flush();
                                    sw.Close();
                                }
                                break;
                            }
                            else if (Action == 2) // komal 12-9-2018 two line log
                            {
                                using (StreamWriter sw = new StreamWriter(strFilePath, true))
                                {
                                    //sw.WriteLine("----------------------------------------{0}---------------------------------------------------------", System.Web.HttpContext.Current.Request.UserHostAddress.ToString());
                                    sw.WriteLine("----------------------------------------{0}---------------------------------------------------------", ip);
                                    sw.WriteLine((Convert.ToString("-----> ") + methodName) + ":" + " Time " + UTC_To_IST() + ": Response - {0}", inputString);
                                    //sw.WriteLine(" Response : {0}", inputString);
                                    //sw.WriteLine("");
                                    sw.Flush();
                                    sw.Close();
                                }
                                break;
                            }
                            // TODO: might not be correct. Was : Exit For
                            //'Exit if success, either write three times
                        }
                        catch (IOException generatedExceptionName)
                        {
                            if (retry < maxRetry - 1)
                            {
                                WriteErrorLog(generatedExceptionName, "Statuscheck app", "WriteRequestLog -Retry:Sleep");
                                // Wait some time before retry (2 secs)
                                Thread.Sleep(2000);
                                // handle unsuccessfull write attempts or just ignore.
                            }
                            else
                            {
                                WriteErrorLog(generatedExceptionName, "Statuscheck app", "WriteRequestLog-Retry Complete");
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Status check app", System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + methodName);
                ex = null;
            }
        }
        #endregion

        #region CommonFunction

        private void GetHistory(ref CommonMethods CommonMethod, walletServiceData walletServiceDataObj, string trnID)
        {
            DataSet dSet = new DataSet();
            DataRow dRows = null;
            string SqlStr = string.Empty;
            decimal amount = 0;
            long trnno = 0;
            long autono = 0;
            string queryResult = "";
            int dayDiff = 1;
            try
            {
                //isreturnwithfailure = false;
                //if (IsProcessing)
                //{
                //    isreturnwithfailure = true;
                //    logs.WriteRequestLog("IsProcessing = true so return ", "CallAPISingle", CommonMethod.SMSCode, Action: 2);
                //    return;
                //}
                //logs.WriteRequestLog("Fetching Rows :  " , "GetHistory", CommonMethod.SMSCode);
                //  dayDiff = walletServiceDataObj.ValidDays; //komal 10/9/2018 get valid days from table
                //dayDiff = (Configuration["DayDiff"] != string.Empty? Convert.ToInt16(Configuration["DayDiff"]):1);
                // ntrivedi 28-05-2018 temperory top 1
                SqlStr = "SELECT top " + walletServiceDataObj.RecordCount.ToString() + " ID, TrnID , Address, SMSCode, Amount, Confirmations,OrderID,isNull(FromAddress,'') as FromAddress From DepositHistory WHERE SMSCode = '" + CommonMethod.SMSCode + "' AND Status = 0 AND IsProcessing = 0  order by UpdatedDate";
                dSet = (new DataCommon()).OpenDataSet("DepositHistory", SqlStr, dSet, 30);
                if (dSet.Tables[0].Rows.Count > 0)
                {
                    //logs.WriteRequestLog("Fetching Rows :  " + dSet.Tables[0].Rows.Count, "GetHistory", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]),Action: 2);
                    Console.WriteLine("Fetch Rows:" + dSet.Tables[0].Rows.Count);
                    IsProcessing = true;
                    WriteRequestLog("IsProcessing = true + Fetching Rows :  " + dSet.Tables[0].Rows.Count, "GetHistory", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2); // komal 10/9/2018 log for IsProcessing = true
                    foreach (DataRow dRow in dSet.Tables[0].Rows)
                    {
                        SqlStr = "UPDATE DepositHistory SET IsProcessing = 1,UpdatedDate=dbo.getistdate() WHERE Id = " + dRow["ID"].ToString() + ""; //ntrivedi 28-05-2018 trnid instead of AutoNo due to slow searching on TrnID
                        (new DataCommon()).ExecuteQuery(SqlStr);
                    }
                    //IsProcessing = false;  //komal 10/9/2018 move to CallAPISingle()
                    WriteRequestLog("Total Count :" + dSet.Tables[0].Rows.Count + " IsProcessing :" + IsProcessing.ToString(), "GetHistory", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);//komal 10/9/2018 test
                    foreach (DataRow dRow in dSet.Tables[0].Rows)
                    {
                        trnID = dRow["TrnID"].ToString();
                        amount = 0;
                        autono = Convert.ToInt64(dRow["ID"]);
                        WriteRequestLog("New Loop TrnID :" + dRow["TrnID"].ToString() + " IsProcessing :" + IsProcessing.ToString() + "", "GetHistory", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                        String Response = CallThirdPartyAPI(ref CommonMethod, trnID, CommonMethod.Authorization, CommonMethod.enterprise, dSet); // Generate ThirdParty API Response

                        ; /*CallThirdPartyCryptoAPI(ref CommonMethod, dRow["Address"].ToString(), dRow["TrnID"].ToString(), dRow["AutoNo"].ToString())*/
                        if (Response == string.Empty) // ntrivedi 28-05-2018
                        {
                            SqlStr = "UPDATE DepositHistory SET IsProcessing = 0 WHERE ID = " + dRow["ID"].ToString() + "";
                            (new DataCommon()).ExecuteQuery(SqlStr);
                            WriteRequestLog("Thirdparty Response Null for TrnID :  " + dRow["TrnID"].ToString(), "GetHistory", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                            continue;
                        }
                        BitGoResponses TPGenerateResponse = new BitGoResponses();
                        RespTransfers transferObj = new RespTransfers();
                        TPGenerateResponse = JsonConvert.DeserializeObject<BitGoResponses>(Response);
                        //JObject GenerateResponse = JObject.Parse(Response);
                        if (TPGenerateResponse != null)
                        {
                            if (TPGenerateResponse.coin.ToLower() == CommonMethod.SMSCode.ToLower() && TPGenerateResponse.type == "receive") // For BTC , BTG , BCH Response 
                            {
                                if (TPGenerateResponse.entries != null)
                                {
                                    string trnid = dSet.Tables[0].Rows[0]["TrnID"].ToString();
                                    trnid = dSet.Tables[0].Rows[0]["SMSCode"].ToString();

                                    if (TPGenerateResponse.txid == (dSet.Tables[0].Rows[0]["TrnID"]).ToString() && TPGenerateResponse.coin.ToLower() == (dSet.Tables[0].Rows[0]["SMSCode"]).ToString().ToLower())
                                    {
                                        if (TPGenerateResponse.entries != null)
                                        {
                                            foreach (var iv in TPGenerateResponse.entries)
                                            {
                                                if (!string.IsNullOrEmpty(iv.address) && iv.value > 0)
                                                {
                                                    if (iv.value <= 0)
                                                    {
                                                        SqlStr = "UPDATE DepositHistory SET IsProcessing = 0,Status=9,StatusMsg='negative amount' WHERE ID = " + dRow["ID"].ToString() + "";
                                                        (new DataCommon()).ExecuteQuery(SqlStr);
                                                        continue;
                                                    }
                                                    if (iv.address.ToString().Equals(dSet.Tables[0].Rows[0]["address"]) && iv.wallet == CommonMethod.ProviderWalletID)
                                                    {
                                                        // iv["address"] = iv["address"].ToString();
                                                        //iv["Amount"] = Convert.ToDecimal(iv["value"]) / Convert.ToDecimal(Configuration[Convert.ToString(iv["btc"])]);
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
                                                            WriteRequestLog("confirmations < 0 :  " + transferObj.txid, "GetHistory", CommonMethod.SMSCode, Action: 2);
                                                            SqlStr = "UPDATE DepositHistory SET IsProcessing = 0,Status=9,StatusMsg='negative confirmation' WHERE ID = " + dRow["ID"].ToString() + "";
                                                            (new DataCommon()).ExecuteQuery(SqlStr);
                                                            continue;
                                                        }

                                                        //System.DateTime dateTime1 = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                                                        transferObj.valueStr = TPGenerateResponse.valueString;
                                                        transferObj.unconfirmedTime = TPGenerateResponse.unconfirmedTime.ToString();
                                                        transferObj.confirmedTime = TPGenerateResponse.confirmedTime.ToString();
                                                        CommonMethod.Transfers.Add(transferObj);
                                                    }
                                                    else
                                                    {
                                                        SqlStr = "UPDATE DepositHistory SET IsProcessing = 1,Status=9,StatusMsg='mismatch walletid or address' WHERE Address='"+ iv.address.ToString() + "' and ID = " + dRow["ID"].ToString() + "";
                                                        (new DataCommon()).ExecuteQuery(SqlStr);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    SqlStr = "UPDATE DepositHistory SET IsProcessing = 1,Status=9,StatusMsg='thirparty amount not found' WHERE ID = " + dRow["ID"].ToString() + "";
                                    (new DataCommon()).ExecuteQuery(SqlStr);
                                }
                            }

                            else
                            {
                                SqlStr = "UPDATE DepositHistory SET IsProcessing = 1,Status=9,StatusMsg='error not null Thirpdarty response' WHERE ID = " + dRow["ID"].ToString() + "";
                                (new DataCommon()).ExecuteQuery(SqlStr);
                            }
                            WriteRequestLog("Loop done :" + dRow["TrnID"].ToString() + " IsProcessing :" + IsProcessing.ToString() + "", "GetHistory", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                        }

                        Console.WriteLine(CommonMethod.SMSCode + " Count : " + CommonMethod.Transfers.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                SqlStr = "UPDATE DepositHistory SET IsProcessing = 0 WHERE ID = " + autono + "";
                (new DataCommon()).ExecuteQuery(SqlStr);
                WriteErrorLog(ex, "Program", "GetHistory");
            }
        }
        public  DateTime UTC_To_IST()
        {
            DateTime myUTC = DateTime.UtcNow;
            // 'Dim utcdate As DateTime = DateTime.ParseExact(DateTime.UtcNow, "M/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
            // Dim utcdate As DateTime = DateTime.ParseExact(myUTC, "M/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
            // 'Dim utcdate As DateTime = DateTime.ParseExact("11/09/2016 6:31:00 PM", "M/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
            DateTime istdate = TimeZoneInfo.ConvertTimeFromUtc(myUTC, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            // MsgBox(myUTC & " - " & utcdate & " - " & istdate)
            return istdate;

        }

        private void TradeDepositHistoryUpdationForBitgo(ref CommonMethods CommonMethod)
        {
            DataSet dSet = new DataSet();
            string SqlStr = string.Empty;
            string queryResult;
            try
            {
                if (CommonMethod.Transfers.Count > 0)
                {
                    foreach (var item in CommonMethod.Transfers)
                    {
                        // update                         
                        CommonMethod.SqlStr = "UPDATE DepositHistory SET Amount=" + item.Amount + ",Confirmations =" + item.confirmations + ", ConfirmedTime ='" + item.confirmedTime + "', epochtimepure  ='" + item.unconfirmedTime + "', UpdatedDate = dbo.GetISTDate(),SystemRemarks='Str Amt:" + item.valueStr + "' WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                        (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                        WriteRequestLog("Update Deposit History :  " + item.txid, "TradeDepositHistoryUpdationForBitgo", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                        // ntrivedi temperory
                        //if ((new DataCommon()).ExecuteScalarWDM("Select Count(TrnID) From TradeDepositCompletedTrn Where TrnID='" + item.txid + "' and Address='" + item.address + "'") == "0")
                        //{
                            // ntrivedi 27-06-2018 IF fromaddress(DepositHistory) is fix DT address then debit from Distributor otherwise debit from ORG 
                            //if (!string.IsNullOrEmpty(item.fromAddress))
                            //{
                            //    SqlStr = "select * from AddressMaster Where PublicAddress=@PublicAddress and Status=1";
                            //    queryResult = (new DataCommon()).ExecuteScalarWDMParameterize("@PublicAddress", item.fromAddress, SqlStr);
                            //    if (string.IsNullOrEmpty(queryResult))
                            //    {
                            //        CommonMethod.DMemberID = 1;// organization
                            //    }
                            //    else
                            //    {
                            //        CommonMethod.DMemberID = Convert.ToInt64(queryResult);
                            //    }
                            //}

                            SqlStr = "select top 1 UserID from BizUserTypeMapping where UserType=0 order by UserID";
                            queryResult = (new DataCommon()).ExecuteScalarWDM(SqlStr);// organization
                            if (string.IsNullOrEmpty(queryResult))
                            {
                                SqlStr = "UPDATE DepositHistory SET StatusMsg='Org Record Not found',Status=9,IsProcessing = 0 WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                                (new DataCommon()).ExecuteQuery(SqlStr);
                                CommonMethod.DMemberID = 0;
                                continue;
                            }
                            else
                            {
                                CommonMethod.DMemberID = Convert.ToInt64(queryResult);
                            }

                            SqlStr = "select ID from AddressMasters Where Address=@PublicAddress and Status=1";
                            queryResult = (new DataCommon()).ExecuteScalarWDMParameterize("@PublicAddress", item.address, SqlStr);
                            if (string.IsNullOrEmpty(queryResult))
                            {
                                SqlStr = "UPDATE DepositHistory SET StatusMsg='Address Not found',Status=9,IsProcessing = 0 WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                                (new DataCommon()).ExecuteQuery(SqlStr);
                                CommonMethod.DMemberID = 0;
                                continue;
                            }

                            //if (item.OrderId == 0)
                            //{
                            //    // create Order
                            //    //CreateOrder(ref CommonMethod, item);
                            //}

                            // Delivery Process Order
                            if (item.confirmations >= 3) ////ntrivedi 11-05-2018 
                            {
                            //DeliveryProcessOrder(ref CommonMethod, item, 1, CommonMethod.DMemberID);
                            // trn pr , status ,date 
                            // ntrivedi move inside delivery process order function
                            //SqlStr = "INSERT INTO TradeDepositCompletedTrn(TrnID, Status, CreatedTime) VALUES('" + item.txid + "', 1 , dbo.GetISTDate())";
                            //(new DataCommon()).ExecuteQuery(SqlStr);
                            //WalletService walletService = new WalletService();
                            //WalletDrCrResponse walletDrCrResponse =   _walletService.DepositionWalletOperation("123456", item.address, item.coin, item.Amount, item.id, enServiceType.WalletService, enWalletTrnType.cr_Deposit,enWalletTranxOrderType.Credit,enWalletLimitType.DepositLimit);
                            //_walletService.GetAddress("123");
                                //if(walletDrCrResponse.ReturnCode == 0)
                                //{
                                //    SqlStr = "UPDATE DepositHistory SET OrderID=" + walletDrCrResponse.TrnNo + ",StatusMsg='Success',Status=1,IsProcessing = 9 WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                                //    (new DataCommon()).ExecuteQuery(SqlStr);
                                //}
                                //else
                                //{
                                //    SqlStr = "UPDATE DepositHistory SET OrderID=" + walletDrCrResponse.TrnNo  + ",StatusMsg='" + walletDrCrResponse.StatusMsg +"',Status=1,IsProcessing = 9 WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                                //    (new DataCommon()).ExecuteQuery(SqlStr);
                                //}
                            }
                        //}
                        //else // Order Already Intialized
                        //{
                        //    SqlStr = "UPDATE DepositHistory SET status = 9 WHERE ID=" + item.id; //TrnID = '" + item.txid + "'";
                        //    (new DataCommon()).ExecuteQuery(SqlStr);
                        //    WriteRequestLog("Order Already Intialized | " + item.txid, "TradeDepositHistoryUpdationForCryptoCoin", CommonMethod.SMSCode, Action: 2);
                        //}

                        SqlStr = "UPDATE DepositHistory SET IsProcessing = 0 WHERE AutoNo=" + item.id; //TrnID = '" + item.txid + "'";
                        (new DataCommon()).ExecuteQuery(SqlStr);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Program", "TradeDepositHistoryUpdationForBitgo");
            }
        }
        private void CallAPISingle(object RefObj)
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
                                //CallThirdPartyAPI(ref CommonMethod,trnID,Authorization,enterprise,null); // Generate ThirdParty API Response
                                //TradeDepositHistoryInsertion(ref CommonMethod); // Insert Trade Deposit History
                                break;
                            case (int)EnAppType.CryptoAPI:
                                //if (!string.IsNullOrEmpty(CommonMethod.UserName) && !string.IsNullOrEmpty(CommonMethod.Password) && !string.IsNullOrEmpty(CommonMethod.RequestBody))
                                //{
                                //GetHistory(ref CommonMethod, walletServiceDataObj, CommonMethod.trnID, CommonMethod.Authorization, CommonMethod.enterprise); // Get History From Deposit History SMScode Wise 
                                //TradeDepositHistoryUpdationForCryptoCoin(ref CommonMethod); // Update Crypto coin into Trade Deposit History
                                //}
                                //else
                                //{
                                //    logs.WriteRequestLog("Transaction Detail not found", "CallAPISingle", CommonMethod.SMSCode, Action: 2);
                                //}
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        WriteRequestLog("Transaction Detail not found", "CallAPISingle", CommonMethod.SMSCode, Action: 2);
                    }
                }
                else
                {
                    WriteRequestLog("Master File Detail not found", "CallAPISingle", CommonMethod.SMSCode, Action: 2);
                }
            }
            catch (Exception ex)
            {
                //ex = null;
                WriteErrorLog(ex, "Program", "CallAPISingle");
            }
            finally
            {
                IsProcessing = false; //komal 10/9/2018 finally stop thread 
                WriteRequestLog("IsProcessing = false ", "CallAPISingle", smscode, Action: 2);
            }
        }
        #endregion

        #region "ReadMasterFile"

        public void ReadMasterFile(string APIName, ref CommonMethods CommonMethod)
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

                    WriteRequestLog("Transaction File Path :  " + CommonMethod.Path_AddressGenerate, "ReadMasterFile", CommonMethod.SMSCode);
                }
                else
                {

                    WriteRequestLog(FilePath + " File Not Found", "ReadMasterFile", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Program", "ReadMasterFile");
            }
        }

        public void ReadTransactionalFile(string Path, ref CommonMethods CommonMethod)
        {

            try
            {
                //CommonMethods CommonMethod = (CommonMethods)CommonMethodObj;
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
                        //Console.WriteLine(CommonMethod.Str_URL + CommonMethod.Str_RequestType + CommonMethod.ContentType);
                    }

                    WriteRequestLog("Transaction URL :  " + CommonMethod.Str_URL + " Request Type : " + CommonMethod.Str_RequestType + " Content Type : " + CommonMethod.ContentType, "ReadTransactionalFile", CommonMethod.SMSCode);
                }
                else
                {
                    WriteRequestLog(Path + " File Not Found", "ReadTransactionalFile", CommonMethod.SMSCode, Convert.ToInt16(Configuration["AllowLog"]), Action: 2);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Program", "ReadTransactionalFile");
            }
        }
        #endregion


        #region "CallThirdPartyAPI"

        private string CallThirdPartyAPI(ref CommonMethods CommonMethod, string trnID, string Authorization, string enterprise, DataSet dSet)
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
                    WriteRequestLog("Request :  " + CommonMethod.Str_URL, "CallThirdPartyAPI", CommonMethod.SMSCode);

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
                    WriteRequestLog("Response :  " + CommonMethod.DepositHistoryResponse, "CallThirdPartyAPI", CommonMethod.SMSCode);
                    //JObject GenerateResponse = JObject.Parse(CommonMethod.DepositHistoryResponse);
                    // ntrivedi parsing will be in  caller method 25-10-2018
                    //JArray jarr = (JArray)person1.SelectToken("transfers");
                    //if (Convert.ToString(GenerateResponse.SelectToken("coin")).ToLower() == CommonMethod.SMSCode.ToLower())
                    //{
                    //    if (CommonMethod.Category == 1) // For BTC , BTG , BCH Response 
                    //    {
                    //        if(GenerateResponse !=null)
                    //        {
                    //            if(GenerateResponse.SelectToken("txid").ToString().Equals(dSet.Tables[0].Rows[0]["TrnID"]) && GenerateResponse.SelectToken("coin").ToString().Equals(dSet.Tables[0].Rows[0]["SMSCode"]))
                    //            {
                    //                if (GenerateResponse.SelectToken("entries") != null)
                    //                {
                    //                    foreach (var iv in GenerateResponse.SelectToken("entries"))
                    //                    {
                    //                        if (!IsNullOrEmpty(iv["address"]) && !IsNullOrEmpty(iv["value"]) && !IsNullOrEmpty(iv["valueString"]))
                    //                        {
                    //                            if (iv["address"].ToString().Equals(dSet.Tables[0].Rows[0]["address"]) && Convert.ToInt32(iv["value"]) >0 && Convert.ToDecimal(iv["value"])== Convert.ToDecimal(dSet.Tables[0].Rows[0]["Amount"]))
                    //                            {
                    //                                iv["address"] = iv["address"].ToString();
                    //                                iv["Amount"] = Convert.ToDecimal(iv["value"]) / Convert.ToDecimal(Configuration[Convert.ToString(iv["btc"])]);                       
                    //                            }
                    //                        }
                    //                    }
                    //                }
                    //            }                                                              
                    //        }                                                      
                    //    }
                    //    else if (CommonMethod.Category == 2) // For LTC Response
                    //    {
                    //        foreach (var item in GenerateResponse.SelectToken("transfers").Select((val, i) => new { i, val }))
                    //        {
                    //            item.val["IsValid"] = false;
                    //            if (!IsNullOrEmpty(item.val["entries"]) && !IsNullOrEmpty(item.val["id"]) && !IsNullOrEmpty(item.val["txid"]) && !IsNullOrEmpty(item.val["coin"]) && !IsNullOrEmpty(item.val["wallet"]) && !IsNullOrEmpty(item.val["value"]) && !IsNullOrEmpty(item.val["state"]))
                    //            {

                    //                foreach (var item1 in item.val.SelectToken("entries"))
                    //                {
                    //                    if (Convert.ToString(item1["value"]) == Convert.ToString(item.val["value"]) && !IsNullOrEmpty(item1["wallet"]))
                    //                    {
                    //                        string value = (string)item1.SelectToken("address");
                    //                        item.val["address"] = value;
                    //                        if (Convert.ToString(item.val["coin"]) == "xrp" && !Convert.ToString(item1.SelectToken("address")).Contains("?dt="))
                    //                        {
                    //                            item.val["IsValid"] = false;
                    //                            continue;
                    //                        }
                    //                        item.val["Amount"] = Convert.ToDecimal(item.val["value"]) / Convert.ToDecimal(Configuration[Convert.ToString(item.val["coin"])]);
                    //                        item.val["IsValid"] = true;
                    //                    }
                    //                }
                    //            }
                    //            if (Convert.ToBoolean(item.val["IsValid"]))
                    //            {
                    //                CommonMethod.Transfers.Add(JsonConvert.DeserializeObject<RespTransfers>(JsonConvert.SerializeObject(item.val)));
                    //                Console.WriteLine(CommonMethod.SMSCode + " Count : " + CommonMethod.Transfers.Count);
                    //            }
                    //        }
                    //    }
                    //}
                    WriteRequestLog("Generate Response :  " + JsonConvert.SerializeObject(CommonMethod.Transfers), "CallThirdPartyAPI", CommonMethod.SMSCode);
                    return CommonMethod.DepositHistoryResponse;
                }
                else
                {
                    WriteRequestLog("Generate Response :  Transaction Detail not found", "CallThirdPartyAPI", CommonMethod.SMSCode, Action: 2);
                    return "";
                }

            }
            catch (Exception ex)
            {

                WriteErrorLog(ex, "Program", "CallThirdPartyAPI");
                throw ex;
            }
        }

        private string CallThirdPartyCryptoAPI(ref CommonMethods CommonMethod, string Address, string TrnID, string AutoNo)
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

                    WriteRequestLog("RPC Address generate Request :  " + ReqStr, "CallThirdPartyCryptoAPI", CommonMethod.SMSCode);
                    StreamWriter sw = new StreamWriter(myReqrpc.GetRequestStream());
                    sw.Write(ReqStr);
                    sw.Close();

                    WebResponse response = myReqrpc.GetResponse();

                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    CommonMethod.DepositHistoryResponse = StreamReader.ReadToEnd();
                    StreamReader.Close();
                    response.Close();

                    WriteRequestLog("RPC Address Response :  " + CommonMethod.DepositHistoryResponse, "CallThirdPartyCryptoAPI", CommonMethod.SMSCode);
                    return CommonMethod.DepositHistoryResponse;
                }
                else
                {
                    WriteRequestLog("Generate Response :  Transaction Detail not found", "CallThirdPartyCryptoAPI", CommonMethod.SMSCode);
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
                    WriteRequestLog("BlockChainTransfer exception : " + CommonMethod.DepositHistoryResponse, "CallThirdPartyCryptoAPI", CommonMethod.SMSCode, Action: 2);
                }

                WriteRequestLog("webex : " + webex, "CallThirdPartyCryptoAPI", CommonMethod.SMSCode, Action: 2);
                return CommonMethod.DepositHistoryResponse;
            }
        }

        #endregion
    }
}
