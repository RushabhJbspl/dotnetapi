using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.NewWallet;
using Worldex.Core.ViewModels.ControlPanel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Interfaces
{
    public interface IWalletConfiguration
    {
        List<WTrnTypeMaster> GetWTrnTypeMaster();
        List<TransactionPolicy> GetTransactionPolicy();
        List<WalletUsagePolicy> GetWalletUsagePolicy();
        List<AllowedChannels> GetAllowedChannels();
        List<TransactionBlockedChannel> GetTransactionBlockedChannel();
        List<CommissionTypeMaster> GetCommissionTypeMaster();
        List<UserRoleMaster> GetUserRoleMaster();
        List<UserTypeMaster> GetUserTypeMaster();
        List<OrganizationMaster> GetOrganizationMaster();
        List<CurrencyTypeMaster> GetCurrencyTypeMaster();
        List<ChargeTypeMaster> GetChargeTypeMaster();
        List<WalletPolicyAllowedDay> GetWalletPolicyAllowedDay();
        List<BlockWalletTrnTypeMaster> GetBlockWalletTrnTypeMaster();
        List<AllowTrnTypeRoleWise> GetAllowTrnTypeRoleWise();
        List<TradingChargeTypeMaster> GetTradingChargeTypeMaster();
        List<CronMaster> GetCronMaster();

        void UpdateCronMasterList();
        void UpdateWTrnTypeMasterList();
        void UpdateTransactionPolicyList();
        void UpdateWalletUsagePolicyList();
        void UpdateAllowedChannelsList();
        void UpdateTransactionBlockedChannelList();
        void UpdateCommissionTypeMasterList();
        void UpdateUserRoleMasterList();
        void UpdateUserTypeMasterList();
        void UpdateOrganizationMasterList();
        void UpdateCurrencyTypeMasterList();
        void UpdateChargeTypeMasterList();
        void UpdateWalletPolicyAllowedDayList();
        void UpdateBlockWalletTrnTypeMasterList();
        void UpdateAllowTrnTypeRoleWiseList();
        void UpdateTradingChargeTypeMasterList();

        bool IsValidChannel(long ChannelID, long TrnType);
        bool IsValidWallet(long WalletId, long TrnType, long WalletType);
        Task<bool> IsValidPolicy(PolicyCommonReq policy);
    }
}
