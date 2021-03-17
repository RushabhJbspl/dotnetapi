using Worldex.Core.ApiModels;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Complaint
{
    public class CompainTrailVirewModel
    {
        [Required(ErrorMessage = "1,Please Enter complaint description,4117")]
        public string Description { get; set; }
        public string Remark { get; set; }
        [Required(ErrorMessage = "1,Please Enter complaint Complainstatus,4120")]
        public string Complainstatus { get; set; }
        public long ComplainId { get; set; }
    }
    public class CompainTrailReqVirewModel
    {
        [Required(ErrorMessage = "1,Please Enter complaint description,4117")]
        public string Description { get; set; }
        public string Remark { get; set; }
        [Required(ErrorMessage = "1,Please Enter complaint Complainstatus,4120")]
        public string Complainstatus { get; set; }
        public long ComplainId { get; set; }
        public int UserID { get; set; }
    }

    public class CompainTrailResponse : BizResponseClass
    { }
}
