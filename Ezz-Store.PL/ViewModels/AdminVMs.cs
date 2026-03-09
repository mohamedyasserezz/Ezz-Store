using System.ComponentModel.DataAnnotations;
using Ezz_Store.DAL.Entites;

namespace Ezz_Store.PL.ViewModels;

public class CategoryFormVM
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }
    public List<CategoryOptionVM> ParentOptions { get; set; } = [];
}

public class ProductFormVM
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Range(0.01, 9999999)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public int CategoryId { get; set; }

    public List<CategoryOptionVM> CategoryOptions { get; set; } = [];
}

public class AdminOrderListVM
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

public class UpdateOrderStatusVM
{
    public int OrderId { get; set; }
    [Required]
    public Status Status { get; set; }
}
