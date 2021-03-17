using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Organization
{
    public class ActivityType_Master : BizBaseExtended
    {
        [StringLength(4000)]
        public string TypeMaster { get; set; }

        [StringLength(1000)]
        public string AliasName { get; set; }

        public bool IsDelete { get; set; }
    }
}
