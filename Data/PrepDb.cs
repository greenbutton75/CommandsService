using System;
using System.Collections.Generic;
using System.Linq;
using CommandsService.Data;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
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
                //SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>());
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

                var platforms = grpcClient?.ReturnAllPlatforms();
                var repo = serviceScope.ServiceProvider.GetService<ICommandRepo>();
                SeedData(repo, platforms);

            }
        }

        //private static void SeedData(AppDbContext context)
        private static void SeedData(ICommandRepo? repo, IEnumerable<Platform>? platforms)
        {
            Console.WriteLine("Seeding new platforms...");

            //repo?.CreatePlatform(new CommandsService.Models.Platform() { Name = "DotNet", ExternalID = 1 });
            //repo?.CreatePlatform(new CommandsService.Models.Platform() { Name = "Redis", ExternalID = 2 });
            //repo?.SaveChanges();

            //context.Platforms.AddRange(new CommandsService.Models.Platform() { Name = "DotNet", ExternalID = 1 },
            //                          new CommandsService.Models.Platform() { Name = "Redis", ExternalID = 2 });
            //context.SaveChanges();

            if (platforms != null)
            {
                foreach (var platform in platforms)
                {
                    if (!repo?.ExternalPlatformExists(platform.ExternalID) ?? false) repo?.CreatePlatform(platform);
                }
                repo?.SaveChanges();
            }

        }
    }

}