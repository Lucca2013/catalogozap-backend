using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CatalogoZap.Infrastructure.JWT;
using CatalogoZap.Services;
using CatalogoZap.Models;
using CatalogoZap.DTOs;
using CatalogoZap.Infrastructure.Exceptions;

namespace CatalogoZap.Controllers;

[ApiController]
[Route("/api/stores")]
public sealed class StoresController(
		StoresService storesService
	) : ControllerBase
{
	[HttpGet]
    [Authorize]
	public async Task<IActionResult> GetStores()
	{
		var UserId = TokenService.GetUserId(User);

		var stores = await storesService.GetStores(UserId);

		return Ok(stores);
	}

	[HttpPost]
	[Authorize]
	public async Task<IActionResult> PostStore ([FromForm] StoreDTO newStore)
	{
		var UserId = TokenService.GetUserId(User);

		await storesService.CreateStore(newStore, UserId);

		return Ok();
	}

	[HttpPatch]
	[Authorize]
	public async Task<IActionResult> PatchStore (ModifyStoreDTO Store)
	{
		var UserId = TokenService.GetUserId(User);

		await storesService.ModifyStore(Store, UserId);

		return Ok();
	}

	[HttpDelete]
	[Authorize]
	public async Task<IActionResult> DeleteStore (Guid StoreId)
	{
		var UserId = TokenService.GetUserId(User);
		
		await storesService.DeleteStore(UserId, StoreId);

		return Ok();
	}
}
