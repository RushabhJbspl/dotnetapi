using MediatR;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class APIReqResStatisticsViewModel
    {
        public long UserID { get; set; }
        public short Status { get; set; }
        public string Path { get; set; } 
        public string MethodType { get; set; }
        public long HTTPErrorCode { get; set; }
        public long HTTPStatusCode { get; set; }
        public string Mode { get; set; } // - web , app
        public string Device { get; set; } //  browser name . device name 
        public string Host { get; set; } //  full url
        public string IPAddress { get; set; } //  ip address  - null
        public short WhitelistIP { get; set; } //
    }

    public class APIStatistics : IRequest
    {
        public long UserID { get; set; }
        public short IsSuccessFaliure { get; set; }
        public string Path { get; set; }
        public string MethodType { get; set; }
        public long HTTPErrorCode { get; set; }
        public long HTTPStatusCode { get; set; }
        public string Mode { get; set; } // - web , app
        public string Device { get; set; } //  browser name . device name 
        public string Host { get; set; } //  full url
        public string IPAddress { get; set; } //  ip address  - null
        public short WhitelistIP { get; set; } 
    }
}
