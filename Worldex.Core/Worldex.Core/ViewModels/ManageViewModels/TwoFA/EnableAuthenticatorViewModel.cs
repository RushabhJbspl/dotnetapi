using Worldex.Core.ApiModels;
using System.ComponentModel;

namespace Worldex.Core.ViewModels.ManageViewModels
{
    public class EnableAuthenticatorViewModel
    {
        [ReadOnly(true)]
        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }
    }

    public class EnableAuthenticationResponse : BizResponseClass
    {
        public EnableAuthenticatorViewModel EnableAuthenticatorViewModel { get; set; }
    }
}
