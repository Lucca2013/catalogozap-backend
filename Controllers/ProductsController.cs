using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CatalogoZap.Infrastructure.JWT;
using CatalogoZap.DTOs;
using CatalogoZap.Services;
using CatalogoZap.Infrastructure.Exceptions;

namespace CatalogoZap.Controllers;

[ApiController]
[Route("/api/products")]
public sealed class ProductsController(
		ProductsService productsService
	) : ControllerBase
{
	[HttpPost]
	[Authorize]
	public async Task<IActionResult> PostProduct([FromForm] ProductDTO dto)
	{
        Guid userId = TokenService.GetUserId(User);

        await productsService.CreateProduct(dto, userId);

		return Ok();
	}

	[HttpGet("{storeId}")]
	public async Task<IActionResult> GetProduct(Guid storeId)
	{
		//It will be null if it is not admin acess
		var UserId = TokenService.TryGetUserId(User);

		var products = await productsService.GetProducts(storeId, UserId);

		return Ok(products);
	}

	[HttpPatch]
	[Authorize]
	public async Task<IActionResult> ModifyProduct([FromForm] ModProductsDTO Product)
	{
		var UserId = TokenService.GetUserId(User);

		await productsService.ModifyProducts(Product, UserId);

		return Ok();
	}

	[HttpDelete]
	[Authorize]
	public async Task<IActionResult> DeleteProduct(Guid Id, Guid StoreId)
	{
		var UserId = TokenService.GetUserId(User);

		await productsService.DeleteProduct(Id, UserId, StoreId);

		return Ok();
	}
}
