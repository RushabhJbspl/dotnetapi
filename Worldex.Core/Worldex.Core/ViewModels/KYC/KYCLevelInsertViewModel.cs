using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.KYC
{
    public class KYCLevelInsertViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter KYCName,8024")]
        public string KYCName { get; set; }
        public int Level { get; set; }
    }
    public class KYCLevelUpdateViewModel : TrackerViewModel
    {
        public long Id { get; set; }
        public short status { get; set; }
        [Required(ErrorMessage = "1,Please Enter KYCName,8024")]
        public string KYCName { get; set; }
        public int Level { get; set; }
    }
    public class KYCLevelUpdateReqViewModel : TrackerViewModel
    {
        public long Id { get; set; }
        public short status { get; set; }
        public string KYCName { get; set; }
        public int Level { get; set; }
        public int UserId { get; set; }
    }
    public class KYCLevelWiseCount
    {
        public int Count { get; set; }
    }
    public class KYCLevelInsertReqViewModel : TrackerViewModel
    {
        public string KYCName { get; set; }
        public int Level { get; set; }
        public int Userid { get; set; }
    }
    public class KYCLevelCountModel 
    {
        public int Count { get; set; }
    }
    public class KYCLevelResponseViewModel : BizResponseClass
    {
        public int Count { get; set; }
    }
}
