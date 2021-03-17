using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.ControlPanel
{
    public class DepositionIntervalViewModel
    {
        [Required(ErrorMessage = "1,Enter Required Parameters")]
        public long DepositHistoryFetchListInterval { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters")]
        public long DepositStatusCheckInterval { get; set; }

        public Int16 Status { get; set; }
    }

    public class DepositionIntervalResponse : BizResponseClass
    {

    }

    public class DepositionIntervalStatusViewModel
    {
        public long Id { get; set; }
    }

    public class DepositionIntervalListViewModel
    {
        public long Id { get; set; }
        public long DepositHistoryFetchListInterval { get; set; }
        public long DepositStatusCheckInterval { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public int Status { get; set; }
    }
    public class ListDepositionIntervalResponse : BizResponseClass
    {
        public List<DepositionIntervalListViewModel> ListDepositionInterval { get; set; }
    }

}
