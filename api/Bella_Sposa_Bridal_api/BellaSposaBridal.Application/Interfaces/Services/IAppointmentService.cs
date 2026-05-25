using BellaSposaBridal.Application.DTOs.Appointment;

namespace BellaSposaBridal.Application.Interfaces.Services;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetAllAsync();
    Task<AppointmentDto?> GetByIdAsync(Guid id);
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto);
    Task UpdateStatusAsync(Guid id, UpdateAppointmentStatusDto dto);
    Task DeleteAsync(Guid id);
    Task<List<string>> GetBookedSlotsAsync(DateOnly date);
}
