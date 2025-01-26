using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
                c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogBaseUrl"]!));

builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketBaseUrl"]!));

builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderBaseUrl"]!));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("api/v1/Shopping/{userName}", async (string userName,
                                                [FromServices] ICatalogService catalogService,
                                                [FromServices] IBasketService basketService,
                                                [FromServices] IOrderService orderService) =>
{
    var basket = await basketService.GetBasket(userName);

    foreach (var item in basket.Items)
    {
        var product = await catalogService.GetCatalog(item.ProductId);

        if (product is null) continue;

        item.Category = product.Category;
        item.Summary = product.Summary;
        item.Description = product.Description;
        item.ImageFile = product.ImageFile;
    }

    var orders = await orderService.GetOrdersByUserName(userName);

    ShoppingModel shoppingModel = new()
    {
        UserName = userName,
        BasketWithProducts = basket,
        Orders = orders
    };

    return Results.Ok(shoppingModel);
})
.WithName("GetShopping")
.Produces<ShoppingModel>(StatusCodes.Status200OK);

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
