using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
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
    public class Startup // Only called one when the host (webserver, console, etc..) starts
    {
        public static IConfigurationRoot Configuration;

        public Startup(IApplicationEnvironment appEnv)
        {
            // order matters...last one wins if you have duplicate settings defined in multiple places
            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();
                //.AddUserSecrets(); // use this instead of hardcoding the 
                

            Configuration = builder.Build(); 
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // MVC 
            services.AddMvc(config =>
            {
#if !DEBUG
                config.Filters.Add(new RequireHttpsAttribute()); // site-wide setting
#endif
            })
                .AddJsonOptions(opt =>
                {
                    // serializer for .net to json and vice versa...make json property names camel-case
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            //services.AddCaching();// need to look into what this provides

            // Identity/Auth :

            services.AddIdentity<WorldUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 8;

                // simple cookie auth
                config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login"; //where to get redirected if not auth'd

                config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents()
                {
                    // event will only fire if the Auth subsystem detected that the request was unauthorized
                    OnRedirectToLogin = ctx =>
                    {
                        // handle api calls differently
                        if (ctx.Request.Path.StartsWithSegments("/api") &&
                            ctx.Response.StatusCode == (int) HttpStatusCode.OK)
                        {
                            // send back a 401 and no payload.  the client-side can make decisions based on the 401
                            ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        return Task.FromResult(0);
                    }
                };

            })
                .AddEntityFrameworkStores<WorldContext>();  // add the identities to the context

            // Configure  Auth pipeline...need to research
            //services.ConfigureAuthorization(config =>
            //{
            //    config.AddPolicy("SalesSenior", policy =>
            //    {
            //        policy.RequireAuthenticatedUser();
            //        policy.RequireClaim("department", "sales");
            //        policy.RequireClaim("status", "senior");
            //    });
            //};

            //// Controller would have:
            //[Authorize("SalesSenior")]
            //public IActionResult Manage()
            //{
            //    //do stuff
            //}


            services.AddLogging(); //build-in support for loggin in asp.net 5/core


            // EF7
            services.AddEntityFramework()
                .AddSqlServer() //core entity doesn't know about sql so we gotta add it
                .AddDbContext<WorldContext>();

            //  (Built-In) DI

            services.AddTransient<TheWorldContextSeedData>(); // only need it once
            services.AddScoped<IWorldRepository, WorldRepository>(); // only want construction of respository/context once per request 
            services.AddScoped<CoordService>();



#if DEBUG
            services.AddScoped<IMailService, DebugMailService>(); //DI
            
#else
             services.AddScoped<IMailService, RealMailService>(); //DI
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, 
            TheWorldContextSeedData seeder, 
            ILoggerFactory loggerFactory,
            IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // change logging level, exception handling, etc..


            }
            else
            {
                
            }

            // logger has debug/console out of the box.  use AddProvider() to hook up your own
            loggerFactory.AddConsole(LogLevel.Debug);
            
            //app.UseDefaultFiles(); //mvc will handle this..dont'want to serve index.html
            app.UseStaticFiles();

            app.UseIdentity();

            // Mappings
            Mapper.Initialize(config =>
            {
                config.CreateMap<Trip, TripViewModel>().ReverseMap();
                config.CreateMap<Stop, StopViewModel>().ReverseMap();
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
            await seeder.EnsureSeedDataAsync();

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
