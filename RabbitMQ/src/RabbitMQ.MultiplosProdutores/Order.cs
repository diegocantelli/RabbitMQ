using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.MultiplosProdutores
{
    public class Order
    {
        public long Id { get; private set; }
        public DateTime CreateDate { get; }
        public DateTime LastUpdated { get; private set; }
        public long Amount { get; private set; }

        public Order(long id, long amount)
        {
            Id = id;
            Amount = amount;
            LastUpdated = DateTime.Now;
        }
    }
}
