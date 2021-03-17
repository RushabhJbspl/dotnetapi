
using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class CopiedLeaderOrdersResponse : BizResponseClass
    {
        public long TotalCount { get; set; }
        public List<CopiedLeaderOrdersInfo> Response { get; set; }
    }
    public class CopiedLeaderOrdersRequest
    {
        public string Pair { get; set; }

        public string TrnType { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string FollowTradeType { get; set; }

        public long FollowingTo { get; set; }

        public int PageNo { get; set; } 

        public int PageSize { get; set; }
        //public short IsMargin { get; set; } = 0;//Rita 21-2-19,   1-for Margin trading cancel txn , not used for now as copy/mirror trade not in Margin trading
    }

    public class CopiedLeaderOrdersBKRequest
    {
        public long? UserID { get; set; }
        public string Pair { get; set; }

        public string TrnType { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string FollowTradeType { get; set; }

        public long? FollowingTo { get; set; }

        public int PageNo { get; set; }

        public int PageSize { get; set; }
        //public short IsMargin { get; set; } = 0;//Rita 21-2-19,   1-for Margin trading cancel txn , not used for now as copy/mirror trade not in Margin trading
    }
    public class CopiedLeaderOrdersQryRes
    {
        public long TrnNo { get; set; }
        public string Type { get; set; }
        public Decimal Price { get; set; }
        public Decimal Amount { get; set; }
        public DateTime DateTime { get; set; }
        public short Status { get; set; }
        public string StatusText { get; set; }
        public String PairName { get; set; }
        public Decimal ChargeRs { get; set; }
        public short IsCancelled { get; set; }
        public short ordertype { get; set; }
        public long StatusCode { get; set; }
        public DateTime? SettledDate { get; set; }
        public Decimal SettledQty { get; set; }
        public long FollowingTo { get; set; }
        public string FollowTradeType { get; set; }
    }
    public class CopiedLeaderOrdersInfo
    {
        public long TrnNo { get; set; }
        public string Type { get; set; }
        public Decimal Price { get; set; }
        public Decimal Amount { get; set; }
        public Decimal Total { get; set; }
        public DateTime DateTime { get; set; }
        public short Status { get; set; }
        public string StatusText { get; set; }
        public String PairName { get; set; }
        public Decimal ChargeRs { get; set; }
        public short IsCancel { get; set; }
        public string OrderType { get; set; }
        public DateTime? SettledDate { get; set; }
        public Decimal SettledQty { get; set; }
        public long FollowingTo { get; set; }
        public string FollowTradeType { get; set; }
    }
    public class CopiedLeaderOrdersResponseV1 : BizResponseClass
    {
        public long TotalCount { get; set; }
        public List<CopiedLeaderOrdersInfoV1> Response { get; set; }
    }
    public class CopiedLeaderOrdersInfoV1
    {
        public string TrnNo { get; set; }
        public string Type { get; set; }
        public Decimal Price { get; set; }
        public Decimal Amount { get; set; }
        public Decimal Total { get; set; }
        public DateTime DateTime { get; set; }
        public short Status { get; set; }
        public string StatusText { get; set; }
        public String PairName { get; set; }
        public Decimal ChargeRs { get; set; }
        public short IsCancel { get; set; }
        public string OrderType { get; set; }
        public DateTime? SettledDate { get; set; }
        public Decimal SettledQty { get; set; }
        public long FollowingTo { get; set; }
        public string FollowTradeType { get; set; }
    }

}
