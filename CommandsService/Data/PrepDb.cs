using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandsService.Data
{
    public class PrepDb
    {
        public static void PopulateData(IApplicationBuilder builder)
        {
            using var scope = builder.ApplicationServices.CreateScope();
            var provider = scope.ServiceProvider;
            var grpcClient = provider.GetRequiredService<IPlatformDataClient>();
            var platforms = grpcClient.ReturnAllPlatforms();

            SeedData(provider.GetRequiredService<ICommandRepository>(), platforms);
        }

        private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding new platforms...");

            foreach (var p in platforms)
            {
                if (!repository.ExternalPlatformExists(p.ExternalId))
                {
                    repository.CreatePlatform(p);
                }
                    repository.SaveChanges();
            }
        }
    }
}
