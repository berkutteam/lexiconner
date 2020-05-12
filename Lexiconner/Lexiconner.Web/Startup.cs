using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Lexiconner.Application.Extensions;
using Lexiconner.Application.Helpers;
using Lexiconner.Domain.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Serilog;

namespace Lexiconner.Web
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
            var config = Configuration.Get<ApplicationSettings>();

            services.AddOptions();
            services.Configure<ApplicationSettings>(Configuration);
            services.Configure<ApplicationClientSettings>(Configuration);

            services.AddHttpClient(); // register IHttpClientFactory
            services.AddHttpContextAccessor();

            // Allows to access the actual controller context (with RouteData)
            // Beware that looks like in .Net Core 3 behaviour was changed and ActionContext can be null
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            ConfigureLogger(services);

            services.AddCors(options =>
            {
                options.AddPolicy("default", builder =>
                {
                    if (config.Cors != null)
                    {
                        builder
                           .WithOrigins(config.Cors.AllowedOrigins.ToArray())
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();

                        // add additional exposed headers (sets Access-Control-Expose-Headers header)
                        builder.WithExposedHeaders(new string[] {
                            HeaderNames.WWWAuthenticate,
                        });
                    }
                });
            });

            services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
            {
                SerializationConfig.GetDefaultJsonSerializerSettings(options.SerializerSettings);
            });
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you. If you
        // need a reference to the container, you need to use the
        // "Without ConfigureContainer" mechanism (see docs).
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacDefaultModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (HostingEnvironmentHelper.IsDevelopmentAny())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                await next();

                // If there's no available file and the request doesn't contain an extension, we're probably trying to access a page.
                // Rewrite request to use app root
                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value) && !context.Request.Path.Value.StartsWith("/api"))
                {
                    //context.Request.Path = "/index.html";
                    context.Request.Path = "/";
                    context.Response.StatusCode = 200; // Make sure we update the status code, otherwise it returns 404
                    await next();
                }
            });

            // app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors("default");

            // UseRouting must go before any authorization. Otherwise authorization won't work properly.
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


        private void ConfigureLogger(IServiceCollection services)
        {
            // Override the current ILogger implementation to use Serilog
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(dispose: true);
            });
        }
    }
}
