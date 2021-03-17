using Worldex.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces
{
    public interface ICommonRepository<T> where T : BizBase
    {
        T GetById(long id);
        Task<T> GetByIdAsync(long id);
        T GetActiveById(long id);
        List<T> List();
        List<T> GetAllList();
        T Add(T entity);
        Task<T> AddAsync(T entity);
        void Update(T entity);
        Task UpdateAsync(T entity);
        void UpdateField(T entity, params Expression<Func<T, object>>[] properties);
        Task UpdateFieldAsync(T entity, params Expression<Func<T, object>>[] properties);// where T : class;
        void Delete(T entity);
        T AddProduct(T entity);
        IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> AllIncludingAsync(params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        T GetSingle(int id);
        T GetSingle(Expression<Func<T, bool>> predicate);
        T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        //Task<T> GetSingleAsync(int id); it was using ID column so it can be done through etbyidasync so added expression and predicate to next method
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);
        decimal GetSum(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> sumPredicate);
        Task<decimal> GetSumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> sumPredicate);
        void UpdateWithAuditLog(T entity);
        void ReloadEntity(T entity);//ntrivedi 08-12-2018
        T GetForceSingle(Expression<Func<T, bool>> predicate); //ntrivedi 30-05-2019
        Task<T> GetForceSingleAsync(Expression<Func<T, bool>> predicate); //ntrivedi 30-05-2019
    }
}
