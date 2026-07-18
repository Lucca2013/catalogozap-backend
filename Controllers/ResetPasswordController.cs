using Microsoft.AspNetCore.Mvc;
using CatalogoZap.DTOs;
using CatalogoZap.Services.Interfaces;
using CatalogoZap.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using CatalogoZap.Infrastructure.JWT;

namespace CatalogoZap.Controllers;

[ApiController]
[Route("/api/profiles/reset-password")]
public class ResetPasswordController : ControllerBase
{
    private readonly IProfilesService _profilesService;

    public ResetPasswordController(IProfilesService profilesService)
    {
        _profilesService = profilesService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ResetPassord(ResetPasswordDTO dto)
    {
        var UserId = TokenService.GetUserId(User);

        try { await _profilesService.ResetPassword(dto, UserId); }
        catch (NotFoundException err) { return NotFound(err.Message); }
        catch (Exception) { return StatusCode(500); }

        return Ok();
    }
}
