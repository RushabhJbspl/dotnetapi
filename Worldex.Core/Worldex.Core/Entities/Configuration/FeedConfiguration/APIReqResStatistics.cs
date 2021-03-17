using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Configuration.FeedConfiguration
{
    public class APIReqResStatistics : BizBase
    {
        public long MethodID { get; set; }
        public long IPId { get; set; }
        public long UserID { get; set; }
        public long SuccessCount { get; set; }
        public long FaliureCount { get; set; }
    }
    public class PublicAPIReqResLog :BizBase
    {
        public long MethodID { get; set; }
        public string Path { get; set; }
        public string MethodType { get; set; }
        public long HTTPErrorCode { get; set; }
        public long HTTPStatusCode { get; set; }
        public string Device { get; set; }
        public string Browser { get; set; }
        public string Host { get; set; }
        public string IPAddress { get; set; }
        public short WhitelistIP { get; set; }
    }
}
