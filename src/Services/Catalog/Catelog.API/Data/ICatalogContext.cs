using Catelog.API.Entities;
using MongoDB.Driver;

namespace Catelog.API.Data;

public interface ICatalogContext
{
    IMongoCollection<Product> Products { get; }
}
