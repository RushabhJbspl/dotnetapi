using Worldex.Core.Entities.Complaint;
using Worldex.Core.Entities.Log;
using Worldex.Core.Entities.Organization;
using Worldex.Core.Entities.User;
using Worldex.Core.Interfaces.Log;
using Worldex.Core.Interfaces.Organization;
using Worldex.Core.Interfaces.User;
using Worldex.Core.ViewModels.AccountViewModels.Log;
using Worldex.Core.ViewModels.Organization;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Worldex.Web.Helper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Web.Filters
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IBasePage _basePage;
        private readonly IActivityMasterConfiguration _IactivityMasterConfiguration;
        //private readonly IActivityRegister _IactivityRegister;
        ActivityType_Master _ActivityTypeMasterService=new ActivityType_Master();
        HostURLMaster _HostURLMasterService;
        Typemaster _TypemasterService;
        private readonly IMediator _mediator;
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IUserService _userService;
        //private readonly IActivityLog _activityLog;

        public RequestResponseLoggingMiddleware(RequestDelegate next, IBasePage basePage, IActivityMasterConfiguration IactivityMasterConfiguration,
            //IActivityRegister IactivityRegister, 
            IMediator mediator ) //, UserManager<ApplicationUser> userManager, IUserService userService, IActivityLog activityLog)
        {
            _next = next;
            _basePage = basePage;
            _IactivityMasterConfiguration = IactivityMasterConfiguration;
            //_IactivityRegister = IactivityRegister;
            _mediator = mediator;
            //_userManager = userManager;
            //_userService = userService;
            //_activityLog = activityLog;
        }

        public async Task Invoke(HttpContext context)
        {
            var injectedRequestStream = new MemoryStream();
            try
            {
               
                
                if (context.Request.Path.ToString().ToLower() != "/api/kyc/personalverification" && context.Request.Path.ToString().ToLower() != "/api/walletcontrolpanel/importaddress" && context.Request.Path.ToString().ToLower() != "/api/walletcontrolpanel/addcurrencylogo")
                {
              
                try
                {
                    var request = (dynamic)null;
                    var requestLog =
                    $"REQUEST Host:{context.Request.Host} ,HttpMethod: {context.Request.Method}, Path: {context.Request.Path}";
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
                        var TrackerData = (dynamic)null;
                        try
                        {
                            if (bodyAsText.Contains("DeviceId") && bodyAsText.Contains("IPAddress"))
                                TrackerData = JsonConvert.DeserializeObject<TrackerData>(bodyAsText);
                        }
                        catch (Exception)
                        {
                            goto a;
                        }
                        a:
                        string[] SubType;
                        if (context.Request.Path.ToString().Contains("/"))
                            SubType = context.Request.Path.ToString().Split("/");
                        else
                            SubType = null;
                        _ActivityTypeMasterService = _IactivityMasterConfiguration.GetActivityTypeData().Where(i => i.TypeMaster == context.Request.Path.ToString()).FirstOrDefault();
                        _HostURLMasterService = _IactivityMasterConfiguration.GetHostURLData().Where(i => i.HostURL == context.Request.Host.ToString()).FirstOrDefault();
                        if (SubType.Length >= 2)
                        {
                            try
                            {
                                if (SubType[2] != null)
                                    _TypemasterService = _IactivityMasterConfiguration.GetTypeMasterData().Where(i => i.SubType == SubType[2].ToString()).FirstOrDefault();
                            }
                            catch (Exception ex)
                            {
                                goto c;
                            }
                        }
                        c:



                        ActivityReqRes model = new ActivityReqRes();
                        model.Id = Guid.NewGuid();
                        model.Remark = string.Empty;
                        model.Connection = string.Empty;
                        model.ApplicationId = Guid.Empty;
                        model.Channel = context.Request.Host.ToString();
                        model.DeviceId = TrackerData != null ? TrackerData?.DeviceId : string.Empty;
                        model.IPAddress = TrackerData != null ? TrackerData?.IPAddress : string.Empty;
                        model.ActivityType = context.Request.Path.ToString();
                        model.Session = string.Empty;
                        model.AccessToken = accessToken;
                        model.AliasName = _ActivityTypeMasterService != null ? _ActivityTypeMasterService?.AliasName : string.Empty;
                        model.ModuleTypeName = _TypemasterService != null ? _TypemasterService.SubType : string.Empty;
                        model.HostURLName = context.Request.Host.ToString();

                        //ActivityRegisterDetViewModel modeldet = new ActivityRegisterDetViewModel();
                        model.ActivityDetId = Guid.NewGuid();
                        model.Request = request;
                        // store accesstoken base guid on 12-27-2018
                        context.Items["ActivityID"] = model.Id;
                        //_IactivityRegister.AddActivityLog(model, modeldet);
                        //Task.Run(() => _IactivityRegister.AddActivityLog(model, modeldet));
                        Task.Run(() => _mediator.Send(model));
                      
                        //if (_ActivityTypeMasterService != null)
                        //{
                        //    ActivityLogViewModel objLog = new ActivityLogViewModel();
                        //    objLog.Device = TrackerData != null ? TrackerData?.DeviceId : model.DeviceId;
                        //    objLog.IpAddress = TrackerData != null ? TrackerData?.IPAddress : model.IPAddress;
                        //    objLog.Mode = TrackerData != null ? TrackerData?.Mode : string.Empty;
                        //    objLog.HostName = context.Request.Path.ToString();
                        //    objLog.Action = context.Request.Method.ToString();
                        //    var Ipwisedata = await _userService.GetIPWiseData(objLog.IpAddress);
                        //    objLog.Location = Ipwisedata != null ? Ipwisedata.CountryCode + "," + Ipwisedata.Location : string.Empty;
                        //    context.Items["LogId"] = _activityLog.AddActivityLog(objLog);
                        //}
                    }
                    if (context.Request.Path.Value.Split("/")[1] != "swagger")
                        HelperForLog.WriteLogIntoFile(1, _basePage.UTC_To_IST(), context.Request.Path.ToString(), context.Request.Path.ToString(), requestLog, accessToken);

                    await _next.Invoke(context);
                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog(_basePage.UTC_To_IST(), "", "", ex.ToString());
                }
                finally
                {
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

    public class TrackerData
    {
        public string DeviceId { get; set; }
        public string Mode { get; set; }
        public string IPAddress { get; set; }
        public string HostName { get; set; }
    }
}
