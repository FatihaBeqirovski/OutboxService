using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using OutboxService.Daemon;
using OutboxService.Database;
using OutboxService.Database.Implementations;
using OutboxService.Database.Interfaces;
using OutboxService.Queue;

namespace OutboxService
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureLogger(hostContext, services);

                    services.AddHostedService<HealthCheckDaemon>();
                    services.AddHostedService<OutboxDaemon>();
                    services.AddHostedService<OutboxRecoveryDaemon>();
                    
                    services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();
                    services.AddTransient<ISqlPollingSource, SqlPollingSource>();

                });
        }

        private static void ConfigureLogger(HostBuilderContext hostContext, IServiceCollection services)
        {
            var nlogConfig = hostContext.Configuration["NLogConfig"];
            if (!string.IsNullOrWhiteSpace(nlogConfig))
            {
                File.WriteAllText("nlog.config", nlogConfig, Encoding.UTF8);
                services.AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddNLog();
                });
            }
        }
    }
}