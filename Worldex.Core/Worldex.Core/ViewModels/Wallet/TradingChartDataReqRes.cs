using Worldex.Core.ApiModels;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Wallet
{
    public class TradingChartDataReqRes
    {
    }

    public class TradingChartDataReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,17100")]
        public long PairId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17101")]
        public string RequestData { get; set; }
    }

    public class TradingChartDataRes : BizResponseClass
    {
        public long PairId { get; set; }

        public string RequestData { get; set; }
    }
    //2019-7-25 vsolanki added response class
    public class ValidateUserForInternalTransferRes : BizResponseClass
    {
        public string Address { get; set; }
    }
}
