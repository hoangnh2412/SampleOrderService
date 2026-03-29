using AutoMapper;
using OrderService.Application.Dtos.Orders;
using OrderService.Domain.Entities;

namespace OrderService.Application.Mapping;

public sealed class OrderDomainMappingProfile : Profile
{
    public OrderDomainMappingProfile()
    {
        CreateMap<OrderDetail, CreateOrderLineDto>()
            .ForMember(d => d.LineId, o => o.MapFrom(s => s.Id));

        CreateMap<Order, CreateOrderResultDto>()
            .ForMember(d => d.Lines, o => o.MapFrom(s => s.Details));
    }
}
