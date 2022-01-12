using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using RestlessDb.Services;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace RestlessDb.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Logger = Program.CustomizeLogger(new LoggerConfiguration()).CreateLogger();
        }

        public IConfiguration Configuration { get; }
        public Serilog.ILogger Logger { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddCors();

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
            catch(Exception e)
            {
                Logger.Error(e, "Exception in ConfigureServices");
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseCors(builder =>
                {
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });

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
            catch (Exception e)
            {
                Logger.Error(e, "Exception in Configure");
            }
        }
    }
}
