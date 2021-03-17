using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Transaction.BackOffice
{
    public class TradingSummaryRequest
    {
        public long MemberID { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }
        
        public string TrnNo { get; set; }

        public short Status { get; set; }

        public string SMSCode { get; set; }

        public string Trade { get; set; }

        public string Pair { get; set; }

        public string MarketType { get; set; }

        public string LPType { get; set; }

        public int PageNo { get; set; } //Uday 12-01-2019 Add Pagination
        public int PageSize { get; set; }
        public short IsMargin { get; set; } = 0;//Rita 4-2-19 for Margin Trading
    }

    public class TradingReconHistoryRequest
    {
        public long MemberID { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string TrnNo { get; set; }

        public short Status { get; set; }

        //public string SMSCode { get; set; }

        public string Trade { get; set; }

        public string Pair { get; set; }

        public string MarketType { get; set; }

        public int LPType { get; set; } // khushali 30-03-2019 for chnage type to int

        public int PageNo { get; set; } //Uday 12-01-2019 Add Pagination
        public int PageSize { get; set; }
        public short IsMargin { get; set; } = 0;//Rita 4-2-19 for Margin Trading
        public short IsProcessing { get; set; } = 999; //khushali 30-03-2019 for isproccessing filteration

    }

    public class TradingSummaryResponse : BizResponseClass
    {
        public List<TradingSummaryViewModel> Response { get; set; }
        public long TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public long TotalPages { get; set; }
    }
    public class TradingSummaryLPResponse : BizResponseClass
    {
        public List<TradingSummaryLPViewModel> Response { get; set; }
        public long TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public long TotalPages { get; set; }
    }

    public class TradingReconHistoryResponse : BizResponseClass
    {
        public List<TradingReconHistoryViewModel> Response { get; set; }
        public long TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public long TotalPages { get; set; }
    }
}
