namespace RookieShop.ImageGallery.Exceptions;

public class ImageNotFoundException : Exception
{
    public ImageNotFoundException(Guid id) : base($"Image with id {id} was not found.") {}
}