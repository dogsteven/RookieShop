namespace RookieShop.ImageGallery.ViewModels;

public class ImageDto
{
    public Guid Id { get; init; }

    public string ContentType { get; init; }

    public string TemporaryEntryId { get; init; }

    public DateTime CreatedDate { get; init; }
    
    public bool IsSynced { get; init; }
    
#pragma warning disable CS8618, CS9264
    public ImageDto() {}
#pragma warning restore CS8618, CS9264
}