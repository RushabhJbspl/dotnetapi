using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.ApiModels;

namespace Worldex.Core.ViewModels.ControlPanel
{
    public class UserWithdrawalResponse : BizResponseClass
    {
        public List<WithdrawalDetail> SuccessTransactions { get; set; }
        public List<WithdrawalDetail> HoldTransactions { get; set; }
    }
    public class WithdrawalDetail
    {
        public string CurrencyName { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal ExternalTrnTotalAmt { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal InternalTrnTotalAmt { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TotalAmount { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal EquivalentUSDAmt { get; set; }
    }

    public class ListUserWithdrawalDTO
    {
        public List<UserWithdrawalDTO> Internal { get; set; }
        public List<UserWithdrawalDTO> External { get; set; }
    }

    public class UserWithdrawalDTO
    {
        public string SMSCode { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal TotalAmount { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal EquivalentUSDAmt { get; set; }
    }

    public class UserWithdrawalDTO2
    {
        public string SMSCode { get; set; }
        public decimal InternalAmount { get; set; }
        public decimal ExternalAmount { get; set; }
    }


    public class UserAllWalletBalanceResp : BizResponseClass
    {
        public List<UserWalletBalance> MainWallet { get; set; }
        public List<UserWalletBalance> ArbitrageWallet { get; set; }
    }

    public class UserWalletBalance
    {
        public string WalletType { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MainBalance { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal OutBoundBalance { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal EquivalentUSDAmt { get; set; }
    }

    public class UserWalletBalDTO
    {
        public string WalletTypeName { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MainBalance { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal OutBoundBalance { get; set; }
        [Column(TypeName = "decimal(28, 18)")]
        public decimal EquivalentUSDAmt { get; set; }
    }

    public class UserTradingSummaryResp : BizResponseClass
    {
        public List<UserTradingSummary> BuyTrade { get; set; }
        public List<UserTradingSummary> SellTrade { get; set; }
    }

    public class UserTradingSummary
    {
        public string PairName { get; set; }
        public string OrderCurrency { get; set; }
        public string DeliveryCurrency { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal TotalOrderQty { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal TotalOrderQtyInUSD { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal TotalDeliveryQty { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal TotalDeliveryQtyInUSD { get; set; }
    }

    public class UserTradingSummaryDTO
    {
        public string PairName { get; set; }
        public string Order_Currency { get; set; }
        public string Delivery_Currency { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal TotalOrderQty { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal TotalOrderQtyUSD { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal TotalDeliveryQty { get; set; }

        [Column(TypeName = "decimal(28, 18)")]
        public decimal TotalDeliveryQtyUSD { get; set; }
    }





    public class SuspiciousUserActivityResp : BizResponseClass
    {
        public SuspiciousUserPersonalInfo SuspiciousUserPersonalInfo { get; set; }

        public List<UserActivityReportActConfirmedEmailResp.UserActivityReportConfEmailResp> PasswordChangeData { get; set; }

        public List<UserActivityReportActConfirmedEmailResp.UserActivityReportConfEmailResp> RefferedUsers { get; set; }

        public List<UserActivityReportConfEmailResp2> DeviceChange { get; set; }
    }

    public class UserActivityReportActConfirmedEmailResp : BizResponseClass
    {
        public List<UserActivityReportConfEmailResp> Response { get; set; }

        public class UserActivityReportConfEmailResp
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Email { get; set; }

            public string MobileNo { get; set; }

            public string Date { get; set; }
        }
    }

    public class UserActivityReportConfEmailResp2
    {
        public Int64 Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string MobileNo { get; set; }

        public string Date { get; set; }

        public string IPAddress { get; set; }

        public string Location { get; set; }
        public string Device { get; set; }
    }

    public class SuspiciousUserPersonalInfo
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string MemberStatus { get; set; }
        public string KYCStatus { get; set; }
        public string KYCDate { get; set; }
        public string EmailConfirmStatus { get; set; }
        public string EmailConfirmDate { get; set; }

        public string ReferredBy { get; set; }

        public string CreatedDate { get; set; }
    }

    public class CurrencyCummulativeData : BizResponseClass
    {
        public List<CurrencyCummulativeDataAct> Data { get; set; }
        public class CurrencyCummulativeDataAct
        {
            public string Currency { get; set; }
            public decimal DepositeQty { get; set; }
            public decimal WithdrawQty { get; set; }
            public decimal UserQty { get; set; }
            public decimal AdminQty { get; set; }

            public decimal ArbitrageUserQty { get; set; }
            public decimal MarginUserQty { get; set; }
            public decimal MarginAdminQty { get; set; }
        }
    }
}
