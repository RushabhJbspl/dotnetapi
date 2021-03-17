using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class WalletStatisticsData
    {
        public decimal StartingBalance { get; set; }
        public decimal EndingBalance { get; set; }
        public List<WalletTransactiondata> TranAmount { get; set; }        
    }
    public class WalletStatisticsData2
    {
        public decimal StartingBalance { get; set; }
        public decimal EndingBalance { get; set; }
        public TempClass MonthwiseData { get; set; }
    }
    public class MonthWiseData
    {
        public long Month { get; set; }
        public List<WalletTransactiondata> Data { get; set; }
    }
    public class TempClass
    {        
        public List<MonthWiseData> TranAmount { get; set; }
    }

    public class TranDetails
    {
        public long TrnTypeID { get; set; }
        public string TrnTypeName { get; set; }
        public decimal TotalAmount { get; set; }
        public Int64 TotalCount { get; set; }
        public long Month { get; set; }
    }
    public class WalletBalancedata
    {
        public decimal StartingBalance { get; set; }
        public decimal EndingBalance { get; set; }
    }
    public class WalletTransactiondata
    {
        public long TrnTypeId { get; set; }
        public string TrnTypeName { get; set; }
        public decimal TotalAmount { get; set; }        
        public long TotalCount { get; set; }
    }
    public class sp_BalanceStatisticRes : BizResponseClass
    {
        public decimal StartingBalance { get; set; }
        public decimal EndingBalance { get; set; }
    }
    public class StatisticsDetailData : BizResponseClass
    {
        public WalletStatisticsData Balances { get; set; }
        public string BaseCurrency { get; set; }
    }
    public class StatisticsDetailData2 : BizResponseClass
    {
        public WalletStatisticsData2 Balances { get; set; }
        public string BaseCurrency { get; set; }
    }
}
