using System.ComponentModel.DataAnnotations;

public class AppUser
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? ProfileImage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}