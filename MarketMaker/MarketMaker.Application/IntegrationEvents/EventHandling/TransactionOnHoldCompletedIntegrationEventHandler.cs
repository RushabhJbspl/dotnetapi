using System;
using EventBusRabbitMQ.EventHandlers;
using FluentValidation.Results;
using MarketMaker.Application.IntegrationEvents.Events;
using MarketMaker.Application.Interfaces.Queries;
using MarketMaker.Application.Interfaces.Services.Redis;
using MarketMaker.Application.ViewModels.Queries;
using MarketMaker.Domain.Enum;
using MediatR;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using LoggingNlog;
using MarketMaker.Application.ViewModels.Config;
using Microsoft.Extensions.Options;

namespace MarketMaker.Application.IntegrationEvents.EventHandling
{
    public sealed class TransactionOnHoldCompletedIntegrationEventHandler : IIntegrationEventHandler<TransactionOnHoldCompletedIntegrationEvent>
    {

        //private const short MARKET_MAKER_ON = 1;
        //private const short MARKET_MAKER_USER_ROLE_VALID = 1;

        private readonly IMarketMakerQueries _iMarketMakerQueries;
        private readonly IRedisTradingManagement _iRedisTradingManagement;
        private readonly IMediator _iMediator;
        private readonly INLogger<TransactionOnHoldCompletedIntegrationEventHandler> _logger;
        private readonly MarketMakerConfigs _marketMakerConfigs;

        public TransactionOnHoldCompletedIntegrationEventHandler(IMarketMakerQueries iMarketMakerQueries, IRedisTradingManagement iRedisTradingManagement, IMediator iMediator, INLogger<TransactionOnHoldCompletedIntegrationEventHandler> logger, IOptions<MarketMakerConfigs> iOptions)
        {
            _iMarketMakerQueries = iMarketMakerQueries;
            _iRedisTradingManagement = iRedisTradingManagement;
            _iMediator = iMediator;
            _logger = logger;
            _marketMakerConfigs = iOptions.Value;
        }

