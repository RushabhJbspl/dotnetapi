using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ViewModels.ControlPanel
{
    public class CreateTextFileReq
    {
        [Required(ErrorMessage = "1,Please Enter Coin Name,31001")]
        public string Coin { get; set; }
        [Required(ErrorMessage = "1,Please Enter AccessToken,31002")]
        public string AccessToken { get; set; }
        public List<UrlReqList> UrlReqList {get;set;}
    }
    public class UrlReqList
    {
        public string Url { get; set; }
        public short UrlType { get; set; }
        public string ReqType { get; set; }
        public string RequestBody { get; set; }
        public int IsDescending { get; set; }
    }
}
