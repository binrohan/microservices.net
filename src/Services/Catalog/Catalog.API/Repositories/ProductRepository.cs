using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class ProductRepository(ICatalogContext context) : IProductRepository
{
    private readonly ICatalogContext _context = context;

    public async Task CreateProduct(Product product)
    {
        await _context.Products.InsertOneAsync(product);
    }

    public async Task<bool> DeleteProduct(string id)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);

        var deleteResult = await _context.Products.DeleteOneAsync(filter);

        return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }

    public async Task<Product> GetProduct(string id)
    {
        return await _context.Products
                             .Find(p => p.Id == id)
                             .FirstOrDefaultAsync();
    }

    public async Task<Product> GetProductsByCategory(string category)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, category);

        return await _context.Products
                             .Find(filter)
                             .FirstOrDefaultAsync();
    }

    public async Task<Product> GetProductByName(string name)
    {
        FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Name, name);

        return await _context.Products
                             .Find(filter)
                             .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _context.Products.Find(p => true).ToListAsync();
    }

    public async Task<bool> UpdateProduct(Product product)
    {
        var updateResult = await _context.Products.ReplaceOneAsync(filter: p => p.Id == product.Id, replacement: product);

        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }
}
