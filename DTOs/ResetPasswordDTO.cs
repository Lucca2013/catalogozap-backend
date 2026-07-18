using System.ComponentModel.DataAnnotations;

namespace CatalogoZap.DTOs;

public class ResetPasswordDTO
{
    [Required] public required string NewPassword { get; set; }
}