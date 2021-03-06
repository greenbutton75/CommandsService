using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CommandsService.Data;
using CommandService.Data;
using StackExchange.Redis;
using CommandsService.EventProcessing;
using CommandsService.AsyncDataServices;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Console.WriteLine("--> Using InMem db");
            //services.AddDbContext<AppDbContext>(opt =>
            //            opt.UseInMemoryDatabase("InMem"));
            Console.WriteLine("--> Using Redis db");
            services.AddSingleton<IConnectionMultiplexer>(opt =>
            ConnectionMultiplexer.Connect(Configuration.GetConnectionString("DockerRedisConnection")));

            //services.AddSingleton<IMessageBusClient, MessageBusClient>();

            services.AddScoped<ICommandRepo, CommandRepo>();
            services.AddSingleton<IEventProcessor, EventProcessor>();
            services.AddScoped<IPlatformDataClient, PlatformDataClient>();

            services.AddControllers();

            services.AddHostedService<MessageBusSubscriber>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CommandsService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CommandsService v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            PrepDb.PrepPopulation(app);

        }
    }
}
