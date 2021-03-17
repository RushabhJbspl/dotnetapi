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
using CleanArchitecture.Infrastructure.Interfaces;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.ViewModels.Wallet;
using CleanArchitecture.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Core.Interfaces.Repository;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Entities;
using CleanArchitecture.Core.Entities.Wallet;
using CleanArchitecture.Infrastructure.Data.Transaction;
using CleanArchitecture.Infrastructure;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Core.Services;
using Microsoft.AspNetCore.Http;
using CleanArchitecture.Core.Services.RadisDatabase;
using System.Text.RegularExpressions;

namespace ETHStatusCheckApp
{
    class ETHStatusCheckApp
    {
        static System.Timers.Timer TransactionTick = new System.Timers.Timer();
        public static IConfiguration Configuration { get; set; }
        static bool IsProcessing = false;

        static void Main(string[] args)
        {
            Console.Write("start");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            TransactionTick.Interval = Convert.ToInt32(Configuration["Interval"]); // 1000;
            TransactionTick.Elapsed += new ElapsedEventHandler(transaction_tick);
            TransactionTick.Start();
            //SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true); //komal 10/9/20108 process rudely exits, 
            Console.WriteLine("Press \'q\' to quit");
            while (Console.Read() != 'q') ;
        }


        #region TimerTick
        private static void transaction_tick(object sender, System.EventArgs e)
        {
            try
            {
                TransactionTick.Stop();
                //TransactionTick.Interval = 1000000; // For Testing temperory ntrivedi
                CallAPI();
                TransactionTick.Start();  //ntrivedi 27-10-2018 temperory test

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "transaction_tick", ex.Source.ToString());
                ex = null;
            }
        }
        #endregion

        #region CallBackProcess

