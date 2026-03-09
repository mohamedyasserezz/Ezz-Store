using Ezz_Store.DAL.Entites;

namespace Ezz_Store.BLL.Models.Orders;

public class AddressOptionDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class CheckoutPageDto
{
    public List<AddressOptionDto> AddressOptions { get; set; } = [];
}

public class CheckoutRequestDto
{
    public int? SelectedAddressId { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Zip { get; set; }
}

public class CheckoutResultDto
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = [];
    public int? OrderId { get; set; }
    public string? OrderNumber { get; set; }
}

public class CustomerOrderListItemDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

public class CustomerOrderDetailsDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string AddressText { get; set; } = string.Empty;
    public List<CustomerOrderItemDto> Items { get; set; } = [];
}

public class CustomerOrderItemDto
{
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

public class AdminOrderListItemDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

public class AdminUpdateOrderStatusDto
{
    public int OrderId { get; set; }
    public Status Status { get; set; }
}
