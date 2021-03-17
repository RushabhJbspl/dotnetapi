using Worldex.Core.Entities.Backoffice.PasswordPolicy;
using Worldex.Core.ViewModels.BackOffice.PasswordPolicy;

namespace Worldex.Core.Interfaces.PasswordPolicy
{
    /// <summary>
    ///Created by pankaj for perform password related opration 09-01-2019
    /// </summary>
    public interface IUserPasswordPolicyMaster
    {
        long Add(UserPasswordPolicyMasterReqViewModel UserPasswordPolicyMaster);
        long Update(UserPasswordPolicyMasterupdatereqViewModel UserPasswordPolicyMasterUpdate);
        long Delete(UserPasswordPolicyMasterDeletereqViewModel UserPasswordPolicyMasterDelete);
        long IsPasswordPolicyExist(UserPasswordPolicyMasterReqViewModel UserPasswordPolicy);
        UserPasswordPolicyMasterresponseListViewModel GetPasswordPolicy(int PageIndex = 0, int Page_Size = 0);
        UserPasswordPolicyMaster GetUserPasswordPolicyConfiguration(int UserId);
        bool CheckOTPAndLinkExpiration(int Userid, int OTPExpirationtime, string VerificationType);
        bool CheckPasswordPolicyExpiration(int Userid, int PwdExpiretime, string VerificationType);
        bool CheckForgotPasswordInday(int Userid, int ForgotPasswordInday, string VerificationType);
        bool CheckForgotPasswordInMonth(int Userid, int ForgotPasswordInMonth, string VerificationType);  ///This method work base on Days calculation
        bool CheckPasswordDefaultPolicyExpiration(int Userid, int PwdExpiretime, string VerificationType);

    }
}
