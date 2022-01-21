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
                //SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>());
            }
        }

        //private static void SeedData(AppDbContext context)
        private static void SeedData(ICommandRepo? repo)
        {
            Console.WriteLine("Seeding new platforms...");

            repo?.CreatePlatform(new CommandsService.Models.Platform() { Name = "DotNet", ExternalID = 1 });
            repo?.CreatePlatform(new CommandsService.Models.Platform() { Name = "Redis", ExternalID = 2 });
            repo?.SaveChanges();
            //context.Platforms.AddRange(new CommandsService.Models.Platform() { Name = "DotNet", ExternalID = 1 },
            //                          new CommandsService.Models.Platform() { Name = "Redis", ExternalID = 2 });
            //context.SaveChanges();


        }
    }

}