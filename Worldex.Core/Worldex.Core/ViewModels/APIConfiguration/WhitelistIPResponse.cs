using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class APIKeyListPResponseV2 : BizResponseClass
    {
        public APIKeyListPResponseInfo Response { get; set; }
    }
    public class APIKeyListPResponse : BizResponseClass
    {
        public long APIKeyLimit { get; set; }
        public long APIKeyCount { get; set; }
        public List<APIKeyListPResponseInfo> Response { get; set; }
    }
    public class APIKeyListPResponseInfo
    {
        public long KeyId { get; set; }
        public string AliasName { get; set; }
        public string APIKey { get; set; }
        public string SecretKey { get; set; }
        public DateTime CreatedDate { get; set; }
        public short APIAccess { get; set; }
        public short IPAccess { get; set; }
        public string QRCode { get; set; }
        public List<WhitelistIPList> IPList { get; set; }
    }
    public class WhitelistIPList
    {
        public long IPId { get; set; }
        public string AliasName { get; set; }
        public string IPAddress { get; set; }
        public short IPType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class WhitelistIPListResponse : BizResponseClass
    {
        public long IPLimitCount { get; set; }
        public long IPCount { get; set; }
        public List<WhitelistIPList> Response { get; set; }
    }
    public class GetKeyWiseIPQryRes
    {
        public long APIKeyID { get; set; }
        public long ID { get; set; }
        public string IPAddress { get; set; }
        public string AliasName { get; set; }
        public short IPType { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
