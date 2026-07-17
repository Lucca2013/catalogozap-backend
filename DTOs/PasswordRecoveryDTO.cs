using System.ComponentModel.DataAnnotations;

namespace CatalogoZap.DTOs;

public class PasswordRecoveryDTO
{
    [Required] public required string Email { get; set; }
}