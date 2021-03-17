using Worldex.Core.Interfaces;

namespace Worldex.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly WorldexContext _context;

        public UnitOfWork(WorldexContext context)
        {
            _context = context;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
