using MarketMaker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketMaker.Infrastructure.Contexts
{
    //TODO extend dbcontext class and implement repositories method
    public sealed class MarketMakerContext : DbContext
    {
        public MarketMakerContext(DbContextOptions<MarketMakerContext> options) : base(options) { }

        public DbSet<Common> Commons { get; set; }
        public DbSet<MarketMakerPreference> MarketMakerPreferences { get; set; }
        public DbSet<MarketMakerRangeDetail> MarketMakerRangeDetails { get; set; }
        public DbSet<MarketMakerMasterConfiguration> MarketMakerMasterConfiguration { get; set; }
    }
}
