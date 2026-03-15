using Ezz_Store.BLL.Models.Orders;
using Ezz_Store.BLL.Services.Orders;
using Ezz_Store.DAL.Entites;
using Ezz_Store.PL.Helpers;
using Ezz_Store.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ezz_Store.PL.Controllers;

[Authorize]
public class OrdersController(
    IOrderService orderService,
    UserManager<ApplicationUser> userManager) : Controller
{
    [HttpGet("Orders/Checkout")]
    [HttpGet("Checkout/Index")]
    public async Task<IActionResult> Checkout()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Orders", new { area = "Admin" });
        }

        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        if (cart.Count == 0)
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        var userId = userManager.GetUserId(User)!;
        var checkoutPage = await orderService.GetCheckoutPageAsync(userId);

        return View(new CheckoutVM
        {
            Cart = new CartVM
            {
                Items = cart.Select(i => new CartItemVM
                {
                    ItemId = i.ItemId,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    AvailableStock = i.AvailableStock
                }).ToList()
            },
            AddressOptions = checkoutPage.AddressOptions.Select(a => new AddressOptionVM { Id = a.Id, Label = a.Label }).ToList()
        });
    }

    [HttpPost("Orders/Checkout")]
    [HttpPost("Checkout/Index")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutVM model)
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Orders", new { area = "Admin" });
        }

        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        if (cart.Count == 0)
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        var userId = userManager.GetUserId(User)!;

        if (!model.SelectedAddressId.HasValue)
        {
            if (string.IsNullOrWhiteSpace(model.Country))
            {
                ModelState.AddModelError(nameof(model.Country), "Country is required.");
            }

            if (string.IsNullOrWhiteSpace(model.City))
            {
                ModelState.AddModelError(nameof(model.City), "City is required.");
            }

            if (string.IsNullOrWhiteSpace(model.Street))
            {
                ModelState.AddModelError(nameof(model.Street), "Street is required.");
            }

            if (string.IsNullOrWhiteSpace(model.Zip))
            {
                ModelState.AddModelError(nameof(model.Zip), "Zip is required.");
            }
        }

        if (!ModelState.IsValid)
        {
            var invalidCheckoutPage = await orderService.GetCheckoutPageAsync(userId);
            model.Cart = new CartVM
            {
                Items = cart.Select(i => new CartItemVM
                {
                    ItemId = i.ItemId,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    AvailableStock = i.AvailableStock
                }).ToList()
            };
            model.AddressOptions = invalidCheckoutPage.AddressOptions.Select(a => new AddressOptionVM { Id = a.Id, Label = a.Label }).ToList();
            return View(model);
        }

        var request = new CheckoutRequestDto
        {
            SelectedAddressId = model.SelectedAddressId,
            Country = model.Country,
            City = model.City,
            Street = model.Street,
            Zip = model.Zip
        };

        var result = await orderService.CheckoutAsync(userId, request, cart);
        if (result.Success)
        {
            CartSessionHelper.Clear(HttpContext.Session);
            TempData["Success"] = $"Order {result.OrderNumber} created successfully.";
            return RedirectToAction(nameof(Details), new { id = result.OrderId });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        var checkoutPage = await orderService.GetCheckoutPageAsync(userId);
        model.Cart = new CartVM
        {
            Items = cart.Select(i => new CartItemVM
            {
                ItemId = i.ItemId,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                AvailableStock = i.AvailableStock
            }).ToList()
        };
        model.AddressOptions = checkoutPage.AddressOptions.Select(a => new AddressOptionVM { Id = a.Id, Label = a.Label }).ToList();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Orders", new { area = "Admin" });
        }

        var userId = userManager.GetUserId(User)!;
        var orders = await orderService.GetUserOrdersAsync(userId);

        return View(orders.Select(o => new OrderListItemVM
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            OrderDate = o.OrderDate,
            Status = o.Status,
            TotalAmount = o.TotalAmount
        }).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Orders", new { area = "Admin" });
        }

        var userId = userManager.GetUserId(User)!;
        var order = await orderService.GetUserOrderDetailsAsync(userId, id);

        if (order is null)
        {
            return NotFound();
        }

        return View(new OrderDetailsVM
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            AddressText = order.AddressText,
            Items = order.Items.Select(i => new OrderItemDetailsVM
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.LineTotal
            }).ToList()
        });
    }
}
