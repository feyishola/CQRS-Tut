using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrdersAPI.Data;

namespace OrdersAPI.Handlers
{
    public class GetOrderSummariesQueryHandler : IRequestHandler<GetOrderSummariesQuery, List<OrderSummaryDto>>
    {
        private readonly ReadDbContext _context;

        public GetOrderSummariesQueryHandler(ReadDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderSummaryDto>> Handle(GetOrderSummariesQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_context.Orders.AsNoTracking().Select(o => new OrderSummaryDto(
                o.Id,
                $"{o.FirstName} {o.LastName}",
                o.Status,
                o.TotalCost
            )).ToList());
        }
    }
}