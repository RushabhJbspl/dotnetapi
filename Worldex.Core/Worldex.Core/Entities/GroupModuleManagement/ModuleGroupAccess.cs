using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.GroupModuleManagement
{
    public class ModuleGroupAccess : BizBase
    {
        // Group master's unique id will associate with it.
        public int GroupID { get; set; }

        // It will store Tool options like Import=8B92994B or Export=D42A3D17
        // For Default NULL-2D3D4DOK ID will be USE. - Nishit Jani on A 2019-03-27 12:53 PM
        // Note that it is array type so multiple value will be stoted with comma (",") separeted value.
        public string UtilityTypes { get; set; } = "100";

        // It will store Crud operation details like Add= or Update=
        // For Default NULL-2D3D4DOK ID will  be Use. -Nishit Jani on A 2019-03-27 12:58 PM
        // Note that it is array type so multiple value will be stoted with comma (",") separeted value.
        public string CrudTypes { get; set; } = "100";

        // It will associate Sub module master table's id with Access table.
        public int SubModuleID { get; set; }       

    }
}
