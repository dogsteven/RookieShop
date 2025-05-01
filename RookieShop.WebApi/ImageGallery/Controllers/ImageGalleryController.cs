using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RookieShop.ImageGallery.Application.Commands;
using RookieShop.ImageGallery.Application.Exceptions;
using RookieShop.ImageGallery.Application.Models;
using RookieShop.ImageGallery.Application.Queries;
using RookieShop.Shared.Models;

namespace RookieShop.WebApi.ImageGallery.Controllers;

[ApiController]
[Route("/image-gallery/api")]
public class ImageGalleryController : ControllerBase
{
    private readonly ImageQueryService _imageQueryService;
    private readonly IScopedMediator _scopedMediator;
    private readonly IMemoryCache _memoryCache;

    public ImageGalleryController(ImageQueryService imageQueryService, IScopedMediator scopedMediator, IMemoryCache memoryCache)
    {
        _imageQueryService = imageQueryService;
        _scopedMediator = scopedMediator;
        _memoryCache = memoryCache;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<ImageDto>))]
    [Produces("application/json")]
    public async Task<ActionResult> GetImagesAsync(
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        return Ok(await _imageQueryService.GetImagesAsync(pageNumber ?? 1, pageSize ?? 20, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetImageByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var (stream, contentType) = await _imageQueryService.OpenStreamAsync(id, cancellationToken);

            Response.Headers.Append("Cache-Control", "public, max-age=1800");

            return File(stream, contentType);
        }
        catch (ImageNotFoundException)
        {
            const string key = "not-found-image";
            
            if (_memoryCache.TryGetValue(key, out var image))
            {
                return File((byte[])image!, "image/jpg");
            }
            
            await using var imageNotFoundFile = System.IO.File.Open("./assets/not-found.jpg", FileMode.Open);
            await using var bufferStream = new MemoryStream();
            await imageNotFoundFile.CopyToAsync(bufferStream, cancellationToken);

            var imageNotFoundBytes = bufferStream.ToArray();
            
            _memoryCache.Set(key, imageNotFoundBytes, TimeSpan.FromMinutes(10));
            
            return File(imageNotFoundBytes, "image/jpg");
        }
    }

    public class UploadImageForm
    {
        public IFormFile File { get; set; } = null!;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> UploadImageAsync(
        [FromForm] UploadImageForm form,
        CancellationToken cancellationToken)
    {
        var stream = form.File.OpenReadStream();

        await _scopedMediator.Send(new UploadImage
        {
            Id = Guid.NewGuid(),
            ContentType = form.File.ContentType,
            Stream = stream
        }, cancellationToken);

        return Created();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteImageAsync(Guid id, CancellationToken cancellationToken)
    {
        await _scopedMediator.Send(new DeleteImage { Id = id }, cancellationToken);
        
        return NoContent();
    }
}