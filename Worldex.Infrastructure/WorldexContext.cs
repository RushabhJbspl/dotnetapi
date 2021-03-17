using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using Worldex.Core.Entities;
using Worldex.Core.Services;
using Microsoft.AspNetCore.Identity;
using Worldex.Core.Entities.User;
using Worldex.Core.Entities.Culture;
using Worldex.Core.Entities.Resource;
using Worldex.Core.Entities.Modes;
using Worldex.Core.Entities.Log;
using Worldex.Core.ApiModels;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Wallet;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.UserChangeLog;
using Microsoft.EntityFrameworkCore.Metadata;
using Worldex.Core.ViewModels;
using Worldex.Core.Entities.Profile_Management;
using Worldex.Core.Entities.Complaint;
using Worldex.Core.Entities.KYC;
using Worldex.Core.ViewModels.Complaint;
using Worldex.Core.ViewModels.Configuration;
using Worldex.Core.Entities.NewWallet;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using Worldex.Infrastructure.Services;
using Worldex.Core.Interfaces;
using Worldex.Core.Helpers;
using Worldex.Core.ViewModels.BackOfficeReports;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletOperations;
using Worldex.Core.Entities.SocialProfile;
using Worldex.Core.Entities.Organization;
using Worldex.Core.Entities.EmailMaster;
using Worldex.Core.Entities.PhoneMaster;
using Worldex.Core.ViewModels.Transaction.BackOfficeCount;
using Worldex.Core.ViewModels.MobileMaster;
using Worldex.Core.ViewModels.EmailMaster;
using Worldex.Core.Entities.KYCConfiguration;
using Worldex.Core.ViewModels.Profile_Management;
using Worldex.Core.ViewModels.KYCConfiguration;
using Worldex.Core.ViewModels.BackOfficeComplain;
using Worldex.Core.ViewModels.KYC;
using Worldex.Core.ViewModels.Organization;
using Worldex.Core.Entities.Backoffice;
using Worldex.Core.ViewModels.BackOffice;
using Worldex.Core.ViewModels.BackOffice.ComplaintSALConfiguration;
using Worldex.Core.Entities.Backoffice.PasswordPolicy;
using Worldex.Core.ViewModels.BackOffice.PasswordPolicy;
using Worldex.Core.Entities.Charges;
using Worldex.Core.Entities.SignalR;
using Worldex.Core.Entities.Backoffice.RoleManagement;
using Worldex.Core.Entities.UserRoleManagement;
using Worldex.Core.Entities.Affiliate;
using Worldex.Core.ViewModels.Configuration.FeedConfiguration;
using Worldex.Core.Entities.Referral;
using Worldex.Core.ViewModels.Referral;
using Worldex.Core.ViewModels.AccountViewModels.Affiliate;
using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.ViewModels.APIConfiguration;
using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.ViewModels.RoleConfig;
using Worldex.Core.Entities.GroupModuleManagement;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.ViewModels.Transaction.Arbitrage;
using Worldex.Core.ViewModels.CCXT;
using Worldex.Core.Entities.IEO;
using Worldex.Core.ViewModels.IEOWallet;
using Worldex.Core.Entities.FiatBankIntegration;
using Worldex.Core.ViewModels.FiatBankIntegration;
using Worldex.Core.Entities.FiatBankIntegration;
using Worldex.Core.ViewModels.Transaction.MarketMaker;
using Worldex.Core.ViewModels.AccountViewModels.Login;

namespace Worldex.Infrastructure
{
    public partial class WorldexContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        private readonly UserResolveService _userService;
        //  ICommonRepository<AuditActivityLog> _repository;

        public DbSet<BuySellTopUpRequest> BuySellTopUpRequest { get; set; }
        public DbSet<SellTopUpRequest> SellTopUpRequest { get; set; }
        public DbSet<FiatCoinConfiguration> FiatCoinConfiguration { get; set; }

        //IEO Data tables
        public DbSet<IEOSlabMaster> IEOSlabMaster { get; set; }
        public DbSet<IEOCronMaster> IEOCronMaster { get; set; }
        public DbSet<IEOPurchaseHistory> IEOPurchaseHistory { get; set; }
        public DbSet<IEORoundMaster> IEORoundMaster { get; set; }
        public DbSet<IEOCurrencyPairMapping> IEOCurrencyPairMapping { get; set; }
        public DbSet<IEOCurrencyMaster> IEOCurrencyMaster { get; set; }
        public DbSet<IEOBannerMaster> IEOBannerMaster { get; set; }
        public DbSet<IEOWalletAdminDeposit> IEOWalletAdminDeposit { get; set; }

        //Role Management 
        public virtual DbSet<ModuleMaster> ModuleMaster { get; set; }
        public virtual DbSet<SubModuleMaster> SubModuleMaster { get; set; }
        public virtual DbSet<FieldMaster> FieldMaster { get; set; }
        public virtual DbSet<ToolMaster> ToolMaster { get; set; }
        public virtual DbSet<UserAccessRights> UserAccessRights { get; set; }
        public virtual DbSet<UserAssignModule> UserAssignModule { get; set; }
        public virtual DbSet<UserAssignSubModule> UserAssignSubModule { get; set; }
        public virtual DbSet<UserAssignFieldRights> UserAssignFieldRights { get; set; }
        public virtual DbSet<UserAssignToolRights> UserAssignToolRights { get; set; }
        public DbSet<ModuleTypeMaster> ModuleTypeMaster { get; set; }
        public DbSet<ModuleCRUDOptMaster> ModuleCRUDOptMaster { get; set; }
        public DbSet<ModuleUtilityMaster> ModuleUtilityMaster { get; set; }
        public DbSet<ModuleDomainMaster> ModuleDomainMaster { get; set; }
        public DbSet<ModuleVisibilityMaster> ModuleVisibilityMaster { get; set; }
        public DbSet<ModuleFieldRequirerMaster> ModuleFieldRequirerMaster { get; set; }
        public DbSet<ModuleAccessRightsMaster> ModuleAccessRightsMaster { get; set; }
        public DbQuery<ModuleGroupAccessQryRes> ModuleGroupAccessQry { get; set; }
        public DbSet<ArbitrageWalletTransfer> ArbitrageWalletTransfer { get; set; }
        //FieldInfo
        public DbQuery<GetUserData> GetUserData { get; set; }
        // Account Management
        public string CurrentUserId { get; internal set; }
        public virtual DbSet<ApplicationUser> Users { get; set; }
        public virtual DbSet<ApplicationUserPhotos> ApplicationUserPhotos { get; set; }
        public virtual DbSet<ApplicationRole> Roles { get; set; }
        public virtual DbSet<Cultures> Cultures { get; set; }
        public virtual DbSet<Resources> Resources { get; set; }
        public virtual DbSet<Mode> Mode { get; set; }
        public virtual DbSet<LoginLog> LoginLog { get; set; }
        public virtual DbSet<FiatTradeConfigurationMaster> FiatTradeConfigurationMaster { get; set; }
        public virtual DbSet<FiatCurrencyMaster> FiatCurrencyMaster { get; set; }

        //Rushabh -11-02-2019
        //public virtual DbSet<RoleMaster> RoleMaster { get; set; }
        public virtual DbSet<RoleHistory> RoleHistory { get; set; }
        public virtual DbSet<PermissionGroupMaster> PermissionGroupMaster { get; set; }
        public virtual DbSet<ApplicationGroupRoles> GroupRoleMapping { get; set; }

        public virtual DbSet<RegisterType> RegisterType { get; set; }
        public virtual DbSet<TempUserRegister> TempUserRegister { get; set; }
        public virtual DbSet<TempOtpMaster> TempOtpMaster { get; set; }
        public virtual DbSet<OtpMaster> OtpMaster { get; set; }
        public DbSet<IpHistory> IpHistory { get; set; } //ntrivedi 02112018 for migration
        public DbSet<LoginHistory> LoginHistory { get; set; } //ntrivedi 02112018 for migration
        // add by nirav savariya for create password for login with mobile and email
        public virtual DbSet<CustomPassword> CustomPassword { get; set; }
        public virtual DbSet<DeviceMaster> DeviceMaster { get; set; }
        public virtual DbSet<UserLogChange> UserLogChange { get; set; }
        public DbSet<ToDoItem> ToDoItems { get; set; }
        public DbSet<MessagingQueue> MessagingQueue { get; set; }
        public DbSet<EmailQueue> EmailQueue { get; set; }
        public DbSet<NotificationQueue> NotificationQueue { get; set; }
        public DbSet<CommAPIServiceMaster> CommAPIServiceMaster { get; set; }
        public DbSet<UserProfitStatistics> UserProfitStatistics { get; set; }
        public DbSet<ArbitrageUserProfitStatistics> ArbitrageUserProfitStatistics { get; set; }

        public DbSet<BackgroundCallMaster> BackgroundCallMaster { get; set; }

        public DbSet<CommServiceMaster> CommServiceMaster { get; set; }
        public DbSet<CommServiceproviderMaster> CommServiceproviderMaster { get; set; }
        public DbSet<CommServiceTypeMaster> CommServiceTypeMaster { get; set; }
        public DbSet<RequestFormatMaster> RequestFormatMaster { get; set; }
        public DbSet<ServiceTypeMaster> ServiceTypeMaster { get; set; }
        public DbSet<TemplateMaster> TemplateMaster { get; set; }
        public DbSet<TemplateCategoryMaster> TemplateCategoryMaster { get; set; }
        public DbSet<UserKeyMaster> UserKeyMaster { get; set; }
        public DbSet<ProfileMaster> ProfileMaster { get; set; } // nirav savariya 11-03-2018
        public DbSet<TradingChartData> TradingChartData { get; set; }
        public DbSet<ProfileLevelMaster> ProfileLevelMaster { get; set; } // nirav savariya 1-16-2019
        public DbQuery<UserWiseProfileData> GetSubscriptionProfileData { get; set; } // added by nirav savariya 1-18-2019
        public DbSet<PersonalVerification> PersonalVerification { get; set; }
        public DbSet<KYCLevelMaster> kYCLevelMaster { get; set; }
        public DbSet<SubscriptionMaster> SubscriptionMaster { get; set; }
        public DbSet<IPAddressMaster> IPAddressMaster { get; set; } // ntrivedi 21-12-2018 for bhai 
        public DbSet<AllowTrnTypeRoleWise> AllowTrnTypeRoleWise { get; set; }
        public DbSet<LeverageMaster> LeverageMaster { get; set; }
        public DbSet<StopLossMaster> StopLossMaster { get; set; }
        public DbSet<CoinListRequest> CoinListRequest { get; set; } //komal 4-1-2018
        public DbSet<ColdWalletMaster> ColdWalletMaster { get; set; }
        public DbSet<SiteTokenRateType> SiteTokenRateType { get; set; } //komal 07-02-2019
        public DbSet<SiteTokenMaster> SiteTokenMaster { get; set; }//komal 07-02-2019
        public DbSet<SiteTokenConversion> SiteTokenConversion { get; set; }
        public DbSet<TradingChargeTypeMaster> TradingChargeTypeMaster { get; set; }
        public DbSet<ChargeFreeMarketCurrencyMaster> ChargeFreeMarketCurrencyMaster { get; set; }
        public DbSet<LanguagePreferenceMaster> LanguagePreferenceMaster { get; set; }
        //Rita 16-4-19
        public DbSet<SiteTokenMasterMargin> SiteTokenMasterMargin { get; set; }
        public DbSet<SiteTokenConversionMargin> SiteTokenConversionMargin { get; set; }
        public DbSet<ProviderWalletTypeMapping> ProviderWalletTypeMapping { get; set; }

        ///Complaint
        public DbSet<Typemaster> Typemaster { get; set; }
        public DbSet<Complainmaster> Complainmaster { get; set; }
        public DbSet<CompainTrail> CompainTrail { get; set; }
        public DbSet<ComplainStatusTypeMaster> ComplainStatusTypeMaster { get; set; }
        public DbSet<SignUpLog> SignUpLog { get; set; } // nirav savariya for signup log report on 12-05-2018
        public DbSet<ActivityLog> ActivityLog { get; set; } // nirav savariya for activity log report on 12-05-2018
        //New Wallets Entity
        //public DbSet<Wallet_TypeMaster> Wallet_TypeMaster { get; set; }
        public DbSet<ErrorInfo> ErrorInfo { get; set; }
        public DbSet<BlockWalletTrnTypeMaster> BlockWalletTrnTypeMaster { get; set; }
        public DbSet<ChargePolicy> ChargePolicy { get; set; }
        public DbSet<CommissionPolicy> CommissionPolicy { get; set; }
        public DbSet<CurrencyTypeMaster> CurrencyTypeMaster { get; set; }
        public DbSet<OrganizationMaster> OrganizationMaster { get; set; }
        public DbSet<OrganizationUserMaster> OrganizationUserMaster { get; set; }
        public DbSet<UserMaster> UserMaster { get; set; }
        public DbSet<UserRoleMaster> UserRoleMaster { get; set; }
        public DbSet<UserTypeMaster> UserTypeMaster { get; set; }
        public DbSet<UserWalletBlockTrnTypeMaster> UserWalletBlockTrnTypeMaster { get; set; }
        public DbSet<MarginUserWalletBlockTrnTypeMaster> MarginUserWalletBlockTrnTypeMaster { get; set; }

        public DbSet<DepositionRecon> DepositionRecon { get; set; } //Rushabh 06-03-2019 For Deposition Recon Process
        public DbSet<ReferralServiceDetail> ReferralServiceDetail { get; set; }
        public DbSet<ReferralSchemeTypeMapping> ReferralSchemeTypeMapping { get; set; }
        public DbSet<ReferralUserLevelMapping> ReferralUserLevelMapping { get; set; }

        // public DbSet<UserWalletMaster> UserWalletMaster { get; set; }
        public DbSet<WalletAuthorizeUserMaster> WalletAuthorizeUserMaster { get; set; }
        public DbSet<WTrnTypeMaster> WTrnTypeMaster { get; set; }
        public DbSet<ActivityTypeMaster> ActivityTypeMaster { get; set; }
        public DbSet<AllowedChannels> AllowedChannels { get; set; }
        public DbSet<AuditActivityLog> AuditActivityLog { get; set; }
        public DbSet<AutorizedApps> AutorizedApps { get; set; }
        public DbSet<ChargeTypeMaster> ChargeTypeMaster { get; set; }
        public DbSet<WalletUsageType> WalletUsageType { get; set; }
        public DbSet<CommissionTypeMaster> CommissionTypeMaster { get; set; }
        public DbSet<ServiceProvider> ServiceProvider { get; set; }
        public DbSet<TransactionBlockedChannel> TransactionBlockedChannel { get; set; }
        public DbSet<MarginTransactionBlockedChannel> MarginTransactionBlockedChannel { get; set; }

