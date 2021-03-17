using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class GetAllLiquidityAPIManager : BizResponseClass
    {
        public List<LiquidityAPIManagerData> Response { get; set; }
    }

    public class GetLiquidityAPIManager : BizResponseClass
    {
        public LiquidityAPIManagerData Response { get; set; }
    }

    public class LiquidityAPIManagerData
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long APIProviderId { get; set; }
        public long LimitId { get; set; }
        public long ProviderMasterId { get; set; }
        public long DeamonConfigId { get; set; }
        public long ServiceProviderCongigId { get; set; }
        public long ProviderTypeId { get; set; }
        public int TransationType { get; set; }
        public string TransactionTypeName { get; set; }
        public short Status { get; set; }
        public string StatusText { get; set; }
    }

    public class GetAllLiquidityAPIManagerArbitrage : BizResponseClass
    {
        public List<LiquidityAPIManagerDataArbitrage> Response { get; set; }
        public long TotalPage { get; set; }
        public long PageSize { get; set; }
        public long Count { get; set; }
    }

    public class GetLiquidityAPIManagerArbitrage : BizResponseClass
    {
        public LiquidityAPIManagerDataArbitrage Response { get; set; }
    }

    public class LiquidityAPIManagerDataArbitrage
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long APIProviderId { get; set; }
        public long ServiceProviderCongigId { get; set; }
        public long ProviderTypeId { get; set; }
        public short Status { get; set; }
        public long ProviderMasterId { get; set; }
        public long Trntype { get; set; }
        public List<AllowOrderType> OrderType { get; set; }
    }

    public class GetAllLiquidityAPIProviderManagerArbitrage : BizResponseClass
    {
        public List<LiquidityAPIProviderManagerArbitrage> Response { get; set; }
        public long TotalPage { get; set; }
        public long PageSize { get; set; }
        public long Count { get; set; }
    }

    public class GetLiquidityAPIProviderManagerArbitrage : BizResponseClass
    {
        public LiquidityAPIProviderManagerArbitrage Response { get; set; }
    }


    public class LiquidityAPIProviderManagerArbitrage
    {
        public long Id { get; set; }
        public string AppKey { get; set; }
        public string APISecret { get; set; }
        public string APIKey { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public short Status { get; set; }
    }
}
