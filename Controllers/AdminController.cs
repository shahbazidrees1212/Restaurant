using Microsoft.AspNetCore.Mvc;
using RestaurantMvcUltimatePro.Services.Interfaces;
namespace RestaurantMvcUltimatePro.Controllers;
public class AdminController(IReservationService reservations, IContactService contacts) : Controller
{
    public async Task<IActionResult> Dashboard(){ ViewBag.Reservations = await reservations.GetLatestAsync(); ViewBag.Messages = await contacts.GetLatestAsync(); return View(); }
}
