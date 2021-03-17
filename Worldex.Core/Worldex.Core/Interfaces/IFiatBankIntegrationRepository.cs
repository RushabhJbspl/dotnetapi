using System.Collections.Generic;
using Worldex.Core.ViewModels.FiatBankIntegration;

namespace Worldex.Core.Interfaces
{
    public interface IFiatBankIntegrationRepository
    {
        List<GetUserBankReq> ListUserBankDetail(short? status,short? RequestType, long UserId);        
        List<FiatTradeConfigurationRes> ListFiatTradeConfiguration(short? status);
        List<FiatCoinConfigurationRes> ListFiatConfiguration(long? FromCurrencyId, long? ToCurrencyId, short? Status, short? TransactionType);
    }
}
