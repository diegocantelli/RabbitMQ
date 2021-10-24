using MassTransit;
using Microsoft.Extensions.Logging;
using RabbitMQ.Masstransit.WorkerApi.API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Masstransit.WorkerApi.API.Consumer
{
    // necessário implementar a interface IConsumer
    public class OrderConsumer : IConsumer<Order>
    {
        private readonly ILogger<OrderConsumer> _logger;

        public OrderConsumer(ILogger<OrderConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<Order> context)
        {
            try
            {
                var order = context.Message;
                _logger.LogInformation($"Order received: {order.OrderId}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao tentar consumir a fila.", ex.Message);
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
