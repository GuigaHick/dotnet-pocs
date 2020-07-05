using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQDomain
{
    public class RabbitMQClient
    {
        private const string QUEUE_NAME = "rpc_queue";
        private IConnection _producerConnection;
        private readonly RabbitMQConfiguration _rabbitMQConfiguration;
        private IModel _channel;
        private string replyQueueName;
        private EventingBasicConsumer consumer;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> callbackMapper =
                new ConcurrentDictionary<string, TaskCompletionSource<string>>();
        
        public RabbitMQClient(ILogger<RabbitMQClient> logger, RabbitMQConfiguration rabbitMQClientConfiguration)
        {
            _rabbitMQConfiguration = rabbitMQClientConfiguration;
            _logger = logger;
            Connect();
        }

        private void Connect()
        {
            var factory = new ConnectionFactory()
            { 
                  HostName = _rabbitMQConfiguration.HostName,
                  Port = _rabbitMQConfiguration.Port,
                  UserName = _rabbitMQConfiguration.UserName,
                  Password = _rabbitMQConfiguration.Password
            };

            _producerConnection = factory.CreateConnection();
            _channel = _producerConnection.CreateModel();

            replyQueueName = _channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                if (!callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out TaskCompletionSource<string> tcs))
                    return;
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                tcs.TrySetResult(response);
                _logger.LogInformation($"Received a response {response} to a request with correlation ID{ea.BasicProperties.CorrelationId}");
            };
        }

        public Task<string> CallAsync(string message, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.LogInformation($"Sending a message {message}");
            IBasicProperties props = _channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var tcs = new TaskCompletionSource<string>();
            callbackMapper.TryAdd(correlationId, tcs);

            _channel.BasicPublish(
                exchange: "",
                routingKey: QUEUE_NAME,
                basicProperties: props,
                body: messageBytes);

            _channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            cancellationToken.Register(() => callbackMapper.TryRemove(correlationId, out var tmp));
            return tcs.Task;
        }

        public void Close()
        {
            _producerConnection.Close();
        }
    }
    
}
