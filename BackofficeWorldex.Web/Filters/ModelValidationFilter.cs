using Worldex.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BackofficeWorldex.Web.Filters
{
    public class ModelValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ModelState.IsValid)
            {
                if (filterContext.HttpContext.Request.Method == "GET")
                {
                    var result = new BadRequestResult();
                    filterContext.Result = result;
                }
                else
                {
                    var result = new ContentResult();
                    string content = Helpers.JsonSerialize(new ApiError(filterContext.ModelState));
                    result.Content = content;
                    result.ContentType = "application/json";

                    filterContext.HttpContext.Response.StatusCode = 400;
                    filterContext.Result = result;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        //public override void OnResultExecuting(ResultExecutingContext context)
        //{
        //    //_logger.LogWarning("ClassFilter OnResultExecuting");
        //    // _logger.LogWarning(((string[])((Microsoft.AspNetCore.Mvc.ObjectResult)context.Result).Value)[0]);
        //    //base.OnResultExecuting(context);
        //}

        //public override void OnResultExecuted(ResultExecutedContext filterContext)
        //{
        //    //_logger.LogWarning("ClassFilter OnResultExecuted");

        //    //string k = ((string[])((Microsoft.AspNetCore.Mvc.ObjectResult)filterContext.Result).Value)[1];
        //    //_logger.LogWarning(((string[])((Microsoft.AspNetCore.Mvc.ObjectResult)context.Result).Value)[1]);
        //    //base.OnResultExecuted(context);
        //}

    }
}