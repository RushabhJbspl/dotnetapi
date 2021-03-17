using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class LiquidityAPIManagerRequest
    {
        public long APIProviderId { get; set; }
        public long LimitId { get; set; }
        public long ProviderMasterId { get; set; }
        public long DeamonConfigId { get; set; }
        public long ServiceProviderCongigId { get; set; }
        public int[] TransationType { get; set; }
        public long ProviderTypeId { get; set; }
        public short Status { get; set; }
    }

    public class LiquidityAPIManagerUpdateRequest
    {
        public long Id { get; set; }
        public long APIProviderId { get; set; }
        public long LimitId { get; set; }
        public long ProviderMasterId { get; set; }
        public long DeamonConfigId { get; set; }
        public long ServiceProviderCongigId { get; set; }
        public int TransationType { get; set; }
        public long ProviderTypeId { get; set; }
        public short Status { get; set; }
    }

    public class LiquidityAPIManagerArbitrageUpdateRequest
    {
        public long Id { get; set; }
        public long APIProviderId { get; set; }
        public long ServiceProviderCongigId { get; set; }
        public long ProviderTypeId { get; set; }
        public short Status { get; set; }
        public long ProviderMasterId { get; set; }
        public long Trntype { get; set; }
        public List<AllowOrderType> OrderType { get; set; }
    }

    public class AllowOrderType
    {
        public long OrderType { get; set; }
        public long ProviderDetailID { get; set; }
        public short Status { get; set; }
    }

    public class LiquidityAPIProviderManagerArbitrageUpdateRequest
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AppKey { get; set; }
        public string APISecret { get; set; }
        public string APIKey { get; set; }
        public short Status { get; set; }
    }
}
