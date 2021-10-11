using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.RPC.Domain
{
    public enum OrderStatus
    {
        Processing = 0,
        Approved,
        Declined
    }
}
