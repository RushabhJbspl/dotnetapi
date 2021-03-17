using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Worldex.Core.ViewModels.IEOWallet
{
    public class ListIEOWalletResponse : BizResponseClass
    {
        public List<IEOWalletResponse> Wallets { get; set; }
    }

    public class IEOWalletResponse
    {
        public Int64 Id { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyFullName { get; set; }
        public string Description { get; set; }
        public string IconPath { get; set; }
        public short Status { get; set; }
        public string IEOTokenTypeName { get; set; }
        public decimal TotalSupply { get; set; }
        public decimal AllocatedSupply { get; set; }
        public decimal MinimumPurchaseAmt { get; set; }
        public decimal MaximumPurchaseAmt { get; set; }
        public decimal CurrencyRate { get; set; }
        public string RoundID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Int16 OccurrenceLimit { get; set; }
        public decimal Bonus { get; set; }
        [NotMapped]
        public List<PurchaseWallets> PurchaseWallets { get; set; }
        [NotMapped]
        public List<Allocation> Allocation { get; set; }
    }

    public class PurchaseWallets
    {
        public Int64 PurchaseWalletID { get; set; }
        public string PurchaseWalletName { get; set; }
        public decimal PurchaseRate { get; set; }
        public Int16 CurrencyConvertType { get; set; }
        public string CurrencyConvertTypeName { get; set; }
        public decimal InstantPercentage { get; set; }
        public decimal USDRate { get; set; }
    }

    public class Allocation
    {
        public decimal AllocationValue { get; set; }
        public Int64 Duration { get; set; }
    }

    public class IEOCronMailData
    {
        public string PurchaseCurrency { get; set; }
        public string PaidCurrency { get; set; }
        public decimal DeliveredAmount { get; set; }
        public decimal MaximumDeliveredAmount { get; set; }
        public decimal DeliveredAmountTillDate { get; set; }
        public string PurchaseHistoryGUID { get; set; }
        public DateTime SubscribedDate { get; set; }
        public decimal PaidAmount { get; set; }
        public long UserID { get; set; }
    }
}
