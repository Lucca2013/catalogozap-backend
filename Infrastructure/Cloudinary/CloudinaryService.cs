using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CatalogoZap.Options.Cloudinary;
using Microsoft.Extensions.Options;

namespace CatalogoZap.Infrastructure.CloudinaryService;

public sealed class CloudinaryService(IOptions<CloudinaryOptions> options)
{
    private readonly Cloudinary cloudinary = new Cloudinary(options.Value.CloudinaryKey);

    public async Task<string> UploadImageAsync(IFormFile image)
    {
        using var stream = image.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(Guid.NewGuid().ToString(), stream),
            Folder = "products"
        };

        var result = await cloudinary.UploadAsync(uploadParams);
        return result.SecureUrl.ToString();
    }

    public async Task<DeletionResult> DeleteImageAsync(string path)
    {
        var deletionParams = new DeletionParams(path)
        {
            ResourceType = ResourceType.Image
        };

        var result = await cloudinary.DestroyAsync(deletionParams);

        return result;

    }

}
