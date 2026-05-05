using RestaurantMvcUltimatePro.Models;
namespace RestaurantMvcUltimatePro.Services.Interfaces;
public interface IContactService { Task CreateAsync(ContactMessage message); Task<List<ContactMessage>> GetLatestAsync(int count=20); }
