using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.SignalR;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Services;
using Worldex.Core.Services.RadisDatabase;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Interfaces;
using Worldex.Web.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Worldex.Web.Filters
{
    public class ResponseRewindMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ISignalRQueue _signalRQueue;
        private readonly IBasePage _basePage;
        private readonly UserManager<ApplicationUser> _userManager;
        private RedisConnectionFactory _fact;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public ResponseRewindMiddleware(RequestDelegate next, IBasePage basePage, UserManager<ApplicationUser> UserManager, RedisConnectionFactory Factory,
            ISignalRQueue signalRQueue, IConfiguration Configuration, IMediator mediator)
        {
            this.next = next;
            _basePage = basePage;
            _userManager = UserManager;
            _fact = Factory;
            _signalRQueue = signalRQueue;
            _configuration = Configuration;
            _mediator = mediator;
        }

        public async Task Invoke(HttpContext context)
        {
            Stream originalBody = context.Response.Body;
            SignalRUserConfiguration User = new SignalRUserConfiguration();
            try
            {
                Console.WriteLine("Current Log: " + "ResponseRewindMiddleware Start: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (context.Request.Path.ToString().ToLower() != "/api/kyc/personalverification" && context.Request.Path.ToString().ToLower() != "/api/backofficerolemanagement/getaccessrightsbyuserv1" && context.Request.Path.ToString().ToLower() != "/api/walletcontrolpanel/importaddress" && context.Request.Path.ToString().ToLower() != "/api/walletcontrolpanel/addcurrencylogo" && context.Request.Path.ToString().ToLower() != "/api/ieowalletcontrolpanel/insertupdatebannerconfiguration" && context.Request.Path.ToString().ToLower() != "/api/ieowalletcontrolpanel/insertupdateadminwalletconfiguration")
                {
                    try
                    {
                        using (var memStream = new MemoryStream())
                        {
                            context.Response.Body = memStream;
                            Console.WriteLine("Current Log: " + "ResponseRewindMiddleware Before: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            await next(context);
                            Console.WriteLine("Current Log: " + "ResponseRewindMiddleware After: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            var responseLog = $"RESPONSE Host:{context.Request.Host}, HttpMethod: {context.Request.Method}, Path: {context.Request.Path}";


                            if (context.Request.Path == "/connect/token")
                            {
                                memStream.Seek(0, SeekOrigin.Begin);
                                var content = new StreamReader(memStream).ReadToEnd();
                                memStream.Seek(0, SeekOrigin.Begin);
                                tokanreponsmodel TokenData = JsonConvert.DeserializeObject<tokanreponsmodel>(content);
                                if (TokenData != null && !string.IsNullOrEmpty(TokenData.access_token))
                                {
                                    using (var bodyReader = new StreamReader(context.Request.Body))
                                    {
                                        var bodyAsText = bodyReader.ReadToEnd();
                                        if (string.IsNullOrWhiteSpace(bodyAsText) == false)
                                        {
                                            User = convertStirngToJson(bodyAsText); // khushali 17-01-2019 issue '&' replaceing on request with null because of password not accepted with '&'                                                                               
                                        }

                                        if (User != null && !string.IsNullOrEmpty(User.username))
                                        {
                                            try
                                            {
                                                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                                                var Userdata = _userManager.FindByNameAsync(User.username).GetAwaiter().GetResult();
                                                if (Userdata != null && Userdata.Id != 0)
                                                {
                                                    string RedisTokenKey = _configuration.GetValue<string>("SignalRKey:RedisToken");
                                                    string oldRefreshToken = Redis.GetHashData(RedisTokenKey + Userdata.Id, "Token"); // khushali 13-02-2019 get  key name from appsetting
                                                    string newRefreshtoken = TokenData.refresh_token;
                                                    ActivityNotificationMessage LogoutActivity = new ActivityNotificationMessage();
                                                    LogoutActivity.MsgCode = Convert.ToInt32(enErrorCode.SessionExpired);
                                                    SignalRComm<ActivityNotificationMessage> CommonData = new SignalRComm<ActivityNotificationMessage>();
                                                    CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                                                    CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.SessionExpired);
                                                    CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.ReceiveSessionExpired);
                                                    CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                                                    CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                                                    CommonData.Data = LogoutActivity;
                                                    CommonData.Parameter = null;

                                                    SignalRData SendData = new SignalRData();
                                                    SendData.Method = enMethodName.SessionExpired;
                                                    SendData.Parameter = oldRefreshToken;
                                                    SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                                                    //Task.Run(() => _signalRQueue.Enqueue(SendData));
                                                    await _mediator.Send(SendData);

                                                    Redis.SaveWithOrigionalKey(RedisTokenKey + Userdata.Id, new ConnetedClientToken { Token = TokenData.refresh_token }, TokenData.refresh_token); // khushali 13-02-2019 get  key name from appsetting                                                                                                                                              //HelperForLog.WriteLogIntoFile(2, _basePage.UTC_To_IST(), context.Request.Path.ToString() + "refresh_token", context.Request.Path.ToString(), AccessToken);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                HelperForLog.WriteErrorLog(_basePage.UTC_To_IST(), "Redis Connection failed", "Direct login", ex.ToString());
                                            }
                                        }
                                    }
                                }
                            }

                            memStream.Position = 0;
                            string responseBody = new StreamReader(memStream).ReadToEnd();
                            var erParams = (dynamic)null;
                            if (responseBody.Contains("ReturnCode"))
                                erParams = JsonConvert.DeserializeObject<ErrorParams>(responseBody);

                            responseLog += $", Response : {responseBody}";


                            //Uday 05-11-2018 don't write log for graph method
                            if (context.Request.Path.Value.Split("/")[1] != "swagger" && !context.Request.Path.Value.Contains("GetGraphDetail"))
                            {
                                if (erParams?.ReturnCode != 9)
                                    HelperForLog.WriteLogIntoFile(2, _basePage.UTC_To_IST(), context.Request.Path.ToString(), context.Request.Path.ToString(), responseLog);
                            }
                            memStream.Position = 0;
                            await memStream.CopyToAsync(originalBody);
                        }

                    }
                    catch (Exception ex)
                    {
                        HelperForLog.WriteErrorLog(_basePage.UTC_To_IST(), "", "", ex.ToString());
                    }
                    finally
                    {
                        Console.WriteLine("Current Log: " + "ResponseRewindMiddleware Finally: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        context.Response.Body = originalBody;
                    }
                }
                else
                {
                    Console.WriteLine("Current Log: " + "ResponseRewindMiddleware Else: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    await next(context);
                    context.Response.Body = originalBody;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(_basePage.UTC_To_IST(), "ResponseRewindMiddleware", "ResponseRewindMiddleware", ex.ToString());
            }
        }

        public SignalRUserConfiguration convertStirngToJson(string Data)
        {
            Data = Data.Replace("=", "\":\"");
            Data = Data.Replace("&", "\",\"");
            Data = "{\"" + Data + "\"}";
            Data = HttpUtility.UrlDecode(Data); // khushali 17-01-2019 issue '&' replaceing on request with null because of password not accepted with '&'
            SignalRUserConfiguration obj = JsonConvert.DeserializeObject<SignalRUserConfiguration>(Data);
            return obj;
        }
    }

    public class ErrorParams
    {
        public long ReturnCode { get; set; }
        public string ReturnMsg { get; set; }
        public int ErrorCode { get; set; }
    }
}
