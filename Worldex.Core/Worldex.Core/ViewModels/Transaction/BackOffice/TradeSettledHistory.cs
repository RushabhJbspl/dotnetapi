using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Transaction.BackOffice
{
    public class TradeSettledHistoryRequest
    {
        public long MemberID { get; set; }
        public string PairName { get; set; }
        public string OrderType { get; set; }
        public string TrnType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string TrnNo { get; set; }
        public int PageNo { get; set; } //Uday 12-01-2019 Add Pagination
        public int PageSize { get; set; }
        public short IsMargin { get; set; } = 0;//Rita 5-3-19 for margin trading
    }
    public class TradeSettledHistoryRequestFront
    {
        public string PairName { get; set; }
        public string OrderType { get; set; }
        public string TrnType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string TrnNo { get; set; }
        public int PageNo { get; set; } //Uday 12-01-2019 Add Pagination
        public int PageSize { get; set; }
        public short IsMargin { get; set; } = 0;//Rita 21-2-19,   1-for Margin trading cancel txn
    }
    public class TradeSettledHistoryResponse : BizResponseClass
    {
        public List<TradeSettledHistory> Response { get; set; }
        public long TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public long TotalPages { get; set; }
    }
    public class TradeSettledHistoryResponseV1 : BizResponseClass
    {
        public List<TradeSettledHistoryV1> Response { get; set; }
        public long TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public long TotalPages { get; set; }
    }
    public class TradeSettledHistory
    {
        public long  TrnNo { get; set; }
        public string TrnType { get; set; }
        public long PairID { get; set; }
        public string PairName { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public string OrderType { get; set; }
        public DateTime TrnDate { get; set; }
        public long MemberID { get; set; }

        public List<TradePoolHistory> Trades { get; set; }
    }
    public class TradeSettledHistoryV1
    {
        public string TrnNo { get; set; }
        public string TrnType { get; set; }
        public long PairID { get; set; }
        public string PairName { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public string OrderType { get; set; }
        public DateTime TrnDate { get; set; }
        public long MemberID { get; set; }

        public List<TradePoolHistoryV1> Trades { get; set; }
    }
    public class TradePoolHistory
    {
        public long TrnNo { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public string TrnType { get; set; }
    }
    public class TradePoolHistoryV1
    {
        public string TrnNo { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public string TrnType { get; set; }
    }
    public class TradeSettledHistoryQueryResponse2
    {
        public long PairID { get; set; }
        public string PairName { get; set; }
        public DateTime TrnDate { get; set; }
        public long MemberID { get; set; }
        public string orderType { get; set; }
        public decimal Price1 { get; set; }
        public decimal Qty1 { get; set; }
        public long TrnNo { get; set; }
        public long Trade { get; set; }
        public decimal Price { get; set; }
        public decimal QTY { get; set; }
        public string TrnTypeName { get; set; }
        public string TradeType { get; set; }
    }
    public class TradeSettledHistoryQueryResponse2V1
    {
        public long PairID { get; set; }
        public string PairName { get; set; }
        public DateTime TrnDate { get; set; }
        public long MemberID { get; set; }
        public string orderType { get; set; }
        public decimal Price1 { get; set; }
        public decimal Qty1 { get; set; }
        public string TrnNo { get; set; }
        public string Trade { get; set; }
        public decimal Price { get; set; }
        public decimal QTY { get; set; }
        public string TrnTypeName { get; set; }
        public string TradeType { get; set; }
    }
}
