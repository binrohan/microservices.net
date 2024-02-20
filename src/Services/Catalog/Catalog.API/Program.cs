using Catalog.API.Data;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => { options.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.API", Version = "v1" }); });
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICatalogContext, CatalogContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));
}

//app.UseHttpsRedirection();

app.MapGet("/products", async ([FromServices] IProductRepository repo) => Results.Ok(await repo.GetProducts()))
.Produces<IEnumerable<Product>>(StatusCodes.Status200OK);

app.MapGet("/products/{id:length(24)}", async ([FromServices] IProductRepository repo,
                                               [FromServices] ILogger<Product> logger,
                                               [FromRoute] string id) =>
{
    var product = await repo.GetProduct(id);

    if (product is null)
    {
        logger.LogError("Product with id: {id}, not found.", id);
        return Results.NotFound();
    }

    return Results.Ok(product);
})
.Produces(StatusCodes.Status404NotFound)
.Produces<Product>(StatusCodes.Status200OK)
.WithName("GetProduct");

app.MapGet("/products/by-category/{category}", async ([FromServices] IProductRepository repo, [FromRoute] string category)
    => await repo.GetProductsByCategory(category))
.Produces<IEnumerable<Product>>(StatusCodes.Status200OK)
.WithName("GetProductsByCategory");

app.MapGet("/products/by-name/{name}", async ([FromServices] IProductRepository repo,
                                              [FromServices] ILogger<Product> logger,
                                              [FromRoute] string name) =>
{
    var product = await repo.GetProductByName(name);

    if (product is null)
    {
        logger.LogError("Product with id: {name}, not found.", name);
        return Results.NotFound();
    }

    return Results.Ok(product);
})
.Produces(StatusCodes.Status404NotFound)
.Produces<Product>(StatusCodes.Status200OK)
.WithName("GetProductByName");

app.MapPost("/products", async ([FromServices] IProductRepository repo, [FromBody] Product product) =>
{
    await repo.CreateProduct(product);

    return Results.CreatedAtRoute("GetProduct", new { id = product.Id }, product);
})
.Produces<Product>(StatusCodes.Status200OK);

app.MapPut("/products", async ([FromServices] IProductRepository repo, [FromBody] Product product)
    => Results.Ok(await repo.UpdateProduct(product)))
.Produces<bool>(StatusCodes.Status200OK);

app.MapDelete("/products/{id:length(24)}", async ([FromServices] IProductRepository repo, [FromRoute] string id)
    => Results.Ok(await repo.DeleteProduct(id)))
.Produces<bool>(StatusCodes.Status200OK)
.WithName("DeleteProduct");

app.Run();
