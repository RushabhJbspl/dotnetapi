using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.ControlPanel
{
    public class ServiceProviderBalanceResponse : BizResponseClass
    {
        public List<ServiceProviderBalance> Data { get; set; }
    }
    public class ServiceProviderBalance
    {
        public string CurrencyName { get; set; }
        public string Address { get; set; }
        public decimal Balance { get; set; }
        public decimal Fee { get; set; }
    }

    public class ServiceProviderBalanceResponsev2 : BizResponseClass
    {
        public List<ServiceProviderBalance> Data { get; set; }
    }


    public class LPServiceProBalanceResponse : BizResponseClass
    {
        public List<LPServiceProviderBalance> Data { get; set; }
    }
    public class LPServiceProviderBalance
    {
        public string CurrencyName { get; set; }
        public string ProviderName { get; set; }
        public decimal Balance { get; set; }
        public decimal WalletBalance { get; set; }
    }


    public class ArbitrageServiceProBalanceResponse : BizResponseClass
    {
        public List<ArbitrageServiceProviderBalance> Data { get; set; }
    }
    public class ArbitrageServiceProviderBalance
    {
        public string CurrencyName { get; set; }
        public string ProviderName { get; set; }
        public decimal Balance { get; set; }
        public decimal WalletBalance { get; set; }
    }
}

