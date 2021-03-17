using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.BackOffice.ComplaintSALConfiguration
{
    public class ComplaintPriorityMasterViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter complaint priority ,4153")]
        [StringLength(50, ErrorMessage = "1,Please Enter valid complaint priority,4154")]
        public string Priority { get; set; }
        [Required(ErrorMessage = "1,Please Enter priority time,4155")]
        [StringLength(50, ErrorMessage = "1,Please Enter valid priority time,4156")]
        public string PriorityTime { get; set; }
    }
    public class ComplaintPriorityMasterreqViewModel
    {
        public int UserId { get; set; }
        public string Priority { get; set; }
        public string PriorityTime { get; set; }

    }
    public class ComplaintPriorityMasterupdateViewModel
    {
        [Required(ErrorMessage = "1,Please Enter complaint priority Id ,4157")]
        public long Id { get; set; }
        [StringLength(50, ErrorMessage = "1,Please Enter valid complaint priority,4154")]
        public string Priority { get; set; }
        [StringLength(50, ErrorMessage = "1,Please Enter valid priority time,4156")]
        public string PriorityTime { get; set; }

    }
    public class ComplaintPriorityMasterupdatereqViewModel
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string Priority { get; set; }
        public string PriorityTime { get; set; }

    }
    public class ComplaintPriorityMasterDeleteViewModel
    {
        [Required(ErrorMessage = "1,Please Enter complaint priority Id ,4157")]
        public long Id { get; set; }

    }

    public class ComplaintPriorityMasterDeleteReqViewModel
    {
        public long Id { get; set; }
        public int UserId { get; set; }
    }

    public class ComplaintPrioritygetdataViewModel
    {
        public string Priority { get; set; }
        public string PriorityTime { get; set; }
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class ComplaintPrioritygetdataResponse : BizResponseClass
    {
        public List<ComplaintPrioritygetdataViewModel> ComplaintPriorityGet { get; set; }
        public int TotalCount { get; set; }
    }

    public class ComplaintPriorityMasterResponse : BizResponseClass
    {

    }
}
