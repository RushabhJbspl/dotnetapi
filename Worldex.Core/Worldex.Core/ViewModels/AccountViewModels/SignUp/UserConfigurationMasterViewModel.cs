using Worldex.Core.SharedKernel;

namespace Worldex.Core.ViewModels.AccountViewModels.SignUp
{
   public class UserConfigurationMasterViewModel : BizBase
    {
        public int UserId { get; set; }

        public string Type { get; set; }

        public string ConfigurationValue { get; set; }

        public bool EnableStatus { get; set; }
    }
}
