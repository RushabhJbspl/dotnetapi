using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class ViewPublicAPIKeysRequest
    {
        public long? UserID { get; set; }
        public long? PlanID { get; set; }
        public short? Status { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public long? Pagesize { get; set; }
        public long? PageNo { get; set; }
    }
    public class ViewPublicAPIKeysResponse : BizResponseClass
    {
        public long TotalCount { get; set; }
        public List<ViewPublicAPIKeysInfo> Response { get; set; }
    }
    public class ViewPublicAPIKeysInfo
    {
        public long Id { get; set; }
        public long UserID { get; set; }
        public string PlanName { get; set; }
        public string AliasName { get; set; }
        public short APIPermission { get; set; }
        public short IPAccess { get; set; }
        public short Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<APIKeysDetailsInfo> KeyDetails { get; set; }
    }
    public class ViewPublicAPIKeysInfoQryRes
    {
        public long Id { get; set; }
        public long UserID { get; set; }
        public string PlanName { get; set; }
        public long APIPlanMasterID { get; set; }
        public string AliasName { get; set; }
        public short APIPermission { get; set; }
        public short IPAccess { get; set; }
        public short Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
    public class APIKeysDetailsInfo
    {
        public string AliasName { get; set; }
        public string IPAddress { get; set; }
        public short IPType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class APIKeysDetailsInfoQryRes
    {
        public string AliasName { get; set; }
        public string IPAddress { get; set; }
        public short IPType { get; set; }
        public DateTime CreatedDate { get; set; }
        public long APIKeyID { get; set; }

    }
}
