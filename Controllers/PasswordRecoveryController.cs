using Microsoft.AspNetCore.Mvc;
using CatalogoZap.DTOs;
using CatalogoZap.Services.Interfaces;
using CatalogoZap.Infrastructure.Exceptions;

namespace CatalogoZap.Controllers;

[ApiController]
[Route("/api/profiles/recovery-password")]
public class PasswordRecoveryController : ControllerBase
{
    private readonly IProfilesService _profilesService;

    public PasswordRecoveryController(IProfilesService profilesService)
    {
        _profilesService = profilesService;
    }

    [HttpPost]
    public async Task<IActionResult> PasswordRecovery(PasswordRecoveryDTO dto)
    {
        try { await _profilesService.PasswordRecovery(dto); }
        catch (NotFoundException err) { return NotFound(err.Message); }
        catch (Exception) { return StatusCode(500); }

        return Created();
    }
}
