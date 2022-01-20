using System;
using System.Linq;
using CommandsService.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommandService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedData(AppDbContext context)
        {
            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data");

                context.Platforms.AddRange(new CommandsService.Models.Platform() { Name = "DotNet", ExternalID = 1 },
                                          new CommandsService.Models.Platform() { Name = "Redis", ExternalID = 2 });
                context.SaveChanges();

            }
            else
            {
                Console.WriteLine("--> there are some platforms in DB already");
            }

        }
    }

}