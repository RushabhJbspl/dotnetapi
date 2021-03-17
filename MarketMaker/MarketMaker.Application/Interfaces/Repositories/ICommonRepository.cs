using MarketMaker.Domain.Entities.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MarketMaker.Application.Interfaces.Repositories
{
    public interface ICommonRepository<T> where T : Entity
    {
        T GetById(long id);
        Task<T> GetByIdAsync(long id);
        List<T> List();
        List<T> GetAllList();
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate);
        T Add(T entity);
        Task<T> AddAsync(T entity);
        void Update(T entity);
        Task UpdateAsync(T entity);
        void Delete(T entity);
        T GetSingle(Expression<Func<T, bool>> predicate);
        T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);
    }
}
