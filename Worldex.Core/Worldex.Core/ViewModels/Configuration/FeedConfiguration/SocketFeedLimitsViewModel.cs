using Worldex.Core.ApiModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration.FeedConfiguration
{
    public class SocketFeedLimitsViewModel
    {
        public long ID { get; set; }
        public long MaxSize { get; set; }
        public long MinSize { get; set; }
        public long RowLenghtSize { get; set; }
        public long MaxRowCount { get; set; }
        public long MaxRecordCount { get; set; }
        public long MinRecordCount { get; set; }
        public long MaxLimit { get; set; }
        public long MinLimit { get; set; }
        public short LimitType { get; set; }
        [StringLength(50)]
        public string LimitDesc { get; set; }
        public short Status { get; set; }
    }

    public class SocketFeedLimitsRequest
    {
        public long? ID { get; set; }
        public long MaxSize { get; set; }
        public long MinSize { get; set; }
        public long RowLenghtSize { get; set; }
        public long MaxRowCount { get; set; }
        public long MaxRecordCount { get; set; }
        public long MinRecordCount { get; set; }
        public long MaxLimit { get; set; }
        public long MinLimit { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,11073")]
        public short LimitType { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11074")]
        public string LimitDesc { get; set; }
        public short Status { get; set; }
    }

    public class SocketFeedLimitsListInfo
    {
        public long ID { get; set; }
        public string LimitDesc { get; set; }
        public short Status { get; set; }
    }

    public class SocketFeedLimitsResponse :BizResponseClass
    {
        public List<SocketFeedLimitsViewModel> Response { get; set; }
    }

    public class SocketFeedLimitsListResponse: BizResponseClass
    {
        public List<SocketFeedLimitsListInfo> Response { get; set; }
    }
}
