using Worldex.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Wallet
{
    public class AddBizUserTypeMappingReq
    {
        [Required(ErrorMessage = "1,Please Enter Parameter, 4234")]
        public long UserID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Parameter, 4235")]
        [EnumDataType(typeof(enUserType),ErrorMessage = "1,Please Enter valid Parameter, 4236")]
        public enUserType UserType { get; set; }
    }
}
