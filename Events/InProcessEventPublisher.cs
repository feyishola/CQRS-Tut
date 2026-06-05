using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Events
{
    public class InProcessEventPublisher : IEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;
        public InProcessEventPublisher(IServiceProvider serviceProvider) // IServiceProvider is used to resolve event handlers i.e. IEventHandler<TEvent> which in simple terms means that we can use it to get instances of event handlers (in this case IEventHandler<TEvent>) that are registered in the dependency injection container.
        {
            _serviceProvider = serviceProvider;
        }
        public async Task PublishAsync<TEvent>(TEvent evt)
        {
            using var scope = _serviceProvider.CreateScope(); // Create a new scope for resolving event handlers. This ensures that any scoped services used by the event handlers are properly disposed of after the event is published.
            var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>(); // Resolve all event handlers for the given event type TEvent. This will return a collection of all registered handlers that implement IEventHandler<TEvent>.

            // var tasks = handlers.Select(handler => handler.HandleAsync(evt)); // For each resolved event handler, call its HandleAsync method to process the event. This will return a collection of tasks representing the asynchronous handling of the event by each handler.
            // await Task.WhenAll(tasks); // Wait for all event handlers to complete their processing of the event. This ensures that the PublishAsync method does not return until all handlers have finished handling the event.
             foreach (var handler in handlers)
            {
                await handler.HandleAsync(evt);
            }
        }

    }
}