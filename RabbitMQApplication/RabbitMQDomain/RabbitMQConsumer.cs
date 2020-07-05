using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQDomain
{
    public class RabbitMQConsumer
    {
        private const string QUEUE_NAME = "rpc_queue";
        private IConnection _consumerConnection;
        private readonly RabbitMQConfiguration _rabbitMQConfiguration;
        private IModel _channel;
        private EventingBasicConsumer consumer;
        private ILogger _logger;

        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger, RabbitMQConfiguration rabbitMQClientConfiguration)
        {
            _rabbitMQConfiguration = rabbitMQClientConfiguration;
            _logger = logger;
            Connect();
        }

        public void Connect()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMQConfiguration.HostName,
                Port = _rabbitMQConfiguration.Port,
                UserName = _rabbitMQConfiguration.UserName,
                Password = _rabbitMQConfiguration.Password
            };

            _consumerConnection = factory.CreateConnection();
            _channel = _consumerConnection.CreateModel();
        
            _channel.QueueDeclare(queue: QUEUE_NAME, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.BasicQos(0, 1, false);
            consumer = new EventingBasicConsumer(_channel);

            _channel.BasicConsume(queue: QUEUE_NAME, autoAck: false, consumer: consumer);
            _logger.LogInformation(" [x] Awaiting RPC requests");

            consumer.Received += (model, ea) =>
            {
                string response = null;

                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogInformation($"Received a RPC with message {message}");
                    int n = int.Parse(message);
                    _logger.LogInformation($"Calculating Fibonacci Sequence of value {n}");
                    response = fib(n).ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(" [.] " + e.Message);
                    response = "";
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    _logger.LogInformation($"Replying with RPC response {response}");
                }
            };
        }

        private static int fib(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }

            return fib(n - 1) + fib(n - 2);
        }
    }
}
