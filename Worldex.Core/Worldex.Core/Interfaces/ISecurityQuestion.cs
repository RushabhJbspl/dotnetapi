using Worldex.Core.ViewModels.SecurityQuestion;
using System;

namespace Worldex.Core.Interfaces
{
   public interface ISecurityQuestion
    {
        Guid Add(SecurityQuestionMasterReqViewModel securityQuestionMasterViewModel);
        Guid Update(SecurityQuestionMasterReqViewModel securityQuestionMasterViewModel);
    }
}
