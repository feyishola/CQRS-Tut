using MediatR;
using Microsoft.EntityFrameworkCore;
using OrdersAPI.Data;

namespace OrdersAPI.Handlers
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
    {
        // private readonly AppDbContext _context;
        private readonly ReadDbContext _context;
        public GetOrderByIdQueryHandler(ReadDbContext context)
        {
            _context = context;
        }


        public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == request.orderId, cancellationToken);

            if (order == null)
            {
                return null;
            }

            return new OrderDto(order.Id, order.FirstName, order.LastName, order.Status, order.CreatedAt, order.TotalCost);
        }

    }
}