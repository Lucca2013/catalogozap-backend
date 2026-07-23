using System.ComponentModel.DataAnnotations;

namespace CatalogoZap.DTOs;

public record ResetPasswordDTO
{
    [Required] public required string NewPassword { get; set; }
}