        public static void CallAPI()
        {
            string SqlStr = string.Empty;
            DataSet dSet = new DataSet();
            Double cnt = 0;
            try
            {
                SqlStr = "SELECT Id,WalletId,WalletTypeId,TrnID,CoinName,BitgoWalletId,CoinSpecific FROM TradeBitGoDelayAddressess WHERE Status=1 AND GenerateBit=0";
                dSet = (new DataCommon()).OpenDataSet("TradeBitGoDelayAddressess", SqlStr, dSet, 30);
                if (dSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dRow in dSet.Tables[0].Rows)
                    {
                        cnt++;
                        // Need common row object for each SMScode
                        walletServiceData walletServiceDataObj = new walletServiceData();
                        walletServiceDataObj.AppType = Convert.ToInt16(1);
                        walletServiceDataObj.WalletID = Convert.ToInt32(dRow["WalletID"]);
                        walletServiceDataObj.WalletTypeID = Convert.ToInt32(dRow["WalletTypeId"]);
                        walletServiceDataObj.TrnID = dRow["TrnID"].ToString();
                        walletServiceDataObj.CoinName = dRow["CoinName"].ToString();
                        walletServiceDataObj.SMSCode = dRow["CoinName"].ToString();
                        walletServiceDataObj.BitgoWalletID = dRow["BitgoWalletId"].ToString();
                        walletServiceDataObj.CoinSpecific = dRow["CoinSpecific"].ToString();

                        Console.Title = walletServiceDataObj.SMSCode + " ETH Status Check App ";

                        WriteRequestLog("New Timer Call For Id: " + dRow["Id"].ToString(), "CallAPI", walletServiceDataObj.SMSCode);
                        lock (walletServiceDataObj)
                        {
                            if (IsProcessing == false)
                            {
                                Console.WriteLine("New Timer Call : " + cnt);
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
                TransactionTick.Start();
            }
        }
        private static void CallAPISingle(object RefObj)
        {

            string TempUrl;
            DataSet DSet = null;
            string smscode = "eth";/*string trnID= "ddaa745649f8f09564daa1aaa51716ef49175b328c8d87aba4b207b3f178d620"; string Authorization = "Bearer v2x5e587227b1085bdcd4d5bd3421f0a702fff86d2137af0cf3a8624655d4e42f71"; string enterprise = "5a79359692f6f738073dffcbbb0c22df";*/
            try
            {
                BitGoResponses response = new BitGoResponses();
                walletServiceData walletServiceDataObj = (walletServiceData)RefObj;
                CommonMethods CommonMethod = new CommonMethods();
                CommonMethod.Transfers = new List<RespTransfers>(); // Need object of RespTransfers  for Thirdparty ApI response
                //CommonMethod.RespLocalCoins = new List<RespLocalCoin>(); // Need object of RespLocalCoins  for Thirdparty ApI response
                CommonMethod.SMSCode = walletServiceDataObj.SMSCode;
                smscode = CommonMethod.SMSCode;
                ReadMasterFile(walletServiceDataObj.SMSCode, ref CommonMethod); // Read  Master File
                if (!string.IsNullOrEmpty(CommonMethod.Path_AddressGenerate))
                {
                    ReadTransactionalFileETH(CommonMethod.Path_StatusCheck, ref CommonMethod); // Read Transaction file for specific coin               
                    if (!string.IsNullOrEmpty(CommonMethod.Path_StatusCheck) && !string.IsNullOrEmpty(CommonMethod.Str_RequestType) && !string.IsNullOrEmpty(CommonMethod.ContentType))
                    {
                        switch (walletServiceDataObj.AppType)
                        {
                            case (int)EnAppType.BitGoAPI:
                                if (!TakePendingAddress_ETH(ref DSet))
                                {
                                    WriteRequestLog("Transaction Detail not found", "CallAPISingle", CommonMethod.SMSCode, Action: 2);
                                    return;
                                }
                                foreach (DataRow drow in DSet.Tables[0].Rows)
                                {
                                    TempUrl = CommonMethod.Str_URL;//every time assign fresh url for replacement
                                    TempUrl = TempUrl.Replace("#TrnID#", drow["TrnID"].ToString());
                                    string RequestType = CommonMethod.RequestType;
                                    string Resp = CallToThirdPartyAPIMethodV1(TempUrl, RequestType);

                                    //Resp = "{\"id\":\"5ad5d23d623b768f1c8d49acc0c04135\",\"address\":\"\",\"chain\":10,\"index\":2,\"coin\":\"btc\",\"wallet\":\"5a969f95528688e03d1625503b286a81\",\"coinSpecific\":{\"redeemScript\":\"00206bbe938e84f03d763e955a2a9e806a55dbb0f803b9c67c8ea641c96ba4c87915\",\"witnessScript\":\"522102f949636c9bfc1ec2411907cf1b50082e9c53cbe51343ff8b88562bfbbb03a57e21039b8691fe6c084c3af2211dd73afc1a3c55392019675618ddee88dbd12fd0687a210240e277667a571e061b64e000f36aadfad97bb6f302dfc4206ebeb5f0b6906fa653ae\"}}";

                                    if (!string.IsNullOrEmpty(Resp))
                                    {
                                        CommonMethod.MatchRegex2 = ParseResponse(Resp, CommonMethod.Address1, CommonMethod.Address2);
                                        response.address = CommonMethod.MatchRegex2;

                                        CommonMethod.MatchRegex2 = ParseResponse(Resp, CommonMethod.TrnID1, CommonMethod.TrnID2);
                                        response.txid = CommonMethod.MatchRegex2;

                                        CommonMethod.MatchRegex2 = ParseResponse(Resp, CommonMethod.coinSpecific1, CommonMethod.coinSpecific2);
                                        response.coinSpecific = CommonMethod.MatchRegex2;

                                        CommonMethod.MatchRegex2 = ParseResponse(Resp, CommonMethod.Wallet1, CommonMethod.Wallet2);
                                        response.wallet = CommonMethod.MatchRegex2;

                                        if (!string.IsNullOrEmpty(response.address))
                                        {
                                            bool SuccessBit;
                                            if (!String.IsNullOrEmpty(response.txid) && response.txid == drow["TrnId"].ToString())
                                            {
                                                //Insert Generated Address
                                                SuccessBit = UpdateAddress_ETH(Convert.ToInt64(drow["Id"]), drow["TrnID"].ToString(), response.address, response.coinSpecific);
                                            }
                                            else
                                            {
                                                WriteRequestLog("Transaction Id Not Match", "CallAPISingle", CommonMethod.SMSCode, Action: 2);
                                            }
                                            WriteRequestLog("Address Not Found", "CallAPISingle", CommonMethod.SMSCode, Action: 2);
                                        }
                                    }
                                }
                                break;
                            case (int)EnAppType.CryptoAPI:
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
                IsProcessing = false;
                WriteRequestLog("IsProcessing = false ", "CallAPISingle", smscode, Action: 2);
            }
        }

        #endregion

        #region Logging

        public static void WriteLogIntoFile(byte CheckReq_Res, DateTime Date, string MethodName, string Controllername, string Req_Res = null, string accessToken = null)
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
        public static void WriteErrorLog(Exception ex, string methodName, string Source, string OtherInfo = "")
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
        public static void WriteRequestLog(string inputString, string methodName, string APIName, int IsAllow = 1, int Action = 1)
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
                        else if (CommonMethod.LeftTitle == "addressgenerate")
                        {
                            CommonMethod.Path_AddressGenerate = CommonMethod.TransactionFile + line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.LeftTitle == "statuscheck")
                        {
                            CommonMethod.Path_StatusCheck = CommonMethod.TransactionFile + line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.LeftTitle == "delayaddress")
                        {
                            CommonMethod.DelayAddress = Convert.ToInt32(line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1));
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
        public static void ReadTransactionalFileETH(string Path, ref CommonMethods CommonMethod)
        {
            try
            {
                CommonMethod.MsgFile = new StreamReader(Path);
                using (StreamReader sr = CommonMethod.MsgFile)
                {
                    CommonMethod.line = sr.ReadLine(); //Read URL
                    CommonMethod.Str_URL = CommonMethod.line.Replace("URL:", "");

                    CommonMethod.line = sr.ReadLine();//Read Content Type
                    CommonMethod.ContentType = CommonMethod.line.Substring(CommonMethod.line.IndexOf(CommonMethod.MainSaperator) + 1).ToLower();

                    CommonMethod.line = sr.ReadLine();
                    CommonMethod.Str_RequestType = CommonMethod.line.Substring(CommonMethod.line.IndexOf(CommonMethod.MainSaperator) + 1).ToLower();
                    //CommonMethod.Str_Request = ""; //After login request clear this
                    while ((CommonMethod.line.ToUpper() != "END REQUEST"))
                    {
                        CommonMethod.line = sr.ReadLine();
                        if (CommonMethod.line.Contains("END REQUEST"))
                            continue;
                        CommonMethod.Str_Request += CommonMethod.line;
                    }

                    CommonMethod.line = sr.ReadLine();
                    while ((CommonMethod.line.ToUpper() != "END RESPONSE"))
                    {
                        CommonMethod.line = sr.ReadLine();
                        if (CommonMethod.line.Contains("END RESPONSE"))
                            continue;
                        CommonMethod.LeftTitle = CommonMethod.line.Substring(0, CommonMethod.line.IndexOf(CommonMethod.MainSaperator)).ToLower();

                        switch (CommonMethod.LeftTitle)
                        {
                            case "address1":
                                CommonMethod.Address1 = CommonMethod.line.Substring(CommonMethod.line.IndexOf(CommonMethod.MainSaperator) + 1);
                                break;
                            case "address2":
                                CommonMethod.Address2 = CommonMethod.line.Substring(CommonMethod.line.IndexOf(CommonMethod.MainSaperator) + 1);
                                break;
                            case "trnid1"://rita 25-4-18 for ETH
                                CommonMethod.TrnID1 = CommonMethod.line.Substring(CommonMethod.line.IndexOf(CommonMethod.MainSaperator) + 1);
                                break;
                            case "trnid2":
                                CommonMethod.TrnID2 = CommonMethod.line.Substring(CommonMethod.line.IndexOf(CommonMethod.MainSaperator) + 1);
                                break;
                            case "coinspecific1"://rita 25-4-18 for ETH
                                CommonMethod.coinSpecific1 = CommonMethod.line.Substring(CommonMethod.line.IndexOf(CommonMethod.MainSaperator) + 1);
                                break;
                            case "coinspecific2":
                                CommonMethod.coinSpecific2 = CommonMethod.line.Substring(CommonMethod.line.IndexOf(CommonMethod.MainSaperator) + 1);
                                break;
                            case "wallet1"://rita 25-4-18 for ETH
                                CommonMethod.Wallet1 = CommonMethod.line.Substring(CommonMethod.line.IndexOf(CommonMethod.MainSaperator) + 1);
                                break;
                            case "wallet2":
                                CommonMethod.Wallet2 = CommonMethod.line.Substring(CommonMethod.line.IndexOf(CommonMethod.MainSaperator) + 1);
                                break;
                        }//Switch END         requesttype          
                    }// While of RESPONSE END
                    CommonMethod.line = sr.ReadLine();
                    if (CommonMethod.line == null)//no line so error occured
                        return;

                    while ((CommonMethod.line.ToUpper() != "END SUCCESSCODE"))
                    {
                        CommonMethod.line = sr.ReadLine();
                        CommonMethod.SuccAPICodes = CommonMethod.line;
                        CommonMethod.SuccAPICodesArray = CommonMethod.SuccAPICodes.Split(',');
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Program", "ReadTransactionalFile");
            }
        }

        #endregion

        #region "CallThirdPartyAPI"

        public static string CallToThirdPartyAPIMethodV1(string UrlString, String MethodType = "POST", Boolean Write = true, String Username = "", String password = "", String Req = "")
        {
            string responseFromServer = "";
            try
            {
                WriteRequestLog("" + UrlString, "ThirdParty URL :", "CallToThirdPartyAPIMethodV1");
                if (MethodType.ToUpper() == "RPC")
                {
                    try
                    {
                        string authInfo = Username + ":" + password;
                        authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                        WebRequest myReqrpc = WebRequest.Create(UrlString);
                        myReqrpc.Headers["Authorization"] = "Basic " + authInfo;
                        myReqrpc.Method = "Post";

                        string ReqStr = Req;//@"{""id"":""" + 1 + @""",""method"":""getnewaddress"",""params"":[]}";
                        WriteRequestLog("" + ReqStr, " RPC Address generate Req =", "CallToThirdPartyAPIMethodV1");

                        StreamWriter sw = new StreamWriter(myReqrpc.GetRequestStream());
                        sw.Write(ReqStr);
                        sw.Close();

                        WebResponse response = myReqrpc.GetResponse();

                        StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                        responseFromServer = StreamReader.ReadToEnd();
                        if (Write)
                            WriteRequestLog("" + responseFromServer, "RPC Response :", "CallToThirdPartyAPIMethodV1");
                        StreamReader.Close();
                        response.Close();
                        return responseFromServer;
                    }
                    catch (WebException webex)
                    {
                        WebResponse errResp = webex.Response;
                        using (Stream respStream = errResp.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(respStream);
                            string Text = reader.ReadToEnd();
                            if (Text.ToLower().Contains("code"))
                                responseFromServer = Text;
                            webex = null;
                            WriteRequestLog("" + responseFromServer, "BlockChainTransfer exception =", "CallToThirdPartyAPIMethodV1");
                        }
                        WriteErrorLog(webex, "CallToThirdPartyAPIMethodV1", System.Reflection.MethodBase.GetCurrentMethod().Name);
                        return responseFromServer;
                    }
                }
                else
                {
                    string AccessToken = Configuration["BitGoToken"];
                    //string EnterpriseID = ConfigurationManager.AppSettings["BitGoEntID"];
                    object ResponseObj = new object();
                    //ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                            | SecurityProtocolType.Tls11
                                                            | SecurityProtocolType.Tls12;
                    //| SecurityProtocolType.Ssl3;
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(UrlString);
                    //httpWebRequest.ProtocolVersion = HttpVersion.Version10;
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Headers.Add("Authorization", AccessToken);
                    //httpWebRequest.Headers.Add("enterprise", EnterpriseID);
                    httpWebRequest.Method = MethodType;// "POST";
                    httpWebRequest.KeepAlive = false;
                    httpWebRequest.Timeout = 180000;

                    HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
                    using (StreamReader sr = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        responseFromServer = sr.ReadToEnd();
                        sr.Close();
                        sr.Dispose();

                    }
                    httpWebResponse.Close();
                    //aepsBase.WriteLogToDB(responseFromServer, MethodName, false, ref requestID);
                    if (Write)
                        WriteRequestLog("" + responseFromServer, "ThirdParty Response :", "CallToThirdPartyAPIMethodV1");
                    return responseFromServer;
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "CallToThirdPartyAPIMethodV1", System.Reflection.MethodBase.GetCurrentMethod().Name);
                return "";
                //throw ex;
            }
        }

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

        #region CommonFunction

        public static Boolean UpdateAddress_ETH(Int64 AutoID, string TrnID, string Address, string CoinSpecific)
        {
            string sqlStr = "";
            try
            {
                sqlStr = "Update TradeBitGoDelayAddressess SET CoinSpecific='" + CoinSpecific + "',GenerateBit=1,Address='" + Address + "',UpdatedDate=dbo.GetISTDate() Where Id=" + AutoID;
                int Count = (new DataCommon()).ExecuteQuery(sqlStr);
                if (Count == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "UpdateAddress_ETH", System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public static DateTime UTC_To_IST()
        {
            DateTime myUTC = DateTime.UtcNow;
            DateTime istdate = TimeZoneInfo.ConvertTimeFromUtc(myUTC, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            return istdate;
        }

        public static string ParseResponse(string StrResponse, string regex1, string regex2)
        {
            string MatchRegex = "";
            string MatchRegex2 = "";
            try
            {
                if (regex1 != null && regex2 != null)
                {
                    MatchRegex = Regex.Match(StrResponse.ToLower(), regex1.ToLower(), new RegexOptions()).Value;
                    if ((!string.IsNullOrEmpty(MatchRegex)))
                    {
                        MatchRegex2 = Regex.Replace(MatchRegex, regex2.ToLower(), "");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "ParseResponse", System.Reflection.MethodBase.GetCurrentMethod().Name + "::" + regex1 + "::" + regex2);
            }
            return MatchRegex2;
        }

        public static Boolean TakePendingAddress_ETH(ref DataSet Dset)
        {
            string sqlStr = String.Empty;

            try
            {
                sqlStr = "select top 10 ID,TrnID from TradeBitGoDelayAddressess where GenerateBit=0";
                Dset = (new DataCommon()).OpenDataSet("TradeBitGoDelayAddresses", sqlStr);
                if (Dset.Tables[0].Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "TakePendingAddress_ETH", System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw ex;
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
            public long WalletID { get; set; }
            public long WalletTypeID { get; set; }
            public string TrnID { get; set; }
            public string CoinName { get; set; }
            public string BitgoWalletID { get; set; }
            public string CoinSpecific { get; set; }
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
            public string Path_AddressGenerate, Path_StatusCheck, Path_CustomerDetail, Path_CustomerValidate, Path_CustomerRegistration, Path_BeneRegistration, Path_VerifyBeneficiary, Path_DeleteBeneficiary, Path_VerifyDeleteBeneficiary, PubKey;
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
            public string TrnID1, TrnID2, coinSpecific1, coinSpecific2, Wallet1, Wallet2; //rita 25-4-18 for ETH Reference No
            public string AssetFromRequest;
            public long DMemberID;
            public int DelayAddress = 0;
            public string Str_Request;
            public List<RespLocalCoin> RespLocalCoins { get; set; }
            public decimal ConvertAmt { get; set; } // ntrivedi
            public string ProviderWalletID { get; set; }

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
      
    }
}
