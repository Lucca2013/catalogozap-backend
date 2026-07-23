using Microsoft.AspNetCore.Mvc;
using CatalogoZap.DTOs;
using CatalogoZap.Services;
using CatalogoZap.Infrastructure.Exceptions;

namespace CatalogoZap.Controllers;

[ApiController]
[Route("/api/profiles/recovery-password")]
public sealed class PasswordRecoveryController(
        ProfilesService profilesService
    ) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PasswordRecovery(PasswordRecoveryDTO dto)
    {
        await profilesService.PasswordRecovery(dto);
        
        return Ok();
    }
}
