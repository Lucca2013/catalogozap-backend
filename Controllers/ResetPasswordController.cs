using Microsoft.AspNetCore.Mvc;
using CatalogoZap.DTOs;
using CatalogoZap.Services;
using CatalogoZap.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using CatalogoZap.Infrastructure.JWT;

namespace CatalogoZap.Controllers;

[ApiController]
[Route("/api/profiles/reset-password")]
public sealed class ResetPasswordController(
        ProfilesService profilesService
    ) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ResetPassord(ResetPasswordDTO dto)
    {
        var UserId = TokenService.GetUserId(User);

        await profilesService.ResetPassword(dto, UserId);

        return Ok();
    }
}
