using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Serialization;
using TheWorld.Interfaces;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld
{
    public class Startup // Only called one when the web server starts
    {
        public static IConfigurationRoot Configuration;

        public Startup(IApplicationEnvironment appEnv)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build(); 
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(opt =>
                {
                    // serializer for .net to json and vice versa...make json property names camel-case
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.AddLogging(); //build-in support for loggin in asp.net 5/core


            // DI Built-In 

            // EF7
            services.AddEntityFramework()
                .AddSqlServer() //core entity doesn't know about sql so we gotta add it
                .AddDbContext<WorldContext>();

            services.AddTransient<TheWorldContextSeedData>(); // only need it once
            services.AddScoped<IWorldRepository, WorldRepository>(); // only want construction of respository/context once per request 

#if DEBUG
            services.AddScoped<IMailService, DebugMailService>(); //DI
            
#else
             services.AddScoped<IMailService, RealMailService>(); //DI
#endif  
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, TheWorldContextSeedData seeder, ILoggerFactory loggerFactory)
        {
            // logger has debug/console out of the box.  use AddProvider() to hook up your own
            loggerFactory.AddConsole(LogLevel.Debug);

            //app.UseDefaultFiles(); //mvc will handle this..dont'want to serve index.html
            app.UseStaticFiles();

            // Mappings
            Mapper.Initialize(config =>
            {
                config.CreateMap<Trip, TripViewModel>()
                    .ReverseMap();
            }
            );

            // Routes
            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new {controller = "App", action = "Index"}
                    );
            }
                );


            //  Data seed/prep (only a 1-time thing...or use it to update schema?)
            seeder.EnsureSeedData();

            //app.UseIISPlatformHandler();
//            app.Run(async (context) =>
//            {
//                await context.Response.WriteAsync(html); //$"Hello World! {context.Request.Path}");
//            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
