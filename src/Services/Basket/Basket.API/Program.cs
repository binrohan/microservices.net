using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/v1/basket", () => Results.Ok())
.Produces<IEnumerable<object>>(StatusCodes.Status200OK);
app.MapPost("/api/v1/basket", ([FromBody]object basket) =>  Results.Created());
app.MapDelete("/api/v1/basket/{username}", ([FromRoute]string username) =>  Results.Created());
app.MapPost("/api/v1/basket/checkout", () =>  Results.Ok());

app.Run();
