using BellaSposaBridal.Application.DTOs.Appointment;

namespace BellaSposaBridal.Application.Interfaces.Services;

public interface IScheduleService
{
    Task<IEnumerable<TimeSlotDto>> GetTimeSlotsAsync();
    Task ReplaceTimeSlotsAsync(UpdateTimeSlotsDto dto);
    Task<DayScheduleDto?> GetDayScheduleAsync(DateOnly date);
    Task SetDayScheduleAsync(DateOnly date, SetDayScheduleDto dto);
    Task DeleteDayScheduleAsync(DateOnly date);
    Task<List<string>> GetAvailableSlotsAsync(DateOnly date);
}
