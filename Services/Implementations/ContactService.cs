using Microsoft.EntityFrameworkCore;
using RestaurantMvcUltimatePro.Data;
using RestaurantMvcUltimatePro.Models;
using RestaurantMvcUltimatePro.Services.Interfaces;

namespace RestaurantMvcUltimatePro.Services.Implementations;

public class ContactService(AppDbContext db) : IContactService
{
    public async Task CreateAsync(ContactMessage message)
    {
        db.ContactMessages.Add(message);

        await db.SaveChangesAsync();
    }

    public async Task<List<ContactMessage>> GetLatestAsync(int count = 20)
    {
        return await db.ContactMessages
            .OrderByDescending(x => x.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}