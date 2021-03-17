using System;
using Dapper;
using MarketMaker.Application.Interfaces.Queries;
using MarketMaker.Application.ViewModels.Queries;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using LoggingNlog;

namespace MarketMaker.Infrastructure.Queries
{
    public sealed class MarketMakerQueries : IMarketMakerQueries
    {
        private readonly IConfiguration _configuration;
        private IDbConnection DbConnectionObj => new SqlConnection(_configuration.GetConnectionString("SqlServerConnectionString"));
        private readonly INLogger<MarketMakerQueries> _logger;


        public MarketMakerQueries(IConfiguration configuration, INLogger<MarketMakerQueries> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<short> GetMarketMakerStatusAsync()
        {
            using (IDbConnection dbConnection = DbConnectionObj)
            {
                dbConnection.Open();

                /*Query get status of MarketMaker is on/off -Sahil 26-09-2019
                   Where Id = 3: value is for MarketMaker 
                   Return: short value 1/0
                */
                string query = @"SELECT Status FROM TradingConfiguration WHERE Id = @id";

                var result = await dbConnection.ExecuteScalarAsync<short>(query, new { id = 3 });
                return result;
            }
        }

        public async Task<short> GetMarketMakerUserRoleStatusAsync(long userId)
        {
            using (IDbConnection dbConnection = DbConnectionObj)
            {
                dbConnection.Open();

                /* Check for MarkerMaker User is exist or not with his Role. -Sahil 26-09-2019
                   Where roleId = 29: value is for MarketMaker Role defined from BizRoles table 
                   Return: short value 1/0
                */
                string query = @"SELECT TOP(1) 1 FROM BizUserRole WHERE UserId = @userId AND RoleId = @roleId";

                var result = await dbConnection.ExecuteScalarAsync<short>(query, new { userId = userId, roleId = 29 });
                return result;
            }
        }

        public async Task<MarketMakerBuyPreferencesViewModel> GetMarketMakerUserBuyPreferencesAsync(long pairId)
        {
            using (IDbConnection dbConnection = DbConnectionObj)
            {
                /* Get list of User's Buy Exchange and trading preferences -Sahil 27-09-2019
                   Remarks: Query eliminates records whose buy LP id has not presence in ServiceProviderMaster table
                */
                string query = @"SELECT m.Id, m.UserId, m.PairId, t.PairName, m.BuyLTPPrefProID, m.BuyUpPercentage, m.BuyDownPercentage, m.BuyThreshold, m.BuyLTPRangeType , s.ProviderName
                                    FROM BizUserRole u INNER JOIN BizRoles r
									ON (u.RoleId = r.Id) 
									INNER JOIN MarketMakerPreferences m 
									ON (u.UserId = m.UserId) 
									INNER JOIN ServiceProviderMaster s 
                                    ON (m.BuyLTPPrefProID = s.Id)
                                    INNER JOIN TradePairMaster t
                                    ON (m.PairId = t.Id)
                                    WHERE m.PairId = @pairId AND r.NormalizedName = 'MARKETMAKER'
	                                      AND m.Status = 1 AND s.Status = 1 AND t.Status = 1 AND r.Status = 1";

                var result = await dbConnection.QueryAsync<MarketMakerBuyPreferencesViewModel>(query, new { pairId = pairId });
                return result.FirstOrDefault() ?? new MarketMakerBuyPreferencesViewModel(); //if result not found it will return empty view model class -Sahil 27-09-2019
            }
        }

        public async Task<MarketMakerSellPreferencesViewModel> GetMarketMakerUserSellPreferencesAsync(long pairId)
        {
            using (IDbConnection dbConnection = DbConnectionObj)
            {
                /* Get list of User's Sell Exchange and trading preferences -Sahil 27-09-2019
                   Remarks: Query eliminates records whose sell LP id has not presence in ServiceProviderMaster table
                */
                string query = @"SELECT m.Id, m.UserId, m.PairId, t.PairName, m.SellLTPPrefProID, m.SellUpPercentage, m.SellDownPercentage, m.SellThreshold, m.SellLTPRangeType , s.ProviderName
                                    FROM BizUserRole u INNER JOIN BizRoles r
									ON (u.RoleId = r.Id) 
									INNER JOIN MarketMakerPreferences m 
									ON (u.UserId = m.UserId) 
									INNER JOIN ServiceProviderMaster s 
                                    ON (m.SellLTPPrefProID = s.Id)
                                    INNER JOIN TradePairMaster t
                                    ON (m.PairId = t.Id)
                                    WHERE m.PairId = @pairId AND r.NormalizedName = 'MARKETMAKER'
	                                      AND m.Status = 1 AND s.Status = 1 AND t.Status = 1 AND r.Status = 1";

                var result = await dbConnection.QueryAsync<MarketMakerSellPreferencesViewModel>(query, new { pairId = pairId });
                return result.FirstOrDefault() ?? new MarketMakerSellPreferencesViewModel(); //if result not found it will return empty view model class -Sahil 27-09-2019
            }
        }

        public async Task<List<MarketMakerUserFixRangeDetail>> GetMarketMakerFixRangeDetailsAsync(long preferenceId)
        {
            using (IDbConnection dbConnection = DbConnectionObj)
            {

                string query = @"SELECT* FROM MarketMakerRangeDetails WHERE PreferenceId = @id";

                var result =
                    await dbConnection.QueryAsync<MarketMakerUserFixRangeDetail>(query, new { id = preferenceId });

                return result.AsList();
            }
        }

        public async Task<decimal> GetMarketMakerHoldOrderRateChange(long pairId)
        {
            using (IDbConnection dbConnection = DbConnectionObj)
            {

                string query = @"SELECT Top(1) HoldOrderRateChange
	                            FROM BizUserRole u INNER JOIN BizRoles r
	                            ON (u.RoleId = r.Id) 
	                            INNER JOIN MarketMakerPreferences m 
	                            ON (u.UserId = m.UserId) 
	                            WHERE m.PairId = @id AND r.NormalizedName = 'MARKETMAKER'
			                            AND m.Status = 1 AND r.Status = 1";

                var result =
                    await dbConnection.ExecuteScalarAsync<decimal>(query, new { id = pairId });

                return result;
            }
        }

        public async Task<MarketMakerTradeCountViewModel> GetMarketMakerTradeCount(string pairName, string orderType)
        {
            try
            {
                using (IDbConnection dbConnection = DbConnectionObj)
                {
                    //string query = @"SELECT TOP(1) t.PairID , COUNT(*) as TradeCount FROM 
                    //         BizUserRole u INNER JOIN BizRoles r 
                    //         ON (u.RoleId = r.Id)
                    //         INNER JOIN TradeTransactionQueue t
                    //         ON (r.id = t.MemberId)
                    //         WHERE r.NormalizedName = 'MARKETMAKER' AND PairName = @pairName AND TrnTypeName = @orderType
                    //           AND t.Status = 4 AND r.Status = 1 
                    //         Group by PairID";

                    //komal 15-11-2019 create new query due to not get proper hold record
                    string query = @"SELECT ISNULL((SELECT top 1 id FROM TradePairMaster WHERE PairName = @pairName and status=1),0)AS PairID,
                        ISNULL((SELECT TOP(1) COUNT(*) as TradeCount FROM BizRoles BR  INNER JOIN BizUserRole BUR ON BR.id=BUR.RoleId
                        INNER JOIN TradeTransactionQueue TTQ ON BUR.userid=TTQ.MemberID WHERE BR.NormalizedName = 'MARKETMAKER' AND BR.Status=1 
                        AND TTQ.status=4  AND PairName = @pairName AND TrnTypeName = @orderType GROUP BY PairID),0) AS TradeCount";


                    var result = await dbConnection.QueryAsync<MarketMakerTradeCountViewModel>(query, new { pairName = pairName, orderType = orderType });
                    return result.FirstOrDefault() ?? new MarketMakerTradeCountViewModel();
                }
            }
            catch (Exception ex)
            {
                _logger.WriteErrorLog("GetMarketMakerTradeCount", ex);
                return null;
            }
        }

        public async Task GetFiatCoinPairList(string pairName, decimal ltp)
        {
            try
            {
                //_logger.WriteInfoLog("GetFiatCoinPairList", " PairName {"+ pairName + "}, LTP {"+ltp+"} ");
                using (IDbConnection dbConnection = DbConnectionObj)
                {
                    string query = @"SELECT  DISTINCT WTMF.WalletTypeName+'_'+WTMB.WalletTypeName AS PairName,cast(FCC.FromCurrencyId as varchar(10))+'000000'+FCC.ToCurrencyID as PairID 
                                FROM FiatCoinConfiguration FCC 
                                INNER JOIN Wallettypemasters WTMF ON WTMF.id = FCC.FromCurrencyId 
	                            INNER JOIN Wallettypemasters WTMB ON WTMB.id = FCC.ToCurrencyID 
	                            WHERE WTMF.WalletTypeName+'_'+WTMB.WalletTypeName = @pairName";
	                            //WHERE WTMF.WalletTypeName+'_'+WTMB.WalletTypeName = @pairName AND FCC.status = 1";

                    var result = await dbConnection.QueryAsync<FiateCoinPairListViewModel>(query, new { pairName = pairName});
                    var pairDetail= result.FirstOrDefault();

                    if(pairDetail!=null)
                    {
                        var WalletTypeID = await dbConnection.ExecuteScalarAsync<long>("select WalletTypeId from CurrencyRateMaster where WalletTypeId=@WalletTypeId", new { WalletTypeId=Convert.ToInt64(pairDetail.PairID) });
                        if(WalletTypeID==0)
                        {
                            _logger.WriteInfoLog("GetFiatCoinPairList", " Insert PairName {" + pairName + "}, LTP {" + ltp + "} ");
                            string insertQuery = @"INSERT INTO CurrencyRateMaster 
	                                    ([Status], [CreatedBy], [UpdatedDate], [CreatedDate], [UpdatedBy], [WalletTypeId],[CurrencyName],[CurrentRate]) 
	                                    VALUES (@status, @createdBy, @updatedDate, @createdDate, @updatedBy, @walletTypeId, @currencyName,@currentRate)";

                            var debugResult = await dbConnection.ExecuteAsync(insertQuery,
                                                                        new
                                                                        {
                                                                            status = 1,
                                                                            createdBy = 1,
                                                                            updatedDate = DateTime.UtcNow,
                                                                            createdDate = DateTime.UtcNow,
                                                                            updatedBy = 1,
                                                                            walletTypeId = pairDetail.PairID,
                                                                            currencyName = pairName,
                                                                            currentRate = ltp
                                                                        });
                        }
                        else
                        {
                            _logger.WriteInfoLog("GetFiatCoinPairList", " Update PairName {" + pairName + "}, LTP {" + ltp + "} ");
                            string updateQuery = @"UPDATE CurrencyRateMaster 
	                                SET CurrentRate = @ltp, UpdatedDate = @updatedDate 
	                                WHERE WalletTypeId=@WalletTypeId";
                            var debugResult = await dbConnection.ExecuteAsync(updateQuery,
                                                            new
                                                            {
                                                                WalletTypeId = WalletTypeID,
                                                                updatedDate = DateTime.UtcNow,
                                                                ltp = ltp
                                                            });
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.WriteErrorLog("GetFiatCoinPairList", ex);
            }
        }

        public async Task UpdateFiatCoinPrice(string pairName, decimal ltp)
        {
            try
            {
                using (IDbConnection dbConnection = DbConnectionObj)
                {
                    string updateQuery = @"UPDATE CurrencyRateMaster 
	                                SET CurrentRate = @ltp, UpdatedDate = @updatedDate 
	                                WHERE CurrencyName = @pairName";
                    var debugResult = await dbConnection.ExecuteAsync(updateQuery,
                                                    new
                                                    {
                                                        pairName = pairName,
                                                        updatedDate = DateTime.UtcNow,
                                                        ltp = ltp
                                                    });
                }
            }
            catch (Exception ex)
            {
                _logger.WriteErrorLog("UpdateFiatCoinPrice", ex);
            }
        }

        public async Task InsertFiatCoinPair(string pairName)
        {
            try
            {
                using (IDbConnection dbConnection = DbConnectionObj)
                {
                    string insertQuery = @"INSERT INTO CurrencyRateMaster 
	                                    ([Status], [CreatedBy], [UpdatedDate], [CreatedDate], [UpdatedBy], [WalletTypeId],[CurrencyName],[CurrentRate]) 
	                                    VALUES (@status, @createdBy, @updatedDate, @createdDate, @updatedBy, @walletTypeId, @currencyName,@currentRate)";

                    var debugResult = await dbConnection.ExecuteAsync(insertQuery,
                                                                new
                                                                {
                                                                    status = 1,
                                                                    createdBy = 1,
                                                                    updatedDate = DateTime.UtcNow,
                                                                    createdDate = DateTime.UtcNow,
                                                                    updatedBy = 1,
                                                                    walletTypeId = 0,
                                                                    currencyName = pairName,
                                                                    currentRate=0
                                                                });
                }
            }
            catch(Exception ex)
            {
                _logger.WriteErrorLog("InsertFiatCoinPair", ex);
            }
            
        }

        public async Task<MarketMakerConfigurationViewModel> GetMarketMakerMssetrConfiguration(long PairID)
        {
            using (IDbConnection dbConnection = DbConnectionObj)
            {
                var result = await dbConnection.QueryAsync<MarketMakerConfigurationViewModel>(
                    @"SELECT TOP 1 NoOfBuyOrder,NoOfSellOrder,Depth,AvgQty,OrderPerCall FROM  MarketMakerPreferences MMP 
                    INNER JOIN MarketMakerMasterConfiguration C ON MMP.id=C.MarketMakerPreferenceID
                    INNER JOIN TradePairMaster TPM ON MMP.PairId=TPM.Id
                    WHERE PairID =@PairID AND MMP.status=1 AND C.status=1", new { PairID = PairID });
                return result.FirstOrDefault() ?? new MarketMakerConfigurationViewModel();
            }
        }
        public async Task<PairDetailDataViewModel> GetPairDetailData(long PairID)
        {
            using (IDbConnection dbConnection = DbConnectionObj)
            {
                var result = await dbConnection.QueryAsync<PairDetailDataViewModel>("select AmtLength,PriceLength,QtyLength from TradePAirDetail where PairID=@PairID", new { PairID = PairID });
                return result.FirstOrDefault() ?? new PairDetailDataViewModel();
            }
        }
    }
}
