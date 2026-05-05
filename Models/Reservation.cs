using System.ComponentModel.DataAnnotations;

namespace RestaurantMvcUltimatePro.Models;

public class Reservation
{
    public int Id { get; set; }
    [Required, StringLength(80)] public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, Phone] public string Phone { get; set; } = string.Empty;
    [Range(1, 50)] public int Guests { get; set; }
    [DataType(DataType.Date)] public DateTime ReservationDate { get; set; } = DateTime.Today;
    [DataType(DataType.Time)] public TimeSpan ReservationTime { get; set; } = new(19, 0, 0);
    [StringLength(500)] public string? SpecialRequest { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
