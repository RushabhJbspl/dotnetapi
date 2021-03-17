using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class ThirdPartyAPIResponseConfigViewModel
    {
        public long Id { get; set; }

        public string BalanceRegex { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4658")]
        public string StatusRegex { get; set; }

        public string StatusMsgRegex { get; set; }

        public string ResponseCodeRegex { get; set; }

        public string ErrorCodeRegex { get; set; }

        public string TrnRefNoRegex { get; set; }

        public string OprTrnRefNoRegex { get; set; }

        public string Param1Regex { get; set; }

        public string Param2Regex { get; set; }

        public string Param3Regex { get; set; }
        public short Status { get; set; }
    }

    public class ThirdPartyAPIConfigurationResponseViewModel
    {
        public long Id { get; set; }
        public string ParsingName { get; set; } //add By jagdish 12-02-2020
        public string BalanceRegex { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4658")]
        public string StatusRegex { get; set; }

        public string StatusMsgRegex { get; set; }

        public string ResponseCodeRegex { get; set; }

        public string ErrorCodeRegex { get; set; }

        public string TrnRefNoRegex { get; set; }

        public string OprTrnRefNoRegex { get; set; }

        public string Param1Regex { get; set; }

        public string Param2Regex { get; set; }

        public string Param3Regex { get; set; }
        public short Status { get; set; }
    }

}
