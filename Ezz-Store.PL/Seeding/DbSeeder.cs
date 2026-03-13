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
            new() { Name = "Electronics" },
            new() { Name = "Fashion" },
            new() { Name = "Home & Kitchen" },
            new() { Name = "Books" },
            new() { Name = "Accessories" }
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

        var products = new List<Product>
        {
            new()
            {
                Name = "Noise Cancelling Headphones",
                SKU = "ELEC-1001",
                Price = 2499.00m,
                StockQuantity = 18,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Electronics"]
            },
            new()
            {
                Name = "Mechanical Keyboard",
                SKU = "ELEC-1002",
                Price = 1799.00m,
                StockQuantity = 24,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Electronics"]
            },
            new()
            {
                Name = "Smart Watch",
                SKU = "ELEC-1003",
                Price = 3299.00m,
                StockQuantity = 12,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Electronics"]
            },
            new()
            {
                Name = "Classic Denim Jacket",
                SKU = "FASH-2001",
                Price = 1299.00m,
                StockQuantity = 20,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Fashion"]
            },
            new()
            {
                Name = "Cotton Crew T-Shirt",
                SKU = "FASH-2002",
                Price = 349.00m,
                StockQuantity = 50,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Fashion"]
            },
            new()
            {
                Name = "Ceramic Dinner Set",
                SKU = "HOME-3001",
                Price = 899.00m,
                StockQuantity = 16,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Home & Kitchen"]
            },
            new()
            {
                Name = "Stainless Steel Bottle",
                SKU = "HOME-3002",
                Price = 275.00m,
                StockQuantity = 40,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Home & Kitchen"]
            },
            new()
            {
                Name = "Clean Architecture",
                SKU = "BOOK-4001",
                Price = 650.00m,
                StockQuantity = 14,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Books"]
            },
            new()
            {
                Name = "Domain-Driven Design Distilled",
                SKU = "BOOK-4002",
                Price = 720.00m,
                StockQuantity = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Books"]
            },
            new()
            {
                Name = "Leather Card Holder",
                SKU = "ACCS-5001",
                Price = 415.00m,
                StockQuantity = 28,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categoryByName["Accessories"]
            }
        };

        await dbContext.Products.AddRangeAsync(products);
        await dbContext.SaveChangesAsync();
    }
}
