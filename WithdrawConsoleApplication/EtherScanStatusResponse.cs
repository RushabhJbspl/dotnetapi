using System;
using System.Collections.Generic;
using System.Text;

namespace WithdrawConsoleApplication
{
    public class EtherScanStatusResponse
    {
        public string isError { get; set; }
        public string msg { get; set; }
        public WithdrwaERCStatusCheckData transaction { get; set; }
    }
  
    public class WithdrwaERCStatusCheckData
    {
        public string blockHash { get; set; }
        public string blockNumber { get; set; }
        public string contractAddress { get; set; }
        public string cumulativeGasUsed { get; set; }
        public string gasUsed { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string transactionHash { get; set; }
        public string status { get; set; } 
        public int transactionIndex { get; set; }
    }

    public class TRNOResponse
    {
        public int isError { get; set; }
        public string txn_id { get; set; }
        public long confirmations { get; set; }
    }

    public class ReceiptResponse
    {
        public int isError { get; set; }
        public string status { get; set; }
        public string receipt { get; set; }
    }

    public class PayUWalletResponse
    {
        public int isError { get; set; }
        public Receipt receipt { get; set; }
    }

    public class Receipt
    {
        public string status { get; set; }
        public string _id { get; set; }
        public string user_id { get; set; }
        public string app_id { get; set; }
        public string coin_code { get; set; }
        public string withdraw_address { get; set; }
        public string withdraw_amount { get; set; }
        public string destination_tag { get; set; }
        public string tx_hash { get; set; }
        public string notify_url { get; set; }
        public string transaction_id { get; set; }
        public string order_id { get; set; }
        public DateTime created_date { get; set; }
        public DateTime expire_datetime { get; set; }
        public string created_ip { get; set; }
        public int __v { get; set; }
    }

    //public class RootObject
    //{
    //    public int isError { get; set; }
    //    public Receipt receipt { get; set; }
    //}
}
