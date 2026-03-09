using Ezz_Store.BLL.Models.Admin;
using Ezz_Store.BLL.Models.Catalog;

namespace Ezz_Store.BLL.Services.Admin;

public interface IAdminProductService
{
    Task<List<ProductListItemDto>> GetAllAsync();
    Task<ProductFormDto?> GetByIdAsync(int id);
    Task<List<CategoryOptionDto>> GetCategoryOptionsAsync();
    Task CreateAsync(ProductFormDto model);
    Task<bool> UpdateAsync(ProductFormDto model);
    Task DeleteAsync(int id);
}
