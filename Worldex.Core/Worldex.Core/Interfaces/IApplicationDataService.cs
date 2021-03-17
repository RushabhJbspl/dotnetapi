using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Worldex.Core.Interfaces
{
    public interface IApplicationDataService
    {
        Task<object> GetApplicationData(HttpContext context);
    }
}