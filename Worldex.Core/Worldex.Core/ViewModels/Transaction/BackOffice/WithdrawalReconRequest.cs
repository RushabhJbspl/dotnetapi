using Worldex.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction.BackOffice
{
    public class WithdrawalReconRequest
    {
        public long TrnNo { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4649")]
        public string ActionMessage { get; set; }

        [EnumDataType(typeof(enWithdrawalReconActionType), ErrorMessage = "1,Invalid Parameter Value,4650")]
        public enWithdrawalReconActionType ActionType { get; set; }

        public string TrnID { get; set; }//2019-4-3
    }
}
