using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.IEOWallet;
using System;

namespace Worldex.Infrastructure.Interfaces
{
    public interface IIEOWalletService
    {
        ListIEOWalletResponse ListWallet(Int16 Status);

        ListIEOPurchaseHistoryResponse ListPurchaseHistory(DateTime FromDate, DateTime ToDate, int Page, int PageSize, Int64 PaidCurrency, Int64 DeliveryCurrency, int UserID);

        ListIEOPurchaseHistoryResponseBO ListPurchaseHistoryBO(DateTime FromDate, DateTime ToDate, int Page, int PageSize, Int64 PaidCurrency, Int64 DeliveryCurrency,string Email);


        PreConfirmResponseV2 PreConfirmation(string PaidCurrencyWallet, decimal PaidQauntity, string PaidCurrency, string DeliveryCurrency, string RoundID, string Remarks,Int64 UserID);

        PreConfirmResponse Confirmation(string PaidCurrencyWallet, decimal PaidQauntity, string PaidCurrency, string DeliveryCurrency, string RoundID, string Remarks, Int64 UserID);

        BizResponseClass InsertUpdateBannerConfiguration(IEOBannerRequest Request,long UserId,string FilePath);

        GetIEOBannerRes GetBannerConfiguration();

        BizResponseClass InsertUpdateAdminWalletConfiguration(IEOAdminWalletRequest Request, long UserId);

        ListGetIEOAdminWalletRes GetAdminWalletConfiguration(long UserId);

        BizResponseClass InsertRoundConfiguration(InsertRoundConfigurationReq Request, long UserId,string fileName);

        BizResponseClass UpdateRoundConfiguration(UpdateRoundConfigurationReq Request, long UserId,string fileName);

        ListRoundConfigurationResponse ListIEORoundConfiguration(Int16 Status);

        BizResponseClass IEOAdminWalletDeposit(IEOAdminWalletCreditReq Req);

        ListAllocateTokenCountRes IEOTokenCount(short IsAllocate);

        ListTokenCountRes IEOTradeTokenCount();

        ListIEOTokenReportDataRes IEOTokenReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, string Email, string PaidCurrency, string DeliveredCurrency, short? Status, string TrnRefNo);

        ListIEOAllocatedTokenReportDataRes IEOAllocatedTokenReport(int PageNo, int PageSize, DateTime? FromDate, DateTime? ToDate, string Email, string PaidCurrency, string DeliveredCurrency, short? Status, string TrnRefNo);
    }
}
