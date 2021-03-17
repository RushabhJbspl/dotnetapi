using Worldex.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.WalletConfiguration
{
    public class CommPolicyReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4261")]
        public string PolicyName { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4210")]
        [EnumDataType(typeof(enWalletTrnType), ErrorMessage = "1,Fail,4273")]
        public enWalletTrnType WalletTrnType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4268")]
        public long WalletType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4269")]
        public long Type { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4274")]
        public long CommType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4275")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal CommValue { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4264")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4266")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaxAmount { get; set; }
    }

    public class UpdateCommPolicyReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4261")]
        public string PolicyName { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4269")]
        public long Type { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4274")]
        public long CommType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4275")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal CommValue { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4264")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4266")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaxAmount { get; set; }
    }
}
