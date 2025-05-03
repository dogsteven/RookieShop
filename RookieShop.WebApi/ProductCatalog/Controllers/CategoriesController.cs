using System.ComponentModel.DataAnnotations;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.ProductCatalog.Application.Commands;
using RookieShop.ProductCatalog.Application.Queries;
using RookieShop.ProductCatalog.ViewModels;

namespace RookieShop.WebApi.ProductCatalog.Controllers;

[ApiController]
[Route("/product-catalog/api/categories")]
[Produces("application/problem+json")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryQueryService _categoryQueryService;
    private readonly IScopedMediator _scopedMediator;

    public CategoriesController(CategoryQueryService categoryQueryService, IScopedMediator scopedMediator)
    {
        _categoryQueryService = categoryQueryService;
        _scopedMediator = scopedMediator;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
    {
        return Ok(await _categoryQueryService.GetCategoryByIdAsync(id, cancellationToken));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CategoryDto>))]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return Ok(await _categoryQueryService.GetCategoriesAsync(cancellationToken));
    }

    public class CreateCategoryBody
    {
        [Required, MinLength(1), MaxLength(100)]
        public string Name { get; set; }
        
        [Required, MinLength(1), MaxLength(250)]
        public string Description { get; set; }
        
#pragma warning disable CS8618, CS9264
        public CreateCategoryBody() {}
#pragma warning restore CS8618, CS9264
    }

    public class CreateCategoryResponse
    {
        public int Id { get; init; }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<CreateCategoryResponse>> CreateCategoryAsync([FromBody] CreateCategoryBody body,
        CancellationToken cancellationToken)
    {
        var createCategory = new CreateCategory
        {
            Name = body.Name,
            Description = body.Description,
        };

        var client = _scopedMediator.CreateRequestClient<CreateCategory>();
        
        var response = await client.GetResponse<CategoryCreatedResponse>(createCategory, cancellationToken);
        
        return Created("", new CreateCategoryResponse { Id = response.Message.Id });
    }
    
    public class UpdateCategoryBody
    {
        [Required, MinLength(1), MaxLength(100)]
        public string Name { get; set; }
        
        [Required, MinLength(1), MaxLength(250)]
        public string Description { get; set; }
        
#pragma warning disable CS8618, CS9264
        public UpdateCategoryBody() {}
#pragma warning restore CS8618, CS9264
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> UpdateCategoryAsync([FromRoute] int id, [FromBody] UpdateCategoryBody body,
        CancellationToken cancellationToken)
    {
        var updateCategory = new UpdateCategory
        {
            Id = id,
            Name = body.Name,
            Description = body.Description,
        };
        
        await _scopedMediator.Send(updateCategory, cancellationToken);
        
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteCategoryAsync(int id, CancellationToken cancellationToken)
    {
        var deleteCategory = new DeleteCategory
        {
            Id = id
        };
        
        await _scopedMediator.Send(deleteCategory, cancellationToken);
        
        return NoContent();
    }
}