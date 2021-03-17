using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class MostActiveIPAddressResponse:BizResponseClass
    {
        public List<MostActiveIPDetailsInfo> Response { get; set; }
    }
    public class MostActiveIPDetailsInfo
    {
        public string UserName { get; set; }
        public string EmailID { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public string IPAddress { get; set; }
        public string Path { get; set; }
        public string Host { get; set; }
        public short WhitelistIP { get; set; }
    }
    [NotMapped]
    public class MostActiveIPDetailsQryRes
    {
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public string IPAddress { get; set; }
        public string Path { get; set; }
        public string Host { get; set; }
        public short WhitelistIP { get; set; }
    }
    public class MostActiveIPCountQryRes
    {
        public long CreatedBy { get; set; }
        public string IPAddress { get; set; }
    }
}