        public DbSet<TransactionPolicy> TransactionPolicy { get; set; }
        public DbSet<TransactionPolicyAllowedRole> TransactionPolicyAllowedRole { get; set; }
        public DbSet<UserActivityLog> UserActivityLog { get; set; }
        public DbSet<WalletPolicyAllowedDay> WalletPolicyAllowedDay { get; set; }
        public DbSet<WalletUsagePolicy> WalletUsagePolicy { get; set; }
        public DbSet<AddRemoveUserWalletRequest> AddRemoveUserWalletRequest { get; set; }
        public DbQuery<WalletMasterResponsev2> WalletMasterResponsev2 { get; set; }
        public DbSet<TransactionStatusCheckRequest> TransactionStatusCheckRequest { get; set; } // khushali 24-01-2019 for LP status check  request response

        // wallet tables
        public DbSet<WalletTypeMaster> WalletTypeMasters { get; set; }
        public DbSet<WalletMaster> WalletMasters { get; set; }
        public DbSet<WalletOrder> WalletOrders { get; set; }
        public DbSet<TransactionAccount> TransactionAccounts { get; set; }
        public DbSet<WalletLedger> WalletLedgers { get; set; }
        public DbSet<DepositHistory> DepositHistory { get; set; }
        public DbSet<TradeBitGoDelayAddresses> TradeBitGoDelayAddressess { get; set; }
        public DbSet<AddressMaster> AddressMasters { get; set; }
        public DbSet<WalletAllowTrn> WalletAllowTrns { get; set; }
        public DbSet<WalletTransactionOrder> WalletTransactionOrders { get; set; }
        public DbSet<WalletTransactionQueue> WalletTransactionQueues { get; set; }
        public DbSet<TrnAcBatch> TrnAcBatch { get; set; }
        public DbSet<WalletLimitConfiguration> WalletLimitConfiguration { get; set; }
        public DbSet<DepositCounterLog> DepositCounterLog { get; set; }
        public DbSet<DepositCounterMaster> DepositCounterMaster { get; set; }
        public DbSet<NEODepositCounter> NEODepositCounter { get; set; }

        public DbSet<TradeDepositCompletedTrn> TradeDepositCompletedTrn { get; set; }
        public DbSet<TrnTypeMaster> TrnTypeMaster { get; set; }
        public DbSet<StatasticsDetail> StatasticsDetail { get; set; }
        public DbSet<Statastics> Statastics { get; set; }
        public DbSet<StakingPolicyDetail> StakingPolicyDetail { get; set; }
        public DbSet<StakingPolicyMaster> StakingPolicyMaster { get; set; }
        public DbSet<TokenStakingHistory> TokenStakingHistory { get; set; }
        public DbSet<TokenUnStakingHistory> TokenUnStakingHistory { get; set; } //ntrivedi 29-12-2018
        public DbSet<WalletTrnLimitConfiguration> WalletTrnLimitConfiguration { get; set; }
        public DbSet<DepositionInterval> DepositionInterval { get; set; }

        public DbSet<LPCharge> LPCharge { get; set; }
        public DbSet<LPWalletMismatch> LPWalletMismatch { get; set; }
        public DbSet<LPWalletMaster> LPWalletMaster { get; set; }
        public DbSet<LPTransactionAccount> LPTransactionAccount { get; set; }
        public DbSet<LPWalletLedger> LPWalletLedger { get; set; }
        public DbSet<UserBankRequest> UserBankRequest { get; set; }
        public DbSet<UserBankMaster> UserBankMaster { get; set; }
        // Rushabh 15-01-2019 New Entity Added For 
        public DbSet<CurrencyRateMaster> CurrencyRateMaster { get; set; }
        public DbSet<BalanceStatistics> BalanceStatistics { get; set; }
        public DbSet<BlockUnblockUserAddress> BlockUnblockUserAddress { get; set; }
        public DbSet<TokenSupplyHistory> TokenSupplyHistory { get; set; }
        public DbSet<TransferFeeHistory> TransferFeeHistory { get; set; }
        //Charges Entity
        public DbSet<TrnChargeLog> TrnChargeLog { get; set; }
        public DbSet<ChargeConfigurationDetail> ChargeConfigurationDetail { get; set; }
        public DbSet<ChargeConfigurationMaster> ChargeConfigurationMaster { get; set; }
        public DbSet<SpecialChargeConfiguration> SpecialChargeConfiguration { get; set; }
        //vsolnki 24-10-2018
        public DbSet<UserStacking> UserStacking { get; set; }
        public DbSet<MemberShadowBalance> MemberShadowBalance { get; set; }
        public DbSet<MemberShadowLimit> MemberShadowLimit { get; set; }
        public DbSet<StckingScheme> StckingScheme { get; set; }
        public DbSet<MarketCapCounterMaster> MarketCapCounterMaster { get; set; }
        public DbSet<CurrencyRateDetail> CurrencyRateDetail { get; set; }

        public DbSet<BizUserTypeMapping> BizUserTypeMapping { get; set; }
        public DbSet<UserPreferencesMaster> UserPreferencesMaster { get; set; }
        public DbSet<BeneficiaryMaster> BeneficiaryMaster { get; set; }
        //public DbSet<WalletLimitConfigurationMaster> WalletLimitConfigurationMaster { get; set; }
        public DbSet<WithdrawHistory> WithdrawHistory { get; set; }

        public DbSet<ConvertFundHistory> ConvertFundHistory { get; set; }
        public DbSet<MarginLoanRequest> MarginLoanRequest { get; set; }//ntrivedi 07-02-2019 ntrivedi table name changes 03-04-2019
        public DbSet<LoanChargeDetail> LoanChargeDetail { get; set; }//ntrivedi 04-04-2019 ntrivedi table name changes 03-04-2019
        public DbSet<MarginChargeOrder> MarginChargeOrder { get; set; }//ntrivedi 08-04-2019 ntrivedi table name changes 03-04-2019
        public DbSet<UserLoanMaster> UserLoanMaster { get; set; }//ntrivedi 11-04-2019
        public DbSet<SubscribeNewsLetter> SubscribeNewsLetter { get; set; }


        public DbSet<MarginDepositCounterLog> MarginDepositCounterLog { get; set; }
        public DbSet<MarginDepositCounterMaster> MarginDepositCounterMaster { get; set; }
        public DbSet<MarginDepositHistory> MarginDepositHistory { get; set; }
        public DbSet<MarginAddressMaster> MarginAddressMaster { get; set; }

        //Feed Configuration
        public DbSet<SocketMethods> SocketMethods { get; set; }
        public DbSet<SocketFeedLimits> SocketFeedLimits { get; set; }
        public DbSet<SocketFeedConfiguration> SocketFeedConfiguration { get; set; }
        public DbSet<FeedLimitCounts> FeedLimitCounts { get; set; }
        public DbSet<APIPlanMaster> APIPlanMaster { get; set; }
        public DbSet<APIPlanDetail> APIPlanDetail { get; set; }
        public DbSet<RestMethods> RestMethods { get; set; }
        public DbSet<APIPlanMethodConfiguration> APIPlanMethodConfiguration { get; set; }
        public DbSet<UserAPIKeyDetails> UserAPIKeyDetails { get; set; }
        public DbSet<WhiteListIPEndPoint> WhiteListIPEndPoint { get; set; }
        public DbSet<UserSubscribeAPIPlan> UserSubscribeAPIPlan { get; set; }
        public DbSet<PublicAPIKeyPolicy> PublicAPIKeyPolicy { get; set; }
        public DbSet<APIKeyWhitelistIPConfig> APIKeyWhitelistIPConfig { get; set; }
        public DbSet<APIPlanConfigurationHistory> APIPlanConfigurationHistory { get; set; }
        public DbSet<UserAPILimitCount> UserAPILimitCount { get; set; }
        public DbSet<APIReqResStatistics> APIReqResStatistics { get; set; }
        public DbSet<PublicAPIReqResLog> PublicAPIReqResLog { get; set; }
        public DbSet<APIPlanMethodConfigurationHistory> APIPlanMethodConfigurationHistory { get; set; }
        public DbSet<APIMethods> APIMethods { get; set; }
        public DbSet<APIMethodConfiguration> APIMethodConfiguration { get; set; }

        //========Transaction Tables
        public DbSet<TradeTransactionQueue> TradeTransactionQueue { get; set; }
        public DbSet<TradeTransactionQueueMargin> TradeTransactionQueueMargin { get; set; }
        public DbSet<TradeTransactionQueueArbitrage> TradeTransactionQueueArbitrage { get; set; }//Rita 31-05-19 for separate Arbitrage trading
        public DbSet<TradePairMaster> TradePairMaster { get; set; }
        public DbSet<TradePairMasterMargin> TradePairMasterMargin { get; set; }//Rita 15-2-19 for separate Margin trading
        public DbSet<TradePairMasterArbitrage> TradePairMasterArbitrage { get; set; }//Rita 31-05-19 for separate Arbitrage trading
        public DbSet<TradePairDetail> TradePairDetail { get; set; }
        public DbSet<TradePairDetailMargin> TradePairDetailMargin { get; set; }//Rita 15-2-19 for separate Margin trading
        public DbSet<TradePairDetailArbitrage> TradePairDetailArbitrage { get; set; }//Rita 31-05-19 for separate Arbitrage trading
        public DbSet<TransactionQueue> TransactionQueue { get; set; }
        public DbSet<TransactionQueueMargin> TransactionQueueMargin { get; set; }//Rita 15-2-19 Margin Trading
        public DbSet<TransactionQueueArbitrage> TransactionQueueArbitrage { get; set; }//Rita 31-5-19 Arbitrage Trading
        //public DbSet<ServiceConfiguration> ServiceConfiguration { get; set; }
        public DbSet<ProductConfiguration> ProductConfiguration { get; set; }
        //public DbSet<ProviderConfiguration> ProviderConfiguration { get; set; }
        public DbSet<RouteConfiguration> RouteConfiguration { get; set; }
        public DbSet<ThirdPartyAPIConfiguration> ThirdPartyAPIConfiguration { get; set; }
        public DbSet<ThirdPartyAPIResponseConfiguration> ThirdPartyAPIResponseConfiguration { get; set; }
        //public DbSet<TradePoolMaster> TradePoolMaster { get; set; }
        public DbSet<TradeCancelQueue> TradeCancelQueue { get; set; }
        public DbSet<TradeCancelQueueMargin> TradeCancelQueueMargin { get; set; }//Rita 21-2-19 for Margin trading
        public DbSet<TradeCancelQueueArbitrage> TradeCancelQueueArbitrage { get; set; }//komal 07-06-2019 Arbitrage trading
        public DbSet<TradeTransactionStatus> TradeTransactionStatus { get; set; }
        public DbSet<Market> Market { get; set; }
        public DbSet<MarketMargin> MarketMargin { get; set; }//Rita 22-2-19 for Margin trading
        public DbSet<MarketArbitrage> MarketArbitrage { get; set; }//Rita 31-05-19 for Arbitrage trading
        public DbSet<TransactionRecon> TransactionRecon { get; set; }
        public DbSet<TransactionMarketType> TransactionMarketType { get; set; } //Uday 12-01-2019  MarketType Entity
        public DbSet<TradingRecon> TradingRecon { get; set; } //Khushali 18-03-2019 for Trading Recon
        public DbSet<TradingReconArbitrage> TradingReconArbitrage { get; set; } //komal 24-06-2019 for Trading Recon
        //public DbSet<TradingReconMargin> TradingReconMargin { get; set; } //Rita 15-05-2019 for Trading Recon
        public DbSet<CloseOpenPostionMargin> CloseOpenPostionMargin { get; set; }//Rita 13-4-19 for request-and-process entry

        public DbSet<ArbitrageLPAddressMaster> ArbitrageLPAddressMaster { get; set; }
        public DbSet<ArbitrageChargeConfigurationDetail> ArbitrageChargeConfigurationDetail { get; set; }
        public DbSet<ArbitrageChargeConfigurationMaster> ArbitrageChargeConfigurationMaster { get; set; }
        public DbSet<ArbitrageTrnChargeLog> ArbitrageTrnChargeLog { get; set; }
        public DbSet<ArbitrageDepositFund> ArbitrageDepositFund { get; set; }
        public DbSet<ArbitrageThirdPartyAPIResponseConfiguration> ArbitrageThirdPartyAPIResponseConfiguration { get; set; }
        public DbSet<ArbitrageThirdPartyAPIConfiguration> ArbitrageThirdPartyAPIConfiguration { get; set; }
        public DbSet<ArbitrageTransactionRequest> ArbitrageTransactionRequest { get; set; }


        //2019-02-20 margin entity
        public DbSet<MarginBlockWalletTrnTypeMaster> MarginBlockWalletTrnTypeMaster { get; set; }
        public DbSet<MarginTransactionAccount> MarginTransactionAccount { get; set; }
        public DbSet<MarginWalletAuthorizeUserMaster> MarginWalletAuthorizeUserMaster { get; set; }
        public DbSet<MarginWalletLedger> MarginWalletLedger { get; set; }
        public DbSet<MarginWalletMaster> MarginWalletMaster { get; set; }
        public DbSet<MarginWalletTransactionOrder> MarginWalletTransactionOrder { get; set; }
        public DbSet<MarginWalletTransactionQueue> MarginWalletTransactionQueue { get; set; }
        public DbSet<MarginWalletTypeMaster> MarginWalletTypeMaster { get; set; }
        public DbSet<MarginWTrnTypeMaster> MarginWTrnTypeMaster { get; set; }
        public DbSet<MarginUserRoleMaster> MarginUserRoleMaster { get; set; }

        public DbSet<MarginSpecialChargeConfiguration> MarginSpecialChargeConfiguration { get; set; }
        public DbSet<MarginTrnChargeLog> MarginTrnChargeLog { get; set; }
        public DbSet<MarginChargeConfigurationMaster> MarginChargeConfigurationMaster { get; set; }
        public DbSet<MarginChargeConfigurationDetail> MarginChargeConfigurationDetail { get; set; }
        public DbSet<OpenPositionMaster> OpenPositionMaster { get; set; } //ntrivedi 28-03-2019
        public DbSet<Core.Entities.MarginEntitiesWallet.OpenPositionDetail> OpenPositionDetail { get; set; } //ntrivedi 28-03-2019
        public DbSet<MarginPNLAccount> MarginPNLAccount { get; set; } //ntrivedi 28-03-2019
        public DbSet<MarginCloseUserPositionWallet> MarginCloseUserPositionWallet { get; set; } //ntrivedi 01-05-2019
        public DbSet<DepositionRequired> DepositionRequired { get; set; } //ntrivedi 01-05-2019
        public DbSet<DestroyFundRequest> DestroyFundRequest { get; set; }
        public DbSet<TokenTransferHistory> TokenTransferHistory { get; set; }
        //public DbSet<MarginChargeOrder> MarginChargeOrder { get; set; } //ntrivedi 05-04-2019

