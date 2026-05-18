using Microsoft.EntityFrameworkCore;
using RestaurantMvcUltimatePro.Data;
using RestaurantMvcUltimatePro.Models;
using RestaurantMvcUltimatePro.Services.Interfaces;

namespace RestaurantMvcUltimatePro.Services.Implementations;

public class MenuService : IMenuService
{
    private readonly AppDbContext db;

    public MenuService(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<List<MenuItem>> GetAllAsync(string? category = null, string? search = null)
    {
        var q = db.MenuItems
                  .Where(x => x.IsAvailable)
                  .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            q = q.Where(x => x.Category == category);

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(x =>
                x.Name.Contains(search) ||
                x.Description.Contains(search));

        return await q.OrderBy(x => x.Category)
                      .ThenByDescending(x => x.IsPopular)
                      .ToListAsync();
    }

    public async Task<List<MenuItem>> GetPopularAsync(int count = 6)
    {
        return await db.MenuItems
                       .Where(x => x.IsPopular || x.IsChefSpecial)
                       .Take(count)
                       .ToListAsync();
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        return await db.MenuItems
                       .Select(x => x.Category)
                       .Distinct()
                       .OrderBy(x => x)
                       .ToListAsync();
    }

    public async Task<MenuItem?> GetByIdAsync(int id)
    {
        return await db.MenuItems
                       .FirstOrDefaultAsync(x => x.Id == id);
    }
}