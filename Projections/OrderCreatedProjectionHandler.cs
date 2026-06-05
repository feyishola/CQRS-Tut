using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrdersAPI.Data;
using OrdersAPI.Events;
using OrdersAPI.Models;

namespace OrdersAPI.Projections
{
    public class OrderCreatedProjectionHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly ReadDbContext _context;

        public OrderCreatedProjectionHandler(ReadDbContext context)
        {
            _context = context;
        }
        public async Task HandleAsync(OrderCreatedEvent evt)
        {
            var order = new Order
            {
                Id = evt.OrderId,
                FirstName = evt.FirstName,
                LastName = evt.LastName,
                TotalCost = evt.TotalCost,
                Status = "Created",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            
        }
    }
}