using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.Application.Models;
using RookieShop.Application.Services;

namespace RookieShop.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Produces("application/problem+json")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CategoryDto>))]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return Ok(await _categoryService.GetCategoriesAsync(cancellationToken));
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
        var id = await _categoryService.CreateCategoryAsync(body.Name, body.Description, cancellationToken);
        
        return Created("", new CreateCategoryResponse { Id = id });
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
        await _categoryService.UpdateCategoryAsync(id, body.Name, body.Description, cancellationToken);
        
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteCategoryAsync(int id, CancellationToken cancellationToken)
    {
        await _categoryService.DeleteCategoryAsync(id, cancellationToken);
        
        return NoContent();
    }
}