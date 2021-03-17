using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.KYC
{
    public class KYCListFilterationViewModel
    {
        public int Status { get; set; }
        public string Mobile { get; set; }
        public string EmailAddress { get; set; }
    }
    public class KYCListFilterationDataViewModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public string ValidIdentityCard { get; set; }
        public string ValidIDCardNumber { get; set; }
        public string FrontImage { get; set; }
        public string BackImage { get; set; }
        public string SelfieImage { get; set; }
        public int VerifyStatus { get; set; }
        public string Statusname { get; set; }
        public DateTime Createddate { get; set; }
    }


    public class KYCListFilterationDataListResponseViewModel : BizResponseClass
    {
        public List<KYCListFilterationDataViewModel> kYCListFilterationDataViewModels { get; set; }
        public int TotalCount { get; set; }
    }

    public class KYCUpdateViewModel
    {
        [Required(ErrorMessage = "1,Please Enter KYC Verification Id,8028")]
        public long id { get; set; }
        public int VerifyStatus { get; set; }
        public string Remark { get; set; }
    }
}
