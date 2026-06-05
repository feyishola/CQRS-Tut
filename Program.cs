using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using OrdersAPI.Data;
using OrdersAPI.Handlers;
using OrdersAPI.Models;
using FluentValidation;
using OrdersAPI.Commands;
using OrdersAPI.Events;
using OrdersAPI.Projections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("BaseConnection"))); // Add the connection string to the appsettings.json file
builder.Services.AddDbContext<ReadDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("ReadDbConnection")));
builder.Services.AddDbContext<WriteDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("WriteDbConnection")));

builder.Services.AddScoped<ICommandHandler<CreateOrderCommand, OrderDto>, CreateOrderCommandHandler>(); // Register the CreateOrderCommandHandler as a service that implements the ICommandHandler interface for handling CreateOrderCommand and returning an OrderDto. This allows us to use dependency injection to inject the handler into our API endpoints or other services that need to handle these commands. the reason for using AddScoped is that we want to have a new instance of the CreateOrderCommandHandler for each request, since it depends on the AppDbContext which is also registered as scoped. This ensures that each request gets its own instance of the handler and the context, and they are disposed of properly at the end of the request.
builder.Services.AddScoped<IQueryHandler<GetOrderByIdQuery, OrderDto>, GetOrderByIdQueryHandler>(); // this registers the GetOrderByIdQueryHandler as a service that implements the IQueryHandler interface for handling GetOrderByIdQuery and returning an OrderDto. This allows us to use dependency injection to inject the handler into our API endpoints or other services that need to handle these queries.
builder.Services.AddScoped<IQueryHandler<GetOrderSummariesQuery, List<OrderSummaryDto>>, GetOrderSummariesQueryHandler>(); // this registers the GetOrderSummariesQueryHandler as a service that implements the IQueryHandler interface for handling GetOrderSummariesQuery and returning a list of OrderSummaryDto. This allows us to use dependency injection to inject the handler into our API endpoints or other services that need to handle these queries.
builder.Services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>(); // this registers the CreateOrderCommandValidator as a service that implements the IValidator interface for validating CreateOrderCommand. This allows us to use dependency injection to inject the validator into our API endpoints or other services that need to validate these commands. The validator will be used to ensure that the data in the CreateOrderCommand is valid before it is processed by the handler.
// builder.Services.AddSingleton<IEventPublisher, ConsoleEventPublisher>(); // this registers the ConsoleEventPublisher as a singleton service that implements the IEventPublisher interface for publishing events. This allows us to use dependency injection to inject the event publisher into our handlers or other services that need to publish events. The ConsoleEventPublisher will simply write the published events to the console, but in a real application, you might want to implement a more robust event publishing mechanism, such as using a message broker or an event bus. the reason for using AddSingleton is that we want to have a single instance of the ConsoleEventPublisher that can be shared across the entire application, since it does not maintain any state and can be safely used by multiple handlers or services without the need for multiple instances.
builder.Services.AddSingleton<IEventPublisher, InProcessEventPublisher>(); // this registers the InProcessEventPublisher as a singleton service that implements the IEventPublisher interface for publishing events. This allows us to use dependency injection to inject the event publisher into our handlers or other services that need to publish events. The InProcessEventPublisher will resolve and invoke all registered event handlers for a given event type within the same process, allowing for a simple in-memory event handling mechanism. The reason for using AddSingleton is that we want to have a single instance of the InProcessEventPublisher that can be shared across the entire application, since it does not maintain any state and can be safely used by multiple handlers or services without the need for multiple instances.
builder.Services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedProjectionHandler>(); // this registers the OrderCreatedProjectionHandler as a scoped service that implements the IEventHandler interface for handling OrderCreatedEvent. This allows us to use dependency injection to inject the event handler into our event publisher or other services that need to handle these events. The OrderCreatedProjectionHandler will be responsible for updating the read model (projections) when an OrderCreatedEvent is published, ensuring that the read database is kept in sync with the write database. The reason for using AddScoped is that we want to have a new instance of the OrderCreatedProjectionHandler for each request, since it may depend on scoped services such as the ReadDbContext, and we want to ensure that each request gets its own instance of the handler and its dependencies, which are disposed of properly at the end of the request.

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();

app.MapPost("/api/orders", async (ICommandHandler<CreateOrderCommand, OrderDto> handler, CreateOrderCommand command) =>
{
    // app.MapPost("/api/orders", async (AppDbContext context, CreateOrderCommand command) =>

    // await context.Orders.AddAsync(order);  // This is the old way of doing it without the handler. We will replace this with the handler below. secondly, we will replace the order object with the command object that we will pass to the handler.
    // await context.SaveChangesAsync();

    // var order = await CreateOrderCommandHandler.Handle(command, context); // This is the new way of doing it with the handler. We are calling the handler and passing in the command and the context.

    try
    {
        var order = await handler.HandleAsync(command); // This is the new way of doing it with the handler. We are calling the handler and passing in the command. The handler will take care of getting the context from the dependency injection container and using it to handle the command.

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

app.MapGet("/api/orders/{id}", async (IQueryHandler<GetOrderByIdQuery, OrderDto> handler, int id) =>
{
    // app.MapGet("/api/orders/{id}", async (AppDbContext context, int id)
    // var order = await context.Orders.FindAsync(id); // This is the old way of doing it without the handler. We will replace this with the handler below.

    // var order = await GetOrderByIdQueryHandler.Handle(new GetOrderByIdQuery(id), context); // This is the new way of doing it with the handler. We are calling the handler and passing in the query and the context.

    var order = await handler.HandleAsync(new GetOrderByIdQuery(id)); // This is the new way of doing it with the handler. We are calling the handler and passing in the query. The handler will take care of getting the context from the dependency injection container and using it to handle the query.

    if (order == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(order);
});

app.MapGet("/api/orders/summaries", async (IQueryHandler<GetOrderSummariesQuery, List<OrderSummaryDto>> handler) =>
{
    var summaries = await handler.HandleAsync(new GetOrderSummariesQuery());
    return Results.Ok(summaries);
});

app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
