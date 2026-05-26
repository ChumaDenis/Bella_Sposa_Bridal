using BellaSposaBridal.Application.DTOs.Appointment;
using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Application.Interfaces.Services;

namespace BellaSposaBridal.Application.Services;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _repo;
    private readonly IAppointmentRepository _appointmentRepo;

    public ScheduleService(IScheduleRepository repo, IAppointmentRepository appointmentRepo)
    {
        _repo = repo;
        _appointmentRepo = appointmentRepo;
    }

    public async Task<IEnumerable<TimeSlotDto>> GetTimeSlotsAsync()
        => (await _repo.GetTimeSlotsAsync()).Select(t => new TimeSlotDto
        {
            Id = t.Id,
            Time = t.Time,
            IsActive = t.IsActive,
            DisplayOrder = t.DisplayOrder
        });

    public async Task ReplaceTimeSlotsAsync(UpdateTimeSlotsDto dto)
        => await _repo.ReplaceTimeSlotsAsync(dto.Slots);

    public async Task<DayScheduleDto?> GetDayScheduleAsync(DateOnly date)
    {
        var ds = await _repo.GetDayScheduleAsync(date);
        if (ds is null) return null;
        return new DayScheduleDto
        {
            Date = ds.Date,
            IsClosed = ds.IsClosed,
            CustomSlots = ds.CustomSlots.Count > 0 ? ds.CustomSlots : null
        };
    }

    public async Task SetDayScheduleAsync(DateOnly date, SetDayScheduleDto dto)
        => await _repo.SetDayScheduleAsync(date, dto.IsClosed, dto.CustomSlots);

    public async Task DeleteDayScheduleAsync(DateOnly date)
        => await _repo.DeleteDayScheduleAsync(date);

    public async Task<List<string>> GetAvailableSlotsAsync(DateOnly date)
    {
        var daySchedule = await _repo.GetDayScheduleAsync(date);
        if (daySchedule?.IsClosed == true) return new List<string>();

        List<string> allSlots;
        if (daySchedule?.CustomSlots is { Count: > 0 })
        {
            allSlots = daySchedule.CustomSlots;
        }
        else
        {
            var global = await _repo.GetTimeSlotsAsync();
            allSlots = global.Where(t => t.IsActive).Select(t => t.Time).ToList();
        }

        var booked = await _appointmentRepo.GetBookedSlotsAsync(date);
        return allSlots.Where(s => !booked.Contains(s)).ToList();
    }
}
