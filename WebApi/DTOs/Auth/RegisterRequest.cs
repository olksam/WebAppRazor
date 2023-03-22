using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Auth;

public class RegisterRequest {
    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;

    [MinLength(6)]
    [Required]
    public string Password { get; set; } = string.Empty;
}