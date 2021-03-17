using System;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities
{
    public class ThirdPartyAPIConfiguration : BizBase
    {
        [Required]
        [StringLength(30)]
        public string APIName { get; set; }

        [Required]
        [Url]
        public string APISendURL { get; set; }
       
        [Url]
        public string APIValidateURL { get; set; }
       
        [Url]
        public string APIBalURL { get; set; }

        [Url]
        public string APIStatusCheckURL { get; set; }

        public string APIRequestBody { get; set; }       

        public string BalCheckRequestBody { get; set; }

        public string TransactionIdPrefix { get; set; }

        public string MerchantCode { get; set; }

        public string ResponseSuccess { get; set; }

        public string ResponseFailure { get; set; }
              
        public string ResponseHold { get; set; }

        public string AuthHeader { get; set; }

        public string ContentType { get; set; }

        public string MethodType { get; set; }

        public string BalCheckMethodType { get; set; }

        public string HashCode { get; set; }

        public string HashCodeRecheck { get; set; }

        public short HashType { get; set; }

        public short AppType { get; set; }

        public long ParsingDataID { get; set; }
        public string TimeStamp { get; set; }

        public void SetActive()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<ThirdPartyAPIConfiguration>(this));
        }
        public void SetInActive()
        {
            Status = Convert.ToInt16(ServiceStatus.InActive);
            Events.Add(new ServiceStatusEvent<ThirdPartyAPIConfiguration>(this));
        }
    }

    public class ArbitrageThirdPartyAPIConfiguration : BizBase
    {
        [Required]
        [StringLength(30)]
        public string APIName { get; set; }

        [Required]
        [Url]
        public string APISendURL { get; set; }

        [Url]
        public string APIValidateURL { get; set; }

        [Url]
        public string APIBalURL { get; set; }

        [Url]
        public string APIStatusCheckURL { get; set; }

        public string APIRequestBody { get; set; }

        public string ValidateRequestBody { get; set; }

        public string BalCheckRequestBody { get; set; }

        public string TransactionIdPrefix { get; set; }

        public string MerchantCode { get; set; }

        public string ResponseSuccess { get; set; }

        public string ResponseFailure { get; set; }

        public string ResponseHold { get; set; }

        public string AuthHeader { get; set; }

        public string ContentType { get; set; }

        public string MethodType { get; set; }

        public string ValidateMethodType { get; set; }

        public string BalCheckMethodType { get; set; }

        public string HashCode { get; set; }

        public string HashCodeRecheck { get; set; }

        public short HashType { get; set; }

        public short AppType { get; set; }

        public long ParsingDataID { get; set; }
        public string TimeStamp { get; set; }

        [Url]//komal 23-07-2019 for CCXT ticker
        public string APITickerURL { get; set; } 
        public string TickerMethodType { get; set; }
        public string TickerRequestBody { get; set; }

        public string APICancellationURL { get; set; }
        public string CancellationMethodType { get; set; }
        public string CancellationRequestBody { get; set; }

        public void SetActive()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<ArbitrageThirdPartyAPIConfiguration>(this));
        }
        public void SetInActive()
        {
            Status = Convert.ToInt16(ServiceStatus.InActive);
            Events.Add(new ServiceStatusEvent<ArbitrageThirdPartyAPIConfiguration>(this));
        }
    }
}
