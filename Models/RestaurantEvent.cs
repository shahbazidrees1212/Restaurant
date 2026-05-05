namespace RestaurantMvcUltimatePro.Models;
public class RestaurantEvent { public int Id { get; set; } public string Title { get; set; } = ""; public string Description { get; set; } = ""; public DateTime EventDate { get; set; } public string ImageUrl { get; set; } = ""; public decimal StartingPrice { get; set; } }
