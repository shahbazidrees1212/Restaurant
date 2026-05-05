using RestaurantMvcUltimatePro.Models;
namespace RestaurantMvcUltimatePro.Services.Interfaces;
public interface IReservationService { Task CreateAsync(Reservation reservation); Task<List<Reservation>> GetLatestAsync(int count=20); }
