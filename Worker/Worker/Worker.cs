using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Worker
{
    public class WorkerConfig
    {
        public string UserName { get; set; }

        public string ServerName { get; set; }

        public int ServerPort { get; set; }

        public string Topic { get; set; }

    }

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MqttCustomerClient mqttCustomerClient;
        private readonly WorkerConfig _workerConfig;

        public Worker(ILogger<Worker> logger, WorkerConfig config)
        {
            _logger = logger;
            mqttCustomerClient = new MqttCustomerClient(logger);
            _workerConfig = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await mqttCustomerClient.Connect(_workerConfig.UserName, _workerConfig.ServerName, _workerConfig.ServerPort);
            mqttCustomerClient.Subscribe(_workerConfig.Topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
