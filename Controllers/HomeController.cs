using Microsoft.AspNetCore.Mvc;
using RestaurantMvcUltimatePro.Models;
using RestaurantMvcUltimatePro.Services.Interfaces;
using RestaurantMvcUltimatePro.ViewModels;

namespace RestaurantMvcUltimatePro.Controllers;
public class HomeController(IMenuService menu, IContentService content, IContactService contact) : Controller
{
    public async Task<IActionResult> Index() => View(new HomeViewModel{
        PopularItems = await menu.GetPopularAsync(), Chefs = await content.GetChefsAsync(), Testimonials = await content.GetTestimonialsAsync(), Offers = await content.GetOffersAsync(), Events = await content.GetEventsAsync(), 
    });
    public IActionResult About() => View();
    [HttpGet] public IActionResult Contact() => View(new ContactMessage());
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Contact(ContactMessage model){ if(!ModelState.IsValid) return View(model); await contact.CreateAsync(model); TempData["Success"]="Your message has been sent successfully."; return RedirectToAction(nameof(Contact)); }
}
