using Microsoft.EntityFrameworkCore;
using OrdersAPI.Data;
using OrdersAPI.Handlers;
using FluentValidation;
using OrdersAPI.Commands;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ReadDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("ReadDbConnection")));
builder.Services.AddDbContext<WriteDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("WriteDbConnection")));

builder.Services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>(); // this registers the CreateOrderCommandValidator as a service that implements the IValidator interface for validating CreateOrderCommand. This allows us to use dependency injection to inject the validator into our API endpoints or other services that need to validate these commands. The validator will be used to ensure that the data in the CreateOrderCommand is valid before it is processed by the handler.

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly)); // this registers MediatR services and automatically scans the assembly containing the Program class for any handlers, requests, or notifications. This allows us to use MediatR for handling commands, queries, and events in a more decoupled and organized manner. By registering services from the assembly, we ensure that all our handlers and related classes are properly registered with the dependency injection container, making it easier to manage dependencies and promote a clean architecture in our application.

var app = builder.Build();


app.MapPost("/api/orders", async (IMediator mediator, CreateOrderCommand command) =>
{

    try
    {
        var order = await mediator.Send(command); // This line sends the CreateOrderCommand to the MediatR pipeline, which will then route it to the appropriate handler (in this case, CreateOrderCommandHandler) for processing. The handler will execute the logic to create a new order based on the data in the command and return the created order as a result. The result is then stored in the 'order' variable for further processing or returning as a response to the client.

        if (order == null)
        {
            return Results.BadRequest("Unable to create order");
        }
        return Results.Created($"/api/orders/{order.Id}", order);
    }
    catch (ValidationException ex)
    {
        var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }); // This is to select the property name and error message from the validation exception and return it as a response to the client. This will help the client to understand what went wrong with the request and how to fix it. The Select method is used to project each validation error into an anonymous object that contains the property name and error message. This allows us to return a more structured and informative response to the client when a validation error occurs.
        return Results.BadRequest(errors);
    }

});

app.MapGet("/api/orders/{id}", async (IMediator mediator, int id) =>
{

    var order = await mediator.Send(new GetOrderByIdQuery(id));

    if (order == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(order);
});

app.MapGet("/api/orders/summaries", async (IMediator mediator) =>
{
    var summaries = await mediator.Send(new GetOrderSummariesQuery());
    return Results.Ok(summaries);
});

app.Run();

