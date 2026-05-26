using BellaSposaBridal.Application.DTOs.Appointment;

namespace BellaSposaBridal.Application.Interfaces.Services;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetAllAsync();
    Task<AppointmentDto?> GetByIdAsync(Guid id);
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto);
    Task UpdateStatusAsync(Guid id, UpdateAppointmentStatusDto dto);
    Task RescheduleAsync(Guid id, RescheduleAppointmentDto dto);
    Task DeleteAsync(Guid id);
    Task<List<string>> GetBookedSlotsAsync(DateOnly date);
    Task UpdateAdminNotesAsync(Guid id, UpdateAdminNotesDto dto);
    Task<AppointmentFileDto> AddFileAsync(Guid appointmentId, string fileName, string url, long size, string contentType);
    Task<string?> GetFileUrlAsync(Guid appointmentId, Guid fileId);
    Task DeleteFileAsync(Guid appointmentId, Guid fileId);
}
