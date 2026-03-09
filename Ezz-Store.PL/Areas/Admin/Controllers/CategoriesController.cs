using Ezz_Store.BLL.Models.Admin;
using Ezz_Store.BLL.Services.Admin;
using Ezz_Store.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ezz_Store.PL.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController(IAdminCategoryService categoryService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await categoryService.GetAllAsync();
        return View(categories);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CategoryFormVM
        {
            ParentOptions = (await categoryService.GetParentOptionsAsync())
                .Select(c => new CategoryOptionVM { Id = c.Id, Name = c.Name })
                .ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormVM model)
    {
        if (!ModelState.IsValid)
        {
            model.ParentOptions = (await categoryService.GetParentOptionsAsync())
                .Select(c => new CategoryOptionVM { Id = c.Id, Name = c.Name })
                .ToList();
            return View(model);
        }

        await categoryService.CreateAsync(new CategoryFormDto
        {
            Id = model.Id,
            Name = model.Name.Trim(),
            ParentCategoryId = model.ParentCategoryId
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await categoryService.GetByIdAsync(id);
        if (category is null)
        {
            return NotFound();
        }

        return View(new CategoryFormVM
        {
            Id = category.Id,
            Name = category.Name,
            ParentCategoryId = category.ParentCategoryId,
            ParentOptions = (await categoryService.GetParentOptionsAsync(id))
                .Select(c => new CategoryOptionVM { Id = c.Id, Name = c.Name })
                .ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryFormVM model)
    {
        if (!ModelState.IsValid)
        {
            model.ParentOptions = (await categoryService.GetParentOptionsAsync(model.Id))
                .Select(c => new CategoryOptionVM { Id = c.Id, Name = c.Name })
                .ToList();
            return View(model);
        }

        var updated = await categoryService.UpdateAsync(new CategoryFormDto
        {
            Id = model.Id,
            Name = model.Name,
            ParentCategoryId = model.ParentCategoryId
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
        await categoryService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}
