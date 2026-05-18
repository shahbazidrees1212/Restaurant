using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantMvcUltimatePro.Data;
using RestaurantMvcUltimatePro.Models;
using RestaurantMvcUltimatePro.Services.Interfaces;

namespace RestaurantMvcUltimatePro.Controllers;

public class ReservationController : Controller
{
    private readonly AppDbContext _context;
    private readonly IReservationService _reservations;

    public ReservationController(
        AppDbContext context,
        IReservationService reservations)
    {
        _context = context;
        _reservations = reservations;
    }

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
        {
            ModelState.AddModelError(
                nameof(model.ReservationDate),
                "Reservation date cannot be in the past.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _reservations.CreateAsync(model);

        TempData["Success"] = "Reservation request received. We will contact you soon.";

        return RedirectToAction(nameof(MyReservations));
    }

    public async Task<IActionResult> MyReservations()
    {
        var reservations = await _context.Reservations
            .OrderByDescending(x => x.ReservationDate)
            .ToListAsync();

        return View(reservations);
    }
    public IActionResult Edit(int id)
    {
        var reservation = _context.Reservations.FirstOrDefault(x => x.Id == id);

        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    [HttpPost]
    public IActionResult Edit(Reservation reservation)
    {
        var existingReservation = _context.Reservations.FirstOrDefault(x => x.Id == reservation.Id);

        if (existingReservation == null)
        {
            return NotFound();
        }

        existingReservation.FullName = reservation.FullName;
        existingReservation.Email = reservation.Email;
        existingReservation.ReservationDate = reservation.ReservationDate;
        existingReservation.ReservationTime = reservation.ReservationTime;
        existingReservation.Guests = reservation.Guests;

        _context.SaveChanges();

        TempData["Success"] = "Reservation updated successfully.";

        return RedirectToAction("MyReservations");
    }
    public IActionResult Delete(int id)
    {
        var reservation = _context.Reservations.FirstOrDefault(x => x.Id == id);

        if (reservation == null)
            return NotFound();

        _context.Reservations.Remove(reservation);
        _context.SaveChanges();

        TempData["Success"] = "Reservation cancelled successfully.";
        return RedirectToAction("MyReservations");
    }
}