        //Darshan Dholakiya added the entity for Arbitrage service configuration changes:10-06-2019
        public DbSet<ServiceDetailArbitrage> ServiceDetailArbitrage { get; set; }

        //Darshan Dholakiya added the entity for Arbitrage service configuration changes:10-06-2019
        public DbSet<ServiceStasticsArbitrage> ServiceStasticsArbitrage { get; set; }

        //Darshan Dholakiya added the entity for Arbitrage service configuration changes:10-06-2019
        public DbSet<ServiceTypeMappingArbitrage> ServiceTypeMappingArbitrage { get; set; }

        //Darshan Dholakiya added the entity for Arbitrage service configuration changes:10-06-2019
        public DbSet<ProductConfigurationArbitrage> ProductConfigurationArbitrage { get; set; }

        //Darshan Dholakiya added the entity for Arbitrage Provider Demon configuration changes:17-06-2019
        public DbSet<DemonConfigurationArbitrage> DemonConfigurationArbitrage { get; set; }

        //Darshan Dholakiya added the DbQuery for Arbitrage Provider CCXT Lp Changes changes:25-07-2019
        public DbQuery<CCXTTranNo> CCXTTranNos { get; set; }



        //public DbQuery<WalletLimitConfiguration> WLimitConfigObj { get; set; }
        public DbQuery<ChargeConfigurationMasterRes> ChargeConfigurationMasterRes { get; set; }
        public DbQuery<ChargeConfigurationDetailRes> ChargeConfigurationDetailRes { get; set; }
        public DbQuery<ChargeConfigurationDetailArbitrageRes> ChargeConfigurationDetailArbitrageRes { get; set; }
        public DbQuery<HistorySumAmount> WithdrawHistoryObj { get; set; }
        public DbQuery<SumAmountAndCount> SumAmountAndCount { get; set; }
        public DbQuery<ConfigureLP> ConfigureLP { get; set; }  //khushali 14-05-2019        
        public DbQuery<ConfigureLPArbitrage> ConfigureLPArbitrage { get; set; }  //khushali 14-05-2019        
        public DbQuery<ReleaseAndStuckOrdercls> ReleaseAndStuckOrder { get; set; }  //khushali 03-04-2019      
        public DbQuery<LPStatusCheckData> LPStatusCheckData { get; set; }  //khushali 23-01-2019
        public DbQuery<LPStatusCheckDataArbitrage> LPStatusCheckDataArbitrage { get; set; }  //khushali 23-01-2019
        public DbQuery<StopLossArbitargeResponse> StopLossArbitargeResponse { get; set; }  //khushali 27-07-2019  for stop loss cron       
        public DbQuery<TradingSummaryLPViewModel> TradingSummaryLPViewModel { get; set; }
        public DbQuery<TradingReconHistoryViewModel> TradingReconHistoryViewModel { get; set; }
        public DbQuery<TradingSummaryViewModel> TradingSummaryViewModel { get; set; }
        public DbQuery<TradeSettledHistoryQueryResponse2V1> SettledHistory2V1 { get; set; }
        public DbQuery<GetTradeHistoryInfoV1> GetTradeHistoryInfo { get; set; } //komal 08-07-2019
        public DbQuery<GetTradeHistoryInfoArbitrageV1> GetTradeHistoryInfoArbitrageV1 { get; set; } //komal 08-07-2019
        public DbQuery<GetOrderHistoryInfo> GetOrderHistoryInfo { get; set; } //komal 08-07-2019
        public DbQuery<GetOrderHistoryInfoArbitrageV1> GetOrderHistoryInfoArbitrageV1 { get; set; } //komal 08-07-2019
        public DbQuery<RecentOrderInfoV1> RecentOrderInfoV1 { get; set; } //komal 12-10-2018
        public DbQuery<RecentOrderInfoArbitrageV1> RecentOrderInfoArbitrageV1 { get; set; } //komal 12-10-2018
        public DbQuery<CommunicationProviderList> CommunicationProviderList { get; set; }
        public DbQuery<WalletGraphRes> WalletGraphRes { get; set; }
        public DbQuery<TransactionTypewiseCount> TransactionTypewiseCount { get; set; }
        public DbQuery<BizResponseClass> BizResponseClass { get; set; }
        public DbQuery<ChannelwiseTranCount> ChannelwiseTranCount { get; set; }
        public DbQuery<OrgMasterRes> OrgMasterRes { get; set; }
        public DbQuery<TodaysCounts> TodaysCounts { get; set; }
        public DbQuery<TempCount> TempCount { get; set; }
        public DbQuery<TypeWiseDetail> TypeWiseDetail { get; set; }
        public DbQuery<OrgDetail> OrgDetail { get; set; }
        public DbQuery<UserRes> UserRes { get; set; }
        public DbQuery<BalanceResponseLimit> BalanceResponseLimit { get; set; }
        public DbQuery<IncomingTrnRes> IncomingTrnRes { get; set; }
        public DbQuery<IncomingTrnResv2> IncomingTrnResv2 { get; set; }
        public DbQuery<TransfersRes> TransfersRes { get; set; }
        public DbQuery<WalletMasterResponse> WalletMasterResponse { get; set; }
        public DbQuery<GetIEOAdminWalletRes> GetIEOAdminWalletRes { get; set; }
        public DbQuery<GetUserBankReq> GetUserBankReq { get; set; }
        public DbQuery<FiatCoinConfigurationRes> FiatCoinConfigurationRes { get; set; }
        public DbQuery<IEOWalletResponse> IEOWalletResponse { get; set; } //Chirag 26/07/2019
        public DbQuery<IEORoundResponse> IEORoundResponse { get; set; } //Chirag 06/08/2019
        public DbQuery<IEOPurchaseHistoryResponse> IEOPurchaseHistoryResponse { get; set; } //Chirag 27/07/2019
        public DbQuery<IEOPurchaseHistoryResponseBO> IEOPurchaseHistoryResponseBO { get; set; }
        public DbQuery<IEOCronMasterResponse> IEOCronMasterResponse { get; set; } //Chirag 02/08/2019
        public DbQuery<PurchaseWallets> PurchaseWallets { get; set; } //Chirag 27/07/2019
        public DbQuery<PurchaseWalletsBack> PurchaseWalletsBack { get; set; } //Chirag 06/08/2019
        public DbQuery<IEORoundResponse> RoundsMasterBack { get; set; } //Chirag 06/08/2019
        public DbQuery<Allocation> Allocation { get; set; } //Chirag 27/07/2019
        public DbQuery<AllocationBack> AllocationBack { get; set; } //Chirag 27/07/2019
        public DbQuery<IEOCronMailData> IEOCronMailData { get; set; } //Chirag 06/08/2019
        public DbQuery<Balance> Balance { get; set; }
        public DbQuery<BalanceTotal> BalanceTotal { get; set; }
        public DbQuery<BalanceLat> BalanceLat { get; set; }//Rushabh 13-12-2019
        public DbQuery<ChargeCurrency> ChargeCurrency { get; set; }
        public DbQuery<WalletTrnLimitConfigResp> WalletTrnLimitConfig { get; set; }
        public DbQuery<ChargeWalletId> ChargeWalletId { get; set; }
        public DbQuery<AffiliateCommissionHistoryReport> AffiliateCommissionHistoryReport { get; set; }
        public DbQuery<InviteFrdClaas> InviteFrdClaas { get; set; }
        public DbQuery<GetMonthWiseCommissionData> GetMonthWiseCommissionData { get; set; }
        public DbQuery<WalletTypeMasterRes> WalletTypeMasterRes { get; set; }
        public DbQuery<WithdrwalAdminReqRes> WithdrwalAdminReqRes { get; set; }
        public DbQuery<TokenSupplyRes> TokenSupplyRes { get; set; }
        public DbQuery<DestroyBlackFundRes> DestroyBlackFundRes { get; set; }
        public DbQuery<TokenTransferRes> TokenTransferRes { get; set; }
        public DbQuery<SetTransferFeeRes> SetTransferFeeRes { get; set; }
        public DbQuery<LPWalletMismatchRes> LPWalletMismatchRes { get; set; }

        public DbQuery<TransactionPolicyRes> TransactionPolicyRes { get; set; }
        public DbQuery<UserWalletBlockTrnTypeRes> UserWalletBlockTrnTypeRes { get; set; }
        public DbQuery<AddWalletRequestRes> AddWalletRequestRes { get; set; }
        public DbQuery<UserWalletWise> UserWalletWise { get; set; }
        public DbQuery<ImpExpAddressRes> ImpExpAddressRes { get; set; }
        public DbQuery<AddressRes> AddressRes { get; set; }
        public DbQuery<DepositeCounterRes> DepositeCounterRes { get; set; }
        public DbQuery<StakingPolicyDetailRes> StakingPolicyDetailRes { get; set; }
        public DbQuery<StakingPolicyDetailResV2> StakingPolicyDetailResV2 { get; set; }
        public DbQuery<StopLossRes> StopLossRes { get; set; }
        public DbQuery<StakingPolicyRes> StakingPolicyRes { get; set; }
        public DbQuery<UnStakingHistory> UnStakingHistory { get; set; }
        public DbQuery<UnStakingHistoryv2> UnStakingHistoryv2 { get; set; }
        public DbQuery<UnStakingHistoryData> UnStakingHistoryData { get; set; }
        public DbQuery<UnStakingHistoryDatav2> UnStakingHistoryDatav2 { get; set; }
        public DbQuery<LeaderBoardRes> LeaderBoardRes { get; set; }
        public DbQuery<HistoricalPerformanceTemp> HistoricalPerformanceTemp { get; set; }
        public DbQuery<MarginWalletMasterRes> MarginWalletMasterRes { get; set; }
        public DbQuery<MarginWalletByUserIdRes> MarginWalletByUserIdRes { get; set; }
        public DbQuery<ArbitrageWalletMasterRes> ArbitrageWalletMasterRes { get; set; }
        public DbQuery<ProviderWalletRes> ProviderWalletRes { get; set; }

        public DbQuery<MarginWalletMasterRes2> MarginWalletMasterRes2 { get; set; }

        public DbQuery<TempEntity> TempEntity { get; set; }
        public DbQuery<GetRoleId> GetRoleId { get; set; }

        public DbQuery<GetPermissionGroup2> GetPermissionGroup { get; set; }
        public DbQuery<GetPermissionGroup> ListPermissionGroup { get; set; }
        public DbQuery<Dipositions> Dipositions { get; set; }
        public DbQuery<ChargesTypeWise> ChargesTypeWise { get; set; }
        public DbQuery<AdminAssetsres> AdminAssetsres { get; set; }
        public DbQuery<LeaverageReportRes> LeaverageReportRes { get; set; }
        public DbQuery<LeaverageReportResv2> LeaverageReportResv2 { get; set; }
        public DbQuery<LeaverageReport> LeaverageReport { get; set; }
        public DbQuery<LeaverageReportv2> LeaverageReportv2 { get; set; }
        public DbQuery<OrgWalletres> OrgWalletres { get; set; }

        public DbQuery<TrnChargeLogRes> TrnChargeLogRes { get; set; }
        public DbQuery<TrnChargeLogResv2> TrnChargeLogResv2 { get; set; }
        public DbQuery<LPArbitrageTrnChargeLogRes> LPArbitrageTrnChargeLogRes { get; set; } //Chirag 01/07/2019
        public DbQuery<ArbitrageWalletTQRes> ArbitrageWalletTQRes { get; set; } //Chirag 02/07/2019
        public DbQuery<LeverageRes> LeverageRes { get; set; }
        public DbQuery<StakingPolicyMasterRes> StakingPolicyMasterRes { get; set; }
        public DbQuery<UserTypeRes> UserTypeRes { get; set; }
        public DbQuery<UserActivityLoging> UserActivityLoging { get; set; }
        public DbQuery<BlockTrnTypewiseReport> BlockTrnTypewiseReport { get; set; }
        public DbQuery<AuthAppRes> AuthAppRes { get; set; }
        public DbQuery<WalletPolicyAllowedDayRes> WalletPolicyAllowedDayRes { get; set; }
        public DbQuery<WalletusagePolicyRes2> WalletusagePolicyRes { get; set; }
        public DbQuery<MinMaxRanges> MinMaxRanges { get; set; }
        public DbQuery<Counts> Counts { get; set; }
        public DbQuery<WalletAuthorizeUserRes> WalletAuthorizeUserRes { get; set; }
        public DbQuery<AllowTrnTypeRoleWiseRes> AllowTrnTypeRoleWiseRes { get; set; }
        public DbQuery<WalletTransactiondata> WalletTransactiondata { get; set; }
        public DbQuery<TranDetails> TranDetails { get; set; }
        public DbQuery<WalletRes> WalletRes { get; set; }
        public DbQuery<WalletResV1> WalletResV1 { get; set; }
        public DbQuery<GetRoleHistoryData> GetRoleHistoryData { get; set; }
        public DbQuery<IEOTokenReportDataRes> IEOTokenReportDataRes { get; set; }
        public DbQuery<IEOAllocatedTokenReportDataRes> IEOAllocatedTokenReportDataRes { get; set; }
        public DbQuery<TokenCountRes> TokenCountRes { get; set; }
        public DbQuery<AllocateTokenCountRes> AllocateTokenCountRes { get; set; }

        public DbQuery<TemplateMasterData> TemplateMasterData { get; set; }
        public DbQuery<TransactionProviderResponse> TransactionProviderResponse { get; set; } // ntrivedi 03-10-2018
        public DbQuery<TransactionProviderResponseV1> TransactionProviderResponseV1 { get; set; } // Khushali 11-07-2019
        public DbQuery<TransactionProviderArbitrageResponse> TransactionProviderArbitrageResponse { get; set; } // Rita 05-06-2019
        public DbQuery<ArbitrageTransactionProviderResponse> ArbitrageTransactionProviderResponse { get; set; }
        public DbQuery<ArbitrageTopUpHistory> ArbitrageTopUpHistory { get; set; }
        public DbQuery<sqlGraphRes> sqlGraphRes { get; set; }
        public DbQuery<NewGraphRes> NewGraphRes { get; set; }
        public DbQuery<sqlGraphAmountRes> sqlGraphAmountRes { get; set; }

