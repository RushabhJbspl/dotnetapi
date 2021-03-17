using Worldex.Core.ViewModels.Transaction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.Interfaces
{
    public interface IWithdrawProcessService
    {
        WithdrawERCAdminAddress GetWithdrawERCAdminAddress(long ServiceId, long ApiId);
    }
}
