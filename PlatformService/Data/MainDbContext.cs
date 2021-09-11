using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) {}

        public DbSet<Platform> Platforms { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Required when your dbcontext inherits form IdentityDbContext since it has an implementation of this method, 
            //not necessary in this case as DbContext has no implementation 
            //of this method.
            base.OnModelCreating(builder); 
            
            //fluent api settings
        }
    }
}
