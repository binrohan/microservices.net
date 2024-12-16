using Basket.API.Entities;
using Basket.API.Repositories;
using Basket.API.GrpcServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Discount.Grpc.Protos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => { options.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket.API", Version = "v1" }); });
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString"); });
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options => options.Address = new Uri(builder.Configuration.GetValue<string>("GrpcSettings:DiscountUrl")!));
builder.Services.AddScoped<DiscountGrpcService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/v1/basket/{username}", async ([FromServices] IBasketRepository repo, [FromRoute] string username) =>
{
    var basket = await repo.GetBasket(username);
    return Results.Ok(basket ?? new ShoppingCart(username));
})
.Produces<ShoppingCart>(StatusCodes.Status200OK)
.WithName("GetBasket");

app.MapPost("/api/v1/basket", async ([FromServices] IBasketRepository repo,
                                     [FromServices] DiscountGrpcService discountGrpService,
                                     [FromBody] ShoppingCart basket) => 
{
    foreach (var item in basket.Items)
    {
        var coupon = await discountGrpService.GetDiscountAsync(item.ProductName);
        item.Price -= coupon.Amount;
    }

    return Results.Ok(await repo.UpdateBasket(basket));
})
.Produces<ShoppingCart>(StatusCodes.Status200OK);

app.MapDelete("/api/v1/basket/{username}", async ([FromServices] IBasketRepository repo, [FromRoute] string username) =>
{
    await repo.DeleteBasket(username);
    return Results.Ok();
})
.Produces(StatusCodes.Status200OK)
.WithName("DeleteBasket");

app.Run();
