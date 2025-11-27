using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SSD_Lab1.Data;
using SSD_Lab1.Models;

namespace SSD_Lab1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;

                // Password settings - Strong password requirements
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 4;

                // Lockout settings - Protect against brute force attacks
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddDefaultUI();

            // Configure cookie security for application cookies
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            // Configure cookie security for external authentication cookies
            builder.Services.ConfigureExternalCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            // Configure all cookies globally
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
            });

            // Configure anti-forgery tokens globally
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
                options.Cookie.Name = "__RequestVerificationToken";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.FormFieldName = "__RequestVerificationToken";
                options.SuppressXFrameOptionsHeader = false;
            });

            builder.Services.AddControllersWithViews();

            var kv = new Uri(builder.Configuration.GetSection("kvURI").Value);
            var azCred = new DefaultAzureCredential();

            builder.Configuration.AddAzureKeyVault(kv, azCred);
            DbInitializer.DemoPassword = builder.Configuration.GetSection("DemoPassword").Value;

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                DbInitializer.SeedUsersAndRoles(scope.ServiceProvider).Wait();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Path traversal protection middleware
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value ?? "";
                
                // Block path traversal attempts
                if (path.Contains("..") || path.Contains("//") || path.Contains("\\\\"))
                {
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsync("Invalid path detected");
                    return;
                }

                await next();
            });

            // Security headers middleware
            app.Use(async (context, next) =>
            {
                // Remove X-Powered-By header
                context.Response.Headers.Remove("X-Powered-By");
                
                // Hide Server header (note: may be overridden by web server)
                context.Response.Headers.Remove("Server");

                // Content Security Policy (strict - no wildcards, no unsafe-inline or unsafe-eval)
                context.Response.Headers.Append("Content-Security-Policy", 
                    "default-src 'self'; " +
                    "script-src 'self' https://cdn.jsdelivr.net; " +
                    "style-src 'self' https://cdn.jsdelivr.net; " +
                    "img-src 'self' data:; " +
                    "font-src 'self' https://cdn.jsdelivr.net; " +
                    "connect-src 'self'; " +
                    "frame-ancestors 'none'; " +
                    "base-uri 'self'; " +
                    "form-action 'self';");

                // Anti-clickjacking header
                context.Response.Headers.Append("X-Frame-Options", "DENY");

                // Prevent MIME type sniffing
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

                // Enable browser XSS protection
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

                // Cache control for sensitive pages
                var path = context.Request.Path.Value?.ToLower() ?? "";
                if (path.StartsWith("/identity") || 
                    path.StartsWith("/account") ||
                    path.StartsWith("/messages/create") ||
                    path.StartsWith("/messages/edit") ||
                    path.StartsWith("/companies/create") ||
                    path.StartsWith("/companies/edit") ||
                    path.StartsWith("/cars/create") ||
                    path.StartsWith("/cars/edit") ||
                    path.StartsWith("/employees/create") ||
                    path.StartsWith("/employees/edit") ||
                    path.Contains("/home/error"))
                {
                    context.Response.Headers.Append("Cache-Control", "no-store, no-cache, must-revalidate, max-age=0");
                    context.Response.Headers.Append("Pragma", "no-cache");
                    context.Response.Headers.Append("Expires", "0");
                }
                else if (path.StartsWith("/wwwroot") || path.StartsWith("/lib") || path.StartsWith("/css") || path.StartsWith("/js"))
                {
                    // Static content - allow caching
                    context.Response.Headers.Append("Cache-Control", "public, max-age=31536000, immutable");
                }
                else
                {
                    // Default cache control for dynamic content
                    context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                    context.Response.Headers.Append("Pragma", "no-cache");
                    context.Response.Headers.Append("Expires", "0");
                }

                await next();
            });

            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            

            app.Run();
        }
    }
}
