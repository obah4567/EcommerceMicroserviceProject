﻿using OrderApi.Application.Convention;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface, HttpClient httpClient,
        ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {

        // GET PRODUCT
        public async Task<ProductDTO> GetProduct(int productId)
        {
            // Call product APi using HttpClient
            // Redirect this call to the API Gateway since product API is not response to outsiders.
            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
            {
                return null!;
            }
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }

        // GET USER
        public async Task<AppUserDTO> GetUser(int userId)
        {
            // Call user APi using HttpClient
            // Redirect this call to the API Gateway since product API is not response to outsiders.
            var getUser = await httpClient.GetAsync($"/api/products/{userId}");
            if (!getUser.IsSuccessStatusCode)
            {
                return null!;
            }
            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user!;
        }

        // GET ORDER DETAILS BY ID 
        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            // Prepare Order
            var order = await orderInterface.FindByIdAsync(orderId);
            if (order is null || order!.Id <= 0)
            {
                return null!;
            }
            //Get Retry pipeline
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");
            // Prepare product
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));
            // Prepare Client
            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ProductId));

            // Populate order Details
            return new OrderDetailsDTO
               (order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.TelephoneNumber,
                productDTO.Name,
                order.PurchaseQuantity,
                productDTO.Price,
                productDTO.Quantity * order.PurchaseQuantity,
                order.OrderDate);
        }

        // Get Orders by Client ID
        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            // Get all client's order
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
            if (!orders.Any())
            {
                return null!;
            }
            // Convert from DTO
            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;
        }
    }
}
