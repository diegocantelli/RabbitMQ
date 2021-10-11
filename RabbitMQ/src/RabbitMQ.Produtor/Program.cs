using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ.Produtor
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
                // criacao da fila
                channel.QueueDeclare(queue: "qwebapp",
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
                        routingKey: "qwebapp",
                        basicProperties: null,
                        body: body);
                    Console.WriteLine(" x Sent {0}", message);
                    Thread.Sleep(200);
                }
            }

            //Console.WriteLine(" Press Enter to exit");
            //Console.ReadLine();
        }
    }
}
