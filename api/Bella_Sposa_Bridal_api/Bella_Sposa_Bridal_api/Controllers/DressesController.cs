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
    public async Task<ActionResult<IEnumerable<DressListDto>>> GetAll()
    {
        var dresses = await _dressService.GetAllActiveAsync();
        return Ok(dresses);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DressDetailDto>> GetById(Guid id)
    {
        var dress = await _dressService.GetByIdAsync(id);
        if (dress is null) return NotFound();
        return Ok(dress);
    }

    [HttpGet("collection/{collectionId:guid}")]
    public async Task<ActionResult<IEnumerable<DressListDto>>> GetByCollection(Guid collectionId)
    {
        var dresses = await _dressService.GetByCollectionIdAsync(collectionId);
        return Ok(dresses);
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
}
