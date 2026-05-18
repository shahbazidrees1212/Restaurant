using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantMvcUltimatePro.Data;
using RestaurantMvcUltimatePro.Models;
using System.Security.Claims;

namespace RestaurantMvcUltimatePro.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<AppUser> _passwordHasher;

        public AccountController(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<AppUser>();
        }

        // ================= REGISTER =================

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check existing email
            var existingUser = _context.AppUsers
                .FirstOrDefault(x => x.Email == model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(model);
            }

            // Create user
            var user = new AppUser
            {
                FullName = model.FullName,
                Email = model.Email
            };

            // Hash password
            user.PasswordHash = _passwordHasher
                .HashPassword(user, model.Password);

            // Save user
            _context.AppUsers.Add(user);
            _context.SaveChanges();

            TempData["Success"] =
                "Account created successfully. Please login.";

            return RedirectToAction("Login");
        }

        // ================= LOGIN =================

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Find user
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            // Verify password
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                model.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            // Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id.ToString())
            };

            // Identity
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            // Sign in
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return RedirectToAction("Index", "Home");
        }

        // ================= LOGOUT =================

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Index", "Home");
        }
    }
}