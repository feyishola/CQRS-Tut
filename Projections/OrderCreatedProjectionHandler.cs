using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using OrdersAPI.Data;
using OrdersAPI.Events;
using OrdersAPI.Models;

namespace OrdersAPI.Projections
{
    public class OrderCreatedProjectionHandler : INotificationHandler<OrderCreatedEvent>
    {
        private readonly ReadDbContext _context;

        public OrderCreatedProjectionHandler(ReadDbContext context)
        {
            _context = context;
        }

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                Id = notification.OrderId,
                FirstName = notification.FirstName,
                LastName = notification.LastName,
                TotalCost = notification.TotalCost,
                Status = "Created",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Orders.AddAsync(order, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            
        }
    }
}