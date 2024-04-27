using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Ebay.Data;
using Ebay.Models;
using Ebay.Reposetory.Interface;
using Ebay.Reposetory.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace Ebay.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender emailSender;
        

        public AccountController(ApplicationDbContext context,IEmailSender emailSender)
        {
            _context = context;
            this.emailSender = emailSender;
          
        }

        [HttpGet]
        public IActionResult Register(string message="")
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Name=model.Name,
                    UserName = model.Email,
                    Password=model.Password,
                    Email = model.Email,
                    IsActive = true,// Assuming you want new users to be active by default
                    Mobile = model.Mobile
                };

                // Save the user to the database
                _context.Users.Add(user);
                bool status = await emailSender.EmailSendAsync(model.Email, "Account Created", $"Dear {model.Name},\r\n\r\nWe're excited to welcome you to Ebay! Your account is now up and running, ready for you to start shopping and selling.\r\n\r\nThank you for choosing Ebay. Enjoy the experience, and happy shopping!\r\n\r\nBest regards,\r\nEbay Team");



                await _context.SaveChangesAsync();
                /*ViewBag.Message = "Register Successful You can Login Now";*/

                // Redirect to login page or any other page after successful registration
                /*return RedirectToAction("Register");*/
                return RedirectToAction("Register", new { message = "You are  successfully Register" });
            }

            // If ModelState is not valid, return the registration view with errors
            return View(model);
        }
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult UserPage()
        {
            var data= _context.Products.ToList();
            return View(data);
        }

        public IActionResult MyAccount()
        {
            // Get the ID of the logged-in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the user by ID
            var user = _context.Users.FirstOrDefault(u => u.Id == int.Parse(userId));

            if (user == null)
            {
                // Handle case where user is not found
                return NotFound();
            }

            return View(user);

        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult About()
        {
           
            return View();
        }
        public IActionResult Faq()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            HttpContext.Session.SetString("IsAuthenticated", "false");
            return View();
        }



        
        [HttpPost]
    [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    HttpContext.Session.SetString("IsAuthenticated", "true");

                    return RedirectToAction("UserPage", "Account");
                }

                else
                {
                    // If user is not found or password is incorrect, add model error
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }
            }
            return View(model);
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        public async Task TaskGLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            }) ;
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result=await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            }) ;
            return RedirectToAction("UserPage", "Account", new { area=""});
        }

        public async Task<IActionResult> Logout()
        {
            // Clear session variables
          /*  HttpContext.Session.Clear();
            HttpContext.Session.Remove("IsAuthenticated");*/
          await HttpContext.SignOutAsync();
            return RedirectToAction("UserPage", "Account");
        }




    }
}
