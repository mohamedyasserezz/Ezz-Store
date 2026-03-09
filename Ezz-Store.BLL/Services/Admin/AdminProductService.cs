using Ezz_Store.BLL.Models.Admin;
using Ezz_Store.BLL.Models.Catalog;
using Ezz_Store.DAL.Entites;
using Ezz_Store.DAL.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Ezz_Store.BLL.Services.Admin;

public class AdminProductService(IUnitOfWork unitOfWork) : IAdminProductService
{
    public async Task<List<ProductListItemDto>> GetAllAsync()
    {
        return await unitOfWork.GetRepository<Product>()
            .GetIQueryable()
            .AsNoTracking()
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                CategoryName = p.Category.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                IsActive = p.IsActive
            })
            .ToListAsync();
    }

    public async Task<ProductFormDto?> GetByIdAsync(int id)
    {
        var product = await unitOfWork.GetRepository<Product>().GetAsync(id);
        if (product is null)
        {
            return null;
        }

        return new ProductFormDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId
        };
    }

    public async Task<List<CategoryOptionDto>> GetCategoryOptionsAsync()
    {
        return await unitOfWork.GetRepository<Category>()
            .GetIQueryable()
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryOptionDto { Id = c.Id, Name = c.Name })
            .ToListAsync();
    }

    public async Task CreateAsync(ProductFormDto model)
    {
        unitOfWork.GetRepository<Product>().Add(new Product
        {
            Name = model.Name.Trim(),
            SKU = model.SKU.Trim(),
            Price = model.Price,
            StockQuantity = model.StockQuantity,
            CategoryId = model.CategoryId,
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        });

        await unitOfWork.CompleteAsync();
    }

    public async Task<bool> UpdateAsync(ProductFormDto model)
    {
        var product = await unitOfWork.GetRepository<Product>().GetAsync(model.Id);
        if (product is null)
        {
            return false;
        }

        product.Name = model.Name.Trim();
        product.SKU = model.SKU.Trim();
        product.Price = model.Price;
        product.StockQuantity = model.StockQuantity;
        product.CategoryId = model.CategoryId;
        product.IsActive = model.IsActive;

        unitOfWork.GetRepository<Product>().Update(product);
        await unitOfWork.CompleteAsync();
        return true;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await unitOfWork.GetRepository<Product>().GetAsync(id);
        if (product is null)
        {
            return;
        }

        unitOfWork.GetRepository<Product>().Delete(product);
        await unitOfWork.CompleteAsync();
    }
}
