using BellaSposaBridal.Application.DTOs.Appointment;
using BellaSposaBridal.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bella_Sposa_Bridal_api.Controllers;

[ApiController]
[Route("api/appointment-types")]
public class AppointmentTypesController : ControllerBase
{
    private readonly IAppointmentTypeService _service;

    public AppointmentTypesController(IAppointmentTypeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentTypeConfigDto>>> GetAll()
    {
        var types = await _service.GetAllAsync();
        return Ok(types);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentTypeConfigDto>> Create([FromBody] CreateAppointmentTypeDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAll), new { }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentTypeDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
