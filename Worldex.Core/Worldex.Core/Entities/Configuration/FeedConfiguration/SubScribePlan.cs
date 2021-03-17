using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Configuration.FeedConfiguration
{
    public class UserSubscribeAPIPlan : BizBase //UserSubscribeAPIPlan
    {
        public long APIPlanMasterID { get; set; }
        [DefaultValue(0)]
        public long CustomeLimitId { get; set; }        
        public DateTime? ExpiryDate { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal Price { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal Charge { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public Decimal TotalAmt { get; set; }
        public long UserID { get; set; }
        public DateTime? ActivationDate { get; set; }
        public short IsAutoRenew { get; set; }
        public short RenewStatus { get; set; }
        public DateTime? RenewDate { get; set; }
        public short? PaymentStatus { get; set; }
        public long? NextAutoRenewId { get; set; }
        public string Perticuler { get; set; }
        public string DebitedAccountID { get; set; }
        public string DebitedCurrency { get; set; }
        public long RenewDays { get; set; }
        public long ServiceID { get; set; }
        public long ChannelID { get; set; }
    }
}
