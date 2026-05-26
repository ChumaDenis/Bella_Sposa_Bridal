using BellaSposaBridal.Application.DTOs.Collection;
using BellaSposaBridal.Application.DTOs.Dress;
using BellaSposaBridal.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bella_Sposa_Bridal_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionsController : ControllerBase
{
    private readonly ICollectionService _collectionService;
    private readonly IDressService _dressService;

    public CollectionsController(ICollectionService collectionService, IDressService dressService)
    {
        _collectionService = collectionService;
        _dressService = dressService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CollectionDto>>> GetAll()
    {
        // public: only active, non-deleted
        var all = await _collectionService.GetAllAsync(includeDeleted: false);
        return Ok(all.Where(c => c.IsActive));
    }

    [HttpGet("admin")]
    public async Task<ActionResult<IEnumerable<CollectionDto>>> GetAllAdmin(
        [FromQuery] bool includeDeleted = false)
    {
        var collections = await _collectionService.GetAllAsync(includeDeleted);
        return Ok(collections);
    }

    [HttpGet("names")]
    public async Task<ActionResult<IEnumerable<CollectionNameDto>>> GetNames()
    {
        var names = await _collectionService.GetNamesAsync();
        return Ok(names);
    }

    [HttpGet("featured")]
    public async Task<ActionResult<IEnumerable<CollectionDto>>> GetFeatured()
    {
        var collections = await _collectionService.GetFeaturedAsync();
        return Ok(collections);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CollectionDto>> GetById(Guid id)
    {
        var collection = await _collectionService.GetByIdAsync(id);
        if (collection is null) return NotFound();
        return Ok(collection);
    }

    [HttpGet("by/{slug}")]
    public async Task<ActionResult<CollectionDto>> GetBySlug(string slug)
    {
        var collection = await _collectionService.GetBySlugAsync(slug);
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

    [HttpPatch("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id)
    {
        var restored = await _collectionService.RestoreAsync(id);
        if (!restored) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id:guid}/active")]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] bool isActive)
    {
        await _collectionService.ToggleActiveAsync(id, isActive);
        return NoContent();
    }

    [HttpPatch("{id:guid}/featured")]
    public async Task<IActionResult> SetFeatured(Guid id, [FromBody] SetFeaturedDto dto)
    {
        await _collectionService.SetFeaturedAsync(id, dto.IsFeatured, dto.Order);
        return NoContent();
    }

    [HttpGet("{id:guid}/nav-dresses")]
    public async Task<ActionResult<IEnumerable<NavDressItemDto>>> GetNavDresses(Guid id)
    {
        var dresses = await _dressService.GetNavDressesForCollectionAsync(id);
        return Ok(dresses);
    }

    [HttpPatch("{id:guid}/dresses/{dressId:guid}/nav-order")]
    public async Task<IActionResult> SetDressNavOrder(Guid id, Guid dressId, [FromBody] int? order)
    {
        await _dressService.SetNavOrderAsync(id, dressId, order);
        return NoContent();
    }
}