        public DbQuery<GetLTP> GetLTP { get; set; }
        public DbQuery<FiatBuyHistory> FiatBuyHistory { get; set; }
        public DbQuery<FiatSellHistory> FiatSellHistory { get; set; }//mansi 15-10-2019
        public DbQuery<GetFiatCurrencyInfo> GetFiatCurrencyInfo { get; set; }//mansi 16-10-2019
        public DbQuery<FiatSellWithdrawTraxn> FiatSellWithdrawTraxn { get; set; }//komal 1 Nov 2019 1:40 for fiatSellWithdraw
        public DbQuery<GetFiatTradeInfo> GetFiatTradeInfo { get; set; }

        public DbQuery<FiatTradeConfigurationRes> FiatTradeConfigurationRes { get; set; }

        public DbQuery<TransactionProviderResponseForWithdraw> TransactionProviderResponseForWithdraw { get; set; } // ntrivedi       03-10-2018
        public DbQuery<TransactionProviderResponse3> TransactionProviderResponse3 { get; set; } // ntrivedi 03-10-2018
        public DbQuery<TransactionProviderResponse2> TransactionProviderResponse2 { get; set; } // ntrivedi 03-10-2018
        public DbQuery<ActiveOrderDataResponse> ActiveOrderDataResponse { get; set; } //komal 12-10-2018
        public DbQuery<ActiveOrderInfoV1> ActiveOrderInfoV1 { get; set; } //komal 12-10-2018
        public DbQuery<ActiveOrderInfoArbitrageV1> ActiveOrderInfoArbitrageV1 { get; set; } //komal 12-10-2018
        public DbQuery<ActiveOrderDataResponseArbitrage> ActiveOrderDataResponseArbitrage { get; set; } //komal 08-06-2019 Arbitrage method
        public DbQuery<GetBuySellBook> BuyerSellerInfo { get; set; } //uday 12-10-2018
        public DbQuery<AffiliateSchemeTypeMappingListViewModel> AffiliateSchemeTypeMappingListViewModel { get; set; }
        public DbQuery<StopLimitBuySellBook> StopLimitBuyerSellerBook { get; set; } //komal 12-10-2018
        //public DbQuery<GetGraphResponse> GetGraphResponse { get; set; } //uday 22-10-2018 
        public DbQuery<GetGraphDetailInfo> GetGraphResponse { get; set; } //uday 14-11-2018
        public DbQuery<TrnChargeSummaryViewModel> chargeSummaryViewModels { get; set; } //komal 28-11-2018
        public DbQuery<ArbitrageAddressRes> ArbitrageAddressRes { get; set; }
        public DbQuery<TradePairTableResponse> TradePairTableResponse { get; set; } //uday 03-11-2018
        public DbQuery<ServiceMasterResponse> ServiceMasterResponse { get; set; } //uday 05-11-2018
        public DbQuery<GetGraphResponsePairWise> GetGraphResponseByPair { get; set; } //khushali 06-11-2018
        public DbQuery<TradePairConfigRequest> PairConfigurationResponse { get; set; } //Uday 15-11-2018
        public DbQuery<ProductConfigrationGetInfo> ProductConfigrationResponse { get; set; } //Uday 15-11-2018
        public DbQuery<RptWithdrawal> RptWithdrawal { get; set; }
        public DbQuery<RptWithdrawalv2> RptWithdrawalv2 { get; set; }
        public DbQuery<HistoryObject> HistoryObject { get; set; }
        public DbQuery<HistoryObjectv2> HistoryObjectv2 { get; set; }
        public DbQuery<HistoryObjectNew> HistoryObjectNew { get; set; }
        public DbQuery<HistoryObjectNewv2> HistoryObjectNewv2 { get; set; }
        public DbQuery<ValidationWithdrawal> ValidationWithdrawal { get; set; }
        public DbQuery<WithdrawHistoryObject> WithdrawHistoryObject { get; set; } //Uday 15-01-2019 
        public DbQuery<BalanceResponse> BalanceResponse { get; set; }
        public DbQuery<WithdrawHistoryObjectv2> WithdrawHistoryObjectv2 { get; set; }
        public DbQuery<CommonCountClass> CommonCountClass { get; set; }
        public DbQuery<BeneUpdate> BeneUpdate { get; set; }
        public DbQuery<OutgoingTrnRes> OutgoingTrnRes { get; set; }
        public DbQuery<OutgoingTrnResv2> OutgoingTrnResv2 { get; set; }
        public DbQuery<HighLowViewModel> HighLowViewModel { get; set; } //Uday 04-12-2018
        public DbQuery<FavouritePairInfo> FavouritePairViewModel { get; set; } //Uday 05-12-2018
        public DbQuery<ExchangeProviderListArbitrage> ExchangeProviderListArbitrage { get; set; } //Rita 07-06-19 for Arbitrage LP list with LTP
        public DbQuery<SmartArbitrageHistoryInfo> SmartArbitrageHistoryInfo { get; set; } //Rita 14-06-19 for Arbitrage History
        public DbQuery<ArbitrageBuySellViewModel> ExchangeProviderBuySellBookArbitrage { get; set; } //Komal 11-06-2019 for Arbitrage LP list with LTP

        public DbQuery<UserWithdrawalDTO> GetUserWithdrawalHistory { get; set; }//Rushabh 09-05-2020
        public DbQuery<UserWalletBalDTO> GetAllUserWalletBalance { get; set; }//Rushabh 09-05-2020
        public DbQuery<UserTradingSummaryDTO> GetUserTradingSummary { get; set; }//Rushabh 09-05-2020

        public DbQuery<CurrencyCummulativeData.CurrencyCummulativeDataAct> CummulativeData { get; set; } //Chirag 08/05/2020
        public DbQuery<SuspiciousUserPersonalInfo> SuspiPersDetails { get; set; } //Chirag 04/05/2020
        public DbQuery<UserActivityReportActConfirmedEmailResp.UserActivityReportConfEmailResp> UserActivityReportConfEmailResp { get; set; } //Chirag 29/02/2020 For User Unique Actions Tracing Report
        public DbQuery<UserActivityReportConfEmailResp2> UserActivityReportConfEmailResp2 { get; set; } //Chirag 05/05/2020 For User Unique Actions Tracing Report

        public DbQuery<WithdrawalSummaryViewModel> WithdrawalSummaryViewModel { get; set; } //Uday 03-12-2018
        public DbQuery<PairTradeSummaryQryResponse> PairTradeSummaryViewModel { get; set; } //komal 08-12-2018
        public DbQuery<AvailableRoute> AvailableRoutes { get; set; } //komal 13-12-2018
        public DbQuery<GetAllWithdrawQueryResponse> WithdrawRoute { get; set; } //komal 19-12-2018
        public DbQuery<ListPairInfo> ListPairInfo { get; set; } //komal 19-12-2018
        public DbQuery<ChannelMasterRes> ChannelMasterRes { get; set; }
        public DbQuery<TradeSettledHistoryQueryResponse2> SettledHistory2 { get; set; }
        public DbQuery<GetTradeRouteConfigurationData> TradeRouteConfigurationData { get; set; }
        public DbQuery<PreStackingConfirmationRes> PreStackingConfirmationRes { get; set; }
        public DbQuery<TransactionBlockedChannelRes> TransactionBlockedChannelRes { get; set; }
        public DbQuery<CompainDetailResponse> compainDetailResponse { get; set; }  //Add by pankaj For Get Detail 
        public DbQuery<UserWiseCompaintDetailResponce> userWiseCompaintDetailResponce { get; set; }
        public DbQuery<BeneficiaryMasterRes> BeneficiaryMasterRes { get; set; }
        public DbQuery<BeneficiaryMasterRes1> BeneficiaryMasterRes1 { get; set; }
        public DbQuery<UserDetailRes> UserDetailRes { get; set; }
        public DbQuery<ActiveTradeUserCount> ActiveTradeUserCount { get; set; } //Uday 20-12-2018
        public DbQuery<ConfigurationCount> ConfigurationCount { get; set; } //Uday 20-12-2018
        public DbQuery<LedgerCountInfo> LedgerCount { get; set; }//komal 21-12-2018
        public DbQuery<OrderSummaryCount> OrderSummaryCount { get; set; } // Uday 21-12-2018
        public DbQuery<TradeSummaryCount> TradeSummaryCount { get; set; } // Uday 21-12-2018
        public DbQuery<ProfitSummaryCount> ProfitSummaryCount { get; set; } //Uday 22-12-2018
        public DbQuery<MarketTickerPairData> MarketTickerPairData { get; set; } //Uday 23-12-2018
        public DbQuery<VolumeDataRespose> MarketTickerVolumeDataRespose { get; set; } //Uday 23-12-2018
        public DbQuery<OpenOrderQryResponse> OpenOrderRespose { get; set; } //komal 28-12-2018
        public DbQuery<TopLooserGainerPairData> TopLooserPairData { get; set; } //Uday 24-12-2018
        public DbQuery<PairStatisticsCalculation> PairStatisticsCalculation { get; set; } //Uday 24-12-2018
        public DbQuery<WalletMasterRes> WalletMasterRes { get; set; }
        public DbQuery<StakingHistoryRes> StakingHistoryRes { get; set; }
        public DbQuery<StakingHistoryResv2> StakingHistoryResv2 { get; set; }
        public DbQuery<HistoricalPerformance> HistoricalPerformance { get; set; } //Uday 08-01-2019
        public DbQuery<EmailLists> emailLists { get; set; } //komal 21-01-2019
        public DbQuery<EmailSMSCountResp> EmailSMSCount { get; set; }
        public DbQuery<CopiedLeaderOrdersQryRes> copiedLeaderOrders { get; set; }//komal 28-01-2019
        public DbQuery<CopiedLeaderOrdersInfoV1> CopiedLeaderOrdersInfoV1 { get; set; }//komal 28-01-2019
        public DbQuery<TopLeaderListInfo> topLeaderLists { get; set; }//komal 29-01-2019
        public DbQuery<TradeWatchListsQryRes> tradeWatchLists { get; set; }//komal 29-01-2019
        public DbQuery<SiteTokenConversionQueryRes> siteTokenConversions { get; set; }
        public DbQuery<SocketFeedConfigQueryRes> feedConfigQueryRes { get; set; }
        public DbQuery<ViewAPIPlanDetailQryResponse> viewAPIPlans { get; set; }
        public DbQuery<ViewAPIPlanDetailQryResponseV2> viewAPIPlansV2 { get; set; }
        public DbQuery<PlanMethodsQryResponse> PlanMethodRes { get; set; }
        public DbQuery<ViewActivePlanDetailInfo> viewActivePlanDetails { get; set; }
        public DbQuery<GetUserDetail> GetUserDetail { get; set; }
        public DbQuery<ViewUnAssignedUserRes> ViewUsers { get; set; }
        public DbQuery<AutoRenewDetailQryRes> AutoRenewDetailQryRes { get; set; }
        public DbQuery<GetActivationDate> GetActivationDate { get; set; }
        public DbQuery<UserAPIPlanHistoryQryRes> UserAPIPlanHistoryQryRes { get; set; }
        public DbQuery<APIPlanUserCountResponseInfo> APIPlanUserCountQryRes { get; set; }
        public DbQuery<UserSubscribeHistoryBKQryRes> userSubscribeHistories { get; set; }
        public DbQuery<IPWhitelistCountQryRes> iPWhitelistCountQryRes { get; set; }
        public DbQuery<APIKeyCountQryRes> aPIKeyCountQryRes { get; set; }
        public DbQuery<ViewPublicAPIKeysInfoQryRes> publicAPIKeysInfoQryRes { get; set; }
        public DbQuery<APIKeysDetailsInfoQryRes> keysDetailsInfoQryRes { get; set; }
        public DbQuery<APIPlanMethodConfigHistoryRes> APIPlanMethodConfigHistoryRes { get; set; }
        public DbQuery<APIRequestStatisticsCountQryRes> APIRequestStatisticsCountQryRes { get; set; }
        public DbQuery<PlanUsersQryRes> PlanUsersQryRes { get; set; }
        public DbQuery<UserWiseAPIReqCountQryRes> UserWiseAPIReqCountQryRes { get; set; }
        public DbQuery<APIReqStatusCodeCountQryRes> APIReqStatusCodeCountQryRes { get; set; }
        public DbQuery<APIReqBrowsweWiseQryRes> APIReqBrowsweWiseQryRes { get; set; }
        public DbQuery<HTTPErrorCodeQryRes> HTTPErrorCodeQryRes { get; set; }
        public DbQuery<FrequentUseAPIQryRes> FrequentUseAPIQryRes { get; set; }
        public DbQuery<FrequentUseAPIResponsInfo> FrequentUseAPIResponsInfo { get; set; }
        public DbQuery<MostActiveIPCountQryRes> MostActiveIPCountQryRes { get; set; }
        public DbQuery<MostActiveIPDetailsQryRes> MostActiveIPDetailsQryRes { get; set; }
        public DbQuery<GetCustomLimitIDQry> GetCustomLimitIDQry { get; set; }
        public DbQuery<APIMethodConfigListQryRes> GetAPIMethodConfigList { get; set; }
        public DbQuery<GetKeyWiseIPQryRes> GetKeyWiseIPQryRes { get; set; }
        public DbQuery<APIPlanConfigurationCountQryRes> APIPlanConfigurationCountQryRes { get; set; }
        public DbQuery<TransactionReportCountQryRes> TransactionReportCountQryRes { get; set; }
        public DbQuery<MostActiveIPWiseReportQryRes> MostActiveIPWiseReportQryRes { get; set; }
        public DbQuery<FrequentUseAPIWiseReportQryRes> FrequentUseAPIWiseReportQryRes { get; set; }
        public DbQuery<ReferralSchemeTypeMappingRes> ReferralSchemeTypeMappingData { get; set; }
        public DbQuery<ReferralServiceDetailRes> ReferralServiceDetailResData { get; set; }
        public DbQuery<BlockUserRes> BlockUserRes { get; set; }
        public DbQuery<ReturnCount> ReturnCount { get; set; }
        public DbQuery<ReferralChannelCount> ReferralChannelCount { get; set; }

