namespace Ezz_Store.DAL.Entites
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }  
        public string Name { get; set; }
        public string SKU { get; set; }  
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public Category Category { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }

}
