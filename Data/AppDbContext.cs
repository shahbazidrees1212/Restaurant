using Microsoft.EntityFrameworkCore;
using RestaurantMvcUltimatePro.Models;

namespace RestaurantMvcUltimatePro.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<Chef> Chefs => Set<Chef>();
    public DbSet<Testimonial> Testimonials => Set<Testimonial>();
    public DbSet<RestaurantEvent> RestaurantEvents => Set<RestaurantEvent>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
}
