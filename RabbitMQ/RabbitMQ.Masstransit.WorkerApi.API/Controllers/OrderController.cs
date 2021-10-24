using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Masstransit.WorkerApi.API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Masstransit.WorkerApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IBusControl _bus;

        public OrderController(ILogger<OrderController> logger, IBusControl bus)
        {
            _bus = bus;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] long orderId)
        {
            await _bus.Publish<Order>(new Order { OrderId = orderId });

            _logger.LogInformation($"Message received. Order Id: {orderId}");

            return Ok($"{DateTime.Now}");
        }
    }
}
