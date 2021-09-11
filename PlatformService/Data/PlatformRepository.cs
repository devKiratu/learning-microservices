using PlatformService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.Data
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly MainDbContext dbContext;
        private readonly IPlatformRepository repository;

        public PlatformRepository(MainDbContext dbContext, IPlatformRepository repository)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public void CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            dbContext.Platforms.Add(platform);
        }

        public Platform GetPlatform(int id)
        {
            return dbContext.Platforms.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Platform> GetPlatforms()
        {
            return dbContext.Platforms.ToList();
        }

        public bool SaveChanges()
        {
            return dbContext.SaveChanges() >= 0;
        }
    }
}
