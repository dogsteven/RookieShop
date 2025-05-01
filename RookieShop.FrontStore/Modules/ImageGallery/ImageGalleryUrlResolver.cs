namespace RookieShop.FrontStore.Modules.ImageGallery;

public class ImageGalleryUrlResolver
{
    private readonly string _baseAddress;

    public ImageGalleryUrlResolver(IConfiguration configuration)
    {
        _baseAddress = configuration["RookieShop:WebApi:Address"]!;
    }
    
    public string ResolveImageUrl(Guid id)
    {
        return $"{_baseAddress}/image-gallery/api/{id}";
    }
}