using Microsoft.EntityFrameworkCore;
using ProduitApi.Domain.Entities;

namespace ProduitApi.Infrastructure.Data
{
    public class ProductDbContext: DbContext
    {
        public ProductDbContext() { }

        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) 
        {

        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
