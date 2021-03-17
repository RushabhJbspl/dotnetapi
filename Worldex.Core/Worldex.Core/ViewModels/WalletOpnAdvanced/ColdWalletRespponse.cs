using System.Collections.Generic;

namespace Worldex.Core.ViewModels.WalletOpnAdvanced
{
    public class ColdWalletRespponse
    {
        public RootObject rootObject { get; set; }
        public ErrorRootObject errorRoot { get; set; }
    }
    public class ErrorRootObject
    {
        public string error { get; set; }
        public string message { get; set; }
    }
    public class User
    {
        public string user { get; set; }
        public List<string> permissions { get; set; }
    }

    public class KeySignatures
    {
        public string bitgoPub { get; set; }
        public string backupPub { get; set; }
    }

    public class Freeze
    {
    }

    public class CoinSpecific1
    {
    }

    public class Admin
    {
    }

    public class CoinSpecific2
    {
        public string redeemScript { get; set; }
    }

    public class ReceiveAddress
    {
        public string id { get; set; }
        public string address { get; set; }
        public int chain { get; set; }
        public int index { get; set; }
        public string coin { get; set; }
        public string wallet { get; set; }
        public CoinSpecific2 coinSpecific { get; set; }
    }

    public class RootObject
    {
        public string id { get; set; }
        public List<User> users { get; set; }
        public string coin { get; set; }
        public string label { get; set; }
        public int m { get; set; }
        public int n { get; set; }
        public List<string> keys { get; set; }
        public KeySignatures keySignatures { get; set; }
        public List<string> tags { get; set; }
        public bool disableTransactionNotifications { get; set; }
        public Freeze freeze { get; set; }
        public bool deleted { get; set; }
        public int approvalsRequired { get; set; }
        public bool isCold { get; set; }
        public CoinSpecific1 coinSpecific { get; set; }
        public Admin admin { get; set; }
        public List<object> clientFlags { get; set; }
        public bool allowBackupKeySigning { get; set; }
        public bool recoverable { get; set; }
        public int balance { get; set; }
        public int confirmedBalance { get; set; }
        public int spendableBalance { get; set; }
        public string balanceString { get; set; }
        public string confirmedBalanceString { get; set; }
        public string spendableBalanceString { get; set; }
        public ReceiveAddress receiveAddress { get; set; }
        public List<object> pendingApprovals { get; set; }
    }
}
