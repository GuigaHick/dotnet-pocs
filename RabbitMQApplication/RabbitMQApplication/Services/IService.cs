using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQApplication.Services
{
    public interface IService
    {
        Task<string> EnqueueData(int data);
    }
}
