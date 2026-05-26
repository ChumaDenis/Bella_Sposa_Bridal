using BellaSposaBridal.Application.DTOs.Appointment;
using BellaSposaBridal.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bella_Sposa_Bridal_api.Controllers;

[ApiController]
[Route("api/schedule")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _service;

    public ScheduleController(IScheduleService service)
    {
        _service = service;
    }

    [HttpGet("time-slots")]
    public async Task<ActionResult<IEnumerable<TimeSlotDto>>> GetTimeSlots()
    {
        var slots = await _service.GetTimeSlotsAsync();
        return Ok(slots);
    }

    [HttpPut("time-slots")]
    public async Task<IActionResult> ReplaceTimeSlots([FromBody] UpdateTimeSlotsDto dto)
    {
        await _service.ReplaceTimeSlotsAsync(dto);
        return NoContent();
    }

    [HttpGet("available-slots")]
    public async Task<ActionResult<List<string>>> GetAvailableSlots([FromQuery] DateOnly date)
    {
        var slots = await _service.GetAvailableSlotsAsync(date);
        return Ok(slots);
    }

    [HttpGet("day/{date}")]
    public async Task<ActionResult<DayScheduleDto>> GetDaySchedule(DateOnly date)
    {
        var schedule = await _service.GetDayScheduleAsync(date);
        if (schedule is null) return NotFound();
        return Ok(schedule);
    }

    [HttpPut("day/{date}")]
    public async Task<IActionResult> SetDaySchedule(DateOnly date, [FromBody] SetDayScheduleDto dto)
    {
        await _service.SetDayScheduleAsync(date, dto);
        return NoContent();
    }

    [HttpDelete("day/{date}")]
    public async Task<IActionResult> DeleteDaySchedule(DateOnly date)
    {
        await _service.DeleteDayScheduleAsync(date);
        return NoContent();
    }
}
