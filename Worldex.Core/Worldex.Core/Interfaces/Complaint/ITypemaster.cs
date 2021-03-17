using Worldex.Core.ViewModels.Complaint;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.Complaint
{
    public interface ITypemaster
    {
        List<TypemasterViewModel> GettypeMaster(string Type);
    }
}
