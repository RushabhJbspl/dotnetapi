using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Communication;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class CommServiceTypeRes: BizResponseClass
    {
       public List<CommServiceTypeMaster> Response { get; set; }
    }
}
