using IpData.Models;

namespace Worldex.Core.ViewModels.AccountViewModels.IpWiseDataViewModel
{
    public  class IPWiseDataViewModel
    {
        public bool IsValid { get; set; }
        public string Location { get; set; } = "Localhost";
        public string CountryCode { get; set; } = "IN";
        public IpInfo IpInfo { get; set; }
    }
    public class IpInfoResponse
    {
        public IpInfo IpInfo { get; set; }
        public bool IsValid { get; set; }
    }
}
