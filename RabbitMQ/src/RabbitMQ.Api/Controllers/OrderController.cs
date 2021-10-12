using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Api.Domain;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMQ.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<Order> _logger;

        public OrderController(ILogger<Order> logger)
        {
            _logger = logger;
        }

        [HttpPost("InsertOrder")]
        public IActionResult InsertOrder(Order order)
        {
            try
            {
                try
                {
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    using (var chanel = connection.CreateModel())
                    {
                        chanel.QueueDeclare(queue: "orderQueue",
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

                        string message = JsonSerializer.Serialize(order);
                        var body = Encoding.UTF8.GetBytes(message);

                        chanel.BasicPublish(exchange: "",
                                            routingKey: "orderQueue",
                                            basicProperties: null,
                                            body: body);

                        Console.WriteLine(" [x] Sent {0}", message);
                    }
                }
                catch (Exception)
                {
                    _logger.LogError("Erro ao inserir na fila");
                    throw;
                }
                return Accepted(order);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao tentar criar um novo pedido", ex);

                return new StatusCodeResult(500);
            }
        }
    }
}
