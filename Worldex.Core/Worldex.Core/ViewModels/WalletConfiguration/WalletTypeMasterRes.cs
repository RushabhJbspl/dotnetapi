namespace Worldex.Core.ViewModels.WalletConfiguration
{
    public class WalletTypeMasterRes
    {
        public long CoinId { get; set; }

        public string CoinName { get; set; }

        public string Discription { get; set; }

        public short IsDepositionAllow { get; set; }

        public short IsWithdrawalAllow { get; set; }

        public short IsTransactionWallet { get; set; }

        public short Status { get; set; }
    }
}
