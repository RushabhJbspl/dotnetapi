using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.Enums;
using Worldex.Infrastructure.Data;
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
                //_logger.LogWarning("ClassFilter OnResultExecuting");
                // _logger.LogWarning(((string[])((Microsoft.AspNetCore.Mvc.ObjectResult)context.Result).Value)[0]);
                //base.OnResultExecuting(context);                
                if (!context.ModelState.IsValid)
                {
                    //context.Result = new ValidationFailedResult(context.ModelState);
                    context.Result = new MyResponse(context.ModelState);
                }
                // khushali 24-04-2019 for 502 bad request gatway
                string MethodName = context.RouteData.Values["action"].ToString();
                if (MethodName.ToLower() != "getaccessrightsbyuserv1")
                {
                    if (((Microsoft.AspNetCore.Http.Internal.DefaultHttpRequest)((Microsoft.AspNetCore.Http.DefaultHttpContext)context.HttpContext).Request).Path.ToString() != _configuration["ASOSToken"].ToString())
                    {
                        string ReturnCode = ((Core.ApiModels.BizResponseClass)((Microsoft.AspNetCore.Mvc.ObjectResult)context.Result).Value)?.ErrorCode.ToString();
                        if (ReturnCode == "Status500InternalServerError")
                        {
                            string ReturnMsg = ((Worldex.Core.ApiModels.BizResponseClass)((Microsoft.AspNetCore.Mvc.ObjectResult)context.Result).Value).ReturnMsg;
                            HelperForLog.WriteLogIntoFile(2, _basePage.UTC_To_IST(), context.HttpContext.Request.Path.ToString(), context.HttpContext.Request.Path.ToString(), ReturnMsg);
                            HelperForLog.WriteErrorLog(_basePage.UTC_To_IST(), context.HttpContext.Request.Path.ToString(), context.HttpContext.Request.Path.ToString(), ReturnMsg);
                            ((Worldex.Core.ApiModels.BizResponseClass)((Microsoft.AspNetCore.Mvc.ObjectResult)context.Result).Value).ReturnMsg = "Error occurred.";
                        }
                    }
                }

                //if (((Microsoft.AspNetCore.Mvc.SignInResult)context.Result).AuthenticationScheme.ToString() != "ASOS")
                //{
                //    string ReturnCode = ((Core.ApiModels.BizResponseClass)((Microsoft.AspNetCore.Mvc.ObjectResult)context.Result).Value)?.ErrorCode.ToString();
                //    if (ReturnCode == "Status500InternalServerError")
                //    {
                //        string ReturnMsg = ((Worldex.Core.ApiModels.BizResponseClass)((Microsoft.AspNetCore.Mvc.ObjectResult)context.Result).Value).ReturnMsg;
                //        HelperForLog.WriteLogIntoFile(2, _basePage.UTC_To_IST(), context.HttpContext.Request.Path.ToString(), context.HttpContext.Request.Path.ToString(), ReturnMsg);
                //        HelperForLog.WriteErrorLog(_basePage.UTC_To_IST(), context.HttpContext.Request.Path.ToString(), context.HttpContext.Request.Path.ToString(), ReturnMsg);
                //        ((Worldex.Core.ApiModels.BizResponseClass)((Microsoft.AspNetCore.Mvc.ObjectResult)context.Result).Value).ReturnMsg = "Error occurred.";
                //    }
                //}
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //string k = (string[])((Worldex.Core.ApiModels.BizResponseClass)((Microsoft.AspNetCore.Mvc.ObjectResult)filterContext.Result).Value);
            //_logger.LogWarning("ClassFilter OnResultExecuted");

            //string k = ((string[])((Microsoft.AspNetCore.Mvc.ObjectResult)filterContext.Result).Value)[1];
            //_logger.LogWarning(((string[])((Microsoft.AspNetCore.Mvc.ObjectResult)context.Result).Value)[1]);
            //base.OnResultExecuted(context);
        }
    }
}
