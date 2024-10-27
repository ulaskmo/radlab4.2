using Microsoft.EntityFrameworkCore;

namespace AdsApi.Models
{
    public class AdsDbContext : DbContext
    {
        public AdsDbContext(DbContextOptions<AdsDbContext> options) : base(options) { }

        public DbSet<Ad> Ads { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ad>()
                .HasOne(a => a.Seller)
                .WithMany(s => s.Ads);

            modelBuilder.Entity<Ad>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Ads);
        }
    }
}
