namespace Worldex.Core.ViewModels.AccountViewModels.Affiliate
{
    public class GetEmailSentRequest
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public long? UserId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}
