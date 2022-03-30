// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using QuickApp.Helpers;
using NLog;
using NLog.Fluent;


namespace QuickApp
{
    public class Program

    {
        private static Logger Mysameplelogger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            LogSample();


            //Seed database
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var databaseInitializer = services.GetRequiredService<IDatabaseInitializer>();
                    databaseInitializer.SeedAsync().Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogCritical(LoggingEvents.INIT_DATABASE, ex, LoggingEvents.INIT_DATABASE.Name);

                    throw new Exception(LoggingEvents.INIT_DATABASE.Name, ex);
                }
            }

            host.Run();
        }

        static void LogSample()
        {

            Mysameplelogger.Trace("Trace: This is a sample Trace Log");
   
            Mysameplelogger.Debug("Debug: This is a sample Debug Log");
   
            Mysameplelogger.Info("Info: This is a sample Info Log");
            Mysameplelogger.Warn("Warn: This is a sample Warn Log");
            Mysameplelogger.Error("Error: This is a sample Error Log");
   
            Mysameplelogger.Fatal("Fatal: This is a sample Fatal Error Log");
   
            Mysameplelogger.Info()
               .Message("This is a test Info message '{0}'.",
                  DateTime.Now.Ticks)
               .Property("Test", "InfoWrite")
               .Write();

        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                    logging.AddFile(hostingContext.Configuration.GetSection("Logging"));
                });


    }
}
