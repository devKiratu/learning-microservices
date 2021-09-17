using CommandsService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandsService.Data
{
    public class CommandRepository : ICommandRepository
    {
        private readonly MainDbContext dbContext;

        public CommandRepository(MainDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public void CreateCommand(int platformId, Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.PlatformId = platformId;
            dbContext.Commands.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            dbContext.Platforms.Add(platform);
        }

        public bool ExternalPlatformExists(int externalPlatformId)
        {
            return dbContext.Platforms.Any(p => p.ExternalId == externalPlatformId);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return dbContext.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return dbContext.Commands.Where(c => c.PlatformId == platformId)
                                     .Where(c => c.Id == commandId)
                                     .FirstOrDefault();
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return dbContext.Commands.Where(p => p.PlatformId == platformId)
                                     .OrderBy(p => p.Platform.Name);
        }

        public bool PlatformExists(int platformId)
        {
            return dbContext.Platforms.Any(p => p.Id == platformId);
        }

        public bool SaveChanges()
        {
            return dbContext.SaveChanges() >= 0;
        }
    }
}
