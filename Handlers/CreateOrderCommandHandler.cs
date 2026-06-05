using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using OrdersAPI.Data;
using OrdersAPI.Events;
using OrdersAPI.Models;

namespace OrdersAPI.Handlers
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDto>
    {
        // private readonly AppDbContext _context;
        private readonly WriteDbContext _context;
        private readonly IValidator<CreateOrderCommand> _validator;
        private readonly IEventPublisher _eventPublisher;

        public CreateOrderCommandHandler(WriteDbContext context, IValidator<CreateOrderCommand> validator, IEventPublisher eventPublisher)
        {
            _context = context;
            _validator = validator;
            _eventPublisher = eventPublisher;
        }


        public async Task<OrderDto> HandleAsync(CreateOrderCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var order = new Order
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Status = command.Status,
                TotalCost = command.TotalCost
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            await _eventPublisher.PublishAsync(new OrderCreatedEvent(order.Id, order.FirstName, order.LastName, order.TotalCost)); // Publish an event after creating the order

            return new OrderDto(
                order.Id,
                order.FirstName,
                order.LastName,
                order.Status,
                order.CreatedAt,
                order.TotalCost
            );
        }

        // public static async Task<Order> Handle(CreateOrderCommand command, AppDbContext context)  // This is the old way of doing it without the handler. We will replace this with the handler above.
        // {
        //     var order = new Order
        //     {
        //         FirstName = command.FirstName,
        //         LastName = command.LastName,
        //         Status = command.Status,
        //         TotalCost = command.TotalCost
        //     };

        //     await context.Orders.AddAsync(order);
        //     await context.SaveChangesAsync();

        //     return order;
        // }

    }
}