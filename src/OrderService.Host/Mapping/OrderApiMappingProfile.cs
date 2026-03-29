using AutoMapper;
using OrderService.Application.Dtos.Orders;
using OrderService.Application.ReadModels;
using OrderService.Host.Models.Responses;

namespace OrderService.Host.Mapping;

public sealed class OrderApiMappingProfile : Profile
{
    public OrderApiMappingProfile()
    {
        CreateMap<CreateOrderLineDto, OrderLineItemResponse>();

        CreateMap<CreateOrderResultDto, OrderDataResponse>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Code))
            .ForMember(d => d.Date, o => o.MapFrom(s => DateOnly.FromDateTime(s.OrderDate)))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAtUtc))
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.CreatedByName, o => o.MapFrom(s => s.CreatedByName))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.Lines));

        CreateMap<OrderSummaryReadModel, OrderSummaryResponse>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Code))
            .ForMember(d => d.Date, o => o.MapFrom(s => DateOnly.FromDateTime(s.OrderDate)))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAtUtc))
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.CreatedByName, o => o.MapFrom(s => s.CreatedByName));

        CreateMap<PagedOrdersReadModel, OrderListDataResponse>();

        CreateMap<CheckoutResultDto, CheckoutDataResponse>();
    }
}
