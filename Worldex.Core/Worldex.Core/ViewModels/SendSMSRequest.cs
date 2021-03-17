using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels
{
    public class SendSMSRequest : IRequest<CommunicationResponse>
    {
        [Required]
        public long MobileNo { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 5)]
        public string Message { get; set; }
        
        public short MessageType { get; set; } // 3 for resend Message

        public long MessageID { get; set; } // 3 for resend Message
    }
    public class PushSMSRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,5011")]
        public List<long> MobileNo { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,5012")]
        [StringLength(300, MinimumLength = 5,ErrorMessage = "1,Please Enter Required Parameters,5013")]
        public string Message { get; set; }

    }
}
