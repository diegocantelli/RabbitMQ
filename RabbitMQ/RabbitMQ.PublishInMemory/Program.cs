using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.PublishInMemory
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        static async void Process()
        {
            var bus = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint("order_queue", ep =>
                {
                    ep.Handler<Message>(context =>
                    {
                        return Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
                    });
                });
            });

            await bus.StartAsync();

            try
            {
                var index = 0;
                while (true)
                {
                    object p = bus.Publish(new Message
                    {
                        OrderId = index,
                        Text = $"{DateTime.UtcNow} => message index order: {index++}"
                    });

                    await Task.Run(() => Thread.Sleep(1000));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                await bus.StopAsync();
            }
        }
    }

    internal class Message
    {
        public int OrderId { get; set; }
        public string Text { get; set; }
    }
}
