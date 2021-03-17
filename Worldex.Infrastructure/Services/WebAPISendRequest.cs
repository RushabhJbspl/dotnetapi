using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services
{
    //For All type of Web Request
    public class WebAPISendRequest : IWebApiSendRequest
    {
        string ControllerName = "WebAPISendRequest";

        public string  SendAPIRequestAsync(string Url, string Request, string ContentType,int Timeout= 180000, WebHeaderCollection   headerDictionary = null, string MethodType = "POST" , long TrnNo=0)
        {
            string responseFromServer = "";
            try
            {               
                object ResponseObj = new object();
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
                
                
                httpWebRequest.Method = MethodType.ToUpper();
                httpWebRequest.KeepAlive = false;
                httpWebRequest.Timeout = Timeout;
                httpWebRequest.Headers = headerDictionary;
                httpWebRequest.ContentType = ContentType;  //ntrivedi 11-12-2018 moving contenttype after the headers assgning otherwise content type is overwritten by headerDirectory

                HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name,Url + "TrnNo::" + TrnNo.ToString() + "::Request::"+ Request); //ntrivedi logging TrnNo 15-05-2019
                //Rushabh 15-10-2019 Log Header Value In Log File As Per Instruction By Nupoora Mam.                
                HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, "HeaderData : " + Helpers.JsonSerialize(headerDictionary));
                if (Request != null)
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(Request);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
                HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
                using (StreamReader sr = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    responseFromServer = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();

                }
                httpWebResponse.Close();
                HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, "SendAPIRequestAsync Response TrnNo::" + TrnNo.ToString() + "::" +  responseFromServer); //ntrivedi 15-05-2019 adding Trnno in logging
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
            
            return responseFromServer;
        }

        public Task<string> SendRequestAsync(string Url, string Request="", string MethodType = "GET", string ContentType="application/json", WebHeaderCollection Headers = null, int Timeout = 9000,bool IsWrite = true)
        {
            string responseFromServer = "";
            try
            {
                try
                {
                    object ResponseObj = new object();
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);

                    httpWebRequest.Method = MethodType.ToUpper();
                    if (Headers != null)
                    {
                        httpWebRequest.Headers = Headers;
                    }
                    httpWebRequest.KeepAlive = false;
                    httpWebRequest.Timeout = Timeout;
                    httpWebRequest.ContentType = ContentType;
                  
                    if (IsWrite)
                    {
                        HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, Url);
                    }                    
                    if (Request != "")
                    {
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            streamWriter.Write(Request);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                    }

                    HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
                    using (StreamReader sr = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        responseFromServer = sr.ReadToEnd();
                        sr.Close();
                        sr.Dispose();
                    }
                    httpWebResponse.Close();
                    if (IsWrite)
                    {
                        HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, responseFromServer);
                    }
                    return Task.FromResult(responseFromServer);
                }
                catch (WebException webex)
                {
                    if (webex.Response != null)
                    {
                        WebResponse errResp = webex.Response;
                        Stream respStream = errResp.GetResponseStream();
                        StreamReader reader = new StreamReader(respStream);
                        string Text = reader.ReadToEnd();
                        if (Text.ToLower().Contains("code"))
                        {
                            responseFromServer = Text;
                        }
                        if (Text.ToLower().Contains("<html>"))
                        {
                            responseFromServer = Text;
                        }
                    }
                    else
                    {
                        responseFromServer = webex.Message;
                    }
                    webex = null;
                    return Task.FromResult(responseFromServer);
                }                
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }            
        }

        public Task<string> SendRequestAsyncLPArbitrage(string Url,ref short IsAPIProceed, string Request = "", string MethodType = "GET", string ContentType = "application/json", WebHeaderCollection Headers = null, int Timeout = 9000, long TrnNo = 0, bool IsWrite = true)
        {
            string responseFromServer = "";
            IsAPIProceed = 0;
            try
            {
                try
                {
                    object ResponseObj = new object();
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);

                    httpWebRequest.Method = MethodType.ToUpper();
                    if (Headers != null)
                    {
                        httpWebRequest.Headers = Headers;
                    }
                    httpWebRequest.KeepAlive = false;
                    httpWebRequest.Timeout = Timeout;
                    httpWebRequest.ContentType = ContentType;

                    if (IsWrite)
                    {
                        HelperForLog.WriteLogIntoFile("SendRequestAsyncLPArbitrage ##TrnNo:" + TrnNo, ControllerName, Url + " B:" + Request);
                    }
                    if (Request != "" && !string.IsNullOrEmpty(Request))
                    {
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            streamWriter.Write(Request);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                    }

                    HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
                    IsAPIProceed = 1;

                    using (StreamReader sr = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        responseFromServer = sr.ReadToEnd();
                        sr.Close();
                        sr.Dispose();
                    }
                    httpWebResponse.Close();
                    if (IsWrite)
                    {
                        HelperForLog.WriteLogIntoFile("SendRequestAsyncLPArbitrage ##TrnNo:" + TrnNo, ControllerName, "Response:" + responseFromServer);
                    }
                    return Task.FromResult(responseFromServer);
                }
                catch (WebException webex)
                {
                    if (IsWrite)
                    {
                        HelperForLog.WriteLogIntoFile("SendRequestAsyncLPArbitrage Internal Error ##TrnNo:" + TrnNo, ControllerName, webex.Message);
                    }

                    if (webex.Response != null)
                    {
                        WebResponse errResp = webex.Response;
                        Stream respStream = errResp.GetResponseStream();
                        StreamReader reader = new StreamReader(respStream);
                        string Text = reader.ReadToEnd();
                        if (Text.ToLower().Contains("code"))
                        {
                            responseFromServer = Text;
                        }
                        if (Text.ToLower().Contains("<html>"))
                        {
                            responseFromServer = Text;
                        }
                        if (Text.ToLower().Contains("status") || Text.ToLower().Contains("error"))
                        {                            
                            responseFromServer = Text;
                        }
                    }
                    else
                    {
                        responseFromServer = webex.Message;
                    }
                    webex = null;
                    if (IsWrite)
                    {
                        HelperForLog.WriteLogIntoFile("SendRequestAsyncLPArbitrage Internal Error Parse Response ##TrnNo:" + TrnNo, ControllerName, responseFromServer);
                    }
                    return Task.FromResult(responseFromServer);
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("SendRequestAsyncLPArbitrage ##TrnNo:" + TrnNo, ControllerName, ex);
                return null;
            }
        }

        public async Task<string> SendTCPSocketRequestAsync(string HostName, string Port, string request)
        {
            string responseFromServer = "";

            int read;
            byte[] buffer1 = new byte[2048];
            bool IsSocketCallDone = false;

            try
            {
                if (string.IsNullOrEmpty(HostName) || string.IsNullOrEmpty(Port))
                {
                    responseFromServer = "Configuration Not Found";
                    return responseFromServer;
                }
                IPAddress ipAddress = IPAddress.Parse(HostName);
                System.Net.Sockets.TcpClient client = new TcpClient();
                await client.ConnectAsync(ipAddress, Convert.ToInt32(Port));
                client.ReceiveTimeout = 61000;
                client.SendTimeout = 61000;
                NetworkStream networkStream = client.GetStream();
                StreamWriter writer = new StreamWriter(networkStream, Encoding.UTF8);

                writer.AutoFlush = true;
                await writer.WriteLineAsync(request);

                read = networkStream.Read(buffer1, 0, buffer1.Length);
                IsSocketCallDone = true;
                byte[] data = new byte[read];
                Array.Copy(buffer1, data, read);
                responseFromServer = Encoding.UTF8.GetString(data);

                networkStream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }

            return responseFromServer;
        }

        public string SendJsonRpcAPIRequestAsync(string Url,string RequestStr,WebHeaderCollection headerDictionary = null)
        {
            try
            {
                string WSResponse = "";
                try
                {
                    var myReqrpc = WebRequest.Create(Url);
                    if(headerDictionary!=null)
                    {
                        myReqrpc.Headers = headerDictionary;

                    }
                    myReqrpc.Method = "Post";
                    var sw = new StreamWriter(myReqrpc.GetRequestStream());
                    sw.Write(RequestStr);
                    sw.Close();

                    WebResponse response;
                    response = myReqrpc.GetResponse();

                    StreamReader StreamReader = new StreamReader(response.GetResponseStream());
                    WSResponse = StreamReader.ReadToEnd();
                    StreamReader.Close();
                    response.Close();

                    return WSResponse;
                }
                catch(WebException webex)
                {
                    if(webex.Response!=null)
                    {
                        WebResponse errResp = webex.Response;
                        Stream respStream = errResp.GetResponseStream();
                        StreamReader reader = new StreamReader(respStream);
                        string Text = reader.ReadToEnd();
                        if (Text.ToLower().Contains("code"))
                        {
                            WSResponse = Text;
                        }
                    }
                    else
                    {
                        WSResponse = webex.Message;
                    }
                    webex = null;

                    return WSResponse;
                }
                
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public string SendAPIRequestAsyncWallet(string Url, string Request, string ContentType, int Timeout = 180000, WebHeaderCollection headerDictionary = null, string MethodType = "POST")
        {
            string responseFromServer = "";
            try
            {
                object ResponseObj = new object();
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
                httpWebRequest.Method = MethodType.ToUpper();
                httpWebRequest.KeepAlive = false;
                httpWebRequest.Timeout = Timeout;
                httpWebRequest.Headers = headerDictionary;
                httpWebRequest.ContentType = ContentType;  //ntrivedi 11-12-2018 moving contenttype after the headers assgning otherwise content type is overwritten by headerDirectory
                HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, Url + "Request::" + Request);
                if (Request != null)
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(Request);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
                HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
                using (StreamReader sr = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    responseFromServer = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                }
                httpWebResponse.Close();
                HelperForLog.WriteLogIntoFile(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, responseFromServer);
                return responseFromServer;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
    }
}
