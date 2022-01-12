using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Reflection;

namespace RestlessDb.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = CustomizeLogger(new LoggerConfiguration()).CreateLogger();
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception ex) 
            {
                logger.Error(ex, "Exception in Program.Main");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((hostingContext, loggerConfiguration) => CustomizeLogger(loggerConfiguration));
            return hostBuilder; 
        }

        public static LoggerConfiguration CustomizeLogger(LoggerConfiguration loggerConfiguration)
        {
            return loggerConfiguration
                    //TODO: Reading configuration from appsettings.json does not work
                    //.ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                    .WriteTo.File($"logs/{Assembly.GetEntryAssembly().GetName().Name}-{DateTime.Now.ToString("yyy-MM-dd")}.log");
        }
    }
}
