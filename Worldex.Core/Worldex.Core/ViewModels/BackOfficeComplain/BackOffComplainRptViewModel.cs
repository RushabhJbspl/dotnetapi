using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.BackOfficeComplain
{
    public class BackOffComplainRptViewModel : TrackerViewModel
    {
        
    }

    public class GetBackOffComRpt
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Subject { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public long ComplainId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreatedDate { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }
    }

    public class GetTotalCountCom
    {
        public long TotalCount { get; set; }
        public long TotalOpenCount { get; set; }
        public long TotalCloseCount { get; set; }
        public long TotalPendidngCount { get; set; }
    }

    public class GetTotalCountResponse : BizResponseClass
    {
        public  GetTotalCountCom TotalCountDetails {get;set;}
    }

    public class GetBackOffComRptResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<GetBackOffComRpt> GetTotalCompList { get; set; }
    }

    public class BackOffComplainRptResponse : BizResponseClass
    {

    }
}
