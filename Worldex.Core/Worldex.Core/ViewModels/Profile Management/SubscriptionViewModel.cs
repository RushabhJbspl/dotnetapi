using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Profile_Management
{
    public class SubscriptionViewModel : TrackerViewModel
    {
        public SubscriptionViewModel()
        {
            AccessibleFeatures = new SubscriptionAccessibleFeatures();
        }
        public int UserId { get; set; }

        public long ProfileId { get; set; }

        public SubscriptionAccessibleFeatures AccessibleFeatures { get; set; }
    }

    public class SubscriptionAccessibleFeatures
    {
        public bool IsIPWhitelisting { get; set; }
        public bool IsDevicewhitelisting { get; set; }
        public bool IsMarginTrading { get; set; }
        public bool IsSocialTrading { get; set; }
        public bool IsAddCoin { get; set; }
        public bool IsWhitelistwithdrawaladdress { get; set; }
        public bool IsStackingBalance { get; set; }
        public bool IsLedger { get; set; }
        public bool IsPaymentFail { get; set; }
        public bool IsPaymentReceived { get; set; }
        public bool IsRefund { get; set; }
        public bool IsUpcomingpaymentnotification { get; set; }
        public bool IsProfileActivation { get; set; }
        public bool IsProfileupgradedowngrade { get; set; }
        public int IsMonthlyrecursion { get; set; } // 1-Monthly
        public int IsYearlyrecursion { get; set; } //  2-Yearly
        public int Isbeforerecursion { get; set; } // 1-Before
        public int IsAfterrecursion { get; set; } // 2-After
    }

    public class MultipalSubscriptionReqViewModel : TrackerViewModel
    {
        [Required]
        public int ProfileId { get; set; }
    }

    public class SubscriptionResponse : BizResponseClass
    {
    }

    public class SubscriptionProfile 
    {
        public long ProfileId { get; set; }
    }

    public class SubscriptionProfileType
    {
        public long ProfileId { get; set; }
        public string profiletype { get; set; }
    }

    public class GetActiveSubscriptionData
    {
        public string ProfileName { get; set; }
        public long Profilelevel { get; set; }
        public long ProfileId { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public AccessibleFeaturesViewModel AccessibleFeatures { get; set; }
    }

    public class GetActiveSubResponse : BizResponseClass
    {
        public GetActiveSubscriptionData ActiveSubscriptionData { get; set; }
    }


    public class AccessibleFeaturesViewModel
    {
        public bool IsIPWhitelisting { get; set; }
        public bool IsDevicewhitelisting { get; set; }
        public bool IsMarginTrading { get; set; }
        public bool IsSocialTrading { get; set; }
        public bool IsAddCoin { get; set; }
        public bool IsWhitelistwithdrawaladdress { get; set; }
        public bool IsStackingBalance { get; set; }
        public bool IsLedger { get; set; }
        public bool IsPaymentFail { get; set; }
        public bool IsPaymentReceived { get; set; }
        public bool IsRefund { get; set; }
        public bool IsUpcomingpaymentnotification { get; set; }
        public bool IsProfileActivation { get; set; }
        public bool IsProfileupgradedowngrade { get; set; }
        public int IsMonthlyrecursion { get; set; }
        public int IsYearlyrecursion { get; set; }
        public int Isbeforerecursion { get; set; }
        public int IsAfterrecursion { get; set; }
    }

    public class ProfileHistoryData
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public long UserId { get; set; }

        public string Request { get; set; }

        public string Response { get; set; }
    }

    public class UserProfileHistoryData
    {
        public long ProfileId { get; set; }

        public long Profilelevel { get; set; }

        public string ProfileName { get; set; }

        public DateTime CreatedDate { get; set; }

    }

    public class UserProfileHistoryDataResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<UserProfileHistoryData> UserProfileHistory { get; set; }
    }




}
