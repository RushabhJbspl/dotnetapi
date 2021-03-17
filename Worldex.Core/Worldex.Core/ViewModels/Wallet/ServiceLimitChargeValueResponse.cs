using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Wallet
{
    public class ServiceLimitChargeValueResponse : BizResponseClass
    {
        public ServiceLimitChargeValue response { get; set; }
    }
    public class ServiceLimitChargeValue
    {
        public enWalletTrnType TrnType { get; set; }
        public string CoinName { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmount { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxAmount { get; set; }
        public enChargeType ChargeType { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChargeValue { get; set; }

        public string DeductWalletTypeName { get; set; } = "";
        public decimal ChargeWalletBalance { get; set; }
    }

    public class ServiceLimitChargeValueResponseStr : BizResponseClass
    {
        public ServiceLimitChargeValueStr response { get; set; }
    }

    public class ServiceLimitChargeValueStr
    {
        public enTrnType TrnType { get; set; }
        public string CoinName { get; set; }
        //[Range(0, 9999999999.99999999), Column(TypeName = "decimal(18, 8)")]
        public string MinAmount { get; set; }
        //[Range(0, 9999999999.99999999), Column(TypeName = "decimal(18, 8)")]
        public string MaxAmount { get; set; }
        public enChargeType ChargeType { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChargeValue { get; set; }
        public string DeductWalletTypeName { get; set; } = "";

        public decimal ChargeWalletBalance { get; set; }
    }
}
