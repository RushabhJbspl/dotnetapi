using Worldex.Core.Entities.Backoffice.RoleManagement;
using Worldex.Core.Entities.GroupModuleManagement;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Interfaces.RoleManagement;
using Worldex.Core.SharedKernel;
using Worldex.Core.ViewModels.BackOffice;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Data.RoleManagement
{
    public class EfRepository1<T> : IAsyncRepository<T> where T : BizBase
    {
        protected readonly WorldexContext _dbContext;

        public EfRepository1(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual T GetById(long id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public T GetSingleBySpec(ISpecification<T> spec)
        {
            return List(spec).FirstOrDefault();
        }

        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public IEnumerable<T> ListAll()
        {
            return _dbContext.Set<T>().AsEnumerable();
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public IEnumerable<T> List(ISpecification<T> spec)
        {
            return ApplySpecification(spec).AsEnumerable();
        }
        public async Task<List<T>> ListAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public int Count(ISpecification<T> spec)
        {
            return ApplySpecification(spec).Count();
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
        }
        public virtual IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Where(predicate).ToList();
        }

        public virtual async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<List<ModuleGroupAccess>> GetAll(long GroupID)
        {
            //return await _dbContext.ModuleGroupAccess.Where(o => o.GroupID == GroupID).ToListAsync();
            var result = from MGA in _dbContext.ModuleGroupAccess
                         join SMM in _dbContext.SubModuleMaster on MGA.SubModuleID equals SMM.Id
                         where MGA.GroupID == GroupID && SMM.ParentID == 0 && MGA.Status == 1 && SMM.Status == 1
                         select new ModuleGroupAccess
                         {
                             CrudTypes = MGA.CrudTypes,
                             GroupID = MGA.GroupID,
                             SubModuleID = MGA.SubModuleID,
                             Id = MGA.Id,
                             Status = MGA.Status,
                             UtilityTypes = MGA.UtilityTypes
                         };
            return await result.ToListAsync();
        }

        public async Task<List<ModuleGroupAccess>> GetAllByParentID(long GroupID, Guid ParentID)
        {
            //return await _dbContext.ModuleGroupAccess.Where(o => o.GroupID == GroupID).ToListAsync();
            var result = from MGA in _dbContext.ModuleGroupAccess
                         join SMM in _dbContext.SubModuleMaster on MGA.SubModuleID equals SMM.Id
                         where MGA.GroupID == GroupID && SMM.ParentGUID == ParentID && MGA.Status == 1 && SMM.Status == 1
                         select new ModuleGroupAccess
                         {
                             CrudTypes = MGA.CrudTypes,
                             GroupID = MGA.GroupID,
                             SubModuleID = MGA.SubModuleID,
                             Id = MGA.Id,
                             Status = MGA.Status,
                             UtilityTypes = MGA.UtilityTypes
                         };
            return await result.ToListAsync();
        }

        public async Task<List<ModuleGroupAccess>> GetAllByParentIDV1(long GroupID, Guid ParentID)
        {
            List<ModuleGroupAccess> _Res = new List<ModuleGroupAccess>();
            IQueryable<ModuleGroupAccessQryRes> Result;
            Result = _dbContext.ModuleGroupAccessQry.FromSql(
                @"SELECT MGA.CrudTypes,MGA.GroupID,MGA.SubModuleID,MGA.Id,MGA.Status,MGA.UtilityTypes FROM ModuleGroupAccess MGA
                INNER JOIN  SubModuleMaster SMM ON MGA.SubModuleID = SMM.Id WHERE MGA.GroupID ={0} AND SMM.ParentGUID = {1} AND SMM.Status = 1 AND MGA.Status = 1",GroupID,ParentID);
            var res = Result.ToList();
            return Result.Select(e => new ModuleGroupAccess()
            {
                CrudTypes = e.CrudTypes,
                GroupID = e.GroupID,
                SubModuleID = e.SubModuleID,
                Id = e.Id,
                Status = e.Status,
                UtilityTypes = e.UtilityTypes
            }).ToList();
            //   return _Res = Result.ToList();
        }
        public bool HasChildB(long GroupID, Guid ParentID)
        {
            var result = from MGA in _dbContext.ModuleGroupAccess
                         join SMM in _dbContext.SubModuleMaster on MGA.SubModuleID equals SMM.Id
                         where MGA.GroupID == GroupID && SMM.ParentGUID == ParentID && MGA.Status == 1 && SMM.Status == 1
                         select MGA;

            var count = result.Count();

            if (count > 0)
                return true;
            else
                return false;

            //return await result.ToListAsync();
        }

        public virtual IEnumerable<T> GetAllWithoutStatus()
        {
            return _dbContext.Set<T>().ToList();
        }

        public int HasChild(long GroupID, Guid ParentID)
        {
            string Query = "select count(SM.ID) " +
                    " from SubModuleMaster SM INNER JOIN ModuleGroupAccess MG ON MG.SubModuleID = SM.ID " +
                    "WHere MG.Status=1 AND SM.Status=1 AND SM.ParentGUID = {0} and GroupID =  {1} ";

            var data = _dbContext.MenuSubDetailViewModelV2.FromSql(Query, GroupID, ParentID).Count();

            return data;
        }
    }

    public class SpecificationEvaluator<T> where T : BizBase
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            // modify the IQueryable using the specification's criteria expression
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Includes all expression-based includes
            query = specification.Includes.Aggregate(query,
                                    (current, include) => current.Include(include));

            // Include any string-based include statements
            query = specification.IncludeStrings.Aggregate(query,
                                    (current, include) => current.Include(include));

            // Apply ordering if expressions are set
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // Apply paging if enabled
            if (specification.isPagingEnabled)
            {
                query = query.Skip(specification.Skip)
                             .Take(specification.Take);
            }
            return query;
        }
    }

    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool isPagingEnabled { get; private set; } = false;

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
        protected virtual void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            isPagingEnabled = true;
        }
        protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }
        protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }
    }

    public class UserAccessRightsSpecification : BaseSpecification<UserAccessRights>
    {
        public UserAccessRightsSpecification(long GroupID)
            : base(o => o.GroupID == GroupID)
        {
            AddInclude($"{nameof(UserAccessRights.Modules)}.{nameof(UserAssignModule.SubModules)}.{nameof(UserAssignSubModule.Tools)}");
            AddInclude($"{nameof(UserAccessRights.Modules)}.{nameof(UserAssignModule.SubModules)}.{nameof(UserAssignSubModule.Fields)}");
        }
    }

    public class GroupRights : BaseSpecification<ModuleGroupAccess>
    {
        public GroupRights(long GroupID)
            : base(o => o.GroupID == GroupID)
        {
            //AddInclude($"{nameof(ModuleGroupAccess.SubModuleID)}");
            //AddInclude($"{nameof(ModuleGroupAccess.SubModuleID)}.{nameof(ModuleGroupAccess.CrudTypes)}");
        }
    }

    public class SubFormModuleList : BaseSpecification<SubModuleFormMaster>
    {
        public SubFormModuleList(long SubModuleID)
            : base(o => o.ModuleGroupAccessID == SubModuleID)
        {
            //AddInclude($"{nameof(SubModuleFormMaster.ModuleGroupAccessID)}.{nameof(SubModuleFormMaster.CrudTypes)}");
            //AddInclude($"{nameof(SubModuleFormMaster.ModuleGroupAccessID)}.{nameof(SubModuleFormMaster.FieldID)}");
        }
    }

    public class GetChildNode : BaseSpecification<SubModuleMaster>
    {
        public GetChildNode(long ModuleID)
            : base(o => o.ParentID == ModuleID)
        {
            //AddInclude($"{nameof(SubModuleMaster.Id)}");
            //AddInclude($"{nameof(SubModuleMaster.Id)}.{nameof(SubModuleMaster.GUID)}");
            //AddInclude($"{nameof(SubModuleMaster.Id)}.{nameof(SubModuleMaster.ModuleDomainType)}");
            //AddInclude($"{nameof(SubModuleMaster.Id)}.{nameof(SubModuleMaster.ModuleID)}");
            //AddInclude($"{nameof(SubModuleMaster.Id)}.{nameof(SubModuleMaster.ParentGUID)}");
            //AddInclude($"{nameof(SubModuleMaster.Id)}.{nameof(SubModuleMaster.ParentID)}");
            //AddInclude($"{nameof(SubModuleMaster.Id)}.{nameof(SubModuleMaster.Status)}");
            //AddInclude($"{nameof(SubModuleMaster.Id)}.{nameof(SubModuleMaster.SubModuleName)}");
            //AddInclude($"{nameof(SubModuleMaster.Id)}.{nameof(SubModuleMaster.Type)}");            
        }
    }

    public class UserAccessRightsSpecificationV1 : BaseSpecification<UserAccessRights>
    {
        public UserAccessRightsSpecificationV1()
            : base(o => o.Id != 0)
        {
            AddInclude($"{nameof(UserAccessRights.Modules)}.{nameof(UserAssignModule.SubModules)}.{nameof(UserAssignSubModule.Tools)}");
            AddInclude($"{nameof(UserAccessRights.Modules)}.{nameof(UserAssignModule.SubModules)}.{nameof(UserAssignSubModule.Fields)}");
        }
    }

    public class ModuleSpecification : BaseSpecification<ModuleMaster>
    {
        public ModuleSpecification()
            : base(o => o.Status == 1)
        {
        }
    }

    public class ModuleChildSpecification : BaseSpecification<ModuleMaster>
    {
        public ModuleChildSpecification()
            : base(o => o.ParentID != 0 && o.Status != 0)
        {
        }
    }

    public class SubModuleParentSpecification : BaseSpecification<SubModuleMaster>
    {
        public SubModuleParentSpecification(long id)
            : base(o => o.ModuleID == id && o.ParentID == 0)
        {
        }
    }

    public class ModuleSpecification1 : BaseSpecification<ModuleMaster>
    {
        public ModuleSpecification1(long id)
            : base(o => o.Id == id)
        {
        }
    }

    public class ModuleSpecification2 : BaseSpecification<SubModuleMaster>
    {
        public ModuleSpecification2(long id)
            : base(o => o.ModuleID == id)
        {
        }
    }

    public class ModuleSpecification3 : BaseSpecification<FieldMaster>
    {
        public ModuleSpecification3(long id)
            : base(o => o.SubModuleID == id)
        {
        }
    }

    public class ModuleSpecification4 : BaseSpecification<ToolMaster>
    {
        public ModuleSpecification4(long id)
            : base(o => o.SubModuleID == id)
        {
        }
    }

    public class CheckModuleExistSpecification1 : BaseSpecification<ModuleMaster>
    {
        public CheckModuleExistSpecification1(string Name)
            : base(o => o.ModuleName == Name)
        {
        }
    }

    public class CheckSubModuleExistSpecification2 : BaseSpecification<SubModuleMaster>
    {
        public CheckSubModuleExistSpecification2(string Name)
            : base(o => o.SubModuleName == Name)
        {
        }
    }

    public class CheckFieldExistSpecification3 : BaseSpecification<FieldMaster>
    {
        public CheckFieldExistSpecification3(string Name)
            : base(o => o.FieldName == Name)
        {
        }
    }

    public class CheckToolExistSpecification4 : BaseSpecification<ToolMaster>
    {
        public CheckToolExistSpecification4(string Name)
            : base(o => o.ToolName == Name)
        {
        }
    }

    public class EfRepository3<T> : IAsyncRepositoryV1<T> where T : UserAssignModule
    {
        protected readonly WorldexContext _dbContext;

        public EfRepository3(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual T GetById(long id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public IEnumerable<T> ListAll()
        {
            return _dbContext.Set<T>().AsEnumerable();
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class EfRepository4<T> : IAsyncRepositoryV2<T> where T : UserAssignSubModule
    {
        protected readonly WorldexContext _dbContext;

        public EfRepository4(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual T GetById(long id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public IEnumerable<T> ListAll()
        {
            return _dbContext.Set<T>().AsEnumerable();
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class EfRepository5<T> : IAsyncRepositoryV3<T> where T : UserAssignFieldRights
    {
        protected readonly WorldexContext _dbContext;

        public EfRepository5(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual T GetById(long id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public IEnumerable<T> ListAll()
        {
            return _dbContext.Set<T>().AsEnumerable();
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class EfRepository6<T> : IAsyncRepositoryV4<T> where T : UserAssignToolRights
    {
        protected readonly WorldexContext _dbContext;

        public EfRepository6(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual T GetById(long id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public IEnumerable<T> ListAll()
        {
            return _dbContext.Set<T>().AsEnumerable();
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public class EfRepository7<T> : IAsyncRepositoryV5<T> where T : ModuleGroupMaster
    {
        protected readonly WorldexContext _dbContext;

        public EfRepository7(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual T GetById(long id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public IEnumerable<T> ListAll()
        {
            return _dbContext.Set<T>().AsEnumerable();
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> IsExist(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).FirstOrDefaultAsync();
        }
    }
}
