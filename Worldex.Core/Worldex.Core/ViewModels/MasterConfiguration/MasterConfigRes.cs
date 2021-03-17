using Worldex.Core.ApiModels;

namespace Worldex.Core.ViewModels.MasterConfiguration
{
    public class Countries
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public short Status { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }
    public class States
    {
        public string StateName { get; set; }
        public short Status { get; set; }
        public string StateCode { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }
    public class Cities
    {
        public string CityName { get; set; }
        public short Status { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }
    public class ZipCodes
    {
        public long ZipCode { get; set; }
        public string ZipAreaName { get; set; }
        public short Status { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }
}
