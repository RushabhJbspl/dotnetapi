using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Worldex.Core.ViewModels
{
    public class SendEmailRequest : IRequest<CommunicationResponse>
    {
        [Required]
        [StringLength(50)]
        public string Recepient { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        [StringLength(50)]
        public string Subject { get; set; }

        [StringLength(500)]
        public string CC { get; set; }

        [StringLength(500)]
        public string BCC { get; set; }

        [StringLength(500)]
        public string Attachment { get; set; }

        public short EmailType { get; set; } // 1 - tempalte Mail , 2 -Admin send mail , 3- resend mail 

        public long EmailID { get; set; } // if resend mail 
    }

    public class PushEmailRequest
    {
        public List<string> Recepient { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,5016")]
        public string Body { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,5017")]
        [StringLength(50, ErrorMessage = "1,Please Enter Required Parameters,5018")]
        public string Subject { get; set; }

        public string[] CC { get; set; }

        public string[] BCC { get; set; }

        [StringLength(500, ErrorMessage = "1,Please Enter Required Parameters,5019")]
        public string Attachment { get; set; }
    }

    public class SendAllEmailRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,5016")]
        public string Body { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,5017")]
        [StringLength(50, ErrorMessage = "1,Please Enter Required Parameters,5018")]
        public string Subject { get; set; }
    }

    public class EmailLists
    {
        public String emailid { get; set; }
    }
}
