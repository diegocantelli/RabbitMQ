using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.MultiplosProdutores
{
    class Program
    {
        static void Main(string[] args)
        {
            // cria a conexao com o RabbitMQ
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())

            using (var channel = connection.CreateModel())
            {

                var queueName = "order";

                var channel1 = CreateChannel(connection);
                var channel2 = CreateChannel(connection);

                // Os publishers compartilham a conexão, mas não os channels
                BuildPublishers(channel1, queueName, "Produtor1");
                BuildPublishers(channel2, queueName, "Produtor2");
                Console.ReadLine();
            
            }
        }

        public static IModel CreateChannel(IConnection connection)
        {
            var channel = connection.CreateModel();

            return channel;
        }

        public static void BuildPublishers(IModel channel, string queue, string publisherName)
        {
            Task.Run(() =>
            {
                int cont = 0;

                channel.QueueDeclare(queue: queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                while (true)
                {
                    string message = $" {publisherName}: Order number {cont++}";
                    var body = Encoding.UTF8.GetBytes(message);
                     
                    channel.BasicPublish(exchange: "",
                        routingKey: queue,
                        basicProperties: null,
                        body: body);
                    Console.WriteLine(" x Sent {0}", message);
                    Thread.Sleep(200);
                }
            });
        }
    }
}
