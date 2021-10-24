using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.RPC.Sever.Domain
{
    public enum OrderStatus
    {
        Processing = 0,
        Approved,
        Declined
    }
}
