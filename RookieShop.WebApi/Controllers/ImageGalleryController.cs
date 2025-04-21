using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.ImageGallery.Commands;
using RookieShop.ImageGallery.Entities;
using RookieShop.ImageGallery.Exceptions;
using RookieShop.ImageGallery.Models;
using RookieShop.ImageGallery.Queries;
using RookieShop.Shared.Models;

namespace RookieShop.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ImageGalleryController : ControllerBase
{
    private readonly ImageQueryService _imageQueryService;
    private readonly IScopedMediator _scopedMediator;

    public ImageGalleryController(ImageQueryService imageQueryService, IScopedMediator scopedMediator)
    {
        _imageQueryService = imageQueryService;
        _scopedMediator = scopedMediator;
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
        var (stream, contentType) = await _imageQueryService.GetImageByIdAsync(id, cancellationToken);
        
        Response.Headers.Append("Cache-Control", "public, max-age=1800");

        return File(stream, contentType);
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