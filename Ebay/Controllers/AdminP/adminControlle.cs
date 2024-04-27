using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;


using Ebay.Data;
using Ebay.Models;
using Ebay.Models.Admin;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;


namespace Ebay.Controllers.AdminP.adminController
{
    public class adminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public adminController(ApplicationDbContext context,IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

      /*  [HttpGet]*/
        public IActionResult AdminLogin()
        {
            /* if (HttpContext.Session.GetString("admin_session") != null)
             {
                 return RedirectToAction("Dashboard", "admin");
             }*/
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "admin");
            }
            return View();
        }
        public JsonResult GetSubCategories(int categoryId)
        {
            var subcategories = _context.SubCategories
                                         .Where(sc => sc.CategoryId == categoryId).ToList();
               
            return new JsonResult(subcategories);
        }

        public async Task<IActionResult> ProductAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

            // If you have a default category selected, you can set the subcategories for it here
            if (categories.Any())
            {
                var defaultCategoryId = categories.First().Id;
                var subcategories = await _context.SubCategories.Where(sc => sc.CategoryId == defaultCategoryId).ToListAsync();
                ViewBag.Subcategories = subcategories.Select(sc => new SelectListItem { Value = sc.SId.ToString(), Text = sc.Name }).ToList();
            }
            else
            {
                ViewBag.Subcategories = new List<SelectListItem>();
            }

            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> Product(Product product, IFormFile file, int categoryId)
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

            var subcategory = await _context.SubCategories
     .FirstOrDefaultAsync(sc => sc.SId == categoryId);

            if (category != null && subcategory != null)
            {
                // Set the CategoryName property of the product
                product.CategoryName = category.Name;
                product.SubCategoryName = subcategory.Name;
            }
            // Set the CategoryId and SubCategoryId of the product
            product.CategoryId = categoryId;
            product.SubCategoryId = subcategory.SId;


            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Product Added Successfully";
            return RedirectToAction("Product");
        }
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult ProductDetails()
        {
            var data = _context.Products.ToList();
            return View(data);
        }
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult ShowCategory()
        {
            // Retrieve categories and subcategories from the database
            var categories = _context.Categories.ToList();
            var subcategories = _context.SubCategories.ToList();

            // Pass categories and subcategories to the view
            ViewBag.Categories = categories;
            ViewBag.Subcategories = subcategories;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public IActionResult AdminLogin(string Username, string Password)
        {
            var data = _context.AdminTable.FirstOrDefault(a => a.Username == Username && a.Password == Password);

            if (data != null)
            {
                HttpContext.Session.SetString("AdminId", data.Id.ToString());
                return RedirectToAction("Dashboard","admin");
            }
            else
            {
                ViewBag.Message = "Invalid Password or username";
                return View();
            }
        }
        [ResponseCache(NoStore =true,Location=ResponseCacheLocation.None)]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("AdminLogin");
            }
            else
            {
                return View();
            }
        }
        //add category
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult AddCategory()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult CreateSubCategory()
        {
            var categories = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            ViewBag.Categories = categories;
            return View();
        }
       
        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("CreateSubCategory", "admin"); // Redirect to home or wherever appropriate
        }
        

        [HttpPost]
        public IActionResult CreateSubCategory(SubCategory subCategory)
        {
            _context.SubCategories.Add(subCategory);
            _context.SaveChanges();
            return RedirectToAction("CreateSubCategory", "admin"); // Redirect to home or wherever appropriate
        }
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult CustomersFetch()
        {
            // Fetch all customers from your table
            var customers = _context.Customers.ToList(); // Assuming _context is your DbContext and Customers is your DbSet

            // Pass the list of customers to the view
            return View(customers);
        }

        // GET: Customer/Edit/5
       
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
  
        //edit product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProductEdit(int id, [Bind("ProductId,ProductName,Price,Decription,CategoryName,SubCategoryNAme,ImagePath,")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    _context.SaveChanges();
                }
                catch
                {
                    throw;
                }
                return RedirectToAction(nameof(ProductEdit));
            }
            return View(product);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("CustomerId,CustomerName,Segment,Country,State,Region,City")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    _context.SaveChanges();
                }
                catch
                {
                    throw;
                }
                return RedirectToAction(nameof(Edit));
            }
            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return RedirectToAction(nameof(CustomersFetch));
        }
        [HttpPost, ActionName("PDelete")]
        [ValidateAntiForgeryToken]
        public IActionResult PDeleteConfirmed(int id)
        {
            var Product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            _context.Products.Remove(Product);
            _context.SaveChanges();
            return RedirectToAction(nameof(ProductDetails));
        }
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult adminProfile()
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            var row = _context.AdminTable.Where(a => a.Id == int.Parse(adminId)).ToList();
            return View(row);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAdminProfile(admin admin)
        {
            if (!ModelState.IsValid)
            {
                return View(admin); // Return the same view with validation errors
            }

            try
            {
                _context.Update(admin);
                _context.SaveChanges();
                return RedirectToAction("AdminProfile");
            }
            catch (DbUpdateException ex)
            {
                // Log the exception or handle it as per your requirement
                return RedirectToAction("Error");
            }
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminId");
            return RedirectToAction("AdminLogin");
        }
    }
}