using RabbitMQ.RPC.Sever.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.RPC.Sever.Services
{
    public class OrderServices
    {
        public static OrderStatus OnStore(decimal amount)
        {
            return (amount < 0 || amount > 1000) ? OrderStatus.Declined : OrderStatus.Approved;
        }
    }
}
