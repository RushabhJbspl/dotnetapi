using Worldex.Core.Enums;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class WalletTrnLimitResponse
    {
        public enResponseCode ReturnCode { get; set; }

        public string ReturnMsg { get; set; }

        public enErrorCode ErrorCode { get; set; }

        public string MinimumAmounts { get; set; }

        public string MaximumAmounts { get; set; }
    }
}
