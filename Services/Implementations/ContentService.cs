using Microsoft.EntityFrameworkCore;
using RestaurantMvcUltimatePro.Data;
using RestaurantMvcUltimatePro.Models;
using RestaurantMvcUltimatePro.Services.Interfaces;

namespace RestaurantMvcUltimatePro.Services.Implementations;

public class ContentService : IContentService
{
    private readonly AppDbContext db;

    public ContentService(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<List<Chef>> GetChefsAsync()
    {
        return await db.Chefs.ToListAsync();
    }

    public async Task<List<Testimonial>> GetTestimonialsAsync()
    {
        return await db.Testimonials.ToListAsync();
    }

    public async Task<List<Offer>> GetOffersAsync()
    {
        return await db.Offers.ToListAsync();
    }

    public async Task<List<RestaurantEvent>> GetEventsAsync()
    {
        return await db.RestaurantEvents
                       .OrderBy(x => x.EventDate)
                       .ToListAsync();
    }
}