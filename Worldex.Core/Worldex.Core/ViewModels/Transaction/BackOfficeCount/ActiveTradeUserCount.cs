using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction.BackOfficeCount
{
    public class ActiveTradeUserCountResponse : BizResponseClass
    {
        public ActiveTradeUserTime Response { get; set; }
    }

    public class TradeUserMarketTypeCountResponse : BizResponseClass
    {
        public TradeSummaryCountResponseInfo Response { get; set; }
    }

    public class ActiveTradeUserTime
    {
       public long TotalCount { get; set; }
       public ActiveTradeUserStatus Today { get; set; }
       public ActiveTradeUserStatus Week { get; set; }
       public ActiveTradeUserStatus Month { get; set; }
       public ActiveTradeUserStatus Year { get; set; }
    }

    public class ActiveTradeUserStatus
    {
        public long Total { get; set; }
        public long Active { get; set; }
        public long Cancel { get; set; }
        public long Settled { get; set; }
        public long PartialCancel { get; set; }
        public long SystemFail { get; set; }
    }

    //Get Data From Database
    public class ActiveTradeUserCount
    {
        public long TotalCount { get; set; }
        public long TodayCount { get; set; }
        public long TodayActiveCount { get; set; }
        public long TodayCancelCount { get; set; }
        public long TodaySettleCount { get; set; }
        public long TodaySystemFailCount { get; set; }
        public long TodayPartialCancelCount { get; set; }
        public long WeekCount { get; set; }
        public long WeekActiveCount { get; set; }
        public long WeekCancelCount { get; set; }
        public long WeekSettleCount { get; set; }
        public long WeekSystemFailCount { get; set; }
        public long WeekPartialCancelCount { get; set; }
        public long MonthCount { get; set; }
        public long MonthActiveCount { get; set; }
        public long MonthCancelCount { get; set; }
        public long MonthSettleCount { get; set; }
        public long MonthSystemFailCount { get; set; }
        public long MonthPartialCancelCount { get; set; }
        public long YearCount { get; set; }
        public long YearActiveCount { get; set; }
        public long YearCancelCount { get; set; }
        public long YearSettleCount { get; set; }
        public long YearSystemFailCount { get; set; }
        public long YearPartialCancelCount { get; set; }
    }
}
