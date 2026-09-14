using BellaSposaBridal.Domain.Entities;

namespace BellaSposaBridal.Application.Interfaces.Repositories;

public interface IScheduleRepository
{
    Task<IEnumerable<TimeSlotConfig>> GetTimeSlotsAsync();
    Task ReplaceTimeSlotsAsync(IEnumerable<string> slots);
    Task<DaySchedule?> GetDayScheduleAsync(DateOnly date);
    Task<List<DaySchedule>> GetUpcomingDaySchedulesAsync(DateOnly fromDate);
    Task SetDayScheduleAsync(DateOnly date, bool isClosed, List<string>? customSlots);
    Task DeleteDayScheduleAsync(DateOnly date);
}
