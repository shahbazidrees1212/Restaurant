using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantMvcUltimatePro.Data;
using RestaurantMvcUltimatePro.Models;
namespace RestaurantMvcUltimatePro.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly AppDbContext _context;

    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _context.AppUsers.FirstOrDefaultAsync();

        ViewBag.ProfileImage = user?.ProfileImage ?? "/images/default-user.jpg";
        ViewBag.FullName = user?.FullName;
        ViewBag.Email = user?.Email;
        ViewBag.PhoneNumber = user?.PhoneNumber;
        ViewBag.Address = user?.Address;

        var userReservations = await _context.Reservations
            .OrderByDescending(x => x.ReservationDate)
            .Take(2)
            .ToListAsync();

        return View(userReservations);
    }
    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var user = await _context.AppUsers.FirstOrDefaultAsync();

        ViewBag.ProfileImage = user?.ProfileImage ?? "/images/default-user.jpg";

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(IFormFile? ProfileImage)
    {
        var user = await _context.AppUsers.FirstOrDefaultAsync();

        if (user == null)
            return Content("User not found");

        if (ProfileImage != null && ProfileImage.Length > 0)
        {
            string folder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                "profile");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
            string filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ProfileImage.CopyToAsync(stream);
            }

            user.ProfileImage = "/images/profile/" + fileName;

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}