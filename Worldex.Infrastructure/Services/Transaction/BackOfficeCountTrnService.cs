using Worldex.Core.Entities;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Transaction.BackOfficeCount;
using Worldex.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.Services.Transaction
{
    public class BackOfficeCountTrnService : IBackOfficeCountTrnService
    {
        private readonly IBackOfficeCountTrnRepository _backOfficeCountTrnRepository;
        private readonly IBasePage _basePage;

        public BackOfficeCountTrnService(IBackOfficeCountTrnRepository backOfficeCountTrnRepository,IBasePage basePage)
        {
            _backOfficeCountTrnRepository = backOfficeCountTrnRepository;
            _basePage = basePage;
        }

        #region count and MArgin Method

        public ActiveTradeUserTime GetActiveTradeUserCount(short IsMargin = 0)//Rita 6-3-19 for Margin Trading
        {
            try
            {
                ActiveTradeUserTime Response = new ActiveTradeUserTime();
                ActiveTradeUserStatus StatusResponse = new ActiveTradeUserStatus();

                //get the all count
                ActiveTradeUserCount Data;
                if (IsMargin == 1)
                    Data = _backOfficeCountTrnRepository.GetActiveTradeUserCountMargin();
                else
                    Data = _backOfficeCountTrnRepository.GetActiveTradeUserCount();

                //asign the all count in statuswise and timewise

                //Total Count
                Response.TotalCount = Data.TotalCount;

                //Today Count
                StatusResponse.Total = Data.TodayCount;
                StatusResponse.Active = Data.TodayActiveCount;
                StatusResponse.Cancel = Data.TodayCancelCount;
                StatusResponse.Settled = Data.TodaySettleCount;
                StatusResponse.PartialCancel = Data.TodayPartialCancelCount;
                StatusResponse.SystemFail = Data.TodaySystemFailCount;
                Response.Today = StatusResponse;

                //Week Count
                StatusResponse = new ActiveTradeUserStatus();
                StatusResponse.Total = Data.WeekCount;
                StatusResponse.Active = Data.WeekActiveCount;
                StatusResponse.Cancel = Data.WeekCancelCount;
                StatusResponse.Settled = Data.WeekSettleCount;
                StatusResponse.PartialCancel = Data.WeekPartialCancelCount;
                StatusResponse.SystemFail = Data.WeekSystemFailCount;
                Response.Week = StatusResponse;

                //Month Count
                StatusResponse = new ActiveTradeUserStatus();
                StatusResponse.Total = Data.MonthCount;
                StatusResponse.Active = Data.MonthActiveCount;
                StatusResponse.Cancel = Data.MonthCancelCount;
                StatusResponse.Settled = Data.MonthSettleCount;
                StatusResponse.PartialCancel = Data.MonthPartialCancelCount;
                StatusResponse.SystemFail = Data.MonthSystemFailCount;
                Response.Month = StatusResponse;

                //Year Count
                StatusResponse = new ActiveTradeUserStatus();
                StatusResponse.Total = Data.YearCount;
                StatusResponse.Active = Data.YearActiveCount;
                StatusResponse.Cancel = Data.YearCancelCount;
                StatusResponse.Settled = Data.YearSettleCount;
                StatusResponse.PartialCancel = Data.YearPartialCancelCount;
                StatusResponse.SystemFail = Data.YearSystemFailCount;
                Response.Year = StatusResponse;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ConfigurationCountResponse GetConfigurationCount(short IsMargin=0)
        {
            try
            {
                ConfigurationCountResponse _Res = new ConfigurationCountResponse();
                ConfigurationCount Data;
                if (IsMargin == 1)
                    Data = _backOfficeCountTrnRepository.GetConfigurationCountMargin();
                else
                    Data = _backOfficeCountTrnRepository.GetConfigurationCount();

                _Res.Response = Data;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public LedgerCountResponse GetLedgerCount(short IsMargin = 0)//Rita 6-3-19 for Margin Trading
        {
            try
            {
                LedgerCountResponse _Res = new LedgerCountResponse();
                LedgerCountInfo countInfo = new LedgerCountInfo();
                if (IsMargin == 1)
                    countInfo = _backOfficeCountTrnRepository.GetLedgerCountMargin();
                else
                    countInfo = _backOfficeCountTrnRepository.GetLedgerCount();

                _Res.Response = countInfo;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public OrderSummaryCount GetOrderSummaryCount(short IsMargin = 0)//Rita 6-3-19 for Margin Trading
        {
            try
            {
                OrderSummaryCount Response = new OrderSummaryCount();
                if (IsMargin == 1)
                    Response = _backOfficeCountTrnRepository.GetOrderSummaryCountMargin();
                else
                    Response = _backOfficeCountTrnRepository.GetOrderSummaryCount();

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TradeSummaryCountResponse GetTradeSummaryCount(short IsMargin = 0)//Rita 6-3-19 for Margin Trading
        {
            try
            {
                TradeSummaryCountResponse _Res = new TradeSummaryCountResponse();
                TradeSummaryCount Data;
                if (IsMargin == 1)
                     Data = _backOfficeCountTrnRepository.GetTradeSummaryCountMargin();
                else
                     Data = _backOfficeCountTrnRepository.GetTradeSummaryCount();

                TradeSummaryCountResponseInfo tradeSummary = new TradeSummaryCountResponseInfo();

                tradeSummary.TOTALTRADE = Data.TOTALTRADE;
                tradeSummary.LIMIT = new LimitCls() {
                    TotLIMIT= Data.LIMIT,
                    LIMIT_BUY = Data.LIMIT_BUY,
                    LIMIT_SELL = Data.LIMIT_SELL,
                    };
                tradeSummary.MARKET= new MarketCls()
                {
                    TotMARKET = Data.MARKET,
                    MARKET_BUY = Data.MARKET_BUY,
                    MARKET_SELL = Data.MARKET_SELL,
                };
                tradeSummary.SPOT= new SpotCls() {
                    TotSPOT = Data.SPOT,
                    SPOT_BUY = Data.SPOT_BUY,
                    SPOT_SELL = Data.SPOT_SELL,
                };
                tradeSummary.STOP_Limit= new Stop_LimitCls() {
                    TotSTOP_Limit = Data.STOP_Limit,
                    STOP_Limit_BUY = Data.STOP_Limit_BUY,
                    STOP_Limit_SELL = Data.STOP_Limit_SELL,
                };
                tradeSummary.STOP= new StopCls() {
                    TotSTOP = Data.STOP,
                    STOP_BUY = Data.STOP_BUY,
                    STOP_SELL = Data.STOP_SELL,
                };
                _Res.Response = tradeSummary;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ProfitSummaryCount GetProfitSummaryCount(short IsMargin = 0)//Rita 6-3-19 for Margin Trading
        {   
            try
            {
                ProfitSummaryCount Response = new ProfitSummaryCount();
                if (IsMargin == 1)
                    Response = _backOfficeCountTrnRepository.GetProfitSummaryCountMargin();
                else
                    Response = _backOfficeCountTrnRepository.GetProfitSummaryCount();

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TradeSummaryCountResponseInfo GetTradeUserMarketTypeCount(string Type, short IsMargin = 0)//Rita 6-3-19 for Margin Trading
        {
            try
            {
                TradeSummaryCount Data;
                if (IsMargin == 1)
                    Data = _backOfficeCountTrnRepository.GetTradeUserMarketTypeCountMargin(Type);
                else
                    Data = _backOfficeCountTrnRepository.GetTradeUserMarketTypeCount(Type);

                TradeSummaryCountResponseInfo tradeSummary = new TradeSummaryCountResponseInfo();

                tradeSummary.TOTALTRADE = Data.TOTALTRADE;
                tradeSummary.LIMIT = new LimitCls()
                {
                    TotLIMIT = Data.LIMIT,
                    LIMIT_BUY = Data.LIMIT_BUY,
                    LIMIT_SELL = Data.LIMIT_SELL,
                };
                tradeSummary.MARKET = new MarketCls()
                {
                    TotMARKET = Data.MARKET,
                    MARKET_BUY = Data.MARKET_BUY,
                    MARKET_SELL = Data.MARKET_SELL,
                };
                tradeSummary.SPOT = new SpotCls()
                {
                    TotSPOT = Data.SPOT,
                    SPOT_BUY = Data.SPOT_BUY,
                    SPOT_SELL = Data.SPOT_SELL,
                };
                tradeSummary.STOP_Limit = new Stop_LimitCls()
                {
                    TotSTOP_Limit = Data.STOP_Limit,
                    STOP_Limit_BUY = Data.STOP_Limit_BUY,
                    STOP_Limit_SELL = Data.STOP_Limit_SELL,
                };
                tradeSummary.STOP = new StopCls()
                {
                    TotSTOP = Data.STOP,
                    STOP_BUY = Data.STOP_BUY,
                    STOP_SELL = Data.STOP_SELL,
                };

                return tradeSummary;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TransactionReportCountResponse TransactionReportCount()
        {
            try
            {
                return _backOfficeCountTrnRepository.TransactionReportCount();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        #endregion

        #region Arbitrage Method

        public ActiveTradeUserTime GetActiveTradeUserCountArbitrage(string Status)
        {
            try
            {
                ActiveTradeUserTime Response = new ActiveTradeUserTime();
                ActiveTradeUserStatus StatusResponse = new ActiveTradeUserStatus();
                //get the all count
                ActiveTradeUserCount Data = _backOfficeCountTrnRepository.GetActiveTradeUserCountArbitrage(Status);


                //asign the all count in statuswise and timewise
                //Total Count
                Response.TotalCount = Data.TotalCount;

                //Today Count
                StatusResponse.Total = Data.TodayCount;
                StatusResponse.Active = Data.TodayActiveCount;
                StatusResponse.Cancel = Data.TodayCancelCount;
                StatusResponse.Settled = Data.TodaySettleCount;
                StatusResponse.PartialCancel = Data.TodayPartialCancelCount;
                StatusResponse.SystemFail = Data.TodaySystemFailCount;
                Response.Today = StatusResponse;

                //Week Count
                StatusResponse = new ActiveTradeUserStatus();
                StatusResponse.Total = Data.WeekCount;
                StatusResponse.Active = Data.WeekActiveCount;
                StatusResponse.Cancel = Data.WeekCancelCount;
                StatusResponse.Settled = Data.WeekSettleCount;
                StatusResponse.PartialCancel = Data.WeekPartialCancelCount;
                StatusResponse.SystemFail = Data.WeekSystemFailCount;
                Response.Week = StatusResponse;

                //Month Count
                StatusResponse = new ActiveTradeUserStatus();
                StatusResponse.Total = Data.MonthCount;
                StatusResponse.Active = Data.MonthActiveCount;
                StatusResponse.Cancel = Data.MonthCancelCount;
                StatusResponse.Settled = Data.MonthSettleCount;
                StatusResponse.PartialCancel = Data.MonthPartialCancelCount;
                StatusResponse.SystemFail = Data.MonthSystemFailCount;
                Response.Month = StatusResponse;

                //Year Count
                StatusResponse = new ActiveTradeUserStatus();
                StatusResponse.Total = Data.YearCount;
                StatusResponse.Active = Data.YearActiveCount;
                StatusResponse.Cancel = Data.YearCancelCount;
                StatusResponse.Settled = Data.YearSettleCount;
                StatusResponse.PartialCancel = Data.YearPartialCancelCount;
                StatusResponse.SystemFail = Data.YearSystemFailCount;
                Response.Year = StatusResponse;

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public ConfigurationCountResponse GetConfigurationCountArbitrage()
        {
            try
            {
                ConfigurationCountResponse _Res = new ConfigurationCountResponse();
                ConfigurationCount Data= _backOfficeCountTrnRepository.GetConfigurationCountArbitrage();

                _Res.Response = Data;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TradeSummaryCountResponse GetTradeSummaryCountArbitrage()
        {
            try
            {
                TradeSummaryCountResponse _Res = new TradeSummaryCountResponse();
                TradeSummaryCount Data = _backOfficeCountTrnRepository.GetTradeSummaryCountArbitrage();

                TradeSummaryCountResponseInfo tradeSummary = new TradeSummaryCountResponseInfo();

                tradeSummary.TOTALTRADE = Data.TOTALTRADE;
                tradeSummary.LIMIT = new LimitCls()
                {
                    TotLIMIT = Data.LIMIT,
                    LIMIT_BUY = Data.LIMIT_BUY,
                    LIMIT_SELL = Data.LIMIT_SELL,
                };
                tradeSummary.MARKET = new MarketCls()
                {
                    TotMARKET = Data.MARKET,
                    MARKET_BUY = Data.MARKET_BUY,
                    MARKET_SELL = Data.MARKET_SELL,
                };
                tradeSummary.SPOT = new SpotCls()
                {
                    TotSPOT = Data.SPOT,
                    SPOT_BUY = Data.SPOT_BUY,
                    SPOT_SELL = Data.SPOT_SELL,
                };
                tradeSummary.STOP_Limit = new Stop_LimitCls()
                {
                    TotSTOP_Limit = Data.STOP_Limit,
                    STOP_Limit_BUY = Data.STOP_Limit_BUY,
                    STOP_Limit_SELL = Data.STOP_Limit_SELL,
                };
                tradeSummary.STOP = new StopCls()
                {
                    TotSTOP = Data.STOP,
                    STOP_BUY = Data.STOP_BUY,
                    STOP_SELL = Data.STOP_SELL,
                };
                _Res.Response = tradeSummary;
                _Res.ReturnCode = enResponseCode.Success;
                _Res.ErrorCode = enErrorCode.Success;
                _Res.ReturnMsg = "Success";
                return _Res;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }

        public TradeSummaryCountResponseInfo GetTradeUserMarketTypeCountArbitrage(string Type, string Status)
        {
            try
            {
                TradeSummaryCount Data = _backOfficeCountTrnRepository.GetTradeUserMarketTypeCountArbitrage(Type, Status);

                TradeSummaryCountResponseInfo tradeSummary = new TradeSummaryCountResponseInfo();

                tradeSummary.TOTALTRADE = Data.TOTALTRADE;
                tradeSummary.LIMIT = new LimitCls()
                {
                    TotLIMIT = Data.LIMIT,
                    LIMIT_BUY = Data.LIMIT_BUY,
                    LIMIT_SELL = Data.LIMIT_SELL,
                };
                tradeSummary.MARKET = new MarketCls()
                {
                    TotMARKET = Data.MARKET,
                    MARKET_BUY = Data.MARKET_BUY,
                    MARKET_SELL = Data.MARKET_SELL,
                };
                tradeSummary.SPOT = new SpotCls()
                {
                    TotSPOT = Data.SPOT,
                    SPOT_BUY = Data.SPOT_BUY,
                    SPOT_SELL = Data.SPOT_SELL,
                };
                tradeSummary.STOP_Limit = new Stop_LimitCls()
                {
                    TotSTOP_Limit = Data.STOP_Limit,
                    STOP_Limit_BUY = Data.STOP_Limit_BUY,
                    STOP_Limit_SELL = Data.STOP_Limit_SELL,
                };
                tradeSummary.STOP = new StopCls()
                {
                    TotSTOP = Data.STOP,
                    STOP_BUY = Data.STOP_BUY,
                    STOP_SELL = Data.STOP_SELL,
                };

                return tradeSummary;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                throw ex;
            }
        }
        #endregion
    }
}
