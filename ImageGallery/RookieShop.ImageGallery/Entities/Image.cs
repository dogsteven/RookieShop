namespace RookieShop.ImageGallery.Entities;

public class Image
{
    public readonly Guid Id;

    public readonly string ContentType;

    public readonly string TempFileName;

    public readonly DateTime CreatedDate;
    
    public bool IsUploaded { get; private set; }
    
#pragma warning disable CS8618, CS9264
    public Image() {}
#pragma warning restore CS8618, CS9264

    public Image(Guid id, string contentType, string tempFileName)
    {
        Id = id;
        ContentType = contentType;
        TempFileName = tempFileName;
        CreatedDate = DateTime.UtcNow;
        IsUploaded = false;
    }

    public void MarkAsUploaded()
    {
        IsUploaded = true;
    }
}