using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.WalletOpnAdvanced
{
    public class InsertWalletRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4329")]
        public string WalletID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4330")]
        public long RoleId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4331")]
        [StringLength(1000,ErrorMessage = "1,Please Enter valid Parameter,4332")]
        public string Message { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4333")]
        [StringLength(100,ErrorMessage = "1,Please Enter valid Parameter,4334")]
        public string Email { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4386")]
        public short RequestType{ get; set; } //1-add, 2-remove

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4418")]
        public long ChannelId { get; set; }
    }

    public class InsertColdWalletRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4433")]
        public string WalletLabel { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4434")]
        public string Password { get; set; } 
    }
}
