using Microsoft.AspNetCore.Mvc;
using CatalogoZap.DTOs;
using CatalogoZap.Services;
using CatalogoZap.Infrastructure.Exceptions;

namespace CatalogoZap.Controllers;

[ApiController]
[Route("/api/users/register")]
public sealed class RegisterController(
        ProfilesService profilesService
    ) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        await profilesService.Register(dto);

        return Created();
    }
}
