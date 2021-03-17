﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Runtime.InteropServices;

namespace DepositConsoleApplication
{
    public class CommonFunctions
    {
        static DataCommon dComm = new DataCommon();

        public void WriteErrorLog(Exception ex, string path, string Source, string OtherInfo = "")
        {
            string strFilePath = string.Empty;
            try
            {

                strFilePath = ConfigurationManager.AppSettings["MainPath"] + "\\ErrorLog.txt";
                using (StreamWriter sw = new StreamWriter(strFilePath, true))
                {
                    sw.WriteLine("--------------------------------------------------------------------------------------");
                    sw.WriteLine("{0} {1}", UTC_To_IST().ToLongTimeString(), UTC_To_IST().ToLongDateString());
                    sw.WriteLine("Error Path          : {0}", path);
                    sw.WriteLine("Error Message       : {0}", ex.Message);
                    sw.WriteLine("Error Source        : {0}", ex.Source);
                    sw.WriteLine("Inner Exception     : {0}", ex.InnerException);
                    sw.WriteLine("System Source       : {0}", Source);
                    if (string.IsNullOrEmpty(OtherInfo) == false)
                    {
                        sw.WriteLine("Other Info          : {0}", OtherInfo);
                    }
                    sw.WriteLine("--------------------------------------------------------------------------------------");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception exW)
            {
                exW = null;
            }
        }

        public void WriteToDataFile(string request, string dataToWrite)
        {
            try
            {

                int fn = 1;
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "/MsgInfo" + DateTime.Now.ToString("yyyyMMdd") + ".txt") == false)
                {
                    File.Create(System.AppDomain.CurrentDomain.BaseDirectory + "/MsgInfo" + DateTime.Now.ToString("yyyyMMdd") + ".txt").Close();

                }
                File.SetAttributes(System.AppDomain.CurrentDomain.BaseDirectory + "/ErrorLog" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileAttributes.Archive);
                using (StreamWriter sw = File.AppendText(System.AppDomain.CurrentDomain.BaseDirectory + "/ErrorLog" + DateTime.Now.ToString("yyyyMMdd") + ".txt"))
                {
                    sw.Write("Log Entry          : ");
                    sw.WriteLine("{0} {1} {2}", DateTime.Now.ToLongTimeString(), request, DateTime.Now.ToLongDateString());
                    sw.WriteLine("Error Message      :{0}", dataToWrite);
                    sw.WriteLine("-------------------------------");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex1)
            {
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "/MsgInfo1" + DateTime.Now.ToString("yyyyMMdd") + ".txt") == false)
                {
                    File.Create(System.AppDomain.CurrentDomain.BaseDirectory + "/MsgInfo1" + DateTime.Now.ToString("yyyyMMdd") + ".txt").Close();
                }
                File.SetAttributes(System.AppDomain.CurrentDomain.BaseDirectory + "/MsgInfo1" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileAttributes.Archive);
                using (StreamWriter sw = File.AppendText(System.AppDomain.CurrentDomain.BaseDirectory + "/MsgInfo1" + DateTime.Now.ToString("yyyyMMdd") + ".txt"))
                {
                    sw.Write("Log Entry          : ");
                    sw.WriteLine("{0} {1} {2}", DateTime.Now.ToLongTimeString(), request, DateTime.Now.ToLongDateString());
                    sw.WriteLine("Error Message      :{0}", ex1.Message);
                    sw.WriteLine("-------------------------------");
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        public void AddToLog(string logMsg, string OtherInfo = "")
        {
            StreamWriter sw = default(StreamWriter);
            string strFilePath = string.Empty;
            string mainPath = ConfigurationManager.AppSettings.Get("MainPath");

            try
            {
                strFilePath = mainPath + "\\Logs\\Logs_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

                if (File.Exists(strFilePath))
                {
                    sw = File.AppendText(strFilePath);
                    sw.WriteLine("--------------------------------------------------------------------------------------");
                    sw.Write(ControlChars.CrLf + "Log Entry           : ");
                    sw.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    sw.WriteLine("Message       : {0}", logMsg);
                    if (string.IsNullOrEmpty(OtherInfo) == false)
                    {
                        sw.WriteLine("Other Info          : {0}", OtherInfo);
                    }
                    sw.WriteLine("--------------------------------------------------------------------------------------");
                    sw.Flush();
                    sw.Close();
                }
                else
                {
                    sw = new StreamWriter(strFilePath, false);
                    sw.WriteLine("--------------------------------------------------------------------------------------");
                    sw.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    sw.WriteLine("Message       : {0}", logMsg);
                    if (string.IsNullOrEmpty(OtherInfo) == false)
                    {
                        sw.WriteLine("Other Info          : {0}", OtherInfo);
                    }
                    sw.WriteLine("--------------------------------------------------------------------------------------");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception exW)
            {
                exW = null;
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
                                WriteErrorLog(generatedExceptionName, this.GetType().Name, "WriteRequestLog-Retry:Sleep");
                                // Wait some time before retry (2 secs)
                                Thread.Sleep(2000);
                                // handle unsuccessfull write attempts or just ignore.
                            }
                            else
                            {
                                WriteErrorLog(generatedExceptionName, this.GetType().Name, "WriteRequestLog-Retry Complete");
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + methodName);
                ex = null;
            }
        }

        public void WriteResponseToLog(string outputString, string methodName)
        {
            StreamWriter sw = default(StreamWriter);
            string strFilePath = string.Empty;
            string mainPath = ConfigurationManager.AppSettings.Get("MainPath");
            try
            {
                strFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles\\LogFile" + DateAndTime.Now.ToString("yyyyMMdd") + ".txt";
                //strFilePath = mainPath + "\\LogFiles\\LogFile" + DateAndTime.Now.ToString("yyyyMMdd") + ".txt";

                if (File.Exists(strFilePath))
                {
                    sw = File.AppendText(strFilePath);
                    sw.WriteLine("----------   " + methodName + " " + "  Response  at " + DateTime.Now);
                    sw.WriteLine("Output              : {0}", outputString);
                    sw.WriteLine("");
                    sw.Flush();
                    sw.Close();
                }
                else
                {
                    sw = new StreamWriter(strFilePath, false);
                    sw.WriteLine("----------   " + methodName + " " + "  Response  at " + DateTime.Now);
                    sw.WriteLine("Output              : {0}", outputString);

                    sw.WriteLine("");

                    sw.Flush();
                    sw.Close();
                }

            }
            catch (Exception exW)
            {
                WriteErrorLog(exW, "WriteResponseToLog", "WriteLog");
                exW = null;
            }
        }

        public DateTime UTC_To_IST()
        {
            try
            {
                DateTime myUTC = DateTime.UtcNow;
                DateTime istdate = TimeZoneInfo.ConvertTimeFromUtc(myUTC, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                return istdate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateTransactionFile(string path, string coin)
        {
            string originalpath = "C:\\Users\\JBSPL\\Desktop\\New folder\\Configuration\\FUN\\FUN_TransactionFile_BITGO.txt";
            string text = System.IO.File.ReadAllText(originalpath);
            text = text.Replace("/fun/", "/" + coin.ToLower() + "/");
            text = text.Replace("AssetName:FUN", "AssetName:" + coin);
            if(!Directory.Exists("C:\\Users\\JBSPL\\Desktop\\New folder\\Configuration\\Configuration\\" + coin))
            {
                Directory.CreateDirectory("C:\\Users\\JBSPL\\Desktop\\New folder\\Configuration\\Configuration\\" + coin);
            }
            using (StreamWriter sw = new StreamWriter(path , true))
            {
                sw.WriteLine(text);
                sw.Flush();
                sw.Close();
            }
        }

        public void CreateMasterFile(string path, string coin)
        {
            string originalpath = "C:\\Users\\JBSPL\\Desktop\\New folder\\Configuration\\Configuration\\MasterFile_FUN_BITGO.txt";
            string text = System.IO.File.ReadAllText(originalpath);
            text = text.Replace("FUN" , coin.ToUpper());          
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(text);
                sw.Flush();
                sw.Close();
            }
        }
    }
}
