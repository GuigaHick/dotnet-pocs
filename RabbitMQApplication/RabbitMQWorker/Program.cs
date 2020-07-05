using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQDomain;

namespace RabbitMQWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(new RabbitMQConfiguration() { ConnectionName = "RPC Client", HostName = "localhost", Port = 5672, UserName = "testes", Password = "Testes2018!" });
                    services.AddSingleton<RabbitMQConsumer>();//One consumer for whole application
                    services.AddHostedService<Worker>();
                });
    }
}
