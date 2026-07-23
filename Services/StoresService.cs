using CatalogoZap.Models;
using CatalogoZap.Services;
using CatalogoZap.Repositories;
using CatalogoZap.DTOs;
using CatalogoZap.Infrastructure.CloudinaryService;
using Microsoft.AspNetCore.Http.HttpResults;
using CatalogoZap.Infrastructure.Exceptions;

namespace CatalogoZap.Services;

public sealed class StoresService(
        StoresRepository storesRepository,
        CloudinaryService cloudinaryService,
        ProductsService productsService
    )
{
    public async Task<List<StoreModel>> GetStores(Guid UserId)
    {
        return await storesRepository.SelectStores(UserId);
    }

    public async Task CreateStore(StoreDTO store, Guid UserId)
    {
        string logoUrl = await cloudinaryService.UploadImageAsync(store.Photo);

        var newStore = new StoreModel
        {
            UserId = UserId,
            Name = store.Name,
            Bio = store.Bio,
            LogoUrl = logoUrl
        };

        await storesRepository.CreateStore(newStore);
    }

    public async Task ModifyStore(ModifyStoreDTO store, Guid UserId)
    {
        var oldStore = await storesRepository.SelectStoreById(store.StoreId) ?? throw new NotFoundException("Store not found");

        string? photoUrl = store.Photo != null ? await cloudinaryService.UploadImageAsync(store.Photo) : null;

        var newStore = new StoreModel
        {
            Id = store.StoreId,
            UserId = UserId,
            Name = store.Name ?? oldStore.Name,
            Bio = store.Bio ?? oldStore.Bio,
            LogoUrl = photoUrl ?? oldStore.LogoUrl
        };

        await storesRepository.ModStore(newStore);

        if (photoUrl != null)
        {
            int startIndex = oldStore.LogoUrl.IndexOf("products/");
            if (startIndex != -1)
            {
                string fullPathWithExtension = oldStore.LogoUrl.Substring(startIndex);
                int lastDotIndex = fullPathWithExtension.LastIndexOf('.');
                string PhotoUrlPath = (lastDotIndex != -1) ? fullPathWithExtension.Substring(0, lastDotIndex) : fullPathWithExtension;
                await cloudinaryService.DeleteImageAsync(PhotoUrlPath);
            }
        }
    }

    public async Task DeleteStore(Guid UserId, Guid StoreId)
    {
        StoreModel store = await storesRepository.SelectStoreById(StoreId) ?? throw new NotFoundException("Store not found");

        await storesRepository.DeleteStore(UserId, StoreId);

        if (!string.IsNullOrWhiteSpace(store.LogoUrl))
        {
            int startIndex = store.LogoUrl.IndexOf("products/");
            if (startIndex != -1)
            {
                string fullPathWithExtension = store.LogoUrl.Substring(startIndex);
                int lastDotIndex = fullPathWithExtension.LastIndexOf('.');
                string PhotoUrlPath = (lastDotIndex != -1) ? fullPathWithExtension.Substring(0, lastDotIndex) : fullPathWithExtension;
                await cloudinaryService.DeleteImageAsync(PhotoUrlPath);
            }
        }

        var products = await productsService.GetProducts(StoreId, null);

        foreach(var product in products){
            await productsService.DeleteProduct(product.Id, product.UserId, product.StoreId);
        }
    }

}