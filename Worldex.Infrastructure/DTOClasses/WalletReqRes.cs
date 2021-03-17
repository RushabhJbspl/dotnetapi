using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Wallet;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.DTOClasses
{
    public class WalletReqRes : IRequest
    {
        public long UserId { get; set; }
    }

    public class SettelementWalletReqRes : IRequest<WalletDrCrResponse>
    {
        public CommonClassCrDr firstCurrObj { get; set; }
        public CommonClassCrDr secondCurrObj { get; set; }
        public string timestamp { get; set; }
        public enServiceType serviceType { get; set; }
        public EnAllowedChannels enAllowedChannels { get; set; }
        public enWalletDeductionType enWalletDeductionType { get; set; } = enWalletDeductionType.Normal;
    }

    public class MarketCapHandleTemp : IRequest
    {
        public string strMarketCapHandleTemp { get; set; } = "";
    }

    public class IEOCallSP : IRequest
    {
        public string strIEOCallSPTemp { get; set; } = "";
    }

    public class ProfitTemp : IRequest
    {
        public string CurrencyName { get; set; } = "USD";
        public DateTime Date { get; set; }
    }

    public class RecurringChargeCalculation : IRequest
    {
        public int Hour { get; set; }
    }

    public class RefferralCommissionTask : IRequest
    {
        public int Hour { get; set; }
    }

    public class RefferralCommissionTaskReq
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long CronRefNo { get; set; }
    }
    public class MarginWalletReqRes : IRequest
    {
        public long UserId { get; set; }
    }
    public class StakingReqRes : IRequest
    {
        public int IsReqFromAdmin { get; set; }
    }

    public class ForceWithdrwLoanv2Req : IRequest
    {
        public int Hour { get; set; }
    }

    public class ReleaseProfitAmountReq : IRequest
    {
        public int Hour { get; set; }
    }

    public class ServiceProviderReq : IRequest<ServiceProviderBalanceResponse>
    {
        public List<TransactionProviderResponse2> transactionProviderResponses2 { get; set; }
    }
    public class FiatSellWithdrawReq : IRequest
    {
        public int Hour { get; set; }
    }
    public class FiatBinnanceLTPChange : IRequest
    {
        public int Hour { get; set; }
    }
    public class FiatPendingHashUpdate : IRequest
    {
        public int Hour { get; set; }
    }
}

