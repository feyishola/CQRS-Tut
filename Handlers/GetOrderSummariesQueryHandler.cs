using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrdersAPI.Data;

namespace OrdersAPI.Handlers
{
    public class GetOrderSummariesQueryHandler : IQueryHandler<GetOrderSummariesQuery, List<OrderSummaryDto>>
    {
        private readonly ReadDbContext _context;

        public GetOrderSummariesQueryHandler(ReadDbContext context)
        {
            _context = context;
        }
        public async Task<List<OrderSummaryDto>?> HandleAsync(GetOrderSummariesQuery query)
        {
            return await Task.FromResult(_context.Orders.Select(o => new OrderSummaryDto(
                o.Id,
                $"{o.FirstName} {o.LastName}",
                o.Status,
                o.TotalCost
            )).ToList());
        }
    }
}