        public Task Handle(TransactionOnHoldCompletedIntegrationEvent @event)
        {
            _logger.WriteInfoLog("Handle", $"transaction event occur for {{userId: {@event.UserId}}}, {{TransactionId: {@event.TransactionId}}}, OrderType: {@event.TransactionType}");

            #region Commented code of MarketMaker on/off and role validation

            //remove MarketMaker on/off and role validation hence it has been placed where event publish in CleanArchitecture.api project -Sahil 03-10-2019 06:30 PM
            //Validate MarkerMaker is on and User have MarketMaker role -Sahil 27-09-2019

            //short marketMakerStatus = _iMarketMakerQueries.GetMarketMakerStatusAsync().Result;
            //short marketMakerUserRoleStatus = _iMarketMakerQueries.GetMarketMakerUserRoleStatusAsync(@event.UserId).Result;

            //if (marketMakerStatus != MARKET_MAKER_ON &&
            //    marketMakerUserRoleStatus != MARKET_MAKER_USER_ROLE_VALID) return Task.CompletedTask;

            #endregion
            try
            {
                if (@event.TransactionType == TransactionType.Buy && IsMarketMakerUserSellTransactionValid(@event.UserId, @event.Pair, @event.Price))
                {
                    _logger.WriteInfoLog("Handle", $"{{userId: {@event.UserId}}}, {{TransactionId: {@event.TransactionId}}},transaction: {@event.TransactionType} IsMarketMakerUserSellTransactionValid: true");

                    //call event directly using mediator for testing buy/sell api -Sahil 07-10-2-2019 04:29 PM
                    //place sell order call for buy transaction type -Sahil 10-10-2019 01:04 PM
                    _iMediator.Publish(new UserBalanceCheckCompletedIntegrationEvent(
                        @event.Pair,
                        @event.Price,
                        @event.Quantity,
                        (short)TransactionType.Sell));
                    return Task.CompletedTask;
                }

                if (@event.TransactionType == TransactionType.Sell && IsMarketMakerUserBuyTransactionValid(@event.TransactionId, @event.Pair, @event.Price))
                {
                    _logger.WriteInfoLog("Handle", $"{{userId: {@event.UserId}}}, {{TransactionId: {@event.TransactionId}}},transaction: {@event.TransactionType} IsMarketMakerUserSellTransactionValid: true");

                    //call event directly using mediator for testing buy/sell api -Sahil 07-10-2-2019 04:29 PM
                    _iMediator.Publish(new UserBalanceCheckCompletedIntegrationEvent(
                        @event.Pair,
                        @event.Price,
                        @event.Quantity,
                        (short)TransactionType.Buy));
                    return Task.CompletedTask;
                }
            }
            catch (Exception e)
            {
                _logger.WriteErrorLog("Handle", e);
                _logger.WriteInfoLog("Handle", $"market maker order not occur for {{userId: {@event.UserId}}}, {{TransactionId: {@event.TransactionId}}}, exception thrown check error log.");

            }

            _logger.WriteInfoLog("Handle", $"{{userId: {@event.UserId}}}, {{TransactionId: {@event.TransactionId}}}, no transaction done, all validation bypassed.");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Check for market maker has set preferences for sell transaction
        /// and sell transaction price is under range  
        /// </summary>
        /// <param name="transactionId"> transaction id</param>
        /// <param name="pair"> currency pair</param>
        /// <param name="price"> market maker transaction price</param>
        /// <returns> return true if all condition valid</returns>
        /// <remarks>-Sahil 30-09-2019</remarks>
        private bool IsMarketMakerUserSellTransactionValid(long transactionId, long pair, decimal price)
        {
            var preference = _iMarketMakerQueries.GetMarketMakerUserSellPreferencesAsync(pair).Result;
            ValidationResult validationResult = new MarketMakerSellPreferencesValidator().Validate(preference);

            _logger.WriteInfoLog("IsMarketMakerUserSellTransactionValid", $"database get MarketMaker preference: {validationResult.IsValid}, empty preferences count: {validationResult.Errors.Count} ");
            if (!validationResult.IsValid) return false;

            if (preference.SellLTPRangeType == RangeType.Percentage)
            {
                var data = _iRedisTradingManagement.GetTickerDataAsync(preference.ProviderName, preference.PairName).Result;

                if (data?.UpdateDate == null) return false;

                var currentTime = DateTime.UtcNow;
                var tickerDataTime = DateTime.ParseExact(data.UpdateDate.ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture); //add for solve date parsing error for redis data date -Sahil 15-10-2019 11:28 AM

                _logger.WriteInfoLog("IsMarketMakerUserBuyTransactionValid", $"currentTime : {currentTime}, tickerDataTime:{tickerDataTime} /n LTP: {data.LTP}, Sell ThresHold: {preference.SellThreshold}");
                if (currentTime.Subtract(tickerDataTime) > TimeSpan.FromMinutes(_marketMakerConfigs.tickerDataTimeLimit)) return false; //ticker data is not older then 1 min validation -Sahil 12-10-2019 03:40 PM

                if (data.LTP >= preference.SellThreshold
                    && IsOrderPriceUnderPercentageRange(price, data.LTP, preference.SellDownPercentage,
                        preference.SellUpPercentage))
                {
                    _logger.WriteInfoLog("IsMarketMakerUserSellTransactionValid", $"{{TransactionId: {transactionId}}},LTP get form ticker: {data.LTP }, preference threshold:{preference.SellThreshold}, IsOrderPriceUnderPercentageRange: true");
                    return true;
                }

                return false;
            }

            if (preference.SellLTPRangeType == RangeType.Fix)
            {
                var result = _iMarketMakerQueries.GetMarketMakerFixRangeDetailsAsync(preference.Id).Result;
                if (CheckOrderPriceUnderFixRange(price, result))
                {
                    _logger.WriteInfoLog("IsMarketMakerUserSellTransactionValid", $"{{TransactionId: {transactionId}}}, CheckOrderPriceUnderFixRange: true");
                    return true;
                }
                return false;
            }

            return false;
        }

        /// <summary>
        /// Check for market maker has set preferences for buy transaction
        /// and buy transaction price is under range  
        /// </summary>
        /// <param name="transactionId">trnasaction id</param>
        /// <param name="pair"> currency pair</param>
        /// <param name="price"> market maker transaction price</param>
        /// <returns> return true if all condition valid</returns>
        /// <remarks>-Sahil 30-09-2019</remarks>
        private bool IsMarketMakerUserBuyTransactionValid(long transactionId, long pair, decimal price)
        {
            var preference = _iMarketMakerQueries.GetMarketMakerUserBuyPreferencesAsync(pair).Result;
            ValidationResult validationResult = new MarketMakerBuyPreferencesValidator().Validate(preference);
            _logger.WriteInfoLog("IsMarketMakerUserBuyTransactionValid", $"preference validation : {validationResult.IsValid}, failed count: {validationResult.Errors.Count}");
            if (!validationResult.IsValid) return false;

            if (preference.BuyLTPRangeType == RangeType.Percentage)
            {
                //fetch last price data from redis and check price is under range -Sahil 30-09-2019
                var data = _iRedisTradingManagement.GetTickerDataAsync(preference.ProviderName, preference.PairName).Result;

                if (data?.UpdateDate == null) return false;

                var currentTime = DateTime.UtcNow;
                var tickerDataTime = DateTime.ParseExact(data.UpdateDate.ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture); //add for solve date parsing error for redis data date -Sahil 15-10-2019 11:28 AM

                _logger.WriteInfoLog("IsMarketMakerUserBuyTransactionValid", $"currentTime : {currentTime}, tickerDataTime:{tickerDataTime} /n LTP: {data.LTP}, Buy ThresHold: {preference.BuyThreshold}");
                if (currentTime.Subtract(tickerDataTime) > TimeSpan.FromMinutes(_marketMakerConfigs.tickerDataTimeLimit)) return false; //ticker data is not older then 1 min validation -Sahil 12-10-2019 03:40 PM


                if (data.LTP <= preference.BuyThreshold
                    && IsOrderPriceUnderPercentageRange(price, data.LTP, preference.BuyDownPercentage,
                        preference.BuyUpPercentage))
                {
                    _logger.WriteInfoLog("IsMarketMakerUserBuyTransactionValid", $"{{TransactionId: {transactionId}}}, LTP get form ticker: {data.LTP }, preference threshold:{preference.BuyThreshold}, IsOrderPriceUnderPercentageRange: true");
                    return true;
                }

                return false;
            }

            if (preference.BuyLTPRangeType == RangeType.Fix)
            {
                //fetch range from database and check price is under range -Sahil 30-09-2019
                var data = _iMarketMakerQueries.GetMarketMakerFixRangeDetailsAsync(preference.Id).Result;
                if (CheckOrderPriceUnderFixRange(price, data))
                {
                    _logger.WriteInfoLog("IsMarketMakerUserBuyTransactionValid", $"{{transactionId: {transactionId}}}, CheckOrderPriceUnderFixRange: true");
                    return true;
                }
                return false;
            }

            return false;
        }

        private bool CheckOrderPriceUnderFixRange(decimal eventPrice, List<MarketMakerUserFixRangeDetail> rangeDetails)
        {
            //loop over user fix range list and return true if minimum <= eventPrice <= maximum -Sahil 30-09-2019
            foreach (var rangeDetail in rangeDetails)
            {
                if (rangeDetail.RangeMin <= eventPrice &&
                    eventPrice <= rangeDetail.RangeMax)
                {
                    return true;
                }
            }

            _logger.WriteInfoLog("CheckOrderPriceUnderFixRange", "method return: false");
            return false;
        }

        private bool IsOrderPriceUnderPercentageRange(decimal eventPrice, decimal dataLtp, double preferenceDownPercentage, double preferenceUpPercentage)
        {
            /* expression take current price (dataLtp) and find minimum and maximum range -Sahil 28-09-2019
             * minimum = dataLtp [current market price] - ((dataLtp * preferenceDownPercentage) / 100)) [user lower limit price]
             * maximum = dataLtp [current market price] + (dataLtp + (dataLtp * preferenceUpPercentage) / 100) [user higher limit price]
             * then check  minimum <= eventPrice <= maximum
             */

            var debugMin = (dataLtp - ((dataLtp * (decimal)preferenceDownPercentage) / 100)); // 1.5 
            var debugMax = (dataLtp + ((dataLtp * (decimal)preferenceUpPercentage) / 100)); // 3

            if (debugMin <= eventPrice &&
                eventPrice <= debugMax)
                return true;

            _logger.WriteInfoLog("IsOrderPriceUnderPercentageRange", $"{eventPrice} not under price range min range: {debugMin}, max range: {debugMax}, method return: false");
            return false;
        }
    }
}
