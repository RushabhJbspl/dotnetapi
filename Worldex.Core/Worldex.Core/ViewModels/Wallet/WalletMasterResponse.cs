using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Wallet
{
    public class WalletMasterResponse
    {
        public string WalletName { get; set; }
        public decimal Balance { get; set; }
        public string CoinName { get; set; }
        public string AccWalletID { get; set; }
        public string PublicAddress { get; set; }
        public byte IsDefaultWallet { get; set; }
        public decimal OutBoundBalance { get; set; }
        public decimal InBoundBalance { get; set; }
        public long OrgID { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class WalletMasterRes
    {
        public string WalletName { get; set; }
        public decimal Balance { get; set; }
        public string CoinName { get; set; }
        public string AccWalletID { get; set; }
        public string PublicAddress { get; set; }
        public byte IsDefaultWallet { get; set; }
        public decimal OutBoundBalance { get; set; }
        public decimal InBoundBalance { get; set; }
        public long OrgID { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
    }
    //komal 13-09-2019 for getSystem user balance for settledbatch transaction
    public class WalletMasterResponsev2
    {
        public long id { get; set; }
        public string WalletName { get; set; }
        public decimal Balance { get; set; }
        public string CoinName { get; set; }
        public string AccWalletID { get; set; }
        public string PublicAddress { get; set; }
        public byte IsDefaultWallet { get; set; }
        public decimal OutBoundBalance { get; set; }
        public decimal InBoundBalance { get; set; }
        public long OrgID { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
    public class MarginWalletMasterRes
    {
        public string WalletName { get; set; }
        public decimal Balance { get; set; }
        public string CoinName { get; set; }
        public string AccWalletID { get; set; }
        public string PublicAddress { get; set; }
        public byte IsDefaultWallet { get; set; }
        public decimal OutBoundBalance { get; set; }
        public decimal InBoundBalance { get; set; }
        public long OrgID { get; set; }
        public long UserId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public int WalletUsageType { get; set; }
        public string WalletUsageTypeName { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }
    public class MarginWalletMasterRes2
    {
        public string WalletName { get; set; }
        public decimal Balance { get; set; }
        public string CoinName { get; set; }
        public string AccWalletID { get; set; }
        public string PublicAddress { get; set; }
        public byte IsDefaultWallet { get; set; }
        public decimal OutBoundBalance { get; set; }
        public decimal InBoundBalance { get; set; }
        public long OrgID { get; set; }
        public long UserId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public int WalletUsageType { get; set; }
        public string WalletUsageTypeName { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class ListMarginWallet : BizResponseClass
    {
        public List<MarginWalletMasterRes> Data { get; set; }
        //public int TotalCount { get; set; }
        //public int PageNo { get; set; }
        //public int PageSize { get; set; }s
    }
    public class ListMarginWallet2 : BizResponseClass
    {
        public List<MarginWalletMasterRes2> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class MarginWalletByUserIdRes
    {
        public long ID { get; set; }
        public string WalletName { get; set; }
        public string AccWalletID { get; set; }
    }

    public class ListMarginWalletByUserIdRes : BizResponseClass
    {
        public List<MarginWalletByUserIdRes> Data { get; set; }
    }
    public class ListWalletMasterRes : BizResponseClass
    {
        public List<WalletMasterRes> Data { get; set; }
    }

    public class LeaderBoardRes
    {
        public long AutoId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public long UserId { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal ProfitPer { get; set; }
    }

    public class ListLeaderBoardRes:BizResponseClass
    {
        public List<LeaderBoardRes> Data { get; set; }
    }

    public class MarginPreConfirmationRes:BizResponseClass
    {
        public decimal LeveragePer { get; set; }
        public decimal LeverageAmount { get; set; }
        public decimal SafetyMarginAmount { get; set; }
        public short IsAutoApproveEnabled { get; set; }
        public decimal FinalCreditAmount { get; set; }
        public decimal ChargePer { get; set; }
        public decimal ChargeAmount { get; set; }
        public long ToWalletId { get; set; }
        public long LeverageId { get; set; }
    }


    public class PairLeverageDetail
    {
       public decimal Leverage { get; set; }
       public decimal LeverageCharge { get; set; }
       public decimal LastLeverageAmount { get; set; }
       public decimal CurrentBalance { get; set; }
       public DateTime? LastLeverageTime { get; set; }
        public int IsLeverageTaken { get; set; }
       //public enLeverageChargeDeductionType LeverageChargeDeductionType { get; set; }
    }
    public class PairLeverageDetailRes : BizResponseClass
    {
        public PairLeverageDetail FirstCurrency { get; set; }
        public PairLeverageDetail SecondCurrency { get; set; }
    }

    public class ArbitrageWalletMasterRes
    {
        public string UserName { get; set; }
        public string WalletName { get; set; }
        public decimal Balance { get; set; }
        public string CoinName { get; set; }
        public string AccWalletID { get; set; }
        public string PublicAddress { get; set; }
        public byte IsDefaultWallet { get; set; }
        public decimal OutBoundBalance { get; set; }
        public decimal InBoundBalance { get; set; }
        public long OrgID { get; set; }
        public long UserId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public short WalletUsageType { get; set; }
        public string WalletUsageTypeName { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public short IsLeaverageAllow { get; set; }
    }
    public class ListArbitrageWallet : BizResponseClass
    {
        public List<ArbitrageWalletMasterRes> Data { get; set; }
        //public int TotalCount { get; set; }
        //public int PageNo { get; set; }
        //public int PageSize { get; set; }s
    }
} 
