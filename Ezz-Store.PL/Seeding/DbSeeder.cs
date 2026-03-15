using Ezz_Store.DAL.Entites;
using Ezz_Store.DAL.Persistance.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ezz_Store.PL.Seeding;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await dbContext.Database.MigrateAsync();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        const string adminEmail = "admin@ezzstore.local";
        const string adminPassword = "Admin123";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Store Administrator",
                EmailConfirmed = true
            };

            var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!createAdminResult.Succeeded)
            {
                var errors = string.Join(", ", createAdminResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to seed the admin user: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        var defaultCategories = new List<Category>
        {
            new() { Name = "Men's Watches" },
            new() { Name = "Women's Watches" },
            new() { Name = "Smart Watches" },
            new() { Name = "Luxury Watches" },
            new() { Name = "Sport Watches" }
        };

        if (!await dbContext.Categories.AnyAsync())
        {
            await dbContext.Categories.AddRangeAsync(defaultCategories);
            await dbContext.SaveChangesAsync();
        }

        var categoryNames = defaultCategories.Select(category => category.Name).ToList();
        var existingCategories = await dbContext.Categories
            .Where(category => categoryNames.Contains(category.Name))
            .ToListAsync();

        var missingCategories = categoryNames
            .Except(existingCategories.Select(category => category.Name), StringComparer.OrdinalIgnoreCase)
            .Select(name => new Category { Name = name })
            .ToList();

        if (missingCategories.Count > 0)
        {
            await dbContext.Categories.AddRangeAsync(missingCategories);
            await dbContext.SaveChangesAsync();
            existingCategories.AddRange(missingCategories);
        }

        if (await dbContext.Products.AnyAsync())
        {
            return;
        }

        var categoryByName = existingCategories.ToDictionary(category => category.Name, category => category.Id, StringComparer.OrdinalIgnoreCase);

        var productImages = new[]
        {
            "/Images/products/Men watch1.jpg",
            "/Images/products/Men watch2.jpg",
            "/Images/products/woment watch1.png",
            "/Images/products/women watch2.jpg",
            "/Images/products/smart watch1.jpg",
            "/Images/products/smart watch2.jpg",
            "/Images/products/Luxury watch1.jpg",
            "/Images/products/Luxury watch2.jpg",
            "/Images/products/sprort watch1.jpg",
            "/Images/products/sprort watch2.jpg"
        };

        var products = new List<Product>
        {
            new()
            {
                Name = "Chrono Classic Men's Watch",
                SKU = "MEN-1001",
                Price = 2499.00m,
                StockQuantity = 18,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Men's Watches"],
                ImageUrl = productImages[0]
            },
            new()
            {
                Name = "Midnight Steel Men's Watch",
                SKU = "MEN-1002",
                Price = 1899.00m,
                StockQuantity = 22,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Men's Watches"],
                ImageUrl = productImages[1]
            },
            new()
            {
                Name = "Rose Gold Women's Watch",
                SKU = "WOM-2001",
                Price = 2199.00m,
                StockQuantity = 16,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Women's Watches"],
                ImageUrl = productImages[2]
            },
            new()
            {
                Name = "Minimal Leather Women's Watch",
                SKU = "WOM-2002",
                Price = 1599.00m,
                StockQuantity = 24,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Women's Watches"],
                ImageUrl = productImages[3]
            },
            new()
            {
                Name = "Active Pro Smart Watch",
                SKU = "SMART-3001",
                Price = 3299.00m,
                StockQuantity = 20,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Smart Watches"],
                ImageUrl = productImages[4]
            },
            new()
            {
                Name = "Urban Fit Smart Watch",
                SKU = "SMART-3002",
                Price = 2899.00m,
                StockQuantity = 25,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Smart Watches"],
                ImageUrl = productImages[5]
            },
            new()
            {
                Name = "Heritage Automatic Luxury Watch",
                SKU = "LUX-4001",
                Price = 7499.00m,
                StockQuantity = 8,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Luxury Watches"],
                ImageUrl = productImages[6]
            },
            new()
            {
                Name = "Diamond Dial Luxury Watch",
                SKU = "LUX-4002",
                Price = 8999.00m,
                StockQuantity = 5,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Luxury Watches"],
                ImageUrl = productImages[7]
            },
            new()
            {
                Name = "Trail Runner Sport Watch",
                SKU = "SPORT-5001",
                Price = 1999.00m,
                StockQuantity = 30,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Sport Watches"],
                ImageUrl = productImages[8]
            },
            new()
            {
                Name = "Ocean Diver Sport Watch",
                SKU = "SPORT-5002",
                Price = 2799.00m,
                StockQuantity = 14,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Sport Watches"],
                ImageUrl = productImages[9]
            }
        };

        await dbContext.Products.AddRangeAsync(products);
        await dbContext.SaveChangesAsync();
    }
}
