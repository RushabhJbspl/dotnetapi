using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Organization
{
    public class ActivityRegisterViewModel 
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(500)]
        public string Remark { get; set; }

        [StringLength(500)]
        public string Connection { get; set; }

        public Guid ApplicationId { get; set; }

        public long StatusCode { get; set; }

        [StringLength(500)]
        public string Channel { get; set; }

        [StringLength(2000)]
        public string DeviceId { get; set; }

        [StringLength(4000)]
        public string IPAddress { get; set; }

        public long ReturnCode { get; set; }

        [StringLength(8000)]
        public string ReturnMsg { get; set; }

        public long ErrorCode { get; set; }

        public string ActivityType { get; set; }

        [StringLength(4000)]
        public string Session { get; set; }

        [StringLength(4000)]
        public string AccessToken { get; set; }

        [StringLength(1000)]
        public string AliasName { get; set; }

        public string ModuleTypeName { get; set; }

        public string HostURLName { get; set; }
    }

    public class ActivityRegisterResponse : BizResponseClass
    {

    }

    public class GetActivityLogData1
    {
        public Guid Id { get; set; }

        public string Remark { get; set; }

        public string Connection { get; set; }

        public Guid ApplicationId { get; set; }

        public long StatusCode { get; set; }

        public string DeviceId { get; set; }

        public string IPAddress { get; set; }

        public long ReturnCode { get; set; }

        public string ReturnMsg { get; set; }

        public long ErrorCode { get; set; }

        public string Session { get; set; }

        public string AccessToken { get; set; }

        public string AliasName { get; set; }

        public string ActivityType { get; set; }

        public string ModuleTypeName { get; set; }

        public string HostURLName { get; set; }

        public string Request { get; set; }

        public string Response { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        public long CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedDate { get; set; }
    }

    public class GetActivityLogResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<GetActivityLogData> GetActivityLogList { get; set; }
    }


    public class GetActivityLogData
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Remark { get; set; }


        public string DeviceId { get; set; }

        public string IPAddress { get; set; }

        public string AliasName { get; set; }

        public string ActivityType { get; set; }

        public string ModuleTypeName { get; set; }


        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        public long CreatedBy { get; set; }

    }

    public class GetModuleData
    {
        public long Id { get; set; }
        public string ModuleName { get; set; }
    }

    public class GetModuleDataResponse : BizResponseClass
    {
        public List<GetModuleData> GetModuleData { get; set; }
    }

}
