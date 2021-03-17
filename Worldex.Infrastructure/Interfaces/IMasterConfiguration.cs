using System;
using System.Collections.Generic;
using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.MasterConfiguration;
using Worldex.Core.ViewModels.BackOfficeReports;

namespace Worldex.Infrastructure.Interfaces
{
    public interface IMasterConfiguration
    {
        BizResponseClass AddCountry(AddCountryReq Request, long UserID);//string CountryName, string CountryCode,long UserID,short Status
        BizResponseClass AddState(AddStateReq Request, long UserID);//string StateName, string StateCode, long CountryID
        BizResponseClass AddCity(AddCityReq Request, long UserID);//string CityName, long StateID
        BizResponseClass AddZipCode(AddZipCodeReq Request, long UserID);//long ZipCode, string AreaName, long CityID

        BizResponseClass UpdateCountry(AddCountryReq Request, long UserID);
        BizResponseClass UpdateState(AddStateReq Request, long UserID);
        BizResponseClass UpdateCity(AddCityReq Request, long UserID);
        BizResponseClass UpdateZipCode(AddZipCodeReq Request, long UserID);

        Countries GetCountry(long CountryID);
        States GetState(long StateID);
        Cities GetCity(long CityID);

        RptWithdrawalRes WithdrawReport(DateTime FromDate, DateTime ToDate, int PageNo, int? PageSize, string CoinName, long? UserID, short? Status, string Address, string TrnID, string TrnNo, long? OrgId, short? IsInternalTransfer);
        RptWithdrawalResv2 WithdrawReportv2(DateTime FromDate, DateTime ToDate, int PageNo, int? PageSize, string CoinName, long? UserID, short? Status, string Address, string TrnID, string TrnNo, long? OrgId, short? IsInternalTransfer);

        RptDepositionRes DepositReport(DateTime FromDate, DateTime ToDate, int PageNo, int? PageSize, string CoinName, long? UserID, short? Status,string Address, string TrnID, long? OrgId);

        RptDepositionResv2 DepositReportv2(DateTime FromDate, DateTime ToDate, int PageNo, int? PageSize, string CoinName, long? UserID, short? Status, string Address, string TrnID, long? OrgId);

        List<EmailLists> GetAllEmail();
    }
}
