using System.ComponentModel.DataAnnotations;

namespace CatalogoZap.DTOs;

public record PasswordRecoveryDTO
{
    [Required] public required string Email { get; set; }
}