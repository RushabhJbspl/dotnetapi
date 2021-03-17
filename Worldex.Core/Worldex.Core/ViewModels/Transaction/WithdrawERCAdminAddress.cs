using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class WithdrawERCAdminAddress
    {
        public string Address { get; set; }
        public long AddressId { get; set; }
        public string RefKey { get; set; }
    }

    public class WithdrwaERCStatusCheck
    {
        public string sitename { get; set; }
        public string site_id { get; set; }
        public string txnid { get; set; }
    }

    public class WithdrwaERCStatusCheckResponse
    {
        public bool isError { get; set; }
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
        public int? status { get; set; }
        public int transactionIndex { get; set; }
    }
}
