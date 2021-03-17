using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.SignalR;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Interfaces.Organization;
using Worldex.Core.Services;
using Worldex.Core.Services.RadisDatabase;
using Worldex.Core.ViewModels.Organization;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Worldex.Web.Helper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Configuration;
using Worldex.Core.Interfaces.Log;

namespace Worldex.Web.Filters
{
    public class ResponseRewindMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ISignalRQueue _signalRQueue;
        private readonly IBasePage _basePage;
        private readonly UserManager<ApplicationUser> _userManager;
        private RedisConnectionFactory _fact;
        //private readonly IActivityRegister _IactivityRegister;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        //private readonly IActivityLog _activityLog;
        public ResponseRewindMiddleware(RequestDelegate next, IBasePage basePage, UserManager<ApplicationUser> UserManager, RedisConnectionFactory Factory,
            //IActivityRegister IactivityRegister,
            IMediator mediator, ISignalRQueue signalRQueue, IConfiguration Configuration) //, IActivityLog activityLog)
        {
            this.next = next;
            _basePage = basePage;
            _userManager = UserManager;
            _fact = Factory;
            //_IactivityRegister = IactivityRegister;
            _mediator = mediator;
            _signalRQueue = signalRQueue;
            _configuration = Configuration;
            //_activityLog = activityLog;
        }

