using Microsoft.AspNetCore.Mvc;
using RestaurantMvcUltimatePro.Models;
using RestaurantMvcUltimatePro.Services.Interfaces;

namespace RestaurantMvcUltimatePro.Controllers;

public class ReservationController(IReservationService reservations) : Controller
{
    [HttpGet]
    public IActionResult Create(int? eventId, string? eventTitle)
    {
        var model = new Reservation
        {
            ReservationDate = DateTime.Today.AddDays(1),
            ReservationTime = new TimeSpan(19, 0, 0),
            Guests = 2
        };

        if (eventId.HasValue)
        {
            model.SpecialRequest = !string.IsNullOrWhiteSpace(eventTitle)
                ? $"Booking for event: {eventTitle}"
                : $"Booking for event ID: {eventId.Value}";
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Reservation model)
    {
        if (model.ReservationDate.Date < DateTime.Today)
            ModelState.AddModelError(nameof(model.ReservationDate), "Reservation date cannot be in the past.");

        if (!ModelState.IsValid)
            return View(model);

        await reservations.CreateAsync(model);

        TempData["Success"] = "Reservation request received. We will contact you soon.";

        return RedirectToAction(nameof(Create));
    }
}