namespace Ebay.Models
{
    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal? Totalprice { get; set; }
        public int TotalQuantity { get; set;}
    }
}
