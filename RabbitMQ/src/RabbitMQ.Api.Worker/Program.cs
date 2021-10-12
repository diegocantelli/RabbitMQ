using RabbitMQ.Api.Worker.Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Api.Worker
{
    class Program
    {
        static void Main(string[] args)
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

                var consumer = new EventingBasicConsumer(chanel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var order = JsonSerializer.Deserialize<Order>(message, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        Console.WriteLine($" [pedido] recebido {order.ItemName}");

                        chanel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception)
                    {
                        chanel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };

                chanel.BasicConsume(queue: "orderQueue",
                                    autoAck: false,
                                    consumer: consumer);

                Console.WriteLine(" Press [Enter] to exit");
                Console.ReadLine();
            }
        }
    }
}
