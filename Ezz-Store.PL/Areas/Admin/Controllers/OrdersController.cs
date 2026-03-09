using Ezz_Store.BLL.Models.Orders;
using Ezz_Store.BLL.Services.Admin;
using Ezz_Store.DAL.Entites;
using Ezz_Store.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ezz_Store.PL.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrdersController(IAdminOrderService adminOrderService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var orders = await adminOrderService.GetAllAsync();

        return View(orders.Select(o => new AdminOrderListVM
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            CustomerName = o.CustomerName,
            OrderDate = o.OrderDate,
            Status = o.Status,
            TotalAmount = o.TotalAmount
        }).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(UpdateOrderStatusVM model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        var updated = await adminOrderService.UpdateStatusAsync(new AdminUpdateOrderStatusDto
        {
            OrderId = model.OrderId,
            Status = model.Status
        });

        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}
