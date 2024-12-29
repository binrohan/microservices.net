using System;
using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;

namespace Ordering.Application.Features.Queries.GetOrdersList;

public class GetOrdersListQueryHandler(IOrderRepository orderRepository,
                                       IMapper mapper) : IRequestHandler<GetOrdersListQuery, List<OrdersDto>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<List<OrdersDto>> Handle(GetOrdersListQuery request,
                                        CancellationToken cancellationToken)
    {
        var ordersList = await _orderRepository.GetOrdersByUserName(request.UserName);
        return _mapper.Map<List<OrdersDto>>(ordersList);
    }
}
