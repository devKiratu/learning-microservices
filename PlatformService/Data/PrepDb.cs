using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
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
        public static void PrepDbPopulation(IApplicationBuilder app, bool isProd)
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
            SeedData(dbContext, isProd);

        }

        private static void SeedData(MainDbContext dbContext, bool isProd)
        {
            if (isProd)
            {
                Console.WriteLine("--> Attempting to apply migrations ...");

                try
                {
                    dbContext.Database.Migrate();   
                } 
                catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }

            }

            if (!dbContext.Platforms.Any())
            {
                Console.WriteLine("--> Seeding data ...");

                //populate db
                dbContext.Platforms.AddRange
                    (
                    new Platform { Cost = "Free", Name = "dotNet", Publisher = "Microsoft" },
                    new Platform { Cost = "Free", Name = "K8s", Publisher = "CNCF" },
                    new Platform { Cost = "Free", Name = "SQL Server Express", Publisher = "Microsoft" }
                    );
                //save changes
                dbContext.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data.");
            }
        }
    }
}
