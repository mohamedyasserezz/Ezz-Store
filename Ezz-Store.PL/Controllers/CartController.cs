using Ezz_Store.BLL.Services.Cart;
using Ezz_Store.PL.Helpers;
using Ezz_Store.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ezz_Store.PL.Controllers;

public class CartController(ICartService cartService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Products", new { area = "Admin" });
        }

        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        return View(new CartVM
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
        });
    }

    [HttpPost("Cart/Add")]
    [HttpPost("Cart/Add/{productId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int qty = 1, string? returnUrl = null)
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Products", new { area = "Admin" });
        }

        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        var result = await cartService.AddAsync(cart, productId, qty);
        if (!result.Found)
        {
            return NotFound();
        }

        CartSessionHelper.SaveCart(HttpContext.Session, result.Cart);
        TempData["Success"] = "Product added to cart.";

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index), "Catalog");
    }

    [HttpPost("Cart/Update")]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int itemId, int qty)
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Products", new { area = "Admin" });
        }

        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        CartSessionHelper.SaveCart(HttpContext.Session, cartService.Update(cart, itemId, qty));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Cart/Remove")]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int itemId)
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Products", new { area = "Admin" });
        }

        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        CartSessionHelper.SaveCart(HttpContext.Session, cartService.Remove(cart, itemId));

        return RedirectToAction(nameof(Index));
    }
}
