using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Charges;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.NewWallet;
using Worldex.Core.Entities.User;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.ControlPanel;
using Worldex.Core.Services;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletConfiguration;
using Worldex.Core.ViewModels.WalletOperations;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Data;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using MediatR;
using System.IdentityModel.Tokens.Jwt;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Core.ViewModels.Configuration;
using System.Text.RegularExpressions;
using Worldex.Core.Entities.Transaction;

namespace Worldex.Infrastructure.Services.ControlPanel
{
    public class ControlPanelServices : IControlPanelServices
    {
        #region Constructor      
        private readonly ICommonRepository<TokenUnStakingHistory> _TokenUnstakingHistoryCommonRepo;
        private readonly ICommonRepository<TransactionRequest> _TrnRequestCommonRepo;
        private readonly EFCommonRepository<TransactionQueue> _TransactionRepository;
        private readonly ICommonRepository<TokenStakingHistory> _TokenstakingHistoryCommonRepo;
        private readonly ICommonRepository<TokenSupplyHistory> _TokenSupplyHistoryCommonRepo;
        private readonly ICommonRepository<BlockUnblockUserAddress> _BlockUnblockUserAddressCommonRepo;
        private readonly ICommonRepository<DestroyFundRequest> _DestroyFundRequestCommonRepo;
        private readonly ICommonRepository<TokenTransferHistory> _TokenTransferHistoryCommonRepo;
        private readonly ICommonRepository<TransferFeeHistory> _TransferFeeHistoryCommonRepo;
        private readonly ICommonRepository<RouteConfiguration> _routeRepository;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IWalletService _IWalletService;
        private readonly ICommonRepository<WalletTrnLimitConfiguration> _walletTrnLimitConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommonRepository<ServiceProviderMaster> _ServiceProviderMaster;
        private readonly ICommonRepository<ServiceProviderDetail> _ServiceProviderDetail;
        private readonly ICommonRepository<DepositCounterMaster> _DepositCounterMaster;
        private readonly ICommonRepository<AddressMaster> _AddressMasterCommonRepo;
        private readonly ICommonRepository<ChargeConfigurationMaster> _ChargeConfigurationMaster;
        private readonly ICommonRepository<ChargeConfigurationDetail> _ChargeConfigurationDetail;
        private readonly ICommonRepository<AllowedChannels> _AllowedChannelRepo;
        private readonly ICommonRepository<StakingPolicyMaster> _StakingPolicyMasterCommonRepo;
        private readonly ICommonRepository<StakingPolicyDetail> _StakingPolicyDetailCommonRepo;
        private readonly ICommonRepository<CurrencyTypeMaster> _CurrencyTypeMasterRepo;
        private readonly ICommonRepository<AllowTrnTypeRoleWise> _AllowTrnTypeRoleWise;
        private readonly ICommonRepository<StopLossMaster> _StopLossMaster;
        private readonly ICommonRepository<LeverageMaster> _LeverageMaster;
        private readonly ICommonRepository<UserRoleMaster> _UserRoleMaster;
        private readonly ICommonRepository<WalletPolicyAllowedDay> _WalletPolicyAllowedDayRepo;
        private readonly IWalletConfiguration _walletConfiguration;
        private readonly ICommonRepository<TransactionBlockedChannel> _TranBlockedChannelRepo;
        private readonly ICommonRepository<OrganizationMaster> _OrgMasterCommonRepo;
        private readonly ICommonRepository<OrganizationUserMaster> _OrgUserCommonRepo;
        private readonly ICommonRepository<WalletUsagePolicy> _WalletUsagePolicy;
        private readonly ICommonRepository<UserMaster> _UserMasterCommonRepo;
        private readonly ICommonRepository<ChargePolicy> _ChargePolicyCommonRepo;
        private readonly ICommonRepository<UserWalletBlockTrnTypeMaster> _UserWalletBlockTrnTypeMaster;
        private readonly ICommonRepository<TransactionPolicy> _TransactionPolicyCommonRepo;
        private readonly ICommonRepository<WTrnTypeMaster> _WTrnTypeMasterRepo;
        private readonly ICommonRepository<WalletTypeMaster> _WalletTypeMaster;
        private readonly IControlPanelRepository _controlPanelRepository;
        private readonly ICommonRepository<BlockWalletTrnTypeMaster> _BlockWalletTrnTypeMasterRepository;
        private readonly ICommonRepository<CommissionPolicy> _commissionPolicyRepository;
        private readonly ICommonRepository<CommissionTypeMaster> _CommissionTypeMaster;
        private readonly ICommonRepository<WalletMaster> _WalletMaster;
        private readonly ICommonRepository<LPWalletMaster> _LPWalletMaster;
        private readonly ICommonRepository<AutorizedApps> _AuthAppCommonRepo;
        private readonly IWebApiRepository _webApiRepository;
        private readonly ICommonRepository<ThirdPartyAPIConfiguration> _thirdPartyCommonRepository;
        private readonly ICommonRepository<TradingChargeTypeMaster> _TradingChargeTypeMaster;
        private readonly ICommonRepository<ChargeFreeMarketCurrencyMaster> _ChargeFreeMarketCurrencyMaster;
        private readonly ICommonRepository<WithdrawAdminRequest> _WithdrawAdminReqCommonRepo;
        List<TransactionProviderResponse> transactionProviderResponses;
        List<TransactionProviderResponse3> transactionProviderResponses3;
        List<TransactionProviderResponse2> transactionProviderResponses2;
        ThirdPartyAPIRequest thirdPartyAPIRequest;
        ThirdPartyAPIConfiguration thirdPartyAPIConfiguration;
        private readonly IWebApiSendRequest _webApiSendRequest;
        private readonly IWithdrawTransactionV1 _IWithdrawTransactionV1;
        private readonly IGetWebRequest _getWebRequest;
        private readonly WebApiParseResponse _WebApiParseResponse;
        private readonly IWalletSPRepositories _walletSPRepositories;
        private readonly ICommonRepository<DepositionInterval> _DepositionInterval;
        private IPushNotificationsQueue<SendEmailRequest> _pushNotificationsQueue;
        private readonly IMessageService _messageService;
        private readonly EncyptedDecrypted _encdecAEC;
        private readonly WorldexContext _dbContext;
        private readonly ICommonRepository<StakingChargeMaster> _StakingChargeCommonRepo;
        private readonly IMediator _mediator;
        private readonly ITransactionConfigService _transactionConfig;
        private readonly IWalletRepository _walletRepository;
        private readonly ICommonRepository<LPWalletMismatch> _mismatchReconRepository;
        public ControlPanelServices(ICommonRepository<AutorizedApps> AuthAppCommonRepo, ICommonRepository<StakingPolicyMaster> StakingPolicyMaster, ICommonRepository<DepositCounterMaster> DepositCounterMaster,
            ICommonRepository<TransactionBlockedChannel> TranBlockedChannelRepo, ICommonRepository<OrganizationMaster> OrgMasterCommonRepo, ICommonRepository<StopLossMaster> StopLossMaster,
            ICommonRepository<LeverageMaster> LeverageMaster, ICommonRepository<ThirdPartyAPIConfiguration> thirdPartyCommonRepository, ICommonRepository<TradingChargeTypeMaster> TradingChargeTypeMaster,
            IWebApiSendRequest webApiSendRequest, IGetWebRequest getWebRequest, WebApiParseResponse WebApiParseResponse, ICommonRepository<DestroyFundRequest> DestroyFundRequestCommonRepo, ICommonRepository<ChargeFreeMarketCurrencyMaster> ChargeFreeMarketCurrencyMaster,
            ICommonRepository<CurrencyTypeMaster> CurrencyTypeMasterRepo, ICommonRepository<TokenSupplyHistory> TokenSupplyHistoryCommonRepo,
            ICommonRepository<CommissionPolicy> commissionPolicyRepository, ICommonRepository<UserMaster> UserMasterCommonRepo, ICommonRepository<ChargeConfigurationMaster> ChargeConfigurationMaster,
            IControlPanelRepository controlPanelRepository, ICommonRepository<CommissionTypeMaster> CommissionTypeMaster, ICommonRepository<WalletMaster> WalletMaster, IWithdrawTransactionV1 IWithdrawTransactionV1,
            ICommonRepository<WalletUsagePolicy> WalletUsagePolicy, ICommonRepository<OrganizationUserMaster> OrgUserCommonRepo, ICommonRepository<TokenTransferHistory> TokenTransferHistoryCommonRepo,
            Microsoft.Extensions.Configuration.IConfiguration configuration, ICommonRepository<TransferFeeHistory> TransferFeeHistoryCommonRepo,
            ICommonRepository<WTrnTypeMaster> WTrnTypeMasterRepo, ICommonRepository<TransactionPolicy> TransactionPolicyCommonRepo, ICommonRepository<UserRoleMaster> UserRoleMaster,
            ICommonRepository<UserWalletBlockTrnTypeMaster> UserWalletBlockTrnTypeMaster, ICommonRepository<TokenUnStakingHistory> TokenUnstakingHistoryCommonRepo, ITransactionConfigService transactionConfig, ICommonRepository<LPWalletMismatch> mismatchReconRepository,
             IWalletConfiguration walletConfiguration, ICommonRepository<StakingChargeMaster> StakingChargeCommonRepo,
            ICommonRepository<BlockWalletTrnTypeMaster> BlockWalletTrnTypeMasterRepository, IWalletSPRepositories walletSPRepositories, ICommonRepository<WithdrawAdminRequest> WithdrawAdminReqCommonRepo,
            ICommonRepository<StakingPolicyMaster> StakingPolicyMasterCommonRepo, UserManager<ApplicationUser> userManager, ICommonRepository<BlockUnblockUserAddress> BlockUnblockUserAddressCommonRepo,
            ICommonRepository<StakingPolicyDetail> StakingPolicyDetailCommonRepo, ICommonRepository<ServiceProviderMaster> serviceProviderMaster, EFCommonRepository<TransactionQueue> TransactionRepository,
            EncyptedDecrypted encdecAEC, IPushNotificationsQueue<SendEmailRequest> pushNotificationsQueue, IMessageService MessageService, ICommonRepository<TokenStakingHistory> TokenstakingHistoryCommonRepo,
            IWalletService walletService, ICommonRepository<AddressMaster> AddressMasterCommonRepo, ICommonRepository<ChargeConfigurationDetail> ChargeConfigurationDetail, ICommonRepository<TransactionRequest> TrnRequestCommonRepo,
            ICommonRepository<WalletPolicyAllowedDay> WalletPolicyAllowedDay, ICommonRepository<AllowedChannels> AllowedChannelRepo, IWebApiRepository webApiRepository, ICommonRepository<LPWalletMaster> LPWalletMaster,
            ICommonRepository<WalletTypeMaster> WalletTypeMaster, ICommonRepository<ChargePolicy> ChargePolicyCommonRepo, ICommonRepository<AllowTrnTypeRoleWise> AllowTrnTypeRoleWise,
            ICommonRepository<ServiceProviderDetail> ServiceProviderDetails, ICommonRepository<RouteConfiguration> routeRepository, IWalletRepository walletRepository,
            ICommonRepository<WalletTrnLimitConfiguration> walletTrnLimitConfiguration, ICommonRepository<DepositionInterval> DepositionInterval, IMediator mediator, WorldexContext dbContext)
        {
            _mismatchReconRepository = mismatchReconRepository;
            _ChargeConfigurationMaster = ChargeConfigurationMaster;
            _TrnRequestCommonRepo = TrnRequestCommonRepo;
            _LPWalletMaster = LPWalletMaster;
            _transactionConfig = transactionConfig;
            _IWithdrawTransactionV1 = IWithdrawTransactionV1;
            _TransactionRepository = TransactionRepository;
            _TokenstakingHistoryCommonRepo = TokenstakingHistoryCommonRepo;
            _DestroyFundRequestCommonRepo = DestroyFundRequestCommonRepo;
            _BlockUnblockUserAddressCommonRepo = BlockUnblockUserAddressCommonRepo;
            _TokenTransferHistoryCommonRepo = TokenTransferHistoryCommonRepo;
            _TokenSupplyHistoryCommonRepo = TokenSupplyHistoryCommonRepo;
            _TransferFeeHistoryCommonRepo = TransferFeeHistoryCommonRepo;
            _ServiceProviderDetail = ServiceProviderDetails;
            _walletTrnLimitConfiguration = walletTrnLimitConfiguration;
            _StakingChargeCommonRepo = StakingChargeCommonRepo;
            _userManager = userManager;
            _TokenUnstakingHistoryCommonRepo = TokenUnstakingHistoryCommonRepo;
            _ChargeConfigurationDetail = ChargeConfigurationDetail;
            _ServiceProviderMaster = serviceProviderMaster;
            _walletSPRepositories = walletSPRepositories;
            _ChargeFreeMarketCurrencyMaster = ChargeFreeMarketCurrencyMaster;
            _DepositCounterMaster = DepositCounterMaster;
            _routeRepository = routeRepository;
            _webApiSendRequest = webApiSendRequest;
            _WebApiParseResponse = WebApiParseResponse;
            _getWebRequest = getWebRequest;
            _AddressMasterCommonRepo = AddressMasterCommonRepo;
            _thirdPartyCommonRepository = thirdPartyCommonRepository;
            _LeverageMaster = LeverageMaster;
            _AllowTrnTypeRoleWise = AllowTrnTypeRoleWise;
            _WalletMaster = WalletMaster;
            _AllowedChannelRepo = AllowedChannelRepo;
            _WalletUsagePolicy = WalletUsagePolicy;
            _StakingPolicyMasterCommonRepo = StakingPolicyMasterCommonRepo;
            _StakingPolicyDetailCommonRepo = StakingPolicyDetailCommonRepo;
            _TranBlockedChannelRepo = TranBlockedChannelRepo;
            _WalletPolicyAllowedDayRepo = WalletPolicyAllowedDay;
            _UserWalletBlockTrnTypeMaster = UserWalletBlockTrnTypeMaster;
            _CurrencyTypeMasterRepo = CurrencyTypeMasterRepo;
            _configuration = configuration;
            _OrgMasterCommonRepo = OrgMasterCommonRepo;
            _commissionPolicyRepository = commissionPolicyRepository;
            _StopLossMaster = StopLossMaster;
            _OrgUserCommonRepo = OrgUserCommonRepo;
            _controlPanelRepository = controlPanelRepository;
            _UserMasterCommonRepo = UserMasterCommonRepo;
            _WTrnTypeMasterRepo = WTrnTypeMasterRepo;
            _BlockWalletTrnTypeMasterRepository = BlockWalletTrnTypeMasterRepository;
            _ChargePolicyCommonRepo = ChargePolicyCommonRepo;
            _WalletTypeMaster = WalletTypeMaster;
            _TransactionPolicyCommonRepo = TransactionPolicyCommonRepo;
            _CommissionTypeMaster = CommissionTypeMaster;
            _AuthAppCommonRepo = AuthAppCommonRepo;
            _walletConfiguration = walletConfiguration;
            _UserRoleMaster = UserRoleMaster;
            _IWalletService = walletService;
            _webApiRepository = webApiRepository;
            _TradingChargeTypeMaster = TradingChargeTypeMaster;
            _pushNotificationsQueue = pushNotificationsQueue;
            _messageService = MessageService;
            _encdecAEC = encdecAEC;
            _DepositionInterval = DepositionInterval;
            _dbContext = dbContext;
            _walletRepository = walletRepository;
            _WithdrawAdminReqCommonRepo = WithdrawAdminReqCommonRepo;
            _mediator = mediator;
        }
        #endregion

        #region TransactionBlockedChannel

