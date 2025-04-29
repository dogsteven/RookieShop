namespace RookieShop.ImageGallery.Application.Exceptions;

public class ImageNotFoundException : Exception
{
    public readonly Guid Id;

    public ImageNotFoundException(Guid id) : base($"Image with id {id} was not found.")
    {
        Id = id;
    }
}