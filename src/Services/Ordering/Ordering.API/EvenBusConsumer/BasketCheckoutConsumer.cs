using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Features.Commands.CheckoutOrder;

namespace Ordering.API.EvenBusConsumer;

public class BasketCheckoutConsumer(IMapper mapper, IMediator mediator, ILogger<BasketCheckoutConsumer> logger) : IConsumer<BasketCheckoutEvent>
{
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly ILogger<BasketCheckoutConsumer> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        CheckoutOrderCommand command = _mapper.Map<CheckoutOrderCommand>(context.Message);
        var result = await _mediator.Send(command);

        logger.LogInformation("BasketCheckoutEvent consumed successfully. Created Order Id: {OrderId}", result);
    }
}