        public async Task<BizResponseClass> BlockTranForChannel(TransactionBlockedChannelReq Request, long UserID)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                if (Request.ID == 0)
                {
                    var IsExist = await _TranBlockedChannelRepo.GetSingleAsync(item => item.TrnType == Convert.ToInt16(Request.TrnType) && item.ChannelID == Request.ChannelID);
                    if (IsExist == null)
                    {
                        TransactionBlockedChannel newObj = new TransactionBlockedChannel();
                        newObj.ChannelID = Request.ChannelID;
                        newObj.TrnType = Convert.ToInt16(Request.TrnType);
                        newObj.Status = Convert.ToInt16(Request.Status == null ? 1 : Request.Status);
                        newObj.CreatedBy = UserID;
                        newObj.CreatedDate = Helpers.UTC_To_IST();
                        await _TranBlockedChannelRepo.AddAsync(newObj);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                        Resp.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        if (IsExist.Status == 9)
                        {
                            IsExist.Status = Convert.ToInt16(ServiceStatus.Active);
                            IsExist.UpdatedBy = UserID;
                            IsExist.UpdatedDate = Helpers.UTC_To_IST();
                            _TranBlockedChannelRepo.UpdateWithAuditLog(IsExist);

                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                            Resp.ErrorCode = enErrorCode.Success;
                        }
                        else
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.AlredyExist;
                            Resp.ErrorCode = enErrorCode.AlredyExist;
                        }
                    }
                }
                else
                {
                    var IsExist = await _TranBlockedChannelRepo.GetByIdAsync(Request.ID);
                    if (IsExist != null)
                    {
                        IsExist.ChannelID = Request.ChannelID;
                        IsExist.TrnType = Convert.ToInt16(Request.TrnType);
                        IsExist.Status = Convert.ToInt16(Request.Status == null ? 1 : Request.Status);
                        IsExist.UpdatedBy = UserID;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        var IsDuplicate = await _TranBlockedChannelRepo.GetSingleAsync(item => item.TrnType == Convert.ToInt16(Request.TrnType) && item.ChannelID == Request.ChannelID && item.Id != Request.ID);
                        if (IsDuplicate == null)
                        {
                            _TranBlockedChannelRepo.UpdateWithAuditLog(IsExist);
                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                            Resp.ErrorCode = enErrorCode.Success;
                        }
                        else
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.AlredyExist;
                            Resp.ErrorCode = enErrorCode.AlredyExist;
                        }
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.NotFound;
                        Resp.ReturnMsg = EnResponseMessage.NotFound;
                    }
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("BlockTranForChannel", "ControlPanelServices", ex);
                throw;
            }
        }

        public ListTransactionBlockedChannelRes GetBlockTranForChannel(long ID, short? Status, long? ChannelID)
        {
            ListTransactionBlockedChannelRes Resp = new ListTransactionBlockedChannelRes();
            try
            {
                var Data = _controlPanelRepository.GetBlockTranChannelDetail(ID, Status, ChannelID);
                if (Data.Count > 0)
                {
                    Resp.Details = Data;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetBlockTranForChannel", "ControlPanelServices", ex);
                throw;
            }
        }

        public ListTransactionBlockedChannelRes ListBlockTranForChannel(enWalletTrnType? trnType, long? channelID, short? status)
        {
            ListTransactionBlockedChannelRes Resp = new ListTransactionBlockedChannelRes();
            try
            {
                var Data = _controlPanelRepository.ListBlockTranChannelDetail(trnType, channelID, status);
                if (Data.Count > 0)
                {
                    Resp.Details = Data;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListBlockTranForChannel", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region User Methods

        public TodaysCount GetUserCount(long? OrgID, long? UserType, short? Status, long? RoleID)
        {
            TodaysCount Resp = new TodaysCount();
            try
            {
                DateTime FromDate = Helpers.UTC_To_IST().Date;
                DateTime Todate = Helpers.UTC_To_IST().Date;

                FromDate = FromDate.AddHours(00).AddMinutes(00).AddSeconds(00);
                Todate = Todate.AddHours(23).AddMinutes(59).AddSeconds(59);

                var user = _controlPanelRepository.GetUserRecCounts(OrgID, UserType, Status, RoleID, FromDate, Todate);
                Resp.TotalCount = user.TotalCount;
                Resp.TodayCount = user.TodayCount;
                if (user.TotalCount == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public StatusWiseRes GetUCntStatusWise()
        {
            StatusWiseRes Resp = new StatusWiseRes();
            try
            {
                var obj = _controlPanelRepository.GetStatusWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public UTypeWiseRes GetUCntTypeWise()
        {
            UTypeWiseRes Resp = new UTypeWiseRes();
            try
            {
                var list = _controlPanelRepository.GetTypeWiseCount();
                if (list.Count > 0)
                {
                    Resp.Counter = list;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public OrgWiseRes GetUCntOrgWise()
        {
            OrgWiseRes Resp = new OrgWiseRes();
            try
            {
                var list = _controlPanelRepository.GetOrgWiseCount();
                if (list.Count > 0)
                {
                    Resp.Counter = list;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public RolewiseUserCount GetUCntRoleWise()
        {
            RolewiseUserCount Resp = new RolewiseUserCount();
            try
            {
                var list = _controlPanelRepository.GetRoleWiseCount();
                if (list.Count > 0)
                {
                    Resp.Counter = list;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public CountRes GetTodayUserCount(short? status)
        {
            CountRes Resp = new CountRes();
            try
            {
                DateTime FromDate = Helpers.UTC_To_IST().Date;
                DateTime Todate = Helpers.UTC_To_IST().Date;

                FromDate = FromDate.AddHours(00).AddMinutes(00).AddSeconds(00);
                Todate = Todate.AddHours(23).AddMinutes(59).AddSeconds(59);

                var org = _controlPanelRepository.GetTodaysUserRecCount(status, FromDate, Todate);
                Resp.Count = org;
                if (org == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public ListUserDetailRes ListAllUserDetails(long? OrgID, long? UserType, short? Status, int? PageNo, int? PageSize)
        {
            ListUserDetailRes Resp = new ListUserDetailRes();
            try
            {
                var usersdata = _controlPanelRepository.ListAllUserDetail(OrgID, UserType, Status, PageNo, PageSize);
                if (usersdata.Details.Count > 0)
                {
                    Resp.Details = usersdata.Details;
                    Resp.TotalPages = usersdata.TotalPages;
                    Resp.PageSize = usersdata.PageSize;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public ListUserTypeRes ListAllUserType(short? status)
        {
            try
            {
                ListUserTypeRes Resp = new ListUserTypeRes();
                var usertypes = _controlPanelRepository.ListAllUserTypes(status);
                if (usertypes.Count > 0)
                {
                    Resp.Details = usertypes;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 27-11-2018
        public UserList ListUserLast5()
        {
            UserList Resp = new UserList();
            try
            {
                var obj = _controlPanelRepository.ListUserLast5();
                Resp.UserLists = obj;
                Resp.WalletUserCount = _controlPanelRepository.GetWalletAuthUserCount(null, null, null, null);
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region WalletPolicyAllowedDay

        public async Task<BizResponseClass> AddWPolicyAllowedDay(WalletPolicyAllowedDayReq Request, long UserID)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                if (Request.ID == 0) //Insert Operation
                {
                    var IsExist = await _WalletPolicyAllowedDayRepo.GetSingleAsync(item => item.WalletPolicyID == Request.WalletPolicyID && item.DayNo == Convert.ToInt16(Request.DayNo));
                    if (IsExist == null)
                    {
                        WalletPolicyAllowedDay newObj = new WalletPolicyAllowedDay();
                        newObj.WalletPolicyID = Request.WalletPolicyID;
                        newObj.DayNo = Convert.ToInt16(Request.DayNo);
                        newObj.Status = Convert.ToInt16(Request.Status == null ? 1 : Request.Status);
                        newObj.CreatedBy = UserID;
                        newObj.CreatedDate = Helpers.UTC_To_IST();
                        await _WalletPolicyAllowedDayRepo.AddAsync(newObj);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                        Resp.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        if (IsExist.Status == 9)
                        {
                            IsExist.Status = Convert.ToInt16(ServiceStatus.Active);
                            IsExist.UpdatedBy = UserID;
                            IsExist.UpdatedDate = Helpers.UTC_To_IST();
                            _WalletPolicyAllowedDayRepo.UpdateWithAuditLog(IsExist);

                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                            Resp.ErrorCode = enErrorCode.Success;
                        }
                        else
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.AlredyExist;
                            Resp.ErrorCode = enErrorCode.AlredyExist;
                        }
                    }
                }
                else //Update Operation
                {
                    var IsExist = await _WalletPolicyAllowedDayRepo.GetByIdAsync(Request.ID);
                    if (IsExist != null)
                    {
                        IsExist.WalletPolicyID = Request.WalletPolicyID;
                        IsExist.DayNo = Convert.ToInt16(Request.DayNo);
                        IsExist.Status = Convert.ToInt16(Request.Status == null ? 1 : Request.Status);
                        IsExist.UpdatedBy = UserID;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        _WalletPolicyAllowedDayRepo.UpdateWithAuditLog(IsExist);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                        Resp.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.NotFound;
                        Resp.ReturnMsg = EnResponseMessage.NotFound;
                    }
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddWPolicyAllowedDay", this.GetType().Name, ex);
                throw;
            }
        }

        public ListWalletPolicyAllowedDayRes GetWPolicyAllowedDay(long ID, EnWeekDays? DayNo, long? PolicyID, short? Status)
        {
            ListWalletPolicyAllowedDayRes Resp = new ListWalletPolicyAllowedDayRes();
            try
            {
                Resp.Details = _controlPanelRepository.GetWPolicyAllowedDays(ID, DayNo, PolicyID, Status);
                if (Resp.Details.Count > 0)
                {
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWPolicyAllowedDay", this.GetType().Name, ex);
                throw;
            }
        }

        public ListWalletPolicyAllowedDayRes ListWPolicyAllowedDay(EnWeekDays? DayNo, long? PolicyID, short? Status)
        {
            ListWalletPolicyAllowedDayRes Resp = new ListWalletPolicyAllowedDayRes();
            try
            {
                Resp.Details = _controlPanelRepository.ListWPolicyAllowedDays(DayNo, PolicyID, Status);
                if (Resp.Details.Count > 0)
                {
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListWPolicyAllowedDay", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region Organization Methods

        public TodaysCount GetOrganizationCount(short? status)
        {
            TodaysCount Resp = new TodaysCount();
            try
            {
                DateTime FromDate = Helpers.UTC_To_IST().Date;
                DateTime Todate = Helpers.UTC_To_IST().Date;

                FromDate = FromDate.AddHours(00).AddMinutes(00).AddSeconds(00);
                Todate = Todate.AddHours(23).AddMinutes(59).AddSeconds(59);

                var org = _controlPanelRepository.GetOrgRecCount(status);
                var today = _controlPanelRepository.GetTodaysUserRecCount(status, FromDate, Todate);

                Resp.TotalCount = org;
                Resp.TodayCount = today;

                if (org == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public CountRes GetTodayOrganizationCount(short? status)
        {
            CountRes Resp = new CountRes();
            try
            {
                DateTime FromDate = Helpers.UTC_To_IST().Date;
                DateTime Todate = Helpers.UTC_To_IST().Date;

                FromDate = FromDate.AddHours(00).AddMinutes(00).AddSeconds(00);
                Todate = Todate.AddHours(23).AddMinutes(59).AddSeconds(59);

                var org = _controlPanelRepository.GetTodaysOrgRecCount(status, FromDate, Todate);
                Resp.Count = org;
                if (org == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<OrgList> ListOrgDetail(short? Status, long? OrgID)
        {
            OrgList Resp = new OrgList();
            try
            {
                OrgMasterRes obj1 = new OrgMasterRes();
                var obj = await _controlPanelRepository.ListOrgDetail(Status, OrgID);
                if (obj.Count > 0)
                {
                    Resp.Organizations = obj;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListOrgDetail", this.GetType().Name, ex);
                throw;
            }
        }

        public BizResponseClass SetDefaultOrganization(long orgID, long userid)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = _OrgMasterCommonRepo.GetSingle(item => item.Id == orgID);
                if (IsExist != null)
                {
                    bool remaining = _controlPanelRepository.SetDefaultZero(userid, Helpers.UTC_To_IST());
                    if (remaining)
                    {
                        IsExist.IsDefault = 1;
                        IsExist.UpdatedBy = userid;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        _OrgMasterCommonRepo.Update(IsExist);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                        Resp.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        Resp.ErrorCode = enErrorCode.RemainingRecordNotUpdated;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.RemainingRecordNotUpdated;
                    }
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
            return Resp;
        }

        #endregion

        #region Wallet Methods

        public CountRes GetWalletCount(long? walletTypeID, short? status, long? orgID, long? userID)
        {
            CountRes Resp = new CountRes();
            try
            {
                var Wallet = _controlPanelRepository.GetWalletRecCounts(walletTypeID, status, orgID, userID);
                Resp.Count = Wallet.TotalCount;
                if (Wallet.TotalCount == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public WalletTypeWiseRes GetWCntTypeWise()
        {
            WalletTypeWiseRes Resp = new WalletTypeWiseRes();
            try
            {
                var obj = _controlPanelRepository.GetWalletTypeWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public WalletTypeWiseRes GetWCntStatusWise()
        {
            WalletTypeWiseRes Resp = new WalletTypeWiseRes();
            try
            {
                var obj = _controlPanelRepository.GetWalletStatusWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public WalletTypeWiseRes GetWCntOrgWise()
        {
            WalletTypeWiseRes Resp = new WalletTypeWiseRes();
            try
            {
                var obj = _controlPanelRepository.GetWalletOrgWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public WalletTypeWiseRes GetWCntUserWise()
        {
            WalletTypeWiseRes Resp = new WalletTypeWiseRes();
            try
            {
                var obj = _controlPanelRepository.GetWalletUserWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-17
        public ListWalletResV1 ListAllWallet(DateTime? FromDate, DateTime? ToDate, short? Status, int PageSize, int Page, long? UserId, long? OrgId, string WalletType)
        {
            ListWalletResV1 Resp = new ListWalletResV1();
            try
            {
                long TotalWallet = 0;
                Resp.PageNo = Page;
                Resp.PageSize = PageSize;
                var obj = _controlPanelRepository.ListAllWallet(FromDate, ToDate, Status, PageSize, Page + 1, UserId, OrgId, WalletType, ref TotalWallet);
                Resp.TotalWallet = TotalWallet;

                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-17
        public WalletRes1 GetWalletIdWise(string AccWalletId)
        {
            WalletRes1 Resp = new WalletRes1();
            try
            {
                var obj = _controlPanelRepository.GetWalletIdWise(AccWalletId);
                Resp.Data = obj;
                if (obj != null)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }
        #endregion

        #region Wallet Authorized User Methods

        public CountRes GetWalletAuthUserCount(short? status, long? orgID, long? userID, long? RoleID)
        {
            CountRes Resp = new CountRes();
            try
            {
                var Wallet = _controlPanelRepository.GetWalletAuthUserCount(status, orgID, userID, RoleID);
                Resp.Count = Wallet;
                if (Wallet == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public WalletTypeWiseRes GetWAUCntStatusWise()
        {
            WalletTypeWiseRes Resp = new WalletTypeWiseRes();
            try
            {
                var obj = _controlPanelRepository.GetWalletAuthUserStatusWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public WalletTypeWiseRes GetWAUCntOrgWise()
        {
            WalletTypeWiseRes Resp = new WalletTypeWiseRes();
            try
            {
                var obj = _controlPanelRepository.GetWalletAuthUserOrgWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public RolewiseUserCount GetWAUCntRoleWise()
        {
            RolewiseUserCount Resp = new RolewiseUserCount();
            try
            {
                var obj = _controlPanelRepository.GetWalletAuthUserRoleWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region User Role Methods

        public CountRes GetUserRoleCount(short? status)
        {
            CountRes Resp = new CountRes();
            try
            {
                var Wallet = _controlPanelRepository.GetUserRoleCount(status);
                Resp.Count = Wallet;
                if (Wallet == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public RoleList ListRoleDetail(short? status)
        {
            RoleList Resp = new RoleList();
            try
            {
                var obj = _controlPanelRepository.ListRoleDetail(status);
                Resp.Roles = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region Authorized Apps

        public async Task<BizResponseClass> AddAuthorizedApps(AuthAppReq Request, long UserID)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                if (Request.AppID == 0)
                {
                    var IsExist = await _AuthAppCommonRepo.GetSingleAsync(item => item.SecretKey == Request.SecreteKey && item.SiteURL == Request.SiteURL); //item.AppName == Request.AppName && --Removed this on 17-12-2018 as per the instruction by nupoora mam
                    if (IsExist == null)
                    {
                        AutorizedApps newobj = new AutorizedApps();
                        newobj.AppName = Request.AppName;
                        newobj.SecretKey = Request.SecreteKey;
                        newobj.Status = Convert.ToInt16(Request.Status == null ? 1 : Request.Status);
                        newobj.SiteURL = Request.SiteURL;
                        newobj.CreatedBy = UserID;
                        newobj.CreatedDate = Helpers.UTC_To_IST();
                        await _AuthAppCommonRepo.AddAsync(newobj);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                        Resp.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        if (IsExist.Status == 9)
                        {
                            IsExist.Status = Convert.ToInt16(ServiceStatus.Active);
                            IsExist.UpdatedBy = UserID;
                            IsExist.UpdatedDate = Helpers.UTC_To_IST();
                            _AuthAppCommonRepo.UpdateWithAuditLog(IsExist);

                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                            Resp.ErrorCode = enErrorCode.Success;
                        }
                        else
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.AlredyExist;
                            Resp.ErrorCode = enErrorCode.AlredyExist;
                        }
                    }
                }
                else
                {
                    var IsExist = await _AuthAppCommonRepo.GetSingleAsync(item => item.Id == Request.AppID && item.SecretKey == Request.SecreteKey && item.SiteURL == Request.SiteURL);
                    if (IsExist != null)
                    {
                        IsExist.AppName = Request.AppName;
                        IsExist.SecretKey = Request.SecreteKey;
                        IsExist.SiteURL = Request.SiteURL;
                        IsExist.Status = Convert.ToInt16(Request.Status == null ? 1 : Request.Status);
                        IsExist.UpdatedBy = UserID;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        _AuthAppCommonRepo.UpdateWithAuditLog(IsExist);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                        Resp.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.NotFound;
                        Resp.ReturnMsg = EnResponseMessage.NotFound;
                    }
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddAuthorizedApps", this.GetType().Name, ex);
                throw;
            }
        }

        public ListAuthAppRes ListAuthorizedAppDetail(short? Status)
        {
            try
            {
                ListAuthAppRes Resp = new ListAuthAppRes();
                var IsExist = _controlPanelRepository.GetAllAuthAppDetail(Status);
                if (IsExist.Count > 0)
                {
                    Resp.Details = IsExist;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListAuthorizedAppDetail", this.GetType().Name, ex);
                throw;
            }
        }

        public ListAuthAppRes GetAuthorizedAppDetail(long AppId, short? Status)
        {
            try
            {
                ListAuthAppRes Resp = new ListAuthAppRes();
                var IsExist = _controlPanelRepository.GetAuthAppDetail(AppId, Status);
                if (IsExist.Count > 0)
                {
                    Resp.Details = IsExist;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetAuthorizedAppDetail", this.GetType().Name, ex);
                throw;
            }
        }


        #endregion

        #region Currency Method

        public CountRes GetCurrencyCount(short? status)
        {
            CountRes Resp = new CountRes();
            try
            {
                var Wallet = _controlPanelRepository.GetCurrencyCount(status);
                Resp.Count = Wallet;
                if (Wallet == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public CurrencyList ListCurrencyDetail(short? status)
        {
            CurrencyList Resp = new CurrencyList();
            try
            {
                var obj = _controlPanelRepository.ListCurrencyDetail(status);
                Resp.Currencies = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> AddNewCurrencyType(CurrencyMasterReq Request, long UserID)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                if (Request.ID == 0)
                {
                    var IsExist = await _CurrencyTypeMasterRepo.GetSingleAsync(item => item.CurrencyTypeName == Request.CurrencyTypeName);
                    if (IsExist == null)
                    {
                        var MaxTypeID = _controlPanelRepository.GetMaxPlusOne();
                        if (MaxTypeID > 0)
                        {
                            CurrencyTypeMaster NewObj = new CurrencyTypeMaster();
                            NewObj.CurrencyTypeName = Request.CurrencyTypeName;
                            NewObj.CurrencyTypeId = MaxTypeID;
                            NewObj.Status = Convert.ToInt16(Request.Status == null ? 1 : Request.Status);
                            NewObj.CreatedDate = Helpers.UTC_To_IST();
                            NewObj.CreatedBy = UserID;
                            await _CurrencyTypeMasterRepo.AddAsync(NewObj);

                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                            Resp.ErrorCode = enErrorCode.Success;
                        }
                        else
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.MaxIdNotFound;
                            Resp.ErrorCode = enErrorCode.MaxIdGenerationFail;
                        }
                    }
                    else
                    {
                        if (IsExist.Status == 9)
                        {
                            IsExist.Status = Convert.ToInt16(ServiceStatus.Active);
                            IsExist.UpdatedBy = UserID;
                            IsExist.UpdatedDate = Helpers.UTC_To_IST();
                            _CurrencyTypeMasterRepo.UpdateWithAuditLog(IsExist);

                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                            Resp.ErrorCode = enErrorCode.Success;
                        }
                        else
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.AlredyExist;
                            Resp.ErrorCode = enErrorCode.AlredyExist;
                        }
                    }
                }
                else
                {
                    var IsExist = await _CurrencyTypeMasterRepo.GetByIdAsync(Request.ID);
                    if (IsExist != null)
                    {
                        IsExist.CurrencyTypeName = Request.CurrencyTypeName;
                        IsExist.Status = Convert.ToInt16(Request.Status == null ? 1 : Request.Status);
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        IsExist.UpdatedBy = UserID;
                        _CurrencyTypeMasterRepo.UpdateWithAuditLog(IsExist);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                        Resp.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.NotFound;
                        Resp.ReturnMsg = EnResponseMessage.NotFound;
                    }
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddNewCurrencyType", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region Wallet Type Methods

        public async Task<BizResponseClass> AddWalletTypeDetails(WalletTypeMasterReq Request, long UserID)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                if (Request.ID == 0)
                {
                    var IsExist = await _WalletTypeMaster.GetSingleAsync(item => item.WalletTypeName == Request.WalletTypeName && item.CurrencyTypeID == Convert.ToInt16(Request.CurrencyTypeID));
                    if (IsExist == null)
                    {
                        WalletTypeMaster NewObj = new WalletTypeMaster();
                        NewObj.WalletTypeName = Request.WalletTypeName;
                        NewObj.CurrencyTypeID = Convert.ToInt16(Request.CurrencyTypeID);
                        NewObj.Description = Convert.ToString(Request.Description ?? "-");
                        NewObj.Status = Convert.ToInt16(Request.Status ?? 1);
                        NewObj.CreatedBy = UserID;
                        NewObj.CreatedDate = Helpers.UTC_To_IST();
                        await _WalletTypeMaster.AddAsync(NewObj);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                        Resp.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        if (IsExist.Status == 9)
                        {
                            IsExist.Status = Convert.ToInt16(ServiceStatus.Active);
                            IsExist.UpdatedBy = UserID;
                            IsExist.UpdatedDate = Helpers.UTC_To_IST();
                            _WalletTypeMaster.UpdateWithAuditLog(IsExist);

                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                            Resp.ErrorCode = enErrorCode.Success;
                        }
                        else
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.AlredyExist;
                            Resp.ErrorCode = enErrorCode.AlredyExist;
                        }
                    }
                }
                else
                {
                    var IsExist = await _WalletTypeMaster.GetByIdAsync(Request.ID);
                    if (IsExist != null)
                    {
                        IsExist.WalletTypeName = Request.WalletTypeName;
                        IsExist.CurrencyTypeID = Convert.ToInt16(Request.CurrencyTypeID);
                        IsExist.Description = Convert.ToString(Request.Description ?? "-");
                        IsExist.Status = Convert.ToInt16(Request.Status ?? 1);
                        IsExist.UpdatedBy = UserID;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        _WalletTypeMaster.UpdateWithAuditLog(IsExist);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                        Resp.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.NotFound;
                        Resp.ReturnMsg = EnResponseMessage.NotFound;
                    }
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddWalletTypeDetails", this.GetType().Name, ex);
                throw;
            }
        }

        public CountRes GetWalletTypeCount(short? status, long? CurrencyTypeID)
        {
            CountRes Resp = new CountRes();
            try
            {
                var Wallet = _controlPanelRepository.GetWalletTypeCount(status, CurrencyTypeID);
                Resp.Count = Wallet;
                if (Wallet == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWalletTypeCount", this.GetType().Name, ex);
                throw;
            }
        }

        public WalletTypeMasterResp GetWalletTypeDetail(long typeID)
        {
            WalletTypeMasterResp Resp = new WalletTypeMasterResp();
            try
            {
                var data = _controlPanelRepository.GetWalletTypeDetails(typeID);
                if (data != null)
                {
                    Resp = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWalletTypeDetail", this.GetType().Name, ex);
                throw;
            }
        }

        public WalletTypeList ListWalletTypeDetail(short? status, long? ServiceProviderId, long? currencyTypeID, short IsMargin)
        {
            WalletTypeList Resp = new WalletTypeList();
            try
            {
                var obj = _controlPanelRepository.ListWalletTypeDetail(status, ServiceProviderId, currencyTypeID, IsMargin);
                Resp.Types = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListWalletTypeDetail", this.GetType().Name, ex);
                throw;
            }
        }


        #endregion

        #region ChargeTypeMaster Methods

        //vsolanki 24-11-2018
        public CountRes GetChargeTypeCount(short? status, long? ChargeTypeID)
        {
            CountRes Resp = new CountRes();
            try
            {
                var item = _controlPanelRepository.GetChargeTypeCount(status, ChargeTypeID);
                Resp.Count = item;
                if (item == 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 24-11-2018
        public ChargeTypeList ListChargeTypeDetail(short? status, long? ChargeTypeID)
        {
            ChargeTypeList Resp = new ChargeTypeList();
            try
            {
                var obj = _controlPanelRepository.ListChargeTypeDetail(status, ChargeTypeID);
                Resp.Types = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-14
        public BizResponseClass InsertUpdateChargeType(ChargeTypeReq Req, long UserId)
        {
            try
            {
                var obj = _controlPanelRepository.InsertUpdateChargeType(Req, UserId);
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        //2018-12-14
        public BizResponseClass ChangeChargeTypeStatus(long Id, short Status, long UserId)
        {
            try
            {
                var obj = _controlPanelRepository.ChangeChargeTypeStatus(Id, Status, UserId);
                return obj;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion

        #region CommissionTypeMaster Methods

        //vsolanki 24-11-2018
        public CountRes GetCommissionTypeCount(short? status, long? TypeID)
        {
            CountRes Resp = new CountRes();
            try
            {
                var item = _controlPanelRepository.GetCommissionTypeCount(status, TypeID);
                Resp.Count = item;
                if (item == 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 24-11-2018
        public CommisssionTypeList ListCommissionTypeDetail(short? status, long? TypeID)
        {
            CommisssionTypeList Resp = new CommisssionTypeList();
            try
            {
                var obj = _controlPanelRepository.ListCommisssionTypeDetail(status, TypeID);
                Resp.Types = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-13
        public BizResponseClass InsertUpdateCommisssionType(CommisssionTypeReq Req, long UserId)
        {
            CommissionTypeMaster Obj = new CommissionTypeMaster();
            try
            {
                if (Req == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidInput, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput };
                }
                var CommissionType = _CommissionTypeMaster.GetSingle(i => i.Id == Req.TypeId);
                if (CommissionType != null)
                {
                    //update
                    CommissionType.Status = Req.Status;
                    CommissionType.TypeName = Req.TypeName;
                    CommissionType.UpdatedBy = UserId;
                    CommissionType.UpdatedDate = Helpers.UTC_To_IST();
                    _CommissionTypeMaster.UpdateWithAuditLog(CommissionType);
                    _walletConfiguration.UpdateCommissionTypeMasterList();
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
                Obj.Status = Req.Status;
                Obj.UpdatedBy = null;
                Obj.UpdatedDate = Helpers.UTC_To_IST();
                Obj.CreatedBy = UserId;
                Obj.CreatedDate = Helpers.UTC_To_IST();
                Obj.TypeName = Req.TypeName;
                //insert
                _CommissionTypeMaster.Add(Obj);

                return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-13
        public BizResponseClass ChangeCommisssionTypeReqStatus(long Id, short Status, long UserId)
        {
            try
            {
                var trnType = _CommissionTypeMaster.GetSingle(i => i.Id == Id);
                if (trnType != null)
                {
                    //update
                    trnType.Status = Status;
                    trnType.UpdatedBy = UserId;
                    trnType.UpdatedDate = Helpers.UTC_To_IST();
                    _CommissionTypeMaster.UpdateWithAuditLog(trnType);
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
                return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region "Charge Policy"

        //vsolanki 24-11-2018
        public CountRes GetChargePolicyRecCount(long? WalletTypeID, short? status, long? WalletTrnTypeID)
        {
            CountRes Resp = new CountRes();
            try
            {
                var item = _controlPanelRepository.GetChargePolicyRecCount(WalletTypeID, status, WalletTrnTypeID);
                Resp.Count = item;
                if (item == 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 24-11-2018
        public WalletTypeWiseRes GetChargePolicyWalletTypeWiseCount()
        {
            WalletTypeWiseRes Resp = new WalletTypeWiseRes();
            try
            {
                var obj = _controlPanelRepository.GetChargePolicyWalletTypeWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 24-11-2018
        public StatusWiseRes GetChargePolicyStatusWiseCount()
        {
            StatusWiseRes Resp = new StatusWiseRes();
            try
            {
                var obj = _controlPanelRepository.GetChargePolicyStatusWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 24-11-2018
        public WalletTrnTypeWiseRes GetChargePolicyWalletTrnTypeWiseCount()
        {
            WalletTrnTypeWiseRes Resp = new WalletTrnTypeWiseRes();
            try
            {
                var obj = _controlPanelRepository.GetChargePolicyWalletTrnTypeWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 24-11-2018
        public ListChargePolicy GetChargePolicyList(short? status, long? WalletType, long? WalletTrnType)
        {
            ListChargePolicy Resp = new ListChargePolicy();
            try
            {
                var obj = _controlPanelRepository.GetChargePolicyList(status, WalletType, WalletTrnType);
                Resp.ChargePolicyList = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 27-11-2018
        public ListChargePolicy ListChargePolicyLast5()
        {
            ListChargePolicy Resp = new ListChargePolicy();
            try
            {
                var obj = _controlPanelRepository.ListChargePolicyLast5();
                Resp.ChargePolicyList = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 2018-11-28
        public BizResponseClass InsertChargePolicy(ChargePolicyReq Req)
        {
            try
            {
                var obj1 = _ChargePolicyCommonRepo.GetSingle(i => i.WalletType == Req.WalletType && i.WalletTrnType == Req.WalletTrnType);
                ChargePolicy Obj = new ChargePolicy();
                if (Req == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidInput, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput };
                }
                if (obj1.WalletTrnType == Req.WalletTrnType && obj1.WalletType == Req.WalletType)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.DuplicateRecord, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DuplicateRecord };
                }
                Obj.CreatedDate = Helpers.UTC_To_IST();
                Obj.CreatedBy = 1;
                Obj.UpdatedBy = null;
                Obj.UpdatedDate = Helpers.UTC_To_IST();
                Obj.Status = Req.Status;
                Obj.PolicyName = Req.PolicyName;
                Obj.WalletTrnType = Req.WalletTrnType;
                Obj.MinAmount = Req.MinAmount;
                Obj.MaxAmount = Req.MaxAmount;
                Obj.WalletType = Req.WalletType;
                Obj.Type = Req.Type;
                Obj.ChargeType = Req.ChargeType;
                Obj.ChargeValue = Req.ChargeValue;
                //insert
                _ChargePolicyCommonRepo.Add(Obj);

                return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 2018-11-28
        public BizResponseClass UpdateChargePolicy(long Id, UpdateChargePolicyReq Req, long UserId)
        {
            try
            {
                ChargePolicy Obj = _ChargePolicyCommonRepo.GetSingle(i => i.Id == Id);
                if (Req == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidInput, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput };
                }
                if (Obj == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                }
                Obj.UpdatedBy = UserId;
                Obj.UpdatedDate = Helpers.UTC_To_IST();
                Obj.Status = Req.Status;
                Obj.PolicyName = Req.PolicyName;
                Obj.MinAmount = Req.MinAmount;
                Obj.MaxAmount = Req.MaxAmount;
                Obj.Type = Req.Type;
                Obj.ChargeType = Req.ChargeType;
                Obj.ChargeValue = Req.ChargeValue;
                //update
                _ChargePolicyCommonRepo.UpdateWithAuditLog(Obj);

                return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }
        #endregion

        #region Commission Policy

        //vsolanki 24-11-2018
        public CountRes GetCommissionPolicyRecCount(long? WalletTypeID, short? status, long? WalletTrnTypeID)
        {
            CountRes Resp = new CountRes();
            try
            {
                var item = _controlPanelRepository.GetCommissionPolicyRecCount(WalletTypeID, status, WalletTrnTypeID);
                Resp.Count = item;
                if (item == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 24-11-2018
        public WalletTypeWiseRes GetCommissionPolicyWalletTypeWiseCount()
        {
            WalletTypeWiseRes Resp = new WalletTypeWiseRes();
            try
            {
                var obj = _controlPanelRepository.GetCommissionPolicyWalletTypeWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 24-11-2018
        public StatusWiseRes GetCommissionPolicyStatusWiseCount()
        {
            StatusWiseRes Resp = new StatusWiseRes();
            try
            {
                var obj = _controlPanelRepository.GetCommissionPolicyStatusWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 24-11-2018
        public WalletTrnTypeWiseRes GetCommissionPolicyWalletTrnTypeWiseCount()
        {
            WalletTrnTypeWiseRes Resp = new WalletTrnTypeWiseRes();
            try
            {
                var obj = _controlPanelRepository.GetCommissionPolicyWalletTrnTypeWiseCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 24-11-2018
        public ListCommissionPolicy GetCommissionPolicyList(short? status, long? WalletType, long? WalletTrnType)
        {
            ListCommissionPolicy Resp = new ListCommissionPolicy();
            try
            {
                var obj = _controlPanelRepository.GetCommissionPolicyList(status, WalletType, WalletTrnType);
                Resp.CommissionPolicyList = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsolanki 24-11-2018
        public ListCommissionPolicy ListCommissionPolicy()
        {
            ListCommissionPolicy Resp = new ListCommissionPolicy();
            try
            {
                var obj = _controlPanelRepository.ListCommissionPolicy();
                Resp.CommissionPolicyList = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //Rushabh 28/11/2018
        public BizResponseClass AddCommPolicy(CommPolicyReq request, long UserID)
        {
            BizResponseClass Res = new BizResponseClass();
            CommissionPolicy IsExist = new CommissionPolicy();
            try
            {
                IsExist = _commissionPolicyRepository.GetSingle(item => item.WalletTrnType == Convert.ToInt64(request.WalletTrnType) && item.WalletType == request.WalletType);
                if (IsExist == null)
                {
                    CommissionPolicy AddNew = new CommissionPolicy();
                    AddNew.CommissionType = request.CommType;
                    AddNew.CommissionValue = request.CommValue;
                    AddNew.CreatedBy = UserID;
                    AddNew.CreatedDate = Helpers.UTC_To_IST();
                    AddNew.MaxAmount = request.MaxAmount;
                    AddNew.MinAmount = request.MinAmount;
                    AddNew.PolicyName = request.PolicyName;
                    AddNew.Status = Convert.ToInt16(ServiceStatus.Active);
                    AddNew.Type = request.Type;
                    AddNew.WalletTrnType = Convert.ToInt64(request.WalletTrnType);
                    AddNew.WalletType = request.WalletType;
                    _commissionPolicyRepository.Add(AddNew);

                    Res.ReturnCode = enResponseCode.Success;
                    Res.ReturnMsg = EnResponseMessage.RecordAdded;
                    Res.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    if (IsExist.Status == 9)
                    {
                        IsExist.Status = Convert.ToInt16(ServiceStatus.Active);
                        IsExist.UpdatedBy = UserID;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        _commissionPolicyRepository.UpdateWithAuditLog(IsExist);

                        Res.ReturnCode = enResponseCode.Success;
                        Res.ReturnMsg = EnResponseMessage.RecordAdded;
                        Res.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        Res.ReturnCode = enResponseCode.Fail;
                        Res.ReturnMsg = EnResponseMessage.AlredyExist;
                        Res.ErrorCode = enErrorCode.AlredyExist;
                    }
                }
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //Rushabh 28/11/2018
        public BizResponseClass UpdateCommPolicyDetail(long CommPolicyID, UpdateCommPolicyReq request, long UserID)
        {
            BizResponseClass Res = new BizResponseClass();
            CommissionPolicy IsExist = new CommissionPolicy();
            try
            {
                IsExist = _commissionPolicyRepository.GetSingle(item => item.Id == CommPolicyID);
                if (IsExist == null)
                {
                    Res.ReturnCode = enResponseCode.Fail;
                    Res.ReturnMsg = EnResponseMessage.NotFound;
                    Res.ErrorCode = enErrorCode.NotFound;
                    return Res;
                }
                else
                {
                    IsExist.CommissionType = request.CommType;
                    IsExist.CommissionValue = request.CommValue;
                    IsExist.UpdatedBy = UserID;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    IsExist.MaxAmount = request.MaxAmount;
                    IsExist.MinAmount = request.MinAmount;
                    IsExist.PolicyName = request.PolicyName;
                    IsExist.Status = Convert.ToInt16(ServiceStatus.Active);
                    IsExist.Type = request.Type;
                    _commissionPolicyRepository.UpdateWithAuditLog(IsExist);

                    Res.ReturnCode = enResponseCode.Success;
                    Res.ReturnMsg = EnResponseMessage.RecordUpdated;
                    Res.ErrorCode = enErrorCode.Success;
                }
                return Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region Graph API

        //vsoalnki 2018-11-27
        public ListWalletGraphRes GraphForUserCount()
        {
            try
            {
                ListWalletGraphRes res = new ListWalletGraphRes();
                res = _controlPanelRepository.GraphForUserCount();
                if (res.Month.Count == 0 || res.TotalCount.Count == 0)
                {
                    res.ErrorCode = enErrorCode.NotFound;
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.NotFound;
                    return res;
                }
                return new ListWalletGraphRes
                {
                    Month = res.Month,
                    TotalCount = res.TotalCount,
                    ErrorCode = enErrorCode.Success,
                    ReturnCode = enResponseCode.Success,
                    ReturnMsg = EnResponseMessage.FindRecored,
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }

        }

        //Rushabh 27-11-2018
        public ListWalletGraphRes GraphForOrgCount()
        {
            try
            {
                ListWalletGraphRes res = new ListWalletGraphRes();
                res = _controlPanelRepository.GraphForOrgCount();
                if (res.Month.Count == 0 || res.TotalCount.Count == 0)
                {
                    res.ErrorCode = enErrorCode.NotFound;
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.NotFound;
                    return res;
                }
                return new ListWalletGraphRes
                {
                    Month = res.Month,
                    TotalCount = res.TotalCount,
                    ErrorCode = enErrorCode.Success,
                    ReturnCode = enResponseCode.Success,
                    ReturnMsg = EnResponseMessage.FindRecored,
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }

        }

        //Rushabh 28-11-2018
        public ListTransactionTypewiseCount GraphForTrnTypewiseCount()
        {
            try
            {
                ListTransactionTypewiseCount res = new ListTransactionTypewiseCount();
                res = _controlPanelRepository.GraphForTrnTypewiseCount();
                if (res.TotalCount.Count == 0) //res.TranType.Count == 0 ||
                {
                    res.ErrorCode = enErrorCode.NotFound;
                    res.ReturnCode = enResponseCode.Fail;
                    res.ReturnMsg = EnResponseMessage.NotFound;
                    return res;
                }
                return new ListTransactionTypewiseCount
                {
                    TypeName = res.TypeName,
                    TotalCount = res.TotalCount,
                    ErrorCode = enErrorCode.Success,
                    ReturnCode = enResponseCode.Success,
                    ReturnMsg = EnResponseMessage.FindRecored,
                };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }

        }

        #endregion

        #region Wallet Usage Policy

        //Rushabh 19-12-2018
        public ListWalletusagePolicy2 ListUsagePolicy(long? WalletTypeID, short? Status)
        {
            ListWalletusagePolicy2 Resp = new ListWalletusagePolicy2();
            try
            {
                Resp.Details = _controlPanelRepository.ListUsagePolicyData(WalletTypeID, Status);
                if (Resp.Details.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }


        //vsolanki 27-11-2018
        public ListWalletusagePolicy ListUsagePolicyLast5()
        {
            ListWalletusagePolicy Resp = new ListWalletusagePolicy();
            try
            {
                var obj = _controlPanelRepository.ListUsagePolicyLast5();
                Resp.UsageList = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-14
        public BizResponseClass InsertUpdateWalletUsagePolicy(AddWalletUsagePolicyReq Req, long UserId)
        {
            if (Req == null)
            {
                return new BizResponseClass { ErrorCode = enErrorCode.InvalidInput, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput };
            }
            var WalletType = _WalletTypeMaster.GetSingle(i => i.Id == Req.WalletType);
            if (WalletType == null)
            {
                return new BizResponseClass { ErrorCode = enErrorCode.InvalidWalletType, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWalletType };
            }
            if (Req.Id != 0)
            {
                var WalletUsageObj = _WalletUsagePolicy.GetSingle(i => i.Id == Req.Id && i.WalletType == Req.WalletType);
                if (WalletUsageObj != null)
                {
                    WalletUsageObj.UpdatedBy = UserId;
                    WalletUsageObj.UpdatedDate = Helpers.UTC_To_IST();
                    WalletUsageObj.Status = Req.Status;
                    WalletUsageObj.MinAmount = Req.MinAmount;
                    WalletUsageObj.MaxAmount = Req.MaxAmount;
                    WalletUsageObj.PolicyName = Req.PolicyName;
                    WalletUsageObj.HourlyTrnAmount = Req.HourlyTrnAmount;
                    WalletUsageObj.HourlyTrnCount = Req.HourlyTrnCount;
                    WalletUsageObj.AllowedIP = Req.AllowedIP;
                    WalletUsageObj.AllowedLocation = Req.AllowedLocation;
                    WalletUsageObj.AuthenticationType = Req.AuthenticationType;
                    WalletUsageObj.StartTime = Req.StartTime;
                    WalletUsageObj.EndTime = Req.EndTime;
                    WalletUsageObj.DailyTrnCount = Req.DailyTrnCount;
                    WalletUsageObj.DailyTrnAmount = Req.DailyTrnAmount;
                    WalletUsageObj.MonthlyTrnCount = Req.MonthlyTrnCount;
                    WalletUsageObj.MonthlyTrnAmount = Req.MonthlyTrnAmount;
                    WalletUsageObj.WeeklyTrnCount = Req.WeeklyTrnCount;
                    WalletUsageObj.WeeklyTrnAmount = Req.WeeklyTrnAmount;
                    WalletUsageObj.YearlyTrnCount = Req.YearlyTrnCount;
                    WalletUsageObj.YearlyTrnAmount = Req.YearlyTrnAmount;
                    WalletUsageObj.LifeTimeTrnAmount = Req.LifeTimeTrnAmount;
                    WalletUsageObj.LifeTimeTrnCount = Req.LifeTimeTrnCount;

                    _WalletUsagePolicy.UpdateWithAuditLog(WalletUsageObj);
                    _controlPanelRepository.AddWPolicyAllowedDay(Req.DayNo, Req.Id, UserId, 2);
                    //update
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
                else
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                }
            }
            else
            {
                var ObjExist = _WalletUsagePolicy.GetSingle(i => i.WalletType == Req.WalletType);
                if (ObjExist == null)
                {
                    WalletUsagePolicy Obj = new WalletUsagePolicy();
                    Obj.CreatedDate = Helpers.UTC_To_IST();
                    Obj.CreatedBy = UserId;
                    Obj.UpdatedBy = null;
                    Obj.UpdatedDate = Helpers.UTC_To_IST();
                    Obj.Status = Req.Status;
                    Obj.MinAmount = Req.MinAmount;
                    Obj.MaxAmount = Req.MaxAmount;
                    Obj.WalletType = Req.WalletType;
                    Obj.PolicyName = Req.PolicyName;
                    Obj.HourlyTrnAmount = Req.HourlyTrnAmount;
                    Obj.HourlyTrnCount = Req.HourlyTrnCount;
                    Obj.AllowedIP = Req.AllowedIP;
                    Obj.AllowedLocation = Req.AllowedLocation;
                    Obj.AuthenticationType = Req.AuthenticationType;
                    Obj.StartTime = Req.StartTime;
                    Obj.EndTime = Req.EndTime;
                    Obj.DailyTrnCount = Req.DailyTrnCount;
                    Obj.DailyTrnAmount = Req.DailyTrnAmount;
                    Obj.MonthlyTrnCount = Req.MonthlyTrnCount;
                    Obj.MonthlyTrnAmount = Req.MonthlyTrnAmount;
                    Obj.WeeklyTrnCount = Req.WeeklyTrnCount;
                    Obj.WeeklyTrnAmount = Req.WeeklyTrnAmount;
                    Obj.YearlyTrnCount = Req.YearlyTrnCount;
                    Obj.YearlyTrnAmount = Req.YearlyTrnAmount;
                    Obj.LifeTimeTrnAmount = Req.LifeTimeTrnAmount;
                    Obj.LifeTimeTrnCount = Req.LifeTimeTrnCount;
                    //insert
                    var newObj = _WalletUsagePolicy.Add(Obj);
                    _controlPanelRepository.AddWPolicyAllowedDay(Req.DayNo, newObj.Id, UserId, 1);
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                }
                else
                {
                    if (ObjExist.Status == 9)
                    {
                        ObjExist.MinAmount = Req.MinAmount;
                        ObjExist.MaxAmount = Req.MaxAmount;
                        ObjExist.PolicyName = Req.PolicyName;
                        ObjExist.HourlyTrnAmount = Req.HourlyTrnAmount;
                        ObjExist.HourlyTrnCount = Req.HourlyTrnCount;
                        ObjExist.AllowedIP = Req.AllowedIP;
                        ObjExist.AllowedLocation = Req.AllowedLocation;
                        ObjExist.AuthenticationType = Req.AuthenticationType;
                        ObjExist.StartTime = Req.StartTime;
                        ObjExist.EndTime = Req.EndTime;
                        ObjExist.DailyTrnCount = Req.DailyTrnCount;
                        ObjExist.DailyTrnAmount = Req.DailyTrnAmount;
                        ObjExist.MonthlyTrnCount = Req.MonthlyTrnCount;
                        ObjExist.MonthlyTrnAmount = Req.MonthlyTrnAmount;
                        ObjExist.WeeklyTrnCount = Req.WeeklyTrnCount;
                        ObjExist.WeeklyTrnAmount = Req.WeeklyTrnAmount;
                        ObjExist.YearlyTrnCount = Req.YearlyTrnCount;
                        ObjExist.YearlyTrnAmount = Req.YearlyTrnAmount;
                        ObjExist.LifeTimeTrnAmount = Req.LifeTimeTrnAmount;
                        ObjExist.LifeTimeTrnCount = Req.LifeTimeTrnCount;
                        ObjExist.UpdatedBy = UserId;
                        ObjExist.UpdatedDate = Helpers.UTC_To_IST();
                        ObjExist.CreatedDate = Helpers.UTC_To_IST();
                        ObjExist.CreatedBy = UserId;
                        ObjExist.Status = Req.Status;
                        _WalletUsagePolicy.UpdateWithAuditLog(ObjExist);
                        _controlPanelRepository.AddWPolicyAllowedDay(Req.DayNo, ObjExist.Id, UserId, 2);

                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                    }
                    return new BizResponseClass { ErrorCode = enErrorCode.AlredyExist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Alredy_Exist };
                }
            }
        }

        //2018-12-14
        public BizResponseClass ChangeWalletUsagePolicyStatus(long Id, short Status, long UserId)
        {
            try
            {
                var _WalletUsageObj = _WalletUsagePolicy.GetSingle(i => i.Id == Id);
                if (_WalletUsageObj != null)
                {
                    //update
                    _WalletUsageObj.Status = Status;
                    _WalletUsageObj.UpdatedBy = UserId;
                    _WalletUsageObj.UpdatedDate = Helpers.UTC_To_IST();
                    _WalletUsagePolicy.UpdateWithAuditLog(_WalletUsageObj);
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
                return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }
        #endregion

        #region WTrnTypeMaster
        //vsoalnki 2018-11-28
        public BizResponseClass UpdateWTrnTypeStatus(long TrnTypeId, short Status, long UserId)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                var WtrnObj = _WTrnTypeMasterRepo.GetSingle(i => i.TrnTypeId == TrnTypeId);
                if (WtrnObj == null)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                WtrnObj.UpdatedBy = UserId;
                WtrnObj.UpdatedDate = Helpers.UTC_To_IST();
                WtrnObj.Status = Status;
                _WTrnTypeMasterRepo.UpdateWithAuditLog(WtrnObj);
                return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsoalnki 2018-11-28
        public ListTypeRes ListWalletTrnType()
        {
            ListTypeRes Resp = new ListTypeRes();
            try
            {
                var obj = _controlPanelRepository.ListWalletTrnType();
                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region BlockWalletTrnTypeMaster
        //vsoalnki 2018-11-28
        public BizResponseClass InsertBlockWalletTrnType(BlockWalletTrnTypeReq Req, long UserId)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                if (Req == null)
                {
                    Resp.ErrorCode = enErrorCode.InvalidInput;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InvalidInput;
                    return Resp;
                }
                var IsExist = _BlockWalletTrnTypeMasterRepository.GetSingle(i => i.TrnTypeID == Req.TrnTypeID && i.WalletTypeID == Req.WalletTypeID);
                if (IsExist == null)
                {
                    if (Req.Status == 0)
                    {
                        BlockWalletTrnTypeMaster Obj = new BlockWalletTrnTypeMaster();
                        Obj.CreatedDate = Helpers.UTC_To_IST();
                        Obj.CreatedBy = UserId;
                        Obj.UpdatedBy = null;
                        Obj.UpdatedDate = Helpers.UTC_To_IST();
                        Obj.Status = 1;
                        Obj.WalletTypeID = Req.WalletTypeID;
                        Obj.TrnTypeID = Req.TrnTypeID;
                        _BlockWalletTrnTypeMasterRepository.Add(Obj);
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                    }
                }
                else if (IsExist != null)
                {
                    if (IsExist.Status == 0)
                    {
                        IsExist.UpdatedBy = UserId;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        IsExist.Status = 1;
                        _BlockWalletTrnTypeMasterRepository.UpdateWithAuditLog(IsExist);
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                    }
                    else
                    {
                        IsExist.UpdatedBy = UserId;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        IsExist.Status = 0;
                        _BlockWalletTrnTypeMasterRepository.UpdateWithAuditLog(IsExist);
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                    }
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //vsoalnki 2018-11-28
        public ListTypeRes GetBlockWTypewiseTrnTypeList(long WalletType)
        {
            ListTypeRes Resp = new ListTypeRes();
            try
            {
                var obj = _controlPanelRepository.GetBlockWTypewiseTrnTypeList(WalletType);
                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }
        #endregion

        #region WalletType

        //vsoalnki 2018-11-28
        public ListTypeRes ListWalletType()
        {
            ListTypeRes Resp = new ListTypeRes();
            try
            {
                var obj = _controlPanelRepository.ListWalletType();
                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region AllowedChannels Method

        public async Task<BizResponseClass> AddAllowedChannels(AllowedChannelReq Request, long UserID)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                if (Request == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.InvalidInput;
                    Resp.ReturnMsg = EnResponseMessage.InvalidInput;
                    return Resp;
                }
                if (Request.ID == 0)
                {
                    var IsExist = await _AllowedChannelRepo.GetSingleAsync(item => item.ChannelName == Request.ChannelName);
                    if (IsExist == null)
                    {
                        AllowedChannels NewObj = new AllowedChannels();
                        NewObj.ChannelID = _controlPanelRepository.GetMaxPlusOneChannelID();
                        NewObj.ChannelName = Request.ChannelName;
                        NewObj.Status = Convert.ToInt16(Request.Status == null ? 1 : Request.Status);
                        NewObj.CreatedBy = UserID;
                        NewObj.CreatedDate = Helpers.UTC_To_IST();
                        await _AllowedChannelRepo.AddAsync(NewObj);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                        Resp.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        if (IsExist.Status == 9)
                        {
                            IsExist.Status = Convert.ToInt16(Request.Status == null ? 1 : Request.Status);
                            IsExist.UpdatedBy = UserID;
                            IsExist.UpdatedDate = Helpers.UTC_To_IST();
                            _AllowedChannelRepo.UpdateWithAuditLog(IsExist);

                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                            Resp.ErrorCode = enErrorCode.Success;

                        }
                        else
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.AlredyExist;
                            Resp.ErrorCode = enErrorCode.AlredyExist;
                        }
                    }
                }
                else
                {
                    var IsExist = await _AllowedChannelRepo.GetSingleAsync(item => item.ChannelID == Request.ID);
                    if (IsExist != null)
                    {
                        IsExist.ChannelName = Request.ChannelName;
                        IsExist.Status = Convert.ToInt16(Request.Status == null ? 1 : Request.Status);
                        IsExist.UpdatedBy = UserID;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                        Resp.ErrorCode = enErrorCode.Success;
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.NotFound;
                        Resp.ReturnMsg = EnResponseMessage.NotFound;
                    }
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddAllowedChannels", this.GetType().Name, ex);
                throw;
            }
        }

        public ListChannels GetChannels(long channelID, short? status)
        {
            ListChannels Resp = new ListChannels();
            try
            {
                Resp.Channels = _controlPanelRepository.GetChannelDetail(channelID, status);
                if (Resp.Channels.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public ListChannels ListChannels()
        {
            ListChannels Resp = new ListChannels();
            try
            {
                var obj = _controlPanelRepository.ListChannels();
                if (obj.Channels.Count > 0)
                {
                    Resp.Channels = obj.Channels;
                    Resp.TotalCount = obj.Channels.Count;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public ListProviderRes ListProviders()
        {
            ListProviderRes Resp = new ListProviderRes();
            try
            {
                var obj = _controlPanelRepository.ListProvider();
                if (obj.Count > 0)
                {
                    Resp.Providers = obj;
                    Resp.TotalCount = obj.Count;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public ListChannelwiseTrnCount ListChannelwiseTrnCnt()
        {
            ListChannelwiseTrnCount Resp = new ListChannelwiseTrnCount();
            try
            {
                var obj = _controlPanelRepository.ChannelwiseTranCount();
                if (obj.Count > 0)
                {
                    Resp.Counter = obj;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region Other Method

        public async Task<ListOrgDetail> GetOrgAllDetail()
        {
            ListOrgDetail Resp = new ListOrgDetail();
            OrgDetail info1 = _controlPanelRepository.GetOrgAllDetail();
            try
            {
                if (info1 == null)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                Resp.Data = info1;
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetOrgAllDetail", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListTypeWiseDetail> GetDetailTypeWise()
        {
            ListTypeWiseDetail Resp = new ListTypeWiseDetail();
            List<TypeWiseDetail> infoList = new List<TypeWiseDetail>();

            try
            {
                infoList = _controlPanelRepository.GetDetailTypeWise();
                if (infoList == null)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                Resp.WalletTypes = infoList;
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetDetailTypeWise", this.GetType().Name, ex);
                throw;
            }
        }

        public ListTypeWiseDetail GetDetailTypeWiseV1()
        {
            ListTypeWiseDetail Resp = new ListTypeWiseDetail();
            try
            {
                var infoList = _controlPanelRepository.GetDetailTypeWise();
                if (infoList != null)
                {
                    Resp.WalletTypes = infoList;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    return Resp;
                }
                Resp.ErrorCode = enErrorCode.NotFound;
                Resp.ReturnCode = enResponseCode.Fail;
                Resp.ReturnMsg = EnResponseMessage.NotFound;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetDetailTypeWiseV1", this.GetType().Name, ex);
                throw;
            }
        }
        #endregion


        #region TransactionPolicy
        //2018-12-12
        public ListTransactionPolicyRes ListTransactionPolicy()
        {
            ListTransactionPolicyRes Resp = new ListTransactionPolicyRes();
            try
            {
                var obj = _controlPanelRepository.ListTransactionPolicy();
                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-12
        public BizResponseClass InsertTransactionPolicy(AddTransactionPolicyReq Req, long UserId)
        {
            try
            {
                var trnType = _WTrnTypeMasterRepo.GetSingle(i => i.TrnTypeId == Req.TrnType);
                if (trnType == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidTrnType, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTrnType };
                }
                var obj1 = _TransactionPolicyCommonRepo.GetSingle(i => i.TrnType == Req.TrnType && i.RoleId == Req.RoleId && i.IsKYCEnable == Req.IsKYCEnable);
                if (Req == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidInput, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput };
                }
                if (obj1 != null)
                {
                    if (obj1.Status == 9)
                    {
                        obj1.CreatedDate = Helpers.UTC_To_IST();
                        obj1.CreatedBy = UserId;
                        obj1.MinAmount = Req.MinAmount;
                        obj1.MaxAmount = Req.MaxAmount;
                        obj1.AllowedIP = Req.AllowedIP;
                        obj1.AllowedLocation = Req.AllowedLocation;
                        obj1.AuthenticationType = Req.AuthenticationType;
                        obj1.StartTime = Req.StartTime;
                        obj1.EndTime = Req.EndTime;
                        obj1.DailyTrnCount = Req.DailyTrnCount;
                        obj1.DailyTrnAmount = Req.DailyTrnAmount;
                        obj1.MonthlyTrnCount = Req.MonthlyTrnCount;
                        obj1.MonthlyTrnAmount = Req.MonthlyTrnAmount;
                        obj1.WeeklyTrnCount = Req.WeeklyTrnCount;
                        obj1.WeeklyTrnAmount = Req.WeeklyTrnAmount;
                        obj1.YearlyTrnCount = Req.YearlyTrnCount;
                        obj1.YearlyTrnAmount = Req.YearlyTrnAmount;
                        obj1.AuthorityType = Req.AuthorityType;
                        obj1.AllowedUserType = Req.AllowedUserType;
                        obj1.Status = Convert.ToInt16(ServiceStatus.Active);
                        obj1.UpdatedBy = UserId;
                        obj1.UpdatedDate = Helpers.UTC_To_IST();
                        _TransactionPolicyCommonRepo.UpdateWithAuditLog(obj1);
                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                    }
                    else
                    {
                        return new BizResponseClass { ErrorCode = enErrorCode.DuplicateRecord, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.DuplicateRecord };
                    }
                }
                TransactionPolicy Obj = new TransactionPolicy();
                Obj.RoleId = Req.RoleId;
                Obj.CreatedDate = Helpers.UTC_To_IST();
                Obj.CreatedBy = UserId;
                Obj.UpdatedBy = null;
                Obj.UpdatedDate = Helpers.UTC_To_IST();
                Obj.Status = Req.Status;
                Obj.MinAmount = Req.MinAmount;
                Obj.MaxAmount = Req.MaxAmount;
                Obj.TrnType = Req.TrnType;
                Obj.AllowedIP = Req.AllowedIP;
                Obj.AllowedLocation = Req.AllowedLocation;
                Obj.AuthenticationType = Req.AuthenticationType;
                Obj.StartTime = Req.StartTime;
                Obj.EndTime = Req.EndTime;
                Obj.DailyTrnCount = Req.DailyTrnCount;
                Obj.DailyTrnAmount = Req.DailyTrnAmount;
                Obj.MonthlyTrnCount = Req.MonthlyTrnCount;
                Obj.MonthlyTrnAmount = Req.MonthlyTrnAmount;
                Obj.WeeklyTrnCount = Req.WeeklyTrnCount;
                Obj.WeeklyTrnAmount = Req.WeeklyTrnAmount;
                Obj.YearlyTrnCount = Req.YearlyTrnCount;
                Obj.YearlyTrnAmount = Req.YearlyTrnAmount;
                Obj.AuthorityType = Req.AuthorityType;
                Obj.AllowedUserType = Req.AllowedUserType;
                Obj.IsKYCEnable = Req.IsKYCEnable;
                //insert
                _TransactionPolicyCommonRepo.Add(Obj);

                return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-12
        public BizResponseClass UpdateTransactionPolicy(UpdateTransactionPolicyReq Req, long UserId, long TrnPolicyId)
        {
            try
            {
                var Obj = _TransactionPolicyCommonRepo.GetSingle(i => i.Id == TrnPolicyId);
                if (Req == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidInput, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput };
                }
                if (Obj == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                }
                Obj.UpdatedBy = UserId;
                Obj.UpdatedDate = Helpers.UTC_To_IST();
                Obj.Status = Req.Status;
                Obj.MinAmount = Req.MinAmount;
                Obj.MaxAmount = Req.MaxAmount;
                Obj.AllowedIP = Req.AllowedIP;
                Obj.AllowedLocation = Req.AllowedLocation;
                Obj.AuthenticationType = Req.AuthenticationType;
                Obj.StartTime = Req.StartTime;
                Obj.EndTime = Req.EndTime;
                Obj.DailyTrnCount = Req.DailyTrnCount;
                Obj.DailyTrnAmount = Req.DailyTrnAmount;
                Obj.MonthlyTrnCount = Req.MonthlyTrnCount;
                Obj.MonthlyTrnAmount = Req.MonthlyTrnAmount;
                Obj.WeeklyTrnCount = Req.WeeklyTrnCount;
                Obj.WeeklyTrnAmount = Req.WeeklyTrnAmount;
                Obj.YearlyTrnCount = Req.YearlyTrnCount;
                Obj.YearlyTrnAmount = Req.YearlyTrnAmount;
                Obj.AuthorityType = Req.AuthorityType;
                Obj.AllowedUserType = Req.AllowedUserType;
                //update
                _TransactionPolicyCommonRepo.UpdateWithAuditLog(Obj);

                return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-12
        public BizResponseClass UpdateTransactionPolicyStatus(short Status, long UserId, long TrnPolicyId)
        {
            try
            {
                var Obj = _TransactionPolicyCommonRepo.GetSingle(i => i.Id == TrnPolicyId);
                if (Obj == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                }
                Obj.UpdatedBy = UserId;
                Obj.UpdatedDate = Helpers.UTC_To_IST();
                Obj.Status = Status;
                //update
                _TransactionPolicyCommonRepo.UpdateWithAuditLog(Obj);

                return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region UserWalletAllowTrnType
        //2018-12-13
        public ListUserWalletBlockTrnType ListUserWalletBlockTrnType(string WalletId, long? TrnTypeId)
        {
            ListUserWalletBlockTrnType Resp = new ListUserWalletBlockTrnType();
            try
            {
                var obj = _controlPanelRepository.ListUserWalletBlockTrnType(WalletId, TrnTypeId);
                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-13
        public BizResponseClass InsertUpdateUserWalletBlockTrnType(InsertUpdateUserWalletBlockTrnTypeReq Req, long UserId)
        {
            UserWalletBlockTrnTypeMaster Obj = new UserWalletBlockTrnTypeMaster();
            try
            {
                if (Req == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidInput, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput };
                }
                var walletObj = _WalletMaster.GetSingle(i => i.AccWalletID == Req.WalletId);
                if (walletObj == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidWallet, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWallet };
                }
                var WtrntypeObj = _WTrnTypeMasterRepo.GetSingle(i => i.TrnTypeId == Req.WTrnTypeMasterID);
                if (WtrntypeObj == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidTrnType, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTrnType };
                }
                if (Req.Status == 0)
                {
                    Req.Status = Convert.ToInt16(1);
                }
                else
                {
                    Req.Status = Convert.ToInt16(0);
                }
                if (Req.Id != 0)
                {
                    var trnType = _UserWalletBlockTrnTypeMaster.GetSingle(i => i.Id == Req.Id && i.WalletID == walletObj.Id && i.WTrnTypeMasterID == Req.WTrnTypeMasterID);
                    if (trnType != null)
                    {
                        //update
                        trnType.Status = Req.Status;
                        trnType.Remarks = Req.Remarks;
                        trnType.UpdatedBy = UserId;
                        trnType.UpdatedDate = Helpers.UTC_To_IST();
                        _UserWalletBlockTrnTypeMaster.UpdateWithAuditLog(trnType);
                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                    }
                    else
                    {
                        return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                    }
                }
                else
                {
                    var trnType = _UserWalletBlockTrnTypeMaster.GetSingle(i => i.WalletID == walletObj.Id && i.WTrnTypeMasterID == Req.WTrnTypeMasterID);
                    if (trnType == null)
                    {
                        Obj.Status = Req.Status;
                        Obj.UpdatedBy = null;
                        Obj.UpdatedDate = Helpers.UTC_To_IST();
                        Obj.CreatedBy = UserId;
                        Obj.CreatedDate = Helpers.UTC_To_IST();
                        Obj.WalletID = walletObj.Id;
                        Obj.Remarks = Req.Remarks;
                        Obj.WTrnTypeMasterID = Req.WTrnTypeMasterID;
                        //insert
                        _UserWalletBlockTrnTypeMaster.Add(Obj);
                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                    }
                    else
                    {
                        if (trnType.Status == 9)
                        {
                            trnType.Status = 1;
                            trnType.UpdatedBy = UserId;
                            trnType.Remarks = Req.Remarks;
                            trnType.UpdatedDate = Helpers.UTC_To_IST();
                            _UserWalletBlockTrnTypeMaster.UpdateWithAuditLog(trnType);
                            return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                        }
                        return new BizResponseClass { ErrorCode = enErrorCode.AlredyExist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Alredy_Exist };
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-13
        public BizResponseClass ChangeUserWalletBlockTrnTypeStatus(long Id, short Status, long UserId)
        {
            try
            {
                var trnType = _UserWalletBlockTrnTypeMaster.GetSingle(i => i.Id == Id);
                if (trnType != null)
                {
                    //update
                    trnType.Status = (Status == 0 ? Convert.ToInt16(1) : Status == 1 ? Convert.ToInt16(0) : Convert.ToInt16(9));
                    trnType.UpdatedBy = UserId;
                    trnType.UpdatedDate = Helpers.UTC_To_IST();
                    _UserWalletBlockTrnTypeMaster.UpdateWithAuditLog(trnType);
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
                return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region WalletAuthorizeUserMaster

        public ListWalletAuthorizeUserRes ListWalletAuthorizeUser(string WalletId)
        {
            ListWalletAuthorizeUserRes Resp = new ListWalletAuthorizeUserRes();
            try
            {
                var obj = _controlPanelRepository.ListWalletAuthorizeUser(WalletId);
                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }


        #endregion

        #region Log Listing Methods

        public ListUserActivityLoging GetUserActivities(long? userID, DateTime? fromDate, DateTime? toDate)
        {
            ListUserActivityLoging Resp = new ListUserActivityLoging();
            try
            {
                var obj = _controlPanelRepository.ListUserActivityData(userID, fromDate, toDate);
                Resp.Details = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region TranTypeWiseReport

        public ListBlockTrnTypewiseReport GetBlockedTrnTypeWiseWalletDetail(enWalletTrnType type, int? PageNo, int? PageSize)
        {
            try
            {
                ListBlockTrnTypewiseReport Resp = new ListBlockTrnTypewiseReport();
                var data = _controlPanelRepository.GetBlockedTrnTypeWiseWalletData(type, PageNo, PageSize);
                Resp = data;
                if (data.Details.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region AllowTrnTypeRoleWise

        //2018-12-22
        public async Task<ListAllowTrnTypeRoleWise> ListAllowTrnTypeRoleWise(long? RoleId, long? TrnTypeId, short? Status)
        {
            ListAllowTrnTypeRoleWise Resp = new ListAllowTrnTypeRoleWise();
            try
            {
                var obj = await _controlPanelRepository.ListAllowTrnTypeRoleWise(RoleId, TrnTypeId, Status);
                _walletConfiguration.GetAllowTrnTypeRoleWise();
                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListAllowTrnTypeRoleWise", this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-22
        public async Task<BizResponseClass> InserUpdateAllowTrnTypeRole(InserUpdateAllowTrnTypeRoleReq Request, long UserId)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();

                var roleObj = _UserRoleMaster.GetSingle(i => i.Id == Request.RoleId);
                var trnTypeObj = _WTrnTypeMasterRepo.GetSingle(i => i.TrnTypeId == Request.TrnTypeId);
                if (roleObj == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidRole, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidRole };
                }
                if (trnTypeObj == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidTrnType, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidTrnType };
                }
                if (Request.Id == 0)
                {
                    var IsExist = _AllowTrnTypeRoleWise.GetSingle(item => item.RoleId == Request.RoleId && item.TrnTypeId == Request.TrnTypeId);
                    if (IsExist == null)
                    {
                        //insert
                        AllowTrnTypeRoleWise Obj = new AllowTrnTypeRoleWise();
                        Obj.RoleId = Request.RoleId;
                        Obj.TrnTypeId = Request.TrnTypeId;
                        Obj.Status = Request.Status;
                        Obj.CreatedBy = UserId;
                        Obj.CreatedDate = Helpers.UTC_To_IST();
                        Obj.UpdatedDate = Helpers.UTC_To_IST();
                        _AllowTrnTypeRoleWise.Add(Obj);
                        _walletConfiguration.UpdateAllowTrnTypeRoleWiseList();
                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                    }
                    else if (IsExist.Status == 9)
                    {
                        //update
                        IsExist.Status = Request.Status;
                        IsExist.UpdatedBy = UserId;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        _AllowTrnTypeRoleWise.UpdateWithAuditLog(IsExist);
                        _walletConfiguration.UpdateAllowTrnTypeRoleWiseList();
                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                    }
                    else
                    {
                        //exist
                        return new BizResponseClass { ErrorCode = enErrorCode.Alredy_Exist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Alredy_Exist };
                    }
                }
                else
                {
                    var allowTrnTypeRoleWiseIsExistObj = _AllowTrnTypeRoleWise.GetSingle(i => i.Id == Request.Id);
                    if (allowTrnTypeRoleWiseIsExistObj == null)
                    {
                        return new BizResponseClass { ErrorCode = enErrorCode.Alredy_Exist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Alredy_Exist };
                    }
                    //update
                    allowTrnTypeRoleWiseIsExistObj.Status = Request.Status;
                    allowTrnTypeRoleWiseIsExistObj.UpdatedBy = UserId;
                    allowTrnTypeRoleWiseIsExistObj.UpdatedDate = Helpers.UTC_To_IST();
                    _AllowTrnTypeRoleWise.UpdateWithAuditLog(allowTrnTypeRoleWiseIsExistObj);
                    _walletConfiguration.UpdateAllowTrnTypeRoleWiseList();
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InserUpdateAllowTrnTypeRole", this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-22
        public async Task<BizResponseClass> ChangeAllowTrnTypeRoleStatus(short Status, long UserId, long Id)
        {
            try
            {
                var ISExist = await _AllowTrnTypeRoleWise.GetSingleAsync(i => i.Id == Id);
                if (ISExist != null)
                {
                    //update
                    ISExist.Status = Status;
                    ISExist.UpdatedBy = UserId;
                    ISExist.UpdatedDate = Helpers.UTC_To_IST();
                    _AllowTrnTypeRoleWise.UpdateWithAuditLog(ISExist);
                    _walletConfiguration.UpdateAllowTrnTypeRoleWiseList();
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
                return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ChangeAllowTrnTypeRoleStatus", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region Staking Policy
        //public async Task<BizResponseClass> AddStakingPolicy(StakingPolicyReq Request, long UserID)
        //{
        //    try
        //    {
        //        BizResponseClass Resp = new BizResponseClass();
        //        int CheckRangeCondition = 0;
        //        bool Fix_FixFlag = false, Fix_RangeFlag = false;
        //        bool Charge_FixFlag = false, Charge_RangeFlag = false;
        //        #region Request Validations
        //        var masterObj = _StakingPolicyMasterCommonRepo.GetSingle(i => i.Id == Request.StakingPolicyID);
        //        if (Request.DurationMonth == 0 && Request.DurationWeek == 0)
        //        {
        //            Resp.ErrorCode = enErrorCode.InvalidDuration;
        //            Resp.ReturnCode = enResponseCode.Fail;
        //            Resp.ReturnMsg = EnResponseMessage.InvalidDuration;
        //            return Resp;
        //        }
        //        if (masterObj.StakingType == Convert.ToInt16(EnStakingType.FixedDeposit))
        //        {
        //            //FixStakeFlag = true;
        //            if (masterObj.SlabType == Convert.ToInt16(EnStakingSlabType.Fixed))
        //            {
        //                Fix_FixFlag = true;
        //                if (Request.Amount == null || Request.Amount <= 0)
        //                {
        //                    Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
        //                    return Resp;
        //                }
        //            }
        //            else
        //            {
        //                Fix_RangeFlag = true;
        //                CheckRangeCondition = 1;
        //                if ((Request.MinAmount == null || Request.MinAmount < 0) && (Request.MaxAmount == null || Request.MaxAmount <= 0))
        //                {
        //                    Resp.ErrorCode = enErrorCode.InvalidMinOrMaxStakingAmount;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
        //                    return Resp;
        //                }
        //            }
        //            if (Request.InterestType == null && (Request.InterestValue == null || Request.InterestValue <= 0))
        //            {
        //                Resp.ErrorCode = enErrorCode.InterestTypeRequired;
        //                Resp.ReturnCode = enResponseCode.Fail;
        //                Resp.ReturnMsg = EnResponseMessage.InvalidValue;
        //                return Resp;
        //            }
        //            if (Request.InterestType == EnInterestType.Percentage && Request.InterestValue > 100)
        //            {
        //                Resp.ErrorCode = enErrorCode.InvalidInterestValue;
        //                Resp.ReturnCode = enResponseCode.Fail;
        //                Resp.ReturnMsg = EnResponseMessage.InvalidValue;
        //                return Resp;
        //            }
        //            if (Request.MaturityCurrency == null || Request.MaturityCurrency <= 0)
        //            {
        //                Resp.ErrorCode = enErrorCode.MaturityCurrencyRequired;
        //                Resp.ReturnCode = enResponseCode.Fail;
        //                Resp.ReturnMsg = EnResponseMessage.RequiredParameterMissing;
        //                return Resp;
        //            }
        //        }
        //        else
        //        {
        //            if (masterObj.SlabType == Convert.ToInt16(EnStakingSlabType.Fixed))
        //            {
        //                Charge_FixFlag = true;
        //                if (Request.Amount == null || Request.Amount <= 0)
        //                {
        //                    Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
        //                    return Resp;
        //                }
        //            }
        //            else
        //            {
        //                Charge_RangeFlag = true;
        //                CheckRangeCondition = 1;
        //                if ((Request.MinAmount == null || Request.MinAmount < 0) && (Request.MaxAmount == null || Request.MaxAmount <= 0))
        //                {
        //                    Resp.ErrorCode = enErrorCode.InvalidMinOrMaxStakingAmount;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
        //                    return Resp;
        //                }
        //            }
        //            if (Request.MakerCharges == null || Request.MakerCharges <= 0)
        //            {
        //                Resp.ErrorCode = enErrorCode.MakerChargeRequired;
        //                Resp.ReturnCode = enResponseCode.Fail;
        //                Resp.ReturnMsg = EnResponseMessage.RequiredParameterMissing;
        //                return Resp;
        //            }
        //            if (Request.TakerCharges == null || Request.TakerCharges <= 0)
        //            {
        //                Resp.ErrorCode = enErrorCode.TakerChargeRequired;
        //                Resp.ReturnCode = enResponseCode.Fail;
        //                Resp.ReturnMsg = EnResponseMessage.RequiredParameterMissing;
        //                return Resp;
        //            }
        //        }

        //        #endregion
        //        if (CheckRangeCondition > 0)
        //        {
        //            var MinMaxRange = _controlPanelRepository.GetRange(Convert.ToDecimal(Request.MaxAmount), Convert.ToDecimal(Request.MinAmount), Request.StakingPolicyID);
        //            if (Fix_RangeFlag == true || Charge_RangeFlag == true)
        //            {
        //                if (MinMaxRange == 0)//for insert into db=1 and 0=give error
        //                {
        //                    Resp.ErrorCode = enErrorCode.StakingRangeValidationFail;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidValue;
        //                    return Resp;
        //                }
        //            }
        //            if (Fix_FixFlag == true || Charge_FixFlag == true)
        //            {
        //                var amountRange = _controlPanelRepository.GetRange(Convert.ToDecimal(Request.Amount), Convert.ToDecimal(Request.Amount), Request.StakingPolicyID);
        //                if (amountRange == 0)//for insert into db=1 and 0=give error
        //                {
        //                    Resp.ErrorCode = enErrorCode.StakingRangeValidationFail;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidValue;
        //                    return Resp;
        //                }
        //            }
        //        }
        //        StakingPolicyDetail NewDetailObj = new StakingPolicyDetail();
        //        NewDetailObj.CreatedDate = Helpers.UTC_To_IST();
        //        NewDetailObj.CreatedBy = UserID;
        //        NewDetailObj.Status = Convert.ToInt16(ServiceStatus.Active);
        //        NewDetailObj.StakingPolicyID = Request.StakingPolicyID;
        //        NewDetailObj.StakingDurationWeek = Convert.ToInt16(Request.DurationWeek);
        //        NewDetailObj.StakingDurationMonth = Convert.ToInt16(Request.DurationMonth);
        //        NewDetailObj.EnableAutoUnstaking = Request.AutoUnstakingEnable;
        //        NewDetailObj.RenewUnstakingEnable = Convert.ToInt16(Request.RenewUnstakingEnable);
        //        NewDetailObj.RenewUnstakingPeriod = Convert.ToInt16(Request.RenewUnstakingPeriod);

        //        if (Fix_FixFlag)
        //        {
        //            NewDetailObj.MinAmount = Convert.ToDecimal(Request.Amount);
        //            NewDetailObj.MaxAmount = Convert.ToDecimal(Request.Amount);
        //            NewDetailObj.InterestType = Convert.ToInt16(Request.InterestType);
        //            NewDetailObj.InterestValue = Convert.ToInt16(Request.InterestValue);
        //            NewDetailObj.InterestWalletTypeID = Convert.ToInt64(Request.MaturityCurrency);
        //            NewDetailObj.MakerCharges = 0;
        //            NewDetailObj.TakerCharges = 0;
        //            NewDetailObj.EnableStakingBeforeMaturity = Convert.ToInt16(Request.EnableStakingBeforeMaturity);
        //            NewDetailObj.EnableStakingBeforeMaturityCharge = Convert.ToDecimal(Request.EnableStakingBeforeMaturityCharge);
        //        }
        //        if (Fix_RangeFlag)
        //        {
        //            NewDetailObj.MinAmount = Convert.ToDecimal(Request.MinAmount);
        //            NewDetailObj.MaxAmount = Convert.ToDecimal(Request.MaxAmount);
        //            NewDetailObj.InterestType = Convert.ToInt16(Request.InterestType);
        //            NewDetailObj.InterestValue = Convert.ToInt16(Request.InterestValue);
        //            NewDetailObj.InterestWalletTypeID = Convert.ToInt64(Request.MaturityCurrency);
        //            NewDetailObj.MakerCharges = 0;
        //            NewDetailObj.TakerCharges = 0;
        //            NewDetailObj.EnableStakingBeforeMaturity = 0;
        //            NewDetailObj.EnableStakingBeforeMaturityCharge = Convert.ToDecimal(Request.EnableStakingBeforeMaturityCharge);
        //        }
        //        if (Charge_FixFlag)
        //        {
        //            NewDetailObj.MinAmount = Convert.ToDecimal(Request.Amount);
        //            NewDetailObj.MaxAmount = Convert.ToDecimal(Request.Amount);
        //            NewDetailObj.InterestType = 0;
        //            NewDetailObj.InterestValue = 0;
        //            NewDetailObj.InterestWalletTypeID = 0;
        //            NewDetailObj.MakerCharges = Convert.ToDecimal(Request.MakerCharges);
        //            NewDetailObj.TakerCharges = Convert.ToDecimal(Request.TakerCharges);
        //            NewDetailObj.EnableStakingBeforeMaturity = Convert.ToInt16(Request.EnableStakingBeforeMaturity);
        //            NewDetailObj.EnableStakingBeforeMaturityCharge = Convert.ToDecimal(Request.EnableStakingBeforeMaturityCharge);
        //        }
        //        if (Charge_RangeFlag)
        //        {
        //            NewDetailObj.MinAmount = Convert.ToDecimal(Request.MinAmount);
        //            NewDetailObj.MaxAmount = Convert.ToDecimal(Request.MaxAmount);
        //            NewDetailObj.InterestType = 0;
        //            NewDetailObj.InterestValue = 0;
        //            NewDetailObj.InterestWalletTypeID = 0;
        //            NewDetailObj.MakerCharges = Convert.ToDecimal(Request.MakerCharges);
        //            NewDetailObj.TakerCharges = Convert.ToDecimal(Request.TakerCharges);
        //            NewDetailObj.EnableStakingBeforeMaturity = Convert.ToInt16(Request.EnableStakingBeforeMaturity);
        //            NewDetailObj.EnableStakingBeforeMaturityCharge = Convert.ToDecimal(Request.EnableStakingBeforeMaturityCharge);
        //        }

        //        await _StakingPolicyDetailCommonRepo.AddAsync(NewDetailObj);
        //        Resp.ErrorCode = enErrorCode.Success;
        //        Resp.ReturnCode = enResponseCode.Success;
        //        Resp.ReturnMsg = EnResponseMessage.RecordAdded;

        //        return Resp;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog("AddStakingPolicy", "ControlPanelServices", ex);
        //        throw;
        //    }
        //}

        public async Task<BizResponseClass> AddStakingPolicy(StakingPolicyReq Request, long UserID)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                int CheckRangeCondition = 0;
                bool Fix_FixFlag = false, Fix_RangeFlag = false;
                bool Charge_FixFlag = false, Charge_RangeFlag = false;
                #region Request Validations
                var masterObj = _StakingPolicyMasterCommonRepo.GetSingle(i => i.Id == Request.StakingPolicyID);
                if (Request.DurationMonth == 0 && Request.DurationWeek == 0)
                {
                    Resp.ErrorCode = enErrorCode.InvalidDuration;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InvalidDuration;
                    return Resp;
                }
                if (masterObj.StakingType == Convert.ToInt16(EnStakingType.FixedDeposit))
                {
                    //FixStakeFlag = true;
                    if (masterObj.SlabType == Convert.ToInt16(EnStakingSlabType.Fixed))
                    {
                        Fix_FixFlag = true;
                        if (Request.Amount == null || Request.Amount <= 0)
                        {
                            Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                            return Resp;
                        }
                    }
                    else
                    {
                        Fix_RangeFlag = true;
                        CheckRangeCondition = 1;
                        if ((Request.MinAmount == null || Request.MinAmount < 0) && (Request.MaxAmount == null || Request.MaxAmount <= 0))
                        {
                            Resp.ErrorCode = enErrorCode.InvalidMinOrMaxStakingAmount;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                            return Resp;
                        }
                    }
                    if (Request.InterestType == null && (Request.InterestValue == null || Request.InterestValue <= 0))
                    {
                        Resp.ErrorCode = enErrorCode.InterestTypeRequired;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidValue;
                        return Resp;
                    }
                    if (Request.InterestType == EnInterestType.Percentage && Request.InterestValue > 100)
                    {
                        Resp.ErrorCode = enErrorCode.InvalidInterestValue;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidValue;
                        return Resp;
                    }
                    if (Request.MaturityCurrency == null || Request.MaturityCurrency <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.MaturityCurrencyRequired;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.RequiredParameterMissing;
                        return Resp;
                    }
                }
                else
                {
                    if (masterObj.SlabType == Convert.ToInt16(EnStakingSlabType.Fixed))
                    {
                        Charge_FixFlag = true;
                        if (Request.Amount == null || Request.Amount <= 0)
                        {
                            Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                            return Resp;
                        }
                    }
                    else
                    {
                        Charge_RangeFlag = true;
                        CheckRangeCondition = 1;
                        if ((Request.MinAmount == null || Request.MinAmount < 0) && (Request.MaxAmount == null || Request.MaxAmount <= 0))
                        {
                            Resp.ErrorCode = enErrorCode.InvalidMinOrMaxStakingAmount;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                            return Resp;
                        }
                    }
                    if (Request.MakerCharges == null || Request.MakerCharges <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.MakerChargeRequired;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.RequiredParameterMissing;
                        return Resp;
                    }
                    if (Request.TakerCharges == null || Request.TakerCharges <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.TakerChargeRequired;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.RequiredParameterMissing;
                        return Resp;
                    }
                }

                #endregion
                if (CheckRangeCondition > 0)
                {
                    var MinMaxRange = _controlPanelRepository.GetRange(Convert.ToDecimal(Request.MaxAmount), Convert.ToDecimal(Request.MinAmount), Request.StakingPolicyID, Convert.ToInt16(Request.InterestType), Convert.ToDecimal(Request.InterestValue), Request.DurationMonth, Request.DurationWeek, 0);
                    if (Fix_RangeFlag == true || Charge_RangeFlag == true)
                    {
                        if (MinMaxRange == 0)//for insert into db=1 and 0=give error
                        {
                            Resp.ErrorCode = enErrorCode.StakingRangeValidationFail;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidValue;
                            return Resp;
                        }
                    }
                    if (Fix_FixFlag == true || Charge_FixFlag == true)
                    {
                        var amountRange = _controlPanelRepository.GetRange(Convert.ToDecimal(Request.Amount), Convert.ToDecimal(Request.Amount), Request.StakingPolicyID, Convert.ToInt16(Request.InterestType), Convert.ToDecimal(Request.InterestValue), Request.DurationMonth, Request.DurationWeek, 0);
                        if (amountRange == 0)//for insert into db=1 and 0=give error
                        {
                            Resp.ErrorCode = enErrorCode.StakingRangeValidationFail;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidValue;
                            return Resp;
                        }
                    }
                }
                StakingPolicyDetail NewDetailObj = new StakingPolicyDetail
                {
                    CreatedDate = Helpers.UTC_To_IST(),
                    CreatedBy = UserID,
                    Status = Convert.ToInt16(Request.Status == null ? 1 : Request.Status),
                    StakingPolicyID = Request.StakingPolicyID,
                    StakingDurationWeek = Convert.ToInt16(Request.DurationWeek),
                    StakingDurationMonth = Convert.ToInt16(Request.DurationMonth),
                    EnableAutoUnstaking = Request.AutoUnstakingEnable,
                    RenewUnstakingEnable = Convert.ToInt16(Request.RenewUnstakingEnable),
                    RenewUnstakingPeriod = Convert.ToInt16(Request.RenewUnstakingPeriod)
                };

                if (Fix_FixFlag)
                {
                    NewDetailObj.MinAmount = Convert.ToDecimal(Request.Amount);
                    NewDetailObj.MaxAmount = Convert.ToDecimal(Request.Amount);
                    NewDetailObj.InterestType = Convert.ToInt16(Request.InterestType);
                    NewDetailObj.InterestValue = Convert.ToInt16(Request.InterestValue);
                    NewDetailObj.InterestWalletTypeID = Convert.ToInt64(Request.MaturityCurrency);
                    NewDetailObj.MakerCharges = 0;
                    NewDetailObj.TakerCharges = 0;
                    NewDetailObj.EnableStakingBeforeMaturity = Convert.ToInt16(Request.EnableStakingBeforeMaturity);
                    NewDetailObj.EnableStakingBeforeMaturityCharge = Convert.ToDecimal(Request.EnableStakingBeforeMaturityCharge);
                }
                if (Fix_RangeFlag)
                {
                    NewDetailObj.MinAmount = Convert.ToDecimal(Request.MinAmount);
                    NewDetailObj.MaxAmount = Convert.ToDecimal(Request.MaxAmount);
                    NewDetailObj.InterestType = Convert.ToInt16(Request.InterestType);
                    NewDetailObj.InterestValue = Convert.ToInt16(Request.InterestValue);
                    NewDetailObj.InterestWalletTypeID = Convert.ToInt64(Request.MaturityCurrency);
                    NewDetailObj.MakerCharges = 0;
                    NewDetailObj.TakerCharges = 0;
                    NewDetailObj.EnableStakingBeforeMaturity = Convert.ToInt16(Request.EnableStakingBeforeMaturity == null ? 0 : Request.EnableStakingBeforeMaturity);
                    NewDetailObj.EnableStakingBeforeMaturityCharge = Convert.ToDecimal(Request.EnableStakingBeforeMaturityCharge == null ? 0 : Request.EnableStakingBeforeMaturityCharge);
                }
                if (Charge_FixFlag)
                {
                    NewDetailObj.MinAmount = Convert.ToDecimal(Request.Amount);
                    NewDetailObj.MaxAmount = Convert.ToDecimal(Request.Amount);
                    NewDetailObj.InterestType = 0;
                    NewDetailObj.InterestValue = 0;
                    NewDetailObj.InterestWalletTypeID = 0;
                    NewDetailObj.MakerCharges = Convert.ToDecimal(Request.MakerCharges);
                    NewDetailObj.TakerCharges = Convert.ToDecimal(Request.TakerCharges);
                    NewDetailObj.EnableStakingBeforeMaturity = Convert.ToInt16(Request.EnableStakingBeforeMaturity);
                    NewDetailObj.EnableStakingBeforeMaturityCharge = Convert.ToDecimal(Request.EnableStakingBeforeMaturityCharge);
                }
                if (Charge_RangeFlag)
                {
                    NewDetailObj.MinAmount = Convert.ToDecimal(Request.MinAmount);
                    NewDetailObj.MaxAmount = Convert.ToDecimal(Request.MaxAmount);
                    NewDetailObj.InterestType = 0;
                    NewDetailObj.InterestValue = 0;
                    NewDetailObj.InterestWalletTypeID = 0;
                    NewDetailObj.MakerCharges = Convert.ToDecimal(Request.MakerCharges);
                    NewDetailObj.TakerCharges = Convert.ToDecimal(Request.TakerCharges);
                    NewDetailObj.EnableStakingBeforeMaturity = Convert.ToInt16(Request.EnableStakingBeforeMaturity == null ? 0 : Request.EnableStakingBeforeMaturity);
                    NewDetailObj.EnableStakingBeforeMaturityCharge = Convert.ToDecimal(Request.EnableStakingBeforeMaturityCharge == null ? 0 : Request.EnableStakingBeforeMaturityCharge);
                }

                await _StakingPolicyDetailCommonRepo.AddAsync(NewDetailObj);
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordAdded;

                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddStakingPolicy", "ControlPanelServices", ex);
                throw;
            }
        }

        //public async Task<BizResponseClass> UpdateStakingPolicy(long PolicyDetailId, UpdateStakingDetailReq Request, long UserID)
        //{
        //    try
        //    {
        //        BizResponseClass Resp = new BizResponseClass();
        //        int flag = -1;
        //        int StakingType = -1;
        //        StakingPolicyDetail IsExist = await _StakingPolicyDetailCommonRepo.GetSingleAsync(item => item.Id == PolicyDetailId && item.Status == 1);
        //        if (IsExist == null)
        //        {
        //            Resp.ReturnCode = enResponseCode.Fail;
        //            Resp.ReturnMsg = EnResponseMessage.NotFound;
        //            Resp.ErrorCode = enErrorCode.NoDataFound;
        //            return Resp;
        //        }
        //        var Masterdata = await _StakingPolicyMasterCommonRepo.GetSingleAsync(item => item.Id == IsExist.StakingPolicyID && item.Status == 1);
        //        if (Masterdata == null)
        //        {
        //            Resp.ErrorCode = enErrorCode.MasterDataNotFound;
        //            Resp.ReturnCode = enResponseCode.Fail;
        //            Resp.ReturnMsg = EnResponseMessage.MasterDataNotFound;
        //            return Resp;
        //        }
        //        if (Request.DurationMonth == 0 && Request.DurationWeek == 0)
        //        {
        //            Resp.ReturnCode = enResponseCode.Fail;
        //            Resp.ReturnMsg = EnResponseMessage.InvalidTimeDuration;
        //            Resp.ErrorCode = enErrorCode.InvalidTimeDuration;
        //            return Resp;
        //        }

        //        if (Masterdata.StakingType == 1)
        //        {
        //            StakingType = 1;
        //            if (Masterdata.SlabType == 1)
        //            {
        //                flag = 1;
        //                if (Request.Amount == null || Request.Amount == 0)
        //                {
        //                    Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
        //                    return Resp;
        //                }
        //                if (Request.EnableStakingBeforeMaturity == null)
        //                {
        //                    Resp.ErrorCode = enErrorCode.EnableStakingBeforeMaturityRequired;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.StakingBeforeMaturityRequired;
        //                    return Resp;
        //                }
        //                if (Request.EnableStakingBeforeMaturityCharge == null)
        //                {
        //                    Resp.ErrorCode = enErrorCode.EnableStakingBeforeMaturityChargeRequired;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.StakingBeforeMaturityChargeRequired;
        //                    return Resp;
        //                }
        //            }
        //            else
        //            {
        //                flag = 0;
        //                if ((Request.MinAmount == null || Request.MinAmount < 0) && (Request.MaxAmount == null || Request.MaxAmount <= 0))
        //                {
        //                    Resp.ErrorCode = enErrorCode.InvalidMinOrMaxStakingAmount;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
        //                    return Resp;
        //                }
        //                if (Request.MinAmount > Request.MaxAmount)
        //                {
        //                    Resp.ErrorCode = enErrorCode.InvalidMinAmount;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidMinValue;
        //                    return Resp;
        //                }
        //                if (Request.MinAmount == Request.MaxAmount)
        //                {
        //                    Resp.ErrorCode = enErrorCode.InvalidMinAndMaxValue;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidMinAndMaxValue;
        //                    return Resp;
        //                }
        //                var amountRange = _controlPanelRepository.GetRange(Convert.ToDecimal(Request.Amount), Convert.ToDecimal(Request.Amount), IsExist.StakingPolicyID);
        //                if (amountRange == 0)//for insert into db=1 and 0=give error
        //                {
        //                    Resp.ErrorCode = enErrorCode.StakingRangeValidationFail;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidValue;
        //                    return Resp;
        //                }
        //            }
        //            if (Request.InterestValue <= 0)
        //            {
        //                Resp.ErrorCode = enErrorCode.InterestTypeRequired;
        //                Resp.ReturnCode = enResponseCode.Fail;
        //                Resp.ReturnMsg = EnResponseMessage.InvalidValue;
        //                return Resp;
        //            }
        //            if (Request.InterestType != null && Request.InterestValue != null)
        //            {
        //                if (Request.InterestType == EnInterestType.Percentage && Request.InterestValue > 100)
        //                {
        //                    Resp.ErrorCode = enErrorCode.InvalidInterestValue;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidValue;
        //                    return Resp;
        //                }
        //            }
        //            else
        //            {
        //                Resp.ErrorCode = enErrorCode.InvalidInterestValue;
        //                Resp.ReturnCode = enResponseCode.Fail;
        //                Resp.ReturnMsg = EnResponseMessage.InvalidValue;
        //                return Resp;
        //            }
        //            if (Request.MaturityCurrency <= 0)
        //            {
        //                Resp.ErrorCode = enErrorCode.MaturityCurrencyRequired;
        //                Resp.ReturnCode = enResponseCode.Fail;
        //                Resp.ReturnMsg = EnResponseMessage.RequiredParameterMissing;
        //                return Resp;
        //            }
        //        }
        //        if (Masterdata.StakingType == 2)
        //        {
        //            StakingType = 2;
        //            if (Request.MakerCharges == null || Request.MakerCharges <= 0)
        //            {
        //                Resp.ErrorCode = enErrorCode.MakerChargeRequired;
        //                Resp.ReturnCode = enResponseCode.Fail;
        //                Resp.ReturnMsg = EnResponseMessage.RequiredParameterMissing;
        //                return Resp;
        //            }
        //            if (Request.TakerCharges == null || Request.TakerCharges <= 0)
        //            {
        //                Resp.ErrorCode = enErrorCode.TakerChargeRequired;
        //                Resp.ReturnCode = enResponseCode.Fail;
        //                Resp.ReturnMsg = EnResponseMessage.RequiredParameterMissing;
        //                return Resp;
        //            }
        //            if (Masterdata.SlabType == 1)
        //            {
        //                flag = 1;
        //                if (Request.Amount == null || Request.Amount == 0)
        //                {
        //                    Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
        //                    return Resp;
        //                }
        //                if (Request.EnableStakingBeforeMaturity == null)
        //                {
        //                    Resp.ErrorCode = enErrorCode.EnableStakingBeforeMaturityRequired;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.StakingBeforeMaturityRequired;
        //                    return Resp;
        //                }
        //                if (Request.EnableStakingBeforeMaturityCharge == null)
        //                {
        //                    Resp.ErrorCode = enErrorCode.EnableStakingBeforeMaturityChargeRequired;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.StakingBeforeMaturityChargeRequired;
        //                    return Resp;
        //                }
        //            }
        //            else
        //            {
        //                flag = 0;
        //                if ((Request.MinAmount == null || Request.MinAmount < 0) && (Request.MaxAmount == null || Request.MaxAmount <= 0))
        //                {
        //                    Resp.ErrorCode = enErrorCode.InvalidMinOrMaxStakingAmount;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
        //                    return Resp;
        //                }
        //                if (Request.MinAmount > Request.MaxAmount)
        //                {
        //                    Resp.ErrorCode = enErrorCode.InvalidMinAmount;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidMinValue;
        //                    return Resp;
        //                }
        //                if (Request.MinAmount == Request.MaxAmount)
        //                {
        //                    Resp.ErrorCode = enErrorCode.InvalidMinAndMaxValue;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidMinAndMaxValue;
        //                    return Resp;
        //                }
        //                var amountRange = _controlPanelRepository.GetRange(Convert.ToDecimal(Request.Amount), Convert.ToDecimal(Request.Amount), IsExist.StakingPolicyID);
        //                if (amountRange == 0)//for insert into db=1 and 0=give error
        //                {
        //                    Resp.ErrorCode = enErrorCode.StakingRangeValidationFail;
        //                    Resp.ReturnCode = enResponseCode.Fail;
        //                    Resp.ReturnMsg = EnResponseMessage.InvalidValue;
        //                    return Resp;
        //                }
        //            }
        //        }

        //        IsExist.StakingDurationWeek = Request.DurationWeek;
        //        IsExist.StakingDurationMonth = Request.DurationMonth;
        //        IsExist.EnableAutoUnstaking = Request.AutoUnstakingEnable;
        //        IsExist.MakerCharges = (Request.MakerCharges == null ? IsExist.MakerCharges : Convert.ToDecimal(Request.MakerCharges));
        //        IsExist.TakerCharges = (Request.TakerCharges == null ? IsExist.TakerCharges : Convert.ToDecimal(Request.TakerCharges));

        //        IsExist.RenewUnstakingEnable = (Request.RenewUnstakingEnable == null ? IsExist.RenewUnstakingEnable : Convert.ToInt16(Request.RenewUnstakingEnable));
        //        IsExist.RenewUnstakingPeriod = (Request.RenewUnstakingPeriod == null ? IsExist.RenewUnstakingPeriod : Convert.ToInt16(Request.RenewUnstakingPeriod));
        //        IsExist.Status = (Request.Status == null ? IsExist.Status : Convert.ToInt16(Request.Status));
        //        if (StakingType == 1)
        //        {
        //            IsExist.InterestWalletTypeID = Convert.ToInt64(Request.MaturityCurrency);
        //            IsExist.InterestType = Convert.ToInt16(Request.InterestType);
        //            IsExist.InterestValue = Convert.ToDecimal(Request.InterestValue);
        //        }
        //        if (StakingType == 2)
        //        {
        //            IsExist.InterestWalletTypeID = 0;
        //            IsExist.InterestType = 0;
        //            IsExist.InterestValue = 0;
        //        }
        //        if (flag == 0)
        //        {
        //            IsExist.MinAmount = Convert.ToDecimal(Request.MinAmount);
        //            IsExist.MaxAmount = Convert.ToDecimal(Request.MaxAmount);
        //            IsExist.EnableStakingBeforeMaturity = 0;
        //            IsExist.EnableStakingBeforeMaturityCharge = 0;
        //        }
        //        if (flag == 1)
        //        {
        //            IsExist.EnableStakingBeforeMaturity = Convert.ToInt16(Request.EnableStakingBeforeMaturity);
        //            IsExist.EnableStakingBeforeMaturityCharge = Convert.ToDecimal(Request.EnableStakingBeforeMaturityCharge);
        //            IsExist.MinAmount = Convert.ToDecimal(Request.Amount);
        //            IsExist.MaxAmount = Convert.ToDecimal(Request.Amount);
        //        }
        //        _StakingPolicyDetailCommonRepo.UpdateWithAuditLog(IsExist);
        //        Resp.ErrorCode = enErrorCode.Success;
        //        Resp.ReturnCode = enResponseCode.Success;
        //        Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
        //        return Resp;
        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog("UpdateStakingPolicy", "ControlPanelServices", ex);
        //        throw;
        //    }
        //}

        public async Task<BizResponseClass> UpdateStakingPolicy(long PolicyDetailId, UpdateStakingDetailReq Request, long UserID)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                int flag = -1;
                int StakingType = -1;
                int DurationFlag = -1;
                StakingPolicyDetail IsExist = await _StakingPolicyDetailCommonRepo.GetSingleAsync(item => item.Id == PolicyDetailId && item.Status == 1);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                    return Resp;
                }
                var Masterdata = await _StakingPolicyMasterCommonRepo.GetSingleAsync(item => item.Id == IsExist.StakingPolicyID && item.Status == 1);
                if (Masterdata == null)
                {
                    Resp.ErrorCode = enErrorCode.MasterDataNotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.MasterDataNotFound;
                    return Resp;
                }
                if (Request.DurationMonth == 0 && Request.DurationWeek == 0)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InvalidTimeDuration;
                    Resp.ErrorCode = enErrorCode.InvalidTimeDuration;
                    return Resp;
                }
                if (Masterdata.StakingType == 1)//FD
                {
                    StakingType = 1;
                    if (Request.InterestValue <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.InterestTypeRequired;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidValue;
                        return Resp;
                    }
                    if (Request.InterestType == null || Request.InterestType <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.InterestTypeRequired;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidValue;
                        return Resp;
                    }
                    if (Masterdata.SlabType == 1)//Fix
                    {
                        flag = 1;
                        if (Request.Amount == null || Request.Amount == 0)
                        {
                            Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                            return Resp;
                        }
                        if (Request.EnableStakingBeforeMaturity == null)
                        {
                            Resp.ErrorCode = enErrorCode.EnableStakingBeforeMaturityRequired;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.StakingBeforeMaturityRequired;
                            return Resp;
                        }
                        if (Request.EnableStakingBeforeMaturityCharge == null)
                        {
                            Resp.ErrorCode = enErrorCode.EnableStakingBeforeMaturityChargeRequired;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.StakingBeforeMaturityChargeRequired;
                            return Resp;
                        }
                    }
                    else//Range
                    {
                        flag = 0;
                        if ((Request.MinAmount == null || Request.MinAmount < 0) && (Request.MaxAmount == null || Request.MaxAmount <= 0))
                        {
                            Resp.ErrorCode = enErrorCode.InvalidMinOrMaxStakingAmount;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                            return Resp;
                        }
                        if (Request.MinAmount > Request.MaxAmount)
                        {
                            Resp.ErrorCode = enErrorCode.InvalidMinAmount;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidMinValue;
                            return Resp;
                        }
                        if (Request.MinAmount == Request.MaxAmount)
                        {
                            Resp.ErrorCode = enErrorCode.InvalidMinAndMaxValue;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidMinAndMaxValue;
                            return Resp;
                        }
                        var amountRange = _controlPanelRepository.GetRange(Convert.ToDecimal(Request.MaxAmount), Convert.ToDecimal(Request.MinAmount), IsExist.StakingPolicyID, Convert.ToInt16(Request.InterestType), Convert.ToDecimal(Request.InterestValue), Request.DurationMonth, Request.DurationWeek, IsExist.Id);
                        if (amountRange == 0)//for insert into db=1 and 0=give error
                        {
                            Resp.ErrorCode = enErrorCode.StakingRangeValidationFail;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidValue;
                            return Resp;
                        }
                    }
                    if (Request.InterestType != null && Request.InterestValue != null)
                    {
                        if (Request.InterestType == EnInterestType.Percentage && Request.InterestValue > 100)
                        {
                            Resp.ErrorCode = enErrorCode.InvalidInterestValue;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidValue;
                            return Resp;
                        }
                    }
                    else
                    {
                        Resp.ErrorCode = enErrorCode.InvalidInterestValue;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidValue;
                        return Resp;
                    }
                    if (Request.MaturityCurrency <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.MaturityCurrencyRequired;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.RequiredParameterMissing;
                        return Resp;
                    }
                }
                if (Masterdata.StakingType == 2)//Charge
                {
                    StakingType = 2;
                    if (Request.MakerCharges == null || Request.MakerCharges <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.MakerChargeRequired;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.RequiredParameterMissing;
                        return Resp;
                    }
                    if (Request.TakerCharges == null || Request.TakerCharges <= 0)
                    {
                        Resp.ErrorCode = enErrorCode.TakerChargeRequired;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.RequiredParameterMissing;
                        return Resp;
                    }
                    if (Request.Amount == null || Request.Amount == 0)
                    {
                        Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                        return Resp;
                    }
                    if (Masterdata.SlabType == 1)//Fix
                    {
                        flag = 1;
                        //Rushabh 06-08-2019 take this condition outside as in charge type the range slab is meaningless to use so amount is compulsory for both.
                        //if (Request.Amount == null || Request.Amount == 0)
                        //{
                        // Resp.ErrorCode = enErrorCode.InvalidStakingAmount;
                        // Resp.ReturnCode = enResponseCode.Fail;
                        // Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                        // return Resp;
                        //}
                        if (Request.EnableStakingBeforeMaturity == null)
                        {
                            Resp.ErrorCode = enErrorCode.EnableStakingBeforeMaturityRequired;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.StakingBeforeMaturityRequired;
                            return Resp;
                        }
                        if (Request.EnableStakingBeforeMaturityCharge == null)
                        {
                            Resp.ErrorCode = enErrorCode.EnableStakingBeforeMaturityChargeRequired;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.StakingBeforeMaturityChargeRequired;
                            return Resp;
                        }
                    }
                    else//Range
                    {
                        flag = 0;
                        if ((Request.MinAmount == null || Request.MinAmount < 0) && (Request.MaxAmount == null || Request.MaxAmount <= 0))
                        {
                            Resp.ErrorCode = enErrorCode.InvalidMinOrMaxStakingAmount;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidAmt;
                            return Resp;
                        }
                        if (Request.MinAmount > Request.MaxAmount)
                        {
                            Resp.ErrorCode = enErrorCode.InvalidMinAmount;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidMinValue;
                            return Resp;
                        }
                        //Rushabh 06-08-2019 in charge type the range slab is meaningless to use.
                        //if (Request.MinAmount == Request.MaxAmount)
                        //{
                        // Resp.ErrorCode = enErrorCode.InvalidMinAndMaxValue;
                        // Resp.ReturnCode = enResponseCode.Fail;
                        // Resp.ReturnMsg = EnResponseMessage.InvalidMinAndMaxValue;
                        // return Resp;
                        //}
                        //var amountRange = _controlPanelRepository.GetRange(Convert.ToDecimal(Request.Amount), Convert.ToDecimal(Request.Amount), IsExist.StakingPolicyID);
                        //if (amountRange == 0)//for insert into db=1 and 0=give error
                        //{
                        // Resp.ErrorCode = enErrorCode.StakingRangeValidationFail;
                        // Resp.ReturnCode = enResponseCode.Fail;
                        // Resp.ReturnMsg = EnResponseMessage.InvalidValue;
                        // return Resp;
                        //}
                    }
                }

                IsExist.StakingDurationWeek = Request.DurationWeek;
                IsExist.StakingDurationMonth = Request.DurationMonth;
                IsExist.EnableAutoUnstaking = Request.AutoUnstakingEnable;
                IsExist.MakerCharges = (Request.MakerCharges == null ? IsExist.MakerCharges : Convert.ToDecimal(Request.MakerCharges));
                IsExist.TakerCharges = (Request.TakerCharges == null ? IsExist.TakerCharges : Convert.ToDecimal(Request.TakerCharges));

                IsExist.RenewUnstakingEnable = (Request.RenewUnstakingEnable == null ? IsExist.RenewUnstakingEnable : Convert.ToInt16(Request.RenewUnstakingEnable));
                IsExist.RenewUnstakingPeriod = (Request.RenewUnstakingPeriod == null ? IsExist.RenewUnstakingPeriod : Convert.ToInt16(Request.RenewUnstakingPeriod));
                IsExist.Status = (Request.Status == null ? IsExist.Status : Convert.ToInt16(Request.Status));
                if (StakingType == 1)
                {
                    IsExist.InterestWalletTypeID = Convert.ToInt64(Request.MaturityCurrency);
                    IsExist.InterestType = Convert.ToInt16(Request.InterestType);
                    IsExist.InterestValue = Convert.ToDecimal(Request.InterestValue);
                }
                if (StakingType == 2)
                {
                    IsExist.InterestWalletTypeID = 0;
                    IsExist.InterestType = 0;
                    IsExist.InterestValue = 0;
                }
                if (flag == 0)
                {
                    IsExist.MinAmount = Convert.ToDecimal(Request.MinAmount);
                    IsExist.MaxAmount = Convert.ToDecimal(Request.MaxAmount);
                    IsExist.EnableStakingBeforeMaturity = Convert.ToInt16(Request.EnableStakingBeforeMaturity == null ? 0 : Request.EnableStakingBeforeMaturity);
                    IsExist.EnableStakingBeforeMaturityCharge = Convert.ToDecimal(Request.EnableStakingBeforeMaturityCharge == null ? 0 : Request.EnableStakingBeforeMaturityCharge);
                }
                if (flag == 1)
                {
                    IsExist.EnableStakingBeforeMaturity = Convert.ToInt16(Request.EnableStakingBeforeMaturity);
                    IsExist.EnableStakingBeforeMaturityCharge = Convert.ToDecimal(Request.EnableStakingBeforeMaturityCharge);
                    IsExist.MinAmount = Convert.ToDecimal(Request.Amount);
                    IsExist.MaxAmount = Convert.ToDecimal(Request.Amount);
                }
                _StakingPolicyDetailCommonRepo.UpdateWithAuditLog(IsExist);
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateStakingPolicy", "ControlPanelServices", ex);
                throw;
            }
        }

        public async Task<BizResponseClass> ChangeStakingPolicyStatus(long PolicyDetailId, ServiceStatus Status, long UserID)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                var IsExist = await _StakingPolicyDetailCommonRepo.GetByIdAsync(PolicyDetailId);
                if (IsExist != null)
                {
                    IsExist.Status = Convert.ToInt16(Status);
                    IsExist.UpdatedBy = UserID;
                    _StakingPolicyDetailCommonRepo.UpdateWithAuditLog(IsExist);

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    if (Status == ServiceStatus.Disable)
                    {
                        Resp.ReturnMsg = EnResponseMessage.RecordDeleted;
                    }
                    else if (Status == ServiceStatus.InActive)
                    {
                        Resp.ReturnMsg = EnResponseMessage.RecordInactivated;
                    }
                    else
                    {
                        Resp.ReturnMsg = EnResponseMessage.RecordActivated;
                    }
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ChangeStakingPolicyStatus", "ControlPanelServices", ex);
                throw;
            }
        }

        public async Task<ListStakingPolicyDetailRes> GetStakingPolicy(long PolicyDetailID, short? Status)
        {
            try
            {
                ListStakingPolicyDetailRes Resp = new ListStakingPolicyDetailRes();
                var data = _controlPanelRepository.GetStakingPolicy(PolicyDetailID, Status);
                Resp.Details = data;

                if (data.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;

                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetStakingPolicy", "ControlPanelServices", ex);
                throw;
            }
        }

        public async Task<ListStakingPolicyDetailRes2> ListStakingPolicy(long StackingPolicyMasterId, EnStakingType? StakingType, EnStakingSlabType? SlabType, short? status)
        {
            try
            {
                ListStakingPolicyDetailRes2 Resp = new ListStakingPolicyDetailRes2();
                var data = _controlPanelRepository.ListStakingPolicyDetails(StackingPolicyMasterId, StakingType, SlabType, status);
                Resp.Details = data.Details;
                Resp.MasterDetail = data.MasterDetail;

                if (data.Details.Count > 0 || data.MasterDetail.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;

                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListStakingPolicy", "ControlPanelServices", ex);
                throw;
            }
        }

        public async Task<ListUnStakingHistory> ListUnStakingHistory(long? userid, short? status, EnUnstakeType unStakingType)
        {
            try
            {
                ListUnStakingHistory Resp = new ListUnStakingHistory();
                List<UnStakingHistory> Unstakingdata = _controlPanelRepository.ListUnStakingHistory(userid, status, unStakingType);
                List<string> list = new List<string>();
                Resp.Unstakings = Unstakingdata;
                if (Unstakingdata.Count > 0)
                {
                    foreach (var x in Unstakingdata)
                    {
                        list.Add(x.TokenStakingHistoryID.ToString());
                    }
                    string HistoryId = string.Join(",", list);
                    var StakingData = _controlPanelRepository.ListUnStakingHistoryData(userid, status, unStakingType, HistoryId);
                    Resp.StakingDetail = StakingData;

                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetUnStakingHistory", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListUnStakingHistoryv2> ListUnStakingHistoryv2(long? userid, short? status, EnUnstakeType unStakingType)
        {
            try
            {
                ListUnStakingHistoryv2 Resp = new ListUnStakingHistoryv2();
                List<UnStakingHistoryv2> Unstakingdata = _controlPanelRepository.ListUnStakingHistoryv2(userid, status, unStakingType);
                List<string> list = new List<string>();
                Resp.Unstakings = Unstakingdata;
                if (Unstakingdata.Count > 0)
                {
                    foreach (var x in Unstakingdata)
                    {
                        list.Add(x.TokenStakingHistoryID.ToString());
                    }
                    string HistoryId = string.Join(",", list);
                    var StakingData = _controlPanelRepository.ListUnStakingHistoryDatav2(userid, status, unStakingType, HistoryId);
                    Resp.StakingDetail = StakingData;

                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetUnStakingHistory", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> AdminUnstakeRequest(long adminReqID, ServiceStatus bit, long UserID, UserUnstakingReq Request)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                if (adminReqID <= 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                else
                {
                    var IsExist = await _TokenUnstakingHistoryCommonRepo.GetSingleAsync(i => i.Id == adminReqID);
                    if (IsExist == null)
                    {
                        Resp.ErrorCode = enErrorCode.NotFound;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.NotFound;
                        return Resp;
                    }
                    IsExist.Status = Convert.ToInt16(bit);
                    IsExist.UpdatedBy = UserID;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    if (Convert.ToInt16(bit) == 1)
                    {
                        var data = _walletSPRepositories.Callsp_UnstakingSchemeRequest(Request, UserID, 1);
                        if (data.ErrorCode == enErrorCode.Success)
                        {
                            _TokenUnstakingHistoryCommonRepo.UpdateWithAuditLog(IsExist);
                        }
                        Resp = data;
                    }
                    else
                    {
                        _TokenUnstakingHistoryCommonRepo.UpdateWithAuditLog(IsExist);
                        var data = _TokenstakingHistoryCommonRepo.GetSingle(i => i.Id == Request.StakingHistoryId);
                        if (data != null)
                        {
                            data.Status = 1;
                            _TokenstakingHistoryCommonRepo.UpdateWithAuditLog(data);
                        }
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnCode = enResponseCode.Success;
                        if (Convert.ToInt16(bit) == 0)
                        {
                            Resp.ReturnMsg = EnResponseMessage.RecordInactivated;
                        }
                        else if (Convert.ToInt16(bit) == 9)
                        {
                            Resp.ReturnMsg = EnResponseMessage.RecordDeleted;
                        }
                    }
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AdminUnstakeRequest", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> AdminUnstakeRequestv2(string adminReqID, ServiceStatus bit, long UserID, UserUnstakingReqv2 Request)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                if (adminReqID == null)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                else
                {
                    var IsExist = await _TokenUnstakingHistoryCommonRepo.GetSingleAsync(i => i.GUID == adminReqID);
                    if (IsExist == null)
                    {
                        Resp.ErrorCode = enErrorCode.NotFound;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.NotFound;
                        return Resp;
                    }
                    IsExist.Status = Convert.ToInt16(bit);
                    IsExist.UpdatedBy = UserID;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    if (Convert.ToInt16(bit) == 1)
                    {
                        var HistoryObj = _TokenstakingHistoryCommonRepo.GetSingle(i => i.GUID == Request.StakingHistoryId);
                        if (HistoryObj != null)
                        {
                            UserUnstakingReq datareq = new UserUnstakingReq();
                            datareq.ChannelID = Request.ChannelID;
                            datareq.StakingAmount = Request.StakingAmount;
                            datareq.StakingHistoryId = HistoryObj.Id;
                            datareq.StakingPolicyDetailId = Request.StakingPolicyDetailId;
                            datareq.Type = Request.Type;
                            var data = _walletSPRepositories.Callsp_UnstakingSchemeRequestv2(datareq, UserID, 1);
                            if (data.ErrorCode == enErrorCode.Success)
                            {
                                _TokenUnstakingHistoryCommonRepo.UpdateWithAuditLog(IsExist);
                            }
                            Resp = data;
                        }
                    }
                    else
                    {
                        _TokenUnstakingHistoryCommonRepo.UpdateWithAuditLog(IsExist);
                        var data = _TokenstakingHistoryCommonRepo.GetSingle(i => i.GUID == Request.StakingHistoryId);
                        if (data != null)
                        {
                            data.Status = 1;
                            _TokenstakingHistoryCommonRepo.UpdateWithAuditLog(data);
                        }
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnCode = enResponseCode.Success;
                        if (Convert.ToInt16(bit) == 0)
                        {
                            Resp.ReturnMsg = EnResponseMessage.RecordInactivated;
                        }
                        else if (Convert.ToInt16(bit) == 9)
                        {
                            Resp.ReturnMsg = EnResponseMessage.RecordDeleted;
                        }
                    }
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AdminUnstakeRequest", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListStakingHistoryRes> GetStackingHistoryData(DateTime? fromDate, DateTime? toDate, EnStakeUnStake? type, int pageSize, int pageNo, EnStakingSlabType? slab, EnStakingType? stakingType, long? userId)
        {
            try
            {
                ListStakingHistoryRes Resp = new ListStakingHistoryRes();
                Resp.PageNo = pageNo;
                pageNo = pageNo + 1;
                if (pageNo <= 0 || pageSize <= 0)
                {
                    Resp.ErrorCode = enErrorCode.InValidPage;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InValidPage;
                    return Resp;
                }
                int TotalCount = 0;
                var obj = _walletRepository.GetStackingHistoryData(fromDate, toDate, type, pageSize, pageNo, slab, stakingType, Convert.ToInt64(userId == null ? 0 : userId), ref TotalCount);
                Resp.Stakings = obj;
                Resp.TotalCount = TotalCount;
                Resp.PageSize = pageSize;

                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetStackingHistoryData", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListStakingHistoryResv2> GetStackingHistoryDatav2(DateTime? fromDate, DateTime? toDate, EnStakeUnStake? type, int pageSize, int pageNo, EnStakingSlabType? slab, EnStakingType? stakingType, long? userId)
        {
            try
            {
                ListStakingHistoryResv2 Resp = new ListStakingHistoryResv2();
                Resp.PageNo = pageNo;
                pageNo = pageNo + 1;
                if (pageNo <= 0 || pageSize <= 0)
                {
                    Resp.ErrorCode = enErrorCode.InValidPage;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.InValidPage;
                    return Resp;
                }
                int TotalCount = 0;
                var obj = _walletRepository.GetStackingHistoryDatav2(fromDate, toDate, type, pageSize, pageNo, slab, stakingType, Convert.ToInt64(userId == null ? 0 : userId), ref TotalCount);
                Resp.Stakings = obj;
                Resp.TotalCount = TotalCount;
                Resp.PageSize = pageSize;

                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetStackingHistoryData", this.GetType().Name, ex);
                throw;
            }
        }


        #endregion

        #region StopLossMaster

        //2018-12-28
        public async Task<ListStopLossRes> ListStopLoss(long? WalletTypeId, short? Status)
        {
            ListStopLossRes Resp = new ListStopLossRes();
            try
            {
                var obj = await _controlPanelRepository.ListStopLoss(WalletTypeId, Status);
                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListStopLoss", this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-28
        public async Task<BizResponseClass> InserUpdateStopLoss(InserUpdateStopLossReq Request, long UserId)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                var IsExist = _StopLossMaster.GetSingle(item => item.Id == Request.Id);

                var stopLossObj = _StopLossMaster.GetSingle(i => i.WalletTypeID == Request.WalletTypeId);

                var walletTypeObj = _WalletTypeMaster.GetSingleAsync(i => i.Id == Request.WalletTypeId);

                if (walletTypeObj == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidWalletType, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWalletType };
                }
                if (Request.Id == 0)
                {
                    if (stopLossObj == null)
                    {
                        //insert
                        StopLossMaster Obj = new StopLossMaster();
                        Obj.WalletTypeID = Request.WalletTypeId;
                        Obj.StopLossPer = Request.StopLossPer;
                        Obj.Status = Request.Status;
                        Obj.CreatedBy = UserId;
                        Obj.CreatedDate = Helpers.UTC_To_IST();
                        Obj.UpdatedDate = Helpers.UTC_To_IST();
                        _StopLossMaster.Add(Obj);
                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                    }
                    else if (stopLossObj.Status == 9)
                    {
                        //update
                        stopLossObj.Status = Request.Status;
                        stopLossObj.UpdatedBy = UserId;
                        stopLossObj.UpdatedDate = Helpers.UTC_To_IST();
                        _StopLossMaster.UpdateWithAuditLog(stopLossObj);
                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                    }
                    else
                    {
                        //exist
                        return new BizResponseClass { ErrorCode = enErrorCode.Alredy_Exist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Alredy_Exist };
                    }
                }
                else
                {
                    if (IsExist == null)
                    {
                        return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                    }
                    //update
                    IsExist.Status = Request.Status;
                    IsExist.StopLossPer = Request.StopLossPer;
                    IsExist.UpdatedBy = UserId;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    IsExist.CreatedDate = Helpers.UTC_To_IST();
                    _StopLossMaster.UpdateWithAuditLog(IsExist);
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InserUpdateStopLoss", this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-28
        public async Task<BizResponseClass> ChangeStopLossStatus(short Status, long UserId, long Id)
        {
            try
            {
                var ISExist = _StopLossMaster.GetSingle(i => i.Id == Id);
                if (ISExist != null)
                {
                    //update
                    ISExist.Status = Status;
                    ISExist.UpdatedBy = UserId;
                    ISExist.UpdatedDate = Helpers.UTC_To_IST();
                    _StopLossMaster.UpdateWithAuditLog(ISExist);
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
                return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ChangeStopLossStatus", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region LevarageMaster

        //2018-12-28
        public async Task<ListLeverageRes> ListLeverage(long? WalletTypeId, short? Status)
        {
            ListLeverageRes Resp = new ListLeverageRes();
            try
            {
                var obj = await _controlPanelRepository.ListLeverage(WalletTypeId, Status);
                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListLeverageRes", this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-28
        public async Task<BizResponseClass> InserUpdateLeverage(InserUpdateLeverageReq Request, long UserId)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                var IsExist = _LeverageMaster.GetSingle(item => item.Id == Request.Id);

                var levrageObj = _LeverageMaster.GetSingle(i => i.WalletTypeID == Request.WalletTypeId);

                var walletTypeObj = _WalletTypeMaster.GetSingle(i => i.Id == Request.WalletTypeId);

                if (walletTypeObj == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidWalletType, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWalletType };
                }
                if (Request.Id == 0)
                {
                    if (levrageObj == null)
                    {
                        //insert
                        LeverageMaster Obj = new LeverageMaster();
                        Obj.WalletTypeID = Request.WalletTypeId;
                        Obj.LeveragePer = Request.LeveragePer;
                        Obj.Status = Request.Status;
                        Obj.CreatedBy = UserId;
                        Obj.CreatedDate = Helpers.UTC_To_IST();
                        Obj.UpdatedDate = Helpers.UTC_To_IST();
                        Obj.SafetyMarginPer = Request.SafetyMarginPer;
                        Obj.MarginChargePer = Request.MarginChargePer;
                        Obj.IsAutoApprove = Request.IsAutoApprove;
                        Obj.LeverageChargeDeductionType = Request.LeverageChargeDeductionType;
                        _LeverageMaster.Add(Obj);
                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                    }
                    else if (levrageObj.Status == 9)
                    {
                        //update
                        levrageObj.Status = Request.Status;
                        levrageObj.UpdatedBy = UserId;
                        levrageObj.UpdatedDate = Helpers.UTC_To_IST();
                        levrageObj.LeveragePer = Request.LeveragePer;
                        levrageObj.SafetyMarginPer = Request.SafetyMarginPer;
                        levrageObj.MarginChargePer = Request.MarginChargePer;
                        levrageObj.IsAutoApprove = Request.IsAutoApprove;
                        levrageObj.LeverageChargeDeductionType = Request.LeverageChargeDeductionType;
                        _LeverageMaster.UpdateWithAuditLog(levrageObj);
                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                    }
                    else
                    {
                        //exist
                        return new BizResponseClass { ErrorCode = enErrorCode.Alredy_Exist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Alredy_Exist };
                    }
                }
                else
                {
                    if (IsExist == null)
                    {
                        return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                    }
                    //update
                    IsExist.Status = Request.Status;
                    IsExist.LeveragePer = Request.LeveragePer;
                    IsExist.UpdatedBy = UserId;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    IsExist.SafetyMarginPer = Request.SafetyMarginPer;
                    IsExist.MarginChargePer = Request.MarginChargePer;
                    IsExist.IsAutoApprove = Request.IsAutoApprove;
                    IsExist.LeverageChargeDeductionType = Request.LeverageChargeDeductionType;
                    _LeverageMaster.UpdateWithAuditLog(IsExist);
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InserUpdateLeverage", this.GetType().Name, ex);
                throw;
            }
        }

        //2018-12-28
        public async Task<BizResponseClass> ChangeLeverageStatus(short Status, long UserId, long Id)
        {
            try
            {
                var ISExist = _LeverageMaster.GetSingle(i => i.Id == Id);
                if (ISExist != null)
                {
                    //update
                    ISExist.Status = Status;
                    ISExist.UpdatedBy = UserId;
                    ISExist.UpdatedDate = Helpers.UTC_To_IST();
                    _LeverageMaster.UpdateWithAuditLog(ISExist);
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordDeleted };
                }
                return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ChangeLeverageStatus", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region Import Export Address

        public async Task<BizResponseClass> ImportAddressDetails(string WebRootPath, string FullPath)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                string filePath = FullPath;
                FileInfo file = new FileInfo(filePath.ToString());
                ListImpExpAddressRes3 ListData = new ListImpExpAddressRes3();
                ListData.Details = new List<ImpExpAddressRes3>();

                using (ExcelPackage package = new ExcelPackage(file))
                {
                    var cnt = package.Workbook.Worksheets.Count;
                    StringBuilder sb = new StringBuilder();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int rowCount1 = worksheet.Dimension.End.Row;
                    //int temp = worksheet.Cells.wh
                    int ColCount = worksheet.Dimension.Columns;

                    #region FileValidation

                    if (rowCount > 1 && ColCount >= 7)
                    {
                        if (worksheet.Cells[1, 2].Value.ToString() != "Address")
                        {
                            Resp.ErrorCode = enErrorCode.AddressRequired;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidFileDetail;
                            return Resp;
                        }
                        if (worksheet.Cells[1, 3].Value.ToString() != "AddressLable")
                        {
                            Resp.ErrorCode = enErrorCode.AddressLableRequired;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidFileDetail;
                            return Resp;
                        }
                        if (worksheet.Cells[1, 4].Value.ToString() != "IsDefaultAddress")
                        {
                            Resp.ErrorCode = enErrorCode.IsDefaultAddressRequired;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidFileDetail;
                            return Resp;
                        }
                        if (worksheet.Cells[1, 5].Value.ToString() != "ServiceProviderName")
                        {
                            Resp.ErrorCode = enErrorCode.ServiceProviderNameRequired;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidFileDetail;
                            return Resp;
                        }
                        if (worksheet.Cells[1, 6].Value.ToString() != "WalletTypeName")
                        {
                            Resp.ErrorCode = enErrorCode.WalletTypeNameRequired;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidFileDetail;
                            return Resp;
                        }
                        if (worksheet.Cells[1, 7].Value.ToString() != "Email")
                        {
                            Resp.ErrorCode = enErrorCode.EmailRequired;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.InvalidFileDetail;
                            return Resp;
                        }
                    }
                    else
                    {
                        Resp.ErrorCode = enErrorCode.InvalidFileData;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidFileDetail;
                        return Resp;
                    }

                    #endregion

                    for (int row = 2; row <= rowCount; row++)
                    {
                        if (worksheet.Cells[row, 2].GetValue<string>() == null)
                        {
                            break;
                        }
                        if (row > Convert.ToInt32(_configuration["MaxImportRecords"]))
                        {
                            Resp.ErrorCode = enErrorCode.MaxNoOfRecordLimitExceed;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.MaxNoOfRecordLimitExceed;
                            Resp.ReturnMsg.Replace("#Limit#", _configuration["MaxImportRecords"].ToString());
                            return Resp;
                        }
                        long WTypeID = 0, Uid = 0;
                        ApplicationUser User = new ApplicationUser();
                        ImpExpAddressRes3 data = new ImpExpAddressRes3();
                        data.Address = worksheet.Cells[row, 2].Value.ToString();
                        data.AddressLable = worksheet.Cells[row, 3].Value.ToString();
                        data.IsDefaultAddress = Convert.ToByte(worksheet.Cells[row, 4].Value);
                        data.ServiceProviderName = worksheet.Cells[row, 5].Value.ToString();
                        data.SerProId = _ServiceProviderMaster.GetSingle(i => i.ProviderName == worksheet.Cells[row, 5].Value.ToString()).Id;
                        data.WalletTypeName = worksheet.Cells[row, 6].Value.ToString();
                        WTypeID = _WalletTypeMaster.GetSingle(i => i.WalletTypeName == worksheet.Cells[row, 6].Value.ToString()).Id;
                        data.WalletTypeId = WTypeID;
                        data.Email = worksheet.Cells[row, 7].Value.ToString();
                        Uid = _userManager.FindByEmailAsync(worksheet.Cells[row, 7].Value.ToString()).GetAwaiter().GetResult().Id;
                        data.UserId = Uid;
                        data.WalletId = _WalletMaster.GetSingle(i => i.WalletTypeID == WTypeID && i.UserID == Uid && i.IsDefaultWallet == 1 && i.Status == 1).Id;
                        ListData.Details.Add(data);
                    }
                    Resp = _controlPanelRepository.AddBulkData(ListData.Details);
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ImportAddressDetails", "ControlPanelServices", ex);
                throw;
            }
        }

        public async Task<BizResponseClass> ExportAddressDetails(long? ServiceProviderID, long? UserID, long? WalletTypeID, long LoginUserID)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                var data = _controlPanelRepository.GetExportAddressList(ServiceProviderID, UserID, WalletTypeID);
                if (data != null && data.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindFileFromEmail;

                    var workbook = new XLWorkbook();
                    workbook.AddWorksheet("ExportAddress");
                    var ws = workbook.Worksheet("ExportAddress");

                    int row = 2;

                    ws.Row(1).Cell(1).Value = "Sr.No.";
                    ws.Row(1).Cell(2).Value = "Address";
                    ws.Row(1).Cell(3).Value = "AddressLable";
                    ws.Row(1).Cell(4).Value = "IsDefaultAddress";
                    ws.Row(1).Cell(5).Value = "ServiceProviderName";
                    ws.Row(1).Cell(6).Value = "WalletTypeName";
                    ws.Row(1).Cell(7).Value = "Email";

                    foreach (ImpExpAddressRes item in data)
                    {
                        ws.Cell("A" + row.ToString()).Value = (row - 1).ToString();
                        ws.Cell("B" + row.ToString()).Value = item.Address.ToString();
                        ws.Cell("C" + row.ToString()).Value = item.AddressLable.ToString();
                        ws.Cell("D" + row.ToString()).Value = item.IsDefaultAddress.ToString();
                        ws.Cell("E" + row.ToString()).Value = item.ServiceProviderName.ToString();
                        ws.Cell("F" + row.ToString()).Value = item.WalletTypeName.ToString();
                        ws.Cell("G" + row.ToString()).Value = item.Email.ToString();
                        row++;
                    }
                    string mainPath = _configuration["ExportFilePath"].ToString();
                    string filePath = "ExportAddress_" + Helpers.UTC_To_IST().ToString("yyyyMMddHHmmssfff") + ".xlsx";

                    workbook.SaveAs(mainPath + filePath);
                    string EmailPath = mainPath + filePath;
                    string downloadfile = _configuration["DownloadFilePath"].ToString() + filePath;

                    #region Email Integration Code
                    byte[] passwordBytes = _encdecAEC.GetPasswordBytes(_configuration["AESSalt"].ToString());
                    var Currentuser = await _userManager.FindByIdAsync(LoginUserID.ToString());

                    ExpAddress_EmailLinkTokenViewModel linkToken = new ExpAddress_EmailLinkTokenViewModel
                    {
                        Id = Currentuser.Id,
                        Username = Currentuser.UserName,
                        Email = Currentuser.Email,
                        DownloadLink = EmailPath,
                        CurrentTime = Helpers.UTC_To_IST(),
                        Expirytime = Helpers.UTC_To_IST() + TimeSpan.FromHours(2)
                    };

                    string UserDetails = JsonConvert.SerializeObject(linkToken);
                    string SubScriptionKey = EncyptedDecrypted.Encrypt(UserDetails, passwordBytes);

                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(SubScriptionKey);
                    string ctokenlink = _configuration["ExportAddressMailURL"].ToString() + Convert.ToBase64String(plainTextBytes);

                    // khushali 30-01-2019 for Common Template Method call 
                    TemplateMasterData TemplateData = new TemplateMasterData();
                    CommunicationParamater communicationParamater = new CommunicationParamater();
                    SendEmailRequest request = new SendEmailRequest();
                    if (!string.IsNullOrEmpty(Currentuser.UserName))
                    {
                        communicationParamater.Param1 = Currentuser.UserName;
                    }
                    else
                    {
                        communicationParamater.Param1 = string.Empty;
                    }
                    communicationParamater.Param2 = ctokenlink;
                    TemplateData = _messageService.ReplaceTemplateMasterData(EnTemplateType.Email_ExportAddress, communicationParamater, enCommunicationServiceType.Email).Result;
                    if (TemplateData != null)
                    {
                        if (TemplateData.IsOnOff == 1)
                        {
                            request.Recepient = Currentuser.Email;
                            request.Body = TemplateData.Content;
                            request.Subject = TemplateData.AdditionalInfo;
                            _pushNotificationsQueue.Enqueue(request);
                        }
                    }

                    #endregion
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ExportAddressDetails", "ControlPanelServices", ex);
                throw;
            }
        }

        public ListAddressRes ListAddressDetails(long? ServiceProviderID, long? UserID, long? WalletTypeID, string Address, int PageNo, int PageSize)
        {
            try
            {
                ListAddressRes Resp = new ListAddressRes();
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                int TotalCount = 0;
                var data = _controlPanelRepository.ListAddressDetails(ServiceProviderID, UserID, WalletTypeID, Address, PageNo + 1, PageSize, ref TotalCount);
                Resp.TotalCount = TotalCount;
                if (data != null && data.Count > 0)
                {
                    Resp.Data = data;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListAddressDetails", "ControlPanelServices", ex);
                throw;
            }
        }
        #endregion

        #region Export Wallet

        public async Task<BizResponseClass> ExportWallet(string FileName, string Coin)
        {
            try
            {
                var providerdata = _webApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { SMSCode = Coin.ToLower(), amount = 0, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.ExportWallet) });
                transactionProviderResponses = providerdata.GetAwaiter().GetResult();
                var apiconfig = _thirdPartyCommonRepository.GetByIdAsync(transactionProviderResponses[0].ThirPartyAPIID);
                thirdPartyAPIConfiguration = await apiconfig;
                FileName = _configuration["ExportWalletFilePath"].ToString() + Coin + "/" + FileName + ".txt";
                thirdPartyAPIRequest = _getWebRequest.MakeWebRequestWallet(transactionProviderResponses[0].RouteID, transactionProviderResponses[0].ThirPartyAPIID, transactionProviderResponses[0].SerProDetailID, FileName, Coin);
                string apiResponse = _webApiSendRequest.SendJsonRpcAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIRequest.keyValuePairsHeader);

                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses[0].ThirPartyAPIID);

                if (ParsedResponse.Status == enTransactionStatus.Success)
                {
                    return new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.ExportWalletSuccess, ErrorCode = enErrorCode.Success };
                }
                return new BizResponseClass { ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ExportWalletFail, ErrorCode = enErrorCode.ExportWalletFail };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region StakingPolicyMaster

        // 15-1-2019
        public ListStakingPolicyRes ListStakingPolicyMaster(long? WalletTypeID, short? Status, short? enStakingSlabType, short? enStakingType)
        {
            ListStakingPolicyRes Resp = new ListStakingPolicyRes();
            try
            {
                Resp.Data = _controlPanelRepository.ListStakingPolicyMaster(WalletTypeID, Status, enStakingSlabType, enStakingType).GetAwaiter().GetResult();
                if (Resp.Data.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        // 15-1-2019
        public BizResponseClass InsertUpdateStakingPolicy(InsertUpdateStakingPolicyReq Req, long UserId)
        {
            if (Req == null)
            {
                return new BizResponseClass { ErrorCode = enErrorCode.InvalidInput, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidInput };
            }
            var WalletType = _WalletTypeMaster.GetSingle(i => i.Id == Req.WalletTypeID);
            if (WalletType == null)
            {
                return new BizResponseClass { ErrorCode = enErrorCode.InvalidWalletType, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.InvalidWalletType };
            }
            if (Req.Id != 0)
            {
                var Obj = _StakingPolicyMasterCommonRepo.GetSingle(i => i.Id == Req.Id);
                if (Obj != null)
                {
                    if (Obj.SlabType != Convert.ToInt16(Req.SlabType))
                    {
                        _controlPanelRepository.BulkUpdateDetail(Obj.Id, 9);
                    }
                    Obj.UpdatedBy = UserId;
                    Obj.UpdatedDate = Helpers.UTC_To_IST();
                    Obj.Status = Req.Status;
                    Obj.SlabType = Convert.ToInt16(Req.SlabType);

                    _StakingPolicyMasterCommonRepo.UpdateWithAuditLog(Obj);
                    //update
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
                else
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                }
            }
            else
            {
                var ObjExist = _StakingPolicyMasterCommonRepo.GetSingle(i => i.WalletTypeID == Req.WalletTypeID
                && i.StakingType == Convert.ToInt16(Req.StakingType));
                if (Req.StakingType == EnStakingType.Charge)
                {
                    if (Req.SlabType == EnStakingSlabType.Range)
                    {
                        return new BizResponseClass
                        {
                            ErrorCode = enErrorCode.InvalidSlabTypeSelecttion,
                            ReturnCode = enResponseCode.Fail,
                            ReturnMsg = EnResponseMessage.InvalidSlabType
                        };
                    }
                }
                if (ObjExist == null)
                {
                    StakingPolicyMaster Obj = new StakingPolicyMaster();
                    Obj.CreatedDate = Helpers.UTC_To_IST();
                    Obj.CreatedBy = UserId;
                    Obj.UpdatedBy = null;
                    Obj.UpdatedDate = Helpers.UTC_To_IST();
                    Obj.Status = Req.Status;
                    Obj.StakingType = Convert.ToInt16(Req.StakingType);
                    Obj.WalletTypeID = Convert.ToInt64(Req.WalletTypeID);
                    Obj.SlabType = Convert.ToInt16(Req.SlabType);
                    //insert
                    _StakingPolicyMasterCommonRepo.Add(Obj);
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                }
                else
                {
                    if (ObjExist.Status == 9)
                    {
                        ObjExist.UpdatedBy = UserId;
                        ObjExist.UpdatedDate = Helpers.UTC_To_IST();
                        ObjExist.Status = Req.Status;
                        ObjExist.SlabType = Convert.ToInt16(Req.SlabType);
                        ObjExist.CreatedDate = Helpers.UTC_To_IST();
                        ObjExist.CreatedBy = UserId;
                        _StakingPolicyMasterCommonRepo.UpdateWithAuditLog(ObjExist);

                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                    }
                    return new BizResponseClass { ErrorCode = enErrorCode.AlredyExist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Alredy_Exist };
                }
            }
        }

        // 15-1-2019
        public BizResponseClass ChangeStakingPolicyStatus(long Id, short Status, long UserId)
        {
            try
            {
                var _obj = _StakingPolicyMasterCommonRepo.GetSingle(i => i.Id == Id);
                if (_obj != null)
                {
                    //update
                    _obj.Status = Status;
                    _obj.UpdatedBy = UserId;
                    _obj.UpdatedDate = Helpers.UTC_To_IST();
                    _StakingPolicyMasterCommonRepo.UpdateWithAuditLog(_obj);
                    _controlPanelRepository.BulkUpdateDetail(Id, Status);
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
                return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }
        #endregion

        #region Charge
        public ListChargesTypeWise ListChargesTypeWise(string WalletTypeName, long? TrnTypeId)
        {
            var typeObj = _WalletTypeMaster.GetSingle(i => i.WalletTypeName == WalletTypeName);
            ListChargesTypeWise Resp = new ListChargesTypeWise();
            List<ChargeWalletType> walletTypes = new List<ChargeWalletType>();
            try
            {
                long? Id = null;
                if (typeObj != null)
                {
                    Id = typeObj.Id;
                }
                var res = _controlPanelRepository.GetChargeWalletType(Id);
                for (int i = 0; i <= res.Count - 1; i++)
                {
                    ChargeWalletType a = new ChargeWalletType();
                    a.WalletTypeName = res[i].WalletTypeName;
                    a.WalletTypeId = res[i].WalletTypeId;
                    a.Charges = new List<ChargesTypeWise>();
                    var data = _controlPanelRepository.ListChargesTypeWise(res[i].WalletTypeId, TrnTypeId);
                    a.Charges = data;
                    walletTypes.Add(a);
                }
                if (walletTypes.Count == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.Data = walletTypes;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public ListTrnChargeLogRes TrnChargeLogReport(int PageNo, int PageSize, short? Status, long? TrnTypeID, long? WalleTypeId, short? SlabType, DateTime? FromDate, DateTime? ToDate, long? TrnNo)
        {
            try
            {
                ListTrnChargeLogRes Resp = new ListTrnChargeLogRes();
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                long TotalCount = 0;
                var data = _controlPanelRepository.TrnChargeLogReport(PageNo + 1, PageSize, Status, TrnTypeID, WalleTypeId, SlabType, FromDate, ToDate, TrnNo, ref TotalCount);
                Resp.TotalCount = TotalCount;
                if (data.Count == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.Data = data;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public ListTrnChargeLogResv2 TrnChargeLogReportv2(int PageNo, int PageSize, short? Status, long? TrnTypeID, long? WalleTypeId, short? SlabType, DateTime? FromDate, DateTime? ToDate, string TrnNo)
        {
            try
            {
                Regex GUIDregex = new Regex(@"[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?", RegexOptions.Compiled);

                ListTrnChargeLogResv2 Resp = new ListTrnChargeLogResv2();
                if (TrnNo != null)
                {
                    if (!GUIDregex.IsMatch(TrnNo))
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.InvalidTrnNo;
                        Resp.ErrorCode = enErrorCode.InvalidTrnNo;
                        return Resp;
                    }

                }

                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                long TotalCount = 0;
                var data = _controlPanelRepository.TrnChargeLogReportv2(PageNo + 1, PageSize, Status, TrnTypeID, WalleTypeId, SlabType, FromDate, ToDate, TrnNo, ref TotalCount);
                Resp.TotalCount = TotalCount;
                if (data.Count == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.Data = data;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region DepositCounterMaster

        public ListDepositeCounterRes GetDepositCounter(long? WalletTypeID, long? SerProId, int PageNo, int PageSize)
        {
            try
            {
                ListDepositeCounterRes Resp = new ListDepositeCounterRes();
                int TotalCount = 0;
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                var data = _controlPanelRepository.GetDepositCounter(WalletTypeID, SerProId, PageNo + 1, PageSize, ref TotalCount);
                Resp.TotalCount = TotalCount;
                if (data.Count == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.Data = data;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public BizResponseClass InsertUpdateDepositCounter(InsertUpdateDepositCounterReq Request)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                int AppTypeID = 1;
                var apptypeObj = (from wt in _dbContext.WalletTypeMasters
                                  where wt.Id == Request.WalletTypeID
                                  select wt).FirstOrDefault();
                if (apptypeObj != null && apptypeObj.IsLocal != null)
                {
                    if (apptypeObj.IsLocal == 1)
                    {
                        AppTypeID = Convert.ToInt32(EnAppType.EtherScan);//for local
                    }
                    if (apptypeObj.IsLocal == 0)
                    {
                        AppTypeID = Convert.ToInt32(EnAppType.BitGoAPI);//for international
                    }
                }

                if (Request.Limit < 0 || Request.MaxLimit < 0 || Request.RecordCount < 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.PositiValue, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.PositiValue };
                }
                if (Request.Limit > Request.MaxLimit)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.LessLimit, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.LessLimit };
                }
                if (Request.Id == 0)
                {
                    var IsExist = _DepositCounterMaster.GetSingle(i => i.WalletTypeID == Request.WalletTypeID && i.SerProId == Request.SerProId);
                    if (IsExist != null)
                    {
                        if (IsExist.Status == 9)
                        {
                            IsExist.UpdatedBy = 1;
                            IsExist.UpdatedDate = Helpers.UTC_To_IST();
                            IsExist.CreatedBy = 1;
                            IsExist.CreatedDate = Helpers.UTC_To_IST();
                            IsExist.Status = Request.Status;
                            IsExist.RecordCount = Request.RecordCount;
                            IsExist.MaxLimit = Request.MaxLimit;
                            IsExist.AppType = AppTypeID;
                            IsExist.Limit = Request.Limit;
                            _DepositCounterMaster.UpdateWithAuditLog(IsExist);
                            return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                        }
                        return new BizResponseClass { ErrorCode = enErrorCode.Alredy_Exist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Alredy_Exist };
                    }
                    else
                    {
                        DepositCounterMaster newObj = new DepositCounterMaster();
                        newObj.CreatedDate = Helpers.UTC_To_IST();
                        newObj.CreatedBy = 1;
                        newObj.UpdatedBy = 1;
                        newObj.UpdatedDate = Helpers.UTC_To_IST();
                        newObj.Status = Request.Status;
                        newObj.RecordCount = Request.RecordCount;
                        newObj.Limit = Request.Limit;
                        newObj.LastTrnID = Request.LastTrnID;
                        newObj.MaxLimit = Request.MaxLimit;
                        newObj.WalletTypeID = Request.WalletTypeID;
                        newObj.SerProId = Request.SerProId;
                        newObj.PreviousTrnID = Request.PreviousTrnID;
                        newObj.TPSPickupStatus = Request.TPSPickupStatus;
                        newObj.prevIterationID = Request.PrevIterationID;
                        newObj.AppType = AppTypeID;
                        _DepositCounterMaster.Add(newObj);
                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
                    }
                }
                else
                {
                    var IsExist = _DepositCounterMaster.GetSingle(i => i.Id == Request.Id);
                    if (IsExist == null)
                    {
                        return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                    }
                    else
                    {
                        IsExist.UpdatedBy = 1;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        IsExist.Status = Request.Status;
                        IsExist.RecordCount = Request.RecordCount;
                        IsExist.AppType = AppTypeID;
                        if (Request.IsResetLimit == 1)
                        {
                            IsExist.Limit = 0;
                        }
                        IsExist.MaxLimit = Request.MaxLimit;
                        _DepositCounterMaster.UpdateWithAuditLog(IsExist);
                        return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        public BizResponseClass ChangeDepositCounterStatus(long Id, short Status)
        {
            try
            {
                var IsExist = _DepositCounterMaster.GetSingle(i => i.Id == Id);
                if (IsExist == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                }
                else
                {
                    IsExist.UpdatedBy = 1;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    IsExist.Status = Status;
                    _DepositCounterMaster.UpdateWithAuditLog(IsExist);
                    return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }
        #endregion

        #region AdminAssets

        public ListAdminAssetsres AdminAssets(long? WalletTypeId, EnWalletUsageType? WalletUsageType, long Userid, int PageNo, int PageSize)
        {
            ListAdminAssetsres Resp = new ListAdminAssetsres();
            try
            {
                Resp.PageNo = PageNo;
                Resp.PageSize = PageSize;
                int TotalCount = 0;
                var data = _controlPanelRepository.AdminAssets(WalletTypeId, WalletUsageType, Userid, PageNo + 1, PageSize, ref TotalCount);
                if (data.Count == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                Resp.TotalCount = TotalCount;
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.Data = data;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }
        #endregion

        #region Organization ledger

        public ListOrgLedger OrganizationLedger(long? WalletTypeId, EnWalletUsageType? WalletUsageType)
        {
            try
            {
                ListOrgLedger Resp = new ListOrgLedger();
                int TotalCount = 0;
                var data = _controlPanelRepository.OrganizationLedger(WalletTypeId, WalletUsageType, ref TotalCount);
                if (data.Count == 0)
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                Resp.TotalCount = TotalCount;
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.FindRecored;
                Resp.Data = data;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region ChargeConfigurationMaster

        public async Task<BizResponseClass> AddNewChargeConfiguration(ChargeConfigurationMasterReq Req, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                if (Req.WalletTypeID == 0)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.InvalidWalletType;
                    Resp.ReturnMsg = EnResponseMessage.InvalidWalletType;
                    return Resp;
                }
                var IsExist = await _ChargeConfigurationMaster.GetSingleAsync(i => i.WalletTypeID == Req.WalletTypeID && i.TrnType == Req.TrnType && i.KYCComplaint == Req.KYCComplaint && i.SlabType == Req.SlabType);
                if (IsExist != null)
                {
                    if (IsExist.Status == 9)
                    {
                        IsExist.Status = 1;
                        IsExist.UpdatedBy = id;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        _ChargeConfigurationMaster.UpdateWithAuditLog(IsExist);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.Alredy_Exist;
                        Resp.ReturnMsg = EnResponseMessage.Alredy_Exist;
                    }
                    return Resp;
                }
                else
                {
                    ChargeConfigurationMaster NewObj = new ChargeConfigurationMaster
                    {
                        CreatedBy = id,
                        CreatedDate = Helpers.UTC_To_IST(),
                        KYCComplaint = Req.KYCComplaint,
                        SlabType = Req.SlabType,
                        SpecialChargeConfigurationID = Req.SpecialChargeConfigurationID,
                        Status = Convert.ToInt16(Req.Status == null ? 1 : Req.Status),
                        TrnType = Req.TrnType,
                        WalletTypeID = Req.WalletTypeID,
                        Remarks = Req.Remarks
                    };
                    _ChargeConfigurationMaster.Add(NewObj);
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                    return Resp;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddNewChargeConfiguration", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> UpdateChargeConfiguration(long MasterId, ChargeConfigurationMasterReq2 Req, long UserId)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _ChargeConfigurationMaster.GetSingleAsync(i => i.Id == MasterId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                IsExist.Status = Convert.ToInt16(Req.Status == null ? 1 : Req.Status);
                IsExist.UpdatedBy = UserId;
                IsExist.UpdatedDate = Helpers.UTC_To_IST();
                IsExist.SlabType = Req.SlabType;
                IsExist.Remarks = Req.Remarks;
                _ChargeConfigurationMaster.UpdateWithAuditLog(IsExist);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateChargeConfiguration", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListChargeConfigurationMasterRes> ListChargeConfiguration(long? WalletTypeId, long? TrnType, short? SlabType, short? Status, int? PageSize, int? PageNo)
        {
            ListChargeConfigurationMasterRes Resp = new ListChargeConfigurationMasterRes();
            try
            {
                int TotalCount = 0;
                var data = _controlPanelRepository.GetChargeConfigMasterList(WalletTypeId, TrnType, SlabType, Status, PageSize, PageNo, ref TotalCount);
                Resp.TotalCount = TotalCount;
                if (PageNo != null && PageSize != null)
                {
                    Resp.PageNo = Convert.ToInt32(PageNo);
                    Resp.PageSize = Convert.ToInt32(PageSize);
                }
                else
                {
                    Resp.PageNo = 0;
                    Resp.PageSize = TotalCount;
                }
                if (data.Count > 0)
                {
                    Resp.Details = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListChargeConfiguration", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ChargeConfigurationMasterRes2> GetChargeConfiguration(long MasterID)
        {
            ChargeConfigurationMasterRes2 Resp = new ChargeConfigurationMasterRes2();
            try
            {
                var data = _controlPanelRepository.GetChargeConfigMasterbyId(MasterID);
                if (data != null)
                {
                    Resp.Details = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListChargeConfiguration", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region ChargeConfigurationDetail

        public async Task<BizResponseClass> AddNewChargeConfigurationDetail(ChargeConfigurationDetailReq Req, long id)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _ChargeConfigurationDetail.GetSingleAsync(i => i.ChargeConfigurationMasterID == Req.ChargeConfigurationMasterID && i.ChargeDistributionBasedOn == Convert.ToInt16(Req.ChargeDistributionBasedOn) && i.ChargeType == Convert.ToInt64(Req.ChargeType) && i.ChargeValue == Req.ChargeValue && i.ChargeValueType == Convert.ToInt16(Req.ChargeValueType) && i.ChargeValue == Req.ChargeValue);
                if (IsExist != null)
                {
                    if (IsExist.Status == 9)
                    {
                        IsExist.Status = 1;
                        IsExist.UpdatedBy = id;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        _ChargeConfigurationDetail.UpdateWithAuditLog(IsExist);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.Alredy_Exist;
                        Resp.ReturnMsg = EnResponseMessage.Alredy_Exist;
                    }
                    return Resp;
                }
                else
                {
                    ChargeConfigurationDetail NewObj = new ChargeConfigurationDetail
                    {
                        ChargeConfigurationMasterID = Req.ChargeConfigurationMasterID,
                        ChargeDistributionBasedOn = Convert.ToInt16(Req.ChargeDistributionBasedOn),
                        ChargeType = Convert.ToInt64(Req.ChargeType),
                        ChargeValue = Req.ChargeValue,
                        ChargeValueType = Convert.ToInt16(Req.ChargeValueType),
                        DeductionWalletTypeId = Req.DeductionWalletTypeId,
                        CreatedBy = id,
                        CreatedDate = Helpers.UTC_To_IST(),
                        MakerCharge = Req.MakerCharge,
                        TakerCharge = Req.TakerCharge,
                        MaxAmount = Req.MaxAmount,
                        MinAmount = Req.MinAmount,
                        Remarks = Req.Remarks,
                        Status = Convert.ToInt16(Req.Status == null ? 1 : Req.Status),
                        DeductChargetType = 0
                    };
                    _ChargeConfigurationDetail.Add(NewObj);
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                    return Resp;
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddNewChargeConfigurationDetail", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> UpdateChargeConfigurationDetail(long DetailId, ChargeConfigurationDetailReq Req, long UserId)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _ChargeConfigurationDetail.GetSingleAsync(i => i.Id == DetailId);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                var IsDuplicate = await _ChargeConfigurationDetail.GetSingleAsync(i => i.Id != DetailId && i.ChargeConfigurationMasterID == Req.ChargeConfigurationMasterID && i.ChargeDistributionBasedOn == Convert.ToInt16(Req.ChargeDistributionBasedOn) && i.ChargeType == Convert.ToInt64(Req.ChargeType) && i.ChargeValue == Req.ChargeValue && i.ChargeValueType == Convert.ToInt16(Req.ChargeValueType) && i.ChargeValue == Req.ChargeValue);
                if (IsDuplicate != null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.Alredy_Exist;
                    Resp.ReturnMsg = EnResponseMessage.Alredy_Exist;
                    return Resp;
                }
                IsExist.Status = Convert.ToInt16(Req.Status == null ? 1 : Req.Status);
                IsExist.UpdatedBy = UserId;
                IsExist.UpdatedDate = Helpers.UTC_To_IST();
                IsExist.ChargeConfigurationMasterID = Req.ChargeConfigurationMasterID;
                IsExist.ChargeDistributionBasedOn = Convert.ToInt16(Req.ChargeDistributionBasedOn);
                IsExist.ChargeType = Convert.ToInt64(Req.ChargeType);
                IsExist.ChargeValue = Req.ChargeValue;
                IsExist.ChargeValueType = Convert.ToInt16(Req.ChargeValueType);
                IsExist.DeductionWalletTypeId = Req.DeductionWalletTypeId;
                IsExist.MakerCharge = Req.MakerCharge;
                IsExist.TakerCharge = Req.TakerCharge;
                IsExist.MaxAmount = Req.MaxAmount;
                IsExist.MinAmount = Req.MinAmount;
                IsExist.Remarks = Req.Remarks;
                IsExist.Status = Convert.ToInt16(Req.Status == null ? 1 : Req.Status);
                _ChargeConfigurationDetail.UpdateWithAuditLog(IsExist);

                Resp.ReturnCode = enResponseCode.Success;
                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateChargeConfiguration", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListChargeConfigurationDetailRes> ListChargeConfigurationDetail(long? MasterId, long? ChargeType, short? ChargeValueType, short? ChargeDistributionBasedOn, short? Status)
        {
            ListChargeConfigurationDetailRes Resp = new ListChargeConfigurationDetailRes();
            try
            {
                var data = _controlPanelRepository.GetChargeConfigDetailList(MasterId, ChargeType, ChargeValueType, ChargeDistributionBasedOn, Status);
                if (data.Count > 0)
                {
                    Resp.Details = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListChargeConfiguration", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ChargeConfigurationDetailRes2> GetChargeConfigurationDetail(long DetailId)
        {
            ChargeConfigurationDetailRes2 Resp = new ChargeConfigurationDetailRes2();
            try
            {
                var data = _controlPanelRepository.GetChargeConfigDetailbyId(DetailId);
                if (data != null)
                {
                    Resp.Details = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListChargeConfiguration", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region Deposition Recon

        public async Task<DepositionHistoryRes> DepositionReport(string trnid, string address, short? isInternal, long? userID, string coinName, long? provider, int pageNo, int pageSize)
        {
            DepositionHistoryRes Resp = new DepositionHistoryRes();
            try
            {
                Resp.PageNo = pageNo;
                Resp.PageSize = pageSize;
                int TotalCount = 0;
                var data = _controlPanelRepository.GetDepositionHistorydata(trnid, address, isInternal, userID, coinName, provider, pageNo + 1, pageSize, ref TotalCount);
                Resp.TotalCount = TotalCount;
                if (data.Count > 0)
                {
                    Resp.Deposit = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("DepositionReport", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListDepositionReconRes> DepositionReconProcess(DepositionReconReq request, long UserId)
        {
            try
            {
                ListDepositionReconRes Resp = new ListDepositionReconRes();
                if (request.TrnNo.Length > 0)
                {
                    Resp.ResponseList = new List<DepositionReconRes>();
                    for (int i = 0; i < request.TrnNo.Length; i++)
                    {

                        var data = _walletSPRepositories.CallSP_DepositionRecon(request.TrnNo[i], request, UserId);
                        DepositionReconRes Res = new DepositionReconRes
                        {
                            TrnNo = request.TrnNo[i],
                            ErrorCode = data.ErrorCode,
                            ReturnCode = data.ReturnCode,
                            ReturnMsg = data.ReturnMsg
                        };
                        Resp.ResponseList.Add(Res);
                    }
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("DepositionReconProcess", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        #region MasterLimitConfiguration

        public async Task<BizResponseClass> AddMasterLimitConfiguration(WalletTrnLimitConfigurationInsReq Request, long UserId)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _walletTrnLimitConfiguration.GetSingleAsync(i => i.TrnType == Convert.ToInt64(Request.TrnType) && i.WalletType == Request.WalletType && i.IsKYCEnable == Convert.ToInt16(Request.IsKYCEnable));
                if (IsExist != null)
                {
                    if (IsExist.Status == 9)
                    {
                        IsExist.HourlyTrnCount = Request.HourlyTrnCount;
                        IsExist.HourlyTrnAmount = Request.HourlyTrnAmount;
                        IsExist.DailyTrnCount = Request.DailyTrnCount;
                        IsExist.DailyTrnAmount = Request.DailyTrnAmount;
                        IsExist.WeeklyTrnAmount = Request.WeeklyTrnAmount;
                        IsExist.WeeklyTrnCount = Request.WeeklyTrnCount;
                        IsExist.YearlyTrnAmount = Request.YearlyTrnAmount;
                        IsExist.YearlyTrnCount = Request.YearlyTrnCount;
                        IsExist.MonthlyTrnCount = Request.MonthlyTrnCount;
                        IsExist.MonthlyTrnAmount = Request.MonthlyTrnAmount;
                        IsExist.MinAmount = Request.PerTranMinAmount;
                        IsExist.MaxAmount = Request.PerTranMaxAmount;
                        IsExist.StartTime = Convert.ToDouble(Request.StartTime == null ? 0 : Request.StartTime);
                        IsExist.EndTime = Convert.ToDouble(Request.EndTime == null ? 0 : Request.EndTime);
                        IsExist.Status = Convert.ToInt16(Request.Status);
                        IsExist.UpdatedBy = UserId;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();
                        _walletTrnLimitConfiguration.UpdateWithAuditLog(IsExist);

                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                        return Resp;
                    }
                    else
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.DataAlreadyExist;
                        Resp.ErrorCode = enErrorCode.DataAlreadyExist;
                        return Resp;
                    }
                }
                else
                {
                    WalletTrnLimitConfiguration NewObj = new WalletTrnLimitConfiguration()
                    {
                        TrnType = Convert.ToInt16(Request.TrnType),
                        WalletType = Request.WalletType,
                        IsKYCEnable = Convert.ToInt16(Request.IsKYCEnable),
                        HourlyTrnCount = Request.HourlyTrnCount,
                        HourlyTrnAmount = Request.HourlyTrnAmount,
                        DailyTrnCount = Request.DailyTrnCount,
                        DailyTrnAmount = Request.DailyTrnAmount,
                        WeeklyTrnAmount = Request.WeeklyTrnAmount,
                        WeeklyTrnCount = Request.WeeklyTrnCount,
                        YearlyTrnAmount = Request.YearlyTrnAmount,
                        YearlyTrnCount = Request.YearlyTrnCount,
                        MonthlyTrnCount = Request.MonthlyTrnCount,
                        MonthlyTrnAmount = Request.MonthlyTrnAmount,
                        MinAmount = Request.PerTranMinAmount,
                        MaxAmount = Request.PerTranMaxAmount,
                        StartTime = Convert.ToDouble(Request.StartTime == null ? 0 : Request.StartTime),
                        EndTime = Convert.ToDouble(Request.EndTime == null ? 0 : Request.EndTime),
                        Status = Convert.ToInt16(Request.Status),
                        CreatedBy = UserId,
                        CreatedDate = Helpers.UTC_To_IST()
                    };
                    _walletTrnLimitConfiguration.Add(NewObj);
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordAdded;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AddMasterLimitConfiguration", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> UpdateMasterLimitConfiguration(WalletTrnLimitConfigurationUpdReq Request, long UserId)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _walletTrnLimitConfiguration.GetSingleAsync(i => i.Id == Request.Id);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                else
                {
                    IsExist.HourlyTrnCount = Request.HourlyTrnCount;
                    IsExist.HourlyTrnAmount = Request.HourlyTrnAmount;
                    IsExist.DailyTrnCount = Request.DailyTrnCount;
                    IsExist.DailyTrnAmount = Request.DailyTrnAmount;
                    IsExist.WeeklyTrnCount = Request.WeeklyTrnCount;
                    IsExist.WeeklyTrnAmount = Request.WeeklyTrnAmount;
                    IsExist.MonthlyTrnCount = Request.MonthlyTrnCount;
                    IsExist.MonthlyTrnAmount = Request.MonthlyTrnAmount;
                    IsExist.YearlyTrnCount = Request.YearlyTrnCount;
                    IsExist.YearlyTrnAmount = Request.YearlyTrnAmount;
                    IsExist.MinAmount = Request.PerTranMinAmount;
                    IsExist.MaxAmount = Request.PerTranMaxAmount;
                    IsExist.StartTime = Convert.ToDouble(Request.StartTime == null ? 0 : Request.StartTime);
                    IsExist.EndTime = Convert.ToDouble(Request.EndTime == null ? 0 : Request.EndTime);
                    IsExist.Status = Convert.ToInt16(Request.Status);
                    IsExist.UpdatedBy = UserId;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();

                    _walletTrnLimitConfiguration.UpdateWithAuditLog(IsExist);
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateMasterLimitConfiguration", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> ChangeMasterLimitConfigStatus(ChangeServiceStatus Request, long UserId)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var IsExist = await _walletTrnLimitConfiguration.GetSingleAsync(i => i.Id == Request.Id);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                else
                {
                    IsExist.Status = Convert.ToInt16(Request.Status);
                    IsExist.UpdatedBy = UserId;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    _walletTrnLimitConfiguration.UpdateWithAuditLog(IsExist);

                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.RecordUpdated;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UpdateMasterLimitConfiguration", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListWalletTrnLimitConfigResp> ListMasterLimitConfiguration(long? WalletTypeId, long? TrnType, EnIsKYCEnable? isKYCEnable, short? Status)
        {
            ListWalletTrnLimitConfigResp Resp = new ListWalletTrnLimitConfigResp();
            try
            {
                var data = _controlPanelRepository.ListMasterLimitConfig(WalletTypeId, TrnType, isKYCEnable, Status);
                if (data.Count > 0)
                {
                    Resp.Details = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListMasterConfiguration", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<GetWalletTrnLimitConfigResp> GetMasterLimitConfiguration(long id)
        {
            try
            {
                GetWalletTrnLimitConfigResp Resp = new GetWalletTrnLimitConfigResp();
                var data = _controlPanelRepository.GetMasterLimitConfig(id);
                if (data != null)
                {
                    Resp.Data = data;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    Resp.ErrorCode = enErrorCode.Success;
                }
                else
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetMasterConfiguration", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion

        public ListTradingChargeTypeRes ListTradingChargeTypeMaster()
        {
            ListTradingChargeTypeRes Resp = new ListTradingChargeTypeRes();
            try
            {
                var obj = _controlPanelRepository.ListTradingChargeTypeMaster();
                _walletConfiguration.GetTradingChargeTypeMaster();
                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListTradingChargeTypeMaster", this.GetType().Name, ex);
                throw;
            }
        }

        public ListMarketCurrencyRes ListChargeFreeMarketCurrencyMaster()
        {
            ListMarketCurrencyRes Resp = new ListMarketCurrencyRes();
            try
            {
                var obj = _controlPanelRepository.ListChargeFreeMarketCurrencyMaster();
                Resp.Data = obj;
                if (obj.Count > 0)
                {
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListChargeFreeMarketCurrencyMaster", this.GetType().Name, ex);
                throw;
            }
        }

        public BizResponseClass InsertTradingChargeType(InsertTradingChargeTypeReq Request)
        {
            try
            {
                var IsExist = _TradingChargeTypeMaster.GetSingle(i => i.Type == Request.Type && i.Status == 1);
                if (IsExist != null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.Alredy_Exist, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.Alredy_Exist };
                }
                TradingChargeTypeMaster newObj = new TradingChargeTypeMaster();
                newObj.Type = Request.Type;
                newObj.TypeName = Request.TypeName;
                newObj.Status = Request.Status;
                newObj.IsChargeFreeMarketEnabled = Request.IsChargeFreeMarketEnabled;
                newObj.IsCommonCurrencyDeductEnable = Request.IsCommonCurrencyDeductEnable;
                newObj.IsDeductChargeMarketCurrency = Request.IsDeductChargeMarketCurrency;
                newObj.DeductCurrency = Request.DeductCurrency;
                newObj.DiscountPercent = Request.DiscountPercent;
                _TradingChargeTypeMaster.Add(newObj);
                _walletConfiguration.UpdateTradingChargeTypeMasterList();

                return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertTradingChargeType", this.GetType().Name, ex);
                throw;
            }
        }

        public BizResponseClass UpdateTradingChargeType(UpdateTradingChargeTypeReq Request)
        {
            try
            {
                var IsExist = _TradingChargeTypeMaster.GetSingle(i => i.Id == Request.Id && i.Status < 9);
                if (IsExist == null)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.NotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.NotFound };
                }
                //TradingChargeTypeMaster newObj = new TradingChargeTypeMaster();
                // IsExist.Type = Request.Type;
                IsExist.TypeName = Request.TypeName;
                IsExist.Status = Request.Status;
                IsExist.CreatedBy = 1;
                IsExist.CreatedDate = Helpers.UTC_To_IST(); ;
                IsExist.IsChargeFreeMarketEnabled = Request.IsChargeFreeMarketEnabled;
                IsExist.IsCommonCurrencyDeductEnable = Request.IsCommonCurrencyDeductEnable;
                IsExist.IsDeductChargeMarketCurrency = Request.IsDeductChargeMarketCurrency;
                IsExist.DeductCurrency = Request.DeductCurrency;
                IsExist.DiscountPercent = Request.DiscountPercent;
                _TradingChargeTypeMaster.UpdateWithAuditLog(IsExist);
                _walletConfiguration.UpdateTradingChargeTypeMasterList();

                return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertTradingChargeType", this.GetType().Name, ex);
                throw;
            }
        }

        public BizResponseClass InsertChargeFreeMarketCurrencyMaster(string Currency, long UserId, short Status)
        {
            try
            {
                var IsExist = _ChargeFreeMarketCurrencyMaster.GetSingle(i => i.MarketCurrency == Currency);
                if (IsExist != null)
                {
                    IsExist.Status = Status;
                    IsExist.UpdatedBy = UserId;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    _ChargeFreeMarketCurrencyMaster.UpdateWithAuditLog(IsExist);

                    return new BizResponseClass { ErrorCode = enErrorCode.RecordUpdatedSuccessfully, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordUpdated };
                }
                ChargeFreeMarketCurrencyMaster newObj = new ChargeFreeMarketCurrencyMaster();
                newObj.MarketCurrency = Currency;
                newObj.Status = Status;
                newObj.CreatedBy = UserId;
                newObj.CreatedDate = Helpers.UTC_To_IST();
                _ChargeFreeMarketCurrencyMaster.Add(newObj);

                return new BizResponseClass { ErrorCode = enErrorCode.Success, ReturnCode = enResponseCode.Success, ReturnMsg = EnResponseMessage.RecordAdded };
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("InsertChargeFreeMarketCurrencyMaster", this.GetType().Name, ex);
                throw;
            }
        }

        #region Deposition Interval

        public long AddDepositionInterval(DepositionIntervalViewModel DepositionIntervalInsert, long UserID)
        {
            try
            {
                var IsExist = _controlPanelRepository.FirstDepositionInterval();
                if (IsExist == null)
                {
                    DepositionInterval ObjDepositionInterval = new DepositionInterval()
                    {
                        DepositHistoryFetchListInterval = DepositionIntervalInsert.DepositHistoryFetchListInterval,
                        DepositStatusCheckInterval = DepositionIntervalInsert.DepositStatusCheckInterval,
                        CreatedBy = UserID,
                        CreatedDate = DateTime.UtcNow,
                        Status = Convert.ToInt16(DepositionIntervalInsert.Status)
                    };
                    _DepositionInterval.AddAsync(ObjDepositionInterval);
                    return ObjDepositionInterval.Id;
                }
                else
                {
                    DepositionInterval ObjDepositionInterval = new DepositionInterval();
                    ObjDepositionInterval.Id = IsExist.Id;
                    ObjDepositionInterval.DepositHistoryFetchListInterval = DepositionIntervalInsert.DepositHistoryFetchListInterval;
                    ObjDepositionInterval.DepositStatusCheckInterval = DepositionIntervalInsert.DepositStatusCheckInterval;
                    ObjDepositionInterval.CreatedDate = IsExist.CreatedDate;
                    ObjDepositionInterval.CreatedBy = IsExist.CreatedBy;
                    ObjDepositionInterval.UpdatedBy = UserID;
                    ObjDepositionInterval.UpdatedDate = DateTime.UtcNow;
                    ObjDepositionInterval.Status = Convert.ToInt16(DepositionIntervalInsert.Status);

                    _DepositionInterval.UpdateWithAuditLog(ObjDepositionInterval);
                    return ObjDepositionInterval.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ListDepositionIntervalResponse ListDepositionInterval()
        {
            try
            {

                var ListData = _controlPanelRepository.ListDepositionInterval(); ;
                if (ListData == null)
                {
                    return null;
                }

                ListDepositionIntervalResponse myViewModel = new ListDepositionIntervalResponse();
                myViewModel.ListDepositionInterval = ListData;
                return myViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DisableDepositionInterval(DepositionIntervalStatusViewModel model, long UserId)
        {
            try
            {
                var Disable = _controlPanelRepository.GetDepositionInterval(model.Id);
                if (Disable != null)
                {
                    DepositionInterval ObjDepositionInterval = new DepositionInterval();
                    ObjDepositionInterval.Id = Disable.Id;
                    ObjDepositionInterval.DepositHistoryFetchListInterval = Disable.DepositHistoryFetchListInterval;
                    ObjDepositionInterval.DepositStatusCheckInterval = Disable.DepositStatusCheckInterval;
                    ObjDepositionInterval.CreatedBy = Disable.CreatedBy;
                    ObjDepositionInterval.CreatedDate = Disable.CreatedDate;
                    ObjDepositionInterval.UpdatedBy = UserId;
                    ObjDepositionInterval.UpdatedDate = DateTime.UtcNow;
                    ObjDepositionInterval.Status = 0;
                    _DepositionInterval.UpdateWithAuditLog(ObjDepositionInterval);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public bool EnableDepositionInterval(DepositionIntervalStatusViewModel model, long UserId)
        {
            try
            {
                var Enable = _controlPanelRepository.GetDepositionInterval(model.Id);
                if (Enable != null)
                {
                    DepositionInterval ObjDepositionInterval = new DepositionInterval();
                    ObjDepositionInterval.Id = Enable.Id;
                    ObjDepositionInterval.DepositHistoryFetchListInterval = Enable.DepositHistoryFetchListInterval;
                    ObjDepositionInterval.DepositStatusCheckInterval = Enable.DepositStatusCheckInterval;
                    ObjDepositionInterval.CreatedBy = Enable.CreatedBy;
                    ObjDepositionInterval.CreatedDate = Enable.CreatedDate;
                    ObjDepositionInterval.UpdatedBy = UserId;
                    ObjDepositionInterval.UpdatedDate = DateTime.UtcNow;
                    ObjDepositionInterval.Status = 1;
                    _DepositionInterval.UpdateWithAuditLog(ObjDepositionInterval);

                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public MultichainConnectionViewModel MultichainConnection(string chainName)
        {
            try
            {
                var items = (from sm in _dbContext.ServiceMaster
                             join RC in _dbContext.RouteConfiguration on sm.Id equals RC.ServiceID
                             join spd in _dbContext.ServiceProviderDetail on RC.SerProDetailID equals spd.Id
                             join spc in _dbContext.ServiceProConfiguration on spd.ServiceProConfigID equals spc.Id
                             join dc in _dbContext.DemonConfiguration on spd.DemonConfigID equals dc.Id
                             where sm.Name.Equals(chainName) && spd.TrnTypeID.Equals(16) && sm.Status.Equals(1) && RC.Status.Equals(1)
                             && spd.Status.Equals(1) && spc.Status.Equals(1) && dc.Status.Equals(1)
                             orderby sm.Id descending
                             select new MultichainConnectionViewModel
                             {
                                 hostname = dc.IPAdd,
                                 port = Convert.ToString(dc.PortAdd),
                                 username = spc.UserName,
                                 password = spc.Password,
                                 chainName = sm.Name
                             }
                          ).FirstOrDefault();

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region Service Provider Balance

        public async Task<ServiceProviderBalanceResponse> GetSerProviderBalance(long ServiceProviderId, enTrnType? TransactionType, string CurrencyName)
        {
            ServiceProviderBalanceResponse Resp = new ServiceProviderBalanceResponse();
            try
            {
                string BalanceResp;
                string ethResp;
                decimal ethfee = 0;
                if (TransactionType != null) //ntrivedi 03-05-2019 trntype condition added
                {
                    if (TransactionType != enTrnType.Withdraw && TransactionType != enTrnType.Generate_Address)
                    {
                        return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.InvalidTranxType, ReturnCode = enResponseCode.Fail, ReturnMsg = "Invalid Transaction Type." };
                    }
                }
                var IsLocal = await _WalletTypeMaster.GetSingleAsync(i => i.WalletTypeName == CurrencyName && i.Status == 1);//&& i.IsLocal != 0
                if (IsLocal != null)
                {
                    if (IsLocal.IsLocal == 1)
                    {
                        transactionProviderResponses2 = await _webApiRepository.GetProviderDataListForBalCheckAsyncV2
                  (
                      ServiceProviderId,
                      new TransactionApiConfigurationRequest
                      {
                          SMSCode = CurrencyName.ToLower(),
                          amount = 0,
                          APIType = enWebAPIRouteType.TransactionAPI,
                          trnType = Convert.ToInt32(TransactionType == null ? 0 : TransactionType)
                      }
                  );
                        if (transactionProviderResponses2 == null || transactionProviderResponses2.Count == 0)
                        {
                            return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.TransactionProviderDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                        }
                        Resp.Data = new List<ServiceProviderBalance>();
                        ServiceProviderReq req = new ServiceProviderReq();
                        req.transactionProviderResponses2 = transactionProviderResponses2;
                        Resp = _mediator.Send(req).GetAwaiter().GetResult();
                    }
                    else if (IsLocal.IsLocal == 0)
                    {
                        transactionProviderResponses3 = await _webApiRepository.GetProviderDataListForBalCheckAsync
                   (
                       ServiceProviderId,
                       new TransactionApiConfigurationRequest
                       {
                           SMSCode = CurrencyName.ToLower(),
                           amount = 0,
                           APIType = enWebAPIRouteType.TransactionAPI,
                           trnType = Convert.ToInt32(TransactionType == null ? 0 : TransactionType)
                       }
                   );
                        if (transactionProviderResponses3 == null || transactionProviderResponses3.Count == 0)
                        {
                            return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.TransactionProviderDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                        }
                        Resp.Data = new List<ServiceProviderBalance>();
                        for (int i = 0; i < transactionProviderResponses3.Count; i++)
                        {
                            if (transactionProviderResponses3[i].ThirPartyAPIID == 0)
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.InvalidThirdPartyAPIID, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                            }
                            var apiconfig = _thirdPartyCommonRepository.GetByIdAsync(transactionProviderResponses3[i].ThirPartyAPIID);
                            thirdPartyAPIConfiguration = await apiconfig;

                            if (thirdPartyAPIConfiguration == null || transactionProviderResponses3.Count == 0)
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.ThirdPartyDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                            }
                            thirdPartyAPIRequest = _getWebRequest.MakeWebRequestV2("", "", transactionProviderResponses3[i].RouteID, transactionProviderResponses3[i].ThirPartyAPIID, transactionProviderResponses3[i].SerProDetailID);
                            string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.BalCheckMethodType);

                            // parse response logic                     
                            if (!string.IsNullOrEmpty(apiResponse))
                            {
                                decimal convertAmt = transactionProviderResponses3[i].ConvertAmount == 0 ? 1 : transactionProviderResponses3[i].ConvertAmount;
                                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses3[i].ThirPartyAPIID);
                                BalanceResp = ParsedResponse.Balance.ToString();
                                if (!string.IsNullOrEmpty(BalanceResp))
                                {
                                    decimal responseString = Convert.ToDecimal(BalanceResp);

                                    ServiceProviderBalance Result = new ServiceProviderBalance
                                    {
                                        Balance = responseString / convertAmt,
                                        Fee = ethfee,
                                        Address = "",
                                        CurrencyName = transactionProviderResponses3[i].OpCode.ToUpper()
                                    };
                                    Resp.Data.Add(Result);
                                    Resp.ErrorCode = enErrorCode.Success;
                                    Resp.ReturnCode = enResponseCode.Success;
                                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                                    //return Resp;
                                }
                                else
                                {
                                    return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.BalanceIsNull, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                                }
                            }
                            else
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.NullResponseFromAPI, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                            }
                        }
                    }
                    else if (IsLocal.IsLocal == 8)
                    {
                        transactionProviderResponses3 = await _webApiRepository.GetProviderDataListForBalCheckAsync
                  (
                      ServiceProviderId,
                      new TransactionApiConfigurationRequest
                      {
                          SMSCode = CurrencyName.ToLower(),
                          amount = 0,
                          APIType = enWebAPIRouteType.TransactionAPI,
                          trnType = Convert.ToInt32(TransactionType == null ? 0 : TransactionType)
                      }
                  );
                        if (transactionProviderResponses3 == null || transactionProviderResponses3.Count == 0)
                        {
                            return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.TransactionProviderDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                        }
                        Resp.Data = new List<ServiceProviderBalance>();
                        for (int i = 0; i < transactionProviderResponses3.Count; i++)
                        {
                            if (transactionProviderResponses3[i].ThirPartyAPIID == 0)
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.InvalidThirdPartyAPIID, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                            }
                            var apiconfig = _thirdPartyCommonRepository.GetByIdAsync(transactionProviderResponses3[i].ThirPartyAPIID);
                            thirdPartyAPIConfiguration = await apiconfig;

                            if (thirdPartyAPIConfiguration == null || transactionProviderResponses3.Count == 0)
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.ThirdPartyDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                            }
                            thirdPartyAPIRequest = _getWebRequest.MakeWebRequestV2("", "", transactionProviderResponses3[i].RouteID, transactionProviderResponses3[i].ThirPartyAPIID, transactionProviderResponses3[i].SerProDetailID);
                            string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.BalCheckMethodType);

                            // parse response logic                     
                            if (!string.IsNullOrEmpty(apiResponse))
                            {
                                decimal convertAmt = transactionProviderResponses3[i].ConvertAmount == 0 ? 1 : transactionProviderResponses3[i].ConvertAmount;
                                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses3[i].ThirPartyAPIID);
                                BalanceResp = ParsedResponse.Balance.ToString();
                                if (!string.IsNullOrEmpty(BalanceResp))
                                {
                                    decimal responseString = Convert.ToDecimal(BalanceResp);

                                    ServiceProviderBalance Result = new ServiceProviderBalance
                                    {
                                        Balance = responseString / convertAmt,
                                        Fee = ethfee,
                                        Address = "",
                                        CurrencyName = transactionProviderResponses3[i].OpCode.ToUpper()
                                    };
                                    Resp.Data.Add(Result);
                                    Resp.ErrorCode = enErrorCode.Success;
                                    Resp.ReturnCode = enResponseCode.Success;
                                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                                    //return Resp;
                                }
                                else
                                {
                                    return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.BalanceIsNull, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                                }
                            }
                            else
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.NullResponseFromAPI, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                            }
                        }
                    }
                    else if (IsLocal.IsLocal == 2 || IsLocal.IsLocal == 6)//6-sox
                    {
                        //call Trx bal
                        transactionProviderResponses2 = await _webApiRepository.GetProviderDataListForBalCheckAsyncV2
              (
                  ServiceProviderId,
                  new TransactionApiConfigurationRequest
                  {
                      SMSCode = CurrencyName.ToLower(),
                      amount = 0,
                      APIType = enWebAPIRouteType.TransactionAPI,
                      trnType = Convert.ToInt32(TransactionType == null ? 0 : TransactionType)
                  }
              );

                        if (transactionProviderResponses2 == null || transactionProviderResponses2.Count == 0)
                        {
                            return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.TransactionProviderDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                        }
                        Resp.Data = new List<ServiceProviderBalance>();
                        for (int i = 0; i < transactionProviderResponses2.Count; i++)
                        {
                            if (transactionProviderResponses2[i].ThirPartyAPIID == 0)
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.InvalidThirdPartyAPIID, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                            }
                            var apiconfig = _thirdPartyCommonRepository.GetByIdAsync(transactionProviderResponses2[i].ThirPartyAPIID);
                            thirdPartyAPIConfiguration = await apiconfig;

                            if (thirdPartyAPIConfiguration == null || transactionProviderResponses2.Count == 0)
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.ThirdPartyDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                            }
                            thirdPartyAPIRequest = _getWebRequest.MakeWebRequestV2(transactionProviderResponses2[i].RefKey, transactionProviderResponses2[i].Address, transactionProviderResponses2[i].RouteID, transactionProviderResponses2[i].ThirPartyAPIID, transactionProviderResponses2[i].SerProDetailID);

                            string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.BalCheckMethodType);

                            // parse response logic                     
                            if (!string.IsNullOrEmpty(apiResponse))
                            {
                                decimal convertAmt = transactionProviderResponses2[i].ConvertAmount == 0 ? 1 : transactionProviderResponses2[i].ConvertAmount;
                                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses2[i].ThirPartyAPIID);
                                BalanceResp = ParsedResponse.Balance.ToString();
                                if (!string.IsNullOrEmpty(BalanceResp))
                                {
                                    decimal responseString = Convert.ToDecimal(BalanceResp);

                                    ServiceProviderBalance Result = new ServiceProviderBalance
                                    {
                                        Balance = responseString / convertAmt,
                                        Fee = ethfee,
                                        Address = transactionProviderResponses2[i].Address,
                                        CurrencyName = transactionProviderResponses2[i].OpCode.ToUpper()
                                    };
                                    Resp.Data.Add(Result);
                                    Resp.ErrorCode = enErrorCode.Success;
                                    Resp.ReturnCode = enResponseCode.Success;
                                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                                    //return Resp;
                                }
                                else
                                {
                                    return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.BalanceIsNull, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                                }
                            }
                            else
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.NullResponseFromAPI, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                            }
                        }
                    }
                    else if (IsLocal.IsLocal == 3 || IsLocal.IsLocal == 4)
                    {
                        //call Trx bal and trc10 ,trc20,7=USDx(sox token 10)
                        transactionProviderResponses2 = await _webApiRepository.GetProviderDataListForBalCheckAsyncV2
                  (
                      ServiceProviderId,
                      new TransactionApiConfigurationRequest
                      {
                          SMSCode = CurrencyName.ToLower(),
                          amount = 0,
                          APIType = enWebAPIRouteType.TransactionAPI,
                          trnType = Convert.ToInt32(TransactionType == null ? 0 : TransactionType)
                      }
                  );
                        if (transactionProviderResponses2 == null || transactionProviderResponses2.Count == 0)
                        {
                            return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.TransactionProviderDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                        }
                        Resp.Data = new List<ServiceProviderBalance>();
                        for (int i = 0; i < transactionProviderResponses2.Count; i++)
                        {
                            if (transactionProviderResponses2[i].ThirPartyAPIID == 0)
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.InvalidThirdPartyAPIID, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                            }
                            var apiconfig = _thirdPartyCommonRepository.GetByIdAsync(transactionProviderResponses2[i].ThirPartyAPIID);
                            thirdPartyAPIConfiguration = await apiconfig;
                            if (thirdPartyAPIConfiguration == null || transactionProviderResponses2.Count == 0)
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.ThirdPartyDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                            }
                            thirdPartyAPIRequest = _getWebRequest.MakeWebRequestV2(transactionProviderResponses2[i].RefKey, transactionProviderResponses2[i].Address, transactionProviderResponses2[i].RouteID, transactionProviderResponses2[i].ThirPartyAPIID, transactionProviderResponses2[i].SerProDetailID);
                            string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.BalCheckMethodType);

                            if (IsLocal.IsLocal == 3)
                            {
                                thirdPartyAPIRequest.RequestURL = thirdPartyAPIRequest.RequestURL.Replace("getTrc20Balance", "getTRXbalanace");
                            }
                            else if (IsLocal.IsLocal == 4)
                            {
                                thirdPartyAPIRequest.RequestURL = thirdPartyAPIRequest.RequestURL.Replace("getTrc10Balance", "getTRXbalanace");
                            }

                            ethResp = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.BalCheckMethodType);

                            if (!string.IsNullOrEmpty(apiResponse))
                            {
                                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses2[i].ThirPartyAPIID);
                                BalanceResp = ParsedResponse.Balance.ToString();
                                if (!string.IsNullOrEmpty(BalanceResp))
                                {
                                    decimal responseString = Convert.ToDecimal(BalanceResp);
                                    ServiceProviderBalance Result = new ServiceProviderBalance
                                    {
                                        Balance = responseString,
                                        Address = transactionProviderResponses2[i].Address,
                                        CurrencyName = transactionProviderResponses2[i].OpCode.ToUpper()
                                    };
                                    Resp.Data.Add(Result);
                                    Resp.ErrorCode = enErrorCode.Success;
                                    Resp.ReturnCode = enResponseCode.Success;
                                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                                    //return Resp;
                                }
                                else
                                {
                                    return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.BalanceIsNull, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                                }
                            }
                            if (!string.IsNullOrEmpty(ethResp))
                            {
                                WebAPIParseResponseCls ParsedResponses = _WebApiParseResponse.TransactionParseResponse(ethResp, transactionProviderResponses2[i].ThirPartyAPIID);
                                BalanceResp = ParsedResponses.Balance.ToString();
                                if (!string.IsNullOrEmpty(BalanceResp))
                                {
                                    ethfee = Convert.ToDecimal(BalanceResp);
                                    ServiceProviderBalance Result = new ServiceProviderBalance
                                    {
                                        Balance = 0,
                                        Fee = ethfee,
                                        Address = transactionProviderResponses2[i].Address,
                                        CurrencyName = "TRX"
                                    };
                                    Resp.Data.Add(Result);
                                }
                            }
                            else
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.NullResponseFromAPI, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                            }
                        }
                    }
                    else if (IsLocal.IsLocal == 5)
                    {
                        //call Trx bal
                        transactionProviderResponses2 = await _webApiRepository.GetProviderDataListForBalCheckAsyncV2
              (
                  ServiceProviderId,
                  new TransactionApiConfigurationRequest
                  {
                      SMSCode = CurrencyName.ToLower(),
                      amount = 0,
                      APIType = enWebAPIRouteType.TransactionAPI,
                      trnType = Convert.ToInt32(TransactionType == null ? 0 : TransactionType)
                  }
              );

                        if (transactionProviderResponses2 == null || transactionProviderResponses2.Count == 0)
                        {
                            return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.TransactionProviderDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                        }
                        Resp.Data = new List<ServiceProviderBalance>();
                        for (int i = 0; i < transactionProviderResponses2.Count; i++)
                        {
                            if (transactionProviderResponses2[i].ThirPartyAPIID == 0)
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.InvalidThirdPartyAPIID, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                            }
                            var apiconfig = _thirdPartyCommonRepository.GetByIdAsync(transactionProviderResponses2[i].ThirPartyAPIID);
                            thirdPartyAPIConfiguration = await apiconfig;

                            if (thirdPartyAPIConfiguration == null || transactionProviderResponses2.Count == 0)
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.ThirdPartyDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                            }
                            thirdPartyAPIRequest = _getWebRequest.MakeWebRequestV2(transactionProviderResponses2[i].RefKey, transactionProviderResponses2[i].Address, transactionProviderResponses2[i].RouteID, transactionProviderResponses2[i].ThirPartyAPIID, transactionProviderResponses2[i].SerProDetailID);

                            string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.BalCheckMethodType);

                            // parse response logic                     
                            if (!string.IsNullOrEmpty(apiResponse))
                            {
                                decimal convertAmt = transactionProviderResponses2[i].ConvertAmount == 0 ? 1 : transactionProviderResponses2[i].ConvertAmount;
                                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses2[i].ThirPartyAPIID);
                                BalanceResp = ParsedResponse.Balance.ToString();
                                if (!string.IsNullOrEmpty(BalanceResp))
                                {
                                    decimal responseString = Convert.ToDecimal(BalanceResp);

                                    ServiceProviderBalance Result = new ServiceProviderBalance
                                    {
                                        Balance = responseString / convertAmt,
                                        Fee = ethfee,
                                        Address = transactionProviderResponses2[i].Address,
                                        CurrencyName = transactionProviderResponses2[i].OpCode.ToUpper()
                                    };
                                    Resp.Data.Add(Result);
                                    Resp.ErrorCode = enErrorCode.Success;
                                    Resp.ReturnCode = enResponseCode.Success;
                                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                                }
                                else
                                {
                                    return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.BalanceIsNull, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                                }
                            }
                            else
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.NullResponseFromAPI, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                            }
                        }
                    }
                    else if (IsLocal.IsLocal == 7)
                    {
                        //call Trx bal and trc10 ,trc20,7=USDx(sox token 10)
                        transactionProviderResponses2 = await _webApiRepository.GetProviderDataListForBalCheckAsyncV2
                  (
                      ServiceProviderId,
                      new TransactionApiConfigurationRequest
                      {
                          SMSCode = CurrencyName.ToLower(),
                          amount = 0,
                          APIType = enWebAPIRouteType.TransactionAPI,
                          trnType = Convert.ToInt32(TransactionType == null ? 0 : TransactionType)
                      }
                  );
                        if (transactionProviderResponses2 == null || transactionProviderResponses2.Count == 0)
                        {
                            return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.TransactionProviderDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                        }
                        Resp.Data = new List<ServiceProviderBalance>();
                        for (int i = 0; i < transactionProviderResponses2.Count; i++)
                        {
                            if (transactionProviderResponses2[i].ThirPartyAPIID == 0)
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.InvalidThirdPartyAPIID, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                            }
                            var apiconfig = _thirdPartyCommonRepository.GetByIdAsync(transactionProviderResponses2[i].ThirPartyAPIID);
                            thirdPartyAPIConfiguration = await apiconfig;
                            if (thirdPartyAPIConfiguration == null || transactionProviderResponses2.Count == 0)
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.ThirdPartyDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                            }
                            thirdPartyAPIRequest = _getWebRequest.MakeWebRequestV2(transactionProviderResponses2[i].RefKey, transactionProviderResponses2[i].Address, transactionProviderResponses2[i].RouteID, transactionProviderResponses2[i].ThirPartyAPIID, transactionProviderResponses2[i].SerProDetailID);
                            string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.BalCheckMethodType);

                            if (IsLocal.IsLocal == 6)
                            {
                                thirdPartyAPIRequest.RequestURL = thirdPartyAPIRequest.RequestURL.Replace("getToken10Balance", "getSOXbalanace");
                            }

                            ethResp = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.BalCheckMethodType);

                            if (!string.IsNullOrEmpty(apiResponse))
                            {
                                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses2[i].ThirPartyAPIID);
                                BalanceResp = ParsedResponse.Balance.ToString();
                                if (!string.IsNullOrEmpty(BalanceResp))
                                {
                                    decimal responseString = Convert.ToDecimal(BalanceResp);
                                    ServiceProviderBalance Result = new ServiceProviderBalance
                                    {
                                        Balance = responseString,
                                        Address = transactionProviderResponses2[i].Address,
                                        CurrencyName = transactionProviderResponses2[i].OpCode.ToUpper()
                                    };
                                    Resp.Data.Add(Result);
                                    Resp.ErrorCode = enErrorCode.Success;
                                    Resp.ReturnCode = enResponseCode.Success;
                                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                                }
                                else
                                {
                                    return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.BalanceIsNull, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                                }
                            }
                            if (!string.IsNullOrEmpty(ethResp))
                            {
                                WebAPIParseResponseCls ParsedResponses = _WebApiParseResponse.TransactionParseResponse(ethResp, transactionProviderResponses2[i].ThirPartyAPIID);
                                BalanceResp = ParsedResponses.Balance.ToString();
                                if (!string.IsNullOrEmpty(BalanceResp))
                                {
                                    ethfee = Convert.ToDecimal(BalanceResp);
                                    ServiceProviderBalance Result = new ServiceProviderBalance
                                    {
                                        Balance = 0,
                                        Fee = ethfee,
                                        Address = transactionProviderResponses2[i].Address,
                                        CurrencyName = "SOX"
                                    };
                                    Resp.Data.Add(Result);
                                }
                            }
                            else
                            {
                                return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.NullResponseFromAPI, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                            }
                        }
                    }
                }
                else
                {
                    return new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.NoRecordFound, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetSerProviderBalance", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<LPServiceProBalanceResponse> GetLPProviderBalance(long SerProID, string SMSCode)
        {
            List<Task<LPServiceProviderBalance>> responseCls = new List<Task<LPServiceProviderBalance>>();
            List<LPServiceProviderBalance> responseData = new List<LPServiceProviderBalance>();
            LPServiceProBalanceResponse response = new LPServiceProBalanceResponse();
            WalletTypeMaster WalletTypeobj = new WalletTypeMaster();
            try
            {
                IEnumerable<LPWalletMaster> WalletListobj;
                List<Task<LPServiceProviderBalance>> taskResult = new List<Task<LPServiceProviderBalance>>();

                WalletTypeobj = _WalletTypeMaster.GetSingle(x => x.WalletTypeName == SMSCode);
                if (WalletTypeobj == null)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.InvalidBaseCurrency;
                    response.ErrorCode = enErrorCode.InvalidSourceCurrency;
                    return response;
                }
                if (SerProID == 0)
                {
                    WalletListobj = _LPWalletMaster.FindBy(x => x.WalletTypeID == WalletTypeobj.Id);
                }
                else
                {
                    WalletListobj = _LPWalletMaster.FindBy(x => x.WalletTypeID == WalletTypeobj.Id && x.SerProID == SerProID);
                }
                foreach (LPWalletMaster subItem in WalletListobj)
                {
                    HelperForLog.WriteLogIntoFileAsync("GetLPProviderBalance", "WalletService", subItem.SerProID.ToString() + "ControlPanelService", " Currency " + subItem.WalletTypeID);
                    taskResult.Add(GetLPProviderBalanceMediatR(subItem.SerProID, subItem.WalletTypeID, subItem.Balance));
                }
                var testResult = await Task.WhenAll<LPServiceProviderBalance>(taskResult);

                response.Data = testResult.ToList();
                response.Data.RemoveAll(item => item == null);
                if (testResult.ToList().Count == 0)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.ProviderDataFetchFail;
                    response.ErrorCode = enErrorCode.NoRecordFound;
                }
                else
                {
                    response.ReturnCode = enResponseCode.Success;
                    response.ReturnMsg = EnResponseMessage.ProviderDataFetchSuccess;
                    response.ErrorCode = enErrorCode.Success;
                }
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<LPServiceProviderBalance> GetLPProviderBalanceMediatR(long SerProID, long WalletTypeID, decimal balance)
        {
            LPBalanceCheck mediatrReq;
            WalletTypeMaster _Currency;
            LPServiceProviderBalance responseCls = new LPServiceProviderBalance();
            Core.ViewModels.Configuration.ServiceProviderViewModel providerViewModel = new ServiceProviderViewModel();
            try
            {
                _Currency = _WalletTypeMaster.GetSingle(x => x.Id == WalletTypeID);
                if (_Currency == null)
                {
                    return null;
                }
                providerViewModel = _transactionConfig.GetPoviderByID(SerProID);
                if (providerViewModel == null)
                {
                    return null;
                }
                mediatrReq = new LPBalanceCheck();
                mediatrReq.SerProID = SerProID;
                mediatrReq.Currency = _Currency.WalletTypeName;
                await _mediator.Send(mediatrReq);
                responseCls.Balance = mediatrReq.Balance;
                responseCls.CurrencyName = _Currency.WalletTypeName;
                responseCls.ProviderName = providerViewModel.ProviderName;
                responseCls.WalletBalance = balance;
                HelperForLog.WriteLogIntoFileAsync("GetLPProviderBalanceMediatR", "ControlPanelService", SerProID + " Currency " + WalletTypeID);
                return responseCls;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }


        #endregion

        #region Block User Address

        public async Task<BizResponseClass> BlockUnblockUserAddress(BlockUserReqRes req, long Userid)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                string password = "";
                string sitename = null;
                string siteid = null;
                string Respaddress = null;
                var wallettype = _WalletTypeMaster.GetSingle(t => t.Id == req.WalletTypeID && t.Status == 1);
                if (wallettype == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.WalletTypeRecNotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                var AddressMasterobj = _AddressMasterCommonRepo.GetSingle(x => x.Address == req.Address || x.OriginalAddress == req.Address && x.Status == 1);
                if (AddressMasterobj == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.WalletRecNotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                var walletMasterobj = await _WalletMaster.GetSingleAsync(item => item.Id == AddressMasterobj.WalletId && item.Status == 1 && item.WalletTypeID == wallettype.Id && item.WalletUsageType == Convert.ToInt16(EnWalletUsageType.Trading_Wallet));
                if (walletMasterobj == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.WalletRecNotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                if (req.ID == null) // Insert
                {
                    var IsDuplicate = await _BlockUnblockUserAddressCommonRepo.GetSingleAsync(x => x.Address == req.Address && x.UserID == Userid && x.WalletID == walletMasterobj.Id && x.WalletTypeID == req.WalletTypeID && x.Status == 1);
                    if (IsDuplicate != null)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.AddressAlreadyBlocked;
                        Resp.ReturnMsg = EnResponseMessage.AlreadyBlockedUserAddress;
                        return Resp;
                    }
                    else
                    {
                        var providerdata = _webApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { SMSCode = wallettype.WalletTypeName.ToLower(), amount = 1, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.BlockUserAddress) });
                        transactionProviderResponses = providerdata.GetAwaiter().GetResult();
                        if (transactionProviderResponses == null || transactionProviderResponses.Count == 0)
                        {
                            return new BizResponseClass { ErrorCode = enErrorCode.ItemNotFoundForGenerateAddress, ReturnCode = enResponseCode.Fail, ReturnMsg = "Please try after sometime." };
                        }
                        if (transactionProviderResponses[0].ThirPartyAPIID == 0)
                        {
                            return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                        }
                        var apiconfig = _thirdPartyCommonRepository.GetById(transactionProviderResponses[0].ThirPartyAPIID);

                        thirdPartyAPIConfiguration = apiconfig;
                        if (thirdPartyAPIConfiguration == null || transactionProviderResponses.Count == 0)
                        {
                            return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                        }
                        password = Userid.ToString();
                        sitename = _configuration["sitename"].ToString();
                        siteid = _configuration["site_id"].ToString();

                        thirdPartyAPIRequest = _getWebRequest.MakeWebRequestERC20(transactionProviderResponses[0].RouteID, transactionProviderResponses[0].ThirPartyAPIID, transactionProviderResponses[0].SerProDetailID, password, sitename, siteid);
                        thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#Address#", req.Address);
                        string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.MethodType);
                        //// parse response logic 

                        WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses[0].ThirPartyAPIID);
                        if (ParsedResponse.Status != enTransactionStatus.Success)
                        {
                            return new BizResponseClass { ErrorCode = enErrorCode.BlockUserAddressOperationFail, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
                        }
                        Respaddress = ParsedResponse.Param1;
                        var Key = ParsedResponse.TrnRefNo;
                        if (Respaddress != null)
                        {
                            string responseString = Respaddress;
                            BlockUnblockUserAddress NewObj = new BlockUnblockUserAddress
                            {
                                Address = Respaddress,
                                Status = Convert.ToInt16(req.Status),
                                Remarks = req.Remarks,
                                UserID = Userid,
                                WalletID = walletMasterobj.Id,
                                WalletTypeID = req.WalletTypeID,
                                CreatedBy = Userid,
                                TrnHash = Key,
                                CreatedDate = Helpers.UTC_To_IST()
                            };
                            await _BlockUnblockUserAddressCommonRepo.AddAsync(NewObj);

                            Resp.ErrorCode = enErrorCode.Success;
                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = EnResponseMessage.BlockUserAddressSuccess;
                        }
                        else
                        {
                            Resp.ErrorCode = enErrorCode.BlockUserAddressOperationFail;
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ReturnMsg = EnResponseMessage.OperationFail;
                        }
                    }
                }
                else //Update
                {
                    var IsExist = await _BlockUnblockUserAddressCommonRepo.GetSingleAsync(x => x.Id == req.ID);
                    if (IsExist == null)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.NoDataFound;
                        Resp.ReturnMsg = EnResponseMessage.NotFound;
                        return Resp;
                    }
                    var IsDuplicate = await _BlockUnblockUserAddressCommonRepo.GetSingleAsync(x => x.Id != req.ID && x.Address == req.Address && x.UserID == Userid && x.WalletID == walletMasterobj.Id && x.WalletTypeID == req.WalletTypeID && x.Status == 2);
                    if (IsDuplicate != null)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.AddressAlreadyUnblocked;
                        Resp.ReturnMsg = EnResponseMessage.AlreadyUnBlockedUserAddress;
                        return Resp;
                    }
                    var providerdata = _webApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { SMSCode = wallettype.WalletTypeName.ToLower(), amount = 1, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.UnblockUser) });
                    transactionProviderResponses = providerdata.GetAwaiter().GetResult();
                    if (transactionProviderResponses == null || transactionProviderResponses.Count == 0)
                    {
                        return new BizResponseClass { ErrorCode = enErrorCode.ItemNotFoundForGenerateAddress, ReturnCode = enResponseCode.Fail, ReturnMsg = "Please try after sometime." };
                    }
                    if (transactionProviderResponses[0].ThirPartyAPIID == 0)
                    {
                        return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                    }
                    var apiconfig = _thirdPartyCommonRepository.GetById(transactionProviderResponses[0].ThirPartyAPIID);

                    thirdPartyAPIConfiguration = apiconfig;
                    if (thirdPartyAPIConfiguration == null || transactionProviderResponses.Count == 0)
                    {
                        return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                    }
                    password = "";
                    sitename = "";
                    siteid = "";

                    thirdPartyAPIRequest = _getWebRequest.MakeWebRequestERC20(transactionProviderResponses[0].RouteID, transactionProviderResponses[0].ThirPartyAPIID, transactionProviderResponses[0].SerProDetailID, password, sitename, siteid);
                    thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#Address#", req.Address);
                    string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.MethodType);
                    //// parse response logic 

                    WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses[0].ThirPartyAPIID);
                    if (ParsedResponse.Status != enTransactionStatus.Success)
                    {
                        return new BizResponseClass { ErrorCode = enErrorCode.BlockUserAddressOperationFail, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
                    }
                    Respaddress = ParsedResponse.Param1;
                    var Key = ParsedResponse.TrnRefNo;
                    if (Respaddress != null)
                    {
                        string responseString = Respaddress;

                        BlockUnblockUserAddress NewObj = new BlockUnblockUserAddress
                        {
                            Address = Respaddress,
                            Status = Convert.ToInt16(req.Status),
                            Remarks = req.Remarks,
                            UserID = Userid,
                            WalletID = walletMasterobj.Id,
                            WalletTypeID = req.WalletTypeID,
                            CreatedBy = Userid,
                            TrnHash = Key,
                            CreatedDate = Helpers.UTC_To_IST()
                        };
                        await _BlockUnblockUserAddressCommonRepo.AddAsync(NewObj);
                        IsExist.Status = 3;
                        IsExist.Remarks = req.Remarks;
                        IsExist.UpdatedBy = Userid;
                        IsExist.UpdatedDate = Helpers.UTC_To_IST();

                        _BlockUnblockUserAddressCommonRepo.UpdateWithAuditLog(IsExist);

                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.UnBlockUserAddressSuccess;
                    }
                    else
                    {
                        Resp.ErrorCode = enErrorCode.BlockUserAddressOperationFail;
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ReturnMsg = EnResponseMessage.OperationFail;
                    }
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("BlockUnblockUserAddress", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListBlockUserRes> ListBlockUnblockUserAddress(long? userId, string address, DateTime? fromDate, DateTime? toDate, short? status)
        {
            ListBlockUserRes Resp = new ListBlockUserRes();
            try
            {
                var data = _controlPanelRepository.ListBlockUserAddresses(userId, address, fromDate, toDate, status);
                if (data.Count > 0)
                {
                    Resp.Data = data;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListBlockUnblockUserAddress", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> DestroyBlackFund(DestroyBlackFundReq req, long Userid)
        {
            BizResponseClass Resp = new BizResponseClass();
            string password = "";
            string sitename = null;
            string siteid = null;
            string Respaddress = null;
            try
            {
                var IsExist = await _BlockUnblockUserAddressCommonRepo.GetSingleAsync(x => x.Id == req.Id && x.Address == req.Address && x.Status == 1);
                if (IsExist == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NoDataFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                if (IsExist.IsDestroyed == 1)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.BlackFundAlreadyDestroyed;
                    Resp.ReturnMsg = EnResponseMessage.BlackFundAlreadyDestroyed;
                    return Resp;
                }
                var wallettype = _WalletTypeMaster.GetSingle(t => t.Id == IsExist.WalletTypeID && t.Status == 1);
                if (wallettype == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.WalletTypeRecNotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                var providerdata = _webApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { SMSCode = wallettype.WalletTypeName.ToLower(), amount = 1, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.DestroyBlackFund) });
                transactionProviderResponses = providerdata.GetAwaiter().GetResult();
                if (transactionProviderResponses == null || transactionProviderResponses.Count == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.ItemNotFoundForGenerateAddress, ReturnCode = enResponseCode.Fail, ReturnMsg = "Please try after sometime." };
                }
                if (transactionProviderResponses[0].ThirPartyAPIID == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                var apiconfig = _thirdPartyCommonRepository.GetById(transactionProviderResponses[0].ThirPartyAPIID);

                thirdPartyAPIConfiguration = apiconfig;
                if (thirdPartyAPIConfiguration == null || transactionProviderResponses.Count == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                password = Userid.ToString();
                sitename = _configuration["sitename"].ToString();
                siteid = _configuration["site_id"].ToString();

                thirdPartyAPIRequest = _getWebRequest.MakeWebRequestERC20(transactionProviderResponses[0].RouteID, transactionProviderResponses[0].ThirPartyAPIID, transactionProviderResponses[0].SerProDetailID, password, sitename, siteid);
                thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#Address#", req.Address);
                string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.MethodType);
                //// parse response logic 

                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses[0].ThirPartyAPIID);
                if (ParsedResponse.Status != enTransactionStatus.Success)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.DestroyBlackFundOperationFail, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
                }
                Respaddress = ParsedResponse.Param1;
                var Key = ParsedResponse.TrnRefNo;
                if (!String.IsNullOrEmpty(Respaddress))
                {
                    string responseString = Respaddress;
                    IsExist.UpdatedBy = Userid;
                    IsExist.IsDestroyed = 1;
                    IsExist.UpdatedDate = Helpers.UTC_To_IST();
                    _BlockUnblockUserAddressCommonRepo.UpdateWithAuditLog(IsExist);

                    DestroyFundRequest NewObj = new DestroyFundRequest
                    {
                        Address = Respaddress,
                        Remarks = req.Remarks,
                        TrnHash = Key.ToString(),
                        CreatedBy = Userid,
                        CreatedDate = Helpers.UTC_To_IST(),
                        Status = 1
                    };
                    await _DestroyFundRequestCommonRepo.AddAsync(NewObj);

                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.BlackFundDestroyedSuccess;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.DestroyBlackFundOperationFail;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.OperationFail;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("DestroyBlackFund", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListDestroyBlackFundRes> DestroyedBlackFundHistory(string address, DateTime? fromDate, DateTime? toDate)
        {
            ListDestroyBlackFundRes Resp = new ListDestroyBlackFundRes();
            try
            {
                var data = _controlPanelRepository.ListDestroyedBlackFund(address, fromDate, toDate);
                if (data.Count > 0)
                {
                    Resp.Data = data;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("DestroyedBlackFundHistory", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> TokenTransfer(TokenTransferReq req, long Userid)
        {
            BizResponseClass Resp = new BizResponseClass();
            string password = "";
            string sitename = null;
            string siteid = null;
            string Respaddress = null;
            try
            {
                var wallettype = await _WalletTypeMaster.GetSingleAsync(t => t.Id == req.FromWalletTypeID && t.Status == 1);
                if (wallettype == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.WalletTypeRecNotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                var providerdata = _webApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { SMSCode = wallettype.WalletTypeName.ToLower(), amount = 1, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.TransferToken) });
                transactionProviderResponses = providerdata.GetAwaiter().GetResult();
                if (transactionProviderResponses == null || transactionProviderResponses.Count == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.ItemNotFoundForGenerateAddress, ReturnCode = enResponseCode.Fail, ReturnMsg = "Please try after sometime." };
                }
                if (transactionProviderResponses[0].ThirPartyAPIID == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                var apiconfig = _thirdPartyCommonRepository.GetById(transactionProviderResponses[0].ThirPartyAPIID);

                thirdPartyAPIConfiguration = apiconfig;
                if (thirdPartyAPIConfiguration == null || transactionProviderResponses.Count == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                password = Userid.ToString();
                sitename = _configuration["sitename"].ToString();
                siteid = _configuration["site_id"].ToString();

                thirdPartyAPIRequest = _getWebRequest.MakeWebRequestERC20(transactionProviderResponses[0].RouteID, transactionProviderResponses[0].ThirPartyAPIID, transactionProviderResponses[0].SerProDetailID, password, sitename, siteid);

                //now onwards from address will be fetched from route configuration's Provider WalletId instead of user input
                thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#FromAddress#", thirdPartyAPIRequest.walletID.ToString());
                thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#ToAddress#", req.ToAddress);
                thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#Amount#", req.Amount.ToString());
                string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.MethodType);
                //// parse response logic 

                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses[0].ThirPartyAPIID);
                if (ParsedResponse.Status != enTransactionStatus.Success)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.TokenTransferOperationFail, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
                }
                Respaddress = ParsedResponse.TrnRefNo;
                if (!String.IsNullOrEmpty(Respaddress))
                {
                    string responseString = Respaddress;
                    TokenTransferHistory NewObj = new TokenTransferHistory
                    {
                        Amount = req.Amount,
                        Remarks = req.Remarks,
                        ToAddress = req.ToAddress,
                        FromAddress = thirdPartyAPIRequest.walletID.ToString(),
                        CreatedBy = Userid,
                        CreatedDate = Helpers.UTC_To_IST(),
                        Status = 1,
                        TrnHash = responseString
                    };
                    await _TokenTransferHistoryCommonRepo.AddAsync(NewObj);
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.TransferTokenSuccess;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.TokenTransferOperationFail;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.OperationFail;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("TokenTransfer", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListTokenTransferRes> TokenTransferHistory(DateTime? fromDate, DateTime? toDate)
        {
            ListTokenTransferRes Resp = new ListTokenTransferRes();
            try
            {
                var data = _controlPanelRepository.ListTokenTransferHistory(fromDate, toDate);
                if (data.Count > 0)
                {
                    Resp.Data = data;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("TokenTransferHistory", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> IncreaseTokenSupply(TokenSupplyReq req, long Userid)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                string password = "";
                string sitename = null;
                string siteid = null;
                string Respaddress = null;

                var wallettype = await _WalletTypeMaster.GetSingleAsync(t => t.Id == req.WalletTypeId && t.Status == 1);
                if (wallettype == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.WalletTypeRecNotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                var providerdata = _webApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { SMSCode = wallettype.WalletTypeName.ToLower(), amount = 1, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.IncreaseTokenSupply) });
                transactionProviderResponses = providerdata.GetAwaiter().GetResult();
                if (transactionProviderResponses == null || transactionProviderResponses.Count == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.ItemNotFoundForGenerateAddress, ReturnCode = enResponseCode.Fail, ReturnMsg = "Please try after sometime." };
                }
                if (transactionProviderResponses[0].ThirPartyAPIID == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                var apiconfig = _thirdPartyCommonRepository.GetById(transactionProviderResponses[0].ThirPartyAPIID);

                thirdPartyAPIConfiguration = apiconfig;
                if (thirdPartyAPIConfiguration == null || transactionProviderResponses.Count == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                password = Userid.ToString();
                sitename = _configuration["sitename"].ToString();
                siteid = _configuration["site_id"].ToString();

                thirdPartyAPIRequest = _getWebRequest.MakeWebRequestERC20(transactionProviderResponses[0].RouteID, transactionProviderResponses[0].ThirPartyAPIID, transactionProviderResponses[0].SerProDetailID, password, sitename, siteid);

                thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#Amount#", req.Amount.ToString());
                string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.MethodType);
                //// parse response logic 

                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses[0].ThirPartyAPIID);
                if (ParsedResponse.Status != enTransactionStatus.Success)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.IncreaseTokenSupplyOperationFail, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
                }
                Respaddress = ParsedResponse.Param1;
                var Key = ParsedResponse.TrnRefNo;
                if (!String.IsNullOrEmpty(Respaddress))
                {
                    string responseString = Respaddress;
                    TokenSupplyHistory NewObj = new TokenSupplyHistory
                    {
                        Amount = req.Amount,
                        ContractAddress = Respaddress,
                        TrnHash = Key.ToString(),
                        CreatedBy = Userid,
                        CreatedDate = Helpers.UTC_To_IST(),
                        Status = 1,
                        WalletTypeId = req.WalletTypeId,
                        IsIncrease = 1,
                        Remarks = req.Remarks
                    };
                    await _TokenSupplyHistoryCommonRepo.AddAsync(NewObj);
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.TokenSupplyIncreaseSuccess;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.IncreaseTokenSupplyOperationFail;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.OperationFail;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("IncreaseTokenSupply", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> DecreaseTokenSupply(TokenSupplyReq req, long Userid)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                string password = "";
                string sitename = null;
                string siteid = null;
                string Respaddress = null;

                var wallettype = await _WalletTypeMaster.GetSingleAsync(t => t.Id == req.WalletTypeId && t.Status == 1);
                if (wallettype == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.WalletTypeRecNotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                var providerdata = _webApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { SMSCode = wallettype.WalletTypeName.ToLower(), amount = 1, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.DecreaseTokenSupply) });
                transactionProviderResponses = providerdata.GetAwaiter().GetResult();
                if (transactionProviderResponses == null || transactionProviderResponses.Count == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.ItemNotFoundForGenerateAddress, ReturnCode = enResponseCode.Fail, ReturnMsg = "Please try after sometime." };
                }
                if (transactionProviderResponses[0].ThirPartyAPIID == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                var apiconfig = _thirdPartyCommonRepository.GetById(transactionProviderResponses[0].ThirPartyAPIID);

                thirdPartyAPIConfiguration = apiconfig;
                if (thirdPartyAPIConfiguration == null || transactionProviderResponses.Count == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                password = Userid.ToString();
                sitename = _configuration["sitename"].ToString();
                siteid = _configuration["site_id"].ToString();

                thirdPartyAPIRequest = _getWebRequest.MakeWebRequestERC20(transactionProviderResponses[0].RouteID, transactionProviderResponses[0].ThirPartyAPIID, transactionProviderResponses[0].SerProDetailID, password, sitename, siteid);

                thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#Amount#", req.Amount.ToString());
                string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.MethodType);
                //// parse response logic 

                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses[0].ThirPartyAPIID);
                if (ParsedResponse.Status != enTransactionStatus.Success)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.DecreaseTokenSupplyOperationFail, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
                }
                Respaddress = ParsedResponse.Param1;
                var Key = ParsedResponse.TrnRefNo;
                if (!String.IsNullOrEmpty(Respaddress))
                {
                    string responseString = Respaddress;
                    TokenSupplyHistory NewObj = new TokenSupplyHistory
                    {
                        Amount = req.Amount,
                        ContractAddress = Respaddress,
                        TrnHash = Key.ToString(),
                        CreatedBy = Userid,
                        CreatedDate = Helpers.UTC_To_IST(),
                        Status = 1,
                        WalletTypeId = req.WalletTypeId,
                        IsIncrease = 2,
                        Remarks = req.Remarks
                    };
                    await _TokenSupplyHistoryCommonRepo.AddAsync(NewObj);
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.TokenSupplyDecreaseSuccess;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.DecreaseTokenSupplyOperationFail;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.OperationFail;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("IncreaseTokenSupply", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListTokenSupplyRes> IncreaseDecreaseTokenSupplyHistory(long? walletTypeId, short? actionType, DateTime? fromDate, DateTime? toDate)
        {
            ListTokenSupplyRes Resp = new ListTokenSupplyRes();
            try
            {
                var data = _controlPanelRepository.ListIncreaseDecreaseTokenSupply(walletTypeId, actionType, fromDate, toDate);
                if (data.Count > 0)
                {
                    Resp.Data = data;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("IncreaseDecreaseTokenSupplyHistory", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<BizResponseClass> SetTransferFee(SetTransferFeeReq req, long Userid)
        {
            try
            {
                BizResponseClass Resp = new BizResponseClass();
                string password = "";
                string sitename = null;
                string siteid = null;
                string Respaddress = null;

                var wallettype = await _WalletTypeMaster.GetSingleAsync(t => t.Id == req.WalletTypeId && t.Status == 1);
                if (wallettype == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.WalletTypeRecNotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                var providerdata = _webApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { SMSCode = wallettype.WalletTypeName.ToLower(), amount = 1, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.SetTransferFee) });
                transactionProviderResponses = providerdata.GetAwaiter().GetResult();
                if (transactionProviderResponses == null || transactionProviderResponses.Count == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.ItemNotFoundForGenerateAddress, ReturnCode = enResponseCode.Fail, ReturnMsg = "Please try after sometime." };
                }
                if (transactionProviderResponses[0].ThirPartyAPIID == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                var apiconfig = _thirdPartyCommonRepository.GetById(transactionProviderResponses[0].ThirPartyAPIID);

                thirdPartyAPIConfiguration = apiconfig;
                if (thirdPartyAPIConfiguration == null || transactionProviderResponses.Count == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                password = Userid.ToString();
                sitename = _configuration["sitename"].ToString();
                siteid = _configuration["site_id"].ToString();

                thirdPartyAPIRequest = _getWebRequest.MakeWebRequestERC20(transactionProviderResponses[0].RouteID, transactionProviderResponses[0].ThirPartyAPIID, transactionProviderResponses[0].SerProDetailID, password, sitename, siteid);

                thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#BasePoint#", req.BasePoint.ToString());
                thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#MinFee#", req.Minfee.ToString());
                thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#MaxFee#", req.Maxfee.ToString());

                string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.MethodType);
                //// parse response logic 

                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses[0].ThirPartyAPIID);
                if (ParsedResponse.Status != enTransactionStatus.Success)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.SetTransferFeeOperationFail, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.CreateWalletFailMsg };
                }
                Respaddress = ParsedResponse.Param1;
                var Key = ParsedResponse.TrnRefNo;
                if (!String.IsNullOrEmpty(Respaddress))
                {
                    string responseString = Respaddress;
                    TransferFeeHistory NewObj = new TransferFeeHistory
                    {
                        ContractAddress = Respaddress,
                        TrnHash = Key.ToString(),
                        CreatedBy = Userid,
                        CreatedDate = Helpers.UTC_To_IST(),
                        Status = 1,
                        WalletTypeId = req.WalletTypeId,
                        Remarks = req.Remarks,
                        BasePoint = req.BasePoint,
                        Maxfee = req.Maxfee,
                        Minfee = req.Minfee
                    };
                    await _TransferFeeHistoryCommonRepo.AddAsync(NewObj);
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.SetTransferFeeSuccess;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.SetTransferFeeOperationFail;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.OperationFail;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("SetTransferFee", this.GetType().Name, ex);
                throw;
            }
        }

        public async Task<ListSetTransferFeeRes> TransferFeeHistory(long? walletTypeId, DateTime? fromDate, DateTime? toDate)
        {
            ListSetTransferFeeRes Resp = new ListSetTransferFeeRes();
            try
            {
                var data = _controlPanelRepository.ListTransferFeeHistory(walletTypeId, fromDate, toDate);
                if (data.Count > 0)
                {
                    Resp.Data = data;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("TransferFeeHistory", this.GetType().Name, ex);
                throw;
            }
        }

        #endregion
        public async Task<ListWithdrawalAdminRes> ListWithdrawalRequest(long? trnNo, DateTime? FromDate, DateTime? ToDate, short? status)
        {
            ListWithdrawalAdminRes Resp = new ListWithdrawalAdminRes();
            try
            {
                var data = _controlPanelRepository.ListWithdrawalReqData(trnNo, FromDate, ToDate, status);
                if (data.Count > 0)
                {
                    Resp.Data = data;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("ListWithdrawalRequest", this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> AdminWithdrawalRequest(long adminReqId, ApprovalStatus bit, long Userid, string remarks)
        {
            BizResponseClass Resp = new BizResponseClass();
            TransactionQueue Newtransaction;
            try
            {
                var IsExist = await _WithdrawAdminReqCommonRepo.GetSingleAsync(x => x.Id == adminReqId);
                if (IsExist != null)
                {
                    //Check if Already Processed Or Not
                    if (IsExist.Status != 0)
                    {
                        Resp.ReturnCode = enResponseCode.Fail;
                        Resp.ErrorCode = enErrorCode.InvalidTrnStatusForProcess;
                        Resp.ReturnMsg = "Invalid Status";
                        return Resp;
                    }
                    else
                    {
                        //Get Transaction From TransactionQueue              
                        var tranTQobj = _TransactionRepository.GetSingle(x => x.Id == IsExist.TrnNo);
                        Newtransaction = new TransactionQueue();
                        Newtransaction = tranTQobj;
                        if (tranTQobj == null)
                        {
                            Resp.ReturnMsg = "Transaction Not Found";
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ErrorCode = enErrorCode.Withdrwal_TranNoNotFound;
                            return Resp;
                        }
                        var TransactionReqData = await _TrnRequestCommonRepo.GetSingleAsync(_data => _data.TrnNo == IsExist.TrnNo);
                        if (TransactionReqData != null)
                        {
                            Resp.ReturnCode = enResponseCode.Fail;
                            Resp.ErrorCode = enErrorCode.RequestAlreadyProcessedByAdmin;
                            Resp.ReturnMsg = "This Transaction Is Already Processed";
                            return Resp;
                        }
                        if (bit == ApprovalStatus.Accept)
                        {
                            Newtransaction.IsVerifiedByAdmin = 1;
                            Newtransaction.StatusMsg = "Approved By Admin";
                            await _TransactionRepository.UpdateAsync(Newtransaction);

                            WithdrawalConfirmationRequest NewReq = new WithdrawalConfirmationRequest
                            {
                                RefNo = Newtransaction.GUID.ToString(),
                                TransactionBit = 1
                            };
                            //log request
                            HelperForLog.WriteLogIntoFile("AdminWithdrawalRequest", this.GetType().Name, "Request For TrnNo: " + Newtransaction.Id + " UserId : " + Newtransaction.MemberID + "IsRequestFromAdmin = 1");
                            var withdrawResponse = await _IWithdrawTransactionV1.WithdrawTransactionAPICallProcessAsync(NewReq, Newtransaction.MemberID, 1);
                            HelperForLog.WriteLogIntoFile("AdminWithdrawalRequest", this.GetType().Name, "Response For TrnNo: " + Newtransaction.Id + " Data : " + withdrawResponse);

                            if (withdrawResponse.ReturnCode == enResponseCodeService.Success)
                            {
                                //Update Admin Request Table Entry
                                IsExist.ApprovalDate = Helpers.UTC_To_IST();
                                IsExist.ApprovedBy = Userid;
                                IsExist.Remarks = remarks;
                                IsExist.Status = 1;
                                _WithdrawAdminReqCommonRepo.Update(IsExist);

                                Resp.ErrorCode = enErrorCode.Success;
                                Resp.ReturnCode = enResponseCode.Success;
                                Resp.ReturnMsg = "Withdrawal Transaction Processed Successfully.";
                                return Resp;
                            }
                            else
                            {
                                Resp.ErrorCode = withdrawResponse.ErrorCode;
                                Resp.ReturnCode = enResponseCode.Fail;
                                Resp.ReturnMsg = withdrawResponse.ReturnMsg;
                                return Resp;
                            }
                        }
                        else
                        {
                            //Update Transaction Queue
                            Newtransaction.IsVerifiedByAdmin = 9;
                            Newtransaction.StatusMsg = "Disapproved By Admin";
                            await _TransactionRepository.UpdateAsync(Newtransaction);

                            //Fail Transaction & Credit Process
                            await _IWithdrawTransactionV1.MarkTransactionOperatorFailv2(Newtransaction.StatusMsg, enErrorCode.WithdrawTrnRejectByAdmin, Newtransaction);

                            //Update Admin Request Table Entry
                            IsExist.ApprovalDate = Helpers.UTC_To_IST();
                            IsExist.ApprovedBy = Userid;
                            IsExist.Remarks = remarks;
                            IsExist.Status = 9;
                            _WithdrawAdminReqCommonRepo.Update(IsExist);

                            Resp.ErrorCode = enErrorCode.Success;
                            Resp.ReturnCode = enResponseCode.Success;
                            Resp.ReturnMsg = "Withdrawal Transaction Rejected Successfully.";
                            return Resp;
                        }
                    }
                }
                else
                {
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("AdminWithdrawalRequest", this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> GetToken()
        {
            BizResponseClass res = new BizResponseClass();
            try
            {
                TimeSpan diff = DateTime.Now - new DateTime(1970, 1, 1);
                var nonce = Convert.ToInt64(diff.TotalMilliseconds);

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                StringBuilder builder = new StringBuilder();
                foreach (KeyValuePair<string, string> pair in parameters)
                {
                    builder.Append(pair.Key).Append("=").Append(pair.Value).Append("&");
                }
                string queryString = builder.ToString().TrimEnd('&');

                var payload = new JwtPayload
                {
                    { "access_key", "Access Key" },
                    { "nonce", nonce },
                    { "query", queryString }
                };
                byte[] keyBytes = Encoding.Default.GetBytes("Secret Key");
                var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
                var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
                var header = new JwtHeader(credentials);
                var secToken = new JwtSecurityToken(header, payload);

                var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
                var authorizationToken = "Bearer " + jwtToken;

                return res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetToken", this.GetType().Name, ex);
                return null;
            }
        }

        public ListLPWalletMismatchRes ListLPWalletMismatch(DateTime FromDate, DateTime ToDate, int Page, int PageSize, long WalletId, long SerProID, Int16 Status)
        {
            try
            {
                ListLPWalletMismatchRes resp = new ListLPWalletMismatchRes();
                resp.PageNo = Page;
                resp.PageSize = PageSize;
                DateTime newToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                FromDate = FromDate.AddHours(0).AddMinutes(0).AddSeconds(0);
                var data = _controlPanelRepository.ListLPWalletMismatchM(FromDate, ToDate, Page, PageSize, WalletId, SerProID, Status);
                resp.TotalCount = data.TotalCount;
                if (data.Data.Count == 0)
                {
                    resp.Data = new List<LPWalletMismatchRes>();
                    resp.ReturnCode = enResponseCode.Fail;
                    resp.ReturnMsg = EnResponseMessage.NotFound;
                    resp.ErrorCode = enErrorCode.NotFound;
                }
                else
                {
                    resp.Data = data.Data;
                    resp.ReturnCode = enResponseCode.Success;
                    resp.ReturnMsg = EnResponseMessage.FindRecored;
                    resp.ErrorCode = enErrorCode.Success;
                }
                return resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<LPServiceProBalanceResponse> GetLPProviderBalance(long SerProID, string SMSCode, int GenerateMismatch)
        {
            List<Task<LPServiceProviderBalance>> responseCls = new List<Task<LPServiceProviderBalance>>();
            List<LPServiceProviderBalance> responseData = new List<LPServiceProviderBalance>();
            LPServiceProBalanceResponse response = new LPServiceProBalanceResponse();
            WalletTypeMaster WalletTypeobj = new WalletTypeMaster();
            try
            {
                IEnumerable<LPWalletMaster> WalletListobj;
                List<Task<LPServiceProviderBalance>> taskResult = new List<Task<LPServiceProviderBalance>>();

                WalletTypeobj = _WalletTypeMaster.GetSingle(x => x.WalletTypeName == SMSCode);
                if (WalletTypeobj == null)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.InvalidBaseCurrency;
                    response.ErrorCode = enErrorCode.InvalidSourceCurrency;
                    return response;
                }
                if (SerProID == 0)
                {
                    WalletListobj = _LPWalletMaster.FindBy(x => x.WalletTypeID == WalletTypeobj.Id);
                }
                else
                {
                    WalletListobj = _LPWalletMaster.FindBy(x => x.WalletTypeID == WalletTypeobj.Id && x.SerProID == SerProID);
                }
                foreach (LPWalletMaster subItem in WalletListobj)
                {
                    HelperForLog.WriteLogIntoFileAsync("GetLPProviderBalance", "WalletService", subItem.SerProID.ToString() + "ControlPanelService", " Currency " + subItem.WalletTypeID);
                    taskResult.Add(GetLPProviderBalanceMediatR(subItem.SerProID, subItem.WalletTypeID, subItem.Balance, subItem.Id, GenerateMismatch));
                }
                var testResult = await Task.WhenAll<LPServiceProviderBalance>(taskResult);

                response.Data = testResult.ToList();
                response.Data.RemoveAll(item => item == null);
                if (testResult.ToList().Count == 0)
                {
                    response.ReturnCode = enResponseCode.Fail;
                    response.ReturnMsg = EnResponseMessage.ProviderDataFetchFail;
                    response.ErrorCode = enErrorCode.NoRecordFound;
                }
                else
                {
                    response.ReturnCode = enResponseCode.Success;
                    response.ReturnMsg = EnResponseMessage.ProviderDataFetchSuccess;
                    response.ErrorCode = enErrorCode.Success;
                }
                return response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public async Task<LPServiceProviderBalance> GetLPProviderBalanceMediatR(long SerProID, long WalletTypeID, decimal balance, long walletid, int GenerateMismatch = 0)
        {
            LPBalanceCheck mediatrReq;
            WalletTypeMaster _Currency;
            LPServiceProviderBalance responseCls = new LPServiceProviderBalance();
            Core.ViewModels.Configuration.ServiceProviderViewModel providerViewModel = new ServiceProviderViewModel();
            try
            {
                _Currency = _WalletTypeMaster.GetSingle(x => x.Id == WalletTypeID);
                if (_Currency == null)
                {
                    return null;
                }
                providerViewModel = _transactionConfig.GetPoviderByID(SerProID);
                if (providerViewModel == null)
                {
                    return null;
                }
                mediatrReq = new LPBalanceCheck();
                mediatrReq.SerProID = SerProID;
                mediatrReq.Currency = _Currency.WalletTypeName;
                await _mediator.Send(mediatrReq);
                responseCls.Balance = mediatrReq.Balance;
                responseCls.CurrencyName = _Currency.WalletTypeName;
                responseCls.ProviderName = providerViewModel.ProviderName;
                responseCls.WalletBalance = balance;
                HelperForLog.WriteLogIntoFileAsync("GetLPProviderBalanceMediatR", "ControlPanelService", SerProID + " Currency " + WalletTypeID);
                if (balance != mediatrReq.Balance && GenerateMismatch == 1 && mediatrReq.Balance > 0) //ntrivedi 16-07-2019
                {
                    LPWalletMismatch lPWallet = new LPWalletMismatch();
                    lPWallet.CreatedBy = 99;
                    lPWallet.CreatedDate = Helpers.UTC_To_IST();
                    lPWallet.MismatchaingAmount = balance - mediatrReq.Balance;
                    lPWallet.Status = 0;
                    lPWallet.TPBalance = mediatrReq.Balance;
                    lPWallet.SystemBalance = balance;
                    lPWallet.WalletID = walletid;
                    lPWallet.Guid = Guid.NewGuid().ToString().Replace("-", "");
                    _mismatchReconRepository.Add(lPWallet);
                }
                return responseCls;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public BizResponseClass UpdatePayUCoinLimitConfiguration(ReqUpdatePayUCoinLimitConfiguration Req)
        {
            BizResponseClass Resp = new BizResponseClass();
            try
            {
                var wallettype = _WalletTypeMaster.GetSingle(t => t.WalletTypeName == Req.CurrencyName && t.Status == 1);
                if (wallettype == null)
                {
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.WalletTypeRecNotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                    return Resp;
                }
                var providerdata = _webApiRepository.GetProviderDataListAsync(new TransactionApiConfigurationRequest { SMSCode = wallettype.WalletTypeName.ToLower(), amount = 1, APIType = enWebAPIRouteType.TransactionAPI, trnType = Convert.ToInt32(enTrnType.UpdateCointLimit) });
                transactionProviderResponses = providerdata.GetAwaiter().GetResult();
                if (transactionProviderResponses == null || transactionProviderResponses.Count == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.ItemNotFoundForGenerateAddress, ReturnCode = enResponseCode.Fail, ReturnMsg = "Please try after sometime." };
                }
                if (transactionProviderResponses[0].ThirPartyAPIID == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                var apiconfig = _thirdPartyCommonRepository.GetById(transactionProviderResponses[0].ThirPartyAPIID);

                thirdPartyAPIConfiguration = apiconfig;
                if (thirdPartyAPIConfiguration == null || transactionProviderResponses.Count == 0)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.InvalidThirdpartyID, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }

                thirdPartyAPIRequest = _getWebRequest.MakeWebRequestERC20(transactionProviderResponses[0].RouteID, transactionProviderResponses[0].ThirPartyAPIID, transactionProviderResponses[0].SerProDetailID, "", "", "");

                thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#coin#", Req.CurrencyName.ToString());
                thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#minimum_amount#", Req.MinLimit.ToString());
                thirdPartyAPIRequest.RequestBody = thirdPartyAPIRequest.RequestBody.Replace("#maximum_amount#", Req.MaxLimit.ToString());

                string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.MethodType);
                //// parse response logic 

                WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses[0].ThirPartyAPIID);

                if (ParsedResponse.Status != enTransactionStatus.Success)
                {
                    return new BizResponseClass { ErrorCode = enErrorCode.API_ResponseFail, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail" };
                }

                Resp.ErrorCode = enErrorCode.Success;
                Resp.ReturnCode = enResponseCode.Success;
                Resp.ReturnMsg = EnResponseMessage.RecordUpdated;

                return Resp;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public Task<BizResponseClass> CreateTextFile(CreateTextFileReq textFileReq)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                string filePath = _configuration["CreateTextFilePath"];
                foreach (var urlReq in textFileReq.UrlReqList)
                {
                    string path = filePath + ((UrlType)urlReq.UrlType).ToString();
                    if (!Directory.Exists(path))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(path);
                    }
                    if (File.Exists(path + "/Master_" + textFileReq.Coin + ".txt") == false)
                    {
                        File.Create(path + "/Master_" + textFileReq.Coin + ".txt").Close();
                        File.SetAttributes(path + "/Master_" + textFileReq.Coin + ".txt", FileAttributes.Archive);                        
                    }
                    using (StreamWriter sw = File.AppendText(path + "/Master_" + textFileReq.Coin + ".txt"))
                    {
                        sw.WriteLine("APIName:" + textFileReq.Coin);
                        sw.WriteLine("TransactionFilePath:" + textFileReq.Coin + "_TransactionFile.txt");                        
                        sw.Flush();
                        sw.Close();
                    }
                    string newPath = path + "\\" + textFileReq.Coin;
                    if (!Directory.Exists(newPath))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(newPath);
                    }
                    if (File.Exists(newPath + "\\" + textFileReq.Coin + "_TransactionFile.txt") == false)
                    {
                        File.Create(newPath + "\\" + textFileReq.Coin + "_TransactionFile.txt").Close();
                        File.SetAttributes(newPath + "\\" + textFileReq.Coin + "_TransactionFile.txt", FileAttributes.Archive);                        
                    }
                    using (StreamWriter sw = File.AppendText(newPath + "\\" + textFileReq.Coin + "_TransactionFile.txt"))
                    {
                        string Authorization = textFileReq.AccessToken;
                        string RequestBody = urlReq.RequestBody;
                        sw.WriteLine("URL:" + urlReq.Url);
                        sw.WriteLine("RequestType:" + urlReq.ReqType);
                        sw.WriteLine("ContentType:application/json");
                        sw.WriteLine("Category:1");
                        sw.WriteLine("Authorization:" + Authorization);
                        sw.WriteLine("RequestBody:" + RequestBody);
                        sw.WriteLine("IsDescending:" + urlReq.IsDescending);
                        sw.Flush();
                        sw.Close();
                    }
                }
                Response.ReturnCode = enResponseCode.Success;
                Response.ErrorCode = enErrorCode.FileCreateSuccessfully;
                Response.ReturnMsg = EnResponseMessage.FileCreateSuccessfully;
                return Task.FromResult(Response);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                Response.ReturnCode = enResponseCode.InternalError;
                Response.ReturnMsg = EnResponseMessage.InternalError;
                Response.ErrorCode = enErrorCode.InternalError;
                return Task.FromResult(Response);
            }
        }
        //public Task<BizResponseClass> CreateTextFile(string URL, string UserName, string Coin, string WalletTypeName, int AppType)
        //{
        //    BizResponseClass Response = new BizResponseClass();
        //    try
        //    {
        //        // create text file for Deposite status check
        //        string depositePath = @"F:\Main\" + WalletTypeName + "_Depositstatuschek\\" + Coin;
        //        if (!Directory.Exists(depositePath))
        //        {
        //            DirectoryInfo di = Directory.CreateDirectory(depositePath);
        //        }
        //        if (File.Exists(depositePath + "/MasterFile_" + WalletTypeName + "_" + ((EnAppType)AppType).ToString() + ".txt") == false)
        //        {
        //            File.Create(depositePath + "\\" + Coin + "_TransactionFile" + ((EnAppType)AppType).ToString() + ".txt").Close();
        //            File.SetAttributes(depositePath + "\\" + Coin + "_TransactionFile" + ((EnAppType)AppType).ToString() + ".txt", FileAttributes.Archive);
        //            using (StreamWriter sw = File.AppendText(depositePath + "\\" + Coin + "_TransactionFile" + ((EnAppType)AppType).ToString() + ".txt"))
        //            {
        //                string Authorization = "IINI0Fgaqt6ysQHBShkIAzrJTZRHx1KWGb6ZpWURJBSQOfMIgwHmjziButu9mfa86e8";
        //                string RequestBody = "{\"sitename\": \"#Username#\", \"coin\":\"#AssetName#\", \"txn_id\" : \"#trnID#\"}";
        //                sw.WriteLine("URL:" + URL);
        //                sw.WriteLine("RequestType:POST");
        //                sw.WriteLine("ContentType:application/json");
        //                sw.WriteLine("Category:1");
        //                sw.WriteLine("Username:" + UserName);
        //                sw.WriteLine("Authorization:" + Authorization);
        //                sw.WriteLine("RequestBody:" + RequestBody);
        //                sw.WriteLine("confirmation:3");
        //                sw.Flush();
        //                sw.Close();
        //            }
        //        }

        //        // create text file for Withdraw app
        //        string withdrawPath = @"F:\Main\" + WalletTypeName + "_WithDrawApp\\" + Coin;
        //        if (!Directory.Exists(withdrawPath))
        //        {
        //            DirectoryInfo di = Directory.CreateDirectory(withdrawPath);
        //        }
        //        if (File.Exists(withdrawPath + "\\" + Coin + "_TransactionFile" + ((EnAppType)AppType).ToString() + ".txt") == false)
        //        {
        //            File.Create(withdrawPath + "\\" + Coin + "_TransactionFile" + ((EnAppType)AppType).ToString() + ".txt").Close();
        //            File.SetAttributes(withdrawPath + "\\" + Coin + "_TransactionFile" + ((EnAppType)AppType).ToString() + ".txt", FileAttributes.Archive);
        //            using (StreamWriter sw = File.AppendText(withdrawPath + "\\" + Coin + "_TransactionFile" + ((EnAppType)AppType).ToString() + ".txt"))
        //            {
        //                string Authorization = "IINI0Fgaqt6ysQHBShkIAzrJTZRHx1KWGb6ZpWURJBSQOfMIgwHmjziButu9mfa86e8";
        //                string RequestBody = "{\"sitename\": \"" + UserName + "\", \"coin\":\"#AssetName#\", \"orderId\" : \"#trnID#\"}";
        //                sw.WriteLine("URL:" + URL);
        //                sw.WriteLine("RequestType:POST");
        //                sw.WriteLine("ContentType:application/json");
        //                sw.WriteLine("Category:1");
        //                sw.WriteLine("Username:" + UserName);
        //                sw.WriteLine("Authorization:" + Authorization);
        //                sw.WriteLine("RequestBody:" + RequestBody);
        //                sw.WriteLine("confirmation:3");
        //                sw.Flush();
        //                sw.Close();
        //            }
        //        }

        //        // create text file for Wallet Deposite
        //        string walletDepoPath = @"F:\Main\" + WalletTypeName + "WalletDeposite\\" + Coin;
        //        if (!Directory.Exists(walletDepoPath))
        //        {
        //            DirectoryInfo di = Directory.CreateDirectory(walletDepoPath);
        //        }
        //        if (File.Exists(walletDepoPath + "\\" + Coin + "_TransactionFile" + ((EnAppType)AppType).ToString() + ".txt") == false)
        //        {
        //            File.Create(walletDepoPath + "\\" + Coin + "_TransactionFile" + ((EnAppType)AppType).ToString() + ".txt").Close();
        //            File.SetAttributes(walletDepoPath + "\\" + Coin + "_TransactionFile" + ((EnAppType)AppType).ToString() + ".txt", FileAttributes.Archive);
        //            using (StreamWriter sw = File.AppendText(walletDepoPath + "\\" + Coin + "_TransactionFile" + ((EnAppType)AppType).ToString() + ".txt"))
        //            {
        //                string Authorization = "IINI0Fgaqt6ysQHBShkIAzrJTZRHx1KWGb6ZpWURJBSQOfMIgwHmjziButu9mfa86e8";
        //                string RequestBody = "{\"sitename\": \"#UserName#\", \"coin\":\"#AssetName#\", \"page\" : \"#PageNo#\", \"limit\" : \"#Limit#\"}";
        //                sw.WriteLine("URL:" + URL);
        //                sw.WriteLine("RequestType:POST");
        //                sw.WriteLine("ContentType:application/json");
        //                sw.WriteLine("Category:1");
        //                sw.WriteLine("Username:" + UserName);
        //                sw.WriteLine("AssetName:" + Coin);
        //                sw.WriteLine("ConvertAmt:1");
        //                sw.WriteLine("Authorization:" + Authorization.ToString());
        //                sw.WriteLine("RequestBody:" + RequestBody);
        //                sw.WriteLine("IsDescending:0");
        //                sw.Flush();
        //                sw.Close();
        //            }
        //        }
        //        Response.ErrorCode = enErrorCode.Success;
        //        Response.ReturnCode = enResponseCode.Success;
        //        Response.ReturnMsg = EnResponseMessage.RecordUpdated;
        //        return Task.FromResult(Response);

        //    }
        //    catch (Exception ex)
        //    {
        //        HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
        //        Response.ReturnCode = enResponseCode.InternalError;
        //        Response.ReturnMsg = EnResponseMessage.InternalError;
        //        Response.ErrorCode = enErrorCode.InternalError;
        //        return Task.FromResult(Response);
        //    }
        //}

        #region User Suspicious Activity Report

        public async Task<UserWithdrawalResponse> GetTotalWithdrawalHistory(long UserId, string CurrencyName)
        {
            UserWithdrawalResponse Resp = new UserWithdrawalResponse();
            List<WithdrawalDetail> SuccessTransactions = new List<WithdrawalDetail>();
            List<WithdrawalDetail> HoldTransactions = new List<WithdrawalDetail>();
            try
            {
                var ExternalSuccessData = _controlPanelRepository.GetTotalWithdrawalHistory(UserId, Convert.ToInt16(EnWithdrwalInternalTransaction.OutSideTransaction), 1, CurrencyName);
                var InternalSuccessData = _controlPanelRepository.GetTotalWithdrawalHistory(UserId, Convert.ToInt16(EnWithdrwalInternalTransaction.InternalTransaction), 1, CurrencyName);
                var ExternalPendingData = _controlPanelRepository.GetTotalWithdrawalHistory(UserId, Convert.ToInt16(EnWithdrwalInternalTransaction.OutSideTransaction), 4, CurrencyName);
                var InternalPendingData = _controlPanelRepository.GetTotalWithdrawalHistory(UserId, Convert.ToInt16(EnWithdrwalInternalTransaction.InternalTransaction), 4, CurrencyName);

                List<UserWithdrawalDTO> ExternalSuccess_Data = await ExternalSuccessData;
                List<UserWithdrawalDTO> InternalSuccess_Data = await InternalSuccessData;

                List<UserWithdrawalDTO> ExternalPending_Data = await ExternalPendingData;
                List<UserWithdrawalDTO> InternalPending_Data = await InternalPendingData;
                int Count = ExternalSuccess_Data.Count + InternalSuccess_Data.Count + ExternalPending_Data.Count + InternalPending_Data.Count;
                if (Count > 0)
                {
                    if (ExternalSuccess_Data.Count > 0)
                    {
                        foreach (var x in ExternalSuccess_Data)
                        {
                            SuccessTransactions.Add(new WithdrawalDetail
                            {
                                CurrencyName = x.SMSCode,
                                ExternalTrnTotalAmt = x.TotalAmount,
                                TotalAmount = x.TotalAmount,
                                EquivalentUSDAmt = x.EquivalentUSDAmt
                            });
                        }
                    }
                    if (InternalSuccess_Data.Count() > 0)
                    {
                        foreach (var x in InternalSuccess_Data)
                        {
                            var IsExist = SuccessTransactions.Find(P => P.CurrencyName == x.SMSCode);
                            if (IsExist == null)
                            {
                                SuccessTransactions.Add(new WithdrawalDetail
                                {
                                    CurrencyName = x.SMSCode,
                                    ExternalTrnTotalAmt = x.TotalAmount,
                                    TotalAmount = x.TotalAmount,
                                    EquivalentUSDAmt = x.EquivalentUSDAmt
                                });
                            }
                            else
                            {
                                var ItemIndex = SuccessTransactions.FindIndex(P => P.CurrencyName == x.SMSCode);
                                var Item = SuccessTransactions.ElementAt(ItemIndex);
                                Item.InternalTrnTotalAmt = x.TotalAmount;
                                Item.TotalAmount += x.TotalAmount;
                            }
                        }
                    }

                    if (ExternalPending_Data.Count > 0)
                    {
                        foreach (var x in ExternalPending_Data)
                        {
                            HoldTransactions.Add(new WithdrawalDetail
                            {
                                CurrencyName = x.SMSCode,
                                ExternalTrnTotalAmt = x.TotalAmount,
                                TotalAmount = x.TotalAmount,
                                EquivalentUSDAmt = x.EquivalentUSDAmt
                            });
                        }
                    }
                    if (InternalPending_Data.Count() > 0)
                    {
                        foreach (var x in InternalPending_Data)
                        {
                            var IsExist = HoldTransactions.Find(P => P.CurrencyName == x.SMSCode);
                            if (IsExist == null)
                            {
                                HoldTransactions.Add(new WithdrawalDetail
                                {
                                    CurrencyName = x.SMSCode,
                                    ExternalTrnTotalAmt = x.TotalAmount,
                                    TotalAmount = x.TotalAmount,
                                    EquivalentUSDAmt = x.EquivalentUSDAmt
                                });
                            }
                            else
                            {
                                var ItemIndex = HoldTransactions.FindIndex(P => P.CurrencyName == x.SMSCode);
                                var Item = SuccessTransactions.ElementAt(ItemIndex);
                                Item.InternalTrnTotalAmt = x.TotalAmount;
                                Item.TotalAmount += x.TotalAmount;
                            }
                        }
                    }
                    Resp.SuccessTransactions = SuccessTransactions;
                    Resp.HoldTransactions = HoldTransactions;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.SuccessTransactions = SuccessTransactions;
                    Resp.HoldTransactions = HoldTransactions;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception Ex)
            {
                HelperForLog.WriteErrorLog("GetTotalWithdrawalHistory", this.GetType().Name, Ex);
                throw Ex;
            }
        }

        public async Task<UserWithdrawalResponse> GetTotalDepositionHistory(long UserId, string CurrencyName)
        {
            UserWithdrawalResponse Resp = new UserWithdrawalResponse();
            List<WithdrawalDetail> SuccessTransactions = new List<WithdrawalDetail>();
            List<WithdrawalDetail> HoldTransactions = new List<WithdrawalDetail>();
            try
            {
                var ExternalSuccessData = _controlPanelRepository.GetTotalDepositionHistory(UserId, 0, 1, CurrencyName);
                var InternalSuccessData = _controlPanelRepository.GetTotalDepositionHistory(UserId, 1, 1, CurrencyName);
                var ExternalPendingData = _controlPanelRepository.GetTotalDepositionHistory(UserId, 0, 4, CurrencyName);
                var InternalPendingData = _controlPanelRepository.GetTotalDepositionHistory(UserId, 1, 4, CurrencyName);

                List<UserWithdrawalDTO> ExternalSuccess_Data = await ExternalSuccessData;
                List<UserWithdrawalDTO> InternalSuccess_Data = await InternalSuccessData;

                List<UserWithdrawalDTO> ExternalPending_Data = await ExternalPendingData;
                List<UserWithdrawalDTO> InternalPending_Data = await InternalPendingData;
                int Count = ExternalSuccess_Data.Count + InternalSuccess_Data.Count + ExternalPending_Data.Count + InternalPending_Data.Count;
                if (Count > 0)
                {
                    if (ExternalSuccess_Data.Count > 0)
                    {
                        foreach (var x in ExternalSuccess_Data)
                        {
                            SuccessTransactions.Add(new WithdrawalDetail
                            {
                                CurrencyName = x.SMSCode,
                                ExternalTrnTotalAmt = x.TotalAmount,
                                TotalAmount = x.TotalAmount,
                                EquivalentUSDAmt = x.EquivalentUSDAmt
                            });
                        }
                    }
                    if (InternalSuccess_Data.Count() > 0)
                    {
                        foreach (var x in InternalSuccess_Data)
                        {
                            var IsExist = SuccessTransactions.Find(P => P.CurrencyName == x.SMSCode);
                            if (IsExist == null)
                            {
                                SuccessTransactions.Add(new WithdrawalDetail
                                {
                                    CurrencyName = x.SMSCode,
                                    ExternalTrnTotalAmt = x.TotalAmount,
                                    TotalAmount = x.TotalAmount,
                                    EquivalentUSDAmt = x.EquivalentUSDAmt
                                });
                            }
                            else
                            {
                                var ItemIndex = SuccessTransactions.FindIndex(P => P.CurrencyName == x.SMSCode);
                                var Item = SuccessTransactions.ElementAt(ItemIndex);
                                Item.InternalTrnTotalAmt = x.TotalAmount;
                                Item.TotalAmount += x.TotalAmount;
                            }
                        }
                    }

                    if (ExternalPending_Data.Count > 0)
                    {
                        foreach (var x in ExternalPending_Data)
                        {
                            HoldTransactions.Add(new WithdrawalDetail
                            {
                                CurrencyName = x.SMSCode,
                                ExternalTrnTotalAmt = x.TotalAmount,
                                TotalAmount = x.TotalAmount
                            });
                        }
                    }
                    if (InternalPending_Data.Count() > 0)
                    {
                        foreach (var x in InternalPending_Data)
                        {
                            var IsExist = HoldTransactions.Find(P => P.CurrencyName == x.SMSCode);
                            if (IsExist == null)
                            {
                                HoldTransactions.Add(new WithdrawalDetail
                                {
                                    CurrencyName = x.SMSCode,
                                    ExternalTrnTotalAmt = x.TotalAmount,
                                    TotalAmount = x.TotalAmount
                                });
                            }
                            else
                            {
                                var ItemIndex = HoldTransactions.FindIndex(P => P.CurrencyName == x.SMSCode);
                                var Item = SuccessTransactions.ElementAt(ItemIndex);
                                Item.InternalTrnTotalAmt = x.TotalAmount;
                                Item.TotalAmount += x.TotalAmount;
                            }
                        }
                    }
                    Resp.SuccessTransactions = SuccessTransactions;
                    Resp.HoldTransactions = HoldTransactions;
                    Resp.ReturnCode = enResponseCode.Success;
                    Resp.ErrorCode = enErrorCode.Success;
                    Resp.ReturnMsg = EnResponseMessage.FindRecored;
                }
                else
                {
                    Resp.SuccessTransactions = SuccessTransactions;
                    Resp.HoldTransactions = HoldTransactions;
                    Resp.ReturnCode = enResponseCode.Fail;
                    Resp.ErrorCode = enErrorCode.NotFound;
                    Resp.ReturnMsg = EnResponseMessage.NotFound;
                }
                return Resp;
            }
            catch (Exception Ex)
            {
                HelperForLog.WriteErrorLog("GetTotalDepositionHistory", this.GetType().Name, Ex);
                throw Ex;
            }
        }

        public async Task<UserAllWalletBalanceResp> GetAllWalletBalanceUserWise(long UserId, string CurrencyName)
        {
            UserAllWalletBalanceResp Response = new UserAllWalletBalanceResp();
            List<UserWalletBalance> MainWallet = new List<UserWalletBalance>();
            List<UserWalletBalance> ArbitrageWallet = new List<UserWalletBalance>();
            try
            {
                var MainWalletData = await _controlPanelRepository.GetAllWalletBalance(UserId, true, CurrencyName);
                var ArbitrageWalletData = await _controlPanelRepository.GetAllWalletBalance(UserId, false, CurrencyName);
                int count = MainWalletData.Count() + ArbitrageWalletData.Count();
                if (count <= 0)
                {
                    Response.ArbitrageWallet = ArbitrageWallet;
                    Response.MainWallet = MainWallet;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NotFound;
                    Response.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    if (MainWalletData != null && MainWalletData.Count > 0)
                    {
                        foreach (var x in MainWalletData)
                        {
                            MainWallet.Add(new UserWalletBalance
                            {
                                WalletType = x.WalletTypeName,
                                MainBalance = x.MainBalance,
                                OutBoundBalance = x.OutBoundBalance,
                                EquivalentUSDAmt = x.EquivalentUSDAmt
                            });
                        }
                    }
                    if (ArbitrageWalletData != null && ArbitrageWalletData.Count > 0)
                    {
                        foreach (var x in ArbitrageWalletData)
                        {
                            ArbitrageWallet.Add(new UserWalletBalance
                            {
                                WalletType = x.WalletTypeName,
                                MainBalance = x.MainBalance,
                                OutBoundBalance = x.OutBoundBalance,
                                EquivalentUSDAmt = x.EquivalentUSDAmt
                            });
                        }
                    }
                    Response.ArbitrageWallet = ArbitrageWallet;
                    Response.MainWallet = MainWallet;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Response;
            }
            catch (Exception Ex)
            {
                HelperForLog.WriteErrorLog("GetAllWalletBalanceUserWise", this.GetType().Name, Ex);
                throw Ex;
            }
        }

        public async Task<UserTradingSummaryResp> GetUserTradeSummary(long UserId)
        {
            UserTradingSummaryResp Response = new UserTradingSummaryResp();
            List<UserTradingSummary> BuyTrade = new List<UserTradingSummary>();
            List<UserTradingSummary> SellTrade = new List<UserTradingSummary>();
            try
            {
                var BuyTradeData = await _controlPanelRepository.GetUserTradingSummary(UserId, true);
                var SellTradeData = await _controlPanelRepository.GetUserTradingSummary(UserId, false);
                int count = BuyTradeData.Count() + SellTradeData.Count();
                if (count <= 0)
                {
                    Response.BuyTrade = BuyTrade;
                    Response.SellTrade = SellTrade;
                    Response.ReturnCode = enResponseCode.Fail;
                    Response.ErrorCode = enErrorCode.NotFound;
                    Response.ReturnMsg = EnResponseMessage.NotFound;
                }
                else
                {
                    if (BuyTradeData != null && BuyTradeData.Count > 0)
                    {
                        foreach (var x in BuyTradeData)
                        {
                            BuyTrade.Add(new UserTradingSummary
                            {
                                PairName = x.PairName,
                                OrderCurrency = x.Order_Currency,
                                DeliveryCurrency = x.Delivery_Currency,
                                TotalOrderQty = x.TotalOrderQty,
                                TotalDeliveryQty = x.TotalDeliveryQty,
                                TotalOrderQtyInUSD = x.TotalOrderQtyUSD,
                                TotalDeliveryQtyInUSD = x.TotalDeliveryQtyUSD
                            });
                        }
                    }
                    if (SellTradeData != null && SellTradeData.Count > 0)
                    {
                        foreach (var x in SellTradeData)
                        {
                            SellTrade.Add(new UserTradingSummary
                            {
                                PairName = x.PairName,
                                OrderCurrency = x.Order_Currency,
                                DeliveryCurrency = x.Delivery_Currency,
                                TotalOrderQty = x.TotalOrderQty,
                                TotalDeliveryQty = x.TotalDeliveryQty,
                                TotalOrderQtyInUSD = x.TotalOrderQtyUSD,
                                TotalDeliveryQtyInUSD = x.TotalDeliveryQtyUSD
                            });
                        }
                    }
                    Response.BuyTrade = BuyTrade;
                    Response.SellTrade = SellTrade;
                    Response.ReturnCode = enResponseCode.Success;
                    Response.ErrorCode = enErrorCode.Success;
                    Response.ReturnMsg = EnResponseMessage.FindRecored;
                }
                return Response;
            }
            catch (Exception Ex)
            {
                HelperForLog.WriteErrorLog("GetUserTradeSummary", this.GetType().Name, Ex);
                throw Ex;
            }
        }

        #endregion
    }
}
