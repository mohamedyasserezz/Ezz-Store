using Ezz_Store.BLL.Models.Orders;
using Ezz_Store.DAL.Entites;
using Ezz_Store.DAL.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Ezz_Store.BLL.Services.Admin;

public class AdminOrderService(IUnitOfWork unitOfWork) : IAdminOrderService
{
    public async Task<List<AdminOrderListItemDto>> GetAllAsync()
    {
        return await unitOfWork.GetRepository<Order>()
            .GetIQueryable()
            .AsNoTracking()
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new AdminOrderListItemDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.User.FullName,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(AdminUpdateOrderStatusDto model)
    {
        var order = await unitOfWork.GetRepository<Order>().GetAsync(model.OrderId);
        if (order is null)
        {
            return false;
        }

        order.Status = model.Status;
        unitOfWork.GetRepository<Order>().Update(order);
        await unitOfWork.CompleteAsync();
        return true;
    }
}
