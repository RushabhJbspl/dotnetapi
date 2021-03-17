using System.Collections.Generic;
using Worldex.Core.SharedKernel;

namespace Worldex.Core.Interfaces.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        T GetById(int id);
        List<T> List();
        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        T AddProduct(T entity); // for testing
    }
}