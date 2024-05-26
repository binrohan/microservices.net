using Dapper;
using Discount.Grpc.Data;
using Discount.Grpc.Entities;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Discount.Grpc.Repositories;

public class DiscountRepository(IOptions<DbSettings> options) : IDiscountRepository
{
    private readonly DbSettings _dbSettings = options is null ? throw new ArgumentNullException(nameof(options))
                                                              : options.Value;

    public async Task<Coupon> Get(string productName)
    {
        using NpgsqlConnection connection = new(_dbSettings.ConnectionString);

        var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>
                     ("SELECT * FROM Coupon WHERE ProductName = @ProductName",
                      new { ProductName = productName });

        if (coupon is null) return new() { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };

        return coupon;
    }

    public async Task<bool> Create(Coupon coupon)
    {
        using NpgsqlConnection connection = new(_dbSettings.ConnectionString);

        var affected = await connection.ExecuteAsync
                       ("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                        new { coupon.ProductName, coupon.Description, coupon.Amount });

        return affected != 0;
    }

    public async Task<bool> Update(Coupon coupon)
    {
        using NpgsqlConnection connection = new(_dbSettings.ConnectionString);

        var affected = await connection.ExecuteAsync
                       ("UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                        new { coupon.ProductName, coupon.Description, coupon.Amount, coupon.Id });

        return affected != 0;
    }

    public async Task<bool> Delete(string productName)
    {
        using NpgsqlConnection connection = new(_dbSettings.ConnectionString);

        var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
                                                     new { ProductName = productName });

       return affected != 0;
    }
}
