using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.BackOfficeComplain
{
    public class BackoffComplainMasterViewModel
    {
        public long ComplainId { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }

    public class BackOffComplainTrailViewModel
    {
        public long TrailID { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
        public string Remark { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreatedDate { get; set; }
    }

    public class GetComplainTrailResponse : BizResponseClass
    {
        public GetComplainAllData CompainAllData { get; set; }
    }

    public class GetComplainAllData
    {
        public List<BackoffComplainMasterViewModel> ComplainMasterData { get; set; }
        public List<BackOffComplainTrailViewModel> CompainTrailData { get; set; }
    }
}
