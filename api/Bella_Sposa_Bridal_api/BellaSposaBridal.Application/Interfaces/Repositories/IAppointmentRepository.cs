using BellaSposaBridal.Domain.Entities;

namespace BellaSposaBridal.Application.Interfaces.Repositories;

public interface IAppointmentRepository
{
    Task<IEnumerable<Appointment>> GetAllAsync();
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<Appointment> AddAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task DeleteAsync(Guid id);
    Task<List<string>> GetBookedSlotsAsync(DateOnly date);
    Task<bool> IsSlotTakenAsync(DateTime appointmentDateTime);
    Task<bool> IsSlotTakenByOtherAsync(Guid excludeId, DateTime appointmentDateTime);
    Task RescheduleAsync(Guid id, DateTime newDateTime);
    Task UpdateAdminNotesAsync(Guid id, string? adminNotes);
    Task<AppointmentFile> AddFileAsync(AppointmentFile file);
    Task<AppointmentFile?> GetFileAsync(Guid fileId);
    Task DeleteFileAsync(Guid fileId);
}
