using System;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Commands.CheckoutOrder;

public class CheckoutOrderCommandHandler(IOrderRepository orderRepository,
                                         IMapper mapper,
                                         IEmailService emailService,
                                         ILogger<CheckoutOrderCommandHandler> logger)
    : IRequestHandler<CheckoutOrderCommand, int>
{
    private readonly IOrderRepository _orderRepository = orderRepository
        ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly IMapper _mapper = mapper
        ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IEmailService _emailService = emailService
        ?? throw new ArgumentNullException(nameof(emailService));
    private readonly ILogger<CheckoutOrderCommandHandler> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var orderEntity = _mapper.Map<Order>(request);
        var newOrder = await _orderRepository.AddAsync(orderEntity);

        _logger.LogInformation("Order {orderId} is successfully created.", newOrder.Id);

        await SendMail(newOrder);

        return newOrder.Id;
    }

    private async Task SendMail(Order order)
    {
        var email = new Email
        {
            To = "binrohan97@gmail.com",
            Subject = "Order was Created",
            Body = $"Order was created."
        };

        try
        {
            await _emailService.SendEmail(email);
        }
        catch (Exception e)
        {
            _logger.LogError("Order {orderId} failed due to an error with the mail service: {message}",
                             order.Id,
                             e.Message);
        }
    }
}
