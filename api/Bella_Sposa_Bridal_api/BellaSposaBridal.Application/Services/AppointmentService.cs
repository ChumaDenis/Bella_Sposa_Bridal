using BellaSposaBridal.Application.DTOs.Appointment;
using BellaSposaBridal.Application.DTOs.Dress;
using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Application.Interfaces.Services;
using BellaSposaBridal.Domain.Entities;
using BellaSposaBridal.Domain.Enums;

namespace BellaSposaBridal.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentService(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
    {
        var appointments = await _appointmentRepository.GetAllAsync();
        return appointments.Select(MapToDto);
    }

    public async Task<AppointmentDto?> GetByIdAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        return appointment is null ? null : MapToDto(appointment);
    }

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto)
    {
        var dressIds = dto.ViewedDressIds.Take(5).ToList();

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone,
            Email = dto.Email,
            AppointmentDateTime = dto.AppointmentDateTime,
            Type = dto.Type,
            Status = AppointmentStatus.Pending,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ViewedDresses = dressIds
                .Select((dressId, index) => new AppointmentViewedDress
                {
                    Order = index + 1,
                    DressId = dressId
                })
                .ToList()
        };

        var created = await _appointmentRepository.AddAsync(appointment);
        var full = await _appointmentRepository.GetByIdAsync(created.Id);
        return MapToDto(full!);
    }

    public async Task UpdateStatusAsync(Guid id, UpdateAppointmentStatusDto dto)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment is null) return;

        appointment.Status = dto.Status;
        appointment.UpdatedAt = DateTime.UtcNow;
        await _appointmentRepository.UpdateAsync(appointment);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _appointmentRepository.DeleteAsync(id);
    }

    private static AppointmentDto MapToDto(Appointment appointment) => new()
    {
        Id = appointment.Id,
        FirstName = appointment.FirstName,
        LastName = appointment.LastName,
        Phone = appointment.Phone,
        Email = appointment.Email,
        AppointmentDateTime = appointment.AppointmentDateTime,
        Type = appointment.Type.ToString(),
        Status = appointment.Status.ToString(),
        Notes = appointment.Notes,
        CreatedAt = appointment.CreatedAt,
        ViewedDresses = appointment.ViewedDresses
            .OrderBy(vd => vd.Order)
            .Select(vd => vd.Dress is not null ? MapDressToListDto(vd.Dress) : null)
            .ToList()
    };

    private static DressListDto MapDressToListDto(Dress dress) => new()
    {
        Id = dress.Id,
        Name = dress.Name,
        Tagline = dress.Tagline,
        Silhouette = dress.Silhouette,
        Color = dress.Color,
        HeroImageUrl = GetHeroImageUrl(dress),
        CollectionNames = dress.Collections
            .Select(dc => dc.Collection?.Name ?? string.Empty)
            .Where(n => !string.IsNullOrEmpty(n))
            .ToList()
    };

    private static string? GetHeroImageUrl(Dress dress)
    {
        var hero = dress.Photos.FirstOrDefault(p => p.Type == PhotoType.Hero);
        if (hero is not null) return hero.Url;
        return dress.Photos.FirstOrDefault(p => p.Type == PhotoType.Front)?.Url;
    }
}
