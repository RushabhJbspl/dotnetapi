using Worldex.Core.Entities.MarginEntitiesWallet;

namespace Worldex.Core.Interfaces.MarginWallet
{
    public interface IMarginWalletTQInsert
    {
        MarginWalletTransactionQueue AddIntoWalletTransactionQueue(MarginWalletTransactionQueue wtq, byte AddorUpdate);
    }
}
