using BellaSposaBridal.Application.DTOs.Appointment;
using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Application.Interfaces.Services;
using BellaSposaBridal.Domain.Entities;

namespace BellaSposaBridal.Application.Services;

public class AppointmentTypeService : IAppointmentTypeService
{
    private readonly IAppointmentTypeRepository _repo;

    public AppointmentTypeService(IAppointmentTypeRepository repo) => _repo = repo;

    public async Task<IEnumerable<AppointmentTypeConfigDto>> GetAllAsync()
        => (await _repo.GetAllAsync()).Select(MapToDto);

    public async Task<AppointmentTypeConfigDto?> GetByIdAsync(int id)
    {
        var config = await _repo.GetByIdAsync(id);
        return config is null ? null : MapToDto(config);
    }

    public async Task<AppointmentTypeConfigDto> CreateAsync(CreateAppointmentTypeDto dto)
    {
        var nextId = await _repo.GetNextIdAsync();
        var config = new AppointmentTypeConfig
        {
            Id = nextId,
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            MainDescription = dto.MainDescription,
            Detail = dto.Detail,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive
        };
        var created = await _repo.AddAsync(config);
        return MapToDto(created);
    }

    public async Task UpdateAsync(int id, UpdateAppointmentTypeDto dto)
    {
        var config = await _repo.GetByIdAsync(id);
        if (config is null) return;
        config.Name = dto.Name;
        config.Price = dto.Price;
        config.Description = dto.Description;
        config.MainDescription = dto.MainDescription;
        config.Detail = dto.Detail;
        config.DisplayOrder = dto.DisplayOrder;
        config.IsActive = dto.IsActive;
        await _repo.UpdateAsync(config);
    }

    public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);

    private static AppointmentTypeConfigDto MapToDto(AppointmentTypeConfig c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Price = c.Price,
        Description = c.Description,
        MainDescription = c.MainDescription,
        Detail = c.Detail,
        DisplayOrder = c.DisplayOrder,
        IsActive = c.IsActive
    };
}
