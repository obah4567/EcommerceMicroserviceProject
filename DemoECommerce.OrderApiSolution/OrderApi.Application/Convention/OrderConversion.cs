using OrderApi.Application.DTOs;
using OrderApi.Domain.Entities;

namespace OrderApi.Application.Convention
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDTO orderDTO) => new Order
        {
            Id = orderDTO.Id,
            ClientId = orderDTO.ClientId,
            ProductId = orderDTO.ProductId,
            OrderDate = orderDTO.OrderDate,
            PurchaseQuantity = orderDTO.PurchaseQuantity
        };

        public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<Order>? orders)
        {
            // Return single
            if (order is not null || orders is null)
            {
                var singleOrder = new OrderDTO(
                    order!.Id,
                    order.ProductId,
                    order.ClientId,
                    order.PurchaseQuantity,
                    order.OrderDate);

                return (singleOrder, null);
            }

            // return list
            if (orders is not null || order is null)
            {
                var _orders = orders!.Select(o =>
                    new OrderDTO(o.Id,
                        o.ClientId,
                        o.ProductId,
                        o.PurchaseQuantity,
                        o.OrderDate)
                    );

                return (null, _orders);
            }

            return (null, null);
        }

    }
}
