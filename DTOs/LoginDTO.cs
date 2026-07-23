using System.ComponentModel.DataAnnotations;

namespace CatalogoZap.DTOs;

public record LoginDTO
{
    [Required] public required string Email { get; set; }
    [Required] public required string Password { get; set; }
}