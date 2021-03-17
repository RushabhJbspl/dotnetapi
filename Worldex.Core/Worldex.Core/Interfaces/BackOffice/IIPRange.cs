using Worldex.Core.ViewModels.BackOffice;
using System;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.BackOffice
{
    public interface IIPRange
    {
        Guid AddIPRange(IPRangeAddViewModel IPRangeAddViewModel );
        Guid ISIPRangeExist(IPRangeAddViewModel IPRangeViewModel);
        Guid DeleteRange(IPRangeDeleteReqViewModel IPRangeDelete);
        List<IPRangeDataviewmodel> GetUserWiseIPRange(int userid);
        bool IPAddressinrange(string IPAddess, int userid);
        IPRangeGetdataResponse GetIPRange(int PageIndex = 0, int Page_Size = 0);
    }
}
