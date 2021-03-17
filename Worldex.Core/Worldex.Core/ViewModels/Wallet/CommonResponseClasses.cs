using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Wallet
{
    public class User
    {
        public string user { get; set; }
        public List<string> permissions { get; set; }
    }
    public class Wallet2
    {
        public Wallet _wallet { get; set; }
    }
    public class Freeze
    {
    }

    public class CoinSpecific
    {
        //for add wallet
        public bool? deployedInBlock { get; set; }
        public string deployTxHash { get; set; }
        public LastChainIndex lastChainIndex { get; set; }
        public string baseAddress { get; set; }
        public string feeAddress { get; set; }
        public bool? pendingChainInitialization { get; set; }
        public List<object> creationFailure { get; set; }
    }
    public class LastChainIndex
    {
        public int __invalid_name__0 { get; set; }
        public int __invalid_name__1 { get; set; }
    }
    public class CoinSpecific2
    {
        public string redeemScript { get; set; }
    }
    public class ReceiveAddress
    {
        public string address { get; set; }
        public int chain { get; set; }
        public int index { get; set; }
        public string coin { get; set; }
        public string wallet { get; set; }
        public CoinSpecific2 coinSpecific { get; set; }
    }
    public class Action
    {
        public string type { get; set; }
    }
    public class Wallet
    {
        public string id { get; set; }
        public List<User> users { get; set; }
        public string coin { get; set; }
        public string label { get; set; }
        public int m { get; set; }
        public int n { get; set; }
        public List<string> keys { get; set; }
        public List<string> tags { get; set; }
        public bool disableTransactionNotifications { get; set; }
        public Freeze freeze { get; set; }
        public bool deleted { get; set; }
        public int approvalsRequired { get; set; }
        public bool? isCold { get; set; }
        public CoinSpecific coinSpecific { get; set; }

        //---for create wallet---
        public int? balance { get; set; }
        public int? confirmedBalance { get; set; }
        public int? spendableBalance { get; set; }
        public string balanceString { get; set; }
        public string confirmedBalanceString { get; set; }
        public string spendableBalanceString { get; set; }
        public ReceiveAddress receiveAddress { get; set; }
        public List<object> pendingApprovals { get; set; }

    }
    public class Condition
    {
        public string amountString { get; set; }
        public int timeWindow { get; set; }
        public List<string> groupTags { get; set; }
        public List<object> excludeTags { get; set; }
    }

    public class Rule
    {
        public string id { get; set; }
        public string coin { get; set; }
        public string type { get; set; }
        public Action action { get; set; }
        public Condition condition { get; set; }
    }

    public class Policy
    {
        public string id { get; set; }
        public string label { get; set; }
        public int version { get; set; }
        public DateTime date { get; set; }
        public List<Rule> rules { get; set; }
    }

    public class Admin
    {
        public Policy policy { get; set; }
    }
    //}
}
