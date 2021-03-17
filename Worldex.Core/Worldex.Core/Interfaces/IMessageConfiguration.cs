using Worldex.Core.ApiModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces
{
    public interface IMessageConfiguration
    {
        Task<IQueryable> GetAPIConfigurationAsync(long ServiceTypeID, long CommServiceTypeID);

        Task<IEnumerable<CommunicationProviderList>> GetAPIConfigurationAsyncV1(long ServiceTypeID, long CommServiceTypeID);

        Task<IQueryable> GetTemplateConfigurationAsync(long ServiceTypeID, int TemplateID, long CommServiceID);

        IList<TemplateMasterData> GetTemplateConfigurationAsyncV1();

        void ReloadTEmplateMaster();
    }
}
