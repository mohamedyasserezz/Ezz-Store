using Ezz_Store.BLL.Models.Admin;
using Ezz_Store.BLL.Services.Admin;
using Ezz_Store.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ezz_Store.PL.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController(IAdminProductService productService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = await productService.GetAllAsync();
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(new ProductFormVM
        {
            CategoryOptions = (await productService.GetCategoryOptionsAsync())
                .Select(c => new CategoryOptionVM { Id = c.Id, Name = c.Name })
                .ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormVM model)
    {
        if (!ModelState.IsValid)
        {
            model.CategoryOptions = (await productService.GetCategoryOptionsAsync())
                .Select(c => new CategoryOptionVM { Id = c.Id, Name = c.Name })
                .ToList();
            return View(model);
        }

        await productService.CreateAsync(new ProductFormDto
        {
            Id = model.Id,
            Name = model.Name.Trim(),
            SKU = model.SKU.Trim(),
            Price = model.Price,
            StockQuantity = model.StockQuantity,
            CategoryId = model.CategoryId,
            IsActive = model.IsActive
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await productService.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        return View(new ProductFormVM
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CategoryOptions = (await productService.GetCategoryOptionsAsync())
                .Select(c => new CategoryOptionVM { Id = c.Id, Name = c.Name })
                .ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductFormVM model)
    {
        if (!ModelState.IsValid)
        {
            model.CategoryOptions = (await productService.GetCategoryOptionsAsync())
                .Select(c => new CategoryOptionVM { Id = c.Id, Name = c.Name })
                .ToList();
            return View(model);
        }

        var updated = await productService.UpdateAsync(new ProductFormDto
        {
            Id = model.Id,
            Name = model.Name,
            SKU = model.SKU,
            Price = model.Price,
            StockQuantity = model.StockQuantity,
            CategoryId = model.CategoryId,
            IsActive = model.IsActive
        });

        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await productService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}