        public DbSet<ServiceMaster> ServiceMaster { get; set; }
        public DbSet<ServiceMasterMargin> ServiceMasterMargin { get; set; }//Rita 15-2-19 Margin Trading
        public DbSet<ServiceMasterArbitrage> ServiceMasterArbitrage { get; set; }//Rita 31-05-19 Margin Trading
        public DbSet<ServiceDetail> ServiceDetail { get; set; }
        public DbSet<ServiceDetailMargin> ServiceDetailMargin { get; set; }//Rita 23-2-19 Margin Trading
        public DbSet<ServiceStastics> ServiceStastics { get; set; }
        public DbSet<ServiceStasticsMargin> ServiceStasticsMargin { get; set; }//Rita 28-2-19 Margin Trading
        public DbSet<Limits> Limits { get; set; }
        public DbSet<AppType> AppType { get; set; }
        public DbSet<OrderTypeMaster> OrderTypeMaster { get; set; }
        public DbSet<DemonConfiguration> DemonConfiguration { get; set; }
        public DbSet<ServiceProConfiguration> ServiceProConfiguration { get; set; }
        public DbSet<ServiceProviderDetail> ServiceProviderDetail { get; set; }
        public DbSet<ServiceProviderType> ServiceProviderType { get; set; }
        public DbSet<ServiceProviderMaster> ServiceProviderMaster { get; set; }
        public DbSet<TradeStopLoss> TradeStopLoss { get; set; }
        public DbSet<TradeStopLossMargin> TradeStopLossMargin { get; set; }//Rita 15-2-19 Margin Trading
        public DbSet<TradeStopLossArbitrage> TradeStopLossArbitrage { get; set; }//Rita 31-05-19 Arbitrage Trading
        public DbSet<IpMaster> IpMaster { get; set; }
        public DbSet<AllowesOrderTypeArbitrage> AllowesOrderTypeArbitrage { get; set; } // Rita 05-06-2019 
        public DbSet<TransactionStatusCheckRequestArbitrage> TransactionStatusCheckRequestArbitrage { get; set; } //Rushabh 17-06-2019
        public DbSet<TradeBuyRequest> TradeBuyRequest { get; set; }
        public DbSet<CountryMaster> CountryMaster { get; set; } //uday 17-10-2018
        public DbSet<StateMaster> StateMaster { get; set; } //uday 17-10-2018
        public DbSet<PoolOrder> PoolOrder { get; set; } //uday 17-10-2018
        public DbSet<CityMaster> CityMaster { get; set; } //uday 22-10-2018
        public DbSet<ZipCodeMaster> ZipCodeMaster { get; set; } //uday 22-10-2018
        public DbSet<TradeGraphDetail> TradeGraphDetail { get; set; } //uday 22-10-2018
        public DbSet<TradeGraphDetailMargin> TradeGraphDetailMargin { get; set; } //uday 22-10-2018
        public DbSet<TradeGraphDetailArbitrage> TradeGraphDetailArbitrage { get; set; } //Rita 04-06-19 fro Arbitrage Trading
        public DbSet<ServiceTypeMapping> ServiceTypeMapping { get; set; } //uday 24-10-2018
        public DbSet<ServiceTypeMappingMargin> ServiceTypeMappingMargin { get; set; } //Rita 28-2-19 for Margin trading
        public DbSet<TradePairStastics> TradePairStastics { get; set; } //uday 25-10-2018
        public DbSet<TradePairStasticsMargin> TradePairStasticsMargin { get; set; } //Rita 15-2-19 for marging trading
        public DbSet<TradePairStasticsArbitrage> TradePairStasticsArbitrage { get; set; } //Rita 31-05-19 for Arbitrage trading
        public DbSet<TransactionRequest> TransactionRequest { get; set; } //Rita 27-10-2018
        public DbSet<FavouritePair> FavouritePair { get; set; } //Uday 29-10-2018
        public DbSet<FavouritePairMargin> FavouritePairMargin { get; set; } //Rita 23-2-19 for Margin Trading
        public DbSet<ChargeRuleMaster> ChargeRuleMaster { get; set; } //Uday 30-10-2018
                                                                      // public DbSet<LimitRuleMaster> LimitRuleMaster { get; set; } //Uday 30-10-2018
                                                                      // public DbSet<TradeBuyerList> TradeBuyerList { get; set; } //Uday 30-10-2018
        public DbSet<TradeBuyerListV1> TradeBuyerListV1 { get; set; } //Rita 22-11-2018
        public DbSet<TradeBuyerListMarginV1> TradeBuyerListMarginV1 { get; set; } //Rita 15-2-19 Marging trading
        public DbSet<TradeBuyerListArbitrageV1> TradeBuyerListArbitrageV1 { get; set; } //Rita 31-05-19 Arbitrage trading
        //public DbSet<TradeSellerList> TradeSellerList { get; set; } //Uday 30-10-2018
        public DbSet<TradeSellerListV1> TradeSellerListV1 { get; set; } //Rita 22-11-2018
        public DbSet<TradeSellerListMarginV1> TradeSellerListMarginV1 { get; set; }//Rita 15-2-19 Margin Trading
        public DbSet<TradeSellerListArbitrageV1> TradeSellerListArbitrageV1 { get; set; }//Rita 31-05-19 Margin Trading
        public DbSet<SettledTradeTransactionQueue> SettledTradeTransactionQueue { get; set; } //Rita 31-10-2018
        public DbSet<SettledTradeTransactionQueueMargin> SettledTradeTransactionQueueMargin { get; set; } //Rita 15-2-19 Margin Trading
        public DbSet<SettledTradeTransactionQueueArbitrage> SettledTradeTransactionQueueArbitrage { get; set; } //Rita 31-5-19 Margin Trading
        public DbSet<TradePoolConfiguration> TradePoolConfiguration { get; set; } //Rita 31-10-2018
        //public DbSet<TradePoolQueue> TradePoolQueue { get; set; } //Rita 31-10-2018
        public DbSet<TradePoolQueueV1> TradePoolQueueV1 { get; set; } //Rita 22-11-2018
        public DbSet<TradePoolQueueMarginV1> TradePoolQueueMarginV1 { get; set; } //Rita 15-2-19 Margin Trading
        public DbSet<TradePoolQueueArbitrageV1> TradePoolQueueArbitrageV1 { get; set; } //Rita 05-06-19 Arbitrage Trading
        public DbSet<TransactionStatus> TransactionStatus { get; set; } //Rita 31-10-2018
        public DbSet<DeviceStore> DeviceStore { get; set; } //Khushali 03-11-2018

        public DbQuery<SumAmount> SumAmounts { get; set; } // ntrivedi 03-12-2018
        public DbQuery<CheckTrnRefNoRes> CheckTrnRefNoRes { get; set; }
        public DbQuery<GetCount> GetCount { get; set; } // ntrivedi 06-12-2018
        public DbQuery<TQTrnAmt> TQTrnAmt { get; set; } // ntrivedi 06-12-2018
        public DbQuery<ComplainMasterDataViewModel> ComplainMasterDataViewModel { get; set; }  // Pankaj 08/12/2018
        public DbQuery<CompainTrailViewModel> CompainTrailViewModel { get; set; }  // Pankaj 08/12/2018
        public DbQuery<PNLAccount> PNLAccount { get; set; }
        public DbQuery<OpenPosition> OpenPosition { get; set; }// ntrivedi 17-12-2018

        public DbQuery<OpenPositionDetailVM> OpenPositionDetailVM { get; set; }
        public DbQuery<BackoffComplainMasterViewModel> BackOffComplainData { get; set; }  // Nirav 24/12/2018
        public DbQuery<BackOffComplainTrailViewModel> BackOffCompainTrailData { get; set; }  // Nirav 24/12/2018
        public DbSet<ActivityTypeLog> ActivityTypeLog { get; set; }
        public DbSet<ActivityTypeHour> ActivityTypeHour { get; set; }
        public DbQuery<GetActivityLogData> GetActivityLogData { get; set; } // added by nirav savariya for get activity log report on 12-28-2018

        // Social profile trading table
        public DbSet<UserSocialProfile> UserSocialProfile { get; set; }
        public DbSet<FollowerMaster> FollowerMaster { get; set; }
        public DbSet<ProfileConfiguration> ProfileConfiguration { get; set; }
        public DbSet<UserProfileConfig> UserProfileConfig { get; set; }
        public DbQuery<SubscriptionProfile> SubscriptionProfileId { get; set; }  // Birju 22/12/2018
        public DbQuery<FrontLeaderProfile> FrontLeaderProfile { get; set; }  // Birju 26/12/2018
        public DbQuery<LeaderModel> FrontGetLeaderList { get; set; } // Birju 10/01/2019
        public DbQuery<GetProfileType> ProfileType { get; set; } // Birju 11/01/2019
        public DbQuery<FollowerList> FollowerList { get; set; } // Birju 11/01/2019
        public DbQuery<FollowerConfigList> FollowerConfigList { get; set; } // Birju 11/01/2019
        public DbQuery<SubscriptionProfileType> SubscriptionProfileType { get; set; }  // Birju 17/01/2018
        public DbQuery<LeaderModel> FollowerWiseLeaders { get; set; } // Birju 19/01/2019
        public DbSet<GroupMaster> GroupMaster { get; set; }
        public DbSet<WatchMaster> WatchMaster { get; set; }
        public DbQuery<GetWatcherModel> WatcherList { get; set; } // added by nirav savariya for follower wise get watch list on 25-01-2019 
        public DbQuery<GetWithdarwData> GetWithdarwData { get; set; }
        
        public DbSet<APIOrderSettlement> APIOrderSettlement { get; set; } //Rita 1-2-2019
        public DbSet<APIOrderSettlementArbitrage> APIOrderSettlementArbitrage { get; set; } //Rita 05-06-2019 for Arbitrage API log
        //public DbSet<GetLeaderwiseFollower> GetLeaderwiseFollowerConfigurationsList { get; set; } // Birju 10/01/2019
        //Organization process table for backoffice  admin panel
        public DbSet<Organization_Master> Organization_Master { get; set; }
        public DbSet<ApplicationMaster> ApplicationMaster { get; set; }
        public DbSet<Org_App_Mapping> Org_App_Mapping { get; set; }
        public DbSet<ActivityRegister> ActivityRegister { get; set; }
        public DbSet<ActivityRegisterDetail> ActivityRegisterDetail { get; set; }
        public DbSet<ActivityType_Master> ActivityType_Master { get; set; }
        public DbSet<HostURLMaster> HostURLMaster { get; set; }
        // Define the process table for User Related Option
        public DbSet<EmailMaster> EmailMaster { get; set; }
        public DbSet<ConfigurationMaster> ConfigurationMaster { get; set; }
        public DbSet<LanguageMaster> LanguageMaster { get; set; }
        public DbSet<UserConfigurationMapping> UserConfigurationMapping { get; set; }
        public DbSet<PhoneMaster> PhoneMaster { get; set; }
        public DbSet<AllowedIPAddress> AllowedIPAddress { get; set; }
        public DbSet<SecurityQuestionMaster> SecurityQuestionMaster { get; set; }
        public DbSet<IPRange> IPRange { get; set; }
        //public virtual DbSet<ApplicationGroupRoles> GroupRoleMapping { get; set; }

        public DbQuery<EmailListViewModel> emailListViewModels { get; set; }
        public DbQuery<MobileNumebrListViewModel> mobileNumebrListViewModels { get; set; }

        public DbSet<KYCIdentityMaster> kYCIdentityMaster { get; set; }
        public DbSet<KYCIdentityConfigurationMapping> kYCIdentityConfigurationMapping { get; set; }
        public DbSet<ComplaintPriorityMaster> ComplaintPriorityMaster { get; set; }
        public DbSet<UserPasswordPolicyMaster> UserPasswordPolicyMaster { get; set; }
        public DbQuery<KYCIndentityMappinglistViewModel> kYCIndentitylistViewModels { get; set; }
        public DbSet<DocumentMaster> DocumentMaster { get; set; }
        public DbQuery<KYCDocumentConfigurationListViewmodel> KYCDocumentConfigurationLists { get; set; }
        public DbQuery<KYCDocumentKYCFormatListViewmodel> KYCDocumentKYCFormatListViewmodels { get; set; }
        public DbQuery<KYCLevelWiseCount> KYCLevelWiseCount { get; set; }
        public DbQuery<KYCListFilterationDataViewModel> kYCListFilterationResponseViewModels { get; set; }
        public DbQuery<DocumentMasterListViewModel> DocumentMasterListViewModels { get; set; }
        public DbQuery<KYCIndentitylistViewModel> KYCIndentitylistViewModels { get; set; }

        public DbQuery<SignReportViewmodel> SignReportViewmodel { get; set; }
        public DbQuery<SignReportCountViewmodel> SignReportCountViewmodels { get; set; }
        public DbQuery<IPRangeDataviewmodel> IPRangeDataviewmodels { get; set; }
        public DbQuery<IPRangeGetDataViewModel> IPRangeGetData { get; set; }
        public DbQuery<ComplaintPrioritygetdataViewModel> ComplaintPrioritygetdata { get; set; }
        public DbQuery<UserPasswordPolicyMasterGetdataViewModel> UserPasswordPolicy { get; set; }
        public DbQuery<GetProfileConfigurationViewModel> GetProfileConfiguration { get; set; }  /// Added by pankaj kathiriya for get the profile configuration
        public DbQuery<ProfilewiseuserlistViewmodel> Profilewiseuserlist { get; set; }  /// Added by pankaj kathiriya for get the profile configuration

        public DbSet<UserLinkMaster> UserLinkMaster { get; set; }
        public DbQuery<ProfilelevelCountViewmodel> ProfilelevelCount { get; set; }

        public DbQuery<PasswordPolicyCheckViewModel> PasswordPolicyCheck { get; set; }
        public DbQuery<ProfileHistoryData> ProfileHistoryData { get; set; } // added by nirav savariya for get user wise profile history in activitylog on 19-1-2019
        public DbQuery<KYCLevelList> KYCLevelList { get; set; }
        public DbQuery<GetProfilelevelmaster> GetProfilelevelmaster { get; set; }
        public DbQuery<GetProfileConfigurationByIdViewModel> GetProfileConfigurationById { get; set; }
        public DbSet<WithdrawERCTokenQueue> WithdrawERCTokenQueue { get; set; } //Uday 30-01-2019  For ERCWithdraw Transaction
        public DbQuery<WithdrawERCAdminAddress> WithdrawERCAdminAddress { get; set; } //Uday 29-01-2019 Get ERC Admin Address

        //Affiliate System Entity  Uday 12-02-2019
        public DbSet<AffiliateUserMaster> AffiliateUserMaster { get; set; }
        public DbSet<AffiliateSchemeTypeMaster> AffiliateSchemeTypeMaster { get; set; }
        public DbSet<AffiliateSchemeTypeMapping> AffiliateSchemeTypeMapping { get; set; }
        public DbSet<AffiliateSchemeMaster> AffiliateSchemeMaster { get; set; }
        public DbSet<AffiliateSchemeDetail> AffiliateSchemeDetail { get; set; }
        public DbSet<AffiliatePromotionUserTypeMapping> AffiliatePromotionUserTypeMapping { get; set; }
        public DbSet<AffiliatePromotionShare> AffiliatePromotionShare { get; set; }
        public DbSet<AffiliatePromotionMaster> AffiliatePromotionMaster { get; set; }
        public DbSet<AffiliateLinkClick> AffiliateLinkClick { get; set; }
        public DbSet<AffiliatePromotionLimitConfiguration> AffiliatePromotionLimitConfiguration { get; set; }
        public DbSet<AffiliateCommissionHistory> AffiliateCommissionHistory { get; set; } // Store Affiliate Commission Detail
        public DbSet<AffiliateCommissionCron> AffiliateCommissionCron { get; set; } // Store Affiliate Commission Cron Detail
        public DbQuery<GetAffiliateSchemePlan> GetAffiliateSchemePlan { get; set; } // Get Affiliate Scheme Plan Detail
        public DbQuery<WalletLedgerResponse> WalletLedgerResponse { get; set; }

