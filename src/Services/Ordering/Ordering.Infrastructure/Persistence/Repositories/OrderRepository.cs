using System;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence.Repositories;

public class OrderRepository(OrderContext dbContext)
    : RepositoryBase<Order>(dbContext), IOrderRepository
{
    public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
        => await _dbContext.Orders.Where(o => o.UserName == userName)
                                  .ToListAsync();
}
