using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.BackOffice
{
  public   class IPRangeViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please Enter StartIp,8030")]
        [StringLength(20, ErrorMessage = "1,Please Enter valid StartIp,8031")]
        public string StartIp { get; set; }

        [Required(ErrorMessage = "1,Please Enter EndId,8032")]
        [StringLength(20, ErrorMessage = "1,Please Enter valid EndId,8033")]
        public string EndIp { get; set; }
    }
    public  class IPRangeAddViewModel
    {
        public string StartIp { get; set; }
        public string EndIp { get; set; }
        public int UserId  { get; set; }
    }

    public class IPRangeGetDataViewModel
    {
        public string StartIp { get; set; }
        public string EndIp { get; set; }
        public Guid Id { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class IPRangeResponse : BizResponseClass
    {

    }
    public class IPRangeGetdataResponse : BizResponseClass
    {
        public List<IPRangeGetDataViewModel> IPRangeGet { get; set; }
        public int TotalCount { get; set; }
    }

}
