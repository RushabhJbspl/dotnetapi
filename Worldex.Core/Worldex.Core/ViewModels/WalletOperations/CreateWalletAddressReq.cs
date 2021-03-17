namespace Worldex.Core.ViewModels.WalletOperations
{
    public class CreateWalletAddressReq
    {
        public bool allowMigrated { get; set; }
        public int chain{ get; set;}
        public int gasPrice { get; set; }
        public bool lowPriority { get; set; }
        public string label { get; set; }
        public int count { get; set; }
    }
}
