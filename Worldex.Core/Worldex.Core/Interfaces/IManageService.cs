using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.ManageViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.Interfaces
{
    public interface IManageService
    {
        UserInfoResponse GetUserInfo(int UserId);
        BizResponseClass AddSubscribeNewsLetter(string Email);
        BizResponseClass RemoveSubscribeNewsLetter(string Email);
    }
}
