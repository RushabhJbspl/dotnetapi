using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.WalletOpnAdvanced
{
    public class CommonResponseWalletAdvClass
    {

    }
    public class CoinSpecific
    {
        public int chain { get; set; }
        public int index { get; set; }
        public string redeemScript { get; set; }
    }

    public class Address
    {
        public string address { get; set; }
        public string coin { get; set; }
        public string label { get; set; }
        public string wallet { get; set; }
        public CoinSpecific coinSpecific { get; set; }
    }
    public class Input
    {
        public string id { get; set; }
        public string address { get; set; }
        public int value { get; set; }
        public string wallet { get; set; }
        public string fromWallet { get; set; }
        public int chain { get; set; }
        public int index { get; set; }
        public string valueString { get; set; }
    }

    public class Output
    {
        public string id { get; set; }
        public string address { get; set; }
        public int value { get; set; }
        
        public string valueString { get; set; }
        public string wallet { get; set; }
        public int? chain { get; set; }
        public int? index { get; set; }
    }

    public class Entry
    {
        public string account { get; set; }
        public int value { get; set; }
        public int inputs { get; set; }
        public int outputs { get; set; }
        public string address { get; set; }
        public string valueString { get; set; }
        public string wallet { get; set; }
    }

    public class Transaction
    {
        public string id { get; set; }
        public int size { get; set; }
        public int fee { get; set; }
        public DateTime date { get; set; }
        public string hex { get; set; }
        public string fromWallet { get; set; }
        public string blockHash { get; set; }
        public int blockHeight { get; set; }
        public int blockPosition { get; set; }
        public List<string> inputIds { get; set; }
        public string comment { get; set; }
        public List<Input> inputs { get; set; }
        public List<Output> outputs { get; set; }
        public List<Entry> entries { get; set; }
    }
    public class Unspent
    {
        public string id { get; set; }
        public string address { get; set; }
        public int value { get; set; }
        public int blockHeight { get; set; }
        public DateTime date { get; set; }
        public string wallet { get; set; }
        public object fromWallet { get; set; }
        public int chain { get; set; }
        public int index { get; set; }
        public string redeemScript { get; set; }
        public bool isSegwit { get; set; }
    }
    public class TxInfo
    {
        public int nP2SHInputs { get; set; }
        public int nSegwitInputs { get; set; }
        public int nOutputs { get; set; }
        public List<Unspent> unspents { get; set; }
        public List<string> changeAddresses { get; set; }
    }

    public class FeeInfo
    {
        public int size { get; set; }
        public int fee { get; set; }
        public int feeRate { get; set; }
        public int payGoFee { get; set; }
        public string payGoFeeString { get; set; }
    }
}
