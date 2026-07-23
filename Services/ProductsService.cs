using CatalogoZap.Services;
using CatalogoZap.DTOs;
using CatalogoZap.Infrastructure.CloudinaryService;
using CatalogoZap.Repositories;
using CatalogoZap.Models;
using CatalogoZap.Infrastructure.Exceptions;

namespace CatalogoZap.Services;

public sealed class ProductsService(
        CloudinaryService cloudinaryService, 
        ProfilesService profilesService, 
        ProductsRepository productsRepository
    )
{
    public async Task CreateProduct(ProductDTO dto, Guid userId)
    {
        if (await profilesService.HasReachedFreeTierLimit(userId))
            throw new UnauthorizedException("Reached free plan products limit.");

        string photoUrl = await cloudinaryService.UploadImageAsync(dto.Photo);

        var data = new ProductModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PhotoUrl = photoUrl,
            Name = dto.Name,
            PriceCents = dto.PriceCents,
            StoreId = dto.StoreId,
            Avaliable = dto.Avaliable,
            Created_at = ""
        };

        await productsRepository.CreateProduct(data);
    }

    public async Task<List<ProductModel>> GetProducts(Guid storeId, Guid? UserId)
    {
        if (UserId == null)
        {
            return await productsRepository.GetProducts(storeId);
        }
        else
        {
            return await productsRepository.GetProductsAdmin(storeId, UserId);
        }
    }

    public async Task ModifyProducts(ModProductsDTO product, Guid UserId)
    {
        var oldProduct = await productsRepository.GetProductById(product.Id, product.StoreId, UserId) 
            ?? throw new NotFoundException("Product doesnt exists");

        string? photoUrl = product.Photo != null ? await cloudinaryService.UploadImageAsync(product.Photo) : null;

        var newproduct = new ProductModel
        {
            Id = product.Id,
            UserId = UserId,
            StoreId = product.StoreId,
            Name = product.Name ?? oldProduct.Name,
            PriceCents = product.PriceCents ?? oldProduct.PriceCents,
            PhotoUrl = photoUrl ?? oldProduct.PhotoUrl,
            Avaliable = product.Avaliable ?? oldProduct.Avaliable,
            Created_at = oldProduct.Created_at
        };

        await productsRepository.ModifyProducts(newproduct);

        if (photoUrl != null)
        {
            int startIndex = oldProduct.PhotoUrl.IndexOf("products/");
            if (startIndex != -1)
            {
                string fullPathWithExtension = oldProduct.PhotoUrl.Substring(startIndex);
                int lastDotIndex = fullPathWithExtension.LastIndexOf('.');
                string PhotoUrlPath = (lastDotIndex != -1) ? fullPathWithExtension.Substring(0, lastDotIndex) : fullPathWithExtension;
                await cloudinaryService.DeleteImageAsync(PhotoUrlPath);
            }
        }
    }

    public async Task DeleteProduct(Guid Id, Guid UserId, Guid StoreId)
    {
        ProductModel product = await productsRepository.GetProductById(Id, StoreId, UserId) 
            ?? throw new NotFoundException("Product not found");

        System.Console.WriteLine(product.Name);

        await productsRepository.DeleteProduct(Id, StoreId, UserId);

        if (!string.IsNullOrWhiteSpace(product.PhotoUrl))
        {
            int startIndex = product.PhotoUrl.IndexOf("products/");
            if (startIndex != -1)
            {
                string fullPathWithExtension = product.PhotoUrl.Substring(startIndex);
                int lastDotIndex = fullPathWithExtension.LastIndexOf('.');
                string PhotoUrlPath = (lastDotIndex != -1) ? fullPathWithExtension.Substring(0, lastDotIndex) : fullPathWithExtension;
                await cloudinaryService.DeleteImageAsync(PhotoUrlPath);
            }
        }
    }
}