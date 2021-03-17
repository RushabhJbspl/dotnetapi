using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System;

namespace Worldex.Core.ViewModels.AccountViewModels.Login
{
    public class CustomtokenViewModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Password { get; set; }
        public bool EnableStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class Customtokenresponse : BizResponseClass
    {
    }

    public class TypeLogRequest
    {
        public long UserID { get; set; }
        public enActivityType ActivityType { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
