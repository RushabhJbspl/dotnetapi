using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class APIRequestStatisticsCountResponse : BizResponseClass
    {
        public long SuccessCount { get; set; }
        public long FaliureCount { get; set; }
        public long APIUsers { get; set; }
        public long RegisterToday { get; set; }
        public List<PlanUsersQryRes> PlanUsers { get; set; }
        public List<PlanUsersQryRes> PurchasePlan { get; set; }
        public List<APIReqStatusCodeCountQryRes> StatusCode { get; set; }
        public List<APIReqBrowsweWiseQryRes> Browser { get; set; }
        public List<HTTPErrorCodeQryRes> ErrorCodeList { get; set; }
    }
    public class APIRequestStatisticsCountQryRes
    {
        public long SuccessCount { get; set; }
        public long FaliureCount { get; set; }
        public long APIUsers { get; set; }
        public long RegisterToday { get; set; }
    }
    public class PlanUsersQryRes
    {
        public long APIUsers { get; set; }
        public long APIPlanMasterID { get; set; }
        public string PlanName { get; set; }
    }

    public class UserWiseAPIReqCountQryRes 
    {
        public long ReqCount { get; set; }
        public long UserID { get; set; }
    }
    public class UserWiseAPIReqCounResponse : BizResponseClass
    {
        public List<UserWiseAPIReqCounResInfo> Response { get; set; }
        public long TotalCount { get; set; }
        public long PageCount { get; set; }
    }
    public class UserWiseAPIReqCounResInfo
    {
        public long ReqCount { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string EmailID { get; set; }
    }
    public class APIReqStatusCodeCountQryRes
    {
        public long ReqCount { get; set; }
        public long HTTPStatusCode { get; set; }
    }
    public class APIReqBrowsweWiseQryRes
    {
        public long ReqCount { get; set; }
        public string Browser { get; set; }
    }

    public class HTTPErrorCodeQryRes
    {
        public long HTTPErrorCode { get; set; }
        public string Path { get; set; }
        public string Host { get; set; }
        public string MethodType { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class FrequentUseAPIRespons : BizResponseClass
    {
        public List<FrequentUseAPIResponsInfo> Response { get; set; }
        
    }
    public class FrequentUseAPIResponsInfo
    {
        public string Path { get; set; }
        public DateTime CreatedDate { get; set; }
        public long HTTPErrorCode { get; set; }
        public long HTTPStatusCode { get; set; }
        public short Status { get; set; }
        public string Host { get; set; }
    }

    public class FrequentUseAPIQryRes
    {
        public long ReqCount { get; set; }
        public string Path { get; set; }
    }

    public class HTTPErrorsReportResponse :BizResponseClass
    {
        public long TotalCount { get; set; }
        public long PageCount { get; set; }
        public List<HTTPErrorCodeQryRes> Response { get; set; }
    }
    public class HTTPErrorsReportRequest
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public long? ErrorCode { get; set; }
        public long? Pagesize { get; set; }
        public long? PageNo { get; set; }
    }

    public class MostActiveIPWiseReportResponse : BizResponseClass
    {
        public long TotalCount { get; set; }
        public long PageCount { get; set; }
        public List<MostActiveIPWiseReportQryRes> Response { get; set; }
    }
    public class MostActiveIPWiseReportQryRes
    {
        public string UserName { get; set; }
        public string EmailID { get; set; }
        public DateTime CreatedDate { get; set; }
        public long MemberID { get; set; }
        public string IPAddress { get; set; }
        public string Path { get; set; }
        public string Host { get; set; }
        public short WhitelistIP { get; set; }
    }
    public class MostActiveIPWiseReportRequest
    {
        public long? MemberID { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string IPAddress { get; set; }
        public long? Pagesize { get; set; }
        public long? PageNo { get; set; }
    }

    public class FrequentUseAPIWiseReportResponse : BizResponseClass
    {
        public long TotalCount { get; set; }
        public long PageCount { get; set; }
        public List<FrequentUseAPIWiseReportQryRes> Response { get; set; }
    }
    public class FrequentUseAPIWiseReportQryRes
    {
        public DateTime CreatedDate { get; set; }
        public string Path { get; set; }
        public string Host { get; set; }
        public long HTTPErrorCode { get; set; }
        public long HTTPStatusCode { get; set; }
        public short Status { get; set; }
        public long MemberID { get; set; }
        public string UserName { get; set; }
        public string EmailID { get; set; }
    }
    public class FrequentUseAPIWiseReportRequest
    {
        public long? MemberID { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public long? Pagesize { get; set; }
        public long? PageNo { get; set; }
    }
}
