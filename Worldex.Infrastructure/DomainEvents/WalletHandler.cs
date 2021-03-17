using Worldex.Core.Entities;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.MarginWallet;
using Worldex.Core.ViewModels.ControlPanel;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Core.ViewModels.WalletOperations;
using Worldex.Infrastructure.DTOClasses;
using Worldex.Infrastructure.Interfaces;
using Worldex.Infrastructure.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace Worldex.Infrastructure.DomainEvents
{
    public class WalletHandler : IRequestHandler<WalletReqRes>
    {
        private readonly IWalletService _IwalletService;
        private readonly IMarginWalletService _IMarginwalletService;
        public WalletHandler(IWalletService IwalletService, IMarginWalletService IMarginwalletService)
        {
            _IwalletService = IwalletService;
            _IMarginwalletService = IMarginwalletService;
        }
        public Task<Unit> Handle(WalletReqRes request, CancellationToken cancellationToken)
        {
            _IwalletService.CreateDefaulWallet(request.UserId);
            _IMarginwalletService.CreateAllMarginWallet(request.UserId);
            return Task.FromResult(new Unit());
        }
    }

    public class SettlementWalletHandler : IRequestHandler<SettelementWalletReqRes, WalletDrCrResponse>
    {
        private readonly IWalletServiceV2 _IwalletService;
        public SettlementWalletHandler(IWalletServiceV2 IwalletService)
        {
            _IwalletService = IwalletService;
        }
        public Task<WalletDrCrResponse> Handle(SettelementWalletReqRes request, CancellationToken cancellationToken)
        {
            var Res = _IwalletService.GetWalletCreditDrForHoldNewAsyncFinal(request.firstCurrObj, request.secondCurrObj, request.timestamp, request.serviceType, request.enAllowedChannels, request.enWalletDeductionType).GetAwaiter().GetResult();
            return Task.FromResult(Res);
        }
    }

    public class MarginWalletHandler : IRequestHandler<MarginWalletReqRes>
    {
        private readonly IMarginWalletService _IwalletService;
        public MarginWalletHandler(IMarginWalletService IwalletService)
        {
            _IwalletService = IwalletService;
        }
        public Task<Unit> Handle(MarginWalletReqRes request, CancellationToken cancellationToken)
        {
            _IwalletService.CreateAllMarginWallet(request.UserId);
            return Task.FromResult(new Unit());
        }
    }

    public class MarketCapHandler : IRequestHandler<MarketCapHandleTemp>
    {
        private readonly IMarketCap _IMarketCap;
        private IMemoryCache _cache;
        private readonly ITrnMasterConfiguration _IWalletConfiguration;
        public MarketCapHandler(IMarketCap IMarketCap, ITrnMasterConfiguration IWalletConfiguration, IMemoryCache cache)
        {
            _IWalletConfiguration = IWalletConfiguration;
            _cache = cache;
            _IMarketCap = IMarketCap;
        }
        public Task<Unit> Handle(MarketCapHandleTemp temp, CancellationToken cancellationToken)
        {
            CronMaster cronMaster = new CronMaster();
            //cronMaster = _IWalletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.MarketCap);
            //if (cronMaster != null && cronMaster.Status == 1)
            List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            if (cronMasterList == null)
            {
                cronMasterList = _IWalletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            else if (cronMasterList.Count() == 0)
            {
                cronMasterList = _IWalletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.MarketCap).FirstOrDefault();
            if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
            {
                HelperForLog.WriteLogIntoFile("MarketCapHandler", "Task:Handle", "CallAPIForMarketCap");
                var res = _IMarketCap.CallAPI();
                HelperForLog.WriteLogIntoFile("MarketCapHandler", "Task:Handle", "Response::" + res.ReturnMsg + "###" + res.ReturnCode);
                if (res.ReturnCode == 0)
                {
                    _IMarketCap.UpdateMarketCapCounter();
                }
            }
            return Task.FromResult(new Unit());
        }
    }

    public class IEOCallSPHandler : IRequestHandler<IEOCallSP>
    {
        private readonly IMarketCap _IMarketCap;
        //private readonly IWalletConfiguration _walletConfiguration;
        private IMemoryCache _cache;
        private readonly ITrnMasterConfiguration _walletConfiguration;
        public IEOCallSPHandler(IMarketCap IMarketCap, ITrnMasterConfiguration walletConfiguration, IMemoryCache cache)
        {
            _IMarketCap = IMarketCap;
            _walletConfiguration = walletConfiguration;
            _cache = cache;
        }

        public Task<Unit> Handle(IEOCallSP request, CancellationToken cancellationToken)
        {
            CronMaster cronMaster = new CronMaster();
            //cronMaster = _walletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.RecurringCharge);
            //if (cronMaster != null && cronMaster.Status == 1)
            List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            if (cronMasterList == null)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            else if (cronMasterList.Count() == 0)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.IEOBGProcess).FirstOrDefault();
            if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
            {
                var res = _IMarketCap.CallSP_IEOCallSP();
            }
            return Task.FromResult(new Unit());
        }
    }



    public class MarginChargeHandler : IRequestHandler<RecurringChargeCalculation>
    {
        private readonly IMarginSPRepositories _walletSPRepositories;
        private readonly IMarginTransactionWallet _WalletTranx;
        private readonly ITrnMasterConfiguration _walletConfiguration;
        private IMemoryCache _cache;
        public MarginChargeHandler(IMarginSPRepositories walletSPRepositories, IMarginTransactionWallet WalletTranx, ITrnMasterConfiguration walletConfiguration, IMemoryCache cache)
        {
            _walletSPRepositories = walletSPRepositories;
            _walletConfiguration = walletConfiguration;
            _WalletTranx = WalletTranx;
            _cache = cache;
        }
        public Task<Unit> Handle(RecurringChargeCalculation temp, CancellationToken cancellationToken)
        {
            CronMaster cronMaster = new CronMaster();
            //cronMaster = _walletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.IEOBGProcess);//ntrivedi 19-08-2019 bg process lp depending 
            //if (cronMaster != null && cronMaster.Status == 1)
            List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            if (cronMasterList == null)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            else if (cronMasterList.Count() == 0)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.RecurringCharge).FirstOrDefault();
            if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
            {
                long batchNo = 0;
                var res = _walletSPRepositories.Callsp_MarginChargeWalletCallBGTaskNew(ref batchNo); //ntrivedi new methodname added
                _WalletTranx.ReleaseMarginWalletforSettleLeverageBalance(batchNo);
            }
            return Task.FromResult(new Unit());
        }
    }

    public class ReferralCommissionHandler : IRequestHandler<RefferralCommissionTask>
    {
        private readonly IWalletSPRepositories _walletSPRepositories;
        //private readonly IWalletConfiguration _walletConfiguration;
        private readonly ITrnMasterConfiguration _walletConfiguration;
        private readonly IMarketCap _IMarketCap;
        private IMemoryCache _cache;
        public ReferralCommissionHandler(IWalletSPRepositories walletSPRepositories, IMarketCap IMarketCap, ITrnMasterConfiguration walletConfiguration, IMemoryCache cache)
        {
            _walletSPRepositories = walletSPRepositories;
            _IMarketCap = IMarketCap;
            _walletConfiguration = walletConfiguration;
            _cache = cache;
        }
        public Task<Unit> Handle(RefferralCommissionTask temp, CancellationToken cancellationToken)
        {
            CronMaster cronMaster = new CronMaster();
            //cronMaster = _walletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.ReferralCommission);
            //if (cronMaster != null && cronMaster.Status == 1)
            List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            if (cronMasterList == null)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            else if (cronMasterList.Count() == 0)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.ReferralCommission).FirstOrDefault();
            if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
            {
                var data = _IMarketCap.GetCronData();
                if (data != null)
                {
                    var today = Helpers.UTC_To_IST();
                    if (Convert.ToDateTime(data.ToDate).AddHours(temp.Hour) < today)
                    {
                        var insertData = _IMarketCap.InsertIntoCron(temp.Hour);
                        if (insertData != null)
                        {
                            var res = _walletSPRepositories.Callsp_ReferCommissionSignUp(insertData.Id, Convert.ToDateTime(insertData.FromDate), Convert.ToDateTime(insertData.ToDate));
                        }
                    }
                }
                else
                {
                    var insertData = _IMarketCap.InsertIntoCron(temp.Hour);
                    if (insertData != null)
                    {
                        var res = _walletSPRepositories.Callsp_ReferCommissionSignUp(insertData.Id, Convert.ToDateTime(insertData.FromDate), Convert.ToDateTime(insertData.ToDate));
                    }
                }
            }
            return Task.FromResult(new Unit());
        }
    }

    public class StakingHandler : IRequestHandler<StakingReqRes>
    {
        private readonly IWalletSPRepositories _walletSPRepositories;
        private readonly IWalletService _WalletService;
        //private readonly IWalletConfiguration _walletConfiguration;
        private IMemoryCache _cache;
        private readonly ITrnMasterConfiguration _walletConfiguration;
        public StakingHandler(IWalletSPRepositories walletSPRepositories, IWalletService WalletService, ITrnMasterConfiguration walletConfiguration, IMemoryCache cache)
        {
            _walletSPRepositories = walletSPRepositories;
            _WalletService = WalletService;
            _walletConfiguration = walletConfiguration;
            _cache = cache;
        }
        public Task<Unit> Handle(StakingReqRes request, CancellationToken cancellationToken)
        {
            CronMaster cronMaster = new CronMaster();
            //cronMaster = _walletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.Staking);
            //if (cronMaster != null && cronMaster.Status == 1)
            List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            if (cronMasterList == null)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            else if (cronMasterList.Count() == 0)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.Staking).FirstOrDefault();
            if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
            {
                UserUnstakingReq req = new UserUnstakingReq();
                var data = _WalletService.GetUnstackingCroneData();
                if (data.Data.Count > 0)
                {
                    foreach (var x in data.Data)
                    {
                        req.ChannelID = x.ChannelID;
                        req.StakingAmount = 0;
                        req.StakingHistoryId = x.Id;
                        req.StakingPolicyDetailId = 0;
                        long UserId = x.UserID;
                        req.Type = Core.Enums.EnUnstakeType.Full;
                        var Resp = _walletSPRepositories.Callsp_UnstakingSchemeRequest(req, UserId, request.IsReqFromAdmin);
                    }
                }
            }
            return Task.FromResult(new Unit());
        }
    }

    public class MarginTransactionHandler : IRequestHandler<ForceWithdrwLoanv2Req>
    {
        private readonly IMarketCap _IwalletService;
        //private readonly IWalletConfiguration _walletConfiguration;
        private readonly ITrnMasterConfiguration _walletConfiguration;
        private IMemoryCache _cache;
        public MarginTransactionHandler(IMarketCap IwalletService, ITrnMasterConfiguration walletConfiguration, IMemoryCache cache)
        {
            _IwalletService = IwalletService;
            _walletConfiguration = walletConfiguration;
            _cache = cache;
        }
        public Task<Unit> Handle(ForceWithdrwLoanv2Req request, CancellationToken cancellationToken)
        {
            CronMaster cronMaster = new CronMaster();
            //cronMaster = _walletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.ForceWithdrwLoan);
            //if (cronMaster != null && cronMaster.Status == 1)
            List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            if (cronMasterList == null)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            else if (cronMasterList.Count() == 0)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.ForceWithdrwLoan).FirstOrDefault();
            if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
            {
                _IwalletService.ForceWithdrwLoan();
            }
            return Task.FromResult(new Unit());
        }
    }

    public class ReleaseProfitHandler : IRequestHandler<ReleaseProfitAmountReq>
    {
        private readonly IWalletSPRepositories _IWalletSPRepositories;
       // private readonly IWalletConfiguration _IWalletConfiguration;
        private readonly ITrnMasterConfiguration _IWalletConfiguration;
        private IMemoryCache _cache;
        public ReleaseProfitHandler(IWalletSPRepositories IWalletSPRepositories, ITrnMasterConfiguration IWalletConfiguration, IMemoryCache cache)
        {
            _IWalletSPRepositories = IWalletSPRepositories;
            _IWalletConfiguration = IWalletConfiguration;
            _cache = cache;
        }
        public Task<Unit> Handle(ReleaseProfitAmountReq request, CancellationToken cancellationToken)
        {
            CronMaster cronMaster = new CronMaster();
            //cronMaster = _IWalletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.NormalTradingFullSettlementReleaseProfitAmount);
            //if (cronMaster != null && cronMaster.Status == 1)
            List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            if (cronMasterList == null)
            {
                cronMasterList = _IWalletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            else if (cronMasterList.Count() == 0)
            {
                cronMasterList = _IWalletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.NormalTradingFullSettlementReleaseProfitAmount).FirstOrDefault();
            if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
            {
                _IWalletSPRepositories.Callsp_NormalTradingFullSettlementReleaseProfitAmountCron(1);
            }
            //ntrivedi 17-07-2019 adding for margin 
            //cronMaster = _IWalletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.MarginTradingFullSettlementReleaseProfitAmount);
            //if (cronMaster != null && cronMaster.Status == 1)
          //  List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            if (cronMasterList == null)
            {
                cronMasterList = _IWalletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            else if (cronMasterList.Count() == 0)
            {
                cronMasterList = _IWalletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.MarginTradingFullSettlementReleaseProfitAmount).FirstOrDefault();
            if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
            {
                _IWalletSPRepositories.Callsp_MarginTradingFullSettlementReleaseProfitAmountCron(1);
            }
            //ntrivedi 17-07-2019 adding 
            cronMaster = _IWalletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.LPHoldLPFailed);
            if (cronMaster != null && cronMaster.Status == 1)
            {
                _IWalletSPRepositories.Callsp_LPHoldLPFailed_HoldAgain(1);
            }
            return Task.FromResult(new Unit());
        }
    }

    public class ServiceProviderHandler : IRequestHandler<ServiceProviderReq, ServiceProviderBalanceResponse>
    {
        string BalanceResp;
        string ethResp;
        decimal ethfee = 0;
        private readonly ICommonRepository<ThirdPartyAPIConfiguration> _thirdPartyCommonRepository;
        private readonly IGetWebRequest _getWebRequest;
        private readonly IWebApiSendRequest _webApiSendRequest;
        private readonly ICommonRepository<ServiceProviderMaster> _ServiceProviderMaster;
        private readonly WebApiParseResponse _WebApiParseResponse;

        public ServiceProviderHandler(ICommonRepository<ThirdPartyAPIConfiguration> thirdPartyCommonRepository, IGetWebRequest getWebRequest, IWebApiSendRequest webApiSendRequest, ICommonRepository<ServiceProviderMaster> ServiceProviderMaster, WebApiParseResponse WebApiParseResponse)
        {
            _thirdPartyCommonRepository = thirdPartyCommonRepository;
            _getWebRequest = getWebRequest;
            _webApiSendRequest = webApiSendRequest;
            _ServiceProviderMaster = ServiceProviderMaster;
            _WebApiParseResponse = WebApiParseResponse;
        }
        public Task<ServiceProviderBalanceResponse> Handle(ServiceProviderReq request, CancellationToken cancellationToken)
        {
            ServiceProviderBalanceResponse Resp = new ServiceProviderBalanceResponse();
            Resp.Data = new List<ServiceProviderBalance>();
            var transactionProviderResponses2 = request.transactionProviderResponses2;
            for (int i = 0; i < transactionProviderResponses2.Count; i++)
            {
                if (transactionProviderResponses2[i].ThirPartyAPIID == 0)
                {
                    Resp = new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.InvalidThirdPartyAPIID, ReturnCode = enResponseCode.Fail, ReturnMsg = "No Data Found." };
                }
                var apiconfig = _thirdPartyCommonRepository.GetByIdAsync(transactionProviderResponses2[i].ThirPartyAPIID);
                var thirdPartyAPIConfiguration = apiconfig.GetAwaiter().GetResult();
                if (thirdPartyAPIConfiguration == null || transactionProviderResponses2.Count == 0)
                {
                    new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.ThirdPartyDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                var thirdPartyAPIRequest = _getWebRequest.MakeWebRequestV2(transactionProviderResponses2[i].RefKey, transactionProviderResponses2[i].Address, transactionProviderResponses2[i].RouteID, transactionProviderResponses2[i].ThirPartyAPIID, transactionProviderResponses2[i].SerProDetailID);
                string apiResponse = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.BalCheckMethodType);

                var serviceProObj = _ServiceProviderMaster.GetById(transactionProviderResponses2[i].ServiceProID);
                if (serviceProObj != null)
                {
                    if (serviceProObj.ProviderName.Equals("ERC-223"))
                    {
                        thirdPartyAPIRequest.RequestURL = thirdPartyAPIRequest.RequestURL.Replace("getTokenBalance", "getEtherBalance");
                    }
                    else
                    {
                        thirdPartyAPIRequest.RequestURL = thirdPartyAPIRequest.RequestURL.Replace("contract/balance", "eth/balance");
                    }
                }
                else
                {
                    Resp = new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.ThirdPartyDataNotFound, ReturnCode = enResponseCode.Fail, ReturnMsg = EnResponseMessage.ItemOrThirdprtyNotFound };
                }
                ethResp = _webApiSendRequest.SendAPIRequestAsync(thirdPartyAPIRequest.RequestURL, thirdPartyAPIRequest.RequestBody, thirdPartyAPIConfiguration.ContentType, 180000, thirdPartyAPIRequest.keyValuePairsHeader, thirdPartyAPIConfiguration.BalCheckMethodType);
                if (!string.IsNullOrEmpty(apiResponse))
                {
                    WebAPIParseResponseCls ParsedResponse = _WebApiParseResponse.TransactionParseResponse(apiResponse, transactionProviderResponses2[i].ThirPartyAPIID);
                    BalanceResp = ParsedResponse.Balance.ToString();
                    if (!string.IsNullOrEmpty(BalanceResp))
                    {
                        decimal responseString = Convert.ToDecimal(BalanceResp);
                        ServiceProviderBalance Result = new ServiceProviderBalance
                        {
                            Balance = responseString,
                            Address = transactionProviderResponses2[i].Address,
                            CurrencyName = transactionProviderResponses2[i].OpCode.ToUpper()
                        };
                        Resp.Data.Add(Result);
                        Resp.ErrorCode = enErrorCode.Success;
                        Resp.ReturnCode = enResponseCode.Success;
                        Resp.ReturnMsg = EnResponseMessage.FindRecored;
                    }
                    else
                    {
                        Resp = new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.BalanceIsNull, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                    }
                }
                if (!string.IsNullOrEmpty(ethResp))
                {
                    WebAPIParseResponseCls ParsedResponses = _WebApiParseResponse.TransactionParseResponse(ethResp, transactionProviderResponses2[i].ThirPartyAPIID);
                    BalanceResp = ParsedResponses.Balance.ToString();
                    if (!string.IsNullOrEmpty(BalanceResp))
                    {
                        ethfee = Convert.ToDecimal(BalanceResp);
                        ServiceProviderBalance Result = new ServiceProviderBalance
                        {
                            Balance = 0,
                            Fee = ethfee,
                            Address = transactionProviderResponses2[i].Address,
                            CurrencyName = "ETH"
                        };
                        Resp.Data.Add(Result);
                    }
                }
                else
                {
                    Resp = new ServiceProviderBalanceResponse { ErrorCode = enErrorCode.NullResponseFromAPI, ReturnCode = enResponseCode.Fail, ReturnMsg = "Fail." };
                }
            }
            return Task.FromResult(Resp);
        }
    }

    public class FiatSellWithdrawHandler : IRequestHandler<FiatSellWithdrawReq>
    {
        private readonly ITrnMasterConfiguration _walletConfiguration;
        private readonly IFiatIntegrateService _fiatIntegrateService;
        private IMemoryCache _cache;
        public FiatSellWithdrawHandler(ITrnMasterConfiguration walletConfiguration, IFiatIntegrateService fiatIntegrateService,IMemoryCache cache)
        {
            _walletConfiguration = walletConfiguration;
            _fiatIntegrateService = fiatIntegrateService;
            _cache = cache;
        }

        public async Task<Unit> Handle(FiatSellWithdrawReq request, CancellationToken cancellationToken)
        {
            CronMaster cronMaster = new CronMaster();
            //cronMaster = _walletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.FiatSellWithdraw);
            //if (cronMaster != null && cronMaster.Status == 1)
            List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            if (cronMasterList == null)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            else if (cronMasterList.Count() == 0)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.FiatSellWithdraw).FirstOrDefault();
            if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
            {
                await _fiatIntegrateService.FiatSellWithdraw();
            }
            throw new NotImplementedException();
        }
        //public async Task<Unit> Handle(FiatBinnanceLTPChange request, CancellationToken cancellationToken)
        //{
        //    //CronMaster cronMaster = new CronMaster();
        //    //cronMaster = _walletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.FiatLTPChangeBinnance);
        //    //if (cronMaster != null && cronMaster.Status == 1)
        //    //{
        //    //    await _fiatIntegrateService.FiatBinnanceLTPUpate();
        //    //}
        //    throw new NotImplementedException();
        //}
    }
    public class FiatLTPWithdrawHandler : IRequestHandler<FiatBinnanceLTPChange>
    {
        private readonly ITrnMasterConfiguration _walletConfiguration;
        private readonly IFiatIntegrateService _fiatIntegrateService;
        private IMemoryCache _cache;
        public FiatLTPWithdrawHandler(ITrnMasterConfiguration walletConfiguration, IFiatIntegrateService fiatIntegrateService, IMemoryCache cache)
        {
            _walletConfiguration = walletConfiguration;
            _fiatIntegrateService = fiatIntegrateService;
            _cache = cache;
        }

        public async Task<Unit> Handle(FiatBinnanceLTPChange request, CancellationToken cancellationToken)
        {
            CronMaster cronMaster = new CronMaster();
            //cronMaster = _walletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.FiatLTPChangeBinnance);
            //if (cronMaster != null && cronMaster.Status == 1)
            List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            if (cronMasterList == null)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            else if (cronMasterList.Count() == 0)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.FiatLTPChangeBinnance).FirstOrDefault();
            if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
            {
                await _fiatIntegrateService.FiatBinnanceLTPUpate();
            }
            throw new NotImplementedException();
        }
    }

    public class FiatPendingWithdrawHandler : IRequestHandler<FiatPendingHashUpdate>
    {
        private readonly ITrnMasterConfiguration _walletConfiguration;
        private readonly IFiatIntegrateService _fiatIntegrateService;
        private IMemoryCache _cache;
        public FiatPendingWithdrawHandler(ITrnMasterConfiguration walletConfiguration, IFiatIntegrateService fiatIntegrateService, IMemoryCache cache)
        {
            _walletConfiguration = walletConfiguration;
            _fiatIntegrateService = fiatIntegrateService;
            _cache = cache;
        }

        public async Task<Unit> Handle(FiatPendingHashUpdate request, CancellationToken cancellationToken)
        {
            CronMaster cronMaster = new CronMaster();
            //cronMaster = _walletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.FiatLTPChangeBinnance);
            //if (cronMaster != null && cronMaster.Status == 1)
            List<CronMaster> cronMasterList = _cache.Get<List<CronMaster>>("CronMaster");
            if (cronMasterList == null)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            else if (cronMasterList.Count() == 0)
            {
                cronMasterList = _walletConfiguration.GetCronMaster();
                _cache.Set("CronMaster", cronMasterList);
            }
            cronMaster = cronMasterList.Where(e => e.Id == (short)enCronMaster.FiatPendingHashUpdate).FirstOrDefault();
            if (cronMaster != null && cronMaster.Status == (short)ServiceStatus.Active)
            {
                 _fiatIntegrateService.UpdateHashFiat();
            }
            throw new NotImplementedException();
        }
    }

    ////ntrivedi 14-08-2019 deleted by chirag bhai by mistake added now
    //public class UserProfitHandler : IRequestHandler<ProfitTemp>
    //{
    //    private readonly IMarketCap _IMarketCap;
    //    private readonly ITrnMasterConfiguration _walletConfiguration;
    //    //private readonly IWalletConfiguration _walletConfiguration;
    //    public UserProfitHandler(IMarketCap IMarketCap, ITrnMasterConfiguration walletConfiguration)
    //    {
    //        _IMarketCap = IMarketCap;
    //        _walletConfiguration = walletConfiguration;
    //    }
    //    public Task<Unit> Handle(ProfitTemp temp, CancellationToken cancellationToken)
    //    {
    //        CronMaster cronMaster; //CronMaster cronMaster = new CronMaster();
    //        cronMaster = _walletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.TradingProfit);
    //        if (cronMaster != null && cronMaster.Status == 1)
    //        {
    //            var res = _IMarketCap.CallSP_InsertUpdateProfit(temp.Date, temp.CurrencyName);
    //        }
    //        cronMaster = _walletConfiguration.GetCronMaster().Find(e => e.Id == (short)enCronMaster.ArbitrageTradingProfit);
    //        if (cronMaster != null && cronMaster.Status == 1)
    //        {
    //            var resArbitrage = _IMarketCap.CallSP_ArbitrageInsertUpdateProfit(temp.Date, temp.CurrencyName);
    //        }
    //        return Task.FromResult(new Unit());
    //    }
    //}
}
