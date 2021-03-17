using Worldex.Core.Entities.Backoffice.RoleManagement;
using Worldex.Core.Entities.GroupModuleManagement;
using Worldex.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces.RoleManagement
{
    public interface IAsyncRepository<T> where T : BizBase
    {
        Task<T> GetByIdAsync(long id);
        Task<List<T>> ListAllAsync();
        Task<List<T>> ListAsync(ISpecification<T> spec);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> CountAsync(ISpecification<T> spec);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);    
        Task<List<ModuleGroupAccess>> GetAll(long GroupID);
        IEnumerable<T> GetAllWithoutStatus();
        Task<List<ModuleGroupAccess>> GetAllByParentID(long GroupID, Guid ParentID);
        int HasChild(long GroupID, Guid ParentID);
        Task<List<ModuleGroupAccess>> GetAllByParentIDV1(long GroupID, Guid ParentID);
    }

    public interface IAsyncRepositoryV1<T> where T : UserAssignModule 
    {
        Task<T> GetByIdAsync(long id);
        Task<List<T>> ListAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }

    public interface IAsyncRepositoryV2<T> where T : UserAssignSubModule
    {
        Task<T> GetByIdAsync(long id);
        Task<List<T>> ListAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }

    public interface IAsyncRepositoryV3<T> where T : UserAssignFieldRights
    {
        Task<T> GetByIdAsync(long id);
        Task<List<T>> ListAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }

    public interface IAsyncRepositoryV4<T> where T : UserAssignToolRights
    {
        Task<T> GetByIdAsync(long id);
        Task<List<T>> ListAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }

    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }

        int Take { get; }
        int Skip { get; }
        bool isPagingEnabled { get; }
    }

    public interface IAsyncRepositoryV5<T> where T : ModuleGroupMaster
    {
        Task<T> GetByIdAsync(long id);
        Task<List<T>> ListAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);                
        Task<T> IsExist(Expression<Func<T, bool>> predicate);
    }

}
