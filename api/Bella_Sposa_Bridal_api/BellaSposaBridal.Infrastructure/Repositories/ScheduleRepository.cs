using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Domain.Entities;
using BellaSposaBridal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BellaSposaBridal.Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly AppDbContext _context;

    public ScheduleRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<TimeSlotConfig>> GetTimeSlotsAsync()
        => await _context.TimeSlotConfigs.OrderBy(t => t.DisplayOrder).AsNoTracking().ToListAsync();

    public async Task ReplaceTimeSlotsAsync(IEnumerable<string> slots)
    {
        var existing = await _context.TimeSlotConfigs.ToListAsync();
        _context.TimeSlotConfigs.RemoveRange(existing);
        int order = 0;
        foreach (var slot in slots)
        {
            _context.TimeSlotConfigs.Add(new TimeSlotConfig
            {
                Id = Guid.NewGuid(),
                Time = slot,
                IsActive = true,
                DisplayOrder = order++
            });
        }
        await _context.SaveChangesAsync();
    }

    public async Task<DaySchedule?> GetDayScheduleAsync(DateOnly date)
        => await _context.DaySchedules.AsNoTracking().FirstOrDefaultAsync(d => d.Date == date);

    public async Task SetDayScheduleAsync(DateOnly date, bool isClosed, List<string>? customSlots)
    {
        var existing = await _context.DaySchedules.FirstOrDefaultAsync(d => d.Date == date);
        if (existing is null)
        {
            _context.DaySchedules.Add(new DaySchedule
            {
                Id = Guid.NewGuid(),
                Date = date,
                IsClosed = isClosed,
                CustomSlots = customSlots ?? new List<string>()
            });
        }
        else
        {
            existing.IsClosed = isClosed;
            existing.CustomSlots = customSlots ?? new List<string>();
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDayScheduleAsync(DateOnly date)
    {
        var existing = await _context.DaySchedules.FirstOrDefaultAsync(d => d.Date == date);
        if (existing is not null)
        {
            _context.DaySchedules.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
