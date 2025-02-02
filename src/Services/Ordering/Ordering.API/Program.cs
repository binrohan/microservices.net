using System.Reflection;
using EventBus.Messages.Common;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.EvenBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Application.Features.Commands.CheckoutOrder;
using Ordering.Application.Features.Commands.DeleteOrder;
using Ordering.Application.Features.Commands.UpdateOrder;
using Ordering.Application.Features.Queries.GetOrdersList;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<BasketCheckoutConsumer>();
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BasketCheckoutConsumer>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, ep =>
        {
            ep.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
        });
    });
});

var app = builder.Build();
app.MigrateDatabase<OrderContext>((context, services) =>
{
    var logger = services.GetRequiredService<ILogger<OrderContextSeed>>();
    OrderContextSeed.SeedAsync(context, logger).Wait();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var api = app.MapGroup("api/v1/order");

api.MapGet("{userName}", async ([FromRoute] string userName, [FromServices] IMediator mediator)
    => Results.Ok(await mediator.Send(new GetOrdersListQuery(userName))))
   .WithName("GetOrder")
   .Produces<IEnumerable<OrdersDto>>(StatusCodes.Status200OK);

api.MapPost("/", async ([FromBody] CheckoutOrderCommand command, [FromServices] IMediator mediator) =>
{
    _ = mediator ?? throw new ArgumentNullException(nameof(mediator));
    return Results.Ok(await mediator.Send(command));
})
.WithName("CheckoutOrder")
.Produces<int>(StatusCodes.Status200OK);

api.MapPut("/", async ([FromBody] UpdateOrderCommand command, [FromServices] IMediator mediator) =>
{
    _ = mediator ?? throw new ArgumentNullException(nameof(mediator));
    await mediator.Send(command);
    return Results.NoContent();
})
.WithName("UpdateOrder")
.WithDescription("Testing Only, Mainly it will be call from GRPC.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

api.MapDelete("{id}", async ([FromRoute] int id, [FromServices] IMediator mediator) =>
{
    var command = new DeleteOrderCommand() { Id = id };
    _ = mediator ?? throw new ArgumentNullException(nameof(mediator));
    await mediator.Send(command);
    return Results.NoContent();
})
.WithName("DeleteOrder")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
