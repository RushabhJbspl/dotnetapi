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
        
        public static List<APIPlanMaster> aPIPlans = new List<APIPlanMaster>();
        public static List<APIPlanDetail> aPIPlanDetails = new List<APIPlanDetail>();
        public static List<RestMethods> restMethods = new List<RestMethods>();
        public static List<UserAPILimitCount> aPILimitCounts = new List<UserAPILimitCount>();
        public static List<WhiteListIPEndPoint> WhiteListIPEndPoint = new List<WhiteListIPEndPoint>();
        private static List<APIPlanMethodConfiguration> PlanMethod = new List<APIPlanMethodConfiguration>();
        private static List<APIMethodConfiguration> MethodConfig = new List<APIMethodConfiguration>();
        private readonly IMediator _mediator;
        public static string MethodList = "";
        public AuthenticationMiddleware(RequestDelegate next, ICommonRepository<UserAPIKeyDetails> UserAPIKeyDetailsRepository,
            UserManager<ApplicationUser> userManager, ICommonRepository<APIPlanMaster> APIPlanMasterRepository, ICommonRepository<APIPlanDetail> APIPlanDetailRepository,
            ICommonRepository<WhiteListIPEndPoint> WhiteListIPEndPointRepository, IMemoryCache cache, ICommonRepository<UserAPILimitCount> UserAPILimitCountRepository,
            ICommonRepository<UserSubscribeAPIPlan> SubScribePlanRepository, ICommonRepository<APIKeyWhitelistIPConfig> APIKeyWhitelistIPConfigRepository,
            IMediator mediator, ICommonRepository<APIPlanMethodConfiguration> APIPlanMethodConfigurationRepository, ICommonRepository<RestMethods> RestMethodsRepository,
            ICommonRepository<APIMethodConfiguration> APIMethodConfiguration)
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

            aPIPlans = _cache.Get<List<APIPlanMaster>>("aPIPlans");
            if (aPIPlans == null)
            {
                _cache.Set("aPIPlans", _APIPlanMasterRepository.List());
                aPIPlans = _APIPlanMasterRepository.List();
            }
            aPIPlanDetails = _cache.Get<List<APIPlanDetail>>("aPIPlanDetails");
            if (aPIPlanDetails == null)
            {
                _cache.Set("aPIPlanDetails", _APIPlanDetailRepository.List());
                aPIPlanDetails = _APIPlanDetailRepository.List();
            }
            aPILimitCounts = _cache.Get<List<UserAPILimitCount>>("aPILimitCounts");
            if (aPILimitCounts == null)
            {
                _cache.Set("aPILimitCounts", _UserAPILimitCountRepository.List());
                aPILimitCounts = _UserAPILimitCountRepository.List();
            }
            WhiteListIPEndPoint = _cache.Get<List<WhiteListIPEndPoint>>("WhiteListIPEndPoint");
            if (WhiteListIPEndPoint == null)
            {
                _cache.Set("WhiteListIPEndPoint", _WhiteListIPEndPointRepository.List());
                WhiteListIPEndPoint = _WhiteListIPEndPointRepository.List();
            }
            PlanMethod = _cache.Get<List<APIPlanMethodConfiguration>>("PlanMethod");
            if (PlanMethod == null)
            {
                PlanMethod = _APIPlanMethodConfigurationRepository.List();
                _cache.Set("PlanMethod", _APIPlanMethodConfigurationRepository.List());
            }
            restMethods = _cache.Get<List<RestMethods>>("restMethods");
            if (restMethods == null)
            {
                restMethods = _RestMethodsRepository.List();
                _cache.Set("restMethods", _RestMethodsRepository.List());
                var methods= Array.ConvertAll<RestMethods, string>(restMethods.ToArray(), x => x.Path);
                MethodList = string.Join(",", methods);
            }
            MethodConfig = _cache.Get<List<APIMethodConfiguration>>("MethodConfig");
            if (MethodConfig == null)
            {
                MethodConfig = _APIMethodConfiguration.List();
                _cache.Set("MethodConfig", _APIMethodConfiguration.List());
            }
        }
        public async Task Invoke(HttpContext context)
        {
            List<APIPlanMethodConfiguration> PlanMethodList = null;
            string ApiKey2 = context.Request.Headers["X-MJBSPLX-APIKEY"];
            string SecretKey2 = context.Request.Headers["X-MJBSPLX-SECURITYKEY"];
            if (string.IsNullOrEmpty(ApiKey2) && string.IsNullOrEmpty(SecretKey2))
            {
                await _next.Invoke(context);
            }
            else
            {
             
                //var path = context.Request.Path.Value.ToString().ToLower();
                //if (context.Request.Path.Value.ToString().ToLower() == "/swagger/index.html" || context.Request.Path.Value.ToString().ToLower() == "/swagger/v1/swagger.json" || context.Request.Path.Value.ToString().ToLower()=="/connect/token")   
                //else
                //{
                //var MethodName = context.Request.Path.Value.ToString().ToLower();
                //if (context.Request.Path.Value.ToString().ToLower() == "/api/backofficeapiconfiguration/viewpublicapikeys")
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
                        //khushali 14-03-2019 Add log for APIStatistics and  PublicAPIReqResLog
                        if (HttpMethods.IsPost(context.Request.Method))
                        {
                            //byte[] actualSignature = await ComputeRequestBodySha512HashAsync(context.Request,Encoding.UTF8.GetBytes(SecretKey));
                            //if (actualSignature == null)
                            //{
                            //    context.Response.StatusCode = 401; //Unauthorized
                            //    return;
                            //}
                            //if (!Equals(expectedSignature, actualSignature))
                            //{
                            //    context.Response.StatusCode = 401; //Unauthorized
                            //    return;
                            //}
                            //var bodyAsText = "";
                            //using (var bodyReader = new StreamReader(context.Request.Body))
                            //{
                            //    bodyAsText = bodyReader.ReadToEnd();
                            //    var bytesToWrite = Encoding.UTF8.GetBytes(bodyAsText);
                            //}
                            //var bodyReader = context.Request.Body;
                            //byte[] buffer = new byte[4096];
                            //int read = bodyReader.ReadAsync(buffer, 0, buffer.Length).Result;
                            //byte[] data = new byte[read];
                            //Array.Copy(buffer, data, read);
                            //string actualSignature = CreateSignature(ApiKey, SecretKey, context.Request.Host.Value.ToString(), data, TimeStamp);
                            //if (actualSignature == null)
                            //{
                            //    //context.Response.StatusCode = 401; //Unauthorized
                            //    //return;
                            //}
                            //if (!Equals(expectedSignature, actualSignature))
                            //{
                            //    //context.Response.StatusCode = 401; //Unauthorized
                            //    //return;
                            //}

                        }
                        DateTime CurTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToDouble(TimeStamp));
                        if (CurTime < DateTime.UtcNow.AddSeconds(-50) && DateTime.UtcNow.AddSeconds(50) > CurTime)
                        {
                            //context.Response.StatusCode = 401; //Unauthorized
                            //return;
                        }

                        var keyDetails = _UserAPIKeyDetailsRepository.FindBy(e => e.SecretKey == SecretKey && e.APIKey == ApiKey && e.Status == 1).FirstOrDefault();
                        if (keyDetails == null)
                        {
                            context.Response.StatusCode = 401; //Unauthorized
                            return;
                        }
                        context.Items.Add("APIUserID", keyDetails.UserID.ToString());
                        //ApplicationUser user = await _userManager.FindByIdAsync(keyDetails.UserID.ToString());
                        if (keyDetails.IPAccess == 1)
                        {
                            if (IPAddress == null)
                            {
                                context.Response.StatusCode = 401; //Unauthorized
                                return;
                            }
                            List<long> KeyIPConfig = _APIKeyWhitelistIPConfigRepository.FindBy(e => e.APIKeyID == keyDetails.Id && e.Status == 1).Select(e => e.IPId).ToList();

                            foreach (long obj in KeyIPConfig)
                            {
                                var ip = WhiteListIPEndPoint.Where(e => e.Id == obj).FirstOrDefault();
                                if (ip.IPAddress == IPAddress)
                                    goto a;
                            }
                        }
                        a:
                        UserSubscribeAPIPlan history = _SubScribePlanRepository.FindBy(e => e.UserID == keyDetails.UserID && e.Status == 1 && e.APIPlanMasterID == keyDetails.APIPlanMasterID).FirstOrDefault();
                        if (history == null)
                        {
                            //context.Response.StatusCode = 401; //Unauthorized
                            //return;
                        }
                        
                        var restMethodObj = restMethods.Where(e => e.Path.Contains(pathNew)).SingleOrDefault();
                        if(restMethodObj==null)
                        {
                            context.Response.StatusCode = 401; //Unauthorized
                            return;
                        }
                        var Methodid = restMethodObj.Id;
                        if (history.CustomeLimitId == 0)
                        {
                            APIPlanMaster limitObj = aPIPlans.Find(e => e.Id == keyDetails.APIPlanMasterID);
                            PlanMethodList = PlanMethod.Where(e => e.APIPlanMasterID == keyDetails.APIPlanMasterID && e.CustomeLimitId == 0 && e.Status == 1).ToList();                            
                            MinLimit = limitObj.MaxPerMinute; DayLimit = limitObj.MaxPerDay; MonthLimit = limitObj.MaxPerMonth; MaxResSize = limitObj.MaxResSize; MaxReqSize = limitObj.MaxReqSize;
                        }
                        else
                        {
                            APIPlanDetail limitObj = aPIPlanDetails.Find(e => e.Id == history.CustomeLimitId);
                            PlanMethodList = PlanMethod.Where(e => e.APIPlanMasterID == keyDetails.APIPlanMasterID && e.CustomeLimitId == history.CustomeLimitId && e.Status == 1).ToList();
                            MinLimit = limitObj.MaxPerMinute; DayLimit = limitObj.MaxPerDay; MonthLimit = limitObj.MaxPerMonth; MaxResSize = limitObj.MaxResSize; MaxReqSize = limitObj.MaxReqSize;
                        }
                        if(PlanMethodList.Count==0)
                        {
                            //context.Response.StatusCode = 429; //Unauthorized
                            //return;
                        }
                        var KeyArr = Array.ConvertAll<APIPlanMethodConfiguration, long>(PlanMethodList.ToArray(), x => (long)x.RestMethodID);
                        var ParentID = MethodConfig.Find(e => e.MethodID == Methodid && e.MethodType == 1);
                        if (ParentID == null)
                        {
                            //context.Response.StatusCode = 429; //Unauthorized
                            //return;
                        }
                        if (!KeyArr.Contains(ParentID.ParentID))
                        {
                            //context.Response.StatusCode = 429; //Unauthorized
                            //return;
                        }
                        var ReqDataSize = System.Text.ASCIIEncoding.ASCII.GetByteCount(context.Request.Body.ToString());
                        if (ReqDataSize > MaxResSize)
                        {
                            //context.Response.StatusCode = 429; //Unauthorized
                            //return;
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
                            return;
                        }
                        await _next.Invoke(context);

                        var ResDataSize = System.Text.ASCIIEncoding.ASCII.GetByteCount(context.Response.Body.ToString());
                        if (ResDataSize > MaxResSize)
                        {
                            context.Response.StatusCode = 429; //Unauthorized
                            return;
                        }

                        //khushali 15-03-2019 Add log for APIStatistics and  PublicAPIReqResLog
                        string responseBody = new StreamReader(context.Response.Body).ReadToEnd();
                        var erParams = (dynamic)null;
                        if (responseBody.Contains("ReturnCode"))
                            erParams = JsonConvert.DeserializeObject<ErrorParams>(responseBody);

                        APIStatistics model = new APIStatistics();
                        if(erParams == null)
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
                        model.IPAddress = IPAddress;
                        model.Path = pathNew;
                        model.MethodType = context.Request.Method;
                        model.WhitelistIP = keyDetails.IPAccess;
                        model.Host = context.Request.Host.Value.ToString();
                        model.UserID = Convert.ToInt64(context.Items["APIUserID"].ToString());
                        Task.Run(() => _mediator.Send(model));
                    }
                    else
                    {
                        // no authorization header
                        context.Response.StatusCode = 401; //Unauthorized
                        return;
                    }
                }
                else
                    await _next.Invoke(context);
            }
            
        }

        //private async Task<byte[]> ComputeRequestBodySha512HashAsync(HttpRequest request,byte[] secret)
        //{
        //    await PrepareRequestBody(request);

        //    using (HMACSHA512 hasher = new HMACSHA512(secret))
        //    {
        //        try
        //        {
        //            Stream inputStream = request.Body;

        //            int bytesRead;
        //            byte[] buffer = new byte[4096];
        //            byte[] buffer1 = new byte[0];

        //            while ((bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        //            {
        //                hasher.TransformBlock(buffer, inputOffset: 0, inputCount: bytesRead,
        //                    outputBuffer: null, outputOffset: 0);
        //            }

        //            hasher.TransformFinalBlock(buffer1, inputOffset: 0, inputCount: 0);

        //            return hasher.Hash;
        //        }
        //        finally
        //        {
        //            request.Body.Seek(0L, SeekOrigin.Begin);
        //        }
        //    }
        //}

        //private async Task PrepareRequestBody(HttpRequest request)
        //{
        //    if (!request.Body.CanSeek)
        //    {
        //        request.EnableBuffering();

        //        await request.Body.DrainAsync(CancellationToken.None);
        //    }

        //    request.Body.Seek(0L, SeekOrigin.Begin);
        //}

        public static string CreateSignature(string API_Key,string Secret,string uri,byte[] parameters, string nonce)
        {
            try
            {
                //SIGNATURE: API_KEY + "POST" + URI + NONCE + POST_PARAMS(signed by secret key according to HMAC - SHA512 method.)
                string endpoint = WebUtility.UrlEncode(uri.ToString()).ToLower();
                //string Request = Convert.ToBase64String(Encoding.UTF8.GetBytes(parameters ?? ""));
                string Request = Convert.ToBase64String(parameters);
                var signature = $"{API_Key}POST{endpoint}{nonce}{Request}";
                using (var hashAlgo = new HMACSHA512(Convert.FromBase64String(Secret)))
                {
                    var signedBytes = hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(HttpUtility.UrlDecode(signature)));
                    return Convert.ToBase64String(signedBytes);
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            
        }
    }
    
}
