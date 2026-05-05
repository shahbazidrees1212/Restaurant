using Microsoft.AspNetCore.Mvc;
using RestaurantMvcUltimatePro.Services.Interfaces;
namespace RestaurantMvcUltimatePro.Controllers;
public class EventsController(IContentService content) : Controller { public async Task<IActionResult> Index() => View(await content.GetEventsAsync()); }
