﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Consumidor
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "qwebapp",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // instanciando um consumidor que irá ler de uma determinada fila e canal
                var consumer = new EventingBasicConsumer(channel);

                // ea.Body estará o conteúdo enviado pelo produtor em bytes
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);

                        // após execução da regra de negócio ele marca a mensagem como entregue
                        channel.BasicAck(ea.DeliveryTag, true);
                    }
                    catch (Exception ex)
                    {
                        // caso ocorra alguma exceção no processamento da msg, ele não confirma a msg como entregue
                        channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };

                //autoAck: true -> se tiver como true toda vez que ele obter um item da fila ele irá confirmar
                // não recomendado, pois caso ocorra alguma exceção, esta mensagem será perdida
                channel.BasicConsume(queue: "qwebapp",
                    autoAck: true,
                    consumer: consumer);
            }

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }
    }
}
