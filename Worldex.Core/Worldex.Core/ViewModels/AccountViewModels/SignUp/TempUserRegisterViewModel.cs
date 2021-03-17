using Worldex.Core.Entities.User;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.ViewModels.AccountViewModels.SignUp
{
    public class TempUserRegisterViewModel : BizBase
    {
        public int RegTypeId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AlternetEmail { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStemp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string AlternetMobile { get; set; }
        public bool RegisterStatus { get; set; }
        public bool IsDeleted { get; set; }
        public string CountryCode { get; set; }
        public RegisterType RegisterType { get; set; }
        public string PreferedLanguage { get; set; }
    }
}
