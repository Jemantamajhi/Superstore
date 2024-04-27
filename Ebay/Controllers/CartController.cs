using Ebay.Data;
using Ebay.Extensions;
using Ebay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ebay.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private List<CartItem> _cartItems;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
            _cartItems=new List<CartItem>();
        }
       public IActionResult Index()
        {
            return View();
        }
       
        public IActionResult AddToCart(int id)
        {
            var data=_context.Products.Find(id);
            var cartItems=HttpContext.Session.Get<List<CartItem>>("Cart")?? new List<CartItem>();
            var existingcartitem = cartItems.FirstOrDefault(item => item.Product.ProductId == id);
            if (existingcartitem != null)
            {

                existingcartitem.Quantity++;
            }
            else {
                cartItems.Add(new CartItem
                {
                    Product = data,
                    Quantity=1
                });
            }
            HttpContext.Session.Set("Cart", cartItems);
            /*TempData["CartMessage"] = $"{data.ImagePath}${data.Price}";*/

            return RedirectToAction("ViewData");
        }
        public IActionResult ViewData()
        {
            var cartItems = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            var data = new CartViewModel
            {
                CartItems = cartItems,
                /*Totalprice = cartItems.Sum(item => item.Product.Price * item.Quantity)*/
                Totalprice = cartItems.Sum(item => (item.Product?.Price ?? 0) * item.Quantity)

            };
            return View(data);
        }
        public IActionResult RemoveData(int id)
        {
            var cartItems = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            var itemToRemove = cartItems.FirstOrDefault(item => item.Product.ProductId == id);
            if (itemToRemove != null)
            {
                if (itemToRemove.Quantity > 1)
                {
                    itemToRemove.Quantity--;
                }
                else
                {
                    cartItems.Remove(itemToRemove);
                }
            }
            HttpContext.Session.Set("Cart", cartItems);
            return RedirectToAction("ViewData");
        }

        public IActionResult Status()
        {
            return View();
        }

        public IActionResult Orderdetails(Customer model)
        {
            if (ModelState.IsValid) {
                var customer = new Customer
                {
                    CustomerName= model.CustomerName,
                    Country= model.Country,
                    City= model.City,
                    State= model.State,
                    Region= model.Region,
                    MobileNo= model.MobileNo,
                };
                _context.Customers.Add(customer);
                _context.SaveChanges();
                /* var order = new Order();
                 order.ProductId = customer.CustomerId;
                 _context.Orders.Add(order);
                 var cartItems = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
                 foreach (var item in cartItems)
                 {
                     _context.Orders.Add(new Order
                     {
                         ProductId= item.ProductId,
                         Quantity= item.Quantity,
                         OrderDate= DateTime.Now,
                         ShipDate= DateTime.Now,
                         CustomerId=item.
                         T

                     });
                 }*/
                return RedirectToAction("Status", "Cart");
            }
                return View();
        }
       
    }
}
