
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ebay.Data;
using Microsoft.AspNetCore.Http;
using Ebay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ebay.Extensions;

namespace Ebay.Controllers.Products
{
   
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        
      
       

        public IActionResult Mens()
        {
            var mensProducts = _context.Products.Where(p => p.SubCategoryName.ToLower() == "mens").ToList();
            return View(mensProducts);
        }
        public IActionResult Furniture()
        {
            var furniture = _context.Products.Where(p => p.CategoryName.ToLower() == "furniture").ToList();
            return View(furniture);
        }
        public IActionResult Womens()
        {
            var WomensProducts = _context.Products.Where(p => p.SubCategoryName.ToLower() == "womens").ToList();
            return View(WomensProducts);
        }
        public IActionResult Kids()
        {

            var kidsProducts = _context.Products.Where(p => p.SubCategoryName.ToLower() == "kids").ToList();
            return View(kidsProducts);
        }
        public IActionResult Fashion()
        {
            var Products = _context.Products.Where(p => p.CategoryName.ToLower() == "clothing").ToList();
            return View(Products);
        }
        public IActionResult Electronics()
        {
            var Products = _context.Products.Where(p => p.CategoryName.ToLower() == "electronics").ToList();
            return View(Products);
        }
        public IActionResult Laptops()
        {
            var Products = _context.Products.Where(p => p.SubCategoryName.ToLower() == "laptops").ToList();
            return View(Products);
        }
        public IActionResult Smartphone()
        {
            var Products = _context.Products.Where(p => p.SubCategoryName.ToLower() == "Smartphones").ToList();
            return View(Products);
        }

        public IActionResult Gaming()
        {
            var products = _context.Products.Where(p => p.ProductName.ToLower().Contains("gaming")).ToList();
            return View(products);
        }

        public IActionResult Shop(string find)
        {
            var products = _context.Products.ToList(); // Assuming _context is your DbContext instance
         
            if (!String.IsNullOrEmpty(find)) {
                // Convert the search term and the properties to lowercase for case-insensitive search
                var searchTerm = find.ToLower();
                products = products.Where(p =>
                    p.CategoryName != null && p.CategoryName.ToLower().Contains(searchTerm) ||
                    p.ProductName != null && p.ProductName.ToLower().Contains(searchTerm) ||
                    p.SubCategoryName != null && p.SubCategoryName.ToLower().Contains(searchTerm)
                ).ToList();
            }
            return View(products);
        }
        [HttpPost]
        public async Task<IActionResult> Product(Product product, IFormFile file,int categoryId,int subcategoryId)
        {
            string uniqueFileName = null;
            if (file != null && file.Length > 0)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Image");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            product.ImagePath = uniqueFileName;
            // Retrieve category name based on the provided categoryId
            var category = await _context.Categories.FindAsync(categoryId);
            var subcatagory=await _context.SubCategories.FindAsync(subcategoryId);
            if (category != null && subcatagory!=null)
            {
                // Set the CategoryName property of the product
                product.CategoryName = category.Name;
                product.SubCategoryName=subcatagory.Name;
            }
            // Set the CategoryId and SubCategoryId of the product
            product.CategoryId = categoryId;
            product.SubCategoryId = subcategoryId;


            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Product Added Successfully";
            return RedirectToAction("Product");
        }

       /* public IActionResult AddToCart() {
        return View();
        }
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            // Retrieve the current user's cart from session
            List<int> cart = HttpContext.Session.Get<List<int>>("Cart");

            // If cart doesn't exist in session, create a new one
            if (cart == null)
            {
                cart = new List<int>();
            }

            // Add the product ID to the cart
            cart.Add(productId);

            // Save the updated cart back to session
            HttpContext.Session.Set("Cart", cart);

            // Redirect the user to the cart page or any other appropriate page
            return RedirectToAction("AddToCart", "Products");
        }*/


    }
}
