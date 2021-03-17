namespace Worldex.Core.ViewModels.AccountViewModels.Affiliate
{
    public class GetAffiateUserRegisteredRequest
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int? Status { get; set; }
        public int? SchemeType { get; set; }
        public long? ParentUser { get; set; }
        public int PageNo { get; set; } 
        public int PageSize { get; set; }
    }
}