        public DbQuery<AffiliateAvailablePromotionLink> AffiliateAvailablePromotionLink { get; set; } // Get Available Affiliate Promotion Type
        public DbQuery<AffiliatePromotionLimitCount> AffiliatePromotionLimitCount { get; set; } //Get Affiliate Promotion Limit Count
        public DbQuery<AffiliateDashboardCount> AffiliateDashboardCount { get; set; } // Get Affiliate Dashboard Count 
        public DbQuery<GetAffiateUserRegisteredData> GetAffiateUserRegisteredData { get; set; } // Uday 04-03-2019 Get Affiliate User Register Data
        public DbQuery<GetReferralLinkClickData> GetReferralLinkClickData { get; set; } // Uday 04-03-2019 Get referral link count data
        public DbQuery<GetFacebookLinkClickData> GetFacebookLinkClickData { get; set; } // Uday 04-03-2019 Get Facebook link count data
        public DbQuery<GetTwitterLinkClickData> GetTwitterLinkClickData { get; set; } // Uday 04-03-2019 Get Twitter link count data
        public DbQuery<GetEmailSentData> GetEmailSentData { get; set; } // Uday 04-03-2019 Get Email Sent Data
        public DbQuery<GetSMSSentData> GetSMSSentData { get; set; } // Uday 04-03-2019 Get Email Sent Data
        public DbQuery<GetAllAffiliateUserData> GetAllAffiliateUserData { get; set; } // Uday 04-03-2019 Get All Affiliate User Data

        //Referral - Pratik Patel 08-02-2019        
        public DbSet<ReferralPayType> ReferralPayType { get; set; }
        public DbSet<ReferralChannel> ReferralChannel { get; set; }
        public DbSet<ReferralChannelType> ReferralChannelType { get; set; }
        public DbSet<ReferralService> ReferralService { get; set; }
        public DbSet<ReferralServiceType> ReferralServiceType { get; set; }
        public DbSet<ReferralUser> ReferralUser { get; set; }
        public DbSet<ReferralUserClick> ReferralUserClick { get; set; }
        public DbQuery<ReferralUserListViewModel> ListReferralUser { get; set; }
        public DbQuery<ReferralLimitCount> ReferralLimitCount { get; set; }
        public DbSet<ReferralRewards> ReferralRewards { get; set; }
        public DbSet<WithdrawAdminRequest> WithdrawAdminRequest { get; set; }
        //khushali 26-04-2019  for Cron on-off table        
        public DbSet<CronMaster> CronMaster { get; set; }

        // Module Access Control -Nishit Jani on A 2019-03-28 3:50 PM
        public DbSet<SubModuleFormMaster> SubModuleFormMaster { get; set; }
        public DbSet<ModuleGroupAccess> ModuleGroupAccess { get; set; }
        public DbSet<ModuleGroupMaster> ModuleGroupMaster { get; set; }

        //khushali Module Access Control CURD
        public DbQuery<MenuSubDetailViewModelV2> MenuSubDetailViewModelV2 { get; set; }
        public DbQuery<ChildNodeFiledViewModel> ChildNodeFiledViewModel { get; set; }

        public DbSet<CryptoWatcher> CryptoWatcher { get; set; }//khushali LTP Watcher
        public DbSet<CryptoWatcherArbitrage> CryptoWatcherArbitrage { get; set; }//Rita 05-06-19 for Arbitrage LTP Watcher
        public DbSet<TradingConfiguration> TradingConfiguration { get; set; }//khushali For Liquidity and market making - tading type
        public DbSet<LPFeedConfiguration> LPFeedConfiguration { get; set; }//mansi - LPConfig for OrederBook and TradeHistory
        public DbQuery<LPKeyVault> LPKeyVault { get; set; }  //khushali 27-05-2019 balance check for LP
        public DbQuery<ArbitrageCryptoWatcherQryRes> ArbitrageCryptoWatcherQryRes { get; set; }

        //ntrivedi margin trading query

        public DbQuery<LeveragePairDetail> LeveragePairDetail { get; set; }
        public DbQuery<PositionValue> PositionValue { get; set; }
        public DbSet<WithdrawLoanMaster> WithdrawLoanMaster { get; set; } //ntrivedi 12-04-2019
        //public DbSet<MarginAddressMaster> MarginAddressMaster { get; set; } //ntrivedi 25-04-2019
        //public DbSet<MarginDepositHistory> MarginDepositHistory { get; set; } //ntrivedi 25-04-2019




        //khushali 11-04-2019  Process for Release Stuck Order - wallet side         
        public DbQuery<CheckTransactionSuccessOrNotRes> CheckTransactionSuccessOrNotRes { get; set; }
        //Rita 25-4-19 for check entry exist or not
        //public DbQuery<WalletTransactionQueue> CheckSettlementProceed { get; set; }
        public DbSet<MarginTradingAllowToUser> MarginTradingAllowToUser { get; set; } //Rita 26-4-19 for allow margin trading
        public DbSet<ArbitrageTradingAllowToUser> ArbitrageTradingAllowToUser { get; set; } //Rita 17-7-19 for allow Arbitrage trading,Buy First,Sell First or non on smaart Arbitrage

        public DbQuery<UserUnstakingReq2> UserUnstakingReq2 { get; set; }


        //khushali 04-06-2019 for Arbitrage trading 
        public DbSet<RouteConfigurationArbitrage> RouteConfigurationArbitrage { get; set; }
        public DbSet<ServiceProConfigurationArbitrage> ServiceProConfigurationArbitrage { get; set; }
        public DbSet<ServiceProviderDetailArbitrage> ServiceProviderDetailArbitrage { get; set; }
        public DbSet<ServiceProviderTypeArbitrage> ServiceProviderTypeArbitrage { get; set; }
        public DbSet<ServiceProviderMasterArbitrage> ServiceProviderMasterArbitrage { get; set; }
        public DbSet<LimitsArbitrage> LimitsArbitrage { get; set; }

        //ntrivedi arbitrage 05-06-2019
        public DbSet<ArbitrageWalletTypeMaster> ArbitrageWalletTypeMaster { get; set; }
        public DbSet<ArbitrageWalletMaster> ArbitrageWalletMaster { get; set; }
        public DbSet<LPArbitrageWalletMaster> LPArbitrageWalletMaster { get; set; }
        public DbSet<ArbitrageWalletTransactionQueue> ArbitrageWalletTransactionQueue { get; set; }
        public DbSet<ArbitrageWalletTransactionOrder> ArbitrageWalletTransactionOrder { get; set; }
        public DbSet<ArbitrageTransactionAccount> ArbitrageTransactionAccount { get; set; }
        public DbSet<LPArbitrageTransactionAccount> LPArbitrageTransactionAccount { get; set; }
        public DbSet<ArbitrageWalletLedger> ArbitrageWalletLedger { get; set; }
        public DbSet<LPArbitrageWalletLedger> LPArbitrageWalletLedger { get; set; }
        public DbSet<ArbitrageWalletAuthorizeUserMaster> ArbitrageWalletAuthorizeUserMaster { get; set; }

        public DbQuery<ArbitrageChargeConfigurationMasterRes> ListArbitrageChargeConfigurationMasterRes { get; set; } //Chirag 12/06/2019
        public DbQuery<ChargeConfigurationMasterArbitrageRes> ListChargeConfigurationMasterArbitrageRes { get; set; } //Chirag 21/06/2019
        public DbQuery<ArbitrageChargeConfigurationDetailRes> ArbitrageChargeConfigurationDetailRes { get; set; } //Chirag 21/06/2019
        public DbQuery<LPArbitrageWalletMismatchRes> ListLPArbitrageWalletMismatchRes { get; set; } //Chirag 22/06/2019
        public DbQuery<LocalPairStatisticsQryRes> LocalPairStatisticsQryRes { get; set; }
        public DbSet<TrnChargeLogArbitrage> TrnChargeLogArbitrage { get; set; } //ntrivedi 18-06-2019
        public DbSet<ChargeConfigurationDetailArbitrage> ChargeConfigurationDetailArbitrage { get; set; }//ntrivedi 18-06-2019
        public DbSet<ChargeConfigurationMasterArbitrage> ChargeConfigurationMasterArbitrage { get; set; }//ntrivedi 18-06-2019
        public DbSet<LPArbitrageWalletMismatch> LPArbitrageWalletMismatch { get; set; }//ntrivedi 22-06-2019

        public DbQuery<GetTradeSettlePrice> GetTradeSettlePrice { get; set; }
        public DbQuery<GetTransactionSettledQty> GetTransactionSettledQty { get; set; }
        public DbQuery<CCXTTickerExchange> GetCCXTExchange { get; set; }
        public DbQuery<CCXTTickerQryObj> CCXTTickerQryObj { get; set; }
        public DbQuery<CheckArbitrageTransactionStatus> CheckArbitrageTransactionStatus { get; set; }

        public DbQuery<ReferralRewardsListViewModel> ReferralRewardsListViewModel { get; set; }//mansi 09-09-2019

        //use for handle BizuserRole data from MarketMaker role check query -Sahil 02-10-2019 7:30 PM
        public DbSet<IdentityUserRole<int>> BizUserRole { get; set; }
        public DbQuery<GetMarketMakerHoldOrderRateChange> GetMarketMakerHoldOrderRateChange { get; set; } //add for store HoldOrderRateChange data -Sahil 16-10-2019 04:45 PM
        public DbQuery<MarketMakerBuyPreferencesViewModel> MarketMakerBuyPreferences { get; set; }
        public DbQuery<MarketMakerSellPreferencesViewModel> MarketMakerSellPreferences { get; set; }
        public DbQuery<LPTPairFiat> LPTPairFiat{ get; set; } //ntrivedi 2-11-2019
        public DbQuery<MarketMakerSettleTrxnByTakeViewModel> MarketMakerSettleTrxnByTakeViewModel { get; set; }
        public DbQuery<GetMarketMakerUser> GetMarketMakerUser { get; set; }
        public DbQuery<MarketMakerBalancePerformanceViewModel> MarketMakerBalancePerformances { get; set; }
        public DbQuery<MarketMakerTradePairList> MarketMakerTradePairList { get; set; }
        public DbQuery<MarketMakerTradeCountListQryRes> MarketMakerTradeCountListQryRes { get; set; }



        public WorldexContext(DbContextOptions<WorldexContext> options, UserResolveService userService) : base(options)
        {
            _userService = userService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(IAuditable).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime>("CreatedAt");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime>("UpdatedAt");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<string>("CreatedBy");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<string>("UpdatedBy");
            }


            // modelBuilder.Entity<ApplicationUser>(b => { b.ToTable("asp_net_users"); });

