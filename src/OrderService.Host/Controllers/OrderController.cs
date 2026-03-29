using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.Orders;
using OrderService.Application.Dtos.Orders;
using OrderService.Application.Interfaces;
using OrderService.Application.Queries.Orders;
using OrderService.Application.ReadModels;
using OrderService.Domain.Entities;
using OrderService.Host.Models;
using OrderService.Host.Models.Requests;
using OrderService.Host.Models.Responses;

namespace OrderService.Host.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/orders")]
public sealed class OrderController(
    IMapper mapper,
    ICommandHandler<CreateOrderCommand, CreateOrderResultDto> createOrder,
    IQueryHandler<SearchOrdersPagedQuery, PagedOrdersReadModel> searchOrders,
    ICommandHandler<CheckoutOrderCommand, CheckoutResultDto> checkoutOrder) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseEnvelop<OrderDataResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderRequest request,
        [FromHeader(Name = "IdempotentId")]
        [Required(ErrorMessage = "Header IdempotentId là bắt buộc.")]
        string idempotentId,
        CancellationToken cancellationToken)
    {
        var lines = request.Items.Select(i => new OrderLineSpec(
            i.ProductId,
            i.ProductName,
            i.UnitPrice,
            i.Quantity,
            i.Amount,
            i.DiscountAmount,
            i.PaymentAmount)).ToList();

        var command = new CreateOrderCommand(
            request.Date,
            request.TotalAmount,
            request.TotalDiscountAmount,
            request.TotalPaymentAmount,
            lines,
            idempotentId.Trim());

        var result = await createOrder.HandleAsync(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, mapper.Map<OrderDataResponse>(result));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponseEnvelop<OrderListDataResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchOrders(
        [FromQuery] string? name,
        [FromQuery] int page = 0,
        [FromQuery] int size = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchOrdersPagedQuery(name, page, size);
        var result = await searchOrders.HandleAsync(query, cancellationToken);
        return Ok(mapper.Map<OrderListDataResponse>(result));
    }

    [HttpPost("{id:guid}/checkout")]
    [ProducesResponseType(typeof(ResponseEnvelop<CheckoutDataResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Checkout(
        [FromRoute] Guid id,
        [FromBody] CheckoutRequest request,
        [FromHeader(Name = "IdempotentId")]
        [Required(ErrorMessage = "Header IdempotentId là bắt buộc.")]
        string idempotentId,
        CancellationToken cancellationToken)
    {
        var command = new CheckoutOrderCommand(
            id,
            request.PaymentServiceId,
            request.PaymentMethod,
            idempotentId.Trim());

        var result = await checkoutOrder.HandleAsync(command, cancellationToken);
        return Ok(mapper.Map<CheckoutDataResponse>(result));
    }

}
