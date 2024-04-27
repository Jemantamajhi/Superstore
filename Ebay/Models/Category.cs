namespace Ebay.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Add other properties as needed

        // Navigation property for SubCategories
        public ICollection<SubCategory> SubCategories { get; set; }

        // Navigation property for Products
        public ICollection<Product> Products { get; set; }
    }
}
