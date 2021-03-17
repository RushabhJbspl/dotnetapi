using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class ChargeRuleMasterRes
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public enTrnType TrnType { get; set; }
        public string StrTrnType { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public string WalletTypeName { get; set; }
        public long WalletType { get; set; }
        public short Status { get; set; }
        public string StatusStr { get; set; }
        public decimal ChargeValue { get; set; }
        public string ChargeType { get; set; }
    }
    public class ListChargeRuleMasterRes : BizResponseClass
    {
        public List<ChargeRuleMasterRes> ChargeRuleObj { get; set; }
    }

    public class ChargeRuleMasterReq
    {
        [Required(ErrorMessage = "1,Enter Required Parameter Value,4716")]
        public string Name { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4717")]
        [EnumDataType(typeof(enTrnType), ErrorMessage = "1,Invalid Parameter Value,4718")]
        public enTrnType TrnType { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4719")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency, ErrorMessage = "1,Enter Required Parameter Value,4720")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4721")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency, ErrorMessage = "1,Enter Required Parameter Value,4722")]
        public decimal MaxAmount { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4723")]
        public long WalletType { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4724")]
        [EnumDataType(typeof(enChargeType), ErrorMessage = "1,Invalid Parameter Value,4725")]
        public enChargeType ChargeType { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4726")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Required Parameter Value,4727")]
        public decimal ChargeValue { get; set; }
    }
}
