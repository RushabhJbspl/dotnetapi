using Worldex.Core.ViewModels.CCXT;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.Repository
{
    public interface ICCXTCommonRepository
    {
        List<CCXTTickerExchange> GetCCXTExchange();
        CCXTTickerQryObj InsertUpdateTickerData(CCXTTickerResObj TickerData);
    }
}
