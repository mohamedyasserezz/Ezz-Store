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
    public async Task<IActionResult> Add(int productId)
    {
        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        var result = await cartService.AddAsync(cart, productId);
        if (!result.Found)
        {
            return NotFound();
        }

        CartSessionHelper.SaveCart(HttpContext.Session, result.Cart);
        TempData["Success"] = "Product added to cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Cart/Update")]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int itemId, int qty)
    {
        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        CartSessionHelper.SaveCart(HttpContext.Session, cartService.Update(cart, itemId, qty));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Cart/Remove")]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int itemId)
    {
        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        CartSessionHelper.SaveCart(HttpContext.Session, cartService.Remove(cart, itemId));

        return RedirectToAction(nameof(Index));
    }
}
