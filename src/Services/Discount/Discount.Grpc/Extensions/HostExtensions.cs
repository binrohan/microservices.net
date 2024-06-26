﻿using Npgsql;
using Microsoft.Extensions.Options;
using Discount.Grpc.Data;

namespace Discount.Grpc.Extensions;

public static class HostExtensions
{
    public static IHost SeedDatabase<TContext>(this IHost host, int retry = 0)
    {
        using var scope = host.Services.CreateScope();
        
        var services = scope.ServiceProvider;
        var dbSettings = services.GetRequiredService<IOptions<DbSettings>>().Value;
        var logger = services.GetRequiredService<ILogger<TContext>>();

        try
        {
            logger.LogInformation("Migrating postresql database");

            using var connection = new NpgsqlConnection(dbSettings.ConnectionString);
            connection.Open();

            using var command = new NpgsqlCommand()
            {
                Connection = connection
            };

            command.CommandText = "DROP TABLE IF EXISTS Coupon";
            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY,
                                                        ProductName VARCHAR(24) NOT NULL,
                                                        Description TEXT,
                                                        Amount INT)";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
            command.ExecuteNonQuery();

            logger.LogInformation("Migrated postresql database.");
        }
        catch (NpgsqlException ex)
        {
            logger.LogError(ex, "An error occurred while migrating the postresql database");

            if (retry < 50)
            {
                Thread.Sleep(2000);
                SeedDatabase<TContext>(host, ++retry);
            }
        }


        return host;
    }
}
