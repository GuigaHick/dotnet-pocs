using Microsoft.AspNetCore.Mvc;
using RabbitMQApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly IService _service;
        public ServiceController(IService service)
        {
            _service = service;
        }

        /// <summary>
        /// Insert a number to calculate its fibonacci value
        /// </summary>
        /// <param name="numero"></param>
        /// <returns></returns>
        [HttpGet("CalculateFibonacci")]
        public string CalculateFibonacci(int numero)
        {
            return _service.EnqueueData(numero).Result;
        }
    }
}