            //base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUserPhotos>().ToTable("BizUserPhotos");
            modelBuilder.Entity<ApplicationRole>().ToTable("BizRoles");
            modelBuilder.Entity<ApplicationUser>().ToTable("BizUser");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("BizRolesClaims");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("BizUserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("BizUserLogin");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("BizUserRole");
            //Rushabh 09-02-2019 Added For User And Role Mapping
            //modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoleMapping");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("BizUserToken");

            //modelBuilder.Entity<IdentityUser>().ToTable("MyUsers").Property(p => p.Id).HasColumnName("UserId");
            ////modelBuilder.Entity<ApplicationUsers>().ToTable("MyUsers").Property(p => p.Id).HasColumnName("UserId");

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            //modelBuilder.Entity<TradeTransactionQueue>().HasKey(e => new { e.Id, e.TrnNo }); // komal 04-10-2018 composite primary key
            //modelBuilder.Entity<TradePoolMaster>().HasKey(e => new { e.Id, e.SellServiceID, e.BuyServiceID, e.BidPrice }); // komal 11-10-2018 composite primary key

            //modelBuilder.Entity<WithdrawHistory>().HasKey(e => new {  e.TrnID, e.Address }); // vsolanki 2018-10-29 composite primary key ntrivedi removing pk 06-11-2018

            modelBuilder.Entity<DepositCounterMaster>().HasKey(e => new { e.WalletTypeID, e.SerProId }); // Rita 22-10-2018 composite primary key
            modelBuilder.Entity<TradeGraphDetail>().HasKey(e => new { e.Id, e.TranNo }); // Uday 22-10-2018 composite primary key
            modelBuilder.Entity<TradeGraphDetailMargin>().HasKey(e => new { e.Id, e.TranNo }); // Rita 21-2-19 composite primary key
            modelBuilder.Entity<TradeGraphDetailArbitrage>().HasKey(e => new { e.Id, e.TranNo }); // Rita 4-6-19 composite primary key
            modelBuilder.Entity<DepositHistory>().HasKey(e => new { e.TrnID, e.Address }); // ntrivedi 23-10-2018 composite primary key
            modelBuilder.Entity<BizUserTypeMapping>().Property(p => p.UserID).ValueGeneratedNever(); // ntrivedi 
            modelBuilder.Entity<MemberShadowLimit>().Property(p => p.MemberTypeId).ValueGeneratedNever(); // ntrivedi 
            modelBuilder.Entity<StckingScheme>().Property(p => p.WalletType).ValueGeneratedNever(); // ntrivedi 
                                                                                                    //modelBuilder.Entity<WalletLimitConfigurationMaster>().Property(p => p.TrnType).ValueGeneratedNever();
            modelBuilder.Entity<ChargeFreeMarketCurrencyMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<ChargeFreeMarketCurrencyMaster>().Property(x => x.MarketCurrency).ValueGeneratedNever();

            modelBuilder.Entity<WalletMaster>().Property(x => x.AccWalletID).ValueGeneratedNever();
            modelBuilder.Entity<WalletMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore; // ntrivedi https://github.com/aspnet/EntityFrameworkCore/issues/7380 29-10-2018
            modelBuilder.Entity<TradeBuyRequest>().HasKey(e => new { e.Id, e.TrnNo });
            modelBuilder.Entity<ChargeRuleMaster>().HasKey(e => new { e.TrnType, e.WalletType });
            //modelBuilder.Entity<LimitRuleMaster>().HasKey(e => new { e.TrnType, e.WalletType });
            modelBuilder.Entity<TradeDepositCompletedTrn>().HasKey(e => new { e.Address, e.TrnID }); // ntrivedi 30-10-2018 composite primary key
            modelBuilder.Entity<TradeDepositCompletedTrn>().Property(x => x.Address).ValueGeneratedNever(); // ntrivedi 30-10-2018 
            modelBuilder.Entity<TradeDepositCompletedTrn>().Property(x => x.TrnID).ValueGeneratedNever(); // ntrivedi 30-10-2018 pk and autoid are different
            modelBuilder.Entity<TradeDepositCompletedTrn>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore; // ntrivedi 30102018 https://github.com/aspnet/EntityFrameworkCore/issues/7380 29-10-2018
            //modelBuilder.Entity<TradeBuyerList>().HasKey(e => new {e.Id, e.TrnNo });

            modelBuilder.Entity<CryptoWatcherArbitrage>().HasKey(e => new { e.LPType, e.PairId }); // Rita 13-6-19 composite primary key
            modelBuilder.Entity<CryptoWatcherArbitrage>().Property(x => x.LPType).ValueGeneratedNever(); // Rita 13-6-19
            modelBuilder.Entity<CryptoWatcherArbitrage>().Property(x => x.PairId).ValueGeneratedNever(); //Rita 13-6-19 pk and autoid are different
            modelBuilder.Entity<CryptoWatcherArbitrage>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;


            //modelBuilder.Entity<TradeSellerList>().HasKey(e => new { e.TrnNo, e.PoolID });
            modelBuilder.Entity<WalletLimitConfiguration>().HasKey(e => new { e.TrnType, e.WalletId }); // ntrivedi 31102018
                                                                                                        // modelBuilder.Entity<WalletLimitConfiguration>().Property(x => x.TrnType).ValueGeneratedNever();// ntrivedi 31102018
            modelBuilder.Entity<UserPreferencesMaster>().Property(x => x.UserID).ValueGeneratedNever();// ntrivedi 31102018
                                                                                                       //   modelBuilder.Entity<WalletLimitConfiguration>().Property(x => x.WalletId).ValueGeneratedNever();// ntrivedi 31102018
            modelBuilder.Entity<WalletLimitConfiguration>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore; // ntrivedi 30102018 https://github.com/aspnet/EntityFrameworkCore/issues/7380 29-10-2018
            modelBuilder.Entity<UserPreferencesMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<TradeBitGoDelayAddresses>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<TradeBitGoDelayAddresses>().Property(x => x.TrnID).ValueGeneratedNever();// ntrivedi 31102018
            modelBuilder.Entity<TrnTypeMaster>().Property(x => x.TrnTypeId).ValueGeneratedNever();
            modelBuilder.Entity<TradeTransactionQueue>().Property(x => x.TrnNo).ValueGeneratedNever();//rita 11-3-2018 ,error : The properties 'TradeTransactionQueue.TrnNo', 'TradeTransactionQueue.Id' are configured to use 'Identity' value generator
            modelBuilder.Entity<SettledTradeTransactionQueue>().Property(x => x.TrnNo).ValueGeneratedNever();
            modelBuilder.Entity<TradeTransactionQueueMargin>().Property(x => x.TrnNo).ValueGeneratedNever();
            modelBuilder.Entity<SettledTradeTransactionQueueMargin>().Property(x => x.TrnNo).ValueGeneratedNever();
            modelBuilder.Entity<TradeTransactionQueueArbitrage>().Property(x => x.TrnNo).ValueGeneratedNever();//rita 31-5-19 for arbitrage
            modelBuilder.Entity<SettledTradeTransactionQueueArbitrage>().Property(x => x.TrnNo).ValueGeneratedNever();//rita 31-5-19 for arbitrage
            //modelBuilder.Entity<TradeBuyerList>().Property(x => x.TrnNo).ValueGeneratedNever();//rita 11-3-2018
            //modelBuilder.Entity<TradeBuyerList>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;//rita 2-11-2018 for error cannot update identity column id
            modelBuilder.Entity<TradeBuyerListV1>().Property(x => x.TrnNo).ValueGeneratedNever();//rita 11-3-2018
            modelBuilder.Entity<TradeBuyerListV1>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;//rita 2-11-2018 for error cannot update identity column id
            modelBuilder.Entity<TradeTransactionQueue>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;//rita 2-11-2018 for error cannot update identity column id
            modelBuilder.Entity<SettledTradeTransactionQueue>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<TradeTransactionQueueMargin>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<SettledTradeTransactionQueueMargin>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<TradeTransactionQueueArbitrage>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<SettledTradeTransactionQueueArbitrage>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            //modelBuilder.Entity<LimitRuleMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;// for error cannot update identity column id
            modelBuilder.Entity<ChargeRuleMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;//2-11-2018 for error cannot update identity column id

            //modelBuilder.Entity<TradeSellerList>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;//rita 3-11-2018 for error cannot update identity column id
            modelBuilder.Entity<TradeSellerListV1>().Property(x => x.TrnNo).ValueGeneratedNever();//rita 11-3-2018
            modelBuilder.Entity<TradeSellerListV1>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;//rita 3-11-2018 for error cannot update identity column id

            modelBuilder.Entity<TradeSellerListMarginV1>().Property(x => x.TrnNo).ValueGeneratedNever();//rita 15-2-19 marging tading
            modelBuilder.Entity<TradeSellerListMarginV1>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;//rita 15-2-19 marging tading for error cannot update identity column id
            modelBuilder.Entity<TradeSellerListArbitrageV1>().Property(x => x.TrnNo).ValueGeneratedNever();//rita 31-5-19 marging tading
            modelBuilder.Entity<TradeSellerListArbitrageV1>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;//rita 31-5-19 marging tading for error cannot update identity column id

            modelBuilder.Entity<TradeBuyerListMarginV1>().Property(x => x.TrnNo).ValueGeneratedNever();//rita 15-2-19 marging tading
            modelBuilder.Entity<TradeBuyerListMarginV1>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;//rita 15-2-19 marging tading for error cannot update identity column id
            modelBuilder.Entity<TradeBuyerListArbitrageV1>().Property(x => x.TrnNo).ValueGeneratedNever();//rita 31-5-19 marging tading
            modelBuilder.Entity<TradeBuyerListArbitrageV1>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;//rita 31-5-19 Arbitrage tading for error cannot update identity column id


            modelBuilder.Entity<TransactionStatus>().HasKey(e => new { e.TrnNo, e.ServiceID, e.SerProID });//Rita 31-10-2018 for composite Primary key
            modelBuilder.Entity<DeviceStore>().HasKey(e => new { e.Id, e.UserID, });//Khushali 03-11-2018 for composite Primary key
            modelBuilder.Entity<DepositCounterMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<LPWalletMaster>().HasKey(e => new { e.WalletTypeID, e.SerProID });
            modelBuilder.Entity<FiatCoinConfiguration>().HasKey(e => new { e.FromCurrencyId, e.ToCurrencyId,e.TransactionType });
            modelBuilder.Entity<FiatCoinConfiguration>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<LPWalletMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<LPWalletMaster>().Property(x => x.WalletTypeID).ValueGeneratedNever();
            modelBuilder.Entity<LPWalletMaster>().Property(x => x.SerProID).ValueGeneratedNever();

            //modelBuilder.Entity<APIOrderSettlement>().Property(x => x.TrnNo).ValueGeneratedNever();//rita 1-2-19 // commented by khushali for log table
            //modelBuilder.Entity<APIOrderSettlement>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;//rita 1-2-19
            modelBuilder.Entity<ApplicationGroupRoles>().HasKey(e => new { e.PermissionGroupId, e.RoleId }); // Rushabh 20-02-2019

            modelBuilder.Entity<MarginDepositCounterMaster>().HasKey(e => new { e.WalletTypeID, e.SerProId });
            modelBuilder.Entity<MarginDepositCounterMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<NEODepositCounter>().HasKey(e => new { e.WalletTypeID, e.SerProId, e.AddressId });
            modelBuilder.Entity<NEODepositCounter>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            //New Wallet

            // Rushabh 04-04-2019 Added PK For Wallet Type Name

            modelBuilder.Entity<WalletTypeMaster>().Property(x => x.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<WalletTypeMaster>().Property(x => x.WalletTypeName).ValueGeneratedNever();

            //ntrivedi 18-04-2019 for added primary key 
            modelBuilder.Entity<MarginWalletTypeMaster>().Property(x => x.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<MarginWalletTypeMaster>().Property(x => x.WalletTypeName).ValueGeneratedNever();

            // Rushabh 30-01-2019 composite primary key

            modelBuilder.Entity<WalletTrnLimitConfiguration>().HasKey(e => new { e.TrnType, e.WalletType, e.IsKYCEnable });
            modelBuilder.Entity<WalletTrnLimitConfiguration>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            // Rushabh 22-12-2018 composite primary key

            modelBuilder.Entity<TransactionBlockedChannel>().HasKey(e => new { e.ChannelID, e.TrnType });
            modelBuilder.Entity<TransactionBlockedChannel>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            // Rushabh 24-12-2018 composite primary key
            modelBuilder.Entity<StakingPolicyMaster>().HasKey(e => new { e.WalletTypeID, e.StakingType });
            modelBuilder.Entity<StakingPolicyMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<StakingPolicyMaster>().Property(x => x.WalletTypeID).ValueGeneratedNever();

            // Rushabh 24-12-2018 composite primary key
            modelBuilder.Entity<StakingChargeMaster>().HasKey(e => new { e.WalletTypeID, e.UserID });
            modelBuilder.Entity<StakingChargeMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            // Rushabh 15-01-2019 composite primary key
            modelBuilder.Entity<CurrencyRateMaster>().HasKey(C => new { C.WalletTypeId });
            modelBuilder.Entity<CurrencyRateMaster>().Property(C => C.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<CurrencyRateMaster>().Property(C => C.WalletTypeId).ValueGeneratedNever();

            modelBuilder.Entity<ChargePolicy>().HasKey(e => new { e.WalletTrnType, e.WalletType });// for composite Primary key
            modelBuilder.Entity<CommissionPolicy>().HasKey(e => new { e.WalletTrnType, e.WalletType });// for composite Primary key

            modelBuilder.Entity<UserWalletBlockTrnTypeMaster>().HasKey(e => new { e.WalletID, e.WTrnTypeMasterID });// for composite Primary key

            modelBuilder.Entity<UserWalletBlockTrnTypeMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<TransactionPolicy>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            //  modelBuilder.Entity<TransactionPolicy>().Property(x => x.TrnType).ValueGeneratedNever();
            modelBuilder.Entity<TransactionPolicy>().HasKey(e => new { e.TrnType, e.RoleId, e.IsKYCEnable });

            modelBuilder.Entity<WalletUsagePolicy>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<WalletUsagePolicy>().Property(x => x.WalletType).ValueGeneratedNever();

            modelBuilder.Entity<CurrencyTypeMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<CurrencyTypeMaster>().Property(x => x.CurrencyTypeId).ValueGeneratedNever();

            modelBuilder.Entity<WTrnTypeMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<WTrnTypeMaster>().Property(x => x.TrnTypeId).ValueGeneratedNever();

            modelBuilder.Entity<AllowedChannels>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<AllowedChannels>().Property(x => x.ChannelID).ValueGeneratedNever();

            modelBuilder.Entity<UserTypeMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<UserTypeMaster>().Property(x => x.UserTypeId).ValueGeneratedNever();

            modelBuilder.Entity<UserMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<UserMaster>().Property(x => x.BizUserID).ValueGeneratedNever();

            //modelBuilder.Entity<CommissionTypeMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            //  modelBuilder.Entity<CommissionTypeMaster>().Property(x => x.Id).ValueGeneratedNever();

            //modelBuilder.Entity<ChargeTypeMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<ChargeTypeMaster>().Property(x => x.Id).ValueGeneratedNever();
            modelBuilder.Entity<WalletUsageType>().Property(x => x.Id).ValueGeneratedNever();

            modelBuilder.Entity<Statastics>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<Statastics>().HasKey(e => new { e.WalletId, e.WalletType, e.TrnType, e.Hour, e.Day, e.Week, e.Month, e.Year, e.UserId });


            modelBuilder.Entity<ChargePolicy>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<CommissionPolicy>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            //modelBuilder.Entity<UserWalletMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            //modelBuilder.Entity<UserWalletMaster>().Property(x => x.AccWalletID).ValueGeneratedNever();
            modelBuilder.Entity<BlockWalletTrnTypeMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<BlockWalletTrnTypeMaster>().HasKey(e => new { e.WalletTypeID, e.TrnTypeID });

            modelBuilder.Entity<OrganizationUserMaster>().HasKey(e => new { e.RoleID, e.UserID }); //ntrivedi 14-12-2018
            modelBuilder.Entity<WalletAuthorizeUserMaster>().HasKey(e => new { e.UserID, e.WalletID });  //ntrivedi 14-12-2018
            modelBuilder.Entity<WalletAuthorizeUserMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<AllowTrnTypeRoleWise>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<AllowTrnTypeRoleWise>().HasKey(e => new { e.TrnTypeId, e.RoleId });

            modelBuilder.Entity<StopLossMaster>().Property(x => x.WalletTypeID).ValueGeneratedNever(); modelBuilder.Entity<LeverageMaster>().Property(x => x.WalletTypeID).ValueGeneratedNever();

            modelBuilder.Entity<StopLossMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<LeverageMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<ChargeConfigurationMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<ChargeConfigurationMaster>().HasKey(e => new { e.WalletTypeID, e.TrnType, e.KYCComplaint, e.SpecialChargeConfigurationID });

            modelBuilder.Entity<MarginChargeConfigurationMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<MarginChargeConfigurationMaster>().HasKey(e => new { e.WalletTypeID, e.TrnType, e.KYCComplaint, e.SpecialChargeConfigurationID });

            modelBuilder.Entity<ChargeConfigurationMasterArbitrage>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<ChargeConfigurationMasterArbitrage>().HasKey(e => new { e.WalletTypeID, e.TrnType, e.KYCComplaint, e.SpecialChargeConfigurationID });

            modelBuilder.Entity<ApplicationGroupRoles>().HasKey(e => new { e.PermissionGroupId, e.RoleId }); // Rushabh 20-02-2019
            //modelBuilder.Entity<TransactionStatusCheckRequest>().Property(x => x.Id).ValueGeneratedNever();// khushali 24-01-2019 for LP status check 
            //modelBuilder.Entity<TransactionStatusCheckRequest>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore; // khushali 24-01-2019 for LP status check 

            modelBuilder.Entity<MarginBlockWalletTrnTypeMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<MarginBlockWalletTrnTypeMaster>().HasKey(e => new { e.WalletTypeID, e.TrnTypeID });
            modelBuilder.Entity<MarginWalletAuthorizeUserMaster>().HasKey(e => new { e.UserID, e.WalletID });
            modelBuilder.Entity<MarginWalletAuthorizeUserMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<MarginWalletMaster>().Property(x => x.AccWalletID).ValueGeneratedNever();
            modelBuilder.Entity<MarginWalletMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<MarginWalletUsagePolicy>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<MarginWalletUsagePolicy>().Property(x => x.WalletType).ValueGeneratedNever();
            modelBuilder.Entity<MarginWTrnTypeMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<MarginWTrnTypeMaster>().Property(x => x.TrnTypeId).ValueGeneratedNever();

            modelBuilder.Entity<MarginUserWalletBlockTrnTypeMaster>().HasKey(e => new { e.WalletID, e.WTrnTypeMasterID });
            modelBuilder.Entity<MarginUserWalletBlockTrnTypeMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<MarginTransactionBlockedChannel>().HasKey(e => new { e.ChannelID, e.TrnType });
            modelBuilder.Entity<MarginTransactionBlockedChannel>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<OpenPositionMaster>().HasKey(e => new { e.PairID, e.UserID, e.BatchNo }); //ntrivedi 28-03-2018
            modelBuilder.Entity<OpenPositionMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;//ntrivedi 28-03-2018


            modelBuilder.Entity<LanguagePreferenceMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<LanguagePreferenceMaster>().Property(x => x.PreferedLanguage).ValueGeneratedNever();

            modelBuilder.Entity<AffiliateSchemeTypeMapping>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            modelBuilder.Entity<AffiliateSchemeTypeMapping>().HasKey(e => new { e.SchemeMstId, e.SchemeTypeMstId });
            modelBuilder.Entity<ModuleTypeMaster>().Property(e => e.Id).ValueGeneratedNever();
            modelBuilder.Entity<ModuleUtilityMaster>().Property(e => e.Id).ValueGeneratedNever();
            modelBuilder.Entity<ModuleCRUDOptMaster>().Property(e => e.Id).ValueGeneratedNever();
            modelBuilder.Entity<ModuleVisibilityMaster>().Property(e => e.Id).ValueGeneratedNever();
            modelBuilder.Entity<ModuleFieldRequirerMaster>().Property(e => e.Id).ValueGeneratedNever();
            modelBuilder.Entity<ModuleAccessRightsMaster>().Property(e => e.Id).ValueGeneratedNever();

            modelBuilder.Entity<MarginDepositHistory>().HasKey(e => new { e.TrnID, e.Address }); // ntrivedi 23-10-2018 composite primary key

            //ntrivedi 05-06-2019 arbitrage entity
            modelBuilder.Entity<ArbitrageWalletTypeMaster>().Property(x => x.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<ArbitrageWalletTypeMaster>().Property(x => x.WalletTypeName).ValueGeneratedNever();
            modelBuilder.Entity<LPArbitrageWalletMaster>().HasKey(e => new { e.WalletTypeID, e.SerProID });
            modelBuilder.Entity<LPArbitrageWalletMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<LPArbitrageWalletMaster>().Property(x => x.WalletTypeID).ValueGeneratedNever();
            modelBuilder.Entity<LPArbitrageWalletMaster>().Property(x => x.SerProID).ValueGeneratedNever();
            modelBuilder.Entity<ArbitrageWalletMaster>().Property(x => x.AccWalletID).ValueGeneratedNever();
            modelBuilder.Entity<ArbitrageWalletMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<ArbitrageWalletAuthorizeUserMaster>().HasKey(e => new { e.UserID, e.WalletID });
            modelBuilder.Entity<ArbitrageWalletAuthorizeUserMaster>().Property(e => e.Id).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            modelBuilder.Entity<UserBankRequest>().HasIndex(i => i.GUID);
            modelBuilder.Entity<UserBankMaster>().HasIndex(i => i.GUID);
            modelBuilder.Entity<WithdrawHistory>().HasIndex(i => i.GUID);
            modelBuilder.Entity<DepositHistory>().HasIndex(i => i.GUID);
            modelBuilder.Entity<TokenStakingHistory>().HasIndex(i => i.GUID);
            modelBuilder.Entity<TokenUnStakingHistory>().HasIndex(i => i.GUID);

            modelBuilder.Entity<BuySellTopUpRequest>().HasIndex(i => i.Guid);

            modelBuilder.Entity<ArbitrageDepositFund>().HasIndex(i => i.GUID);
            modelBuilder.Entity<MarginWalletTransactionQueue>().HasIndex(i => i.RefGuid);
            modelBuilder.Entity<WalletTransactionQueue>().HasIndex(i => i.RefGuid);
            modelBuilder.Entity<ArbitrageWalletTransactionQueue>().HasIndex(i => i.RefGuid);
            modelBuilder.Entity<MarginLoanRequest>().HasIndex(i => i.Guid);
            modelBuilder.Entity<UserLoanMaster>().HasIndex(i => i.Guid);
            modelBuilder.Entity<WithdrawLoanMaster>().HasIndex(i => i.Guid);
            modelBuilder.Entity<MarginDepositHistory>().HasIndex(i => i.GUID);
            modelBuilder.Entity<LPArbitrageWalletMismatch>().HasIndex(i => i.Guid);
            //2019-7-17 vsolanki added guid
            modelBuilder.Entity<ArbitrageWalletTransfer>().HasIndex(i => i.Guid);
            modelBuilder.Entity<AffiliateCommissionHistory>().HasIndex(i => new { i.AffiliateUserId, i.TrnUserId, i.SchemeMappingId });
            modelBuilder.Entity<AffiliateSchemeTypeMapping>().HasIndex(i => i.SchemeMstId);
            modelBuilder.Entity<AffiliateSchemeTypeMapping>().HasIndex(i => i.SchemeTypeMstId);
            //IEO Tables
            modelBuilder.Entity<IEOSlabMaster>().HasIndex(i => i.Guid);
            modelBuilder.Entity<IEOCronMaster>().HasIndex(i => i.Guid);
            modelBuilder.Entity<IEOPurchaseHistory>().HasIndex(i => i.Guid);
            modelBuilder.Entity<IEOPurchaseHistory>().HasIndex(i => i.UserID);
            modelBuilder.Entity<IEOPurchaseHistory>().HasIndex(i => i.DeliveredCurrency);
            modelBuilder.Entity<IEOPurchaseHistory>().HasIndex(i => i.PaidCurrency);
            modelBuilder.Entity<IEORoundMaster>().HasIndex(i => i.Guid);
            modelBuilder.Entity<IEOCurrencyPairMapping>().HasIndex(i => i.Guid);
            modelBuilder.Entity<IEOCurrencyMaster>().HasIndex(i => i.Guid);
            modelBuilder.Entity<IEOBannerMaster>().HasIndex(i => i.GUID);
            modelBuilder.Entity<IEOWalletAdminDeposit>().HasIndex(i => i.GUID);

            // To create cluster key with index on multiple column combination. -Nishit Jani on A-H 2019-07-06 3:33 AM
            //modelBuilder.Entity<DeviceMaster>(b =>
            //{
            //    b.Property(e => e.Device).HasConversion(p => p, s => s.ToUpper()).IsRequired(true);
            //    b.Property(e => e.DeviceId).HasConversion(p => p, s => s.ToUpper()).IsRequired(true);
            //    b.Property(e => e.DeviceOS).HasConversion(p => p, s => s.ToUpper()).IsRequired(true);
            //    b.Property(e => e.UserId).IsRequired(true);
            //    b.HasIndex(d => new { d.Device, d.DeviceId, d.DeviceOS, d.UserId }).HasName("UserUniqueDeviceIndex").IsUnique();
            //    b.HasAlternateKey(d => new { d.Device, d.DeviceId, d.DeviceOS, d.UserId }).HasName("IX_UserUniqueDevice");
            //});

            //{
            //    b.Property(e => e.Device).IsRequired(true);
            //    b.Property(e => e.DeviceId).IsRequired(true);
            //    b.Property(e => e.DeviceOS).IsRequired(true);
            //    b.Property(e => e.UserId).IsRequired(true);
            //    b.HasIndex(d => new { d.Device, d.DeviceId, d.DeviceOS, d.UserId }).HasName("UserUniqueDeviceIndex").IsUnique();
            //    b.HasAlternateKey(d => new { d.Device, d.DeviceId, d.DeviceOS, d.UserId }).HasName("IX_UserUniqueDevice");
            //});
        }



        /// <summary>
        /// Override SaveChanges so we can call the new AuditEntities method.
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            this.AuditEntities();
            return base.SaveChanges();
        }
        /// <summary>
        /// Override SaveChangesAsync so we can call the new AuditEntities method.
        /// </summary>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            this.AuditEntities();
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Method that will set the Audit properties for every added or modified Entity marked with the 
        /// IAuditable interface.
        /// </summary>
        private void AuditEntities()
        {

            DateTime now = DateTime.Now;
            // Get the authenticated user name 
            string userName = _userService.GetUser();

            // For every changed entity marked as IAditable set the values for the audit properties
            foreach (EntityEntry<IAuditable> entry in ChangeTracker.Entries<IAuditable>())
            {
                // If the entity was added.
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedBy").CurrentValue = userName;
                    entry.Property("CreatedAt").CurrentValue = now;
                }
                else if (entry.State == EntityState.Modified) // If the entity was updated
                {
                    entry.Property("UpdatedBy").CurrentValue = userName;
                    entry.Property("UpdatedAt").CurrentValue = now;
                }
            }
        }
        //vsolanki 2018-11-29
        public void InsertAuditLog()
        {

            try
            {
                var modifiedEntities = ChangeTracker.Entries()
           .Where(p => p.State == EntityState.Modified).ToList();

                foreach (var entity in modifiedEntities)
                {
                    var ename = entity.Entity.GetType().Name;
                    string originalValue = "", currentValue = "";
                    foreach (var prop in entity.OriginalValues.Properties)
                    {
                        if (entity.OriginalValues[prop] == null && entity.CurrentValues[prop] == null)
                        {
                            originalValue = "";
                            currentValue = "";

                        }

                        else if (entity.OriginalValues[prop] == null && entity.CurrentValues[prop] != null)
                        {
                            originalValue = "";
                            currentValue = entity.CurrentValues[prop].ToString();
                        }
                        else if (entity.CurrentValues[prop] != null)
                        {
                            originalValue = entity.OriginalValues[prop].ToString();
                            currentValue = entity.CurrentValues[prop].ToString();

                        }
                        else
                        {
                            originalValue = entity.OriginalValues[prop].ToString();
                            currentValue = "";
                        }


                        if (originalValue != currentValue && prop.Name.ToString() != "UpdatedDate" && prop.Name.ToString() != "UpdatedBy") //Only create a log if the value changes
                        {
                            //Create the Change Log
                            AuditActivityLog log = new AuditActivityLog();
                            log.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                            log.CreatedBy = 1;
                            log.UpdatedBy = 1;
                            log.UpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                            log.Status = 1;
                            log.EntityType = ename;
                            log.ColumnName = prop.Name.ToString();
                            log.OldValue = originalValue.ToString();
                            log.NewValue = currentValue.ToString();
                            log.Remarks = "Updated";
                            base.Add(log);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertAuditLog ", "WorldexContext", ex);
            }
        }

        #region  Comment
        /*

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseSqlServer("Data Source=LAPTOP-5JVOHDJQ\\SQLEXPRESS;Initial Catalog=Worldex;Persist Security Info=True;User ID=sa;Password=admin_1");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUserPhotos>(entity =>
        {
            entity.HasIndex(e => e.ApplicationUserId)
                .IsUnique();

            entity.HasOne(d => d.ApplicationUser)
                .WithOne(p => p.ApplicationUserPhotos)
                .HasForeignKey<ApplicationUserPhotos>(d => d.ApplicationUserId)
                .HasConstraintName("FK_ApplicationUserPhotos_AspNetUsers_ApplicationUserId");
        });

        modelBuilder.Entity<OpenIddictApplications>(entity =>
        {
            entity.HasIndex(e => e.ClientId)
                .IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.ClientId).IsRequired();

            entity.Property(e => e.Type).IsRequired();
        });

        modelBuilder.Entity<OpenIddictAuthorizations>(entity =>
        {
            entity.HasIndex(e => e.ApplicationId);

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Status).IsRequired();

            entity.Property(e => e.Subject).IsRequired();

            entity.Property(e => e.Type).IsRequired();

            entity.HasOne(d => d.Application)
                .WithMany(p => p.OpenIddictAuthorizations)
                .HasForeignKey(d => d.ApplicationId);
        });

        modelBuilder.Entity<OpenIddictScopes>(entity =>
        {
            entity.HasIndex(e => e.Name)
                .IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name).IsRequired();
        });

        modelBuilder.Entity<OpenIddictTokens>(entity =>
        {
            entity.HasIndex(e => e.ApplicationId);

            entity.HasIndex(e => e.AuthorizationId);

            entity.HasIndex(e => e.ReferenceId)
                .IsUnique()
                .HasFilter("([ReferenceId] IS NOT NULL)");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Subject).IsRequired();

            entity.Property(e => e.Type).IsRequired();

            entity.HasOne(d => d.Application)
                .WithMany(p => p.OpenIddictTokens)
                .HasForeignKey(d => d.ApplicationId);

            entity.HasOne(d => d.Authorization)
                .WithMany(p => p.OpenIddictTokens)
                .HasForeignKey(d => d.AuthorizationId);
        });

        modelBuilder.Entity<Resources>(entity =>
        {
            entity.HasIndex(e => e.CultureId);

            entity.HasOne(d => d.Culture)
                .WithMany(p => p.Resources)
                .HasForeignKey(d => d.CultureId);
        });

        modelBuilder.Entity<RoleClaims>(entity =>
        {
            entity.HasIndex(e => e.RoleId)
                .HasName("IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.RoleClaims)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_AspNetRoleClaims_AspNetRoles_RoleId");
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName)
                .HasName("RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Description).HasMaxLength(250);

            entity.Property(e => e.Name).HasMaxLength(256);

            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<UserClaims>(entity =>
        {
            entity.HasIndex(e => e.UserId)
                .HasName("IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserClaims)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AspNetUserClaims_AspNetUsers_UserId");
        });

        modelBuilder.Entity<UserLogins>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId)
                .HasName("IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AspNetUserLogins_AspNetUsers_UserId");
        });

        modelBuilder.Entity<UserRoles>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasIndex(e => e.RoleId)
                .HasName("IX_AspNetUserRoles_RoleId");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_AspNetUserRoles_AspNetRoles_RoleId");

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AspNetUserRoles_AspNetUsers_UserId");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail)
                .HasName("EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName)
                .HasName("UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);

            entity.Property(e => e.FirstName).HasMaxLength(250);

            entity.Property(e => e.LastName).HasMaxLength(250);

            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<UserTokens>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AspNetUserTokens_AspNetUsers_UserId");
        });
    }
    */

        #endregion
    }


}
