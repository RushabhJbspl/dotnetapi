using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Configuration;
using System.Timers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Text;

namespace DepositConsoleApplication
{
    public class Program
    {
        // static DataCommon dComm = new DataCommon();
        static CommonFunctions logs = new CommonFunctions();
        static System.Timers.Timer TopupTick = new System.Timers.Timer();
        static System.Timers.Timer TransactionTick = new System.Timers.Timer();
        //string Url = ConfigurationManager.AppSettings["CallURL"];
        static bool IsProcessing = false;
        static long LastLimit = 0;
        static string prevID = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        public static void Main(string[] args)
        {
            // ntrivedi when restarting app set tpspickupstatus = 0 
            //(new DataCommon()).ExecuteQuery("UPDATE MemberBankMaster SET TPSPickupStatus=0 where TPSPickupStatus=1");
            (new DataCommon()).ExecuteQuery("UPDATE DepositCounterMaster SET TPSPickupStatus=0 where WalletTypeID in (select ID from WalletTypeMasters where isdepositionallow=1)");
            TransactionTick.Interval = Convert.ToDouble( ConfigurationManager.AppSettings["CallTime"]); //12000; // 1000; temperory ntrivedi
            TransactionTick.Elapsed += new ElapsedEventHandler(transaction_tick);            
            TransactionTick.Start();

            Console.WriteLine("Press \'q\' to quit");
            while (Console.Read() != 'q') ;
            (new DataCommon()).ExecuteQuery("UPDATE DepositCounterMaster SET TPSPickupStatus=0 where WalletTypeID in (select ID from WalletTypeMasters where  isdepositionallow=1)");
        }

        //static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        (new DataCommon()).ExecuteQuery("UPDATE DepositCounterMaster SET TPSPickupStatus=0 where WalletTypeID in (select ID from WalletTypeMasters where  isdepositionallow=1)");
        //    }
        //    catch (Exception ex)
        //    {
        //        logs.WriteErrorLog(ex, "transaction_tick", ex.Source.ToString());
        //        ex = null;
        //    }

        //}
        #region TimerTick
        private static void transaction_tick(object sender, System.EventArgs e)
        {
            try
            {
                TransactionTick.Stop();
                // TransactionTick.Interval = 2000000; // For Testing
                CallAPI();
            }
            catch (Exception ex)
            {
                logs.WriteErrorLog(ex, "transaction_tick", ex.Source.ToString());
                ex = null;
            }
            finally
            {
                //TransactionTick.Start();// temperory ntrivedi
            }
        }
        #endregion

