using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class ProductConfigurationRequest
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4508")]
        [StringLength(30)]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4509")]
        public long ServiceID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4510")]
        public long CountryID { get; set; }
    }
}
