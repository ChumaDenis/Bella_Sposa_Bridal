using BellaSposaBridal.Application.DTOs.Appointment;
using BellaSposaBridal.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bella_Sposa_Bridal_api.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll()
    {
        var appointments = await _appointmentService.GetAllAsync();
        return Ok(appointments);
    }

    [HttpGet("booked-slots")]
    public async Task<ActionResult<List<string>>> GetBookedSlots([FromQuery] DateOnly date)
    {
        var slots = await _appointmentService.GetBookedSlotsAsync(date);
        return Ok(slots);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AppointmentDto>> GetById(Guid id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment is null) return NotFound();
        return Ok(appointment);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> Create([FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var created = await _appointmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateAppointmentStatusDto dto)
    {
        await _appointmentService.UpdateStatusAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("{id:guid}/reschedule")]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleAppointmentDto dto)
    {
        try
        {
            await _appointmentService.RescheduleAsync(id, dto);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _appointmentService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("{id:guid}/admin-notes")]
    public async Task<IActionResult> UpdateAdminNotes(Guid id, [FromBody] UpdateAdminNotesDto dto)
    {
        await _appointmentService.UpdateAdminNotesAsync(id, dto);
        return NoContent();
    }

    [HttpPost("{id:guid}/files")]
    [RequestSizeLimit(150_000_000)]
    public async Task<ActionResult<AppointmentFileDto>> UploadFile(Guid id, IFormFile file, [FromServices] IStorageService storageService)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file provided" });

        await using var stream = file.OpenReadStream();
        var url = await storageService.UploadAsync(stream, file.FileName, file.ContentType, "appointment-files");
        var dto = await _appointmentService.AddFileAsync(id, file.FileName, url, file.Length, file.ContentType);
        return Ok(dto);
    }

    [HttpDelete("{id:guid}/files/{fileId:guid}")]
    public async Task<IActionResult> DeleteFile(Guid id, Guid fileId, [FromServices] IStorageService storageService)
    {
        var url = await _appointmentService.GetFileUrlAsync(id, fileId);
        if (url is null) return NotFound();

        await storageService.DeleteAsync(url);
        await _appointmentService.DeleteFileAsync(id, fileId);
        return NoContent();
    }
}
