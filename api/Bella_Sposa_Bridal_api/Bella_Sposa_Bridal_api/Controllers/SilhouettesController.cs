using BellaSposaBridal.Application.DTOs.Silhouette;
using BellaSposaBridal.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bella_Sposa_Bridal_api.Controllers;

[ApiController]
[Route("api/silhouettes")]
public class SilhouettesController : ControllerBase
{
    private readonly ISilhouetteTypeService _service;

    public SilhouettesController(ISilhouetteTypeService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SilhouetteTypeDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<ActionResult<SilhouetteTypeDto>> Create([FromBody] CreateSilhouetteTypeDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAll), created);
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder([FromBody] ReorderSilhouetteTypesDto dto)
    {
        await _service.ReorderAsync(dto);
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SilhouetteTypeDto>> Update(int id, [FromBody] UpdateSilhouetteTypeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "Name is required." });
        var result = await _service.UpdateAsync(id, dto);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
