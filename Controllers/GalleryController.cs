using Microsoft.AspNetCore.Mvc;
using RestaurantMvcUltimatePro.Services.Interfaces;
namespace RestaurantMvcUltimatePro.Controllers;
public class GalleryController(IContentService content) : Controller { public async Task<IActionResult> Index() => View(await content.GetGalleryAsync()); }
