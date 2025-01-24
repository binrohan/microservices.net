using System;

namespace Shopping.Aggregator.Models;

public class ShoppingModel
{
    public required string UserName { get; set; }
    public BasketModel? BasketWithProducts { get; set; }
    public IEnumerable<OrderResponseModel> Orders { get; set; } = [];
}
