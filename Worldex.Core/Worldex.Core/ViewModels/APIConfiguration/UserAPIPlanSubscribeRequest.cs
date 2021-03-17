using Worldex.Core.ApiModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class UserAPIPlanSubscribeRequest
    {
        [Required]
        public long SubscribePlanID { get; set; }
        [Required]
        public short ChannelID { get; set; }
        public long? OldPlanID { get; set; }
    }
    public class APIPlanSubscribeProcess
    {
        public string PlanName { get; set; }
        public long PlanID { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public Decimal Price { get; set; }
        public Decimal Charge { get; set; }
        public Decimal TotalAmt { get; set; }
        public long UserID { get; set; }
        public short? RenewalStatus { get; set; }
        public DateTime? ActivationDate { get; set; }
        public string Perticuler { get; set; }
        public string DebitedAccountID { get; set; }
        public string DebitedCurrency { get; set; }
        public int PlanValidity { get; set; }
        public int PlanValidityType { get; set; }
        public long? SubscribePlanID { get; set; }
        public long RenewDays { get; set; }
        public long CustomeLimitId { get; set; }
        public short IsAutoRenew { get; set; }
        public long ServiceID { get; set; }
        public long ChannelID { get; set; }
    }

    public class APIPlanSubscribeWalletDeduct :BizResponseClass
    {
        public string AccountID { get; set; }
        public string Currency { get; set; }
    }
    public class sp_APIPlanDepositProcessResponse : BizResponseClass
    {
        public string AccountID { get; set; }
    }
}
