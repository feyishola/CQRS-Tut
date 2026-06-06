using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using OrdersAPI.Data;
using OrdersAPI.Events;
using OrdersAPI.Models;

namespace OrdersAPI.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly WriteDbContext _context;
        private readonly IValidator<CreateOrderCommand> _validator;
        private readonly IMediator _mediator;


        public CreateOrderCommandHandler(WriteDbContext context, IValidator<CreateOrderCommand> validator, IMediator mediator)
        {
            _context = context;
            _validator = validator;
            _mediator = mediator;
        }

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
             var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var order = new Order
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Status = request.Status,
                TotalCost = request.TotalCost
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var orderCreatedEvent = new OrderCreatedEvent(order.Id, order.FirstName, order.LastName, order.TotalCost);
            
            await _mediator.Publish(orderCreatedEvent, cancellationToken); // Publish an event after creating

            return new OrderDto(
                order.Id,
                order.FirstName,
                order.LastName,
                order.Status,
                order.CreatedAt,
                order.TotalCost
            );
        }

    }
}