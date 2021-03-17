using Worldex.Core.ApiModels;
using System;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class PublicAPIKeyPolicyRequest
    {
        public long ID { get; set; }
        public int AddMaxLimit { get; set; }
        public int AddPerDayFrequency { get; set; }
        public int AddFrequency { get; set; }
        public int AddFrequencyType { get; set; }
        public int DeleteMaxLimit { get; set; }
        public int DeletePerDayFrequency { get; set; }
        public int DeleteFrequency { get; set; }
        public int DeleteFrequencyType { get; set; }
        public short Status { get; set; }
    }
    public class PublicAPIKeyPolicyResponse : BizResponseClass
    {
        public PublicAPIKeyPolicyResponseInfo Response { get; set; }
    }
    public class PublicAPIKeyPolicyResponseInfo
    {
        public long ID { get; set; }
        public int AddMaxLimit { get; set; }
        public int AddPerDayFrequency { get; set; }
        public int AddFrequency { get; set; }
        public int AddFrequencyType { get; set; }
        public int DeleteMaxLimit { get; set; }
        public int DeletePerDayFrequency { get; set; }
        public int DeleteFrequency { get; set; }
        public int DeleteFrequencyType { get; set; }
        public short Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
