using Ezz_Store.BLL.Services.Catalog;
using Ezz_Store.PL.Helpers;
using Ezz_Store.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ezz_Store.PL.Controllers;

public class CatalogController(ICatalogService catalogService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int? categoryId, string? q, string? sort, int page = 1)
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Products", new { area = "Admin" });
        }

        var result = await catalogService.GetCatalogAsync(categoryId, q, sort, page);

        return View(new ProductListVM
        {
            Items = result.Items.Select(p => new ProductCardVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.CategoryName,
                StockQuantity = p.StockQuantity,
                ImageUrl = ProductImageHelper.GetDisplayImageUrl(p.ImageUrl, p.Id)
            }).ToList(),
            Categories = result.Categories.Select(c => new CategoryOptionVM { Id = c.Id, Name = c.Name }).ToList(),
            CategoryId = categoryId,
            Query = q,
            Sort = sort,
            Page = result.Page,
            TotalPages = result.TotalPages
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Products", new { area = "Admin" });
        }

        var product = await catalogService.GetDetailsAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        return View(new ProductDetailsVM
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryName = product.CategoryName,
            IsActive = product.IsActive,
            ImageUrl = ProductImageHelper.GetDisplayImageUrl(product.ImageUrl, product.Id)
        });
    }
}
