namespace Ezz_Store.PL.ViewModels;

public class CartVM
{
    public List<CartItemVM> Items { get; set; } = [];
    public decimal SubTotal => Items.Sum(i => i.LineTotal);
    public int TotalQuantity => Items.Sum(i => i.Quantity);
}

public class CartItemVM
{
    public int ItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int AvailableStock { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}
