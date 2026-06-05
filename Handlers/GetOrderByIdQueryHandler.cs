using OrdersAPI.Data;

namespace OrdersAPI.Handlers
{
    public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDto>
    {
        // private readonly AppDbContext _context;
        private readonly ReadDbContext _context;
        public GetOrderByIdQueryHandler(ReadDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDto?> HandleAsync(GetOrderByIdQuery query)
        {
            var order = await _context.Orders.FindAsync(query.orderId);

            if (order == null)
            {
                return null;
            }

            return new OrderDto(order.Id, order.FirstName, order.LastName, order.Status, order.CreatedAt, order.TotalCost);
        }

        // public static async Task<Order?> Handle(GetOrderByIdQuery query, AppDbContext context) // This is the old way of doing it without the handler. We will replace this with the handler above.
        // {
        //     var order = await context.Orders.FindAsync(query.orderId);

        //     if (order == null)
        //     {
        //         return null;
        //     }

        //     return order;
        // }

    }
}