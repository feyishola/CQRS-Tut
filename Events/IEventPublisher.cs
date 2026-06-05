using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Events
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent evt);
    }
}