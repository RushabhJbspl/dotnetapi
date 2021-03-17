using Worldex.Core.ApiModels;
using System;

namespace Worldex.Core.ViewModels.AccountViewModels.UserKey
{
    public class UserKeyViewModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UniqueKey { get; set; }
        public bool EnableStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class UserKeyResponse : BizResponseClass
    {
    }
}


