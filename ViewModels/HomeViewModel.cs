using RestaurantMvcUltimatePro.Models;
namespace RestaurantMvcUltimatePro.ViewModels;
public class HomeViewModel
{
    public List<MenuItem> PopularItems { get; set; } = [];
    public List<Chef> Chefs { get; set; } = [];
    public List<Testimonial> Testimonials { get; set; } = [];
    public List<Offer> Offers { get; set; } = [];
    public List<RestaurantEvent> Events { get; set; } = [];
    public List<GalleryImage> Gallery { get; set; } = [];
}
