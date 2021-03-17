using Worldex.Core.Entities.NewWallet;
using Worldex.Core.Entities.Wallet;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class BGTaskAddressGeneration
    {
        public UserActivityLog userActivityLog { get; set; }
        public AddressMaster Address { get; set; }
        public string Token { get; set; }
        public string Coin { get; set; }
        public string WalletName { get; set; }
        public string UID { get; set; }
        public decimal DestinationPrice { get; set; }
        public decimal SorcrPrice { get; set; }
        public string WalletType { get; set; }
        public string AccWalletId { get; set; }
        public string Date { get; set; }
        public decimal CurrentBalance { get; set; }
        public string SourceWalletType { get; set; }
        public string DestinationWalletType { get; set; }
        public string TrnType { get; set; }
        public decimal LimitPerHour { get; set; }
        public decimal LimitPerDay { get; set; }
        public decimal LimitPerTransaction { get; set; }
        public string PublicAddress { get; set; }
        public short IsWhiteListed { get; set; }
        public decimal Amount { get; set; }
        public string RefNo { get; set; }
        public string BenificaryName { get; set; }
    }
    public class ETHDBOperation
    {
        public TradeBitGoDelayAddresses Mainaddress { get; set; }
        public long walletID { get; set; }
        public long WalletTypeId { get; set; }
        public string TrnID { get; set; }
        public string address { get; set; }
        public string BitgoWalletId { get; set; }
        public long createdBy { get; set; }
        public string CoinSpecific { get; set; }
        public short status { get; set; }
        public byte generatebit { get; set; }
        public string coin { get; set; }
    }
}
