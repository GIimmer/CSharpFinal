using Microsoft.EntityFrameworkCore;

namespace CSharpFinal.Models
{
    public class CSharpFinalContext : DbContext
    {
        public CSharpFinalContext(DbContextOptions<CSharpFinalContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Bidder> Bidders { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}