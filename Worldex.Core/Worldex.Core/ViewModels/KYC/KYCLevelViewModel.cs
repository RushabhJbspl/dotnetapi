using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ViewModels.KYC
{
    public class KYCLevelViewModel : TrackerViewModel
    {
        [StringLength(150)]
        [Required]
        public string KYCName { get; set; }

        public int Level { get; set; }

        public bool EnableStatus { get; set; }

        public bool IsDelete { get; set; }
    }
    public class KYCLevelList
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Level { get; set; }
        public string KYCName { get; set; }
    }
    public class KYCLevelListResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<KYCLevelList> KYCLevelList { get; set; }
    }
    public class KYCLevelDropDownListResponse : BizResponseClass
    {
        public List<KYCLevelList> KYCLevelList { get; set; }
    }
}
