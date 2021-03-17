using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Complaint
{
    public class TypemasterViewModel
    {
        public long id { get; set; }
        public string Type { get; set; }
    }

    public class TypeMasterResponse : BizResponseClass
    {
        public List<TypemasterViewModel> TypeMasterList { get; set; }
    }

    public class ComplainStatusTypeModel
    {
        public int StatusId { get; set; }
        public string ComplainStatus { get; set; }
    }

    public class ComplainStatusTypeResponse : BizResponseClass
    {
        public List<ComplainStatusTypeModel> ComplainStatus { get; set; }
    }
}
