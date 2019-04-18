using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexiconner.Api.ImportAndExport;
using Lexiconner.Api.Seed;
using Lexiconner.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Lexiconner.Persistence.Repositories.MongoDb;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Lexiconner.Api
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
            services.AddTransient<IWordTxtImporter, WordTxtImporter>();
            services.AddTransient<ISeeder, MongoDbSeeder>(); // replace with other if needed
            services.AddTransient<IStudyItemJsonRepository, StudyItemJsonRepository>(serviceProvider =>
            {
                return new StudyItemJsonRepository(Configuration.GetValue<string>("JsonStorePath"));
            });
            services.AddTransient<IStudyItemRepository, StudyItemRepository>();
            
            /*
             * Typically you only create one MongoClient instance for a given cluster and use it across your application. 
             * Creating multiple MongoClients will, however, still share the same pool of connections if and only if the connection strings are identical.
            */
            services.AddTransient<MongoClient>(serviceProvider => new MongoClient(config.MongoDbConnectionString));
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen();

            services.AddApiVersioning(options =>
            {
                // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(
               options =>
               {
                   // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                   // note: the specified format code will format the version as "'v'major[.minor][-status]"
                   options.GroupNameFormat = "'v'VVV";

                   // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                   // can also be used to control the format of the API version in route templates
                   options.SubstituteApiVersionInUrl = true;
               });

            services.AddCors(options =>
            {
                options.AddPolicy("js-client-policy", builder =>
                {
                    builder.AllowAnyMethod();
                    builder.AllowAnyMethod();
                    builder.AllowCredentials();
                    builder.WithOrigins(Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>());
                });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCors("js-client-policy");
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(
               options =>
               {
                   foreach (var description in provider.ApiVersionDescriptions)
                   {
                       options.SwaggerEndpoint(
                           $"/swagger/{description.GroupName}/swagger.json",
                           description.GroupName.ToUpperInvariant());
                   }
               });

            // seed
            if (env.IsDevelopment())
            {
                var seeder = app.ApplicationServices.GetRequiredService<ISeeder>();
                seeder.Seed().Wait();
            }
        }
    }
}
