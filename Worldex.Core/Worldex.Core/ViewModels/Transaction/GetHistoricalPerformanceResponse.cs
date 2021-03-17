using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class GetHistoricalPerformanceResponse : BizResponseClass
    {
        public List<HistoricalPerformanceYear> Response { get; set; }
    }

    public class HistoricalPerformanceMonth
    {
        public string MonthName { get; set; }
        public long MonthIndex { get; set; }
        public string PerformanceCurrency { get; set; }
        public decimal PerformanceValue { get; set; }
    }

    public class HistoricalPerformanceYear
    {
        public int Year { get; set; }
        //public List<HistoricalPerformanceMonth> MonthData { get; set; }
        public decimal[] Data { get; set; }
        public decimal Total { get; set; }
    }

    public class HistoricalPerformance
    {
        public decimal Amount;
    }

    public class HistoricalPerformanceTemp
    {
        public decimal ProfitPer { get;set;}
        public long AutoNo { get; set; }
    }

    public class ListTemp
    {
        public List<HistoricalPerformanceTemp> Data { get; set; }
    }

}
