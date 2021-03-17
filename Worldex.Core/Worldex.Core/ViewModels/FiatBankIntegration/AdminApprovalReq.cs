using System;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.Enums;

namespace Worldex.Core.ViewModels.FiatBankIntegration
{
    public class AdminApprovalReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,17263")]
        public Guid Guid { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17264")]
        [EnumDataType(typeof(ApprovalStatus), ErrorMessage = "1,Invalid Parameter,17265")]
        public ApprovalStatus Bit { get; set; }

        public string Remarks { get; set; }
    }
}
