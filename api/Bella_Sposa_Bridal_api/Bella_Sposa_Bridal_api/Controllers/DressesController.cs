using BellaSposaBridal.Application.DTOs;
using BellaSposaBridal.Application.DTOs.Dress;
using BellaSposaBridal.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bella_Sposa_Bridal_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DressesController : ControllerBase
{
    private readonly IDressService _dressService;

    public DressesController(IDressService dressService)
    {
        _dressService = dressService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<DressListDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] Guid? collectionId = null,
        [FromQuery] int? silhouette = null,
        [FromQuery] string? size = null)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var result = await _dressService.GetAllActivePagedAsync(page, pageSize, collectionId, silhouette, size);
        return Ok(result);
    }

    [HttpGet("meta")]
    public async Task<ActionResult<DressFilterMeta>> GetFilterMeta()
    {
        var meta = await _dressService.GetFilterMetaAsync();
        return Ok(meta);
    }

    [HttpGet("admin")]
    public async Task<ActionResult<IEnumerable<DressListDto>>> GetAllAdmin(
        [FromQuery] bool includeDeleted = false)
    {
        var dresses = await _dressService.GetAllAsync(includeDeleted);
        return Ok(dresses);
    }

    [HttpPatch("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id)
    {
        var restored = await _dressService.RestoreAsync(id);
        if (!restored) return NotFound();
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DressDetailDto>> GetById(Guid id)
    {
        var dress = await _dressService.GetByIdAsync(id);
        if (dress is null) return NotFound();
        return Ok(dress);
    }

    [HttpGet("by/{slug}")]
    public async Task<ActionResult<DressDetailDto>> GetBySlug(string slug)
    {
        var dress = await _dressService.GetBySlugAsync(slug);
        if (dress is null) return NotFound();
        return Ok(dress);
    }

    [HttpGet("collection/{collectionId:guid}")]
    public async Task<ActionResult<PagedResult<DressListDto>>> GetByCollection(
        Guid collectionId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var result = await _dressService.GetByCollectionIdPagedAsync(collectionId, page, pageSize);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<DressDetailDto>> Create([FromBody] CreateDressDto dto)
    {
        var created = await _dressService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DressDetailDto>> Update(Guid id, [FromBody] CreateDressDto dto)
    {
        var updated = await _dressService.UpdateAsync(id, dto);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _dressService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("homepage")]
    public async Task<ActionResult<IEnumerable<DressListDto>>> GetHomepageFeatured()
    {
        var dresses = await _dressService.GetHomepageFeaturedAsync();
        return Ok(dresses);
    }

    [HttpPatch("{id:guid}/active")]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] bool isActive)
    {
        await _dressService.ToggleActiveAsync(id, isActive);
        return NoContent();
    }

    [HttpPatch("{id:guid}/homepage-featured")]
    public async Task<IActionResult> SetHomepageFeatured(Guid id, [FromBody] SetHomepageFeaturedDto dto)
    {
        await _dressService.SetHomepageFeaturedAsync(id, dto.IsFeatured, dto.Order);
        return NoContent();
    }

    [HttpPost("{id:guid}/photos")]
    public async Task<ActionResult<DressPhotoDto>> AddPhoto(Guid id, [FromBody] AddDressPhotoDto dto)
    {
        var photo = await _dressService.AddPhotoAsync(id, dto);
        if (photo is null) return NotFound();
        return Ok(photo);
    }

    [HttpDelete("{id:guid}/photos/{photoId:guid}")]
    public async Task<IActionResult> DeletePhoto(Guid id, Guid photoId)
    {
        var deleted = await _dressService.DeletePhotoAsync(id, photoId);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id:guid}/photos/reorder")]
    public async Task<IActionResult> ReorderPhotos(Guid id, [FromBody] List<Guid> orderedIds)
    {
        await _dressService.ReorderPhotosAsync(id, orderedIds);
        return NoContent();
    }

    [HttpPost("{id:guid}/videos")]
    public async Task<ActionResult<DressVideoDto>> AddVideo(Guid id, [FromBody] AddDressVideoDto dto)
    {
        var video = await _dressService.AddVideoAsync(id, dto);
        if (video is null) return NotFound();
        return Ok(video);
    }

    [HttpDelete("{id:guid}/videos/{videoId:guid}")]
    public async Task<IActionResult> DeleteVideo(Guid id, Guid videoId)
    {
        var deleted = await _dressService.DeleteVideoAsync(id, videoId);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
