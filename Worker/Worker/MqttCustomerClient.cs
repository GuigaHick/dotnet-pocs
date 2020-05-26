using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Formatter;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    public class MqttCustomerClient
    {
        private IMqttClient _mqttClient;
        private ILogger _logger;

        public MqttCustomerClient(ILogger logger)
        {
            _logger = logger;
            _mqttClient = new MqttFactory().CreateMqttClient();
            _mqttClient.UseApplicationMessageReceivedHandler(new
                Action<MqttApplicationMessageReceivedEventArgs>(MqttClient_ApplicationMessageReceived));
            _mqttClient.UseDisconnectedHandler(new
                Action<MqttClientDisconnectedEventArgs>(MqttClient_Disconnected));
            _mqttClient.UseConnectedHandler(new
                Action<MqttClientConnectedEventArgs>(MqttClient_Connected));
        }

        public async Task Connect(string username, string server, int port)
        {
            var clientOptionsBuilder = new MqttClientOptionsBuilder();
            clientOptionsBuilder.WithProtocolVersion((MqttProtocolVersion.V310));
            if (!string.IsNullOrWhiteSpace(username))
            {
                MqttClientCredentials credentials = new MqttClientCredentials();
                credentials.Username = username;
                clientOptionsBuilder.WithCredentials(credentials);
            }
          
            clientOptionsBuilder.WithTcpServer(server, port);

            await _mqttClient.ConnectAsync(clientOptionsBuilder.Build());
        }
        public  async void Subscribe(string topic)
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                return;
            }
            if (!string.IsNullOrWhiteSpace(topic))
            {
                MqttTopicFilter builder = new MqttTopicFilter();

                builder.Topic = topic;
                builder.QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce;
                try
                {
                    await _mqttClient.SubscribeAsync(builder);
                }
                catch (Exception ex)
                {
                    
                }
            }
        }
        private void MqttClient_Connected(MqttClientConnectedEventArgs e)
        {
            _logger.LogInformation("Connected");
        }

        private void MqttClient_Disconnected(MqttClientDisconnectedEventArgs e)
        {
            _logger.LogInformation("Disconnected");
        }
        private void MqttClient_ApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            _logger.LogInformation(ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload));
        }
    }
}
