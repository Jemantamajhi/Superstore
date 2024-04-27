using System.ComponentModel.DataAnnotations;

namespace Ebay.Models
{
    public class SubCategory
    {
        [Key]
        public int SId { get; set; }
        public string Name { get; set; }
        // Add other properties as needed

        // Foreign key for Category
        public int CategoryId { get; set; }

        public Category Category { get; set; }
        // Navigation property for Products
        public ICollection<Product> Products { get; set; }
    }
}
