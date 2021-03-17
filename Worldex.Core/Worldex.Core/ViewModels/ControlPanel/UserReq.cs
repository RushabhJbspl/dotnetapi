using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ViewModels.ControlPanel
{
    public class UserReq
    {
    }
    public class BlockWalletTrnTypeReq
    {
        [Required(ErrorMessage = "1,Enter Required Parameters,4248")]
        public long WalletTypeID { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4249")]
        public long TrnTypeID { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4250")]
        public short Status { get; set; }
    }

    public class ChargePolicyReq
    {
        [Required(ErrorMessage = "1,Enter Required Parameters,4261")]
        [StringLength(50, ErrorMessage = "1,Enter Valid Parameters,4262")]
        public string PolicyName { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4263")]
        public long WalletTrnType { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4264")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4265")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4266")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4267")]
        public decimal MaxAmount { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4268")]
        public long WalletType { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4269")]
        public long Type { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4270")]
        public long ChargeType { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4271")]
        public decimal ChargeValue { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4272")]
        public short Status { get; set; }
    }

    public class UpdateChargePolicyReq
    {
        [Required(ErrorMessage = "1,Enter Required Parameters,4261")]
        [StringLength(50, ErrorMessage = "1,Enter Valid Parameters,4262")]
        public string PolicyName { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4264")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4265")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4266")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4267")]
        public decimal MaxAmount { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4269")]
        public long Type { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4270")]
        public long ChargeType { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4271")]
        public decimal ChargeValue { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4272")]
        public short Status { get; set; }
    }

    public class AddTransactionPolicyReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4249")]
        public long TrnType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4283")]
        public string AllowedIP { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4284")]
        public string AllowedLocation { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4285")]
        public int AuthenticationType { get; set; }

        public double? StartTime { get; set; }

        public double? EndTime { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4286")]
        public long DailyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4287")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4888")]
        public decimal DailyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4289")]
        public long MonthlyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4290")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4291")]
        public decimal MonthlyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4292")]
        public long WeeklyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4293")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4294")]
        public decimal WeeklyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4295")]
        public long YearlyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4296")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4296")]
        public decimal YearlyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4264")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4265")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4266")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4267")]
        public decimal MaxAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4298")]
        public short AuthorityType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4299")]
        public short AllowedUserType { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4272")]
        public short Status { get; set; }

        public long RoleId { get; set; }

        public short IsKYCEnable { get; set; }

    }

    public class UpdateTransactionPolicyReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4283")]
        public string AllowedIP { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4284")]
        public string AllowedLocation { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4285")]
        public int AuthenticationType { get; set; }

        public double? StartTime { get; set; }

        public double? EndTime { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4286")]
        public long DailyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4287")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4888")]
        public decimal DailyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4289")]
        public long MonthlyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4290")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4291")]
        public decimal MonthlyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4292")]
        public long WeeklyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4293")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4294")]
        public decimal WeeklyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4295")]
        public long YearlyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4296")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4296")]
        public decimal YearlyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4264")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4265")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4266")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4267")]
        public decimal MaxAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4298")]
        public short AuthorityType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4299")]
        public short AllowedUserType { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4272")]
        public short Status { get; set; }
    }

    public class TransactionPolicyRes
    {
        public long Id { get; set; }
        public long TrnType { get; set; }
        public string StrTrnType { get; set; }
        public string AllowedIP { get; set; }
        public string AllowedLocation { get; set; }
        public int AuthenticationType { get; set; }
        public double? StartTime { get; set; }
        public double? EndTime { get; set; }
        public long DailyTrnCount { get; set; }
        public decimal DailyTrnAmount { get; set; }
        public long MonthlyTrnCount { get; set; }
        public decimal MonthlyTrnAmount { get; set; }
        public long WeeklyTrnCount { get; set; }
        public decimal WeeklyTrnAmount { get; set; }
        public long YearlyTrnCount { get; set; }
        public decimal YearlyTrnAmount { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public short AuthorityType { get; set; }
        public short AllowedUserType { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public short IsKYCEnable { get; set; }
    }

    public class ListTransactionPolicyRes : BizResponseClass
    {
        public List<TransactionPolicyRes> Data { get; set; }
    }

    public class GetTransactionPolicyRes : BizResponseClass
    {
        public TransactionPolicyRes Data { get; set; }
    }

    public class ListUserWalletBlockTrnType : BizResponseClass
    {
        public List<UserWalletBlockTrnTypeRes> Data { get; set; }
    }

    public class UserWalletBlockTrnTypeRes
    {
        public long Id { get; set; }
        public long TrnTypeId { get; set; }
        public string TrnTypeName { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public string WalletId { get; set; }
        public string WalletName { get; set; }
        public string WalletType { get; set; }
        public string Remarks { get;set; }
    }

    public class InsertUpdateUserWalletBlockTrnTypeReq
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4305")]
        public string WalletId { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4249")]
        public long WTrnTypeMasterID { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4250")]
        public short Status { get; set; }

        public string Remarks { get; set; }
    }

    public class ListWalletAuthorizeUserRes : BizResponseClass
    {
        public List<WalletAuthorizeUserRes> Data { get; set; }
    }

    public class WalletAuthorizeUserRes
    {
        public string WalletName { get; set; }
        public string OrgName { get; set; }
        public string WalletType { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public string UserName { get; set; }
        public string UserRoleName { get; set; }
    }

    public class CommisssionTypeReq
    {
        public long TypeId { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4307")]
        public string TypeName { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4250")]
        public short Status { get; set; }
    }

    public class AddWalletUsagePolicyReq
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4268")]
        public long WalletType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4261")]
        [StringLength(50, ErrorMessage = "1,Enter Valid Parameters,4262")]
        public string PolicyName { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4283")]
        public string AllowedIP { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4284")]
        public string AllowedLocation { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4285")]
        public int AuthenticationType { get; set; }

        public double? StartTime { get; set; }

        public double? EndTime { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4286")]
        public long DailyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4287")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4888")]
        public decimal DailyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4308")]
        public long HourlyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4309")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4310")]
        public decimal HourlyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4289")]
        public long MonthlyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4290")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4291")]
        public decimal MonthlyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4292")]
        public long WeeklyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4293")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4294")]
        public decimal WeeklyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4295")]
        public long YearlyTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4296")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4296")]
        public decimal YearlyTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4312")]
        public long LifeTimeTrnCount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4313")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4314")]
        public decimal LifeTimeTrnAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4264")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4265")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4266")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Enter Valid Parameters,4267")]
        public decimal MaxAmount { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4272")]
        public short Status { get; set; }

        public short[] DayNo { get; set; }
    }

    public class ChargeTypeReq
    {
        [Required(ErrorMessage = "1,Enter Required Parameters,4306")]
        public long TypeId { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4307")]
        public string TypeName { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4250")]
        public short Status { get; set; }
    }

    public class InserUpdateAllowTrnTypeRoleReq
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4330")]
        public long RoleId { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4249")]
        public long TrnTypeId { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4272")]
        public short Status { get; set; }
    }

    public class InserUpdateStopLossReq
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4248")]
        public long WalletTypeId { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4411")]
        [Range(0, 9999999999.99999999,ErrorMessage ="1,Enter valid Parameter,4412")]
        public decimal StopLossPer { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4272")]
        public short Status { get; set; }
    }

    public class InserUpdateLeverageReq
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4248")]
        public long WalletTypeId { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4413")]
        [Range(0, 100, ErrorMessage = "1,Enter valid Parameter,4414")]
        public decimal LeveragePer { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4272")]
        public short Status { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4467")]
        public short IsAutoApprove { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4468")]
        [Range(0, 100, ErrorMessage = "1,Enter valid Parameter,4469")]
        public decimal SafetyMarginPer { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4470")]
        [Range(0, 100, ErrorMessage = "1,Enter valid Parameter,4471")]
        public decimal MarginChargePer { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4472")]
        [EnumDataType(typeof(enLeverageChargeDeductionType), ErrorMessage = "1,Enter Valid Param,4473")]
        public enLeverageChargeDeductionType LeverageChargeDeductionType { get; set; }

        //ntrivedi adding  27-03-2019 
        [Required(ErrorMessage = "1,Enter Required Parameters,4472")]
        [Range(0, 100, ErrorMessage = "1,Enter valid Parameter,17024")]
        public decimal InstantChargePer { get; set; }

    }
    public class InsertUpdateStakingPolicyReq
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4248")]
        public long WalletTypeID { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4435")]
        [EnumDataType(typeof(EnStakingSlabType), ErrorMessage = "1,Enter Valid Param,4436")]
        public EnStakingSlabType SlabType { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4437")]
        [EnumDataType(typeof(EnStakingType), ErrorMessage = "1,Enter Valid Param,4438")]
        public EnStakingType StakingType { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4250")]
        public short Status { get; set; }
    }


    public class InsertUpdateDepositCounterReq
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4248")]
        public long WalletTypeID { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4250")]
        public short Status { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4250")]
        public int RecordCount { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4250")]
        public long Limit { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4250")]
        public long MaxLimit { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameters,4250")]
        public long SerProId { get; set; }

        public short IsResetLimit { get; set; }//1=reset to 0 and 0=not reset
        public string LastTrnID { get; set; }
        public string PreviousTrnID { get; set; }
        public string PrevIterationID { get; set; }
        public long TPSPickupStatus { get; set; }
    }
}
