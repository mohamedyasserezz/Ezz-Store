namespace Ezz_Store.DAL.Entites
{
    public class Category
    {
        public int Id { get; set; }   
        public string Name { get; set; }   
        public int? ParentCategoryId { get; set; } 
        public Category Parent { get; set; }
        public ICollection<Category> Children { get; set; }
        public ICollection<Product> Products { get; set; }
    }

}
