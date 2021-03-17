using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Worldex.Core.ViewModels.AccountViewModels.Log;

namespace Worldex.Core.Interfaces.Log
{
    public interface IHistoryService
    {
        Task<long> AddHistory(HistoryViewModel model);
    }
}
