using Ezz_Store.BLL.Models.Admin;
using Ezz_Store.BLL.Models.Catalog;
using Ezz_Store.DAL.Entites;
using Ezz_Store.DAL.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Ezz_Store.BLL.Services.Admin;

public class AdminCategoryService(IUnitOfWork unitOfWork) : IAdminCategoryService
{
    public async Task<List<CategoryListItemDto>> GetAllAsync()
    {
        return await unitOfWork.GetRepository<Category>()
            .GetIQueryable()
            .AsNoTracking()
            .Include(c => c.Parent)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryListItemDto
            {
                Id = c.Id,
                Name = c.Name,
                ParentName = c.Parent != null ? c.Parent.Name : null
            })
            .ToListAsync();
    }

    public async Task<CategoryFormDto?> GetByIdAsync(int id)
    {
        var category = await unitOfWork.GetRepository<Category>().GetAsync(id);
        if (category is null)
        {
            return null;
        }

        return new CategoryFormDto
        {
            Id = category.Id,
            Name = category.Name,
            ParentCategoryId = category.ParentCategoryId
        };
    }

    public async Task<List<CategoryOptionDto>> GetParentOptionsAsync(int? currentCategoryId = null)
    {
        return await unitOfWork.GetRepository<Category>()
            .GetIQueryable()
            .AsNoTracking()
            .Where(c => !currentCategoryId.HasValue || c.Id != currentCategoryId.Value)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryOptionDto { Id = c.Id, Name = c.Name })
            .ToListAsync();
    }

    public async Task CreateAsync(CategoryFormDto model)
    {
        unitOfWork.GetRepository<Category>().Add(new Category
        {
            Name = model.Name.Trim(),
            ParentCategoryId = model.ParentCategoryId
        });
        await unitOfWork.CompleteAsync();
    }

    public async Task<bool> UpdateAsync(CategoryFormDto model)
    {
        var category = await unitOfWork.GetRepository<Category>().GetAsync(model.Id);
        if (category is null)
        {
            return false;
        }

        category.Name = model.Name.Trim();
        category.ParentCategoryId = model.ParentCategoryId;
        unitOfWork.GetRepository<Category>().Update(category);
        await unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<DeleteOperationResult> DeleteAsync(int id)
    {
        var categoryRepo = unitOfWork.GetRepository<Category>();
        var productRepo = unitOfWork.GetRepository<Product>();
        var orderItemRepo = unitOfWork.GetRepository<OrderItem>();

        var category = await categoryRepo.GetAsync(id);
        if (category is null)
        {
            return new DeleteOperationResult
            {
                Succeeded = false,
                Message = "Category was not found."
            };
        }

        var productIds = await productRepo
            .GetIQueryable()
            .AsNoTracking()
            .Where(p => p.CategoryId == id)
            .Select(p => p.Id)
            .ToListAsync();

        if (productIds.Count > 0)
        {
            var hasPendingOrders = await orderItemRepo
                .GetIQueryable()
                .AsNoTracking()
                .AnyAsync(oi => productIds.Contains(oi.ProductId) && oi.Order.Status == Status.pending);

            if (hasPendingOrders)
            {
                return new DeleteOperationResult
                {
                    Succeeded = false,
                    Message = "Cannot delete this category because one or more products belong to pending orders."
                };
            }

            return new DeleteOperationResult
            {
                Succeeded = false,
                Message = "Cannot delete this category because it still contains products."
            };
        }

        categoryRepo.Delete(category);
        await unitOfWork.CompleteAsync();

        return new DeleteOperationResult
        {
            Succeeded = true,
            Message = "Category deleted successfully."
        };
    }
}
