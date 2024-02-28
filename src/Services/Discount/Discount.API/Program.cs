using Discount.API.Data;
using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("DbSettings"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => { options.SwaggerDoc("v1", new OpenApiInfo { Title = "Discount.API", Version = "v1" }); });
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("api/v1/discount/{productName}", async ([FromServices] IDiscountRepository repo, [FromRoute] string productName) 
    => Results.Ok(await repo.Get(productName)))
.Produces<Coupon>(StatusCodes.Status200OK)
.WithName("GetDiscount");

app.MapPost("api/v1/discount", async ([FromServices] IDiscountRepository repo, [FromBody] Coupon coupon) => 
{
    await repo.Create(coupon);
    return Results.CreatedAtRoute("GetDiscount", new { productName =coupon.ProductName }, coupon);
})
.Produces<Coupon>(StatusCodes.Status201Created);

app.MapPut("api/v1/discount", async ([FromServices] IDiscountRepository repo, [FromBody] Coupon coupon) 
    => Results.Ok(await repo.Update(coupon)))
.Produces<bool>(StatusCodes.Status200OK);

app.MapDelete("api/v1/discount/{productName}", async ([FromServices] IDiscountRepository repo, [FromRoute] string productName) 
    => Results.Ok(await repo.Delete(productName)))
.Produces<bool>(StatusCodes.Status200OK)
.WithName("DeleteDiscount");

app.Run();

