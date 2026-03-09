using Ezz_Store.BLL.Models.Catalog;
using Ezz_Store.DAL.Entites;
using Ezz_Store.DAL.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Ezz_Store.BLL.Services.Catalog;

public class CatalogService(IUnitOfWork unitOfWork) : ICatalogService
{
    private const int PageSize = 8;

    public async Task<CatalogResultDto> GetCatalogAsync(int? categoryId, string? query, string? sort, int page)
    {
        if (page < 1) page = 1;

        var productsQuery = unitOfWork
            .GetRepository<Product>()
            .GetIQueryable()
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive);

        if (categoryId.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.Trim().ToLower();
            productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(q) || p.SKU.ToLower().Contains(q));
        }

        productsQuery = sort switch
        {
            "price_desc" => productsQuery.OrderByDescending(p => p.Price),
            "name" => productsQuery.OrderBy(p => p.Name),
            "name_desc" => productsQuery.OrderByDescending(p => p.Name),
            _ => productsQuery.OrderBy(p => p.Price)
        };

        var totalCount = await productsQuery.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
        if (page > totalPages) page = totalPages;

        var items = await productsQuery
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(p => new ProductCardDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name,
                StockQuantity = p.StockQuantity
            })
            .ToListAsync();

        var categories = await unitOfWork
            .GetRepository<Category>()
            .GetIQueryable()
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryOptionDto { Id = c.Id, Name = c.Name })
            .ToListAsync();

        return new CatalogResultDto
        {
            Items = items,
            Categories = categories,
            Page = page,
            TotalPages = totalPages
        };
    }

    public async Task<ProductDetailsDto?> GetDetailsAsync(int id)
    {
        var product = await unitOfWork
            .GetRepository<Product>()
            .GetIQueryable()
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (product is null)
        {
            return null;
        }

        return new ProductDetailsDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryName = product.Category.Name,
            IsActive = product.IsActive
        };
    }
}
