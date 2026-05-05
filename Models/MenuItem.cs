using System.ComponentModel.DataAnnotations;

namespace RestaurantMvcUltimatePro.Models;

public class MenuItem
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(50)] public string Category { get; set; } = string.Empty;
    [StringLength(250)] public string Description { get; set; } = string.Empty;
    [Range(0, 999999)] public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPopular { get; set; }
    public bool IsChefSpecial { get; set; }
    public bool IsVegetarian { get; set; }
    public bool IsAvailable { get; set; } = true;
}
