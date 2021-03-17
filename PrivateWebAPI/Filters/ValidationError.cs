using System.Collections.Generic;
using System.Linq;
using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;



using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;


namespace Worldex.Web.Filters
{
    public class ValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        public string Message { get; }

        public ValidationError(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }

    public class MyResultModel : BizResponseClass
    {
        private string Message { get; }

        private List<ValidationError> Errors { get; }

        private string BTErrorArray { get; }

        private string[] BTActaulError { get; }

        public MyResultModel(ModelStateDictionary modelState)
        {
            try
            {


                Message = "Validation Failed";
                Errors = modelState.Keys
                        .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                        .ToList();

                BTErrorArray = Errors.Select(y => y.Message).FirstOrDefault();

                BTActaulError = BTErrorArray.Split(",");

                ReturnCode = (enResponseCode)System.Enum.Parse(typeof(enResponseCode), BTActaulError[0]);
                ReturnMsg = BTActaulError[1];
                ErrorCode = (enErrorCode)System.Enum.Parse(typeof(enErrorCode), BTActaulError[2]);
            }
            catch (Exception ex)
            {
                ReturnCode = enResponseCode.InternalError;
                ReturnMsg = ex.ToString();
                ErrorCode = enErrorCode.Status500InternalServerError;
            }
        }

    }

    public class MyResponse : ObjectResult
    {
        public MyResponse(ModelStateDictionary modelState)
            : base(new MyResultModel(modelState))
        {
            StatusCode = 400;
        }
    }
}
