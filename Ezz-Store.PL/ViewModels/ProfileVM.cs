using System.ComponentModel.DataAnnotations;

namespace Ezz_Store.PL.ViewModels;

public class ProfileVM
{
    [Required]
    [MaxLength(100)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [MaxLength(20)]
    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }
}
