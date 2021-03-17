using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Transaction;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.Repository
{
    public interface IWithdrawTransactionRepository
    {
        BizResponseClass WithdrwalInteranlTransferProcess(string RefId, string timestamp, int ChannelId);
        List<WithdrawERCAdminAddress> GetERCAdminAddress(string Coin);
    }
}
