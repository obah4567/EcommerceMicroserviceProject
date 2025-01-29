﻿using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.Convention;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController(IOrder orderInterface, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await orderInterface.GetAllAsync();
            if (!orders.Any())
            {
                return NotFound("No order detected in the database");
            }

            var (_, list) = OrderConversion.FromEntity(null, orders);

            return !list!.Any() ? NotFound() : Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrderById(int id)
        {
            var order = await orderInterface.FindByIdAsync(id);
            if (order is null)
            {
                return NotFound(null);
            }
            var (_order, _) = OrderConversion.FromEntity(order, null);
            return Ok(order);
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientId)
        {
            if (clientId <= 0)
            {
                return BadRequest("Invalid data provided");
            }

            var orders = await orderService.GetOrdersByClientId(clientId);
            return !orders.Any() ? NotFound(null) : Ok(orders);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0)
            {
                return BadRequest("Invalid data provided");
            }

            var orderDetail = await orderService.GetOrderDetails(orderId);
            return orderDetail.OrderId > 0 ? Ok(orderDetail) : NotFound("No order found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDTO)
        {
            // Check model state if all data annotations are passed.
            if (!ModelState.IsValid)
            {
                return BadRequest("Incomplete data submitted");
            }

            // convert to entity
            var getEntity = OrderConversion.ToEntity(orderDTO);
            var response = await orderInterface.CreateAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);

        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO orderDTO)
        {
            // Convert from DTO to entity
            var order = OrderConversion.ToEntity(orderDTO);
            var response = await orderInterface.UpdateAsync(order);

            return response.Flag ? Ok(response) : BadRequest(response);

        }

        [HttpDelete()]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDTO order)
        {
            // Convert from DTO to entity
            var deleteOrder = OrderConversion.ToEntity(order);
            var response = await orderInterface.DeleteAsync(deleteOrder);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

    }
}
