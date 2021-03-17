using Worldex.Core.Entities;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Infrastructure.DTOClasses;

namespace Worldex.Infrastructure.Interfaces
{
    public interface IGetWebRequest
    {
        ThirdPartyAPIRequest MakeWebRequest(long routeID, long thirdpartyID, long serproDetailID, TransactionQueue TQ = null, WithdrawERCAdminAddress AdminAddress = null);
        ThirdPartyAPIRequest MakeWebRequestV2(string RefKey, string Address,long routeID, long thirdpartyID, long serproDetailID, TransactionQueue TQ = null, WithdrawERCAdminAddress AdminAddress = null, short IsValidateUrl = 0);
        ThirdPartyAPIRequest MakeWebRequestWallet(long routeID, long thirdpartyID, long serproDetailID, string FileName, string Coin);
        ThirdPartyAPIRequest MakeWebRequestColdWallet(long routeID, long thirdpartyID, long serproDetailID, string ReqBody, string Coin);
        //khushali 29-01-2019 for LP integration
        ServiceProConfiguration GetServiceProviderConfiguration(long serproDetailID);
        ThirdPartyAPIRequest MakeWebRequestERC20(long routeID, long thirdpartyID, long serproDetailID, string password, string sitename, string siteid);

        //Rita for Arbitrage LP trasaction
        ServiceProConfigurationArbitrage GetServiceProviderConfigurationArbitrage(long serproDetailID);

        ThirdPartyAPIRequestArbitrage ArbitrageMakeWebRequest(long routeID, long thirdpartyID, long serproDetailID, TransactionQueueArbitrage TQ = null, TradeTransactionQueueArbitrage TradeTQ = null, WithdrawERCAdminAddress AdminAddress = null, short IsValidateUrl = 0,string Token = "",string TrnNo="");
        ThirdPartyAPIRequestArbitrage RegularMakeWebRequest(long routeID, long thirdpartyID, long serproDetailID, TransactionQueueArbitrage TQ = null, TradeTransactionQueueArbitrage TradeTQ = null, WithdrawERCAdminAddress AdminAddress = null, short IsValidateUrl = 0,string Token = "",string TrnNo="");

    }
}
