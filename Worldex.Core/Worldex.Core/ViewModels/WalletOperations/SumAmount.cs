namespace Worldex.Core.ViewModels.WalletOperations
{
    public class HistorySumAmount
    {
        public decimal TotalAmount { get; set; }
    }
    public class HistoryAllSumAmount
    {
        public decimal Daily { get; set; }
        public decimal Hourly { get; set; }
        public decimal LifeTime { get; set; }
    }

    public class AllSumAmount
    {
        public decimal DailyAmount { get; set; }
        public decimal HourlyAmount { get; set; }
        public decimal WeeklyAmount { get; set; }
        public decimal MonthlyAmount { get; set; }
        public decimal LifeTimeAmount { get; set; }
        public decimal YearlyAmount { get; set; }

        public long DailyCount { get; set; }
        public long HourlyCount { get; set; }
        public long WeeklyCount { get; set; }
        public long MonthlyCount { get; set; }
        public long LifeTimeCount { get; set; }
        public long YearlyCount { get; set; }
    }

    public class SumAmountAndCount
    {
        public decimal TotalAmount { get; set; }
        public long TotalCount { get; set; }
    }
}
