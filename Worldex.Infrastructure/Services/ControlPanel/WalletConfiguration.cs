using Worldex.Core.Entities;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.NewWallet;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.ControlPanel
{
    public class WalletConfiguration : IWalletConfiguration
    {
        #region Variable
        private List<CronMaster> CronMasterList;
        private List<WTrnTypeMaster> WalletTrnTypeMasterList;
        private List<TransactionPolicy> TransactionPolicyList;
        private List<WalletUsagePolicy> WalletUsagePolicyList;
        private List<AllowedChannels> AllowedChannelsList;
        private List<TransactionBlockedChannel> TransactionBlockedChannelList;
        private List<WalletTypeMaster> WalletTypeMasterList;
        private List<OrganizationMaster> OrganizationMasterList;
        private List<UserTypeMaster> UserTypeMasterList;
        private List<UserRoleMaster> UserRoleMasterList;
        private List<CommissionTypeMaster> CommissionTypeMasterList;
        private List<ChargeTypeMaster> ChargeTypeMasterList;
        private List<CurrencyTypeMaster> CurrencyTypeMasterList;
        private List<WalletPolicyAllowedDay> WalletPolicyAllowedDayList;
        private List<BlockWalletTrnTypeMaster> BlockWalletTrnTypeMasterList;
        private List<AllowTrnTypeRoleWise> AllowTrnTypeRoleWiseList;
        private List<TradingChargeTypeMaster> TradingChargeTypeMasterList;
        
        private IWalletRepository _walletRepository;

        private IMemoryCache _cache { get; set; }
        private readonly ICommonRepository<WTrnTypeMaster> _WalletTrnTypeMaster;
        private readonly ICommonRepository<TradingChargeTypeMaster> _TradingChargeTypeMaster;
        private readonly ICommonRepository<TransactionPolicy> _TransactionPolicy;
        private readonly ICommonRepository<WalletUsagePolicy> _WalletUsagePolicy;
        private readonly ICommonRepository<AllowedChannels> _AllowedChannels;
        private readonly ICommonRepository<TransactionBlockedChannel> _TransactionBlockedChannel;
        private readonly ICommonRepository<WalletTypeMaster> _WalletTypeMaster;
        private readonly ICommonRepository<OrganizationMaster> _OrganizationMaster;
        private readonly ICommonRepository<UserTypeMaster> _UserTypeMaster;
        private readonly ICommonRepository<UserRoleMaster> _UserRoleMaster;
        private readonly ICommonRepository<CommissionTypeMaster> _CommissionTypeMaster;
        private readonly ICommonRepository<ChargeTypeMaster> _ChargeTypeMaster;
        private readonly ICommonRepository<CurrencyTypeMaster> _CurrencyTypeMaster;
        private readonly ICommonRepository<WalletPolicyAllowedDay> _WalletPolicyAllowedDay;
        private readonly ICommonRepository<BlockWalletTrnTypeMaster> _BlockWalletTrnTypeMaster;
        private readonly ICommonRepository<WalletMaster> _WalletMaster;
        private readonly ICommonRepository<UserWalletBlockTrnTypeMaster> _UserWalletBlockTrnTypeMaster;
        private readonly ICommonRepository<AllowTrnTypeRoleWise> _AllowTrnTypeRoleWise;
        private readonly ICommonRepository<CronMaster> _CronMaster;

        #endregion

        #region Cotr
        public WalletConfiguration(ICommonRepository<WTrnTypeMaster> WalletTrnTypeMaster,
            ICommonRepository<TransactionPolicy> TransactionPolicy,
            ICommonRepository<WalletUsagePolicy> WalletUsagePolicy,
            ICommonRepository<AllowedChannels> AllowedChannels,
            ICommonRepository<TransactionBlockedChannel> TransactionBlockedChannel,
            ICommonRepository<WalletTypeMaster> WalletTypeMaster,
            ICommonRepository<OrganizationMaster> OrganizationMaster,
            ICommonRepository<UserTypeMaster> UserTypeMaster,
            ICommonRepository<UserRoleMaster> UserRoleMaster,
            ICommonRepository<CommissionTypeMaster> CommissionTypeMaster,
            ICommonRepository<ChargeTypeMaster> ChargeTypeMaster,
            ICommonRepository<CurrencyTypeMaster> CurrencyTypeMaster,
            ICommonRepository<WalletPolicyAllowedDay> WalletPolicyAllowedDay,
            ICommonRepository<BlockWalletTrnTypeMaster> BlockWalletTrnTypeMaster,
            ICommonRepository<WalletMaster> WalletMaster,
            ICommonRepository<UserWalletBlockTrnTypeMaster> UserWalletBlockTrnTypeMaster,
            ICommonRepository<AllowTrnTypeRoleWise> AllowTrnTypeRoleWise,
            IWalletRepository walletRepository, ICommonRepository<TradingChargeTypeMaster> TradingChargeTypeMaster, ICommonRepository<CronMaster> CronMaster,
             IMemoryCache cache
           )
        {
            _WalletTrnTypeMaster = WalletTrnTypeMaster;
            _TransactionPolicy = TransactionPolicy;
            _WalletUsagePolicy = WalletUsagePolicy;
            _AllowedChannels = AllowedChannels;
            _TransactionBlockedChannel = TransactionBlockedChannel;
            _WalletTypeMaster = WalletTypeMaster;
            _OrganizationMaster = OrganizationMaster;
            _UserTypeMaster = UserTypeMaster;
            _UserRoleMaster = UserRoleMaster;
            _CommissionTypeMaster = CommissionTypeMaster;
            _ChargeTypeMaster = ChargeTypeMaster;
            _CurrencyTypeMaster = CurrencyTypeMaster;
            _WalletPolicyAllowedDay = WalletPolicyAllowedDay;
            _BlockWalletTrnTypeMaster = BlockWalletTrnTypeMaster;
            _WalletMaster = WalletMaster;
            _UserWalletBlockTrnTypeMaster = UserWalletBlockTrnTypeMaster;
            _AllowTrnTypeRoleWise = AllowTrnTypeRoleWise;
            _cache = cache;
            _walletRepository = walletRepository;
            _TradingChargeTypeMaster = TradingChargeTypeMaster;
            _CronMaster = CronMaster;
            //WalletTrnTypeMasterList = _WalletTrnTypeMaster.List();
            //TransactionPolicyList = _TransactionPolicy.List();
            //WalletUsagePolicyList = _WalletUsagePolicy.List();
            //AllowedChannelsList = _AllowedChannels.List();
            //TransactionBlockedChannelList = TransactionBlockedChannel.List();
            //WalletTypeMasterList = _WalletTypeMaster.List();
            //OrganizationMasterList = _OrganizationMaster.List();
            //UserTypeMasterList = _UserTypeMaster.List();
            //UserRoleMasterList = _UserRoleMaster.List();
            //CommissionTypeMasterList = _CommissionTypeMaster.List();
            //ChargeTypeMasterList = _ChargeTypeMaster.List();
            //CurrencyTypeMasterList = _CurrencyTypeMaster.List();
            //WalletPolicyAllowedDayList = _WalletPolicyAllowedDay.List();
            //BlockWalletTrnTypeMasterList = _BlockWalletTrnTypeMaster.List();
            //AllowTrnTypeRoleWiseList = _AllowTrnTypeRoleWise.List();

            CronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            if (CronMasterList == null)
            {
                _cache.Set("CronMaster", _CronMaster.List());
            }

            TradingChargeTypeMasterList = _cache.Get<List<TradingChargeTypeMaster>>("TradingChargeTypeMaster");
            if (TradingChargeTypeMasterList == null)
            {
                _cache.Set("TradingChargeTypeMaster", _TradingChargeTypeMaster.List());
            }

            WalletTrnTypeMasterList = _cache.Get<List<WTrnTypeMaster>>("WTrnTypeMaster");
            if (WalletTrnTypeMasterList == null)
            {
                _cache.Set("WTrnTypeMaster", _WalletTrnTypeMaster.List());
            }

            TransactionPolicyList = _cache.Get<List<TransactionPolicy>>("TransactionPolicy");
            if (TransactionPolicyList == null)
            {
                _cache.Set("TransactionPolicy", _TransactionPolicy.List());
            }

            WalletUsagePolicyList = _cache.Get<List<WalletUsagePolicy>>("WalletUsagePolicy");
            if (WalletUsagePolicyList == null)
            {
                _cache.Set("WalletUsagePolicy", _WalletUsagePolicy.List());
            }

            AllowedChannelsList = _cache.Get<List<AllowedChannels>>("AllowedChannels");
            if (AllowedChannelsList == null)
            {
                _cache.Set("AllowedChannels", _AllowedChannels.List());
            }

            TransactionBlockedChannelList = _cache.Get<List<TransactionBlockedChannel>>("TransactionBlockedChannel");
            if (TransactionBlockedChannelList == null)
            {
                _cache.Set("TransactionBlockedChannel", _TransactionBlockedChannel.List());
            }

            WalletTypeMasterList = _cache.Get<List<WalletTypeMaster>>("WalletTypeMaster");
            if (WalletTypeMasterList == null)
            {
                _cache.Set("WalletTypeMaster", _WalletTypeMaster.List());
            }

            OrganizationMasterList = _cache.Get<List<OrganizationMaster>>("OrganizationMaster");
            if (OrganizationMasterList == null)
            {
                _cache.Set("OrganizationMaster", _OrganizationMaster.List());
            }

            UserTypeMasterList = _cache.Get<List<UserTypeMaster>>("UserTypeMaster");
            if (UserTypeMasterList == null)
            {
                _cache.Set("UserTypeMaster", _UserTypeMaster.List());
            }

            UserTypeMasterList = _cache.Get<List<UserTypeMaster>>("UserTypeMaster");
            if (UserTypeMasterList == null)
            {
                _cache.Set("UserTypeMaster", _UserTypeMaster.List());
            }

            UserRoleMasterList = _cache.Get<List<UserRoleMaster>>("UserRoleMaster");
            if (UserRoleMasterList == null)
            {
                _cache.Set("UserRoleMaster", _UserRoleMaster.List());
            }

            CommissionTypeMasterList = _cache.Get<List<CommissionTypeMaster>>("CommissionTypeMaster");
            if (CommissionTypeMasterList == null)
            {
                _cache.Set("CommissionTypeMaster", _CommissionTypeMaster.List());
            }

            ChargeTypeMasterList = _cache.Get<List<ChargeTypeMaster>>("ChargeTypeMaster");
            if (ChargeTypeMasterList == null)
            {
                _cache.Set("ChargeTypeMaster", _ChargeTypeMaster.List());
            }

            CurrencyTypeMasterList = _cache.Get<List<CurrencyTypeMaster>>("CurrencyTypeMaster");
            if (CurrencyTypeMasterList == null)
            {
                _cache.Set("CurrencyTypeMaster", _CurrencyTypeMaster.List());
            }

            WalletPolicyAllowedDayList = _cache.Get<List<WalletPolicyAllowedDay>>("WalletPolicyAllowedDay");
            if (WalletPolicyAllowedDayList == null)
            {
                _cache.Set("WalletPolicyAllowedDay", _WalletPolicyAllowedDay.List());
            }

            BlockWalletTrnTypeMasterList = _cache.Get<List<BlockWalletTrnTypeMaster>>("BlockWalletTrnTypeMaster");
            if (BlockWalletTrnTypeMasterList == null)
            {
                _cache.Set("BlockWalletTrnTypeMaster", _BlockWalletTrnTypeMaster.List());
            }

            AllowTrnTypeRoleWiseList = _cache.Get<List<AllowTrnTypeRoleWise>>("AllowTrnTypeRoleWise");
            if (AllowTrnTypeRoleWiseList == null)
            {
                _cache.Set("BlockWalletTrnTypeMaster", _AllowTrnTypeRoleWise.List());
            }

            //List<ServiceMaster> serviceMasterList1 = _cache.Get<List<ServiceMaster>>("ServiceMaster");
            //if (serviceMasterList1 == null)
            //{
            //    var Result = _serviceMasterRepo.List();
            //    _cache.Set<List<ServiceMaster>>("ServiceMaster", Result);
            //}
        }

        #endregion

        #region Listing Method

        public List<CronMaster> GetCronMaster()
        {
            CronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            return CronMasterList;
        }

        public List<TradingChargeTypeMaster> GetTradingChargeTypeMaster()
        {
            TradingChargeTypeMasterList = _cache.Get<List<TradingChargeTypeMaster>>("TradingChargeTypeMaster");
            return TradingChargeTypeMasterList;
        }

        public List<WTrnTypeMaster> GetWTrnTypeMaster()
        {
            WalletTrnTypeMasterList = _cache.Get<List<WTrnTypeMaster>>("WTrnTypeMaster");
            return WalletTrnTypeMasterList;
        }

        public List<TransactionPolicy> GetTransactionPolicy()
        {
            TransactionPolicyList = _cache.Get<List<TransactionPolicy>>("TransactionPolicy");
            return TransactionPolicyList;
        }

        public List<WalletUsagePolicy> GetWalletUsagePolicy()
        {
            WalletUsagePolicyList = _cache.Get<List<WalletUsagePolicy>>("WalletUsagePolicy");
            return WalletUsagePolicyList;
        }

        public List<AllowedChannels> GetAllowedChannels()
        {
            AllowedChannelsList = _cache.Get<List<AllowedChannels>>("AllowedChannels");
            return AllowedChannelsList;
        }

        public List<TransactionBlockedChannel> GetTransactionBlockedChannel()
        {
            TransactionBlockedChannelList = _cache.Get<List<TransactionBlockedChannel>>("TransactionBlockedChannel");
            return TransactionBlockedChannelList;
        }

        public List<WalletTypeMaster> GetWalletTypeMaster()
        {
            WalletTypeMasterList = _cache.Get<List<WalletTypeMaster>>("WalletTypeMaster");
            return WalletTypeMasterList;
        }

        public List<OrganizationMaster> GetOrganizationMaster()
        {
            OrganizationMasterList = _cache.Get<List<OrganizationMaster>>("OrganizationMaster");
            return OrganizationMasterList;
        }

        public List<UserTypeMaster> GetUserTypeMaster()
        {
            UserTypeMasterList = _cache.Get<List<UserTypeMaster>>("UserTypeMaster");
            return UserTypeMasterList;
        }

        public List<UserRoleMaster> GetUserRoleMaster()
        {
            UserRoleMasterList = _cache.Get<List<UserRoleMaster>>("UserRoleMaster");
            return UserRoleMasterList;
        }

        public List<CommissionTypeMaster> GetCommissionTypeMaster()
        {
            CommissionTypeMasterList = _cache.Get<List<CommissionTypeMaster>>("CommissionTypeMaster");
            return CommissionTypeMasterList;
        }

        public List<ChargeTypeMaster> GetChargeTypeMaster()
        {
            ChargeTypeMasterList = _cache.Get<List<ChargeTypeMaster>>("ChargeTypeMaster");
            return ChargeTypeMasterList;
        }

        public List<CurrencyTypeMaster> GetCurrencyTypeMaster()
        {
            CurrencyTypeMasterList = _cache.Get<List<CurrencyTypeMaster>>("CurrencyTypeMaster");
            return CurrencyTypeMasterList;
        }

        public List<WalletPolicyAllowedDay> GetWalletPolicyAllowedDay()
        {
            WalletPolicyAllowedDayList = _cache.Get<List<WalletPolicyAllowedDay>>("WalletPolicyAllowedDay");
            return WalletPolicyAllowedDayList;
        }

        public List<BlockWalletTrnTypeMaster> GetBlockWalletTrnTypeMaster()
        {
            BlockWalletTrnTypeMasterList = _cache.Get<List<BlockWalletTrnTypeMaster>>("BlockWalletTrnTypeMaster");
            return BlockWalletTrnTypeMasterList;
        }

        public List<AllowTrnTypeRoleWise> GetAllowTrnTypeRoleWise()
        {
            AllowTrnTypeRoleWiseList = _cache.Get<List<AllowTrnTypeRoleWise>>("AllowTrnTypeRoleWise");
            return AllowTrnTypeRoleWiseList;
        }
        #endregion

        #region Update Method

        public void UpdateCronMasterList()
        {
            CronMasterList = _cache.Set("CronMaster", _CronMaster.List());
        }

        public void UpdateWTrnTypeMasterList()
        {
            WalletTrnTypeMasterList = _cache.Set("WTrnTypeMaster", _WalletTrnTypeMaster.List());
        }

        public void UpdateTransactionPolicyList()
        {
            TransactionPolicyList = _cache.Set("TransactionPolicy", _TransactionPolicy.List());
        }

        public void UpdateWalletUsagePolicyList()
        {
            WalletUsagePolicyList = _cache.Set("WalletUsagePolicy", _WalletUsagePolicy.List());
        }

        public void UpdateAllowedChannelsList()
        {
            AllowedChannelsList = _cache.Set("AllowedChannels", _AllowedChannels.List());
        }

        public void UpdateTransactionBlockedChannelList()
        {
            TransactionBlockedChannelList = _cache.Set("TransactionBlockedChannel", _TransactionBlockedChannel.List());
        }

        public void UpdateWalletTypeMasterList()
        {
            WalletTypeMasterList = _cache.Set("WalletTypeMaster", _WalletTypeMaster.List());
        }

        public void UpdateOrganizationMasterList()
        {
            OrganizationMasterList = _cache.Set("OrganizationMaster", _OrganizationMaster.List());
        }

        public void UpdateUserTypeMasterList()
        {
            UserTypeMasterList = _cache.Set("UserTypeMaster", _UserTypeMaster.List());
        }

        public void UpdateUserRoleMasterList()
        {
            UserRoleMasterList = _cache.Set("UserRoleMaster", _UserRoleMaster.List());
        }

        public void UpdateCommissionTypeMasterList()
        {
            CommissionTypeMasterList = _cache.Set("CommissionTypeMaster", _CommissionTypeMaster.List());
        }

        public void UpdateChargeTypeMasterList()
        {
            ChargeTypeMasterList = _cache.Set("ChargeTypeMaster", _ChargeTypeMaster.List());
        }

        public void UpdateCurrencyTypeMasterList()
        {
            CurrencyTypeMasterList = _cache.Set("CurrencyTypeMaster", _CurrencyTypeMaster.List());
        }

        public void UpdateWalletPolicyAllowedDayList()
        {
            WalletPolicyAllowedDayList = _cache.Set("WalletPolicyAllowedDay", _WalletPolicyAllowedDay.List());
        }

        public void UpdateBlockWalletTrnTypeMasterList()
        {
            BlockWalletTrnTypeMasterList = _cache.Set("BlockWalletTrnTypeMaster", _BlockWalletTrnTypeMaster.List());
        }

        public void UpdateAllowTrnTypeRoleWiseList()
        {
            AllowTrnTypeRoleWiseList = _cache.Set("AllowTrnTypeRoleWise", _AllowTrnTypeRoleWise.List());
        }

        public void UpdateTradingChargeTypeMasterList()
        {
            TradingChargeTypeMasterList = _cache.Set("TradingChargeTypeMaster", _TradingChargeTypeMaster.List());
        }
        #endregion

        #region Other method

        //2018-12-18
        public bool IsValidChannel(long ChannelID, long TrnType)
        {
            try
            {
                var allowedChannelsObj = GetAllowedChannels().Where(i => i.Status == 1 && i.ChannelID == ChannelID).FirstOrDefault();//check entry for master tbl

                var trnBlockChannelObj = GetTransactionBlockedChannel().Where(i => i.TrnType == TrnType && i.ChannelID == ChannelID && i.Status == 1).FirstOrDefault();//check entry for allow trn/channel(status=0/9)

                if (allowedChannelsObj == null || trnBlockChannelObj != null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        //2018-12-18
        public bool IsValidWallet(long WalletId, long TrnType, long WalletType)
        {
            try
            {
                var blockWalletTrntypeObj = GetBlockWalletTrnTypeMaster().Where(i => i.TrnTypeID == TrnType && i.WalletTypeID == WalletType && i.Status == 1).FirstOrDefault();

                var walletObj = _WalletMaster.GetSingleAsync(i => i.Id == WalletId && i.Status == 1);

                var walletBlockTrnTypeObj = _UserWalletBlockTrnTypeMaster.GetSingleAsync(i => i.WalletID == WalletId && i.Status == 1);

                var wTrnTypeObj = GetWTrnTypeMaster().Where(i => i.TrnTypeId == TrnType && i.Status == 1).FirstOrDefault();

                var walletTypeObj = GetWalletTypeMaster().Where(i => i.Id == WalletType && i.Status == 1).FirstOrDefault();

                if (blockWalletTrntypeObj != null || wTrnTypeObj == null || walletTypeObj == null || walletObj == null && walletBlockTrnTypeObj != null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }

        //2018-12-18
        public async Task<bool> IsValidPolicy(PolicyCommonReq policy)
        {
            try
            {
                TransactionPolicy trnPolicyObj = new TransactionPolicy();
                WalletUsagePolicy walletTypePolicyObj = new WalletUsagePolicy();
                var SumAmtTrnType = _walletRepository.GetSumForPolicy(0, policy.TrnType).GetAwaiter().GetResult();
                var SumAmtWalletType = _walletRepository.GetSumForPolicy(policy.WalletType, 0).GetAwaiter().GetResult();

                trnPolicyObj = GetTransactionPolicy().Where(i => i.TrnType == policy.TrnType).FirstOrDefault();

                walletTypePolicyObj = GetWalletUsagePolicy().Where(i => i.WalletType == policy.WalletType).FirstOrDefault();

                if (trnPolicyObj == null && walletTypePolicyObj == null)
                {
                    return false;
                }
                else if (trnPolicyObj != null && walletTypePolicyObj != null)
                {
                    if (!trnPolicyObj.AllowedIP.Contains("*") && !trnPolicyObj.AllowedLocation.Contains("*") && !walletTypePolicyObj.AllowedIP.Contains("*") && !walletTypePolicyObj.AllowedLocation.Contains("*"))
                    {
                        if (trnPolicyObj.AllowedIP != policy.AllowedIP && trnPolicyObj.AllowedLocation != policy.AllowedLocation && walletTypePolicyObj.AllowedIP != policy.AllowedIP && walletTypePolicyObj.AllowedLocation != policy.AllowedLocation)
                        {
                            return false;
                        }
                    }
                    DateTime myUTC = DateTime.UtcNow;
                    DateTime istdate = Helpers.UTC_To_IST();
                    TimeSpan TodayTime = istdate.TimeOfDay;

                    DateTime dtMStartTrnType = new DateTime();
                    DateTime dtMEndTrnType = new DateTime();
                    DateTime dtMStartWalletType = new DateTime();//for another obj
                    DateTime dtMEndWalletType = new DateTime();
                    dtMStartTrnType = dtMStartTrnType.AddSeconds(Convert.ToDouble(trnPolicyObj.StartTime)).ToLocalTime();
                    dtMEndTrnType = dtMEndTrnType.AddSeconds(Convert.ToDouble(trnPolicyObj.EndTime)).ToLocalTime();
                    dtMStartWalletType = dtMStartWalletType.AddSeconds(Convert.ToDouble(walletTypePolicyObj.StartTime)).ToLocalTime();
                    dtMEndWalletType = dtMEndWalletType.AddSeconds(Convert.ToDouble(walletTypePolicyObj.EndTime)).ToLocalTime();
                    TimeSpan MStartTime = dtMStartTrnType.TimeOfDay;
                    TimeSpan MEndTime = dtMEndTrnType.TimeOfDay;
                    TimeSpan MStartTimeWalletType = dtMStartWalletType.TimeOfDay;
                    TimeSpan MEndTimeWalletType = dtMEndWalletType.TimeOfDay;
                    if (TodayTime <= MStartTime && TodayTime >= MEndTime && TodayTime <= MStartTimeWalletType && TodayTime >= MEndTimeWalletType)
                    {
                        return false;
                    }

                    if ((policy.DailyTrnAmount <= trnPolicyObj.DailyTrnAmount || trnPolicyObj.DailyTrnAmount == 0)
                        && (policy.WeeklyTrnAmount <= trnPolicyObj.WeeklyTrnAmount || trnPolicyObj.WeeklyTrnAmount == 0)
                        && (policy.MonthlyTrnAmount <= trnPolicyObj.MonthlyTrnAmount || trnPolicyObj.MonthlyTrnAmount == 0)
                        && (policy.YearlyTrnAmount <= trnPolicyObj.YearlyTrnAmount || trnPolicyObj.YearlyTrnAmount == 0)
                        && (policy.DailyTrnAmount <= walletTypePolicyObj.DailyTrnAmount || walletTypePolicyObj.DailyTrnAmount == 0)
                        && (policy.WeeklyTrnAmount <= walletTypePolicyObj.WeeklyTrnAmount || walletTypePolicyObj.WeeklyTrnAmount == 0)
                        && (policy.MonthlyTrnAmount <= walletTypePolicyObj.MonthlyTrnAmount || walletTypePolicyObj.MonthlyTrnAmount == 0)
                        && (policy.YearlyTrnAmount <= walletTypePolicyObj.YearlyTrnAmount || walletTypePolicyObj.YearlyTrnAmount == 0)
                        && (policy.HourlyTrnAmount <= walletTypePolicyObj.HourlyTrnAmount || walletTypePolicyObj.HourlyTrnAmount == 0)
                        && (policy.LifeTimeTrnAmount <= walletTypePolicyObj.LifeTimeTrnAmount || walletTypePolicyObj.LifeTimeTrnAmount == 0)
                        && (policy.DailyTrnCount <= trnPolicyObj.DailyTrnCount || trnPolicyObj.DailyTrnCount == 0)
                        && (policy.WeeklyTrnCount <= trnPolicyObj.WeeklyTrnCount || trnPolicyObj.WeeklyTrnCount == 0)
                        && (policy.MonthlyTrnCount <= trnPolicyObj.MonthlyTrnCount || trnPolicyObj.MonthlyTrnCount == 0)
                        && (policy.YearlyTrnCount <= trnPolicyObj.YearlyTrnCount || trnPolicyObj.YearlyTrnCount == 0)
                        && (policy.DailyTrnCount <= walletTypePolicyObj.DailyTrnCount || walletTypePolicyObj.DailyTrnCount == 0)
                        && (policy.WeeklyTrnCount <= walletTypePolicyObj.WeeklyTrnCount || walletTypePolicyObj.WeeklyTrnCount == 0)
                        && (policy.MonthlyTrnCount <= walletTypePolicyObj.MonthlyTrnCount || walletTypePolicyObj.MonthlyTrnCount == 0)
                        && (policy.YearlyTrnCount <= walletTypePolicyObj.YearlyTrnCount || walletTypePolicyObj.YearlyTrnCount == 0)
                        && (policy.HourlyTrnCount <= walletTypePolicyObj.HourlyTrnCount || walletTypePolicyObj.HourlyTrnCount == 0)
                        && (policy.LifeTimeTrnCount <= walletTypePolicyObj.LifeTimeTrnCount || walletTypePolicyObj.LifeTimeTrnCount == 0))
                    {
                        return true;
                    }
                    else
                    {
                        if ((SumAmtTrnType.DailyAmount + policy.DailyTrnAmount) <= trnPolicyObj.DailyTrnAmount && 
                            (SumAmtTrnType.WeeklyAmount + policy.WeeklyTrnAmount) <= trnPolicyObj.WeeklyTrnAmount && 
                            (SumAmtTrnType.MonthlyAmount + policy.MonthlyTrnAmount) <= trnPolicyObj.MonthlyTrnAmount && 
                            (SumAmtTrnType.YearlyAmount + policy.YearlyTrnAmount) <= trnPolicyObj.YearlyTrnAmount && 
                            (SumAmtTrnType.DailyCount +1) <= trnPolicyObj.DailyTrnCount &&
                            (SumAmtTrnType.WeeklyCount + 1) <= trnPolicyObj.WeeklyTrnCount &&
                            (SumAmtTrnType.MonthlyCount + 1) <= trnPolicyObj.MonthlyTrnCount &&
                            (SumAmtTrnType.YearlyCount + 1) <= trnPolicyObj.YearlyTrnCount &&
                            (SumAmtWalletType.DailyAmount + policy.DailyTrnAmount) <= walletTypePolicyObj.DailyTrnAmount &&
                            (SumAmtWalletType.WeeklyAmount + policy.WeeklyTrnAmount) <= walletTypePolicyObj.WeeklyTrnAmount &&
                            (SumAmtWalletType.MonthlyAmount + policy.MonthlyTrnAmount) <= walletTypePolicyObj.MonthlyTrnAmount &&
                            (SumAmtWalletType.YearlyAmount + policy.YearlyTrnAmount) <= walletTypePolicyObj.YearlyTrnAmount &&
                            (SumAmtWalletType.HourlyAmount + policy.HourlyTrnAmount) <= walletTypePolicyObj.HourlyTrnAmount &&
                            (SumAmtWalletType.LifeTimeAmount + policy.LifeTimeTrnAmount) <= walletTypePolicyObj.LifeTimeTrnAmount &&
                            (SumAmtWalletType.DailyCount + 1) <= walletTypePolicyObj.DailyTrnCount &&
                            (SumAmtWalletType.WeeklyCount + 1) <= walletTypePolicyObj.WeeklyTrnCount &&
                            (SumAmtWalletType.MonthlyCount + 1) <= walletTypePolicyObj.MonthlyTrnCount &&
                            (SumAmtWalletType.YearlyCount + 1) <= walletTypePolicyObj.YearlyTrnCount &&
                            (SumAmtWalletType.HourlyCount + 1) <= walletTypePolicyObj.HourlyTrnCount &&
                            (SumAmtWalletType.LifeTimeCount + 1) <= walletTypePolicyObj.LifeTimeTrnCount)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return false;
            }
        }
        #endregion

    }
}
