using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.CCXT;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Data
{
    public class CCXTCommonRepository : ICCXTCommonRepository
    {
        private readonly ILogger<CCXTCommonRepository> _logger;
        private readonly WorldexContext _dbContext;

        public CCXTCommonRepository(ILogger<CCXTCommonRepository> logger, WorldexContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public List<CCXTTickerExchange> GetCCXTExchange()
        {
            try
            {
                return _dbContext.GetCCXTExchange.FromSql(
                    @"select distinct TM.PairName as Pair , cast (AP.Id as smallint) as LPType,SM.ProviderName as ExchangeName,TM.Id AS PairID
                        FROM RouteConfigurationArbitrage RC
                        INNER JOIN ServiceProviderDetailArbitrage SD ON  SD.Id = RC.SerProDetailID
                        INNER JOIN  AppType AP ON AP.Id = SD.AppTypeID
                        INNER JOIN  ServiceProviderMasterArbitrage SM ON SM.Id = SD.ServiceProID
                        INNER JOIN TradePairMasterArbitrage TM ON TM.id = RC.PairId
                        WHERE RC.TrnType in (4, 5) and
                        SD.Status = 1 AND SM.Status = 1 AND RC.Status = 1 AND TM.Status = 1 and AP.Id in (9,10,13,12,18,20,21,22,23,25,26)").ToList();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CCXTCommonRepository", "GetCCXTExchange ", ex);
                return null;
            }
        }

        public CCXTTickerQryObj InsertUpdateTickerData(CCXTTickerResObj TickerData)
        {
            string Qry = "";
            try
            {
                if(TickerData.Volume==0)
                    Qry = @"update CryptoWatcherArbitrage set LTP = {0},ChangePer = {1}, UpDownBit=CASE WHEN LTP > {0} THEN 1  WHEN LTP < {0} THEN 0 ELSE UpDownBit END,UpdatedBy=2,UpdateDate=dbo.getistdate() where  PairId = {2} and LPType = {3} ";
                else
                    Qry = @"update CryptoWatcherArbitrage set LTP = {0},ChangePer = {1}, volume={4},UpDownBit=CASE WHEN LTP > {0} THEN 1  WHEN LTP < {0} THEN 0 ELSE UpDownBit END,UpdatedBy=2,UpdateDate=dbo.getistdate() where  PairId = {2} and LPType = {3} ";

                var Identity = _dbContext.Database.ExecuteSqlCommand(Qry, TickerData.LTP, TickerData.ChangePer, TickerData.PairId, TickerData.LPType,TickerData.Volume);

                if (Identity == 0)
                {
                    Qry = @"insert into CryptoWatcherArbitrage values ({0},{1},{2},{3},{4},{5},{6},{7},2,dbo.getistdate())";
                    var res = _dbContext.Database.ExecuteSqlCommand(Qry, TickerData.LTP, TickerData.Pair, TickerData.LPType, TickerData.ChangePer, TickerData.Fees, TickerData.PairId, TickerData.UpDownBit, TickerData.Volume);
                }
                Qry = "select LTP,Pair,LPType,PairId,ChangePer,Volume,UpDownBit,Fees from CryptoWatcherArbitrage where PairId={0} and LPType={1}";
                return _dbContext.CCXTTickerQryObj.FromSql(Qry,TickerData.PairId,TickerData.LPType).FirstOrDefault();
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
    }
}
