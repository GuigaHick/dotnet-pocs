using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MqttCustomerClient mqttCustomerClient;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            mqttCustomerClient = new MqttCustomerClient(logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await mqttCustomerClient.Connect("guilherme", "localhost", 1883);
            mqttCustomerClient.Subscribe("validar");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
