using Ezz_Store.BLL.Models.Cart;
using Ezz_Store.DAL.Entites;
using Ezz_Store.DAL.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Ezz_Store.BLL.Services.Cart;

public class CartService(IUnitOfWork unitOfWork) : ICartService
{
    public async Task<(bool Found, List<CartItemDto> Cart)> AddAsync(List<CartItemDto> cart, int productId, int quantity)
    {
        var product = await unitOfWork.GetRepository<Product>()
            .GetIQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);

        if (product is null)
        {
            return (false, cart);
        }

        var existing = cart.FirstOrDefault(i => i.ProductId == productId);

        var safeQuantity = Math.Max(1, quantity);

        if (existing is null)
        {
            cart.Add(new CartItemDto
            {
                ItemId = cart.Count == 0 ? 1 : cart.Max(x => x.ItemId) + 1,
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = Math.Min(safeQuantity, product.StockQuantity),
                AvailableStock = product.StockQuantity
            });
        }
        else
        {
            existing.Quantity = Math.Min(existing.Quantity + safeQuantity, product.StockQuantity);
            existing.AvailableStock = product.StockQuantity;
        }

        return (true, cart);
    }

    public List<CartItemDto> Update(List<CartItemDto> cart, int itemId, int qty)
    {
        var item = cart.FirstOrDefault(i => i.ItemId == itemId);
        if (item is null)
        {
            return cart;
        }

        if (qty <= 0)
        {
            cart.Remove(item);
        }
        else
        {
            item.Quantity = Math.Min(qty, Math.Max(1, item.AvailableStock));
        }

        return cart;
    }

    public List<CartItemDto> Remove(List<CartItemDto> cart, int itemId)
    {
        var item = cart.FirstOrDefault(i => i.ItemId == itemId);
        if (item is not null)
        {
            cart.Remove(item);
        }

        return cart;
    }
}
