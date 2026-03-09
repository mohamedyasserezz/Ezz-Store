using Microsoft.AspNetCore.Identity;

namespace Ezz_Store.DAL.Entites
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Order> Orders { get; set; }
    }

}
