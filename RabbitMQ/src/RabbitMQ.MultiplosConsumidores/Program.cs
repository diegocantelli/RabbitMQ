﻿using RabbitMQ.Client;
using System;

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
            }
            
        }
    }
}
