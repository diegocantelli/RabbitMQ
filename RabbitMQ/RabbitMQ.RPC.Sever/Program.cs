using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.RPC.Sever.Domain;
using System;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.RPC.Sever
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var consumer = InitializeConsumer(channel, nameof(Order));

                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var incommingMessage = Encoding.UTF8.GetString(ea.Body);
                        Console.WriteLine($"Incoming message: {incommingMessage}");

                        var order = JsonSerializer.Deserialize<Order>(incommingMessage);
                        //order.Status 

                        var replyMessage = JsonSerializer.Serialize(order);
                        global::System.Console.WriteLine($"Reply message {replyMessage}");

                        SendReplyMessage(replyMessage, channel, ea);
                    }
                    catch (global::System.Exception)
                    {

                        throw;
                    }
                };
            }
        }

        private static EventingBasicConsumer InitializeConsumer(IModel channel, string queueName)
        {
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            return consumer;
        }

        public static void SendReplyMessage(string replyMessage, IModel channel, BasicDeliverEventArgs ea)
        {
            var props = ea.BasicProperties;
            var replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            var responseBytes = Encoding.UTF8.GetBytes(replyMessage);

            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);

            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }
}
