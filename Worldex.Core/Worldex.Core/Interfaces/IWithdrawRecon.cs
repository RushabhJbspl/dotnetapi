using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.Transaction.BackOffice;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces
{
    public interface IWithdrawRecon
    {
        Task<BizResponseClass> WithdrawalReconV1(WithdrawalReconRequest request, long UserId, string accessToken);

        void TransactionReconEntry(long TrnNo, enTransactionStatus NewStatus, short OldStatus, long SerProID, long ServiceID, string Remarks, long UserID);
    }
}
