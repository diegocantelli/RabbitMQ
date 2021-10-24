using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.RPC.Sever.Domain
{
    public class Order
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
        public string Status => OrderStatus.ToString();
        private OrderStatus OrderStatus { get; set; }

        public Order(decimal amount)
        {
            Id = DateTime.Now.Ticks;
            OrderStatus = OrderStatus.Processing;
            Amount = amount;
        }
    }
}
