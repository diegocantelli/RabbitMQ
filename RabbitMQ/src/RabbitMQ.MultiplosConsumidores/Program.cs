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
            {
                for (int i = 0; i < 2; i++)
                { 
                    var channel = CreateChannel(connection);

                    channel.QueueDeclare(queue: "order",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    for (int j = 0; j < 2; i++)
                    {
                        // no caso de consumidores, múltiplos consumidores podem compartilhar o mesmo channel e consequentemente
                        // a mesma connection
                        BuildWorkerAndRun(channel, $"Worker {j}", "order");

                    }
                }
            }
        }

        public static IModel CreateChannel(IConnection connection)
        {
            var channel = connection.CreateModel();

            return channel;
        }

        public static void BuildWorkerAndRun(IModel channel, string worker, string queue)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" Channel: {channel.ChannelNumber} - Worker  {worker} Received {message}");
            };

            channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);

            Console.ReadLine();
        }
    }
}
