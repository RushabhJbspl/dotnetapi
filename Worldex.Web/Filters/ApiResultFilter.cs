using System;
using Worldex.Infrastructure.Interfaces;
using Worldex.Web.Helper;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace Worldex.Web.Filters
{
    public class ApiResultFilter : ActionFilterAttribute
    {
        private readonly IBasePage _basePage;
        private readonly IConfiguration _configuration;
        public ApiResultFilter(IBasePage basePage, IConfiguration configuration)
        {
            _basePage = basePage;
            _configuration = configuration;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            try
            {
                Console.WriteLine("Current Log: " + "ApiResultFilter OnResultExecuting Start: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (!context.ModelState.IsValid)
                {                   
                    context.Result = new MyResponse(context.ModelState);
                }
                // khushali 24-04-2019 for 502 bad request gatway
                //string MethodName = context.RouteData.Values["action"].ToString();
                //if (MethodName.ToLower() != "getaccessrightsbyuserv1")
                //{
                //    if (((Microsoft.AspNetCore.Http.Internal.DefaultHttpRequest)((Microsoft.AspNetCore.Http.DefaultHttpContext)context.HttpContext).Request).Path.ToString() != _configuration["ASOSToken"].ToString())
                //    {
                //        string ReturnCode = ((Core.ApiModels.BizResponseClass)((Microsoft.AspNetCore.Mvc.ObjectResult)context.Result).Value)?.ErrorCode.ToString();
                //        if (ReturnCode == "Status500InternalServerError")
                //        {
                //            string ReturnMsg = ((Worldex.Core.ApiModels.BizResponseClass)((Microsoft.AspNetCore.Mvc.ObjectResult)context.Result).Value).ReturnMsg;
                //            HelperForLog.WriteLogIntoFile(2, _basePage.UTC_To_IST(), context.HttpContext.Request.Path.ToString(), context.HttpContext.Request.Path.ToString(), ReturnMsg);
                //            HelperForLog.WriteErrorLog(_basePage.UTC_To_IST(), context.HttpContext.Request.Path.ToString(), context.HttpContext.Request.Path.ToString(), ReturnMsg);
                //            ((Worldex.Core.ApiModels.BizResponseClass)((Microsoft.AspNetCore.Mvc.ObjectResult)context.Result).Value).ReturnMsg = "Error occurred.";
                //        }
                //    }
                //}
                Console.WriteLine("Current Log: " + "ApiResultFilter OnResultExecuting End: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }            
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        //public override void OnResultExecuted(ResultExecutedContext filterContext)
        //{
        //    Console.WriteLine("Current Log: " + "ApiResultFilter OnResultExecuted: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        //}
    }
}
