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
                IsActive = p.IsActive,
                ImageUrl = p.ImageUrl
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
            CategoryId = product.CategoryId,
            ImageUrl = product.ImageUrl
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
            ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim(),
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
        if (!string.IsNullOrWhiteSpace(model.ImageUrl))
        {
            product.ImageUrl = model.ImageUrl.Trim();
        }

        unitOfWork.GetRepository<Product>().Update(product);
        await unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<DeleteOperationResult> DeleteAsync(int id)
    {
        var productRepo = unitOfWork.GetRepository<Product>();
        var orderItemRepo = unitOfWork.GetRepository<OrderItem>();

        var hasPendingOrders = await orderItemRepo
            .GetIQueryable()
            .AsNoTracking()
            .AnyAsync(oi => oi.ProductId == id && oi.Order.Status == Status.pending);

        if (hasPendingOrders)
        {
            return new DeleteOperationResult
            {
                Succeeded = false,
                Message = "Cannot delete this product because it is used by a pending order."
            };
        }

        var product = await productRepo
            .GetIQueryable()
            .Include(p => p.OrderItems)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return new DeleteOperationResult
            {
                Succeeded = false,
                Message = "Product was not found."
            };
        }

        if (product.OrderItems is not null && product.OrderItems.Any())
        {
            product.IsActive = false;
            productRepo.Update(product);

            await unitOfWork.CompleteAsync();
            return new DeleteOperationResult
            {
                Succeeded = true,
                Message = "Product is linked to previous orders, so it was deactivated instead of deleted."
            };
        }

        productRepo.Delete(product);
        await unitOfWork.CompleteAsync();

        return new DeleteOperationResult
        {
            Succeeded = true,
            Message = "Product deleted successfully."
        };
    }
}
