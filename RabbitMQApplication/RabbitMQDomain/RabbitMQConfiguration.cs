using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQDomain
{
    public class RabbitMQConfiguration
    {
        public string VirtualHostName { get; set; }
        public string ConnectionName { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
