using Microsoft.EntityFrameworkCore;
using PMSvc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSvc.Infrastructure
{
    public class ProductDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(
                product =>
                {
                    product.Property(p => p.Name).HasMaxLength(50);
                    product.Property(p => p.Model).HasMaxLength(50);
                    product.Property(p => p.Brand).HasMaxLength(50);
                    product.Property(p => p.Manufacter).HasMaxLength(50);
                    product.Property(p => p.Image).HasMaxLength(150);

                    product.HasMany(p => p.Reviews)
                            .WithOne();
                });

            modelBuilder.Entity<Review>(
                review => 
                {
                    review.Property(r => r.Id).ValueGeneratedNever();
                    review.Property(r => r.Rating).HasColumnType("int");
                    review.Property(r => r.Comment).HasMaxLength(500);
                });
        }
    }
}
