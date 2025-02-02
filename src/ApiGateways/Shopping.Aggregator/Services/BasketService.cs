using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services;

public class BasketService(HttpClient client) : IBasketService
{
    private readonly HttpClient _client = client ?? throw new ArgumentNullException(nameof(client));

    public async Task<BasketModel> GetBasket(string userName)
    {
        var response = await _client.GetAsync($"/api/v1/Basket/{userName}");
        return await response.ReadContentAs<BasketModel>() ?? new BasketModel { UserName = userName };
    }
}
