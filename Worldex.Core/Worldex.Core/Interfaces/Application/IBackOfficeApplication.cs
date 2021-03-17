using Worldex.Core.ViewModels.Organization;
using System;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.Application
{
    public interface IBackOfficeApplication
    {
        Guid AddApplication(ApplicationViewModel model);
        bool EnableApplication(ApplicationEnableDisable model);
        bool DisableApplication(ApplicationEnableDisable model);
        bool GetApplicationData(string ApplicationName);
        ApplicationTotalCount GetTotalApplicationCount();
        GetTotalApplicationListResponse GetTotalApplicationList(int pageIndex, int pageSize);
        GetTotalActiveApplicationResponse GetTotalActiveApplicationList(int pageIndex, int pageSize);
        // partik patel 08-02-2019 for optimization
        GetTotalActiveApplicationResponse GetTotalActiveApplicationListV1(int pageIndex, int pageSize);
        GetTotalDisactiveApplicationResponse GetTotalDisActiveApplicationList(int pageIndex, int pageSize);
        List<GetApplicationData> GetAllApplicationData();
        bool CheckValidMasterApplication(Guid Id);
        Guid UserWiseAddApplication(CreateUserWiseApplication model);
        GetUserWiseAppData GetUserWiseAppdataDet(Guid Id);
        Guid UpdateUserWiseApplication(UpdateUserGUIDWiseAppData model, int UserId);
        GetTotalUserApplicationListResponse GetUserApplicationList(int pageIndex, int pageSize,int UserId);
        bool CheckUserWiseAppData(Guid DomainId,Guid AppId,string appName,long UserId);
    }
}
