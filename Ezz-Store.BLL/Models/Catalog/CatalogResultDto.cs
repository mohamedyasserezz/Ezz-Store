namespace Ezz_Store.BLL.Models.Catalog;

public class CatalogResultDto
{
    public List<ProductCardDto> Items { get; set; } = [];
    public List<CategoryOptionDto> Categories { get; set; } = [];
    public int Page { get; set; }
    public int TotalPages { get; set; }
}

public class ProductCardDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
}

public class CategoryOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ProductDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
}
