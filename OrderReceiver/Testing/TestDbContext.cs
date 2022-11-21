using Microsoft.EntityFrameworkCore;
using OrderReceiver.Models;

namespace OrderReceiver.Testing
{
    public class TestDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "Orders");
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<ProductPack> ProductPacks { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}
