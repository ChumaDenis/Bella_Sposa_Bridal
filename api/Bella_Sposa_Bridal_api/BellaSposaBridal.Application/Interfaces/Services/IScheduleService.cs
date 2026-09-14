using BellaSposaBridal.Application.DTOs.Appointment;

namespace BellaSposaBridal.Application.Interfaces.Services;

public interface IScheduleService
{
    Task<IEnumerable<TimeSlotDto>> GetTimeSlotsAsync();
    Task ReplaceTimeSlotsAsync(UpdateTimeSlotsDto dto);
    Task<DayScheduleDto?> GetDayScheduleAsync(DateOnly date);
    Task<List<DayScheduleDto>> GetUpcomingDaySchedulesAsync();
    Task SetDayScheduleAsync(DateOnly date, SetDayScheduleDto dto);
    Task DeleteDayScheduleAsync(DateOnly date);
    Task<List<string>> GetAvailableSlotsAsync(DateOnly date);
    Task<List<string>> GetUnavailableDatesAsync(DateOnly from, DateOnly to);
}
