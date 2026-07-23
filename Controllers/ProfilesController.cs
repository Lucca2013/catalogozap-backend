using Microsoft.AspNetCore.Mvc;
using CatalogoZap.Services;
using CatalogoZap.DTOs;
using CatalogoZap.Infrastructure.JWT;
using Microsoft.AspNetCore.Authorization;
using CatalogoZap.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CatalogoZap.Controllers;

[ApiController]
[Route("/api/profiles")]
public sealed class ProfilesController(
        ProfilesService profilesService
    ) : ControllerBase
{
    [HttpGet("userId")]
    public async Task<IActionResult> GetProfiles(Guid UserId)
    {
        var result = await profilesService.GetProfiles(UserId); 
        return Ok(result);
    }

    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> ModifyProfile(ModifyProfileDTO update)
    {
        var UserId = TokenService.GetUserId(User);

        await profilesService.ModifyProfile(update, UserId);

        return Ok();
    }
}
