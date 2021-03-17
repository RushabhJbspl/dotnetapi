using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ViewModels.IEOWallet
{
    public class PreConfirmRequest
    {
        [Required(ErrorMessage = "1,Please Enter Parameter, 17103")]
        public string PaidAccWalletId { get; set; }
        [Required(ErrorMessage = "1,Please Enter Parameter, 17104")]
        public decimal PaidQauntity { get; set; }
        [Required(ErrorMessage = "1,Please Enter Parameter, 17105")]
        public string PaidCurrency { get; set; }
        [Required(ErrorMessage = "1,Please Enter Parameter, 17106")]
        public string DeliveredCurrency { get; set; }
        [Required(ErrorMessage = "1,Please Enter Parameter, 17107")]
        public string RoundGuid { get; set; }
        public string Remarks { get; set; }
    }
}
