using Microsoft.AspNetCore.Mvc;
using CatalogoZap.DTOs;
using CatalogoZap.Services;

namespace CatalogoZap.Controllers;

[ApiController]
[Route("/api/users/login")]
public sealed class LoginController(
        ProfilesService profilesService
    ) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        var result = await profilesService.Login(dto);
        return Ok(result);
    }
}
