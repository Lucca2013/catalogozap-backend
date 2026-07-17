using CatalogoZap.Models;
using CatalogoZap.Services.Interfaces;
using CatalogoZap.Repositories.Interfaces;
using CatalogoZap.DTOs;
using CatalogoZap.Infrastructure.CloudinaryService;
using Microsoft.AspNetCore.Http.HttpResults;
using CatalogoZap.Infrastructure.Exceptions;

namespace CatalogoZap.Services;

public class StoresService : IStoresService
{
    private readonly IStoresRepository _storesRepository;
    private readonly IProductsService _productsService;
    private readonly ICloudinaryService _cloudinaryService;

    public StoresService(IStoresRepository storesRepository, ICloudinaryService cloudinaryService, IProductsService productsService)
    {
        _storesRepository = storesRepository;
        _productsService = productsService;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<List<StoreModel>> GetStores(Guid UserId)
    {
        return await _storesRepository.SelectStores(UserId);
    }

    public async Task CreateStore(StoreDTO store, Guid UserId)
    {
        string logoUrl = await _cloudinaryService.UploadImageAsync(store.Photo);

        var newStore = new StoreModel
        {
            UserId = UserId,
            Name = store.Name,
            Bio = store.Bio,
            LogoUrl = logoUrl
        };

        await _storesRepository.CreateStore(newStore);
    }

    public async Task ModifyStore(ModifyStoreDTO store, Guid UserId)
    {
        var oldStore = await _storesRepository.SelectStoreById(store.StoreId) ?? throw new NotFoundException("Store not found");

        string? photoUrl = store.Photo != null ? await _cloudinaryService.UploadImageAsync(store.Photo) : null;

        var newStore = new StoreModel
        {
            Id = store.StoreId,
            UserId = UserId,
            Name = store.Name ?? oldStore.Name,
            Bio = store.Bio ?? oldStore.Bio,
            LogoUrl = photoUrl ?? oldStore.LogoUrl
        };

        await _storesRepository.ModStore(newStore);

        if (photoUrl != null)
        {
            int startIndex = oldStore.LogoUrl.IndexOf("products/");
            if (startIndex != -1)
            {
                string fullPathWithExtension = oldStore.LogoUrl.Substring(startIndex);
                int lastDotIndex = fullPathWithExtension.LastIndexOf('.');
                string PhotoUrlPath = (lastDotIndex != -1) ? fullPathWithExtension.Substring(0, lastDotIndex) : fullPathWithExtension;
                await _cloudinaryService.DeleteImageAsync(PhotoUrlPath);
            }
        }
    }

    public async Task DeleteStore(Guid UserId, Guid StoreId)
    {
        StoreModel store = await _storesRepository.SelectStoreById(StoreId) ?? throw new NotFoundException("Store not found");

        await _storesRepository.DeleteStore(UserId, StoreId);

        if (!string.IsNullOrWhiteSpace(store.LogoUrl))
        {
            int startIndex = store.LogoUrl.IndexOf("products/");
            if (startIndex != -1)
            {
                string fullPathWithExtension = store.LogoUrl.Substring(startIndex);
                int lastDotIndex = fullPathWithExtension.LastIndexOf('.');
                string PhotoUrlPath = (lastDotIndex != -1) ? fullPathWithExtension.Substring(0, lastDotIndex) : fullPathWithExtension;
                await _cloudinaryService.DeleteImageAsync(PhotoUrlPath);
            }
        }

        var products = await _productsService.GetProducts(StoreId, null);

        foreach(var product in products){
            await _productsService.DeleteProduct(product.Id, product.UserId, product.StoreId);
        }
    }

}