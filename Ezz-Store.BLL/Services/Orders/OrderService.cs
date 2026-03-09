using Ezz_Store.BLL.Models.Cart;
using Ezz_Store.BLL.Models.Orders;
using Ezz_Store.DAL.Entites;
using Ezz_Store.DAL.Persistance;
using Ezz_Store.DAL.Persistance.Data;
using Microsoft.EntityFrameworkCore;

namespace Ezz_Store.BLL.Services.Orders;

public class OrderService(IUnitOfWork unitOfWork, ApplicationDbContext dbContext) : IOrderService
{
    public async Task<CheckoutPageDto> GetCheckoutPageAsync(string userId)
    {
        var addresses = await unitOfWork.GetRepository<Address>()
            .GetIQueryable()
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.Country)
            .ToListAsync();

        return new CheckoutPageDto
        {
            AddressOptions = addresses.Select(a => new AddressOptionDto
            {
                Id = a.Id,
                Label = $"{a.Country}, {a.City}, {a.Street}, {a.Zip}"
            }).ToList()
        };
    }

    public async Task<CheckoutResultDto> CheckoutAsync(string userId, CheckoutRequestDto request, List<CartItemDto> cart)
    {
        var result = new CheckoutResultDto();

        if (cart.Count == 0)
        {
            result.Errors.Add("Your cart is empty.");
            return result;
        }

        var dbProducts = await unitOfWork.GetRepository<Product>()
            .GetIQueryable()
            .Where(p => cart.Select(c => c.ProductId).Contains(p.Id))
            .ToListAsync();

        foreach (var item in cart)
        {
            var product = dbProducts.FirstOrDefault(p => p.Id == item.ProductId);
            if (product is null || !product.IsActive)
            {
                result.Errors.Add($"Product '{item.ProductName}' is unavailable.");
                continue;
            }

            if (product.StockQuantity < item.Quantity)
            {
                result.Errors.Add($"Not enough stock for '{product.Name}'.");
            }
        }

        Address? shippingAddress = null;

        if (request.SelectedAddressId.HasValue)
        {
            shippingAddress = await unitOfWork.GetRepository<Address>()
                .GetIQueryable()
                .FirstOrDefaultAsync(a => a.Id == request.SelectedAddressId && a.UserId == userId);

            if (shippingAddress is null)
            {
                result.Errors.Add("Selected address is invalid.");
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.Country) ||
                string.IsNullOrWhiteSpace(request.City) ||
                string.IsNullOrWhiteSpace(request.Street) ||
                string.IsNullOrWhiteSpace(request.Zip))
            {
                result.Errors.Add("Please provide shipping address details.");
            }
        }

        if (result.Errors.Count > 0)
        {
            return result;
        }

        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            if (shippingAddress is null)
            {
                shippingAddress = new Address
                {
                    UserId = userId,
                    Country = request.Country!.Trim(),
                    City = request.City!.Trim(),
                    Street = request.Street!.Trim(),
                    Zip = request.Zip!.Trim()
                };
                unitOfWork.GetRepository<Address>().Add(shippingAddress);
                await unitOfWork.CompleteAsync();
            }

            var order = new Order
            {
                UserId = userId,
                ShippingAddressId = shippingAddress.Id,
                OrderNumber = $"EZ-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}",
                Status = Status.pending,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 0m,
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0;
            foreach (var item in cart)
            {
                var product = dbProducts.First(p => p.Id == item.ProductId);
                var lineTotal = product.Price * item.Quantity;
                total += lineTotal;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    LineTotal = lineTotal
                });

                product.StockQuantity -= item.Quantity;
                unitOfWork.GetRepository<Product>().Update(product);
            }

            order.TotalAmount = total;
            unitOfWork.GetRepository<Order>().Add(order);
            await unitOfWork.CompleteAsync();
            await tx.CommitAsync();

            result.Success = true;
            result.OrderId = order.Id;
            result.OrderNumber = order.OrderNumber;
            return result;
        }
        catch
        {
            await tx.RollbackAsync();
            result.Errors.Add("Checkout failed. Please try again.");
            return result;
        }
    }

    public async Task<List<CustomerOrderListItemDto>> GetUserOrdersAsync(string userId)
    {
        return await unitOfWork.GetRepository<Order>()
            .GetIQueryable()
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new CustomerOrderListItemDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount
            })
            .ToListAsync();
    }

    public async Task<CustomerOrderDetailsDto?> GetUserOrderDetailsAsync(string userId, int orderId)
    {
        var order = await unitOfWork.GetRepository<Order>()
            .GetIQueryable()
            .AsNoTracking()
            .Include(o => o.ShippingAddress)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order is null)
        {
            return null;
        }

        return new CustomerOrderDetailsDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            AddressText = $"{order.ShippingAddress.Country}, {order.ShippingAddress.City}, {order.ShippingAddress.Street}, {order.ShippingAddress.Zip}",
            Items = order.OrderItems.Select(i => new CustomerOrderItemDto
            {
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.LineTotal
            }).ToList()
        };
    }
}
