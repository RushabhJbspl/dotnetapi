using System.Collections.Generic;
using System.Threading.Tasks;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Interfaces
{
    public interface IMessageRepository<T> where T : BizBase
    {
        T GetById(long id);
        Task<List<T>> List();
        Task<T> Add(T entity);
        void Update(T entity);
    }
}