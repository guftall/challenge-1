using HiliTechChallenge.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HiliTechChallenge.Core
{
    public class AdsDbContext : DbContext
    {
        public DbSet<AdsEntity> Advertises { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        
        
        public AdsDbContext(DbContextOptions<AdsDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryEntity>()
                .HasOne(c => c.ParentCategoryEntity)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId);

            modelBuilder.Entity<AdsEntity>()
                .HasOne(a => a.CategoryEntity)
                .WithMany(c => c.Advertises)
                .HasForeignKey(a => a.CategoryId);
        }
    }
}