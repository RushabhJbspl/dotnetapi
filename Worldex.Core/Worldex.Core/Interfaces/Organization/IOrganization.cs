using Worldex.Core.ViewModels.Organization;
using System;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.Organization
{
    public interface IOrganization
    {
        Guid AddDomaim(OrganizationViewModel model);
        OrganizationTotalDomainCount GetTotalDomainCount(int userid);
        GetTotalOrganizationResponse GetTotalDomainList(int userid,string UserName, int pageIndex, int pageSize);
        GetTotalOrganizationResponse GetTotalActiveDomainList(int userid, string UserName, int pageIndex, int pageSize);
        GetTotalOrganizationResponse GetTotalDisactiveDomainList(int userid, string UserName, int pageIndex, int pageSize);
        bool EnableDomain(OrganizationViewModel model);
        bool DisActiveDomain(OrganizationViewModel model);
        bool GetDomainByUserwise(string DomainName, int UserId);
        List<GetUserWiseDomainData> GetUserWiseDomainData(int UserId);
        bool CheckValidDomain(Guid Id, int UserId);
    }
}
