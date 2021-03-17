using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Configuration.FeedConfiguration
{
    public class SocketFeedLimits : BizBase
    {
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
    }
}

