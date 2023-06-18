using Microsoft.EntityFrameworkCore;
using System.Net;

namespace techtrekAPI.Entities
{
    public class TechtrekContext:DbContext
    {
        public TechtrekContext() { }

        public TechtrekContext(DbContextOptions<TechtrekContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        public DbSet<User> users { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<Cart> cart_items { get; set; }
        public DbSet<Order> orders { get; set; }
    }
}
