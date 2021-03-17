using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Profile_Management
{
    public class ProfileMasterViewModel : TrackerViewModel
    {
        public string TypeId { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Price { get; set; }

        [StringLength(2000)]
        [Required]
        public string Description { get; set; }

        public int Level { get; set; }

        [StringLength(150)]
        [Required]
        public string LevelName { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal DepositFee { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Withdrawalfee { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Tradingfee { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal WithdrawalLimit { get; set; }

        public bool EnableStatus { get; set; }

        public bool ActiveStatus { get; set; }

        public long ProfileId { get; set; }
    }


    public class ProfileMasterData
    {
        public string Type { get; set; }

        public decimal ProfileFree { get; set; }

        public string Description { get; set; }

        public int KYCLevel { get; set; }

        public string LevelName { get; set; }

        public decimal DepositFee { get; set; }

        public decimal Withdrawalfee { get; set; }

        public decimal Tradingfee { get; set; }

        public bool ActiveStatus { get; set; }

        public long ProfileId { get; set; }

        public long ProfileLevel { get; set; }

        public bool IsProfileExpiry { get; set; }

        public decimal SubscriptionAmount { get; set; }

        public bool IsRecursive { get; set; }
    }

    public class UserWiseProfileData
    {
        public string Type { get; set; }

        public decimal ProfileFree { get; set; }

        public string Description { get; set; }

        public int KYCLevel { get; set; }

        public string KYCLevelName { get; set; }

        public string LevelName { get; set; }

        public decimal DepositFee { get; set; }

        public decimal Withdrawalfee { get; set; }

        public decimal Tradingfee { get; set; }

        public bool Status { get; set; }

        public long ProfileId { get; set; }

        public long ProfileLevel { get; set; }

        public bool IsProfileExpiry { get; set; }

        public decimal SubscriptionAmount { get; set; }

        public bool IsRecursive { get; set; }
    }

    public class GetProfileDataResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<UserWiseProfileData> GetUserWiseProfileData { get; set; }
    }


    public class ProfileMasterResponse : BizResponseClass
    {
        public List<ProfileMasterData> ProfileList { get; set; }
    }


    public class SocialProfileModel
    {
        public long ProfileId { get; set; }
        public string ProfileType { get; set; }
        public decimal Price { get; set; }
        public bool Subscribe { get; set; }
        public string Profile_Visiblity { get; set; }
        public string Can_Have_Followers { get; set; }
        public string Can_Follow_Leaders { get; set; }
        public string Can_Copy_Trade { get; set; }
        public string Can_Mirror_Trade { get; set; }
        public string Minimum_Trade_Volume { get; set; }
    }

    public class SocialProfileResponse : BizResponseClass
    {
        public List<SocialProfileModel> SocialProfileList { get; set; }
    }

    ///Added by pankaj kathiriya
    ///
    public class ProfileConfigurationAddViewModel : TrackerViewModel

    {
        [Required(ErrorMessage = "1,Please enter a TypeId ,9068")]
        public long TypeId { get; set; }
        public decimal ProfileFree { get; set; }
        [Required(ErrorMessage = "1,Please enter the description ,9069")]
        public string Description { get; set; }
        public int KYCLevel { get; set; }
        [Required(ErrorMessage = "1,Please enter the KYCLevelName ,9070")]
        public string LevelName { get; set; }
        public decimal DepositFee { get; set; }
        public decimal Withdrawalfee { get; set; }
        public decimal Tradingfee { get; set; }
        public long Profilelevel { get; set; }
        public bool IsProfileExpiry { get; set; }
        public List<TransactionLimitViewModel> TransactionLimit { get; set; }
        public List<TransactionLimitViewModel> WithdrawalLimit { get; set; }
        public List<TransactionLimitViewModel> TradeLimit { get; set; }
        public List<TransactionLimitViewModel> DepositLimit { get; set; }
        public decimal SubscriptionAmount { get; set; }
        public bool IsRecursive { get; set; }
    }
    public class ProfileConfigurationResponseViewmodel : BizResponseClass
    {

    }
    public class TransactionLimitViewModel
    {
        [Range(0, Int64.MaxValue, ErrorMessage = "Currency should not contain characters and negative number.")]
        public long CurrencyId { get; set; }
        
        public string CurrancyName { get; set;}
        [Range(0, Int64.MaxValue, ErrorMessage = "Hourly number should not contain characters and negative number.")]
        public long Hourly { get; set; }
        [Range(0, Int64.MaxValue, ErrorMessage = "Daily number should not contain characters and negative number.")]
        public long Daily { get; set; }
        [Range(0, Int64.MaxValue, ErrorMessage = "Weekly number should not contain characters and negative number.")]
        public long Weekly { get; set; }
        [Range(0, Int64.MaxValue, ErrorMessage = "Monthly number should not contain characters and negative number.")]
        public long Monthly { get; set; }
        [Range(0, Int64.MaxValue, ErrorMessage = "Quaterly number should not contain characters and negative number.")]
        public long Qauterly { get; set; }
        [Range(0, Int64.MaxValue, ErrorMessage = "Yearly number should not contain characters and negative number.")]
        public long Yearly { get; set; }
    }


    public class ProfileConfigurationAddReqViewModel

    {
        public int UserId { get; set; }
        public long TypeId { get; set; }
        public decimal ProfileFree { get; set; }

        public string Description { get; set; }
        public int KYCLevel { get; set; }

        [StringLength(150, ErrorMessage = "1,Level name allow string.,9078")]
        public string LevelName { get; set; }
        public decimal DepositFee { get; set; }
        public decimal Withdrawalfee { get; set; }
        public decimal Tradingfee { get; set; }
        public long Profilelevel { get; set; }
        public bool IsProfileExpiry { get; set; }
        public string TransactionLimit { get; set; }
        public string WithdrawalLimit { get; set; }
        public string TradeLimit { get; set; }
        public string DepositLimit { get; set; }

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Subscription amount should be grater than 0.0 ,9077"), Column(TypeName = "decimal(28, 18)")]
        public decimal SubscriptionAmount { get; set; }
        public bool IsRecursive { get; set; }
    }

    public class ProfileConfigurationUpdateViewModel : TrackerViewModel

    {
        [Required(ErrorMessage = "1,Please enter the profile configuration id ,9068")]
        public long Id { get; set; }
        [Required(ErrorMessage = "1,Please enter a TypeId ,9068")]
        public long TypeId { get; set; }
        public decimal ProfileFree { get; set; }
        [Required(ErrorMessage = "1,Please enter the description ,9069")]
        public string Description { get; set; }
        public int KYCLevel { get; set; }
        [Required(ErrorMessage = "1,Please enter the KYCLevelName ,9070")]
        public string LevelName { get; set; }
        public decimal DepositFee { get; set; }
        public decimal Withdrawalfee { get; set; }
        public decimal Tradingfee { get; set; }
        public long Profilelevel { get; set; }
        public bool IsProfileExpiry { get; set; }
        public List<TransactionLimitViewModel> TransactionLimit { get; set; }
        public List<TransactionLimitViewModel> WithdrawalLimit { get; set; }
        public List<TransactionLimitViewModel> TradeLimit { get; set; }
        public List<TransactionLimitViewModel> DepositLimit { get; set; }
        public decimal SubscriptionAmount { get; set; }
        public bool IsRecursive { get; set; }
        
    }

    public class ProfileConfigurationDeleteViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please enter the profile configuration id ,9068")]
        public long Id { get; set; }
    }
    public class ProfileConfigurationDeleteReqViewModel : TrackerViewModel
    {
        public long Id { get; set; }
        public int UserId { get; set; }
    }

    public class ProfileConfigurationUpdateReqViewModel

    {
        [Required(ErrorMessage = "1,Please enter the profile configuration id ,9068")]
        public long Id { get; set; }
        [Required(ErrorMessage = "1,Please enter a TypeId ,9068")]
        public long TypeId { get; set; }
        public decimal ProfileFree { get; set; }
        [Required(ErrorMessage = "1,Please enter the description ,9069")]
        public string Description { get; set; }
        public int KYCLevel { get; set; }
        [Required(ErrorMessage = "1,Please enter the KYCLevelName ,9070")]
        public string LevelName { get; set; }
        public decimal DepositFee { get; set; }
        public decimal Withdrawalfee { get; set; }
        public decimal Tradingfee { get; set; }
        public long Profilelevel { get; set; }
        public bool IsProfileExpiry { get; set; }
        public string TransactionLimit { get; set; }
        public string WithdrawalLimit { get; set; }
        public string TradeLimit { get; set; }
        public string DepositLimit { get; set; }
        public decimal SubscriptionAmount { get; set; }
        public bool IsRecursive { get; set; }
        public int UserId { get; set; }
        public short Status { get; set; }
    }

    public class GetProfileConfigurationViewModel
    {
        public long Id { get; set; }
        public string TypeName { get; set; }
        public long Typeid { get; set; }


        public int KYCLevel { get; set; }
        public string LevelName { get; set; }
        public string KYCName { get; set; }

        public DateTime CreatedDate { get; set; }
        public decimal SubscriptionAmount { get; set; }
        public string Description { get; set; }
        public decimal DepositFee { get; set; }
        public decimal Withdrawalfee { get; set; }
        public bool IsRecursive { get; set; }
        public bool IsProfileExpiry { get; set; }
        public decimal ProfileFree { get; set; }
        public long Profilelevel { get; set; }
        public decimal Tradingfee { get; set; }
        public string ProfileName { get; set; }
        public string TradeLimit { get; set; }
        public string DepositLimit { get; set; }
        public string TransactionLimit { get; set; }
        public string WithdrawalLimit { get; set; }
    }

    public class GetProfileConfigurationByIdViewModel
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public long Typeid { get; set; }

        public decimal SubscriptionAmount { get; set; }
        public string Description { get; set; }
        public int KYCLevel { get; set; }
        public string LevelName { get; set; }
       // public string KYCName { get; set; }
        public decimal DepositFee { get; set; }
        public decimal Withdrawalfee { get; set; }
        public decimal Tradingfee { get; set; }
        public string WithdrawalLimit { get; set; }

        public bool IsRecursive { get; set; }
        public string DepositLimit { get; set; }
        public bool IsProfileExpiry { get; set; }
        public decimal ProfileFree { get; set; }
        public long Profilelevel { get; set; }


        public string TradeLimit { get; set; }


        public string TransactionLimit { get; set; }

    }

    public class GetProfileConfigurationbyidWithJsonViewModel
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public long Typeid { get; set; }
        public decimal SubscriptionAmount { get; set; }
        public string Description { get; set; }
        public int KYCLevel { get; set; }
        public string LevelName { get; set; }
        public decimal DepositFee { get; set; }
        public decimal Withdrawalfee { get; set; }
        public decimal Tradingfee { get; set; }
        public bool IsRecursive { get; set; }
        public bool IsProfileExpiry { get; set; }
        public decimal ProfileFree { get; set; }
        public long Profilelevel { get; set; }
        public List<TransactionLimitViewModel> TransactionLimit { get; set; }
        public List<TransactionLimitViewModel> WithdrawalLimit { get; set; }
        public List<TransactionLimitViewModel> TradeLimit { get; set; }
        public List<TransactionLimitViewModel> DepositLimit { get; set; }
    }


    public class GetProfileConfigurationWithJsonViewModel
    {
        public long Id { get; set; }
        public string TypeName { get; set; }
        public long Typeid { get; set; }


        public int KYCLevel { get; set; }
        public string LevelName { get; set; }
        public string KYCName { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal SubscriptionAmount { get; set; }
        public string Description { get; set; }
        public decimal DepositFee { get; set; }
        public decimal Withdrawalfee { get; set; }
        public bool IsRecursive { get; set; }
        public bool IsProfileExpiry { get; set; }
        public decimal ProfileFree { get; set; }
        public long Profilelevel { get; set; }
        public decimal Tradingfee { get; set; }
        public string ProfileName { get; set; }
        public List<TransactionLimitViewModel> TransactionLimit { get; set; }
        public List<TransactionLimitViewModel> WithdrawalLimit { get; set; }
        public List<TransactionLimitViewModel> TradeLimit { get; set; }
        public List<TransactionLimitViewModel> DepositLimit { get; set; }
    }

    public class ProfileConfigurationGetResponseViewmodel : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<GetProfileConfigurationWithJsonViewModel> getProfileConfiguration { get; set; }
    }
    public class ProfileConfigurationGetResponseByIdViewmodel : BizResponseClass
    {
        
        public List<GetProfileConfigurationbyidWithJsonViewModel> getProfileConfiguration { get; set; }
    }



    public class ProfilelevelCountViewmodel
    {
        public long ProfileId { get; set; }

        public string ProfileName { get; set; }

        public int LevelCount { get; set; }
    }
    public class ProfilelevelCountresponseViewmodel : BizResponseClass
    {
        public List<ProfilelevelCountViewmodel> ProfilelevelCount { get; set; }
    }

    public class ProfilewiseuserlistViewmodel
    {
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }
    public class ProfilewiseuserlistResponseVoewmodel : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<ProfilewiseuserlistViewmodel> profilewiseuserlist { get; set; }
    }
    public class GetProfilelevelmaster
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ProfileName { get; set; }
    }
    public class GetProfilelevelmasterResponse : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<GetProfilelevelmaster> GetProfilelevelmasters { get; set; }
    }
    public class GetProfilelevelDropDownListResponse : BizResponseClass
    {

        public List<GetProfilelevelmaster> GetProfilelevelmasters { get; set; }
    }
}
