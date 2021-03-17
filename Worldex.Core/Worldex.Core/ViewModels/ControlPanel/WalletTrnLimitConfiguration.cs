using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.ControlPanel
{

    public class WalletTrnLimitConfigurationInsReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4486")]
        [EnumDataType(typeof(enWalletLimitType), ErrorMessage = "1,Fail,17003")]
        public enWalletLimitType TrnType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4487")]
        public long WalletType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4488")] 
        [EnumDataType(typeof(EnIsKYCEnable), ErrorMessage = "1,Fail,17004")]
        public EnIsKYCEnable IsKYCEnable { get; set; }

        public double? StartTime { get; set; }

        public double? EndTime { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4489")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal PerTranMinAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4490")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal PerTranMaxAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4491")]
        public long HourlyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4492")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal HourlyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4493")]
        public long DailyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4494")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DailyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4495")]
        public long WeeklyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4496")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal WeeklyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4497")]
        public long MonthlyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4498")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MonthlyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4499")]
        public long YearlyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17000")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal YearlyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17005")]
        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Fail,17006")]
        public ServiceStatus Status { get; set; }
    }

    public class WalletTrnLimitConfigurationUpdReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,17007")]
        public long Id { get; set; }

        public double? StartTime { get; set; }

        public double? EndTime { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4489")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal PerTranMinAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4490")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal PerTranMaxAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4491")]
        public long HourlyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4492")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal HourlyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4493")]
        public long DailyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4494")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DailyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4495")]
        public long WeeklyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4496")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal WeeklyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4497")]
        public long MonthlyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4498")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MonthlyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4499")]
        public long YearlyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17000")]
        [Range(0, 9999999999.99999999), Column(TypeName = "decimal(28, 18)")]
        public decimal YearlyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17005")]
        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Fail,17006")]
        public ServiceStatus Status { get; set; }
    }

    public class ChangeServiceStatus
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,17007")]
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17005")]
        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Fail,17006")]
        public ServiceStatus Status { get; set; }
    }

    public class GetWalletTrnLimitConfigResp : BizResponseClass
    {
        public WalletTrnLimitConfigResp Data { get; set; }
    }

    public class WalletTrnLimitConfigResp
    {
        public long Id { get; set; }

        public long TrnType { get; set; }

        public string StrTrnType { get; set; }

        public long WalletType { get; set; }

        public string WalletTypeName { get; set; }
        
        public short IsKYCEnable { get; set; }

        public double StartTime { get; set; }

        public double EndTime { get; set; }

        public decimal PerTranMinAmount { get; set; }

        public decimal PerTranMaxAmount { get; set; }
        
        public long HourlyTrnCount { get; set; }
        
        public decimal HourlyTrnAmount { get; set; }
        
        public long DailyTrnCount { get; set; }
        
        public decimal DailyTrnAmount { get; set; }
        
        public long WeeklyTrnCount { get; set; }

        public decimal WeeklyTrnAmount { get; set; }

        public long MonthlyTrnCount { get; set; }

        public decimal MonthlyTrnAmount { get; set; }
        
        public long YearlyTrnCount { get; set; }

        public decimal YearlyTrnAmount { get; set; }

        public short Status { get; set; }
    }

    public class ListWalletTrnLimitConfigResp : BizResponseClass
    {
        public List<WalletTrnLimitConfigResp> Details { get; set; }
    }

}
