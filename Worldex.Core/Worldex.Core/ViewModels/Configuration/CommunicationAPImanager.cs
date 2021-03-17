using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class CommunicationServiceConfigViewModel
    {
        public long APID { get; set; }        
        public long ServiceID { get; set; }
        public long SerproID { get; set; }
        public long ServiceTypeID { get; set; }        
        public string SenderID { get; set; } // port or title        
        public string SendURL { get; set; }        
        public int Priority { get; set; }       
        public long RequestID { get; set; }
        public string RequestName { get; set; }
        public string ServiceName { get; set; }        
        public long ParsingDataID { get; set; }
        public string ParsingDataName { get; set; } //ThirdPArtyParsingAPIName
        public string SerproName { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public short status { get; set; }        
    }

    public class CommunicationServiceViewmodel
    {
        public long ServicetypeId { get; set; }
        public long ServiceName { get; set; }
    }

    public class CommunicationServiceResponse : BizResponseClass
    {
         public List<KeyValuePair<string, int>> Result { get; set; }
    }

    public class CommunicationServiceConfigAllData : BizResponseClass
    {
        public List<CommunicationServiceConfigViewModel> Result { get; set; }
    }

    public class CommunicationServiceConfigRequest
    {
        public long APID { get; set; }
        public long ServiceID { get; set; }
        public long SerproID { get; set; }

        [Required]
        public long ServiceTypeID { get; set; }
        [Required]
        public string SenderID { get; set; } // port or title
        [Required]
        public string SendURL { get; set; }
        [Required]
        public int Priority { get; set; }
        [Required]
        public long RequestID { get; set; }

        [Required]
        public string ServiceName { get; set; }
        [Required]
        public long ParsingDataID { get; set; }
        [Required]
        public string SerproName { get; set; }
        [Required]
        public string UserID { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public short Status { get; set; }
    }

    public class CommunicationServiceConfigRes : BizResponseClass
    {
        public CommunicationServiceConfigViewModel Result { get; set; }
    }

    public class RequestFormatViewModel
    {        
        public string RequestName { get; set; }        
        public long RequestID { get; set; }
        public string ContentType { get; set; }        
        public string MethodType { get; set; }        
        public string RequestFormat { get; set; }
        public long RequestType { get; set; }
        public short Status { get; set; }
    }

    public class RequestFormatRequest
    {
        public long RequestID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4523")]
        [StringLength(60)]
        public string RequestName { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4524")]
        [StringLength(60)]
        public string ContentType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4525")]
        [StringLength(20)]
        public string MethodType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4526")]
        [StringLength(500)]
        public string RequestFormat { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4527")]
        public long RequestType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4528")]
        public short Status { get; set; }

    }

    public class RequestFormatResAllData : BizResponseClass
    {
        public List<RequestFormatViewModel> Result { get; set; }
    }

    public class RequestFormatResponse : BizResponseClass
    {
        public RequestFormatViewModel Result { get; set; }
    }

    public class EmailSMSCountData : BizResponseClass
    {
        public EmailSMSCountResp Data { get; set; }
    }
    public class EmailSMSCountResp 
    {
        public long TotalEmailAPIManager { get; set; }
        public long TotalSMSAPIManager { get; set; }
    }
}
