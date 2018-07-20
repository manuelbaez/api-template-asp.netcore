using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using DataBaseAccess.Contexts;

namespace Api
{
    public partial class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver
                    = new CamelCasePropertyNamesContractResolver();
            });

            services.AddDbContext<TestContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("Dev"))
                );

            services.AddAutoMapper(t =>
            {
                // t.AddProfiles(assembliesToScan: Assembly.GetAssembly(typeof(TipoVehiculoProfile)));
            });

            services.ConfigureRepositories();

            services.ConfigureServices();

            services.AddCors(o => o.AddPolicy("EveryOne", builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.WithExposedHeaders("Pagination");
            }));

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("EveryOne"));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("EveryOne");
            app.UseMvc();
        }

    }
}
