using Worldex.Infrastructure.Interfaces;
using Worldex.Web.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Web.Filters
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IBasePage _basePage;
        
        public RequestResponseLoggingMiddleware(RequestDelegate next, IBasePage basePage)
        {
            _next = next;
            _basePage = basePage;            
        }

        public async Task Invoke(HttpContext context)
        {
            var injectedRequestStream = new MemoryStream();
            try
            {
                Console.WriteLine("Current Log: " + "RequestResponseLoggingMiddleware Start: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (context.Request.Path.ToString().ToLower() != "/api/kyc/personalverification" && context.Request.Path.ToString().ToLower() != "/api/walletcontrolpanel/importaddress" && context.Request.Path.ToString().ToLower() != "/api/walletcontrolpanel/addcurrencylogo" && context.Request.Path.ToString().ToLower() != "/api/ieowalletcontrolpanel/insertupdatebannerconfiguration" && context.Request.Path.ToString().ToLower() != "/api/ieowalletcontrolpanel/insertupdateadminwalletconfiguration")
                {
                    try
                    {
                        var request = (dynamic)null;
                        var requestLog =
                        $"REQUEST Host:{context.Request.Host},IpAddress:{context.Connection.RemoteIpAddress} ,HttpMethod: {context.Request.Method}, Path: {context.Request.Path}";
                        var bodyAsText = (dynamic)null;
                        var accessToken = await context.Request.HttpContext.GetTokenAsync("access_token");
                        using (var bodyReader = new StreamReader(context.Request.Body))
                        {
                            bodyAsText = bodyReader.ReadToEnd();
                            if (string.IsNullOrWhiteSpace(bodyAsText) == false)
                            {
                                requestLog += $", Body : {bodyAsText}";
                                request = $" { bodyAsText}";
                            }

                            var bytesToWrite = Encoding.UTF8.GetBytes(bodyAsText);
                            injectedRequestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                            injectedRequestStream.Seek(0, SeekOrigin.Begin);
                            context.Request.Body = injectedRequestStream;
                        }
                        if (context.Request.Path.ToString().ToLower() != "/market" && context.Request.Path.ToString().ToLower() != "/market/negotiate" && context.Request.Path.ToString().ToLower() != "/chat/negotiate" && context.Request.Path.ToString().ToLower() != "/chat" &&
                            context.Request.Path.ToString().ToLower() != "/api/kyc/personalverification")
                        {
                            if (context.Request.Path.Value.Split("/")[1] != "swagger")
                                HelperForLog.WriteLogIntoFile(1, _basePage.UTC_To_IST(), context.Request.Path.ToString(), context.Request.Path.ToString(), requestLog, accessToken);
                        }
                        Console.WriteLine("Current Log: " + "RequestResponseLoggingMiddleware GoTo Next: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        await _next.Invoke(context);
                        Console.WriteLine("Current Log: " + "RequestResponseLoggingMiddleware Complete: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    }
                    catch (Exception ex)
                    {
                        HelperForLog.WriteErrorLog(_basePage.UTC_To_IST(), "", "", ex.ToString());
                    }
                    finally
                    {
                        Console.WriteLine("Current Log: " + "RequestResponseLoggingMiddleware Finally: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        injectedRequestStream.Dispose();
                    }
                }
                else
                    await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(_basePage.UTC_To_IST(), "", "", ex.ToString());
            }
        }
    }    
}
