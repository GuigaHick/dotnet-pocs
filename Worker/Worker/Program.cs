using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Worker
{
    public class Program
    {
        private const string WORKER_CONFIG = "WorkerConfig";
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    WorkerConfig options = configuration.GetSection(WORKER_CONFIG).Get<WorkerConfig>();

                    services.AddSingleton(options);
                    services.AddHostedService<Worker>();
                });
    }
}
