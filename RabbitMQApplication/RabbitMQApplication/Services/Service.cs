using RabbitMQDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQApplication.Services
{
    public class Service : IService
    {
        private RabbitMQClient _rabbitMQClient;

        public Service(RabbitMQClient rabbitMQClient)
        {
            _rabbitMQClient = rabbitMQClient;
        }

        public async Task<string> EnqueueData(int data)
        {
            var response = await _rabbitMQClient.CallAsync(data.ToString());

            return $"Value:{ data }: Fibonnaci {response}";
        }
    }
}
