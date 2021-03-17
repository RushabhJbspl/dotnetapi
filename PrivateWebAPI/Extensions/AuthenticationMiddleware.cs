using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.APIConfiguration;
using Worldex.Web.Filters;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Buffers.Text;

namespace Worldex.Web.Extensions
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private ICommonRepository<UserAPIKeyDetails> _UserAPIKeyDetailsRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private ICommonRepository<APIPlanMaster> _APIPlanMasterRepository;
        private ICommonRepository<APIPlanDetail> _APIPlanDetailRepository;
        private ICommonRepository<RestMethods> _RestMethodsRepository;
        private ICommonRepository<WhiteListIPEndPoint> _WhiteListIPEndPointRepository;
        private ICommonRepository<UserAPILimitCount> _UserAPILimitCountRepository;
        private ICommonRepository<UserSubscribeAPIPlan> _SubScribePlanRepository;
        private ICommonRepository<APIKeyWhitelistIPConfig> _APIKeyWhitelistIPConfigRepository;
        private ICommonRepository<APIPlanMethodConfiguration> _APIPlanMethodConfigurationRepository;
        private ICommonRepository<APIMethodConfiguration> _APIMethodConfiguration;
        private IMemoryCache _cache { get; set; }
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IMediator _mediator;
        public static string MethodList = "";

        public AuthenticationMiddleware(RequestDelegate next, ICommonRepository<UserAPIKeyDetails> UserAPIKeyDetailsRepository,
            UserManager<ApplicationUser> userManager, ICommonRepository<APIPlanMaster> APIPlanMasterRepository, ICommonRepository<APIPlanDetail> APIPlanDetailRepository,
            ICommonRepository<WhiteListIPEndPoint> WhiteListIPEndPointRepository, IMemoryCache cache, ICommonRepository<UserAPILimitCount> UserAPILimitCountRepository,
            ICommonRepository<UserSubscribeAPIPlan> SubScribePlanRepository, ICommonRepository<APIKeyWhitelistIPConfig> APIKeyWhitelistIPConfigRepository,
            IMediator mediator, ICommonRepository<APIPlanMethodConfiguration> APIPlanMethodConfigurationRepository, ICommonRepository<RestMethods> RestMethodsRepository,
            ICommonRepository<APIMethodConfiguration> APIMethodConfiguration, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _next = next;
            _UserAPIKeyDetailsRepository = UserAPIKeyDetailsRepository;
            _APIMethodConfiguration = APIMethodConfiguration;
            _userManager = userManager;
            _RestMethodsRepository = RestMethodsRepository;
            _APIPlanMasterRepository = APIPlanMasterRepository;
            _APIPlanDetailRepository = APIPlanDetailRepository;
            _WhiteListIPEndPointRepository = WhiteListIPEndPointRepository;
            _UserAPILimitCountRepository = UserAPILimitCountRepository;
            _SubScribePlanRepository = SubScribePlanRepository;
            _APIKeyWhitelistIPConfigRepository = APIKeyWhitelistIPConfigRepository;
            _APIPlanMethodConfigurationRepository = APIPlanMethodConfigurationRepository;
            _cache = cache;
            _mediator = mediator;
            _configuration = configuration;
            
                var restMethods = _RestMethodsRepository.List();
                _cache.Set("restMethods", _RestMethodsRepository.List());
                var methods = Array.ConvertAll<RestMethods, string>(restMethods.ToArray(), x => x.Path);
                MethodList = string.Join(",", methods);
            
        }

        public async Task Invoke(HttpContext context)
        {
            APIStatistics model = new APIStatistics();
            List<APIPlanMethodConfiguration> PlanMethodList = null;
            //if(context.Request.Path.Value.ToString().ToLower() == "/swagger/index.html" || context.Request.Path.Value.ToString().ToLower() == "/swagger/v1/swagger.json")
            if(context.Request.Path.Value.ToString().ToLower().Contains("swagger") || context.Request.Path.Value.ToString().ToLower() == "/connect/token")
            {
                await _next.Invoke(context);
            }
            else
            {
                
                var path = context.Request.Path.Value.ToString().Split("/");
                var pathNew = "/" + path[1] + "/" + path[2] + "/" + path[3];
                if (MethodList.Contains(pathNew))
                {
                    long MinLimit = 0, MaxResSize = 0, DayLimit = 0, MonthLimit = 0, MaxReqSize = 0;
                    string ApiKey = context.Request.Headers["X-MJBSPLX-APIKEY"];
                    string SecretKey = context.Request.Headers["X-MJBSPLX-SECURITYKEY"];
                    string TimeStamp = context.Request.Headers["TimeStamp"];
                    string Autho = context.Request.Headers["Authorization"];
                    string IPAddress = context.Request.Headers["IPEndPoint"];
                    string expectedSignature = context.Request.Headers["X-Hub-Signature"];

                    if (ApiKey != null && SecretKey != null && TimeStamp != null && Autho != null && Autho.StartsWith("Bearer") && TimeStamp.Length == 10)
                    {
                        HelperForLog.WriteLogForActivity("AuthenticationMiddleware ", "AuthenticationMiddleware ", "Method : " + pathNew);
                        DateTime CurTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToDouble(TimeStamp));
                        
                        var mintime = DateTime.UtcNow.AddSeconds(-7);
                        var maxtime = DateTime.UtcNow.AddSeconds(3);
                        if (!(CurTime > mintime && CurTime < maxtime))
                        {
                            context.Response.StatusCode = 416; //416 Requested Range Not Satisfiable
                            //return;
                        }

                        //khushali 14-03-2019 Add log for APIStatistics and  PublicAPIReqResLog
                        if (HttpMethods.IsPost(context.Request.Method))
                        {
                            
                            var bodyReader = context.Request.Body;
                            byte[] buffer = new byte[4096];
                            int read = bodyReader.ReadAsync(buffer, 0, buffer.Length).Result;
                            byte[] data = new byte[read];
                            Array.Copy(buffer, data, read);
                            Stream stream = new MemoryStream(data);
                            context.Request.Body = stream;

                            //string actualSignature = CreateSignature(ApiKey, SecretKey, context.Request.Host.Value.ToString()+ "/PrivateAPI/" + context.Request.Path.Value.ToString(), data, TimeStamp);
                            string actualSignature = CreateSignature(ApiKey, SecretKey, _configuration["PrivateAPIHostPath"] + context.Request.Path.Value.ToString(), data, TimeStamp);
                            HelperForLog.WriteLogForActivity("AuthenticationMiddleware ", "AuthenticationMiddleware ", "actualSignature HMAC256 : " + actualSignature);
                            HelperForLog.WriteLogForActivity("AuthenticationMiddleware ", "AuthenticationMiddleware ", "expectedSignature HMAC256 : " + expectedSignature);
                            if (actualSignature == null)
                            {
                                context.Response.StatusCode = 416; //Unauthorized
                                return;
                            }
                            if (!Equals(expectedSignature, actualSignature))
                            {
                                context.Response.StatusCode = 401; //Unauthorized
                                return;
                            }
                        }

                        model.Path = pathNew;
                        model.MethodType = context.Request.Method;
                        model.Host = context.Request.Host.Value.ToString();

                        
                        var keyDetails = _UserAPIKeyDetailsRepository.FindBy(e => e.SecretKey == SecretKey && e.APIKey == ApiKey && e.Status == 1).FirstOrDefault();
                        if (keyDetails == null)
                        {
                            context.Response.StatusCode = 401; //416 Requested Range Not Satisfiable
                            model.HTTPErrorCode = 401;
                            Task.Run(() => _mediator.Send(model));
                            return;
                        }
                        context.Items.Add("APIUserID", keyDetails.UserID.ToString());
                        model.IPAddress = IPAddress;
                        model.WhitelistIP = keyDetails.IPAccess;
                        model.UserID = Convert.ToInt64(context.Items["APIUserID"].ToString());
                        model.IsSuccessFaliure = 1;
                        //ApplicationUser user = await _userManager.FindByIdAsync(keyDetails.UserID.ToString());
                        if (keyDetails.IPAccess == 1)
                        {
                            if (IPAddress == null)
                            {
                                context.Response.StatusCode = 416; //416 Requested Range Not Satisfiable
                                model.HTTPErrorCode = 416;
                                Task.Run(() => _mediator.Send(model));
                                return;
                            }
                            List<long> KeyIPConfig = _APIKeyWhitelistIPConfigRepository.FindBy(e => e.APIKeyID == keyDetails.Id && e.Status == 1 ).Select(e => e.IPId).ToList();

                            foreach (long obj in KeyIPConfig)
                            {
                                var ip = _WhiteListIPEndPointRepository.FindBy(e => e.Id == obj && e.Status == 1 && e.IPType==(short)enIPType.Whitelist).FirstOrDefault();
                                if (ip.IPAddress == IPAddress)
                                    goto a;

                                context.Response.StatusCode = 401; //416 Requested Range Not Satisfiable
                                model.HTTPErrorCode = 401;
                                Task.Run(() => _mediator.Send(model));
                                return;
                            }
                        }
                        a:
                        UserSubscribeAPIPlan history = _SubScribePlanRepository.FindBy(e => e.UserID == keyDetails.UserID && e.Status == 1 && e.APIPlanMasterID == keyDetails.APIPlanMasterID).FirstOrDefault();
                        if (history == null)
                        {
                            context.Response.StatusCode = 401; //Unauthorized
                            model.HTTPErrorCode = 401;
                            Task.Run(() => _mediator.Send(model));
                            return;
                        }

                        var restMethodObj = _RestMethodsRepository.FindBy(e => e.Path.Contains(pathNew)).SingleOrDefault();
                        if (restMethodObj == null)
                        {
                            context.Response.StatusCode = 401; //Unauthorized
                            model.HTTPErrorCode = 401;
                            Task.Run(() => _mediator.Send(model));
                            return;
                        }
                        var Methodid = restMethodObj.Id;
                        if (history.CustomeLimitId == 0)
                        {
                            APIPlanMaster limitObj = _APIPlanMasterRepository.FindBy(e => e.Id == keyDetails.APIPlanMasterID).FirstOrDefault();
                            PlanMethodList = _APIPlanMethodConfigurationRepository.FindBy(e => e.APIPlanMasterID == keyDetails.APIPlanMasterID && e.CustomeLimitId == 0 && e.Status == 1).ToList();
                            MinLimit = limitObj.MaxPerMinute; DayLimit = limitObj.MaxPerDay; MonthLimit = limitObj.MaxPerMonth; MaxResSize = limitObj.MaxResSize; MaxReqSize = limitObj.MaxReqSize;
                        }
                        else
                        {
                            APIPlanDetail limitObj = _APIPlanDetailRepository.FindBy(e => e.Id == history.CustomeLimitId).FirstOrDefault();
                            PlanMethodList = _APIPlanMethodConfigurationRepository.FindBy(e => e.APIPlanMasterID == keyDetails.APIPlanMasterID && e.CustomeLimitId == history.CustomeLimitId && e.Status == 1).ToList();
                            MinLimit = limitObj.MaxPerMinute; DayLimit = limitObj.MaxPerDay; MonthLimit = limitObj.MaxPerMonth; MaxResSize = limitObj.MaxResSize; MaxReqSize = limitObj.MaxReqSize;
                        }
                        if (PlanMethodList.Count == 0)
                        {
                            context.Response.StatusCode = 405; //405 Method Not Allowed
                            model.HTTPErrorCode = 405;
                            Task.Run(() => _mediator.Send(model));
                            return;
                        }
                        var KeyArr = Array.ConvertAll<APIPlanMethodConfiguration, long>(PlanMethodList.ToArray(), x => (long)x.RestMethodID);
                        var ParentID = _APIMethodConfiguration.FindBy(e => e.MethodID == Methodid && e.MethodType == 1 && e.Status == 1).ToList();
                        if (ParentID.Count == 0)
                        {
                            context.Response.StatusCode = 405; //405 Method Not Allowed
                            model.HTTPErrorCode = 405;
                            Task.Run(() => _mediator.Send(model));
                            return;
                        }
                        var MethodConfig = Array.ConvertAll<APIMethodConfiguration, long>(ParentID.ToArray(), x => (long)x.ParentID);
                        if (!KeyArr.Intersect(MethodConfig).Any())
                        {
                            context.Response.StatusCode = 405; //405 Method Not Allowed
                            model.HTTPErrorCode = 405;
                            Task.Run(() => _mediator.Send(model));
                            return;
                        }
                        var ReqDataSize = System.Text.ASCIIEncoding.ASCII.GetByteCount(context.Request.Body.ToString());
                        if (ReqDataSize > MaxResSize)
                        {
                            context.Response.StatusCode = 429; //Unauthorized
                            model.HTTPErrorCode = 429;
                            Task.Run(() => _mediator.Send(model));
                            return;
                        }
                        //var userLimit= aPILimitCounts.Find(e=>e.PlanID== keyDetails.APIPlanMasterID && e.UserID==keyDetails.UserID);/from cache
                        var userLimit = _UserAPILimitCountRepository.FindBy(e => e.PlanID == keyDetails.APIPlanMasterID && e.UserID == keyDetails.UserID && e.Status == 1).FirstOrDefault();

                        if (userLimit != null)
                        {

                            if (userLimit.PerMinCount > MinLimit || userLimit.PerDayCount > DayLimit || userLimit.PerMonthCount > MonthLimit)
                            {
                                if ((Helpers.UTC_To_IST().AddMinutes(-1) - userLimit.PerMinUpdatedDate).TotalSeconds >= 1)
                                    userLimit.PerMinUpdatedDate = DateTime.UtcNow; userLimit.PerMinCount = 0;
                                if ((Helpers.UTC_To_IST().AddDays(-1) - userLimit.PerMinUpdatedDate).TotalSeconds >= 1)
                                    userLimit.PerDayUpdatedDate = DateTime.UtcNow; userLimit.PerDayCount = 0;
                                if ((Helpers.UTC_To_IST().AddMonths(-1) - userLimit.PerMonthUpdatedDate).TotalSeconds >= 1)
                                    userLimit.PerMonthUpdatedDate = DateTime.UtcNow; userLimit.PerMonthCount = 0;
                                _UserAPILimitCountRepository.Update(userLimit);
                                context.Response.StatusCode = 429; //Unauthorized
                                return;
                            }
                            userLimit.PerMinCount += 1;
                            userLimit.PerDayCount += 1;
                            userLimit.PerMonthCount += 1;
                            userLimit.TotalCall += 1;
                            _UserAPILimitCountRepository.Update(userLimit);
                        }
                        else
                        {
                            //khushlai - 15-03-2019 For intiallize call - testing
                            _UserAPILimitCountRepository.Add(new UserAPILimitCount
                            {
                                PlanID = keyDetails.APIPlanMasterID,
                                PerDayCount = 0,
                                PerDayUpdatedDate = DateTime.UtcNow,
                                PerMinCount = 0,
                                PerMonthUpdatedDate = DateTime.UtcNow,
                                TotalCall = 0,
                                PerMonthCount = 0,
                                PerMinUpdatedDate = DateTime.UtcNow,
                                CreatedDate = DateTime.UtcNow,
                                CreatedBy = 1,
                                Status = 1,
                                UserID = keyDetails.UserID
                            });

                            context.Response.StatusCode = 401; //Unauthorized
                            model.HTTPErrorCode = 401;
                            Task.Run(() => _mediator.Send(model));
                            return;
                        }
                        await _next.Invoke(context);

                        var ResDataSize = System.Text.ASCIIEncoding.ASCII.GetByteCount(context.Response.Body.ToString());
                        if (ResDataSize > MaxResSize)
                        {
                            context.Response.StatusCode = 429; //Unauthorized
                            model.HTTPErrorCode = 429;
                            Task.Run(() => _mediator.Send(model));
                            return;
                        }

                        //khushali 15-03-2019 Add log for APIStatistics and  PublicAPIReqResLog
                        string responseBody = new StreamReader(context.Response.Body).ReadToEnd();
                        var erParams = (dynamic)null;
                        if (responseBody.Contains("ReturnCode"))
                            erParams = JsonConvert.DeserializeObject<ErrorParams>(responseBody);


                        if (erParams == null)
                        {
                            model.IsSuccessFaliure = Convert.ToInt16(enResponseCode.Fail);
                            model.HTTPErrorCode = context.Response.StatusCode;
                        }
                        else
                        {
                            model.IsSuccessFaliure = erParams?.ReturnCode;
                            model.HTTPErrorCode = erParams?.ErrorCode;
                        }

                        model.HTTPStatusCode = context.Response.StatusCode;
                        Task.Run(() => _mediator.Send(model));
                    }
                    else
                    {
                        // no authorization header
                        context.Response.StatusCode = 416; //Unauthorized
                        return;
                    }
                    b:
                    return;
                }
                else
                {
                    context.Response.StatusCode = 405;
                    return;
                }
            }
        }

        public static string CreateSignature(string API_Key,string Secret,string uri, byte[] parameters, string nonce)
        {
            try
            {
                var Request = Convert.ToBase64String(parameters);
                var signature = $"{API_Key}POST{uri}{nonce}{Request}";
                HelperForLog.WriteLogForActivity("AuthenticationMiddleware ", "AuthenticationMiddleware ", "signature : " + signature);
                Byte[] keyBytes = Encoding.UTF8.GetBytes(Secret);
                Byte[] textBytes = Encoding.UTF8.GetBytes(signature);
                using (var hashAlgo = new HMACSHA256(keyBytes))
                {
                    var signedBytes = hashAlgo.ComputeHash(textBytes);
                    return BitConverter.ToString(signedBytes).Replace("-", "").ToLower();
                }
               
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            
        }
    }
    
}
