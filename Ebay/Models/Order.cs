namespace Ebay.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShipDate { get; set; }
        public string ShipMode { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } // Navigation property
        public int ProductId { get; set; }
        public Product Product { get; set; } // Navigation property
        public decimal Sales { get; set; }
        public int Quantity { get; set; }
        public decimal Profit { get; set; }
        public bool Returns { get; set; }
        public string PaymentMode { get; set; }
    }
}
