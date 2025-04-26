namespace RookieShop.ImageGallery.Application.Entities;

public class Image
{
    public readonly Guid Id;

    public readonly string ContentType;

    public readonly string TemporaryEntryId;

    public readonly DateTime CreatedDate;
    
    public bool IsSynced { get; private set; }
    
#pragma warning disable CS8618, CS9264
    public Image() {}
#pragma warning restore CS8618, CS9264

    public Image(Guid id, string contentType, string temporaryEntryId)
    {
        Id = id;
        ContentType = contentType;
        TemporaryEntryId = temporaryEntryId;
        CreatedDate = DateTime.UtcNow;
        IsSynced = false;
    }

    public void MarkAsSynced()
    {
        IsSynced = true;
    }
}