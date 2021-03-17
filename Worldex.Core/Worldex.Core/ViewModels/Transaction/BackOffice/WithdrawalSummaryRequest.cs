using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction.BackOffice
{
    public class WithdrawalSummaryRequest
    {
        public long MemberID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4655")]
        public string FromDate { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4656")]
        public string ToDate { get; set; }

        public long TrnNo { get; set; }

        public short Status { get; set; }

        public string SMSCode { get; set; }
    }
}