        #region CallBackProcess
        private static void CallAPI()
        {
            string SqlStr = string.Empty;
            DataSet dSet = new DataSet();
            try
            {
                //SqlStr = "SELECT w.ServiceID , s.SMSCode, w.AppType , w.Status AS WalletStatus , s.Status AS ServiceStatus FROM WalletMaster w INNER JOIN ServiceMaster s ON s.Serviceid = w.ServiceId WHERE w.IsDepositAPI = 1";
                SqlStr = "select cm.SerProID,cm.ID as AutoNo,WalletTypeName as SMSCode ,isNull(cm.prevIterationID,'') as prevIterationID,1 as AppType,RecordCount,Limit,isNull(LastTrnID, '') as LastTrnID,isNull(PreviousTrnID, '') as PreviousTrnID,MaxLimit " +
                 " from WalletTypeMasters WM inner join DepositCounterMaster CM  on CM.WalletTypeID = WM.ID " +
                 " where WM.IsDepositionAllow = 1 and WM.Status = 1 and TPSPickupStatus<>1";
                // ntrivedi 21-05-2018 skip organization address in fetching transaction list
                dSet = (new DataCommon()).OpenDataSet("WalletMaster", SqlStr, dSet, 30);
                if (dSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dRow in dSet.Tables[0].Rows)
                    {
                        // Need common row object for each SMScode
                        walletServiceData walletServiceDataObj = new walletServiceData();
                        //walletServiceDataObj.ServiceID = Convert.ToInt32(dRow["ServiceID"]);
                        walletServiceDataObj.AutoNo = Convert.ToInt64(dRow["AutoNo"]);
                        walletServiceDataObj.SMSCode = dRow["SMSCode"].ToString();
                        walletServiceDataObj.LastTrnID = dRow["LastTrnID"].ToString();
                        walletServiceDataObj.PreviousTrnID = dRow["PreviousTrnID"].ToString();
                        walletServiceDataObj.RecordCount = Convert.ToInt64(dRow["RecordCount"]);
                        walletServiceDataObj.Limit = Convert.ToInt64(dRow["Limit"]);
                        LastLimit = walletServiceDataObj.Limit;
                        walletServiceDataObj.MaxLimit = Convert.ToInt64(dRow["MaxLimit"]);
                        Console.Title = walletServiceDataObj.SMSCode + " Deposit" ; // ntrivedi easy to find 13-09-2018
                        //walletServiceDataObj.TrnCount = Convert.ToInt16( ConfigurationManager.AppSettings["TrnCount"]);
                        //walletServiceDataObj.WallletStatus = Convert.ToInt16(dRow["WalletStatus"]);
                        //walletServiceDataObj.ServiceStatus = Convert.ToInt16(dRow["ServiceStatus"]);
                        walletServiceDataObj.AppType = Convert.ToInt16(dRow["AppType"]); // 2 for Local Coin
                        walletServiceDataObj.prevIterationID = dRow["prevIterationID"].ToString();
                        walletServiceDataObj.serProID = Convert.ToInt64(dRow["serProID"]);

                        lock (walletServiceDataObj)
                        {
                            //if (walletServiceDataObj.SMSCode == "FUN") // for testing temperory
                            //{
                                Console.WriteLine(walletServiceDataObj.SMSCode);
                            (new DataCommon()).ExecuteQuery("UPDATE DepositCounterMaster SET TPSPickupStatus=1 WHERE ID=" + walletServiceDataObj.AutoNo);
                            WaitCallback callBack;
                            callBack = new WaitCallback(CallAPISingle); // create thread for each SMSCode
                            ThreadPool.QueueUserWorkItem(callBack, walletServiceDataObj);
                            //Thread.Sleep(100);
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //ex = null;
                logs.WriteErrorLog(ex, "Program", "CallAPI");
            }
            finally
            {
                TransactionTick.Start(); //temperory comment
            }
        }

        private static void CallAPISingle(object RefObj)
        {
            try
            {
                walletServiceData walletServiceDataObj = (walletServiceData)RefObj;
                CommonMethods CommonMethod = new CommonMethods();
                CommonMethod.Transfers = new List<RespTransfers>(); // Need object of RespTransfers  for Thirdparty ApI response
                //CommonMethod.RespLocalCoins = new List<RespLocalCoin>(); // Need object of RespLocalCoins  for Thirdparty ApI response
               CommonMethod.SMSCode = walletServiceDataObj.SMSCode;

                ReadMasterFile(walletServiceDataObj.SMSCode, ref CommonMethod); // Read  Master File
                if (!string.IsNullOrEmpty(CommonMethod.Path_AddressGenerate))
                {
                    ReadTransactionalFile(CommonMethod.Path_AddressGenerate, ref CommonMethod); // Read Transaction file for specific coin               
                    if (!string.IsNullOrEmpty(CommonMethod.Str_URL) && !string.IsNullOrEmpty(CommonMethod.Str_RequestType) && !string.IsNullOrEmpty(CommonMethod.ContentType))
                    {
                        switch (walletServiceDataObj.AppType)
                        {
                            case (int)EnAppType.BitGoAPI:
                                CallThirdPartyAPI(ref CommonMethod,walletServiceDataObj.prevIterationID, CommonMethod.authorization, CommonMethod.enterprise,CommonMethod.ProviderWalletID); // Generate ThirdParty API Response
                                //TradeDepositHistoryInsertion(ref CommonMethod); // Insert Trade Deposit History
                                TradeDepositHistoryInsertion(ref CommonMethod, walletServiceDataObj);
                                break;
                            case (int)EnAppType.CryptoAPI:
                                if (!string.IsNullOrEmpty(CommonMethod.UserName) && !string.IsNullOrEmpty(CommonMethod.Password) && !string.IsNullOrEmpty(CommonMethod.RequestBody))
                                {
                                   // CommonMethod.resultCount
                                    CallThirdPartyCryptoAPI(ref CommonMethod, walletServiceDataObj.RecordCount, walletServiceDataObj.Limit);
                                   // GetHistory(ref CommonMethod); // Get History From Deposit History SMScode Wise
                                    TradeDepositHistoryInsertion(ref CommonMethod, walletServiceDataObj); // Update Crypto coin into Trade Deposit History                                    
                                }
                                else
                                {
                                    logs.WriteRequestLog("Transaction Detail not found", "CallAPISingle", CommonMethod.SMSCode, Action: 2);
                                }
                                break;
                            case (int)EnAppType.EtherScan: // khushali 29-01-2019
                                if (!string.IsNullOrEmpty(CommonMethod.UserName) && !string.IsNullOrEmpty(CommonMethod.Password) && !string.IsNullOrEmpty(CommonMethod.RequestBody))
                                {
                                    // CommonMethod.resultCount
                                    CallThirdPartyERC20API(ref CommonMethod, walletServiceDataObj.prevIterationID, CommonMethod.authorization, CommonMethod.enterprise, CommonMethod.ProviderWalletID); // Generate ThirdParty API Response
                                    // GetHistory(ref CommonMethod); // Get History From Deposit History SMScode Wise
                                    TradeDepositHistoryInsertion(ref CommonMethod, walletServiceDataObj); // Update Crypto coin into Trade Deposit History                                    
                                }
                                else
                                {
                                    logs.WriteRequestLog("Transaction Detail not found", "CallAPISingle", CommonMethod.SMSCode, Action: 2);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        logs.WriteRequestLog("Transaction Detail not found", "CallAPISingle", CommonMethod.SMSCode, Action:2);
                    }
                }
                else
                {
                    logs.WriteRequestLog("Master File Detail not found", "CallAPISingle", CommonMethod.SMSCode,Action:2);
                }
            }
            catch (Exception ex)
            {
                //ex = null;
                logs.WriteErrorLog(ex, "Program", "CallAPISingle");
                walletServiceData walletServiceDataObj = (walletServiceData)RefObj;
                (new DataCommon()).ExecuteQuery("UPDATE DepositCounterMaster SET TPSPickupStatus=0 WHERE id=" + walletServiceDataObj.AutoNo);
            }
            finally
            {
                walletServiceData walletServiceDataObj = (walletServiceData)RefObj;
                (new DataCommon()).ExecuteQuery("UPDATE DepositCounterMaster SET TPSPickupStatus=0 WHERE id=" + walletServiceDataObj.AutoNo);
            }
        }

        //private static void GetHistory(ref CommonMethods CommonMethod)
        //{
        //    DataSet dSet = new DataSet();
        //    DataRow dRows = null;
        //    string SqlStr = string.Empty;            
        //    try
        //    {
        //        if (IsProcessing)
        //            return;
        //        SqlStr = "SELECT AutoNo, TrnID , Address, SMSCode,Value, Amount, Confirmations,OrderID From DepositHistory WHERE SMSCode = '" + CommonMethod.SMSCode + "' AND Status = 0 AND IsProcessing = 0";
        //        dSet = (new DataCommon()).OpenDataSet("DepositHistory", SqlStr, dSet, 30);

        //        if (dSet.Tables[0].Rows.Count > 0)
        //        {
        //            IsProcessing = true;
        //            foreach (DataRow dRow in dSet.Tables[0].Rows)
        //            {
        //                SqlStr = "UPDATE DepositHistory SET IsProcessing = 1 WHERE TrnID = '" + dRow["TrnID"].ToString() + "'";
        //                (new DataCommon()).ExecuteQuery(SqlStr);
        //            }
        //            IsProcessing = false;
        //            foreach (DataRow dRow in dSet.Tables[0].Rows)
        //            {
        //                String Response = CallThirdPartyCryptoAPI(ref CommonMethod,dRow["Address"].ToString(),Convert.ToInt64(dRow["TrnID"]), Convert.ToInt64(dRow["AutoNo"]));
        //                JObject GenerateResponse = JObject.Parse(Response);
        //                GenerateResponse["result"]["coin"] = CommonMethod.SMSCode;
        //                GenerateResponse["result"]["txid"] = dRow["TrnID"].ToString();
        //                GenerateResponse["result"]["value"] = Convert.ToDecimal(dRow["Value"]);
        //                GenerateResponse["result"]["Amount"] = dRow["Amount"].ToString();
        //                GenerateResponse["result"]["OrderID"] = dRow["OrderID"].ToString();
        //                GenerateResponse["result"]["address"] = dRow["Address"].ToString();
        //                GenerateResponse["result"]["confirmations"] = GenerateResponse["result"]["confirmations"];
        //                GenerateResponse["result"]["unconfirmedTime"] = GenerateResponse["result"]["blocktime"];
        //                GenerateResponse["result"]["confirmedTime"] = GenerateResponse["result"]["timereceived"];
        //                CommonMethod.Transfers.Add(JsonConvert.DeserializeObject<RespTransfers>(JsonConvert.SerializeObject(GenerateResponse.SelectToken("result"))));
        //                Console.WriteLine(CommonMethod.SMSCode + " Count : " + CommonMethod.Transfers.Count);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logs.WriteErrorLog(ex, "Program", "GetHistory");
        //    }
        //}

        //private static void TradeDepositHistoryUpdationForCryptoCoin(ref CommonMethods CommonMethod)
        //{
        //    DataSet dSet = new DataSet();
        //    string SqlStr = string.Empty;
        //    try
        //    {
        //        if (CommonMethod.Transfers.Count > 0)
        //        {
        //            foreach (var item in CommonMethod.Transfers)
        //            {                      
        //                // update
        //                CommonMethod.SqlStr = "UPDATE DepositHistory SET Confirmations =" + item.confirmations + ", confirmedTime ='" + item.confirmedTime + "', unconfirmedTime  ='" + item.unconfirmedTime + "', UpdatedDateTime = dbo.GetISTDate() WHERE TrnID = '" + item.txid + "'";
        //                (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
        //                logs.WriteRequestLog("Update Deposit History :  " + item.txid, "TradeDepositHistoryInsertion", CommonMethod.SMSCode);

        //                if ((new DataCommon()).ExecuteScalarWDM("Select Count(TrnID) From TradeDepositCompletedTrn Where TrnID='" + item.txid + "'") == "0")
        //                {
        //                    if (item.OrderId == 0)
        //                    {
        //                        // create Order
        //                        CreateOrder(ref CommonMethod, item);
        //                    }

        //                    // Delivery Process Order
        //                    if (item.confirmations > 3)
        //                    {
        //                        DeliveryProcessOrder(ref CommonMethod, item);
        //                        // trn pr , status ,date 
        //                        SqlStr = "INSERT INTO TradeDepositCompletedTrn(TrnID, Status, CreatedTime) VALUES('" + item.txid + "', 1 , dbo.GetISTDate())";
        //                        (new DataCommon()).ExecuteQuery(SqlStr);
        //                    }
        //                }
        //                else // Order Already Intialized
        //                {
        //                    SqlStr = "UPDATE DepositHistory SET status = 9 WHERE TrnID = '" + item.txid + "'";
        //                    (new DataCommon()).ExecuteQuery(SqlStr);
        //                    logs.WriteRequestLog("Order Already Intialized | " + item.txid, "TradeDepositHistoryUpdationForCryptoCoin", CommonMethod.SMSCode);
        //                }

        //                SqlStr = "UPDATE DepositHistory SET IsProcessing = 0 WHERE TrnID = '" + item.txid + "'";
        //                (new DataCommon()).ExecuteQuery(SqlStr);

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logs.WriteErrorLog(ex, "Program", "TradeDepositHistoryUpdationForCryptoCoin");
        //    }
        //}

        // For Crypto API
        private static void TradeDepositHistoryInsertion(ref CommonMethods CommonMethod, walletServiceData mainObj)
        {
            DataSet dSet = new DataSet();
            DataRow dRows = null;
            //string SqlStr = string.Empty;
            string txnID = "";
            int count = 0;
            long i = 0;
            bool flag = false;
            try
            {
               
                CommonMethod.Transfers.Reverse();
                logs.WriteRequestLog("Update Deposit History :  ", "TradeDepositHistoryInsertion:count=" + CommonMethod.Transfers.Count.ToString(), CommonMethod.SMSCode, Convert.ToInt16(ConfigurationManager.AppSettings["AllowLog"]));
                //if (mainObj.MaxLimit <= (mainObj.Limit + mainObj.RecordCount) && CommonMethod.Transfers.Count == 0 )
                //{
                //    Console.WriteLine("MaxLimit:{0},Limit:{1},Count:{2}", mainObj.MaxLimit, mainObj.Limit, mainObj.RecordCount.ToString());
                //    logs.WriteRequestLog("Deposit History MaxLimit Reach 0 record Maxlimit:" + mainObj.MaxLimit.ToString() + "mainObj.Limit" + mainObj.Limit.ToString() + ",i:" + i.ToString(), "TradeDepositHistoryInsertion", CommonMethod.SMSCode);
                //    CommonMethod.SqlStr = "update DepositCounterMaster set Limit=0 where AutoNo=" + mainObj.AutoNo;
                //    (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                //    flag = true;
                //    return;
                //}
                foreach (var item in CommonMethod.Transfers)
                {
                    i++;
                    //flag = false; ntrivedi if 29-05-2018 if previoustrnid found with 6 th position means next 4 records will not exist in our db so update limit = 0
                    if (i == 1)
                    {
                        txnID = item.txid;
                    }
                    logs.WriteRequestLog("DepositHistoryTrnID :" + item.txid, "i=" + i.ToString(), CommonMethod.SMSCode,Action:2);
                    if (item.txid == mainObj.PreviousTrnID || item.txid == mainObj.LastTrnID)
                    {
                        Console.WriteLine("TrnID Match PreviousTrnID:{0},Limit:{1},Count:{2}", item.txid, mainObj.Limit, mainObj.RecordCount.ToString());
                        logs.WriteRequestLog("DepositHistoryTrnID match:" + item.txid, "TradeDepositHistoryInsertion", CommonMethod.SMSCode, Action: 2);
                        CommonMethod.SqlStr = "update DepositCounterMaster set Limit=0,prevIterationId='' where ID=" + mainObj.AutoNo;
                        (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                        flag = true;
                        LastLimit = mainObj.Limit;
                        break; //ntrivedi 29-05-2018 down word  transaction may be new 
                    }
                    //if (mainObj.MaxLimit <= (mainObj.Limit + i))
                    //{
                    //    Console.WriteLine("MaxLimit:{0},Limit:{1},Count:{2}", mainObj.MaxLimit, mainObj.Limit, mainObj.RecordCount.ToString());
                    //    logs.WriteRequestLog("DepositHistory MaxLimit Reach Maxlimit:" + mainObj.MaxLimit.ToString() + "mainObj.Limit" + mainObj.Limit.ToString() + ",i:" + i.ToString(), "TradeDepositHistoryInsertion", CommonMethod.SMSCode);
                    //    CommonMethod.SqlStr = "update DepositCounterMaster set Limit=0 where AutoNo=" + mainObj.AutoNo;
                    //    (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                    //    flag = true;
                    //    break;
                    //}

                    //ntrivedi check whether address is exist in our database 
                    // ntrivedi temperory
                    CommonMethod.SqlStr = "select count(*) from AddressMasters where Address=@AccountNo and Status=1 and SerProID=" + mainObj.serProID; // ntrivedi provider matching
                    count = Convert.ToInt32((new DataCommon()).ExecuteScalarWDMParameterize("@AccountNo", item.address, CommonMethod.SqlStr));
                    if (count == 0)
                    {
                        logs.WriteRequestLog("Update Deposit History Address not found in system:" + txnID, "Deposit HistoryInsertion", CommonMethod.SMSCode, Action: 2);
                        continue; // address not found in our system skip this record ntrivedi temperory 29-05-2018
                        // ntrivedi temperory
                        //CommonMethod.SqlStr = "INSERT INTO [dbo].[AddressMasters]([CreatedDate],[CreatedBy],[UpdatedBy],[UpdatedDate] " +
                        //         ",[Status],[WalletId],[Address],[IsDefaultAddress],[SerProID],[AddressLable]) " +
                        //         " VALUES(dbo.GetISTDate(),900,900, dbo.GetISTDate(),1,40, '" + item.address + "',0,0,'test')";
                        //(new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                    }

                    if ((new DataCommon()).ExecuteScalarWDM("Select Count(TrnID) From TradeDepositCompletedTrn Where TrnID='" + item.txid + "'") != "0")
                    {
                        logs.WriteRequestLog("Alrady treated transaction:" + txnID, "Deposit HistoryInsertion", CommonMethod.SMSCode, Action: 2);
                        continue;
                    }

                    CommonMethod.SqlStr = "SELECT count(*) as Count From [DepositHistory] WHERE TrnID = @TrnID and Address=@Address";
                    //dSet = (new DataCommon()).OpenDataSet("DepositHistoryv1", CommonMethod.SqlStr, dSet, 30);
                    count = Convert.ToInt32((new DataCommon()).ExecuteScalarWDMParameterize("@TrnID,@Address", item.txid + "," + item.address, CommonMethod.SqlStr));
                    if (count > 0)
                    {  
                        // update
                        CommonMethod.SqlStr = "UPDATE [DepositHistory] SET FromAddress='"+ item.fromaddress +"', Amount =" + item.Amount + ", Confirmations =" + item.confirmations + ", confirmedTime ='" + item.confirmedTime + "', UpdatedDate = dbo.GetISTDate()" +
                        " WHERE TrnID = @TrnID and Address=@Address  and Status<>1"; // tranx is success then no need to update it ntrivedi added status<>1 condition 07-09-2018
                        (new DataCommon()).ExecuteQueryParameterize("@TrnID,@Address", item.txid + "," + item.address, CommonMethod.SqlStr);
                        //logs.WriteRequestLog("Update Deposit History :  " + item.txid, "TradeDepositHistoryInsertion", CommonMethod.SMSCode); unnecessary logs 07-09-2018 ntrivedi
                    }
                    else
                    {
                        string status = "0", state = "Initialized";
                        if (item.confirmations < 0) //komal 10/9/2018 add check for negative confirmation
                        {
                            status = "9";
                            state = "negative confirmation";
                        }
                        String MStatus = "";
                        MStatus = "Limit=" + mainObj.Limit+",Count="+mainObj.RecordCount+ ",ValueStr" + item.valueStr; //komal 8-9-2018 add mstatus
                        // insert
                        CommonMethod.SqlStr = "INSERT INTO [DepositHistory](TrnID,SMSCode,Address,Status,Confirmations,confirmedTime,createdDate,StatusMsg,UpdatedDate,OrderID,Amount,FromAddress,SystemRemarks,CreatedBy,TimeEpoch,IsProcessing,SerProID,UserID) VALUES('" + item.txid + "','" + item.coin.ToUpper() + "','" + item.address + "'," + status + "," + item.confirmations + ",'" + item.confirmedTime + "','" + item.createdTime + "','"+ state +"', dbo.GetISTDate(),0," + item.Amount + ",'" + item.fromaddress + "','"+ MStatus +"',1,'',0,"+ mainObj.serProID +"," + item.UserID + ")";
                        (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                        //logs.WriteRequestLog("Insert Deposit History :  " + item.txid, "TradeDepositHistoryInsertion", CommonMethod.SMSCode);unnecessary logs 07-09-2018 ntrivedi

                    }
                }
                if (mainObj.MaxLimit <= (mainObj.Limit + mainObj.RecordCount))
                {
                    Console.WriteLine("MaxLimit:{0},Limit:{1},Count:{2}", mainObj.MaxLimit, mainObj.Limit, mainObj.RecordCount.ToString());
                    logs.WriteRequestLog("Deposit History MaxLimit Reach 0 record Maxlimit:" + mainObj.MaxLimit.ToString() + "mainObj.Limit" + mainObj.Limit.ToString() + ",i:" + i.ToString(), "TradeDepositHistoryInsertion", CommonMethod.SMSCode, Action: 2);
                    CommonMethod.SqlStr = "update DepositCounterMaster set Limit=0,prevIterationId='' where ID=" + mainObj.AutoNo;
                    (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                    flag = true;
                    LastLimit = mainObj.Limit;
                    return;
                }
                if (mainObj.Limit == 0) // if this is a first fetch that means i need to update LastTrnID

                {
                    logs.WriteRequestLog("Update Deposit History Count=0 fetch first page fetch:  PreviousTrnID:" + mainObj.PreviousTrnID + ",LastTrnID=" + mainObj.LastTrnID, "TradeDepositHistoryInsertion", CommonMethod.SMSCode, Action: 2);
                    logs.WriteRequestLog("Update Deposit History Count=0 Last txnID  in current array is txnID:" + txnID, "Deposit HistoryInsertion", CommonMethod.SMSCode, Action: 2);
                    if (!string.IsNullOrEmpty(txnID))
                    {
                        Console.WriteLine("Limit=0:{0},Limit:{1},Count:{2}", mainObj.PreviousTrnID, mainObj.Limit, mainObj.RecordCount.ToString());
                        CommonMethod.SqlStr = "update DepositCounterMaster set PreviousTrnID=LastTrnID,UpdatedDate=dbo.getistdate() where ID=" + mainObj.AutoNo;
                        CommonMethod.SqlStr += ";update DepositCounterMaster set LastTrnID = @LastTrnID where ID=" + mainObj.AutoNo;
                        (new DataCommon()).ExecuteQueryParameterize("@LastTrnID", txnID, CommonMethod.SqlStr);
                        //komal 11-9-2018 add for log 
                        //CommonMethod.SqlStr = "Insert into DepositCounterLog  ([NewTxnID],[PreviousTrnID],[LastTrnID],[LastLimit],[CreatedDate]) values('" + txnID + "','" + mainObj.PreviousTrnID + "','" + mainObj.LastTrnID + "'," + LastLimit.ToString() + ",dbo.GetISTDate())";
                        //(new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                    }
                    if (flag == false) // lasttrnid do not match so update limit to resultcount
                    {
                        CommonMethod.SqlStr = "update DepositCounterMaster set prevIterationId='" + prevID + "',Limit=Limit+ " + CommonMethod.resultCount + " where ID=" + mainObj.AutoNo;
                        (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                        ////komal 11-9-2018 add for log ntrivedi comment due to call every 2 second and duplicate record insert only when limit=0
                        //CommonMethod.SqlStr = "Insert into DepositCounterLog  ([NewTxnID],[PreviousTrnID],[LastTrnID],[LastLimit],[CreatedDate]) values('" + txnID + "','" + mainObj.PreviousTrnID + "','" + mainObj.LastTrnID + "'," + mainObj.Limit + ",dbo.GetISTDate())";
                        //(new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                    }                   
                }
                else if (flag == false)
                {
                    CommonMethod.SqlStr = "update DepositCounterMaster set prevIterationId='"+ prevID + "',Limit=Limit+ " + CommonMethod.resultCount + " where ID=" + mainObj.AutoNo;
                    (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                   
                    ////komal 11-9-2018 add for log ntrivedi comment due to call every 2 second and duplicate record insert only when limit=0
                    //CommonMethod.SqlStr = "Insert into DepositCounterLog  ([NewTxnID],[PreviousTrnID],[LastTrnID],[LastLimit],[CreatedDate]) values('" + txnID + "','" + mainObj.PreviousTrnID + "','" + mainObj.LastTrnID + "'," + mainObj.Limit + ",dbo.GetISTDate())";
                    //(new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                }

                //if(txnID != mainObj.PreviousTrnID) //ntrivedi  due to call every 2 second and duplicate record insert only when limit=0 so added inside if condition 13-09-2018
                //{
                    //komal 11-9-2018 add for log 
                    CommonMethod.SqlStr = "Insert into DepositCounterLog  ([NextBatchPrvID],[NewTxnID],[PreviousTrnID],[LastTrnID],[LastLimit],[CreatedDate],[DepositCounterMasterId],[CreatedBy],[UpdatedDate],[Status]) values('"+ prevID +"','" + txnID + "','" + mainObj.PreviousTrnID + "','" + mainObj.LastTrnID + "'," + mainObj.Limit + ",dbo.GetISTDate()," + mainObj.AutoNo + ",1,dbo.GetISTDate(),1)";
                    (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                //}

            }
            catch (Exception ex)
            {
                logs.WriteErrorLog(ex, "Program", "TradeDepositHistoryInsertion : " + CommonMethod.SqlStr);
            }
        }

        //private static void CreateOrder(ref CommonMethods CommonMethod, RespTransfers item)
        //{
        //    long MemberID;
        //    long WalletID;
        //    int RetCode = 0;
        //    string RetMsg = string.Empty;
        //    long OrderID = 0;
        //    int BankID = 0;
        //    string BranchName = string.Empty;
        //    DataSet dSet = new DataSet();
        //    DataRow dRows = null;
        //    string SqlStr = string.Empty;
        //    try
        //    {
        //        SqlStr = "SELECT m.MemberID, w.WalletID ,m.BranchName FROM MemberBankMaster AS m INNER JOIN Wallet As w ON m.MemberID = w.MemberId AND m.BankID=w.Servicetypeid WHERE m.Type=1 AND AccountNo = '" + item.address + "' AND m.BranchName='" + item.coin.ToUpper() + "'";
        //        dSet = (new DataCommon()).OpenDataSet("MemberBankMaster", SqlStr, dSet, 30);
        //        if (dSet.Tables[0].Rows.Count > 0)
        //        {
        //            dRows = dSet.Tables[0].Rows[0];
        //            MemberID = Convert.ToInt64(dRows["MemberID"]);
        //            WalletID = Convert.ToInt64(dRows["WalletID"]);
        //            //BankID = Convert.ToInt16(dRows["BankID"]);
        //            BranchName = Convert.ToString(dRows["BranchName"]);
        //            if (MemberID != 0 && WalletID != 0 && !string.IsNullOrEmpty(BranchName))
        //            {
        //                SqlParameter[] Params = new SqlParameter[] {
        //                    new SqlParameter("@OrderID", SqlDbType.BigInt, 10, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, 0),
        //                    new SqlParameter("@OrderDate", SqlDbType.DateTime, 4, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, logs.UTC_To_IST()),
        //                    new SqlParameter("@TrnMode", SqlDbType.TinyInt, 4, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, 11),
        //                    new SqlParameter("@OMemberID", SqlDbType.BigInt, 10, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, MemberID),
        //                    new SqlParameter("@PayMode", SqlDbType.TinyInt, 4, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, 3), // 3 for Transfer
        //                    //new SqlParameter("@OrdAmount", SqlDbType.Money, 10, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, item.Amount),
        //                    new SqlParameter("@OrdAmount", SqlDbType.Decimal, 18, ParameterDirection.Input, true, 18, 8, String.Empty, DataRowVersion.Default, item.Amount),
        //                    new SqlParameter("@DiscPer", SqlDbType.Float, 6, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, 0),
        //                    new SqlParameter("@DiscRs", SqlDbType.Money, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, 0),
        //                    new SqlParameter("@OBankID", SqlDbType.SmallInt, 8, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, 0),
        //                    new SqlParameter("@OBranchName", SqlDbType.VarChar, 50, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, BranchName),
        //                    new SqlParameter("@OAccountNo", SqlDbType.VarChar, 250, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, item.address),
        //                    new SqlParameter("@OChequeDate", SqlDbType.DateTime, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, DBNull.Value),
        //                    new SqlParameter("@OChequeNo", SqlDbType.VarChar, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, ""),
        //                    new SqlParameter("@DMemberID", SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, 1),
        //                    new SqlParameter("@DBankID", SqlDbType.SmallInt, 8, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, 0),
        //                    new SqlParameter("@DAccountNo", SqlDbType.VarChar, 25, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, ""),
        //                    new SqlParameter("@Status", SqlDbType.TinyInt, 4, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, 0),
        //                    new SqlParameter("@OrderType", SqlDbType.TinyInt, 4, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, 1),
        //                    new SqlParameter("@ORemarks", SqlDbType.VarChar, 250, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, "System Transaction Order"),
        //                    //new SqlParameter("@DeliveryAmt", SqlDbType.Money, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, item.value),
        //                    new SqlParameter("@DeliveryAmt", SqlDbType.Decimal, 18, ParameterDirection.Input, true, 18, 8, String.Empty, DataRowVersion.Default, item.Amount),
        //                    new SqlParameter("@DRemarks", SqlDbType.VarChar, 250, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,""),
        //                    new SqlParameter("@DeliveryGivenBy", SqlDbType.BigInt, 10, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, 1),
        //                    new SqlParameter("@DeliveryGivenDate", SqlDbType.DateTime, 4, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, DBNull.Value),
        //                    new SqlParameter("@AlertRec", SqlDbType.TinyInt, 4, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, 0),
        //                    new SqlParameter("@CashChargePer", SqlDbType.Float, 6, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, 0),
        //                    new SqlParameter("@CashChargeRs", SqlDbType.Money, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default,0),
        //                    new SqlParameter("@WalletAmt", SqlDbType.Money, 10, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, 0),
        //                    new SqlParameter("@ReturnCode", SqlDbType.Int, 8, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Default, RetCode),
        //                    new SqlParameter("@ReturnMsg", SqlDbType.VarChar, 500, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Default, RetMsg),
        //                    new SqlParameter("@OWalletID", SqlDbType.BigInt, 10, ParameterDirection.Input, true, 0, 0, String.Empty, DataRowVersion.Default, WalletID)
        //               };

        //                (new DataCommon()).ExecuteSP("sp_CreateOrder", ref Params);
        //                RetCode = Convert.ToInt16(Params[27].Value);
        //                RetMsg = Convert.ToString(Params[28].Value);
        //                if (!(Params[0].Value is DBNull))
        //                {
        //                    OrderID = Convert.ToInt64(Params[0].Value);
        //                }
        //                if (RetCode == 0 && OrderID > 0)
        //                {
        //                    CommonMethod.SqlStr = "Update DepositHistory set OrderID =" + OrderID + ", UpdatedDateTime = dbo.GetISTDate() where TrnID = '" + item.txid + "'";
        //                    (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
        //                }
        //                logs.WriteRequestLog("sp_CreateOrder :  " + RetMsg + " | " + item.txid, "CreateOrder", CommonMethod.SMSCode);
        //            }
        //            else
        //            {
        //                logs.WriteRequestLog("CreateOrder : Member not found or Wallet not found | " + item.txid, "CreateOrder", CommonMethod.SMSCode);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logs.WriteErrorLog(ex, "Program", "CreateOrder");
        //    }
        //}

        private static void DeliveryProcessOrder(ref CommonMethods CommonMethod, RespTransfers item)
        {
            int ReturnCode = 0;
            string ReturnMsg = string.Empty;
            long MemberID;
            long OrderID = 0;
            int Status = 0;
            DataSet dSet = new DataSet();
            DataRow dRows = null;
            string SqlStr = string.Empty;
            try
            {
                SqlStr = "SELECT TOP 1 m.MemberID , d.OrderID , d.Status FROM ExchangeCoinAddress AS m INNER JOIN DepositHistory AS d ON m.PublicAddress = d.Address AND m.WalletName=d.SMScode WHERE m.Type=1 and d.TrnID = '" + item.txid + "'";//on txnid
                dSet = (new DataCommon()).OpenDataSet("ExchangeCoinAddress", SqlStr, dSet, 30);
                if (dSet.Tables[0].Rows.Count > 0)
                {
                    dRows = dSet.Tables[0].Rows[0];
                    MemberID = Convert.ToInt64(dRows["MemberID"]);
                    OrderID = Convert.ToInt64(dRows["OrderID"]);
                    Status = Convert.ToInt16(dRows["Status"]);
                    if (OrderID > 0 && Status == 0 && MemberID != 0)
                    {
                        SqlParameter[] Params = new SqlParameter[] {
                        new SqlParameter("@OrderID", SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, OrderID),
                        new SqlParameter("@MemberID", SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, MemberID),
                        new SqlParameter("@Action", SqlDbType.TinyInt, 4, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, 1), // accept
                        new SqlParameter("@DeliveryAmt", SqlDbType.Decimal, 18, ParameterDirection.Input, false, 18, 8, String.Empty, DataRowVersion.Default,item.Amount), // value converted
                        new SqlParameter("@Remarks", SqlDbType.VarChar, 250, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, "System Delivery Process Order"),
                        new SqlParameter("@GivenBy", SqlDbType.BigInt, 10, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, 1), // by Org
                        new SqlParameter("@ReturnCode", SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, 0),
                        new SqlParameter("@ReturnMsg", SqlDbType.VarChar, 500, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, "")
                        };

                        (new DataCommon()).ExecuteSP("sp_DeliveryProcess", ref Params);

                        ReturnCode = Convert.ToInt32(Params[6].Value);
                        ReturnMsg = Convert.ToString(Params[7].Value);

                        if (ReturnCode == 0) //Success
                        {
                            CommonMethod.SqlStr = "UPDATE DepositHistory SET Status =" + 1 + ", UpdatedDateTime = dbo.GetISTDate() where TrnID = '" + item.txid + "'";
                            (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                        }
                        else
                        {
                            CommonMethod.SqlStr = "UPDATE DepositHistory SET Status =" + 9 + ", UpdatedDateTime = dbo.GetISTDate() where TrnID = '" + item.txid + "'";
                            (new DataCommon()).ExecuteQuery(CommonMethod.SqlStr);
                        }

                        logs.WriteRequestLog("sp_DeliveryProcess :  " + ReturnMsg + " | " + item.txid, "DeliveryProcessOrder", CommonMethod.SMSCode, Convert.ToInt16(ConfigurationManager.AppSettings["AllowLog"]), Action: 2);
                        //Console.WriteLine(CommonMethod.Transfers.Count + "  " + ReturnMsg);

                    }
                    else
                    {
                        logs.WriteRequestLog("DeliveryProcessOrder :  Member not found or Order not found " + item.txid, "DeliveryProcessOrder", CommonMethod.SMSCode, Convert.ToInt16(ConfigurationManager.AppSettings["AllowLog"]),Action:2);
                    }
                }
            }
            catch (Exception ex)
            {
                logs.WriteErrorLog(ex, "Program", "DeliveryProcessOrder");
            }
        }

        #endregion

        #region "CallThirdPartyAPI"

        private static void CallThirdPartyAPI(ref CommonMethods CommonMethod,string iterationPrevID , string Authorization,string enterprise,string walletid)
        {
            try
            {
                string sqlStr = "";
                long WalletID = 0;
                long userID = 0;
                //CommonMethods CommonMethod = (CommonMethods)CommonMethodObj;
                Console.WriteLine("API Call " + CommonMethod.SMSCode);
                CommonMethod.Str_URL = CommonMethod.Str_URL.Replace("#prevId#", iterationPrevID.ToString());
                CommonMethod.Str_URL = CommonMethod.Str_URL.Replace("#ProviderWalletID#", walletid.ToString());


                if (!string.IsNullOrEmpty(CommonMethod.Str_URL) && !string.IsNullOrEmpty(CommonMethod.Str_RequestType) && !string.IsNullOrEmpty(CommonMethod.ContentType))
                {
                    logs.WriteRequestLog("Request :  " + CommonMethod.Str_URL, "CallThirdPartyAPI", CommonMethod.SMSCode);
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(CommonMethod.Str_URL);
                    httpWebRequest.ContentType = CommonMethod.ContentType;
                    httpWebRequest.Method = CommonMethod.Str_RequestType;
                    httpWebRequest.KeepAlive = false;
                    httpWebRequest.Timeout = 180000;
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
                    logs.WriteRequestLog("Response :  " + CommonMethod.DepositHistoryResponse, "CallThirdPartyAPI", CommonMethod.SMSCode);
                    if (string.IsNullOrEmpty(CommonMethod.DepositHistoryResponse))
                    {
                        return ;
                    }
                    //CommonMethod.DepositHistoryResponse = "{\"coin\":\"eth\",\"transfers\":[{\"normalizedTxHash\":\"0x5a2a7d11585e45b65158982f7bdd4f4f04f3deb265f42b1666e8e4145b46f803\",\"id\":\"5b94df8c048f8dec06dba1b27bbf74f5\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x5a2a7d11585e45b65158982f7bdd4f4f04f3deb265f42b1666e8e4145b46f803\",\"height\":999999999,\"date\":\"2018-09-09T08:53:32.887Z\",\"confirmations\":0,\"type\":\"send\",\"valueString\":\"-302227200000000\",\"feeString\":\"0\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-0.057701217,\"usdRate\":190.92,\"state\":\"signed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-09T08:53:32.887Z\",\"action\":\"signed\"},{\"date\":\"2018-09-09T08:53:32.574Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-302227200000000,\"valueString\":\"-302227200000000\"},{\"address\":\"0x6d26a621a1c3b23e5a145307b39400412294b3ba\",\"value\":302227200000000,\"valueString\":\"302227200000000\"}],\"signedTime\":\"2018-09-09T08:53:32.887Z\",\"createdTime\":\"2018-09-09T08:53:32.574Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0xac019134415759747e3263d4f2380f21dff75ab7eb2972a0d12d3a0cbc58dffc\",\"id\":\"5b93b84ddc6761a61ff4954afdcc41d9\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0xac019134415759747e3263d4f2380f21dff75ab7eb2972a0d12d3a0cbc58dffc\",\"height\":999999999,\"date\":\"2018-09-08T11:53:50.101Z\",\"confirmations\":0,\"type\":\"send\",\"valueString\":\"-29700000000000000\",\"feeString\":\"0\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-6.463017,\"usdRate\":217.61,\"state\":\"signed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-08T11:53:50.100Z\",\"action\":\"signed\"},{\"date\":\"2018-09-08T11:53:49.857Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-29700000000000000,\"valueString\":\"-29700000000000000\"},{\"address\":\"0x57e69fb3ed3c10dee6a2f4b2c83a760102011416\",\"value\":29700000000000000,\"valueString\":\"29700000000000000\"}],\"signedTime\":\"2018-09-08T11:53:50.100Z\",\"createdTime\":\"2018-09-08T11:53:49.857Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0x47b4ecc03e72f8a3b6247008a87050acbfb97a74f37a963b34a65adb3b02b672\",\"id\":\"5b939751884491e606d49b86e1f8cc78\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x47b4ecc03e72f8a3b6247008a87050acbfb97a74f37a963b34a65adb3b02b672\",\"height\":999999999,\"date\":\"2018-09-08T09:33:05.836Z\",\"confirmations\":0,\"type\":\"send\",\"valueString\":\"-6504300000000000\",\"feeString\":\"0\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-1.419758604,\"usdRate\":218.28,\"state\":\"signed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-08T09:33:05.835Z\",\"action\":\"signed\"},{\"date\":\"2018-09-08T09:33:05.569Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-6504300000000000,\"valueString\":\"-6504300000000000\"},{\"address\":\"0x2A437124387CF900e28E689389de5b463827e0E4\",\"value\":6504300000000000,\"valueString\":\"6504300000000000\"}],\"signedTime\":\"2018-09-08T09:33:05.835Z\",\"createdTime\":\"2018-09-08T09:33:05.569Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0x3c1e3b6798b144f53282b795ac77af3882ee0cbfc9595f295a3c147a832c556f\",\"id\":\"5b937dccbfe7d9ec0628628d175a7eab\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x3c1e3b6798b144f53282b795ac77af3882ee0cbfc9595f295a3c147a832c556f\",\"height\":999999999,\"date\":\"2018-09-08T07:44:12.839Z\",\"confirmations\":0,\"type\":\"send\",\"valueString\":\"-5481531000000000\",\"feeString\":\"0\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-1.1892181505,\"usdRate\":216.95,\"state\":\"signed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-08T07:44:12.838Z\",\"action\":\"signed\"},{\"date\":\"2018-09-08T07:44:12.542Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-5481531000000000,\"valueString\":\"-5481531000000000\"},{\"address\":\"0x39665e5a573572080cc6ea063a95a1b15c7a72e4\",\"value\":5481531000000000,\"valueString\":\"5481531000000000\"}],\"signedTime\":\"2018-09-08T07:44:12.838Z\",\"createdTime\":\"2018-09-08T07:44:12.542Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0xa38946ebe18f8471f1f2d2e0324792ae811186ebd3f29b14bf78829f9381cd0e\",\"id\":\"5b7ff5d00f419d19071826c42e8a89b4\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0xa38946ebe18f8471f1f2d2e0324792ae811186ebd3f29b14bf78829f9381cd0e\",\"height\":999999999,\"date\":\"2018-08-24T12:10:56.377Z\",\"confirmations\":0,\"type\":\"send\",\"valueString\":\"-99000000000000\",\"feeString\":\"0\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-0.02729727,\"usdRate\":275.73,\"state\":\"failed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-05T21:08:06.379Z\",\"action\":\"failed\",\"comment\":\"ethExecutionFailure\"},{\"date\":\"2018-08-24T12:10:56.376Z\",\"action\":\"signed\"},{\"date\":\"2018-08-24T12:10:56.173Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-99000000000000,\"valueString\":\"-99000000000000\"},{\"address\":\"0x5b2ba0f5345977f95a0bcd6d0113e48e232f0c6e\",\"value\":99000000000000,\"valueString\":\"99000000000000\"}],\"failedTime\":\"2018-09-05T21:08:06.379Z\",\"signedTime\":\"2018-08-24T12:10:56.376Z\",\"createdTime\":\"2018-08-24T12:10:56.173Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0x941dfb5a1bee31a442ac8b24269281c5a4dc137a792ba7e59819841e465a4c4e\",\"id\":\"5b778435584c03bd07bb968c276cf5e5\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x941dfb5a1bee31a442ac8b24269281c5a4dc137a792ba7e59819841e465a4c4e\",\"height\":999999999,\"date\":\"2018-08-18T02:28:05.751Z\",\"confirmations\":0,\"type\":\"send\",\"valueString\":\"-29700000000000000\",\"feeString\":\"0\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-9.365004,\"usdRate\":315.32,\"state\":\"failed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-05T06:45:50.162Z\",\"action\":\"failed\",\"comment\":\"ethExecutionFailure\"},{\"date\":\"2018-08-18T02:28:05.751Z\",\"action\":\"signed\"},{\"date\":\"2018-08-18T02:28:05.445Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-29700000000000000,\"valueString\":\"-29700000000000000\"},{\"address\":\"0xd5297c8e12dc55c8c06959a70be0447284522a41\",\"value\":29700000000000000,\"valueString\":\"29700000000000000\"}],\"failedTime\":\"2018-09-05T06:45:50.162Z\",\"signedTime\":\"2018-08-18T02:28:05.751Z\",\"createdTime\":\"2018-08-18T02:28:05.445Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0x5343ae7870c9d1b04351b19bac62abe7623f161ee8e1005871e4e20a290e1a39\",\"id\":\"5b744557a1f4d47a07ffc21505301ac7\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x5343ae7870c9d1b04351b19bac62abe7623f161ee8e1005871e4e20a290e1a39\",\"height\":999999999,\"date\":\"2018-08-15T15:23:03.634Z\",\"confirmations\":0,\"type\":\"send\",\"valueString\":\"-43560000000000000\",\"feeString\":\"0\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-12.804462,\"usdRate\":293.95,\"state\":\"failed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-05T06:45:49.506Z\",\"action\":\"failed\",\"comment\":\"ethExecutionFailure\"},{\"date\":\"2018-08-15T15:23:03.634Z\",\"action\":\"signed\"},{\"date\":\"2018-08-15T15:23:03.377Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-43560000000000000,\"valueString\":\"-43560000000000000\"},{\"address\":\"0xD6506CDE9Be2657052e4a90130Cd3a2b2f4D8bcb\",\"value\":43560000000000000,\"valueString\":\"43560000000000000\"}],\"failedTime\":\"2018-09-05T06:45:49.506Z\",\"signedTime\":\"2018-08-15T15:23:03.634Z\",\"createdTime\":\"2018-08-15T15:23:03.377Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0xd185cc0cd11be180abe23f06cee05f29df89bf6d942600c79f01e3e8e60e3e2d\",\"id\":\"5b742cd2054128f4065af24caada34b2\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0xd185cc0cd11be180abe23f06cee05f29df89bf6d942600c79f01e3e8e60e3e2d\",\"height\":999999999,\"date\":\"2018-08-15T13:38:27.178Z\",\"confirmations\":0,\"type\":\"send\",\"valueString\":\"-123263217000000000\",\"feeString\":\"0\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-35.3272379922,\"usdRate\":286.6,\"state\":\"failed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-05T06:45:49.500Z\",\"action\":\"failed\",\"comment\":\"ethExecutionFailure\"},{\"date\":\"2018-08-15T13:38:27.177Z\",\"action\":\"signed\"},{\"date\":\"2018-08-15T13:38:26.925Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-123263217000000000,\"valueString\":\"-123263217000000000\"},{\"address\":\"0x8331c648998ffbe0e814730298b0c863317d2b2c\",\"value\":123263217000000000,\"valueString\":\"123263217000000000\"}],\"failedTime\":\"2018-09-05T06:45:49.500Z\",\"signedTime\":\"2018-08-15T13:38:27.177Z\",\"createdTime\":\"2018-08-15T13:38:26.925Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0x64329089fa0aea7d6270bd382ee41d8b77b4e2ac075cf4fa1e377fe59b0bb0c0\",\"id\":\"5b712bfa776c64190771f26e219d0a27\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x64329089fa0aea7d6270bd382ee41d8b77b4e2ac075cf4fa1e377fe59b0bb0c0\",\"height\":999999999,\"date\":\"2018-08-13T06:58:02.295Z\",\"confirmations\":0,\"type\":\"send\",\"valueString\":\"-57036672000000000\",\"feeString\":\"0\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-18.0429808205,\"usdRate\":316.34,\"state\":\"failed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-05T06:42:56.301Z\",\"action\":\"failed\",\"comment\":\"ethExecutionFailure\"},{\"date\":\"2018-08-13T06:58:02.295Z\",\"action\":\"signed\"},{\"date\":\"2018-08-13T06:58:02.053Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-57036672000000000,\"valueString\":\"-57036672000000000\"},{\"address\":\"0x0a0431F8593D9da68Bd16D93B5440D886181C3f5\",\"value\":57036672000000000,\"valueString\":\"57036672000000000\"}],\"failedTime\":\"2018-09-05T06:42:56.301Z\",\"signedTime\":\"2018-08-13T06:58:02.295Z\",\"createdTime\":\"2018-08-13T06:58:02.053Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0xa68c18b2d28c1b0f35ec569ae5972fb797c86bec43edcad804d8131583d4d3d6\",\"id\":\"5b7001aa7b776d945972da6735de2ed8\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0xa68c18b2d28c1b0f35ec569ae5972fb797c86bec43edcad804d8131583d4d3d6\",\"height\":999999999,\"date\":\"2018-08-12T09:45:14.669Z\",\"confirmations\":0,\"type\":\"send\",\"valueString\":\"-52702947000000000\",\"feeString\":\"0\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-17.0193626747,\"usdRate\":322.93,\"state\":\"failed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-05T06:42:55.288Z\",\"action\":\"failed\",\"comment\":\"ethExecutionFailure\"},{\"date\":\"2018-08-12T09:45:14.668Z\",\"action\":\"signed\"},{\"date\":\"2018-08-12T09:45:14.428Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-52702947000000000,\"valueString\":\"-52702947000000000\"},{\"address\":\"0xd80412a7f1a8c0920beef831970768eb28b19d3f\",\"value\":52702947000000000,\"valueString\":\"52702947000000000\"}],\"failedTime\":\"2018-09-05T06:42:55.288Z\",\"signedTime\":\"2018-08-12T09:45:14.668Z\",\"createdTime\":\"2018-08-12T09:45:14.428Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0x42c53bcc0c2837a8bc23791ecafbc12112e8764f9612345e503117980d8cee71\",\"id\":\"5b6ec30bc6fb6832072c4088f0952d04\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x42c53bcc0c2837a8bc23791ecafbc12112e8764f9612345e503117980d8cee71\",\"height\":999999999,\"date\":\"2018-08-11T11:05:47.855Z\",\"confirmations\":0,\"type\":\"send\",\"valueString\":\"-17386380000000000\",\"feeString\":\"0\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-5.5121779152,\"usdRate\":317.04,\"state\":\"failed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-05T06:42:55.284Z\",\"action\":\"failed\",\"comment\":\"ethExecutionFailure\"},{\"date\":\"2018-08-11T11:05:47.854Z\",\"action\":\"signed\"},{\"date\":\"2018-08-11T11:05:47.612Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-17386380000000000,\"valueString\":\"-17386380000000000\"},{\"address\":\"0xaf80889c8fd7b1f1482076657462b665968ca077\",\"value\":17386380000000000,\"valueString\":\"17386380000000000\"}],\"failedTime\":\"2018-09-05T06:42:55.284Z\",\"signedTime\":\"2018-08-11T11:05:47.854Z\",\"createdTime\":\"2018-08-11T11:05:47.612Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0xbf9ec20b2643088fbdf297cb12ff84616f89f84c27d6c843daee3454d3cb245b\",\"id\":\"5b8f80146145d52e074064f491c31a41\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0xbf9ec20b2643088fbdf297cb12ff84616f89f84c27d6c843daee3454d3cb245b\",\"height\":6292896,\"date\":\"2018-09-08T07:26:23.725Z\",\"confirmations\":13461,\"type\":\"send\",\"valueString\":\"-154440000000000000\",\"feeString\":\"405246000000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-43.9335468,\"usdRate\":284.47,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-08T07:26:23.725Z\",\"action\":\"confirmed\"},{\"date\":\"2018-09-05T21:08:09.569Z\",\"action\":\"removed\",\"comment\":\"error\"},{\"date\":\"2018-09-05T07:04:52.527Z\",\"action\":\"signed\"},{\"date\":\"2018-09-05T07:04:52.276Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0x3cb733437abc30d2e41f860e3ee95fd94fb1a8fe\",\"value\":154440000000000000,\"valueString\":\"154440000000000000\",\"isPayGo\":false},{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-154440000000000000,\"valueString\":\"-154440000000000000\",\"isPayGo\":false}],\"confirmedTime\":\"2018-09-08T07:26:23.725Z\",\"removedTime\":\"2018-09-05T21:08:09.569Z\",\"signedTime\":\"2018-09-05T07:04:52.527Z\",\"createdTime\":\"2018-09-05T07:04:52.276Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0x788711cea8c3676f98ef3b06f270fadd7b1c08a7a3e6a82cdf5c8052eaeedea5\",\"id\":\"5b9275abca90a881075bdadc093d0792\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x788711cea8c3676f98ef3b06f270fadd7b1c08a7a3e6a82cdf5c8052eaeedea5\",\"height\":6288366,\"date\":\"2018-09-07T12:57:00.000Z\",\"confirmations\":17991,\"type\":\"receive\",\"value\":38000000000000000,\"valueString\":\"38000000000000000\",\"feeString\":\"674100000000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":8.40142,\"usdRate\":221.09,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-07T12:57:00.000Z\",\"action\":\"confirmed\"},{\"date\":\"2018-09-07T12:57:00.000Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xbd6d79f3f02584cfcb754437ac6776c4c6e0a0ec\",\"value\":-38000000000000000,\"valueString\":\"-38000000000000000\",\"isPayGo\":false},{\"address\":\"0x382c8612ab2a9bfc7cae6323567e2028b8aab966\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":38000000000000000,\"valueString\":\"38000000000000000\",\"isPayGo\":false}],\"confirmedTime\":\"2018-09-07T12:57:00.000Z\",\"createdTime\":\"2018-09-07T12:57:00.000Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0x7928ee75143c3ec562281c116499a7aad05f7ca3b7f8cf1be6592a83eb6c6e0e\",\"id\":\"5b91ef0f5ce1130508b4d46c1d24161f\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x7928ee75143c3ec562281c116499a7aad05f7ca3b7f8cf1be6592a83eb6c6e0e\",\"height\":6285958,\"date\":\"2018-09-07T03:22:35.000Z\",\"confirmations\":20399,\"type\":\"receive\",\"value\":964287270000000000,\"valueString\":\"964287270000000000\",\"feeString\":\"2022300000000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":222.6732163884,\"usdRate\":230.92,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-07T03:22:35.000Z\",\"action\":\"confirmed\"},{\"date\":\"2018-09-07T03:22:35.000Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0x9b58404cd02e752a7550dc5822b37f28181919bc\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":964287270000000000,\"valueString\":\"964287270000000000\",\"isPayGo\":false},{\"address\":\"0x59aa0e99cbe397dd5f2eb4013de9094f804a4f37\",\"value\":-964287270000000000,\"valueString\":\"-964287270000000000\",\"isPayGo\":false}],\"confirmedTime\":\"2018-09-07T03:22:35.000Z\",\"createdTime\":\"2018-09-07T03:22:35.000Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0xea8f947f272f9305a8e44e237fb1f9936b6932f9e949b8286b54f551f1014469\",\"id\":\"5b90fa73bdd1208d07913612aea291c3\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0xea8f947f272f9305a8e44e237fb1f9936b6932f9e949b8286b54f551f1014469\",\"height\":6281615,\"date\":\"2018-09-06T09:59:10.000Z\",\"confirmations\":24742,\"type\":\"receive\",\"value\":144000000000000000,\"valueString\":\"144000000000000000\",\"feeString\":\"1685250000000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":32.51664,\"usdRate\":225.81,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-09-06T09:59:10.000Z\",\"action\":\"confirmed\"},{\"date\":\"2018-09-06T09:59:10.000Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0x55b05132bd1435b28308fee8409c34e15401a09d\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":144000000000000000,\"valueString\":\"144000000000000000\",\"isPayGo\":false},{\"address\":\"0xfbb1b73c4f0bda4f67dca266ce6ef42f520fbb98\",\"value\":-144000000000000000,\"valueString\":\"-144000000000000000\",\"isPayGo\":false}],\"confirmedTime\":\"2018-09-06T09:59:10.000Z\",\"createdTime\":\"2018-09-06T09:59:10.000Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0x20cda42f617fedf6fa7f9a09a50746e73e1db73c24c8950f329e80dcf8fef2c0\",\"id\":\"5b7160471cc368660bc9d50a3be5ba4a\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x20cda42f617fedf6fa7f9a09a50746e73e1db73c24c8950f329e80dcf8fef2c0\",\"height\":6139549,\"date\":\"2018-08-13T10:40:58.000Z\",\"confirmations\":166808,\"type\":\"receive\",\"value\":194640540000000000,\"valueString\":\"194640540000000000\",\"feeString\":\"2022300000000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":62.197384557,\"usdRate\":319.55,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-08-13T10:40:58.000Z\",\"action\":\"confirmed\"},{\"date\":\"2018-08-13T10:40:58.000Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0x59aa0e99cbe397dd5f2eb4013de9094f804a4f37\",\"value\":-194640540000000000,\"valueString\":\"-194640540000000000\",\"isPayGo\":false},{\"address\":\"0xb33d05cb20bef8488d46a06aa8b67b9306bf554f\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":194640540000000000,\"valueString\":\"194640540000000000\",\"isPayGo\":false}],\"confirmedTime\":\"2018-08-13T10:40:58.000Z\",\"createdTime\":\"2018-08-13T10:40:58.000Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0xf339f82d9d9e9e4ee757a96842ac30462820ef0cf8f1a78ce137beb7a9f3685a\",\"id\":\"5b71185691f796ea085ff6b5f099ab73\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0xf339f82d9d9e9e4ee757a96842ac30462820ef0cf8f1a78ce137beb7a9f3685a\",\"height\":6138280,\"date\":\"2018-08-13T05:34:05.000Z\",\"confirmations\":168077,\"type\":\"receive\",\"value\":124508300000000000,\"valueString\":\"124508300000000000\",\"feeString\":\"2022300000000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":39.485317179,\"usdRate\":317.13,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-08-13T05:34:05.000Z\",\"action\":\"confirmed\"},{\"date\":\"2018-08-13T05:34:05.000Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0x59aa0e99cbe397dd5f2eb4013de9094f804a4f37\",\"value\":-124508300000000000,\"valueString\":\"-124508300000000000\",\"isPayGo\":false},{\"address\":\"0x1c61b846e9265f3bcc7589b380b3ce7fbb7125a6\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":124508300000000000,\"valueString\":\"124508300000000000\",\"isPayGo\":false}],\"confirmedTime\":\"2018-08-13T05:34:05.000Z\",\"createdTime\":\"2018-08-13T05:34:05.000Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0xc1b7cd5bd03db82c1a67ab3e5e7511fcfa34e06841a4fdf66fd30a735e2f7db0\",\"id\":\"5b69f686b577a3cd2033501b4e780043\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0xc1b7cd5bd03db82c1a67ab3e5e7511fcfa34e06841a4fdf66fd30a735e2f7db0\",\"height\":6106306,\"date\":\"2018-08-07T19:44:39.287Z\",\"confirmations\":200051,\"type\":\"send\",\"valueString\":\"-990000000000000\",\"feeString\":\"5456143000000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-0.3986433,\"usdRate\":402.67,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-08-07T19:44:39.287Z\",\"action\":\"confirmed\"},{\"date\":\"2018-08-07T19:44:06.909Z\",\"action\":\"signed\"},{\"date\":\"2018-08-07T19:44:06.681Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe08ca42594abe833a776ebfeb787c65760ae1053\",\"value\":990000000000000,\"valueString\":\"990000000000000\",\"isPayGo\":false},{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-990000000000000,\"valueString\":\"-990000000000000\",\"isPayGo\":false}],\"confirmedTime\":\"2018-08-07T19:44:39.287Z\",\"signedTime\":\"2018-08-07T19:44:06.909Z\",\"createdTime\":\"2018-08-07T19:44:06.681Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0x027c4b510ba934bd6ec8e97b894b366b83063b28c364fe9db64b85be5c292d90\",\"id\":\"5b69e4cb01a65fd3170c01e8e44e0107\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x027c4b510ba934bd6ec8e97b894b366b83063b28c364fe9db64b85be5c292d90\",\"height\":6106002,\"date\":\"2018-08-07T18:27:47.000Z\",\"confirmations\":200355,\"type\":\"receive\",\"value\":63665430000000000,\"valueString\":\"63665430000000000\",\"feeString\":\"505575000000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":25.790865693,\"usdRate\":405.1,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-08-07T18:27:47.000Z\",\"action\":\"confirmed\"},{\"date\":\"2018-08-07T18:27:47.000Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xfac24d7c365f658894c0341321f2198580dd0801\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":63665430000000000,\"valueString\":\"63665430000000000\",\"isPayGo\":false},{\"address\":\"0xe03c23519e18d64f144d2800e30e81b0065c48b5\",\"value\":-63665430000000000,\"valueString\":\"-63665430000000000\",\"isPayGo\":false}],\"confirmedTime\":\"2018-08-07T18:27:47.000Z\",\"createdTime\":\"2018-08-07T18:27:47.000Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0xf85970562ec5ad587c9394ad04d80d22a721917a26d1bc954cb560e4deb1ae2e\",\"id\":\"5b67df3246e138e306cdc16a5bbbaa20\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0xf85970562ec5ad587c9394ad04d80d22a721917a26d1bc954cb560e4deb1ae2e\",\"height\":6096913,\"date\":\"2018-08-06T05:40:43.488Z\",\"confirmations\":209444,\"type\":\"send\",\"valueString\":\"-8365222800000001\",\"feeString\":\"185082000000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-3.4362662218,\"usdRate\":410.78,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-08-06T05:40:43.488Z\",\"action\":\"confirmed\"},{\"date\":\"2018-08-06T05:40:02.529Z\",\"action\":\"signed\"},{\"date\":\"2018-08-06T05:40:02.319Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0x52128c6b6646114fb84138139cca5d0b9a6a07d6\",\"value\":8365222800000001,\"valueString\":\"8365222800000001\",\"isPayGo\":false},{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-8365222800000001,\"valueString\":\"-8365222800000001\",\"isPayGo\":false}],\"confirmedTime\":\"2018-08-06T05:40:43.488Z\",\"signedTime\":\"2018-08-06T05:40:02.529Z\",\"createdTime\":\"2018-08-06T05:40:02.319Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0x28b2967e09b8629dbd17db7965c20424f65cfefd9d040060cfd97ab284aeccd0\",\"id\":\"5b67dde0d90a9ce106ea07e9b0abd2fc\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x28b2967e09b8629dbd17db7965c20424f65cfefd9d040060cfd97ab284aeccd0\",\"height\":6096890,\"date\":\"2018-08-06T05:35:23.029Z\",\"confirmations\":209467,\"type\":\"send\",\"valueString\":\"-1782000000000000\",\"feeString\":\"134826000000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-0.73113678,\"usdRate\":410.29,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-08-06T05:35:23.029Z\",\"action\":\"confirmed\"},{\"date\":\"2018-08-06T05:34:24.325Z\",\"action\":\"signed\"},{\"date\":\"2018-08-06T05:34:24.133Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xaf72ee53c14d3b18de4dd8ac3f6496b01d3b0b9b\",\"value\":1782000000000000,\"valueString\":\"1782000000000000\",\"isPayGo\":false},{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-1782000000000000,\"valueString\":\"-1782000000000000\",\"isPayGo\":false}],\"confirmedTime\":\"2018-08-06T05:35:23.029Z\",\"signedTime\":\"2018-08-06T05:34:24.325Z\",\"createdTime\":\"2018-08-06T05:34:24.133Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0x0c505b063d46584c35b24dd4e4d5ed2c14a3afa98579992d54ad1c9f91c1419c\",\"id\":\"5b67dd2864bc3afc06445e6e2883a26f\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x0c505b063d46584c35b24dd4e4d5ed2c14a3afa98579992d54ad1c9f91c1419c\",\"height\":6096883,\"date\":\"2018-08-06T05:32:30.474Z\",\"confirmations\":209474,\"type\":\"send\",\"valueString\":\"-990000000000000\",\"feeString\":\"134954000000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-0.4061079,\"usdRate\":410.21,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-08-06T05:32:30.474Z\",\"action\":\"confirmed\"},{\"date\":\"2018-08-06T05:31:20.774Z\",\"action\":\"signed\"},{\"date\":\"2018-08-06T05:31:20.573Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0x9abc03367a864d2e322980f4c252293c1cfa9790\",\"value\":990000000000000,\"valueString\":\"990000000000000\",\"isPayGo\":false},{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-990000000000000,\"valueString\":\"-990000000000000\",\"isPayGo\":false}],\"confirmedTime\":\"2018-08-06T05:32:30.474Z\",\"signedTime\":\"2018-08-06T05:31:20.774Z\",\"createdTime\":\"2018-08-06T05:31:20.573Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0xaac470cf4053a0bb73e02cd8cb6d10e589ebaf9223d9f1ed0c8f7dd4ba9a23c9\",\"id\":\"5b67dc8ed0c51e54072ffdccb9b8eb7f\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0xaac470cf4053a0bb73e02cd8cb6d10e589ebaf9223d9f1ed0c8f7dd4ba9a23c9\",\"height\":6096873,\"date\":\"2018-08-06T05:30:14.556Z\",\"confirmations\":209484,\"type\":\"send\",\"valueString\":\"-1980000000000000\",\"feeString\":\"134954000000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-0.8128296,\"usdRate\":410.52,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-08-06T05:30:14.556Z\",\"action\":\"confirmed\"},{\"date\":\"2018-08-06T05:28:46.700Z\",\"action\":\"signed\"},{\"date\":\"2018-08-06T05:28:46.415Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xd271a8101acd1da7c01333614cb7ca41f58c5ebf\",\"value\":1980000000000000,\"valueString\":\"1980000000000000\",\"isPayGo\":false},{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-1980000000000000,\"valueString\":\"-1980000000000000\",\"isPayGo\":false}],\"confirmedTime\":\"2018-08-06T05:30:14.556Z\",\"signedTime\":\"2018-08-06T05:28:46.700Z\",\"createdTime\":\"2018-08-06T05:28:46.415Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0x2721777131b799fabdf6984b29e3f9a181c2b8a413e40a9744e3f722809073fd\",\"id\":\"5b67d76fbb9934c30bcb4c8a2d6262a2\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0x2721777131b799fabdf6984b29e3f9a181c2b8a413e40a9744e3f722809073fd\",\"height\":6096775,\"date\":\"2018-08-06T05:06:42.000Z\",\"confirmations\":209582,\"type\":\"receive\",\"value\":13249720000000000,\"valueString\":\"13249720000000000\",\"feeString\":\"893182500000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":5.4477548752,\"usdRate\":411.16,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-08-06T05:06:42.000Z\",\"action\":\"confirmed\"},{\"date\":\"2018-08-06T05:06:42.000Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0x4e667ed19fbf4cd37696d90f5de3f4aeada9599b\",\"value\":-13249720000000000,\"valueString\":\"-13249720000000000\",\"isPayGo\":false},{\"address\":\"0xfac24d7c365f658894c0341321f2198580dd0801\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":13249720000000000,\"valueString\":\"13249720000000000\",\"isPayGo\":false}],\"confirmedTime\":\"2018-08-06T05:06:42.000Z\",\"createdTime\":\"2018-08-06T05:06:42.000Z\",\"label\":\"\"},{\"normalizedTxHash\":\"0xe47793b1b1ffc95e776db5c9f5c2df84fb3f827f3b2e64ba69e63c2986c14bb7\",\"id\":\"5b67423ce4127a2b07beef1d43d46b68\",\"coin\":\"eth\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"enterprise\":\"5a79359692f6f738073dffcbbb0c22df\",\"txid\":\"0xe47793b1b1ffc95e776db5c9f5c2df84fb3f827f3b2e64ba69e63c2986c14bb7\",\"height\":6094165,\"date\":\"2018-08-05T18:31:00.646Z\",\"confirmations\":212192,\"type\":\"send\",\"valueString\":\"-3376018800000000\",\"feeString\":\"221944800000000\",\"payGoFee\":0,\"payGoFeeString\":\"0\",\"usd\":-1.3679290576,\"usdRate\":405.19,\"state\":\"confirmed\",\"tags\":[\"5ae6fce3cee2f9225448f44cf9154900\",\"5a79359692f6f738073dffcbbb0c22df\"],\"history\":[{\"date\":\"2018-08-05T18:31:00.646Z\",\"action\":\"confirmed\"},{\"date\":\"2018-08-05T18:30:20.473Z\",\"action\":\"signed\"},{\"date\":\"2018-08-05T18:30:20.275Z\",\"action\":\"created\"}],\"entries\":[{\"address\":\"0xe6b8b5c49d83619e91a57d50d169eabee8f91f6b\",\"wallet\":\"5ae6fce3cee2f9225448f44cf9154900\",\"value\":-3376018800000000,\"valueString\":\"-3376018800000000\",\"isPayGo\":false},{\"address\":\"0x74ba2e467524725d03f2234c11b3a768a70bc14b\",\"value\":3376018800000000,\"valueString\":\"3376018800000000\",\"isPayGo\":false}],\"confirmedTime\":\"2018-08-05T18:31:00.646Z\",\"signedTime\":\"2018-08-05T18:30:20.473Z\",\"createdTime\":\"2018-08-05T18:30:20.275Z\",\"label\":\"\"}],\"nextBatchPrevId\":\"6094165-5b67423ce4127a2b07beef1d43d46b68\"}"; // ntrivedi tempeorry
                    JObject GenerateResponse = JObject.Parse(CommonMethod.DepositHistoryResponse);                   
                    BitgoResponse GenerateResponseClsObj = (JsonConvert.DeserializeObject<BitgoResponse>(JsonConvert.SerializeObject(GenerateResponse)));
                    //JArray jarr = (JArray)person1.SelectToken("transfers");
                    foreach (var item in GenerateResponseClsObj.transfers)
                    {
                        logs.WriteRequestLog("Found in List TxID=" + item.txid, "CallThirdPartyAPI", CommonMethod.SMSCode);
                        //if(item.txid == "0xdd84228cb4b0268d9cf50ac829a65e06f16ece363738fced924a3774b7fef0f5")
                        //{
                        //    string test = "12";
                        //}
                        if (item.coin.ToLower() == CommonMethod.SMSCode.ToLower()) // value condition removed 04-01-2018
                        {                           
                            foreach (var item1 in item.entries)
                            {//ntrivedi 04-01-2018 removing receive condition 
                               
                                if (item1.value > 0 && item1.address != string.Empty && item1.wallet == CommonMethod.ProviderWalletID)
                                {                                  
                                    sqlStr = "select WalletID from AddressMasters where Address='" + item1.address + "' and Status=1 and WalletID in (select id from WalletMasters where WalletTypeID in (select id from Wallettypemasters where wallettypename='"+ CommonMethod.SMSCode +"'))";
                                    DataSet dataSet = (new DataCommon()).OpenDataSet("AddressMasters", sqlStr);
                                    if (dataSet != null)
                                    {
                                        if (dataSet.Tables[0].Rows.Count > 0) //ntrivedi temperory || dataSet.Tables[0].Rows.Count == 0
                                        {
                                            //addressID = Convert.ToInt64(dataSet.Tables[0].Rows[0]["ID"]); ntrivedi temperory
                                            //WalletID = 1; // temperory ntrivedi
                                            WalletID = Convert.ToInt64(dataSet.Tables[0].Rows[0]["WalletID"]); //ntrivedi temperory
                                            if (WalletID > 0) // address exist to our db
                                            {
                                                sqlStr = "select UserID from WalletMasters where ID=@WalletID";
                                                userID = Convert.ToInt64((new DataCommon()).ExecuteScalarWDMParameterize("@WalletID", WalletID.ToString(), sqlStr));

                                                RespTransfers respTransfers = new RespTransfers();
                                                respTransfers.address = item1.address;
                                                respTransfers.Amount = Math.Round((item1.value / CommonMethod.ConvertAmt), 8); //item1.value / CommonMethod.ConvertAmt;
                                                respTransfers.coin = item.coin;
                                                respTransfers.confirmations = item.confirmations;
                                                respTransfers.confirmedTime = item.confirmedTime.ToString();
                                                respTransfers.createdTime = DateTime.Now.ToString();
                                                respTransfers.fromaddress = "";
                                                respTransfers.IsValid = true;
                                                respTransfers.OrderId = 0;
                                                respTransfers.state = "Initialize";
                                                respTransfers.txid = item.txid;
                                                respTransfers.unconfirmedTime = item.unconfirmedTime.ToString();
                                                respTransfers.value = item1.value / CommonMethod.ConvertAmt;
                                                respTransfers.valueStr = item1.valueString;
                                                respTransfers.wallet = CommonMethod.ProviderWalletID;
                                                respTransfers.UserID = userID;
                                                //respTransfers..
                                                CommonMethod.Transfers.Add(respTransfers);
                                                logs.WriteRequestLog("Added to Array TxID=" +  item.txid, "CallThirdPartyAPI", CommonMethod.SMSCode);
                                            }
                                        }
                                    }
                                }
                                }
                            }
                            prevID = GenerateResponseClsObj.nextBatchPrevId;
                        }
                    
                    CommonMethod.resultCount = CommonMethod.Transfers.Count;
                    //if (Convert.ToString(GenerateResponse.SelectToken("coin")).ToLower() == CommonMethod.SMSCode.ToLower())
                    //{
                    //    if (CommonMethod.Category == 1) // For BTC , BTG , BCH Response 
                    //    {
                    //        foreach (var item in GenerateResponse.SelectToken("transfers").Select((val, i) => new { i, val }))
                    //        {
                    //            item.val["IsValid"] = false;
                    //            if (!IsNullOrEmpty(item.val["outputs"]) && !IsNullOrEmpty(item.val["id"]) && !IsNullOrEmpty(item.val["txid"]) && !IsNullOrEmpty(item.val["coin"]) && !IsNullOrEmpty(item.val["wallet"]) && !IsNullOrEmpty(item.val["confirmedTime"]) && !IsNullOrEmpty(item.val["unconfirmedTime"]) && !IsNullOrEmpty(item.val["createdTime"]) && !IsNullOrEmpty(item.val["value"]) && !IsNullOrEmpty(item.val["confirmations"]) && !IsNullOrEmpty(item.val["state"]))
                    //            {
                    //                foreach (var item1 in item.val.SelectToken("outputs"))
                    //                {
                    //                    //if(Convert.ToString(item1["chain"]) == "0")
                    //                    //{
                    //                    //    Console.WriteLine(Convert.ToString(item1["id"]).Split(':')[0].ToString());
                    //                    //    Console.WriteLine(Convert.ToString(item1["id"]).Split(':')[0] == Convert.ToString(item.val["txid"]));

                    //                    //    if (Convert.ToString(item1["id"]).Split(':')[0] == Convert.ToString(item.val["txid"]))
                    //                    //    {
                    //                    //        if(Convert.ToString(item1["value"]) == Convert.ToString(Convert.ToString(item1["value"])))
                    //                    //        {
                    //                    //            Console.WriteLine(Convert.ToString(item1["value"]));
                    //                    //            Console.WriteLine(Convert.ToString(Convert.ToString(item1["value"])));

                    //                    //        }
                    //                    //    }
                    //                    //}
                    //                    //Console.WriteLine("{0} {1} {2} {3}" , Convert.ToString(item1["chain"]) == "0" , Convert.ToString(item1["id"]).Split(':')[0], item1["id"] == item.val["txid"] , item1["value"] == item.val["value"]);

                    //                    if (Convert.ToString(item1["chain"]) == "0" && Convert.ToString(item1["id"]).Split(':')[0] == Convert.ToString(item.val["txid"]) && Convert.ToString(item1["value"]) == Convert.ToString(item.val["value"]) && !IsNullOrEmpty(item1["redeemScript"]) && !IsNullOrEmpty(item1["index"]))
                    //                    {
                    //                        string value = (string)item1.SelectToken("address");
                    //                        item.val["address"] = value;
                    //                        item.val["Amount"] = Convert.ToDecimal(item.val["value"]) / Convert.ToDecimal(ConfigurationManager.AppSettings[Convert.ToString(item.val["coin"])]);
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
                    //                        item.val["Amount"] = Convert.ToDecimal(item.val["value"]) / Convert.ToDecimal(ConfigurationManager.AppSettings[Convert.ToString(item.val["coin"])]);
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
                    logs.WriteRequestLog("Generate Response :  " + JsonConvert.SerializeObject(CommonMethod.Transfers), "CallThirdPartyAPI", CommonMethod.SMSCode, Convert.ToInt16(ConfigurationManager.AppSettings["AllowLog"]), Action: 2);
                }
                else
                {
                    logs.WriteRequestLog("Generate Response :  Transaction Detail not found", "CallThirdPartyAPI", CommonMethod.SMSCode, Convert.ToInt16(ConfigurationManager.AppSettings["AllowLog"]), Action: 2);
                }
            }
            catch (Exception ex)
            {
                logs.WriteErrorLog(ex, "Program", "CallThirdPartyAPI");
            }
        }

        private static string CallThirdPartyCryptoAPI(ref CommonMethods CommonMethod, long count, long limit)
        {
            string SqlStr = string.Empty;
            string address = string.Empty;
            string fromaddress = string.Empty;
            decimal Amount = 0;
            // long count = 0;
            try
            {
                if (!string.IsNullOrEmpty(CommonMethod.Str_URL) && !string.IsNullOrEmpty(CommonMethod.Str_RequestType) && !string.IsNullOrEmpty(CommonMethod.ContentType))
                {

                    string authInfo = CommonMethod.UserName + ":" + CommonMethod.Password;
                    authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                    WebRequest myReqrpc = WebRequest.Create(CommonMethod.Str_URL);
                    myReqrpc.Headers["Authorization"] = "Basic " + authInfo;
                    myReqrpc.Method = CommonMethod.Str_RequestType;

                    string ReqStr = @"" + CommonMethod.RequestBody;
                    ReqStr = ReqStr.Replace("#AutoNo#", Convert.ToString(DateTime.Now.ToString("ddMMyyyyHHmmss")));
                    //ReqStr = ReqStr.Replace("#Address#", Address);
                    ReqStr = ReqStr.Replace("#Limit#", limit.ToString());
                    ReqStr = ReqStr.Replace("#Count#", count.ToString());
                    Console.WriteLine("Limit:{0} ,Count:{1}", limit.ToString(), count.ToString());
                    logs.WriteRequestLog("RPC call Request:" + ReqStr, "CallThirdPartyCryptoAPI", CommonMethod.SMSCode,Action:2);
                    StreamWriter sw = new StreamWriter(myReqrpc.GetRequestStream());
                    sw.Write(ReqStr);
                    sw.Close();

                    WebResponse response = myReqrpc.GetResponse();

                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    CommonMethod.DepositHistoryResponse = StreamReader.ReadToEnd();
                    StreamReader.Close();
                    response.Close();

                    logs.WriteRequestLog("RPC Address Response :  " + CommonMethod.DepositHistoryResponse, "CallThirdPartyCryptoAPI", CommonMethod.SMSCode);
                    if (!string.IsNullOrEmpty(CommonMethod.DepositHistoryResponse))
                    {
                        TPResponse responseObj = new DepositConsoleApplication.TPResponse();
                        responseObj = JsonConvert.DeserializeObject<TPResponse>(CommonMethod.DepositHistoryResponse);
                        foreach (Result temp in responseObj.result)
                        {
                            RespTransfers obj = new RespTransfers();
                            fromaddress = "";
                            address = "";
                            Amount = 0;
                            if (temp.valid == true) //nt
                            {
                                if (temp.balance.assets != null)
                                {
                                    if (temp.balance.assets.Count == 1)
                                    {
                                        if (temp.balance.assets[0].qty < 0)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            Amount = temp.balance.assets[0].qty;
                                        }
                                    }
                                }                                
                                if (temp.myaddresses != null)
                                {
                                    if (temp.myaddresses.Count == 2)
                                    {
                                        //address = temp.myaddresses[1];
                                        //fromaddress = temp.myaddresses[0];
                                        address = temp.myaddresses[0]; //ntrivedi index changes 27-06-2018 after testing http://40.65.191.77:2688/ bhai coin 0 th index is destination (receiver) and first index is sender
                                        fromaddress = temp.myaddresses[1];
                                        if (fromaddress != string.Empty)
                                        {
                                            SqlStr = "select Count(TS.TrnNo) from TransactionStatus  TS inner join TransactionQueue TQ on TQ.TrnNo=TS.TrnNo " +
                                            " where TQ.Status = 4 and TS.OprTrnID = @OprTrnID " +
                                            "and TQ.MemberMobile = @MemberMobile and TQ.CustomerMobile = @CustomerMobile";
                                            int tempcount = Convert.ToInt16((new DataCommon()).ExecuteScalarWDMParameterize("@OprTrnID,@MemberMobile,@CustomerMobile", temp.txid + "," + temp.myaddresses[0] + "," + temp.myaddresses[1], SqlStr));
                                            if (tempcount > 0) // interchange the two addresses
                                            {
                                                address = temp.myaddresses[1];
                                                fromaddress = temp.myaddresses[0];
                                                //logs.WriteRequestLog("RPC Address Interchange :  " + temp.txid, "CallThirdPartyCryptoAPI", CommonMethod.SMSCode); remove extra logging 07-09-2018
                                            }
                                        }
                                    }
                                    else if (temp.myaddresses.Count == 1)
                                    {
                                        address = temp.myaddresses[0];
                                        if (temp.addresses.Count > 0)
                                        {
                                            fromaddress = temp.addresses[0];
                                        }
                                    }
                                    else if (temp.myaddresses.Count > 2)
                                    {
                                        if (temp.addresses.Count > 0)
                                        {
                                            fromaddress = temp.addresses[0]; // saving first index from address 
                                        }
                                        if (Amount > 0) // ntrivedi need to confirm with bhai for amount condition 
                                        {
                                            logs.WriteRequestLog("Parse Response :  Multiple deposite address found", "CallThirdPartyCryptoAPI", CommonMethod.SMSCode);
                                            foreach (string dummyAddress in temp.myaddresses)
                                            {
                                                System.DateTime dateTime1 = new System.DateTime(1970, 1, 1, 0, 0, 0, 0); //komal 8-9-2018 add date convert code
                                                string printDate = dateTime1.ToShortDateString() + " " + dateTime1.ToShortTimeString();
                                                //logs.WriteRequestLog("printDate 1 :  " + printDate + ", dayDiff" + dayDiff, "GetHistory", CommonMethod.SMSCode);
                                                // Add the number of seconds in UNIX timestamp to be converted.
                                                dateTime1 = dateTime1.AddSeconds(Convert.ToInt64(temp.time));

                                                // The dateTime now contains the right date/time so to format the string,
                                                // use the standard formatting methods of the DateTime object.
                                                printDate = dateTime1.ToShortDateString() + " " + dateTime1.ToShortTimeString();
                                                RespTransfers tempobj = new RespTransfers();
                                                tempobj.address = dummyAddress;
                                                tempobj.coin = CommonMethod.SMSCode;
                                                tempobj.value = 0;
                                                tempobj.Amount = 0; // multiple address and single txnid will have exat amount in status check api
                                                tempobj.confirmations = temp.confirmations;
                                                tempobj.confirmedTime = temp.timereceived.ToString();
                                                //tempobj.createdTime = logs.UTC_To_IST().ToString();
                                                tempobj.createdTime = printDate.ToString(); 
                                                tempobj.txid = temp.txid;
                                                tempobj.unconfirmedTime = temp.timereceived.ToString();
                                                tempobj.IsValid = temp.valid;
                                                tempobj.OrderId = 0;
                                                tempobj.fromaddress = fromaddress;
                                                
                                                CommonMethod.Transfers.Add(tempobj);
                                            }
                                            continue;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(address)) // ntrivedi temperory fetch first index 
                                    {
                                        obj.address = address; // ntrivedi temperory fetch first index 
                                        obj.coin = CommonMethod.SMSCode;
                                        obj.value = 0;
                                        obj.Amount = Amount; // this amount will be updated by rita's statuscheck tps
                                        obj.confirmations = temp.confirmations;
                                        obj.confirmedTime = temp.timereceived.ToString();
                                        obj.createdTime = logs.UTC_To_IST().ToString();
                                        obj.txid = temp.txid;
                                        obj.unconfirmedTime = temp.timereceived.ToString();
                                        obj.IsValid = temp.valid;
                                        obj.OrderId = 0;
                                        obj.fromaddress = fromaddress;
                                        CommonMethod.Transfers.Add(obj);
                                    }
                                }
                            }
                        }
                        CommonMethod.resultCount = responseObj.result.Count;
                        return CommonMethod.DepositHistoryResponse;
                    }
                    else
                    {
                        logs.WriteRequestLog("Generate Response :  Transaction Detail not found", "CallThirdPartyCryptoAPI", CommonMethod.SMSCode);
                        return CommonMethod.DepositHistoryResponse;
                    }
                    return CommonMethod.DepositHistoryResponse;
                }
                return CommonMethod.DepositHistoryResponse;
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
                    logs.WriteRequestLog("BlockChainTransfer exception : " + CommonMethod.DepositHistoryResponse, "CallThirdPartyCryptoAPI", CommonMethod.SMSCode, Action: 2);
                }

                logs.WriteRequestLog("webex : " + webex, "CallThirdPartyCryptoAPI", CommonMethod.SMSCode, Action: 2);
                return CommonMethod.DepositHistoryResponse;
            }
        }

        // khushali 29-01-2019
        private static void CallThirdPartyERC20API(ref CommonMethods CommonMethod, string iterationPrevID, string Authorization, string enterprise, string walletid)
        {
            try
            {
                string sqlStr = "";
                long WalletID = 0;
                long userID = 0;
                //CommonMethods CommonMethod = (CommonMethods)CommonMethodObj;
                Console.WriteLine("API Call " + CommonMethod.SMSCode);
                CommonMethod.Str_URL = CommonMethod.Str_URL.Replace("#prevId#", iterationPrevID.ToString());
                CommonMethod.Str_URL = CommonMethod.Str_URL.Replace("#ProviderWalletID#", walletid.ToString());


                if (!string.IsNullOrEmpty(CommonMethod.Str_URL) && !string.IsNullOrEmpty(CommonMethod.Str_RequestType) && !string.IsNullOrEmpty(CommonMethod.ContentType))
                {
                    logs.WriteRequestLog("Request :  " + CommonMethod.Str_URL, "CallThirdPartyAPI", CommonMethod.SMSCode);
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(CommonMethod.Str_URL);
                    httpWebRequest.ContentType = CommonMethod.ContentType;
                    httpWebRequest.Method = CommonMethod.Str_RequestType;
                    httpWebRequest.KeepAlive = false;
                    httpWebRequest.Timeout = 180000;
                    //httpWebRequest.Headers.Add("Authorization", Authorization);
                    //httpWebRequest.Headers.Add("enterprise", enterprise);
                    var i = 1;
                    do
                     {
                        CommonMethod.RequestBody = CommonMethod.RequestBody.Replace("#Username#", CommonMethod.UserName);
                        CommonMethod.RequestBody = CommonMethod.RequestBody.Replace("#Password#", CommonMethod.Password);
                        CommonMethod.RequestBody = CommonMethod.RequestBody.Replace("#AssetName#", CommonMethod.AssetName);
                        CommonMethod.RequestBody = CommonMethod.RequestBody.Replace("#PageNo#", i.ToString());
                        CommonMethod.RequestBody = CommonMethod.RequestBody.Replace("#Limit#", "1000");
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            streamWriter.Write(CommonMethod.RequestBody);
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
                        logs.WriteRequestLog("Response :  " + CommonMethod.DepositHistoryResponse, "CallThirdPartyAPI", CommonMethod.SMSCode);
                        if (string.IsNullOrEmpty(CommonMethod.DepositHistoryResponse))
                        {
                            return;
                        }
                        if (CommonMethod.DepositHistoryResponse.Count() == 1000)
                        {
                            i = i + 1;
                        }
                        else
                        {
                            i = 0;
                        }
                    } while (i == 0);
                                        
                    logs.WriteRequestLog("Generate Response :  " + JsonConvert.SerializeObject(CommonMethod.Transfers), "CallThirdPartyAPI", CommonMethod.SMSCode, Convert.ToInt16(ConfigurationManager.AppSettings["AllowLog"]), Action: 2);
                }
                else
                {
                    logs.WriteRequestLog("Generate Response :  Transaction Detail not found", "CallThirdPartyAPI", CommonMethod.SMSCode, Convert.ToInt16(ConfigurationManager.AppSettings["AllowLog"]), Action: 2);
                }
            }
            catch (Exception ex)
            {
                logs.WriteErrorLog(ex, "Program", "CallThirdPartyAPI");
            }
        }
        #endregion


        #region "ReadMasterFile"

        public static void ReadMasterFile(string APIName, ref CommonMethods CommonMethod)
        {
            string FilePath = ConfigurationManager.AppSettings["MainPath"] + "\\MasterFile_" + APIName + ".txt";

            try
            {
                if (System.IO.File.Exists(FilePath) == true)
                {
                    CommonMethod.StaticArray[0] = "0";
                    CommonMethod.TransactionFile = ConfigurationManager.AppSettings["MainPath"]; //FilePath

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

                    logs.WriteRequestLog("Transaction File Path :  " + CommonMethod.Path_AddressGenerate, "ReadMasterFile", CommonMethod.SMSCode,Action:2);
                }
                else
                {
                    //logs.CreateMasterFile(FilePath, CommonMethod.SMSCode);
                    logs.WriteRequestLog(FilePath + " File Not Found", "ReadMasterFile", CommonMethod.SMSCode,Action :2);
                }
            }
            catch (Exception ex)
            {
                logs.WriteErrorLog(ex, "Program", "ReadMasterFile");
            }
        }

        public static void ReadTransactionalFile(string Path, ref CommonMethods CommonMethod)
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
                        else if (CommonMethod.TrnLeftTitle.Contains("trnid")) //Read RequestBody
                        {
                            CommonMethod.trnID = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("authorization")) //Read RequestBody
                        {
                            CommonMethod.authorization = line.Substring(line.IndexOf(CommonMethod.MainSaperator) + 1);
                        }
                        else if (CommonMethod.TrnLeftTitle.Contains("enterprise")) //Read RequestBody
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

                    logs.WriteRequestLog("Transaction URL :  " + CommonMethod.Str_URL + " Request Type : " + CommonMethod.Str_RequestType + " Content Type : " + CommonMethod.ContentType, "ReadTransactionalFile", CommonMethod.SMSCode, Convert.ToInt16(ConfigurationManager.AppSettings["AllowLog"]), Action :2);
                }
                else
                {
                    //logs.CreateTransactionFile(Path, CommonMethod.SMSCode);
                    logs.WriteRequestLog(Path + " File Not Found", "ReadTransactionalFile", CommonMethod.SMSCode, Action: 2);
                }
            }
            catch (Exception ex)
            {
                logs.WriteErrorLog(ex, "Program", "ReadTransactionalFile");
            }
        }
        #endregion

        public class walletServiceData
        { //select top 50 AutoNo,AccountNo,AddTrnCount,BranchName as SMSCode from MemberBankMaster where Type=1 and TPSPickupStatus=0 order by TPSPickupDate

            //public long AutoNo { get; set; }
            //public string Address { get; set; }
            // public long AddTrnCount { get; set; }
            public string SMSCode { get; set; }
            public long RecordCount { get; set; }
            public long Limit { get; set; }
            public string PreviousTrnID { get; set; }
            public string LastTrnID { get; set; }
            public long MaxLimit { get; set; }
            public long AutoNo { get; set; }
           public long serProID { get; set; }
            //public int WallletStatus { get; set; }
            //public int ServiceStatus { get; set; }
            public int AppType { get; set; }
            public string prevIterationID { get; set; } // bitgo previd

            // public int TrnCount { get; set; }
        }

        public class CommonMethods
        {
            public string TransactionFile;
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
            public string trnID;
            public int resultCount = 0;
            public List<RespLocalCoin> RespLocalCoins { get; set; }
            public string authorization { get; set; }
            public string enterprise { get; set; }
            public decimal ConvertAmt { get; set; }
            public string ProviderWalletID { get; set; }
            //authorization, 
        }

        enum EnAppType
        {
            BitGoAPI = 1,
            CryptoAPI = 2,
            EtherScan = 3
        }

        public class Transaction
        {
            public string blockNumber { get; set; }
            public string timeStamp { get; set; }
            public string hash { get; set; }
            public string blockHash { get; set; }
            public string nonce { get; set; }
            public string from { get; set; }
            public string to { get; set; }
            public string tokenName { get; set; }
            public string tokenSymbol { get; set; }
            public string tokenDecimal { get; set; }
            public string value { get; set; }
            public string gas { get; set; }
            public string gasPrice { get; set; }
            public string cumulativeGasUsed { get; set; }
            public string gasUsed { get; set; }
            public string confirmations { get; set; }
        }

        public class ERC20Response
        {
            public bool isError { get; set; } // true id request failed 
            public IList<Transaction> transactions { get; set; }
        }

        public class RespTransfers
        {
            public string id { get; set; }
            public string coin { get; set; }
            public string wallet { get; set; }
            public string txid { get; set; }
            public string address { get; set; }
            public string fromaddress { get; set; }

            //public int height { get; set; }
            //public DateTime date { get; set; }
            public long confirmations { get; set; }
            public decimal value { get; set; }
            public string valueStr { get; set; }

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
            public long UserID { get; set; }

        }

        public class RespLocalCoin
        {
            public int confirmations { get; set; }
            public string txid { get; set; }
            public string address { get; set; }
            public string confirmedTime { get; set; }
            public string unconfirmedTime { get; set; }
            public int value { get; set; }
            public decimal Amount { get; set; }
        }

        public static bool IsNullOrEmpty(JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }

    }
}