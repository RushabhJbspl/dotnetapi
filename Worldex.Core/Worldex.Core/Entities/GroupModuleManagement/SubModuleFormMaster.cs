using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.GroupModuleManagement
{
    // Following class/table created by Nishit Jani on A 2019-03-27 1:04 PM 
    // To Defile inner Form access rights of Sub Module Master
    public class SubModuleFormMaster : BizBase
    {
        // Primary key of Sub Module Master table
        [Required]
        public int ModuleGroupAccessID { get; set; }

        public string CrudTypes { get; set; }

        // Following is Field master's primary key to define field names
        public string FieldID { get; set; }
        public short Visibility { get; set; } //add foreign key => ModuleVisibilityMaster
    }
}
