using Worldex.Core.Entities.KYC;
using Worldex.Core.ViewModels.KYC;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces.KYC
{
   public interface IPersonalVerificationService
    {
        Task<long> AddPersonalVerification(PersonalVerificationViewModel model);
        Task<long> UpdatePersonalVerification(PersonalVerificationViewModel model);
        PersonalVerificationViewModel GetPersonalVerification(int Userid);
        PersonalVerification IsUserKYCExist(PersonalVerificationViewModel model);
        int UserKYCStatus(long UserId);
    }
}
