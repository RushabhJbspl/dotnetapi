using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Wallet
{
    public class CreateWalletRequest
    {
        //vsolanki 10-10-2018 
        [Required(ErrorMessage = "1,Please Enter Required parameters,4207")]
        [StringLength(250, ErrorMessage = "1,Please enter a valid  parameters,4208")]
        public string WalletName { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required parameters,4209")]
        //[StringLength(6, MinimumLength = 6,ErrorMessage = "1,Please Enter Valid OTP,4026")]
        public long OTP { get; set; }

        public byte IsDefaultWallet { get; set; }

        public int[] AllowTrnType { get; set; }

        public long OrgID { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
