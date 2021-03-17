using Worldex.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class WalletLimitConfigurationReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4210")]
        [EnumDataType(typeof(enWalletLimitType), ErrorMessage ="1,Fail,4214")]
        public enWalletLimitType TrnType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4211")]
        [Range(0, 99999, ErrorMessage = "1,Invalid Parameter,4244")]
        public decimal LimitPerHour { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4212")]
        [Range(0, 99999, ErrorMessage = "1,Invalid Parameter,4243")]
        public decimal LimitPerDay { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4213")]
        [Range(0, 99999, ErrorMessage = "1,Invalid Parameter,4245")]
        public decimal LimitPerTransaction { get; set; }

        public double? StartTimeUnix { get; set; }

        public double? EndTimeUnix { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4247")]
        [Range(0, 999999, ErrorMessage = "1,Invalid Parameter,4246")]
        public decimal LifeTime { get; set; }
    }
}
