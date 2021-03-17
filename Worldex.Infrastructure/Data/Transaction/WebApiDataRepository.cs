using Worldex.Core.ApiModels;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
//using Worldex.Core.ViewModels.Wallet;
//using Worldex.Infrastructure.DTOClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
//using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Data.Transaction
{
    public class WebApiDataRepository : IWebApiRepository
    {
        private readonly WorldexContext _dbContext;
        public readonly ILogger<WebApiDataRepository> _log;
        private IMemoryCache _cache;

        public WebApiDataRepository(WorldexContext dbContext, ILogger<WebApiDataRepository> log, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _log = log;
            _cache = cache;
        }

        public WebApiConfigurationResponse GetThirdPartyAPIData(long ThirPartyAPIID)
        {
            var result = from TP in _dbContext.ThirdPartyAPIConfiguration
                         where TP.Id == ThirPartyAPIID && TP.Status == 1
                         select new WebApiConfigurationResponse
                         {
                             ThirPartyAPIID = TP.Id,
                             APISendURL = TP.APISendURL,
                             APIValidateURL = TP.APIValidateURL,
                             APIBalURL = TP.APIBalURL,
                             APIStatusCheckURL = TP.APIStatusCheckURL,
                             APIRequestBody = TP.APIRequestBody,
                             TransactionIdPrefix = TP.TransactionIdPrefix,
                             MerchantCode = TP.MerchantCode,
                             //UserID = TP.UserID,
                             //Password = TP.Password,
                             AuthHeader = TP.AuthHeader,
                             ContentType = TP.ContentType,
                             MethodType = TP.MethodType,
                             HashCode = TP.HashCode,
                             HashCodeRecheck = TP.HashCodeRecheck,
                             HashType = TP.HashType,
                             AppType = TP.AppType
                         };
            return result.FirstOrDefault();
        }
        public GetDataForParsingAPI GetDataForParsingAPI(long ThirPartyAPIID)
        {
            var result = from TP in _dbContext.ThirdPartyAPIConfiguration
                         join Regex in _dbContext.ThirdPartyAPIResponseConfiguration on TP.ParsingDataID equals Regex.Id
                         where TP.Id == ThirPartyAPIID && TP.Status == 1
                         select new GetDataForParsingAPI
                         {
                             ResponseSuccess = TP.ResponseSuccess,
                             ResponseFailure = TP.ResponseFailure,
                             ResponseHold = TP.ResponseHold,
                             BalanceRegex = Regex.BalanceRegex,
                             StatusRegex = Regex.StatusRegex,
                             StatusMsgRegex = Regex.StatusMsgRegex,
                             ResponseCodeRegex = Regex.ResponseCodeRegex,
                             ErrorCodeRegex = Regex.ErrorCodeRegex,
                             TrnRefNoRegex = Regex.TrnRefNoRegex,
                             OprTrnRefNoRegex = Regex.OprTrnRefNoRegex,
                             Param1Regex = Regex.Param1Regex,
                             Param2Regex = Regex.Param2Regex,
                             Param3Regex = Regex.Param3Regex
                         };
            return result.FirstOrDefault();
        }

        public GetDataForParsingAPI ArbitrageGetDataForParsingAPI(long ThirPartyAPIID)
        {
            var result = from TP in _dbContext.ArbitrageThirdPartyAPIConfiguration
                         join Regex in _dbContext.ArbitrageThirdPartyAPIResponseConfiguration on TP.ParsingDataID equals Regex.Id
                         where TP.Id == ThirPartyAPIID && TP.Status == 1
                         select new GetDataForParsingAPI
                         {
                             ResponseSuccess = TP.ResponseSuccess,
                             ResponseFailure = TP.ResponseFailure,
                             ResponseHold = TP.ResponseHold,
                             BalanceRegex = Regex.BalanceRegex,
                             StatusRegex = Regex.StatusRegex,
                             StatusMsgRegex = Regex.StatusMsgRegex,
                             ResponseCodeRegex = Regex.ResponseCodeRegex,
                             ErrorCodeRegex = Regex.ErrorCodeRegex,
                             TrnRefNoRegex = Regex.TrnRefNoRegex,
                             OprTrnRefNoRegex = Regex.OprTrnRefNoRegex,
                             Param1Regex = Regex.Param1Regex,
                             Param2Regex = Regex.Param2Regex,
                             Param3Regex = Regex.Param3Regex,
                             Param4Regex = Regex.Param4Regex,
                             Param5Regex = Regex.Param5Regex,
                             Param6Regex = Regex.Param6Regex,
                             Param7Regex = Regex.Param7Regex,
                         };
            return result.FirstOrDefault();
        }
        //ntrivedi fetch route
        public List<TransactionProviderResponse> GetProviderDataList(TransactionApiConfigurationRequest Request)
        {
            try
            {
                #region old code
                //and {2} between RC.MinimumAmount and RC.MaximumAmount
                //and {2}  between SC.MinimumAmount and SC.MaximumAmount
                //IQueryable<TransactionProviderResponse> Result = _dbContext.TransactionProviderResponse.FromSql(
                //            @"select SC.ID as ServiceID,SC.ServiceName,Prc.ID as SerProDetailID,Prc.ServiceProID,RC.ID as RouteID,
                //            PC.ID as ProductID,RC.RouteName,SC.ServiceType,Prc.ThirPartyAPIID,Prc.AppTypeID,RC.MinimumAmount as MinimumAmountItem,
                //            RC.MaximumAmount as MaximumAmountItem,SC.MinimumAmount as MinimumAmountService,SC.MaximumAmount as MaximumAmountService
                //            from ServiceConfiguration SC inner join  ProductConfiguration PC on
                //   PC.ServiceID = SC.Id inner join RouteConfiguration RC on RC.ProductID = PC.Id  
                //   inner join ServiceProviderDetail PrC on Prc.ServiceProID = RC.SerProID AND Prc.TrnTypeID={1} 
                //   where SC.SMSCode = {0} and RC.TrnType={1} 
                //   and {2} between RC.MinimumAmount and RC.MaximumAmount
                //   and {3} between SC.MinimumAmount and SC.MaximumAmount
                //   and SC.Status = 1 and RC.Status = 1 and Prc.Status=1 
                //   order by RC.Priority", Request.SMSCode, Request.trnType, Request.amount,Request.amount);

                // ntrivedi limit changes done 12-10-2018
                //Rita 13-10-2018  remove as no present in New table Service master
                //,SC.MinimumAmount as MinimumAmountService,SC.MaximumAmount as MaximumAmountService
                //and {3} between SC.MinimumAmount and SC.MaximumAmount
                //Rita 17-10-18 further make inner joi in RoutMaster with SerProDetailID with providerdetail
                #endregion
                IQueryable<TransactionProviderResponse> Result = _dbContext.TransactionProviderResponse.FromSql(
                           @"select SC.ID as ServiceID,SC.Name as ServiceName,Prc.ID as SerProDetailID,Prc.ServiceProID,RC.ID as RouteID,
                            PC.ID as ProductID,RC.RouteName,SC.ServiceType,Prc.ThirPartyAPIID,Prc.AppTypeID,LC.MinAmt as MinimumAmountItem,
                            LC.MaxAmt as MaximumAmountItem
                            from ServiceMaster SC inner join  ProductConfiguration PC on
			                PC.ServiceID = SC.Id inner join RouteConfiguration RC on RC.ProductID = PC.Id  
			                inner join ServiceProviderDetail PrC on Prc.Id = RC.SerProDetailID AND Prc.TrnTypeID={1} 
							inner join Limits LC on LC.ID = RC.LimitID 
			                where SC.SMSCode = {0}  and RC.TrnType={1} 
			                and {2} between LC.MinAmt and LC.MaxAmt			                
			                and SC.Status = 1 and RC.Status = 1 and Prc.Status=1 
			                order by RC.Priority", Request.SMSCode, Request.trnType, Request.amount);

                #region old code

                //return Result.ToList();Prc.SerProName,


                //IQueryable<TransactionProviderResponse> Result = _dbContext.TransactionProviderResponse.FromSql(
                //        @"select SC.ID as ServiceID,SC.ServiceName,Prc.ID as SerProID,Prc.SerProName,RC.ID as RouteID,PC.ID as ProductID,RC.RouteName,SC.ServiceType,
                //        Prc.ThirPartyAPIID,Prc.AppType,RC.MinimumAmount as MinimumAmountItem,RC.MaximumAmount as MaximumAmountItem,SC.MinimumAmount as MinimumAmountService,SC.MaximumAmount as MaximumAmountService
                //        from ServiceConfiguration SC inner
                //        join ProductConfiguration PC on
                //        PC.ServiceID = SC.Id inner
                //        join RouteConfiguration RC on RC.ProductID = PC.Id  inner
                //        join ProviderConfiguration PrC on Prc.Id = RC.SerProID
                //        where SC.SMSCode = {0} and RC.TrnType ={1}   and {2}
                //        between RC.MinimumAmount and RC.MaximumAmount and {3}
                //        between SC.MinimumAmount and SC.MaximumAmount
                //        and SC.Status = 1 and RC.Status = 1 and Prc.Status = 1
                //        order by RC.Priority ", Request.SMSCode,Request.trnType, Request.amount,Request.amount);

                #endregion

                var list = new List<TransactionProviderResponse>(Result);
                return list;
                //return Result.ToList();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "MethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        //khushali Fetch Liquidity Provider
        public async Task<List<TransactionProviderResponseV1>> GetProviderDataListV2Async(TransactionApiConfigurationRequest Request)
        {
            try
            {

                #region New Route configuratin by khushali
                IQueryable<TransactionProviderResponseV1> Result = _dbContext.TransactionProviderResponseV1.FromSql(
                            //@"select SM.ID as ServiceID,SM.ProviderName as ServiceName,SD.ID as SerProDetailID,SD.ServiceProID,RC.ID as RouteID,
                            // RC.RouteName,RC.OpCode,TP.APIBalURL,TP.APISendURL,TP.APIValidateURL,TP.ContentType,
                            // TP.MethodType,TP.ParsingDataID,SD.ThirPartyAPIID,SD.AppTypeID,LC.MinAmt as MinimumAmountItem,
                            // LC.MaxAmt as MaximumAmountItem,SD.ProTypeID ,RC.ProductID ,'' as ProviderWalletID ,0 as ServiceType FROM RouteConfiguration RC 
                            // INNER JOIN   ServiceProviderDetail SD ON  SD.Id = RC.SerProDetailID  
                            // INNER JOIN  ServiceProviderMaster SM ON SM.Id = SD.ServiceProID 
                            // INNER JOIN ThirdPartyAPIConfiguration TP ON TP.id = SD.ThirPartyAPIID
                            // INNER JOIN TradePairMaster TM ON TM.id = RC.PairId
                            // INNER JOIN Limits LC ON LC.ID = RC.LimitID 
                            // WHERE {2} between LC.MinAmt and LC.MaxAmt AND SD.TrnTypeID = {1} AND RC.TrnType = {1}  AND  RC.OrderType = {0}  AND 
                            // SD.Status = 1 AND RC.Status = 1 AND TM.Status=1", Request.OrderType, Request.trnType, Request.amount);
                            //Rita 4-4-18 added ,RC.AccNoStartsWith,RC.AccNoValidationRegex,RC.AccountNoLen as added in class by rushbh bhai
                            @"select ISNULL(SP.APIKey,'') as APIKey, ISNULL(SP.SecretKey,'') as SecretKey,RC.IsAdminApprovalRequired , SM.ID as ServiceID,SM.ProviderName as ServiceName,SD.ID as SerProDetailID,SD.ServiceProID,RC.ID as RouteID,
                            RC.RouteName,RC.OpCode,'' as APIBalURL,'' as  APISendURL,'' as APIValidateURL,'' as ContentType,
                            '' as MethodType,0 as ParsingDataID,SD.ThirPartyAPIID,SD.AppTypeID,ISNULL(LC.MinAmt,0) as MinimumAmountItem,
                            ISNULL(LC.MaxAmt,0) as MaximumAmountItem,SD.ProTypeID ,RC.ProductID ,'' as ProviderWalletID ,0 as ServiceType
                            FROM RouteConfiguration RC 
                            INNER JOIN  ServiceProviderDetail SD ON  SD.Id = RC.SerProDetailID  
                            INNER JOIN  ServiceProviderMaster SM ON SM.Id = SD.ServiceProID 
                            INNER JOIN TradePairMaster TM ON TM.id = RC.PairId
                            LEFT JOIN Limits LC ON LC.ID = RC.LimitID
                            LEFT JOIN ServiceProConfiguration SP ON SP.ID = SD.ServiceProConfigID
                            WHERE ({2} between LC.MinAmt and LC.MaxAmt OR RC.LimitID=0) AND SD.TrnTypeID = {1} AND RC.TrnType = {1}  AND  RC.OrderType = {0}  AND 
                            SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1 AND RC.PairId = {3} order by RC.priority", Request.OrderType, Request.trnType, Request.amount, Request.PairID);
                //var list = Result.Cast<TransactionProviderResponse>().ToList();
                var list = new List<TransactionProviderResponseV1>(Result);
                return list;
                #endregion
                //return Result.ToList();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "MethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public async Task<List<TransactionProviderResponse>> GetProviderDataListArbitrageV2Async(TransactionApiConfigurationRequest Request)
        {
            try
            {

                #region New Route configuratin by khushali
                IQueryable<TransactionProviderResponse> Result = _dbContext.TransactionProviderResponse.FromSql(
                            @"select RC.IsAdminApprovalRequired , SM.ID as ServiceID,SM.ProviderName as ServiceName,SD.ID as SerProDetailID,SD.ServiceProID,RC.ID as RouteID,
                            RC.RouteName,RC.OpCode,'' as APIBalURL,'' as  APISendURL,'' as APIValidateURL,'' as ContentType,
                            '' as MethodType,0 as ParsingDataID,SD.ThirPartyAPIID,SD.AppTypeID,ISNULL(LC.MinAmt,0) as MinimumAmountItem,
                            ISNULL(LC.MaxAmt,0) as MaximumAmountItem,SD.ProTypeID ,RC.ProductID ,'' as ProviderWalletID ,0 as ServiceType
                            FROM RouteConfiguration RC 
                            INNER JOIN  ServiceProviderDetail SD ON  SD.Id = RC.SerProDetailID  
                            INNER JOIN  ServiceProviderMaster SM ON SM.Id = SD.ServiceProID 
                            INNER JOIN TradePairMaster TM ON TM.id = RC.PairId
                            LEFT JOIN Limits LC ON LC.ID = RC.LimitID 
                            WHERE ({2} between LC.MinAmt and LC.MaxAmt OR RC.LimitID=0) AND SD.TrnTypeID = {1} AND RC.TrnType = {1}  AND  RC.OrderType = {0}  AND 
                            SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND RC.PairId = {3} AND SD.AppTypeID = {4}  order by RC.priority", Request.OrderType, Request.trnType, Request.amount, Request.PairID, Request.LPType);
                //var list = Result.Cast<TransactionProviderResponse>().ToList();
                var list = new List<TransactionProviderResponse>(Result);
                return list;
                #endregion
                //return Result.ToList();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "MethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }


        //Rita 5-6-19 for Arbitrage Fetch Liquidity Provider
        public async Task<List<TransactionProviderArbitrageResponse>> GetProviderDataListArbitrageAsync(TransactionApiConfigurationRequest Request)
        {
            List<TransactionProviderArbitrageResponse> list;
            try
            {
                //Rita 23-7-19 taken from cache
                list = _cache.Get<List<TransactionProviderArbitrageResponse>>("RouteDataArbitrage");
                if (list == null)
                {
                    //C.LPType,C.LTP  , INNER JOIN cryptowatcherarbitrage C ON C.LPType = SD.AppTypeID and C.PairID=TM.ID
                    IQueryable<TransactionProviderArbitrageResponse> Result = _dbContext.TransactionProviderArbitrageResponse.FromSql(
                          @"select ISNULL(SP.Param2,'0') as IsStoplossOrder,ISNULL(MinAmt,0) as MinNotional,ISNULL(MaxAmt,0) as MaxNotional,
                            ISNULL(MinPrice,0) as MinPrice,ISNULL(MaxPrice,0) as MaxPrice,
                            ISNULL(MinQty,0) as MinQty,ISNULL(MaxQty,0) as MaxQty,TM.PairName,
                            ISNULL(SP.APIKey,'') as APIKey, ISNULL(SP.SecretKey,'') as SecretKey,SD.AppTypeID As LPType,RC.ID as RouteID,rc.ordertype,RC.RouteName,SM.ID as ProviderID,SM.ProviderName,
                            SD.ID as SerProDetailID,SD.TrnTypeID,RC.TrnType,RC.PairId,SD.ProTypeID,SD.ThirPartyAPIID as ThirdPartyAPIID,ISNULL(SP.Param1,'') as LPProviderName  
                            FROM RouteConfigurationArbitrage RC 
                            INNER JOIN  ServiceProviderDetailArbitrage SD ON  SD.Id = RC.SerProDetailID  
                            INNER JOIN  ServiceProviderMasterArbitrage SM ON SM.Id = SD.ServiceProID 
                            INNER JOIN TradePairMasterArbitrage TM ON TM.id = RC.PairId                            
                            LEFT JOIN ServiceProConfigurationarbitrage SP ON SP.ID = SD.ServiceProConfigID
                            Left JOIN LimitsArbitrage LM ON LM.id = RC.limitid 
                            WHERE SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1");
                    list = new List<TransactionProviderArbitrageResponse>(Result.ToList());
                    _cache.Set("RouteDataArbitrage", list);
                }
                else if (list.Count == 0)
                {
                    //C.LPType,C.LTP  , INNER JOIN cryptowatcherarbitrage C ON C.LPType = SD.AppTypeID and C.PairID=TM.ID
                    IQueryable<TransactionProviderArbitrageResponse> Result = _dbContext.TransactionProviderArbitrageResponse.FromSql(
                          @"select ISNULL(SP.Param2,'0') as IsStoplossOrder,ISNULL(MinAmt,0) as MinNotional,ISNULL(MaxAmt,0) as MaxNotional,
                            ISNULL(MinPrice,0) as MinPrice,ISNULL(MaxPrice,0) as MaxPrice,
                            ISNULL(MinQty,0) as MinQty,ISNULL(MaxQty,0) as MaxQty,TM.PairName,
                            ISNULL(SP.APIKey,'') as APIKey, ISNULL(SP.SecretKey,'') as SecretKey,SD.AppTypeID As LPType,RC.ID as RouteID,rc.ordertype,RC.RouteName,SM.ID as ProviderID,SM.ProviderName,
                            SD.ID as SerProDetailID,SD.TrnTypeID,RC.TrnType,RC.PairId,SD.ProTypeID,SD.ThirPartyAPIID as ThirdPartyAPIID,ISNULL(SP.Param1,'') as LPProviderName  
                            FROM RouteConfigurationArbitrage RC 
                            INNER JOIN  ServiceProviderDetailArbitrage SD ON  SD.Id = RC.SerProDetailID  
                            INNER JOIN  ServiceProviderMasterArbitrage SM ON SM.Id = SD.ServiceProID 
                            INNER JOIN TradePairMasterArbitrage TM ON TM.id = RC.PairId                            
                            LEFT JOIN ServiceProConfigurationarbitrage SP ON SP.ID = SD.ServiceProConfigID
                            Left JOIN LimitsArbitrage LM ON LM.id = RC.limitid 
                            WHERE SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1 ");
                    list = new List<TransactionProviderArbitrageResponse>(Result.ToList());
                    _cache.Set("RouteDataArbitrage", list);
                }

                list = list.Where(e => e.PairId == Request.PairID && e.TrnTypeID == Request.trnType && e.TrnType == Request.trnType
                                    && e.LPType == Request.LPType).ToList();
                return list;

                //Rita 23-7-19 c.Pair=TM.PairName to c.PairID=TM.ID
                //             IQueryable<TransactionProviderArbitrageResponse> Result = _dbContext.TransactionProviderArbitrageResponse.FromSql(
                //                     @"select ISNULL(MinAmt,0) as MinNotional,ISNULL(MaxAmt,0) as MaxNotional,
                //                     ISNULL(MinPrice,0) as MinPrice,ISNULL(MaxPrice,0) as MaxPrice,
                //                     ISNULL(MinQty,0) as MinQty,ISNULL(MaxQty,0) as MaxQty,
                //                     ISNULL(SP.APIKey,'') as APIKey, ISNULL(SP.SecretKey,'') as SecretKey, C.LPType,RC.ID as RouteID,rc.ordertype,RC.RouteName,SM.ID as ProviderID,SM.ProviderName,
                //                     SD.ID as SerProDetailID,SD.TrnTypeID as TrnType,C.LTP,SD.ProTypeID,SD.ThirPartyAPIID as ThirdPartyAPIID  
                //                     FROM RouteConfigurationArbitrage RC 
                //                     INNER JOIN  ServiceProviderDetailArbitrage SD ON  SD.Id = RC.SerProDetailID  
                //                     INNER JOIN  ServiceProviderMasterArbitrage SM ON SM.Id = SD.ServiceProID 
                //                     INNER JOIN TradePairMasterArbitrage TM ON TM.id = RC.PairId
                //                     INNER JOIN cryptowatcherarbitrage C ON C.LPType = SD.AppTypeID and C.PairID=TM.ID
                //                     LEFT JOIN ServiceProConfigurationarbitrage SP ON SP.ID = SD.ServiceProConfigID
                //                     Left JOIN LimitsArbitrage LM ON LM.id = RC.limitid 
                //                     WHERE SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1 AND RC.PairId = {0} 
                //AND SD.TrnTypeID = {1} AND RC.TrnType = {1} AND C.LPType = {2}", Request.PairID, Request.trnType, Request.LPType);
                //             list = new List<TransactionProviderArbitrageResponse>(Result);

            }
            catch (Exception ex)
            {
                //_log.LogError(ex, "MethodName: GetProviderDataListArbitrageAsync \nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog("GetProviderDataListArbitrageAsync", "WebApiDataRepository", ex);
                //throw ex;
                return null;
            }
        }

        //Rushabh 13-01-2020 Added New Method For Node API Call Process Related To New Coin Configuration
        public async Task<List<TransactionProviderResponse>> GetProviderDataListAsyncForCoinConfig(TransactionApiConfigurationRequest Request,long SerProId)
        {
            try
            {
                IQueryable<TransactionProviderResponse> Result = _dbContext.TransactionProviderResponse.FromSql(
                           @"SELECT CAST(0 AS BIGINT) AS ServiceID,'' AS ServiceName,Prc.ID AS SerProDetailID,Prc.ServiceProID,RC.ID AS RouteID,CAST(0 AS BIGINT) AS ProductID,RC.RouteName,
                             0 AS ServiceType,Prc.ThirPartyAPIID,Prc.AppTypeID,LC.MinAmt AS MinimumAmountItem,LC.MaxAmt AS MaximumAmountItem,RC.ProviderWalletID,
                             RC.IsAdminApprovalRequired,RC.OpCode,CAST(1 AS BIGINT) AS ProTypeID ,'' AS APIBalURL,'' AS APISendURL,'' AS APIValidateURL,'' AS ContentType,
                             '' AS MethodType,0 AS ParsingDataID,RC.AccNoStartsWith,RC.AccNoValidationRegex,RC.AccountNoLen 
                             FROM ServiceProviderDetail PrC 
                             INNER JOIN RouteConfiguration RC ON Prc.Id = RC.SerProDetailID AND Prc.TrnTypeID={0} 
                             INNER JOIN Limits LC ON LC.ID = RC.LimitID WHERE RC.TrnType={0} AND {1} BETWEEN LC.MinAmt AND LC.MaxAmt AND RC.Status = 1 and Prc.Status=1 
                             AND Prc.ServiceProID = {2} ORDER BY RC.Priority", Request.trnType, Request.amount,SerProId);
                var list = new List<TransactionProviderResponse>(Result);
                return list;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "MethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public async Task<List<TransactionProviderResponse>> GetProviderDataListAsync(TransactionApiConfigurationRequest Request)
        {
            try
            {
                #region old code
                //and {2} between RC.MinimumAmount and RC.MaximumAmount
                //and {2}  between SC.MinimumAmount and SC.MaximumAmount
                //IQueryable<TransactionProviderResponse> Result = _dbContext.TransactionProviderResponse.FromSql(
                //            @"select SC.ID as ServiceID,SC.ServiceName,Prc.ID as SerProDetailID,Prc.ServiceProID,RC.ID as RouteID,
                //            PC.ID as ProductID,RC.RouteName,SC.ServiceType,Prc.ThirPartyAPIID,Prc.AppTypeID,RC.MinimumAmount as MinimumAmountItem,
                //            RC.MaximumAmount as MaximumAmountItem,SC.MinimumAmount as MinimumAmountService,SC.MaximumAmount as MaximumAmountService
                //            from ServiceConfiguration SC inner join  ProductConfiguration PC on
                //   PC.ServiceID = SC.Id inner join RouteConfiguration RC on RC.ProductID = PC.Id  
                //   inner join ServiceProviderDetail PrC on Prc.ServiceProID = RC.SerProID AND Prc.TrnTypeID={1} 
                //   where SC.SMSCode = {0} and RC.TrnType={1} 
                //   and {2} between RC.MinimumAmount and RC.MaximumAmount
                //   and {3} between SC.MinimumAmount and SC.MaximumAmount
                //   and SC.Status = 1 and RC.Status = 1 and Prc.Status=1 
                //   order by RC.Priority", Request.SMSCode, Request.trnType, Request.amount,Request.amount);

                // ntrivedi limit changes done 12-10-2018
                //Rita 13-10-2018  remove as no present in New table Service master
                //,SC.MinimumAmount as MinimumAmountService,SC.MaximumAmount as MaximumAmountService
                //and {3} between SC.MinimumAmount and SC.MaximumAmount
                //Rita 17-10-18 further make inner joi in RoutMaster with SerProDetailID with providerdetail
                #endregion

                #region New Route configuration by khushali
                //var OrderType = 2;
                //IQueryable<TransactionProviderResponse> Result = _dbContext.TransactionProviderResponse.FromSql(
                //           @"select SM.ID as ServiceID,SM.ProviderName as ServiceName,SD.ID as SerProDetailID,SD.ServiceProID,RC.ID as RouteID,
                //            RC.RouteName,RC.OpCode,TP.APIBalURL,TP.APISendURL,TP.APIValidateURL,TP.ContentType,
                //            TP.MethodType,TP.ParsingDataID,SD.ThirPartyAPIID,SD.AppTypeID,LC.MinAmt as MinimumAmountItem,
                //            LC.MaxAmt as MaximumAmountItem,SD.ProTypeID FROM RouteConfiguration RC 
                //            INNER JOIN   ServiceProviderDetail SD ON  SD.Id = RC.SerProDetailID  
                //            INNER JOIN  ServiceProviderMaster SM ON SM.Id = SD.ServiceProID 
                //            INNER JOIN ThirdPartyAPIConfiguration TP ON TP.id = SD.ThirPartyAPIID
                //            INNER JOIN TradePairMaster TM ON TM.id = RC.PairId
                //            INNER JOIN Limits LC ON LC.ID = RC.LimitID 
                //            WHERE {2} between LC.MinAmt and LC.MaxAmt AND SD.TrnTypeID = {1} AND RC.TrnType = {1}  AND  RC.OrderType = {0}  AND 
                //            SD.Status = 1 AND RC.Status = 1 AND TM.Status=1  ORDER BY RC.Priority", OrderType, Request.trnType, Request.amount);

                //var list = new List<TransactionProviderResponse>(Result);
                //return list;
                #endregion

                IQueryable<TransactionProviderResponse> Result = _dbContext.TransactionProviderResponse.FromSql(
                           @"select SC.ID as ServiceID,SC.Name as ServiceName,Prc.ID as SerProDetailID,Prc.ServiceProID,RC.ID as RouteID,
                            PC.ID as ProductID,RC.RouteName,cast(SC.ServiceType as int) as ServiceType,Prc.ThirPartyAPIID,Prc.AppTypeID,LC.MinAmt as MinimumAmountItem,
                            LC.MaxAmt as MaximumAmountItem,RC.ProviderWalletID,RC.IsAdminApprovalRequired,RC.OpCode,cast(1  as bigint) as ProTypeID ,'' as APIBalURL,'' as APISendURL,'' as APIValidateURL,'' as ContentType,
                            '' as MethodType,0 as ParsingDataID,RC.AccNoStartsWith,RC.AccNoValidationRegex,RC.AccountNoLen
                            from ServiceMaster SC inner join  ProductConfiguration PC on
			                PC.ServiceID = SC.Id inner join RouteConfiguration RC on RC.ProductID = PC.Id  
			                inner join ServiceProviderDetail PrC on Prc.Id = RC.SerProDetailID AND Prc.TrnTypeID={1} 
							inner join Limits LC on LC.ID = RC.LimitID 
			                where SC.SMSCode = {0}  and RC.TrnType={1} 
			                and {2} between LC.MinAmt and LC.MaxAmt			                
			                and SC.Status = 1 and RC.Status = 1 and Prc.Status=1 
			                order by RC.Priority", Request.SMSCode, Request.trnType, Request.amount);
                //,RC.AccNoStartsWith,RC.AccNoValidationRegex,RC.AccountNoLen Added By Rushabh 23-03-2019 For Address Validation
                var list = new List<TransactionProviderResponse>(Result);
                return list;
                //return Result.ToList();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "MethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public async Task<List<TransactionProviderResponseForWithdraw>> GetProviderDataListAsyncForWithdraw(TransactionApiConfigurationRequest Request)
        {
            try
            {
                IQueryable<TransactionProviderResponseForWithdraw> Result = _dbContext.TransactionProviderResponseForWithdraw.FromSql(
                           @"select IsIntAmountAllow AS IsOnlyIntAmountAllow,SC.ID as ServiceID,SC.Name as ServiceName,Prc.ID as SerProDetailID,Prc.ServiceProID,RC.ID as RouteID,
                            PC.ID as ProductID,RC.RouteName,cast(SC.ServiceType as int) as ServiceType,Prc.ThirPartyAPIID,Prc.AppTypeID,LC.MinAmt as MinimumAmountItem,
                            LC.MaxAmt as MaximumAmountItem,RC.ProviderWalletID,RC.IsAdminApprovalRequired,RC.OpCode,cast(1  as bigint) as ProTypeID ,'' as APIBalURL,'' as APISendURL,'' as APIValidateURL,'' as ContentType,
                            '' as MethodType,0 as ParsingDataID,RC.AccNoStartsWith,RC.AccNoValidationRegex,RC.AccountNoLen
                            from ServiceMaster SC inner join  ProductConfiguration PC on
			                PC.ServiceID = SC.Id inner join RouteConfiguration RC on RC.ProductID = PC.Id  
			                inner join ServiceProviderDetail PrC on Prc.Id = RC.SerProDetailID AND Prc.TrnTypeID={1} 
							inner join Limits LC on LC.ID = RC.LimitID 
			                where SC.SMSCode = {0}  and RC.TrnType={1} 
			                and {2} between LC.MinAmt and LC.MaxAmt			                
			                and SC.Status = 1 and RC.Status = 1 and Prc.Status=1 
			                order by RC.Priority", Request.SMSCode, Request.trnType, Request.amount);
                var list = new List<TransactionProviderResponseForWithdraw>(Result);
                return list;
                //return Result.ToList();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "MethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public async Task<List<TransactionProviderResponse2>> GetProviderDataListForBalCheckAsyncV2(long SerProID, TransactionApiConfigurationRequest Request)
        {
            try
            {
                IQueryable<TransactionProviderResponse2> Result = _dbContext.TransactionProviderResponse2.FromSql(
                            @"select SC.ID as ServiceID,SC.Name as ServiceName,Prc.ID as SerProDetailID,Prc.ServiceProID,RC.ID as RouteID,RC.ConvertAmount,
                             PC.ID as ProductID,RC.RouteName,cast(SC.ServiceType as int) as ServiceType,Prc.ThirPartyAPIID,Prc.AppTypeID,LC.MinAmt as MinimumAmountItem,
                             LC.MaxAmt as MaximumAmountItem,RC.ProviderWalletID,RC.OpCode,cast(1  as bigint) as ProTypeID ,'' as APIBalURL,'' as APISendURL,'' as APIValidateURL,'' as ContentType,
                             '' as MethodType,0 as ParsingDataID,RC.AccNoStartsWith,RC.AccNoValidationRegex,RC.AccountNoLen,Am.Id as [AddressId],OriginalAddress as [Address],Am.GUID as RefKey 
                             from ServiceMaster SC inner join  ProductConfiguration PC on
			                 PC.ServiceID = SC.Id inner join RouteConfiguration RC on RC.ProductID = PC.Id  
			                 inner join ServiceProviderDetail PrC on Prc.Id = RC.SerProDetailID AND Prc.TrnTypeID IN (9,6)
							 inner join Limits LC on LC.ID = RC.LimitID 
                             left join AddressMasters AM on AM.SerProID = Prc.ServiceProID and AM.Status=1
                             left join WalletMasters wm on wm.id=am.WalletId  and wm.UserID = (select top 1 UserID from BizUserTypeMapping where UserType=0) and wm.isdefaultwallet=1 
                             left join WalletTypeMasters WTM on WTM.Id= wm.wallettypeid and SC.SMSCode=WTM.WalletTypeName 
			                 where Prc.ServiceProID = {0} AND (SC.SMSCode = {1} OR {1}='') and (RC.TrnType={2} OR {2}=0) 			                		                
			                 and SC.Status = 1 and RC.Status = 1 and Prc.Status=1 and (WTM.WalletTypeName={1} OR {1}='')                             
			                 order by RC.Priority", SerProID, Request.SMSCode, Request.trnType);
                var list = new List<TransactionProviderResponse2>(Result);
                return list;
                //return Result.ToList();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "MethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public async Task<List<TransactionProviderResponse3>> GetProviderDataListForBalCheckAsync(long SerProID, TransactionApiConfigurationRequest Request)
        {
            try
            {
                IQueryable<TransactionProviderResponse3> Result = _dbContext.TransactionProviderResponse3.FromSql(
                            @"select SC.ID as ServiceID,SC.Name as ServiceName,Prc.ID as SerProDetailID,Prc.ServiceProID,RC.ID as RouteID,RC.ConvertAmount,
                             PC.ID as ProductID,RC.RouteName,cast(SC.ServiceType as int) as ServiceType,Prc.ThirPartyAPIID,Prc.AppTypeID,LC.MinAmt as MinimumAmountItem,
                             LC.MaxAmt as MaximumAmountItem,RC.ProviderWalletID,RC.OpCode,cast(1  as bigint) as ProTypeID ,'' as APIBalURL,'' as APISendURL,'' as APIValidateURL,'' as ContentType,
                             '' as MethodType,0 as ParsingDataID,RC.AccNoStartsWith,RC.AccNoValidationRegex,RC.AccountNoLen 
                             from ServiceMaster SC inner join  ProductConfiguration PC on
			                 PC.ServiceID = SC.Id inner join RouteConfiguration RC on RC.ProductID = PC.Id  
			                 inner join ServiceProviderDetail PrC on Prc.Id = RC.SerProDetailID AND Prc.TrnTypeID IN (9,6)
							 inner join Limits LC on LC.ID = RC.LimitID                              
			                 where Prc.ServiceProID = {0} AND (SC.SMSCode = {1} OR {1}='') and (RC.TrnType={2} OR {2}=0) 			                		                
			                 and SC.Status = 1 and RC.Status = 1 and Prc.Status=1                              
			                 order by RC.Priority", SerProID, Request.SMSCode, Request.trnType);
                var list = new List<TransactionProviderResponse3>(Result);
                return list;
                //return Result.ToList();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "MethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        //public List<WalletServiceData> StatusCheck()
        //{
        //    try
        //    {               
        //        IQueryable<WalletServiceData> Result = _dbContext.WalletServiceData.FromSql(
        //                   @"SELECT SM.Id as ServiceID , SM.SMSCode,WM.Status AS WalletStatus , SM.Status AS ServiceStatus FROM WalletTypeMasters WM INNER JOIN ServiceMaster SM ON SM.WalletTypeID=WM.id WHERE WM.status = 1 and WM.IsDepositionAllow = 1");

        //        var list = new List<WalletServiceData>(Result);
        //        return list;                
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.LogError(ex, "MethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
        //        throw ex;
        //    }
        //}
        //Darshan Dholakiya added this changes for CCXTLpHoldTransaction 25-07-2019
        public List<CCXTTranNo> CCXTLpHoldTransaction(int Lptype, long PairID)
        {
            try
            {
                SqlParameter[] param1 = new SqlParameter[]{
                new SqlParameter("@LpType",SqlDbType.Int, 10, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, Lptype),
                new SqlParameter("@PairID",SqlDbType.BigInt, 10, ParameterDirection.Input, false, 28, 18, String.Empty, DataRowVersion.Default, PairID)};
                List<CCXTTranNo> Result = _dbContext.CCXTTranNos.FromSql("SP_CCXTGetHoldTransaction @LpType,@PairID", param1).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "MethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        #region For Regular LP

        public async Task<List<TransactionProviderArbitrageResponse>> GetProviderDataListRegularAsync(TransactionApiConfigurationRequest Request)
        {
            List<TransactionProviderArbitrageResponse> list;
            try
            {
                list = _cache.Get<List<TransactionProviderArbitrageResponse>>("RouteDataRegular");
                if (list == null)
                {                    
                    IQueryable<TransactionProviderArbitrageResponse> Result = _dbContext.TransactionProviderArbitrageResponse.FromSql(
                          @"select ISNULL(SP.Param2,'0') as IsStoplossOrder,ISNULL(MinAmt,0) as MinNotional,ISNULL(MaxAmt,0) as MaxNotional,
                            CAST(0 as decimal) as MinPrice,CAST(0 as decimal) as MaxPrice,CAST(0 as decimal) as MinQty,CAST(0 as decimal) as MaxQty,TM.PairName,
                            ISNULL(SP.APIKey,'') as APIKey, ISNULL(SP.SecretKey,'') as SecretKey,SD.AppTypeID As LPType,RC.ID as RouteID,rc.ordertype,RC.RouteName,SM.ID as ProviderID,SM.ProviderName,
                            SD.ID as SerProDetailID,SD.TrnTypeID,RC.TrnType,RC.PairId,SD.ProTypeID,SD.ThirPartyAPIID as ThirdPartyAPIID  
                            FROM RouteConfiguration RC 
                            INNER JOIN  ServiceProviderDetail SD ON  SD.Id = RC.SerProDetailID  
                            INNER JOIN  ServiceProviderMaster SM ON SM.Id = SD.ServiceProID 
                            INNER JOIN TradePairMaster TM ON TM.id = RC.PairId                            
                            LEFT JOIN ServiceProConfiguration SP ON SP.ID = SD.ServiceProConfigID
                            Left JOIN Limits LM ON LM.id = RC.limitid 
                            WHERE SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1");
                    list = new List<TransactionProviderArbitrageResponse>(Result.ToList());
                    _cache.Set("RouteDataRegular", list);
                }
                else if (list.Count == 0)
                {                    
                    IQueryable<TransactionProviderArbitrageResponse> Result = _dbContext.TransactionProviderArbitrageResponse.FromSql(
                          @"select ISNULL(MinAmt,0) as MinNotional,ISNULL(MaxAmt,0) as MaxNotional,
                            CAST(0 AS decimal) as MinPrice,CAST(0 AS decimal) as MaxPrice,
                            CAST(0 AS decimal) as MinQty,CAST(0 AS decimal) as MaxQty,TM.PairName,
                            ISNULL(SP.APIKey,'') as APIKey, ISNULL(SP.SecretKey,'') as SecretKey,SD.AppTypeID As LPType,RC.ID as RouteID,rc.ordertype,RC.RouteName,SM.ID as ProviderID,SM.ProviderName,
                            SD.ID as SerProDetailID,SD.TrnTypeID,RC.TrnType,RC.PairId,SD.ProTypeID,SD.ThirPartyAPIID as ThirdPartyAPIID  
                            FROM RouteConfiguration RC 
                            INNER JOIN  ServiceProviderDetail SD ON  SD.Id = RC.SerProDetailID  
                            INNER JOIN  ServiceProviderMaster SM ON SM.Id = SD.ServiceProID 
                            INNER JOIN TradePairMaster TM ON TM.id = RC.PairId                            
                            LEFT JOIN ServiceProConfiguration SP ON SP.ID = SD.ServiceProConfigID
                            Left JOIN Limits LM ON LM.id = RC.limitid 
                            WHERE SD.Status = 1 AND RC.Status = 1 AND TM.Status=1 AND SM.Status=1 ");
                    list = new List<TransactionProviderArbitrageResponse>(Result.ToList());
                    _cache.Set("RouteDataRegular", list);
                }

                list = list.Where(e => e.PairId == Request.PairID && e.TrnTypeID == Request.trnType && e.TrnType == Request.trnType
                                    && e.LPType == Request.LPType).ToList();
                return list;

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetProviderDataListRegularAsync", "WebApiDataRepository", ex);
                return null;
            }
        }

        public GetDataForParsingAPI RegularGetDataForParsingAPI(long ThirPartyAPIID)
        {
            var result = from TP in _dbContext.ThirdPartyAPIConfiguration
                         join Regex in _dbContext.ThirdPartyAPIResponseConfiguration on TP.ParsingDataID equals Regex.Id
                         where TP.Id == ThirPartyAPIID && TP.Status == 1
                         select new GetDataForParsingAPI
                         {
                             ResponseSuccess = TP.ResponseSuccess,
                             ResponseFailure = TP.ResponseFailure,
                             ResponseHold = TP.ResponseHold,
                             BalanceRegex = Regex.BalanceRegex,
                             StatusRegex = Regex.StatusRegex,
                             StatusMsgRegex = Regex.StatusMsgRegex,
                             ResponseCodeRegex = Regex.ResponseCodeRegex,
                             ErrorCodeRegex = Regex.ErrorCodeRegex,
                             TrnRefNoRegex = Regex.TrnRefNoRegex,
                             OprTrnRefNoRegex = Regex.OprTrnRefNoRegex,
                             Param1Regex = Regex.Param1Regex,
                             Param2Regex = Regex.Param2Regex,
                             Param3Regex = Regex.Param3Regex,
                             //Param4Regex = Regex.Param4Regex,
                             //Param5Regex = Regex.Param5Regex,
                             //Param6Regex = Regex.Param6Regex,
                             //Param7Regex = Regex.Param7Regex,
                         };
            return result.FirstOrDefault();
        }

        #endregion
    }
}
