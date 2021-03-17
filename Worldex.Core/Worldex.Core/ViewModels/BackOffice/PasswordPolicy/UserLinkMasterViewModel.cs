using System;

namespace Worldex.Core.ViewModels.BackOffice.PasswordPolicy
{
    public class UserLinkMasterViewModel
    {
        public string UserLinkData { get; set; }
        public int UserId { get; set; }
        public int LinkvalidTime { get; set; }
    }
    public class UserLinkMasterUpdateViewModel
    {
        public Guid Id { get; set; }
        public string UserLinkData { get; set; }
        public int UserId { get; set; }
        public int LinkvalidTime { get; set; }
    }
}
