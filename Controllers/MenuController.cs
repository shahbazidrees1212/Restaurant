using Microsoft.AspNetCore.Mvc;
using RestaurantMvcUltimatePro.Services.Interfaces;

namespace RestaurantMvcUltimatePro.Controllers;

public class MenuController(IMenuService menu) : Controller
{
    public async Task<IActionResult> Index(string? category, string? search)
    {
        ViewBag.Categories = await menu.GetCategoriesAsync();
        ViewBag.SelectedCategory = category;
        ViewBag.Search = search;

        return View(await menu.GetAllAsync(category, search));
    }
}