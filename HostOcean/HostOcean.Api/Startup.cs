using Hangfire;
using Hangfire.Dashboard;
using HostOcean.Api.Filters;
using HostOcean.Api.Hubs;
using HostOcean.Api.StartupSettings.StartupExtensions;
using HostOcean.Domain.Entities;
using HostOcean.Infrastructure.GroupScheduleService;
using HostOcean.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;

namespace HostOcean.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .MinimumLevel.Debug()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(Configuration["ElasticsearchConfiguration:Uri"]))
                {
                    AutoRegisterTemplate = true
                })
                .CreateLogger();
        }

        private IHostingEnvironment HostingEnvironment { get; }
        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            services
                .RegisterHangfire(Configuration)
                .RegisterMvc()
                .RegisterAutoMapper()
                .AddOptions()
                .ConfigureDataBase(Configuration)
                .ConfigureCors()
                .ConfigureIdentity<User, IdentityRole, HostOceanDbContext>()
                .RegisterDependencies()
                .RegisterSwaggerGen(HostingEnvironment,Configuration)
                .RegisterMediatoR()
                .RegisterJwtAuthentication(Configuration)
                .RegisterHttpClients(Configuration)
                .AddGoogleCalendarClient<IGoogleCalendarClient, GoogleCalendarV3Client, GoogleCalendarApiConfiguration>
                    (Configuration);

            services.AddSignalR(config =>
            {
                config.EnableDetailedErrors = true;
            })
            .AddJsonProtocol(config =>
            {
                config.PayloadSerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            app.UseHangfireServer();
            app.UseHangfireDashboard(Configuration["HangfireSettings:HangfirePath"], new DashboardOptions()
            {
                Authorization = new List<IDashboardAuthorizationFilter>
                {
                    new HangfireNoAuthFilter(),
                }
            });
            app.InitializeHangfireSheduleCommands();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");
            app.UseWebSockets();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSignalR(route =>
            {
                route.MapHub<HostOceanHub>("/hubs/hostocean");
            });
            app.UseMvc();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(swaggerConfiguration =>
                {
                    swaggerConfiguration.SwaggerEndpoint(
                        Configuration["Swagger:SwaggerPath"],
                        Configuration["Swagger:ApplicationName"]
                    );
                });
            }
        }
    }
}