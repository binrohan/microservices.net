using System;
using MediatR;

namespace Ordering.Application.Features.Queries.GetOrdersList;

public class GetOrdersListQuery(string userName) : IRequest<List<OrdersDto>>
{
    public string UserName { get; set; } = userName ?? throw new ArgumentNullException(nameof(userName));
}
