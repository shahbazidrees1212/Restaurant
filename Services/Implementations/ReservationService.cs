using Microsoft.EntityFrameworkCore;
using RestaurantMvcUltimatePro.Data;
using RestaurantMvcUltimatePro.Models;
using RestaurantMvcUltimatePro.Services.Interfaces;
namespace RestaurantMvcUltimatePro.Services.Implementations;
public class ReservationService(AppDbContext db) : IReservationService
{
    public async Task CreateAsync(Reservation reservation){ db.Reservations.Add(reservation); await db.SaveChangesAsync(); }
    public async Task<List<Reservation>> GetLatestAsync(int count=20) => await db.Reservations.OrderByDescending(x=>x.CreatedAt).Take(count).ToListAsync();
}
