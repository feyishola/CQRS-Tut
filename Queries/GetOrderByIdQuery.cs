using MediatR;

public record GetOrderByIdQuery(int orderId) : IRequest<OrderDto?>;