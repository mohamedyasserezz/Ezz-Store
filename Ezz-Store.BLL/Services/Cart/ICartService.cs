using Ezz_Store.BLL.Models.Cart;

namespace Ezz_Store.BLL.Services.Cart;

public interface ICartService
{
    Task<(bool Found, List<CartItemDto> Cart)> AddAsync(List<CartItemDto> cart, int productId, int quantity);
    List<CartItemDto> Update(List<CartItemDto> cart, int itemId, int qty);
    List<CartItemDto> Remove(List<CartItemDto> cart, int itemId);
}