        public async Task Invoke(HttpContext context)
        {
            Stream originalBody = context.Response.Body;
            SignalRUserConfiguration User = new SignalRUserConfiguration();
            try
            {
                if (context.Request.Path.ToString().ToLower() != "/api/kyc/personalverification" && context.Request.Path.ToString().ToLower() != "/api/backofficerolemanagement/getaccessrightsbyuserv1" && context.Request.Path.ToString().ToLower() != "/api/walletcontrolpanel/importaddress" && context.Request.Path.ToString().ToLower() != "/api/walletcontrolpanel/addcurrencylogo")
                {
                try
                {
                    using (var memStream = new MemoryStream())
                    {
                        var newBody = new MemoryStream();
                        var newContent = string.Empty;
                        var accessToken = await context.Response.HttpContext.GetTokenAsync("access_token");
                        context.Response.Body = memStream;
                        Guid RequestId = Guid.Empty;

                        if (context.Items.Count > 0)
                            //RequestId = (Guid)context.Items[accessToken];
                            RequestId = (Guid)context.Items["ActivityID"];

                        await next(context);
                        long UserId = 0;
                        try
                        {
                            if (context.Items.Count > 2)
                                UserId = context.Items[accessToken] != null ? Convert.ToInt64(context.Items[accessToken]) : 0;
                        }
                        catch (Exception ex)
                        {
                            goto a;
                        }
                        a:
                        try
                        {
                            if (context.Items.Count > 2 && UserId == 0)
                            {
                                UserId = context.Items["UserId"] != null ? Convert.ToInt64(context.Items["UserId"]) : 0;
                                //if (context.Items["LogId"] != null && UserId != 0)
                                //{
                                //    long LogId = Convert.ToInt64(context.Items["LogId"]);
                                //    _activityLog.UpdateActivityLog(LogId, UserId);
                                //}
                            }
                        }
                        catch (Exception ex)
                        {
                            goto b;
                        }

                        b:
                        var responseLog = $"RESPONSE Host:{context.Request.Host}, HttpMethod: {context.Request.Method}, Path: {context.Request.Path}";

                        //string ResponsePath = context.Request.Path.ToString();
                        //string[] ResonseDetails = ResponsePath.Split("/");
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
                                        //User = convertStirngToJson(HttpUtility.UrlDecode(bodyAsText));
                                        User = convertStirngToJson(bodyAsText); // khushali 17-01-2019 issue '&' replaceing on request with null because of password not accepted with '&'
                                                                                // User.username = "user@user.com";
                                    }
                                    //if (User.grant_type == "refresh_token")
                                    //{
                                    //    var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                                    //    var key = Redis.GetPairOrMarketData(User.refresh_token, ":", "Tokens");
                                    //    Redis.SaveWithOrigionalKey("Tokens:" + key, new ConnetedClientToken { Token = TokenData.access_token }, User.refresh_token);

                                    //}
                                    //else
                                    //{
                                    if (User != null && !string.IsNullOrEmpty(User.username))
                                    {
                                        try
                                        {
                                            var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                                            var Userdata = _userManager.FindByNameAsync(User.username).GetAwaiter().GetResult();
                                            if (Userdata != null && Userdata.Id != 0)
                                            {
                                                //string oldRefreshToken = Redis.GetHashData("Tokens:" + Userdata.Id, "Token");
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
                                                
                                                //SignalRDataNotify SendData = new SignalRDataNotify();
                                                SignalRData SendData = new SignalRData();
                                                SendData.Method = enMethodName.SessionExpired;
                                                SendData.Parameter = oldRefreshToken;
                                                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                                                Task.Run(() => _signalRQueue.Enqueue(SendData));
                                                //Redis.SaveWithOrigionalKey("Tokens:" + Userdata.Id, new ConnetedClientToken { Token = TokenData.refresh_token }, TokenData.refresh_token);
                                                Redis.SaveWithOrigionalKey(RedisTokenKey + Userdata.Id, new ConnetedClientToken { Token = TokenData.refresh_token }, TokenData.refresh_token); // khushali 13-02-2019 get  key name from appsetting                                                                                                                                              //HelperForLog.WriteLogIntoFile(2, _basePage.UTC_To_IST(), context.Request.Path.ToString() + "refresh_token", context.Request.Path.ToString(), AccessToken);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            HelperForLog.WriteErrorLog(_basePage.UTC_To_IST(), "Redis Connection failed", "Direct login", ex.ToString());
                                        }                                        
                                    }
                                        //}
                                }
                            }
                        }

                        memStream.Position = 0;
                        string responseBody = new StreamReader(memStream).ReadToEnd();
                        var erParams = (dynamic)null;
                        if (responseBody.Contains("ReturnCode"))
                            erParams = JsonConvert.DeserializeObject<ErrorParams>(responseBody);

                        responseLog += $", Response : {responseBody}";

                        if (RequestId != Guid.Empty && erParams != null)
                        {
                            //ActivityRegisterViewModel model = new ActivityRegisterViewModel();
                            //model.ErrorCode = erParams?.ReturnCode;
                            //model.ReturnCode = erParams?.ErrorCode;
                            //model.ReturnMsg = erParams?.ReturnMsg;
                            //model.StatusCode = context.Response.StatusCode;
                            //model.Id = RequestId;
                            //ActivityRegisterDetViewModel modeldet = new ActivityRegisterDetViewModel();
                            //modeldet.Response = responseLog;
                            //modeldet.ActivityId = RequestId;

                            ActivityRes model = new ActivityRes();
                            model.ErrorCode = erParams?.ReturnCode;
                            model.ReturnCode = erParams?.ErrorCode;
                            model.ReturnMsg = erParams?.ReturnMsg;
                            model.StatusCode = context.Response.StatusCode;
                            model.Response = responseLog;
                            model.ActivityId = RequestId;
                            model.Id = RequestId;
                            model.CreatedBy = UserId;
                            Task.Run(() => _mediator.Send(model));
                            //Task.Run(() => _IactivityRegister.UpdateActivityLog(model, modeldet));
                        }


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
                    context.Response.Body = originalBody;
                }
                }
                else
                {
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
            //string str = "clientId=Worldex&grant_type=password&username=user@user.com&password=P@ssw0rd!&scope=openid profile email offline_access client_id roles phone";
            Data = Data.Replace("=", "\":\"");
            Data = Data.Replace("&", "\",\"");
            Data = "{\"" + Data + "\"}";
            Data = HttpUtility. UrlDecode(Data); // khushali 17-01-2019 issue '&' replaceing on request with null because of password not accepted with '&'
            SignalRUserConfiguration obj = JsonConvert.DeserializeObject<SignalRUserConfiguration>(Data);
            return obj;
            //var jsonData = JsonConvert.SerializeObject(obj);
        }
    }

    public class ErrorParams
    {
        public long ReturnCode { get; set; }
        public string ReturnMsg { get; set; }
        public int ErrorCode { get; set; }
    }
}
