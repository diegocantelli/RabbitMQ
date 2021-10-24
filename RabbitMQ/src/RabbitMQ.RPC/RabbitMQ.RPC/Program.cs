using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.RPC.Domain;
using System;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.RPC
{
    class Program
    {
        private static string correlationId = Guid.NewGuid().ToString();

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var replyQueue = $"{nameof(Order)}_return";
            //var correlationId = Guid.NewGuid().ToString();

            channel.QueueDeclare(queue: replyQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: nameof(Order), durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += Consumer_Received;

            channel.BasicConsume(queue: replyQueue, autoAck: true, consumer: consumer);

            var props = channel.CreateBasicProperties();

            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueue;

            while (true)
            {
                Console.WriteLine("Informe o valor do pedido: ");

                var amount = decimal.Parse(Console.ReadLine());
                var order = new Order(amount);
                var message = JsonSerializer.Serialize(order);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: nameof(order), basicProperties: props, body: body);

                Console.WriteLine("Published!");
                Console.ReadLine();
                Console.Clear();
            }
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            //var correlationId = Guid.NewGuid().ToString();

            if(correlationId == e.BasicProperties.CorrelationId)
            {
                var body = e.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received message {message}");

                return;
            }

            Console.WriteLine($"Mensagem descartada, identificadores de correlação inválidos");

        }
    }
}
