using Ezz_Store.DAL.Entites;
using Ezz_Store.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ezz_Store.PL.Controllers;

[Authorize]
public class AccountController(UserManager<ApplicationUser> userManager) : Controller
{
    private IActionResult? BlockAdminAccess()
    {
        if (!User.IsInRole("Admin"))
            return null;

        TempData["Error"] = "Admin accounts cannot be modified from this page.";
        return RedirectToAction("Index", "Products", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (BlockAdminAccess() is IActionResult blockedResult)
            return blockedResult;

        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return RedirectToAction("Index", "Catalog");

        return View(new ProfileVM
        {
            FullName = user.FullName ?? "",
            Email = user.Email ?? "",
            PhoneNumber = user.PhoneNumber
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        if (BlockAdminAccess() is IActionResult blockedResult)
            return blockedResult;

        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return RedirectToAction("Index", "Catalog");

        return View(new ProfileVM
        {
            FullName = user.FullName ?? "",
            Email = user.Email ?? "",
            PhoneNumber = user.PhoneNumber
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProfileVM model)
    {
        if (BlockAdminAccess() is IActionResult blockedResult)
            return blockedResult;

        if (!ModelState.IsValid)
            return View(model);

        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return RedirectToAction("Index", "Catalog");

        user.FullName = model.FullName.Trim();
        user.Email = model.Email.Trim();
        user.UserName = model.Email.Trim();
        user.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Trim();

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        await userManager.UpdateSecurityStampAsync(user);

        TempData["Success"] = "Your profile has been updated.";
        return RedirectToAction(nameof(Index));
    }
}
