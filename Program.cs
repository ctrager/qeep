using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace qeep
{
    public class Program
    {

        public static int Main(string[] args)
        {
            Console.WriteLine("Main"); // here on purpose

            // We have to load_config first to get Serilog's folder
            qp_config.load_config();

            string log_file_location = qp_config.get(qp_config.LogFileFolder) + "/qeep_log_.txt";

            LogEventLevel microsoft_level = (LogEventLevel)qp_config.get(qp_config.DebugLogLevelMicrosoft);
            LogEventLevel qeep_level = (LogEventLevel)qp_config.get(qp_config.DebugLogLevelQeep);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()

                .MinimumLevel.Override("Microsoft", microsoft_level)
                .MinimumLevel.Override("bd", qeep_level)

                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate:
                "{Timestamp:HH:mm:ss.ms} {Level:u3} {SourceContext} {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(log_file_location,
                    rollingInterval: RollingInterval.Day,
                outputTemplate:
                "{Timestamp:HH:mm:ss.ms} {Level:u3} {SourceContext} {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            // We have to do this after the Serilog setup.
            qp_util.init_serilog_context();

            // Write config to log, even though qeep can pick up most changes without
            // being restarted and we don't log the changed values.
            qp_config.log_config();

            try
            {
                Log.Information("Starting host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseUrls("http://localhost:5003");
            webBuilder.UseKestrel(options =>
            {
                // Because we will control it with nginx,
                // which gives good specific 413 status rather than vague 400 status
                options.Limits.MaxRequestBodySize = null;

                // This works
                //options.ListenAnyIP(8000);
            });
        });

        // public static IHostBuilder CreateHostBuilder(string[] args) =>
        //     Host.CreateDefaultBuilder(args)
        //         // dotnet install package Serilog.AspNetCore
        //         .UseSerilog()

        //         .ConfigureWebHostDefaults(webBuilder =>
        //         {
        //             webBuilder.UseStartup<Startup>();
        //         });
    }
}
