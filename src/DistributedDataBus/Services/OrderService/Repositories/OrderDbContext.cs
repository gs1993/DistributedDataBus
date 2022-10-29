using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Repositories
{
    public class OrderDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public OrderDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
