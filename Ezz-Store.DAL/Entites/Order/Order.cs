namespace Ezz_Store.DAL.Entites
{
    public class Order : EntityBase
    {
        public int Id { get; set; }
        public string UserId { get; set; }  
        public int ShippingAddressId { get; set; }  
        public string OrderNumber { get; set; }  
        public Status Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public ApplicationUser User { get; set; }
        public Address ShippingAddress { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

    }
}
