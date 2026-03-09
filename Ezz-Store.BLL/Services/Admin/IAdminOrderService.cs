using Ezz_Store.BLL.Models.Orders;

namespace Ezz_Store.BLL.Services.Admin;

public interface IAdminOrderService
{
    Task<List<AdminOrderListItemDto>> GetAllAsync();
    Task<bool> UpdateStatusAsync(AdminUpdateOrderStatusDto model);
}
