using System.ComponentModel.DataAnnotations.Schema;

namespace Ebay.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }

        // Foreign key for Category
        public int CategoryId { get; set; }  // Remove [NotMapped] attribute
        public Category Category { get; set; }

        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        // Foreign key for SubCategory
        public int SubCategoryId { get; set; }  // Remove [NotMapped] attribute
        public SubCategory SubCategory { get; set; }
    }
}
