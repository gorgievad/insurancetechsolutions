using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Infrastructure
{
    using Claims.Domain;

    public class ClaimsContext : DbContext
    {

        public DbSet<Claim> Claims { get; set; }
        public DbSet<Cover> Covers { get; set; }

        public ClaimsContext(DbContextOptions<ClaimsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Claim>().ToCollection("claims");
            modelBuilder.Entity<Cover>().ToCollection("covers");
        }
    }
}
