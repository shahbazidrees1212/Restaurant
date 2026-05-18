using RestaurantMvcUltimatePro.Models;
namespace RestaurantMvcUltimatePro.Services.Interfaces;
public interface IContentService { Task<List<Chef>> GetChefsAsync(); Task<List<Testimonial>> GetTestimonialsAsync(); Task<List<Offer>> GetOffersAsync(); Task<List<RestaurantEvent>> GetEventsAsync();  }
