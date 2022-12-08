using DemoBlazorServerEFCore.Model;
using Microsoft.EntityFrameworkCore;

namespace DemoBlazorServerEFCore
{
    public class DemoContext:DbContext
    {
        public DemoContext(DbContextOptions<DemoContext> options):base(options)
        {

        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Titles> Titles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().HasKey(o => o.Id);
            modelBuilder.Entity<Titles>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
    }
}
