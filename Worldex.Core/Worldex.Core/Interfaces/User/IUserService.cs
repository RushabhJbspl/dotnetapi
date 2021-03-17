using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.User;
using Worldex.Core.ViewModels.AccountViewModels.IpWiseDataViewModel;
using Worldex.Core.ViewModels.AccountViewModels.Login;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Worldex.Core.Interfaces.User
{
    public interface IUserService
    {
        bool GetMobileNumber(string MobileNumber);
        long GenerateRandomOTP();
        Task<ApplicationUser> FindByMobileNumber(string MobileNumber);
        Task<bool> IsValidPhoneNumber(string Mobilenumber, string CountryCode);
        Task<string> GetCountryByIP(string ipAddress);
        Task<string> GetLocationByIP(string ipAddress);
        Task<IPWiseDataViewModel> GetIPWiseData(string ipAddress);  
        string GenerateRandomOTPWithPassword(PasswordOptions opts = null);
        SocialCustomPasswordViewMoel GenerateRandomSocialPassword(string ProvideKey);
        Task<ApplicationUser> FindUserDataByUserNameEmailMobile(string UserName);
        List<GetUserData> GetAllUserData();
        string GenerateRandomPassword(PasswordOptions opts = null);
        DateTime GetUserJoiningDate(long UserId);
        bool GetUserById(long UserId);
        bool CheckMobileNumberExists(string MobileNumber);
        ApplicationUser GetUserDataById(long UserId);
        //khushali 04-04-2019 Move from Worldex/Web/API/ManageController TO  UserService
        ApplicationUserPhotos GetUserPhoto(long UserId);
        Task<BizResponseClass> PostUserPhotoAsync(IFormFile file, int UserId);
        LanguagePreferenceMaster GetLanguagePreferenceMaster(string PreferedLanguage);
        Task<BizResponseClass> UserValidationForWithdraw(long UserId);
    }
}
