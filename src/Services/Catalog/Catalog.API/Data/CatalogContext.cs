using Catalog.API.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.API.Data;

public class CatalogContext : ICatalogContext
{
    public CatalogContext(IOptions<DbSettings> dbSettings)
    {
        var client = new MongoClient(dbSettings.Value.ConnectionString);
        var database = client.GetDatabase(dbSettings.Value.DatabaseName);

        Products = database.GetCollection<Product>(dbSettings.Value.CollectionName);
        CatalogContextSeed.SeedData(Products);
    }
    public IMongoCollection<Product> Products { get; }
}
