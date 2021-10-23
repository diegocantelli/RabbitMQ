using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ.PublisherConfirmation
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
                // Ativa a confirmação do recebimento da msg
                channel.ConfirmSelect();

                channel.BasicAcks += Channel_BasicAcks;
                channel.BasicNacks += Channel_BasicNacks;

                // criacao da fila
                channel.QueueDeclare(queue: "order",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                int cont = 0;
                while (true)
                {
                    string message = $"{cont++} Hello World";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                        routingKey: "order",
                        basicProperties: null,
                        body: body);
                    Console.WriteLine(" x Sent {0}", message);
                    Thread.Sleep(200);

                    Console.WriteLine("Press Enter to continue");
                    Console.ReadKey();
                }
            }
        }

        private static void Channel_BasicNacks(object sender, Client.Events.BasicNackEventArgs e)
        {
            Console.WriteLine($"{DateTime.UtcNow} -> Basic Nacks");
        }

        private static void Channel_BasicAcks(object sender, Client.Events.BasicAckEventArgs e)
        {
            Console.WriteLine($"{DateTime.UtcNow} -> Basic Acks");
        }
    }
}
