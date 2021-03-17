using Worldex.Core.Entities.Backoffice.PasswordPolicy;
using Worldex.Core.ViewModels.BackOffice.PasswordPolicy;
using System;

namespace Worldex.Core.Interfaces.PasswordPolicy
{
  public  interface IUserLinkMaster
    {
        Guid Add(UserLinkMasterViewModel userLinkMastes);
        Guid Update(UserLinkMasterUpdateViewModel userLinkMastes);
        UserLinkMaster VerifyUserLink(Guid id);
        UserLinkMaster GetUserLinkData(Guid id);
    }
}
