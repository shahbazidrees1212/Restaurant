using System.ComponentModel.DataAnnotations;

namespace RestaurantMvcUltimatePro.Models;

public class ContactMessage
{
    public int Id { get; set; }
    [Required, StringLength(80)] public string Name { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, StringLength(120)] public string Subject { get; set; } = string.Empty;
    [Required, StringLength(1000)] public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
