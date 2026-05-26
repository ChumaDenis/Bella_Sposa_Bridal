using BellaSposaBridal.Domain.Entities;

namespace BellaSposaBridal.Application.Interfaces.Repositories;

public interface IAppointmentTypeRepository
{
    Task<IEnumerable<AppointmentTypeConfig>> GetAllAsync();
    Task<AppointmentTypeConfig?> GetByIdAsync(int id);
    Task<AppointmentTypeConfig> AddAsync(AppointmentTypeConfig config);
    Task UpdateAsync(AppointmentTypeConfig config);
    Task DeleteAsync(int id);
    Task<int> GetNextIdAsync();
}
