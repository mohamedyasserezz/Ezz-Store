using Ezz_Store.BLL.Models.Admin;
using Ezz_Store.BLL.Models.Catalog;

namespace Ezz_Store.BLL.Services.Admin;

public interface IAdminCategoryService
{
    Task<List<CategoryListItemDto>> GetAllAsync();
    Task<CategoryFormDto?> GetByIdAsync(int id);
    Task<List<CategoryOptionDto>> GetParentOptionsAsync(int? currentCategoryId = null);
    Task CreateAsync(CategoryFormDto model);
    Task<bool> UpdateAsync(CategoryFormDto model);
    Task<DeleteOperationResult> DeleteAsync(int id);
}
