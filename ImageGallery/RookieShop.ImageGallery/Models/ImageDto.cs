using RookieShop.ImageGallery.Entities;

namespace RookieShop.ImageGallery.Models;

public class ImageDto
{
    public Guid Id { get; init; }

    public string ContentType { get; init; }

    public string TempFileName { get; init; }

    public DateTime CreatedDate { get; init; }
    
    public bool IsSynced { get; init; }
    
#pragma warning disable CS8618, CS9264
    public ImageDto() {}
#pragma warning restore CS8618, CS9264

    internal ImageDto(Image image)
    {
        Id = image.Id;
        ContentType = image.ContentType;
        TempFileName = image.TempFileName;
        CreatedDate = image.CreatedDate;
        IsSynced = image.IsSynced;
    }
}