using JwtAuthAspNetCoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthAspNetCoreApi.Data.DatabaseContext
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public virtual DbSet<UserAccount> UserAccounts { get; set; }
        public virtual DbSet<Product> Products { get; set; }
    }
}
