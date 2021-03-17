using Worldex.Core.ViewModels.IEOWallet;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.Interfaces
{
    public interface IIEOWalletRepository
    {
        List<IEOWalletResponse> ListIEOWallet(Int16 Status);

        List<IEOPurchaseHistoryResponse> ListPurchaseHistory(DateTime FromDate, DateTime ToDate, int Page, int PageSize, long PaidCurrency, long DeliveryCurrency, int UserID, ref int TotalCount);

        List<IEOPurchaseHistoryResponseBO> ListPurchaseHistoryBO(DateTime FromDate, DateTime ToDate, int Page, int PageSize, long PaidCurrency, long DeliveryCurrency,string Email, ref int TotalCount);

        List<GetIEOAdminWalletRes> GetAdminWalletConfiguration(long UserId);

        List<IEORoundResponse> ListIEORounds(Int16 Status);

        ListIEOTokenReportDataRes IEOTokenReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, string Email, string PaidCurrency, string DeliveredCurrency, short? Status,string TrnRefNo);

        ListIEOAllocatedTokenReportDataRes IEOAllocatedTokenReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, string Email, string PaidCurrency, string DeliveredCurrency, short? Status, string TrnRefNo);

        List<AllocateTokenCountRes> IEOTokenCount(short IsAllocate);

        List<TokenCountRes> IEOTradeTokenCount();

        long getOrgID();
    }
}
