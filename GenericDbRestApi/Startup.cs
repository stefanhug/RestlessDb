using GenericDbRestApi.Lib.Repositories;
using GenericDbRestApi.Lib.Managers;
using GenericDBRestApi.Lib.Formatters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Text.Json.Serialization;
using GenericDbRestApi.Lib.DataLayer;
using GenericDbRestApi.Lib.Services;

namespace testwebapi
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
            // needed for string serialiation of enum values
            services.AddControllers().AddJsonOptions
            (opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // DbRestApi registration
            var connectionString = Configuration.GetSection("AppSettings").GetValue<string>("ConnectionString");
            services.AddDbRestApi(connectionString);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
                //endpoints.MapControllerRoute(
                //    name: "dbapi",
                //    pattern: "dbapi/{query}");
            });
        }
    }
}
