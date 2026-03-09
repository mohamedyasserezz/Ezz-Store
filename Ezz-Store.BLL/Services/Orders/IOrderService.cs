using Ezz_Store.BLL.Models.Cart;
using Ezz_Store.BLL.Models.Orders;

namespace Ezz_Store.BLL.Services.Orders;

public interface IOrderService
{
    Task<CheckoutPageDto> GetCheckoutPageAsync(string userId);
    Task<CheckoutResultDto> CheckoutAsync(string userId, CheckoutRequestDto request, List<CartItemDto> cart);
    Task<List<CustomerOrderListItemDto>> GetUserOrdersAsync(string userId);
    Task<CustomerOrderDetailsDto?> GetUserOrderDetailsAsync(string userId, int orderId);
}
