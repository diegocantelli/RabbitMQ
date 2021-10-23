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

                // caso não seja possível processar a msg por algum motivo, a mensagem será devolvida.
                channel.BasicReturn += Channel_BasicReturn;

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
                        routingKey: "ordersss",
                        basicProperties: null,
                        body: body,
                        //mandatory: garante que o nome da fila seja válido
                        mandatory: true);

                    // Aguarda a confirmação de recebimento da msg por 2 segundos, caso não ocorra em 2 segundos,
                    // será lançada uma exception
                    channel.WaitForConfirms(new TimeSpan(0, 0, seconds: 2));

                    Console.WriteLine(" x Sent {0}", message);
                    Thread.Sleep(200);

                    Console.WriteLine("Press Enter to continue");
                    Console.ReadKey();
                }
            }
        }

        private static void Channel_BasicReturn(object sender, Client.Events.BasicReturnEventArgs e)
        {
            Console.WriteLine($"{DateTime.UtcNow} -> Basic Return: {e.Body}");
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
