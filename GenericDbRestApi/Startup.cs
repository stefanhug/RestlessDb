using GenericDbRestApi.DataLayer;
using GenericDbRestApi.Managers;
using GenericDBRestApi.Formatters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Text.Json.Serialization;

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
            services.AddControllers().AddJsonOptions
            (opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            var connectionString = Configuration.GetSection("AppSettings").GetValue<string>("ConnectionString");
            services.AddScoped<GenericQueryRepository, GenericQueryRepository>();
            services.AddScoped<GenericQueryManager, GenericQueryManager>();
            services.AddScoped<SqlConnection, SqlConnection >( p => new SqlConnection(connectionString));

            //Formatters
            services.AddSingleton<IQueryFormatter, QueryJsonFormatter>();
            services.AddSingleton<IQueryFormatter, QueryXmlFormatter>();
            services.AddSingleton<IQueryFormatter, QueryCsvFormatter>();
            services.AddSingleton<IQueryFormatter, QueryExcelFormatter>();
            
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
            });
        }
    }
}
