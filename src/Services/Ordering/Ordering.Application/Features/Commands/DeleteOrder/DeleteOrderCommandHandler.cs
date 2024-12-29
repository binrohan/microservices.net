using System;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Expections;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Commands.DeleteOrder;

public class DeleteOrderCommandHandler(IOrderRepository orderRepository,
                                       IMapper mapper,
                                       ILogger<DeleteOrderCommandHandler> logger) : IRequestHandler<DeleteOrderCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository
        ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly IMapper _mapper = mapper
        ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ILogger<DeleteOrderCommandHandler> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var orderToDetele = await _orderRepository.GetByIdAsync(request.Id);
        if (orderToDetele is null)
        {
            _logger.LogError("Order not exist on database.");
            throw new NotFoundException(nameof(Order), request.Id);
        }

        await _orderRepository.DeleteAsync(orderToDetele);
        _logger.LogInformation("Order {orderId} is successfully deleted.", orderToDetele.Id);
    }
}
