using System;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Expections;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Commands.UpdateOrder;

public class UpdateOrderCommandHandler(IOrderRepository orderRepository,
                                       IMapper mapper,
                                       ILogger<UpdateOrderCommandHandler> logger) : IRequestHandler<UpdateOrderCommand>
{
    private readonly IOrderRepository _orderRepository = orderRepository
        ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly IMapper _mapper = mapper
        ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly ILogger<UpdateOrderCommandHandler> _logger = logger
        ?? throw new ArgumentNullException(nameof(orderRepository));

    public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _orderRepository.GetByIdAsync(request.Id);
        if (orderToUpdate is null)
        {
            _logger.LogError("Order not exist on database.");
            throw new NotFoundException(nameof(Order), request.Id);
        }

        _mapper.Map(request, orderToUpdate, typeof(UpdateOrderCommand), typeof(Order));
        // _mapper.Map(request, orderToUpdate);

        await _orderRepository.UpdateAsync(orderToUpdate);

        _logger.LogInformation("Order {orderId} is successfully updated.", orderToUpdate.Id);
    }
}
