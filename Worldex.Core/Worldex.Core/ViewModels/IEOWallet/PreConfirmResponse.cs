using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.IEOWallet
{
    public class PreConfirmResponse : BizResponseClass
    {
        public decimal InstantDeliverdQuantity { get; set; }
        public string DeliveryCurrency { get; set; }
        public decimal MaxDeliverQuantity { get; set; }
        public decimal MinimumPurchaseAmt { get; set; }
        public decimal MaximumPurchaseAmt { get; set; }
        public string RefNo { get; set; }
    }

    public class PreConfirmResponseV2 : BizResponseClass
    {
        public decimal InstantDeliverdQuantity { get; set; }
        public string DeliveryCurrency { get; set; }
        public decimal MaxDeliverQuantity { get; set; }
        public decimal MinimumPurchaseAmt { get; set; }
        public decimal MaximumPurchaseAmt { get; set; }
        public long PaidWalletId { get; set; }
        public long DeliveredWalletId { get; set; }
        public long DeliveredCurrencyId { get; set; }
        public long PaidCurrencyId { get; set; }
        public long RoundId { get; set; }
        public decimal InstantPercentage { get; set; }
    }
}
