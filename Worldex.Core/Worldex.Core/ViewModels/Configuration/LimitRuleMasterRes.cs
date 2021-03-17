using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class LimitRuleMasterRes
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
    }

    public class ListLimitRuleMasterRes : BizResponseClass
    {
        public List<LimitRuleMasterRes> LimitRuleObj { get; set; }
    }

    public class LimitRuleMasterReq
    {
        [Required(ErrorMessage = "1,Enter Required Parameter Value,4708")]
        public string Name { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4709")]
        [EnumDataType(typeof(enTrnType), ErrorMessage = "1,Invalid Parameter Value,4710")]
        public enTrnType TrnType { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4711")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency, ErrorMessage = "1,Enter Required Parameter Value,4712")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4713")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency, ErrorMessage = "1,Enter Required Parameter Value,4714")]
        public decimal MaxAmount { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4715")]
        public long WalletType { get; set; }
    }
}
