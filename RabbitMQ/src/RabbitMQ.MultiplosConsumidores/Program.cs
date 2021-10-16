using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.MultiplosConsumidores
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "order",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // no caso de consumidores, múltiplos consumidores podem compartilhar o mesmo channel e consequentemente
                // a mesma connection
                BuildWorker(channel, "Worker 1", "order");
                BuildWorker(channel, "Worker 1", "order");
            }
            
        }

        public static void BuildWorker(IModel channel, string worker, string queue)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" {worker} Received {message}");
            };

            channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);

            Console.ReadLine();
        }
    }
}
