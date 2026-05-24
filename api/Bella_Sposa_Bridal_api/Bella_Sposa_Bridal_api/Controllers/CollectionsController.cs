using BellaSposaBridal.Application.DTOs.Collection;
using BellaSposaBridal.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bella_Sposa_Bridal_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionsController : ControllerBase
{
    private readonly ICollectionService _collectionService;

    public CollectionsController(ICollectionService collectionService)
    {
        _collectionService = collectionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CollectionDto>>> GetAll()
    {
        var collections = await _collectionService.GetAllAsync();
        return Ok(collections);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CollectionDto>> GetById(Guid id)
    {
        var collection = await _collectionService.GetByIdAsync(id);
        if (collection is null) return NotFound();
        return Ok(collection);
    }

    [HttpPost]
    public async Task<ActionResult<CollectionDto>> Create([FromBody] CreateCollectionDto dto)
    {
        var created = await _collectionService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CollectionDto>> Update(Guid id, [FromBody] CreateCollectionDto dto)
    {
        var updated = await _collectionService.UpdateAsync(id, dto);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _collectionService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
