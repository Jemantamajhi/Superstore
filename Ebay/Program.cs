using Ebay.Data;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Ebay.Models;
using Ebay.Reposetory.Interface;
using Ebay.Reposetory.Service;
using Ebay.ViewModels;


using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    /*.AddCookie()*/
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
     {
         options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
         options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
     });

// Add services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString")
    ?? throw new InvalidOperationException("Connection string 'MyConnectionString' not found.")));


//Cookies/session For login logout authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(option =>
{
    option.ExpireTimeSpan = TimeSpan.FromMinutes(60 * 1);
    option.LoginPath = "/admin/AdminLogin";
    option.AccessDeniedPath = "/admin/AccessDenied";
    
});
//for seession and it will expire in 5 min if no action are performed
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(5);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication(); // Add this line to enable authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=UserPage}/{id?}");

app.Run();
