namespace Ezz_Store.DAL.Entites
{
    public class Address
    {
        public int Id { get; set; }
        public string UserId { get; set; }  
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public bool IsDefault { get; set; }
        public ApplicationUser User { get; set; }
    }

}
