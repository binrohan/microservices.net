using System;
using AutoMapper;
using Ordering.Application.Features.Commands;
using Ordering.Application.Features.Commands.CheckoutOrder;
using Ordering.Application.Features.Commands.UpdateOrder;
using Ordering.Application.Features.Queries.GetOrdersList;
using Ordering.Domain.Entities;

namespace Ordering.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrdersDto>().ReverseMap();
        CreateMap<Order, CheckoutOrderCommand>().ReverseMap();
        CreateMap<Order, UpdateOrderCommand>().ReverseMap();
    }
}
