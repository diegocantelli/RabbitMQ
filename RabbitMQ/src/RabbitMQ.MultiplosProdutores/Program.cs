using RabbitMQ.Client;
using System;
using System.Collections.Generic;
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

                // Os channels podem ser entendidos como diferentes Threads
                var channel1 = CreateChannel(connection);
                //var channel2 = CreateChannel(connection);

                // Os publishers compartilham a conexão, mas não os channels
                BuildPublishers(channel1, queueName, "Produtor1");
                //BuildPublishers(channel2, queueName, "Produtor2");
                Console.ReadLine();
            
            }
        }

        public static IModel CreateChannel(IConnection connection)
        {
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "order", durable: false, exclusive: false, autoDelete: false, arguments: null);
            //channel.QueueDeclare(queue: "logs", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "finance_orders", durable: false, exclusive: false, autoDelete: false, arguments: null);

            

            // declaração do exchange que será responsável por direcionar as mensagens de uma fila para outra fila
            // fanout -> tipo que irá copiar as mensagens 
            //channel.ExchangeDeclare("order", type: "fanout");

            channel.ExchangeDeclare("order", type: "direct");


            // Aqui é feito o link entre as filas e o exchange
            channel.QueueBind("order", exchange: "order", routingKey: "order_new");
            channel.QueueBind("logs", exchange: "order", routingKey: "order_upd");
            channel.QueueBind("finance_orders", exchange: "order", routingKey: "order_new");

            return channel;
        }

        public static void BuildPublishers(IModel channel, string queue, string publisherName)
        {
            Task.Run(() =>
            {
                int cont = 0;

                // aplicando ttl para todas as msgs na fila
                var args = new Dictionary<string, object>();
                args.Add("x-message-ttl", 20000);

                channel.QueueDeclare(queue: queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    // aplicando os argumentos de ttl
                    arguments: args);

                // definindo um tempo de expiração da msg
                var props = channel.CreateBasicProperties();
                props.Expiration = "10000";

                while (true)
                {
                    string message = $" {publisherName}: Order number {cont++}";
                    var body = Encoding.UTF8.GetBytes(message);
                     
                    // Aqui é definido o exchange
                    channel.BasicPublish(exchange: "order",
                        routingKey: "",
                        basicProperties: null,
                        body: body);
                    Console.WriteLine(" x Sent {0}", message);
                    Thread.Sleep(200);
                }
            });
        }
    }
}
