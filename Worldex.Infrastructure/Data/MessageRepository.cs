using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.Interfaces;
using Worldex.Core.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Worldex.Infrastructure.Data
{
    public class MessageRepository<T> : IMessageRepository<T> where T : BizBase
    {
        private readonly WorldexContext _dbContext;

        public MessageRepository(WorldexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T GetById(long id)
        {
            return _dbContext.Set<T>().SingleOrDefault(e => e.Id == id);
        }

        public Task<List<T>> List()
        {
            return Task.FromResult(_dbContext.Set<T>().ToList());
        }

        public Task<T> Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();
            return Task.FromResult(entity);
        }

        public async void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
        
    }
}