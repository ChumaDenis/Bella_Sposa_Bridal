using BellaSposaBridal.Application.DTOs.Appointment;

namespace BellaSposaBridal.Application.Interfaces.Services;

public interface IAppointmentTypeService
{
    Task<IEnumerable<AppointmentTypeConfigDto>> GetAllAsync();
    Task<AppointmentTypeConfigDto?> GetByIdAsync(int id);
    Task<AppointmentTypeConfigDto> CreateAsync(CreateAppointmentTypeDto dto);
    Task UpdateAsync(int id, UpdateAppointmentTypeDto dto);
    Task DeleteAsync(int id);
}
