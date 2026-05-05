namespace RestaurantMvcUltimatePro.Models;
public class Offer { public int Id { get; set; } public string Title { get; set; } = ""; public string Description { get; set; } = ""; public int DiscountPercent { get; set; } public DateTime ValidUntil { get; set; } }
