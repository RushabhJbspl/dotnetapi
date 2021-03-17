using Worldex.Core.ApiModels;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class APIPlanConfigurationCountResponse : BizResponseClass
    {
        public APIPlanConfigurationCountQryRes Response { get; set; }
    }
    public class APIPlanConfigurationCountQryRes
    {
        public long APIPlanCount { get; set; }
        public long SubscriptionCount { get; set; }
        public long PlanConfigHistoryCount { get; set; }
        public long KeyCount { get; set; }
        public long APIKeyPolicyCount { get; set; }
        public long APIMethodCount { get; set; }
        public long IPWiseRequestCount { get; set; }
    }
}
