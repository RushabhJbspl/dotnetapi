using System.Collections.Generic;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;

namespace Worldex.Core.Interfaces
{
    public interface IWebApiRepository
    {
        WebApiConfigurationResponse GetThirdPartyAPIData(long ThirPartyAPIID);

        GetDataForParsingAPI GetDataForParsingAPI(long ThirPartyAPIID);

        //ntrivedi fetch route
        List<TransactionProviderResponse> GetProviderDataList(TransactionApiConfigurationRequest Request);
        Task<List<TransactionProviderResponse>> GetProviderDataListAsync(TransactionApiConfigurationRequest Request);
        Task<List<TransactionProviderResponseForWithdraw>> GetProviderDataListAsyncForWithdraw(TransactionApiConfigurationRequest Request);
        Task<List<TransactionProviderResponse3>> GetProviderDataListForBalCheckAsync(long SerProId,TransactionApiConfigurationRequest Request);
        Task<List<TransactionProviderResponse2>> GetProviderDataListForBalCheckAsyncV2(long SerProID, TransactionApiConfigurationRequest Request);
        Task<List<TransactionProviderResponseV1>> GetProviderDataListV2Async(TransactionApiConfigurationRequest Request);
        Task<List<TransactionProviderResponse>> GetProviderDataListArbitrageV2Async(TransactionApiConfigurationRequest Request);
        Task<List<TransactionProviderArbitrageResponse>> GetProviderDataListArbitrageAsync(TransactionApiConfigurationRequest Request);//Rita 11-6-19
        Task<List<TransactionProviderArbitrageResponse>> GetProviderDataListRegularAsync(TransactionApiConfigurationRequest Request);//Rushabh 07-08-2019
        //Darshan Dholakiya added this method for CCXTLpHoldTransaction changes-25-07-2019
        List<CCXTTranNo> CCXTLpHoldTransaction(int LpType, long PairID);
        Task<List<TransactionProviderResponse>> GetProviderDataListAsyncForCoinConfig(TransactionApiConfigurationRequest Request, long SerProId);//Rushabh 13-01-2020


    }
}
