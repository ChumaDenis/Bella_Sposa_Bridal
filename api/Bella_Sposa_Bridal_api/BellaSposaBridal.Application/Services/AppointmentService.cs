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
    private readonly IAppointmentTypeRepository _typeRepo;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IAppointmentTypeRepository typeRepo)
    {
        _appointmentRepository = appointmentRepository;
        _typeRepo = typeRepo;
    }

    public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
    {
        var appointments = await _appointmentRepository.GetAllAsync();
        var types = (await _typeRepo.GetAllAsync()).ToDictionary(t => t.Id, t => t.Name);
        return appointments.Select(a => MapToDto(a, types));
    }

    public async Task<AppointmentDto?> GetByIdAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment is null) return null;
        var types = (await _typeRepo.GetAllAsync()).ToDictionary(t => t.Id, t => t.Name);
        return MapToDto(appointment, types);
    }

    public async Task<List<string>> GetBookedSlotsAsync(DateOnly date)
        => await _appointmentRepository.GetBookedSlotsAsync(date);

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto)
    {
        var isCallback = dto.AppointmentDateTime.Year >= 2099;
        if (!isCallback && await _appointmentRepository.IsSlotTakenAsync(dto.AppointmentDateTime))
            throw new InvalidOperationException("This time slot is already booked.");

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
        var types = (await _typeRepo.GetAllAsync()).ToDictionary(t => t.Id, t => t.Name);
        return MapToDto(full!, types);
    }

    public async Task UpdateStatusAsync(Guid id, UpdateAppointmentStatusDto dto)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment is null) return;

        appointment.Status = dto.Status;
        appointment.UpdatedAt = DateTime.UtcNow;
        await _appointmentRepository.UpdateAsync(appointment);
    }

    public async Task RescheduleAsync(Guid id, RescheduleAppointmentDto dto)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment is null) return;

        var isCallback = dto.AppointmentDateTime.Year >= 2099;
        if (!isCallback && !dto.Force)
        {
            var isTaken = await _appointmentRepository.IsSlotTakenByOtherAsync(id, dto.AppointmentDateTime);
            if (isTaken)
                throw new InvalidOperationException("This time slot is already booked by another appointment.");
        }

        await _appointmentRepository.RescheduleAsync(id, dto.AppointmentDateTime);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _appointmentRepository.DeleteAsync(id);
    }

    public async Task UpdateAdminNotesAsync(Guid id, UpdateAdminNotesDto dto)
    {
        await _appointmentRepository.UpdateAdminNotesAsync(id, dto.AdminNotes);
    }

    public async Task<AppointmentFileDto> AddFileAsync(Guid appointmentId, string fileName, string url, long size, string contentType)
    {
        var file = new Domain.Entities.AppointmentFile
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointmentId,
            FileName = fileName,
            Url = url,
            Size = size,
            ContentType = contentType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var saved = await _appointmentRepository.AddFileAsync(file);
        return MapFileToDto(saved);
    }

    public async Task<string?> GetFileUrlAsync(Guid appointmentId, Guid fileId)
    {
        var file = await _appointmentRepository.GetFileAsync(fileId);
        if (file is null || file.AppointmentId != appointmentId) return null;
        return file.Url;
    }

    public async Task DeleteFileAsync(Guid appointmentId, Guid fileId)
    {
        var file = await _appointmentRepository.GetFileAsync(fileId);
        if (file is null || file.AppointmentId != appointmentId) return;
        await _appointmentRepository.DeleteFileAsync(fileId);
    }

    private static AppointmentDto MapToDto(Appointment appointment, Dictionary<int, string> typeNames) => new()
    {
        Id = appointment.Id,
        FirstName = appointment.FirstName,
        LastName = appointment.LastName,
        Phone = appointment.Phone,
        Email = appointment.Email,
        AppointmentDateTime = appointment.AppointmentDateTime,
        TypeId = appointment.Type,
        Type = typeNames.TryGetValue(appointment.Type, out var n) ? n : $"Type {appointment.Type}",
        Status = appointment.Status.ToString(),
        Notes = appointment.Notes,
        AdminNotes = appointment.AdminNotes,
        CreatedAt = appointment.CreatedAt,
        ViewedDresses = appointment.ViewedDresses
            .OrderBy(vd => vd.Order)
            .Select(vd => vd.Dress is not null ? MapDressToListDto(vd.Dress) : null)
            .ToList(),
        Files = appointment.Files
            .OrderBy(f => f.CreatedAt)
            .Select(MapFileToDto)
            .ToList()
    };

    private static AppointmentFileDto MapFileToDto(Domain.Entities.AppointmentFile f) => new()
    {
        Id = f.Id,
        FileName = f.FileName,
        Url = f.Url,
        Size = f.Size,
        ContentType = f.ContentType,
        UploadedAt = f.CreatedAt
    };

    private static DressListDto MapDressToListDto(Dress dress) => new()
    {
        Id = dress.Id,
        Name = dress.Name,
        Tagline = dress.Tagline,
        Silhouette = dress.SilhouetteId,
        SilhouetteName = dress.SilhouetteType?.Name ?? string.Empty,
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
