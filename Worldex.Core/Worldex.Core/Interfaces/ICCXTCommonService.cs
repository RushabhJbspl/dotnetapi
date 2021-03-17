using Worldex.Core.ViewModels.CCXT;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces
{
    public interface ICCXTCommonService
    {
        List<CCXTTickerExchange> GetCCXTExchange();
        CCXTTickerQryObj InsertUpdateTickerData(CCXTTickerResObj TickerData);
    }
}
