using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Events
{
    public class ConsoleEventPublisher : IEventPublisher
    {
        public Task PublishAsync<TEvent>(TEvent evt)
        {
            Console.WriteLine($"Event published: {typeof(TEvent).Name} - {evt}");
            return Task.CompletedTask;
        }
    }
}