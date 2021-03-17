using System.Collections.Generic;
using Worldex.Core.ApiModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Worldex.Core.ViewModels.AccountViewModels
{
    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; set; }

        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
    }
    public class SendCodeResponse : BizResponseClass
    {

    }
}
