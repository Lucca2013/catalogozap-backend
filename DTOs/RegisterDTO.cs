using System.ComponentModel.DataAnnotations;

namespace CatalogoZap.DTOs;

public record RegisterDTO
{
    [Required] public required string Username { get; set; }
    [Required] public required string Email { get; set; }
    [Required] public required string Password { get; set; }
}