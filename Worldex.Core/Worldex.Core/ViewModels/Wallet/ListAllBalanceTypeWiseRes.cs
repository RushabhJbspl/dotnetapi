using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Wallet
{
    public class ListAllBalanceTypeWiseRes
    {
        public List<AllBalanceTypeWiseRes> Wallets { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }
    public class AllBalanceTypeWiseRes
    {
        public WalletResponse Wallet { get; set; }
    }
    public class WalletResponse
    {
        public string WalletName { get; set; }
        public string TypeName { get; set; }
        public string AccWalletID { get; set; }
        public string PublicAddress { get; set; }
        public byte IsDefaultWallet { get; set; }
        public Balance Balance { get; set; }
    }

    public class Balance
    {
        public decimal UnSettledBalance { get; set; }
        public decimal UnClearedBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal ShadowBalance { get; set; }
        public decimal StackingBalance { get; set; }
    }

    public class ListChargesTypeWise : BizResponseClass
    {
        public List<ChargeWalletType> Data { get; set; }
    }
    public class ChargesTypeWise
    {
        public string TrnTypeName { get; set; }
        public long TrnTypeId { get; set; }
        public string DeductWalletTypeName { get; set; }
        public decimal ChargeValue { get; set; }
        public decimal MakerCharge { get; set; }
        public decimal TakerCharge { get; set; }
    }
    public class WalletType
    {
        public long WalletTypeId { get; set; }
        public string WalletTypeName { get; set; }
        // public List<ChargesTypeWise> Data { get; set; }
    }
    public class ChargeWalletType
    {
        public string WalletTypeName { get; set; }
        public long WalletTypeId { get; set; }
        public List<ChargesTypeWise> Charges { get; set; }
    }
    public class TrnType
    {
        public List<Charge> Charges { get; set; }
    }
    public class Charge
    {
        public decimal MakerCharge { get; set; }
        public decimal TakerCharge { get; set; }
    }
    public class LeveragePairDetail
    {
        public decimal Amount { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public decimal Leverage { get; set; }
        public decimal LeverageCharge { get; set; }
        public int IsLeverageTaken { get; set; }
    }

    public class PositionValue
    {
        public decimal BidPrice { get; set; }
        public decimal Qty { get; set; }
        public decimal LandingPrice { get; set; }
        public decimal BuyBidPrice { get; set; }
        public decimal BuyQty { get; set; }
        public decimal BuyLandingPrice { get; set; }
        public decimal SellBidPrice { get; set; }
        public decimal SellQty { get; set; }
        public decimal SellLandingPrice { get; set; }
    }

    public class BalanceTotal
    {
        public decimal TotalBalance { get; set; }
    }
    public class ChargeWalletId
    {
        public long Id { get; set; }
    }

    public class ChargeCurrency
    {
        public string Name { get; set; }
    }
    public class AllBalanceResponse
    {
        public Balance Balance { get; set; }
        public string WalletType { get; set; }
        public string WalletName { get; set; }
        public byte IsDefaultWallet { get; set; }
        public decimal WithdrawalDailyLimit { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }

    public class ListAllBalanceTypeWiseResLat
    {
        public List<AllBalanceTypeWiseResLat> Wallets { get; set; }
        public decimal TotalBalance { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }
    public class AllBalanceTypeWiseResLat
    {
        public string WalletName { get; set; }
        public string TypeName { get; set; }
        public string AccWalletID { get; set; }
        public string PublicAddress { get; set; }
        public byte IsDefaultWallet { get; set; }
        public BalanceLat Balance { get; set; }
    }
    public class BalanceLat
    {
        public decimal UnSettledBalance { get; set; }
        public decimal UnClearedBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal ShadowBalance { get; set; }
        public decimal StackingBalance { get; set; }
        [NotMapped]
        public decimal BTCAvailableBalance { get; set; }
    }
}
