namespace Worldex.Core.ViewModels.Wallet
{
    public class BalanceResponse
    {
        public decimal Balance { get; set; }
        public long WalletId { get; set; }
        public string WalletType { get; set; }
    }
}
