using Microsoft.EntityFrameworkCore;
using RestaurantMvcUltimatePro.Data;
using RestaurantMvcUltimatePro.Models;
using RestaurantMvcUltimatePro.Services.Interfaces;
namespace RestaurantMvcUltimatePro.Services.Implementations;
public class ContentService(AppDbContext db) : IContentService
{
    public Task<List<Chef>> GetChefsAsync() => db.Chefs.ToListAsync();
    public Task<List<Testimonial>> GetTestimonialsAsync() => db.Testimonials.ToListAsync();
    public Task<List<Offer>> GetOffersAsync() => db.Offers.ToListAsync();
    public Task<List<RestaurantEvent>> GetEventsAsync() => db.RestaurantEvents.OrderBy(x=>x.EventDate).ToListAsync();
    public Task<List<GalleryImage>> GetGalleryAsync() => db.GalleryImages.ToListAsync();
}
