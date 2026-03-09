using Ezz_Store.BLL.Models.Catalog;

namespace Ezz_Store.BLL.Services.Catalog;

public interface ICatalogService
{
    Task<CatalogResultDto> GetCatalogAsync(int? categoryId, string? query, string? sort, int page);
    Task<ProductDetailsDto?> GetDetailsAsync(int id);
}
