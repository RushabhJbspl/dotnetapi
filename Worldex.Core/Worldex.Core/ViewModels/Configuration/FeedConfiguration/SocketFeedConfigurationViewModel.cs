using Worldex.Core.ApiModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration.FeedConfiguration
{
    public class SocketFeedConfigurationViewModel
    {
        public long ID { get; set; }
        public long MethodID { get; set; }
        public long FeedLimitID { get; set; }
        public short Status { get; set; }
    }
    public class SocketFeedConfigurationResponse : BizResponseClass
    {
        List<SocketFeedConfigurationViewModel> Response { get; set; }
    }
    public class SocketFeedConfigurationRequest
    {
        public long? ID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11075")]
        public long MethodID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11076")]
        public long FeedLimitID { get; set; }
        public short Status { get; set; }
    }

    public class SocketFeedConfigQueryRes
    {
        public long Id { get; set; }
        public long MethodID { get; set; }
        public long LimitID { get; set; }
        public string MethodName { get; set; }
        public short EnumCode { get; set; }
        public string LimitDesc { get; set; }
        public short LimitType { get; set; }
        public long MaxLimit { get; set; }
        public long MinLimit { get; set; }
        public long MaxSize { get; set; }
        public long MinSize { get; set; }
        public long MaxRecordCount { get; set; }
        public long RowLenghtSize { get; set; }
        public long MaxRowCount { get; set; }
        public short Status { get; set; }
    }

    public class SocketFeedConfigResData
    {
        public long Id { get; set; }
        public long MethodID { get; set; }
        public long LimitID { get; set; }
        public string MethodName { get; set; }
        public short EnumCode { get; set; }
        public string LimitDesc { get; set; }
        public short LimitType { get; set; }
        public long MaxLimit { get; set; }
        public long MinLimit { get; set; }
        public long MaxSize { get; set; }
        public long MinSize { get; set; }
        public long MaxRecordCount { get; set; }
        public long RowLenghtSize { get; set; }
        public long MaxRowCount { get; set; }
        public short Status { get; set; }
    }
    public class SocketFeedConfigResponse :BizResponseClass
    {
        public List<SocketFeedConfigResData> Response { get; set; }
    }
}
