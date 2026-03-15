namespace Ezz_Store.PL.ViewModels;

public class ProductListVM
{
    public List<ProductCardVM> Items { get; set; } = [];
    public List<CategoryOptionVM> Categories { get; set; } = [];
    public int? CategoryId { get; set; }
    public string? Query { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
}

public class ProductCardVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
}

public class CategoryOptionVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
