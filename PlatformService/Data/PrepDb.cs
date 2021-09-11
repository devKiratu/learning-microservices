using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepDbPopulation(IApplicationBuilder app)
        {
            //You cannot inject dbcontext using constructor injection since
            //this is a static class. 
            //Remedy: create a service scope and request an instance of the 
            //DbContext. 
            //The PrepDbPopulation is called in the Configure method of 
            //Startup.cs and passed in an instance of the application builder
            using var serviceScope = app.ApplicationServices.CreateScope();
            var provider = serviceScope.ServiceProvider;
            var dbContext = provider.GetRequiredService<MainDbContext>();
            SeedData(dbContext);

        }

        private static void SeedData(MainDbContext dbContext)
        {
            if (!dbContext.Platforms.Any())
            {
                Console.WriteLine("--> Seeding data ...");

                //populate db
                dbContext.Platforms.AddRange
                    (
                    new Platform { Id = 1, Cost = "Free", Name = "dotNet", Publisher = "Microsoft" },
                    new Platform { Id = 2, Cost = "Free", Name = "K8s", Publisher = "CNCF" },
                    new Platform { Id = 3, Cost = "Free", Name = "SQL Server Express", Publisher = "Microsoft" }
                    );
                //save changes
                dbContext.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data in the InMemoryDB");
            }
        }
    }
}
