namespace Worldex.Core.ViewModels.ControlPanel
{
    public class PolicyCommonReq
    {
        public long WalletType { get; set; }
        public long TrnType { get; set; }
        public string PolicyName { get; set; }
        public short Status { get; set; }
        public string AllowedIP { get; set; }
        public string AllowedLocation { get; set; }
        public int AuthenticationType { get; set; }
        public double? StartTime { get; set; }
        public double? EndTime { get; set; }
        public long HourlyTrnCount { get; set; }
        public decimal HourlyTrnAmount { get; set; }
        public long DailyTrnCount { get; set; }
        public decimal DailyTrnAmount { get; set; }
        public long MonthlyTrnCount { get; set; }
        public decimal MonthlyTrnAmount { get; set; }
        public long WeeklyTrnCount { get; set; }
        public decimal WeeklyTrnAmount { get; set; }
        public long YearlyTrnCount { get; set; }
        public decimal YearlyTrnAmount { get; set; }
        public long LifeTimeTrnCount { get; set; }
        public decimal LifeTimeTrnAmount { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public short AuthorityType { get; set; }
        public short AllowedUserType { get; set; }
        public long RoleId { get; set; }

        public PolicyCommonReq()
        {
            AllowedIP = "*";
            AllowedLocation = "*";
        }
    }
}
