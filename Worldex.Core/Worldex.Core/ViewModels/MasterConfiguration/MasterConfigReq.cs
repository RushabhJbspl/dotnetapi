using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.MasterConfiguration
{
    public class AddStateReq
    {
        public long? StateID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        public string StateName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4902")]
        [Range(1, 99999, ErrorMessage = "1,Invalid Parameter,4911")]
        public long CountryID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4903")]
        public string StateCode { get; set; }
        public short Status { get; set; }
    }

    public class AddCityReq
    {
        public long? CityID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4904")]
        public string CityName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4905")]
        [Range(1, 99999, ErrorMessage = "1,Invalid Parameter,4912")]
        public long StateID { get; set; }
        public short Status { get; set; }
    }

    public class AddCountryReq
    {
        public long? CountryID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4906")]
        public string CountryName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4907")]
        public string CountryCode { get; set; }
        public short Status { get; set; }
    }

    public class AddZipCodeReq
    {
        public long? ZipCodeID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4908")]
        public long ZipCode { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4909")]
        public string AreaName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4910")]
        [Range(1, 99999, ErrorMessage = "1,Invalid Parameter,4913")]
        public long CityID { get; set; }
        public short Status { get; set; }
    }
}
