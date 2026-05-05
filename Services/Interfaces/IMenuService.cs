using RestaurantMvcUltimatePro.Models;
namespace RestaurantMvcUltimatePro.Services.Interfaces;
public interface IMenuService { Task<List<MenuItem>> GetAllAsync(string? category=null, string? search=null); Task<List<MenuItem>> GetPopularAsync(int count=6); Task<List<string>> GetCategoriesAsync();
    Task<MenuItem?> GetByIdAsync(int id);
}
